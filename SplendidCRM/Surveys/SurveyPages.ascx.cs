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

namespace SplendidCRM.Surveys
{
	/// <summary>
	///		Summary description for SurveyPages.
	/// </summary>
	public class SurveyPages : SubPanelControl
	{
		// 06/03/2015 Paul.  Combine ListHeader and DynamicButtons. 
		protected _controls.SubPanelButtons ctlDynamicButtons;
		protected _controls.SearchView     ctlSearchView     ;
		protected UniqueStringCollection   arrSelectFields   ;
		protected Guid                     gID               ;
		protected DataView                 vwMain            ;
		protected SplendidGrid             grdMain           ;
		protected SplendidGrid             grdPages          ;
		protected HiddenField              txtPAGE_MOVE      ;
		protected HiddenField              txtQUESTION_MOVE  ;
		protected Button                   btnINDEX_MOVE     ;
		protected Button                   btnCreateInline   ;
		protected Panel                    pnlNewRecordInline;
		protected SplendidCRM.SurveyPages.NewRecord ctlNewRecord   ;
		// 11/08/2018 Paul.  Allow questions to be added directly to the survey. 
		protected HtmlInputHidden          txtSURVEY_QUESTION_ID;

		protected void grdMain_OnItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if ( e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem )
			{
				DataView vw = grdMain.DataSource as DataView;
				if ( vw != null && vw.Count > 0 )
				{
					DataGridItem itm = e.Item;
					DataRowView row = itm.DataItem as DataRowView;
					if ( row != null )
					{
						Guid gSURVEY_PAGE_ID = Sql.ToGuid(row["ID"]);
						SplendidGrid grdQuestions = itm.FindControl("grdQuestions") as SplendidGrid;
						if ( grdQuestions != null )
						{
							grdQuestions.L10nTranslate();
							grdQuestions.ItemCreated += new DataGridItemEventHandler(grdQuestions_ItemCreated);
							DbProviderFactory dbf = DbProviderFactories.GetFactory();
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								string sSQL;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									UniqueStringCollection arrQuestionFields = new UniqueStringCollection();
									arrQuestionFields.Add("ID"                     );
									arrQuestionFields.Add("SURVEY_QUESTION_ID"     );
									arrQuestionFields.Add("NAME"                   );
									arrQuestionFields.Add("DESCRIPTION"            );
									arrQuestionFields.Add("QUESTION_NUMBER"        );
									arrQuestionFields.Add("QUESTION_TYPE"          );
									arrQuestionFields.Add("SURVEY_PAGE_ID"         );
									arrQuestionFields.Add("ASSIGNED_USER_ID"       );
									arrQuestionFields.Add("SURVEY_ASSIGNED_USER_ID");
									sSQL = "select " + Sql.FormatSelectFields(arrQuestionFields)
									     + "  from vwSURVEY_PAGES_QUESTIONS" + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									Security.Filter(cmd, "SurveyQuestions", "list");
									Sql.AppendParameter(cmd, gSURVEY_PAGE_ID, "SURVEY_PAGE_ID");
									cmd.CommandText += " order by QUESTION_NUMBER asc";
									try
									{
										using ( DbDataAdapter da = dbf.CreateDataAdapter() )
										{
											((IDbDataAdapter)da).SelectCommand = cmd;
											using ( DataTable dt = new DataTable() )
											{
												da.Fill(dt);
												//this.ApplyGridViewRules("SurveyPages.SurveyQuestions", dt);
												DataView vwQuestions = dt.DefaultView;
												grdQuestions.DataSource = vwQuestions;
												grdQuestions.DataBind();
												if ( vwQuestions.Count == 0 )
													grdQuestions.Visible = false;
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
					}
				}
			}
		}

		protected void grdQuestions_ItemCreated(object sender, DataGridItemEventArgs e)
		{
			if ( e.Item.ItemType == ListItemType.Header || e.Item.ItemType == ListItemType.Footer )
			{
				e.Item.CssClass += " nodrag nodrop";
			}
			else if ( e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem )
			{
				e.Item.CssClass += " nodrag nodrop";
			}
		}

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
					//e.Item.CssClass += " nodrag nodrop";
				}
			}
		}

