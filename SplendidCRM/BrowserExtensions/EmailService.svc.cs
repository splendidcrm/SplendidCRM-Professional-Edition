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
using System.Diagnostics;

using MimeKit;

namespace SplendidCRM.BrowserExtensions
{
	public class EmailDetails : ExtensionDetails
	{
		public string   NAME            ;
		public DateTime DATE_TIME       ;
		public string   PARENT_TYPE     ;
		public Guid     PARENT_ID       ;
		public string   DESCRIPTION     ;
		public string   FROM_ADDR       ;
		public string   FROM_NAME       ;
		public string   TO_ADDRS        ;
		public string   CC_ADDRS        ;
		public string   BCC_ADDRS       ;
		public string   TYPE            ;
		public string   MESSAGE_ID      ;
		public string   REPLY_TO_NAME   ;
		public string   REPLY_TO_ADDR   ;
		public string   INTENT          ;
		public Guid     MAILBOX_ID      ;
		public Guid     TEAM_ID         ;
		public string   TEAM_SET_LIST   ;
		public string   HEADERS         ;
		public string   EDIT_VIEW       ;
		public string   SplendidCRM_URL ;

		public EmailDetails()
		{
			NAME             = String.Empty;
			DATE_TIME        = DateTime.MinValue;
			PARENT_TYPE      = String.Empty;
			PARENT_ID        = Guid.Empty  ;
			DESCRIPTION      = String.Empty;
			FROM_ADDR        = String.Empty;
			FROM_NAME        = String.Empty;
			TO_ADDRS         = String.Empty;
			CC_ADDRS         = String.Empty;
			BCC_ADDRS        = String.Empty;
			TYPE             = String.Empty;
			MESSAGE_ID       = String.Empty;
			REPLY_TO_NAME    = String.Empty;
			REPLY_TO_ADDR    = String.Empty;
			INTENT           = String.Empty;
			MAILBOX_ID       = Guid.Empty  ;
			TEAM_ID          = Guid.Empty  ;
			TEAM_SET_LIST    = String.Empty;
			HEADERS          = String.Empty;
			EDIT_VIEW        = String.Empty;
			SplendidCRM_URL  = String.Empty;
		}

		public override string this[string sFieldName]
		{
			get
			{
				string sValue = String.Empty;
				switch ( sFieldName.ToUpper() )
				{
					case "NAME"            :  sValue = this.NAME            ;  break;
					case "DATE_TIME"       :  sValue = this.DATE_TIME       .ToString();  break;
					case "PARENT_TYPE"     :  sValue = this.PARENT_TYPE     ;  break;
					case "PARENT_ID"       :  sValue = this.PARENT_ID       .ToString();  break;
					case "DESCRIPTION"     :  sValue = this.DESCRIPTION     ;  break;
					case "FROM_ADDR"       :  sValue = this.FROM_ADDR       ;  break;
					case "FROM_NAME"       :  sValue = this.FROM_NAME       ;  break;
					case "TO_ADDRS"        :  sValue = this.TO_ADDRS        ;  break;
					case "CC_ADDRS"        :  sValue = this.CC_ADDRS        ;  break;
					case "BCC_ADDRS"       :  sValue = this.BCC_ADDRS       ;  break;
					case "TYPE"            :  sValue = this.TYPE            ;  break;
					case "MESSAGE_ID"      :  sValue = this.MESSAGE_ID      ;  break;
					case "REPLY_TO_NAME"   :  sValue = this.REPLY_TO_NAME   ;  break;
					case "REPLY_TO_ADDR"   :  sValue = this.REPLY_TO_ADDR   ;  break;
					case "INTENT"          :  sValue = this.INTENT          ;  break;
					case "MAILBOX_ID"      :  sValue = this.MAILBOX_ID      .ToString();  break;
					case "TEAM_ID"         :  sValue = this.TEAM_ID         .ToString();  break;
					case "TEAM_SET_LIST"   :  sValue = this.TEAM_SET_LIST   ;  break;
					case "HEADERS"         :  sValue = this.HEADERS         ;  break;
				}
				return sValue;
			}
			set
			{
				switch ( sFieldName.ToUpper() )
				{
					case "NAME"            :  this.NAME             = value;  break;
					case "DATE_TIME"       :  this.DATE_TIME        = Sql.ToDateTime(value);  break;
					case "PARENT_TYPE"     :  this.PARENT_TYPE      = value;  break;
					case "PARENT_ID"       :  this.PARENT_ID        = Sql.ToGuid(value);  break;
					case "DESCRIPTION"     :  this.DESCRIPTION      = value;  break;
					case "FROM_ADDR"       :  this.FROM_ADDR        = value;  break;
					case "FROM_NAME"       :  this.FROM_NAME        = value;  break;
					case "TO_ADDRS"        :  this.TO_ADDRS         = value;  break;
					case "CC_ADDRS"        :  this.CC_ADDRS         = value;  break;
					case "BCC_ADDRS"       :  this.BCC_ADDRS        = value;  break;
					case "TYPE"            :  this.TYPE             = value;  break;
					case "MESSAGE_ID"      :  this.MESSAGE_ID       = value;  break;
					case "REPLY_TO_NAME"   :  this.REPLY_TO_NAME    = value;  break;
					case "REPLY_TO_ADDR"   :  this.REPLY_TO_ADDR    = value;  break;
					case "INTENT"          :  this.INTENT           = value;  break;
					case "MAILBOX_ID"      :  this.MAILBOX_ID       = Sql.ToGuid(value);  break;
					case "TEAM_ID"         :  this.TEAM_ID          = Sql.ToGuid(value);  break;
					case "TEAM_SET_LIST"   :  this.TEAM_SET_LIST    = value;  break;
					case "HEADERS"         :  this.HEADERS          = value;  break;
				}
			}
		}
	}

