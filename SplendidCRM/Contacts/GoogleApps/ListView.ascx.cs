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
using System.Data.Common;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

using Google.Apis.Contacts.v3;
using Google.Apis.Contacts.v3.Data;

namespace SplendidCRM.Contacts.GoogleApps
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected _controls.HeaderButtons ctlModuleHeader;
		protected _controls.ExportHeader ctlExportHeader;
		protected _controls.SearchView   ctlSearchView  ;
		protected _controls.CheckAll     ctlCheckAll    ;

		protected UniqueStringCollection arrSelectFields;
		protected DataView      vwMain         ;
		protected SplendidGrid  grdMain        ;
		protected Label         lblError       ;
		protected MassUpdate    ctlMassUpdate  ;
		// 09/05/2015 Paul.  Google now uses OAuth 2.0. 
		protected TextBox       OAUTH_ACCESS_TOKEN       ;
		protected TextBox       OAUTH_REFRESH_TOKEN      ;
		protected TextBox       OAUTH_EXPIRES_IN         ;
		protected Button        btnGoogleAuthorized      ;
		// 06/06/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected Panel         pnlMassUpdateSeven;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Search" )
				{
					// 10/13/2005 Paul.  Make sure to clear the page index prior to applying search. 
					grdMain.CurrentPageIndex = 0;
					// 04/27/2008 Paul.  Sorting has been moved to the database to increase performance. 
					grdMain.DataBind();
				}
				// 12/14/2007 Paul.  We need to capture the sort event from the SearchView. 
				else if ( e.CommandName == "SortGrid" )
				{
					grdMain.SetSortFields(e.CommandArgument as string[]);
					// 04/27/2008 Paul.  Sorting has been moved to the database to increase performance. 
					// 03/17/2011 Paul.  We need to treat a comma-separated list of fields as an array. 
					arrSelectFields.AddFields(grdMain.SortColumn);
				}
				// 11/17/2010 Paul.  Populate the hidden Selected field with all IDs. 
				else if ( e.CommandName == "SelectAll" )
				{
					// 05/22/2011 Paul.  When using custom paging, vwMain may not be defined. 
					if ( vwMain == null )
						grdMain.DataBind();
					ctlCheckAll.SelectAll(vwMain, "ID");
					grdMain.DataBind();
				}
				// 06/06/2015 Paul.  Change standard MassUpdate command to a command to toggle visibility. 
				else if ( e.CommandName == "ToggleMassUpdate" )
				{
					pnlMassUpdateSeven.Visible = !pnlMassUpdateSeven.Visible;
				}
				else if ( e.CommandName == "MassDelete" )
				{
					// 11/27/2010 Paul.  Use new selected items. 
					string[] arrID = ctlCheckAll.SelectedItemsArray;
					if ( arrID != null )
					{
						Google.Apis.Services.BaseClientService.Initializer initializer = SplendidCRM.GoogleApps.GetUserCredentialInitializer(Application, Security.USER_ID, ContactsService.Scope.Contacts);
						ContactsService service = new ContactsService(initializer);
						foreach ( string sId in arrID )
						{
							ContactsResource.DeleteRequest reqDelete = service.Contacts.Delete(sId);
							reqDelete.Execute();
						}
						Response.Redirect("default.aspx");
					}
				}
				else if ( e.CommandName == "Delete" )
				{
					string sId = Sql.ToString(e.CommandArgument);
					Google.Apis.Services.BaseClientService.Initializer initializer = SplendidCRM.GoogleApps.GetUserCredentialInitializer(Application, Security.USER_ID, ContactsService.Scope.Contacts);
					ContactsService service = new ContactsService(initializer);
					ContactsResource.DeleteRequest reqDelete = service.Contacts.Delete(sId);
					reqDelete.Execute();
					Response.Redirect("default.aspx");
				}
				else if ( e.CommandName == "Export" )
				{
					// 11/03/2006 Paul.  Apply ACL rules to Export. 
					int nACLACCESS = SplendidCRM.Security.GetUserAccess(m_sMODULE, "export");
					if ( nACLACCESS  >= 0 )
					{
						// 10/05/2009 Paul.  When exporting, we may need to manually bind.  Custom paging should be disabled when exporting all. 
						if ( vwMain == null )
							grdMain.DataBind();
						if ( nACLACCESS == ACL_ACCESS.OWNER )
							vwMain.RowFilter = "ASSIGNED_USER_ID = '" + Security.USER_ID.ToString() + "'";
						// 11/27/2010 Paul.  Use new selected items. 
						string[] arrID = ctlCheckAll.SelectedItemsArray;
						SplendidExport.Export(vwMain, m_sMODULE, ctlExportHeader.ExportFormat, ctlExportHeader.ExportRange, grdMain.CurrentPageIndex, grdMain.PageSize, arrID, grdMain.AllowCustomPaging);
					}
				}
				// 09/05/2015 Paul.  Google now uses OAuth 2.0. 
				else if ( e.CommandName == "GoogleApps.Authorize" )
				{
					try
					{
						DateTime dtOAUTH_EXPIRES_AT = DateTime.Now.AddSeconds(Sql.ToInteger(OAUTH_EXPIRES_IN.Text));
						SqlProcs.spOAUTH_TOKENS_Update(Security.USER_ID, "GoogleApps", OAUTH_ACCESS_TOKEN.Text, String.Empty, dtOAUTH_EXPIRES_AT, OAUTH_REFRESH_TOKEN.Text);
						lblError.Text = L10n.Term("Google.LBL_TEST_SUCCESSFUL");
					}
					catch(Exception ex)
					{
						lblError.Text = ex.Message;
					}
				}
				else if ( e.CommandName == "GoogleApps.RefreshToken" )
				{
					try
					{
						SplendidCRM.GoogleApps.RefreshAccessToken(Application, Security.USER_ID, true);
						lblError.Text = L10n.Term("Google.LBL_TEST_SUCCESSFUL");
						Bind();
					}
					catch(Exception ex)
					{
						lblError.Text =  Utils.ExpandException(ex);
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		private void Bind()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("ID"                        , typeof(String  ));
			dt.Columns.Add("NAME"                      , typeof(String  ));
			dt.Columns.Add("DATE_ENTERED"              , typeof(DateTime));
			dt.Columns.Add("DATE_MODIFIED"             , typeof(DateTime));
			dt.Columns.Add("ASSIGNED_TO_NAME"          , typeof(String  ));
			dt.Columns.Add("CREATED_BY_NAME"           , typeof(String  ));
			dt.Columns.Add("MODIFIED_BY_NAME"          , typeof(String  ));
			dt.Columns.Add("MODIFIED_USER_ID"          , typeof(Guid    ));
			dt.Columns.Add("TEAM_ID"                   , typeof(Guid    ));
			dt.Columns.Add("TEAM_NAME"                 , typeof(String  ));
			dt.Columns.Add("TEAM_SET_NAME"             , typeof(String  ));
			dt.Columns.Add("TEAM_SET_LIST"             , typeof(String  ));
			dt.Columns.Add("SALUTATION"                , typeof(String  ));
			dt.Columns.Add("FIRST_NAME"                , typeof(String  ));
			dt.Columns.Add("LAST_NAME"                 , typeof(String  ));
			dt.Columns.Add("LEAD_SOURCE"               , typeof(String  ));
			dt.Columns.Add("TITLE"                     , typeof(String  ));
			dt.Columns.Add("DEPARTMENT"                , typeof(String  ));
			dt.Columns.Add("BIRTHDATE"                 , typeof(DateTime));
			dt.Columns.Add("DO_NOT_CALL"               , typeof(bool    ));
			dt.Columns.Add("PHONE_HOME"                , typeof(String  ));
			dt.Columns.Add("PHONE_MOBILE"              , typeof(String  ));
			dt.Columns.Add("PHONE_WORK"                , typeof(String  ));
			dt.Columns.Add("PHONE_OTHER"               , typeof(String  ));
			dt.Columns.Add("PHONE_FAX"                 , typeof(String  ));
			dt.Columns.Add("EMAIL1"                    , typeof(String  ));
			dt.Columns.Add("EMAIL2"                    , typeof(String  ));
			dt.Columns.Add("ASSISTANT"                 , typeof(String  ));
			dt.Columns.Add("ASSISTANT_PHONE"           , typeof(String  ));
			dt.Columns.Add("EMAIL_OPT_OUT"             , typeof(bool    ));
			dt.Columns.Add("INVALID_EMAIL"             , typeof(bool    ));
			dt.Columns.Add("SMS_OPT_IN"                , typeof(String  ));
			dt.Columns.Add("TWITTER_SCREEN_NAME"       , typeof(String  ));
			dt.Columns.Add("PRIMARY_ADDRESS_STREET"    , typeof(String  ));
			dt.Columns.Add("PRIMARY_ADDRESS_CITY"      , typeof(String  ));
			dt.Columns.Add("PRIMARY_ADDRESS_STATE"     , typeof(String  ));
			dt.Columns.Add("PRIMARY_ADDRESS_POSTALCODE", typeof(String  ));
			dt.Columns.Add("PRIMARY_ADDRESS_COUNTRY"   , typeof(String  ));
			dt.Columns.Add("ALT_ADDRESS_STREET"        , typeof(String  ));
			dt.Columns.Add("ALT_ADDRESS_CITY"          , typeof(String  ));
			dt.Columns.Add("ALT_ADDRESS_STATE"         , typeof(String  ));
			dt.Columns.Add("ALT_ADDRESS_POSTALCODE"    , typeof(String  ));
			dt.Columns.Add("ALT_ADDRESS_COUNTRY"       , typeof(String  ));
			dt.Columns.Add("DESCRIPTION"               , typeof(String  ));
			dt.Columns.Add("GROUPS"                    , typeof(String  ));
			
			// https://developers.google.com/google-apps/contacts/v3/?hl=en
			// https://developers.google.com/google-apps/contacts/v3/reference
			Google.Apis.Services.BaseClientService.Initializer initializer = SplendidCRM.GoogleApps.GetUserCredentialInitializer(Application, Security.USER_ID, ContactsService.Scope.Contacts);
			
			Dictionary<string, string> dictGroups = new Dictionary<string,string>();
			ContactsService service = new ContactsService(initializer);
			GroupsResource.ListRequest reqGroups = service.Groups.List();
			reqGroups.MaxResults = 10000;
			Google.Apis.Contacts.v3.Data.Groups groups = reqGroups.Execute();
			if ( groups.Feed.Items != null && groups.Feed.Items.Count > 0 )
			{
				string sTestGroupName = Sql.ToString(Application["CONFIG.GoogleApps.GroupName"]).Trim();
				string sTestGroupID = String.Empty;
				foreach ( Google.Apis.Contacts.v3.Data.Group group in groups.Feed.Items )
				{
					dictGroups.Add(group.Id.Value, group.Title.Value);
					//GroupsResource.GetRequest reqGroupsContacts = service.Groups.Get(group.IdOnly);
					//Google.Apis.Contacts.v3.Data.GroupEntry grpEntry = reqGroupsContacts.Execute();
					if ( group.Title.Value == sTestGroupName )
					{
						sTestGroupID = group.IdOnly;
					}
				}
				//if ( Sql.IsEmptyString(sTestGroupID) )
				//{
				//	Google.Apis.Contacts.v3.Data.GroupEntry group = new Google.Apis.Contacts.v3.Data.GroupEntry();
				//	group.CreateNew(sTestGroupName);
				//	GroupsResource.InsertRequest reqInsert = service.Groups.Insert(group);
				//	reqInsert.Execute();
				//}
				//else
				//{
				//	GroupsResource.DeleteRequest reqDelete = service.Groups.Delete(sTestGroupID);
				//	reqDelete.Execute();
				//}
			}
			
			StringBuilder sbQuery = new StringBuilder();
			string sFIRST_NAME = new DynamicControl(ctlSearchView, "FIRST_NAME").Text;
			string sLAST_NAME  = new DynamicControl(ctlSearchView, "LAST_NAME" ).Text;
			string sEMAIL1     = new DynamicControl(ctlSearchView, "EMAIL1"    ).Text;
			if ( !Sql.IsEmptyString(sFIRST_NAME) )
			{
				sbQuery.Append(sFIRST_NAME);
			}
			if ( !Sql.IsEmptyString(sLAST_NAME) )
			{
				if ( sbQuery.Length > 0 )
					sbQuery.Append(" ");
				sbQuery.Append(sLAST_NAME);
			}
			if ( !Sql.IsEmptyString(sEMAIL1) )
			{
				if ( sbQuery.Length > 0 )
					sbQuery.Append(" ");
				sbQuery.Append(sEMAIL1);
			}
			
			ContactsResource.ListRequest reqContacts = service.Contacts.List();
			reqContacts.ShowDeleted = false;
			reqContacts.MaxResults  = 10000;
			if ( sbQuery.Length > 0 )
				reqContacts.Query = sbQuery.ToString();
			Google.Apis.Contacts.v3.Data.Contacts contacts = reqContacts.Execute();
			if ( contacts.Feed.Items != null && contacts.Feed.Items.Count > 0 )
			{
				foreach ( Google.Apis.Contacts.v3.Data.Contact contact in contacts.Feed.Items )
				{
					DataRow row = dt.NewRow();
					//ContactsResource.GetRequest reqContact = service.Contacts.Get(contact.IdOnly);
					//Google.Apis.Contacts.v3.Data.ContactEntry conEntry = reqContact.Execute();

					row["ID"                        ] = contact.IdOnly;
					row["DATE_MODIFIED"             ] = (contact.Updated.HasValue ? contact.Updated.Value : DateTime.MinValue);
					row["ASSIGNED_TO_NAME"          ] = Security.USER_NAME;
					row["CREATED_BY_NAME"           ] = Security.USER_NAME;
					row["MODIFIED_BY_NAME"          ] = Security.USER_NAME;
					row["MODIFIED_USER_ID"          ] = Security.USER_ID  ;
					row["TEAM_NAME"                 ] = Security.TEAM_NAME;
					row["TEAM_ID"                   ] = Security.TEAM_ID  ;
					row["TEAM_SET_NAME"             ] = Security.TEAM_NAME;
					row["LEAD_SOURCE"               ] = String.Empty;
					row["DEPARTMENT"                ] = String.Empty;
					row["BIRTHDATE"                 ] = DateTime.MinValue;
					row["DO_NOT_CALL"               ] = false;
					row["ASSISTANT"                 ] = String.Empty;
					row["ASSISTANT_PHONE"           ] = String.Empty;
					row["EMAIL_OPT_OUT"             ] = false;
					row["INVALID_EMAIL"             ] = false;
					row["SMS_OPT_IN"                ] = String.Empty;
					row["TWITTER_SCREEN_NAME"       ] = String.Empty;
					row["GROUPS"                    ] = String.Empty;
					if ( contact.Name != null )
					{
						row["SALUTATION"] = contact.Name.Salutation;
						row["FIRST_NAME"] = contact.Name.FirstName ;
						row["LAST_NAME" ] = contact.Name.LastName  ;
						row["NAME"      ] = contact.Name.FullName  ;
					}
					else
					{
						row["SALUTATION"] = String.Empty;
						row["FIRST_NAME"] = String.Empty;
						row["LAST_NAME" ] = contact.Title;
						row["NAME"      ] = contact.Title;
					}
					if ( contact.GroupMemberships != null && contact.GroupMemberships.Count > 0 )
					{
						StringBuilder sbGroups = new StringBuilder();
						foreach ( GroupMembership group in contact.GroupMemberships )
						{
							if ( dictGroups.ContainsKey(group.Href) )
							{
								if ( sbGroups.Length > 0 )
									sbGroups.Append(", ");
								sbGroups.Append(dictGroups[group.Href]);
							}
						}
						row["GROUPS"] = sbGroups.ToString();
					}
					if ( contact.Emails != null )
					{
						foreach ( EmailValue email in contact.Emails )
						{
							// https://developers.google.com/gdata/docs/1.0/elements#gdEmail
							if ( email.Rel == "http://schemas.google.com/g/2005#work" || (email.Primary.HasValue && email.Primary.Value) )
							{
								row["EMAIL1"] = email.Address;
							}
							else if ( email.Rel == "http://schemas.google.com/g/2005#other" )
							{
								row["EMAIL2"] = email.Address;
							}
						}
					}
					if ( contact.PhoneNumbers != null )
					{
						foreach ( PhoneNumberValue phone in contact.PhoneNumbers )
						{
							// https://developers.google.com/gdata/docs/1.0/elements#gdPhoneNumber
							switch ( phone.Rel )
							{
								case "http://schemas.google.com/g/2005#home"    :  row["PHONE_HOME"  ] = phone.Value;  break;
								case "http://schemas.google.com/g/2005#mobile"  :  row["PHONE_MOBILE"] = phone.Value;  break;
								case "http://schemas.google.com/g/2005#work"    :  row["PHONE_WORK"  ] = phone.Value;  break;
								case "http://schemas.google.com/g/2005#other"   :  row["PHONE_OTHER" ] = phone.Value;  break;
								case "http://schemas.google.com/g/2005#work_fax":  row["PHONE_FAX"   ] = phone.Value;  break;
							}
						}
					}
					//if ( contact.PostalAddresses != null )
					//{
					//	foreach ( PostalAddressValue postal in contact.PostalAddresses )
					//	{
					//		// https://developers.google.com/gdata/docs/1.0/elements#gdPostalAddress
					//		if ( postal.Rel == "http://schemas.google.com/g/2005#work" )
					//		{
					//			row["PRIMARY_ADDRESS_STREET"    ] = postal.Value;
					//			row["PRIMARY_ADDRESS_CITY"      ] = String.Empty;
					//			row["PRIMARY_ADDRESS_STATE"     ] = String.Empty;
					//			row["PRIMARY_ADDRESS_POSTALCODE"] = String.Empty;
					//			row["PRIMARY_ADDRESS_COUNTRY"   ] = String.Empty;
					//		}
					//		else if ( postal.Rel == "http://schemas.google.com/g/2005#other" )
					//		{
					//			row["ALT_ADDRESS_STREET"        ] = postal.Value;
					//			row["ALT_ADDRESS_CITY"          ] = String.Empty;
					//			row["ALT_ADDRESS_STATE"         ] = String.Empty;
					//			row["ALT_ADDRESS_POSTALCODE"    ] = String.Empty;
					//			row["ALT_ADDRESS_COUNTRY"       ] = String.Empty;
					//		}
					//	}
					//}
					if ( contact.StructuredPostalAddresses != null )
					{
						foreach ( StructuredPostalAddress postal in contact.StructuredPostalAddresses )
						{
							// https://developers.google.com/gdata/docs/1.0/elements#gdPostalAddress
							if ( postal.Rel == "http://schemas.google.com/g/2005#work" )
							{
								row["PRIMARY_ADDRESS_STREET"    ] = postal.Street    ;
								row["PRIMARY_ADDRESS_CITY"      ] = postal.City      ;
								row["PRIMARY_ADDRESS_STATE"     ] = postal.Street    ;
								row["PRIMARY_ADDRESS_POSTALCODE"] = postal.PostalCode;
								row["PRIMARY_ADDRESS_COUNTRY"   ] = postal.Country   ;
							}
							else if ( postal.Rel == "http://schemas.google.com/g/2005#other" )
							{
								row["ALT_ADDRESS_STREET"        ] = postal.Street    ;
								row["ALT_ADDRESS_CITY"          ] = postal.City      ;
								row["ALT_ADDRESS_STATE"         ] = postal.Street    ;
								row["ALT_ADDRESS_POSTALCODE"    ] = postal.PostalCode;
								row["ALT_ADDRESS_COUNTRY"       ] = postal.Country   ;
							}
						}
					}
					if ( contact.Organizations != null )
					{
						foreach ( OrganizationValue org in contact.Organizations )
						{
							row["DEPARTMENT"  ] = org.Department;
							row["TITLE"       ] = org.Title     ;
							break;
						}
					}
					if ( contact.Notes != null )
						row["DESCRIPTION"] = contact.Notes;

					dt.Rows.Add(row);
				}
			}
			//this.ApplyGridViewRules(m_sMODULE + "." + LayoutListView, dt);
			vwMain = dt.DefaultView;
			grdMain.DataSource = vwMain ;
			ViewState["Contacts"] = dt;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(m_sMODULE + ".LBL_LIST_FORM_TITLE"));
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "list") >= 0);
			if ( !this.Visible || !SplendidCRM.GoogleApps.GoogleAppsEnabled(Application) )
				return;
			// 12/06/2018 Paul.  Skip during precompile. 
			if ( Sql.ToBoolean(Request["PrecompileOnly"]) )
				return;

			try
			{
				string sFIRST_NAME = new DynamicControl(ctlSearchView, "FIRST_NAME").Text;
				string sLAST_NAME  = new DynamicControl(ctlSearchView, "LAST_NAME" ).Text;
				string sEMAIL1     = new DynamicControl(ctlSearchView, "EMAIL1"    ).Text;
				if ( !IsPostBack || !Sql.IsEmptyString(sFIRST_NAME) || !Sql.IsEmptyString(sLAST_NAME) || !Sql.IsEmptyString(sEMAIL1) )
				{
					grdMain.OrderByClause("NAME", "asc");
					Bind();
					
					ctlExportHeader.Visible = true;
					ctlMassUpdate.Visible = ctlExportHeader.Visible && !PrintView && SplendidCRM.Crm.Modules.MassUpdate(m_sMODULE);
					// 06/06/2015 Paul.  Change standard MassUpdate command to a command to toggle visibility. 
					ctlCheckAll  .Visible = ctlExportHeader.Visible && !PrintView && SplendidCRM.Crm.Modules.MassUpdate(m_sMODULE);
				}
				else
				{
					DataTable dt = ViewState["Contacts"] as DataTable;
					if ( dt != null )
					{
						vwMain = dt.DefaultView;
						grdMain.DataSource = vwMain ;
					}
				}
				if ( !IsPostBack )
				{
					grdMain.DataBind();
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This Contact is required by the ASP.NET Web Form Designer.
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
			ctlSearchView  .Command += new CommandEventHandler(Page_Command);
			ctlExportHeader.Command += new CommandEventHandler(Page_Command);
			ctlMassUpdate  .Command += new CommandEventHandler(Page_Command);
			ctlCheckAll    .Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "Contacts";
			SetMenu(m_sMODULE);
			arrSelectFields = new UniqueStringCollection();
			arrSelectFields.Add("NAME");
			this.AppendGridColumns(grdMain, m_sMODULE + "." + LayoutListView, arrSelectFields);
			if ( Security.GetUserAccess(m_sMODULE, "delete") < 0 && Security.GetUserAccess(m_sMODULE, "edit") < 0 )
				ctlMassUpdate.Visible = false;
			
			// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
			if ( SplendidDynamic.StackedLayout(Page.Theme) )
			{
				ctlModuleHeader.Command += new CommandEventHandler(Page_Command);
				ctlModuleHeader.AppendButtons(m_sMODULE + "." + LayoutListView, Guid.Empty, null);
				// 06/05/2015 Paul.  Move MassUpdate buttons to the SplendidGrid. 
				grdMain.IsMobile       = this.IsMobile;
				grdMain.MassUpdateView = m_sMODULE + ".MassUpdate";
				grdMain.Command       += new CommandEventHandler(Page_Command);
				if ( !IsPostBack )
					pnlMassUpdateSeven.Visible = false;
			}
		}
		#endregion
	}
}

