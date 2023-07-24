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
using System.Xml;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Charts
{
	/// <summary>
	///		Summary description for DashletChart.
	/// </summary>
	public class DashletChart : DashletControl
	{
		protected _controls.DashletHeader  ctlDashletHeader;
		protected _controls.SearchView     ctlSearchView   ;
		protected ChartView                ctlChartView    ;

		protected Guid               gID       ;
		protected string             sChartName;
		protected string             sReportSQL;
		protected Label              lblError  ;
		protected bool               bShowEditDialog = false;
		protected string             sDivEditName = "my_charts_edit";

		// 04/10/2011 Paul.  Allow the report dashlets to add fields. 
		private void ctlSearchView_EventViewLoaded(DataTable dtSearchView)
		{
			// 05/03/2011 Paul.  We need to include the USER_ID because we cache the Assigned User ID and the Team ID. 
			// 02/03/2012 Paul.  Use ChartParametersEditView instead of ReportParametersEditView. 
			DataTable dtReportParameters = SplendidCache.ChartParametersEditView(gID, Security.USER_ID);
			if ( dtReportParameters != null && dtReportParameters.Rows.Count > 0 )
			{
				foreach ( DataRow rowParameters in dtReportParameters.Rows )
				{
					DataRow rowNew = dtSearchView.NewRow();
					dtSearchView.Rows.Add(rowNew);
					foreach ( DataColumn col in dtReportParameters.Columns )
					{
						if ( dtSearchView.Columns.Contains(col.ColumnName) )
							rowNew[col.ColumnName] = rowParameters[col.ColumnName];
					}
				}
			}
		}

		private void ctlSearchView_SavedSearchApplied(XmlDocument xml)
		{
			if ( !IsPostBack )
			{
				// 05/03/2011 Paul.  We need to include the USER_ID because we cache the Assigned User ID and the Team ID. 
				// 02/03/2012 Paul.  Use ChartParameters instead of ReportParameters. 
				DataTable dtReportParameters = SplendidCache.ChartParameters(gID, Security.USER_ID);
				foreach ( DataRow rowParameter in dtReportParameters.Rows )
				{
					string sDATA_FIELD    = Sql.ToString(rowParameter["NAME"         ]);
					string sDEFAULT_VALUE = Sql.ToString(rowParameter["DEFAULT_VALUE"]);
					if ( !Sql.IsEmptyString(sDEFAULT_VALUE) )
					{
						// 04/15/2011 Paul.  If the field does not exist in the saved search, then set its value. 
						XmlNode xField = xml.DocumentElement.SelectSingleNode("SearchFields/Field[@Name='" + sDATA_FIELD + "']");
						if ( xField == null )
						{
							DynamicControl ctl = new DynamicControl(ctlSearchView, sDATA_FIELD);
							if ( ctl.Exists )
							{
								// 04/16/2011 Paul.  When the report first loads, use the default value to populate. 
								// This should work with multi-selection listboxes because we convert multiple default values to XML. 
								// 07/26/2012 Paul.  Setting a list control by index is not working due to a problem with SelectedIndex. 
								// Just for reports and charts, change to selection by value. 
								DropDownList lst = ctlSearchView.FindControl(sDATA_FIELD) as DropDownList;
								if ( lst != null )
								{
									Utils.SetSelectedValue(lst, sDEFAULT_VALUE);
								}
								else
								{
									ctl.Text = sDEFAULT_VALUE;
								}
							}
						}
					}
				}
			}
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Search" )
				{
					bShowEditDialog = true;
					Bind(true);
					// 01/24/2010 Paul.  Update the Dashlet title to the report name. 
					// 04/09/2011 Paul.  The report no longer changes, so we don't need to update the name. 
					//SqlProcs.spDASHLETS_USERS_UpdateTitle(gDashletID, sReportName);
				}
				else if ( e.CommandName == "Refresh" )
				{
					Bind(true);
				}
				else if ( e.CommandName == "Remove" )
				{
					if ( !Sql.IsEmptyString(sDetailView) )
					{
						// 01/24/2010 Paul.  The Report Dashlet will allow multiple instances, so disable by ID. 
						SqlProcs.spDASHLETS_USERS_Disable(gDashletID);
						SplendidCache.ClearUserDashlets(sDetailView);
						Response.Redirect(Page.AppRelativeVirtualPath + Request.Url.Query);
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		protected void Bind(bool bBind)
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					// 04/06/2011 Paul.  We must keep the call to SqlSearchClause so that the EditView fields will get initialized. 
					ctlSearchView.SqlSearchClause(cmd);
				}
			}
			// 04/06/2011 Paul.  Cache reports. 
			// 04/10/2011 Paul.  Instead of pulling the report ID from the SearchView data, just use the DashletID. 
			DataTable dtChart = SplendidCache.Chart(gID);
			if ( dtChart.Rows.Count > 0 )
			{
				DataRow rdr = dtChart.Rows[0];
				sChartName = Sql.ToString(rdr["NAME"]);
				ViewState["ChartName"] = sChartName;
				string sXML = Sql.ToString(rdr["RDL"]);
				// 01/19/2010 Paul.  The Module Name is needed in order to apply ACL Field Security. 
				string sMODULE_NAME = Sql.ToString(rdr["MODULE_NAME"]);
				try
				{
					if ( !Sql.IsEmptyString(sXML) )
					{
						ctlChartView.RunReport(sXML, sMODULE_NAME);
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			this.Visible = this.Visible && (SplendidCRM.Security.GetUserAccess(m_sMODULE, "list") >= 0);
			if ( !this.Visible )
				return;

			try
			{
				// 09/23/2010 Paul.  Make sure not to rebind on every page request as it will prevent pagination. 
				bool bIsPostBack = this.IsPostBack && !NotPostBack;
				if ( !bIsPostBack )
				{
					Bind(!IsPostBack);
					sChartName = Sql.ToString(ViewState["ChartName"]);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
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
			ctlDashletHeader.Command         += new CommandEventHandler(Page_Command);
			ctlSearchView.Command            += new CommandEventHandler(Page_Command);
			ctlSearchView.EditViewLoaded     += new SplendidCRM._controls.SearchView.SearchViewEditViewLoad(ctlSearchView_EventViewLoaded);
			ctlSearchView.SavedSearchApplied += new SplendidCRM._controls.SearchView.SearchViewSavedSearchApplied(ctlSearchView_SavedSearchApplied);
			m_sMODULE = "Charts";
			ctlSearchView.DynamicSearch = gDashletID.ToString();
			sDivEditName = "my_charts_edit_" + gDashletID.ToString().Replace('-', '_');

			try
			{
			// 04/10/2011 Paul.  We need to read the saved search data here because we need the report ID before SearchView loads the data. 
				string sDYNAMIC_SEARCH_VIEW = "Charts.SearchDashlet." + gDashletID.ToString();
				// 04/10/2011 Paul.  Cache the Report ID for the dashlet as it will not change. 
				gID = Sql.ToGuid(Session[sDYNAMIC_SEARCH_VIEW]);
				if ( Sql.IsEmptyGuid(gID) )
				{
					DataView vwSavedSearches = new DataView(SplendidCache.SavedSearch(sDYNAMIC_SEARCH_VIEW));
					vwSavedSearches.RowFilter = "NAME is null";
					if ( vwSavedSearches.Count > 0 )
					{
						string sXML = Sql.ToString(vwSavedSearches[0]["CONTENTS"]);
						//ApplySavedSearch(sXML);
						if ( !Sql.IsEmptyString(sXML) )
						{
							XmlDocument xml = new XmlDocument();
							// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
							// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
							// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
							xml.XmlResolver = null;
							xml.LoadXml(sXML);
							XmlNode xID = xml.DocumentElement.SelectSingleNode("SearchFields/Field[@Name='ID']");
							if ( xID != null )
							{
								gID = Sql.ToGuid(xID.InnerText);
								Session[sDYNAMIC_SEARCH_VIEW] = gID;
							}
						}
					}
				}
			}
			catch
			{
			}
			// 04/10/2011 Paul.  Initialize is happening too early.  When we are tracking the load, then we need to defer the initialize. 
			if ( IsPostBack )
			{
				ctlSearchView.InitializeDynamicView();
			}
		}
		#endregion
	}
}