	public class ArchiveEmailResponse
	{
		public string ID  ;
		public string NAME;

		public ArchiveEmailResponse()
		{
			ID   = String.Empty;
			NAME = String.Empty;
		}
	}

	[ServiceContract]
	[ServiceBehavior(IncludeExceptionDetailInFaults=true)]
	[AspNetCompatibilityRequirements(RequirementsMode=AspNetCompatibilityRequirementsMode.Required)]
	public class EmailService
	{
		[OperationContract]
		// 02/26/2011 Paul.  We can support only one HTTP method, so lets use POST so that the size of the data is not limited by the maximum in a URL. 
		//[WebGet(ResponseFormat=WebMessageFormat.Json)]
		public EmailDetails ParseEmail(string EmailHeaders)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpSessionState     Session     = HttpContext.Current.Session    ;

			EmailDetails info = new EmailDetails();
			System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
			EmailHeaders = EmailHeaders.TrimStart();
			using ( MemoryStream mem = new MemoryStream(encoding.GetBytes(EmailHeaders)) )
			{
				//Pop3.RxMailMessage mm = new Pop3.RxMailMessage();
				//Pop3.Pop3MimeClient pop3Embedded = new Pop3.Pop3MimeClient(mem);
				//pop3Embedded.ProcessEmbedded(ref mm);
				//info.NAME       = mm.Subject;
				//info.DATE_TIME  = mm.DeliveryDate;
				//info.MESSAGE_ID = mm.MessageId;
				//info.FROM_ADDR  = (mm.From != null) ? mm.From.ToString() : String.Empty;
				//info.TO_ADDRS   = (mm.To   != null) ? mm.To  .ToString() : String.Empty;
				//info.CC_ADDRS   = (mm.CC   != null) ? mm.CC  .ToString() : String.Empty;
				//info.HEADERS    = EmailHeaders;
				
				// 01/21/2017 Paul.  Convert to MimeKit. 
				MimeMessage mm = MimeMessage.Load(mem);
				info.NAME       = mm.Subject;
				info.DATE_TIME  = mm.Date.DateTime;
				info.MESSAGE_ID = mm.MessageId;
				info.FROM_ADDR  = (mm.From != null) ? mm.From.ToString() : String.Empty;
				info.TO_ADDRS   = (mm.To   != null) ? mm.To  .ToString() : String.Empty;
				info.CC_ADDRS   = (mm.Cc   != null) ? mm.Cc  .ToString() : String.Empty;
				info.HEADERS    = EmailHeaders;
			}

