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
using System.Xml;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

namespace SplendidCRM.Surveys
{
	/// <summary>
	/// Summary description for Export.
	/// </summary>
	public class Export : SplendidPage
	{
		private static void ExportXml(XmlTextWriter xw, DataTable dt, string sModuleName, StringCollection arrExclude)
		{
			for ( int i = 0; i < dt.Rows.Count; i++ )
			{
				xw.WriteStartElement(sModuleName.ToLower());
				DataRow row = dt.Rows[i];
				for ( int nColumn = 0; nColumn < dt.Columns.Count; nColumn++ )
				{
					DataColumn col = dt.Columns[nColumn];
					if ( arrExclude.Contains(col.ColumnName.ToUpper()) )
						continue;
					xw.WriteStartElement(col.ColumnName.ToLower());
					if ( row[nColumn] != DBNull.Value )
					{
						switch ( col.DataType.FullName )
						{
							case "System.Boolean" :  xw.WriteString(Sql.ToBoolean (row[nColumn]) ? "1" : "0");  break;
							case "System.Single"  :  xw.WriteString(Sql.ToDouble  (row[nColumn]).ToString() );  break;
							case "System.Double"  :  xw.WriteString(Sql.ToDouble  (row[nColumn]).ToString() );  break;
							case "System.Int16"   :  xw.WriteString(Sql.ToInteger (row[nColumn]).ToString() );  break;
							case "System.Int32"   :  xw.WriteString(Sql.ToInteger (row[nColumn]).ToString() );  break;
							case "System.Int64"   :  xw.WriteString(Sql.ToLong    (row[nColumn]).ToString() );  break;
							case "System.Decimal" :  xw.WriteString(Sql.ToDecimal (row[nColumn]).ToString() );  break;
							case "System.DateTime":  xw.WriteString(Sql.ToDateTime(row[nColumn]).ToUniversalTime().ToString(CalendarControl.SqlDateTimeFormat));  break;
							case "System.Guid"    :  xw.WriteString(Sql.ToGuid    (row[nColumn]).ToString().ToUpper());  break;
							case "System.String"  :  xw.WriteString(Sql.ToString  (row[nColumn]));  break;
							case "System.Byte[]"  :
							{
								byte[] buffer = Sql.ToByteArray((System.Array) row[nColumn]);
								xw.WriteBase64(buffer, 0, buffer.Length);
								break;
							}
							//default:
							//	throw(new Exception("Unsupported field type: " + rdr.GetFieldType(nColumn).FullName));
						}
					}
					xw.WriteEndElement();
				}
				xw.WriteEndElement();
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			this.Visible = (SplendidCRM.Security.GetUserAccess("Surveys", "export") >= 0);
			if ( !this.Visible )
				return;
			
			try
			{
				Guid gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL ;
							Guid gSURVEY_THEME_ID = Guid.Empty;
							using ( DataTable dtSurvey = new DataTable() )
							{
								sSQL = "select *        " + ControlChars.CrLf
								     + "  from vwSURVEYS" + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Security.Filter(cmd, "Surveys", "view");
									Sql.AppendParameter(cmd, gID, "ID", false);
									using ( DbDataAdapter da = dbf.CreateDataAdapter() )
									{
										((IDbDataAdapter)da).SelectCommand = cmd;
										da.Fill(dtSurvey);
										if ( dtSurvey.Rows.Count > 0 )
										{
											DataRow rdr = dtSurvey.Rows[0];
											string sFileName = Sql.ToString(rdr["NAME"           ]);
											gSURVEY_THEME_ID = Sql.ToGuid  (rdr["SURVEY_THEME_ID"]);
											Response.ContentType = "text/xml";
											Response.AddHeader("Content-Disposition", "attachment;filename=" + Utils.ContentDispositionEncode(Request.Browser, sFileName + ".xml"));
										}
										else
										{
											Response.ContentType = "text/plain";
											Response.Write("Survey not found.");
											return;
										}
									}
								}
								XmlTextWriter xw = new XmlTextWriter(Response.OutputStream, Encoding.UTF8);
								xw.Formatting  = Formatting.Indented;
								xw.IndentChar  = ControlChars.Tab;
								xw.Indentation = 1;
								xw.WriteStartDocument();
								xw.WriteStartElement("splendidcrm");
								
								StringCollection arrExclude = new StringCollection();
								arrExclude.Add("ASSIGNED_USER_ID"      );
								arrExclude.Add("DATE_ENTERED"          );
								arrExclude.Add("DATE_MODIFIED"         );
								arrExclude.Add("DATE_MODIFIED_UTC"     );
								arrExclude.Add("TEAM_ID"               );
								arrExclude.Add("TEAM_NAME"             );
								arrExclude.Add("ASSIGNED_TO"           );
								arrExclude.Add("CREATED_BY"            );
								arrExclude.Add("MODIFIED_BY"           );
								arrExclude.Add("CREATED_BY_ID"         );
								arrExclude.Add("MODIFIED_USER_ID"      );
								arrExclude.Add("TEAM_SET_ID"           );
								arrExclude.Add("TEAM_SET_NAME"         );
								arrExclude.Add("TEAM_SET_LIST"         );
								arrExclude.Add("ASSIGNED_TO_NAME"      );
								arrExclude.Add("CREATED_BY_NAME"       );
								arrExclude.Add("MODIFIED_BY_NAME"      );
								arrExclude.Add("MEMBERSHIP_USER_ID"    );
								arrExclude.Add("MEMBERSHIP_TEAM_SET_ID");
								arrExclude.Add("ID_C"                  );
								
								ExportXml(xw, dtSurvey, "Surveys", arrExclude);
								
								// 04/11/2019 Paul.  Also export the background image. 
								string sPAGE_BACKGROUND_IMAGE    = String.Empty;
								Guid   gPAGE_BACKGROUND_IMAGE_ID = Guid.Empty;
								if ( !Sql.IsEmptyGuid(gSURVEY_THEME_ID) )
								{
									sSQL = "select *              " + ControlChars.CrLf
									     + "  from vwSURVEY_THEMES" + ControlChars.CrLf
									     + " where ID = @ID       " + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandText = sSQL;
										Sql.AddParameter(cmd, "@ID", gSURVEY_THEME_ID);
										using ( DbDataAdapter da = dbf.CreateDataAdapter() )
										{
											((IDbDataAdapter)da).SelectCommand = cmd;
											using ( DataTable dt = new DataTable() )
											{
												da.Fill(dt);
												if ( dt.Rows.Count > 0 )
												{
													sPAGE_BACKGROUND_IMAGE = Sql.ToString(dt.Rows[0]["PAGE_BACKGROUND_IMAGE"]);
													if ( sPAGE_BACKGROUND_IMAGE.Contains("Images/EmailImage.aspx?ID=") )
													{
														sPAGE_BACKGROUND_IMAGE = sPAGE_BACKGROUND_IMAGE.Split('=')[1];
														sPAGE_BACKGROUND_IMAGE = sPAGE_BACKGROUND_IMAGE.Substring(0, 36);
														gPAGE_BACKGROUND_IMAGE_ID = Sql.ToGuid(sPAGE_BACKGROUND_IMAGE);
													}
												}
												ExportXml(xw, dt, "SurveyThemes", arrExclude);
											}
										}
									}
								}
								
								sSQL = "select *                 " + ControlChars.CrLf
								     + "  from vwSURVEY_QUESTIONS" + ControlChars.CrLf
								     + " where ID in (select SURVEY_QUESTION_ID from vwSURVEY_PAGES_QUESTIONS where SURVEY_ID = @SURVEY_ID)" + ControlChars.CrLf
								     + " order by DESCRIPTION, NAME" + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@SURVEY_ID", gID);
									using ( DbDataAdapter da = dbf.CreateDataAdapter() )
									{
										((IDbDataAdapter)da).SelectCommand = cmd;
										using ( DataTable dt = new DataTable() )
										{
											da.Fill(dt);
											ExportXml(xw, dt, "SurveyQuestions", arrExclude);
										}
									}
								}
								
								sSQL = "select *                     " + ControlChars.CrLf
								     + "  from vwSURVEY_PAGES        " + ControlChars.CrLf
								     + " where SURVEY_ID = @SURVEY_ID" + ControlChars.CrLf
								     + " order by PAGE_NUMBER        " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@SURVEY_ID", gID);
									using ( DbDataAdapter da = dbf.CreateDataAdapter() )
									{
										((IDbDataAdapter)da).SelectCommand = cmd;
										using ( DataTable dt = new DataTable() )
										{
											da.Fill(dt);
											ExportXml(xw, dt, "SurveyPages", arrExclude);
										}
									}
								}
								
