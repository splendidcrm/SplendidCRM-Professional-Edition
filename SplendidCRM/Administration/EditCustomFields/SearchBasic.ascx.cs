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

namespace SplendidCRM.Administration.EditCustomFields
{
	/// <summary>
	///		Summary description for SearchBasic.
	/// </summary>
	public class SearchBasic : SearchControl
	{
		protected DropDownList lstMODULE_NAME;

		public string MODULE_NAME
		{
			get
			{
				return lstMODULE_NAME.SelectedValue;
			}
		}

		public override void ClearForm()
		{
			lstMODULE_NAME.SelectedIndex = 0;
		}

		public override void SqlSearchClause(IDbCommand cmd)
		{
			Sql.AppendParameter(cmd, lstMODULE_NAME, "CUSTOM_MODULE");
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( !this.IsPostBack )
			{
				// 05/20/2007 Paul.  Make sure to copy the table before modifying the data, otherwise the changes will get applied to the cached table. 
				DataTable dtCustomEditModules = SplendidCache.CustomEditModules().Copy();
				foreach(DataRow row in dtCustomEditModules.Rows)
				{
					row["DISPLAY_NAME"] = L10n.Term(".moduleList." + row["DISPLAY_NAME"]);
				}
				lstMODULE_NAME.DataSource = dtCustomEditModules;
				lstMODULE_NAME.DataBind();
				// 01/05/2006 Paul.  Can't seem to set the selected value from ListView.ascx. 
				string sMODULE_NAME = Sql.ToString(Request["MODULE_NAME"]);
				try
				{
					// 08/19/2010 Paul.  Check the list before assigning the value. 
					Utils.SetSelectedValue(lstMODULE_NAME, sMODULE_NAME);
				}
				catch
				{
				}
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

