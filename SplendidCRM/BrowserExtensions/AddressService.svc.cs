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
using System.Web;
using System.Web.SessionState;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Web.Script.Serialization;

namespace SplendidCRM.BrowserExtensions
{
	public class CreateRecordResponse
	{
		public string ID  ;
		public string URL ;
		public string NAME;

		public CreateRecordResponse()
		{
			ID   = String.Empty;
			URL  = String.Empty;
			NAME = String.Empty;
		}
	}

	[ServiceContract]
	[ServiceBehavior(IncludeExceptionDetailInFaults=true)]
	[AspNetCompatibilityRequirements(RequirementsMode=AspNetCompatibilityRequirementsMode.Required)]
	public class AddressService
	{
		private static string ConvertToRegexMatch(string sRegex)
		{
			if ( sRegex.StartsWith("^") && sRegex.EndsWith("$") )
				sRegex = "(" + sRegex.Substring(1, sRegex.Length - 2) + ")";
			return sRegex;
		}

		[OperationContract]
		// 02/26/2011 Paul.  We can support only one HTTP method, so lets use POST so that the size of the data is not limited by the maximum in a URL. 
		//[WebGet(ResponseFormat=WebMessageFormat.Json)]
		public AddressDetails ParseAddress(string Module, string Website, string Address)
		{
			// 03/05/2011 Paul.  We can skip the authentication during testing as there is no real database actions, just public Google address processing. 
			//if ( !Security.IsAuthenticated() )
			//	throw(new Exception("Authentication required"));

			// 02/28/2011 Paul.  Chrome sends just \n, Firefox sends \r\n.  Normal so that sLabelRegex will work. 
			Address = Address.Replace("\r\n", "\n");
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpSessionState     Session     = HttpContext.Current.Session    ;

			string sEmailAddressRegex = Sql.ToString(Application["FIELD_VALIDATORS.Email Address"]);
			// 04/16/2011 Paul.  We are having a problem with the street number being treated as a phone numbers. 
			// http://blog.stevenlevithan.com/archives/validate-phone-number
			//string sPhoneNumberRegex  = Sql.ToString(Application["FIELD_VALIDATORS.Phone Number" ]);
			string sPhoneNumberRegex  = "\\(?\\b([0-9]{3})\\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})";
			string sPhoneNumberIntl   = "\\+(?:[0-9] ?){6,14}[0-9]";
			string sUrlRegex          = Sql.ToString(Application["FIELD_VALIDATORS.URL"          ]);
			string sLabelRegex        = "(\n[a-zA-Z]+\\:\\s\n)|(\n[a-zA-Z]+\\:\\s$)";  // Filter Phone: or Email: .
			//sPhoneNumberRegex  = ConvertToRegexMatch(sPhoneNumberRegex );
			sEmailAddressRegex = ConvertToRegexMatch(sEmailAddressRegex);
			sUrlRegex          = ConvertToRegexMatch(sUrlRegex         );
			// 02/26/2011 Paul.  Don't allow alphas in a phone number as it can take the postal address. 
			sPhoneNumberRegex  = sPhoneNumberRegex.Replace("a-zA-Z", String.Empty);
			
			AddressDetails info = new AddressDetails();
			info.SplendidCRM_URL = HttpContext.Current.Request.Url.AbsoluteUri.Replace("AddressService.svc/ParseAddress", "");
			//Address = "Google, Inc., 1600 Amphitheatre Parkway, Mountain View, CA, (800) 555-1212, 800-555-1212, http://www.google.com, support@google.com, support@Gmail.com";
			if ( !Sql.IsEmptyString(Website) )
				info.WEBSITE = Website;
			if ( !Sql.IsEmptyString(Address) )
			{
				if ( !Sql.IsEmptyString(sUrlRegex) )
				{
					MatchCollection matches = Regex.Matches(Address, sUrlRegex);
					if ( matches.Count > 0 )
					{
						info.WEBSITE = matches[0].Value;
						if ( !Sql.IsEmptyString(info.WEBSITE) )
						{
							Address = Address.Replace(info.WEBSITE, String.Empty);
						}
					}
				}
				if ( !Sql.IsEmptyString(sEmailAddressRegex) )
				{
					MatchCollection matches = Regex.Matches(Address, sEmailAddressRegex);
					if ( matches.Count > 0 )
					{
						info.EMAIL1 = matches[0].Value;
						if ( !Sql.IsEmptyString(info.EMAIL1) )
						{
							Address = Address.Replace(info.EMAIL1, String.Empty);
							if ( matches.Count > 1 )
							{
								info.EMAIL2 = matches[1].Value;
								if ( !Sql.IsEmptyString(info.EMAIL2) )
									Address = Address.Replace(info.EMAIL2, String.Empty);
							}
						}
					}
				}
				int nPhoneNumbers = 0;
				if ( !Sql.IsEmptyString(sPhoneNumberRegex) )
				{
					MatchCollection matches = Regex.Matches(Address, sPhoneNumberRegex);
					if ( matches.Count > 0 )
					{
						info.PHONE1 = matches[0].Value;
						if ( !Sql.IsEmptyString(info.PHONE1) )
						{
							Address = Address.Replace(info.PHONE1, String.Empty);
							if ( matches.Count > 1 )
							{
								info.PHONE2 = matches[1].Value;
								if ( !Sql.IsEmptyString(info.PHONE2) )
									Address = Address.Replace(info.PHONE2, String.Empty);
							}
						}
					}
				}
				if ( nPhoneNumbers < 1 )
				{
					if ( !Sql.IsEmptyString(sPhoneNumberIntl) )
					{
						MatchCollection matches = Regex.Matches(Address, sPhoneNumberIntl);
						if ( matches.Count > 0 )
						{
							info.PHONE1 = matches[0].Value;
							if ( !Sql.IsEmptyString(info.PHONE1) )
							{
								Address = Address.Replace(info.PHONE1, String.Empty);
								if ( matches.Count > 1 )
								{
									info.PHONE2 = matches[1].Value;
									if ( !Sql.IsEmptyString(info.PHONE2) )
										Address = Address.Replace(info.PHONE2, String.Empty);
								}
							}
						}
					}
				}
				// 02/26/2011 Paul.  After extracting phone numbers and emails, lets try and remove the labels as the cause Google Maps to fail. 
				if ( !Sql.IsEmptyString(sLabelRegex) )
				{
					MatchCollection matches = Regex.Matches(Address, sLabelRegex);
					foreach ( Match match in matches )
					{
						if ( !Sql.IsEmptyString(match.Value) )
							Address = Address.Replace(match.Value, String.Empty);
					}
				}

				// 08/26/2011 Paul.  Geocoding API V3 does not require a key. 
				//string sGoogleMapsKey = Sql.ToString(Application["CONFIG.GoogleMaps.Key"]);
				//if ( !Sql.IsEmptyString(sGoogleMapsKey) )
				{
					bool bShortStateName   = Sql.ToBoolean(Application["CONFIG.GoogleMaps.ShortStateName"  ]);
					bool bShortCountryName = Sql.ToBoolean(Application["CONFIG.GoogleMaps.ShortCountryName"]);
					GoogleUtils.ConvertAddressV3(Address, bShortStateName, bShortCountryName, ref info);
				}
				string sFirstAddressToken = String.Empty;
				// 02/26/2011 Paul.  The street may not be identical, so just use the first token of the street address. 
				if      ( !Sql.IsEmptyString(info.ADDRESS_STREET    ) ) sFirstAddressToken = info.ADDRESS_STREET    .Split(' ')[0];
				else if ( !Sql.IsEmptyString(info.ADDRESS_CITY      ) ) sFirstAddressToken = info.ADDRESS_CITY      ;
				else if ( !Sql.IsEmptyString(info.ADDRESS_STATE     ) ) sFirstAddressToken = info.ADDRESS_STATE     ;
				else if ( !Sql.IsEmptyString(info.ADDRESS_POSTALCODE) ) sFirstAddressToken = info.ADDRESS_POSTALCODE;
				else if ( !Sql.IsEmptyString(info.ADDRESS_COUNTRY   ) ) sFirstAddressToken = info.ADDRESS_COUNTRY   ;
				else if ( !Sql.IsEmptyString(info.PHONE1            ) ) sFirstAddressToken = info.PHONE1            ;
				else if ( !Sql.IsEmptyString(info.PHONE2            ) ) sFirstAddressToken = info.PHONE2            ;
				else if ( !Sql.IsEmptyString(info.EMAIL1            ) ) sFirstAddressToken = info.EMAIL1            ;
				else if ( !Sql.IsEmptyString(info.EMAIL2            ) ) sFirstAddressToken = info.EMAIL2            ;
				if ( Address.IndexOf(sFirstAddressToken) > 0 )
				{
					info.NAME = Address.Substring(0, Address.IndexOf(sFirstAddressToken));
					info.NAME = info.NAME.Trim();
					if ( info.NAME.EndsWith(",") )
						info.NAME = info.NAME.Substring(0, info.NAME.Length - 1);
					string[] arrName = info.NAME.Split(' ');
					if ( arrName.Length > 0 )
					{
						if ( arrName.Length == 1 )
						{
							info.FIRST_NAME = String.Empty;
							info.LAST_NAME  = arrName[0];
						}
						else
						{
							info.FIRST_NAME = arrName[0];
							info.LAST_NAME  = arrName[1];
							if ( info.FIRST_NAME.EndsWith(",") )
								info.FIRST_NAME = info.FIRST_NAME.Substring(0, info.FIRST_NAME.Length - 1);
						}
					}
				}
			}
			L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));

