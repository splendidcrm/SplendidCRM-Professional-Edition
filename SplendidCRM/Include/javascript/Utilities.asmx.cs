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
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Web.Services;
using System.ComponentModel;
using SplendidCRM;

namespace SplendidCRM.Utilities
{
	/// <summary>
	/// Summary description for Modules
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.Web.Script.Services.ScriptService]
	[ToolboxItem(false)]
	public class Modules : System.Web.Services.WebService
	{
		[WebMethod(EnableSession=true)]
		public bool AddToFavorites(string sMODULE, Guid gID)
		{
			bool bSucceeded = false;
			//try
			{
				if ( !Security.IsAuthenticated() )
					throw(new Exception("Authentication required"));
				
				// 03/31/2012 Paul.  Use the standard filter to verify that the user can view the record. 
				if ( !Sql.IsEmptyString(sMODULE) && !Sql.IsEmptyGuid(gID) && SplendidCRM.Security.GetUserAccess(sMODULE, "view") >= 0 )
				{
					string sTABLE_NAME = Crm.Modules.TableName(sMODULE);
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						string sSQL ;
						sSQL = "select NAME           " + ControlChars.CrLf
						    + "  from vw" + sTABLE_NAME + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Security.Filter(cmd, sMODULE, "view");
							Sql.AppendParameter(cmd, gID, "ID", false);
							con.Open();

							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									if ( dt.Rows.Count > 0 )
									{
										DataRow rdr = dt.Rows[0];
										string sNAME = Sql.ToString(rdr["NAME"]);
										SqlProcs.spSUGARFAVORITES_Update(Security.USER_ID, sMODULE, gID, sNAME);
										SplendidCache.ClearFavorites();
										bSucceeded = true;
									}
								}
							}
						}
					}
				}
			}
			//catch
			{
				// 02/04/2007 Paul.  Don't catch the exception.  
				// It is a web service, so the exception will be handled properly by the AJAX framework. 
			}
			return bSucceeded;
		}

		[WebMethod(EnableSession=true)]
		public bool RemoveFromFavorites(string sMODULE, Guid gID)
		{
			bool bSucceeded = false;
			//try
			{
				if ( !Security.IsAuthenticated() )
					throw(new Exception("Authentication required"));
				
				// 03/31/2012 Paul.  No need to validate on remove as the item would not be in the list if the user did not have access to it. 
				if ( !Sql.IsEmptyString(sMODULE) && !Sql.IsEmptyGuid(gID) && SplendidCRM.Security.GetUserAccess(sMODULE, "view") >= 0 )
				{
					SqlProcs.spSUGARFAVORITES_Delete(Security.USER_ID, gID);
					SplendidCache.ClearFavorites();
					bSucceeded = true;
				}
			}
			//catch
			{
				// 02/04/2007 Paul.  Don't catch the exception.  
				// It is a web service, so the exception will be handled properly by the AJAX framework. 
			}
			return bSucceeded;
		}

		// 10/09/2015 Paul.  Add methods to manage subscriptions. 
		[WebMethod(EnableSession=true)]
		public bool AddSubscription(string sMODULE, Guid gID)
		{
			bool bSucceeded = false;
			//try
			{
				if ( !Security.IsAuthenticated() )
					throw(new Exception("Authentication required"));
				
				// 03/31/2012 Paul.  Use the standard filter to verify that the user can view the record. 
				if ( !Sql.IsEmptyString(sMODULE) && !Sql.IsEmptyGuid(gID) && SplendidCRM.Security.GetUserAccess(sMODULE, "view") >= 0 )
				{
					string sTABLE_NAME = Crm.Modules.TableName(sMODULE);
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						string sSQL ;
						sSQL = "select NAME           " + ControlChars.CrLf
						    + "  from vw" + sTABLE_NAME + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Security.Filter(cmd, sMODULE, "view");
							Sql.AppendParameter(cmd, gID, "ID", false);
							con.Open();

							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									if ( dt.Rows.Count > 0 )
									{
										DataRow rdr = dt.Rows[0];
										string sNAME = Sql.ToString(rdr["NAME"]);
										SqlProcs.spSUBSCRIPTIONS_Update(Security.USER_ID, sMODULE, gID);
										SplendidCache.ClearSubscriptions();
										bSucceeded = true;
									}
								}
							}
						}
					}
				}
			}
			//catch
			{
				// 02/04/2007 Paul.  Don't catch the exception.  
				// It is a web service, so the exception will be handled properly by the AJAX framework. 
			}
			return bSucceeded;
		}

		// 10/09/2015 Paul.  Add methods to manage subscriptions. 
		[WebMethod(EnableSession=true)]
		public bool RemoveSubscription(string sMODULE, Guid gID)
		{
			bool bSucceeded = false;
			//try
			{
				if ( !Security.IsAuthenticated() )
					throw(new Exception("Authentication required"));
				
				// 03/31/2012 Paul.  No need to validate on remove as the item would not be in the list if the user did not have access to it. 
				if ( !Sql.IsEmptyString(sMODULE) && !Sql.IsEmptyGuid(gID) && SplendidCRM.Security.GetUserAccess(sMODULE, "view") >= 0 )
				{
					SqlProcs.spSUBSCRIPTIONS_Delete(Security.USER_ID, gID);
					SplendidCache.ClearSubscriptions();
					bSucceeded = true;
				}
			}
			//catch
			{
				// 02/04/2007 Paul.  Don't catch the exception.  
				// It is a web service, so the exception will be handled properly by the AJAX framework. 
			}
			return bSucceeded;
		}
	}
}


