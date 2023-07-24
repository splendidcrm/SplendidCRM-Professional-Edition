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

namespace SplendidCRM.Administration.Dropdown
{
	/// <summary>
	///		Summary description for SearchBasic.
	/// </summary>
	public class SearchBasic : SearchControl
	{
		protected DropDownList lstDROPDOWN_OPTIONS;
		protected DropDownList lstLANGUAGE_OPTIONS;

		public string DROPDOWN
		{
			get
			{
				return lstDROPDOWN_OPTIONS.SelectedValue;
			}
			set
			{
				if ( lstDROPDOWN_OPTIONS.DataSource == null )
				{
					lstDROPDOWN_OPTIONS.DataSource = SplendidCache.TerminologyPickLists();
					lstDROPDOWN_OPTIONS.DataBind();
				}
				// 08/19/2010 Paul.  Check the list before assigning the value. 
				Utils.SetSelectedValue(lstDROPDOWN_OPTIONS, value);
			}
		}

		public string LANGUAGE
		{
			get
			{
				return lstLANGUAGE_OPTIONS.SelectedValue;
			}
			set
			{
				if ( lstLANGUAGE_OPTIONS.DataSource == null )
				{
					lstLANGUAGE_OPTIONS.DataSource = SplendidCache.Languages();
					lstLANGUAGE_OPTIONS.DataBind();
				}
				Utils.SetValue(lstLANGUAGE_OPTIONS, L10N.NormalizeCulture(value));
			}
		}

		public override void ClearForm()
		{
			lstDROPDOWN_OPTIONS.SelectedIndex = 0;
			// 08/19/2010 Paul.  Check the list before assigning the value. 
			// 09/18/2012 Paul.  Default to current culture. 
			Utils.SetSelectedValue(lstLANGUAGE_OPTIONS, L10N.NormalizeCulture(L10n.NAME));
		}

		public override void SqlSearchClause(IDbCommand cmd)
		{
			Sql.AppendParameter(cmd, lstDROPDOWN_OPTIONS.SelectedValue, 50, Sql.SqlFilterMode.Exact, "LIST_NAME");
			Sql.AppendParameter(cmd, lstLANGUAGE_OPTIONS.SelectedValue, 10, Sql.SqlFilterMode.Exact, "LANG"     );
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( !IsPostBack )
			{
				if ( lstDROPDOWN_OPTIONS.DataSource == null )
				{
					lstDROPDOWN_OPTIONS.DataSource = SplendidCache.TerminologyPickLists();
					lstDROPDOWN_OPTIONS.DataBind();
				}
				if ( lstLANGUAGE_OPTIONS.DataSource == null )
				{
					lstLANGUAGE_OPTIONS.DataSource = SplendidCache.Languages();
					lstLANGUAGE_OPTIONS.DataBind();
					Utils.SetValue(lstLANGUAGE_OPTIONS, L10N.NormalizeCulture(L10n.NAME));
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