			StringBuilder sb = new StringBuilder();
			sb.AppendLine("<form name=\"frmSplendidCRM\" id=\"frmSplendidCRM\" onsubmit=\"return false;\">");
			sb.AppendLine("	<table id=\"tblSplendidButtons\">");
			sb.AppendLine("		<tr>");
			sb.AppendLine("			<td width=\"99%\">");
			if ( SplendidCRM.Security.GetUserAccess(Module, "edit") >= 0 )
			{
				string sSINGULAR_NAME = Module;
				if ( sSINGULAR_NAME.EndsWith("ies") )
					sSINGULAR_NAME = sSINGULAR_NAME.Substring(0, sSINGULAR_NAME.Length - 3) + "y";
				else if ( sSINGULAR_NAME.EndsWith("s") )
					sSINGULAR_NAME = sSINGULAR_NAME.Substring(0, sSINGULAR_NAME.Length - 1);
				sb.AppendLine("				<input type=\"button\" value=\"  " + L10n.Term(Module + ".LNK_NEW_" + sSINGULAR_NAME.ToUpper()) + "  \" onclick=\"var splendidCreateEvent = document.createEvent('Event'); splendidCreateEvent.initEvent('SplendidCreateEvent', true, true); document.getElementById('divSplendidCRMExtension').dispatchEvent(splendidCreateEvent); return false;\" /> ");
			}
			sb.AppendLine("				<input type=\"button\" value=\"  " + L10n.Term(".LBL_CANCEL_BUTTON_LABEL") + "  \" onclick=\"var splendidCancelEvent  = document.createEvent('Event'); splendidCancelEvent.initEvent ('SplendidCancelEvent' , true, true); document.getElementById('divSplendidCRMExtension').dispatchEvent(splendidCancelEvent ); return false;\" />&nbsp;");
			sb.AppendLine("				<input name=\"SplendidModule\" type=\"hidden\" value=\"" + Module + "\" />");
			sb.AppendLine("				<span id='spnSplendidCRMExtensionStatus' class=\"SplendidDataLabel\">" + info.LocationStatus + "</span>");
			sb.AppendLine("			</td>");
			sb.AppendLine("			<td width=\"1%\">");
			sb.AppendLine("				<img id='imgSplendidCRMExtensionLogo' src='SplendidCRM-40.png' width='40' height='40' />");
			sb.AppendLine("			</td>");
			sb.AppendLine("		</tr>");
			sb.AppendLine("	</table>");

