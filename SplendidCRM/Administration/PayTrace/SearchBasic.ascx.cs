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
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace SplendidCRM.Administration.PayTrace
{
	/// <summary>
	///		Summary description for SearchBasic.
	/// </summary>
	public class SearchBasic : SearchControl
	{
		protected _controls.SearchButtons ctlSearchButtons;

		protected _controls.DatePicker ctlSTART_DATE        ;
		protected _controls.DatePicker ctlEND_DATE          ;
		protected DropDownList         lstTRANSACTION_TYPE  ;
		protected TextBox              txtSEARCH_TEXT       ;

		public DateTime START_DATE
		{
			get { return ctlSTART_DATE.Value; }
			set { ctlSTART_DATE.Value = value; }
		}

		public DateTime END_DATE
		{
			get { return ctlEND_DATE.Value; }
			set { ctlEND_DATE.Value = value; }
		}

		public string TRANSACTION_TYPE
		{
			get
			{
				return lstTRANSACTION_TYPE.SelectedValue;
			}
			set
			{
				Utils.SetSelectedValue(lstTRANSACTION_TYPE, value);
			}
		}

		public string SEARCH_TEXT
		{
			get
			{
				return txtSEARCH_TEXT.Text;
			}
			set
			{
				txtSEARCH_TEXT.Text = value;
			}
		}

		public override void ClearForm()
		{
			ctlSTART_DATE.DateText = String.Empty;
			ctlEND_DATE  .DateText = String.Empty;
			txtSEARCH_TEXT.Text    = String.Empty;
			lstTRANSACTION_TYPE.SelectedIndex = 0;
		}

		public override void SqlSearchClause(IDbCommand cmd)
		{
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( !IsPostBack )
			{
				lstTRANSACTION_TYPE.DataSource = SplendidCache.List("paytrace_transaction_type");
				lstTRANSACTION_TYPE.DataBind();
				lstTRANSACTION_TYPE.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
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
			ctlSearchButtons.Command += new CommandEventHandler(Page_Command);
		}
		#endregion
	}
}