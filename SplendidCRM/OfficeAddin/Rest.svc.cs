/*
 * Copyright (C) 2016-2018 SplendidCRM Software, Inc. All Rights Reserved. 
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
 */
using System;
using System.IO;
using System.Xml;
using System.Web;
using System.Web.SessionState;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Web.Script.Serialization;
using System.Diagnostics;
using Microsoft.Exchange.WebServices.Auth.Validation;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.IdentityModel.Claims;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using System.IdentityModel.Selectors;
using System.IdentityModel.Services;
using System.IdentityModel.Protocols.WSTrust;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
// Install-Package System.IdentityModel.Tokens.Jwt -Version 4.0.3.308261200
using System.IdentityModel.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Linq;

namespace SplendidCRM.OfficeAddin
{
	public class IdentityTokenResponse
	{
		public string errorMessage { get; set; }
		public IdentityToken token { get; set; }
		public IdentityTokenResponse()
		{
			this.token = new IdentityToken();
		}
	}

	public class IdentityTokenRequest
	{
		public string token { get; set; }
	}

	public class IdentityToken
	{
		public DateTime ExpirationDate    ;
		public string   Audience          ;
		public string   UniqueUserID      ;
		public string   ExchangeUserID    ;
		public string   SplendidID        ;
		public string   ContextSenderID   ;
		public string   MetadataURL       ;
		public bool     AutoLoggedIn      ;
		public string[] SplendidRelated   ;
	}

	[ServiceContract]
	[ServiceBehavior(IncludeExceptionDetailInFaults=true)]
	[AspNetCompatibilityRequirements(RequirementsMode=AspNetCompatibilityRequirementsMode.Required)]
	public class Rest
	{
		// https://yorkporc.wordpress.com/2014/04/11/wcf-server-for-jwt-handlingvalidation/
		private List<X509SecurityToken> ReadSigningCertsFromMetadata(System.IdentityModel.Metadata.EntityDescriptor entityDescriptor)
		{
			List<X509SecurityToken> stsSigningTokens = new List<X509SecurityToken>();
 			System.IdentityModel.Metadata.SecurityTokenServiceDescriptor stsd = entityDescriptor.RoleDescriptors.OfType<System.IdentityModel.Metadata.SecurityTokenServiceDescriptor>().First();
 			if ( stsd != null )
			{
				// read non-null X509Data keyInfo elements meant for Signing
				IEnumerable<X509RawDataKeyIdentifierClause> x509DataClauses = stsd.Keys.Where(key => key.KeyInfo != null && (key.Use == KeyType.Signing || key.Use == KeyType.Unspecified)).
					Select(key => key.KeyInfo.OfType<X509RawDataKeyIdentifierClause>().First());
 
				stsSigningTokens.AddRange(x509DataClauses.Select(token => new X509SecurityToken(new X509Certificate2(token.GetX509RawData()))));
			}
			else
			{
				throw new InvalidOperationException("There is no RoleDescriptor of type SecurityTokenServiceType in the metadata");
			}
			return stsSigningTokens;
		}

		[OperationContract]
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Guid Login(string UserName, string Password, string Version, string ExchangeUserID)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpSessionState     Session     = HttpContext.Current.Session    ;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			
			// 11/05/2018 Paul.  Protect against null inputs. 
			string sUSER_NAME   = Sql.ToString(UserName);
			string sPASSWORD    = Sql.ToString(Password);
			string sVERSION     = Sql.ToString(Version );
			Guid gUSER_ID       = Guid.Empty;
			Guid gUSER_LOGIN_ID = Guid.Empty;
			
