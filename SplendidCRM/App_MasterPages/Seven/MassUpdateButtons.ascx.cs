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
using System.Data;

namespace SplendidCRM.Themes.Seven
{
	/// <summary>
	///		Summary description for MassUpdateButtons.
	/// </summary>
	public class MassUpdateButtons : SplendidCRM.Themes.Sugar.MassUpdateButtons
	{
		protected Panel       phButtonHover;
		protected AjaxControlToolkit.HoverMenuExtender hexHoverMenuExtender;

		#region DynamicButton methods

		public override void DisableAll()
		{
			base.DisableAll();
			if ( phButtonHover != null )
			{
				foreach ( Control ctl in phButtonHover.Controls )
				{
					if ( ctl is Button )
						(ctl as Button).Enabled = false;
					else if ( ctl is HyperLink )
						(ctl as HyperLink).Enabled = false;
					else if ( ctl is ImageButton )
						(ctl as ImageButton).Enabled = false;
				}
			}
		}

		public override void HideAll()
		{
			base.HideAll();
			if ( phButtonHover != null )
			{
				foreach ( Control ctl in phButtonHover.Controls )
				{
					if ( ctl is Button )
						(ctl as Button).Visible = false;
					else if ( ctl is HyperLink )
						(ctl as HyperLink).Visible = false;
					else if ( ctl is ImageButton )
						(ctl as ImageButton).Visible = false;
				}
			}
		}

		public override void ShowAll()
		{
			base.ShowAll();
			if ( phButtonHover != null )
			{
				foreach ( Control ctl in phButtonHover.Controls )
				{
					if ( ctl is Button )
						(ctl as Button).Visible = true;
					else if ( ctl is HyperLink )
						(ctl as HyperLink).Visible = true;
					else if ( ctl is ImageButton )
						(ctl as ImageButton).Visible = true;
				}
			}
		}

		public override void ShowButton(string sCommandName, bool bVisible)
		{
			base.ShowButton(sCommandName, bVisible);
			if ( phButtonHover != null )
			{
				foreach ( Control ctl in phButtonHover.Controls )
				{
					if ( ctl is Button )
					{
						Button btn = ctl as Button;
						if ( btn.CommandName == sCommandName )
							btn.Visible = bVisible;
					}
					else if ( ctl is ImageButton )
					{
						ImageButton btn = ctl as ImageButton;
						if ( btn.CommandName == sCommandName )
							btn.Visible = bVisible;
					}
				}
			}
		}

		public override void ShowHyperLink(string sURL, bool bVisible)
		{
			base.ShowHyperLink(sURL, bVisible);
			if ( phButtonHover != null )
			{
				foreach ( Control ctl in phButtonHover.Controls )
				{
					if ( ctl is HyperLink )
					{
						HyperLink lnk = ctl as HyperLink;
						if ( lnk.NavigateUrl == sURL )
							lnk.Visible = bVisible;
					}
				}
			}
		}

		public override void EnableButton(string sCommandName, bool bEnabled)
		{
			base.EnableButton(sCommandName, bEnabled);
			if ( phButtonHover != null )
			{
				foreach ( Control ctl in phButtonHover.Controls )
				{
					if ( ctl is Button )
					{
						Button btn = ctl as Button;
						if ( btn.CommandName == sCommandName )
							btn.Enabled = bEnabled;
					}
					else if ( ctl is ImageButton )
					{
						ImageButton btn = ctl as ImageButton;
						if ( btn.CommandName == sCommandName )
							btn.Enabled = bEnabled;
					}
				}
			}
		}

		public override void SetButtonText(string sCommandName, string sText)
		{
			base.SetButtonText(sCommandName, sText);
			if ( phButtonHover != null )
			{
				foreach ( Control ctl in phButtonHover.Controls )
				{
					if ( ctl is Button )
					{
						Button btn = ctl as Button;
						if ( btn.CommandName == sCommandName )
							btn.Text = sText;
					}
				}
			}
		}

		public override string ButtonClientID(string sCommandName)
		{
			string sClientID = base.ButtonClientID(sCommandName);
			if ( phButtonHover != null && Sql.IsEmptyString(ClientID) )
			{
				foreach ( Control ctl in phButtonHover.Controls )
				{
					if ( ctl is Button )
					{
						Button btn = ctl as Button;
						if ( btn.CommandName == sCommandName )
						{
							sClientID = btn.ClientID;
							break;
						}
					}
					else if ( ctl is ImageButton )
					{
						ImageButton btn = ctl as ImageButton;
						if ( btn.CommandName == sCommandName )
						{
							sClientID = btn.ClientID;
							break;
						}
					}
				}
			}
			return sClientID;
		}

		public override Button FindButton(string sCommandName)
		{
			Button btnCommand = base.FindButton(sCommandName);
			if ( phButtonHover != null && btnCommand == null )
			{
				foreach ( Control ctl in phButtonHover.Controls )
				{
					if ( ctl is Button )
					{
						Button btn = ctl as Button;
						if ( btn.CommandName == sCommandName )
						{
							btnCommand = btn;
							break;
						}
					}
				}
			}
			return btnCommand;
		}
		#endregion

		public override void AppendButtons(string sVIEW_NAME, Guid gASSIGNED_USER_ID, DataRow rdr)
		{
			if ( pnlDynamicButtons != null )
			{
				sVIEW_NAME = ".MassUpdate";
				int nButtonCount = SplendidDynamic.AppendButtons(sVIEW_NAME, gASSIGNED_USER_ID, this.pnlDynamicButtons, this.phButtonHover, "MassUpdateHeader", this.IsMobile, rdr, this.GetL10n(), new CommandEventHandler(Page_Command));
				hexHoverMenuExtender.Enabled = (nButtonCount > 1);
			}
		}

		public override void AppendButtons(string sVIEW_NAME, Guid gASSIGNED_USER_ID, Guid gID)
		{
			if ( pnlDynamicButtons != null )
			{
				using ( DataTable dt = new DataTable() )
				{
					dt.Columns.Add("ID", typeof(Guid));
					DataRow row = dt.NewRow();
					dt.Rows.Add(row);
					row["ID"] = gID;

					sVIEW_NAME = ".MassUpdate";
					int nButtonCount = SplendidDynamic.AppendButtons(sVIEW_NAME, gASSIGNED_USER_ID, this.pnlDynamicButtons, this.phButtonHover, "MassUpdateHeader", this.IsMobile, row, this.GetL10n(), new CommandEventHandler(Page_Command));
					hexHoverMenuExtender.Enabled = (nButtonCount > 1);
				}
			}
		}

		protected override void Page_Command(object sender, CommandEventArgs e)
		{
			if ( Command != null )
				Command(this, e);
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
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

