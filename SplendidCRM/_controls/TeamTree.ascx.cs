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
using System.Web.UI.WebControls;
using SplendidCRM;
using System.Collections.Generic;
using System.Diagnostics;

namespace SplendidCRM._controls
{
	/// <summary>
	///		Summary description for TeamTree.
	/// </summary>
	public class TeamTree : SplendidControl
	{
		protected TreeView      treeMain     ;
		protected XmlDataSource xdsMain      ;
		protected Label         txtScope     ;

		protected void treeMain_SelectedNodeChanged(Object sender, EventArgs e)
		{
			string sNAME = treeMain.SelectedNode.Text;
			Guid   gID   = Guid.Empty;
			// 02/23/2017 Paul.  The Value will match the Text for the root node. 
			if ( treeMain.SelectedNode.Value != treeMain.SelectedNode.Text )
				gID = Sql.ToGuid(treeMain.SelectedNode.Value);
			
			XmlDocument xml = new XmlDocument();
			xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
			XmlNode xSavedSearch = xml.CreateElement("SavedSearch");
			xml.AppendChild(xSavedSearch);
				XmlNode xSearchFields = xml.CreateElement("SearchFields");
				xSavedSearch.AppendChild(xSearchFields);
					XmlNode xFieldNAME = xml.CreateElement("Field");
					xSearchFields.AppendChild(xFieldNAME);
						XmlUtil.SetSingleNodeAttribute(xml, xFieldNAME, "Name", "NAME"   );
						XmlUtil.SetSingleNodeAttribute(xml, xFieldNAME, "Type", "TextBox");
						xFieldNAME.InnerText = sNAME;
					
					XmlNode xFieldVALUE = xml.CreateElement("Field");
					xSearchFields.AppendChild(xFieldVALUE);
						XmlUtil.SetSingleNodeAttribute(xml, xFieldVALUE, "Name", "ID"     );
						XmlUtil.SetSingleNodeAttribute(xml, xFieldVALUE, "Type", "TextBox");
						xFieldVALUE.InnerText = gID.ToString();
			
			string sXML             = xml.OuterXml;
			Guid   gSAVED_SEARCH_ID = Guid.Empty;
			// 01/05/2020 Paul.  Provide central location for constant. 
			const string sSEARCH_MODULE = Security.TeamHierarchyModule;
			SqlProcs.spSAVED_SEARCH_Update(ref gSAVED_SEARCH_ID, Security.USER_ID, String.Empty, sSEARCH_MODULE, sXML, String.Empty, Guid.Empty);
			SplendidCache.ClearSavedSearch(sSEARCH_MODULE);
			
			Page.Items["TeamTree.TEAM_NAME"] = sNAME;
			Page.Items["TeamTree.TEAM_ID"  ] = gID  ;
			txtScope.Text = sNAME;
			Response.Redirect(Request.RawUrl);
		}

		protected void Bind()
		{
			try
			{
				DataSet dsMain = new DataSet("TeamTree");
				XmlDocument xml = new XmlDocument();
				SplendidCRM.DbProviderFactory dbf = SplendidCRM.DbProviderFactories.GetFactory();
				using (IDbConnection con = dbf.CreateConnection())
				{
					string sSQL;
					// 08/12/2017 Paul.  Fix replacement to &amp;. 
					// 01/08/2018 Paul.  Change the name from Teams. 
					string sRootName = L10n.Term("Teams.LBL_TEAM_TREE_ROOT").Replace("'", "");
					sSQL = "select TEAMS.ID                          as '@ID'       " + ControlChars.CrLf
					     + "     , replace(TEAMS.NAME, '&', '&amp;') as '@NAME'     " + ControlChars.CrLf
					     + "     , TEAMS.PARENT_ID                   as '@PARENT_ID'" + ControlChars.CrLf
					     + "     , dbo.fnTEAM_HIERARCHY_ChildrenXml(TEAMS.ID)       " + ControlChars.CrLf
					     + "  from      TEAMS                                       " + ControlChars.CrLf
					     + " inner join TEAM_MEMBERSHIPS                            " + ControlChars.CrLf
					     + "         on TEAM_MEMBERSHIPS.TEAM_ID = TEAMS.ID         " + ControlChars.CrLf
					     + " where TEAM_MEMBERSHIPS.USER_ID         = @USER_ID      " + ControlChars.CrLf
					     + "   and TEAM_MEMBERSHIPS.EXPLICIT_ASSIGN = 1             " + ControlChars.CrLf
					     + "   and TEAM_MEMBERSHIPS.DELETED         = 0             " + ControlChars.CrLf
					     + "   and TEAMS.PRIVATE = 0                                " + ControlChars.CrLf
					     + "   and TEAMS.DELETED = 0                                " + ControlChars.CrLf
					     + " order by '@NAME'                                       " + ControlChars.CrLf
					     + "   for xml path('TEAM'), root('" + sRootName + "'), type" + ControlChars.CrLf;
					using (IDbCommand cmd = con.CreateCommand())
					{
						con.Open();
						cmd.CommandText = sSQL;
						Guid gUSER_ID = Security.USER_ID;
						Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							StringBuilder sbXML = new StringBuilder();
							using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
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
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			bool bEnableTeamManagement = Crm.Config.enable_team_management();
			bool bEnableTeamHierarchy  = Crm.Config.enable_team_hierarchy();
			this.Visible = bEnableTeamManagement && bEnableTeamHierarchy;
			if ( bEnableTeamManagement && bEnableTeamHierarchy )
			{
				try
				{
					Bind();
					
					Guid   gTEAM_ID   = Guid.Empty;
					string sTEAM_NAME = String.Empty;
					Security.TeamHierarchySavedSearch(ref gTEAM_ID, ref sTEAM_NAME);
					
					Page.Items["TeamTree.TEAM_ID"  ] = gTEAM_ID.ToString();
					Page.Items["TeamTree.TEAM_NAME"] = sTEAM_NAME;
					txtScope.Text = sTEAM_NAME;
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
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
		}
		#endregion
	}
}