			info.SplendidCRM_URL = HttpContext.Current.Request.Url.AbsoluteUri.Replace("EmailService.svc/ParseEmail", "");
			L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));

			StringBuilder sb = new StringBuilder();
			sb.AppendLine("<form name=\"frmSplendidCRM\" id=\"frmSplendidCRM\" onsubmit=\"return false;\">");
			sb.AppendLine("	<table id=\"tblSplendidButtons\">");
			sb.AppendLine("		<tr>");
			sb.AppendLine("			<td width=\"99%\">");
			if ( SplendidCRM.Security.GetUserAccess("Emails", "edit") >= 0 )
			{
				sb.AppendLine("				<input type=\"button\" value=\"  " + L10n.Term("Emails.LNK_NEW_EMAIL") + "  \" onclick=\"var splendidCreateEvent = document.createEvent('Event'); splendidCreateEvent.initEvent('SplendidCreateEvent', true, true); document.getElementById('divSplendidCRMExtension').dispatchEvent(splendidCreateEvent); return false;\" /> ");
			}
			sb.AppendLine("				<input type=\"button\" value=\"  " + L10n.Term(".LBL_CANCEL_BUTTON_LABEL") + "  \" onclick=\"var splendidCancelEvent  = document.createEvent('Event'); splendidCancelEvent.initEvent ('SplendidCancelEvent' , true, true); document.getElementById('divSplendidCRMExtension').dispatchEvent(splendidCancelEvent ); return false;\" />&nbsp;");
			sb.AppendLine("			</td>");
			sb.AppendLine("			<td width=\"1%\">");
			sb.AppendLine("				<img id='imgSplendidCRMExtensionLogo' src='SplendidCRM-40.png' width='40' height='40' />");
			sb.AppendLine("			</td>");
			sb.AppendLine("		</tr>");
			sb.AppendLine("	</table>");

			ExtensionUtils.AppendEditViewFields("Emails", info, sb);
			sb.AppendLine("</form>");
			info.EDIT_VIEW = sb.ToString();
			return info;
		}

		[OperationContract]
		public ArchiveEmailResponse ArchiveEmail(Stream input)
		{
			ArchiveEmailResponse result = new ArchiveEmailResponse();
			if ( Security.IsAuthenticated() )
			{
				int nACLACCESS = Security.GetUserAccess("Emails", "edit");
				if ( nACLACCESS < 0 )
				{
					L10N L10n = new L10N("en-US");
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
				}
				MimeMessage mm = MimeMessage.Load(input);
				
				DbProviderFactory dbf = DbProviderFactories.GetFactory(HttpContext.Current.Application);
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					sSQL = "select ID                      " + ControlChars.CrLf
					     + "  from vwEMAILS_Inbound        " + ControlChars.CrLf
					     + " where MESSAGE_ID = @MESSAGE_ID" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						// 09/04/2011 Paul.  In order to prevent duplicate emails, we need to use the unique message ID. 
						// 01/21/2017 Paul.  Convert to MimeKit. 
						string sDeliveredTo = (mm.Headers.Contains("Delivered-To") ? mm.Headers["Delivered-To"] : String.Empty);
						string sUNIQUE_MESSAGE_ID = mm.MessageId + (!Sql.IsEmptyString(sDeliveredTo) ? sDeliveredTo : Security.USER_ID.ToString());
						Sql.AddParameter(cmd, "@MESSAGE_ID", sUNIQUE_MESSAGE_ID);
						Guid gEMAIL_ID = Sql.ToGuid(cmd.ExecuteScalar());
						if ( Sql.IsEmptyGuid(gEMAIL_ID) )
						{
							// 01/28/2017 Paul.  Use new GROUP_TEAM_ID value associated with InboundEmail record. 
							gEMAIL_ID = MimeUtils.ImportInboundEmail(HttpContext.Current, con, mm, Guid.Empty, String.Empty, Security.USER_ID, Security.TEAM_ID, sUNIQUE_MESSAGE_ID);
						}
						result.NAME = mm.Subject;
						result.ID   = gEMAIL_ID.ToString();
					}
				}
			}
			else
			{
				throw(new Exception("The session ID is invalid."));
			}
			return result;
		}

		public class EmailRelationship
		{
			public string Module;
			public string ID    ;
			public string Name  ;

			public EmailRelationship()
			{
				Module = String.Empty;
				ID     = String.Empty;
				Name   = String.Empty;
			}
		}

		[OperationContract]
		public int SetEmailRelationships(Guid ID, EmailRelationship[] Selection)
		{
			if ( Security.IsAuthenticated() )
			{
				if ( Sql.IsEmptyGuid(ID) )
					throw(new MissingFieldException("ID is required."));
				
				DbProviderFactory dbf = DbProviderFactories.GetFactory(HttpContext.Current.Application);
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					using ( IDbTransaction trn = Sql.BeginTransaction(con) )
					{
						try
						{
							foreach ( EmailRelationship rel in Selection )
							{
								SqlProcs.spEMAILS_RELATED_Update(ID, rel.Module, Sql.ToGuid(rel.ID), trn);
							}
							trn.Commit();
						}
						catch(Exception ex)
						{
							trn.Rollback();
							SplendidError.SystemMessage(HttpContext.Current, "Error", new StackTrace(true).GetFrame(0), ex);
							throw;
						}
					}
				}
			}
			else
			{
				throw(new Exception("The session ID is invalid."));
			}
			return Selection.Length;
		}
	}
}