								sSQL = "select SURVEY_PAGE_ID                 " + ControlChars.CrLf
								     + "     , SURVEY_QUESTION_ID             " + ControlChars.CrLf
								     + "     , PAGE_NUMBER                    " + ControlChars.CrLf
								     + "     , QUESTION_NUMBER                " + ControlChars.CrLf
								     + "  from vwSURVEY_PAGES_QUESTIONS       " + ControlChars.CrLf
								     + " where SURVEY_ID = @SURVEY_ID         " + ControlChars.CrLf
								     + " order by PAGE_NUMBER, QUESTION_NUMBER" + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@SURVEY_ID", gID);
									using ( DbDataAdapter da = dbf.CreateDataAdapter() )
									{
										((IDbDataAdapter)da).SelectCommand = cmd;
										using ( DataTable dt = new DataTable() )
										{
											da.Fill(dt);
											ExportXml(xw, dt, "SurveyPagesQuestions", arrExclude);
										}
									}
								}
								// 11/12/2018 Paul.  Export images that are not URLs. 
								List<Guid> arrEMAIL_IMAGES = new List<Guid>();
								sSQL = "select IMAGE_URL                                     " + ControlChars.CrLf
								     + "  from vwSURVEY_QUESTIONS                            " + ControlChars.CrLf
								     + " where ID in (select SURVEY_QUESTION_ID from vwSURVEY_PAGES_QUESTIONS where SURVEY_ID = @SURVEY_ID)" + ControlChars.CrLf
								     + "   and QUESTION_TYPE = 'Image'                       " + ControlChars.CrLf
								     + "   and IMAGE_URL like '~/Images/EmailImage.aspx?ID=%'" + ControlChars.CrLf
								     + " order by DESCRIPTION, NAME                          " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@SURVEY_ID", gID);
									using ( DbDataAdapter da = dbf.CreateDataAdapter() )
									{
										((IDbDataAdapter)da).SelectCommand = cmd;
										using ( DataTable dt = new DataTable() )
										{
											da.Fill(dt);
											foreach ( DataRow row in dt.Rows )
											{
												string sIMAGE_URL = Sql.ToString(row["IMAGE_URL"]);
												sIMAGE_URL = sIMAGE_URL.Replace("~/Images/EmailImage.aspx?ID=", String.Empty);
												try
												{
													arrEMAIL_IMAGES.Add(Sql.ToGuid(sIMAGE_URL));
												}
												catch(Exception ex)
												{
													SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
												}
											}
											//ExportXml(xw, dt, "Images", arrExclude);
										}
									}
								}
								// 04/11/2019 Paul.  Also export the background image. 
								if ( !Sql.IsEmptyGuid(gPAGE_BACKGROUND_IMAGE_ID) )
								{
									arrEMAIL_IMAGES.Add(gPAGE_BACKGROUND_IMAGE_ID);
								}
								if ( arrEMAIL_IMAGES.Count > 0 )
								{
									sSQL = "select *             " + ControlChars.CrLf
									     + "     , (select CONTENT from vwEMAIL_IMAGES_CONTENT where vwEMAIL_IMAGES_CONTENT.ID = vwEMAIL_IMAGES.ID) as CONTENT" + ControlChars.CrLf
									     + "  from vwEMAIL_IMAGES" + ControlChars.CrLf
									     + " where 1 = 1         " + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandText = sSQL;
										Sql.AppendGuids(cmd, arrEMAIL_IMAGES.ToArray(), "ID");
										using ( DbDataAdapter da = dbf.CreateDataAdapter() )
										{
											((IDbDataAdapter)da).SelectCommand = cmd;
											using ( DataTable dt = new DataTable() )
											{
												da.Fill(dt);
												ExportXml(xw, dt, "Images", arrExclude);
											}
										}
									}
								}
								
								xw.WriteEndElement();
								xw.WriteEndDocument();
								xw.Flush();
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				if ( ex.Message == "Thread was being aborted." )
					return;
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				Response.Write(ex.Message);
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
