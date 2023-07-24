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
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Leads
{
	public partial class WebToLeadCapture : SplendidCRM.SplendidPage
	{
		override protected bool AuthenticationRequired()
		{
			return false;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
			
			string sRedirect = Sql.ToString(Request["Redirect"]);
			if ( Request.QueryString.Count == 0 && Request.Form.Count == 0 )
			{
				Response.Write("Missing data.");
				if ( !Sql.IsEmptyString(sRedirect) )
				{
					sRedirect += (sRedirect.Contains("?") ? "&" : "?") + "Error=Missing data.";
					Response.Redirect(sRedirect);
				}
				return;
			}
			// 08/03/2012 Paul.  Provide a way to disable Web Capture. 
			if ( Sql.ToBoolean(Application["WebToLeadCapture.Disabled"]) )
			{
				Response.Write("Web Capture has been disabled.");
				if ( !Sql.IsEmptyString(sRedirect) )
				{
					sRedirect += (sRedirect.Contains("?") ? "&" : "?") + "Error=Web Capture has been disabled.";
					Response.Redirect(sRedirect);
				}
				return;
			}

			try
			{
				string sCUSTOM_MODULE = "LEADS";
				DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sCUSTOM_MODULE);
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
					using ( IDbTransaction trn = Sql.BeginTransaction(con) )
					{
						try
						{
							Guid gID = Guid.Empty;
							// 04/26/2012 Paul.  Add ASSISTANT, ASSISTANT_PHONE, BIRTHDATE, WEBSITE. 
							SqlProcs.spLEADS_Update
								( ref gID
								, Sql.ToGuid   (Request["ASSIGNED_USER_ID"          ])
								, Sql.ToString (Request["SALUTATION"                ])
								, Sql.ToString (Request["FIRST_NAME"                ])
								, Sql.ToString (Request["LAST_NAME"                 ])
								, Sql.ToString (Request["TITLE"                     ])
								, Sql.ToString (Request["REFERED_BY"                ])
								, Sql.ToString (Request["LEAD_SOURCE"               ])
								, Sql.ToString (Request["LEAD_SOURCE_DESCRIPTION"   ])
								, Sql.ToString (Request["STATUS"                    ])
								, Sql.ToString (Request["STATUS_DESCRIPTION"        ])
								, Sql.ToString (Request["DEPARTMENT"                ])
								, Guid.Empty  // 06/24/2005. REPORTS_TO_ID is not used in version 3.0. 
								, Sql.ToBoolean(Request["DO_NOT_CALL"               ])
								, Sql.ToString (Request["PHONE_HOME"                ])
								, Sql.ToString (Request["PHONE_MOBILE"              ])
								, Sql.ToString (Request["PHONE_WORK"                ])
								, Sql.ToString (Request["PHONE_OTHER"               ])
								, Sql.ToString (Request["PHONE_FAX"                 ])
								, Sql.ToString (Request["EMAIL1"                    ])
								, Sql.ToString (Request["EMAIL2"                    ])
								, Sql.ToBoolean(Request["EMAIL_OPT_OUT"             ])
								, Sql.ToBoolean(Request["INVALID_EMAIL"             ])
								, Sql.ToString (Request["PRIMARY_ADDRESS_STREET"    ])
								, Sql.ToString (Request["PRIMARY_ADDRESS_CITY"      ])
								, Sql.ToString (Request["PRIMARY_ADDRESS_STATE"     ])
								, Sql.ToString (Request["PRIMARY_ADDRESS_POSTALCODE"])
								, Sql.ToString (Request["PRIMARY_ADDRESS_COUNTRY"   ])
								, Sql.ToString (Request["ALT_ADDRESS_STREET"        ])
								, Sql.ToString (Request["ALT_ADDRESS_CITY"          ])
								, Sql.ToString (Request["ALT_ADDRESS_STATE"         ])
								, Sql.ToString (Request["ALT_ADDRESS_POSTALCODE"    ])
								, Sql.ToString (Request["ALT_ADDRESS_COUNTRY"       ])
								, Sql.ToString (Request["DESCRIPTION"               ])
								, Sql.ToString (Request["ACCOUNT_NAME"              ])
								, Sql.ToGuid   (Request["CAMPAIGN_ID"               ])
								, Sql.ToGuid   (Request["TEAM_ID"                   ])
								, String.Empty // 09/13/2009 Paul.  It does not seem practical to allow TEAM_SET_LIST as a parameter. 
								, Guid.Empty   // 02/20/2010 Paul.  It does not make sense to support CONTACT_ID. 
								, Guid.Empty   // 02/20/2010 Paul.  It does not make sense to support ACCOUNT_ID. 
								, false        // 04/08/2010 Paul.  EXCHANGE_FOLDER is not supported in this context. 
								, Sql.ToDateTime(Request["BIRTHDATE"                ])
								, Sql.ToString  (Request["ASSISTANT"                ])
								, Sql.ToString  (Request["ASSISTANT_PHONE"          ])
								, Sql.ToString  (Request["WEBSITE"                  ])
								// 09/27/2013 Paul.  SMS messages need to be opt-in. 
								, Sql.ToString  (Request["SMS_OPT_IN"               ])
								// 10/22/2013 Paul.  Provide a way to map Tweets to a parent. 
								, Sql.ToString  (Request["TWITTER_SCREEN_NAME"      ])
								// 10/29/2015 Paul.  Add picture. 
								, String.Empty  // PICTURE
								// 05/12/2016 Paul.  Add Tags module. 
								, String.Empty  // TAG_SET_NAME
								// 06/20/2017 Paul.  Add number fields to Contacts, Leads, Prospects, Opportunities and Campaigns. 
								, String.Empty  // LEAD_NUMBER
								// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
								, String.Empty  // ASSIGNED_SET_LIST
								// 06/23/2018 Paul.  Add DP_BUSINESS_PURPOSE and DP_CONSENT_LAST_UPDATED for data privacy. 
								, String.Empty       // DP_BUSINESS_PURPOSE
								, DateTime.MinValue  // DP_CONSENT_LAST_UPDATED
								, trn
								);
							SplendidDynamic.UpdateCustomFields(this, trn, gID, sCUSTOM_MODULE, dtCustomFields);
							trn.Commit();
						}
						catch(Exception ex)
						{
							trn.Rollback();
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
							Response.Write(ex.Message);
							return;
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				Response.Write(ex.Message);
				return;
			}
			Response.Write("Thank you.");
			if ( !Sql.IsEmptyString(sRedirect) )
			{
				Response.Redirect(sRedirect);
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
