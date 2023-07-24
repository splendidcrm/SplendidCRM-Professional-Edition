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
using System.Text;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace SplendidCRM._controls
{
	/// <summary>
	///		Summary description for CheckAll.
	/// </summary>
	public class CheckAll : SplendidControl
	{
		protected string      sFieldName      = "chkMain";
		protected bool        bShowSelectAll  = true;
		protected Label       lblSelectedLabel;
		protected HiddenField hidSelectedItems;
		protected HyperLink   lnkSelectPage   ;
		protected LinkButton  btnSelectAll    ;
		protected HyperLink   lnkDeselectAll  ;

		public CommandEventHandler Command ;

		public string FieldName
		{
			get { return sFieldName; }
			set { sFieldName = value; }
		}

		public bool ShowSelectAll
		{
			get { return bShowSelectAll; }
			set { bShowSelectAll = value; }
		}

		public HiddenField SelectedItems
		{
			get { return hidSelectedItems; }
		}

		// 09/18/2012 Paul.  We need a quick way to determine if SelectAll was checked. 
		public bool SelectAllChecked
		{
			get { return btnSelectAll.UniqueID == Sql.ToString(Request["__EVENTTARGET"]); }
		}

		public string[] SelectedItemsArray
		{
			get
			{
				// 09/21/2013 Paul.  Selected items is used by PayTrace as a collection of integer IDs. 
				// 10/24/2013 Paul.  When not counting Guid lengths, make sure the string is not empty. 
				if ( hidSelectedItems.Value.Length >= 36 || (hidSelectedItems.Value.Length > 0 && sFieldName != "chkMain") )
					return hidSelectedItems.Value.Split(',');
				return null;
			}
		}

		public void SelectAll(DataView vw, string sFieldID)
		{
			StringBuilder sb = new StringBuilder();
			foreach ( DataRowView row in vw )
			{
				if ( sb.Length > 0 )
					sb.Append(",");
				sb.Append(Sql.ToString(row[sFieldID]));
			}
			hidSelectedItems.Value = sb.ToString();
			lblSelectedLabel.Text = String.Format(L10n.Term(".LBL_SELECTED"), (hidSelectedItems.Value.Length+1)/37);
		}

		// 08/10/2013 Paul.  Provide a way to clear all selected items. 
		public void ClearAll()
		{
			ClearAll(this.sFieldName);
		}

		public void ClearAll(string sFieldID)
		{
			StringBuilder sb = new StringBuilder();
			hidSelectedItems.Value = sb.ToString();
			lblSelectedLabel.Text = String.Format(L10n.Term(".LBL_SELECTED"), 0);
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			if ( Command != null )
				Command(this, e) ;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 11/15/2007 Paul.  CheckAll is not displayed on a mobile browser. 
			if ( this.IsMobile )
				this.Visible = false;
			lblSelectedLabel.Text = String.Format(L10n.Term(".LBL_SELECTED"), (hidSelectedItems.Value.Length+1)/37);
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

