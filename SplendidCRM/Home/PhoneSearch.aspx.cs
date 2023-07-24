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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Home
{
	/// <summary>
	/// Summary description for PhoneSearch.
	/// </summary>
	public class PhoneSearch : SplendidPage
	{
		protected string        m_sMODULE      ;
		protected Label         lblWarning     ;
		protected PlaceHolder   plcSubPanel    ;
		protected Label         lblNoResults   ;

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("Home.LBL_SEARCH_RESULTS"));
			// 07/27/2012 Paul.  Match the normalization within database function fnNormalizePhone. 
			string sPhoneNumber = Utils.NormalizePhone(Request["PhoneNumber"]);
			if ( Sql.IsEmptyString(sPhoneNumber) )
			{
				// 08/31/2010 Paul.  Skip during precompile. 
				if ( !Sql.ToBoolean(Request["PrecompileOnly"]) )
					lblWarning.Text = L10n.Term("Home.ERR_ONE_CHAR");
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
			m_sMODULE = "Home";
			SetMenu(m_sMODULE);
			this.AppendDetailViewRelationships(m_sMODULE + ".PhoneSearch", plcSubPanel);
		}
		#endregion
	}
}