			/*
			sb.AppendLine("	<table border=\"0\" cellpadding=\"3\" cellspacing=\"1\" width=\"100%\" style=\"padding-left: 10px; padding-right: 10px;\">");
			sb.AppendLine("		<tr>");
			sb.AppendLine("			<td class=\"SplendidDataLabel\" width=\"15%\">" + L10n.Term("Contacts.LBL_ACCOUNT_NAME"   ) + "</td><td width=\"85%\" colspan=\"3\"><input    name=\"NAME\"               class=\"SplendidDataField\" value=\"" + HttpUtility.HtmlEncode(info.NAME              ) + "\" /></td>");
			sb.AppendLine("		</tr><tr>");
			sb.AppendLine("			<td class=\"SplendidDataLabel\" width=\"15%\">" + L10n.Term("Contacts.LBL_FIRST_NAME"     ) + "</td><td width=\"35%\"              ><input    name=\"FIRST_NAME\"         class=\"SplendidDataField\" value=\"" + HttpUtility.HtmlEncode(info.FIRST_NAME        ) + "\" /></td>");
			sb.AppendLine("			<td class=\"SplendidDataLabel\" width=\"15%\">" + L10n.Term("Contacts.LBL_LAST_NAME"      ) + "</td><td width=\"35%\"              ><input    name=\"LAST_NAME\"          class=\"SplendidDataField\" value=\"" + HttpUtility.HtmlEncode(info.LAST_NAME         ) + "\" /></td>");
			sb.AppendLine("		</tr><tr>");
			sb.AppendLine("			<td class=\"SplendidDataLabel\" width=\"15%\">" + L10n.Term("Contacts.LBL_PRIMARY_ADDRESS") + "</td><td width=\"85%\" colspan=\"3\"><textarea class=\"SplendidDataField\" name=\"ADDRESS_STREET\" rows=\"2\">"  + HttpUtility.HtmlEncode(info.ADDRESS_STREET    ) + "</textarea></td>");
			sb.AppendLine("		</tr><tr>");
			sb.AppendLine("			<td class=\"SplendidDataLabel\" width=\"15%\">" + L10n.Term("Contacts.LBL_CITY"           ) + "</td><td width=\"35%\"              ><input    name=\"ADDRESS_CITY\"       class=\"SplendidDataField\" value=\"" + HttpUtility.HtmlEncode(info.ADDRESS_CITY      ) + "\" /></td>");
			sb.AppendLine("			<td class=\"SplendidDataLabel\" width=\"15%\">" + L10n.Term("Contacts.LBL_STATE"          ) + "</td><td width=\"35%\"              ><input    name=\"ADDRESS_STATE\"      class=\"SplendidDataField\" value=\"" + HttpUtility.HtmlEncode(info.ADDRESS_STATE     ) + "\" /></td>");
			sb.AppendLine("		</tr><tr>");
			sb.AppendLine("			<td class=\"SplendidDataLabel\" width=\"15%\">" + L10n.Term("Contacts.LBL_POSTAL_CODE"    ) + "</td><td width=\"35%\"              ><input    name=\"ADDRESS_POSTALCODE\" class=\"SplendidDataField\" value=\"" + HttpUtility.HtmlEncode(info.ADDRESS_POSTALCODE) + "\" /></td>");
			sb.AppendLine("			<td class=\"SplendidDataLabel\" width=\"15%\">" + L10n.Term("Contacts.LBL_COUNTRY"        ) + "</td><td width=\"35%\"              ><input    name=\"ADDRESS_COUNTRY\"    class=\"SplendidDataField\" value=\"" + HttpUtility.HtmlEncode(info.ADDRESS_COUNTRY   ) + "\" /></td>");
			sb.AppendLine("		</tr><tr>");
			sb.AppendLine("			<td class=\"SplendidDataLabel\" width=\"15%\">" + L10n.Term("Contacts.LBL_EMAIL1"         ) + "</td><td width=\"35%\"              ><input    name=\"EMAIL1\"             class=\"SplendidDataField\" value=\"" + HttpUtility.HtmlEncode(info.EMAIL1            ) + "\" /></td>");
			sb.AppendLine("			<td class=\"SplendidDataLabel\" width=\"15%\">" + L10n.Term("Contacts.LBL_EMAIL2"         ) + "</td><td width=\"35%\"              ><input    name=\"EMAIL2\"             class=\"SplendidDataField\" value=\"" + HttpUtility.HtmlEncode(info.EMAIL2            ) + "\" /></td>");
			sb.AppendLine("		</tr><tr>");
			sb.AppendLine("			<td class=\"SplendidDataLabel\" width=\"15%\">" + L10n.Term("Contacts.LBL_OFFICE_PHONE"   ) + "</td><td width=\"35%\"              ><input    name=\"PHONE1\"             class=\"SplendidDataField\" value=\"" + HttpUtility.HtmlEncode(info.PHONE1            ) + "\" /></td>");
			sb.AppendLine("			<td class=\"SplendidDataLabel\" width=\"15%\">" + L10n.Term("Contacts.LBL_FAX_PHONE"      ) + "</td><td width=\"35%\"              ><input    name=\"PHONE2\"             class=\"SplendidDataField\" value=\"" + HttpUtility.HtmlEncode(info.PHONE2            ) + "\" /></td>");
			sb.AppendLine("		</tr><tr>");
			sb.AppendLine("			<td class=\"SplendidDataLabel\" width=\"15%\">" + L10n.Term("Accounts.LBL_WEBSITE"        ) + "</td><td width=\"85%\" colspan=\"3\"><input    name=\"WEBSITE\"            class=\"SplendidDataField\" value=\"" + HttpUtility.HtmlEncode(info.WEBSITE           ) + "\" /></td>");
			sb.AppendLine("		</tr>");
			sb.AppendLine("	</table>");
			*/
			
