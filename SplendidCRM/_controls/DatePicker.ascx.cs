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
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Threading;

namespace SplendidCRM._controls
{
	/// <summary>
	///		Summary description for DatePicker.
	/// </summary>
	public class DatePicker : SplendidControl
	{
		protected TextBox  txtDATE      ;
		protected Label    lblDateFormat;
		protected System.Web.UI.WebControls.Image    imgCalendar  ;
		protected RequiredFieldValidator     reqDATE;
		// 08/31/2006 Paul.  We cannot use a regular expression validator because there are just too many date formats.
		protected DateValidator              valDATE;
		// 08/12/2014 Paul.  Add format to calendar. 
		protected AjaxControlToolkit.CalendarExtender extDATE;
		// 08/08/2015 Paul.  Provide a way to hide the date format. 
		protected bool     bShowDateFormat = true;

		public DateTime Value
		{
			get
			{
				// 07/09/2006 Paul.  Dates are no longer converted inside this control. 
				return Sql.ToDateTime(txtDATE.Text);
			}
			set
			{
				txtDATE.Text = Sql.ToDateString(value);
			}
		}

		public string DateText
		{
			get
			{
				return txtDATE.Text;
			}
			set
			{
				txtDATE.Text = value;
			}
		}

		public string DateClientID
		{
			get
			{
				return txtDATE.ClientID;
			}
		}

		public short TabIndex
		{
			get
			{
				return txtDATE.TabIndex;
			}
			set
			{
				txtDATE.TabIndex = value;
			}
		}

		public bool EnableDateFormat
		{
			get
			{
				return lblDateFormat.Visible;
			}
			set
			{
				lblDateFormat.Visible = value;
			}
		}

		public bool Enabled
		{
			// 05/28/2018 Paul.  We need to disable custom controls. 
			get
			{
				return txtDATE.Enabled;
			}
			set
			{
				extDATE.Enabled = value;
				txtDATE.Enabled = value;
				imgCalendar.Visible = value;
			}
		}

		public bool ShowDateFormat
		{
			get { return bShowDateFormat; }
			set { bShowDateFormat = value; }
		}

		// 04/05/2006 Paul.  Need a way to clear the date. 
		public void Clear()
		{
			txtDATE.Text = String.Empty;
		}

		// 11/11/2010 Paul.  Provide a way to disable validation in a rule. 
		public void Validate()
		{
			Validate(true);
		}

		public void Validate(bool bEnabled)
		{
			reqDATE.Enabled = bEnabled;
			valDATE.Enabled = bEnabled;
			// 11/07/2005 Paul.  Not sure why rglDATE is not available. 
			//rglDATE.Enabled = true;
			// 04/15/2006 Paul.  The error message is not binding properly.  Just assign here as a quick solution. 
			// 06/09/2006 Paul.  Now that we have solved the data binding issues, we can let the binding fill the message. 
			//rglDATE.ErrorMessage = L10n.Term(".ERR_INVALID_DATE");
			// 08/31/2006 Paul.  Enable and perform date validation. 
			reqDATE.Validate();
			valDATE.Validate();
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 06/09/2006 Paul.  Always set the message as this control does not remember its state. 
			reqDATE.ErrorMessage = L10n.Term(".ERR_REQUIRED_FIELD");
			// 08/31/2006 Paul.  Need to bind the text. 
			valDATE.ErrorMessage = L10n.Term(".ERR_INVALID_DATE");
			// 08/12/2014 Paul.  Add format to calendar. The binding needs to be forced in order for the format to be applied. 
			// 08/13/2014 Paul.  ChartDatePicker does not use the CalendarExtender. 
			if ( extDATE != null )
				extDATE.DataBind();

			// 11/26/2008 Paul.  In order for javascript to render in an UpdatePanel, it must be registered with the ScriptManager. 
			string sChangeJS = "<script type=\"text/javascript\">\nfunction ChangeDate" + txtDATE.ClientID.Replace(":", "_") + "(sDATE)\n{\n\tdocument.getElementById('" + txtDATE.ClientID + "').value = sDATE;\n}\n</script>\n";
			ScriptManager mgrAjax = ScriptManager.GetCurrent(this.Page);
			if ( mgrAjax != null )
			{
				// 11/27/2008 Paul.  The name of the script block must be unique for each instance of this control. 
				// 06/21/2009 Paul.  Use RegisterStartupScript instead of RegisterClientScriptBlock so that the script will run after the control has been created. 
				ScriptManager.RegisterStartupScript(this, typeof(System.String), "AjaxChangeDate_" + txtDATE.ClientID.Replace(":", "_"), sChangeJS, false);
			}
			else
			{
				#pragma warning disable 618
				Page.ClientScript.RegisterStartupScript(typeof(System.String), "PageChangeDate_" + txtDATE.ClientID.Replace(":", "_"), sChangeJS);
				#pragma warning restore 618
			}
			// 05/06/2010 Paul.  Use a special Page flag to override the default IsPostBack behavior. 
			bool bIsPostBack = this.IsPostBack && !NotPostBack;
			if ( !bIsPostBack )
			{
				// 12/12/2009 Paul.  The calendar popup will not work on a Blackberry. 
				bool bSupportsPopups = true;
				if ( this.IsMobile )
				{
					// 11/24/2010 Paul.  .NET 4 has broken the compatibility of the browser file system. 
					// We are going to minimize our reliance on browser files in order to reduce deployment issues. 
					bSupportsPopups = Utils.SupportsPopups;
				}
				imgCalendar.Visible = bSupportsPopups;
				// 06/29/2006 Paul.  The image needs to be manually bound in Administration/ProductTemplates/EditView.ascx
				imgCalendar.DataBind();
				// 07/05/2006 Paul.  Need to bind the label manually. 
				// 07/06/2005 Paul.  lblDateFormat is not defined in ChartDatePicker, so we must test if lblDateFormat exists. 
				if ( lblDateFormat != null )
					lblDateFormat.DataBind();
				//this.DataBind();
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

