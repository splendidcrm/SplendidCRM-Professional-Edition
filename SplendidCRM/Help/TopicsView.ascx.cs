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
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace SplendidCRM.Help
{
	/// <summary>
	/// Summary description for TopicsView.
	/// </summary>
	public class TopicsView : SplendidControl
	{
		protected DropDownList lstLAYOUT_VIEWS;
		protected XmlDataSource xdsViews      ;
		protected TreeView      treeMain      ;
		protected XmlDocument   xml           ;

		private void Page_Load(object sender, System.EventArgs e)
		{
			xml = new XmlDocument();
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					sSQL = "select *                        " + ControlChars.CrLf
					     + "  from vwTERMINOLOGY_HELP_Layout" + ControlChars.CrLf
					     + " where LANG = @LANG             " + ControlChars.CrLf
					     + " order by MODULE_NAME, NAME     " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@LANG", L10n.NAME);
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								xml.AppendChild(xml.CreateProcessingInstruction("xml" , "version=\"1.0\" encoding=\"UTF-8\""));
								xml.AppendChild(xml.CreateElement("Modules"));
								XmlNode xModules = xml.DocumentElement;
								XmlUtil.SetSingleNodeAttribute(xml, xModules, "Name", L10n.Term(".LNK_HELP"));
								foreach ( DataRow row in dt.Rows )
								{
									string sNAME        = Sql.ToString(row["NAME"       ]);
									string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
									XmlNode xModule = xModules.SelectSingleNode("Module[@Name=\'" + sMODULE_NAME + "\']");
									if ( xModule == null )
									{
										xModule = xml.CreateElement("Module");
										xModules.AppendChild(xModule);
										XmlUtil.SetSingleNodeAttribute(xml, xModule, "Name", sMODULE_NAME);
									}
									XmlNode xView = xml.CreateElement("View");
									xModule.AppendChild(xView);
									XmlUtil.SetSingleNodeAttribute(xml, xView, "Name"       , sNAME);
									XmlUtil.SetSingleNodeAttribute(xml, xView, "DisplayName", sNAME);
									XmlUtil.SetSingleNodeAttribute(xml, xView, "URL"        , "view.aspx?MODULE=" + sMODULE_NAME + "&NAME=" + sNAME);
								}
								//ViewState["xml"] = xml.OuterXml;
								xdsViews.Data = xml.OuterXml;
								treeMain.DataSourceID = "xdsViews";
								treeMain.DataBind();
								// 07/27/2010 Paul.  Select the first view from the first module. 
								// This did not work.  The SearchBasic control is not bound earily enough for it to take affect. 
								/*
								if ( treeMain.Nodes.Count > 0 )
								{
									TreeNode nodeModules = treeMain.Nodes[0];
									if ( nodeModules.ChildNodes.Count > 0 )
									{
										TreeNode nodeModule = nodeModules.ChildNodes[0];
										nodeModule.Expand();
										if ( nodeModule.ChildNodes.Count > 0 )
										{
											TreeNode nodeView = nodeModule.ChildNodes[0];
											nodeView.Select();
										}
									}
								}
								*/
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
			if ( !IsPostBack )
			{
				// 06/09/2006 Paul.  The primary data binding will now only occur in the ASPX pages so that this is only one per cycle. 
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
			m_sMODULE = "Help";
		}
		#endregion
	}
}

