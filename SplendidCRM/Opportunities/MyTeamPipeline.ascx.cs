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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Opportunities
{
	/// <summary>
	///		Summary description for MyTeamPipeline.
	/// </summary>
	public class MyTeamPipeline : DashletControl
	{
		protected _controls.DashletHeader  ctlDashletHeader ;

		protected _controls.ChartDatePicker ctlDATE_START ;
		protected _controls.ChartDatePicker ctlDATE_END   ;
		protected ListBox                   lstSALES_STAGE;
		// 08/31/2006 Paul.  Provide an additional error message that will appear even when the edit box is not visible. 
		protected DatePickerValidator       valDATE_START ;
		protected DatePickerValidator       valDATE_END   ;
		protected bool                      bShowEditDialog = false;
		protected HyperLink                 lnkXML        ;

		protected string PipelineQueryString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("DATE_START=");
			// 07/09/2006 Paul.  The date is passed as TimeZone time, which is what the control value returns, so no conversion is necessary. 
			sb.Append(Server.UrlEncode(Sql.ToDateString(ctlDATE_START.Value)));
			sb.Append("&DATE_END=");
			sb.Append(Server.UrlEncode(Sql.ToDateString(ctlDATE_END.Value)));
			// 09/16/2005 Paul.  Since this is MyPipeline, specify current user. 
			sb.Append("&ASSIGNED_USER_ID=");
			sb.Append(Server.UrlEncode(Security.USER_ID.ToString()));
			foreach(ListItem item in lstSALES_STAGE.Items)
			{
				if ( item.Selected )
				{
					sb.Append("&SALES_STAGE=");
					sb.Append(Server.UrlEncode(item.Value));
				}
			}
			// 09/15/2005 Paul.  The hBarS flash will append a "?0.12341234" timestamp to the URL. 
			// Use a bogus parameter to separate the timestamp from the last sales stage. 
			sb.Append("&TIME_STAMP=");
			return sb.ToString();
		}

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Submit" )
				{
					valDATE_START.Enabled = true;
					valDATE_END  .Enabled = true;
					ctlDATE_START.Validate();
					ctlDATE_END  .Validate();
					valDATE_START.Validate();
					valDATE_END .Validate();
					if ( Page.IsValid )
					{
						ViewState["MyPipelineQueryString"] = PipelineQueryString();
					}
					// 01/19/2007 Paul.  Keep the edit dialog visible.
					bShowEditDialog = true;
					// 03/29/2008 Paul.  Update the data binding of just the XML link. 
					lnkXML.DataBind();
				}
				// 07/10/2009 Paul.  Allow the dashlet to be removed. 
				else if ( e.CommandName == "Remove" )
				{
					if ( !Sql.IsEmptyString(sDetailView) )
					{
						SqlProcs.spDASHLETS_USERS_InitDisable(Security.USER_ID, sDetailView, m_sMODULE, this.AppRelativeVirtualPath.Substring(0, this.AppRelativeVirtualPath.Length-5));
						SplendidCache.ClearUserDashlets(sDetailView);
						Response.Redirect(Page.AppRelativeVirtualPath + Request.Url.Query);
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				//lblError.Text = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 11/05/2007 Paul.  Don't show panel if it was manually hidden. 
			this.Visible = this.Visible && (SplendidCRM.Security.GetUserAccess(m_sMODULE, "list") >= 0);
			// 09/09/2007 Paul.  We are having trouble dynamically adding user controls to the WebPartZone. 
			// Instead, control visibility manually here.  This approach as the added benefit of hiding the 
			// control even if the WebPartManager has moved it to an alternate zone. 
			// 07/10/2009 Paul.  The end-user will be able to hide or show the Dashlet controls. 
			/*
			if ( this.Visible && !Sql.IsEmptyString(sDetailView) )
			{
				// 01/17/2008 Paul.  We need to use the sDetailView property and not the hard-coded view name. 
				DataView vwFields = new DataView(SplendidCache.DetailViewRelationships(sDetailView));
				vwFields.RowFilter = "CONTROL_NAME = '~/Opportunities/MyPipeline'";
				this.Visible = vwFields.Count > 0;
			}
			*/
			if ( !this.Visible )
				return;

			valDATE_START.ErrorMessage = L10n.Term(".ERR_INVALID_DATE");
			valDATE_END  .ErrorMessage = L10n.Term(".ERR_INVALID_DATE");
			if ( !IsPostBack )
			{
				lstSALES_STAGE.DataSource = SplendidCache.List("sales_stage_dom");
				lstSALES_STAGE.DataBind();
				// 09/14/2005 Paul.  Default to today, and all sales stages. 
				foreach(ListItem item in lstSALES_STAGE.Items)
				{
					item.Selected = true;
				}
				// 07/09/2006 Paul.  The date is passed in TimeZone time, so convert from server time. 
				ctlDATE_START.Value = T10n.FromServerTime(DateTime.Today);
				// 07/06/2016 Paul.  Use +5 years instead of 2100 to reduce truncation when displaying year as yy. 
				ctlDATE_END  .Value = T10n.FromServerTime(new DateTime(DateTime.Today.Year + 5, 1, 1));
				// 09/15/2005 Paul.  Maintain the pipeline query string separately so that we can respond to specific submit requests 
				// and ignore all other control events on the page. 
				ViewState["MyPipelineQueryString"] = PipelineQueryString();
				// 03/29/2008 Paul.  Update the data binding of just the XML link. 
				lnkXML.DataBind();
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
			ctlDashletHeader.Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "Opportunities";
		}
		#endregion
	}
}