		protected void txtPAGE_MOVE_ValueChanged(object sender, EventArgs e)
		{
			try
			{
				string[] arrValueChanged = txtPAGE_MOVE.Value.Split(',');
				if ( arrValueChanged.Length < 2 )
					throw(new Exception("Invalid changed values: " + txtPAGE_MOVE.Value));
				
				txtPAGE_MOVE.Value = String.Empty;
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
				
				int nOLD_INDEX = Sql.ToInteger(vwMain[nOLD_VALUE]["PAGE_NUMBER"]);
				int nNEW_INDEX = Sql.ToInteger(vwMain[nNEW_VALUE]["PAGE_NUMBER"]);
				SqlProcs.spSURVEYS_MovePage(gID, nOLD_INDEX, nNEW_INDEX);
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

		protected void txtQUESTION_MOVE_ValueChanged(object sender, EventArgs e)
		{
			try
			{
				string[] arrValueChanged = txtQUESTION_MOVE.Value.Split(',');
				if ( arrValueChanged.Length < 3 )
					throw(new Exception("Invalid changed values: " + txtQUESTION_MOVE.Value));
				
				Guid gSURVEY_QUESTION_ID = Sql.ToGuid(arrValueChanged[0]);
				Guid gOLD_PAGE_ID        = Sql.ToGuid(arrValueChanged[1]);
				Guid gNEW_PAGE_ID        = Sql.ToGuid(arrValueChanged[2]);
				if ( gOLD_PAGE_ID != gNEW_PAGE_ID )
					SqlProcs.spSURVEY_PAGES_QUESTIONS_Page(gSURVEY_QUESTION_ID, gOLD_PAGE_ID, gNEW_PAGE_ID);
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
				Guid gSURVEY_PAGE_ID = Sql.ToGuid(e.CommandArgument);
				switch ( e.CommandName )
				{
					/*
					case "SurveyPages.MoveUp":
					{
						if ( Sql.IsEmptyGuid(gSURVEY_PAGE_ID) )
							throw(new Exception("Unspecified argument"));
						SqlProcs.spSURVEY_PAGES_MoveUp(gSURVEY_PAGE_ID);
						break;
					}
					case "SurveyPages.MoveDown":
					{
						if ( Sql.IsEmptyGuid(gSURVEY_PAGE_ID) )
							throw(new Exception("Unspecified argument"));
						SqlProcs.spSURVEY_PAGES_Down(gSURVEY_PAGE_ID);
						break;
					}
					*/
					case "SurveyPages.Delete":
					{
						if ( bEditView )
						{
							this.DeleteEditViewRelationship(gSURVEY_PAGE_ID);
						}
						else
						{
							SqlProcs.spSURVEY_PAGES_Delete(gSURVEY_PAGE_ID);
						}
						BindGrid(true);
						break;
					}
					case "SurveyPages.View":
					{
						Response.Redirect("~/SurveyPages/view.aspx?ID=" + gSURVEY_PAGE_ID.ToString() + "&SURVEY_ID=" + gID.ToString());
						break;
					}
					case "SurveyPages.Edit":
					{
						Response.Redirect("~/SurveyPages/edit.aspx?ID=" + gSURVEY_PAGE_ID.ToString());
						break;
					}
					case "SurveyPages.Create":
						if ( this.IsMobile || Sql.ToBoolean(Application["CONFIG.disable_editview_inline"]) )
							Response.Redirect("~/SurveyPages/edit.aspx?SURVEY_ID=" + gID.ToString());
						else
						{
							pnlNewRecordInline.Style.Add(HtmlTextWriterStyle.Display, "inline");
							ctlDynamicButtons.HideAll();
						}
						break;
					case "NewRecord.Cancel":
						pnlNewRecordInline.Style.Add(HtmlTextWriterStyle.Display, "none");
						ctlDynamicButtons.ShowAll();
						break;
					case "NewRecord.FullForm":
						Response.Redirect("~/SurveyPages/edit.aspx?SURVEY_ID=" + gID.ToString());
						break;
					case "NewRecord":
						Response.Redirect(Request.RawUrl);
						break;
					case "SurveyPages.Search":
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
						arrSelectFields.Remove("SURVEY_PAGE_ID"  );
						arrSelectFields.Remove("SURVEY_PAGE_NAME");
						// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
						m_sVIEW_NAME = "vwSURVEY_PAGES";
						sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
						     + "     , ID   as SURVEY_PAGE_ID  " + ControlChars.CrLf
						     + "     , NAME as SURVEY_PAGE_NAME" + ControlChars.CrLf
						     + Sql.AppendRecordLevelSecurityField(m_sMODULE, "edit", m_sVIEW_NAME)
						     + "  from " + m_sVIEW_NAME + ControlChars.CrLf;
						cmd.CommandText = sSQL;
						Security.Filter(cmd, m_sMODULE, "list");
						Sql.AppendParameter(cmd, arrUPDATED.ToArray(), "ID");
					}
					else
					{
						// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
						m_sVIEW_NAME = "vwSURVEYS_SURVEY_PAGES";
						sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
						     + Sql.AppendRecordLevelSecurityField(m_sMODULE, "edit", m_sVIEW_NAME)
						     + "  from " + m_sVIEW_NAME + ControlChars.CrLf;
						cmd.CommandText = sSQL;
						Security.Filter(cmd, m_sMODULE, "list");
						Sql.AppendParameter(cmd, gID, "SURVEY_ID");
					}
					ctlSearchView.SqlSearchClause(cmd);
					cmd.CommandText += " order by PAGE_NUMBER asc";

					if ( bDebug )
						RegisterClientScriptBlock("vwSURVEYS_SURVEY_PAGES", Sql.ClientScriptBlock(cmd));

					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								this.ApplyGridViewRules("Surveys." + m_sMODULE, dt);
								vwMain = dt.DefaultView;
								grdMain.DataSource = vwMain ;
								grdPages.DataSource = vwMain ;
								if ( bBind )
								{
									grdMain.DataBind();
									grdPages.DataBind();
									if ( bEditView && !IsPostBack )
									{
										this.CreateEditViewRelationships(dt, "SURVEY_PAGE_ID");
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
					SqlProcs.spSURVEY_PAGES_Delete(gDELETE_ID, trn);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			gID = Sql.ToGuid(Request["ID"]);
			// 11/08/2018 Paul.  Allow questions to be added directly to the survey. 
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
									string sSQL;
									sSQL = "select ID       " + ControlChars.CrLf
									     + "  from vwSURVEYS" + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandText = sSQL;
										Security.Filter(cmd, "Surveys", "list");
										Sql.AppendParameter(cmd, gID, "ID");
										Guid gSURVEY_ID = Sql.ToGuid(cmd.ExecuteScalar());
										if ( !Sql.IsEmptyGuid(gSURVEY_ID ) )
										{
											cmd.Parameters.Clear();
											sSQL = "select SURVEY_PAGE_ID        " + ControlChars.CrLf
											     + "  from vwSURVEYS_SURVEY_PAGES" + ControlChars.CrLf;
											cmd.CommandText = sSQL;
											Security.Filter(cmd, m_sMODULE, "list");
											Sql.AppendParameter(cmd, gSURVEY_ID, "SURVEY_ID");
											ctlSearchView.SqlSearchClause(cmd);
											// 11/08/2018 Paul.  Add questions to the last page. 
											cmd.CommandText += " order by PAGE_NUMBER desc";
											Guid gSURVEY_PAGE_ID = Guid.Empty;
											using ( IDataReader rdr = cmd.ExecuteReader() )
											{
												if ( rdr.Read() )
												{
													gSURVEY_PAGE_ID = Sql.ToGuid(rdr["SURVEY_PAGE_ID"]);
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
				ctlDynamicButtons.AppendButtons("Surveys." + m_sMODULE, gASSIGNED_USER_ID, gID);
				ctlNewRecord.SURVEY_ID = gID;
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
			ctlDynamicButtons.Command     += new CommandEventHandler(Page_Command);
			ctlNewRecord.Command          += new CommandEventHandler(Page_Command);
			ctlSearchView.Command         += new CommandEventHandler(Page_Command);
			grdMain.ItemCreated           += new DataGridItemEventHandler(grdMain_ItemCreated);
			grdMain.ItemDataBound         += new DataGridItemEventHandler(grdMain_OnItemDataBound);
			txtPAGE_MOVE.ValueChanged     += new EventHandler(txtPAGE_MOVE_ValueChanged);
			txtQUESTION_MOVE.ValueChanged += new EventHandler(txtQUESTION_MOVE_ValueChanged);
			m_sMODULE = "SurveyPages";
			arrSelectFields = new UniqueStringCollection();
			arrSelectFields.Add("ID"              );
			arrSelectFields.Add("NAME"            );
			arrSelectFields.Add("SURVEY_PAGE_ID"  );
			arrSelectFields.Add("ASSIGNED_USER_ID");
			arrSelectFields.Add("PAGE_NUMBER"     );
			//// 06/07/2015 Paul.  Must include Page_Command in order for Preview to fire. 
			this.AppendGridColumns(grdMain, "Surveys." + m_sMODULE, arrSelectFields, Page_Command);
			if ( IsPostBack )
				ctlDynamicButtons.AppendButtons("Surveys." + m_sMODULE, Guid.Empty, Guid.Empty);
		}
		#endregion
	}
}

