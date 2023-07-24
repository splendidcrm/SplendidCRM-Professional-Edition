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

namespace SplendidCRM.Users
{
	/// <summary>
	///		Summary description for Signatures.
	/// </summary>
	public class Signatures : SplendidControl
	{
		// 06/03/2015 Paul.  Combine ListHeader and DynamicButtons. 
		protected _controls.SubPanelButtons ctlDynamicButtons;
		protected UniqueStringCollection arrSelectFields;
		protected Guid            gID            ;
		protected DataView        vwMain         ;
		protected SplendidGrid    grdMain        ;
		protected bool            bMyAccount     ;

		public bool MyAccount
		{
			get
			{
				return bMyAccount;
			}
			set
			{
				bMyAccount = value;
			}
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				switch ( e.CommandName )
				{
					case "UserSignatures.Create":
					{
						if ( bMyAccount )
							Response.Redirect("~/Users/UserSignatures/edit.aspx");
						else
							Response.Redirect("~/Users/UserSignatures/edit.aspx?USER_ID=" + gID.ToString());
						break;
					}
					case "UserSignatures.Edit":
					{
						Guid gSIGNATURE_ID = Sql.ToGuid(e.CommandArgument);
						Response.Redirect("~/Users/UserSignatures/edit.aspx?ID=" + gSIGNATURE_ID.ToString());
						break;
					}
					case "UserSignatures.Delete":
					{
						Guid gSIGNATURE_ID = Sql.ToGuid(e.CommandArgument);
						SqlProcs.spUSERS_SIGNATURES_Delete(gSIGNATURE_ID);
						BindGrid();
						break;
					}
					default:
						throw(new Exception("Unknown command: " + e.CommandName));
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
		}

		protected void BindGrid()
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL;
				sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
				     + "  from vwUSERS_SIGNATURES" + ControlChars.CrLf
				     + " where 1 = 1             " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AppendParameter(cmd, gID, "USER_ID");
					cmd.CommandText += grdMain.OrderByClause("NAME", "asc");

					if ( bDebug )
						RegisterClientScriptBlock("vwUSERS_SIGNATURES", Sql.ClientScriptBlock(cmd));

					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								vwMain = dt.DefaultView;
								grdMain.DataSource = vwMain ;
								grdMain.DataBind();
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						ctlDynamicButtons.ErrorText = ex.Message;
					}
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 08/07/2013 Paul.  Team Management should not be required for signatures. 
			gID = Sql.ToGuid(Request["ID"]);
			// 03/08/2007 Paul.  We need to disable the buttons unless the user is an administrator. 
			if ( bMyAccount )
			{
				gID = Security.USER_ID;
			}
			// 09/28/2018 Paul.  Signatures may not be supported. 
			if ( Utils.CachedFileExists(Context, "~/Users/UserSignatures/edit.aspx") && this.Visible )
				BindGrid();
			else
				this.Visible = false;

			if ( !IsPostBack )
			{
				ctlDynamicButtons.AppendButtons("Users." + m_sMODULE, Guid.Empty, gID);
				// 11/19/2008 Paul.  HideAll must be after the buttons are appended.
				// 08/07/2013 Paul.  My Account should have access to their signatures. 
				if ( !bMyAccount && !(SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0) )
				{
					ctlDynamicButtons.HideAll();
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
			ctlDynamicButtons.Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "UserSignatures";
			arrSelectFields = new UniqueStringCollection();
			arrSelectFields.Add("NAME");
			this.AppendGridColumns(grdMain, "Users." + m_sMODULE, arrSelectFields);
			if ( IsPostBack )
				ctlDynamicButtons.AppendButtons("Users." + m_sMODULE, Guid.Empty, Guid.Empty);
		}
		#endregion
	}
}
