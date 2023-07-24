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
using System.Threading;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM._controls
{
	/// <summary>
	///		Summary description for KBTagSelect.
	/// </summary>
	public class KBTagSelect : SplendidControl
	{
		protected DataTable       dtLineItems           ;
		protected GridView        grdMain               ;
		protected String          sKBTAG_SET_LIST       ;
		protected Panel           pnlAddReplace         ;
		protected RadioButton     radKBTagSetReplace    ;
		protected RadioButton     radKBTagSetAdd        ;
		protected bool            bShowAddReplace       = false;
		protected RequiredFieldValidatorForKBTagSelect valKBTagSelect;
		protected bool            bEnabled              = true;
		protected bool            bAjaxAutoComplete     = false;
		protected short           nTagIndex             = 12;

		// 12/10/2017 Paul.  Provide a way to set the tab index. 
		public short TabIndex
		{
			get
			{
				return nTagIndex;
			}
			set
			{
				nTagIndex = value;
			}
		}

		public DataTable LineItems
		{
			get
			{
				// 08/31/2009 Paul.  When called from within the SearchView control, dtLineItems is not initialized. 
				if ( dtLineItems == null )
					dtLineItems = ViewState["LineItems"] as DataTable;
				return dtLineItems;
			}
			set
			{
				dtLineItems = value;
				
				// 08/31/2009 Paul.  Add a blank row after loading. 
				DataRow rowNew = dtLineItems.NewRow();
				dtLineItems.Rows.Add(rowNew);
				
				ViewState["LineItems"] = dtLineItems;
				grdMain.DataSource = dtLineItems;
				// 02/03/2007 Paul.  Start with last line enabled for editing. 
				grdMain.EditIndex = dtLineItems.Rows.Count - 1;
				grdMain.DataBind();
			}
		}

		public bool ShowAddReplace
		{
			get { return bShowAddReplace; }
			set { bShowAddReplace = value; }
		}

		public bool Enabled
		{
			get { return bEnabled; }
			set { bEnabled = value; }
		}

		public string KBTAG_SET_LIST
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				// 08/31/2009 Paul.  When called from within the SearchView control, dtLineItems is not initialized. 
				if ( dtLineItems == null )
					dtLineItems = ViewState["LineItems"] as DataTable;
				if ( dtLineItems != null )
				{
					DataRow[] aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
					foreach ( DataRow row in aCurrentRows )
					{
						// 08/23/2009 Paul.  Although the KBTAG_ID should never be NULL or Empty, check for this condition anyway. 
						Guid gKBTAG_ID = Sql.ToGuid(row["KBTAG_ID"]);
						if ( gKBTAG_ID != Guid.Empty )
						{
							if ( sb.Length > 0 )
								sb.Append(",");
							sb.Append(gKBTAG_ID.ToString());
						}
					}
				}
				return sb.ToString();
			}
			// 04/14/2013 Paul.  Allow sets to be modified. 
			set
			{
				if ( dtLineItems == null )
				{
					dtLineItems = ViewState["LineItems"] as DataTable;
					if ( dtLineItems == null )
						InitTable();
				}
				string[] arrKBTags = value.Split(',');
				List<Guid> lstNew = new List<Guid>();
				foreach ( string sKBTAG_ID in arrKBTags )
				{
					try
					{
						if ( !Sql.IsEmptyString(sKBTAG_ID.Trim()) )
						{
							Guid gKBTAG_ID = Sql.ToGuid(sKBTAG_ID.Trim());
							if ( !lstNew.Contains(gKBTAG_ID) )
								lstNew.Add(gKBTAG_ID);
						}
					}
					catch
					{
					}
				}
				List<Guid> lstExisting = new List<Guid>();
				DataRow[] aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				foreach ( DataRow row in aCurrentRows )
				{
					Guid gKBTAG_ID = Sql.ToGuid(row["KBTAG_ID"]);
					// 04/14/2013 Paul.  Delete existing records that do not exist in the new list. 
					if ( !lstNew.Contains(gKBTAG_ID) )
						row.Delete();
					else if ( !lstExisting.Contains(gKBTAG_ID) )
						lstExisting.Add(gKBTAG_ID);
				}
				foreach ( Guid gKBTAG_ID in lstNew )
				{
					// 04/14/2013 Paul.  Add new records not found in existing list. 
					if ( !lstExisting.Contains(gKBTAG_ID) )
					{
						string sKBTAG_NAME = Crm.Modules.ItemName(HttpContext.Current.Application, "KBTags", gKBTAG_ID);
						if ( !Sql.IsEmptyString(sKBTAG_NAME) )
						{
							DataRow rowNew = dtLineItems.NewRow();
							dtLineItems.Rows.Add(rowNew);
							rowNew["KBTAG_ID"  ] = gKBTAG_ID;
							rowNew["KBTAG_NAME"] = sKBTAG_NAME;
						}
					}
				}
				// 04/14/2013 Paul.  The blank row at the bottom is always deleted, so always add it back. 
				DataRow rowBlank = dtLineItems.NewRow();
				dtLineItems.Rows.Add(rowBlank);
				ViewState["LineItems"] = dtLineItems;
			}
		}

		public bool ADD_KBTAG_SET
		{
			get
			{
				return pnlAddReplace.Visible && radKBTagSetAdd.Checked;
			}
		}

		public void InitTable()
		{
			dtLineItems = new DataTable();
			DataColumn colKBTAG_ID      = new DataColumn("KBTAG_ID"     , Type.GetType("System.Guid"   ));
			DataColumn colKBTAG_NAME    = new DataColumn("KBTAG_NAME"   , Type.GetType("System.String" ));
			dtLineItems.Columns.Add(colKBTAG_ID     );
			dtLineItems.Columns.Add(colKBTAG_NAME   );
		}

		public void Clear()
		{
			InitTable();

			DataRow rowNew = dtLineItems.NewRow();
			dtLineItems.Rows.Add(rowNew);

			ViewState["LineItems"] = dtLineItems;
			grdMain.DataSource = dtLineItems;
			// 02/03/2007 Paul.  Start with last line enabled for editing. 
			grdMain.EditIndex = dtLineItems.Rows.Count - 1;
			grdMain.DataBind();
		}

		// 11/11/2010 Paul.  Provide a way to disable validation in a rule. 
		public void Validate()
		{
			Validate(true);
		}

		public void Validate(bool bEnabled)
		{
			valKBTagSelect.Enabled = bEnabled;
			valKBTagSelect.Validate();
		}

		#region Line Item Editing
		protected void grdMain_RowCreated(object sender, GridViewRowEventArgs e)
		{
			if ( (e.Row.RowState & DataControlRowState.Edit) == DataControlRowState.Edit )
			{
				// 07/28/2010 Paul.  Save AjaxAutoComplete and SupportsPopups for use in TeamSelect and KBSelect. 
				// We are having issues with the data binding event occurring before the page load. 
				if ( bAjaxAutoComplete || Sql.ToBoolean(Page.Items["AjaxAutoComplete"]) )
				{
					// <ajaxToolkit:AutoCompleteExtender ID="autoKBTAG_NAME" TargetControlID="KBTAG_NAME" ServiceMethod="KBTAGS_KBTAG_NAME_List" OnClientItemSelected="KBTAGS_KBTAG_NAME_ItemSelected" ServicePath="~/KBDocuments/KBTags/AutoComplete.asmx" MinimumPrefixLength="2" CompletionInterval="250" EnableCaching="true" CompletionSetCount="12" runat="server" />
					AjaxControlToolkit.AutoCompleteExtender auto = new AjaxControlToolkit.AutoCompleteExtender();
					e.Row.Cells[0].Controls.Add(auto);
					auto.ID                   = "autoKBTAG_NAME";
					auto.TargetControlID      = "KBTAG_NAME";
					auto.ServiceMethod        = "KBTAGS_KBTAG_NAME_List";
					auto.OnClientItemSelected = "KBTAGS_KBTAG_NAME_ItemSelected";
					auto.ServicePath          = "~/KBDocuments/KBTags/AutoComplete.asmx";
					auto.MinimumPrefixLength  = 2;
					auto.CompletionInterval   = 250;
					auto.EnableCaching        = true;
					// 12/09/2010 Paul.  Provide a way to customize the AutoComplete.CompletionSetCount. 
					auto.CompletionSetCount   = Crm.Config.CompletionSetCount();
				}
			}
		}

		protected void grdMain_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if ( e.Row.RowType == DataControlRowType.DataRow )
			{
			}
		}

		protected void grdMain_RowEditing(object sender, GridViewEditEventArgs e)
		{
			grdMain.EditIndex = e.NewEditIndex;
			if ( dtLineItems != null )
			{
				grdMain.DataSource = dtLineItems;
				grdMain.DataBind();
			}
		}

		protected void grdMain_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			if ( dtLineItems != null )
			{
				//dtLineItems.Rows.RemoveAt(e.RowIndex);
				//dtLineItems.Rows[e.RowIndex].Delete();
				// 08/07/2007 fhsakai.  There might already be deleted rows, so make sure to first obtain the current rows. 
				DataRow[] aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				aCurrentRows[e.RowIndex].Delete();
				
				aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				// 02/04/2007 Paul.  Always allow editing of the last empty row. Add blank row if necessary. 
				// 08/11/2007 Paul.  Allow an item to be manually added.  Require either a product ID or a name. 
				if ( aCurrentRows.Length == 0 || !Sql.IsEmptyString(aCurrentRows[aCurrentRows.Length-1]["KBTAG_NAME"]) || !Sql.IsEmptyGuid(aCurrentRows[aCurrentRows.Length-1]["KBTAG_ID"]) )
				{
					DataRow rowNew = dtLineItems.NewRow();
					dtLineItems.Rows.Add(rowNew);
					aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				}

				ViewState["LineItems"] = dtLineItems;
				grdMain.DataSource = dtLineItems;
				// 03/15/2007 Paul.  Make sure to use the last row of the current set, not the total rows of the table.  Some rows may be deleted. 
				grdMain.EditIndex = aCurrentRows.Length - 1;
				grdMain.DataBind();
			}
		}

		protected void grdMain_RowUpdating(object sender, GridViewUpdateEventArgs e)
		{
			if ( dtLineItems != null )
			{
				GridViewRow gr = grdMain.Rows[e.RowIndex];
				HiddenField        txtKBTAG_ID     = gr.FindControl("KBTAG_ID"             ) as HiddenField;
				TextBox            txtKBTAG_NAME   = gr.FindControl("KBTAG_NAME"           ) as TextBox    ;
				HtmlGenericControl spnAjaxErrors   = gr.FindControl("KBTAG_NAME_AjaxErrors") as HtmlGenericControl;
				Guid gKBTAG_ID = Guid.Empty;
				if ( txtKBTAG_ID != null )
					gKBTAG_ID = Sql.ToGuid(txtKBTAG_ID.Value);

				if ( gKBTAG_ID != Guid.Empty )
				{
					//DataRow row = dtLineItems.Rows[e.RowIndex];
					// 12/07/2007 garf.  If there are deleted rows in the set, then the index will be wrong.  Make sure to use the current rowset. 
					DataRow[] aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
					DataRow row = aCurrentRows[e.RowIndex];
					if ( txtKBTAG_ID      != null ) row["KBTAG_ID"     ] = gKBTAG_ID;
					if ( txtKBTAG_NAME    != null ) row["KBTAG_NAME"   ] = txtKBTAG_NAME.Text;
					
					// 12/07/2007 Paul.  aCurrentRows is defined above. 
					//DataRow[] aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
					// 03/30/2007 Paul.  Always allow editing of the last empty row. Add blank row if necessary. 
					// 08/11/2007 Paul.  Allow an item to be manually added.  Require either a product ID or a name. 
					if ( aCurrentRows.Length == 0 || !Sql.IsEmptyString(aCurrentRows[aCurrentRows.Length-1]["KBTAG_NAME"]) || !Sql.IsEmptyGuid(aCurrentRows[aCurrentRows.Length-1]["KBTAG_ID"]) )
					{
						DataRow rowNew = dtLineItems.NewRow();
						dtLineItems.Rows.Add(rowNew);
						aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
					}

					ViewState["LineItems"] = dtLineItems;
					grdMain.DataSource = dtLineItems;
					// 03/30/2007 Paul.  Make sure to use the last row of the current set, not the total rows of the table.  Some rows may be deleted. 
					grdMain.EditIndex = aCurrentRows.Length - 1;
					grdMain.DataBind();
				}
				// 02/07/2010 Paul.  Defensive programming, check for valid spnAjaxErrors control. 
				else if ( spnAjaxErrors != null )
				{
					spnAjaxErrors.InnerHtml = "<br />" + L10n.Term("KBTags.ERR_INVALID_KBTAG");
				}
			}
		}

		protected void grdMain_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
		{
			grdMain.EditIndex = -1;
			if ( dtLineItems != null )
			{
				DataRow[] aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				grdMain.DataSource = dtLineItems;
				// 03/15/2007 Paul.  Make sure to use the last row of the current set, not the total rows of the table.  Some rows may be deleted. 
				grdMain.EditIndex = aCurrentRows.Length - 1;
				grdMain.DataBind();
			}
		}
		#endregion

		public void LoadLineItems(Guid gKBDOCUMENT_ID)
		{
			// 05/06/2010 Paul.  Use a special Page flag to override the default IsPostBack behavior. 
			bool bIsPostBack = this.IsPostBack && !NotPostBack;
			if ( !bIsPostBack )
			{
				// 08/29/2009 Paul.  Not sure why, but we need to manually bind the Add/Replace controls. 
				pnlAddReplace     .DataBind();
				radKBTagSetReplace.DataBind();
				radKBTagSetAdd    .DataBind();
				foreach ( DataControlField col in grdMain.Columns )
				{
					if ( !Sql.IsEmptyString(col.HeaderText) )
					{
						col.HeaderText = L10n.Term(col.HeaderText);
					}
					CommandField cf = col as CommandField;
					if ( cf != null )
					{
						// 01/18/2010 Paul.  These fields must be set in code as they are not bindable. 
						cf.ShowEditButton   = bEnabled;
						cf.ShowDeleteButton = bEnabled;
						// 08/31/2009 Paul.  Now that we are using our own ImageButtons in a TemplateField, 
						// we no longer need this CommandField logic. 
						/*
						if ( cf.Visible )
						{
							cf.EditText       = L10n.Term(cf.EditText  );
							cf.DeleteText     = L10n.Term(cf.DeleteText);
							cf.UpdateText     = L10n.Term(cf.UpdateText);
							cf.CancelText     = L10n.Term(cf.CancelText);
							cf.EditImageUrl   = Session["themeURL"] + "images/edit_inline.gif"   ;
							cf.DeleteImageUrl = Session["themeURL"] + "images/delete_inline.gif" ;
							cf.UpdateImageUrl = Session["themeURL"] + "images/accept_inline.gif" ;
							cf.CancelImageUrl = Session["themeURL"] + "images/decline_inline.gif";
						}
						*/
					}
				}
				if ( (!Sql.IsEmptyGuid(gKBDOCUMENT_ID)) )
				{
					// 08/24/2009 Paul.  We need to create another connection, even though there is usually an existing open connection. 
					// This is because we cannot perform another query while the rdr in the existing connection is still open. 
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						string sSQL;
						sSQL = "select *                             " + ControlChars.CrLf
						     + "  from vwKBDOCUMENTS_KBTAGS          " + ControlChars.CrLf
						     + " where KBDOCUMENT_ID = @KBDOCUMENT_ID" + ControlChars.CrLf
						     + " order by FULL_TAG_NAME asc          " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@KBDOCUMENT_ID", gKBDOCUMENT_ID);
							
							if ( bDebug )
								RegisterClientScriptBlock("vwKBDOCUMENTS_KBTAGS", Sql.ClientScriptBlock(cmd));
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dtLineItems = new DataTable();
								da.Fill(dtLineItems);
								
								// 03/27/2007 Paul.  Always add blank line to allow quick editing. 
								DataRow rowNew = dtLineItems.NewRow();
								dtLineItems.Rows.Add(rowNew);
								
								ViewState["LineItems"] = dtLineItems;
								grdMain.DataSource = dtLineItems;
								// 03/27/2007 Paul.  Start with last line enabled for editing. 
								grdMain.EditIndex = dtLineItems.Rows.Count - 1;
								grdMain.DataBind();
							}
						}
					}
				}
				else
				{
					InitTable();

					DataRow rowNew = dtLineItems.NewRow();
					dtLineItems.Rows.Add(rowNew);

					ViewState["LineItems"] = dtLineItems;
					grdMain.DataSource = dtLineItems;
					// 02/03/2007 Paul.  Start with last line enabled for editing. 
					grdMain.EditIndex = dtLineItems.Rows.Count - 1;
					grdMain.DataBind();
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			valKBTagSelect.ErrorMessage = L10n.Term(".ERR_REQUIRED_FIELD");
			// 05/06/2010 Paul.  Use a special Page flag to override the default IsPostBack behavior. 
			bool bIsPostBack = this.IsPostBack && !NotPostBack;
			if ( bIsPostBack )
			{
				dtLineItems = ViewState["LineItems"] as DataTable;
				grdMain.DataSource = dtLineItems;
				// 03/31/2007 Paul.  Don't bind the grid, otherwise edits will be lost. 
				//grdMain.DataBind();
			}
			
			// 05/06/2010 Paul.  Move the ajax refence code to Page_Load as it only needs to be called once. 
			ScriptManager mgrAjax = ScriptManager.GetCurrent(this.Page);
			// 11/23/2009 Paul.  SplendidCRM 4.0 is very slow on Blackberry devices.  Lets try and turn off AJAX AutoComplete. 
			bAjaxAutoComplete = (mgrAjax != null);
			if ( this.IsMobile )
			{
				// 11/24/2010 Paul.  .NET 4 has broken the compatibility of the browser file system. 
				// We are going to minimize our reliance on browser files in order to reduce deployment issues. 
				bAjaxAutoComplete = Utils.AllowAutoComplete && (mgrAjax != null);
			}
			if ( bAjaxAutoComplete && mgrAjax != null )
			{
				ServiceReference svc = new ServiceReference("~/KBDocuments/KBTags/AutoComplete.asmx");
				ScriptReference  scr = new ScriptReference ("~/KBDocuments/KBTags/AutoComplete.js"  );
				if ( !mgrAjax.Services.Contains(svc) )
					mgrAjax.Services.Add(svc);
				if ( !mgrAjax.Scripts.Contains(scr) )
					mgrAjax.Scripts.Add(scr);
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
