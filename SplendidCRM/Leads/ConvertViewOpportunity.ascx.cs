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
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using SplendidCRM._controls;

namespace SplendidCRM.Leads
{
	/// <summary>
	///		Summary description for ConvertViewOpportunity.
	/// </summary>
	public class ConvertViewOpportunity : InlineEditControl
	{
		protected Guid                   gOPPORTUNITY_ID                      ;
		protected Guid                   gACCOUNT_ID                          ;
		protected Guid                   gASSIGNED_USER_ID                    ;
		protected Guid                   gTEAM_ID                             ;
		protected string                 sTEAM_SET_LIST                       ;
		// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
		protected string                 sASSIGNED_SET_LIST                   ;

		protected HtmlTable              tblMain                              ;

		protected CheckBox               chkCreateOpportunity                 ;
		// 11/03/2008 Paul.  Allow selection of an opportunity. 
		protected HtmlInputHidden        txtSELECT_OPPORTUNITY_ID             ;
		protected TextBox                txtSELECT_OPPORTUNITY_NAME           ;
		protected RequiredFieldValidatorForHiddenInputs reqSELECT_OPPORTUNITY_ID;
		protected Label                  lblError                             ;

		protected CheckBox               chkCreateNote                        ;
		protected ConvertViewNote        ctlConvertViewNote                   ;

		public Guid OPPORTUNITY_ID
		{
			get { return gOPPORTUNITY_ID; }
			set { gOPPORTUNITY_ID = value; }
		}

		public Guid ACCOUNT_ID
		{
			get { return gACCOUNT_ID; }
			set { gACCOUNT_ID = value; }
		}

		public Guid ASSIGNED_USER_ID
		{
			get { return gASSIGNED_USER_ID; }
			set { gASSIGNED_USER_ID = value; }
		}

		public Guid TEAM_ID
		{
			get { return gTEAM_ID; }
			set { gTEAM_ID = value; }
		}

		public string TEAM_SET_LIST
		{
			get { return sTEAM_SET_LIST; }
			set { sTEAM_SET_LIST = value; }
		}

		// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
		public string ASSIGNED_SET_LIST
		{
			get { return sASSIGNED_SET_LIST; }
			set { sASSIGNED_SET_LIST = value; }
		}

		public string OPPORTUNITY_NAME
		{
			get { return new DynamicControl(this, "NAME").Text; }
		}

		public string OPPORTUNITY_AMOUNT
		{
			get { return new DynamicControl(this, "AMOUNT").Text; }
		}

		public override bool IsEmpty()
		{
			string sNAME = new DynamicControl(this, "NAME").Text;
			return Sql.IsEmptyString(sNAME);
		}

		public override void ValidateEditViewFields()
		{
			if ( chkCreateOpportunity.Checked )
			{
				this.ValidateEditViewFields           ("Leads.ConvertViewOpportunity");
				this.ApplyEditViewValidationEventRules("Leads.ConvertViewOpportunity");
				if ( chkCreateNote.Checked )
				{
					ctlConvertViewNote.ValidateEditViewFields();
				}
			}
		}

		// 03/22/2016 Paul.  Duplicate check during conversion. 
		public void DuplicateCheck(string sCommandName, IDbConnection con, _controls.HeaderButtons ctlDynamicButtons)
		{
			if ( chkCreateOpportunity.Checked )
			{
				bool bDUPLICATE_CHECHING_ENABLED = Sql.ToBoolean(Application["CONFIG.enable_duplicate_check"]) && Sql.ToBoolean(Application["Modules." + m_sMODULE + ".DuplicateCheckingEnabled"]) && (sCommandName != "SaveDuplicate");
				if ( bDUPLICATE_CHECHING_ENABLED )
				{
					if ( Utils.DuplicateCheck(Application, con, m_sMODULE, Guid.Empty, this, null) > 0 )
					{
						ctlDynamicButtons.ShowButton("SaveDuplicate", true);
						lblError.Text = L10n.Term(".ERR_DUPLICATE_EXCEPTION");
						throw(new Exception(L10n.Term(".moduleList.", m_sMODULE) + ": " + L10n.Term(".ERR_DUPLICATE_EXCEPTION")));
					}
				}
			}
		}

