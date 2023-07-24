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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Diagnostics;
using System.Xml;

namespace SplendidCRM
{
	public class CreateItemTemplateReportFilterList : ITemplate
	{
		protected string       sDATA_FIELD;
		protected DropDownList lst        ;
		protected DataTable    dt         ;
		
		public CreateItemTemplateReportFilterList(string sDATA_FIELD)
		{
			this.sDATA_FIELD = sDATA_FIELD;
		}

		public void InstantiateIn(Control objContainer)
		{
			lst = new DropDownList();
			objContainer.Controls.Add(lst);
			lst.DataBinding += new EventHandler(lst_OnDataBinding);
		}
		private void lst_OnDataBinding(object sender, EventArgs e)
		{
			DataGridItem     objContainer = (DataGridItem) lst.NamingContainer;
			DataRowView      row = objContainer.DataItem as DataRowView;
			ReportFilterGrid grd = objContainer.Parent.Parent as ReportFilterGrid;
			if ( row != null )
			{
				// 04/25/2006 Paul.  We always need to translate the items, even during postback.
				// This is because we always build the DropDownList. 
				// 04/30/2006 Paul.  Use the Context to store pointers to the localization objects.
				// This is so that we don't need to require that the page inherits from SplendidPage. 
				L10N L10n = HttpContext.Current.Items["L10n"] as L10N;
				if ( L10n == null )
				{
					// 04/26/2006 Paul.  We want to have the AccessView on the SystemCheck page. 
					L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				}
				if ( row[sDATA_FIELD] != DBNull.Value )
				{
					string sID     = Sql.ToString(row["ID"         ]);
					string sMODULE = Sql.ToString(row["MODULE_NAME"]);
					lst.ID = sDATA_FIELD + "_" + sID;
					try
					{
						if ( sDATA_FIELD == "MODULE_NAME" )
						{
							XmlDocument xml = grd.Rdl;
							/*
							// 06/20/2006 Paul.  New RdlDocument handles custom properties. 
							string sRelationships = RdlUtil.GetCustomProperty(xml.DocumentElement, "Relationships");
							if ( !Sql.IsEmptyString(sRelationships) )
							{
								XmlDocument xmlRelationship = new XmlDocument();
								xmlRelationship.LoadXml(sRelationships);
								dt = XmlUtil.CreateDataTable(xmlRelationship.DocumentElement, "Relationship", new string[] {"MODULE_NAME", "DISPLAY_NAME"});
								lst.AutoPostBack = true;
								foreach ( DataRow rowRelationship in dt.Rows )
								{
									lst.Items.Add(new ListItem(Sql.ToString(rowRelationship["DISPLAY_NAME"]), Sql.ToString(rowRelationship["MODULE_NAME"])));
								}
							}
							*/
						}
						else if ( sDATA_FIELD == "DATA_FIELD" )
						{
							DbProviderFactory dbf = DbProviderFactories.GetFactory();
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								string sSQL;
								// 02/29/2008 Niall.  Some SQL Server 2005 installations require matching case for the parameters. 
								// Since we force the parameter to be uppercase, we must also make it uppercase in the command text. 
								sSQL = "select ColumnName as NAME              " + ControlChars.CrLf
								     + "     , ColumnName as DISPLAY_NAME      " + ControlChars.CrLf
								     + "  from vwSqlColumns                    " + ControlChars.CrLf
								     + " where ObjectName = @OBJECTNAME        " + ControlChars.CrLf
								     + "   and ColumnName not in ('ID', 'ID_C')" + ControlChars.CrLf
								     + "   and ColumnName not like '%_ID'      " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									DropDownList lstMODULE_NAME = null;
									// 05/28/2006 Paul.  Not sure why, but grd.FindFilterControl() does not work. 
									foreach(DataGridItem itm in objContainer.Parent.Controls)
									{
										lstMODULE_NAME = itm.FindControl("MODULE_NAME" + "_" + sID) as DropDownList;
										if ( lstMODULE_NAME != null )
											break;
									}
									string sMODULE_NAME = lstMODULE_NAME.SelectedValue;
									string[] arrModule = sMODULE_NAME.Split(' ');
									string sModule     = arrModule[0];
									string sTableAlias = arrModule[0];
									if ( arrModule.Length > 1 )
										sTableAlias = arrModule[1].ToUpper();
									// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
									Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, "vw" + sModule));

									using ( DbDataAdapter da = dbf.CreateDataAdapter() )
									{
										( (IDbDataAdapter) da ).SelectCommand = cmd;
										dt = new DataTable();
										da.Fill(dt);
										foreach ( DataRow rowColumn in dt.Rows )
										{
											rowColumn["NAME"        ] = sMODULE + "." + Sql.ToString(rowColumn["NAME"]);
											rowColumn["DISPLAY_NAME"] = L10n.Term(sModule + ".LBL_" + Sql.ToString(rowColumn["DISPLAY_NAME"])).Replace(":", "");
										}
										DataView vwColumns = new DataView(dt);
										vwColumns.Sort = "DISPLAY_NAME";
										foreach ( DataRowView rowColumn in vwColumns )
										{
											lst.Items.Add(new ListItem(Sql.ToString(rowColumn["DISPLAY_NAME"]), Sql.ToString(rowColumn["NAME"])));
										}
									}
								}
							}
						}
					}
					catch
					{
					}
					try
					{
						// 04/25/2006 Paul.  Don't update values on postback, otherwise it will over-write modified values. 
						if ( !objContainer.Page.IsPostBack )
						{
							// 08/19/2010 Paul.  Check the list before assigning the value. 
							Utils.SetSelectedValue(lst, Sql.ToString(row[sDATA_FIELD]));
						}
					}
					catch
					{
					}
				}
				
				/*
				// 04/25/2006 Paul.  Make sure to translate the text.  
				// It cannot be translated in InstantiateIn() because the Page is not defined. 
				foreach(ListItem itm in lst.Items )
				{
					itm.Text = L10n.Term(itm.Text);
				}
				*/
			}
		}
	}

	public class CreateItemTemplateReportFilterText : ITemplate
	{
		protected string       sDATA_FIELD;
		protected TextBox      txt        ;
		
		public CreateItemTemplateReportFilterText(string sDATA_FIELD)
		{
			this.sDATA_FIELD = sDATA_FIELD;
		}

		public void InstantiateIn(Control objContainer)
		{
			txt = new TextBox();
			objContainer.Controls.Add(txt);
			txt.DataBinding += new EventHandler(txt_OnDataBinding);
		}
		private void txt_OnDataBinding(object sender, EventArgs e)
		{
			DataGridItem     objContainer = (DataGridItem) txt.NamingContainer;
			DataRowView      row = objContainer.DataItem as DataRowView;
			ReportFilterGrid grd = objContainer.Parent.Parent as ReportFilterGrid;
			
			if ( row != null )
			{
				if ( row[sDATA_FIELD] != DBNull.Value )
				{
					string sID     = Sql.ToString(row["ID"         ]);
					string sMODULE = Sql.ToString(row["MODULE_NAME"]);
					txt.ID = sDATA_FIELD + "_" + sID;
					try
					{
						// 04/25/2006 Paul.  Don't update values on postback, otherwise it will over-write modified values. 
						if ( !objContainer.Page.IsPostBack )
						{
							txt.Text = Sql.ToString(row[sDATA_FIELD]);
						}
					}
					catch
					{
					}
				}
			}
		}
	}

	/// <summary>
	/// Summary description for ACLGrid.
	/// </summary>
	public class ReportFilterGrid : SplendidGrid
	{
		protected XmlDocument xml;

		public ReportFilterGrid()
		{
			this.Init += new EventHandler(OnInit);
		}

		public XmlDocument Rdl
		{
			get { return xml; }
			set { xml = value; }
		}

		public DataBoundControl FindFilterControl(string sID, string sDATA_FIELD)
		{
			DataBoundControl ctl = null;
			foreach(DataGridItem itm in Items)
			{
				ctl = itm.FindControl(sDATA_FIELD + "_" + sID) as DataBoundControl;
				if ( ctl != null )
					break;
			}
			return ctl;
		}

		private void AppendFilterColumn(string sDATA_FIELD, string sTYPE)
		{
			TemplateColumn tpl = new TemplateColumn();
			tpl.ItemStyle.Width             = new Unit("12%");
			//tpl.ItemStyle.CssClass          = "tabDetailViewDF";
			tpl.ItemStyle.HorizontalAlign   = HorizontalAlign.NotSet;
			tpl.ItemStyle.VerticalAlign     = VerticalAlign.NotSet  ;
			tpl.ItemStyle.Wrap              = false;
			tpl.HeaderText = sDATA_FIELD;
			if ( sTYPE == "DropDownList" )
				tpl.ItemTemplate   = new CreateItemTemplateReportFilterList(sDATA_FIELD);
			else if ( sTYPE == "TextBox" )
				tpl.ItemTemplate   = new CreateItemTemplateReportFilterText(sDATA_FIELD);
			this.Columns.Add(tpl);
		}

		protected void OnInit(object sender, System.EventArgs e)
		{
			this.Width = new Unit();
			AppendFilterColumn("MODULE_NAME" , "DropDownList");
			AppendFilterColumn("DATA_FIELD"  , "DropDownList");
			AppendFilterColumn("OPERATOR"    , "DropDownList");
			AppendFilterColumn("SEARCH_FIELD", "TextBox"     );
		}

	}
}

