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
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using SplendidCRM._controls;

namespace SplendidCRM.EmailClient.Exchange
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		// 03/16/2016 Paul.  Combine ModuleHeader and DynamicButtons.
		protected _controls.HeaderButtons  ctlDynamicButtons;
		protected EmailClient.ImportView   ctlImportView    ;
		protected EmailClient.DetailView   ctlDetailView    ;
		protected EmailClient.EditView     ctlEditView      ;

		protected Table              tblEmailView        ;
		protected HtmlGenericControl divMain             ;
		protected DataTable          dtMain              ;
		protected SplendidGrid       grdMain             ;
		protected TreeView           treeMain            ;
		protected XmlDataSource      xdsFolders          ;
		protected HiddenField        hidSelectedUniqueID ;
		protected Label              lblFolderTitle      ;
		protected HiddenField        hidQuickCreateModule;
		protected PlaceHolder        plcQuickCreate      ;

		protected void QuickCreate_Load(Object sender, EventArgs e)
		{
			if ( !Sql.IsEmptyString(hidSelectedUniqueID.Value) && plcQuickCreate.Controls.Count > 0 )
			{
				string sUNIQUE_ID = hidSelectedUniqueID.Value;
				// 07/17/2010 Paul.  Method name should be singular as only one message is being retrieved. 
				// 11/23/2011 Paul.  Add MAIL_SMTPUSER and MAIL_SMTPPASS so that we can avoid impersonation. 
				ExchangeSync.UserSync User = new ExchangeSync.UserSync(Context);
				DataTable dtMain = ExchangeUtils.GetMessage(User, sUNIQUE_ID);
				if ( dtMain.Rows.Count > 0 )
				{
					DataRow row = dtMain.Rows[0];
					TextBox      ctlNAME        = plcQuickCreate.Controls[0].FindControl("NAME"       ) as TextBox;
					TextBox      ctlFIRST_NAME  = plcQuickCreate.Controls[0].FindControl("FIRST_NAME" ) as TextBox;
					TextBox      ctlLAST_NAME   = plcQuickCreate.Controls[0].FindControl("LAST_NAME"  ) as TextBox;
					TextBox      ctlEMAIL1      = plcQuickCreate.Controls[0].FindControl("EMAIL1"     ) as TextBox;
					DropDownList ctlLEAD_SOURCE = plcQuickCreate.Controls[0].FindControl("LEAD_SOURCE") as DropDownList;
					TextBox      ctlDESCRIPTION = plcQuickCreate.Controls[0].FindControl("DESCRIPTION") as TextBox;
					string sNAME       = Sql.ToString(row["FROM_NAME"]);
					string sFIRST_NAME = String.Empty;
					string sLAST_NAME  = String.Empty;
					if ( sNAME.Contains("@") )
						sNAME = sNAME.Split('@')[0];
					sNAME = sNAME.Replace('.', ' ');
					sNAME = sNAME.Replace('_', ' ');
					string[] arrNAME = sNAME.Split(' ');
					if ( arrNAME.Length > 1 )
					{
						sFIRST_NAME = arrNAME[0];
						sLAST_NAME  = arrNAME[arrNAME.Length - 1];
					}
					else
					{
						sLAST_NAME = sNAME;
					}
					if ( ctlNAME        != null ) ctlNAME       .Text = Sql.ToString(row["NAME"       ]);
					if ( ctlFIRST_NAME  != null ) ctlFIRST_NAME .Text = sFIRST_NAME;
					if ( ctlLAST_NAME   != null ) ctlLAST_NAME  .Text = sLAST_NAME ;
					if ( ctlEMAIL1      != null ) ctlEMAIL1     .Text = Sql.ToString(row["FROM_ADDR"  ]);
					// 06/04/2010 Paul.  Use the Text version of the body. 
					if ( ctlDESCRIPTION != null ) ctlDESCRIPTION.Text = Sql.ToString(row["DESCRIPTION"]);
					if ( ctlLEAD_SOURCE != null )
					{
						try
						{
							// 08/19/2010 Paul.  Check the list before assigning the value. 
							Utils.SetSelectedValue(ctlLEAD_SOURCE, "Email");
						}
						catch
						{
						}
					}
				}
			}
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "CheckMail" )
				{
					BindTree();
				}
				else if ( e.CommandName == "Compose" )
				{
					// 06/01/2010 Paul.  Lets restore the Select action on the selected node and clear the selection. 
					string sLastSelectedNodeValuePath = Sql.ToString(ViewState["LastSelectedNodeValuePath"]);
					string sLastSelectedNodeValue     = Sql.ToString(ViewState["LastSelectedNodeValue"    ]);
					if ( !Sql.IsEmptyString(sLastSelectedNodeValue) )
					{
						// 03/10/2015 Paul.  treeMain.FindNode() is not working, so manually search the tree. 
						TreeNode node = FindNode(treeMain, sLastSelectedNodeValue);
						if ( node != null )
						{
							node.SelectAction = TreeNodeSelectAction.Select;
							node.Selected     = false;
							ViewState["LastSelectedNodeValuePath"] = String.Empty;
							ViewState["LastSelectedNodeValue"    ] = String.Empty;
						}
					}
					divMain      .Visible = false;
					ctlImportView.Visible = false;
					ctlDetailView.Visible = false;
					ctlEditView  .Visible = true ;
					ctlImportView.ClearForm();
					ctlDetailView.ClearForm();
					ctlEditView  .ClearForm();
					hidSelectedUniqueID.Value = String.Empty;
					grdMain.SelectedIndex = -1;
					hidQuickCreateModule.Value = String.Empty;
					plcQuickCreate.Controls.Clear();
				}
				else if ( e.CommandName == "Reply" || e.CommandName == "ReplyAll" || e.CommandName == "Forward" )
				{
					if ( grdMain.SelectedItem != null )
					{
						DataView vw = grdMain.DataSource as DataView;
						if ( vw != null && vw.Count >= grdMain.SelectedItem.ItemIndex )
						{
							string sUNIQUE_ID = Sql.ToString(vw[grdMain.SelectedItem.ItemIndex]["UNIQUE_ID"]);
							hidSelectedUniqueID.Value = sUNIQUE_ID;
							// 07/17/2010 Paul.  Method name should be singular as only one message is being retrieved. 
							// 11/23/2011 Paul.  Add MAIL_SMTPUSER and MAIL_SMTPPASS so that we can avoid impersonation. 
							ExchangeSync.UserSync User = new ExchangeSync.UserSync(Context);
							DataTable dtMain = ExchangeUtils.GetMessage(User, sUNIQUE_ID);
							if ( dtMain.Rows.Count > 0 )
							{
								// 06/01/2010 Paul.  Lets restore the Select action on the selected node and clear the selection. 
								string sLastSelectedNodeValuePath = Sql.ToString(ViewState["LastSelectedNodeValuePath"]);
								string sLastSelectedNodeValue     = Sql.ToString(ViewState["LastSelectedNodeValue"    ]);
								if ( !Sql.IsEmptyString(sLastSelectedNodeValue) )
								{
									// 03/10/2015 Paul.  treeMain.FindNode() is not working, so manually search the tree. 
									TreeNode node = FindNode(treeMain, sLastSelectedNodeValue);
									if ( node != null )
									{
										node.SelectAction = TreeNodeSelectAction.Select;
										node.Selected     = false;
										ViewState["LastSelectedNodeValuePath"] = String.Empty;
										ViewState["LastSelectedNodeValue"    ] = String.Empty;
									}
								}
								divMain      .Visible = false;
								ctlImportView.Visible = false;
								ctlDetailView.Visible = false;
								ctlEditView  .Visible = true ;
								ctlImportView.ClearForm();
								ctlDetailView.ClearForm();
								ctlEditView  .ClearForm();
								hidQuickCreateModule.Value = String.Empty;
								plcQuickCreate.Controls.Clear();
								
								DataRow row = dtMain.Rows[0];
								
								// 06/01/2010 Paul.  Add reply header. 
								string sReplyDelimiter = String.Empty;  //"> ";
								StringBuilder sbReplyHeader = new StringBuilder();
								sbReplyHeader.Append("<br />\r\n");
								sbReplyHeader.Append("<br />\r\n");
								sbReplyHeader.Append("<hr />\r\n");
								sbReplyHeader.Append(sReplyDelimiter + "<b>" + L10n.Term("Emails.LBL_FROM"     ) + "</b> " + HttpUtility.HtmlEncode(Sql.ToString  (row["FROM"      ])) + "<br />\r\n");
								sbReplyHeader.Append(sReplyDelimiter + "<b>" + L10n.Term("Emails.LBL_DATE_SENT") + "</b> " + Sql.ToDateTime(row["DATE_START"]) + "<br />\r\n");
								sbReplyHeader.Append(sReplyDelimiter + "<b>" + L10n.Term("Emails.LBL_TO"       ) + "</b> " + HttpUtility.HtmlEncode(Sql.ToString  (row["TO_ADDRS"  ])) + "<br />\r\n");
								if ( !Sql.IsEmptyString(row["CC_ADDRS"]) )
									sbReplyHeader.Append(sReplyDelimiter + "<b>" + L10n.Term("Emails.LBL_CC"       ) + "</b> " + HttpUtility.HtmlEncode(Sql.ToString  (row["CC_ADDRS"  ])) + "<br />\r\n");
								sbReplyHeader.Append(sReplyDelimiter + "<b>" + L10n.Term("Emails.LBL_SUBJECT"  ) + "</b> " + HttpUtility.HtmlEncode(Sql.ToString  (row["NAME"      ])) + "<br />\r\n");
								sbReplyHeader.Append(sReplyDelimiter + "<br />\r\n");
								
								// 11/05/2010 Paul.  Each user can have their own email account, but they all will share the same server. 
								// Remove all references to USER_SETTINGS/MAIL_FROMADDRESS and USER_SETTINGS/MAIL_FROMNAME. 
								string sFrom = Security.EMAIL1;
								List<String> arrTo = new List<String>();
								arrTo.Add(Sql.ToString(row["FROM"]));
								string[] arrTO_ADDRS = Sql.ToString(row["TO_ADDRS"]).Split(';');
								foreach ( string sTO in arrTO_ADDRS )
								{
									if ( sTO.IndexOf(sFrom) < 0 && arrTo.FindIndex(delegate(string s){return s == sTO;}) < 0 )
									{
										arrTo.Add(sTO.Trim());
									}
								}
								
								if ( e.CommandName == "Reply" || e.CommandName == "ReplyAll" )
									ctlEditView.TO_ADDRS = String.Join("; ", arrTo.ToArray());
								if ( e.CommandName == "ReplyAll" )
									ctlEditView.CC_ADDRS = Sql.ToString(row["CC_ADDRS"]);
								ctlEditView.NAME     = HttpUtility.HtmlEncode(Sql.ToString(row["NAME"]));
								// 06/04/2010 Paul.  Use the HTML version of the body. 
								string sDESCRIPTION  = Sql.ToString(row["DESCRIPTION_HTML"]);
								
								// 05/23/2010 Paul.  XssFilter will remove <html>, so we have to check first. 
								if ( !(sDESCRIPTION.IndexOf("<html", StringComparison.CurrentCultureIgnoreCase) >= 0 || sDESCRIPTION.IndexOf("<body", StringComparison.CurrentCultureIgnoreCase) >= 0 || sDESCRIPTION.IndexOf("<br", StringComparison.CurrentCultureIgnoreCase) >= 0) )
								{
									// 06/04/2010 Paul.  Try and prevent excess blank lines. 
									sDESCRIPTION = EmailUtils.NormalizeDescription(sDESCRIPTION);
								}
								sDESCRIPTION = EmailUtils.XssFilter(sDESCRIPTION, Sql.ToString(Application["CONFIG.email_xss"]));
								ctlEditView.DESCRIPTION = sbReplyHeader + sDESCRIPTION;
								
								// 05/18/2014 Paul.  Import the message if it does not already exist in the system so that we can include attachments in forward. 
								string sPARENT_TYPE      = String.Empty;
								Guid   gPARENT_ID        = Guid.Empty;
								Guid   gASSIGNED_USER_ID = Security.USER_ID;
								Guid   gTEAM_ID          = Security.TEAM_ID;
								string sTEAM_SET_LIST    = String.Empty;
								Guid gEMAIL_ID = ExchangeUtils.ImportMessage(User, Session, sPARENT_TYPE, gPARENT_ID, gASSIGNED_USER_ID, gTEAM_ID, sTEAM_SET_LIST, sUNIQUE_ID);
								if ( e.CommandName == "Forward" )
									ctlEditView.LoadEmailAttachments(gEMAIL_ID);
							}
						}
					}
				}
				else if ( e.CommandName == "Send" || e.CommandName == "Save" ||e.CommandName == "Cancel" )
				{
					divMain      .Visible = true ;
					ctlImportView.Visible = false;
					ctlDetailView.Visible = false;
					ctlEditView  .Visible = false;
					ctlEditView.ClearForm();
					hidSelectedUniqueID.Value = String.Empty;
					grdMain.SelectedIndex = -1;
					hidQuickCreateModule.Value = String.Empty;
					plcQuickCreate.Controls.Clear();
				}
				else if ( e.CommandName == "Delete" )
				{
					if ( !Sql.IsEmptyString(hidSelectedUniqueID.Value) )
					{
						// 11/23/2011 Paul.  Add MAIL_SMTPUSER and MAIL_SMTPPASS so that we can avoid impersonation. 
						ExchangeSync.UserSync User = new ExchangeSync.UserSync(Context);
						ExchangeUtils.DeleteMessage(User, hidSelectedUniqueID.Value);
						hidSelectedUniqueID.Value = String.Empty;
						grdMain.SelectedIndex = -1;
						treeMain_SelectedNodeChanged(null, null);
					}
				}
				else if ( e.CommandName == "Import" )
				{
					divMain.Visible = true;
					hidQuickCreateModule.Value = String.Empty;
					plcQuickCreate.Controls.Clear();
					ctlImportView.Visible = !ctlImportView.Visible;
					if ( ctlImportView.Visible )
						ctlImportView.ClearForm();
				}
				else if ( e.CommandName == "Import.Cancel" )
				{
					ctlImportView.Visible = false;
				}
				else if ( e.CommandName == "Import.Save" )
				{
					if ( !Sql.IsEmptyString(hidSelectedUniqueID.Value) )
					{
						string sUNIQUE_ID        = hidSelectedUniqueID.Value;
						string sPARENT_TYPE      = String.Empty;
						Guid   gPARENT_ID        = Guid.Empty;
						Guid   gASSIGNED_USER_ID = Security.USER_ID;
						Guid   gTEAM_ID          = Security.TEAM_ID;
						string sTEAM_SET_LIST    = String.Empty;
						DropDownList    ctlPARENT_TYPE      = ctlImportView.Controls[0].FindControl("PARENT_TYPE"     ) as DropDownList;
						HtmlInputHidden ctlPARENT_ID        = ctlImportView.Controls[0].FindControl("PARENT_ID"       ) as HtmlInputHidden;
						HtmlInputHidden ctlASSIGNED_USER_ID = ctlImportView.Controls[0].FindControl("ASSIGNED_USER_ID") as HtmlInputHidden;
						HtmlInputHidden ctlTEAM_ID          = ctlImportView.Controls[0].FindControl("TEAM_ID"         ) as HtmlInputHidden;
						TeamSelect      ctlTeamSelect       = ctlImportView.Controls[0].FindControl("TEAM_SET_NAME"   ) as TeamSelect;
						if ( ctlPARENT_TYPE      != null ) sPARENT_TYPE      = ctlPARENT_TYPE.SelectedValue;
						if ( ctlPARENT_ID        != null ) gPARENT_ID        = Sql.ToGuid(ctlPARENT_ID.Value);
						if ( ctlASSIGNED_USER_ID != null ) gASSIGNED_USER_ID = Sql.ToGuid(ctlASSIGNED_USER_ID.Value);
						if ( ctlTEAM_ID          != null ) gTEAM_ID          = Sql.ToGuid(ctlTEAM_ID.Value);
						if ( ctlTeamSelect != null )
						{
							gTEAM_ID       = ctlTeamSelect.TEAM_ID;
							sTEAM_SET_LIST = ctlTeamSelect.TEAM_SET_LIST;
						}
						// 11/23/2011 Paul.  Add MAIL_SMTPUSER and MAIL_SMTPPASS so that we can avoid impersonation. 
						ExchangeSync.UserSync User = new ExchangeSync.UserSync(Context);
						ExchangeUtils.ImportMessage(User, Session, sPARENT_TYPE, gPARENT_ID, gASSIGNED_USER_ID, gTEAM_ID, sTEAM_SET_LIST, sUNIQUE_ID);
					}
					ctlImportView.Visible = false;
				}
				else if ( e.CommandName == "QuickCreate")
				{
					plcQuickCreate.Controls.Clear();
					Control ctl = LoadControl("~/" + Sql.ToString(e.CommandArgument) + "/NewRecord.ascx");
					NewRecordControl ctlNewRecord = ctl as NewRecordControl;
					if ( ctlNewRecord != null )
					{
						divMain.Visible = false;
						ctlNewRecord.EditView          = "EditView.Inline";
						ctlNewRecord.Width             = new Unit("100%");
						ctlNewRecord.ShowHeader        = true ;
						ctlNewRecord.ShowInlineHeader  = false;
						ctlNewRecord.ShowTopButtons    = true ;
						ctlNewRecord.ShowBottomButtons = false;
						ctlNewRecord.ShowCancel        = true ;
						ctlNewRecord.Command          += new CommandEventHandler(Page_Command);
						ctlNewRecord.EditViewLoad     += new EventHandler(QuickCreate_Load);
						ctlNewRecord.NotPostBack       = true;
						plcQuickCreate.Controls.Add(ctlNewRecord);
						hidQuickCreateModule.Value = Sql.ToString(e.CommandArgument);
					}
				}
				else if ( e.CommandName == "NewRecord" )
				{
					if ( !Sql.IsEmptyString(hidSelectedUniqueID.Value) && plcQuickCreate.Controls.Count > 0 )
					{
						string sUNIQUE_ID        = hidSelectedUniqueID.Value     ;
						string sPARENT_TYPE      = hidQuickCreateModule.Value    ;
						Guid   gPARENT_ID        = Sql.ToGuid(e.CommandArgument) ;
						Guid   gASSIGNED_USER_ID = Security.USER_ID;
						Guid   gTEAM_ID          = Security.TEAM_ID;
						string sTEAM_SET_LIST    = String.Empty;
						HtmlInputHidden ctlASSIGNED_USER_ID = plcQuickCreate.Controls[0].FindControl("ASSIGNED_USER_ID") as HtmlInputHidden;
						HtmlInputHidden ctlTEAM_ID          = plcQuickCreate.Controls[0].FindControl("TEAM_ID"         ) as HtmlInputHidden;
						TeamSelect      ctlTeamSelect       = plcQuickCreate.Controls[0].FindControl("TEAM_SET_NAME"   ) as TeamSelect;
						if ( ctlASSIGNED_USER_ID != null )
							gASSIGNED_USER_ID = Sql.ToGuid(ctlASSIGNED_USER_ID.Value);
						if ( ctlTEAM_ID != null )
							gTEAM_ID = Sql.ToGuid(ctlTEAM_ID.Value);
						if ( ctlTeamSelect != null )
						{
							gTEAM_ID       = ctlTeamSelect.TEAM_ID;
							sTEAM_SET_LIST = ctlTeamSelect.TEAM_SET_LIST;
						}
						// 11/23/2011 Paul.  Add MAIL_SMTPUSER and MAIL_SMTPPASS so that we can avoid impersonation. 
						ExchangeSync.UserSync User = new ExchangeSync.UserSync(Context);
						ExchangeUtils.ImportMessage(User, Session, sPARENT_TYPE, gPARENT_ID, gASSIGNED_USER_ID, gTEAM_ID, sTEAM_SET_LIST, sUNIQUE_ID);
					}
					divMain.Visible = true;
					hidQuickCreateModule.Value = String.Empty;
					plcQuickCreate.Controls.Clear();
				}
				else if ( e.CommandName == "NewRecord.Cancel" )
				{
					divMain.Visible = true;
					hidQuickCreateModule.Value = String.Empty;
					plcQuickCreate.Controls.Clear();
				}
				else
				{
#if DEBUG
					ctlDynamicButtons.ErrorText = "Unknown CommandName: " + e.CommandName;
#endif
				}
				// 06/01/2010 Paul.  We need to rebind, otherwise the pagination disappears and the selection fails. 
				grdMain.DataBind();
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
		}

		// 07/30/2010 Paul.  We are going to get the folder item counts as the tree is expanded. 
		protected void treeMain_TreeNodeExpanded(object sender, TreeNodeEventArgs e)
		{
			try
			{
				// 07/30/2010 Paul.  The root node does not have a mailbox. 
				if ( e.Node.Parent != null )
				{
					XmlDocument xml = new XmlDocument();
					// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
					// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
					// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
					xml.XmlResolver = null;
					xml.LoadXml(Sql.ToString(Session["Exchange.xmlFolders"]));
					// 07/30/2010 Paul.  The leading // means to search for any node, regardless of the node position. 
					// http://www.dotnetjohn.com/articles.aspx?articleid=154
					XmlNode xFolder = xml.SelectSingleNode("//Folder[@Id=" + XmlUtil.EncaseXpathString(e.Node.Value) + "]");
					if ( xFolder != null )
					{
						// 11/23/2011 Paul.  Add MAIL_SMTPUSER and MAIL_SMTPPASS so that we can avoid impersonation. 
						ExchangeSync.UserSync User = new ExchangeSync.UserSync(Context);
						ExchangeUtils.UpdateFolderTreeNodeCounts(User, xFolder);
						Session["Exchange.xmlFolders"] = xml.OuterXml;
						// 07/30/2010 Paul.  The updated text needs to be manually applied as rebinding will cause another expanded event. 
						foreach ( TreeNode child in e.Node.ChildNodes )
						{
							XmlNode xChild = xFolder.SelectSingleNode("Folder[@Id=" + XmlUtil.EncaseXpathString(child.Value) + "]");
							if ( xChild != null )
							{
								child.Text = XmlUtil.GetNamedItem(xChild, "DisplayName");
							}
						}
					}
					else
					{
						ctlDynamicButtons.ErrorText = "Could not find " + e.Node.Value;
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
		}

		// 03/10/2015 Paul.  treeMain.FindNode() is not working, so manually search the tree. 
		private TreeNode FindNode(TreeView parent, string sValue)
		{
			TreeNode node = null;
			if ( parent.Nodes != null && parent.Nodes.Count > 0 )
			{
				for ( int i = 0; i < parent.Nodes.Count; i++ )
				{
					node = FindNode(parent.Nodes[i], sValue);
				}
			}
			return node;
		}

		private TreeNode FindNode(TreeNode parent, string sValue)
		{
			TreeNode node = null;
			if ( parent.Value == sValue )
			{
				node = parent;
			}
			else
			{
				if ( parent.ChildNodes != null && parent.ChildNodes.Count > 0 )
				{
					for ( int i = 0; i < parent.ChildNodes.Count && node == null; i++ )
					{
						node = FindNode(parent.ChildNodes[i], sValue);
					}
				}
			}
			return node;
		}

		protected void treeMain_SelectedNodeChanged(Object sender, EventArgs e)
		{
			try
			{
				// 05/30/2010 Paul.  Disable selection of current node as the postback will not generate an event. 
				// Without the event, we are unable to bind the Main grid. 
				// We need to keep track of the last selected node so that we can restore the SelectAction. 
				string sLastSelectedNodeValuePath = Sql.ToString(ViewState["LastSelectedNodeValuePath"]);
				string sLastSelectedNodeValue     = Sql.ToString(ViewState["LastSelectedNodeValue"    ]);
				if ( !Sql.IsEmptyString(sLastSelectedNodeValue) )
				{
					// 03/10/2015 Paul.  treeMain.FindNode() is not working, so manually search the tree. 
					TreeNode node = FindNode(treeMain, sLastSelectedNodeValue);
					if ( node != null )
						node.SelectAction = TreeNodeSelectAction.Select;
				}
				if ( treeMain.SelectedNode != null )
				{
					ViewState["LastSelectedNodeValuePath"] = treeMain.SelectedNode.ValuePath;
					ViewState["LastSelectedNodeValue"    ] = treeMain.SelectedNode.Value    ;
					treeMain.SelectedNode.SelectAction = TreeNodeSelectAction.None;
					
					// 05/30/2010 Paul.  Clear any previous selection. 
					// 11/05/2010 Paul.  Clear the grid. 
					grdMain.CurrentPageIndex = 0;
					grdMain.DataSource = null;
					grdMain.DataBind();
					divMain      .Visible = true ;
					ctlImportView.Visible = false;
					ctlDetailView.Visible = false;
					ctlEditView  .Visible = false;
					ctlImportView.ClearForm();
					ctlDetailView.ClearForm();
					hidSelectedUniqueID.Value = String.Empty;
					grdMain.SelectedIndex = -1;
					hidQuickCreateModule.Value = String.Empty;
					plcQuickCreate.Controls.Clear();
					//ctlDynamicButtons.ErrorText = treeMain.SelectedNode.Text;
					string sFOLDER_ID = treeMain.SelectedNode.Value;
					lblFolderTitle.Text = treeMain.SelectedNode.Text;
					
					int nTotalCount  = 0;
					int nUnreadCount = 0;
					// 11/23/2011 Paul.  Add MAIL_SMTPUSER and MAIL_SMTPPASS so that we can avoid impersonation. 
					ExchangeSync.UserSync User = new ExchangeSync.UserSync(Context);
					ExchangeUtils.GetFolderCount(User, sFOLDER_ID, ref nTotalCount, ref nUnreadCount);
					grdMain.VirtualItemCount = nTotalCount;
					lblFolderTitle.Text = String.Format(L10n.Term("EmailClient.LBL_FOLDER_TITLE"), treeMain.SelectedNode.Text, grdMain.VirtualItemCount);
					
					XmlDocument xml = new XmlDocument();
					// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
					// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
					// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
					xml.XmlResolver = null;
					xml.LoadXml(Sql.ToString(Session["Exchange.xmlFolders"]));
					// 07/17/2010 Paul.  Lets update the Recent count when we get the messages. 
					// 07/30/2010 Paul.  The leading // means to search for any node, regardless of the node position. 
					// http://www.dotnetjohn.com/articles.aspx?articleid=154
					XmlNode xFolder = xml.SelectSingleNode("//Folder[@Id=" + XmlUtil.EncaseXpathString(sFOLDER_ID) + "]");
					if ( xFolder != null )
					{
						string sName = XmlUtil.GetNamedItem(xFolder, "Name");
						string sDisplayName = String.Empty;
						if ( nUnreadCount > 0 )
							sDisplayName = "<b>" + sName + "</b> <font color=blue>(" + nUnreadCount.ToString() + ")</font>";
						else
							sDisplayName = sName;
						treeMain.SelectedNode.Text = sDisplayName;
						
						XmlUtil.SetSingleNodeAttribute(xml, xFolder, "TotalCount" , nTotalCount .ToString());
						XmlUtil.SetSingleNodeAttribute(xml, xFolder, "UnreadCount", nUnreadCount.ToString());
						XmlUtil.SetSingleNodeAttribute(xml, xFolder, "DisplayName", sDisplayName           );
						Session["Exchange.xmlFolders"] = xml.OuterXml;
					}
					
					grdMain.SortColumn = "DATE_START";
					grdMain.SortOrder  = "desc";
					grdMain.DataBind();
				}
				else
				{
					ViewState["LastSelectedNodeValuePath"] = String.Empty;
					ViewState["LastSelectedNodeValue"    ] = String.Empty;
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
		}

		protected void grdMain_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			// 06/01/2010 Paul.  We got an exception when the selected item was no longer visible. 
			// It is safer just to hide the selection. 
			ctlImportView.Visible = false;
			ctlDetailView.Visible = false;
			ctlImportView.ClearForm();
			ctlDetailView.ClearForm();
			hidSelectedUniqueID.Value = String.Empty;
			grdMain.SelectedIndex = -1;
			hidQuickCreateModule.Value = String.Empty;
			plcQuickCreate.Controls.Clear();
			/*
			// 05/30/2010 Paul.  We need to update the SelectedIndex after a sort command. 
			if ( !Sql.IsEmptyString(hidSelectedUniqueID.Value) )
			{
				DataView vw = grdMain.DataSource as DataView;
				if ( vw != null && vw.Count >= grdMain.SelectedItem.ItemIndex )
				{
					for ( int i = 0; i < vw.Count; i++ )
					{
						DataRowView row = vw[i];
						if ( hidSelectedUniqueID.Value == Sql.ToString(row["UNIQUE_ID"]) )
						{
							grdMain.SelectedIndex = i;
							// 05/30/2010 Paul.  Return if item found so that all other cases would clear the selection. 
							return;
						}
					}
				}
			}
			grdMain.SelectedIndex = -1;
			*/
		}

		protected void grdMain_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			ctlImportView.Visible = false;
			ctlDetailView.Visible = false;
			ctlImportView.ClearForm();
			ctlDetailView.ClearForm();
			hidSelectedUniqueID.Value = String.Empty;
			grdMain.SelectedIndex = -1;
			hidQuickCreateModule.Value = String.Empty;
			plcQuickCreate.Controls.Clear();
		}

		protected void grdMain_OnSelectMethod(int nCurrentPageIndex, int nPageSize)
		{
			try
			{
				string sFOLDER_ID = Sql.ToString(ViewState["LastSelectedNodeValue"]);
				if ( !Sql.IsEmptyString(sFOLDER_ID) )
				{
					int nPageOffset = nCurrentPageIndex * nPageSize;
					// 11/23/2011 Paul.  Add MAIL_SMTPUSER and MAIL_SMTPPASS so that we can avoid impersonation. 
					ExchangeSync.UserSync User = new ExchangeSync.UserSync(Context);
					dtMain = ExchangeUtils.GetFolderMessages(User, sFOLDER_ID, nPageSize, nPageOffset, grdMain.SortColumn, grdMain.SortOrder);
					if ( dtMain != null )
						grdMain.DataSource = new DataView(dtMain);
					else
						grdMain.DataSource = null;
					ViewState["Messages"] = dtMain;
					// 11/06/2010 Paul.  Also save the messages in the session so that the Attachment page will have quick access. 
					Session["Exchange.Messages"] = dtMain;
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
		}

		protected void grdMain_OnItemCreated(object sender, DataGridItemEventArgs e)
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
						TableCell td1 = itm.Controls[0] as TableCell;
						if ( td1 != null && td1.Controls.Count > 0 )
						{
							Button lnkSelect = td1.Controls[0] as Button;
							if ( lnkSelect != null )
							{
								string sMsgID = "msg" + Sql.ToString(row["ID"]);
								itm.Attributes.Add("id", sMsgID);
								itm.Attributes.Add("onclick", "InboxMessageClickSubmitButton('" + sMsgID + "');");
							}
						}
					}
				}
			}
		}
		
		protected void grdMain_SelectedIndexChanged(object sender, EventArgs e)
		{
			ctlImportView.Visible = false;
			ctlDetailView.Visible = true ;
			ctlEditView  .Visible = false;
			hidQuickCreateModule.Value = String.Empty;
			plcQuickCreate.Controls.Clear();
			if ( grdMain.SelectedItem != null )
			{
				DataView vw = grdMain.DataSource as DataView;
				if ( vw != null && vw.Count >= grdMain.SelectedItem.ItemIndex )
				{
					string sUNIQUE_ID = Sql.ToString(vw[grdMain.SelectedItem.ItemIndex]["UNIQUE_ID"]);
					hidSelectedUniqueID.Value = sUNIQUE_ID;
					// 07/17/2010 Paul.  Method name should be singular as only one message is being retrieved. 
					// 11/23/2011 Paul.  Add MAIL_SMTPUSER and MAIL_SMTPPASS so that we can avoid impersonation. 
					ExchangeSync.UserSync User = new ExchangeSync.UserSync(Context);
					DataTable dtMain = ExchangeUtils.GetMessage(User, sUNIQUE_ID);
					if ( dtMain.Rows.Count > 0 )
					{
						DataRow row = dtMain.Rows[0];
						ctlDetailView.FROM             = HttpUtility.HtmlEncode(Sql.ToString  (row["FROM"       ]));
						ctlDetailView.DATE_START       = (row["DATE_START"] != DBNull.Value) ? Sql.ToDateTime(row["DATE_START" ]).ToString("g") : String.Empty;
						ctlDetailView.TO_ADDRS         = HttpUtility.HtmlEncode(Sql.ToString  (row["TO_ADDRS"   ]));
						ctlDetailView.CC_ADDRS         = HttpUtility.HtmlEncode(Sql.ToString  (row["CC_ADDRS"   ]));
						ctlDetailView.NAME             = HttpUtility.HtmlEncode(Sql.ToString  (row["NAME"       ]));
						ctlDetailView.INTERNET_HEADERS = Sql.ToString(row["INTERNET_HEADERS"]);
						// 06/04/2010 Paul.  Use the HTML version of the body. 
						string sDESCRIPTION            = Sql.ToString(row["DESCRIPTION_HTML"]);
						
						// 05/23/2010 Paul.  XssFilter will remove <html>, so we have to check first. 
						if ( !(sDESCRIPTION.IndexOf("<html", StringComparison.CurrentCultureIgnoreCase) >= 0 || sDESCRIPTION.IndexOf("<body", StringComparison.CurrentCultureIgnoreCase) >= 0 || sDESCRIPTION.IndexOf("<br", StringComparison.CurrentCultureIgnoreCase) >= 0) )
						{
							// 06/04/2010 Paul.  Try and prevent excess blank lines. 
							sDESCRIPTION = EmailUtils.NormalizeDescription(sDESCRIPTION);
						}
						sDESCRIPTION = EmailUtils.XssFilter(sDESCRIPTION, Sql.ToString(Application["CONFIG.email_xss"]));
						// 11/06/2010 Paul.  Return the Attachments so that we can show embedded images or download the attachments. 
						string sATTACHMENTS = Sql.ToString(row["ATTACHMENTS"]);
						try
						{
							XmlDocument xml = new XmlDocument();
							// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
							// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
							// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
							xml.XmlResolver = null;
							xml.LoadXml(sATTACHMENTS);
							string[] arrColumns = new string[]
								{ "ID"                
								, "Name"              
								, "IsInline"          
								, "FileName"          
								, "Size"              
								, "MediaType"         
								, "CharSet"           
								, "ContentType"       
								, "ContentID"         
								, "ContentDescription"
								, "ContentEncoding"   
								, "ContentMD5"        
								, "ContentLanguage"   
								, "Disposition"       
								, "Boundary"          
								, "Location"          
								, "LastModifiedTime"  
								, "URL"               
								};
							DataTable dtATTACHMENTS = XmlUtil.CreateDataTable(xml.DocumentElement, "Attachment", arrColumns);
							foreach ( DataRow rowAttachment in dtATTACHMENTS.Rows )
							{
								// 01/24/2017 Paul.  Use the Filename instead of the ID as it is common across MimeKit. 
								//string sID        = Sql.ToString(rowAttachment["ID"       ]);
								string sContentID = Sql.ToString(rowAttachment["ContentID"]);
								string sFileName  = Sql.ToString(rowAttachment["FileName" ]);
								string sURL       = "Attachment.aspx?UNIQUE_ID=" + HttpUtility.UrlEncode(sUNIQUE_ID) + "&ATTACHMENT_ID=" + HttpUtility.UrlEncode(sFileName);
								rowAttachment["URL"] = "Exchange/" + sURL;
								if ( !Sql.IsEmptyString(sContentID) )
								{
									rowAttachment["ContentID"] = HttpUtility.HtmlEncode(sContentID);
									if ( sContentID.StartsWith("<") && sContentID.EndsWith(">") )
										sContentID = sContentID.Substring(1, sContentID.Length - 2);
									sDESCRIPTION = sDESCRIPTION.Replace("cid:" + sContentID, sURL);
								}
							}
							ctlDetailView.ATTACHMENTS = dtATTACHMENTS;
						}
						catch
						{
						}
						ctlDetailView.DESCRIPTION = sDESCRIPTION;
					}
				}
			}
			// 05/23/2010 Paul.  We need to bind after the selection change, otherwise the pagination will disappear. 
			grdMain.DataBind();
		}

		protected void BindTree()
		{
			divMain      .Visible = true ;
			ctlImportView.Visible = false;
			ctlDetailView.Visible = false;
			ctlEditView  .Visible = false;
			ctlImportView.ClearForm();
			ctlDetailView.ClearForm();
			ctlEditView  .ClearForm();
			hidSelectedUniqueID.Value = String.Empty;
			grdMain.SelectedIndex = -1;
			hidQuickCreateModule.Value = String.Empty;
			plcQuickCreate.Controls.Clear();
			dtMain = null;
			grdMain.DataSource = null;
			
			// 05/30/2010 Paul.  Clear the last selection before we bind. 
			ViewState["LastSelectedNodeValuePath"] = String.Empty;
			ViewState["LastSelectedNodeValue"    ] = String.Empty;
			string sInboxFolderId = String.Empty;
			// 08/31/2010 Paul.  Don't query Exchange if it has not been configured. 
			string sSERVER_URL = Sql.ToString(Context.Application["CONFIG.Exchange.ServerURL"]);
			// 01/24/2017 Paul.  Add support for Office365. 
			if ( !Sql.IsEmptyString(sSERVER_URL) || Sql.ToBoolean(Session["OFFICE365_OAUTH_ENABLED"]) )
			{
				// 11/23/2011 Paul.  Add MAIL_SMTPUSER and MAIL_SMTPPASS so that we can avoid impersonation. 
				ExchangeSync.UserSync User = new ExchangeSync.UserSync(Context);
				XmlDocument xmlFolders = ExchangeUtils.GetFolderTree(User, ref sInboxFolderId);
				xdsFolders.Data = xmlFolders.OuterXml;
				treeMain.DataSourceID = "xdsFolders";
				treeMain.DataBind();
				// 07/31/2010 Paul.  Store in Session state for performance reasons. 
				Session["Exchange.xmlFolders"] = xmlFolders.OuterXml;
				
				if ( treeMain.Nodes.Count > 0 )
				{
					TreeNode root = treeMain.Nodes[0];
					foreach ( TreeNode node in root.ChildNodes )
					{
						if ( node.Value == sInboxFolderId )
						{
							node.Select();
							treeMain_SelectedNodeChanged(null, null);
							break;
						}
					}
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(m_sMODULE + ".LBL_LIST_FORM_TITLE"));
			// 01/23/2017 Paul.  Enable for Office365. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "list") >= 0); //&& !Sql.IsEmptyString(Security.EXCHANGE_ALIAS);
			if ( !this.Visible )
				return;

			try
			{
				if ( !IsPostBack )
				{
					BindTree();
				}
				else
				{
					dtMain = ViewState["Messages"] as DataTable;
					// 07/30/2010 Paul.  When changing connection settings, the messages table will be reset. 
					if ( dtMain != null )
					{
						grdMain.DataSource = new DataView(dtMain);
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
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
			ctlImportView    .Command += new CommandEventHandler(Page_Command);
			ctlDetailView    .Command += new CommandEventHandler(Page_Command);
			ctlEditView      .Command += new CommandEventHandler(Page_Command);
			treeMain.SelectedNodeChanged  += new EventHandler(treeMain_SelectedNodeChanged);
			treeMain.TreeNodeExpanded     += new TreeNodeEventHandler(treeMain_TreeNodeExpanded);
			grdMain.ItemCreated           += new DataGridItemEventHandler(grdMain_OnItemCreated);
			grdMain.SortCommand           += new DataGridSortCommandEventHandler(grdMain_SortCommand);
			grdMain.PageIndexChanged      += new DataGridPageChangedEventHandler(grdMain_PageIndexChanged);
			grdMain.SelectMethod          += new SelectMethodHandler(grdMain_OnSelectMethod);
			grdMain.SelectedIndexChanged  += new EventHandler(grdMain_SelectedIndexChanged);

			m_sMODULE = "EmailClient";
			SetMenu(m_sMODULE);
			// 07/16/2010 Paul.  Exchange can have different buttons than Imap. 
			ctlDynamicButtons.AppendButtons(m_sMODULE + ".ListView.Exchange", Guid.Empty, null);
			string sQuickCreateModule = Sql.ToString(Request[hidQuickCreateModule.UniqueID]);
			if ( !Sql.IsEmptyString(sQuickCreateModule) )
			{
				Control ctl = LoadControl("~/" + sQuickCreateModule + "/NewRecord.ascx");
				NewRecordControl ctlNewRecord = ctl as NewRecordControl;
				if ( ctlNewRecord != null )
				{
					ctlNewRecord.EditView          = "EditView.Inline";
					ctlNewRecord.Width             = new Unit("100%");
					ctlNewRecord.ShowHeader        = true ;
					ctlNewRecord.ShowInlineHeader  = false;
					ctlNewRecord.ShowTopButtons    = true ;
					ctlNewRecord.ShowBottomButtons = false;
					ctlNewRecord.ShowCancel        = true ;
					ctlNewRecord.Command          += new CommandEventHandler(Page_Command);
					plcQuickCreate.Controls.Add(ctlNewRecord);
				}
			}
		}
		#endregion
	}
}
