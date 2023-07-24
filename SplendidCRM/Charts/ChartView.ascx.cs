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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace SplendidCRM.Charts
{
	/// <summary>
	///		Summary description for ChartView.
	/// </summary>
	public class ChartView : SplendidControl
	{
		protected Label        lblError ;
		protected string       sReportSQL;
		protected Reports.ParameterView  ctlParameterView ;
		protected string       sSeriesData;
		protected string       sChartData ;

		public class ChartData
		{
			public int?   width ;
			public int?   height;
			public class Title
			{
				public string text;
				public bool?  show;
				public string fontFamily;
				public string fontSize;
				public string fontAlign;
				public string fontColor;
			}
			public Title title;
			public class Cursor
			{
				public bool? show;
				public bool? zoom;
			}
			[DataMember(IsRequired=false)]
			public Cursor cursor;
			public class SeriesDefaults
			{
				public string renderer;
				public class RendererOptions
				{
					public int?     barPadding;
					public int?     barMargin;
					public string   barDirection;
					public int?     barWidth;
					public int?     shadowOffset;
					public int?     shadowDepth;
					public int?     shadowAlpha;
					public bool?    waterfall;
					public int?     groups;
					public bool?    varBarColor;
					public bool?    highlightMouseOver;
					public bool?    highlightMouseDown;
					public bool?    fillToZero;
					public bool?    showDataLabels;
					public string[] highlightColors;
				}
				public RendererOptions rendererOptions;
				public SeriesDefaults()
				{
					rendererOptions = new RendererOptions();
				}
			}
			public SeriesDefaults seriesDefaults;
			public class Series
			{
				public string label;
			}
			public Series[] series;
			public class Legend
			{
				public bool?  show;
				public string placement;
				public string location;
			}
			public Legend legend;
			public class Axes
			{
				public class Axis
				{
					public bool?  show;
					public string tickRenderer;
					public string labelRenderer;
					public string label;
					public bool?  showLabel;
					public int?   min;
					public int?   max;
					public bool?  autoscale;
					public string renderer;
					public class TickOptions
					{
						public string formatString;
					}
					public TickOptions tickOptions;
					public bool?  showTicks;
					public bool?  showTicksMarks;
					public bool?  showMinorTicks;
					public int?   borderWidth;
					public Axis()
					{
						tickOptions = new TickOptions();
					}
				}
				public Axis xaxis;
				public Axis yaxis;
				public Axes()
				{
					xaxis = new Axis();
					yaxis = new Axis();
				}
			}
			public Axes axes;

			public ChartData()
			{
				title          = new Title();
				cursor         = new Cursor();
				seriesDefaults = new SeriesDefaults();
				series         = new Series[1];
				legend         = new Legend();
				axes           = new Axes();
			}
		}

		private void RemoveNulls(Dictionary<string, object> dict)
		{
			List<string> arrDelete = new List<string>();
			foreach ( string key in dict.Keys )
			{
				if ( dict[key] == null )
				{
					arrDelete.Add(key);
				}
				else if ( dict[key].GetType() == typeof(Dictionary<string, object>) )
				{
					Dictionary<string, object> sub = dict[key] as Dictionary<string, object>;
					RemoveNulls(sub);
					if ( sub.Count == 0 )
					{
						arrDelete.Add(key);
					}
				}
				else if ( dict[key].GetType() == typeof(ArrayList) )
				{
					ArrayList sub = dict[key] as ArrayList;
					for ( int i = sub.Count - 1; i >= 0; i-- )
					{
						if ( sub[i] == null )
							sub.RemoveAt(i);
					}
					if ( sub.Count == 0 )
					{
						arrDelete.Add(key);
					}
				}
			}
			foreach ( string key in arrDelete )
			{
				dict.Remove(key);
			}
		}

		public string GetSeriesData()
		{
			return Sql.IsEmptyString(sSeriesData) ? "[]" : sSeriesData;
		}

		public string GetChartData()
		{
			return Sql.IsEmptyString(sChartData) ? "{}" : sChartData;
		}

		public string ReportSQL
		{
			get { return sReportSQL; }
		}

		protected void UpdateChartData(RdlDocument rdl, DataSet ds)
		{
			sSeriesData = String.Empty;
			sChartData  = String.Empty;

			JavaScriptSerializer json = new JavaScriptSerializer();
			json.MaxJsonLength = 20 * 1024 * 1024;
			if ( ds.Tables.Count > 0 )
			{
				DataTable dt = ds.Tables[0];
				
				ChartData data = new ChartData();
				data.width = 600;
				data.height = data.width / 2;
				// http://www.jqplot.com/docs/files/plugins/jqplot-barRenderer-js.html

				/*
				string sPageHeight      = rdl.SelectNodeValue("Page/PageHeight"   );
				string sPageWidth       = rdl.SelectNodeValue("Page/PageWidth"    );
				string sTopMargin       = rdl.SelectNodeValue("Page/TopMargin"    );
				string sBottomMargin    = rdl.SelectNodeValue("Page/BottomMargin" );
				string sLeftMargin      = rdl.SelectNodeValue("Page/LeftMargin"   );
				string sRightMargin     = rdl.SelectNodeValue("Page/RightMargin"  );
				string sAutoRefresh     = rdl.SelectNodeValue("AutoRefresh"       );
				string sDataSource      = rdl.SelectNodeAttribute("DataSources/DataSource", "Name");
				string sDataSet         = rdl.SelectNodeAttribute("DataSets/DataSet", "Name");
				string sReportUnitType  = rdl.SelectNodeValue("rd:ReportUnitType");
				string sReportID        = rdl.SelectNodeValue("rd:ReportID");
				string sReportWidth     = rdl.SelectNodeValue("Width");
				XmlNode xDataSource     = rdl.SelectNode("DataSources/DataSource[@Name='" + sDataSource + "']");
				XmlNode xDataSources    = rdl.SelectNode("DataSources");
				XmlNode xDataSets       = rdl.SelectNode("DataSets");
				*/
				
				// http://www.jqplot.com/docs/files/jqplot-core-js.html
				// ReportSections/ReportSection/ is part of the RDL 2010 definition, but we remove that to create RDL 2008 definition. 
				
				// Chart Size
				float flWidth  = Sql.ToFloat(rdl.SelectNodeValue("Body/ReportItems/Chart/Width" ).Replace("in", ""));
				if ( flWidth > 0 )
					data.width = Sql.ToInteger(flWidth * 100);
				data.height = data.width / 2;
				float flHeight = Sql.ToFloat(rdl.SelectNodeValue("Body/ReportItems/Chart/Height").Replace("in", ""));
				if ( flHeight > 0 )
					data.height = Sql.ToInteger(flHeight * 100);
				
				// Chart Title
				data.title.text = rdl.SelectNodeValue("Body/ReportItems/Chart/ChartTitles/ChartTitle/Caption");
				data.title.show = !Sql.IsEmptyString(data.title.text);
				
				// Chart Legend
				if ( rdl.SelectNode("Body/ReportItems/Chart/ChartLegends/ChartLegend") != null )
				{
					data.legend.show      = true;
					//data.legend.placement = "outsideGrid";
					//data.legend.location = "e";
				}
				
				// Chart Type
				string sChartType = rdl.SelectNodeValue("Body/ReportItems/Chart/ChartData/ChartSeriesCollection/ChartSeries/Type");
				// 03/12/2012 Paul.  An early release used Columns and not Column and broke compatibility with MS Report Builder 3.0. 
				if ( sChartType == "Columns" )
					sChartType = "Column";

				string sYField            = rdl.SelectNodeValue("Body/ReportItems/Chart/ChartData/ChartSeriesCollection/ChartSeries/ChartDataPoints/ChartDataPoint/ChartDataPointValues/Y");
				string sXField            = rdl.SelectNodeValue("Body/ReportItems/Chart/ChartCategoryHierarchy/ChartMembers/ChartMember/Group/GroupExpressions/GroupExpression");
				string sSeriesField       = rdl.LookupDateField(sYField);
				if ( sYField.Contains("*") )
					sSeriesField = rdl.LookupDateField(rdl.SelectNodeAttribute("Body/ReportItems/Chart/ChartData/ChartSeriesCollection/ChartSeries", "Name"));
				string sCategoryField     = rdl.LookupDateField(sXField);
				
				List<object> arrSeriesData = new List<object>();
				if ( sChartType == "Line" )
				{
					data.seriesDefaults.renderer = "$.jqplot.LineRenderer";
					data.axes.xaxis.renderer  = "$.jqplot.CategoryAxisRenderer";
					//data.axes.yaxis.autoscale = true;
					//data.axes.yaxis.tickOptions.formatString = "%d";
					
					data.axes.yaxis.label = rdl.SelectNodeValue("Body/ReportItems/Chart/ChartAreas/ChartArea/ChartValueAxes/ChartAxis/ChartAxisTitle/Caption");
					data.axes.yaxis.show  = !Sql.IsEmptyString(data.axes.yaxis.label);
					data.axes.xaxis.label = rdl.SelectNodeValue("Body/ReportItems/Chart/ChartAreas/ChartArea/ChartCategoryAxes/ChartAxis/ChartAxisTitle/Caption");
					data.axes.xaxis.show  = !Sql.IsEmptyString(data.axes.xaxis.label);
					data.series[0] = new ChartData.Series();
					data.series[0].label  = rdl.SelectNodeValue("Body/ReportItems/Chart/ChartSeriesHierarchy/ChartMembers/ChartMember/Label");
					
					List<object> arrSeries1 = new List<object>();
					arrSeriesData.Add(arrSeries1);
					
					foreach ( DataRow row in dt.Rows )
					{
						object[] arr = new object[2];
						arr[0] = row[sCategoryField];
						arr[1] = row[sSeriesField  ];
						arrSeries1.Add(arr);
					}
				}
				else if ( sChartType == "Shape" )
				{
					data.seriesDefaults.renderer = "$.jqplot.PieRenderer";
					data.seriesDefaults.rendererOptions.showDataLabels = true;
					
					List<object> arrSeries1 = new List<object>();
					arrSeriesData.Add(arrSeries1);
					
					foreach ( DataRow row in dt.Rows )
					{
						object[] arr = new object[2];
						arr[0] = row[sCategoryField];
						arr[1] = row[sSeriesField  ];
						arrSeries1.Add(arr);
					}
				}
				else if ( sChartType == "Bar" )
				{
					data.cursor.show = true;
					data.cursor.zoom = true;
					data.seriesDefaults.renderer = "$.jqplot.BarRenderer";
					data.seriesDefaults.rendererOptions.barDirection = "horizontal";
					data.seriesDefaults.rendererOptions.fillToZero   = true;
					data.axes.yaxis.renderer  = "$.jqplot.CategoryAxisRenderer";
					data.axes.xaxis.autoscale = true;
					//data.axes.xaxis.tickOptions.formatString = "%d";
					
					data.axes.xaxis.label = rdl.SelectNodeValue("Body/ReportItems/Chart/ChartAreas/ChartArea/ChartValueAxes/ChartAxis/ChartAxisTitle/Caption");
					data.axes.xaxis.show  = !Sql.IsEmptyString(data.axes.xaxis.label);
					data.axes.yaxis.label = rdl.SelectNodeValue("Body/ReportItems/Chart/ChartAreas/ChartArea/ChartCategoryAxes/ChartAxis/ChartAxisTitle/Caption");
					data.axes.yaxis.show  = !Sql.IsEmptyString(data.axes.yaxis.label);
					data.series[0] = new ChartData.Series();
					data.series[0].label  = rdl.SelectNodeValue("Body/ReportItems/Chart/ChartSeriesHierarchy/ChartMembers/ChartMember/Label");
					
					List<object> arrSeries1 = new List<object>();
					arrSeriesData.Add(arrSeries1);
					
					foreach ( DataRow row in dt.Rows )
					{
						object[] arr = new object[2];
						arr[0] = row[sSeriesField  ];
						arr[1] = row[sCategoryField];
						arrSeries1.Add(arr);
					}
					sSeriesData = json.Serialize(arrSeriesData);
				}
				else // if ( sChartType == "" )
				{
					data.cursor.show = true;
					data.cursor.zoom = true;
					data.seriesDefaults.renderer = "$.jqplot.BarRenderer";
					data.seriesDefaults.rendererOptions.barDirection = "vertical";
					data.seriesDefaults.rendererOptions.fillToZero   = true;
					data.axes.xaxis.renderer  = "$.jqplot.CategoryAxisRenderer";
					data.axes.yaxis.autoscale = true;
					//data.axes.yaxis.tickOptions.formatString = "%d";
					
					data.axes.yaxis.label = rdl.SelectNodeValue("Body/ReportItems/Chart/ChartAreas/ChartArea/ChartValueAxes/ChartAxis/ChartAxisTitle/Caption");
					data.axes.yaxis.show  = !Sql.IsEmptyString(data.axes.yaxis.label);
					data.axes.xaxis.label = rdl.SelectNodeValue("Body/ReportItems/Chart/ChartAreas/ChartArea/ChartCategoryAxes/ChartAxis/ChartAxisTitle/Caption");
					data.axes.xaxis.show  = !Sql.IsEmptyString(data.axes.xaxis.label);
					data.series[0] = new ChartData.Series();
					data.series[0].label  = rdl.SelectNodeValue("Body/ReportItems/Chart/ChartSeriesHierarchy/ChartMembers/ChartMember/Label");
					
					List<object> arrSeries1 = new List<object>();
					arrSeriesData.Add(arrSeries1);
					
					foreach ( DataRow row in dt.Rows )
					{
						object[] arr = new object[2];
						arr[0] = row[sCategoryField];
						arr[1] = row[sSeriesField  ];
						arrSeries1.Add(arr);
					}
				}
				sSeriesData = json.Serialize(arrSeriesData);
				sChartData  = json.Serialize(data);
				
				Dictionary<string, object> dict = json.Deserialize<Dictionary<string, object>>(sChartData);
				RemoveNulls(dict);
				sChartData = json.Serialize(dict);
				// 10/31/2011 Paul.  Need to convert quoted strings with $.jqplot. to unquoted strings. 
				while ( sChartData.IndexOf("\"$.jqplot.") >= 0 )
				{
					int nStart = sChartData.IndexOf("\"$.jqplot.");
					sChartData = sChartData.Substring(0, nStart) + sChartData.Substring(nStart + 1);
					int nEnd = sChartData.IndexOf("\"", nStart);
					sChartData = sChartData.Substring(0, nEnd) + sChartData.Substring(nEnd + 1);
				}
			}
		}

		// 01/19/2010 Paul.  The Module Name is needed in order to apply ACL Field Security. 
		public void RunReport(string sRDL, string sMODULE_NAME)
		{
			try
			{
				// 01/24/2010 Paul.  Pass the context so that it can be used in the Validation call. 
				// 12/04/2010 Paul.  L10n is needed by the Rules Engine to allow translation of list terms. 
				// 04/13/2011 Paul.  A scheduled report does not have a Session, so we need to create a session using the same approach used for ExchangeSync. 
				// 10/06/2012 Paul.  Last parameter is the SubreportProcessingEventArgs. 
				DataSet ds = new DataSet();
				// 03/24/2016 Paul.  We need an alternate way to provide parameters to render a report with a signature. 
				RdlDocument rdl = RdlUtil.LocalLoadReportDefinition(this.Context, null, null, L10n, T10n, ds, sRDL, sMODULE_NAME, Guid.Empty, out sReportSQL, true, null);
				UpdateChartData(rdl, ds);
			}
			catch ( Exception ex )
			{
				lblError.Text = Utils.ExpandException(ex);
			}
			if ( bDebug )
				RegisterClientScriptBlock("SQLCode", "<script type=\"text/javascript\">sDebugSQL += '" + Sql.EscapeJavaScript(sReportSQL) + "';</script>");
		}

		// 04/06/2011 Paul.  We need a way to pull data from the Parameters form. 
		public void RunReport(string sRDL, string sMODULE_NAME, SplendidControl ctlParameterView)
		{
			try
			{
				DataSet ds = new DataSet();
				// 04/13/2011 Paul.  A scheduled report does not have a Session, so we need to create a session using the same approach used for ExchangeSync. 
				// 10/06/2012 Paul.  Last parameter is the SubreportProcessingEventArgs. 
				// 03/24/2016 Paul.  We need an alternate way to provide parameters to render a report with a signature. 
				RdlDocument rdl = RdlUtil.LocalLoadReportDefinition(this.Context, null, ctlParameterView, L10n, T10n, ds, sRDL, sMODULE_NAME, Guid.Empty, out sReportSQL, true, null);
				UpdateChartData(rdl, ds);

			}
			catch ( Exception ex )
			{
				lblError.Text = Utils.ExpandException(ex);
			}
			if ( bDebug )
				RegisterClientScriptBlock("SQLCode", "<script type=\"text/javascript\">sDebugSQL += '" + Sql.EscapeJavaScript(sReportSQL) + "';</script>");
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				// 06/20/2013 Paul.  Move code to shared Util file. 
				ChartUtil.RegisterScripts(this.Page);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
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
