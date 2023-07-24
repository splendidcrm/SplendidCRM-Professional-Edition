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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.SurveyPages
{
	/// <summary>
	///		Summary description for SurveyQuestions.
	/// </summary>
	public class SurveyQuestions : SubPanelControl
	{
		// 06/03/2015 Paul.  Combine ListHeader and DynamicButtons. 
		protected _controls.SubPanelButtons ctlDynamicButtons;
		protected _controls.SearchView     ctlSearchView        ;
		protected UniqueStringCollection   arrSelectFields      ;
		protected Guid                     gID                  ;
		protected DataView                 vwMain               ;
		protected SplendidGrid             grdMain              ;
		protected HiddenField              txtINDEX             ;
		protected Button                   btnINDEX_MOVE        ;
		protected HtmlInputHidden          txtSURVEY_QUESTION_ID;
		protected Button                   btnCreateInline      ;
		protected Panel                    pnlNewRecordInline   ;
		protected SplendidCRM.SurveyQuestions.NewRecord ctlNewRecord   ;

		protected void grdMain_ItemCreated(object sender, DataGridItemEventArgs e)
		{
			if ( e.Item.ItemType == ListItemType.Header || e.Item.ItemType == ListItemType.Footer )
			{
				e.Item.CssClass += " nodrag nodrop";
			}
			else if ( e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem )
			{
				DataRowView row = e.Item.DataItem as DataRowView;
				if ( row != null )
				{
					if ( !(SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit") >= 0) )
						e.Item.CssClass += " nodrag nodrop";
				}
			}
		}

		protected void txtINDEX_ValueChanged(object sender, EventArgs e)
		{
			try
			{
				string[] arrValueChanged = txtINDEX.Value.Split(',');
				if ( arrValueChanged.Length < 2 )
					throw(new Exception("Invalid changed values: " + txtINDEX.Value));
				
				txtINDEX.Value = String.Empty;
				int nOLD_VALUE = Sql.ToInteger(arrValueChanged[0]);
				int nNEW_VALUE = Sql.ToInteger(arrValueChanged[1]);
				if ( nOLD_VALUE < 0 )
					throw(new Exception("OldIndex cannot be negative."));
				if ( nNEW_VALUE < 0 )
					throw(new Exception("NewIndex cannot be negative."));
				if ( nOLD_VALUE >= vwMain.Count )
					throw(new Exception("OldIndex cannot exceed " + vwMain.Count.ToString()));
				if ( nNEW_VALUE >= vwMain.Count )
					throw(new Exception("NewIndex cannot exceed " + vwMain.Count.ToString()));
				
				int nOLD_INDEX = Sql.ToInteger(vwMain[nOLD_VALUE]["QUESTION_NUMBER"]);
				int nNEW_INDEX = Sql.ToInteger(vwMain[nNEW_VALUE]["QUESTION_NUMBER"]);
				SqlProcs.spSURVEY_PAGES_MoveQuestion(gID, nOLD_INDEX, nNEW_INDEX);
				BindGrid(true);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
#if DEBUG
				ctlDynamicButtons.ErrorText += ex.StackTrace;
#endif
			}
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				switch ( e.CommandName )
				{
					/*
					case "SurveyQuestions.MoveUp":
					{
						if ( Sql.IsEmptyGuid(gSURVEY_PAGE_ID) )
							throw(new Exception("Unspecified argument"));
						SqlProcs.spSURVEY_PAGES_QUESTIONS_Up(gSURVEY_PAGE_ID);
						break;
					}
					case "SurveyQuestions.MoveDown":
					{
						if ( Sql.IsEmptyGuid(gSURVEY_PAGE_ID) )
							throw(new Exception("Unspecified argument"));
						SqlProcs.spSURVEY_PAGES_QUESTIONS_Down(gSURVEY_PAGE_ID);
						break;
					}
					*/
					case "SurveyQuestions.Edit":
					{
						Guid gSURVEY_QUESTION_ID = Sql.ToGuid(e.CommandArgument);
						Response.Redirect("~/SurveyQuestions/edit.aspx?ID=" + gSURVEY_QUESTION_ID.ToString() + "&SURVEY_PAGE_ID=" + gID.ToString());
						break;
					}
					case "SurveyQuestions.Remove":
					{
						Guid gSURVEY_QUESTION_ID = Sql.ToGuid(e.CommandArgument);
						if ( bEditView )
						{
							this.DeleteEditViewRelationship(gSURVEY_QUESTION_ID);
						}
						else
						{
							SqlProcs.spSURVEY_PAGES_QUESTIONS_Delete(gID, gSURVEY_QUESTION_ID);
						}
						BindGrid(true);
						break;
					}
					case "SurveyQuestions.Create":
						// 06/01/2013 Paul.  We are not going to allow inline creation at this time. 
						//if ( this.IsMobile || Sql.ToBoolean(Application["CONFIG.disable_editview_inline"]) )
							Response.Redirect("~/" + m_sMODULE + "/edit.aspx?SURVEY_PAGE_ID=" + gID.ToString());
						//else
						//{
						//	pnlNewRecordInline.Style.Add(HtmlTextWriterStyle.Display, "inline");
						//	ctlDynamicButtons.HideAll();
						//}
						break;
					case "NewRecord.Cancel":
						pnlNewRecordInline.Style.Add(HtmlTextWriterStyle.Display, "none");
						ctlDynamicButtons.ShowAll();
						break;
					case "NewRecord.FullForm":
						Response.Redirect("~/" + m_sMODULE + "/edit.aspx?PARENT_ID=" + gID.ToString());
						break;
					case "NewRecord":
						Response.Redirect(Request.RawUrl);
						break;
					case "SurveyQuestions.Search":
						ctlSearchView.Visible = !ctlSearchView.Visible;
						break;
					case "Search":
						break;
					case "Clear":
						BindGrid(true);
						break;
					case "SortGrid":
						break;
					// 06/07/2015 Paul.  Add support for Preview button. 
					case "Preview":
						if ( Page.Master is SplendidMaster )
						{
							CommandEventArgs ePreview = new CommandEventArgs(e.CommandName, new PreviewData(m_sMODULE, Sql.ToGuid(e.CommandArgument)));
							(Page.Master as SplendidMaster).Page_Command(sender, ePreview);
						}
						break;
					default:
						throw(new Exception("Unknown command: " + e.CommandName));
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
		}

		protected void BindGrid(bool bBind)
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					UniqueGuidCollection arrUPDATED = this.GetUpdatedEditViewRelationships();
					if ( bEditView && IsPostBack && arrUPDATED.Count > 0 )
					{
						arrSelectFields.Remove("SURVEY_QUESTION_ID"     );
						arrSelectFields.Remove("SURVEY_QUESTION_NAME"   );
						arrSelectFields.Remove("SURVEY_PAGE_ID"         );
						arrSelectFields.Remove("SURVEY_PAGE_NAME"       );
						arrSelectFields.Remove("SURVEY_ASSIGNED_USER_ID");
						sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
						     + "     , ID                       as SURVEY_QUESTION_ID     " + ControlChars.CrLf
						     + "     , NAME                     as SURVEY_QUESTION_NAME   " + ControlChars.CrLf
						     + "     , @SURVEY_PAGE_ID          as SURVEY_PAGE_ID         " + ControlChars.CrLf
						     + "     , @SURVEY_PAGE_NAME        as SURVEY_PAGE_NAME       " + ControlChars.CrLf
						     + "     , @SURVEY_ASSIGNED_USER_ID as SURVEY_ASSIGNED_USER_ID" + ControlChars.CrLf
						     + "  from vwSURVEY_QUESTIONS" + ControlChars.CrLf;
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@SURVEY_PAGE_ID"         , gID);
						Sql.AddParameter(cmd, "@SURVEY_PAGE_NAME"       , Sql.ToString(Page.Items["NAME"            ]));
						Sql.AddParameter(cmd, "@SURVEY_ASSIGNED_USER_ID", Sql.ToGuid  (Page.Items["ASSIGNED_USER_ID"]));
						Security.Filter(cmd, m_sMODULE, "list");
						Sql.AppendParameter(cmd, arrUPDATED.ToArray(), "ID");
					}
					else
					{
						sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
						     + "  from vwSURVEY_PAGES_QUESTIONS" + ControlChars.CrLf;
						cmd.CommandText = sSQL;
						Security.Filter(cmd, m_sMODULE, "list");
						Sql.AppendParameter(cmd, gID, "SURVEY_PAGE_ID");
					}
					ctlSearchView.SqlSearchClause(cmd);
					cmd.CommandText += grdMain.OrderByClause("QUESTION_NUMBER", "asc");

					if ( bDebug )
						RegisterClientScriptBlock("vwSURVEY_PAGES_QUESTIONS", Sql.ClientScriptBlock(cmd));

					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								this.ApplyGridViewRules("SurveyPages." + m_sMODULE, dt);
								vwMain = dt.DefaultView;
								grdMain.DataSource = vwMain ;
								if ( bBind )
								{
									grdMain.DataBind();
									if ( bEditView && !IsPostBack )
									{
										this.CreateEditViewRelationships(dt, "SURVEY_QUESTION_ID");
									}
								}
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						ctlDynamicButtons.ErrorText = ex.Message;
					}
				}
			}
		}

		public override void Save(Guid gPARENT_ID, string sPARENT_TYPE, IDbTransaction trn)
		{
			UniqueGuidCollection arrDELETED = this.GetDeletedEditViewRelationships();
			foreach ( Guid gDELETE_ID in arrDELETED )
			{
				if ( !Sql.IsEmptyGuid(gDELETE_ID) )
					SqlProcs.spSURVEY_PAGES_QUESTIONS_Delete(gPARENT_ID, gDELETE_ID, trn);
			}

			UniqueGuidCollection arrUPDATED = this.GetUpdatedEditViewRelationships();
			foreach ( Guid gUPDATE_ID in arrUPDATED )
			{
				if ( !Sql.IsEmptyGuid(gUPDATE_ID) )
					SqlProcs.spSURVEY_PAGES_QUESTIONS_Update(gPARENT_ID, gUPDATE_ID, 0, trn);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			gID = Sql.ToGuid(Request["ID"]);
			if ( !Sql.IsEmptyString(txtSURVEY_QUESTION_ID.Value) )
			{
				try
				{
					string[] arrID = txtSURVEY_QUESTION_ID.Value.Split(',');
					if ( arrID != null )
					{
						if ( bEditView )
						{
							this.UpdateEditViewRelationship(arrID);
						}
						else
						{
							System.Collections.Stack stk = Utils.FilterByACL_Stack(m_sMODULE, "list", arrID, "SURVEY_QUESTIONS");
							if ( stk.Count > 0 )
							{
								DbProviderFactory dbf = DbProviderFactories.GetFactory();
								using ( IDbConnection con = dbf.CreateConnection() )
								{
									con.Open();
									using ( IDbTransaction trn = Sql.BeginTransaction(con) )
									{
										try
										{
											while ( stk.Count > 0 )
											{
												string sIDs = Utils.BuildMassIDs(stk);
												SqlProcs.spSURVEY_PAGES_QUESTIONS_MassUpdate(sIDs, gID, trn);
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
								txtSURVEY_QUESTION_ID.Value = String.Empty;
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
				}
			}
			try
			{
				// 11/09/2018 Paul.  Register all the Survey JavaScript files. 
				SurveyUtil.RegisterScripts(this.Page);
				
				Guid gSURVEY_THEME_ID = Sql.ToGuid(Application["CONFIG.Surveys.DefaultTheme"]);
				// 11/09/2018 Paul.  The stylesheet needs to be loaded separately. 
				HtmlLink cssSurveyStylesheet = new HtmlLink();
				cssSurveyStylesheet.Attributes.Add("id"   , gSURVEY_THEME_ID.ToString().Replace("-", "_"));
				cssSurveyStylesheet.Attributes.Add("href" , "~/Surveys/stylesheet.aspx?ID=" + gSURVEY_THEME_ID.ToString());
				cssSurveyStylesheet.Attributes.Add("type" , "text/css"  );
				cssSurveyStylesheet.Attributes.Add("rel"  , "stylesheet");
				Page.Header.Controls.Add(cssSurveyStylesheet);
				
				ScriptManager mgrAjax = ScriptManager.GetCurrent(this.Page);
				// 08/25/2013 Paul.  jQuery now registered in the master pages. 
				//ScriptReference  scrJQuery         = new ScriptReference ("~/Include/javascript/jquery-1.4.2.min.js"   );
				ScriptReference  scrJQueryTableDnD = new ScriptReference ("~/Include/javascript/jquery.tablednd_0_5.js");
				//if ( !mgrAjax.Scripts.Contains(scrJQuery) )
				//	mgrAjax.Scripts.Add(scrJQuery);
				if ( !mgrAjax.Scripts.Contains(scrJQueryTableDnD) )
					mgrAjax.Scripts.Add(scrJQueryTableDnD);
				BindGrid(true);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}

			if ( !IsPostBack )
			{
				Guid gASSIGNED_USER_ID = Sql.ToGuid(Page.Items["ASSIGNED_USER_ID"]);
				ctlDynamicButtons.AppendButtons("SurveyPages." + m_sMODULE, gASSIGNED_USER_ID, gID);
				ctlNewRecord.SURVEY_PAGE_ID = gID;
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
			ctlDynamicButtons.Command += new CommandEventHandler(Page_Command);
			ctlNewRecord.Command      += new CommandEventHandler(Page_Command);
			ctlSearchView.Command     += new CommandEventHandler(Page_Command);
			grdMain.ItemCreated       += new DataGridItemEventHandler(grdMain_ItemCreated);
			txtINDEX.ValueChanged     += new EventHandler(txtINDEX_ValueChanged);
			m_sMODULE = "SurveyQuestions";
			arrSelectFields = new UniqueStringCollection();
			arrSelectFields.Add("SURVEY_QUESTION_ID"     );
			arrSelectFields.Add("ASSIGNED_USER_ID"       );
			arrSelectFields.Add("SURVEY_ASSIGNED_USER_ID");
			arrSelectFields.Add("QUESTION_NUMBER"        );
			// 06/07/2015 Paul.  Must include Page_Command in order for Preview to fire. 
			this.AppendGridColumns(grdMain, "SurveyPages." + m_sMODULE, arrSelectFields, Page_Command);
			if ( IsPostBack )
				ctlDynamicButtons.AppendButtons("SurveyPages." + m_sMODULE, Guid.Empty, Guid.Empty);
		}
		#endregion
	}
}

