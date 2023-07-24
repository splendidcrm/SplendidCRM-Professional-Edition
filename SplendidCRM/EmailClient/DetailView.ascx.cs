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

namespace SplendidCRM.EmailClient
{
	/// <summary>
	/// Summary description for DetailView.
	/// </summary>
	public class DetailView : SplendidControl
	{
		protected Label    lblError           ;
		protected Label    txtFROM            ;
		protected Label    txtNAME            ;
		protected Label    txtDATE_START      ;
		protected Label    txtTO_ADDRS        ;
		protected Label    txtCC_ADDRS        ;
		protected Label    txtDESCRIPTION     ;
		protected Literal  litINTERNET_HEADERS;
		
		protected TableRow trFROM             ;
		protected TableRow trNAME             ;
		protected TableRow trDATE_START       ;
		protected TableRow trTO_ADDRS         ;
		protected TableRow trCC_ADDRS         ;
		protected TableRow trINTERNET_HEADERS ;
		protected TableRow trATTACHMENTS      ;
		protected DataGrid grdATTACHMENTS     ;
		protected Repeater rptATTACHMENTS     ;

		public CommandEventHandler Command;

		public string FROM
		{
			get { return txtFROM.Text; }
			set { txtFROM.Text = value; }
		}

		public string NAME
		{
			get { return txtNAME.Text; }
			set { txtNAME.Text = value; }
		}

		public string DATE_START
		{
			get { return txtDATE_START.Text; }
			set { txtDATE_START.Text = value; }
		}

		public string TO_ADDRS
		{
			get { return txtTO_ADDRS.Text; }
			set { txtTO_ADDRS.Text = value; }
		}

		public string CC_ADDRS
		{
			get { return txtCC_ADDRS.Text; }
			set
			{
				txtCC_ADDRS.Text = value;
				trCC_ADDRS.Visible = !Sql.IsEmptyString(txtCC_ADDRS.Text);
			}
		}

		public string DESCRIPTION
		{
			get { return txtDESCRIPTION.Text; }
			set { txtDESCRIPTION.Text = value; }
		}

		public string INTERNET_HEADERS
		{
			get { return litINTERNET_HEADERS.Text; }
			set
			{
				try
				{
					XmlDocument xml = new XmlDocument();
					// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
					// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
					// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
					xml.XmlResolver = null;
					xml.LoadXml(value);

					StringBuilder sb = new StringBuilder();
					sb.AppendLine("<table class=\"tabDetailView\">");
					XmlNodeList nlHeaders = xml.SelectNodes("Headers/Header");
					foreach ( XmlNode xHeader in nlHeaders )
					{
						string sName  = XmlUtil.SelectSingleNode(xHeader, "Name" );
						string sValue = XmlUtil.SelectSingleNode(xHeader, "Value");
						if ( L10n.IsLanguageRTL() )
							sName = ":" + sName;
						else
							sName = sName + ":";
						sb.AppendLine("	<tr>");
						sb.AppendLine("		<td width=\"15%\" class=\"EmailDetailViewDL\">" + sName + "</td>");
						// 07/17/2010 Paul.  Make sure not to encode the <br />. 
						sb.AppendLine("		<td width=\"85%\" class=\"EmailDetailViewDF\">" + HttpUtility.HtmlEncode(sValue).Replace("\n", "<br />\n") + "</td>");
						sb.AppendLine("	</tr>");
					}
					sb.AppendLine("</table>");
					litINTERNET_HEADERS.Text = sb.ToString();
				}
				catch
				{
				}
			}
		}

		// 11/06/2010 Paul.  Return the Attachments so that we can show embedded images or download the attachments. 
		public DataTable ATTACHMENTS
		{
			set
			{
				// 11/06/2010 Paul.  First make sure to clear the previous data source. 
				grdATTACHMENTS.DataSource = null;
				grdATTACHMENTS.DataBind();
				rptATTACHMENTS.DataSource = null;
				rptATTACHMENTS.DataBind();
				if ( value != null )
				{
					DataView vwAttachments = new DataView(value);
#if DEBUG
					grdATTACHMENTS.DataSource = vwAttachments;
					grdATTACHMENTS.DataBind();
#else
					// 11/06/2010 Paul.  Exclude the Inline images from the list of download links. 
					vwAttachments.RowFilter = "IsInline = 'False'";
#endif
					rptATTACHMENTS.DataSource = vwAttachments;
					rptATTACHMENTS.DataBind();
					trATTACHMENTS.Visible = true;
				}
			}
		}


		public void ClearForm()
		{
			txtFROM            .Text = String.Empty;
			txtNAME            .Text = String.Empty;
			txtDATE_START      .Text = String.Empty;
			txtTO_ADDRS        .Text = String.Empty;
			txtCC_ADDRS        .Text = String.Empty;
			txtDESCRIPTION     .Text = String.Empty;
			litINTERNET_HEADERS.Text = String.Empty;
			trFROM            .Visible = true ;
			trNAME            .Visible = true ;
			trDATE_START      .Visible = true ;
			trTO_ADDRS        .Visible = true ;
			trCC_ADDRS        .Visible = false;
			trINTERNET_HEADERS.Visible = false;
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "ShowHeaders" )
				{
					trINTERNET_HEADERS.Visible = !trINTERNET_HEADERS.Visible;
					// 05/30/2010 Paul.  I like seeing the original header in addition to the raw headers. 
					//trFROM            .Visible = !trINTERNET_HEADERS.Visible;
					//trNAME            .Visible = !trINTERNET_HEADERS.Visible;
					//trDATE_START      .Visible = !trINTERNET_HEADERS.Visible;
					//trTO_ADDRS        .Visible = !trINTERNET_HEADERS.Visible;
					//trCC_ADDRS        .Visible = !trINTERNET_HEADERS.Visible && !Sql.IsEmptyString(txtCC_ADDRS.Text);
				}
				// 07/17/2010 Paul.  Always send the command to the parent so that it can rebind the grid, 
				// otherwise the pagination disappears and the selection fails. 
				if ( Command != null )
					Command(this, e);
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
			m_sMODULE = "EmailClient";
		}
		#endregion
	}
}

