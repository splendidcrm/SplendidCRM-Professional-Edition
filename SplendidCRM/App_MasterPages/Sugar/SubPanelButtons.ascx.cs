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

namespace SplendidCRM.Themes.Sugar
{
	/// <summary>
	///		Summary description for SubPanelButtons.
	/// </summary>
	public class SubPanelButtons : SplendidControl
	{
		// 09/25/2016 Paul.  Move tblSubPanelFrame to base class. 
		protected Table       tblSubPanelFrame;
		#region ListHeader
		protected HyperLink lnkShowSubPanel;
		protected HyperLink lnkHideSubPanel;

		protected string    sModule   = String.Empty;
		protected string    sTitle    = String.Empty;
		protected string    sSubPanel = String.Empty;

		public string Module
		{
			get
			{
				return sModule;
			}
			set
			{
				sModule = value;
			}
		}

		public string Title
		{
			get
			{
				return sTitle;
			}
			set
			{
				sTitle = value;
			}
		}

		public string SubPanel
		{
			get
			{
				return sSubPanel;
			}
			set
			{
				sSubPanel = value;
			}
		}
		#endregion

		#region DynamicButtons
		protected PlaceHolder pnlDynamicButtons;
		protected Label       lblError         ;
		protected TableCell   tdButtons        ;
		protected TableCell   tdError          ;
		protected TableCell   tdRequired       ;

		public CommandEventHandler Command;

		public HorizontalAlign HorizontalAlign
		{
			get
			{
				if ( tdButtons != null )
					return tdButtons.HorizontalAlign;
				return HorizontalAlign.Left;
			}
			set
			{
				if ( tdButtons != null )
					tdButtons.HorizontalAlign = value;
			}
		}

		public bool ShowRequired
		{
			get
			{
				if ( tdRequired != null )
					return tdRequired.Visible;
				return false;
			}
			set
			{
				if ( tdRequired != null )
					tdRequired.Visible = value;
			}
		}

		public bool ShowError
		{
			get
			{
				if ( lblError != null )
					return lblError.Visible;
				return false;
			}
			set
			{
				if ( tdError != null )
					tdError.Visible = value;
				if ( lblError != null )
					lblError.Visible = value;
			}
		}

		public string ErrorText
		{
			get
			{
				if ( lblError != null )
					return lblError.Text;
				return String.Empty;
			}
			set
			{
				if ( lblError != null )
					lblError.Text = value;
			}
		}

		public string ErrorClass
		{
			get
			{
				if ( lblError != null )
					return lblError.CssClass;
				return String.Empty;
			}
			set
			{
				if ( lblError != null )
					lblError.CssClass = value;
			}
		}

		public virtual void DisableAll()
		{
			if ( pnlDynamicButtons != null )
			{
				foreach ( Control ctl in pnlDynamicButtons.Controls )
				{
					if ( ctl is Button )
						(ctl as Button).Enabled = false;
					else if ( ctl is HyperLink )
						(ctl as HyperLink).Enabled = false;
				}
			}
		}

		public virtual void HideAll()
		{
			if ( pnlDynamicButtons != null )
			{
				foreach ( Control ctl in pnlDynamicButtons.Controls )
				{
					if ( ctl is Button )
						(ctl as Button).Visible = false;
					else if ( ctl is HyperLink )
						(ctl as HyperLink).Visible = false;
				}
			}
		}

		public virtual void ShowAll()
		{
			if ( pnlDynamicButtons != null )
			{
				foreach ( Control ctl in pnlDynamicButtons.Controls )
				{
					if ( ctl is Button )
						(ctl as Button).Visible = true;
					else if ( ctl is HyperLink )
						(ctl as HyperLink).Visible = true;
				}
			}
		}

		public virtual void ShowButton(string sCommandName, bool bVisible)
		{
			if ( pnlDynamicButtons != null )
			{
				foreach ( Control ctl in pnlDynamicButtons.Controls )
				{
					if ( ctl is Button )
					{
						Button btn = ctl as Button;
						if ( btn.CommandName == sCommandName )
							btn.Visible = bVisible;
					}
				}
			}
		}

		public virtual void ShowHyperLink(string sURL, bool bVisible)
		{
			if ( pnlDynamicButtons != null )
			{
				foreach ( Control ctl in pnlDynamicButtons.Controls )
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

		public virtual void EnableButton(string sCommandName, bool bEnabled)
		{
			if ( pnlDynamicButtons != null )
			{
				foreach ( Control ctl in pnlDynamicButtons.Controls )
				{
					if ( ctl is Button )
					{
						Button btn = ctl as Button;
						if ( btn.CommandName == sCommandName )
							btn.Enabled = bEnabled;
					}
				}
			}
		}

		public virtual void SetButtonText(string sCommandName, string sText)
		{
			if ( pnlDynamicButtons != null )
			{
				foreach ( Control ctl in pnlDynamicButtons.Controls )
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

		public virtual string ButtonClientID(string sCommandName)
		{
			if ( pnlDynamicButtons != null )
			{
				foreach ( Control ctl in pnlDynamicButtons.Controls )
				{
					if ( ctl is Button )
					{
						Button btn = ctl as Button;
						if ( btn.CommandName == sCommandName )
							return btn.ClientID;
					}
				}
			}
			return String.Empty;
		}

		public virtual Button FindButton(string sCommandName)
		{
			if ( pnlDynamicButtons != null )
			{
				foreach ( Control ctl in pnlDynamicButtons.Controls )
				{
					if ( ctl is Button )
					{
						Button btn = ctl as Button;
						if ( btn.CommandName == sCommandName )
							return btn;
					}
				}
			}
			return null;
		}

		public virtual void AppendButtons(string sVIEW_NAME, Guid gASSIGNED_USER_ID, DataRow rdr)
		{
			if ( pnlDynamicButtons != null )
				SplendidDynamic.AppendButtons(sVIEW_NAME, gASSIGNED_USER_ID, pnlDynamicButtons, this.IsMobile, rdr, this.GetL10n(), new CommandEventHandler(Page_Command));
		}

		public virtual void AppendButtons(string sVIEW_NAME, Guid gASSIGNED_USER_ID, Guid gID)
		{
			if ( pnlDynamicButtons != null )
			{
				// 04/29/2008 Paul.  Don't create a reader if the ID is null. 
				// 04/28/2009 Paul.  Always create the reader and read the first row.. 
				using ( DataTable dt = new DataTable() )
				{
					dt.Columns.Add("ID", typeof(Guid));
					DataRow row = dt.NewRow();
					dt.Rows.Add(row);
					row["ID"] = gID;

					// 11/22/2010 Paul.  Convert data reader to data table for Rules Wizard. 
					//using ( DataTableReader rdr = dt.CreateDataReader() )
					{
						// 04/28/2009 Paul.  Make sure to read the first row, otherwise an exception will be thrown when the reader is accessed. 
						//rdr.Read();
						SplendidDynamic.AppendButtons(sVIEW_NAME, gASSIGNED_USER_ID, pnlDynamicButtons, this.IsMobile, row, this.GetL10n(), new CommandEventHandler(Page_Command));
					}
				}
			}
		}
		#endregion

		protected virtual void Page_Command(object sender, CommandEventArgs e)
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

