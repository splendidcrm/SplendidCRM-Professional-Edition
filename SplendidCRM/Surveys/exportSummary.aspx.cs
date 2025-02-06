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
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace SplendidCRM.Surveys
{
	/// <summary>
	/// Summary description for ExportSummary.
	/// </summary>
	public class ExportSummary : SplendidPage
	{
		protected string CleanseName(string sName)
		{
			sName = sName.Replace(ControlChars.CrLf, " ");
			sName = sName.Replace("<", "");
			sName = sName.Replace(">", "");
			sName = sName.Replace("/", "");
			sName = sName.Replace(":", "");
			sName = sName.Replace("\"", "");
			sName = sName.Replace("\'", "");
			sName = sName.Replace("  ", " ").Trim();
			return sName;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			this.Visible = (SplendidCRM.Security.GetUserAccess("SurveyResults", "export") >= 0);
			if ( !this.Visible )
				return;
			
			try
			{
				Guid   gSURVEY_ID          = Sql.ToGuid  (Request["SURVEY_ID"         ]);
				Guid   gSURVEY_PAGE_ID     = Sql.ToGuid  (Request["SURVEY_PAGE_ID"    ]);
				Guid   gSURVEY_QUESTION_ID = Sql.ToGuid  (Request["SURVEY_QUESTION_ID"]);
				string sExportFormat       = Sql.ToString(Request["ExportFormat"      ]);
				if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gSURVEY_ID) && !Sql.IsEmptyGuid(gSURVEY_PAGE_ID) && !Sql.IsEmptyGuid(gSURVEY_QUESTION_ID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL ;
							string sSURVEY_NAME   = String.Empty;
							sSQL = "select NAME     " + ControlChars.CrLf
							     + "  from vwSURVEYS" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Security.Filter(cmd, "Surveys", "export");
								Sql.AppendParameter(cmd, gSURVEY_ID, "ID", false);
								using ( IDataReader rdr = cmd.ExecuteReader() )
								{
									if ( rdr.Read() )
									{
										sSURVEY_NAME = Sql.ToString(rdr["NAME"]);
									}
									else
									{
										throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
									}
								}
							}
							DataTable dtQuestion = new DataTable();
							DataRow   rowQuestion = null;
							string sQUESTION_NAME  = String.Empty;
							string sQUESTION_TYPE  = String.Empty;
							sSQL = "select *                 " + ControlChars.CrLf
							     + "  from vwSURVEY_QUESTIONS" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Security.Filter(cmd, "SurveyQuestions", "export");
								Sql.AppendParameter(cmd, gSURVEY_QUESTION_ID, "ID", false);
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dtQuestion);
									if ( dtQuestion.Rows.Count > 0 )
									{
										rowQuestion = dtQuestion.Rows[0];
										sQUESTION_NAME  = Sql.ToString(rowQuestion["DESCRIPTION"   ]);
										sQUESTION_TYPE  = Sql.ToString(rowQuestion["QUESTION_TYPE" ]);
									}
									else
									{
										throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
									}
								}
							}
							string sSELECT = "*";
							switch ( sQUESTION_TYPE )
							{
								case "Checkbox"         :  sSELECT = "SURVEY_RESULT_ID, DATE_ENTERED, ANSWER_ID, ANSWER_TEXT, OTHER_TEXT";  break;
								case "Checkbox Matrix"  :  sSELECT = "SURVEY_RESULT_ID, DATE_ENTERED, ANSWER_ID, ANSWER_TEXT, COLUMN_ID, COLUMN_TEXT, OTHER_TEXT";  break;
								case "Date"             :  sSELECT = "SURVEY_RESULT_ID, DATE_ENTERED, ANSWER_ID, ANSWER_TEXT";  break;
								case "Demographic"      :  sSELECT = "SURVEY_RESULT_ID, DATE_ENTERED, ANSWER_ID, ANSWER_TEXT";  break;
								case "Dropdown"         :  sSELECT = "SURVEY_RESULT_ID, DATE_ENTERED, ANSWER_ID, ANSWER_TEXT, OTHER_TEXT";  break;
								case "Dropdown Matrix"  :  sSELECT = "SURVEY_RESULT_ID, DATE_ENTERED, ANSWER_ID, ANSWER_TEXT, COLUMN_ID, COLUMN_TEXT, MENU_ID, MENU_TEXT, OTHER_TEXT";  break;
								case "Radio"            :  sSELECT = "SURVEY_RESULT_ID, DATE_ENTERED, ANSWER_ID, ANSWER_TEXT, OTHER_TEXT";  break;
								case "Radio Matrix"     :  sSELECT = "SURVEY_RESULT_ID, DATE_ENTERED, ANSWER_ID, ANSWER_TEXT, COLUMN_ID, COLUMN_TEXT, OTHER_TEXT";  break;
								case "Range"            :  sSELECT = "SURVEY_RESULT_ID, DATE_ENTERED, ANSWER_ID, ANSWER_TEXT";  break;
								case "Ranking"          :  sSELECT = "SURVEY_RESULT_ID, DATE_ENTERED, ANSWER_ID, ANSWER_TEXT, WEIGHT";  break;
								case "Rating Scale"     :  sSELECT = "SURVEY_RESULT_ID, DATE_ENTERED, ANSWER_ID, ANSWER_TEXT, COLUMN_ID, COLUMN_TEXT, WEIGHT, OTHER_TEXT";  break;
								case "Text Area"        :  sSELECT = "SURVEY_RESULT_ID, DATE_ENTERED, ANSWER_TEXT";  break;
								case "Textbox"          :  sSELECT = "SURVEY_RESULT_ID, DATE_ENTERED, ANSWER_TEXT";  break;
								case "Textbox Multiple" :  sSELECT = "SURVEY_RESULT_ID, DATE_ENTERED, ANSWER_ID, ANSWER_TEXT";  break;
								case "Textbox Numerical":  sSELECT = "SURVEY_RESULT_ID, DATE_ENTERED, ANSWER_ID, ANSWER_TEXT";  break;
								default                 :  throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
							}
							using ( DataTable dtSurveyResults = new DataTable() )
							{
								sSQL = "select " + sSELECT                 + ControlChars.CrLf
								     + "  from vwSURVEY_QUESTIONS_RESULTS" + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Security.Filter(cmd, "SurveyResults", "export");
									Sql.AppendParameter(cmd, gSURVEY_ID         , "SURVEY_ID"         , false);
									Sql.AppendParameter(cmd, gSURVEY_PAGE_ID    , "SURVEY_PAGE_ID"    , false);
									Sql.AppendParameter(cmd, gSURVEY_QUESTION_ID, "SURVEY_QUESTION_ID", false);
									cmd.CommandText += " order by DATE_ENTERED desc" + ControlChars.CrLf;
									using ( DbDataAdapter da = dbf.CreateDataAdapter() )
									{
										((IDbDataAdapter)da).SelectCommand = cmd;
										da.Fill(dtSurveyResults);
										if ( dtSurveyResults.Rows.Count > 0 )
										{
											try
											{
												string       sTitle     = sSURVEY_NAME  ;
												string       sSubTitle1 = sQUESTION_NAME;
												string       sSubTitle2 = String.Empty  ;
												List<string> lstHeaders = new List<string>();
												DataTable    dtSummary  = new DataTable();
												switch ( sQUESTION_TYPE )
												{
													case "Checkbox"         :  SummaryResults_MultipleChoice      (rowQuestion, dtSurveyResults, lstHeaders, dtSummary, ref sSubTitle2);  break;
													case "Checkbox Matrix"  :  SummaryResults_MultipleChoiceMatrix(rowQuestion, dtSurveyResults, lstHeaders, dtSummary, ref sSubTitle2);  break;
													case "Date"             :  SummaryResults_TextMatrix          (rowQuestion, dtSurveyResults, lstHeaders, dtSummary, ref sSubTitle2);  break;
													case "Demographic"      :  SummaryResults_Demographic         (rowQuestion, dtSurveyResults, lstHeaders, dtSummary, ref sSubTitle2);  break;
													case "Dropdown"         :  SummaryResults_MultipleChoice      (rowQuestion, dtSurveyResults, lstHeaders, dtSummary, ref sSubTitle2);  break;
													case "Dropdown Matrix"  :  SummaryResults_DropdownMatrix      (rowQuestion, dtSurveyResults, lstHeaders, dtSummary, ref sSubTitle2);  break;
													case "Radio"            :  SummaryResults_MultipleChoice      (rowQuestion, dtSurveyResults, lstHeaders, dtSummary, ref sSubTitle2);  break;
													case "Radio Matrix"     :  SummaryResults_MultipleChoiceMatrix(rowQuestion, dtSurveyResults, lstHeaders, dtSummary, ref sSubTitle2);  break;
													case "Range"            :  SummaryResults_Range               (rowQuestion, dtSurveyResults, lstHeaders, dtSummary, ref sSubTitle2);  break;
													case "Ranking"          :  SummaryResults_Ranking             (rowQuestion, dtSurveyResults, lstHeaders, dtSummary, ref sSubTitle2);  break;
													case "Rating Scale"     :  SummaryResults_RatingScale         (rowQuestion, dtSurveyResults, lstHeaders, dtSummary, ref sSubTitle2);  break;
													case "Text Area"        :  SummaryResults_Text                (rowQuestion, dtSurveyResults, lstHeaders, dtSummary, ref sSubTitle2);  break;
													case "Textbox"          :  SummaryResults_Text                (rowQuestion, dtSurveyResults, lstHeaders, dtSummary, ref sSubTitle2);  break;
													case "Textbox Multiple" :  SummaryResults_TextMatrix          (rowQuestion, dtSurveyResults, lstHeaders, dtSummary, ref sSubTitle2);  break;
													case "Textbox Numerical":  SummaryResults_TextMatrix          (rowQuestion, dtSurveyResults, lstHeaders, dtSummary, ref sSubTitle2);  break;
												}
												
												DataView vw = new DataView(dtSurveyResults);
												int    nStartRecord  = 0;
												int    nEndRecord    = vw.Count;
												string sModuleName   = "SurveyResults";
												string sFilename     = (CleanseName(sSURVEY_NAME) + " - " + CleanseName(sQUESTION_NAME)).Trim();
												switch ( sExportFormat )
												{
													case "csv"  :
													{
														Response.ContentType = "text/csv";
														Response.AddHeader("Content-Disposition", "attachment;filename=" + Utils.ContentDispositionEncode(Request.Browser, sFilename + ".csv"));
														StreamWriter wt = new StreamWriter(Response.OutputStream);
														if ( !Sql.IsEmptyString(sTitle    ) ) wt.WriteLine(sTitle    );
														if ( !Sql.IsEmptyString(sSubTitle1) ) wt.WriteLine(sSubTitle1);
														if ( !Sql.IsEmptyString(sSubTitle2) ) wt.WriteLine(sSubTitle2);
														wt.Flush();
														SplendidExport.ExportDelimited(Response.OutputStream, vw, sModuleName, nStartRecord, nEndRecord, ',' );
														Response.End();
														break;
													}
													case "tab"  :
													{
														// 08/17/2024 Paul.  The correct MIME type is text/plain. 
														Response.ContentType = "text/plain";
														Response.AddHeader("Content-Disposition", "attachment;filename=" + Utils.ContentDispositionEncode(Request.Browser, sFilename + ".txt"));
														StreamWriter wt = new StreamWriter(Response.OutputStream);
														if ( !Sql.IsEmptyString(sTitle    ) ) wt.WriteLine(sTitle    );
														if ( !Sql.IsEmptyString(sSubTitle1) ) wt.WriteLine(sSubTitle1);
														if ( !Sql.IsEmptyString(sSubTitle2) ) wt.WriteLine(sSubTitle2);
														wt.Flush();
														SplendidExport.ExportDelimited(Response.OutputStream, vw, sModuleName, nStartRecord, nEndRecord, '\t');
														Response.End();
														break;
													}
													case "xml"  :
													{
														Response.ContentType = "text/xml";
														Response.AddHeader("Content-Disposition", "attachment;filename=" + Utils.ContentDispositionEncode(Request.Browser, sFilename + ".xml"));
														SplendidExport.ExportXml(Response.OutputStream, vw, sModuleName, nStartRecord, nEndRecord);
														Response.End();
														break;
													}
													//case "Excel":
													default     :
														Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";  //"application/vnd.ms-excel";
														Response.AddHeader("Content-Disposition", "attachment;filename=" + Utils.ContentDispositionEncode(Request.Browser, sFilename + ".xlsx"));
														SplendidExport.ExportExcelOpenXML(Response.OutputStream, dtSummary, lstHeaders.ToArray(), sTitle, sSubTitle1, sSubTitle2);
														Response.End();
														break;
												}
											}
											catch(Exception ex)
											{
												if ( !(ex is System.Threading.ThreadAbortException) )
												{
													Response.ClearContent();
													Response.ClearHeaders();
													Response.ContentType = "text/html";
													throw(ex);
												}
											}
										}
										else
										{
											Response.ContentType = "text/plain";
											Response.Write("No results found.");
											return;
										}
									}
								}
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

		public class SummaryResults
		{
			public string        ANSWER_TEXT   = String.Empty;
			public Guid          ANSWER_ID     = Guid.Empty;
			public List<DataRow> ANSWERED      = new List<DataRow>();
			public List<DataRow> SKIPPED       = new List<DataRow>();
			public List<DataRow> OTHER_SUMMARY = new List<DataRow>();
			public List<ColumnResults> COLUMNS = new List<ColumnResults>();
			public int           ANSWER_TOTAL  = 0;
			public Decimal       WEIGHT_TOTAL  = 0.0m;

			public class Comparer : IComparer<SummaryResults>
			{
				public int Compare(SummaryResults x, SummaryResults y)
				{
					if ( Sql.ToDecimal(x.ANSWER_TEXT) == Sql.ToDecimal(y.ANSWER_TEXT) )
						return 0;
					return Sql.ToDecimal(x.ANSWER_TEXT) > Sql.ToDecimal(y.ANSWER_TEXT) ? 1 : -1;
				}
			}
		}

		public class ColumnResults
		{
			public string               COLUMN_TEXT   = String.Empty;
			public string               COLUMN_LABEL  = String.Empty;
			public Guid                 COLUMN_ID     = Guid.Empty;
			public int                  WEIGHT        = 0;
			public string[]             OPTIONS       = new string[0];
			public List<MenuResults>    MENUS         = new List<MenuResults>();
			public List<DataRow>        ANSWERED      = new List<DataRow>();
			public List<DataRow>        SKIPPED       = new List<DataRow>();
		}

		public class MenuResults
		{
			public string        MENU_TEXT = String.Empty;
			public Guid          MENU_ID   = Guid.Empty;
			public List<DataRow> ANSWERED  = new List<DataRow>();
			public List<DataRow> SKIPPED   = new List<DataRow>();
			public int           ANSWER_TOTAL  = 0;
		}

		public class DropdownMenu
		{
			public string   Heading;
			public string[] OPTIONS;
		}

		private void SummaryResults_MultipleChoice(DataRow rowQuestion, DataTable dtSurveyResults, List<string> lstHeaders, DataTable dtSummary, ref string sAnsweredSkipped)
		{
			List<SummaryResults> ANSWER_CHOICES_SUMMARY = new List<SummaryResults>();
			List<DataRow>        OTHER_SUMMARY          = new List<DataRow>();
			Guid   gOTHER_ID        = SplendidCRM.Survey.ConvertAnswerID(SplendidCRM.Survey.md5("Other"));
			bool   bOTHER_ENABLED   = Sql.ToBoolean(rowQuestion["OTHER_ENABLED"  ]);
			bool   bOTHER_AS_CHOICE = Sql.ToBoolean(rowQuestion["OTHER_AS_CHOICE"]);
			string sANSWER_CHOICES  = Sql.ToString (rowQuestion["ANSWER_CHOICES" ]);
			if ( !Sql.IsEmptyString(sANSWER_CHOICES) )
			{
				string[] arrANSWER_CHOICES = sANSWER_CHOICES.Split(new string[] { ControlChars.CrLf }, StringSplitOptions.None);
				for ( int i = 0; i < arrANSWER_CHOICES.Length; i++ )
				{
					var oSUMMARY = new SummaryResults();
					oSUMMARY.ANSWER_TEXT   = arrANSWER_CHOICES[i];
					oSUMMARY.ANSWER_ID     = SplendidCRM.Survey.ConvertAnswerID(SplendidCRM.Survey.md5(arrANSWER_CHOICES[i]));
					oSUMMARY.ANSWERED      = new List<DataRow>();
					oSUMMARY.SKIPPED       = new List<DataRow>();
					oSUMMARY.OTHER_SUMMARY = new List<DataRow>();
					ANSWER_CHOICES_SUMMARY.Add(oSUMMARY);
				}
			}
			if ( bOTHER_ENABLED )
			{
				if ( bOTHER_AS_CHOICE )
				{
					var oSUMMARY = new SummaryResults();
					oSUMMARY.ANSWER_TEXT   = "Other";
					oSUMMARY.ANSWER_ID     = gOTHER_ID;
					oSUMMARY.ANSWERED      = new List<DataRow>();
					oSUMMARY.SKIPPED       = new List<DataRow>();
					oSUMMARY.OTHER_SUMMARY = new List<DataRow>();
					ANSWER_CHOICES_SUMMARY.Add(oSUMMARY);
				}
			}
			
			int nANSWERED = 0;
			int nSKIPPED  = 0;
			Dictionary<Guid, bool> oANSWERED = new Dictionary<Guid, bool>();
			Dictionary<Guid, bool> oSKIPPED  = new Dictionary<Guid, bool>();
			foreach ( DataRow row in dtSurveyResults.Rows )
			{
				Guid gSURVEY_RESULT_ID = Sql.ToGuid(row["SURVEY_RESULT_ID"]);
				Guid gANSWER_ID        = Sql.ToGuid(row["ANSWER_ID"       ]);
				if ( Sql.IsEmptyGuid(gANSWER_ID) )
				{
					// 08/15/2013 Paul.  Other can still be specified even if no answer is selected. 
					if ( bOTHER_ENABLED && !bOTHER_AS_CHOICE && row["OTHER_TEXT"] != DBNull.Value )
					{
						OTHER_SUMMARY.Add(row);
					}
					else if ( !oSKIPPED.ContainsKey(gSURVEY_RESULT_ID) )
					{
						oSKIPPED[gSURVEY_RESULT_ID] = true;
						nSKIPPED++;
					}
				}
				else
				{
					for ( int j = 0; j < ANSWER_CHOICES_SUMMARY.Count; j++ )
					{
						SummaryResults oANSWER_CHOICES_SUMMARY = ANSWER_CHOICES_SUMMARY[j];
						if ( oANSWER_CHOICES_SUMMARY.ANSWER_ID == gANSWER_ID )
						{
							if ( row["ANSWER_TEXT"] != DBNull.Value )
							{
								oANSWER_CHOICES_SUMMARY.ANSWERED.Add(row);
								if ( !oANSWERED.ContainsKey(gSURVEY_RESULT_ID) )
								{
									oANSWERED[gSURVEY_RESULT_ID] = true;
									nANSWERED++;
								}
							}
							else
							{
								oANSWER_CHOICES_SUMMARY.SKIPPED.Add(row);
								if ( !oSKIPPED.ContainsKey(gSURVEY_RESULT_ID) )
								{
									oSKIPPED[gSURVEY_RESULT_ID] = true;
									nSKIPPED++;
								}
							}
							if ( bOTHER_AS_CHOICE && gOTHER_ID == gANSWER_ID )
							{
								oANSWER_CHOICES_SUMMARY.OTHER_SUMMARY.Add(row);
							}
						}
						if ( bOTHER_ENABLED && !bOTHER_AS_CHOICE && gOTHER_ID == gANSWER_ID )
						{
							if ( row["ANSWER_TEXT"] != DBNull.Value )
							{
								OTHER_SUMMARY.Add(row);
							}
						}
					}
				}
			}
			
			dtSummary.Columns.Add("Answers"  , typeof(System.String));
			dtSummary.Columns.Add("Responses", typeof(System.Int32 ));
			foreach ( SummaryResults summary in ANSWER_CHOICES_SUMMARY )
			{
				DataRow rowSummary = dtSummary.NewRow();
				dtSummary.Rows.Add(rowSummary);
				rowSummary["Answers"  ] = summary.ANSWER_TEXT;
				rowSummary["Responses"] = summary.ANSWERED.Count;
			}
			
			lstHeaders.Add(L10n.Term("SurveyResults.LBL_ANSWER_CHOICES"));
			lstHeaders.Add(L10n.Term("SurveyResults.LBL_RESPONSES"     ));
			sAnsweredSkipped = String.Format(L10n.Term("SurveyResults.LBL_ANSWERED"), nANSWERED) + "  " + String.Format(L10n.Term("SurveyResults.LBL_SKIPPED"), nSKIPPED);
		}

		private void SummaryResults_MultipleChoiceMatrix(DataRow rowQuestion, DataTable dtSurveyResults, List<string> lstHeaders, DataTable dtSummary, ref string sAnsweredSkipped)
		{
			List<SummaryResults> ANSWER_CHOICES_SUMMARY = new List<SummaryResults>();
			List<DataRow>        OTHER_SUMMARY          = new List<DataRow>();
			Guid   gOTHER_ID        = SplendidCRM.Survey.ConvertAnswerID(SplendidCRM.Survey.md5("Other"));
			bool   bOTHER_ENABLED   = Sql.ToBoolean(rowQuestion["OTHER_ENABLED" ]);
			string sANSWER_CHOICES  = Sql.ToString (rowQuestion["ANSWER_CHOICES"]);
			string sCOLUMN_CHOICES  = Sql.ToString (rowQuestion["COLUMN_CHOICES"]);
			string[] arrANSWER_CHOICES = null;
			string[] arrCOLUMN_CHOICES = null;
			if ( !Sql.IsEmptyString(sANSWER_CHOICES) && !Sql.IsEmptyString(sCOLUMN_CHOICES) )
			{
				arrANSWER_CHOICES = sANSWER_CHOICES.Split(new string[] { ControlChars.CrLf }, StringSplitOptions.None);
				arrCOLUMN_CHOICES = sCOLUMN_CHOICES.Split(new string[] { ControlChars.CrLf }, StringSplitOptions.None);
				for ( int i = 0; i < arrANSWER_CHOICES.Length; i++ )
				{
					SummaryResults oSUMMARY = new SummaryResults();
					oSUMMARY.ANSWER_TEXT   = arrANSWER_CHOICES[i];
					oSUMMARY.ANSWER_ID     = SplendidCRM.Survey.ConvertAnswerID(SplendidCRM.Survey.md5(arrANSWER_CHOICES[i]));
					oSUMMARY.ANSWERED      = new List<DataRow>();
					oSUMMARY.SKIPPED       = new List<DataRow>();
					oSUMMARY.OTHER_SUMMARY = new List<DataRow>();
					ANSWER_CHOICES_SUMMARY.Add(oSUMMARY);
					for ( int j = 0; j < arrCOLUMN_CHOICES.Length; j++ )
					{
						ColumnResults oCOLUMN = new ColumnResults();
						oCOLUMN.COLUMN_TEXT   = arrCOLUMN_CHOICES[j];
						oCOLUMN.COLUMN_ID     = SplendidCRM.Survey.ConvertAnswerID(SplendidCRM.Survey.md5(arrCOLUMN_CHOICES[j]));
						oCOLUMN.ANSWERED      = new List<DataRow>();
						oSUMMARY.COLUMNS.Add(oCOLUMN);
					}
				}
			}
			
			int nANSWERED = 0;
			int nSKIPPED  = 0;
			Dictionary<Guid, bool> oANSWERED = new Dictionary<Guid, bool>();
			Dictionary<Guid, bool> oSKIPPED  = new Dictionary<Guid, bool>();
			foreach ( DataRow row in dtSurveyResults.Rows )
			{
				Guid gSURVEY_RESULT_ID = Sql.ToGuid(row["SURVEY_RESULT_ID"]);
				Guid gANSWER_ID        = Sql.ToGuid(row["ANSWER_ID"       ]);
				Guid gCOLUMN_ID        = Sql.ToGuid(row["COLUMN_ID"       ]);
				if ( Sql.IsEmptyGuid(gANSWER_ID) || Sql.IsEmptyGuid(gCOLUMN_ID) )
				{
					if ( !oSKIPPED.ContainsKey(gSURVEY_RESULT_ID) )
					{
						oSKIPPED[gSURVEY_RESULT_ID] = true;
						nSKIPPED++;
					}
					if ( bOTHER_ENABLED && gOTHER_ID == gANSWER_ID )
					{
						OTHER_SUMMARY.Add(row);
					}
				}
				else
				{
					for ( int j = 0; j < ANSWER_CHOICES_SUMMARY.Count; j++ )
					{
						SummaryResults oANSWER_CHOICES_SUMMARY = ANSWER_CHOICES_SUMMARY[j];
						if ( oANSWER_CHOICES_SUMMARY.ANSWER_ID == gANSWER_ID )
						{
							if ( row["ANSWER_TEXT"] != DBNull.Value )
							{
								for ( int k = 0; k < oANSWER_CHOICES_SUMMARY.COLUMNS.Count; k++ )
								{
									ColumnResults oCOLUMN = oANSWER_CHOICES_SUMMARY.COLUMNS[k];
									if ( oCOLUMN.COLUMN_ID == gCOLUMN_ID )
									{
										if ( row["COLUMN_TEXT"] != DBNull.Value )
										{
											oCOLUMN.ANSWERED.Add(row);
											oANSWER_CHOICES_SUMMARY.ANSWER_TOTAL++;
											if ( !oANSWERED.ContainsKey(gSURVEY_RESULT_ID) )
											{
												oANSWERED[gSURVEY_RESULT_ID] = true;
												nANSWERED++;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			
			dtSummary.Columns.Add("Answers", typeof(System.String));
			lstHeaders.Add(String.Empty);
			if ( arrCOLUMN_CHOICES != null )
			{
				for ( int j = 0; j < arrCOLUMN_CHOICES.Length; j++ )
				{
					string sColumnName = "C" + (j+1).ToString();
					dtSummary.Columns.Add(sColumnName, typeof(System.Int32 ));
					lstHeaders.Add(arrCOLUMN_CHOICES[j]);
				}
			}
			dtSummary.Columns.Add("Responses", typeof(System.Int32 ));
			lstHeaders.Add(L10n.Term("SurveyResults.LBL_RESPONSES"));
			
			foreach ( SummaryResults summary in ANSWER_CHOICES_SUMMARY )
			{
				DataRow rowSummary = dtSummary.NewRow();
				dtSummary.Rows.Add(rowSummary);
				rowSummary["Answers"  ] = summary.ANSWER_TEXT;
				rowSummary["Responses"] = summary.ANSWER_TOTAL;
				if ( arrCOLUMN_CHOICES != null )
				{
					for ( int j = 0; j < arrCOLUMN_CHOICES.Length; j++ )
					{
						string sColumnName = "C" + (j+1).ToString();
						rowSummary[sColumnName] = summary.COLUMNS[j].ANSWERED.Count;
					}
				}
			}
			if ( bOTHER_ENABLED )
			{
				DataRow rowSummary = dtSummary.NewRow();
				dtSummary.Rows.Add(rowSummary);
				rowSummary["Answers"  ] = L10n.Term("SurveyResults.LBL_LIST_OTHER_TEXT");
				rowSummary["Responses"] = OTHER_SUMMARY.Count;
			}
			sAnsweredSkipped = String.Format(L10n.Term("SurveyResults.LBL_ANSWERED"), nANSWERED) + "  " + String.Format(L10n.Term("SurveyResults.LBL_SKIPPED"), nSKIPPED);
		}

		private void SummaryResults_Text(DataRow rowQuestion, DataTable dtSurveyResults, List<string> lstHeaders, DataTable dtSummary, ref string sAnsweredSkipped)
		{
			int nANSWERED = 0;
			int nSKIPPED  = 0;
			dtSummary.Columns.Add("Date"    , typeof(System.DateTime));
			dtSummary.Columns.Add("Response", typeof(System.String  ));
			foreach ( DataRow row in dtSurveyResults.Rows )
			{
				Guid gSURVEY_RESULT_ID = Sql.ToGuid(row["SURVEY_RESULT_ID"]);
				if ( row["ANSWER_TEXT"] != DBNull.Value )
				{
					DataRow rowSummary = dtSummary.NewRow();
					dtSummary.Rows.Add(rowSummary);
					rowSummary["Date"    ] = Sql.ToDateTime(row["DATE_ENTERED"]);
					rowSummary["Response"] = Sql.ToString  (row["ANSWER_TEXT" ]);
					nANSWERED++;
				}
				else
				{
					nSKIPPED++;
				}
			}
			
			lstHeaders.Add(L10n.Term("SurveyResults.LBL_LIST_DATE_ENTERED"));
			lstHeaders.Add(L10n.Term("SurveyResults.LBL_RESPONSES"        ));
			sAnsweredSkipped = String.Format(L10n.Term("SurveyResults.LBL_ANSWERED"), nANSWERED) + "  " + String.Format(L10n.Term("SurveyResults.LBL_SKIPPED"), nSKIPPED);
		}

		private void SummaryResults_TextMatrix(DataRow rowQuestion, DataTable dtSurveyResults, List<string> lstHeaders, DataTable dtSummary, ref string sAnsweredSkipped)
		{
			int nANSWERED = 0;
			int nSKIPPED  = 0;
			dtSummary.Columns.Add("Date", typeof(System.DateTime));
			lstHeaders.Add(L10n.Term("SurveyResults.LBL_LIST_DATE_ENTERED"));
			
			string sQUESTION_TYPE  = Sql.ToString(rowQuestion["QUESTION_TYPE" ]);
			string sANSWER_CHOICES = Sql.ToString(rowQuestion["ANSWER_CHOICES"]);
			List<Guid> lstANSWER_IDs  = new List<Guid>();
			if ( !Sql.IsEmptyString(sANSWER_CHOICES) )
			{
				string[] arrANSWER_CHOICES = sANSWER_CHOICES.Split(new string[] { ControlChars.CrLf }, StringSplitOptions.None);
				for ( int i = 0; i < arrANSWER_CHOICES.Length; i++ )
				{
					string sColumnName = "A" + (i+1).ToString();
					Guid   gANSWER_ID  = SplendidCRM.Survey.ConvertAnswerID(SplendidCRM.Survey.md5(arrANSWER_CHOICES[i]));
					lstANSWER_IDs.Add(gANSWER_ID);
					lstHeaders.Add(arrANSWER_CHOICES[i]);
					switch ( sQUESTION_TYPE )
					{
						case "Date"             :  dtSummary.Columns.Add(sColumnName, typeof(System.DateTime));  break;
						case "Textbox Numerical":  dtSummary.Columns.Add(sColumnName, typeof(System.Decimal ));  break;
						default                 :  dtSummary.Columns.Add(sColumnName, typeof(System.String  ));  break;
					}
				}
			}
			foreach ( DataRow row in dtSurveyResults.Rows )
			{
				Guid gSURVEY_RESULT_ID = Sql.ToGuid(row["SURVEY_RESULT_ID"]);
				Guid gANSWER_ID        = Sql.ToGuid(row["ANSWER_ID"       ]);
				if ( row["ANSWER_TEXT"] != DBNull.Value )
				{
					DataRow rowSummary = dtSummary.NewRow();
					dtSummary.Rows.Add(rowSummary);
					rowSummary["Date"] = Sql.ToDateTime(row["DATE_ENTERED"]);
					if ( lstANSWER_IDs.Contains(gANSWER_ID) )
					{
						int    nColumn     = lstANSWER_IDs.IndexOf(gANSWER_ID);
						string sColumnName = "A" + (nColumn+1).ToString();
						switch ( sQUESTION_TYPE )
						{
							case "Date"             :  rowSummary[sColumnName] = Sql.ToDateTime(row["ANSWER_TEXT"]);  break;
							case "Textbox Numerical":  rowSummary[sColumnName] = Sql.ToDecimal (row["ANSWER_TEXT"]);  break;
							default                 :  rowSummary[sColumnName] = Sql.ToString  (row["ANSWER_TEXT"]);  break;
						}
					}
					nANSWERED++;
				}
				else
				{
					nSKIPPED++;
				}
			}
			
			sAnsweredSkipped = String.Format(L10n.Term("SurveyResults.LBL_ANSWERED"), nANSWERED) + "  " + String.Format(L10n.Term("SurveyResults.LBL_SKIPPED"), nSKIPPED);
		}

		private void SummaryResults_Demographic(DataRow rowQuestion, DataTable dtSurveyResults, List<string> lstHeaders, DataTable dtSummary, ref string sAnsweredSkipped)
		{
			int nANSWERED = 0;
			int nSKIPPED  = 0;
			dtSummary.Columns.Add("Date", typeof(System.DateTime));
			lstHeaders.Add(L10n.Term("SurveyResults.LBL_LIST_DATE_ENTERED"));
			
			// <Demographic><Field Name="NAME" Visible="" Required="">Name:</Field></Demographic>
			string sQUESTION_TYPE  = Sql.ToString(rowQuestion["QUESTION_TYPE" ]);
			string sCOLUMN_CHOICES = Sql.ToString(rowQuestion["COLUMN_CHOICES"]);
			Dictionary<Guid, ColumnResults> lstCOLUMN_CHOICES  = new Dictionary<Guid, ColumnResults>();
			if ( !Sql.IsEmptyString(sCOLUMN_CHOICES) )
			{
				XmlDocument xml = new XmlDocument();
				xml.XmlResolver = null;
				xml.LoadXml(sCOLUMN_CHOICES);
				XmlNodeList nlFields = xml.DocumentElement.SelectNodes("Field");
				foreach ( XmlNode xField in nlFields )
				{
					ColumnResults oCOLUMN = new ColumnResults();
					oCOLUMN.COLUMN_TEXT  = XmlUtil.GetNamedItem(xField, "Name");
					oCOLUMN.COLUMN_LABEL = xField.InnerText;
					oCOLUMN.COLUMN_ID     = SplendidCRM.Survey.ConvertAnswerID(SplendidCRM.Survey.md5(oCOLUMN.COLUMN_TEXT));
					lstCOLUMN_CHOICES.Add(oCOLUMN.COLUMN_ID, oCOLUMN);
					dtSummary.Columns.Add(oCOLUMN.COLUMN_TEXT, typeof(System.String));
					lstHeaders.Add(oCOLUMN.COLUMN_LABEL.Replace(":", String.Empty));
				}
			}
			bool bStartRecord = false;
			DataRow rowSummary = dtSummary.NewRow();
			Guid gLAST_SURVEY_RESULT_ID = Guid.Empty;
			foreach ( DataRow row in dtSurveyResults.Rows )
			{
				Guid gSURVEY_RESULT_ID = Sql.ToGuid(row["SURVEY_RESULT_ID"]);
				Guid gANSWER_ID        = Sql.ToGuid(row["ANSWER_ID"       ]);
				if ( gSURVEY_RESULT_ID != gLAST_SURVEY_RESULT_ID && !Sql.IsEmptyGuid(gLAST_SURVEY_RESULT_ID) )
				{
					if ( bStartRecord )
					{
						dtSummary.Rows.Add(rowSummary);
						nANSWERED++;
						bStartRecord = false;
					}
					gLAST_SURVEY_RESULT_ID = gSURVEY_RESULT_ID;
				}
				if ( !Sql.IsEmptyGuid(gANSWER_ID) )
				{
					bStartRecord = true;
					if ( lstCOLUMN_CHOICES.ContainsKey(gANSWER_ID) )
					{
						ColumnResults oCOLUMN = lstCOLUMN_CHOICES[gANSWER_ID];
						rowSummary["Date"             ] = Sql.ToDateTime(row["DATE_ENTERED"]);
						rowSummary[oCOLUMN.COLUMN_TEXT] = Sql.ToString  (row["ANSWER_TEXT" ]);
					}
				}
				else
				{
					nSKIPPED++;
				}
			}
			if ( bStartRecord )
			{
				dtSummary.Rows.Add(rowSummary);
				nANSWERED++;
				bStartRecord = false;
			}
			
			sAnsweredSkipped = String.Format(L10n.Term("SurveyResults.LBL_ANSWERED"), nANSWERED) + "  " + String.Format(L10n.Term("SurveyResults.LBL_SKIPPED"), nSKIPPED);
		}

		private void SummaryResults_RatingScale(DataRow rowQuestion, DataTable dtSurveyResults, List<string> lstHeaders, DataTable dtSummary, ref string sAnsweredSkipped)
		{
			List<SummaryResults> ANSWER_CHOICES_SUMMARY = new List<SummaryResults>();
			List<DataRow>        OTHER_SUMMARY          = new List<DataRow>();
			Guid   gOTHER_ID          = SplendidCRM.Survey.ConvertAnswerID(SplendidCRM.Survey.md5("Other"));
			bool   bOTHER_ENABLED     = Sql.ToBoolean(rowQuestion["OTHER_ENABLED"    ]);
			bool   bNA_ENABLED        = Sql.ToBoolean(rowQuestion["NA_ENABLED"       ]);
			string sNA_LABEL          = Sql.ToString (rowQuestion["NA_LABEL"         ]);
			bool   bOTHER_ONE_PER_ROW = Sql.ToBoolean(rowQuestion["OTHER_ONE_PER_ROW"]);
			string sANSWER_CHOICES    = Sql.ToString (rowQuestion["ANSWER_CHOICES"   ]);
			string sCOLUMN_CHOICES    = Sql.ToString (rowQuestion["COLUMN_CHOICES"   ]);
			string[] arrANSWER_CHOICES = null;
			XmlDocument xml = new XmlDocument();
			xml.XmlResolver = null;
			XmlNodeList nlRatings = null;
			if ( !Sql.IsEmptyString(sCOLUMN_CHOICES) )
			{
				xml.LoadXml(sCOLUMN_CHOICES);
				nlRatings = xml.DocumentElement.SelectNodes("Rating");
			}
			if ( !Sql.IsEmptyString(sANSWER_CHOICES) )
			{
				arrANSWER_CHOICES = sANSWER_CHOICES.Split(new string[] { ControlChars.CrLf }, StringSplitOptions.None);
				for ( int i = 0; i < arrANSWER_CHOICES.Length; i++ )
				{
					SummaryResults oSUMMARY = new SummaryResults();
					oSUMMARY.ANSWER_TEXT   = arrANSWER_CHOICES[i];
					oSUMMARY.ANSWER_ID     = SplendidCRM.Survey.ConvertAnswerID(SplendidCRM.Survey.md5(arrANSWER_CHOICES[i]));
					oSUMMARY.ANSWERED      = new List<DataRow>();
					oSUMMARY.SKIPPED       = new List<DataRow>();
					oSUMMARY.OTHER_SUMMARY = new List<DataRow>();
					oSUMMARY.ANSWER_TOTAL  = 0;
					oSUMMARY.WEIGHT_TOTAL  = 0.0m;
					ANSWER_CHOICES_SUMMARY.Add(oSUMMARY);
					if ( nlRatings != null )
					{
						// <Ratings><Rating><Label>11</Label><Weight>1</Weight></Rating></Ratings>
						foreach ( XmlNode xRating in nlRatings )
						{
							ColumnResults oCOLUMN = new ColumnResults();
							oCOLUMN.COLUMN_TEXT   = XmlUtil.SelectSingleNode(xRating, "Label");
							oCOLUMN.WEIGHT        = Sql.ToInteger(XmlUtil.SelectSingleNode(xRating, "Weight"));
							oCOLUMN.COLUMN_LABEL  = oCOLUMN.COLUMN_TEXT;
							oCOLUMN.COLUMN_ID     = SplendidCRM.Survey.ConvertAnswerID(SplendidCRM.Survey.md5(oCOLUMN.COLUMN_TEXT));
							oSUMMARY.COLUMNS.Add(oCOLUMN);
						}
					}
					if ( bNA_ENABLED )
					{
						ColumnResults oCOLUMN = new ColumnResults();
						oCOLUMN.COLUMN_TEXT   = "N/A";
						oCOLUMN.WEIGHT        = 0;
						oCOLUMN.COLUMN_LABEL  = oCOLUMN.COLUMN_TEXT;
						oCOLUMN.COLUMN_ID     = SplendidCRM.Survey.ConvertAnswerID(SplendidCRM.Survey.md5(oCOLUMN.COLUMN_TEXT));
						oSUMMARY.COLUMNS.Add(oCOLUMN);
					}
				}
			}
			
			int nANSWERED = 0;
			int nSKIPPED  = 0;
			Dictionary<Guid, bool> oANSWERED = new Dictionary<Guid, bool>();
			Dictionary<Guid, bool> oSKIPPED  = new Dictionary<Guid, bool>();
			foreach ( DataRow row in dtSurveyResults.Rows )
			{
				Guid gSURVEY_RESULT_ID = Sql.ToGuid(row["SURVEY_RESULT_ID"]);
				Guid gANSWER_ID        = Sql.ToGuid(row["ANSWER_ID"       ]);
				Guid gCOLUMN_ID        = Sql.ToGuid(row["COLUMN_ID"       ]);
				if ( Sql.IsEmptyGuid(gANSWER_ID) || Sql.IsEmptyGuid(gCOLUMN_ID) )
				{
					if ( !oSKIPPED.ContainsKey(gSURVEY_RESULT_ID) )
					{
						oSKIPPED[gSURVEY_RESULT_ID] = true;
						nSKIPPED++;
					}
					if ( bOTHER_ENABLED && gOTHER_ID == gANSWER_ID )
					{
						OTHER_SUMMARY.Add(row);
					}
				}
				else
				{
					for ( int j = 0; j < ANSWER_CHOICES_SUMMARY.Count; j++ )
					{
						SummaryResults oANSWER_CHOICES_SUMMARY = ANSWER_CHOICES_SUMMARY[j];
						if ( oANSWER_CHOICES_SUMMARY.ANSWER_ID == gANSWER_ID )
						{
							if ( row["ANSWER_TEXT"] != DBNull.Value )
							{
								for ( int k = 0; k < oANSWER_CHOICES_SUMMARY.COLUMNS.Count; k++ )
								{
									ColumnResults oCOLUMN = oANSWER_CHOICES_SUMMARY.COLUMNS[k];
									if ( oCOLUMN.COLUMN_ID == gCOLUMN_ID )
									{
										if ( row["COLUMN_TEXT"] != DBNull.Value )
										{
											if ( !oANSWERED.ContainsKey(gSURVEY_RESULT_ID) )
											{
												oANSWERED[gSURVEY_RESULT_ID] = true;
												nANSWERED++;
											}
											if ( bOTHER_ONE_PER_ROW && gOTHER_ID == Sql.ToGuid(row["COLUMN_ID"]) )
											{
												oANSWER_CHOICES_SUMMARY.OTHER_SUMMARY.Add(row);
											}
											else
											{
												oCOLUMN.ANSWERED.Add(row);
												oANSWER_CHOICES_SUMMARY.ANSWER_TOTAL++;
												oANSWER_CHOICES_SUMMARY.WEIGHT_TOTAL += Sql.ToDecimal(row["WEIGHT"]);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			
			DataRow rowSummary = null;
			dtSummary.Columns.Add("Answers", typeof(System.String));
			lstHeaders.Add(String.Empty);
			if ( nlRatings != null )
			{
				rowSummary = dtSummary.NewRow();
				dtSummary.Rows.Add(rowSummary);
				rowSummary["Answers"] = L10n.Term("SurveyResults.LBL_LIST_WEIGHT");
				foreach ( XmlNode xRating in nlRatings )
				{
					string sColumnName = XmlUtil.SelectSingleNode(xRating, "Label");
					dtSummary.Columns.Add(sColumnName, typeof(System.Int32 ));
					lstHeaders.Add(sColumnName);
					rowSummary[sColumnName] = Sql.ToInteger(XmlUtil.SelectSingleNode(xRating, "Weight"));
				}
			}
			if ( bNA_ENABLED )
			{
				string sColumnName = "N/A";
				dtSummary.Columns.Add(sColumnName, typeof(System.Int32 ));
				lstHeaders.Add(L10n.Term("SurveyResults.LBL_NA"));
			}
			dtSummary.Columns.Add("Responses"    , typeof(System.Int32  ));
			dtSummary.Columns.Add("AverageRating", typeof(System.Decimal));
			lstHeaders.Add(L10n.Term("SurveyResults.LBL_RESPONSES"     ));
			lstHeaders.Add(L10n.Term("SurveyResults.LBL_AVERAGE_RATING"));
			
			foreach ( SummaryResults summary in ANSWER_CHOICES_SUMMARY )
			{
				rowSummary = dtSummary.NewRow();
				dtSummary.Rows.Add(rowSummary);
				rowSummary["Answers"  ] = summary.ANSWER_TEXT;
				rowSummary["Responses"] = summary.ANSWER_TOTAL;
				foreach ( ColumnResults oCOLUMN in summary.COLUMNS )
				{
					rowSummary[oCOLUMN.COLUMN_TEXT] = oCOLUMN.ANSWERED.Count;
				}
				if ( summary.ANSWER_TOTAL > 0 )
				{
					rowSummary["AverageRating"] = (summary.WEIGHT_TOTAL / summary.ANSWER_TOTAL);
				}
				else
				{
					rowSummary["AverageRating"] = 0.0m;
				}
			}
			if ( bOTHER_ENABLED )
			{
				rowSummary = dtSummary.NewRow();
				dtSummary.Rows.Add(rowSummary);
				rowSummary["Answers"  ] = L10n.Term("SurveyResults.LBL_LIST_OTHER_TEXT");
				rowSummary["Responses"] = OTHER_SUMMARY.Count;
			}
			sAnsweredSkipped = String.Format(L10n.Term("SurveyResults.LBL_ANSWERED"), nANSWERED) + "  " + String.Format(L10n.Term("SurveyResults.LBL_SKIPPED"), nSKIPPED);
		}

		private void SummaryResults_Ranking(DataRow rowQuestion, DataTable dtSurveyResults, List<string> lstHeaders, DataTable dtSummary, ref string sAnsweredSkipped)
		{
			List<SummaryResults> ANSWER_CHOICES_SUMMARY = new List<SummaryResults>();
			List<DataRow>        OTHER_SUMMARY          = new List<DataRow>();
			bool   bNA_ENABLED        = Sql.ToBoolean(rowQuestion["NA_ENABLED"       ]);
			string sNA_LABEL          = Sql.ToString (rowQuestion["NA_LABEL"         ]);
			string sANSWER_CHOICES    = Sql.ToString (rowQuestion["ANSWER_CHOICES"   ]);
			string sCOLUMN_CHOICES    = Sql.ToString (rowQuestion["COLUMN_CHOICES"   ]);
			string[] arrANSWER_CHOICES = null;
			if ( !Sql.IsEmptyString(sANSWER_CHOICES) )
			{
				arrANSWER_CHOICES = sANSWER_CHOICES.Split(new string[] { ControlChars.CrLf }, StringSplitOptions.None);
				for ( int i = 0; i < arrANSWER_CHOICES.Length; i++ )
				{
					SummaryResults oSUMMARY = new SummaryResults();
					oSUMMARY.ANSWER_TEXT   = arrANSWER_CHOICES[i];
					oSUMMARY.ANSWER_ID     = SplendidCRM.Survey.ConvertAnswerID(SplendidCRM.Survey.md5(arrANSWER_CHOICES[i]));
					oSUMMARY.ANSWERED      = new List<DataRow>();
					oSUMMARY.SKIPPED       = new List<DataRow>();
					oSUMMARY.OTHER_SUMMARY = new List<DataRow>();
					oSUMMARY.ANSWER_TOTAL  = 0;
					oSUMMARY.WEIGHT_TOTAL  = 0.0m;
					ANSWER_CHOICES_SUMMARY.Add(oSUMMARY);
					for ( int j = 0; j < arrANSWER_CHOICES.Length; j++ )
					{
						ColumnResults oCOLUMN = new ColumnResults();
						oCOLUMN.COLUMN_TEXT   = (j + 1).ToString();
						oCOLUMN.WEIGHT        = (j + 1);
						oCOLUMN.COLUMN_LABEL  = oCOLUMN.COLUMN_TEXT;
						oCOLUMN.COLUMN_ID     = SplendidCRM.Survey.ConvertAnswerID(SplendidCRM.Survey.md5(oCOLUMN.COLUMN_TEXT));
						oSUMMARY.COLUMNS.Add(oCOLUMN);
					}
					if ( bNA_ENABLED )
					{
						ColumnResults oCOLUMN = new ColumnResults();
						oCOLUMN.COLUMN_TEXT   = "N/A";
						oCOLUMN.WEIGHT        = 0;
						oCOLUMN.COLUMN_LABEL  = oCOLUMN.COLUMN_TEXT;
						oCOLUMN.COLUMN_ID     = SplendidCRM.Survey.ConvertAnswerID(SplendidCRM.Survey.md5(oCOLUMN.COLUMN_TEXT));
						oSUMMARY.COLUMNS.Add(oCOLUMN);
					}
				}
			}
			
			int nANSWERED = 0;
			int nSKIPPED  = 0;
			Dictionary<Guid, bool> oANSWERED = new Dictionary<Guid, bool>();
			Dictionary<Guid, bool> oSKIPPED  = new Dictionary<Guid, bool>();
			foreach ( DataRow row in dtSurveyResults.Rows )
			{
				Guid gSURVEY_RESULT_ID = Sql.ToGuid(row["SURVEY_RESULT_ID"]);
				Guid gANSWER_ID        = Sql.ToGuid(row["ANSWER_ID"       ]);
				if ( Sql.IsEmptyGuid(gANSWER_ID) )
				{
					if ( !oSKIPPED.ContainsKey(gSURVEY_RESULT_ID) )
					{
						oSKIPPED[gSURVEY_RESULT_ID] = true;
						nSKIPPED++;
					}
				}
				else
				{
					for ( int j = 0; j < ANSWER_CHOICES_SUMMARY.Count; j++ )
					{
						SummaryResults oANSWER_CHOICES_SUMMARY = ANSWER_CHOICES_SUMMARY[j];
						if ( oANSWER_CHOICES_SUMMARY.ANSWER_ID == gANSWER_ID )
						{
							if ( row["ANSWER_TEXT"] != DBNull.Value )
							{
								for ( int k = 0; k < oANSWER_CHOICES_SUMMARY.COLUMNS.Count; k++ )
								{
									ColumnResults oCOLUMN = oANSWER_CHOICES_SUMMARY.COLUMNS[k];
									if ( oCOLUMN.WEIGHT == Sql.ToDecimal(row["WEIGHT"]) )
									{
										if ( !oANSWERED.ContainsKey(gSURVEY_RESULT_ID) )
										{
											oANSWERED[gSURVEY_RESULT_ID] = true;
											nANSWERED++;
										}
										oCOLUMN.ANSWERED.Add(row);
										oANSWER_CHOICES_SUMMARY.ANSWER_TOTAL++;
										oANSWER_CHOICES_SUMMARY.WEIGHT_TOTAL += Sql.ToDecimal(row["WEIGHT"]);
									}
								}
							}
						}
					}
				}
			}
			
			dtSummary.Columns.Add("Answers", typeof(System.String));
			lstHeaders.Add(String.Empty);
			for ( int j = 0; j < ANSWER_CHOICES_SUMMARY.Count; j++ )
			{
				string sColumnName = "A" + (j + 1).ToString();
				dtSummary.Columns.Add(sColumnName, typeof(System.Int32 ));
				lstHeaders.Add(sColumnName);
			}
			if ( bNA_ENABLED )
			{
				string sColumnName = "N/A";
				dtSummary.Columns.Add(sColumnName, typeof(System.Int32 ));
				lstHeaders.Add(L10n.Term("SurveyResults.LBL_NA"));
			}
			dtSummary.Columns.Add("Responses"    , typeof(System.Int32  ));
			dtSummary.Columns.Add("AverageRating", typeof(System.Decimal));
			lstHeaders.Add(L10n.Term("SurveyResults.LBL_RESPONSES"     ));
			lstHeaders.Add(L10n.Term("SurveyResults.LBL_AVERAGE_RATING"));
			
			foreach ( SummaryResults summary in ANSWER_CHOICES_SUMMARY )
			{
				DataRow rowSummary = dtSummary.NewRow();
				dtSummary.Rows.Add(rowSummary);
				rowSummary["Answers"  ] = summary.ANSWER_TEXT;
				rowSummary["Responses"] = summary.ANSWER_TOTAL;
				foreach ( ColumnResults oCOLUMN in summary.COLUMNS )
				{
					rowSummary[oCOLUMN.COLUMN_TEXT] = oCOLUMN.ANSWERED.Count;
				}
				if ( summary.ANSWER_TOTAL > 0 )
				{
					rowSummary["AverageRating"] = (summary.WEIGHT_TOTAL / summary.ANSWER_TOTAL);
				}
				else
				{
					rowSummary["AverageRating"] = 0.0m;
				}
			}
			sAnsweredSkipped = String.Format(L10n.Term("SurveyResults.LBL_ANSWERED"), nANSWERED) + "  " + String.Format(L10n.Term("SurveyResults.LBL_SKIPPED"), nSKIPPED);
		}

		private void SummaryResults_Range(DataRow rowQuestion, DataTable dtSurveyResults, List<string> lstHeaders, DataTable dtSummary, ref string sAnsweredSkipped)
		{
			List<SummaryResults> ANSWER_CHOICES_SUMMARY = new List<SummaryResults>();
			List<DataRow>        OTHER_SUMMARY          = new List<DataRow>();
			string sANSWER_CHOICES    = Sql.ToString (rowQuestion["ANSWER_CHOICES"   ]);
			int nRANGE_MIN  = 0  ;
			int nRANGE_MAX  = 100;
			int nRANGE_STEP = 1  ;
			if ( !Sql.IsEmptyString(sANSWER_CHOICES) )
			{
				string[] arrANSWER_CHOICES = sANSWER_CHOICES.Split(new string[] { ControlChars.CrLf }, StringSplitOptions.None);
				nRANGE_MIN  = Sql.ToInteger(arrANSWER_CHOICES[0]);
				nRANGE_MAX  = 100;
				nRANGE_STEP = 1;
				if ( arrANSWER_CHOICES.Length > 0 )
					nRANGE_MAX  = Sql.ToInteger(arrANSWER_CHOICES[1]);
				if ( arrANSWER_CHOICES.Length > 1 )
					nRANGE_STEP = Sql.ToInteger(arrANSWER_CHOICES[2]);
				if ( nRANGE_STEP == 0 )
					nRANGE_STEP = 1;
				// 12/26/2015 Paul.  We will have a loop creating summary rows, so we need to make sure the values are valid. 
				if ( nRANGE_STEP > 0 )
				{
					if ( nRANGE_MIN > nRANGE_MAX )
					{
						nRANGE_MIN  = 0  ;
						nRANGE_MAX  = 100;
					}
				}
				else
				{
					if ( nRANGE_MIN < nRANGE_MAX )
					{
						nRANGE_MIN  = 0  ;
						nRANGE_MAX  = 100;
					}
				}
			}
			
			int nANSWERED = 0;
			int nSKIPPED  = 0;
			Dictionary<Guid, bool> oANSWERED = new Dictionary<Guid, bool>();
			Dictionary<Guid, bool> oSKIPPED  = new Dictionary<Guid, bool>();
			foreach ( DataRow row in dtSurveyResults.Rows )
			{
				Guid gSURVEY_RESULT_ID = Sql.ToGuid(row["SURVEY_RESULT_ID"]);
				if ( row["ANSWER_TEXT"] != DBNull.Value )
				{
					bool bFound = false;
					for ( int j = 0; j < ANSWER_CHOICES_SUMMARY.Count; j++ )
					{
						SummaryResults oANSWER_CHOICES_SUMMARY = ANSWER_CHOICES_SUMMARY[j];
						if ( oANSWER_CHOICES_SUMMARY.ANSWER_TEXT == Sql.ToString(row["ANSWER_TEXT"]) )
						{
							oANSWER_CHOICES_SUMMARY.ANSWERED.Add(row);
							oANSWER_CHOICES_SUMMARY.WEIGHT_TOTAL += Sql.ToDecimal(row["ANSWER_TEXT"]);
							if ( !oANSWERED.ContainsKey(gSURVEY_RESULT_ID) )
							{
								oANSWERED[gSURVEY_RESULT_ID] = true;
								nANSWERED++;
							}
							bFound = true;
						}
					}
					if ( !bFound )
					{
						SummaryResults oSUMMARY = new SummaryResults();
						oSUMMARY.ANSWER_TEXT   =  Sql.ToString(row["ANSWER_TEXT"]);
						oSUMMARY.ANSWER_ID     = Guid.Empty;
						oSUMMARY.ANSWERED      = new List<DataRow>();
						oSUMMARY.SKIPPED       = new List<DataRow>();
						oSUMMARY.WEIGHT_TOTAL  = Sql.ToDecimal(row["ANSWER_TEXT"]);
						oSUMMARY.ANSWERED.Add(row);
						ANSWER_CHOICES_SUMMARY.Add(oSUMMARY);
						if ( !oANSWERED.ContainsKey(gSURVEY_RESULT_ID) )
						{
							oANSWERED[gSURVEY_RESULT_ID] = true;
							nANSWERED++;
						}
					}
				}
				else
				{
					if ( !oSKIPPED.ContainsKey(gSURVEY_RESULT_ID) )
					{
						oSKIPPED[gSURVEY_RESULT_ID] = true;
						nSKIPPED++;
					}
				}
			}
			
			dtSummary.Columns.Add("Answers", typeof(System.String));
			lstHeaders.Add(String.Empty);
			dtSummary.Columns.Add("Average"  , typeof(System.Decimal));
			dtSummary.Columns.Add("Total"    , typeof(System.Decimal));
			dtSummary.Columns.Add("Responses", typeof(System.Int32  ));
			lstHeaders.Add(L10n.Term("SurveyResults.LBL_AVERAGE"  ));
			lstHeaders.Add(L10n.Term("SurveyResults.LBL_TOTAL"    ));
			lstHeaders.Add(L10n.Term("SurveyResults.LBL_RESPONSES"));

			ANSWER_CHOICES_SUMMARY.Sort(new SummaryResults.Comparer());
			
			foreach ( SummaryResults summary in ANSWER_CHOICES_SUMMARY )
			{
				DataRow rowSummary = dtSummary.NewRow();
				dtSummary.Rows.Add(rowSummary);
				rowSummary["Answers"  ] = summary.ANSWER_TEXT;
				rowSummary["Total"    ] = summary.WEIGHT_TOTAL;
				rowSummary["Responses"] = summary.ANSWERED.Count;
				rowSummary["Average"  ] = summary.WEIGHT_TOTAL / nANSWERED;
			}
			sAnsweredSkipped = String.Format(L10n.Term("SurveyResults.LBL_ANSWERED"), nANSWERED) + "  " + String.Format(L10n.Term("SurveyResults.LBL_SKIPPED"), nSKIPPED);
		}

		private void SummaryResults_DropdownMatrix(DataRow rowQuestion, DataTable dtSurveyResults, List<string> lstHeaders, DataTable dtSummary, ref string sAnsweredSkipped)
		{
			List<SummaryResults> ANSWER_CHOICES_SUMMARY = new List<SummaryResults>();
			List<DataRow>        OTHER_SUMMARY          = new List<DataRow>();
			List<DropdownMenu>   arrCOLUMN_CHOICES      = new List<DropdownMenu>();
			Guid     gOTHER_ID         = SplendidCRM.Survey.ConvertAnswerID(SplendidCRM.Survey.md5("Other"));
			bool     bOTHER_ENABLED    = Sql.ToBoolean(rowQuestion["OTHER_ENABLED" ]);
			string   sANSWER_CHOICES   = Sql.ToString (rowQuestion["ANSWER_CHOICES"]);
			string   sCOLUMN_CHOICES   = Sql.ToString (rowQuestion["COLUMN_CHOICES"]);
			string[] arrANSWER_CHOICES = null;
			int      nMENU_MAX         = 0;
			if ( !Sql.IsEmptyString(sANSWER_CHOICES) && !Sql.IsEmptyString(sCOLUMN_CHOICES) )
			{
				arrANSWER_CHOICES = sANSWER_CHOICES.Split(new string[] { ControlChars.CrLf }, StringSplitOptions.None);
				XmlDocument xml = new XmlDocument();
				xml.XmlResolver = null;
				xml.LoadXml(sCOLUMN_CHOICES);
				// <Menus><Menu><Heading></Heading><Options></Options></Menu></Menus>
				XmlNodeList nlMenus = xml.DocumentElement.SelectNodes("Menu");
				foreach ( XmlNode xMenu in nlMenus )
				{
					DropdownMenu oMENU = new DropdownMenu();
					string sOptions = XmlUtil.SelectSingleNode(xMenu, "Options");
					oMENU.Heading   = XmlUtil.SelectSingleNode(xMenu, "Heading");
					oMENU.OPTIONS   = sOptions.Split(new string[] { ControlChars.CrLf }, StringSplitOptions.None);
					arrCOLUMN_CHOICES.Add(oMENU);
				}
				
				for ( int i = 0; i < arrANSWER_CHOICES.Length; i++ )
				{
					SummaryResults oSUMMARY = new SummaryResults();
					oSUMMARY.ANSWER_TEXT   = arrANSWER_CHOICES[i];
					oSUMMARY.ANSWER_ID     = SplendidCRM.Survey.ConvertAnswerID(SplendidCRM.Survey.md5(oSUMMARY.ANSWER_TEXT));
					oSUMMARY.COLUMNS       = new List<ColumnResults>();
					ANSWER_CHOICES_SUMMARY.Add(oSUMMARY);
					for ( int j = 0; j < arrCOLUMN_CHOICES.Count; j++ )
					{
						ColumnResults oCOLUMN = new ColumnResults();
						oCOLUMN.COLUMN_TEXT   = arrCOLUMN_CHOICES[j].Heading;
						oCOLUMN.COLUMN_ID     = SplendidCRM.Survey.ConvertAnswerID(SplendidCRM.Survey.md5(oCOLUMN.COLUMN_TEXT));
						oCOLUMN.OPTIONS       = arrCOLUMN_CHOICES[j].OPTIONS;
						oCOLUMN.MENUS         = new List<MenuResults>();
						oSUMMARY.COLUMNS.Add(oCOLUMN);
						for ( int k = 0; k < arrCOLUMN_CHOICES[j].OPTIONS.Length; k++ )
						{
							MenuResults oMENU = new MenuResults();
							oMENU.MENU_TEXT = arrCOLUMN_CHOICES[j].OPTIONS[k];
							oMENU.MENU_ID   = SplendidCRM.Survey.ConvertAnswerID(SplendidCRM.Survey.md5(oMENU.MENU_TEXT));
							oMENU.ANSWERED  = new List<DataRow>();
							oMENU.SKIPPED   = new List<DataRow>();
							oCOLUMN.MENUS.Add(oMENU);
						}
						nMENU_MAX = Math.Max(nMENU_MAX, oCOLUMN.OPTIONS.Length);
					}
				}
			}
			
			int nANSWERED = 0;
			int nSKIPPED  = 0;
			Dictionary<Guid, bool> oANSWERED = new Dictionary<Guid, bool>();
			Dictionary<Guid, bool> oSKIPPED  = new Dictionary<Guid, bool>();
			foreach ( DataRow row in dtSurveyResults.Rows )
			{
				Guid gSURVEY_RESULT_ID = Sql.ToGuid(row["SURVEY_RESULT_ID"]);
				Guid gANSWER_ID        = Sql.ToGuid(row["ANSWER_ID"       ]);
				Guid gCOLUMN_ID        = Sql.ToGuid(row["COLUMN_ID"       ]);
				Guid gMENU_ID          = Sql.ToGuid(row["MENU_ID"         ]);
				if ( Sql.IsEmptyGuid(gANSWER_ID) || Sql.IsEmptyGuid(gCOLUMN_ID) || Sql.IsEmptyGuid(gMENU_ID) )
				{
					if ( !oSKIPPED.ContainsKey(gSURVEY_RESULT_ID) )
					{
						oSKIPPED[gSURVEY_RESULT_ID] = true;
						nSKIPPED++;
					}
					if ( bOTHER_ENABLED && gOTHER_ID == gANSWER_ID )
					{
						OTHER_SUMMARY.Add(row);
					}
				}
				else
				{
					for ( int j = 0; j < ANSWER_CHOICES_SUMMARY.Count; j++ )
					{
						SummaryResults oSUMMARY = ANSWER_CHOICES_SUMMARY[j];
						if ( oSUMMARY.ANSWER_ID == gANSWER_ID )
						{
							if ( row["ANSWER_TEXT"] != DBNull.Value )
							{
								for ( int k = 0; k < oSUMMARY.COLUMNS.Count; k++ )
								{
									ColumnResults oCOLUMN = oSUMMARY.COLUMNS[k];
									if ( oCOLUMN.COLUMN_ID == gCOLUMN_ID )
									{
										if ( row["COLUMN_TEXT"] != DBNull.Value )
										{
											for ( int l = 0; l < oCOLUMN.MENUS.Count; l++ )
											{
												var oMENU = oCOLUMN.MENUS[l];
												if ( oMENU.MENU_ID == gMENU_ID )
												{
													if ( row["MENU_TEXT"] != DBNull.Value )
													{
														oMENU.ANSWERED.Add(row);
														oMENU.ANSWER_TOTAL++;
														if ( !oANSWERED.ContainsKey(gSURVEY_RESULT_ID) )
														{
															oANSWERED[gSURVEY_RESULT_ID] = true;
															nANSWERED++;
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			
			dtSummary.Columns.Add("Answers", typeof(System.String));
			lstHeaders.Add(String.Empty);
			for ( int j = 0; j < arrCOLUMN_CHOICES.Count; j++ )
			{
				dtSummary.Columns.Add("C" + j.ToString() + "Heading", typeof(System.String));
				dtSummary.Columns.Add("C" + j.ToString() + "Value"  , typeof(System.Int32 ));
				lstHeaders.Add(arrCOLUMN_CHOICES[j].Heading);
				lstHeaders.Add(String.Empty);
			}
			
			for ( int l = 0; l < ANSWER_CHOICES_SUMMARY.Count; l++ )
			{
				SummaryResults oSUMMARY = ANSWER_CHOICES_SUMMARY[l];
				DataRow rowSummary = dtSummary.NewRow();
				dtSummary.Rows.Add(rowSummary);
				rowSummary["Answers"] = oSUMMARY.ANSWER_TEXT;
				
				for ( int k = 0; k < nMENU_MAX; k++ )
				{
					rowSummary = dtSummary.NewRow();
					dtSummary.Rows.Add(rowSummary);
					for ( int j = 0; j < oSUMMARY.COLUMNS.Count; j++ )
					{
						ColumnResults oCOLUMN = oSUMMARY.COLUMNS[j];
						if ( k < oCOLUMN.MENUS.Count )
						{
							MenuResults oMENU = oCOLUMN.MENUS[k];
							rowSummary["C" + j.ToString() + "Heading"] = oMENU.MENU_TEXT;
							rowSummary["C" + j.ToString() + "Value"  ] = oMENU.ANSWER_TOTAL;
						}
					}
				}
			}
			sAnsweredSkipped = String.Format(L10n.Term("SurveyResults.LBL_ANSWERED"), nANSWERED) + "  " + String.Format(L10n.Term("SurveyResults.LBL_SKIPPED"), nSKIPPED);
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
