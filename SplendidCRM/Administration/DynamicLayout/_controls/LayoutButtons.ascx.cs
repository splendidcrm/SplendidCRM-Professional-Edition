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
using System.Web;
using System.Web.UI.WebControls;

namespace SplendidCRM.Administration.DynamicLayout._controls
{
	/// <summary>
	///		Summary description for LayoutButtons.
	/// </summary>
	public class LayoutButtons : SplendidCRM._controls.EditButtons
	{
		protected Button   btnNew       ;
		protected Button   btnDefaults  ;
		protected Button   btnExport    ;
		protected TextBox  txtCopyLayout;
		protected CheckBox chkPreview   ;
		protected HiddenField hidPreviousPreview;
		protected string   sVIEW_NAME   ;

		// 05/22/2009 Paul.  We need to pass the view name to the Export popup. 
		public string VIEW_NAME
		{
			get { return sVIEW_NAME; }
			set { sVIEW_NAME = value; }
		}

		public bool Preview(bool bInitialize)
		{
			// 04/11/2011 Paul.  We cannot use the chkPreview.checked field because we are calling Preview too early. 
			// We need to get the flag directly from the Request. 
			// 04/11/2011 Paul.  We need to save the previous value so that the the binding can happen properly in the InitializeComponent. 
			if ( bInitialize )
				return Sql.ToBoolean(Request[hidPreviousPreview.UniqueID]);
			else
				return Sql.ToBoolean(Request[chkPreview.UniqueID]);
		}

		public void ShowExport(bool bValue)
		{
			btnExport.Visible = bValue;
		}

		public void ShowDefaults(bool bValue)
		{
			btnDefaults.Visible = bValue;
		}

		// 02/14/2013 Paul.  Provide access to the CopyLayout textbox. 
		public TextBox CopyLayout
		{
			get { return txtCopyLayout; }
		}

		// 04/11/2011 Paul.  Allow the layout mode to be turned off to preview the result. 
		protected void chkPreview_CheckedChanged(object sender, EventArgs e)
		{
			if ( Command != null )
			{
				CommandEventArgs ePreview = new CommandEventArgs("PreviewChanged", null);
				Command(sender, ePreview);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			hidPreviousPreview.Value = chkPreview.Checked.ToString();
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

