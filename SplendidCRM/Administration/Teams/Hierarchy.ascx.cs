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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Administration.Teams
{
	/// <summary>
	///		Summary description for Hierarchy.
	/// </summary>
	public class Hierarchy : SplendidControl
	{
		protected _controls.SubPanelButtons ctlDynamicButtons;
		protected Guid            gID            ;
		protected Label           lblError       ;
		protected TreeView        treeMain       ;
		protected DataSet         dsMain         ;
		protected XmlDataSource   xdsMain        ;
		protected XmlDocument     xml            ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
		}

		protected void treeMain_SelectedNodeChanged(object sender, EventArgs e)
		{
			Response.Redirect("view.aspx?ID=" + treeMain.SelectedValue);
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			gID = Sql.ToGuid(Request["ID"]);
			// 04/28/2016 Paul.  Make sure the user can view the teams. 
			this.Visible = (SplendidCRM.Security.AdminUserAccess("Teams", "view") >= 0) && Crm.Config.enable_team_hierarchy();
			if ( !this.Visible )
				return;
			try
			{
				dsMain = new DataSet("TeamTree");
				xml = new XmlDocument();
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					if ( Sql.IsSQLServer(con) )
					{
						string sSQL;
						sSQL = "select ID                          as '@ID'       " + ControlChars.CrLf
						     + "     , replace(NAME, '&', '&amp;') as '@NAME'     " + ControlChars.CrLf
						     + "     , PARENT_ID                   as '@PARENT_ID'" + ControlChars.CrLf
						     + "     , dbo.fnTEAM_HIERARCHY_ChildrenXml(ID)       " + ControlChars.CrLf
						     + "  from vwTEAMS                                    " + ControlChars.CrLf
						     + " where ID = @ID                                   " + ControlChars.CrLf
						     + " order by '@NAME'                                 " + ControlChars.CrLf
						     + "   for xml path('TEAM'), type      " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@ID", gID);

							if ( bDebug )
								RegisterClientScriptBlock("fnTEAM_HIERARCHY_ChildrenXml", Sql.ClientScriptBlock(cmd));

							try
							{
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									StringBuilder sbXML = new StringBuilder();
									// 05/20/2016 Paul.  Do not select single row as it will return an incomplete XML. 
									using ( IDataReader rdr = cmd.ExecuteReader() )
									{
										while ( rdr.Read() )
										{
											sbXML.Append(Sql.ToString(rdr[0]));
										}
									}
									string sXML = sbXML.ToString();
									if ( !Sql.IsEmptyString(sXML) )
									{
										xml.LoadXml(sXML);
										xdsMain.Data = sbXML.ToString();
										treeMain.DataSourceID = "xdsMain";
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
					// 05/20/2016 Paul.  Oracle supports hierarchical queries. 
					// http://docs.oracle.com/cd/E11882_01/server.112/e41084/queries003.htm#SQLRF52335
					else if ( Sql.IsOracle(con) )
					{
						string sSQL;
						sSQL = "select ID, NAME, PARENT_ID      " + ControlChars.CrLf
						     + "  from vwTEAMS                  " + ControlChars.CrLf
						     + " start with ID = @ID            " + ControlChars.CrLf
						     + " connect by prior ID = PARENT_ID" + ControlChars.CrLf
						     + " order siblings by NAME         " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@ID", gID);

							if ( bDebug )
								RegisterClientScriptBlock("fnTEAM_HIERARCHY_ChildrenXml", Sql.ClientScriptBlock(cmd));

							try
							{
								xml.AppendChild(xml.CreateProcessingInstruction("xml" , "version=\"1.0\" encoding=\"UTF-8\""));
								xml.AppendChild(xml.CreateElement("xml"));
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( IDataReader rdr = cmd.ExecuteReader() )
									{
										while ( rdr.Read() )
										{
											Guid   gTEAM_ID   = Sql.ToGuid  (rdr["ID"       ]);
											string sNAME      = Sql.ToString(rdr["NAME"     ]);
											Guid   gPARENT_ID = Sql.ToGuid  (rdr["PARENT_ID"]);
											// 05/20/2016 Paul.  The leading // means to search all child nodes. 
											XmlNode xPARENT_ID = xml.DocumentElement.SelectSingleNode("//TEAM[@ID='" + gPARENT_ID.ToString() + "']");
											if ( xPARENT_ID == null )
											{
												XmlNode xTEAM = xml.CreateElement("TEAM");
												xml.DocumentElement.AppendChild(xTEAM);
												XmlAttribute attrID = xml.CreateAttribute("ID");
												attrID.Value = gTEAM_ID.ToString();
												xTEAM.Attributes.SetNamedItem(attrID);
												XmlAttribute attrNAME = xml.CreateAttribute("NAME");
												attrNAME.Value = sNAME;
												xTEAM.Attributes.SetNamedItem(attrNAME);
											}
											else
											{
												XmlNode xTEAM = xml.CreateElement("TEAM");
												xPARENT_ID.AppendChild(xTEAM);
												XmlAttribute attrID = xml.CreateAttribute("ID");
												attrID.Value = gTEAM_ID.ToString();
												xTEAM.Attributes.SetNamedItem(attrID);
												XmlAttribute attrNAME = xml.CreateAttribute("NAME");
												attrNAME.Value = sNAME;
												xTEAM.Attributes.SetNamedItem(attrNAME);
												XmlAttribute attrPARENT_ID = xml.CreateAttribute("PARENT_ID");
												attrPARENT_ID.Value = gPARENT_ID.ToString();
												xTEAM.Attributes.SetNamedItem(attrPARENT_ID);
											}
										}
									}
									xdsMain.Data = xml.DocumentElement.InnerXml;
									treeMain.DataSourceID = "xdsMain";
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
			m_sMODULE = "Teams";
		}
		#endregion
	}
}
