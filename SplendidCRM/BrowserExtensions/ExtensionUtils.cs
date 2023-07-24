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
using System.Web;
using System.Web.SessionState;
using System.Text;
using System.Data;

namespace SplendidCRM.BrowserExtensions
{
	public class ExtensionUtils
	{
		public static void AppendEditViewFields(string sMODULE_NAME, ExtensionDetails info, StringBuilder sb)
		{
			HttpSessionState Session = HttpContext.Current.Session;
			DataTable dtFields = SplendidCache.EditViewFields(sMODULE_NAME + ".EditExtension");
			bool bEnableTeamManagement  = Crm.Config.enable_team_management();
			bool bRequireTeamManagement = Crm.Config.require_team_management();
			bool bRequireUserAssignment = Crm.Config.require_user_assignment();
			bool bEnableDynamicTeams    = Crm.Config.enable_dynamic_teams();
			Guid gASSIGNED_USER_ID = Security.USER_ID;
			Guid gCURRENCY_ID      = Sql.ToGuid(Session["USER_SETTINGS/CURRENCY"]);
			L10N     L10n = new L10N(Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
			//TimeZone T10n = TimeZone.CreateTimeZone(Sql.ToGuid(Session["USER_SETTINGS/TIMEZONE"]));
			
			int nRowIndex = 0;
			int nColIndex = 0;
			sb.AppendLine("	<table id=\"tblSplendidEditView\" cellspacing=\"1\">");
			sb.AppendLine("		<tr>");
			foreach(DataRowView row in dtFields.DefaultView)
			{
				//string sEDIT_NAME         = Sql.ToString (row["EDIT_NAME"        ]);
				//int    nFIELD_INDEX       = Sql.ToInteger(row["FIELD_INDEX"      ]);
				string sFIELD_TYPE        = Sql.ToString (row["FIELD_TYPE"       ]);
				string sDATA_LABEL        = Sql.ToString (row["DATA_LABEL"       ]);
				string sDATA_FIELD        = Sql.ToString (row["DATA_FIELD"       ]);
				//string sDATA_FORMAT       = Sql.ToString (row["DATA_FORMAT"      ]);
				//string sDISPLAY_FIELD     = Sql.ToString (row["DISPLAY_FIELD"    ]);
				string sCACHE_NAME        = Sql.ToString (row["CACHE_NAME"       ]);
				//bool   bDATA_REQUIRED     = Sql.ToBoolean(row["DATA_REQUIRED"    ]);
				bool   bUI_REQUIRED       = Sql.ToBoolean(row["UI_REQUIRED"      ]);
				//string sONCLICK_SCRIPT    = Sql.ToString (row["ONCLICK_SCRIPT"   ]);
				//string sFORMAT_SCRIPT     = Sql.ToString (row["FORMAT_SCRIPT"    ]);
				//short  nFORMAT_TAB_INDEX  = Sql.ToShort  (row["FORMAT_TAB_INDEX" ]);
				int    nFORMAT_MAX_LENGTH = Sql.ToInteger(row["FORMAT_MAX_LENGTH"]);
				int    nFORMAT_SIZE       = Sql.ToInteger(row["FORMAT_SIZE"      ]);
				int    nFORMAT_ROWS       = Math.Abs(Sql.ToInteger(row["FORMAT_ROWS"]));
				int    nFORMAT_COLUMNS    = Sql.ToInteger(row["FORMAT_COLUMNS"   ]);
				int    nCOLSPAN           = Sql.ToInteger(row["COLSPAN"          ]);
				//int    nROWSPAN           = Sql.ToInteger(row["ROWSPAN"          ]);
				string sLABEL_WIDTH       = Sql.ToString (row["LABEL_WIDTH"      ]);
				string sFIELD_WIDTH       = Sql.ToString (row["FIELD_WIDTH"      ]);
				int    nDATA_COLUMNS      = Sql.ToInteger(row["DATA_COLUMNS"     ]);
				//string sMODULE_TYPE       = Sql.ToString (row["MODULE_TYPE"      ]);
				// 03/12/2011 Paul.  We are not going to be able to support related fields or validation. 
				/*
				string sRELATED_SOURCE_MODULE_NAME   = Sql.ToString (row["RELATED_SOURCE_MODULE_NAME"  ]);
				string sRELATED_SOURCE_VIEW_NAME     = Sql.ToString (row["RELATED_SOURCE_VIEW_NAME"    ]);
				string sRELATED_SOURCE_ID_FIELD      = Sql.ToString (row["RELATED_SOURCE_ID_FIELD"     ]);
				string sRELATED_SOURCE_NAME_FIELD    = Sql.ToString (row["RELATED_SOURCE_NAME_FIELD"   ]);
				string sRELATED_VIEW_NAME            = Sql.ToString (row["RELATED_VIEW_NAME"           ]);
				string sRELATED_ID_FIELD             = Sql.ToString (row["RELATED_ID_FIELD"            ]);
				string sRELATED_NAME_FIELD           = Sql.ToString (row["RELATED_NAME_FIELD"          ]);
				string sRELATED_JOIN_FIELD           = Sql.ToString (row["RELATED_JOIN_FIELD"          ]);
				bool bVALID_RELATED =  !Sql.IsEmptyString(sRELATED_SOURCE_VIEW_NAME) && !Sql.IsEmptyString(sRELATED_SOURCE_ID_FIELD) && !Sql.IsEmptyString(sRELATED_SOURCE_NAME_FIELD) 
				               && !Sql.IsEmptyString(sRELATED_VIEW_NAME       ) && !Sql.IsEmptyString(sRELATED_ID_FIELD       ) && !Sql.IsEmptyString(sRELATED_NAME_FIELD       ) 
				               && !Sql.IsEmptyString(sRELATED_JOIN_FIELD      );
				string sPARENT_FIELD            = Sql.ToString (row["PARENT_FIELD"           ]);
				string sFIELD_VALIDATOR_MESSAGE = Sql.ToString (row["FIELD_VALIDATOR_MESSAGE"]);
				string sVALIDATION_TYPE         = Sql.ToString (row["VALIDATION_TYPE"        ]);
				string sREGULAR_EXPRESSION      = Sql.ToString (row["REGULAR_EXPRESSION"     ]);
				string sDATA_TYPE               = Sql.ToString (row["DATA_TYPE"              ]);
				string sMININUM_VALUE           = Sql.ToString (row["MININUM_VALUE"          ]);
				string sMAXIMUM_VALUE           = Sql.ToString (row["MAXIMUM_VALUE"          ]);
				string sCOMPARE_OPERATOR        = Sql.ToString (row["COMPARE_OPERATOR"       ]);
				string sTOOL_TIP                = Sql.ToString (row["TOOL_TIP"               ]);
				*/
				if ( nDATA_COLUMNS == 0 )
					nDATA_COLUMNS = 2;
				
				bool bIsReadable  = true;
				bool bIsWriteable = true;
				if ( SplendidInit.bEnableACLFieldSecurity )
				{
					Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, sDATA_FIELD, gASSIGNED_USER_ID);
					bIsReadable  = acl.IsReadable();
					bIsWriteable = acl.IsWriteable();
				}
				// 03/12/2011 Paul.  We are not going to support setting the team or the Assigned User ID. 
				if ( Sql.IsEmptyString(sDATA_FIELD) || sDATA_FIELD == "ASSIGNED_TO" || sDATA_FIELD == "ASSIGNED_USER_ID" || sDATA_FIELD == "TEAM_ID" || sDATA_FIELD == "TEAM_SET_NAME" )
				{
					sFIELD_TYPE = "Blank";
					bUI_REQUIRED = false;
				}
				if ( nCOLSPAN >= 0 && nColIndex == 0 )
				{
					if ( nRowIndex > 0 )
						sb.AppendLine("		</tr><tr>");
					nRowIndex++;
				}
				if ( String.Compare(sFIELD_TYPE, "TextBox", true) == 0 )
				{
					sb.Append("			<td");
					sb.Append(" class=\"SplendidDataLabel\"");
					sb.Append(" width=\"" + sLABEL_WIDTH + "\"");
					sb.Append(">");
					if ( bIsWriteable )
						sb.Append(L10n.Term(sDATA_LABEL));
					sb.Append("</td>");
					sb.Append("<td");
					sb.Append(" width=\"" + sFIELD_WIDTH + "\"");
					if ( nCOLSPAN > 0 )
						sb.Append(" colspan=\"" + nCOLSPAN.ToString() + "\"");
					sb.Append(">");
					if ( bIsWriteable )
					{
						if ( nFORMAT_ROWS > 0 && nFORMAT_COLUMNS > 0 )
						{
							sb.AppendLine("<textarea");
							sb.Append(" class=\"SplendidDataField\"");
							sb.Append(" name=\"" + sDATA_FIELD + "\"");
							sb.Append(" rows=\"" + nFORMAT_ROWS.ToString() + "\"");
							sb.Append(" cols=\"" + nFORMAT_COLUMNS.ToString() + "\"");
							sb.Append(">"  + HttpUtility.HtmlEncode(info[sDATA_FIELD]));
							sb.Append("</textarea>");
						}
						else
						{
							sb.Append("<input");
							sb.Append(" name=\"" + sDATA_FIELD + "\"");
							sb.Append(" type=\"text\"");
							sb.Append(" class=\"SplendidDataField\"");
							sb.Append(" maxlength=\"" + nFORMAT_MAX_LENGTH.ToString() + "\"");
							if ( nFORMAT_SIZE > 0 )
								sb.Append(" size=\"" + nFORMAT_SIZE.ToString() + "\"");
							sb.Append(" value=\"" + HttpUtility.HtmlEncode(info[sDATA_FIELD]) + "\"");
							sb.Append(" />");
						}
					}
					sb.AppendLine("</td>");
				}
				else if ( String.Compare(sFIELD_TYPE, "ListBox", true) == 0 )
				{
					sb.Append("			<td");
					sb.Append(" class=\"SplendidDataLabel\"");
					sb.Append(" width=\"" + sLABEL_WIDTH + "\"");
					sb.Append(">");
					if ( bIsWriteable && !Sql.IsEmptyString(sCACHE_NAME) )
						sb.Append(L10n.Term(sDATA_LABEL));
					sb.Append("</td>");
					sb.Append("<td");
					sb.Append(" width=\"" + sFIELD_WIDTH + "\"");
					if ( nCOLSPAN > 0 )
						sb.Append(" colspan=\"" + nCOLSPAN.ToString() + "\"");
					sb.Append(">");
					if ( bIsWriteable && !Sql.IsEmptyString(sCACHE_NAME) )
					{
						sb.Append("				<select");
						sb.Append(" name=\"" + sDATA_FIELD + "\"");
						sb.Append(" class=\"SplendidDataField\"");
						if ( nFORMAT_ROWS > 0 )
							sb.Append(" multiple=\"multiple\"");
						sb.Append(">");
						if ( !bUI_REQUIRED && nFORMAT_ROWS <= 0 )
						{
							sb.Append("					<option");
							sb.Append(" value=\"" + String.Empty + "\"");
							sb.Append(">");
							sb.Append(L10n.Term(".LBL_NONE"));
							sb.AppendLine("</option>");
						}
						bool bCustomCache = false;
						// 10/04/2015 Paul.  Changed custom caches to a dynamic list. 
						System.Collections.Generic.List<SplendidCacheReference> arrCustomCaches = SplendidCache.CustomCaches;
						foreach ( SplendidCacheReference cache in arrCustomCaches )
						{
							if ( cache.Name == sCACHE_NAME )
							{
								SplendidCacheCallback cbkDataSource = cache.DataSource;
								DataTable dtCache = cbkDataSource();
								foreach ( DataRow rowCache in dtCache.Rows )
								{
									string sValue = Sql.ToString(rowCache[cache.DataValueField]);
									string sText  = Sql.ToString(rowCache[cache.DataTextField ]);
									sb.Append("					<option");
									sb.Append(" value=\"" + HttpUtility.HtmlEncode(sValue) + "\"");
									if ( sCACHE_NAME == "Currencies" )
									{
										if ( sValue == gCURRENCY_ID.ToString() )
											sb.Append(" selected=\"selected\"");
									}
									sb.Append(">");
									sb.Append(HttpUtility.HtmlEncode(sText));
									sb.AppendLine("</option>");
								}
								bCustomCache = true;
							}
						}
						if ( !bCustomCache )
						{
							DataTable dtCache = SplendidCache.List(sCACHE_NAME);
							foreach ( DataRow rowCache in dtCache.Rows )
							{
								string sValue = Sql.ToString(rowCache["NAME"        ]);
								string sText  = Sql.ToString(rowCache["DISPLAY_NAME"]);
								sb.Append("					<option");
								sb.Append(" value=\"" + HttpUtility.HtmlEncode(sValue) + "\"");
								sb.Append(">");
								sb.Append(HttpUtility.HtmlEncode(sText));
								sb.AppendLine("</option>");
							}
						}
						sb.Append("/select>");
					}
					sb.AppendLine("</td>");
				}
				// 03/12/2011 Paul.  CheckBoxList is difficult as it is hard to determine if it exists in JavaScript. 
				/*
				else if ( String.Compare(sFIELD_TYPE, "CheckBoxList", true) == 0 )
				{
					sb.Append("			<td");
					sb.Append(" class=\"SplendidDataLabel\"");
					sb.Append(" width=\"" + sLABEL_WIDTH + "\"");
					sb.Append(">");
					if ( bIsWriteable && !Sql.IsEmptyString(sCACHE_NAME) )
						sb.Append(L10n.Term(sDATA_LABEL));
					sb.Append("</td>");
					sb.Append("<td");
					sb.Append(" width=\"" + sFIELD_WIDTH + "\"");
					if ( nCOLSPAN > 0 )
						sb.Append(" colspan=\"" + nCOLSPAN.ToString() + "\"");
					sb.Append(">");
					if ( bIsWriteable && !Sql.IsEmptyString(sCACHE_NAME) )
					{
						if ( nFORMAT_ROWS > 0 )
						{
							sb.Append("				<div");
							sb.Append(" style=\"overflow-y: auto;height: " + nFORMAT_ROWS.ToString() + "px\"");
							sb.Append(">");
						}
						bool bCustomCache = false;
						SplendidCacheReference[] arrCustomCaches = SplendidCache.CustomCaches;
						foreach ( SplendidCacheReference cache in arrCustomCaches )
						{
							if ( cache.Name == sCACHE_NAME )
							{
								SplendidCacheCallback cbkDataSource = cache.DataSource;
								DataTable dtCache = cbkDataSource();
								foreach ( DataRow rowCache in dtCache.Rows )
								{
									string sValue = Sql.ToString(rowCache[cache.DataValueField]);
									string sText  = Sql.ToString(rowCache[cache.DataTextField ]);
									sb.Append("					<input");
									sb.Append(" name=\"" + sDATA_FIELD + "\"");
									sb.Append(" type=\"checkbox\"");
									sb.Append(" value=\"" + HttpUtility.HtmlEncode(sValue) + "\"");
									sb.Append(" /> ");
									sb.Append(HttpUtility.HtmlEncode(sText));
									sb.AppendLine("<br />");
								}
								bCustomCache = true;
							}
						}
						if ( !bCustomCache )
						{
							DataTable dtCache = SplendidCache.List(sCACHE_NAME);
							foreach ( DataRow rowCache in dtCache.Rows )
							{
								string sValue = Sql.ToString(rowCache["NAME"        ]);
								string sText  = Sql.ToString(rowCache["DISPLAY_NAME"]);
								sb.Append("					<input");
								sb.Append(" name=\"" + sDATA_FIELD + "\"");
								sb.Append(" type=\"checkbox\"");
								sb.Append(" value=\"" + HttpUtility.HtmlEncode(sValue) + "\"");
								sb.Append(" /> ");
								sb.Append(HttpUtility.HtmlEncode(sText));
								sb.AppendLine("<br />");
							}
						}
						if ( nFORMAT_ROWS > 0 )
							sb.Append("/div>");
					}
					sb.AppendLine("</td>");
				}
				*/
				else if ( String.Compare(sFIELD_TYPE, "Radio", true) == 0 )
				{
					sb.Append("			<td");
					sb.Append(" class=\"SplendidDataLabel\"");
					sb.Append(" width=\"" + sLABEL_WIDTH + "\"");
					sb.Append(">");
					if ( bIsWriteable && !Sql.IsEmptyString(sCACHE_NAME) )
						sb.Append(L10n.Term(sDATA_LABEL));
					sb.Append("</td>");
					sb.Append("<td");
					sb.Append(" width=\"" + sFIELD_WIDTH + "\"");
					if ( nCOLSPAN > 0 )
						sb.Append(" colspan=\"" + nCOLSPAN.ToString() + "\"");
					sb.Append(">");
					if ( bIsWriteable && !Sql.IsEmptyString(sCACHE_NAME) )
					{
						if ( nFORMAT_ROWS > 0 )
						{
							sb.Append("				<div");
							sb.Append(" style=\"overflow-y: auto;height: " + nFORMAT_ROWS.ToString() + "px\"");
							sb.Append(">");
						}
						if ( !bUI_REQUIRED )
						{
							sb.Append("					<input");
							sb.Append(" name=\"" + sDATA_FIELD + "\"");
							sb.Append(" type=\"radio\"");
							sb.Append(" value=\"" + String.Empty + "\"");
							sb.Append(" /> ");
							sb.Append(L10n.Term(".LBL_NONE"));
							sb.AppendLine("<br />");
						}
						bool bCustomCache = false;
						// 10/04/2015 Paul.  Changed custom caches to a dynamic list. 
						System.Collections.Generic.List<SplendidCacheReference> arrCustomCaches = SplendidCache.CustomCaches;
						foreach ( SplendidCacheReference cache in arrCustomCaches )
						{
							if ( cache.Name == sCACHE_NAME )
							{
								SplendidCacheCallback cbkDataSource = cache.DataSource;
								DataTable dtCache = cbkDataSource();
								foreach ( DataRow rowCache in dtCache.Rows )
								{
									string sValue = Sql.ToString(rowCache[cache.DataValueField]);
									string sText  = Sql.ToString(rowCache[cache.DataTextField ]);
									sb.Append("					<input");
									sb.Append(" name=\"" + sDATA_FIELD + "\"");
									sb.Append(" type=\"radio\"");
									sb.Append(" value=\"" + HttpUtility.HtmlEncode(sValue) + "\"");
									sb.Append(" /> ");
									sb.Append(HttpUtility.HtmlEncode(sText));
									sb.AppendLine("<br />");
								}
								bCustomCache = true;
							}
						}
						if ( !bCustomCache )
						{
							DataTable dtCache = SplendidCache.List(sCACHE_NAME);
							foreach ( DataRow rowCache in dtCache.Rows )
							{
								string sValue = Sql.ToString(rowCache["NAME"        ]);
								string sText  = Sql.ToString(rowCache["DISPLAY_NAME"]);
								sb.Append("					<input");
								sb.Append(" name=\"" + sDATA_FIELD + "\"");
								sb.Append(" type=\"radio\"");
								sb.Append(" value=\"" + HttpUtility.HtmlEncode(sValue) + "\"");
								sb.Append(" /> ");
								sb.Append(HttpUtility.HtmlEncode(sText));
								sb.AppendLine("<br />");
							}
						}
						if ( nFORMAT_ROWS > 0 )
							sb.Append("/div>");
					}
					sb.AppendLine("</td>");
				}
				else if ( String.Compare(sFIELD_TYPE, "CheckBox", true) == 0 )
				{
					sb.Append("			<td");
					sb.Append(" class=\"SplendidDataLabel\"");
					sb.Append(" width=\"" + sLABEL_WIDTH + "\"");
					sb.Append(">");
					if ( bIsWriteable )
						sb.Append(L10n.Term(sDATA_LABEL));
					sb.Append("</td>");
					sb.Append("<td");
					sb.Append(" width=\"" + sFIELD_WIDTH + "\"");
					if ( nCOLSPAN > 0 )
						sb.Append(" colspan=\"" + nCOLSPAN.ToString() + "\"");
					sb.Append(">");
					if ( bIsWriteable )
					{
						sb.Append("<input");
						sb.Append(" name=\"" + sDATA_FIELD + "\"");
						sb.Append(" type=\"checkbox\"");
						sb.Append(" value=\"1\"");
						sb.Append(" />");
					}
					sb.AppendLine("</td>");
				}
				// 03/12/2011 Paul.  Unsupported fields will be blank. 
				else //if ( String.Compare(sFIELD_TYPE, "Blank", true) == 0 )
				{
					sb.Append("			<td");
					sb.Append(" class=\"SplendidDataLabel\"");
					sb.Append(" width=\"" + sLABEL_WIDTH + "\"");
					sb.Append(">");
					sb.Append("</td>");
					sb.Append("<td");
					sb.Append(" width=\"" + sFIELD_WIDTH + "\"");
					if ( nCOLSPAN > 0 )
						sb.Append(" colspan=\"" + nCOLSPAN.ToString() + "\"");
					sb.Append(">");
					sb.AppendLine("</td>");
				}
				if ( nCOLSPAN > 0 )
					nColIndex += nCOLSPAN;
				else if ( nCOLSPAN == 0 )
					nColIndex++;
				if ( nColIndex >= nDATA_COLUMNS )
					nColIndex = 0;
			}
			sb.AppendLine("		</tr>");
			sb.AppendLine("	</table>");
		}

		public static void SetParameterWithACLFieldSecurity(IDbCommand cmdUpdate, string sMODULE, string sNAME, string sVALUE, Guid gASSIGNED_USER_ID)
		{
			bool bIsWriteable = true;
			if ( SplendidInit.bEnableACLFieldSecurity )
			{
				Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE, sNAME, gASSIGNED_USER_ID);
				bIsWriteable = acl.IsWriteable();
			}
			if ( bIsWriteable )
			{
				Sql.SetParameter(cmdUpdate, "@" + sNAME, sVALUE);
			}
		}

	}
}
