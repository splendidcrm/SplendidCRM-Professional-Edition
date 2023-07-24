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
using System.IO;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Globalization;
using System.Threading;
using System.Diagnostics;

namespace SplendidCRM.Users
{
	/// <summary>
	///		Summary description for ChangePassword.
	/// </summary>
	public class ChangePasswordView : SplendidControl
	{
		protected SplendidPassword ctlNEW_PASSWORD_STRENGTH;
		protected _controls.ModuleHeader ctlModuleHeader;

		protected Guid            gID                             ;
		protected Guid            gUSER_ID                        ;
		protected string          sUSER_NAME                      ;
		
		protected TableRow        trError                         ;
		protected Label           lblError                        ;
		protected TextBox         txtUSER_NAME                    ;
		protected TextBox         txtNEW_PASSWORD                 ;
		protected TextBox         txtCONFIRM_PASSWORD             ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Login" )
				{
					bool bPageIsValid = Page.IsValid;
					txtUSER_NAME       .Text = txtUSER_NAME       .Text.Trim();
					txtNEW_PASSWORD    .Text = txtNEW_PASSWORD    .Text.Trim();
					txtCONFIRM_PASSWORD.Text = txtCONFIRM_PASSWORD.Text.Trim();
					if ( Sql.IsEmptyString(txtNEW_PASSWORD.Text) )
					{
						trError.Visible = true;
						lblError.Text = L10n.TermJavaScript("Users.ERR_ENTER_NEW_PASSWORD");
						bPageIsValid = false;
					}
					else if ( Sql.IsEmptyString(txtCONFIRM_PASSWORD.Text) )
					{
						trError.Visible = true;
						lblError.Text = L10n.TermJavaScript("Users.ERR_ENTER_CONFIRMATION_PASSWORD");
						bPageIsValid = false;
					}
					// 02/16/2011 Paul.  Fix the error condition.  It was previously generating an error if the values were identical. 
					else if ( txtNEW_PASSWORD.Text != txtCONFIRM_PASSWORD.Text )
					{
						trError.Visible = true;
						lblError.Text = L10n.TermJavaScript("Users.ERR_REENTER_PASSWORDS");
						bPageIsValid = false;
					}
					else
					{
						string sPASSWORD_REQUIREMENTS = String.Empty;
						if ( !ctlNEW_PASSWORD_STRENGTH.IsValid(txtNEW_PASSWORD.Text, ref sPASSWORD_REQUIREMENTS) )
						{
							trError.Visible = true;
							lblError.Text = sPASSWORD_REQUIREMENTS;
							bPageIsValid = false;
						}
					}
					if ( bPageIsValid )
					{
						if ( txtUSER_NAME.Text == sUSER_NAME )
						{
							string sUSER_HASH = Security.HashPassword(txtNEW_PASSWORD.Text);
							DbProviderFactory dbf = DbProviderFactories.GetFactory();
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								string sSQL;
								// 02/20/2011 Paul.  Prevent use of previous passwords. 
								sSQL = "select count(*)                " + ControlChars.CrLf
								     + "  from vwUSERS_PASSWORD_HISTORY" + ControlChars.CrLf
								     + " where USER_ID   = @USER_ID    " + ControlChars.CrLf
								     + "   and USER_HASH = @USER_HASH  " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@USER_ID"  , gUSER_ID  );
									Sql.AddParameter(cmd, "@USER_HASH", sUSER_HASH);
									int nLastPassword = Sql.ToInteger(cmd.ExecuteScalar());
									if ( nLastPassword > 0 )
									{
										trError.Visible = true;
										lblError.Text = L10n.Term("Users.ERR_CANNOT_REUSE_PASSWORD");
										return;
									}
								}
								using ( IDbTransaction trn = Sql.BeginTransaction(con) )
								{
									try
									{
										SqlProcs.spUSERS_PasswordUpdate(gUSER_ID, sUSER_HASH, trn);
										SqlProcs.spUSERS_PASSWORD_LINK_Delete(gID, trn);
										trn.Commit();
									}
									catch(Exception ex)
									{
										trn.Rollback();
										SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
										throw;
									}
								}
							}
						}
						// 03/05/2011 Paul.  After changing the password, use the new credentials to login. 
						bool bValidUser = SplendidInit.LoginUser(sUSER_NAME, txtNEW_PASSWORD.Text, String.Empty, String.Empty);
						if ( bValidUser )
						{
							LoginRedirect();
						}
						else
						{
							trError.Visible = true;
							lblError.Text = L10n.Term("Users.ERR_INVALID_PASSWORD");
						}
					}
				}
			}
			catch(Exception ex)
			{
				trError.Visible = true;
				lblError.Text = ex.Message;
				return;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 08/18/2011 Paul.  Make sure to use the terminology table for the browser title. 
			SetPageTitle(L10n.Term(".LBL_BROWSER_TITLE"));
			// 12/06/2018 Paul.  Skip during precompile. 
			if ( Sql.ToBoolean(Request["PrecompileOnly"]) )
			{
				this.Visible = false;
				return;
			}
			try
			{
				if ( !IsPostBack )
				{
					ctlNEW_PASSWORD_STRENGTH.PreferredPasswordLength             = Crm.Password.PreferredPasswordLength            ;
					ctlNEW_PASSWORD_STRENGTH.MinimumLowerCaseCharacters          = Crm.Password.MinimumLowerCaseCharacters         ;
					ctlNEW_PASSWORD_STRENGTH.MinimumUpperCaseCharacters          = Crm.Password.MinimumUpperCaseCharacters         ;
					ctlNEW_PASSWORD_STRENGTH.MinimumNumericCharacters            = Crm.Password.MinimumNumericCharacters           ;
					ctlNEW_PASSWORD_STRENGTH.MinimumSymbolCharacters             = Crm.Password.MinimumSymbolCharacters            ;
					ctlNEW_PASSWORD_STRENGTH.PrefixText                          = Crm.Password.PrefixText                         ;
					ctlNEW_PASSWORD_STRENGTH.TextStrengthDescriptions            = Crm.Password.TextStrengthDescriptions           ;
					ctlNEW_PASSWORD_STRENGTH.SymbolCharacters                    = Crm.Password.SymbolCharacters                   ;
					ctlNEW_PASSWORD_STRENGTH.ComplexityNumber                    = Crm.Password.ComplexityNumber                   ;

					ctlNEW_PASSWORD_STRENGTH.MessageRemainingCharacters          = L10n.Term("Users.LBL_PASSWORD_REMAINING_CHARACTERS");
					ctlNEW_PASSWORD_STRENGTH.MessageRemainingNumbers             = L10n.Term("Users.LBL_PASSWORD_REMAINING_NUMBERS"   );
					ctlNEW_PASSWORD_STRENGTH.MessageRemainingLowerCase           = L10n.Term("Users.LBL_PASSWORD_REMAINING_LOWERCASE" );
					ctlNEW_PASSWORD_STRENGTH.MessageRemainingUpperCase           = L10n.Term("Users.LBL_PASSWORD_REMAINING_UPPERCASE" );
					ctlNEW_PASSWORD_STRENGTH.MessageRemainingMixedCase           = L10n.Term("Users.LBL_PASSWORD_REMAINING_MIXEDCASE" );
					ctlNEW_PASSWORD_STRENGTH.MessageRemainingSymbols             = L10n.Term("Users.LBL_PASSWORD_REMAINING_SYMBOLS"   );
					ctlNEW_PASSWORD_STRENGTH.MessageSatisfied                    = L10n.Term("Users.LBL_PASSWORD_SATISFIED"           );
				}
				
				gID = Sql.ToGuid(Request["ID"]);
				DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					sSQL = "select *                    " + ControlChars.CrLf
					     + "  from vwUSERS_PASSWORD_LINK" + ControlChars.CrLf
					     + " where ID = @ID             " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@ID", gID);
						using ( IDataReader rdr = cmd.ExecuteReader() )
						{
							if ( rdr.Read() )
							{
								gUSER_ID   = Sql.ToGuid  (rdr["USER_ID"  ]);
								sUSER_NAME = Sql.ToString(rdr["USER_NAME"]);
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
#if !DEBUG
			if ( Sql.IsEmptyGuid(gUSER_ID) || Sql.IsEmptyString(sUSER_NAME) )
				Response.Redirect("~/Users/Login.aspx");
#endif
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

