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
	///		Summary description for CloudView.
	/// </summary>
	public class CloudView : SplendidControl
	{
		protected Image     imgGOOGLE;
		protected HyperLink lnkGOOGLE;
		protected Label     lblGOOGLE;

		protected Image     imgQuickBooks;
		protected HyperLink lnkQuickBooks;
		protected Label     lblQuickBooks;

		protected Image     imgHubSpot;
		protected HyperLink lnkHubSpot;
		protected Label     lblHubSpot;

		protected Image     imgiContact;
		protected HyperLink lnkiContact;
		protected Label     lbliContact;

		protected Image     imgConstantContact;
		protected HyperLink lnkConstantContact;
		protected Label     lblConstantContact;

		protected Image     imgMarketo;
		protected HyperLink lnkMarketo;
		protected Label     lblMarketo;

		protected Image     imgGetResponse;
		protected HyperLink lnkGetResponse;
		protected Label     lblGetResponse;

		protected Image     imgMailChimp;
		protected HyperLink lnkMailChimp;
		protected Label     lblMailChimp;

		protected Image     imgPardot;
		protected HyperLink lnkPardot;
		protected Label     lblPardot;

		protected Image     imgWatson;
		protected HyperLink lnkWatson;
		protected Label     lblWatson;

		protected Image     imgPhoneBurner;
		protected HyperLink lnkPhoneBurner;
		protected Label     lblPhoneBurner;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( !IsPostBack )
			{
				// 08/25/2013 Paul.  File IO is slow, so cache existance test. 
				imgGOOGLE.Visible = Utils.CachedFileExists(Context, lnkGOOGLE.NavigateUrl);
				lnkGOOGLE.Visible = imgGOOGLE.Visible;
				lblGOOGLE.Visible = imgGOOGLE.Visible;
				// 08/25/2013 Paul.  File IO is slow, so cache existance test. 
				imgQuickBooks.Visible = Utils.CachedFileExists(Context, lnkQuickBooks.NavigateUrl);
				lnkQuickBooks.Visible = imgQuickBooks.Visible;
				lblQuickBooks.Visible = imgQuickBooks.Visible;
				// 04/27/2015 Paul.  Add support for HubSpot to Professional or higher. 
				imgHubSpot.Visible = Utils.CachedFileExists(Context, lnkHubSpot.NavigateUrl);
				lnkHubSpot.Visible = imgHubSpot.Visible;
				lblHubSpot.Visible = imgHubSpot.Visible;
				// 06/28/2015 Paul.  Add support for iContact to Professional or higher. 
				imgiContact.Visible = Utils.CachedFileExists(Context, lnkiContact.NavigateUrl);
				lnkiContact.Visible = imgiContact.Visible;
				lbliContact.Visible = imgiContact.Visible;
				// 06/28/2015 Paul.  Add support for ConstantContact to Professional or higher. 
				imgConstantContact.Visible = Utils.CachedFileExists(Context, lnkConstantContact.NavigateUrl);
				lnkConstantContact.Visible = imgConstantContact.Visible;
				lblConstantContact.Visible = imgConstantContact.Visible;
				// 06/28/2015 Paul.  Add support for Marketo to Professional or higher. 
				imgMarketo.Visible = Utils.CachedFileExists(Context, lnkMarketo.NavigateUrl);
				lnkMarketo.Visible = imgMarketo.Visible;
				lblMarketo.Visible = imgMarketo.Visible;
				// 06/28/2015 Paul.  Add support for GetResponse to Professional or higher. 
				imgGetResponse.Visible = Utils.CachedFileExists(Context, lnkGetResponse.NavigateUrl);
				// 06/28/2015 Paul.  We are not ready to support GetResponse. 
#if !DEBUG
				imgGetResponse.Visible = false;
#endif
				lnkGetResponse.Visible = imgGetResponse.Visible;
				lblGetResponse.Visible = imgGetResponse.Visible;
				// 04/30/2016 Paul.  Add support MailChimp. 
				imgMailChimp.Visible = Utils.CachedFileExists(Context, lnkMailChimp.NavigateUrl);
				lnkMailChimp.Visible = imgMailChimp.Visible;
				lblMailChimp.Visible = imgMailChimp.Visible;
				// 07/15/2017 Paul.  Add support Pardot. 
				imgPardot.Visible = Utils.CachedFileExists(Context, lnkPardot.NavigateUrl);
				lnkPardot.Visible = imgPardot.Visible;
				lblPardot.Visible = imgPardot.Visible;
				// 01/25/2018 Paul.  Add support Watson. 
				imgWatson.Visible = Utils.CachedFileExists(Context, lnkWatson.NavigateUrl);
				lnkWatson.Visible = imgWatson.Visible;
				lblWatson.Visible = imgWatson.Visible;
				// 09/11/2020 Paul.  Add support PhoneBurner. 
				imgPhoneBurner.Visible = Utils.CachedFileExists(Context, lnkPhoneBurner.NavigateUrl);
				lnkPhoneBurner.Visible = imgPhoneBurner.Visible;
				lblPhoneBurner.Visible = imgPhoneBurner.Visible;
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

