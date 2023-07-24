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
using System.Net;
using System.Xml;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace SplendidCRM.Feeds
{
	/// <summary>
	///		Summary description for FeedSummaryView.
	/// </summary>
	public class FeedSummaryView : SplendidControl
	{
		protected Guid      gID             ;
		protected Label     lblError        ;
		protected string    sChannelTitle   ;
		protected string    sChannelLink    ;
		protected string    sLastBuildDate  ;
		protected Label     lblLastBuildDate;
		protected string    sURL            ;
		protected Repeater  rpFeed          ;
		protected DataTable dtChannel       ;
		protected DataTable dtItems         ;

		public Guid FEED_ID
		{
			get
			{
				return gID;
			}
			set
			{
				gID = value;
			}
		}

		public string URL
		{
			get
			{
				return sURL;
			}
			set
			{
				sURL = value;
			}
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "MoveUp" )
				{
				}
				else if ( e.CommandName == "MoveDown" )
				{
				}
				else if ( e.CommandName == "Delete" )
				{
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				// 12/06/2005 Paul.  Can't use the DataSet reader because it returns the following error:
				// The same table (description) cannot be the child table in two nested relations, caused by News.com feed. 
				XmlDocument xml = new XmlDocument();
				// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
				// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
				// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
				xml.XmlResolver = null;
				xml.Load(sURL);
				sChannelTitle  = XmlUtil.SelectSingleNode(xml, "channel/title"        );
				sChannelLink   = XmlUtil.SelectSingleNode(xml, "channel/link"         );
				sLastBuildDate = XmlUtil.SelectSingleNode(xml, "channel/lastBuildDate");
				if ( !Sql.IsEmptyString(sLastBuildDate) )
				{
					sLastBuildDate = L10n.Term("Feeds.LBL_LAST_UPDATED") + ": " + sLastBuildDate;
				}
				lblLastBuildDate.Text = sLastBuildDate;

				dtItems = new DataTable();
				DataColumn colTitle       = new DataColumn("title"      , Type.GetType("System.String"));
				DataColumn colLink        = new DataColumn("link"       , Type.GetType("System.String"));
				DataColumn colDescription = new DataColumn("description", Type.GetType("System.String"));
				DataColumn colCategory    = new DataColumn("category"   , Type.GetType("System.String"));
				DataColumn colPubDate     = new DataColumn("pubDate"    , Type.GetType("System.String"));
				dtItems.Columns.Add(colTitle      );
				dtItems.Columns.Add(colLink       );
				dtItems.Columns.Add(colDescription);
				dtItems.Columns.Add(colCategory   );
				dtItems.Columns.Add(colPubDate    );
				try
				{
					XmlNodeList nl = xml.DocumentElement.SelectNodes("channel/item");
					int nRows = 0;
					foreach(XmlNode item in nl)
					{
						DataRow row = dtItems.NewRow();
						dtItems.Rows.Add(row);
						row["title"      ] = XmlUtil.SelectSingleNode(item, "title"      );
						row["link"       ] = XmlUtil.SelectSingleNode(item, "link"       );
						row["description"] = XmlUtil.SelectSingleNode(item, "description");
						row["category"   ] = XmlUtil.SelectSingleNode(item, "category"   );
						row["pubDate"    ] = XmlUtil.SelectSingleNode(item, "pubDate"    );
						nRows++;
						if ( nRows == 5 )
							break;
					}
					rpFeed.DataSource = dtItems;
					rpFeed.DataBind();
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// Ignore errors for now. 
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				Response.Write(ex.Message);
			}
			// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
			//Page.DataBind();
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

