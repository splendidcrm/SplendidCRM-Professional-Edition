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
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Contracts
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
				// 02/21/2010 Paul.  An EditView Inline will use the ViewState, and a NewRecord Inline will use the Request. 
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

		public Guid OPPORTUNITY_ID
		{
			get
			{
				// 02/21/2010 Paul.  An EditView Inline will use the ViewState, and a NewRecord Inline will use the Request. 
				Guid gOPPORTUNITY_ID = Sql.ToGuid(ViewState["OPPORTUNITY_ID"]);
				if ( Sql.IsEmptyGuid(gOPPORTUNITY_ID) )
					gOPPORTUNITY_ID = Sql.ToGuid(Request["OPPORTUNITY_ID"]);
				return gOPPORTUNITY_ID;
			}
			set
			{
				ViewState["OPPORTUNITY_ID"] = value;
			}
		}

		// 04/20/2010 Paul.  Add functions to allow this control to be used as part of an InlineEdit operation. 
		public override bool IsEmpty()
		{
			string sNAME = new DynamicControl(this, "NAME").Text;
			return Sql.IsEmptyString(sNAME);
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
			
			Guid gASSIGNED_USER_ID = new DynamicControl(this, "ASSIGNED_USER_ID").ID;
			Guid gTEAM_ID          = new DynamicControl(this, "TEAM_ID"         ).ID;
			Guid gACCOUNT_ID       = new DynamicControl(this, "ACCOUNT_ID"      ).ID;
			Guid gOPPORTUNITY_ID   = new DynamicControl(this, "OPPORTUNITY_ID"  ).ID;
			// 05/01/2013 Paul.  Add Contacts field to support B2C. 
			Guid gB2C_CONTACT_ID   = new DynamicControl(this, "B2C_CONTACT_ID"  ).ID;
			if ( Sql.IsEmptyGuid(gASSIGNED_USER_ID) )
				gASSIGNED_USER_ID = Security.USER_ID;
			if ( Sql.IsEmptyGuid(gTEAM_ID) )
				gTEAM_ID = Security.TEAM_ID;
			if ( Sql.IsEmptyGuid(gACCOUNT_ID) )
				gACCOUNT_ID = this.ACCOUNT_ID;
			if ( Sql.IsEmptyGuid(gOPPORTUNITY_ID) )
				gOPPORTUNITY_ID = this.OPPORTUNITY_ID;
			if ( sPARENT_TYPE == "Accounts" && !Sql.IsEmptyGuid(gPARENT_ID) )
				gACCOUNT_ID = gPARENT_ID;
			if ( sPARENT_TYPE == "Opportunities" && !Sql.IsEmptyGuid(gPARENT_ID) )
				gOPPORTUNITY_ID = gPARENT_ID;
			// 05/01/2013 Paul.  Add Contacts field to support B2C. 
			if ( Sql.IsEmptyGuid(gB2C_CONTACT_ID) )
				gB2C_CONTACT_ID = this.B2C_CONTACT_ID;
			if ( sPARENT_TYPE == "Contacts" && !Sql.IsEmptyGuid(gPARENT_ID) )
				gB2C_CONTACT_ID = gPARENT_ID;
			// 05/01/2013 Paul.  Add Contacts field to support B2C. 
			SqlProcs.spCONTRACTS_Update
				( ref gID
				, gASSIGNED_USER_ID
				, new DynamicControl(this, "NAME"                ).Text
				, new DynamicControl(this, "REFERENCE_CODE"      ).Text
				, new DynamicControl(this, "STATUS"              ).Text
				, new DynamicControl(this, "TYPE_ID"             ).ID
				, gACCOUNT_ID
				, gOPPORTUNITY_ID
				, new DynamicControl(this, "START_DATE"          ).DateValue
				, new DynamicControl(this, "END_DATE"            ).DateValue
				, new DynamicControl(this, "COMPANY_SIGNED_DATE" ).DateValue
				, new DynamicControl(this, "CUSTOMER_SIGNED_DATE").DateValue
				, new DynamicControl(this, "EXPIRATION_NOTICE"   ).DateValue
				, new DynamicControl(this, "CURRENCY_ID"         ).ID
				, new DynamicControl(this, "TOTAL_CONTRACT_VALUE").DecimalValue
				, new DynamicControl(this, "DESCRIPTION"         ).Text
				, gTEAM_ID
				, new DynamicControl(this, "TEAM_SET_LIST"       ).Text
				, gB2C_CONTACT_ID
				// 05/12/2016 Paul.  Add Tags module. 
				, new DynamicControl(this, "TAG_SET_NAME"        ).Text
				// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
				, new DynamicControl(this, "ASSIGNED_SET_LIST"   ).Text
				, trn
				);
			SplendidDynamic.UpdateCustomFields(this, trn, gID, sTABLE_NAME, dtCustomFields);
			// 04/20/2010 Paul.  For those procedures that do not include a PARENT_TYPE, 
			// we need a new relationship procedure. 
			SqlProcs.spCONTRACTS_InsRelated(gID, sPARENT_TYPE, gPARENT_ID, trn);
		}

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "NewRecord" )
				{
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
									// 10/21/2010 Paul.  Make sure to pass the parent values.
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
					// 06/04/2010 Paul.  Notify the parent that the fields have been loaded. 
					if ( EditViewLoad != null )
						EditViewLoad(this, null);
					// 06/03/2010 Paul.  Prefill the Opportunity and the Account. 
					// 06/03/2010 Paul.  Allow a contract to be created from an Account or an Opportunity. 
					Guid gACCOUNT_ID = this.ACCOUNT_ID;
					if ( !Sql.IsEmptyGuid(gACCOUNT_ID) )
					{
						string sMODULE      = String.Empty;
						string sPARENT_TYPE = String.Empty;
						string sPARENT_NAME = String.Empty;
						SqlProcs.spPARENT_Get(ref gACCOUNT_ID, ref sMODULE, ref sPARENT_TYPE, ref sPARENT_NAME);
						if ( !Sql.IsEmptyGuid(gACCOUNT_ID) )
						{
							new DynamicControl(this, "ACCOUNT_ID"  ).ID   = gACCOUNT_ID;
							new DynamicControl(this, "ACCOUNT_NAME").Text = sPARENT_NAME;
						}
					}
					// 05/01/2013 Paul.  Add Contacts field to support B2C. 
					else if ( !Sql.IsEmptyGuid(this.B2C_CONTACT_ID) )
					{
						Guid gB2C_CONTACT_ID = this.B2C_CONTACT_ID;
						string sMODULE      = String.Empty;
						string sPARENT_TYPE = String.Empty;
						string sPARENT_NAME = String.Empty;
						SqlProcs.spPARENT_Get(ref gB2C_CONTACT_ID, ref sMODULE, ref sPARENT_TYPE, ref sPARENT_NAME);
						if ( !Sql.IsEmptyGuid(gB2C_CONTACT_ID) )
						{
							new DynamicControl(this, "B2C_CONTACT_ID"  ).ID   = gB2C_CONTACT_ID;
							new DynamicControl(this, "B2C_CONTACT_NAME").Text = sPARENT_NAME;
						}
					}
					Guid gOPPORTUNITY_ID = OPPORTUNITY_ID;
					if ( !Sql.IsEmptyGuid(gOPPORTUNITY_ID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL ;
							sSQL = "select *              " + ControlChars.CrLf
							     + "  from vwOPPORTUNITIES" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Security.Filter(cmd, m_sMODULE, "view");
								Sql.AppendParameter(cmd, gOPPORTUNITY_ID, "ID", false);

								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										new DynamicControl(this, "OPPORTUNITY_ID"  ).ID   = gOPPORTUNITY_ID;
										new DynamicControl(this, "OPPORTUNITY_NAME").Text = Sql.ToString(rdr["NAME"        ]);
										new DynamicControl(this, "ACCOUNT_ID"      ).ID   = Sql.ToGuid  (rdr["ACCOUNT_ID"  ]);
										new DynamicControl(this, "ACCOUNT_NAME"    ).Text = Sql.ToString(rdr["ACCOUNT_NAME"]);
										// 05/01/2013 Paul.  Add Contacts field to support B2C. 
										new DynamicControl(this, "B2C_CONTACT_ID"  ).ID   = Sql.ToGuid  (rdr["B2C_CONTACT_ID"  ]);
										new DynamicControl(this, "B2C_CONTACT_NAME").Text = Sql.ToString(rdr["B2C_CONTACT_NAME"]);
									}
								}
							}
						}
					}
					
					// 02/21/2010 Paul.  When the Full Form buttons are used, we don't want the panel to have margins. 
					if ( bShowFullForm || bShowCancel || sEditView != "NewRecord" )
					{
						pnlMain.CssClass = "";
						pnlEdit.CssClass = "tabForm";
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
			m_sMODULE = "Contracts";
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