		public override void Save(Guid gPARENT_ID, string sPARENT_TYPE, IDbTransaction trn)
		{
			//if ( IsEmpty() )
			//	return;
			
			string sBusinessMode = Sql.ToString(Application["CONFIG.BusinessMode"]);
			if ( chkCreateOpportunity.Checked )
			{
				string    sTABLE_NAME    = Crm.Modules.TableName(m_sMODULE);
				DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sTABLE_NAME);
			
				if ( FindControl("ASSIGNED_USER_ID") != null )
					gASSIGNED_USER_ID = new DynamicControl(this, "ASSIGNED_USER_ID").ID;
				if ( FindControl("TEAM_ID") != null || FindControl("TEAM_SET_NAME") != null )
					gTEAM_ID = new DynamicControl(this, "TEAM_ID").ID;
				if ( FindControl("TEAM_SET_LIST") != null || FindControl("TEAM_SET_NAME") != null )
					sTEAM_SET_LIST = new DynamicControl(this, "TEAM_SET_LIST").Text;
				// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
				if ( FindControl("ASSIGNED_SET_LIST") != null || FindControl("ASSIGNED_SET_NAME") != null )
					sASSIGNED_SET_LIST = new DynamicControl(this, "ASSIGNED_SET_LIST").Text;
				
				this.ApplyEditViewPreSaveEventRules("Leads.ConvertViewOpportunity", null);
				Guid gB2C_CONTACT_ID = gPARENT_ID;
				// 08/08/2015 Paul.  Separate relationship for Leads/Opportunities. 
				Guid gLEAD_ID = Sql.ToGuid(Request["ID"]);
				SqlProcs.spOPPORTUNITIES_Update
					( ref gOPPORTUNITY_ID
					, gASSIGNED_USER_ID
					, gACCOUNT_ID
					, new DynamicControl(this, "NAME"            ).Text
					, new DynamicControl(this, "OPPORTUNITY_TYPE").SelectedValue
					, new DynamicControl(this, "LEAD_SOURCE"     ).SelectedValue
					, new DynamicControl(this, "AMOUNT"          ).DecimalValue
					, new DynamicControl(this, "CURRENCY_ID"     ).ID
					, new DynamicControl(this, "DATE_CLOSED"     ).DateValue
					, new DynamicControl(this, "NEXT_STEP"       ).Text
					, new DynamicControl(this, "SALES_STAGE"     ).SelectedValue
					, new DynamicControl(this, "PROBABILITY"     ).FloatValue
					, new DynamicControl(this, "DESCRIPTION"     ).Text
					, sPARENT_TYPE
					, gPARENT_ID
					, String.Empty
					, gTEAM_ID
					, sTEAM_SET_LIST
					, new DynamicControl(this, "CAMPAIGN_ID"     ).ID
					, new DynamicControl(this, "EXCHANGE_FOLDER" ).Checked
					, gB2C_CONTACT_ID
					, gLEAD_ID
					// 05/12/2016 Paul.  Add Tags module. 
					, new DynamicControl(this, "TAG_SET_NAME"    ).Text
					// 06/20/2017 Paul.  Add number fields to Contacts, Leads, Prospects, Opportunities and Campaigns. 
					, new DynamicControl(this, "OPPORTUNITY_NUMBER").Text
					// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
					, sASSIGNED_SET_LIST
					, trn
					);
				SplendidDynamic.UpdateCustomFields(this, trn, gOPPORTUNITY_ID, sTABLE_NAME, dtCustomFields);
				SqlProcs.spOPPORTUNITIES_InsRelated(gOPPORTUNITY_ID, sPARENT_TYPE, gPARENT_ID, trn);
				
				if ( chkCreateNote.Checked )
				{
					ctlConvertViewNote.CONTACT_ID       = gPARENT_ID       ;
					ctlConvertViewNote.ASSIGNED_USER_ID = gASSIGNED_USER_ID;
					ctlConvertViewNote.TEAM_ID          = gTEAM_ID         ;
					ctlConvertViewNote.TEAM_SET_LIST    = sTEAM_SET_LIST   ;
					ctlConvertViewNote.Save(gOPPORTUNITY_ID, "Opportunities", trn);
				}
			}
			else
			{
				// 11/03/2008 Paul.  Allow selection of an opportunity. 
				gOPPORTUNITY_ID = Sql.ToGuid(txtSELECT_OPPORTUNITY_ID.Value);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 04/17/2016 Paul.  Hide panel if module disabled. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit") >= 0) && Sql.ToBoolean(Application["Modules." + m_sMODULE + ".Valid"]);
			if ( !this.Visible )
				return;

			try
			{
				reqSELECT_OPPORTUNITY_ID.DataBind();
				Guid gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					chkCreateOpportunity.Attributes.Add("onclick", "return ToggleCreateOpportunity();");
					chkCreateNote       .Attributes.Add("onclick", "toggleDisplay('divCreateOpportunityNote');");
					if ( !Sql.IsEmptyGuid(gID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *                         " + ControlChars.CrLf
							     + "  from vwLEADS_ConvertOpportunity" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Security.Filter(cmd, m_sMODULE, "edit");
								Sql.AppendParameter(cmd, gID, "ID", false);
								con.Open();

								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dtCurrent = new DataTable() )
									{
										da.Fill(dtCurrent);
										if ( dtCurrent.Rows.Count > 0 )
										{
											DataRow rdr = dtCurrent.Rows[0];
											this.ApplyEditViewPreLoadEventRules ("Leads.ConvertViewOpportunity", rdr);
											this.AppendEditViewFields           ("Leads.ConvertViewOpportunity", tblMain, rdr);
											this.ApplyEditViewPostLoadEventRules("Leads.ConvertViewOpportunity", rdr);
										}
									}
								}
							}
						}
					}
					else
					{
						this.AppendEditViewFields      ("Leads.ConvertViewOpportunity", tblMain, null);
						this.ApplyEditViewNewEventRules("Leads.ConvertViewOpportunity");
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				//ctlDynamicButtons.ErrorText = ex.Message;
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
			m_sMODULE = "Opportunities";
			if ( IsPostBack )
			{
				this.AppendEditViewFields("Leads.ConvertViewOpportunity", tblMain, null);
				Page.Validators.Add(new RulesValidator(this));
			}
		}
		#endregion
	}
}

