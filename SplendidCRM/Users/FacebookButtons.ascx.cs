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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Users
{
	/// <summary>
	///		Summary description for FacebookButtons.
	/// </summary>
	// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
	public class FacebookButtons : SplendidControl
	{
		protected string sFACEBOOK_ID = String.Empty;

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 09/09/2015 Paul.  Allow this user control to be placed inside an UpdatePanel. 
			if ( this.Parent.FindControl("FACEBOOK_ID") != null )
			{
				sFACEBOOK_ID = this.Parent.FindControl("FACEBOOK_ID").ClientID;
			}
			if ( !IsPostBack )
			{
				// 09/04/2013 Paul.  ASP.NET 4.5 is enforcing a rule that the root be an HtmlElement and not an HtmlGenericControl. 
				HtmlContainerControl htmlRoot = this.Page.Master.FindControl("htmlRoot") as HtmlContainerControl;
				if ( htmlRoot != null )
				{
					htmlRoot.Attributes.Add("xmlns", "http://www.w3.org/1999/xhtml");
					htmlRoot.Attributes.Add("xmlns:fb", "http://www.facebook.com/2008/fbml");
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

