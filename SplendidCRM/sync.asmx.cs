/**********************************************************************************************************************
 * SplendidCRM is a Customer Relationship Management program created by SplendidCRM Software, Inc. 
 * Copyright (C) 2005-2023 SplendidCRM Software, Inc. All rights reserved.
 *
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License 
 * Agreement, or other written agreement between you and SplendidCRM ("License"). By installing or 
 * using this file, you have unconditionally agreed to the terms and conditions of the License, 
 * including but not limited to restrictions on the number of users therein, and you may not use this 
 * file except in compliance with the License. 
 * 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and 
 * trademarks, in and to the contents of this file.  You will not link to or in any way combine the 
 * contents of this file or any derivatives with any Open Source Code in any manner that would require 
 * the contents of this file to be made available to any third party. 
 * 
 * IN NO EVENT SHALL SPLENDIDCRM BE RESPONSIBLE FOR ANY DAMAGES OF ANY KIND, INCLUDING ANY DIRECT, 
 * SPECIAL, PUNITIVE, INDIRECT, INCIDENTAL OR CONSEQUENTIAL DAMAGES.  Other limitations of liability 
 * and disclaimers set forth in the License. 
 * 
 *********************************************************************************************************************/
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Web;
using System.Web.SessionState;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.Caching;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for sync
	/// </summary>
	[WebService(Namespace = "http://splendidcrm.com/sync", Name="SplendidSync", Description="SplendidCRM database synchronization service.")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	public class sync : System.Web.Services.WebService
	{
		#region Scalar functions
		[WebMethod(EnableSession=true)]
		public string Version()
		{
			return Sql.ToString(Application["SplendidVersion"]);
		}

		[WebMethod(EnableSession=true)]
		public string Edition()
		{
			return Sql.ToString(Application["CONFIG.service_level"]);
		}

		[WebMethod(EnableSession=true)]
		public DateTime UtcTime()
		{
			return DateTime.UtcNow;
		}

		[WebMethod(EnableSession=true)]
		public bool IsAuthenticated()
		{
			return Security.IsAuthenticated();
		}
		#endregion

		// 11/06/2009 Paul.  The login function returns the User ID. 
		// 11/08/2009 Paul.  Include version to allow a chance to reject based on incompatible version. 
		[WebMethod(EnableSession=true)]
		public Guid Login(string UserName, string Password, string Version)
		{
			HttpSessionState     Session     = HttpContext.Current.Session    ;
			HttpRequest          Request     = HttpContext.Current.Request    ;

			// 11/05/2018 Paul.  Protect against null inputs. 
			string sUSER_NAME   = Sql.ToString(UserName);
			string sPASSWORD    = Sql.ToString(Password);
			string sVERSION     = Sql.ToString(Version );
			Guid gUSER_ID       = Guid.Empty;
			Guid gUSER_LOGIN_ID = Guid.Empty;
			
			// 02/23/2011 Paul.  SYNC service should check for lockout. 
			if ( SplendidInit.LoginFailures(Application, sUSER_NAME) >= Crm.Password.LoginLockoutCount(Application) )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("Users.ERR_USER_LOCKED_OUT")));
			}
			// 04/16/2013 Paul.  Allow system to be restricted by IP Address. 
			if ( SplendidInit.InvalidIPAddress(Application, Request.UserHostAddress) )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("Users.ERR_INVALID_IP_ADDRESS")));
			}
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				sSQL = "select ID                    " + ControlChars.CrLf
				     + "     , USER_NAME             " + ControlChars.CrLf
				     + "     , FULL_NAME             " + ControlChars.CrLf
				     + "     , IS_ADMIN              " + ControlChars.CrLf
				     + "     , STATUS                " + ControlChars.CrLf
				     + "     , PORTAL_ONLY           " + ControlChars.CrLf
				     + "     , TEAM_ID               " + ControlChars.CrLf
				     + "     , TEAM_NAME             " + ControlChars.CrLf
				     + "  from vwUSERS_Login         " + ControlChars.CrLf
				     + " where USER_NAME = @USER_NAME" + ControlChars.CrLf
				     + "   and USER_HASH = @USER_HASH" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					string sUSER_HASH = Security.HashPassword(sPASSWORD);
					// 12/25/2009 Paul.  Use lowercase username to match the primary authentication function. 
					Sql.AddParameter(cmd, "@USER_NAME", sUSER_NAME.ToLower());
					Sql.AddParameter(cmd, "@USER_HASH", sUSER_HASH);
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						using ( DataTable dt = new DataTable() )
						{
							da.Fill(dt);
							if ( dt.Rows.Count > 0 )
							{
								DataRow row = dt.Rows[0];
								Security.USER_ID     = Sql.ToGuid   (row["ID"         ]);
								Security.USER_NAME   = Sql.ToString (row["USER_NAME"  ]);
								Security.FULL_NAME   = Sql.ToString (row["FULL_NAME"  ]);
								Security.IS_ADMIN    = Sql.ToBoolean(row["IS_ADMIN"   ]);
								Security.PORTAL_ONLY = Sql.ToBoolean(row["PORTAL_ONLY"]);
								Security.TEAM_ID     = Sql.ToGuid   (row["TEAM_ID"    ]);
								Security.TEAM_NAME   = Sql.ToString (row["TEAM_NAME"  ]);
								gUSER_ID = Sql.ToGuid(row["ID"]);

								SplendidInit.LoadUserPreferences(gUSER_ID, String.Empty, String.Empty);
								SplendidInit.LoadUserACL(gUSER_ID);
								
								SqlProcs.spUSERS_LOGINS_InsertOnly(ref gUSER_LOGIN_ID, gUSER_ID, sUSER_NAME, "Anonymous", "Succeeded", Session.SessionID, Request.UserHostName, Request.Url.Host, Request.Path, Request.AppRelativeCurrentExecutionFilePath, Request.UserAgent);
								Security.USER_LOGIN_ID = gUSER_LOGIN_ID;
								// 02/20/2011 Paul.  Log the success so that we can lockout the user. 
								SplendidInit.LoginTracking(Application, sUSER_NAME, true);
								SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "SyncUser login for " + sUSER_NAME);
							}
							else
							{
								SqlProcs.spUSERS_LOGINS_InsertOnly(ref gUSER_LOGIN_ID, Guid.Empty, sUSER_NAME, "Anonymous", "Failed", Session.SessionID, Request.UserHostName, Request.Url.Host, Request.Path, Request.AppRelativeCurrentExecutionFilePath, Request.UserAgent);
								// 02/20/2011 Paul.  Log the failure so that we can lockout the user. 
								SplendidInit.LoginTracking(Application, sUSER_NAME, false);
								SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "SECURITY: failed attempted login for " + sUSER_NAME + " using Sync api");
							}
						}
					}
				}
			}
			if ( gUSER_ID == Guid.Empty )
			{
				SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "Invalid username and/or password for " + sUSER_NAME);
				throw(new Exception("Invalid username and/or password for " + sUSER_NAME));
			}
			return gUSER_ID;
		}

		[WebMethod(EnableSession=true)]
		public DataTable GetSystemTable(string TableName, DateTime DateModified, string RequestedModules)
		{
			return GetTable(TableName, DateModified, RequestedModules, -1, null);
		}

		// 11/13/2009 Paul.  We need to be able to get a specific list if items that may be in conflict. 
		[WebMethod(EnableSession=true)]
		public DataTable GetModuleTable(string TableName, DateTime DateModified, int MaxRecords, Guid[] Items)
		{
			return GetTable(TableName, DateModified, String.Empty, MaxRecords, Items);
		}

		private DataTable GetTable(string sTABLE_NAME, DateTime dtDATE_MODIFIED_UTC, string sREQUESTED_MODULES, int nMAX_RECORDS, Guid[] arrITEMS)
		{
			DataTable dt = null;
			try
			{
				if ( Security.IsAuthenticated() )
				{
					Regex r = new Regex(@"[^A-Za-z0-9_]");
					sTABLE_NAME = r.Replace(sTABLE_NAME, "");
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						// 06/03/2011 Paul.  Cache the Sync Table data. 
						using ( DataTable dtSYNC_TABLES = SplendidCache.SyncTables(sTABLE_NAME, false) )
						{
							string sSQL = String.Empty;
							if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count > 0 )
							{
								DataRow rowSYNC_TABLE = dtSYNC_TABLES.Rows[0];
								string sMODULE_NAME         = Sql.ToString (rowSYNC_TABLE["MODULE_NAME"        ]);
								string sVIEW_NAME           = Sql.ToString (rowSYNC_TABLE["VIEW_NAME"          ]);
								bool   bHAS_CUSTOM          = Sql.ToBoolean(rowSYNC_TABLE["HAS_CUSTOM"         ]);
								int    nMODULE_SPECIFIC     = Sql.ToInteger(rowSYNC_TABLE["MODULE_SPECIFIC"    ]);
								string sMODULE_FIELD_NAME   = Sql.ToString (rowSYNC_TABLE["MODULE_FIELD_NAME"  ]);
								bool   bIS_RELATIONSHIP     = Sql.ToBoolean(rowSYNC_TABLE["IS_RELATIONSHIP"    ]);
								string sMODULE_NAME_RELATED = Sql.ToString (rowSYNC_TABLE["MODULE_NAME_RELATED"]);
								string sASSIGNED_FIELD_NAME = Sql.ToString (rowSYNC_TABLE["ASSIGNED_FIELD_NAME"]);
								// 11/01/2009 Paul.  Protect against SQL Injection. A table name will never have a space character.
								sTABLE_NAME                 = Sql.ToString (rowSYNC_TABLE["TABLE_NAME"         ]);
								sTABLE_NAME        = r.Replace(sTABLE_NAME       , "");
								sVIEW_NAME         = r.Replace(sVIEW_NAME        , "");
								sMODULE_FIELD_NAME = r.Replace(sMODULE_FIELD_NAME, "");
								// 08/02/2019 Paul.  The React Client will need access to views that require a filter, like CAMPAIGN_ID. 
								if ( dtSYNC_TABLES.Columns.Contains("REQUIRED_FIELDS") )
								{
									string sREQUIRED_FIELDS = Sql.ToString (rowSYNC_TABLE["REQUIRED_FIELDS"]);
									if ( !Sql.IsEmptyString(sREQUIRED_FIELDS) )
									{
										throw(new Exception("Missing required fields: " + sREQUIRED_FIELDS));
									}
								}
								
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									// 11/04/2009 Paul.  We cannot select all fields as the filter will join to the teams table. 
									// We need to make sure only to return the fields from the base table and/or the custom field table. 
									if ( bHAS_CUSTOM )
									{
										sSQL = "select " + sVIEW_NAME  + ".*     " + ControlChars.CrLf
										     + "     , " + sTABLE_NAME + "_CSTM.*" + ControlChars.CrLf
										     + "  from " + sVIEW_NAME              + ControlChars.CrLf
										     + "  left outer join " + sTABLE_NAME + "_CSTM" + ControlChars.CrLf
										     + "               on " + sTABLE_NAME + "_CSTM.ID_C = " + sVIEW_NAME + ".ID" + ControlChars.CrLf;
									}
									else
									{
										// 01/27/2015 Paul.  Need to reduce length of vwDETAILVIEWS_RELATIONSHIPS_Sync to support Oracle. 
										sSQL = "select " + Sql.MetadataName(cmd, sVIEW_NAME) + ".*" + ControlChars.CrLf
										     + "  from " + Sql.MetadataName(cmd, sVIEW_NAME)        + ControlChars.CrLf;
									}
									cmd.CommandText = sSQL;
									cmd.CommandTimeout = 0;
									// 10/27/2009 Paul.  Apply the standard filters. 
									// 11/03/2009 Paul.  Relationship tables will not have Team or Assigned fields. 
									if ( bIS_RELATIONSHIP )
									{
										cmd.CommandText += " where 1 = 1" + ControlChars.CrLf;
										// 11/06/2009 Paul.  Use the relationship table to get the module information. 
										DataView vwRelationships = new DataView(SplendidCache.ReportingRelationships(Context.Application));
										vwRelationships.RowFilter = "(JOIN_TABLE = '" + sTABLE_NAME + "' and RELATIONSHIP_TYPE = 'many-to-many') or (RHS_TABLE = '" + sTABLE_NAME + "' and RELATIONSHIP_TYPE = 'one-to-many')";
										if ( vwRelationships.Count > 0 )
										{
											foreach ( DataRowView rowRelationship in vwRelationships )
											{
												string sJOIN_KEY_LHS             = Sql.ToString(rowRelationship["JOIN_KEY_LHS"            ]).ToUpper();
												string sJOIN_KEY_RHS             = Sql.ToString(rowRelationship["JOIN_KEY_RHS"            ]).ToUpper();
												string sLHS_MODULE               = Sql.ToString(rowRelationship["LHS_MODULE"              ]);
												string sRHS_MODULE               = Sql.ToString(rowRelationship["RHS_MODULE"              ]);
												string sLHS_TABLE                = Sql.ToString(rowRelationship["LHS_TABLE"               ]).ToUpper();
												string sRHS_TABLE                = Sql.ToString(rowRelationship["RHS_TABLE"               ]).ToUpper();
												string sLHS_KEY                  = Sql.ToString(rowRelationship["LHS_KEY"                 ]).ToUpper();
												string sRHS_KEY                  = Sql.ToString(rowRelationship["RHS_KEY"                 ]).ToUpper();
												string sRELATIONSHIP_TYPE        = Sql.ToString(rowRelationship["RELATIONSHIP_TYPE"       ]);
												string sRELATIONSHIP_ROLE_COLUMN = Sql.ToString(rowRelationship["RELATIONSHIP_ROLE_COLUMN"]).ToUpper();
												sJOIN_KEY_LHS = r.Replace(sJOIN_KEY_LHS, "");
												sJOIN_KEY_RHS = r.Replace(sJOIN_KEY_RHS, "");
												sLHS_MODULE   = r.Replace(sLHS_MODULE  , "");
												sRHS_MODULE   = r.Replace(sRHS_MODULE  , "");
												sLHS_TABLE    = r.Replace(sLHS_TABLE   , "");
												sRHS_TABLE    = r.Replace(sRHS_TABLE   , "");
												sLHS_KEY      = r.Replace(sLHS_KEY     , "");
												sRHS_KEY      = r.Replace(sRHS_KEY     , "");
												if ( sRELATIONSHIP_TYPE == "many-to-many" )
												{
													cmd.CommandText += "   and " + sJOIN_KEY_LHS + " in " + ControlChars.CrLf;
													cmd.CommandText += "(select " + sLHS_KEY + " from " + sLHS_TABLE + ControlChars.CrLf;
													Security.Filter(cmd, sLHS_MODULE, "list");
													cmd.CommandText += ")" + ControlChars.CrLf;
													
													// 11/12/2009 Paul.  We don't want to deal with relationships to multiple tables, so just ignore for now. 
													if ( sRELATIONSHIP_ROLE_COLUMN != "RELATED_TYPE" )
													{
														cmd.CommandText += "   and " + sJOIN_KEY_RHS + " in " + ControlChars.CrLf;
														cmd.CommandText += "(select " + sRHS_KEY + " from " + sRHS_TABLE + ControlChars.CrLf;
														Security.Filter(cmd, sRHS_MODULE, "list");
														cmd.CommandText += ")" + ControlChars.CrLf;
													}
												}
												else if ( sRELATIONSHIP_TYPE == "one-to-many" )
												{
													cmd.CommandText += "   and " + sRHS_KEY + " in " + ControlChars.CrLf;
													cmd.CommandText += "(select " + sLHS_KEY + " from " + sLHS_TABLE + ControlChars.CrLf;
													Security.Filter(cmd, sLHS_MODULE, "list");
													cmd.CommandText += ")" + ControlChars.CrLf;
												}
											}
										}
										else
										{
											// 11/12/2009 Paul.  EMAIL_IMAGES is a special table that is related to EMAILS or KBDOCUMENTS. 
											if ( sTABLE_NAME == "EMAIL_IMAGES" )
											{
												// 11/12/2009 Paul.  There does not appear to be an easy way to filter the EMAIL_IMAGES table. 
												// For now, just return the EMAIL related images. 
												cmd.CommandText += "   and PARENT_ID in " + ControlChars.CrLf;
												cmd.CommandText += "(select ID from EMAILS" + ControlChars.CrLf;
												Security.Filter(cmd, "Emails", "list");
												cmd.CommandText += "union all" + ControlChars.CrLf;
												cmd.CommandText += "select ID from KBDOCUMENTS" + ControlChars.CrLf;
												Security.Filter(cmd, "KBDocuments", "list");
												cmd.CommandText += ")" + ControlChars.CrLf;
											}
											// 11/06/2009 Paul.  If the relationship is not in the RELATIONSHIPS table, then try and build it manually. 
											// 11/05/2009 Paul.  We cannot use the standard filter on the Teams table (or TeamNotices). 
											else if ( !Sql.IsEmptyString(sMODULE_NAME) && !sMODULE_NAME.StartsWith("Team") )
											{
												// 11/05/2009 Paul.  We could query the foreign key tables to perpare the filters, but that is slow. 
												string sMODULE_TABLE_NAME   = Sql.ToString(Context.Application["Modules." + sMODULE_NAME + ".TableName"]).ToUpper();
												if ( !Sql.IsEmptyString(sMODULE_TABLE_NAME) )
												{
													string sMODULE_FIELD_ID = String.Empty;
													if ( sMODULE_TABLE_NAME.EndsWith("IES") )
														sMODULE_FIELD_ID = sMODULE_TABLE_NAME.Substring(0, sMODULE_TABLE_NAME.Length - 3) + "Y_ID";
													else if ( sMODULE_TABLE_NAME.EndsWith("S") )
														sMODULE_FIELD_ID = sMODULE_TABLE_NAME.Substring(0, sMODULE_TABLE_NAME.Length - 1) + "_ID";
													else
														sMODULE_FIELD_ID = sMODULE_TABLE_NAME + "_ID";
													
													cmd.CommandText += "   and " + sMODULE_FIELD_ID + " in " + ControlChars.CrLf;
													cmd.CommandText += "(select ID from " + sMODULE_TABLE_NAME + ControlChars.CrLf;
													Security.Filter(cmd, sMODULE_NAME, "list");
													cmd.CommandText += ")" + ControlChars.CrLf;
												}
											}
											// 11/05/2009 Paul.  We cannot use the standard filter on the Teams table. 
											if ( !Sql.IsEmptyString(sMODULE_NAME_RELATED) && !sMODULE_NAME_RELATED.StartsWith("Team") )
											{
												string sMODULE_TABLE_RELATED = Sql.ToString(Context.Application["Modules." + sMODULE_NAME_RELATED + ".TableName"]).ToUpper();
												if ( !Sql.IsEmptyString(sMODULE_TABLE_RELATED) )
												{
													string sMODULE_RELATED_ID = String.Empty;
													if ( sMODULE_TABLE_RELATED.EndsWith("IES") )
														sMODULE_RELATED_ID = sMODULE_TABLE_RELATED.Substring(0, sMODULE_TABLE_RELATED.Length - 3) + "Y_ID";
													else if ( sMODULE_TABLE_RELATED.EndsWith("S") )
														sMODULE_RELATED_ID = sMODULE_TABLE_RELATED.Substring(0, sMODULE_TABLE_RELATED.Length - 1) + "_ID";
													else
														sMODULE_RELATED_ID = sMODULE_TABLE_RELATED + "_ID";
													// 11/05/2009 Paul.  Some tables use ASSIGNED_USER_ID as the relationship ID instead of the USER_ID. 
													if ( sMODULE_RELATED_ID == "USER_ID" && !Sql.IsEmptyString(sASSIGNED_FIELD_NAME) )
														sMODULE_RELATED_ID = sASSIGNED_FIELD_NAME;
													
													cmd.CommandText += "   and " + sMODULE_RELATED_ID + " in " + ControlChars.CrLf;
													cmd.CommandText += "(select ID from " + sMODULE_TABLE_RELATED + ControlChars.CrLf;
													Security.Filter(cmd, sMODULE_NAME_RELATED, "list");
													cmd.CommandText += ")" + ControlChars.CrLf;
												}
											}
										}
									}
									else
									{
										// 02/14/2010 Paul.  GetTable should only require read-only access. 
										// We were previously requiring Edit access, but that seems to be a high bar. 
										Security.Filter(cmd, sMODULE_NAME, "view");
									}
									if ( !Sql.IsEmptyString(sMODULE_FIELD_NAME) )
									{
										// 11/08/2009 Paul.  We need to combine the two module lists into a single list. 
										List<string> lstMODULES = new List<string>();
										List<string> lstAVAILABLE_MODULES = SplendidCache.AccessibleModules(Context, Security.USER_ID);
										if ( lstAVAILABLE_MODULES != null )
										{
											if ( !Sql.IsEmptyString(sREQUESTED_MODULES) )
											{
												sREQUESTED_MODULES = Regex.Replace(sREQUESTED_MODULES, @"[^A-Za-z0-9_,]", "");
												string[] arrREQUESTED_MODULES = sREQUESTED_MODULES.Split(',');
												foreach ( string s in arrREQUESTED_MODULES )
												{
													// 11/08/2009 Paul.  The module must be in both lists. 
													if ( lstAVAILABLE_MODULES.Contains(s) )
														lstMODULES.Add(s);
													// 09/30/2014 Paul.  Need to add activities manually. 
													if ( s == "Calls" || s == "Meetings" )
														lstMODULES.Add("Activities");
												}
											}
											else
											{
												lstMODULES = lstAVAILABLE_MODULES;
											}
										}
										// 11/14/2009 Paul.  Make sure to add Teams to the list of available modules. 
										if ( Crm.Config.enable_team_management() )
										{
											if ( !lstMODULES.Contains("Teams") )
												lstMODULES.Add("Teams");
											if ( !lstMODULES.Contains("TeamNotices") )
												lstMODULES.Add("TeamNotices");
										}
										// 11/22/2009 Paul.  Simplify the logic by having a local list of system modules. 
										string[] arrSystemModules = new string[] { "ACL", "ACLActions", "ACLRoles", "Audit", "Config", "Currencies", "Dashlets"
										                                         , "DocumentRevisions", "DynamicButtons", "Export", "FieldValidators", "Import"
										                                         , "Merge", "Modules", "Offline", "Releases", "Roles", "SavedSearch", "Shortcuts"
										                                         , "Teams", "TeamNotices", "Terminology", "Users", "SystemSyncLog"
										                                         };
										foreach ( string sSystemModule in arrSystemModules )
											lstMODULES.Add(sSystemModule);
										
										if ( sTABLE_NAME == "MODULES" )
										{
											// 11/27/2009 Paul.  Don't filter the MODULES table. It can cause system tables to get deleted. 
											// 11/28/2009 Paul.  Keep the filter on the Modules table, but add the System Sync Tables to the list. 
											// We should make sure that the clients do not get module records for unnecessary or disabled modules. 
											Sql.AppendParameter(cmd, lstMODULES.ToArray(), sMODULE_FIELD_NAME);
										}
										else if ( nMODULE_SPECIFIC == 1 )
										{
											Sql.AppendParameter(cmd, lstMODULES.ToArray(), sMODULE_FIELD_NAME);
											// 09/30/2014 Paul.  We need to filter target buttons as well. 
											if ( sTABLE_NAME == "DYNAMIC_BUTTONS" )
											{
												cmd.CommandText += "   and ( 1 = 0" + ControlChars.CrLf;
												cmd.CommandText += "         or TARGET_NAME is null" + ControlChars.CrLf;
												cmd.CommandText += "     ";
												Sql.AppendParameter(cmd, lstMODULES.ToArray(), "TARGET_NAME", true);
												cmd.CommandText += "       )" + ControlChars.CrLf;
											}
										}
										else if ( nMODULE_SPECIFIC == 2 )
										{
											// 04/05/2012 Paul.  AppendLikeModules is a special like that assumes that the search is for a module related value 
											Sql.AppendLikeModules(cmd, lstMODULES.ToArray(), sMODULE_FIELD_NAME);
										}
										else if ( nMODULE_SPECIFIC == 3 )
										{
											cmd.CommandText += "   and ( 1 = 0" + ControlChars.CrLf;
											cmd.CommandText += "         or " + sMODULE_FIELD_NAME + " is null" + ControlChars.CrLf;
											// 11/02/2009 Paul.  There are a number of terms with undefined modules. 
											// ACL, ACLActions, Audit, Config, Dashlets, DocumentRevisions, Export, Merge, Roles, SavedSearch, Teams
											cmd.CommandText += "     ";
											Sql.AppendParameter(cmd, lstMODULES.ToArray(), sMODULE_FIELD_NAME, true);
											cmd.CommandText += "       )" + ControlChars.CrLf;
										}
										// 11/22/2009 Paul.  Make sure to only send the selected user language.  This will dramatically reduce the amount of data. 
										if ( sTABLE_NAME == "TERMINOLOGY" || sTABLE_NAME == "TERMINOLOGY_HELP" )
										{
											cmd.CommandText += "   and LANG in ('en-US', @LANG)" + ControlChars.CrLf;
											string sCULTURE  = Sql.ToString(Session["USER_SETTINGS/CULTURE" ]);
											Sql.AddParameter(cmd, "@LANG", sCULTURE);
										}
									}
		
									if ( dtDATE_MODIFIED_UTC != DateTime.MinValue )
									{
										cmd.CommandText += "   and " + Sql.MetadataName(cmd, sVIEW_NAME) + ".DATE_MODIFIED_UTC > @DATE_MODIFIED_UTC" + ControlChars.CrLf;
										Sql.AddParameter(cmd, "@DATE_MODIFIED_UTC", dtDATE_MODIFIED_UTC);
									}
									if ( arrITEMS != null )
									{
										// 11/13/2009 Paul.  If a list of items is provided, then the max records field is ignored. 
										nMAX_RECORDS = -1;
										Sql.AppendGuids(cmd, arrITEMS, "ID");
									}
									else if ( sTABLE_NAME == "IMAGES" )
									{
										// 02/14/2010 Paul.  There is no easy way to filter IMAGES table, so we are simply going to fetch 
										// images that the user has created.  Otherwise, images that are accessible to the user will 
										// need to be retrieved by ID.
										Sql.AppendParameter(cmd, Security.USER_ID, "CREATED_BY");
									}
									cmd.CommandText += " order by " + Sql.MetadataName(cmd, sVIEW_NAME) + ".DATE_MODIFIED_UTC" + ControlChars.CrLf;
#if DEBUG
									SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
#endif
									
									using ( DbDataAdapter da = dbf.CreateDataAdapter() )
									{
										((IDbDataAdapter)da).SelectCommand = cmd;
										// 11/08/2009 Paul.  The table name is required in order to serialize the DataTable. 
										dt = new DataTable(sTABLE_NAME);
										if ( nMAX_RECORDS > 0 )
										{
											using ( DataSet ds = new DataSet() )
											{
												ds.Tables.Add(dt);
												da.Fill(ds, 0, nMAX_RECORDS, sTABLE_NAME);
											}
										}
										else
										{
											da.Fill(dt);
										}
										// 01/18/2010 Paul.  Apply ACL Field Security. 
										// 02/01/2010 Paul.  System tables may not have a valid Module name, so Field Security will not apply. 
										if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(sMODULE_NAME) )
										{
											bool bApplyACL = false;
											bool bASSIGNED_USER_ID_Exists = dt.Columns.Contains("ASSIGNED_USER_ID");
											foreach ( DataRow row in dt.Rows )
											{
												Guid gASSIGNED_USER_ID = Guid.Empty;
												if ( bASSIGNED_USER_ID_Exists )
													gASSIGNED_USER_ID = Sql.ToGuid(row["ASSIGNED_USER_ID"]);
												foreach ( DataColumn col in dt.Columns )
												{
													Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, col.ColumnName, gASSIGNED_USER_ID);
													if ( !acl.IsReadable() )
													{
														row[col.ColumnName] = DBNull.Value;
														bApplyACL = true;
													}
												}
											}
											if ( bApplyACL )
												dt.AcceptChanges();
										}
										if ( sTABLE_NAME == "USERS" )
										{
											// 11/12/2009 Paul.  For the USERS table, we are going to limit the data return to the client. 
											foreach ( DataRow row in dt.Rows )
											{
												if ( Sql.ToGuid(row["ID"]) != Security.USER_ID )
												{
													foreach ( DataColumn col in dt.Columns )
													{
														// 11/12/2009 Paul.  Allow auditing fields and basic user info. 
														if (  col.ColumnName != "ID"               
														   && col.ColumnName != "DELETED"          
														   && col.ColumnName != "CREATED_BY"       
														   && col.ColumnName != "DATE_ENTERED"     
														   && col.ColumnName != "MODIFIED_USER_ID" 
														   && col.ColumnName != "DATE_MODIFIED"    
														   && col.ColumnName != "DATE_MODIFIED_UTC"
														   && col.ColumnName != "USER_NAME"        
														   && col.ColumnName != "FIRST_NAME"       
														   && col.ColumnName != "LAST_NAME"        
														   && col.ColumnName != "REPORTS_TO_ID"    
														   && col.ColumnName != "EMAIL1"           
														   && col.ColumnName != "STATUS"           
														   && col.ColumnName != "IS_GROUP"         
														   && col.ColumnName != "PORTAL_ONLY"      
														   && col.ColumnName != "EMPLOYEE_STATUS"  
														   )
														{
															row[col.ColumnName] = DBNull.Value;
														}
													}
												}
											}
											dt.AcceptChanges();
										}
									}
								}
							}
							else
							{
								SplendidError.SystemError(new StackTrace(true).GetFrame(0), sTABLE_NAME + " cannot be synchronized.");
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
			return dt;
		}

		[WebMethod(EnableSession=true)]
		public DataTable UpdateModuleTable(string TableName, DataTable Table)
		{
			return UpdateTable(TableName, Table);
		}

		private DataTable UpdateTable(string sTABLE_NAME, DataTable dtUPDATE)
		{
			DataTable dtResults = dtUPDATE.Clone();
			try
			{
				dtResults.Columns.Add("SPLENDID_SYNC_STATUS" , typeof(System.String));
				dtResults.Columns.Add("SPLENDID_SYNC_MESSAGE", typeof(System.String));
				if ( Security.IsAuthenticated() )
				{
					string sCULTURE = Sql.ToString (Session["USER_SETTINGS/CULTURE"]);
					L10N   L10n     = new L10N(sCULTURE);
					Regex  r        = new Regex(@"[^A-Za-z0-9_]");
					sTABLE_NAME = r.Replace(sTABLE_NAME, "").ToUpper();
					
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						// 06/03/2011 Paul.  Cache the Sync Table data. 
						// 11/26/2009 Paul.  System tables cannot be updated. 
						using ( DataTable dtSYNC_TABLES = SplendidCache.SyncTables(sTABLE_NAME, true) )
						{
							string sSQL = String.Empty;
							if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count > 0 )
							{
								DataRow rowSYNC_TABLE = dtSYNC_TABLES.Rows[0];
								string sMODULE_NAME = Sql.ToString (rowSYNC_TABLE["MODULE_NAME"]);
								string sVIEW_NAME   = Sql.ToString (rowSYNC_TABLE["VIEW_NAME"  ]);
								bool   bHAS_CUSTOM  = Sql.ToBoolean(rowSYNC_TABLE["HAS_CUSTOM" ]);
								// 02/14/2010 Paul.  GetUserAccess requires a non-null sMODULE_NAME. 
								// Lets catch the exception here so that we can throw a meaningful error. 
								if ( Sql.IsEmptyString(sMODULE_NAME) && !sTABLE_NAME.StartsWith("TEAM_SETS") )
								{
									throw(new Exception("sMODULE_NAME should not be empty for table " + sTABLE_NAME));
								}
								
								// 11/11/2009 Paul.  First check if the user has access to this module. 
								// 11/27/2009 Paul.  TEAM_SETS and TEAM_SETS_TEAMS are special module-type tables that do not have the normal security rules. 
								if ( sTABLE_NAME.StartsWith("TEAM_SETS") || SplendidCRM.Security.GetUserAccess(sMODULE_NAME, "edit") >= 0 )
								{
									foreach ( DataRow row in dtUPDATE.Rows )
									{
										bool      bRecordExists              = false;
										bool      bAccessAllowed             = false;
										bool      bConflicted                = false;
										Guid      gID                        = Sql.ToGuid    (row["ID"                      ]);
										DateTime  dtREMOTE_DATE_MODIFIED_UTC = Sql.ToDateTime(row["REMOTE_DATE_MODIFIED_UTC"]);
										Guid      gLOCAL_ASSIGNED_USER_ID    = Guid.Empty;
										DataRow   rowCurrent                 = null;
										DataTable dtCurrent                  = new DataTable();
										sSQL = "select *"              + ControlChars.CrLf
										     + "  from " + sTABLE_NAME + ControlChars.CrLf
										     + " where 1 = 1"          + ControlChars.CrLf;
										using ( IDbCommand cmd = con.CreateCommand() )
										{
											cmd.CommandText = sSQL;
											Sql.AppendParameter(cmd, gID, "ID");
											using ( DbDataAdapter da = dbf.CreateDataAdapter() )
											{
												((IDbDataAdapter)da).SelectCommand = cmd;
												// 11/27/2009 Paul.  It may be useful to log the SQL during errors at this location. 
												try
												{
													da.Fill(dtCurrent);
												}
												catch
												{
													SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
													throw;
												}
												if ( dtCurrent.Rows.Count > 0 )
												{
													rowCurrent = dtCurrent.Rows[0];
													bRecordExists = true;
													if ( dtREMOTE_DATE_MODIFIED_UTC != DateTime.MinValue )
													{
														if ( Sql.ToDateTime(rowCurrent["DATE_MODIFIED_UTC"]) > dtREMOTE_DATE_MODIFIED_UTC )
														{
															bConflicted = true;
														}
													}
													// 01/18/2010 Paul.  Apply ACL Field Security. 
													if ( dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
													{
														gLOCAL_ASSIGNED_USER_ID = Sql.ToGuid(rowCurrent["ASSIGNED_USER_ID"]);
													}
												}
											}
										}
										// 11/11/2009 Paul.  If we have conflicting edits, exit without generating an error. 
										// The conflict will eventually get reported on the client side. 
										if ( !bConflicted )
										{
											// 11/27/2009 Paul.  TEAM_SETS and TEAM_SETS_TEAMS are special module-type tables that do not have the normal security rules. 
											if ( sTABLE_NAME.StartsWith("TEAM_SETS") )
												bAccessAllowed = true;
											else if ( bRecordExists )
											{
												sSQL = "select count(*)"       + ControlChars.CrLf
												     + "  from " + sTABLE_NAME + ControlChars.CrLf;
												using ( IDbCommand cmd = con.CreateCommand() )
												{
													cmd.CommandText = sSQL;
													Security.Filter(cmd, sMODULE_NAME, "edit");
													Sql.AppendParameter(cmd, gID, "ID");
													// 11/27/2009 Paul.  It may be useful to log the SQL during errors at this location. 
													try
													{
														if ( Sql.ToInteger(cmd.ExecuteScalar()) > 0 )
															bAccessAllowed = true;
													}
													catch
													{
														SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
														throw;
													}
												}
											}
											if ( !bRecordExists || bAccessAllowed )
											{
												DataTable dtMetadata = SplendidCache.SqlColumns(sTABLE_NAME);
												using ( IDbTransaction trn = Sql.BeginTransaction(con) )
												{
													try
													{
														bool bEnableTeamManagement  = Crm.Config.enable_team_management();
														bool bRequireTeamManagement = Crm.Config.require_team_management();
														bool bRequireUserAssignment = Crm.Config.require_user_assignment();
														//IDbCommand cmdUpdate = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Update");
														// 11/26/2009 Paul.  Using the update stored procedures is problematic when relationships are created. 
														// The problem is that the relationship will be sync'd separately and can cause duplicate records. 
														// 08/27/2014 Paul.  There is too much logic to ignore, including normalizing teams and adding to the phone search table. 
														// Most relationship procedures prevent duplicates, so it should not be an issue. 
														/*
														IDbCommand cmdUpdate = con.CreateCommand();
														cmdUpdate.CommandType = CommandType.Text;
														cmdUpdate.Transaction = trn;
														if ( !bRecordExists )
														{
															// 11/26/2009 Paul.  Build the command text first.  This is necessary in order for the parameter function
															// to properly replace the @ symbol with the database-specific token. 
															StringBuilder sb = new StringBuilder();
															sb.Append("insert into " + sTABLE_NAME + "(");
															for( int i=0 ; i < dtMetadata.Rows.Count; i++ )
															{
																DataRow rowMetadata = dtMetadata.Rows[i];
																if ( i > 0 )
																	sb.Append(", ");
																sb.Append(Sql.ToString (rowMetadata["ColumnName"]));
															}
															sb.AppendLine(")" + ControlChars.CrLf);
															sb.AppendLine("values(");
															for( int i=0 ; i < dtMetadata.Rows.Count; i++ )
															{
																DataRow rowMetadata = dtMetadata.Rows[i];
																if ( i > 0 )
																	sb.Append(", ");
																sb.Append(Sql.CreateDbName(cmdUpdate, "@" + Sql.ToString(rowMetadata["ColumnName"])));
															}
															sb.AppendLine(")");
															cmdUpdate.CommandText = sb.ToString();
															
															foreach ( DataRow rowMetadata in dtMetadata.Rows )
															{
																string sColumnName = Sql.ToString (rowMetadata["ColumnName"]);
																string sCsType     = Sql.ToString (rowMetadata["CsType"    ]);
																int    nLength     = Sql.ToInteger(rowMetadata["length"    ]);
																IDbDataParameter par = Sql.CreateParameter(cmdUpdate, "@" + sColumnName, sCsType, nLength);
																// 06/04/2009 Paul.  If Team is required, then make sure to initialize the TEAM_ID.  Same is true for ASSIGNED_USER_ID. 
																if ( String.Compare(sColumnName, "TEAM_ID", true) == 0 && bEnableTeamManagement ) // 02/26/2011 Paul.  Ignore the Required flag. && bRequireTeamManagement )
																	par.Value = Sql.ToDBGuid(Security.TEAM_ID);  // 02/26/2011 Paul.  Make sure to convert Guid.Empty to DBNull. 
																else if ( String.Compare(sColumnName, "ASSIGNED_USER_ID", true) == 0 ) // 02/26/2011 Paul.  Always set the Assigned User ID. && bRequireUserAssignment )
																	par.Value = Sql.ToDBGuid(Security.USER_ID);  // 02/26/2011 Paul.  Make sure to convert Guid.Empty to DBNull. 
																// 11/26/2009 Paul.  The UTC modified date should be set to Now. 
																else if ( String.Compare(sColumnName, "DATE_MODIFIED_UTC", true) == 0 )
																	par.Value = DateTime.UtcNow;
																else
																	par.Value = DBNull.Value;
															}
														}
														else
														{
															// 11/26/2009 Paul.  Build the command text first.  This is necessary in order for the parameter function
															// to properly replace the @ symbol with the database-specific token. 
															StringBuilder sb = new StringBuilder();
															for( int i=0 ; i < dtMetadata.Rows.Count; i++ )
															{
																DataRow rowMetadata = dtMetadata.Rows[i];
																string sColumnName = Sql.ToString(rowMetadata["ColumnName"]);
																if ( String.Compare(sColumnName, "ID", true) != 0 )
																{
																	if ( sb.Length == 0 )
																		sb.Append("   set ");
																	else
																		sb.Append("     , ");
																	sb.Append(sColumnName);
																	sb.Append(" = @");
																	sb.Append(sColumnName);
																	sb.Append(ControlChars.CrLf);
																}
															}
															cmdUpdate.CommandText = "update " + sTABLE_NAME + ControlChars.CrLf
															                      + sb.ToString()
															                      + " where ID = @ID";
															
															foreach ( DataRow rowMetadata in dtMetadata.Rows )
															{
																string sColumnName = Sql.ToString (rowMetadata["ColumnName"]);
																string sCsType     = Sql.ToString (rowMetadata["CsType"    ]);
																int    nLength     = Sql.ToInteger(rowMetadata["length"    ]);
																if ( String.Compare(sColumnName, "ID", true) != 0 )
																{
																	IDbDataParameter par = Sql.CreateParameter(cmdUpdate, "@" + sColumnName, sCsType, nLength);
																	// 06/04/2009 Paul.  If Team is required, then make sure to initialize the TEAM_ID.  Same is true for ASSIGNED_USER_ID. 
																	if ( String.Compare(sColumnName, "TEAM_ID", true) == 0 && bEnableTeamManagement ) // 02/26/2011 Paul.  Ignore the Required flag. && bRequireTeamManagement )
																		par.Value = Sql.ToDBGuid(Security.TEAM_ID);  // 02/26/2011 Paul.  Make sure to convert Guid.Empty to DBNull. 
																	else if ( String.Compare(sColumnName, "ASSIGNED_USER_ID", true) == 0 ) // 02/26/2011 Paul.  Always set the Assigned User ID. && bRequireUserAssignment )
																		par.Value = Sql.ToDBGuid(Security.USER_ID);  // 02/26/2011 Paul.  Make sure to convert Guid.Empty to DBNull. 
																	// 11/26/2009 Paul.  The UTC modified date should be set to Now. 
																	else if ( String.Compare(sColumnName, "DATE_MODIFIED_UTC", true) == 0 )
																		par.Value = DateTime.UtcNow;
																	else
																		par.Value = DBNull.Value;
																}
															}
															Sql.AddParameter(cmdUpdate, "@ID", gID);
															// 11/11/2009 Paul.  If the record already exists, then the current values are treated as default values. 
															foreach ( DataColumn col in rowCurrent.Table.Columns )
															{
																IDbDataParameter par = Sql.FindParameter(cmdUpdate, col.ColumnName);
																// 11/26/2009 Paul.  The UTC modified date should be set to Now. 
																if ( par != null && String.Compare(col.ColumnName, "DATE_MODIFIED_UTC", true) != 0 )
																	par.Value = rowCurrent[col.ColumnName];
															}
														}
														
														foreach ( DataColumn col in row.Table.Columns )
														{
															// 01/18/2010 Paul.  Apply ACL Field Security. 
															// 02/01/2010 Paul.  System tables may not have a valid Module name, so Field Security will not apply. 
															bool bIsWriteable = true;
															if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(sMODULE_NAME) )
															{
																Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, col.ColumnName, Guid.Empty);
																bIsWriteable = acl.IsWriteable();
															}
															if ( bIsWriteable )
															{
																IDbDataParameter par = Sql.FindParameter(cmdUpdate, col.ColumnName);
																// 11/26/2009 Paul.  The UTC modified date should be set to Now. 
																if ( par != null && String.Compare(col.ColumnName, "DATE_MODIFIED_UTC", true) != 0 )
																	par.Value = row[col.ColumnName];
															}
														}
														*/
														// 08/27/2014 Paul.  There is too much logic to ignore, including normalizing teams and adding to the phone search table. 
														// Most relationship procedures prevent duplicates, so it should not be an issue. 
														// 08/27/2014 Paul.  The previous technique updated the DELETED flag.  We need to manage it separately here. 
														if ( row.Table.Columns.Contains("DELETED") && Sql.ToBoolean(row["DELETED"]) )
														{
															IDbCommand cmdDelete = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Delete");
															cmdDelete.Transaction = trn;
															Sql.SetParameter(cmdDelete, "@ID"              , gID             );
															Sql.SetParameter(cmdDelete, "@MODIFIED_USER_ID", Security.USER_ID);
															cmdDelete.ExecuteNonQuery();
														}
														else
														{
															IDbCommand cmdUpdate = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Update");
															cmdUpdate.Transaction = trn;
															foreach(IDbDataParameter par in cmdUpdate.Parameters)
															{
																// 03/27/2010 Paul.  The ParameterName will start with @, so we need to remove it. 
																string sParameterName = Sql.ExtractDbName(cmdUpdate, par.ParameterName).ToUpper();
																if ( sParameterName == "TEAM_ID" && bEnableTeamManagement )
																	par.Value = Sql.ToDBGuid(Security.TEAM_ID);  // 02/26/2011 Paul.  Make sure to convert Guid.Empty to DBNull. 
																else if ( sParameterName == "ASSIGNED_USER_ID" )
																	par.Value = Sql.ToDBGuid(Security.USER_ID);  // 02/26/2011 Paul.  Make sure to convert Guid.Empty to DBNull. 
																else if ( sParameterName == "MODIFIED_USER_ID" )
																	par.Value = Sql.ToDBGuid(Security.USER_ID);
																else
																	par.Value = DBNull.Value;
															}
															if ( bRecordExists )
															{
																// 11/11/2009 Paul.  If the record already exists, then the current values are treated as default values. 
																foreach ( DataColumn col in rowCurrent.Table.Columns )
																{
																	IDbDataParameter par = Sql.FindParameter(cmdUpdate, col.ColumnName);
																	// 11/26/2009 Paul.  The UTC modified date should be set to Now. 
																	if ( par != null && String.Compare(col.ColumnName, "DATE_MODIFIED_UTC", true) != 0 )
																		par.Value = rowCurrent[col.ColumnName];
																}
															}
															foreach ( DataColumn col in row.Table.Columns )
															{
																// 01/18/2010 Paul.  Apply ACL Field Security. 
																// 02/01/2010 Paul.  System tables may not have a valid Module name, so Field Security will not apply. 
																bool bIsWriteable = true;
																if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(sMODULE_NAME) )
																{
																	Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, col.ColumnName, Guid.Empty);
																	bIsWriteable = acl.IsWriteable();
																}
																if ( bIsWriteable )
																{
																	IDbDataParameter par = Sql.FindParameter(cmdUpdate, col.ColumnName);
																	// 11/26/2009 Paul.  The UTC modified date should be set to Now. 
																	if ( par != null )
																	{
																		switch ( par.DbType )
																		{
																			// 10/08/2011 Paul.  We must use Sql.ToDBDateTime, otherwise we get a an error whe DateTime.MinValue is used. 
																			// SqlDateTime overflow. Must be between 1/1/1753 12:00:00 AM and 12/31/9999 11:59:59 PM.
																			// 05/05/2013 Paul.  We need to convert the date to the user's timezone. 
																			case DbType.Date                 :  par.Value = Sql.ToDBDateTime(row[col.ColumnName]);  break;
																			case DbType.DateTime             :  par.Value = Sql.ToDBDateTime(row[col.ColumnName]);  break;
																			case DbType.Int16                :  par.Value = Sql.ToDBInteger (row[col.ColumnName]);  break;
																			case DbType.Int32                :  par.Value = Sql.ToDBInteger (row[col.ColumnName]);  break;
																			case DbType.Int64                :  par.Value = Sql.ToDBInteger (row[col.ColumnName]);  break;
																			case DbType.UInt16               :  par.Value = Sql.ToDBInteger (row[col.ColumnName]);  break;
																			case DbType.UInt32               :  par.Value = Sql.ToDBInteger (row[col.ColumnName]);  break;
																			case DbType.UInt64               :  par.Value = Sql.ToDBInteger (row[col.ColumnName]);  break;
																			case DbType.Single               :  par.Value = Sql.ToDBFloat   (row[col.ColumnName]);  break;
																			case DbType.Double               :  par.Value = Sql.ToDBFloat   (row[col.ColumnName]);  break;
																			case DbType.Decimal              :  par.Value = Sql.ToDBDecimal (row[col.ColumnName]);  break;
																			case DbType.Currency             :  par.Value = Sql.ToDBDecimal (row[col.ColumnName]);  break;
																			case DbType.Boolean              :  par.Value = Sql.ToDBBoolean (row[col.ColumnName]);  break;
																			case DbType.Guid                 :  par.Value = Sql.ToDBGuid    (row[col.ColumnName]);  break;
																			case DbType.String               :  par.Value = Sql.ToDBString  (row[col.ColumnName]);  break;
																			case DbType.StringFixedLength    :  par.Value = Sql.ToDBString  (row[col.ColumnName]);  break;
																			case DbType.AnsiString           :  par.Value = Sql.ToDBString  (row[col.ColumnName]);  break;
																			case DbType.AnsiStringFixedLength:  par.Value = Sql.ToDBString  (row[col.ColumnName]);  break;
																		}
																	}
																}
															}
															cmdUpdate.ExecuteScalar();
															if ( bHAS_CUSTOM )
															{
																DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sTABLE_NAME);
																SplendidDynamic.UpdateCustomFields(row, trn, gID, sTABLE_NAME, dtCustomFields);
															}
														}
														trn.Commit();
													}
													catch
													{
														trn.Rollback();
														throw;
													}
													try
													{
														if ( bHAS_CUSTOM )
														{
															sSQL = "select " + sVIEW_NAME  + ".*     " + ControlChars.CrLf
															     + "     , " + sTABLE_NAME + "_CSTM.*" + ControlChars.CrLf
															     + "  from " + sVIEW_NAME              + ControlChars.CrLf
															     + "  left outer join " + sTABLE_NAME + "_CSTM" + ControlChars.CrLf
															     + "               on " + sTABLE_NAME + "_CSTM.ID_C = " + sVIEW_NAME + ".ID" + ControlChars.CrLf
															     + " where 1 = 1"                      + ControlChars.CrLf;
														}
														else
														{
															sSQL = "select " + sVIEW_NAME + ".*" + ControlChars.CrLf
															     + "  from " + sVIEW_NAME        + ControlChars.CrLf
															     + " where 1 = 1"                + ControlChars.CrLf;
														}
														using ( IDbCommand cmd = con.CreateCommand() )
														{
															cmd.CommandText = sSQL;
															cmd.CommandTimeout = 0;
															Sql.AppendParameter(cmd, gID, "ID");
															using ( DbDataAdapter da = dbf.CreateDataAdapter() )
															{
																((IDbDataAdapter)da).SelectCommand = cmd;
																using ( DataTable dtItem = new DataTable() )
																{
																	da.Fill(dtItem);
																	if ( dtItem.Rows.Count > 0 )
																	{
																		DataRow rowItem   = dtItem.Rows[0];
																		DataRow rowResult = dtResults.NewRow();
																		rowResult["SPLENDID_SYNC_STATUS" ] = "Updated";
																		rowResult["SPLENDID_SYNC_MESSAGE"] = DBNull.Value;
																		foreach ( DataColumn colItem in dtItem.Columns )
																		{
																			if ( rowResult.Table.Columns.Contains(colItem.ColumnName) )
																				rowResult[colItem.ColumnName] = rowItem[colItem.ColumnName];
																		}
																		dtResults.Rows.Add(rowResult);
																	}
																}
															}
														}
													}
													catch(Exception ex)
													{
														SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
														DataRow rowError = dtResults.NewRow();
														dtResults.Rows.Add(rowError);
														rowError["ID"                   ] = gID;
														rowError["SPLENDID_SYNC_STATUS" ] = "Updated with Read Error";
														rowError["SPLENDID_SYNC_MESSAGE"] = L10n.Term("Offline.LBL_READ_ERROR");
													}
												}
											}
											else
											{
												DataRow rowError = dtResults.NewRow();
												dtResults.Rows.Add(rowError);
												rowError["ID"                   ] = gID;
												rowError["SPLENDID_SYNC_STATUS" ] = "Access Denied";
												rowError["SPLENDID_SYNC_MESSAGE"] = L10n.Term("ACL.LBL_NO_ACCESS");
											}
										}
										else
										{
											DataRow rowConflicted = dtResults.NewRow();
											dtResults.Rows.Add(rowConflicted);
											rowConflicted["ID"                   ] = gID;
											rowConflicted["SPLENDID_SYNC_STATUS" ] = "Conflicted";
											rowConflicted["SPLENDID_SYNC_MESSAGE"] = L10n.Term("Offline.ERR_CONFLICTED");
										}
									}
								}
								else
								{
									throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
			return dtResults;
		}
	}
}