			if ( Sql.IsEmptyString(Module) )
				Module = "Contacts";
			ExtensionUtils.AppendEditViewFields(Module, info, sb);
			sb.AppendLine("</form>");
			info.EDIT_VIEW = sb.ToString();
			return info;
		}

		[OperationContract]
		// 03/13/2011 Paul.  Must use octet-stream instead of json, outherwise we get the following error. 
		// Incoming message for operation 'CreateRecord' (contract 'AddressService' with namespace 'http://tempuri.org/') contains an unrecognized http body format value 'Json'. 
		// The expected body format value is 'Raw'. This can be because a WebContentTypeMapper has not been configured on the binding. See the documentation of WebContentTypeMapper for more details.
		//xhr.setRequestHeader('content-type', 'application/octet-stream');
		public CreateRecordResponse CreateRecord(Stream input)
		{
			string sRequest = String.Empty;
			using ( StreamReader stmRequest = new StreamReader(input, System.Text.Encoding.UTF8) )
			{
				sRequest = stmRequest.ReadToEnd();
			}
			// http://weblogs.asp.net/hajan/archive/2010/07/23/javascriptserializer-dictionary-to-json-serialization-and-deserialization.aspx
			JavaScriptSerializer json = new JavaScriptSerializer();
			Dictionary<string, object> dict = json.Deserialize<Dictionary<string, object>>(sRequest);

			string Module = Sql.ToString(dict["Module"]);
			Dictionary<string, object> info = dict["info"] as Dictionary<string, object>;
			if ( Sql.IsEmptyString(Module) )
			{
				throw(new Exception("The module name must be specified."));
			}
			CreateRecordResponse result = new CreateRecordResponse();
			if ( Security.IsAuthenticated() )
			{
				int nACLACCESS = Security.GetUserAccess(Module, "edit");
				if ( nACLACCESS < 0 )
				{
					L10N L10n = new L10N("en-US");
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
				}

				HttpApplicationState Application = HttpContext.Current.Application;
				string sTABLE_NAME = Crm.Modules.TableName(Application, Module);
				DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					IDbCommand cmdUpdate = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Update");
					IDbDataParameter parID = Sql.FindParameter(cmdUpdate, "@ID");
					bool bEnableTeamManagement  = Crm.Config.enable_team_management();
					bool bRequireTeamManagement = Crm.Config.require_team_management();
					bool bRequireUserAssignment = Crm.Config.require_user_assignment();
					foreach(IDbDataParameter par in cmdUpdate.Parameters)
					{
						string sParameterName = Sql.ExtractDbName(cmdUpdate, par.ParameterName).ToUpper();
						if ( sParameterName == "TEAM_ID" && bEnableTeamManagement ) // 02/26/2011 Paul.  Ignore the Required flag. && bRequireTeamManagement )
							par.Value = Sql.ToDBGuid(Security.TEAM_ID);  // 02/26/2011 Paul.  Make sure to convert Guid.Empty to DBNull. 
						else if ( sParameterName == "ASSIGNED_USER_ID" ) // 02/26/2011 Paul.  Always set the Assigned User ID. && bRequireUserAssignment )
							par.Value = Sql.ToDBGuid(Security.USER_ID);  // 02/26/2011 Paul.  Make sure to convert Guid.Empty to DBNull. 
						// 02/20/2013 Paul.  We need to set the MODIFIED_USER_ID. 
						else if ( sParameterName == "MODIFIED_USER_ID" )
							par.Value = Sql.ToDBGuid(Security.USER_ID);
						else
							par.Value = DBNull.Value;
					}
					Guid gASSIGNED_USER_ID = Security.USER_ID;
					Sql.SetParameter(cmdUpdate, "@MODIFIED_USER_ID", Security.USER_ID);
					foreach ( string sFieldName in info.Keys )
					{
						if ( info[sFieldName] is ArrayList )
						{
							ArrayList lst = info[sFieldName] as ArrayList;
							XmlDocument xml = new XmlDocument();
							xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
							xml.AppendChild(xml.CreateElement("Values"));
							foreach(string item in lst)
							{
								XmlNode xValue = xml.CreateElement("Value");
								xml.DocumentElement.AppendChild(xValue);
								xValue.InnerText = item;
							}
							string sVALUE = xml.OuterXml;
							ExtensionUtils.SetParameterWithACLFieldSecurity(cmdUpdate, Module, sFieldName, sVALUE, gASSIGNED_USER_ID);
						}
						else
						{
							ExtensionUtils.SetParameterWithACLFieldSecurity(cmdUpdate, Module, sFieldName, Sql.ToString(info[sFieldName]), gASSIGNED_USER_ID);
						}
					}
					cmdUpdate.ExecuteNonQuery();
					if ( parID != null )
					{
						string sSiteURL = Crm.Config.SiteURL(Application);
						if ( HttpContext.Current.Request.IsSecureConnection )
							sSiteURL = sSiteURL.Replace("http://", "https://");
						
						result.ID   = Sql.ToString(parID.Value);
						result.URL  = sSiteURL + Module + "/view.aspx?ID=" + result.ID;
						result.NAME = Crm.Modules.ItemName(Application, Module, Sql.ToGuid(parID.Value));
					}
				}
			}
			else
			{
				throw(new Exception("The session ID is invalid."));
			}
			return result;
		}
	}
}
