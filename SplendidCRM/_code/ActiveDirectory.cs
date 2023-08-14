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
using System.Net;
using System.Xml;
using System.Web;
using System.Web.SessionState;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using Microsoft.Exchange.WebServices.Data;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IdentityModel.Tokens;
using System.IdentityModel.Selectors;
using System.IdentityModel.Services;
using System.IdentityModel.Protocols.WSTrust;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
// Install-Package System.IdentityModel.Tokens.Jwt -Version 4.0.3.308261200
using System.IdentityModel.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
// Install-Package System.IdentityModel.Tokens.ValidatingIssuerNameRegistry
// ValidatingIssuerNameRegistry
// http://www.cloudidentity.com/blog/2013/02/08/multitenant-sts-and-token-validation-4/


namespace SplendidCRM
{
	[DataContract]
	public class Office365AccessToken
	{
		[DataMember] public string token_type    { get; set; }
		[DataMember] public string scope         { get; set; }
		[DataMember] public string expires_in    { get; set; }
		[DataMember] public string expires_on    { get; set; }
		[DataMember] public string access_token  { get; set; }
		[DataMember] public string refresh_token { get; set; }

		public string AccessToken
		{
			get { return access_token;  }
			set { access_token = value; }
		}
		public string RefreshToken
		{
			get { return refresh_token;  }
			set { refresh_token = value; }
		}
		public Int64 ExpiresInSeconds
		{
			get { return Sql.ToInt64(expires_in);  }
			set { expires_in = Sql.ToString(value); }
		}
		public string TokenType
		{
			get { return token_type;  }
			set { token_type = value; }
		}
	}

	// https://graph.microsoft.io/en-us/docs
	[DataContract]
	public class MicrosoftGraphProfile
	{
		[DataMember] public string id                { get; set; }
		[DataMember] public string userPrincipalName { get; set; }
		[DataMember] public string displayName       { get; set; }
		[DataMember] public string givenName         { get; set; }
		[DataMember] public string surname           { get; set; }
		[DataMember] public string jobTitle          { get; set; }
		[DataMember] public string mail              { get; set; }
		[DataMember] public string officeLocation    { get; set; }
		[DataMember] public string preferredLanguage { get; set; }
		[DataMember] public string mobilePhone       { get; set; }
		[DataMember] public string[] businessPhones  { get; set; }

		public string Name
		{
			get { return displayName; }
			set { displayName = value; }
		}
		public string FirstName
		{
			get { return givenName; }
			set { givenName = value; }
		}
		public string LastName
		{
			get { return surname; }
			set { surname = value; }
		}
		public string UserName
		{
			get { return userPrincipalName; }
			set { userPrincipalName = value; }
		}
		public string EmailAddress
		{
			get { return mail; }
			set { mail = value; }
		}
	}

	public class ActiveDirectory
	{
		// 05/12/2023 Paul.  Keep track of failures and don't delete token immediately. 
		private static Dictionary<Guid, int> dictUserRefreshFailures = new Dictionary<Guid,int>();

		// https://yorkporc.wordpress.com/2014/04/11/wcf-server-for-jwt-handlingvalidation/
		private static List<X509SecurityToken> ReadSigningCertsFromMetadata(System.IdentityModel.Metadata.EntityDescriptor entityDescriptor)
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

		public static string AzureLogin(HttpContext Context)
		{
			HttpApplicationState Application = Context.Application;
			HttpRequest          Request     = Context.Request;

			string sAadTenantDomain  = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.AadTenantDomain"]);
			string sRealm            = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.Realm"          ]);
			string sAuthority        = "https://login.microsoftonline.com/" + sAadTenantDomain + "/wsfed";
			//string sRedirectURL = Request.Url.Scheme + "://" + Request.Url.Host + Request.ApplicationPath + "/Users/Login.aspx";
			string sRedirectURL = "https://" + Request.Url.Host + Request.ApplicationPath + "/Users/Login.aspx";
			// 07/08/2017 Paul.  We cannot support the Redirect as it is unlikely that the full redirect URL will be added to the App registration. 
			//if ( !Sql.IsEmptyString(Request["Redirect"]) )
			//	sRedirectURL += "?Redirect=" + HttpUtility.UrlEncode(Request["Redirect"]);
			// https://login.microsoftonline.com/salessplendidcrm.onmicrosoft.com/wsfed?wa=wsignin1.0&wtrealm=https%3a%2f%2fsalessplendidcrm.onmicrosoft.com%2fSplendidCRM6_Azure&wreply=https%3a%2f%2flocalhost%2fSplendidCRM6_Azure%2fUsers%2fLogin.aspx
			SignInRequestMessage signinRequest = new System.IdentityModel.Services.SignInRequestMessage(new Uri(sAuthority), sRealm, sRedirectURL);
			string sRequestURL = signinRequest.RequestUrl;
			Debug.WriteLine(sRequestURL);
			return sRequestURL;
		}

		// 12/25/2018 Paul.  Logout should perform Azure or ADFS logout. 
		public static string AzureLogout(HttpContext Context)
		{
			HttpApplicationState Application = Context.Application;
			HttpRequest          Request     = Context.Request;

			string sAadTenantDomain  = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.AadTenantDomain"]);
			string sAuthority        = "https://login.microsoftonline.com/" + sAadTenantDomain + "/oauth2/logout";
			string sRedirectURL = "https://" + Request.Url.Host + Request.ApplicationPath + "/Users/Login.aspx?wa=wsignoutcleanup1.0";
			string sRequestURL = sAuthority + "?post_logout_redirect_uri=" + HttpUtility.UrlEncode(sRedirectURL);
			Debug.WriteLine(sRequestURL);
			return sRequestURL;
		}

