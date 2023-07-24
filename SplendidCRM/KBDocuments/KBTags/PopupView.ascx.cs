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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.KBDocuments.KBTags
{
	/// <summary>
	///		Summary description for PopupView.
	/// </summary>
	public class PopupView : SplendidControl
	{
		protected _controls.SearchView     ctlSearchView    ;
		protected _controls.DynamicButtons ctlDynamicButtons;
		protected _controls.CheckAll       ctlCheckAll      ;

		protected UniqueStringCollection arrSelectFields;
		protected DataView      vwMain         ;
		protected SplendidGrid  grdMain        ;
		protected bool          bMultiSelect   ;

		protected TreeView      treeMain       ;
		protected XmlDataSource xdsMain        ;
		protected XmlDocument   xml = new XmlDocument();

		public bool MultiSelect
		{
			get { return bMultiSelect; }
			set { bMultiSelect = value; }
		}

		public int ExpandDepth
		{
			get { return treeMain.ExpandDepth; }
			set { treeMain.ExpandDepth = value; }
		}

		protected void treeMain_TreeNodeDataBound(object sender, TreeNodeEventArgs e)
		{
			if ( e.Node.Value != "Tags" )
			{
				XmlElement xKBTag = e.Node.DataItem as XmlElement;
				int nARTICLE_COUNT = Sql.ToInteger(XmlUtil.GetNamedItem(xKBTag, "ARTICLE_COUNT"));
				string sDISPLAY_NAME = e.Node.Text;
				if ( nARTICLE_COUNT > 0 )
				{
					sDISPLAY_NAME += " (" + nARTICLE_COUNT + ")";
				}
				e.Node.Text = @"<span onclick=""javascript:SelectKBTag('" + e.Node.Value + @"', '" + Sql.EscapeJavaScript(e.Node.Text) + @"')"">" + sDISPLAY_NAME + "</span>";
			}
			else
			{
				e.Node.SelectAction = TreeNodeSelectAction.None;
			}
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Search" )
				{
					// 10/13/2005 Paul.  Make sure to clear the page index prior to applying search. 
					grdMain.CurrentPageIndex = 0;
					// 04/27/2008 Paul.  Sorting has been moved to the database to increase performance. 
					grdMain.DataBind();
				}
				// 12/14/2007 Paul.  We need to capture the sort event from the SearchView. 
				else if ( e.CommandName == "SortGrid" )
				{
					grdMain.SetSortFields(e.CommandArgument as string[]);
					// 04/27/2008 Paul.  Sorting has been moved to the database to increase performance. 
					// 03/17/2011 Paul.  We need to treat a comma-separated list of fields as an array. 
					arrSelectFields.AddFields(grdMain.SortColumn);
				}
				// 11/17/2010 Paul.  Populate the hidden Selected field with all IDs. 
				else if ( e.CommandName == "SelectAll" )
				{
					ctlCheckAll.SelectAll(vwMain, "ID");
					grdMain.DataBind();
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
			// 07/05/2009 Paul.  We don't use access control on the team list as all users can assign a record to any team. 
			//this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "list") >= 0);
			if ( !this.Visible )
				return;

			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					/*
					sSQL = "  from vwKBTAGS_List" + ControlChars.CrLf
					     + " where 1 = 1        " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						// 04/27/2008 Paul.  A ListView will need to set and build the order clause in two setps 
						// so that the SavedSearch sort value can be taken into account. 
						grdMain.OrderByClause("TAG_NAME", "asc");
						ctlSearchView.SqlSearchClause(cmd);
						// 04/27/2008 Paul.  The fields in the search clause need to be prepended after any Saved Search sort has been determined.
						cmd.CommandText = "select " + Sql.FormatSelectFields(arrSelectFields)
						                + cmd.CommandText
						                + grdMain.OrderByClause();

						if ( bDebug )
							Page.ClientScript.RegisterClientScriptBlock(System.Type.GetType("System.String"), "SQLCode", Sql.ClientScriptBlock(cmd));

						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								// 11/06/2012 Paul.  Apply Business Rules to PopupView. 
								this.ApplyGridViewRules(m_sMODULE + "." + LayoutListView, dt);
								
								vwMain = dt.DefaultView;
								grdMain.DataSource = vwMain ;
								if ( !IsPostBack )
								{
									// 12/14/2007 Paul.  Only set the default sort if it is not already set.  It may have been set by SearchView. 
									// 04/27/2008 Paul.  Sorting has been moved to the database to increase performance. 
									grdMain.DataBind();
								}
							}
						}
					}
					*/
					if ( Sql.IsOracle(con) )
					{
						sSQL = "select xmlelement( \"KBTAG\", xmlattributes( ID            as \"ID\"           " + ControlChars.CrLf
						     + "                                           , TAG_NAME      as \"TAG_NAME\"     " + ControlChars.CrLf
						     + "                                           , PARENT_TAG_ID as \"PARENT_TAG_ID\"" + ControlChars.CrLf
						     + "                                           )                                   " + ControlChars.CrLf
						     + "                            , (select xmlagg(xmlelement( \"KBTAG\", xmlattributes( vwKBTAGS_CHILD.ID           as \"ID\"           " + ControlChars.CrLf
						     + "                                                                                , vwKBTAGS_CHILD.TAG_NAME      as \"TAG_NAME\"     " + ControlChars.CrLf
						     + "                                                                                , vwKBTAGS_CHILD.PARENT_TAG_ID as \"PARENT_TAG_ID\"" + ControlChars.CrLf
						     + "                                                                                )    " + ControlChars.CrLf
						     + "                                                       )                             " + ControlChars.CrLf
						     + "                                             order by vwKBTAGS_CHILD.TAG_NAME asc    " + ControlChars.CrLf
						     + "                                            )                                        " + ControlChars.CrLf
						     + "                                 from vwKBTAGS_List vwKBTAGS_CHILD                   " + ControlChars.CrLf
						     + "                                where vwKBTAGS_CHILD.PARENT_TAG_ID = vwKBTAGS_List.ID" + ControlChars.CrLf
						     + "                              )          " + ControlChars.CrLf
						     + "                 )                       " + ControlChars.CrLf
						     + "                                         " + ControlChars.CrLf
						     + "  from vwKBTAGS_List                                 " + ControlChars.CrLf
						     + " where 1 = 1                                         " + ControlChars.CrLf;
					}
					else
					{
						sSQL = "select ID                                   as '@ID'           " + ControlChars.CrLf
						     + "     , TAG_NAME                             as '@TAG_NAME'     " + ControlChars.CrLf
						     + "     , PARENT_TAG_ID                        as '@PARENT_TAG_ID'" + ControlChars.CrLf
						     + "     , ARTICLE_COUNT                        as '@ARTICLE_COUNT'" + ControlChars.CrLf
						     + "     , dbo.fnKBTAGS_ChildrenXml(ID)                            " + ControlChars.CrLf
						     + "  from vwKBTAGS_List                                           " + ControlChars.CrLf
						     + " where 1 = 1                                                   " + ControlChars.CrLf;
					}
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						con.Open();
						cmd.CommandText = sSQL;
						ctlSearchView.SqlSearchClause(cmd);
						if ( Sql.IsOracle(con) )
						{
							cmd.CommandText += " order by \"TAG_NAME\"" + ControlChars.CrLf;
						}
						else
						{
							cmd.CommandText += " order by '@TAG_NAME'                       " + ControlChars.CrLf;
							cmd.CommandText += "   for xml path('KBTAG'), root('Tags'), type" + ControlChars.CrLf;
						}
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;

							if ( bDebug )
								RegisterClientScriptBlock("xdsMain", Sql.ClientScriptBlock(cmd));

							StringBuilder sbXML = new StringBuilder();
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
								// 08/19/2010 Paul.  It is painful to use Oracle to generate the root tag, so just manually add it. 
								if ( Sql.IsOracle(con) )
									sXML = "<Tags>" + sXML + "</Tags>";
								// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
								// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
								// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
								xml.XmlResolver = null;
								// 10/23/2009 Paul.  We don't need the XML document, but it will catch errors. 
								xml.LoadXml(sXML);
								xdsMain.Data = sXML;
								treeMain.DataSourceID = "xdsMain";
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
			if ( !IsPostBack )
			{
				// 03/11/2008 Paul.  Move the primary binding to SplendidPage. 
				//Page DataBind();
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
			ctlDynamicButtons.Command += new CommandEventHandler(Page_Command);
			ctlSearchView    .Command += new CommandEventHandler(Page_Command);
			ctlCheckAll      .Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "KBTags";
			// 07/26/2007 Paul.  Use the new PopupView so that the view is customizable. 
			// 02/08/2008 Paul.  We need to build a list of the fields used by the search clause. 
			arrSelectFields = new UniqueStringCollection();
			// 05/04/2017 Paul.  ID is a required field. 
			arrSelectFields.Add("ID"  );
			arrSelectFields.Add("TAG_NAME");
			this.AppendGridColumns(grdMain, m_sMODULE + ".PopupView", arrSelectFields);
			// 04/29/2008 Paul.  Make use of dynamic buttons. 
			ctlDynamicButtons.AppendButtons(m_sMODULE + ".Popup" + (bMultiSelect ? "MultiSelect" : "View"), Guid.Empty, Guid.Empty);
			if ( !IsPostBack && !bMultiSelect )
				ctlDynamicButtons.ShowButton("Clear", !Sql.ToBoolean(Request["ClearDisabled"]));
		}
		#endregion
	}
}
