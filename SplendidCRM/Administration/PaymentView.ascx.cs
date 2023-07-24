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

namespace SplendidCRM.Administration
{
	/// <summary>
	///		Summary description for PaymentView.
	/// </summary>
	public class PaymentView : SplendidControl
	{
		protected Image     imgAuthorizeNet            ;
		protected HyperLink lnkAuthorizeNet            ;
		protected Label     lblAuthorizeNet            ;
		protected Image     imgAuthorizeNetTransactions;
		protected HyperLink lnkAuthorizeNetTransactions;
		protected Label     lblAuthorizeNetTransactions;

		protected Image     imgPayPal                  ;
		protected HyperLink lnkPayPal                  ;
		protected Label     lblPayPal                  ;
		protected Image     imgPayPalTransactions      ;
		protected HyperLink lnkPayPalTransactions      ;
		protected Label     lblPayPalTransactions      ;

		protected Image     imgPayTrace                ;
		protected HyperLink lnkPayTrace                ;
		protected Label     lblPayTrace                ;
		protected Image     imgPayTraceTransactions    ;
		protected HyperLink lnkPayTraceTransactions    ;
		protected Label     lblPayTraceTransactions    ;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( !IsPostBack )
			{
				imgAuthorizeNet            .Visible = Utils.CachedFileExists(Context, lnkAuthorizeNet.NavigateUrl);
				lnkAuthorizeNet            .Visible = imgAuthorizeNet.Visible;
				lblAuthorizeNet            .Visible = imgAuthorizeNet.Visible;
				imgAuthorizeNetTransactions.Visible = Utils.CachedFileExists(Context, lnkAuthorizeNetTransactions.NavigateUrl);
				lnkAuthorizeNetTransactions.Visible = imgAuthorizeNet.Visible;
				lblAuthorizeNetTransactions.Visible = imgAuthorizeNet.Visible;

				imgPayPal                  .Visible = Utils.CachedFileExists(Context, lnkPayPal.NavigateUrl);
				lnkPayPal                  .Visible = imgPayPal.Visible;
				lblPayPal                  .Visible = imgPayPal.Visible;
				imgPayPalTransactions      .Visible = Utils.CachedFileExists(Context, lnkPayPalTransactions.NavigateUrl);
				lnkPayPalTransactions      .Visible = imgPayPal.Visible;
				lblPayPalTransactions      .Visible = imgPayPal.Visible;

				imgPayTrace                .Visible = Utils.CachedFileExists(Context, lnkPayTrace.NavigateUrl);
				lnkPayTrace                .Visible = imgPayTrace.Visible;
				lblPayTrace                .Visible = imgPayTrace.Visible;
				imgPayTraceTransactions    .Visible = Utils.CachedFileExists(Context, lnkPayTraceTransactions.NavigateUrl);
				lnkPayTraceTransactions    .Visible = imgPayTrace.Visible;
				lblPayTraceTransactions    .Visible = imgPayTrace.Visible;
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