		public static Guid AzureValidate(HttpApplicationState Application, string sToken, ref string sError)
		{
			string sAadTenantDomain    = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.AadTenantDomain"   ]);
			string sRealm              = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.Realm"             ]);
			string sFederationMetadata = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.FederationMetadata"]);
			string sAuthority          = "https://login.microsoftonline.com/" + sAadTenantDomain + "/wsfed";
			// 12/05/2018 Paul.  Allow authorization by USER_NAME instead of by EMAIL1. 
			bool   bAuthByUserName     = Sql.ToBoolean(Application["CONFIG.Azure.SingleSignOn.AuthByUserName"]);
			Guid   gUSER_ID            = Guid.Empty;
			try
			{
				SignInResponseMessage signinResponse = new System.IdentityModel.Services.SignInResponseMessage(new Uri(sAuthority), sToken);
				// 01/08/2017 Paul.  How to grab serialized in http request claims in a code using WIF?
				// http://oocms.org/question/2822274/how-to-grab-serialized-in-http-request-claims-in-a-code-using-wif
				//var message = SignInResponseMessage.CreateFromFormPost(Request) as SignInResponseMessage;

				RequestSecurityTokenResponse rstr = new WSFederationSerializer().CreateResponse(signinResponse, new WSTrustSerializationContext(SecurityTokenHandlerCollectionManager.CreateDefaultSecurityTokenHandlerCollectionManager()));

				// 01/08/2017 Paul.  Consider using System.IdentityModel.Tokens.ValidatingIssuerNameRegistry. 
				// ValidatingIssuerNameRegistry issuers = new System.IdentityModel.Tokens.ValidatingIssuerNameRegistry();
				// https://www.nuget.org/packages/System.IdentityModel.Tokens.ValidatingIssuerNameRegistry/
				
				IssuingAuthority issuingAuthority = Application["Azure.IssuingAuthority"] as IssuingAuthority;
				if ( issuingAuthority == null )
				{
					issuingAuthority = ValidatingIssuerNameRegistry.GetIssuingAuthority(sFederationMetadata);
					Application["Azure.IssuingAuthority"] = issuingAuthority;
				}
				ValidatingIssuerNameRegistry issuers = new ValidatingIssuerNameRegistry(issuingAuthority);

				Saml2SecurityTokenHandler tokenHandler = new System.IdentityModel.Tokens.Saml2SecurityTokenHandler {CertificateValidator = X509CertificateValidator.None};
				SecurityTokenHandlerConfiguration config = new SecurityTokenHandlerConfiguration { CertificateValidator = X509CertificateValidator.None, IssuerNameRegistry = issuers };

				config.AudienceRestriction.AllowedAudienceUris.Add(new Uri(sRealm));
				tokenHandler.Configuration = config;
				using ( XmlReader reader = XmlReader.Create(new StringReader(rstr.RequestedSecurityToken.SecurityTokenXml.OuterXml)) )
				{
					SecurityToken samlSecurityToken = tokenHandler.ReadToken(reader);
					// 01/08/2017 Paul.  ID4175 will be thrown if the thumbprint is incorrect. It must come from ADFS. 
					// ID4175: The issuer of the security token was not recognized by the IssuerNameRegistry. To accept security tokens from this issuer, configure the IssuerNameRegistry to return a valid name for this issuer.
					ReadOnlyCollection<System.Security.Claims.ClaimsIdentity> claimsIdentity = tokenHandler.ValidateToken(samlSecurityToken);
					if ( claimsIdentity.Count > 0 )
					{
						string sUSER_NAME  = String.Empty;
						string sLAST_NAME  = String.Empty;
						string sFIRST_NAME = String.Empty;
						string sEMAIL1     = String.Empty;
						//bool   bIsAdmin   = false;
						List<string> roles = new List<string>();
						System.Security.Claims.ClaimsIdentity identity = claimsIdentity[0];
						foreach ( System.Security.Claims.Claim claim in identity.Claims )
						{
							Debug.WriteLine(claim.Type + " = " + claim.Value);
							// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname = Rony
							// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname = Paul
							// http://schemas.microsoft.com/identity/claims/displayname = Paul Rony
							// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress = sales@splendidcrm.com
							// http://schemas.microsoft.com/identity/claims/identityprovider = live.com

							// 01/15/2017 Paul.  Alternate Azure login. 
							// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name = paul@splendidcrm.onmicrosoft.com
							// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname = Rony
							// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname = Paul
							// http://schemas.microsoft.com/identity/claims/displayname = Paul Rony
							// http://schemas.microsoft.com/identity/claims/identityprovider = https://sts.windows.net/2378bf60-1010-4140-9b4a-2a7312df5779/
							switch ( claim.Type )
							{
								case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"          :  sUSER_NAME  = claim.Value;  break;
								case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"       :  sLAST_NAME  = claim.Value;  break;
								case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"     :  sFIRST_NAME = claim.Value;  break;
								case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"  :  sEMAIL1     = claim.Value;  break;
							}
						}
						if ( Sql.IsEmptyString(sEMAIL1) && sUSER_NAME.Contains("@") )
							sEMAIL1 = sUSER_NAME;
						//if ( roles.Contains("Domain Admins") || roles.Contains("Administrators") || roles.Contains("SplendidCRM Administrators") )
						//	bIsAdmin = true;
						if ( !Sql.IsEmptyString(sEMAIL1) )
						{
							DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								string sSQL;
								// 12/05/2018 Paul.  Allow authorization by USER_NAME instead of by EMAIL1. 
								if ( bAuthByUserName )
								{
									sSQL = "select ID                    " + ControlChars.CrLf
									     + "  from vwUSERS_Login         " + ControlChars.CrLf
									     + " where USER_NAME = @EMAIL1   " + ControlChars.CrLf;
								}
								else
								{
									sSQL = "select ID                    " + ControlChars.CrLf
									     + "  from vwUSERS_Login         " + ControlChars.CrLf
									     + " where EMAIL1 = @EMAIL1      " + ControlChars.CrLf;
								}
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									cmd.CommandTimeout = 0;
									Sql.AddParameter(cmd, "@EMAIL1", sEMAIL1);
									gUSER_ID = Sql.ToGuid(cmd.ExecuteScalar());
									if ( Sql.IsEmptyGuid(gUSER_ID) )
									{
										SplendidInit.LoginTracking(Application, sUSER_NAME, false);
										sError = "SECURITY: failed attempted login for " + sEMAIL1 + " using Azure AD. ";
										SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), sError);
									}
								}
							}
						}
						else
						{
							sError = "SECURITY: Failed attempted login using Azure AD. Missing Email ID from Claim token.";
							SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), sError);
						}
					}
					else
					{
						sError = "SECURITY: failed attempted login using Azure AD. No SecurityToken identities found.";
						SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), sError);
					}
				}
			}
			catch(Exception ex)
			{
				sError = "SECURITY: failed attempted login using Azure AD. " + ex.Message;
				SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), sError);
			}
			return gUSER_ID;
		}

		// 05/02/2017 Paul.  Need a separate flag for the mobile client. 
		public static Guid AzureValidateJwt(HttpContext Context, string sToken, bool bMobileClient, ref string sError)
		{
			HttpApplicationState Application = Context.Application;
			HttpRequest          Request     = Context.Request;
			HttpSessionState     Session     = Context.Session;
			
			Guid gUSER_ID       = Guid.Empty;
			Guid gUSER_LOGIN_ID = Guid.Empty;
			try
			{
				//string sAadTenantDomain     = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.AadTenantDomain"   ]);
				string sAadClientId         = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.AadClientId"       ]);
				//string sRealm               = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.Realm"             ]);
				string sFederationMetadata  = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.FederationMetadata"]);
				//string stsDiscoveryEndpoint = "https://login.microsoftonline.com/" + sAadTenantDomain + "/.well-known/openid-configuration";
				// 05/03/2017 Paul.  Instead of validating against the resource, validate against the clientId as it is easier. 
				//string sResourceUrl = Request.Url.ToString();
				//sResourceUrl = sResourceUrl.Substring(0, sResourceUrl.Length - "Rest.svc/Login".Length);
				// 05/02/2017 Paul.  Need a separate flag for the mobile client. 
				// 12/05/2018 Paul.  Allow authorization by USER_NAME instead of by EMAIL1. 
				bool   bAuthByUserName     = Sql.ToBoolean(Application["CONFIG.Azure.SingleSignOn.AuthByUserName"]);
				if ( bMobileClient )
				{
					// 05/03/2017 Paul.  As we are using the MobileClientId to validate the token, we must also use it as the resourceUrl when acquiring the token. 
					sAadClientId   = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.MobileClientId"  ]);
				}

				// 02/14/2022 Paul.  Use the new metadata serializer. 
				// https://www.nuget.org/packages/Microsoft.IdentityModel.Protocols.WsFederation/
				Microsoft.IdentityModel.Protocols.WsFederation.WsFederationMetadataSerializer serializer = new Microsoft.IdentityModel.Protocols.WsFederation.WsFederationMetadataSerializer();
				Microsoft.IdentityModel.Protocols.WsFederation.WsFederationConfiguration metadata = Application["Azure.FederationMetadata"] as Microsoft.IdentityModel.Protocols.WsFederation.WsFederationConfiguration;
				if ( metadata == null )
				{
					metadata = serializer.ReadMetadata(XmlReader.Create(sFederationMetadata));
					Application["Azure.FederationMetadata"] = metadata;
				}
				
				// 02/14/2022 Paul.  Update System.IdentityModel.Tokens.Jwt to support Apple Signin. 
				// https://www.nuget.org/packages/System.IdentityModel.Tokens.Jwt/
				System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
				Microsoft.IdentityModel.Tokens.TokenValidationParameters validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
				{
					ValidIssuer         = metadata.Issuer,
					IssuerSigningKeys   = metadata.SigningKeys,
					ValidAudience       = sAadClientId
				};

				Microsoft.IdentityModel.Tokens.SecurityToken validatedToken = null;
				// Throws an Exception as the token is invalid (expired, invalid-formatted, etc.)
				System.Security.Claims.ClaimsPrincipal identity = tokenHandler.ValidateToken(sToken, validationParameters, out validatedToken);
				if ( identity != null )
				{
					string sUSER_NAME  = String.Empty;
					string sLAST_NAME  = String.Empty;
					string sFIRST_NAME = String.Empty;
					string sEMAIL1     = String.Empty;
					foreach ( System.Security.Claims.Claim claim in identity.Claims )
					{
						Debug.WriteLine(claim.Type + " = " + claim.Value);
						// http://schemas.microsoft.com/claims/authnmethodsreferences = pwd
						// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress = paul@splendidcrm.com
						// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname = Rony
						// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname = Paul
						// http://schemas.microsoft.com/identity/claims/identityprovider = live.com
						// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name = live.com#paul@splendidcrm.com
						// iat = 1484136100
						// nbf = 1484136100
						// exp = 1484140000
						// name = Paul Rony
						// platf = 3
						// ver = 1.0

						// 01/15/2017 Paul.  Alternate login. 
						// http://schemas.microsoft.com/claims/authnmethodsreferences = pwd
						// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname = Rony
						// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname = Paul
						// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name = paul@splendidcrm.onmicrosoft.com
						// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn = paul@splendidcrm.onmicrosoft.com
						// iat = 1484512667
						// nbf = 1484512667
						// exp = 1484516567
						// name = Paul Rony
						// platf = 3
						// ver = 1.0
						switch ( claim.Type )
						{
							// 12/25/2018 Paul.  Remove live.com# prefix. 
							case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"          :  sUSER_NAME  = claim.Value.Replace("live.com#", "");  break;
							case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"       :  sLAST_NAME  = claim.Value;  break;
							case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"     :  sFIRST_NAME = claim.Value;  break;
							case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"  :  sEMAIL1     = claim.Value;  break;
						}
					}
					if ( Sql.IsEmptyString(sEMAIL1) && sUSER_NAME.Contains("@") )
						sEMAIL1 = sUSER_NAME;
					if ( !Sql.IsEmptyString(sEMAIL1) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							// 12/05/2018 Paul.  Allow authorization by USER_NAME instead of by EMAIL1. 
							if ( bAuthByUserName )
							{
								sSQL = "select ID                    " + ControlChars.CrLf
								     + "  from vwUSERS_Login         " + ControlChars.CrLf
								     + " where USER_NAME = @EMAIL1   " + ControlChars.CrLf;
							}
							else
							{
								sSQL = "select ID                    " + ControlChars.CrLf
								     + "  from vwUSERS_Login         " + ControlChars.CrLf
								     + " where EMAIL1 = @EMAIL1      " + ControlChars.CrLf;
							}
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@EMAIL1", sEMAIL1.ToLower());
								gUSER_ID = Sql.ToGuid(cmd.ExecuteScalar());
								if ( Sql.IsEmptyGuid(gUSER_ID) )
								{
									// 01/13/2017 Paul.  Cannot log an unknown user. 
									//SqlProcs.spUSERS_LOGINS_InsertOnly(ref gUSER_LOGIN_ID, Guid.Empty, sEMAIL1, "Azure AD", "Failed", Session.SessionID, Request.UserHostName, Request.Url.Host, Request.Path, Request.AppRelativeCurrentExecutionFilePath, Request.UserAgent);
									// 01/13/2017 Paul.  Cannot lock-out an unknown user. 
									//SplendidInit.LoginTracking(Application, sEMAIL1, false);
									sError = "SECURITY: failed attempted login for " + sEMAIL1 + " using Azure AD/REST API";
									SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), sError);
								}
							}
						}
					}
					else
					{
						sError = "SECURITY: Failed attempted login using ADFS. Missing Email ID from Claim token.";
						SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), sError);
					}
				}
				else
				{
					sError = "SECURITY: failed attempted login using Azure AD. No SecurityToken identities found.";
					SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), sError);
				}
			}
			catch(Exception ex)
			{
				string sUSER_NAME = "(Unknown Azure AD)";
				// 01/13/2017 Paul.  Cannot log an unknown user. 
				//SqlProcs.spUSERS_LOGINS_InsertOnly(ref gUSER_LOGIN_ID, Guid.Empty, sUSER_NAME, "Azure AD", "Failed", Session.SessionID, Request.UserHostName, Request.Url.Host, Request.Path, Request.AppRelativeCurrentExecutionFilePath, Request.UserAgent);
				// 01/13/2017 Paul.  Cannot lock-out an unknown user. 
				//SplendidInit.LoginTracking(Application, sUSER_NAME, false);
				sError = "SECURITY: failed attempted login for " + sUSER_NAME + " using Azure AD/REST API. " + ex.Message;
				SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), sError);
			}
			return gUSER_ID;
		}

		public static string FederationServicesLogin(HttpContext Context)
		{
			HttpApplicationState Application = Context.Application;
			HttpRequest          Request     = Context.Request;

			string sRealm       = Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.Realm"    ]);
			string sAuthority   = Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.Authority"]);
			if ( !sAuthority.EndsWith("/") )
				sAuthority += "/";
			sAuthority += "adfs/ls";
			//string sRedirectURL = Request.Url.Scheme + "://" + Request.Url.Host + Request.ApplicationPath + "/Users/Login.aspx";
			string sRedirectURL = "https://" + Request.Url.Host + Request.ApplicationPath + "/Users/Login.aspx";
			// 07/08/2017 Paul.  We cannot support the Redirect as it is unlikely that the full redirect URL will be added to the App registration. 
			//if ( !Sql.IsEmptyString(Request["Redirect"]) )
			//	sRedirectURL += "?Redirect=" + HttpUtility.UrlEncode(Request["Redirect"]);
			// https://adfs.splendidcrm.com/adfs/ls/?wa=wsignin1.0&wtrealm=https%3a%2f%2flocalhost%2fSplendidCRM6_Azure%2f&wreply=http%3a%2f%2flocalhost%2fSplendidCRM6_Azure%2fUsers%2fLogin.aspx
			SignInRequestMessage signinRequest = new System.IdentityModel.Services.SignInRequestMessage(new Uri(sAuthority), sRealm, sRedirectURL);
			string sRequestURL = signinRequest.RequestUrl;
			Debug.WriteLine(sRequestURL);
			return sRequestURL;
		}

		// 12/25/2018 Paul.  Logout should perform Azure or ADFS logout. 
		public static string FederationServicesLogout(HttpContext Context)
		{
			HttpApplicationState Application = Context.Application;
			HttpRequest          Request     = Context.Request;

			string sAuthority   = Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.Authority"]);
			if ( !sAuthority.EndsWith("/") )
				sAuthority += "/";
			sAuthority += "adfs/ls";
			string sRedirectURL = "https://" + Request.Url.Host + Request.ApplicationPath + "/Users/Login.aspx?wa=wsignoutcleanup1.0";
			string sRequestURL = sAuthority + "?wa=wsignout1.0&wreply=" + HttpUtility.UrlEncode(sRedirectURL);
			Debug.WriteLine(sRequestURL);
			return sRequestURL;
		}

		public static Guid FederationServicesValidate(HttpContext Context, string sToken, ref string sError)
		{
			HttpApplicationState Application = Context.Application;
			HttpRequest          Request     = Context.Request;

			string sRealm              = Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.Realm"     ]);
			string sAuthority          = Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.Authority" ]);
			string sThumbprint         = Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.Thumbprint"]);
			// 12/20/2018 Paul.  Allow authorization by EMAIL1 instead of by USER_NAME. 
			bool   bAuthByEmail        = Sql.ToBoolean(Application["CONFIG.Azure.SingleSignOn.AuthByEmail"]);
			Guid   gUSER_ID            = Guid.Empty;
			if ( !sAuthority.EndsWith("/") )
				sAuthority += "/";
			try
			{
				SignInResponseMessage signinResponse = new System.IdentityModel.Services.SignInResponseMessage(new Uri(sAuthority), sToken);
				string sIssuerURL = sAuthority.Replace("https:", "http:") + "adfs/services/trust";
				// 01/08/2017 Paul.  How to grab serialized in http request claims in a code using WIF?
				// http://oocms.org/question/2822274/how-to-grab-serialized-in-http-request-claims-in-a-code-using-wif
				//var message = SignInResponseMessage.CreateFromFormPost(Request) as SignInResponseMessage;

				RequestSecurityTokenResponse rstr = new WSFederationSerializer().CreateResponse(signinResponse, new WSTrustSerializationContext(SecurityTokenHandlerCollectionManager.CreateDefaultSecurityTokenHandlerCollectionManager()));
				XmlNode xIssuer = rstr.RequestedSecurityToken.SecurityTokenXml.Attributes.GetNamedItem("Issuer");
				if ( xIssuer != null )
					sIssuerURL = xIssuer.InnerText;

				// 01/08/2017 Paul.  Consider using System.IdentityModel.Tokens.ValidatingIssuerNameRegistry. 
				// ValidatingIssuerNameRegistry issuers = new System.IdentityModel.Tokens.ValidatingIssuerNameRegistry();
				// https://www.nuget.org/packages/System.IdentityModel.Tokens.ValidatingIssuerNameRegistry/
				
				ConfigurationBasedIssuerNameRegistry issuers = new ConfigurationBasedIssuerNameRegistry();
				// 01/08/2017 Paul.  The thumbprint comes from ADFS.  Open AD FS 2.0 Management > Service > Certificates then right-click on the Primary Token-signing certificate and choose View certificate. 
				// 02/13/2019 Paul.  Another way to get the thumbprint is to use powershell Get-ADFSCertificate.  Use Token-Signing value. 
				// http://docs.sdl.com/LiveContent/content/en-US/SDL%20LiveContent%20full%20documentation-v1/GUID-0652296B-F1FF-4088-8258-3EAAE0CD2EEA
				issuers.AddTrustedIssuer(sThumbprint, sIssuerURL);

				SamlSecurityTokenHandler tokenHandler = new System.IdentityModel.Tokens.SamlSecurityTokenHandler {CertificateValidator = X509CertificateValidator.None};
				SecurityTokenHandlerConfiguration config = new SecurityTokenHandlerConfiguration { CertificateValidator = X509CertificateValidator.None, IssuerNameRegistry = issuers };

				config.AudienceRestriction.AllowedAudienceUris.Add(new Uri(sRealm));
				tokenHandler.Configuration = config;
				using ( XmlReader reader = XmlReader.Create(new StringReader(rstr.RequestedSecurityToken.SecurityTokenXml.OuterXml)) )
				{
					SecurityToken samlSecurityToken = tokenHandler.ReadToken(reader);
					// 01/08/2017 Paul.  ID4175 will be thrown if the thumbprint is incorrect. It must come from ADFS. 
					// ID4175: The issuer of the security token was not recognized by the IssuerNameRegistry. To accept security tokens from this issuer, configure the IssuerNameRegistry to return a valid name for this issuer.
					ReadOnlyCollection<System.Security.Claims.ClaimsIdentity> claimsIdentity = tokenHandler.ValidateToken(samlSecurityToken);
					if ( claimsIdentity.Count > 0 )
					{
						string sUSER_NAME  = String.Empty;
						string sLAST_NAME  = String.Empty;
						string sFIRST_NAME = String.Empty;
						string sEMAIL1     = String.Empty;
						//bool   bIsAdmin   = false;
						List<string> roles = new List<string>();
						System.Security.Claims.ClaimsIdentity identity = claimsIdentity[0];
						foreach ( System.Security.Claims.Claim claim in identity.Claims )
						{
							Debug.WriteLine(claim.Type + " = " + claim.Value);
							// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier = paulrony
							// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname = Rony
							// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress = Paul@splendidcrm.com
							// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname = Paul
							// http://schemas.microsoft.com/ws/2008/06/identity/claims/role = Domain Users
							// http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod = http://schemas.microsoft.com/ws/2008/06/identity/authenticationmethod/windows
							// http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationinstant = 2017-01-08T13:19:38.747Z
							switch ( claim.Type )
							{
								// 01/08/2019 Paul.  Our instructions say to map SAM-Account-Name to Name ID, not name. 
								case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier":  sUSER_NAME  = claim.Value;  break;
								case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"       :  sLAST_NAME  = claim.Value;  break;
								case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"     :  sFIRST_NAME = claim.Value;  break;
								case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"  :  sEMAIL1     = claim.Value;  break;
								case "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"        :  roles.Add(claim.Value);  break;
								case "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod" :  break;
								case "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationinstant":  break;
							}
						}
						if ( !Sql.IsEmptyString(sUSER_NAME) )
						{
							//if ( roles.Contains("Domain Admins") || roles.Contains("Administrators") || roles.Contains("SplendidCRM Administrators") )
							//	bIsAdmin = true;
							DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								string sSQL;
								sSQL = "select ID                    " + ControlChars.CrLf
								     + "  from vwUSERS_Login         " + ControlChars.CrLf
								     + " where USER_NAME = @USER_NAME" + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									cmd.CommandTimeout = 0;
									// 12/20/2018 Paul.  Allow authorization by EMAIL1 instead of by USER_NAME. 
									if ( bAuthByEmail )
										Sql.AddParameter(cmd, "@USER_NAME", sEMAIL1);
									else
										Sql.AddParameter(cmd, "@USER_NAME", sUSER_NAME);
									gUSER_ID = Sql.ToGuid(cmd.ExecuteScalar());
									if ( Sql.IsEmptyGuid(gUSER_ID) )
									{
										SplendidInit.LoginTracking(Application, sUSER_NAME, false);
										sError = "SECURITY: Failed attempted login for " + sUSER_NAME + " using ADFS.";
										SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), sError);
									}
								}
							}
						}
						else
						{
							sError = "SECURITY: Failed attempted login using ADFS. Missing Username/Name ID from Claim token.";
							SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), sError);
						}
					}
					else
					{
						sError = "SECURITY: Failed attempted login using ADFS. No SecurityToken identities found.";
						SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), sError);
					}
				}
			}
			catch(Exception ex)
			{
				sError = "SECURITY: Failed attempted login using ADFS. " + ex.Message;
				SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), sError);
			}
			return gUSER_ID;
		}

		// 01/13/2017 Paul.  This method use an undesireable approach to manually passing the username and password.  It is required for ADFS 3.0 on Windows Server 2012 R2. 
		public static Guid FederationServicesValidate(HttpContext Context, string sUSER_NAME, string sPASSWORD, ref string sError)
		{
			HttpApplicationState Application = Context.Application;
			HttpRequest          Request     = Context.Request;
			HttpSessionState     Session     = Context.Session;

			Guid gUSER_ID       = Guid.Empty;
			Guid gUSER_LOGIN_ID = Guid.Empty;
			// WCF and Identity in .NET 4.5: External Authentication with WS-Trust
			// https://leastprivilege.com/2012/11/16/wcf-and-identity-in-net-4-5-external-authentication-with-ws-trust/
			// How to validate ADFS SAML token
			// http://stackoverflow.com/questions/18701681/how-to-validate-adfs-saml-token

			string sRealm      = Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.Realm"     ]);
			string sAuthority  = Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.Authority" ]);
			string sDoamin     = Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.Domain"    ]);
			string sThumbprint = Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.Thumbprint"]);
			// 12/20/2018 Paul.  Allow authorization by EMAIL1 instead of by USER_NAME. 
			bool   bAuthByEmail = Sql.ToBoolean(Application["CONFIG.Azure.SingleSignOn.AuthByEmail"]);
			if ( !sAuthority.EndsWith("/") )
				sAuthority += "/";
			string sIssuerURL  = sAuthority.Replace("https:", "http:") + "adfs/services/trust";
			using ( WSTrustChannelFactory factory = new WSTrustChannelFactory(new Microsoft.IdentityModel.Protocols.WSTrust.Bindings.UserNameWSTrustBinding(System.ServiceModel.SecurityMode.TransportWithMessageCredential), new EndpointAddress(sAuthority + "adfs/services/trust/2005/usernamemixed")) )
			{
				try
				{
					string[] arrUserName = sUSER_NAME.Split('\\');
					string sUSER_DOMAIN = String.Empty;
					if ( arrUserName.Length > 1 )
					{
						sUSER_DOMAIN = arrUserName[0];
						sUSER_NAME   = arrUserName[1];
					}
					else
					{
						sUSER_DOMAIN = sDoamin;
						sUSER_NAME   = arrUserName[0];
					}
					factory.TrustVersion = TrustVersion.WSTrustFeb2005;
					factory.Credentials.UserName.UserName  = sUSER_DOMAIN + "\\" + sUSER_NAME;
					factory.Credentials.UserName.Password  = sPASSWORD ;
					var rst = new RequestSecurityToken
					{
						RequestType = System.IdentityModel.Protocols.WSTrust.RequestTypes.Issue,
						AppliesTo = new EndpointReference(sRealm),
						KeyType = KeyTypes.Bearer
					};
					IWSTrustChannelContract channel = factory.CreateChannel();
					GenericXmlSecurityToken genericToken = channel.Issue(rst) as GenericXmlSecurityToken;
					XmlNode xIssuer = genericToken.TokenXml.Attributes.GetNamedItem("Issuer");
					if ( xIssuer != null )
						sIssuerURL = xIssuer.InnerText;
					
					ConfigurationBasedIssuerNameRegistry issuers = new ConfigurationBasedIssuerNameRegistry();
					// 01/08/2017 Paul.  The thumbprint comes from ADFS.  Open AD FS 2.0 Management > Service > Certificates then right-click on the Primary Token-signing certificate and choose View certificate. 
					// 02/13/2019 Paul.  Another way to get the thumbprint is to use powershell Get-ADFSCertificate.  Use Token-Signing value. 
					// http://docs.sdl.com/LiveContent/content/en-US/SDL%20LiveContent%20full%20documentation-v1/GUID-0652296B-F1FF-4088-8258-3EAAE0CD2EEA
					issuers.AddTrustedIssuer(sThumbprint, sIssuerURL);
					
					SamlSecurityTokenHandler tokenHandler = new System.IdentityModel.Tokens.SamlSecurityTokenHandler {CertificateValidator = X509CertificateValidator.None};
					SecurityTokenHandlerConfiguration config = new SecurityTokenHandlerConfiguration { CertificateValidator = X509CertificateValidator.None, IssuerNameRegistry = issuers };
					config.AudienceRestriction.AllowedAudienceUris.Add(new Uri(sRealm));
					tokenHandler.Configuration = config;
					using ( XmlReader reader = XmlReader.Create(new StringReader(genericToken.TokenXml.OuterXml)) )
					{
						SecurityToken samlSecurityToken = tokenHandler.ReadToken(reader);
						// 01/08/2017 Paul.  ID4175 will be thrown if the thumbprint is incorrect. It must come from ADFS. 
						// ID4175: The issuer of the security token was not recognized by the IssuerNameRegistry. To accept security tokens from this issuer, configure the IssuerNameRegistry to return a valid name for this issuer.
						ReadOnlyCollection<System.Security.Claims.ClaimsIdentity> claimsIdentity = tokenHandler.ValidateToken(samlSecurityToken);
						if ( claimsIdentity.Count > 0 )
						{
							string sLAST_NAME  = String.Empty;
							string sFIRST_NAME = String.Empty;
							string sEMAIL1     = String.Empty;
							//bool   bIsAdmin   = false;
							List<string> roles = new List<string>();
							System.Security.Claims.ClaimsIdentity identity = claimsIdentity[0];
							foreach ( System.Security.Claims.Claim claim in identity.Claims )
							{
								Debug.WriteLine(claim.Type + " = " + claim.Value);
								// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier = paulrony
								// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname = Rony
								// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress = Paul@splendidcrm.com
								// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname = Paul
								// http://schemas.microsoft.com/ws/2008/06/identity/claims/role = Domain Users
								// http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod = http://schemas.microsoft.com/ws/2008/06/identity/authenticationmethod/windows
								// http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationinstant = 2017-01-08T13:19:38.747Z
								switch ( claim.Type )
								{
									// 01/08/2019 Paul.  Our instructions say to map SAM-Account-Name to Name ID, not name. 
									case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier":  sUSER_NAME  = claim.Value;  break;
									case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"       :  sLAST_NAME  = claim.Value;  break;
									case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"     :  sFIRST_NAME = claim.Value;  break;
									case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"  :  sEMAIL1     = claim.Value;  break;
									case "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"        :  roles.Add(claim.Value);  break;
									case "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod" :  break;
									case "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationinstant":  break;
								}
							}
							//if ( roles.Contains("Domain Admins") || roles.Contains("Administrators") || roles.Contains("SplendidCRM Administrators") )
							//	bIsAdmin = true;
							if ( !Sql.IsEmptyString(sUSER_NAME) )
							{
								DbProviderFactory dbf = DbProviderFactories.GetFactory();
								using ( IDbConnection con = dbf.CreateConnection() )
								{
									con.Open();
									string sSQL;
									sSQL = "select ID                    " + ControlChars.CrLf
									     + "  from vwUSERS_Login         " + ControlChars.CrLf
									     + " where USER_NAME = @USER_NAME" + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandText = sSQL;
										// 12/20/2018 Paul.  Allow authorization by EMAIL1 instead of by USER_NAME. 
										if ( bAuthByEmail )
											Sql.AddParameter(cmd, "@USER_NAME", sEMAIL1);
										else
											Sql.AddParameter(cmd, "@USER_NAME", sUSER_NAME.ToLower());
										gUSER_ID = Sql.ToGuid(cmd.ExecuteScalar());
										if ( Sql.IsEmptyGuid(gUSER_ID) )
										{
											SqlProcs.spUSERS_LOGINS_InsertOnly(ref gUSER_LOGIN_ID, Guid.Empty, sUSER_NAME, "ADFS", "Failed", Session.SessionID, Request.UserHostName, Request.Url.Host, Request.Path, Request.AppRelativeCurrentExecutionFilePath, Request.UserAgent);
											SplendidInit.LoginTracking(Application, sUSER_NAME, false);
											sError = "SECURITY: failed attempted login for " + sUSER_NAME + " using ADFS/REST API";
											SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), sError);
										}
									}
								}
							}
							else
							{
								sError = "SECURITY: failed attempted login using ADFS. Missing Username/Name ID from Claim token.";
								SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), sError);
							}
						}
						else
						{
							SqlProcs.spUSERS_LOGINS_InsertOnly(ref gUSER_LOGIN_ID, Guid.Empty, sUSER_NAME, "ADFS", "Failed", Session.SessionID, Request.UserHostName, Request.Url.Host, Request.Path, Request.AppRelativeCurrentExecutionFilePath, Request.UserAgent);
							SplendidInit.LoginTracking(Application, sUSER_NAME, false);
							sError = "SECURITY: failed attempted login for " + sUSER_NAME + " using ADFS/REST API. No SecurityToken identities found.";
							SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), sError);
						}
					}
				}
				catch(Exception ex)
				{
					SqlProcs.spUSERS_LOGINS_InsertOnly(ref gUSER_LOGIN_ID, Guid.Empty, sUSER_NAME, "ADFS", "Failed", Session.SessionID, Request.UserHostName, Request.Url.Host, Request.Path, Request.AppRelativeCurrentExecutionFilePath, Request.UserAgent);
					SplendidInit.LoginTracking(Application, sUSER_NAME, false);
					sError = "SECURITY: failed attempted login for " + sUSER_NAME + " using ADFS/REST API. " + ex.Message;
					SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), sError);
				}
			}
			return gUSER_ID;
		}

		// 05/02/2017 Paul.  Need a separate flag for the mobile client. 
		public static Guid FederationServicesValidateJwt(HttpContext Context, string sToken, bool bMobileClient, ref string sError)
		{
			HttpApplicationState Application = Context.Application;
			HttpRequest          Request     = Context.Request;
			HttpSessionState     Session     = Context.Session;

			Guid gUSER_ID       = Guid.Empty;
			Guid gUSER_LOGIN_ID = Guid.Empty;
			try
			{
				//string sRealm      = Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.Realm"     ]);
				string sAuthority  = Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.Authority" ]);
				string sClientId   = Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.ClientId"  ]);
				// 05/02/2017 Paul.  Need a separate flag for the mobile client. 
				//if ( bMobileClient )
				//	sClientId   = Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.MobileClientId"  ]);
				// 01/08/2018 Paul.  ADFS 3.0 will require us to register both client and mobile as valid audiences. 
				string sMobileClientId = Sql.ToString(Application["CONFIG.ADFS.SingleSignOn.MobileClientId"  ]);
				if ( !sAuthority.EndsWith("/") )
					sAuthority += "/";
				string sFederationMetadata  = sAuthority + "FederationMetadata/2007-06/FederationMetadata.xml";

				// 02/14/2022 Paul.  Use the new metadata serializer. 
				// https://www.nuget.org/packages/Microsoft.IdentityModel.Protocols.WsFederation/
				Microsoft.IdentityModel.Protocols.WsFederation.WsFederationMetadataSerializer serializer = new Microsoft.IdentityModel.Protocols.WsFederation.WsFederationMetadataSerializer();
				Microsoft.IdentityModel.Protocols.WsFederation.WsFederationConfiguration metadata = Application["ADFS.FederationMetadata"] as Microsoft.IdentityModel.Protocols.WsFederation.WsFederationConfiguration;
				if ( metadata == null )
				{
					metadata = serializer.ReadMetadata(XmlReader.Create(sFederationMetadata));
					Application["ADFS.FederationMetadata"] = metadata;
				}

				// 12/25/2018 Paul.  Not sure why server is using http instead of https.  
				// IDX10205: Issuer validation failed. Issuer: 'http://adfs4.splendidcrm.com/adfs/services/trust'. Did not match: validationParameters.ValidIssuer: 'https://adfs4.splendidcrm.com/adfs/services/trust' or validationParameters.ValidIssuers: 'null'.
				// IDX10204: Unable to validate issuer. validationParameters.ValidIssuer is null or whitespace AND validationParameters.ValidIssuers is null.
				StringList arrValidIssuers = new StringList();
				arrValidIssuers.Add(sAuthority + "adfs");
				arrValidIssuers.Add(sAuthority + "adfs/services/trust");
				arrValidIssuers.Add(sAuthority.Replace("https:", "http:") + "adfs/services/trust");
				// IDX10214: Audience validation failed. Audiences: 'urn:microsoft:userinfo'. Did not match:  validationParameters.ValidAudience: 'microsoft:identityserver:86a54b29-a28e-4bcb-9477-07e25a41ee24' or validationParameters.ValidAudiences: 'null'
				StringList arrAudiences = new StringList();
				arrAudiences.Add(sClientId);
				arrAudiences.Add("microsoft:identityserver:" + sClientId);
				// 01/08/2018 Paul.  ADFS 3.0 will require us to register both client and mobile as valid audiences. 
				if ( sClientId != sMobileClientId && !Sql.IsEmptyString(sMobileClientId) )
				{
					arrAudiences.Add(sMobileClientId);
					arrAudiences.Add("microsoft:identityserver:" + sMobileClientId);
				}
				arrAudiences.Add("urn:microsoft:userinfo");
				// 02/14/2019 Paul.  Use Grant-AdfsApplicationPermission to grant the plug-in access to the resource. 
				// https://community.dynamics.com/crm/f/117/t/246239
				// Grant-AdfsApplicationPermission -ClientRoleIdentifier "0dff791c-21b4-49cd-b3be-e7c37d29d6c0" -ServerRoleIdentifier "https://SplendidPlugin"
				// 02/14/2019 Paul.  https://SplendidPlugin is hardcoded to the Outlook and Word plug-ins. 
				arrAudiences.Add("https://SplendidPlugin");
				// 02/14/2019 Paul.  Lets include survey and mobile. 
				arrAudiences.Add("https://SplendidMobile");
				arrAudiences.Add("https://auth.expo.io/@splendidcrm/splendidsurvey");
				// 02/14/2022 Paul.  Update System.IdentityModel.Tokens.Jwt to support Apple Signin. 
				// https://www.nuget.org/packages/System.IdentityModel.Tokens.Jwt/
				System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
				Microsoft.IdentityModel.Tokens.TokenValidationParameters validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
				{
					//ValidIssuer         = sAuthority + "adfs",
					ValidIssuers        = arrValidIssuers,
					ValidAudiences      = arrAudiences,
					IssuerSigningKeys   = metadata.SigningKeys
				};

				Microsoft.IdentityModel.Tokens.SecurityToken validatedToken = null;
				//validatedToken = tokenHandler.ReadToken(sToken);
				// Throws an Exception as the token is invalid (expired, invalid-formatted, etc.)
				System.Security.Claims.ClaimsPrincipal identity = tokenHandler.ValidateToken(sToken, validationParameters, out validatedToken);
				if ( identity != null )
				{
					string sUSER_NAME  = String.Empty;
					string sFIRST_NAME = String.Empty;
					string sLAST_NAME  = String.Empty;
					string sEMAIL1     = String.Empty;
					foreach ( System.Security.Claims.Claim claim in identity.Claims )
					{
						//Debug.WriteLine(claim.Type + " = " + claim.Value);
						// http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationinstant = 1484346928
						// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier = zwkv1IHDGSe1FDyDrc6LO2+XxDD0LWfs1SL35ZdOxF0=
						// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn = paulrony@merchantware.local
						// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name = MERCHANTWARE\paulrony
						// aud = FD3ABD16-F96F-4BE7-98DB-D45C55DB0048
						// iss = https://adfs4.splendidcrm.com/adfs
						// iat = 1484346928
						// exp = 1484350528
						// nonce = 8f864f39-b44e-4d02-9cb2-38d5113643af
						switch ( claim.Type )
						{
							case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"          :  sUSER_NAME  = claim.Value;  break;
							// 01/08/2019 Paul.  Our instructions say to map SAM-Account-Name to Name ID, not name. 
							case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier":  sUSER_NAME  = claim.Value;  break;
							case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"       :  sLAST_NAME  = claim.Value;  break;
							case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"     :  sFIRST_NAME = claim.Value;  break;
							case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"  :  sEMAIL1     = claim.Value;  break;
						}
					}
					string[] arrUserName = sUSER_NAME.Split('\\');
					if ( arrUserName.Length > 1 )
						sUSER_NAME   = arrUserName[1];
					else
						sUSER_NAME   = arrUserName[0];
					if ( !Sql.IsEmptyString(sUSER_NAME) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select ID                    " + ControlChars.CrLf
							     + "  from vwUSERS_Login         " + ControlChars.CrLf
							     + " where USER_NAME = @USER_NAME" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_NAME", sUSER_NAME.ToLower());
								gUSER_ID = Sql.ToGuid(cmd.ExecuteScalar());
								if ( Sql.IsEmptyGuid(gUSER_ID) )
								{
									// 01/13/2017 Paul.  Cannot log an unknown user. 
									//SqlProcs.spUSERS_LOGINS_InsertOnly(ref gUSER_LOGIN_ID, Guid.Empty, sEMAIL1, "Azure AD", "Failed", Session.SessionID, Request.UserHostName, Request.Url.Host, Request.Path, Request.AppRelativeCurrentExecutionFilePath, Request.UserAgent);
									// 01/13/2017 Paul.  Cannot lock-out an unknown user. 
									//SplendidInit.LoginTracking(Application, sEMAIL1, false);
									sError = "SECURITY: failed attempted login for " + sUSER_NAME + " using ADFS/REST API.";
									SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), sError);
								}
							}
						}
					}
					else
					{
						sError = "SECURITY: Failed attempted login using ADFS/REST API. Missing Username/Name ID from Claim token.";
						SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), sError);
					}
				}
				else
				{
					sError = "SECURITY: failed attempted login using ADFS/REST API. No SecurityToken identities found.";
					SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), sError);
				}
			}
			catch(Exception ex)
			{
				string sUSER_NAME = "(Unknown ADFS)";
				// 01/13/2017 Paul.  Cannot log an unknown user. 
				//SqlProcs.spUSERS_LOGINS_InsertOnly(ref gUSER_LOGIN_ID, Guid.Empty, sUSER_NAME, "Azure AD", "Failed", Session.SessionID, Request.UserHostName, Request.Url.Host, Request.Path, Request.AppRelativeCurrentExecutionFilePath, Request.UserAgent);
				// 01/13/2017 Paul.  Cannot lock-out an unknown user. 
				//SplendidInit.LoginTracking(Application, sUSER_NAME, false);
				sError = "SECURITY: failed attempted login for " + sUSER_NAME + " using ADFS/REST API. " + ex.Message;
				SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), sError);
			}
			return gUSER_ID;
		}

		// 11/09/2019 Paul.  Pass the RedirectURL so that we can call from the React client. 
		// 02/04/2023 Paul.  Directory Tenant is now required for single tenant app registrations. 
		public static Office365AccessToken Office365AcquireAccessToken(HttpContext Context, string sOAuthDirectoryTenatID, string sOAuthClientID, string sOAuthClientSecret, Guid gUSER_ID, string sAuthorizationCode, string sRedirect)
		{
			HttpApplicationState Application = Context.Application;
			HttpRequest          Request     = Context.Request;
			Office365AccessToken token       = null;
			string sOAuthAccessToken  = String.Empty;
			string sOAuthRefreshToken = String.Empty;
			string sOAuthExpiresAt    = String.Empty;
			try
			{
				DateTime dtOAuthExpiresAt = DateTime.MinValue;
				if ( Sql.IsEmptyString(sRedirect) )
					sRedirect = Request.Url.Scheme + "://" + Request.Url.Host + Sql.ToString(Application["rootURL"]) + "OAuth/Office365Landing.aspx";
				
				DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					try
					{
						// 02/10/2017 Paul.  Change to https://login.microsoftonline.com. 
						// https://blogs.technet.microsoft.com/enterprisemobility/2015/03/06/simplifying-our-azure-ad-authentication-flows/
						// 11/29/2020 Paul.  Update to version 2, https://docs.microsoft.com/en-us/graph/auth-v2-user
						// 02/04/2023 Paul.  Directory Tenant is now required for single tenant app registrations. 
						if ( Sql.IsEmptyString(sOAuthDirectoryTenatID) )
							sOAuthDirectoryTenatID = "common";
						WebRequest webRequest = WebRequest.Create("https://login.microsoftonline.com/" + sOAuthDirectoryTenatID + "/oauth2/v2.0/token");
						webRequest.ContentType = "application/x-www-form-urlencoded";
						webRequest.Method = "POST";
						string scope = Spring.Social.Office365.Office365Sync.scope;
						// https://docs.microsoft.com/en-us/azure/active-directory/active-directory-v2-protocols-oauth-code
						// https://blogs.msdn.microsoft.com/exchangedev/2014/03/25/using-oauth2-to-access-calendar-contact-and-mail-api-in-office-365-exchange-online/
						string requestDetails = "grant_type=authorization_code&scope=" + HttpUtility.UrlEncode(scope) + "&code=" + HttpUtility.UrlEncode(sAuthorizationCode) + "&redirect_uri=" + HttpUtility.UrlEncode(sRedirect) + "&client_id=" + sOAuthClientID + "&client_secret=" + HttpUtility.UrlEncode(sOAuthClientSecret);
						byte[] bytes = System.Text.Encoding.ASCII.GetBytes(requestDetails);
						webRequest.ContentLength = bytes.Length;
						using ( Stream outputStream = webRequest.GetRequestStream() )
						{
							outputStream.Write(bytes, 0, bytes.Length);
						}
						using ( WebResponse webResponse = webRequest.GetResponse() )
						{
							DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Office365AccessToken));
							token = (Office365AccessToken)serializer.ReadObject(webResponse.GetResponseStream());
						}
					}
					catch
					{
						// 01/16/2017 Paul.  If the refresh fails, delete the database record so that we will not retry the sync. 
						using ( IDbTransaction trn = Sql.BeginTransaction(con) )
						{
							try
							{
								SqlProcs.spOAUTH_TOKENS_Delete(gUSER_ID, "Office365", trn);
								trn.Commit();
							}
							catch
							{
								trn.Rollback();
							}
						}
						throw;
					}
					sOAuthAccessToken  = token.AccessToken ;
					sOAuthRefreshToken = token.RefreshToken;
					dtOAuthExpiresAt   = DateTime.Now.AddSeconds(token.ExpiresInSeconds);
					sOAuthExpiresAt    = dtOAuthExpiresAt.ToShortDateString() + " " + dtOAuthExpiresAt.ToShortTimeString();
					Application["CONFIG.Office365." + gUSER_ID.ToString() + ".OAuthAccessToken" ] = sOAuthAccessToken ;
					Application["CONFIG.Office365." + gUSER_ID.ToString() + ".OAuthRefreshToken"] = sOAuthRefreshToken;
					Application["CONFIG.Office365." + gUSER_ID.ToString() + ".OAuthExpiresAt"   ] = sOAuthExpiresAt   ;
					using ( IDbTransaction trn = Sql.BeginTransaction(con) )
					{
						try
						{
							// 01/19/2017 Paul.  Name must match SEND_TYPE. 
							SqlProcs.spOAUTH_TOKENS_Update(gUSER_ID, "Office365", sOAuthAccessToken, String.Empty, dtOAuthExpiresAt, sOAuthRefreshToken, trn);
							trn.Commit();
						}
						catch
						{
							trn.Rollback();
							throw;
						}
					}
				}
			}
			catch
			{
				Application.Remove("CONFIG.Office365." + gUSER_ID.ToString() + ".OAuthAccessToken" );
				Application.Remove("CONFIG.Office365." + gUSER_ID.ToString() + ".OAuthRefreshToken");
				Application.Remove("CONFIG.Office365." + gUSER_ID.ToString() + ".OAuthExpiresAt"   );
				throw;
			}
			return token;
		}

		// 02/04/2023 Paul.  Directory Tenant is now required for single tenant app registrations. 
		public static Office365AccessToken Office365RefreshAccessToken(HttpApplicationState Application, string sOAuthDirectoryTenatID, string sOAuthClientID, string sOAuthClientSecret, Guid gUSER_ID, bool bForceRefresh)
		{
			Office365AccessToken token = null;
			string sOAuthAccessToken  = Sql.ToString(Application["CONFIG.Office365." + gUSER_ID.ToString() + ".OAuthAccessToken" ]);
			string sOAuthRefreshToken = Sql.ToString(Application["CONFIG.Office365." + gUSER_ID.ToString() + ".OAuthRefreshToken"]);
			string sOAuthExpiresAt    = Sql.ToString(Application["CONFIG.Office365." + gUSER_ID.ToString() + ".OAuthExpiresAt"   ]);
			DateTime dtOAuthExpiresAt = Sql.ToDateTime(sOAuthExpiresAt);
			try
			{
				// 01/10/2021 Paul.  We were getting expired token errors, so try and get the profile prior to a sync operation. 
				if ( !(Sql.IsEmptyString(sOAuthAccessToken) || dtOAuthExpiresAt == DateTime.MinValue ||  DateTime.Now > dtOAuthExpiresAt || bForceRefresh) )
				{
					Spring.Social.Office365.Api.IOffice365 office365 = Spring.Social.Office365.Office365Sync.CreateApi(Application, sOAuthAccessToken);
					// 07/18/2023 Paul.  Provide debug information. 
					Spring.Social.Office365.Api.MyProfile profile = office365.MyProfileOperations.GetMyProfile();
					Debug.WriteLine("Office365RefreshAccessToken: " + profile.UserPrincipalName + " " + profile.DisplayName);
				}
			}
			catch
			{
				bForceRefresh = true;
			}
			try
			{
				// 07/09/2018 Paul.  Decrease forward looking to 6 minutes. 
				if ( Sql.IsEmptyString(sOAuthAccessToken) || dtOAuthExpiresAt == DateTime.MinValue ||  DateTime.Now > dtOAuthExpiresAt || bForceRefresh )
				{
					Application.Remove("CONFIG.Office365." + gUSER_ID.ToString() + ".OAuthAccessToken" );
					Application.Remove("CONFIG.Office365." + gUSER_ID.ToString() + ".OAuthRefreshToken");
					Application.Remove("CONFIG.Office365." + gUSER_ID.ToString() + ".OAuthExpiresAt"   );
					
					sOAuthAccessToken = String.Empty;
					dtOAuthExpiresAt  = DateTime.MinValue;
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						if ( Sql.IsEmptyString(sOAuthAccessToken) )
						{
							string sSQL = String.Empty;
							sSQL = "select TOKEN                               " + ControlChars.CrLf
							     + "     , TOKEN_EXPIRES_AT                    " + ControlChars.CrLf
							     + "     , REFRESH_TOKEN                       " + ControlChars.CrLf
							     + "  from vwOAUTH_TOKENS                      " + ControlChars.CrLf
							     + " where NAME             = @NAME            " + ControlChars.CrLf
							     + "   and ASSIGNED_USER_ID = @ASSIGNED_USER_ID" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@NAME"            , "Office365");
								Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gUSER_ID    );
								using ( IDataReader rdr = cmd.ExecuteReader() )
								{
									if ( rdr.Read() )
									{
										sOAuthAccessToken  = Sql.ToString  (rdr["TOKEN"           ]);
										sOAuthRefreshToken = Sql.ToString  (rdr["REFRESH_TOKEN"   ]);
										dtOAuthExpiresAt   = Sql.ToDateTime(rdr["TOKEN_EXPIRES_AT"]);
										sOAuthExpiresAt    = dtOAuthExpiresAt.ToShortDateString() + " " + dtOAuthExpiresAt.ToShortTimeString();
									}
								}
							}
						}
						if ( Sql.IsEmptyString(sOAuthAccessToken) )
						{
							throw(new Exception("Office 365 Access Token does not exist for user " + gUSER_ID.ToString()));
						}
						else if ( Sql.IsEmptyString(sOAuthRefreshToken) )
						{
							throw(new Exception("Office 365 Refresh Token does not exist for user " + gUSER_ID.ToString()));
						}
						// 07/09/2018 Paul.  Decrease forward looking to 6 minutes. 
						else if ( dtOAuthExpiresAt == DateTime.MinValue ||  DateTime.Now > dtOAuthExpiresAt || bForceRefresh )
						{
							// 02/10/2017 Paul.  One endpoint to rule them all does not work with ExchangeService. https://graph.microsoft.io/en-us/
							// 02/10/2017 Paul.  Use new endpoint. https://msdn.microsoft.com/en-us/office/office365/api/use-outlook-rest-api
							try
							{
								// 02/10/2017 Paul.  Change to https://login.microsoftonline.com. 
								// https://blogs.technet.microsoft.com/enterprisemobility/2015/03/06/simplifying-our-azure-ad-authentication-flows/
								// 11/29/2020 Paul.  Update to version 2, https://docs.microsoft.com/en-us/graph/auth-v2-user
								// 02/04/2023 Paul.  Directory Tenant is now required for single tenant app registrations. 
								if ( Sql.IsEmptyString(sOAuthDirectoryTenatID) )
									sOAuthDirectoryTenatID = "common";
								WebRequest webRequest = WebRequest.Create("https://login.microsoftonline.com/" + sOAuthDirectoryTenatID + "/oauth2/v2.0/token");
								webRequest.ContentType = "application/x-www-form-urlencoded";
								webRequest.Method = "POST";
								string scope = Spring.Social.Office365.Office365Sync.scope;
								// https://docs.microsoft.com/en-us/azure/active-directory/active-directory-v2-protocols-oauth-code
								// https://blogs.msdn.microsoft.com/exchangedev/2014/03/25/using-oauth2-to-access-calendar-contact-and-mail-api-in-office-365-exchange-online/
								
								// 11/29/2020 Paul.  The docs say the redirect_url is required, but the call succeeds without it.  Maybe it is succeeding because the token has not expired during our tests. 
								//  The problem with supplying the original URL is that it can come from the ASP.NET site or from the React Client. 
								string requestDetails = "grant_type=refresh_token&refresh_token=" + HttpUtility.UrlEncode(sOAuthRefreshToken) + "&scope=" + HttpUtility.UrlEncode(scope) + "&client_id=" + sOAuthClientID + "&client_secret=" + HttpUtility.UrlEncode(sOAuthClientSecret);
								byte[] bytes = System.Text.Encoding.ASCII.GetBytes(requestDetails);
								webRequest.ContentLength = bytes.Length;
								using ( Stream outputStream = webRequest.GetRequestStream() )
								{
									outputStream.Write(bytes, 0, bytes.Length);
								}
								using ( WebResponse webResponse = webRequest.GetResponse() )
								{
									/*
									using ( StreamReader stm = new StreamReader(webResponse.GetResponseStream()) )
									{
										sResponse = stm.ReadToEnd();
										JavaScriptSerializer json = new JavaScriptSerializer();
										Spring.Social.Office365.AccessToken token = json.Deserialize<Spring.Social.Office365.AccessToken>(sResponse);
										OAUTH_REFRESH_TOKEN.Text = token.refresh_token;
									}
									*/
									DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Office365AccessToken));
									token = (Office365AccessToken)serializer.ReadObject(webResponse.GetResponseStream());
								}
							}
							catch(Exception ex)
							{
								// 05/12/2023 Paul.  Keep track of failures and don't delete token immediately. 
								int nRefreshFailures    = 1;
								if ( dictUserRefreshFailures.ContainsKey(gUSER_ID) )
									nRefreshFailures = dictUserRefreshFailures[gUSER_ID] + 1;
								dictUserRefreshFailures[gUSER_ID] = nRefreshFailures;
								// 02/19/2021 Paul.  Token refresh is working most of the time, so maybe we can just ignore this error and retry a few minutes later. 
								if ( !ex.Message.Contains("The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel.") )
								{
									int nMaxRefreshFailures = Sql.ToInteger(Application["CONFIG.Office365.MaxRefreshFailures"]);
									if ( nMaxRefreshFailures == 0 )
										nMaxRefreshFailures = 12;
									if ( nRefreshFailures >= nMaxRefreshFailures )
									{
										// 01/16/2017 Paul.  If the refresh fails, delete the database record so that we will not retry the sync. 
										using ( IDbTransaction trn = Sql.BeginTransaction(con) )
										{
											try
											{
												SqlProcs.spOAUTH_TOKENS_Delete(gUSER_ID, "Office365", trn);
												trn.Commit();
											}
											catch
											{
												trn.Rollback();
											}
										}
										//if ( dictUserRefreshFailures.ContainsKey(gUSER_ID) )
										//	dictUserRefreshFailures.Remove(gUSER_ID);
									}
								}
								// 07/05/2018 Paul.  Add better error when refresh token fails. 
								throw(new Exception("Office 365 Refresh Token failed (" + nRefreshFailures.ToString() + " times) for user " + gUSER_ID.ToString(), ex));
							}
							// 05/12/2023 Paul.  Keep track of failures and don't delete token immediately. 
							if ( dictUserRefreshFailures.ContainsKey(gUSER_ID) )
								dictUserRefreshFailures.Remove(gUSER_ID);
							
							sOAuthAccessToken  = token.AccessToken ;
							sOAuthRefreshToken = token.RefreshToken;
							// 07/09/2018 Paul.  Office365 returns 3599, so token expires in 1 hour. 
							dtOAuthExpiresAt   = DateTime.Now.AddSeconds(token.ExpiresInSeconds);
							sOAuthExpiresAt    = dtOAuthExpiresAt.ToShortDateString() + " " + dtOAuthExpiresAt.ToShortTimeString();
							Application["CONFIG.Office365." + gUSER_ID.ToString() + ".OAuthAccessToken" ] = sOAuthAccessToken ;
							Application["CONFIG.Office365." + gUSER_ID.ToString() + ".OAuthRefreshToken"] = sOAuthRefreshToken;
							Application["CONFIG.Office365." + gUSER_ID.ToString() + ".OAuthExpiresAt"   ] = sOAuthExpiresAt   ;
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								try
								{
									// 01/19/2017 Paul.  Name must match SEND_TYPE. 
									SqlProcs.spOAUTH_TOKENS_Update(gUSER_ID, "Office365", sOAuthAccessToken, String.Empty, dtOAuthExpiresAt, sOAuthRefreshToken, trn);
									trn.Commit();
								}
								catch
								{
									trn.Rollback();
									throw;
								}
							}
						}
						else
						{
							Application["CONFIG.Office365." + gUSER_ID.ToString() + ".OAuthAccessToken" ] = sOAuthAccessToken ;
							Application["CONFIG.Office365." + gUSER_ID.ToString() + ".OAuthRefreshToken"] = sOAuthRefreshToken;
							Application["CONFIG.Office365." + gUSER_ID.ToString() + ".OAuthExpiresAt"   ] = sOAuthExpiresAt   ;
							token = new Office365AccessToken();
							token.AccessToken      = sOAuthAccessToken ;
							token.RefreshToken     = sOAuthRefreshToken;
							token.ExpiresInSeconds = Convert.ToInt64((dtOAuthExpiresAt - DateTime.Now).TotalSeconds);
							token.TokenType        = "Bearer";
						}
					}
				}
				else
				{
					token = new Office365AccessToken();
					token.AccessToken      = sOAuthAccessToken ;
					token.RefreshToken     = sOAuthRefreshToken;
					token.ExpiresInSeconds = Convert.ToInt64((dtOAuthExpiresAt - DateTime.Now).TotalSeconds);
					token.TokenType        = "Bearer";
				}
			}
			catch
			{
				Application.Remove("CONFIG.Office365." + gUSER_ID.ToString() + ".OAuthAccessToken" );
				Application.Remove("CONFIG.Office365." + gUSER_ID.ToString() + ".OAuthRefreshToken");
				Application.Remove("CONFIG.Office365." + gUSER_ID.ToString() + ".OAuthExpiresAt"   );
				throw;
			}
			return token;
		}

		// 02/04/2023 Paul.  Directory Tenant is now required for single tenant app registrations. 
		public static bool Office365TestAccessToken(HttpApplicationState Application, string sOAuthDirectoryTenatID, string sOAuthClientID, string sOAuthClientSecret, Guid gUSER_ID, StringBuilder sbErrors)
		{
			bool bValidSource = false;
			try
			{
				Office365AccessToken token = Office365RefreshAccessToken(Application, sOAuthDirectoryTenatID, sOAuthClientID, sOAuthClientSecret, gUSER_ID, false);
				// 02/10/2017 Paul.  https://outlook.office.com/EWS/Exchange.asmx does not work. 
				string sSERVER_URL = "https://outlook.office365.com/EWS/Exchange.asmx";
				if ( !Sql.IsEmptyString(sSERVER_URL) )
				{
					/*
					ExchangeVersion version = ExchangeVersion.Exchange2013_SP1;
					ExchangeService service = new ExchangeService(version, TimeZoneInfo.Utc);
					// How to: Authenticate an EWS application by using OAuth
					// https://msdn.microsoft.com/en-us/library/office/dn903761(v=exchg.150).aspx
					service.Credentials = new OAuthCredentials(token.access_token);
					// 08/30/2013 Paul.  Office365 requires that we use auto-discover to get the server URL. 
					//if ( sSERVER_URL.ToLower().StartsWith("autodiscover") && !Sql.IsEmptyString(sUSER_NAME) )
					//{
					//	service.AutodiscoverUrl(sUSER_NAME, delegate (String redirectionUrl)
					//	{
					//		return (redirectionUrl == "https://autodiscover-s.outlook.com/autodiscover/autodiscover.xml");
					//	});
					//	sbErrors.AppendLine("Using AutodiscoverURL: " + service.Url + ".  ");
					//}
					//else
					{
						service.Url = new Uri(sSERVER_URL);
					}
					Folder fldInbox = Folder.Bind(service, WellKnownFolderName.Inbox);
					int nUnreadCount = fldInbox.UnreadCount;
					*/
					// 08/09/2018 Paul.  Allow translation of connection success. 
					Spring.Social.Office365.Api.IOffice365 office365 = Spring.Social.Office365.Office365Sync.CreateApi(Application, token.access_token);
					IList<Spring.Social.Office365.Api.MailFolder> folders = office365.FolderOperations.GetAll(String.Empty);
					int nUnreadCount = office365.MailOperations.GetInboxUnreadCount();
					office365.ContactOperations.GetCount();
					office365.EventOperations.GetCount();

					string sCULTURE = Sql.ToString(Application["CONFIG.default_language"]);
					if ( HttpContext.Current != null && HttpContext.Current.Session != null )
						sCULTURE = Sql.ToString (HttpContext.Current.Session["USER_SETTINGS/CULTURE"]);
					sbErrors.AppendLine(String.Format(L10N.Term(Application, sCULTURE, "Users.LBL_CONNECTION_SUCCESSFUL"), nUnreadCount.ToString(), "Inbox"));
					//sbErrors.AppendLine("Connection successful. " + nUnreadCount.ToString() + " items in Inbox" + "<br />");
				}
			}
			catch(Exception ex)
			{
				sbErrors.AppendLine(ex.Message);
			}
			return bValidSource;
		}

		// https://blogs.msdn.microsoft.com/laurelle/2016/02/12/how-to-use-microsoft-graph-and-office-365-api-in-a-service-or-in-a-windows-appuwp-without-a-graphical-interface/
		// https://graph.microsoft.io/en-us/docs
		/*
		public static MicrosoftGraphProfile GetProfile(HttpApplicationState Application, string sAccessToken)
		{
			MicrosoftGraphProfile profile = null;
			try
			{
				// 02/10/2017 Paul.  Could not get user profile. 
				// https://msdn.microsoft.com/en-us/office/office365/api/use-outlook-rest-api
				 string sURL = "https://outlook.office.com/api/v1.0/me";
				// 02/10/2017 Paul.  Not working.  Not authorize for resource https://outlook.office365.com/. 
				// string sURL = "https://outlook.office365.com/api/v1.0/me";
				// string sURL = "https://graph.microsoft.com/v1.0/me";
				WebRequest webRequest = WebRequest.Create(sURL);
				webRequest.ContentType = "application/json";
				webRequest.CachePolicy =  new  System.Net.Cache.HttpRequestCachePolicy(System.Net.Cache.HttpRequestCacheLevel.NoCacheNoStore);
				webRequest.Method      = "GET";
				webRequest.PreAuthenticate = true;
				webRequest.Headers.Add("Authorization", "Bearer " + sAccessToken);
			
				using ( WebResponse webResponse = webRequest.GetResponse() )
				{
					//DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(MicrosoftGraphProfile));
					//profile = (MicrosoftGraphProfile) serializer.ReadObject(webResponse.GetResponseStream());
					using ( StreamReader readStream = new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8) )
					{
						string sResponse = readStream.ReadToEnd();
						Debug.WriteLine(sResponse);
					}
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
			return profile;
		}
		*/

		// 02/10/2017 Paul.  Cannot find the API to get the user profile, so extract from the AccessToken. 
		public static MicrosoftGraphProfile GetProfile(HttpApplicationState Application, string sToken)
		{
			MicrosoftGraphProfile profile = null;
			//string sAadTenantDomain    = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.AadTenantDomain"   ]);
			//string sRealm              = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.Realm"             ]);
			// 02/10/2017 Paul.  The FederationMetadata comes from the Azure Portal under the Application Registration / Endpoints. 
			string sFederationMetadata = Sql.ToString(Application["CONFIG.Azure.SingleSignOn.FederationMetadata"]);
			try
			{
				if ( !Sql.IsEmptyString(sFederationMetadata) )
				{
					// 02/14/2022 Paul.  Use the new metadata serializer. 
					// https://www.nuget.org/packages/Microsoft.IdentityModel.Protocols.WsFederation/
					Microsoft.IdentityModel.Protocols.WsFederation.WsFederationMetadataSerializer serializer = new Microsoft.IdentityModel.Protocols.WsFederation.WsFederationMetadataSerializer();
					Microsoft.IdentityModel.Protocols.WsFederation.WsFederationConfiguration metadata = Application["Azure.FederationMetadata"] as Microsoft.IdentityModel.Protocols.WsFederation.WsFederationConfiguration;
					if ( metadata == null )
					{
						metadata = serializer.ReadMetadata(XmlReader.Create(sFederationMetadata));
						Application["Azure.FederationMetadata"] = metadata;
					}

					// 02/14/2022 Paul.  Update System.IdentityModel.Tokens.Jwt to support Apple Signin. 
					// https://www.nuget.org/packages/System.IdentityModel.Tokens.Jwt/
					System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
					Microsoft.IdentityModel.Tokens.TokenValidationParameters validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
					{
						ValidIssuer         = metadata.Issuer,
						IssuerSigningKeys   = metadata.SigningKeys,
						ValidAudience       = "https://outlook.office.com"
					};

					Microsoft.IdentityModel.Tokens.SecurityToken validatedToken = null;
					// Throws an Exception as the token is invalid (expired, invalid-formatted, etc.)
					System.Security.Claims.ClaimsPrincipal identity = tokenHandler.ValidateToken(sToken, validationParameters, out validatedToken);
					if ( identity != null )
					{
						string sUSER_NAME  = String.Empty;
						string sLAST_NAME  = String.Empty;
						string sFIRST_NAME = String.Empty;
						string sEMAIL1     = String.Empty;
						foreach ( System.Security.Claims.Claim claim in identity.Claims )
						{
							Debug.WriteLine(claim.Type + " = " + claim.Value);
							// http://schemas.microsoft.com/claims/authnmethodsreferences = pwd
							// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress = paul@splendidcrm.com
							// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname = Rony
							// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname = Paul
							// http://schemas.microsoft.com/identity/claims/identityprovider = live.com
							// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name = live.com#paul@splendidcrm.com
							// iat = 1484136100
							// nbf = 1484136100
							// exp = 1484140000
							// name = Paul Rony
							// platf = 3
							// ver = 1.0

							// 01/15/2017 Paul.  Alternate login. 
							// http://schemas.microsoft.com/claims/authnmethodsreferences = pwd
							// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname = Rony
							// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname = Paul
							// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name = paul@splendidcrm.onmicrosoft.com
							// http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn = paul@splendidcrm.onmicrosoft.com
							// iat = 1484512667
							// nbf = 1484512667
							// exp = 1484516567
							// name = Paul Rony
							// platf = 3
							// ver = 1.0
							switch ( claim.Type )
							{
								case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"          :  sUSER_NAME  = claim.Value;  break;
								case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"       :  sLAST_NAME  = claim.Value;  break;
								case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"     :  sFIRST_NAME = claim.Value;  break;
								case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"  :  sEMAIL1     = claim.Value;  break;
							}
						}
						if ( Sql.IsEmptyString(sEMAIL1) && sUSER_NAME.Contains("@") )
							sEMAIL1 = sUSER_NAME;
						profile = new MicrosoftGraphProfile();
						profile.FirstName    = sFIRST_NAME;
						profile.LastName     = sLAST_NAME ;
						profile.EmailAddress = sEMAIL1    ;
						profile.UserName     = sUSER_NAME ;
						profile.Name         = (sFIRST_NAME + " " + sLAST_NAME).Trim();
					}
				}
				else
				{
					Debug.WriteLine("FederationMetadata is empty.");
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine("Failed to get identity. " + ex.Message);
			}
			return profile;
		}

	}
}
