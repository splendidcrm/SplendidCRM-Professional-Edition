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
using System.IO;
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Charts
{
	/// <summary>
	///		Summary description for ImportView.
	/// </summary>
	public class ImportView : SplendidControl
	{
		protected _controls.DynamicButtons ctlDynamicButtons;
		protected _controls.TeamSelect     ctlTeamSelect    ;
		// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
		protected _controls.UserSelect     ctlUserSelect    ;

		protected TextBox                txtNAME                 ;
		protected DropDownList           lstMODULE               ;
		protected DropDownList           lstCHART_TYPE          ;
		protected TextBox                txtASSIGNED_TO          ;
		protected HiddenField            txtASSIGNED_USER_ID     ;
		protected HtmlInputFile          fileIMPORT              ;
		protected RequiredFieldValidator reqNAME                 ;
		protected RequiredFieldValidator reqFILENAME             ;
		// 06/17/2010 Paul.  Manually manage singular Team field. 
		protected TextBox         TEAM_NAME                    ;
		protected HiddenField     TEAM_ID                      ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Import" )
				{
					// 08/19/2010 Paul.  Remove the .rdl extension. 
					txtNAME.Text = txtNAME.Text.Trim();
					if ( txtNAME.Text.ToLower().EndsWith(".rdl") )
						txtNAME.Text = txtNAME.Text.Substring(0, txtNAME.Text.Length - 4);

					reqNAME.Enabled = true;
					reqNAME.Validate();
					reqFILENAME.Enabled = true;
					reqFILENAME.Validate();
					if ( Page.IsValid )
					{
						HttpPostedFile pstIMPORT = fileIMPORT.PostedFile;
						if ( pstIMPORT != null )
						{
							if ( pstIMPORT.FileName.Length > 0 )
							{
								string sFILENAME       = Path.GetFileName (pstIMPORT.FileName);
								string sFILE_EXT       = Path.GetExtension(sFILENAME);
								string sFILE_MIME_TYPE = pstIMPORT.ContentType;
								
								RdlDocument rdl = new RdlDocument();
								rdl.Load(pstIMPORT.InputStream);
								rdl.SetSingleNodeAttribute(rdl.DocumentElement, "Name", txtNAME.Text);
								// 10/22/2007 Paul.  Use the Assigned User ID field when saving the record. 
								Guid gID = Guid.Empty;
								Guid gPRE_LOAD_EVENT_ID  = Guid.Empty;
								Guid gPOST_LOAD_EVENT_ID = Guid.Empty;
								// 05/06/2009 Paul.  Replace existing report by name. 
								// We need to make it easy to update an existing report. 
								DbProviderFactory dbf = DbProviderFactories.GetFactory();
								using ( IDbConnection con = dbf.CreateConnection() )
								{
									con.Open();
									string sSQL;
									// 02/04/2011 Paul.  Needed to include the Load Event IDs in the select list. 
									sSQL = "select ID                " + ControlChars.CrLf
									     + "     , PRE_LOAD_EVENT_ID " + ControlChars.CrLf
									     + "     , POST_LOAD_EVENT_ID" + ControlChars.CrLf
									     + "  from vwCHARTS_List    " + ControlChars.CrLf
									     + " where NAME = @NAME      " + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandText = sSQL;
										Sql.AddParameter(cmd, "@NAME", txtNAME.Text);
										using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
										{
											if ( rdr.Read() )
											{
												gID                 = Sql.ToGuid  (rdr["ID"                ]);
												// 12/04/2010 Paul.  Add support for Business Rules Framework to Reports. 
												gPRE_LOAD_EVENT_ID  = Sql.ToGuid  (rdr["PRE_LOAD_EVENT_ID" ]);
												gPOST_LOAD_EVENT_ID = Sql.ToGuid  (rdr["POST_LOAD_EVENT_ID"]);
											}
										}
									}
								}
								// 06/17/2010 Paul.  Manually manage singular Team field. 
								Guid gTEAM_ID = Guid.Empty;
								if ( SplendidCRM.Crm.Config.enable_dynamic_teams() )
									gTEAM_ID = ctlTeamSelect.TEAM_ID;
								else
									gTEAM_ID = Sql.ToGuid(TEAM_ID.Value);
								// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
								Guid gASSIGNED_USER_ID = Guid.Empty;
								if ( SplendidCRM.Crm.Config.enable_dynamic_assignment() )
									gASSIGNED_USER_ID = ctlUserSelect.USER_ID;
								else
									gASSIGNED_USER_ID = Sql.ToGuid(txtASSIGNED_USER_ID.Value);
								// 12/04/2010 Paul.  Add support for Business Rules Framework to Reports. 
								SqlProcs.spCHARTS_Update
									( ref gID
									, gASSIGNED_USER_ID
									, txtNAME.Text
									, lstMODULE.SelectedValue
									, lstCHART_TYPE.SelectedValue
									, rdl.OuterXml
									, gTEAM_ID
									, ctlTeamSelect.TEAM_SET_LIST
									, gPRE_LOAD_EVENT_ID
									, gPOST_LOAD_EVENT_ID
									// 05/17/2017 Paul.  Add Tags module. 
									, String.Empty  // TAG_SET_NAME
									// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
									, ctlUserSelect.ASSIGNED_SET_LIST
									);
								// 02/11/2010 Paul.  Must clear the available reports, otherwise an imported report will not be available as a Dashlet. 
								SplendidCache.ClearCharts();
								// 04/06/2011 Paul.  Cache reports. 
								SplendidCache.ClearChart(gID);
							}
						}
						Response.Redirect("default.aspx");
					}
				}
				else if ( e.CommandName == "Cancel" )
				{
					Response.Redirect("default.aspx");
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(m_sMODULE + ".LBL_LIST_FORM_TITLE"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "list") >= 0);
			if ( !this.Visible )
				return;
			
			reqNAME.DataBind();
			reqFILENAME.DataBind();
			try
			{
				if ( !IsPostBack )
				{
					txtASSIGNED_TO.Text       = Security.USER_NAME;
					txtASSIGNED_USER_ID.Value = Security.USER_ID.ToString();
					ViewState["ASSIGNED_USER_ID"] = txtASSIGNED_USER_ID.Value;
					// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
					ctlUserSelect.LoadLineItems(Guid.Empty, true);
					// 07/23/2010 Paul.  Make sure to sort the Modules table. 
					DataView vwMODULES = new DataView(SplendidCache.ReportingModules());
					vwMODULES.Sort = "DISPLAY_NAME";
					lstMODULE.DataSource = vwMODULES;
					lstMODULE.DataBind();
					lstCHART_TYPE.DataSource = SplendidCache.List("dom_chart_types");
					lstCHART_TYPE.DataBind();
					try
					{
						// 08/19/2010 Paul.  Check the list before assigning the value. 
						Utils.SetSelectedValue(lstCHART_TYPE, "Freeform");
					}
					catch
					{
					}
					// 06/17/2010 Paul.  Manually manage singular Team field. 
					TEAM_NAME.Text    = Security.TEAM_NAME;
					TEAM_ID.Value     = Security.TEAM_ID.ToString();
					ctlTeamSelect.LoadLineItems(Guid.Empty, true);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
			if ( !IsPostBack )
			{
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
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
			m_sMODULE = "Charts";
			SetMenu(m_sMODULE);
			// 04/29/2008 Paul.  Make use of dynamic buttons. 
			ctlDynamicButtons.AppendButtons(m_sMODULE + ".ImportView", Guid.Empty, Guid.Empty);
		}
		#endregion
	}
}