			// 02/23/2011 Paul.  SYNC service should check for lockout. 
			if ( SplendidInit.LoginFailures(Application, sUSER_NAME) >= Crm.Password.LoginLockoutCount(Application) )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("Users.ERR_USER_LOCKED_OUT")));
			}
			// 04/16/2013 Paul.  Allow system to be restricted by IP Address. 
			if ( SplendidInit.InvalidIPAddress(Application, Request.UserHostAddress) )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("Users.ERR_INVALID_IP_ADDRESS")));
			}

			// 01/09/2017 Paul.  Add support for ADFS Single-Sign-On.  Using WS-Federation Desktop authentication (username/password). 
			string sError = String.Empty;
			if ( Sql.ToBoolean(Application["CONFIG.ADFS.SingleSignOn.Enabled"]) )
			{
				// 05/02/2017 Paul.  Need a separate flag for the mobile client. 
				gUSER_ID = ActiveDirectory.FederationServicesValidateJwt(HttpContext.Current, sPASSWORD, false, ref sError);
				if ( !Sql.IsEmptyGuid(gUSER_ID) )
				{
					SplendidInit.LoginUser(gUSER_ID, "Azure AD");
				}
			}
			else if ( Sql.ToBoolean(Application["CONFIG.Azure.SingleSignOn.Enabled"]) )
			{
				// 05/02/2017 Paul.  Need a separate flag for the mobile client. 
				gUSER_ID = ActiveDirectory.AzureValidateJwt(HttpContext.Current, sPASSWORD, false, ref sError);
				if ( !Sql.IsEmptyGuid(gUSER_ID) )
				{
					SplendidInit.LoginUser(gUSER_ID, "Azure AD");
				}
			}
			else
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					sSQL = "select ID                    " + ControlChars.CrLf
					     + "  from vwUSERS_Login         " + ControlChars.CrLf
					     + " where USER_NAME = @USER_NAME" + ControlChars.CrLf
					     + "   and USER_HASH = @USER_HASH" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						string sUSER_HASH = Security.HashPassword(sPASSWORD);
						// 12/25/2009 Paul.  Use lowercase username to match the primary authentication function. 
						Sql.AddParameter(cmd, "@USER_NAME", sUSER_NAME.ToLower());
						Sql.AddParameter(cmd, "@USER_HASH", sUSER_HASH);
						gUSER_ID = Sql.ToGuid(cmd.ExecuteScalar());
						if ( Sql.IsEmptyGuid(gUSER_ID) )
						{
							SqlProcs.spUSERS_LOGINS_InsertOnly(ref gUSER_LOGIN_ID, Guid.Empty, sUSER_NAME, "Anonymous", "Failed", Session.SessionID, Request.UserHostName, Request.Url.Host, Request.Path, Request.AppRelativeCurrentExecutionFilePath, Request.UserAgent);
							// 02/20/2011 Paul.  Log the failure so that we can lockout the user. 
							SplendidInit.LoginTracking(Application, sUSER_NAME, false);
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "SECURITY: failed attempted login for " + sUSER_NAME + " using REST API");
						}
						else
						{
							SplendidInit.LoginUser(gUSER_ID, "Anonymous");
						}
					}
				}
			}
			if ( gUSER_ID == Guid.Empty )
			{
				SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "Invalid username and/or password for " + sUSER_NAME);
				throw(new Exception("Invalid username and/or password for " + sUSER_NAME));
			}
			return gUSER_ID;
		}

		[OperationContract]
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public void Logout()
		{
			try
			{
				SqlProcs.spEXCHANGE_ADDIN_LOGINS_Logout(Security.USER_ID);
				
				Guid gUSER_LOGIN_ID = Security.USER_LOGIN_ID;
				if ( !Sql.IsEmptyGuid(gUSER_LOGIN_ID) )
					SqlProcs.spUSERS_LOGINS_Logout(gUSER_LOGIN_ID);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
			HttpContext.Current.Session.Abandon();
			// 11/15/2014 Paul.  Prevent resuse of SessionID. 
			// http://support.microsoft.com/kb/899918
			HttpContext.Current.Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
		}

		[OperationContract]
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Stream CreateAndValidateIdentityToken(string token, string hostUri, string ewsUrl, string internetMessageId)
		{
			JavaScriptSerializer json = new JavaScriptSerializer();
			IdentityTokenResponse response = new IdentityTokenResponse();
			try
			{
				//string sRequest = String.Empty;
				//using ( StreamReader stmRequest = new StreamReader(input, System.Text.Encoding.UTF8) )
				//{
				//	sRequest = stmRequest.ReadToEnd();
				//}
				//json.MaxJsonLength = int.MaxValue;
				//Dictionary<string, object> dict = json.Deserialize<Dictionary<string, object>>(sRequest);
				//string token   = Sql.ToString(dict["token"  ]);
				//string hostUri = Sql.ToString(dict["hostUri"]);
	
				// Use the Exchange token validation library
				// https://msdn.microsoft.com/en-us/library/f7f4813a-3b2d-47bb-bf93-71b64620a56b
				//string hostUri = "https://developer9/SplendidCRM6/default.aspx";
				Debug.WriteLine(hostUri);
				if ( !Sql.IsEmptyString(ewsUrl) )
				{
					AppIdentityToken t = (AppIdentityToken) AuthToken.Parse(token);
					t.Validate(new Uri(hostUri));
					response.token.ExpirationDate = t.ExpirationDate;
					response.token.Audience       = hostUri;
					response.token.UniqueUserID   = t.UniqueUserIdentification;
					response.token.AutoLoggedIn   = false;
					foreach ( IClaimsIdentity claimIdentity in t.Claims )
					{
						foreach ( Claim claim in claimIdentity.Claims )
						{
							Debug.WriteLine(claim.ClaimType + " " + claim.Value);
							if ( claim.ClaimType == "appctxsender" )
							{
								response.token.ContextSenderID = claim.Value;
							}
							else if ( claim.ClaimType == "appctx" )
							{
								Dictionary<string, string> dict = json.Deserialize<Dictionary<string, string>>(claim.Value);
								response.token.ExchangeUserID = dict["msexchuid"];
								response.token.MetadataURL    = dict["amurl"    ];
							}
						}
					}
				}
				else
				{
					// 04/02/160 Paul.  Word and Excell use a simulated Exchange User ID. 
					response.token.ExchangeUserID = Sql.ToString(token);
				}
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					if ( !Security.IsAuthenticated() )
					{
						sSQL = "select ASSIGNED_USER_ID                    " + ControlChars.CrLf
						     + "  from vwEXCHANGE_ADDIN_LOGINS             " + ControlChars.CrLf
						     + " where EXCHANGE_USER_ID = @EXCHANGE_USER_ID" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							cmd.CommandTimeout = 0;
							Sql.AddParameter(cmd, "@EXCHANGE_USER_ID", Sql.ToGuid(response.token.ExchangeUserID));
							Guid gUSER_ID = Sql.ToGuid(cmd.ExecuteScalar());
							if ( !Sql.IsEmptyGuid(gUSER_ID) )
							{
								SplendidInit.LoginUser(gUSER_ID, "ExchangeAddin");
								response.token.AutoLoggedIn = true;
							}
						}
					}
					if ( Security.IsAuthenticated() )
					{
						if ( !Sql.IsEmptyString(internetMessageId) )
						{
							Guid gEMAIL_ID = Guid.Empty;
							sSQL = "select ID                      " + ControlChars.CrLf
							     + "  from vwEMAILS_Inbound        " + ControlChars.CrLf
							     + " where MESSAGE_ID = @MESSAGE_ID" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								cmd.CommandTimeout = 0;
								Sql.AddParameter(cmd, "@MESSAGE_ID", internetMessageId);
								gEMAIL_ID = Sql.ToGuid(cmd.ExecuteScalar());
								if ( !Sql.IsEmptyGuid(gEMAIL_ID) )
								{
									response.token.SplendidID = gEMAIL_ID.ToString();
								}
							}
							
							if ( !Sql.IsEmptyGuid(gEMAIL_ID) )
							{
								List<string> lstSplendidRelated = new List<string>();
								sSQL = "select PARENT_ID               " + ControlChars.CrLf
								     + "  from vwEMAILS_Related        " + ControlChars.CrLf
								     + " where EMAIL_ID = @EMAIL_ID    " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									cmd.CommandTimeout = 0;
									Sql.AddParameter(cmd, "@EMAIL_ID", gEMAIL_ID);
									using ( DbDataAdapter da = dbf.CreateDataAdapter() )
									{
										((IDbDataAdapter)da).SelectCommand = cmd;
										using ( DataTable dt = new DataTable() )
										{
											da.Fill(dt);
											foreach ( DataRow row in dt.Rows )
											{
												lstSplendidRelated.Add(Sql.ToString(row["PARENT_ID"]));
											}
										}
									}
								}
								response.token.SplendidRelated = lstSplendidRelated.ToArray();
							}
						}
					}
				}
			}
			catch (TokenValidationException ex)
			{
				response.errorMessage = ex.Message;
			}
			string sResponse = json.Serialize(response);
			byte[] byResponse = Encoding.UTF8.GetBytes(sResponse);
			return new MemoryStream(byResponse);
		}

		[OperationContract]
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public int ArchiveEmail(string authToken, string ewsUrl, string internetMessageId, string itemID, bool sentItemsFolder, string MODULE_NAME, string PARENT_ID)
		{
			// 03/22/2016 Paul.  Make sure to use Utc as that is what the Import code is expecting. 
			ExchangeService service = new ExchangeService(TimeZoneInfo.Utc);
			service.Credentials = new OAuthCredentials(authToken);
			service.Url = new Uri(ewsUrl);
			
			if ( Security.IsAuthenticated() )
			{
				HttpContext          Context         = HttpContext.Current            ;
				HttpApplicationState Application     = HttpContext.Current.Application;
				HttpSessionState     Session         = HttpContext.Current.Session    ;
				Guid                 gUSER_ID        = Security.USER_ID               ;
				string               sMODULE_NAME    = Sql.ToString(MODULE_NAME);
				Guid                 gPARENT_ID      = Sql.ToGuid  (PARENT_ID  );

				StringBuilder sbErrors = new StringBuilder();
				Microsoft.Exchange.WebServices.Data.EmailMessage email = Microsoft.Exchange.WebServices.Data.EmailMessage.Bind(service, itemID);
				DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					// 03/23/2016 Paul.  email.IsFromMe is the way to determine if sent item. 
					/*
					bool bSentItems = (String.Compare(email.Sender.Address, sEXCHANGE_EMAIL, true) == 0);
					try
					{
						// 03/22/2016 Paul.  We do not have permission to get the folder. 
						PropertySet psWellKnownFolder = new PropertySet(BasePropertySet.IdOnly);//, FolderSchema.WellKnownFolderName, FolderSchema.DisplayName);
						//Folder fldSentItems = Folder.Bind(service, WellKnownFolderName.SentItems, psWellKnownFolder);
						Folder fld = Folder.Bind(service, email.ParentFolderId, psWellKnownFolder);
						if ( fld.WellKnownFolderName.HasValue && fld.WellKnownFolderName.Value == WellKnownFolderName.SentItems )
							bSentItems = true;
					}
					catch
					{
					}
					*/
					ImportMessage(Context, Session, con, sMODULE_NAME, gPARENT_ID, gUSER_ID, email, sbErrors);
				}
				return 1;
			}
			else
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
		}

		public static void ImportMessage(HttpContext Context, HttpSessionState Session, IDbConnection con, string sPARENT_TYPE, Guid gPARENT_ID, Guid gUSER_ID, EmailMessage email, StringBuilder sbErrors)
		{
			HttpApplicationState Application = Context.Application;
			bool   bVERBOSE_STATUS = Sql.ToBoolean(Application["CONFIG.Exchange.VerboseStatus"]);
			Guid   gTEAM_ID        = Sql.ToGuid(Session["TEAM_ID"]);
			long   lUploadMaxSize  = Sql.ToLong(Application["CONFIG.upload_maxsize"]);
			string sCULTURE        = L10N.NormalizeCulture(Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
			
			IDbCommand spEMAILS_Update = SqlProcs.Factory(con, "spEMAILS_Update");
			
			// 11/14/2017 Paul.  We started using InternetMessageId on 03/23/2016, but the property is not always available. 
			// You must load or assign this property before you can read its value at Microsoft.Exchange.WebServices.Data.EmailMessage.get_InternetMessageId()
			string sInternetMessageId = String.Empty;
			try
			{
				sInternetMessageId = email.InternetMessageId;
			}
			catch
			{
				try
				{
					EmailMessage email1 = EmailMessage.Bind(email.Service, email.Id, new PropertySet(EmailMessageSchema.InternetMessageId));
					sInternetMessageId = email1.InternetMessageId;
				}
				catch
				{
				}
			}
			string sREMOTE_KEY = email.Id.UniqueId;
			string sSQL = String.Empty;
			sSQL = "select SYNC_LOCAL_ID                                 " + ControlChars.CrLf
			     + "  from vwEMAIL_CLIENT_SYNC                           " + ControlChars.CrLf
			     + " where SYNC_ASSIGNED_USER_ID = @SYNC_ASSIGNED_USER_ID" + ControlChars.CrLf
			     + "   and SYNC_REMOTE_KEY       = @SYNC_REMOTE_KEY      " + /* Sql.CaseSensitiveCollation(con) + */ ControlChars.CrLf;
			using ( IDbCommand cmd = con.CreateCommand() )
			{
				cmd.CommandText = sSQL;
				Sql.AddParameter(cmd, "@SYNC_ASSIGNED_USER_ID", gUSER_ID   );
				Sql.AddParameter(cmd, "@SYNC_REMOTE_KEY"      , sREMOTE_KEY);
				Guid gEMAIL_ID = Sql.ToGuid(cmd.ExecuteScalar());
				// 03/23/2016 Paul.  Email may have already been imported using the OfficeAddin. 
				if ( Sql.IsEmptyGuid(gEMAIL_ID) )
				{
					cmd.Parameters.Clear();
					sSQL = "select ID                      " + ControlChars.CrLf
					     + "  from vwEMAILS_Inbound        " + ControlChars.CrLf
					     + " where MESSAGE_ID = @MESSAGE_ID" + ControlChars.CrLf;
					cmd.CommandText = sSQL;
					cmd.CommandTimeout = 0;
					// 11/14/2017 Paul.  email.InternetMessageId needs to be loaded in advance as it is not part of the FirstClassProperties. 
					Sql.AddParameter(cmd, "@MESSAGE_ID", sInternetMessageId);
					gEMAIL_ID = Sql.ToGuid(cmd.ExecuteScalar());
					// 03/23/2016 Paul.  If email found in the system, then just add to the sync table. 
					if ( !Sql.IsEmptyGuid(gEMAIL_ID) )
					{
						DateTime dtREMOTE_DATE_MODIFIED_UTC = email.LastModifiedTime;
						DateTime dtREMOTE_DATE_MODIFIED     = TimeZoneInfo.ConvertTimeFromUtc(dtREMOTE_DATE_MODIFIED_UTC, TimeZoneInfo.Local);
						using ( IDbTransaction trn = Sql.BeginTransaction(con) )
						{
							try
							{
								SqlProcs.spEMAIL_CLIENT_SYNC_Update(gUSER_ID, gEMAIL_ID, sREMOTE_KEY, sPARENT_TYPE, gPARENT_ID, dtREMOTE_DATE_MODIFIED, dtREMOTE_DATE_MODIFIED_UTC, trn);
								SqlProcs.spEMAILS_InsertRelated(gEMAIL_ID, sPARENT_TYPE, gPARENT_ID, trn);
								trn.Commit();
							}
							catch(Exception ex)
							{
								trn.Rollback();
								SyncError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), ex);
								throw;
							}
						}
					}
				}
				else
				{
					// 03/24/2016 Paul.  The sync record exists, but this specific relationship may not. 
					SqlProcs.spEMAILS_InsertRelated(gEMAIL_ID, sPARENT_TYPE, gPARENT_ID);
				}
				if ( Sql.IsEmptyGuid(gEMAIL_ID) )
				{
					bool bLoadSuccessful = false;
					string sDESCRIPTION = String.Empty;
					try
					{
						email.Load();
						// 06/04/2010 Paul.  First load the plain-text body, then load the reset of the properties. 
						PropertySet psBodyText = new PropertySet(BasePropertySet.IdOnly, EmailMessageSchema.Body);
						psBodyText.RequestedBodyType = BodyType.Text;
						// 06/04/2010 Paul.  Can't use the same object to load the text body. 
						EmailMessage email1 = EmailMessage.Bind(email.Service, email.Id, psBodyText);
						sDESCRIPTION = email1.Body.Text;
						bLoadSuccessful = true;
					}
					catch(Exception ex)
					{
						string sError = "Error loading email " + email.Id.UniqueId + "." + ControlChars.CrLf;
						sError += Utils.ExpandException(ex) + ControlChars.CrLf;
						SyncError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), sError);
						sbErrors.AppendLine(sError);
					}
					if ( bLoadSuccessful )
					{
						DateTime dtREMOTE_DATE_MODIFIED_UTC = email.LastModifiedTime;
						DateTime dtREMOTE_DATE_MODIFIED     = TimeZoneInfo.ConvertTimeFromUtc(dtREMOTE_DATE_MODIFIED_UTC, TimeZoneInfo.Local);
						
						List<string> lstRecipients = new List<string>();
						if ( !email.IsFromMe )
							lstRecipients.Add(email.From.Address);
						
						string sREPLY_TO_NAME = String.Empty;
						string sREPLY_TO_ADDR = String.Empty;
						if ( email.ReplyTo != null && email.ReplyTo.Count > 0 )
						{
							sREPLY_TO_NAME = email.ReplyTo[0].Name   ;
							sREPLY_TO_ADDR = email.ReplyTo[0].Address;
						}
						
						StringBuilder sbTO_ADDRS_IDS    = new StringBuilder();
						StringBuilder sbTO_ADDRS_NAMES  = new StringBuilder();
						StringBuilder sbTO_ADDRS_EMAILS = new StringBuilder();
						if ( email.ToRecipients != null && email.ToRecipients.Count > 0 )
						{
							foreach ( EmailAddress addr in email.ToRecipients )
							{
								lstRecipients.Add(addr.Address);
								if ( sbTO_ADDRS_NAMES .Length > 0 ) sbTO_ADDRS_NAMES .Append(';');
								if ( sbTO_ADDRS_EMAILS.Length > 0 ) sbTO_ADDRS_EMAILS.Append(';');
								sbTO_ADDRS_NAMES .Append(addr.Name   );
								sbTO_ADDRS_EMAILS.Append(addr.Address);
								// 07/18/2010 Paul.  Exchange, Imap and Pop3 utils will all use this method to lookup a contact by email. 
								// 08/30/2010 Paul.  The previous method only returned Contacts, where as this new method returns Contacts, Leads and Prospects. 
								Guid gRECIPIENT_ID = Crm.Emails.RecipientByEmail(con, addr.Address);
								if ( !Sql.IsEmptyGuid(gRECIPIENT_ID) )
								{
									if ( sbTO_ADDRS_IDS.Length > 0 )
										sbTO_ADDRS_IDS.Append(';');
									sbTO_ADDRS_IDS.Append(gRECIPIENT_ID.ToString());
								}
							}
						}
						StringBuilder sbCC_ADDRS_IDS    = new StringBuilder();
						StringBuilder sbCC_ADDRS_NAMES  = new StringBuilder();
						StringBuilder sbCC_ADDRS_EMAILS = new StringBuilder();
						if ( email.CcRecipients != null && email.CcRecipients.Count > 0 )
						{
							foreach ( EmailAddress addr in email.CcRecipients )
							{
								lstRecipients.Add(addr.Address);
								if ( sbCC_ADDRS_NAMES .Length > 0 ) sbCC_ADDRS_NAMES .Append(';');
								if ( sbCC_ADDRS_EMAILS.Length > 0 ) sbCC_ADDRS_EMAILS.Append(';');
								sbCC_ADDRS_NAMES .Append(addr.Name   );
								sbCC_ADDRS_EMAILS.Append(addr.Address);
								// 07/18/2010 Paul.  Exchange, Imap and Pop3 utils will all use this method to lookup a contact by email. 
								// 08/30/2010 Paul.  The previous method only returned Contacts, where as this new method returns Contacts, Leads and Prospects. 
								Guid gRECIPIENT_ID = Crm.Emails.RecipientByEmail(con, addr.Address);
								if ( !Sql.IsEmptyGuid(gRECIPIENT_ID) )
								{
									if ( sbCC_ADDRS_IDS.Length > 0 )
										sbCC_ADDRS_IDS.Append(';');
									sbCC_ADDRS_IDS.Append(gRECIPIENT_ID.ToString());
								}
							}
						}
						StringBuilder sbBCC_ADDRS_IDS    = new StringBuilder();
						StringBuilder sbBCC_ADDRS_NAMES  = new StringBuilder();
						StringBuilder sbBCC_ADDRS_EMAILS = new StringBuilder();
						if ( email.BccRecipients != null && email.BccRecipients.Count > 0 )
						{
							foreach ( EmailAddress addr in email.BccRecipients )
							{
								lstRecipients.Add(addr.Address);
								if ( sbBCC_ADDRS_NAMES .Length > 0 ) sbBCC_ADDRS_NAMES .Append(';');
								if ( sbBCC_ADDRS_EMAILS.Length > 0 ) sbBCC_ADDRS_EMAILS.Append(';');
								sbBCC_ADDRS_NAMES .Append(addr.Name   );
								sbBCC_ADDRS_EMAILS.Append(addr.Address);
								// 07/18/2010 Paul.  Exchange, Imap and Pop3 utils will all use this method to lookup a contact by email. 
								// 08/30/2010 Paul.  The previous method only returned Contacts, where as this new method returns Contacts, Leads and Prospects. 
								Guid gRECIPIENT_ID = Crm.Emails.RecipientByEmail(con, addr.Address);
								if ( !Sql.IsEmptyGuid(gRECIPIENT_ID) )
								{
									if ( sbBCC_ADDRS_IDS.Length > 0 )
										sbBCC_ADDRS_IDS.Append(';');
									sbBCC_ADDRS_IDS.Append(gRECIPIENT_ID.ToString());
								}
							}
						}
						DataTable dtRecipients = new DataTable();
						dtRecipients.Columns.Add("PARENT_ID"  , Type.GetType("System.Guid"  ));
						dtRecipients.Columns.Add("PARENT_TYPE", Type.GetType("System.String"));
						cmd.Parameters.Clear();
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						// 03/23/2016 Paul.  Build a separate list of recipients to ensure that the From.Address relationship is created. 
						//sSQL = "select PARENT_ID              " + ControlChars.CrLf
						//     + "     , PARENT_TYPE            " + ControlChars.CrLf
						//     + "  from vwPARENTS_EMAIL_ADDRESS" + ControlChars.CrLf
						//     + " where 1 = 0                  " + ControlChars.CrLf;
						//cmd.CommandText = sSQL;
						//Sql.AppendParameter(cmd, lstRecipients.ToArray(), "EMAIL1", true);
						//using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						//{
						//	((IDbDataAdapter)da).SelectCommand = cmd;
						//	da.Fill(dtRecipients);
						//}
						// 04/26/2018 Paul.  Exchange Sync needs to follow team hierarchy rules. 
						// 04/26/2018 Paul.  We need to apply each module security rule separately. 
						string sMODULE_NAME = "Accounts";
						if ( Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".Valid"]) && lstRecipients.Count > 0 )
						{
							string sTABLE_NAME = Sql.ToString(Application["Modules." + sMODULE_NAME + ".TableName"]);
							sSQL = "select ID        " + ControlChars.CrLf
							     + "     , EMAIL1    " + ControlChars.CrLf
							     + "  from vw" + sTABLE_NAME + ControlChars.CrLf;
							cmd.Parameters.Clear();
							cmd.CommandText = sSQL;
							Security.Filter(cmd, sMODULE_NAME, "view");
							Sql.AppendParameter(cmd, lstRecipients.ToArray(), "EMAIL1");
							using ( DataTable dt = new DataTable() )
							{
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									foreach ( DataRow row in dt.Rows )
									{
										DataRow rowRecipient = dtRecipients.NewRow();
										rowRecipient["PARENT_ID"  ] = Sql.ToGuid(row["ID"]);
										rowRecipient["PARENT_TYPE"] = sMODULE_NAME;
										dtRecipients.Rows.Add(rowRecipient);
									}
								}
							}
						}
						sMODULE_NAME = "Contacts";
						if ( Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".Valid"]) && lstRecipients.Count > 0 )
						{
							string sTABLE_NAME = Sql.ToString(Application["Modules." + sMODULE_NAME + ".TableName"]);
							sSQL = "select ID        " + ControlChars.CrLf
							     + "     , EMAIL1    " + ControlChars.CrLf
							     + "  from vw" + sTABLE_NAME + ControlChars.CrLf;
							cmd.Parameters.Clear();
							cmd.CommandText = sSQL;
							Security.Filter(cmd, sMODULE_NAME, "view");
							Sql.AppendParameter(cmd, lstRecipients.ToArray(), "EMAIL1");
							using ( DataTable dt = new DataTable() )
							{
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									foreach ( DataRow row in dt.Rows )
									{
										DataRow rowRecipient = dtRecipients.NewRow();
										rowRecipient["PARENT_ID"  ] = Sql.ToGuid(row["ID"]);
										rowRecipient["PARENT_TYPE"] = sMODULE_NAME;
										dtRecipients.Rows.Add(rowRecipient);
									}
								}
							}
						}
						sMODULE_NAME = "Leads";
						if ( Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".Valid"]) && lstRecipients.Count > 0 )
						{
							string sTABLE_NAME = Sql.ToString(Application["Modules." + sMODULE_NAME + ".TableName"]);
							sSQL = "select ID        " + ControlChars.CrLf
							     + "     , EMAIL1    " + ControlChars.CrLf
							     + "  from vw" + sTABLE_NAME + ControlChars.CrLf;
							cmd.Parameters.Clear();
							cmd.CommandText = sSQL;
							Security.Filter(cmd, sMODULE_NAME, "view");
							Sql.AppendParameter(cmd, lstRecipients.ToArray(), "EMAIL1");
							using ( DataTable dt = new DataTable() )
							{
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									foreach ( DataRow row in dt.Rows )
									{
										DataRow rowRecipient = dtRecipients.NewRow();
										rowRecipient["PARENT_ID"  ] = Sql.ToGuid(row["ID"]);
										rowRecipient["PARENT_TYPE"] = sMODULE_NAME;
										dtRecipients.Rows.Add(rowRecipient);
									}
								}
							}
						}
						sMODULE_NAME = "Prospects";
						if ( Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".Valid"]) && lstRecipients.Count > 0 )
						{
							string sTABLE_NAME = Sql.ToString(Application["Modules." + sMODULE_NAME + ".TableName"]);
							sSQL = "select ID        " + ControlChars.CrLf
							     + "     , EMAIL1    " + ControlChars.CrLf
							     + "  from vw" + sTABLE_NAME + ControlChars.CrLf;
							cmd.Parameters.Clear();
							cmd.CommandText = sSQL;
							Security.Filter(cmd, sMODULE_NAME, "view");
							Sql.AppendParameter(cmd, lstRecipients.ToArray(), "EMAIL1");
							using ( DataTable dt = new DataTable() )
							{
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									foreach ( DataRow row in dt.Rows )
									{
										DataRow rowRecipient = dtRecipients.NewRow();
										rowRecipient["PARENT_ID"  ] = Sql.ToGuid(row["ID"]);
										rowRecipient["PARENT_TYPE"] = sMODULE_NAME;
										dtRecipients.Rows.Add(rowRecipient);
									}
								}
							}
						}
						using ( IDbTransaction trn = Sql.BeginTransaction(con) )
						{
							try
							{
								if ( bVERBOSE_STATUS )
									SyncError.SystemMessage(Context, "Warning", new StackTrace(true).GetFrame(0), "ImportMessage: Retrieving email " + Sql.ToString(email.Subject) + " " + Sql.ToString(email.DateTimeReceived.ToLocalTime().ToString()));
								
								spEMAILS_Update.Transaction = trn;
								foreach(IDbDataParameter par in spEMAILS_Update.Parameters)
								{
									// 03/27/2010 Paul.  The ParameterName will start with @, so we need to remove it. 
									string sParameterName = Sql.ExtractDbName(spEMAILS_Update, par.ParameterName).ToUpper();
									if ( sParameterName == "TEAM_ID" )
										par.Value = gTEAM_ID;
									else if ( sParameterName == "ASSIGNED_USER_ID" )
										par.Value = gUSER_ID;
									else if ( sParameterName == "MODIFIED_USER_ID" )
										par.Value = gUSER_ID;
									else
										par.Value = DBNull.Value;
								}
								
								foreach(IDbDataParameter par in spEMAILS_Update.Parameters)
								{
									Security.ACL_FIELD_ACCESS acl = new Security.ACL_FIELD_ACCESS(Security.ACL_FIELD_ACCESS.FULL_ACCESS, Guid.Empty);
									string sColumnName = Sql.ExtractDbName(spEMAILS_Update, par.ParameterName).ToUpper();
									if ( SplendidInit.bEnableACLFieldSecurity )
									{
										acl = Security.GetUserFieldSecurity("Emails", sColumnName, gUSER_ID);
									}
									if ( acl.IsWriteable() )
									{
										try
										{
											switch ( sColumnName )
											{
												case "MODIFIED_USER_ID"  :  par.Value = gUSER_ID;  break;
												case "DATE_TIME"         :  par.Value = email.IsFromMe ? Sql.ToDBDateTime(email.DateTimeSent.ToLocalTime()) : Sql.ToDBDateTime(email.DateTimeReceived.ToLocalTime());  break;
												case "TYPE"              :  par.Value = email.IsFromMe && !email.IsDraft ? "sent" : "archived";  break;
												case "NAME"              :  par.Value = Sql.ToDBString(email.Subject                );  break;
												case "DESCRIPTION"       :  par.Value = Sql.ToDBString(sDESCRIPTION                 );  break;
												case "DESCRIPTION_HTML"  :  par.Value = Sql.ToDBString(email.Body.Text              );  break;
												case "PARENT_TYPE"       :  par.Value = Sql.ToDBString(sPARENT_TYPE                 );  break;
												case "PARENT_ID"         :  par.Value = Sql.ToDBGuid  (gPARENT_ID                   );  break;
												// 11/14/2017 Paul.  email.InternetMessageId needs to be loaded in advance as it is not part of the FirstClassProperties. 
												case "MESSAGE_ID"        :  par.Value = Sql.ToDBString(sInternetMessageId           );  break;
												case "FROM_NAME"         :  par.Value = Sql.ToDBString(email.From.Name              );  break;
												case "FROM_ADDR"         :  par.Value = Sql.ToDBString(email.From.Address           );  break;
												case "REPLY_TO_NAME"     :  par.Value = Sql.ToDBString(sREPLY_TO_NAME               );  break;
												case "REPLY_TO_ADDR"     :  par.Value = Sql.ToDBString(sREPLY_TO_ADDR               );  break;
												case "TO_ADDRS"          :  par.Value = Sql.ToDBString(email.DisplayTo              );  break;
												case "CC_ADDRS"          :  par.Value = Sql.ToDBString(email.DisplayCc              );  break;
												case "BCC_ADDRS"         :  par.Value = Sql.ToDBString(String.Empty                 );  break;
												case "TO_ADDRS_IDS"      :  par.Value = Sql.ToDBString(sbTO_ADDRS_IDS    .ToString());  break;
												case "TO_ADDRS_NAMES"    :  par.Value = Sql.ToDBString(sbTO_ADDRS_NAMES  .ToString());  break;
												case "TO_ADDRS_EMAILS"   :  par.Value = Sql.ToDBString(sbTO_ADDRS_EMAILS .ToString());  break;
												case "CC_ADDRS_IDS"      :  par.Value = Sql.ToDBString(sbCC_ADDRS_IDS    .ToString());  break;
												case "CC_ADDRS_NAMES"    :  par.Value = Sql.ToDBString(sbCC_ADDRS_NAMES  .ToString());  break;
												case "CC_ADDRS_EMAILS"   :  par.Value = Sql.ToDBString(sbCC_ADDRS_EMAILS .ToString());  break;
												case "BCC_ADDRS_IDS"     :  par.Value = Sql.ToDBString(sbBCC_ADDRS_IDS   .ToString());  break;
												case "BCC_ADDRS_NAMES"   :  par.Value = Sql.ToDBString(sbBCC_ADDRS_NAMES .ToString());  break;
												case "BCC_ADDRS_EMAILS"  :  par.Value = Sql.ToDBString(sbBCC_ADDRS_EMAILS.ToString());  break;
												//case "INTERNET_HEADERS"  :  par.Value = Sql.ToDBString(email.InternetMessageHeaders.ToString() );  break;
											}
										}
										catch
										{
											// 03/27/2010 Paul.  Some fields are not available.  Lets just ignore them. 
										}
									}
								}
								spEMAILS_Update.ExecuteNonQuery();
								IDbDataParameter parEMAIL_ID = Sql.FindParameter(spEMAILS_Update, "@ID");
								gEMAIL_ID = Sql.ToGuid(parEMAIL_ID.Value);
								
								SqlProcs.spEMAILS_USERS_Update(gEMAIL_ID, gUSER_ID, trn);
								if ( email.HasAttachments )
								{
									// 03/31/2010 Paul.  Web do not need to load the attachments separately. 
									// email.Load(new PropertySet(ItemSchema.Attachments));
									foreach ( Attachment attach in email.Attachments )
									{
										if ( attach is FileAttachment )
										{
											FileAttachment file = attach as FileAttachment;
											file.Load();
											if ( file.Content != null )
											{
												// 04/01/2010 Paul.  file.Size is only available on Exchange 2010. 
												long lFileSize = file.Content.Length;  // file.Size;
												if ( (lUploadMaxSize == 0) || (lFileSize <= lUploadMaxSize) )
												{
													string sFILENAME       = Path.GetFileName (file.FileName);
													if ( Sql.IsEmptyString(sFILENAME) )
														sFILENAME = file.Name;
													string sFILE_EXT       = Path.GetExtension(sFILENAME);
													string sFILE_MIME_TYPE = file.ContentType;
													
													Guid gNOTE_ID = Guid.Empty;
													SqlProcs.spNOTES_Update
														( ref gNOTE_ID
														, L10N.Term(Application, sCULTURE, "Emails.LBL_EMAIL_ATTACHMENT") + ": " + sFILENAME
														, "Emails"   // Parent Type
														, gEMAIL_ID  // Parent ID
														, Guid.Empty
														, String.Empty
														, gTEAM_ID
														, String.Empty
														, gUSER_ID
														// 05/17/2017 Paul.  Add Tags module. 
														, String.Empty  // TAG_SET_NAME
														// 11/07/2017 Paul.  Add IS_PRIVATE for use by a large customer. 
														, false         // IS_PRIVATE
														// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
														, String.Empty  // ASSIGNED_SET_LIST
														, trn
														);
													
													Guid gNOTE_ATTACHMENT_ID = Guid.Empty;
													SqlProcs.spNOTE_ATTACHMENTS_Insert(ref gNOTE_ATTACHMENT_ID, gNOTE_ID, file.FileName, sFILENAME, sFILE_EXT, sFILE_MIME_TYPE, trn);
													Crm.NoteAttachments.LoadFile(gNOTE_ATTACHMENT_ID, file.Content, trn);
												}
											}
										}
									}
								}
								SqlProcs.spEMAIL_CLIENT_SYNC_Update(gUSER_ID, gEMAIL_ID, sREMOTE_KEY, sPARENT_TYPE, gPARENT_ID, dtREMOTE_DATE_MODIFIED, dtREMOTE_DATE_MODIFIED_UTC, trn);
								foreach ( DataRow row in dtRecipients.Rows )
								{
									Guid   gRECIPIENT_PARENT_ID   = Sql.ToGuid  (row["PARENT_ID"  ]);
									string sRECIPIENT_PARENT_TYPE = Sql.ToString(row["PARENT_TYPE"]);
									SqlProcs.spEMAILS_InsertRelated(gEMAIL_ID, sRECIPIENT_PARENT_TYPE, gRECIPIENT_PARENT_ID, trn);
								}
								SqlProcs.spEMAILS_InsertRelated(gEMAIL_ID, sPARENT_TYPE, gPARENT_ID, trn);
								trn.Commit();
							}
							catch(Exception ex)
							{
								trn.Rollback();
								SyncError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), ex);
								throw;
							}
						}
					}
				}
			}
		}
	}
}
