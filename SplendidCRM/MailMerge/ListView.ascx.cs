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
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Diagnostics;

namespace SplendidCRM.MailMerge
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected Label           lblError                 ;

		protected DropDownList    lstPRIMARY_MODULE        ;
		protected HiddenField     hidSECONDARY_MODULE      ;
		protected Label           lblSECONDARY_MODULE      ;
		protected DropDownList    lstDOCUMENT_TEMPLATE     ;
		protected DataTable       dtMain                   ;
		protected DataView        vwMain                   ;
		protected SplendidGrid    grdMain                  ;
		protected HiddenField     txtADD_RECORDS           ;
		protected HiddenField     txtPRIMARY_ID            ;
		protected HiddenField     txtSECONDARY_ID          ;
		protected string          sSINGULAR_NAME           ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Primary.Delete" )
				{
					DataRow[] rows = dtMain.Select("ID = '" + e.CommandArgument + "'");
					if ( rows.Length > 0 )
					{
						rows[0].Delete();
						
						ViewState["dtMain"] = dtMain;
						vwMain = new DataView(dtMain);
						vwMain.Sort = "NAME asc";
						grdMain.DataSource = vwMain;
						grdMain.DataBind();
					}
				}
				else if ( e.CommandName == "Secondary.Delete" )
				{
					DataRow[] rows = dtMain.Select("ID = '" + e.CommandArgument + "'");
					if ( rows.Length > 0 )
					{
						rows[0]["SECONDARY_ID"  ] = DBNull.Value;
						rows[0]["SECONDARY_NAME"] = DBNull.Value;
						
						ViewState["dtMain"] = dtMain;
						vwMain = new DataView(dtMain);
						vwMain.Sort = "NAME asc";
						grdMain.DataSource = vwMain;
						grdMain.DataBind();
					}
				}
				else if ( e.CommandName == "Generate" )
				{
					Guid gDOCUMENT_ID = Sql.ToGuid(lstDOCUMENT_TEMPLATE.SelectedValue);
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL ;
						Guid   gDOCUMENT_REVISION_ID = Guid.Empty;
						string sFILE_MIME_TYPE       = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
						string sFILENAME             = String.Empty;
						sSQL = "select DOCUMENT_REVISION_ID" + ControlChars.CrLf
						     + "     , FILE_MIME_TYPE      " + ControlChars.CrLf
						     + "     , FILENAME            " + ControlChars.CrLf
						     + "  from vwDOCUMENTS         " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Security.Filter(cmd, "Documents", "view");
							Sql.AppendParameter(cmd, gDOCUMENT_ID, "ID", false);
							cmd.CommandText += "   and IS_TEMPLATE = 1" + ControlChars.CrLf;
							cmd.CommandText += "   and (FILENAME like '%.docx' or FILE_MIME_TYPE = 'application/vnd.openxmlformats-officedocument.wordprocessingml.document')" + ControlChars.CrLf;
							using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
							{
								if ( rdr.Read() )
								{
									gDOCUMENT_REVISION_ID = Sql.ToGuid  (rdr["DOCUMENT_REVISION_ID"]);
									sFILENAME             = Sql.ToString(rdr["FILENAME"            ]);
								}
							}
						}
						if ( !Sql.IsEmptyGuid(gDOCUMENT_REVISION_ID) )
						{
							byte[] byDocTemplate = null;
							using ( MemoryStream stm = new MemoryStream() )
							{
								using ( BinaryWriter writer = new BinaryWriter(stm) )
								{
									Documents.MailMerge.WriteStream(gDOCUMENT_REVISION_ID, con, writer);
									// 05/12/2011 Paul.  ToArray is easier than GetBuffer as it will return the correct size. 
									stm.Seek(0, SeekOrigin.Begin);
									byDocTemplate = stm.ToArray();
								}
							}
							string sPRIMARY_MODULE = lstPRIMARY_MODULE.SelectedValue;
							List<byte[]> lstParts = new List<byte[]>();
							foreach ( DataRow row in dtMain.Rows )
							{
								Guid   gID           = Sql.ToGuid  (row["ID"          ]);
								string sMODULE_NAME  = Sql.ToString(row["MODULE_NAME" ]);
								Guid   gSECONDARY_ID = Sql.ToGuid  (row["SECONDARY_ID"]);
								// 05/17/2011 Paul.  For Campaigns and ProspectLists, the primary record could vary modules. 
								string sTABLE_NAME  = Crm.Modules.TableName(sMODULE_NAME);
								Dictionary<string, string> dictValues = new Dictionary<string, string>();
								sSQL = "select * " + ControlChars.CrLf
								     + "  from vw" + sTABLE_NAME + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Security.Filter(cmd, sMODULE_NAME, "view");
									Sql.AppendParameter(cmd, gID, "ID", false);
									using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
									{
										if ( rdr.Read() )
										{
											for ( int nFieldIndex = 0; nFieldIndex < rdr.FieldCount; nFieldIndex++ )
											{
												string sNAME  = rdr.GetName(nFieldIndex).ToLower();
												string sVALUE = Sql.ToString(rdr.GetValue(nFieldIndex));
												// 05/17/2011 Paul.  We will allow the use of templates for either Contacts, Leads or Prospects. 
												if ( sPRIMARY_MODULE == "Campaigns" || sPRIMARY_MODULE == "ProspectLists" )
												{
													dictValues.Add("Contacts_"  + sNAME, sVALUE);
													dictValues.Add("Leads_"     + sNAME, sVALUE);
													dictValues.Add("Prospects_" + sNAME, sVALUE);
												}
												else
												{
													dictValues.Add(sMODULE_NAME + "_" + sNAME, sVALUE);
												}
											}
										}
									}
								}
								string sSECONDARY_MODULE = hidSECONDARY_MODULE.Value;
								if ( !Sql.IsEmptyString(sSECONDARY_MODULE) && !Sql.IsEmptyGuid(gSECONDARY_ID) )
								{
									string sSECONDARY_TABLE = Crm.Modules.TableName(sSECONDARY_MODULE);
									sSQL = "select * " + ControlChars.CrLf
									     + "  from vw" + sSECONDARY_TABLE + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandText = sSQL;
										Security.Filter(cmd, sSECONDARY_MODULE, "view");
										Sql.AppendParameter(cmd, gSECONDARY_ID, "ID", false);
										using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
										{
											if ( rdr.Read() )
											{
												for ( int nFieldIndex = 0; nFieldIndex < rdr.FieldCount; nFieldIndex++ )
												{
													string sNAME  = sSECONDARY_MODULE + "_" + rdr.GetName(nFieldIndex).ToLower();
													string sVALUE = Sql.ToString(rdr.GetValue(nFieldIndex));
													dictValues.Add(sNAME, sVALUE);
												}
											}
										}
									}
								}
								byte[] byMergedDoc = TRIS.FormFill.Lib.FormFiller.GetWordReport(byDocTemplate, null, dictValues);
								lstParts.Add(byMergedDoc);
							}
							if ( lstParts.Count == 1 )
							{
								Response.Clear();
								Response.ContentType = sFILE_MIME_TYPE;
								Response.AddHeader("Content-Disposition", "attachment;filename=" + Utils.ContentDispositionEncode(Request.Browser, sFILENAME));
								Response.BinaryWrite(lstParts[0]);
								//Response.BinaryWrite(byDocTemplate);
								Response.End();
							}
							else
							{
								List<OpenXml.PowerTools.Source> sources = new List<OpenXml.PowerTools.Source>();
								foreach ( byte[] byMergedDoc in lstParts )
								{
									MemoryStream stream = new MemoryStream();
									stream.Write(byMergedDoc, 0, byMergedDoc.Length);
									WordprocessingDocument docx = WordprocessingDocument.Open(stream, true);
									sources.Add(new OpenXml.PowerTools.Source(docx, true));
								}
								// 05/12/2011 Paul.  Using DocumentBuilder has the advantage of adding section breaks between the merged documents. 
								using ( MemoryStream stm = new MemoryStream() )
								{
									using ( WordprocessingDocument docx = OpenXml.PowerTools.DocumentBuilder.BuildOpenDocument(sources, stm) )
									{
										docx.Close();
									}
									
									Response.Clear();
									Response.ContentType = sFILE_MIME_TYPE;
									Response.AddHeader("Content-Disposition", "attachment;filename=" + Utils.ContentDispositionEncode(Request.Browser, sFILENAME));
									// 05/12/2011 Paul.  We need to use stm.ToArray() as stm.GetBuffer() causes an error: "There was an error opening the file."
									stm.Seek(0, SeekOrigin.Begin);
									Response.BinaryWrite(stm.ToArray());
									Response.End();
								}
							}
						}
						else
						{
							throw(new Exception("Document Template not found."));
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		protected void lstDOCUMENT_TEMPLATE_SelectedIndexChanged(object sender, EventArgs e)
		{
			Guid gDOCUMENT_ID = Sql.ToGuid(lstDOCUMENT_TEMPLATE.SelectedValue);
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL ;
					Guid   gDOCUMENT_REVISION_ID = Guid.Empty;
					//string sFILE_MIME_TYPE       = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
					//string sFILENAME             = String.Empty;
					sSQL = "select DOCUMENT_REVISION_ID" + ControlChars.CrLf
					     + "     , FILE_MIME_TYPE      " + ControlChars.CrLf
					     + "     , FILENAME            " + ControlChars.CrLf
					     + "  from vwDOCUMENTS         " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Security.Filter(cmd, "Documents", "view");
						Sql.AppendParameter(cmd, gDOCUMENT_ID, "ID", false);
						cmd.CommandText += "   and IS_TEMPLATE = 1" + ControlChars.CrLf;
						cmd.CommandText += "   and (FILENAME like '%.docx' or FILE_MIME_TYPE = 'application/vnd.openxmlformats-officedocument.wordprocessingml.document')" + ControlChars.CrLf;
						using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
						{
							if ( rdr.Read() )
							{
								gDOCUMENT_REVISION_ID = Sql.ToGuid  (rdr["DOCUMENT_REVISION_ID"]);
								//sFILENAME             = Sql.ToString(rdr["FILENAME"            ]);
							}
						}
					}
					if ( !Sql.IsEmptyGuid(gDOCUMENT_REVISION_ID) )
					{
						byte[] byDocTemplate = null;
						using ( MemoryStream stm = new MemoryStream() )
						{
							using ( BinaryWriter writer = new BinaryWriter(stm) )
							{
								Documents.MailMerge.WriteStream(gDOCUMENT_REVISION_ID, con, writer);
								// 05/12/2011 Paul.  ToArray is easier than GetBuffer as it will return the correct size. 
								stm.Seek(0, SeekOrigin.Begin);
								byDocTemplate = stm.ToArray();
							}
						}
						using ( MemoryStream stm = new MemoryStream() )
						{
							stm.Write(byDocTemplate, 0, byDocTemplate.Length);
							using ( WordprocessingDocument docx = WordprocessingDocument.Open(stm, true) )
							{
								if ( docx.MainDocumentPart != null && docx.MainDocumentPart.DocumentSettingsPart != null && docx.MainDocumentPart.DocumentSettingsPart.Settings != null )
								{
									List<DocumentVariable> docVars = docx.MainDocumentPart.DocumentSettingsPart.Settings.Descendants<DocumentVariable>().ToList();
									hidSECONDARY_MODULE.Value = String.Empty;
									lblSECONDARY_MODULE.Text  = String.Empty;
									foreach ( DocumentVariable v in docVars )
									{
										if ( v.Name == "MasterModule" )
										{
											if ( lstPRIMARY_MODULE.SelectedValue != "Campaigns" && lstPRIMARY_MODULE.SelectedValue != "ProspectLists" )
											{
												if ( lstPRIMARY_MODULE.SelectedValue != v.Val )
												{
													dtMain.Rows.Clear();
													//dtMain.AcceptChanges();
													grdMain.DataBind();
												}
												Utils.SetSelectedValue(lstPRIMARY_MODULE, v.Val);
											}
										}
										else if ( v.Name == "SecondaryModule" )
										{
											hidSECONDARY_MODULE.Value = v.Val;
											lblSECONDARY_MODULE.Text  = L10n.Term(".moduleList." + v.Val);
										}
									}
									grdMain.Columns[3].Visible = !Sql.IsEmptyString(hidSECONDARY_MODULE.Value);
									grdMain.Columns[4].Visible = grdMain.Columns[3].Visible;
									if ( !grdMain.Columns[3].Visible )
									{
										foreach ( DataRow row in dtMain.Rows )
										{
											row["SECONDARY_ID"  ] = DBNull.Value;
											row["SECONDARY_NAME"] = DBNull.Value;
										}
									}
								}
							}
						}
					}
					else
					{
						throw(new Exception("Document Template not found."));
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		protected void lstPRIMARY_MODULE_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL = String.Empty;
					sSQL = "select ID                            " + ControlChars.CrLf
					     + "     , NAME                          " + ControlChars.CrLf
					     + "  from vwDOCUMENTS_MailMergeTemplates" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Security.Filter(cmd, "Documents", "list");
						
						string sPRIMARY_MODULE = lstPRIMARY_MODULE.SelectedValue;
						if ( !Sql.IsEmptyString(sPRIMARY_MODULE) )
						{
							if ( sPRIMARY_MODULE == "Campaigns" || sPRIMARY_MODULE == "ProspectLists" )
							{
								string[] arrCampaignModules = new string[] { "Contacts", "Leads", "Prospects" };
								Sql.AppendParameter(cmd, arrCampaignModules, "PRIMARY_MODULE");
							}
							else
							{
								Sql.AppendParameter(cmd, sPRIMARY_MODULE, "PRIMARY_MODULE");
							}
						}
						cmd.CommandText += " order by NAME" + ControlChars.CrLf;
						
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								lstDOCUMENT_TEMPLATE.DataSource = dt;
								lstDOCUMENT_TEMPLATE.DataBind();
								
								hidSECONDARY_MODULE.Value = String.Empty;
								lblSECONDARY_MODULE.Text  = String.Empty;
								if ( lstDOCUMENT_TEMPLATE.Items.Count > 0 )
									lstDOCUMENT_TEMPLATE_SelectedIndexChanged(null, null);
							}
						}
					}
				}
				if ( IsPostBack )
				{
					dtMain.Rows.Clear();
					ViewState["dtMain"] = dtMain;
					vwMain = new DataView(dtMain);
					vwMain.Sort = "NAME asc";
					grdMain.DataSource = vwMain;
					grdMain.DataBind();
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		protected void btnADD_RECORDS_Click(object sender, EventArgs e)
		{
			try
			{
				if ( !Sql.IsEmptyString(txtADD_RECORDS.Value) )
				{
					string[] arrID = txtADD_RECORDS.Value.Split(',');
					if ( arrID != null )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL = String.Empty;
							string sPRIMARY_MODULE = lstPRIMARY_MODULE.SelectedValue;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								if ( sPRIMARY_MODULE == "ProspectLists" )
								{
									sSQL = "select ID                        " + ControlChars.CrLf
									     + "     , NAME                      " + ControlChars.CrLf
									     + "     , MODULE_NAME               " + ControlChars.CrLf
									     + "  from vwPROSPECT_LISTS_MailMerge" + ControlChars.CrLf
									     + " where 1 = 1                     " + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									Sql.AppendParameter(cmd, arrID, "PROSPECT_LIST_ID");
								}
								else if ( sPRIMARY_MODULE == "Campaigns" )
								{
									sSQL = "select distinct                  " + ControlChars.CrLf
									     + "       ID                        " + ControlChars.CrLf
									     + "     , NAME                      " + ControlChars.CrLf
									     + "     , MODULE_NAME               " + ControlChars.CrLf
									     + "  from vwCAMPAIGNS_MailMerge     " + ControlChars.CrLf
									     + " where 1 = 1                     " + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									Sql.AppendParameter(cmd, arrID, "CAMPAIGN_ID");
								}
								else
								{
									sSQL = "select ID   " + ControlChars.CrLf
									     + "     , NAME " + ControlChars.CrLf
									     + "     , '" + sPRIMARY_MODULE + "' as MODULE_NAME" + ControlChars.CrLf
									     + "  from vw" + Crm.Modules.TableName(sPRIMARY_MODULE) + ControlChars.CrLf
									     + " where 1 = 1" + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									Sql.AppendParameter(cmd, arrID, "ID");
								}
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										foreach ( DataRow row in dt.Rows )
										{
											Guid gID = Sql.ToGuid(row["ID"]);
											vwMain.RowFilter = "ID = '" + gID.ToString() + "'";
											if ( vwMain.Count == 0 )
											{
												string sNAME        = Sql.ToString(row["NAME"       ]);
												string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
												DataRow rowNew = dtMain.NewRow();
												rowNew["ID"         ] = gID         ;
												rowNew["NAME"       ] = sNAME       ;
												rowNew["MODULE_NAME"] = sMODULE_NAME;
												dtMain.Rows.Add(rowNew);
											}
										}
										ViewState["dtMain"] = dtMain;
										vwMain.RowFilter = String.Empty;
										vwMain.Sort = "NAME asc";
										grdMain.DataBind();
									}
								}
							}
						}
					}
					txtADD_RECORDS.Value = String.Empty;
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		protected void btnCHANGE_SECONDARY_Click(object sender, EventArgs e)
		{
			try
			{
				if ( !Sql.IsEmptyString(txtPRIMARY_ID.Value) )
				{
					DataRow[] rows = dtMain.Select("ID = '" + txtPRIMARY_ID.Value + "'");
					if ( rows.Length > 0 )
					{
						if ( !Sql.IsEmptyString(txtSECONDARY_ID.Value) )
						{
							Guid gSECONDARY_ID = Sql.ToGuid(txtSECONDARY_ID.Value);
							rows[0]["SECONDARY_ID"  ] = gSECONDARY_ID;
							rows[0]["SECONDARY_NAME"] = Crm.Modules.ItemName(Application, hidSECONDARY_MODULE.Value, gSECONDARY_ID);
						}
						else
						{
							rows[0]["SECONDARY_ID"  ] = DBNull.Value;
							rows[0]["SECONDARY_NAME"] = DBNull.Value;
						}
						ViewState["dtMain"] = dtMain;
						vwMain = new DataView(dtMain);
						vwMain.Sort = "NAME asc";
						grdMain.DataSource = vwMain;
						grdMain.DataBind();
					}
					txtPRIMARY_ID  .Value = String.Empty;
					txtSECONDARY_ID.Value = String.Empty;
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(m_sMODULE + ".LBL_LIST_FORM_TITLE"));
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "list") >= 0);
			if ( !this.Visible )
				return;

			try
			{
				if ( !IsPostBack )
				{
					dtMain = new DataTable();
					dtMain.Columns.Add("ID"            , typeof(System.Guid  ));
					dtMain.Columns.Add("NAME"          , typeof(System.String));
					dtMain.Columns.Add("MODULE_NAME"   , typeof(System.String));
					dtMain.Columns.Add("SECONDARY_ID"  , typeof(System.Guid  ));
					dtMain.Columns.Add("SECONDARY_NAME", typeof(System.String));

					string[] arrModules = SplendidCache.AccessibleModules(HttpContext.Current, Security.USER_ID).ToArray();
					// 05/14/2011 Paul.  Accounts, Contacts, Leads, Prospects, Cases, Opportunities. 
					DataTable dtPrimaryModules = SplendidCache.DetailViewRelationships("Modules.MailMerge").Copy();
					foreach ( DataRow row in dtPrimaryModules.Rows )
					{
						row["TITLE"] = L10n.Term(".moduleList." + Sql.ToString(row["MODULE_NAME"]));
					}
					DataRow rowCampaigns     = dtPrimaryModules.NewRow();
					DataRow rowProspectLists = dtPrimaryModules.NewRow();
					rowCampaigns    ["MODULE_NAME"] = "Campaigns";
					rowCampaigns    ["TITLE"      ] = L10n.Term(".moduleList.Campaigns");
					rowProspectLists["MODULE_NAME"] = "ProspectLists";
					rowProspectLists["TITLE"      ] = L10n.Term(".moduleList.ProspectLists");
					dtPrimaryModules.Rows.Add(rowCampaigns    );
					dtPrimaryModules.Rows.Add(rowProspectLists);

					DataView vwPrimaryModules = new DataView(dtPrimaryModules);
					vwPrimaryModules.RowFilter = "MODULE_NAME in ('" + String.Join("','", arrModules) + "')";
					vwPrimaryModules.Sort      = "TITLE";
					lstPRIMARY_MODULE.DataSource = vwPrimaryModules;
					lstPRIMARY_MODULE.DataBind();
					
					string sPRIMARY_MODULE = Sql.ToString(Request.QueryString["Module"]);
					if ( !Sql.IsEmptyString(sPRIMARY_MODULE) )
					{
						Utils.SetSelectedValue(lstPRIMARY_MODULE, sPRIMARY_MODULE);
						lstPRIMARY_MODULE.Enabled = false;
					}
					lstPRIMARY_MODULE_SelectedIndexChanged(null, null);
					
					ViewState["dtMain"] = dtMain;
					vwMain = new DataView(dtMain);
					vwMain.Sort = "NAME asc";
					
					string[] arrID = Request.Form.GetValues("chkMain");
					if ( arrID != null && arrID.Length > 0 )
					{
						txtADD_RECORDS.Value = String.Join(",", arrID);
						btnADD_RECORDS_Click(null, null);
					}
					else
					{
						arrID = Request.QueryString.GetValues("chkMain");
						if ( arrID != null && arrID.Length > 0 )
						{
							txtADD_RECORDS.Value = String.Join(",", arrID);
							btnADD_RECORDS_Click(null, null);
						}
					}

					grdMain.DataSource = vwMain;
					grdMain.DataBind();
				}
				else
				{
					dtMain = ViewState["dtMain"] as DataTable;
					vwMain = new DataView(dtMain);
					vwMain.Sort = "NAME asc";
					grdMain.DataSource = vwMain;
				}
				
				sSINGULAR_NAME = Crm.Modules.TableName(lstPRIMARY_MODULE.SelectedValue);
				if ( sSINGULAR_NAME.EndsWith("IES") )
					sSINGULAR_NAME = sSINGULAR_NAME.Substring(0, sSINGULAR_NAME.Length - 3) + "Y";
				else if ( sSINGULAR_NAME.EndsWith("S") )
					sSINGULAR_NAME = sSINGULAR_NAME.Substring(0, sSINGULAR_NAME.Length - 1);
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
			m_sMODULE = "MailMerge";
			SetMenu(m_sMODULE);
		}
		#endregion
	}
}

