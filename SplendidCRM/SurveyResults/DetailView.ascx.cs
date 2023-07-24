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
using System.Xml;
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.SurveyResults
{
	/// <summary>
	///		Summary OBJECTIVE for DetailView.
	/// </summary>
	public class DetailView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons ctlDynamicButtons;

		protected Guid                     gID              ;
		protected string                   sSURVEY_RESULT_ID;
		protected bool                     bIS_COMPLETE     ;
		protected DateTime                 dtSTART_DATE     ;
		protected DateTime                 dtSUBMIT_DATE    ;
		protected DateTime                 dtDATE_MODIFIED  ;
		protected string                   sIP_ADDRESS      ;
		protected HyperLink                lnkSURVEY        ;
		protected HyperLink                lnkRESPONDANT    ;

		protected string TimeSpent(object oSUBMIT_DATE, object oSTART_DATE)
		{
			string sTimeSpent = String.Empty;
			if ( Sql.ToDateTime(oSUBMIT_DATE) != DateTime.MinValue )
			{
				TimeSpan ts = Sql.ToDateTime(oSUBMIT_DATE) - Sql.ToDateTime(oSTART_DATE);
				sTimeSpent = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
			}
			return sTimeSpent;
		}

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "view") >= 0);
			if ( !this.Visible )
				return;
			
			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				sSURVEY_RESULT_ID = gID.ToString();
				if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL ;
							sSQL = "select *                    " + ControlChars.CrLf
							     + "  from vwSURVEY_RESULTS_Edit" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Security.Filter(cmd, "SurveyResults", "view");
								Sql.AppendParameter(cmd, gID, "SURVEY_RESULT_ID", false);
								
								if ( bDebug )
									RegisterClientScriptBlock("vwSURVEY_RESULTS_Edit", Sql.ClientScriptBlock(cmd));
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dtCurrent = new DataTable() )
									{
										da.Fill(dtCurrent);
										if ( dtCurrent.Rows.Count > 0 )
										{
											DataRow rdr = dtCurrent.Rows[0];
											
											// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
											ctlDynamicButtons.Title = Sql.ToString(rdr["SURVEY_NAME"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
											ViewState["ctlDynamicButtons.Title"] = ctlDynamicButtons.Title;
											
											bIS_COMPLETE    = Sql.ToBoolean (rdr["IS_COMPLETE"  ]);
											dtSTART_DATE    = Sql.ToDateTime(rdr["START_DATE"   ]);
											dtSUBMIT_DATE   = Sql.ToDateTime(rdr["SUBMIT_DATE"  ]);
											dtDATE_MODIFIED = Sql.ToDateTime(rdr["DATE_MODIFIED"]);
											sIP_ADDRESS     = Sql.ToString  (rdr["IP_ADDRESS"   ]);
											lnkSURVEY.NavigateUrl     = "~/Surveys/results.aspx?ID=" + Sql.ToString(rdr["SURVEY_ID"]);
											lnkSURVEY.Text            = Sql.ToString(rdr["SURVEY_NAME"]);
											lnkRESPONDANT.NavigateUrl = "~/" + Sql.ToString(rdr["PARENT_TYPE"]) + "/view.aspx?ID=" + Sql.ToString(rdr["PARENT_ID"]);
											lnkRESPONDANT.Text        = Sql.ToString(rdr["PARENT_NAME"]);
											lnkRESPONDANT.Visible     = !Sql.IsEmptyGuid(rdr["PARENT_ID"]);
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, rdr);
										}
										else
										{
											sSURVEY_RESULT_ID = String.Empty;
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
											ctlDynamicButtons.DisableAll();
											ctlDynamicButtons.ErrorText = L10n.Term("ACL.LBL_NO_ACCESS");
										}
									}
								}
							}
						}
					}
					else
					{
						ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
					}
				}
				else
				{
					ctlDynamicButtons.Title = Sql.ToString(ViewState["ctlDynamicButtons.Title"]);
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
				}
			}
			catch(Exception ex)
			{
				sSURVEY_RESULT_ID = String.Empty;
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
			// 06/11/2013 Paul.  Register all the Survey JavaScript files. 
			SurveyUtil.RegisterScripts(this.Page);
			
			// 06/12/2013 Paul.  The stylesheet needs to be loaded separately. 
			HtmlLink cssSurveyStylesheet = new HtmlLink();
			cssSurveyStylesheet.Attributes.Add("href" , "~/Surveys/stylesheet.aspx");
			cssSurveyStylesheet.Attributes.Add("type" , "text/css"  );
			cssSurveyStylesheet.Attributes.Add("rel"  , "stylesheet");
			Page.Header.Controls.Add(cssSurveyStylesheet);
				
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
			ctlDynamicButtons.Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "SurveyResults";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
				Page.Validators.Add(new RulesValidator(this));
			}
		}
		#endregion
	}
}

