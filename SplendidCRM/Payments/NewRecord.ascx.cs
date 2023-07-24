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
using System.Text;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Payments
{
	/// <summary>
	///		Summary description for New.
	/// </summary>
	public class NewRecord : NewRecordControl
	{
		protected _controls.DynamicButtons ctlDynamicButtons;
		protected _controls.DynamicButtons ctlFooterButtons ;
		protected _controls.HeaderLeft     ctlHeaderLeft    ;

		protected Guid            gID                             ;
		protected HtmlTable       tblMain                         ;
		protected Label           lblError                        ;
		protected Panel           pnlMain                         ;
		protected Panel           pnlEdit                         ;

		// 05/06/2010 Paul.  We need a common way to attach a command from the Toolbar. 

		public Guid ACCOUNT_ID
		{
			get
			{
				// 02/21/2010 Paul.  An EditView Inline will use the ViewState, and a NewRecord Inline will use the Request. 
				Guid gACCOUNT_ID = Sql.ToGuid(ViewState["ACCOUNT_ID"]);
				if ( Sql.IsEmptyGuid(gACCOUNT_ID) )
					gACCOUNT_ID = Sql.ToGuid(Request["ACCOUNT_ID"]);
				return gACCOUNT_ID;
			}
			set
			{
				ViewState["ACCOUNT_ID"] = value;
			}
		}

		// 05/01/2013 Paul.  Add Contacts field to support B2C. 
		public Guid B2C_CONTACT_ID
		{
			get
			{
				Guid gB2C_CONTACT_ID = Sql.ToGuid(ViewState["B2C_CONTACT_ID"]);
				if ( Sql.IsEmptyGuid(gB2C_CONTACT_ID) )
					gB2C_CONTACT_ID = Sql.ToGuid(Request["B2C_CONTACT_ID"]);
				return gB2C_CONTACT_ID;
			}
			set
			{
				ViewState["B2C_CONTACT_ID"] = value;
			}
		}

		// 04/20/2010 Paul.  Add functions to allow this control to be used as part of an InlineEdit operation. 
		public override bool IsEmpty()
		{
			string sAMOUNT = new DynamicControl(this, "AMOUNT").Text;
			return Sql.IsEmptyString(sAMOUNT);
		}

		public override void ValidateEditViewFields()
		{
			if ( !IsEmpty() )
			{
				this.ValidateEditViewFields(m_sMODULE + "." + sEditView);
				// 10/20/2011 Paul.  Apply Business Rules to NewRecord. 
				this.ApplyEditViewValidationEventRules(m_sMODULE + "." + sEditView);
			}
		}

		public override void Save(Guid gPARENT_ID, string sPARENT_TYPE, IDbTransaction trn)
		{
			if ( IsEmpty() )
				return;
			
			string    sTABLE_NAME    = Crm.Modules.TableName(m_sMODULE);
			DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sTABLE_NAME);
			
			Guid gCURRENCY_ID      = new DynamicControl(this, "CURRENCY_ID"     ).ID;
			Guid gASSIGNED_USER_ID = new DynamicControl(this, "ASSIGNED_USER_ID").ID;
			Guid gTEAM_ID          = new DynamicControl(this, "TEAM_ID"         ).ID;
			// 05/01/2013 Paul.  Add Contacts field to support B2C. 
			Guid gACCOUNT_ID       = new DynamicControl(this, "ACCOUNT_ID"      ).ID;
			Guid gB2C_CONTACT_ID   = new DynamicControl(this, "B2C_CONTACT_ID"  ).ID;
			if ( Sql.IsEmptyGuid(gCURRENCY_ID) )
			{
				// 04/30/2016 Paul.  We should be using the user's currency. 
				gCURRENCY_ID      = Sql.ToGuid(Session["USER_SETTINGS/CURRENCY"]);
			}
			if ( Sql.IsEmptyGuid(gASSIGNED_USER_ID) )
				gASSIGNED_USER_ID = Security.USER_ID;
			if ( Sql.IsEmptyGuid(gTEAM_ID) )
				gTEAM_ID = Security.TEAM_ID;
			if ( Sql.IsEmptyGuid(gACCOUNT_ID) )
				gACCOUNT_ID = this.ACCOUNT_ID;
			if ( sPARENT_TYPE == "Accounts" && !Sql.IsEmptyGuid(gPARENT_ID) )
				gACCOUNT_ID = gPARENT_ID;
			// 05/01/2013 Paul.  Add Contacts field to support B2C. 
			if ( Sql.IsEmptyGuid(gB2C_CONTACT_ID) )
				gB2C_CONTACT_ID = this.B2C_CONTACT_ID;
			if ( sPARENT_TYPE == "Contacts" && !Sql.IsEmptyGuid(gPARENT_ID) )
				gB2C_CONTACT_ID = gPARENT_ID;

			Decimal  dAMOUNT             = new DynamicControl(this, "AMOUNT"            ).DecimalValue;
			// 08/26/2010 Paul.  We need a bank fee field to allow for a difference between allocated and received payment. 
			Decimal  dBANK_FEE           = new DynamicControl(this, "BANK_FEE"          ).DecimalValue;
			DateTime dtPAYMENT_DATE      = new DynamicControl(this, "PAYMENT_DATE"      ).DateValue;
			string   sPAYMENT_TYPE       = new DynamicControl(this, "PAYMENT_TYPE"      ).SelectedValue;
			string   sCUSTOMER_REFERENCE = new DynamicControl(this, "CUSTOMER_REFERENCE").Text;
			string   sDESCRIPTION        = new DynamicControl(this, "DESCRIPTION"       ).Text;
			Guid     gCREDIT_CARD_ID     = new DynamicControl(this, "CREDIT_CARD_ID"    ).ID;
			string   sPAYMENT_NUM        = new DynamicControl(this, "PAYMENT_NUM"       ).Text;
			string   sTEAM_SET_LIST      = new DynamicControl(this, "TEAM_SET_LIST"     ).Text;
			// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
			string   sASSIGNED_SET_LIST  = new DynamicControl(this, "ASSIGNED_SET_LIST" ).Text;
			// 10/12/2010 Paul.  If the currency field is not displayed, then use the default. 
			if ( Sql.IsEmptyGuid(gCURRENCY_ID) )
				gCURRENCY_ID = C10n.ID;
			// 03/12/2008 Paul.  There might be a temporary credit card value that needs to be cleared. 
			if ( sPAYMENT_TYPE != "Credit Card" )
				gCREDIT_CARD_ID = Guid.Empty;
			// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
			float fEXCHANGE_RATE         = 1.0f;
			StringBuilder sbINVOICE_NUMBER = new StringBuilder();
			if ( dtPAYMENT_DATE == DateTime.MinValue )
				dtPAYMENT_DATE = DateTime.Now;

			// 05/01/2013 Paul.  Add Contacts field to support B2C. 
			SqlProcs.spPAYMENTS_Update
				( ref gID
				, gASSIGNED_USER_ID
				, gACCOUNT_ID
				, dtPAYMENT_DATE
				, sPAYMENT_TYPE
				, sCUSTOMER_REFERENCE
				// 05/26/2007 Paul.  Exchange rate is stored in the AllocationsView. 
				, fEXCHANGE_RATE
				, gCURRENCY_ID
				, dAMOUNT
				, sDESCRIPTION
				, gCREDIT_CARD_ID
				, sPAYMENT_NUM
				, gTEAM_ID
				, sTEAM_SET_LIST
				, dBANK_FEE
				, gB2C_CONTACT_ID
				// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
				, sASSIGNED_SET_LIST
				, trn
				);
			SplendidDynamic.UpdateCustomFields(this, trn, gID, sTABLE_NAME, dtCustomFields);
			//SqlProcs.spPAYMENTS_InsRelated(gID, sPARENT_TYPE, gPARENT_ID, trn);
		}

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "NewRecord" )
				{
					// 06/20/2009 Paul.  Use a Dynamic View that is nearly idential to the EditView version. 
					this.ValidateEditViewFields(m_sMODULE + "." + sEditView);
					// 10/20/2011 Paul.  Apply Business Rules to NewRecord. 
					this.ApplyEditViewValidationEventRules(m_sMODULE + "." + sEditView);
					if ( Page.IsValid )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							// 10/20/2011 Paul.  Apply Business Rules to NewRecord. 
							this.ApplyEditViewPreSaveEventRules(m_sMODULE + "." + sEditView, null);
							
							// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								try
								{
									Guid   gPARENT_ID   = this.PARENT_ID;
									string sPARENT_TYPE = this.PARENT_TYPE;
									Save(gPARENT_ID, sPARENT_TYPE, trn);
									trn.Commit();
								}
								catch(Exception ex)
								{
									trn.Rollback();
									SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
									if ( bShowFullForm || bShowCancel )
										ctlFooterButtons.ErrorText = ex.Message;
									else
										lblError.Text = ex.Message;
									return;
								}
							}
							// 10/20/2011 Paul.  Apply Business Rules to NewRecord. 
							// 12/10/2012 Paul.  Provide access to the item data. 
							DataRow rowCurrent = Crm.Modules.ItemEdit(m_sMODULE, gID);
							this.ApplyEditViewPostSaveEventRules(m_sMODULE + "." + sEditView, rowCurrent);
						}
						if ( !Sql.IsEmptyString(RulesRedirectURL) )
							Response.Redirect(RulesRedirectURL);
						// 02/21/2010 Paul.  An error should not forward the command so that the error remains. 
						// In case of success, send the command so that the page can be rebuilt. 
						// 06/02/2010 Paul.  We need a way to pass the ID up the command chain. 
						else if ( Command != null )
							Command(sender, new CommandEventArgs(e.CommandName, gID.ToString()));
						else if ( !Sql.IsEmptyGuid(gID) )
							Response.Redirect("~/" + m_sMODULE + "/view.aspx?ID=" + gID.ToString());
					}
				}
				else if ( Command != null )
				{
					Command(sender, e);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				if ( bShowFullForm || bShowCancel )
					ctlFooterButtons.ErrorText = ex.Message;
				else
					lblError.Text = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 06/04/2006 Paul.  NewRecord should not be displayed if the user does not have edit rights. 
			// 01/02/2020 Paul.  Allow the NewRecord to be disabled per module using config table. 
			this.Visible = (!Sql.ToBoolean(Application["CONFIG." + m_sMODULE + ".DisableNewRecord"]) || sEditView != "NewRecord") && (SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
				return;

			try
			{
				// 05/06/2010 Paul.  Use a special Page flag to override the default IsPostBack behavior. 
				bool bIsPostBack = this.IsPostBack && !NotPostBack;
				if ( !bIsPostBack )
				{
					// 05/06/2010 Paul.  When the control is created out-of-band, we need to manually bind the controls. 
					if ( NotPostBack )
						this.DataBind();
					this.AppendEditViewFields(m_sMODULE + "." + sEditView, tblMain, null);
					new DynamicControl(this, "PAYMENT_DATE").DateValue = DateTime.Today;
					// 06/04/2010 Paul.  Notify the parent that the fields have been loaded. 
					if ( EditViewLoad != null )
						EditViewLoad(this, null);
					
					// 02/21/2010 Paul.  When the Full Form buttons are used, we don't want the panel to have margins. 
					if ( bShowFullForm || bShowCancel || sEditView != "NewRecord" )
					{
						pnlMain.CssClass = "";
						pnlEdit.CssClass = "tabForm";
						
						Guid gPARENT_ID = this.ACCOUNT_ID;
						if ( !Sql.IsEmptyGuid(gPARENT_ID) )
						{
							string sMODULE      = String.Empty;
							string sPARENT_TYPE = String.Empty;
							string sPARENT_NAME = String.Empty;
							SqlProcs.spPARENT_Get( ref gPARENT_ID, ref sMODULE, ref sPARENT_TYPE, ref sPARENT_NAME);
							if ( sPARENT_TYPE == "Accounts" && !Sql.IsEmptyGuid(gPARENT_ID) )
							{
								new DynamicControl(this, "ACCOUNT_ID"  ).ID   = gPARENT_ID;
								new DynamicControl(this, "ACCOUNT_NAME").Text = sPARENT_NAME;
							}
							// 05/01/2013 Paul.  Add Contacts field to support B2C. 
							else if ( sPARENT_TYPE == "Contacts" && !Sql.IsEmptyGuid(gPARENT_ID) )
							{
								new DynamicControl(this, "B2C_CONTACT_ID"  ).ID   = gPARENT_ID;
								new DynamicControl(this, "B2C_CONTACT_NAME").Text = sPARENT_NAME;
							}
						}
					}
					// 10/20/2011 Paul.  Apply Business Rules to NewRecord. 
					this.ApplyEditViewNewEventRules(m_sMODULE + "." + sEditView);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				if ( bShowFullForm || bShowCancel )
					ctlFooterButtons.ErrorText = ex.Message;
				else
					lblError.Text = ex.Message;
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
			ctlFooterButtons .Command += new CommandEventHandler(Page_Command);

			ctlDynamicButtons.AppendButtons("NewRecord." + (bShowFullForm ? "FullForm" : (bShowCancel ? "WithCancel" : "SaveOnly")), Guid.Empty, Guid.Empty);
			ctlFooterButtons .AppendButtons("NewRecord." + (bShowFullForm ? "FullForm" : (bShowCancel ? "WithCancel" : "SaveOnly")), Guid.Empty, Guid.Empty);
			m_sMODULE = "Payments";
			// 05/06/2010 Paul.  Use a special Page flag to override the default IsPostBack behavior. 
			bool bIsPostBack = this.IsPostBack && !NotPostBack;
			if ( bIsPostBack )
			{
				this.AppendEditViewFields(m_sMODULE + "." + sEditView, tblMain, null, ctlFooterButtons.ButtonClientID("NewRecord"));
				// 06/04/2010 Paul.  Notify the parent that the fields have been loaded. 
				if ( EditViewLoad != null )
					EditViewLoad(this, null);
				// 10/20/2011 Paul.  Apply Business Rules to NewRecord. 
				Page.Validators.Add(new RulesValidator(this));
			}
		}
		#endregion
	}
}
