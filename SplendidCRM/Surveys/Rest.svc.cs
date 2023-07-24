/*
 * Copyright (C) 2021 SplendidCRM Software, Inc. All Rights Reserved. 
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
 */
using System;
using System.IO;
using System.Xml;
using System.Web;
using System.Web.SessionState;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using System.Threading;
using System.Diagnostics;

namespace SplendidCRM.Surveys
{
	[ServiceContract]
	[ServiceBehavior( IncludeExceptionDetailInFaults = true )]
	[AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Required )]
	public class Rest
	{
		[OperationContract]
		public void AddQuestions(Stream input)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			
			string sRequest = String.Empty;
			using ( StreamReader stmRequest = new StreamReader(input, System.Text.Encoding.UTF8) )
			{
				sRequest = stmRequest.ReadToEnd();
			}
			// http://weblogs.asp.net/hajan/archive/2010/07/23/javascriptserializer-dictionary-to-json-serialization-and-deserialization.aspx
			JavaScriptSerializer json = new JavaScriptSerializer();
			json.MaxJsonLength = int.MaxValue;
			Dictionary<string, object> dict = json.Deserialize<Dictionary<string, object>>(sRequest);

			L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
			string sModuleName = "Surveys";
			if ( Sql.IsEmptyString(sModuleName) )
				throw(new Exception("The module name must be specified."));
			int nACLACCESS = Security.GetUserAccess(sModuleName, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + sModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + sModuleName));
			}
			SplendidSession.CreateSession(HttpContext.Current.Session);
			
			string sTableName = Sql.ToString(Application["Modules." + sModuleName + ".TableName"]);
			if ( Sql.IsEmptyString(sTableName) )
				throw(new Exception("Unknown module: " + sModuleName));
			
			Guid         gSURVEY_ID      = Guid.Empty;
			Guid         gSURVEY_PAGE_ID = Guid.Empty;
			List<string> arrID_LIST      = new List<string>();
			foreach ( string sColumnName in dict.Keys )
			{
				switch ( sColumnName )
				{
					case "SURVEY_ID"        :  gSURVEY_ID           = Sql.ToGuid   (dict[sColumnName]);  break;
					case "SURVEY_PAGE_ID"   :  gSURVEY_PAGE_ID      = Sql.ToGuid   (dict[sColumnName]);  break;
					case "ID_LIST":
					{
						if ( dict[sColumnName] is System.Collections.ArrayList )
						{
							System.Collections.ArrayList lst = dict[sColumnName] as System.Collections.ArrayList;
							if ( lst.Count > 0 )
							{
								foreach ( string item in lst )
								{
									arrID_LIST.Add(item);
								}
							}
						}
						break;
					}
				}
			}
			string m_sMODULE = "SurveyPages";
			System.Collections.Stack stk = Utils.FilterByACL_Stack(m_sMODULE, "list", arrID_LIST.ToArray(), "SURVEY_QUESTIONS");
			if ( stk.Count > 0 )
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					sSQL = "select ID       " + ControlChars.CrLf
					     + "  from vwSURVEYS" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Security.Filter(cmd, "Surveys", "list");
						Sql.AppendParameter(cmd, gSURVEY_ID, "ID");
						gSURVEY_ID = Sql.ToGuid(cmd.ExecuteScalar());
						if ( !Sql.IsEmptyGuid(gSURVEY_ID ) )
						{
							cmd.Parameters.Clear();
							if ( Sql.IsEmptyGuid(gSURVEY_PAGE_ID) )
							{
								sSQL = "select SURVEY_PAGE_ID        " + ControlChars.CrLf
								     + "  from vwSURVEYS_SURVEY_PAGES" + ControlChars.CrLf;
								cmd.CommandText = sSQL;
								Security.Filter(cmd, m_sMODULE, "list");
								Sql.AppendParameter(cmd, gSURVEY_ID, "SURVEY_ID");
								// 11/08/2018 Paul.  Add questions to the last page. 
								cmd.CommandText += " order by PAGE_NUMBER desc";
								using ( IDataReader rdr = cmd.ExecuteReader() )
								{
									if ( rdr.Read() )
									{
										gSURVEY_PAGE_ID = Sql.ToGuid(rdr["SURVEY_PAGE_ID"]);
									}
								}
							}
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								try
								{
									// 11/08/2018 Paul.  If no pages exist, then create a page. 
									if ( Sql.IsEmptyGuid(gSURVEY_PAGE_ID) )
									{
										SqlProcs.spSURVEY_PAGES_Update
											( ref gSURVEY_PAGE_ID
											, gSURVEY_ID
											, String.Empty // NAME
											, String.Empty // QUESTION_RANDOMIZATION
											, String.Empty // DESCRIPTION
											, trn
											);
									}
									while ( stk.Count > 0 )
									{
										string sIDs = Utils.BuildMassIDs(stk);
										SqlProcs.spSURVEY_PAGES_QUESTIONS_MassUpdate(sIDs, gSURVEY_PAGE_ID, trn);
									}
									trn.Commit();
								}
								catch(Exception ex)
								{
									trn.Rollback();
									throw(new Exception(ex.Message, ex.InnerException));
								}
							}
						}
					}
				}
			}
		}
	}
}
