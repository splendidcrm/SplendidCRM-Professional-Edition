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
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Diagnostics;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

using Asterisk.NET.Manager;
using Asterisk.NET.Manager.Event;
using Asterisk.NET.Manager.Action;
using Asterisk.NET.Manager.Response;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for AsteriskManager.
	/// </summary>
	public class AsteriskManager : IDisposable
	{
		/*
		private class SplendidPipelineModule : HubPipelineModule
		{
			public SplendidPipelineModule()
			{
				Debug.WriteLine("SplendidPipelineModule " + DateTime.Now.ToString());
			}

			protected override bool OnBeforeIncoming(IHubIncomingInvokerContext context)
			{
				return base.OnBeforeIncoming(context);
			}

			protected override bool OnBeforeOutgoing(IHubOutgoingInvokerContext context)
			{
				return base.OnBeforeOutgoing(context);
			}
		}
		*/

		#region Properties
		private string                _sAsteriskTrunk    ;
		private string                _sOriginateContext ;
		private ManagerConnection     _asteriskConnection;
		private HttpContext           Context ;
		// 09/14/2020 Paul.  Convert to SignalR 2.4.1 
		private IHubConnectionContext<dynamic> Clients { get; set; }
		private Dictionary<string, NewChannelEvent> _outgoingCalls;
		private Dictionary<string, NewChannelEvent> _incomingCalls;

		// Singleton instance
		private static AsteriskManager _instance = null;

		public static AsteriskManager Instance
		{
			get { return _instance; }
		}
		#endregion

		#region Initialization
		public static void InitApp(HttpContext Context)
		{
			_instance = new AsteriskManager(Context, GlobalHost.ConnectionManager.GetHubContext<AsteriskManagerHub>().Clients);
			System.Threading.Thread t = new System.Threading.Thread(_instance.Start);
			t.Start();
		}

		public static void RegisterScripts(HttpContext Context, ScriptManager mgrAjax)
		{
			if ( mgrAjax != null )
			{
				HttpApplicationState Application = Context.Application;
				// 08/29/2013 Paul.  Click to Call is only used with Asterisk at this time. 
				string sAsteriskHost = Sql.ToString(Application["CONFIG.Asterisk.Host"]);
				if ( !Sql.IsEmptyString(sAsteriskHost) )
				{
					string sAsteriskUsername = Sql.ToString(Application["CONFIG.Asterisk.UserName"]);
					string sAsteriskPassword = Sql.ToString(Application["CONFIG.Asterisk.Password"]);
					if ( !Sql.IsEmptyString(sAsteriskUsername) && !Sql.IsEmptyString(sAsteriskPassword) )
					{
						// 08/24/2013 Paul.  Add EXTENSION_C in preparation for Asterisk click-to-call. 
						// 08/25/2013 Paul.  File IO is slow, so cache existance test. 
						// 09/03/2013 Paul.  Move ClickToCall code to AsteriskManager. 
						//if ( Utils.CachedFileExists(Context, "~/Calls/ClickToCall.asmx") )
						//{
						//	ServiceReference svc = new ServiceReference("~/Calls/ClickToCall.asmx");
						//	ScriptReference  scr = new ScriptReference ("~/Calls/ClickToCall.js"  );
						//	if ( !mgrAjax.Services.Contains(svc) ) mgrAjax.Services.Add(svc);
						//	if ( !mgrAjax.Scripts .Contains(scr) ) mgrAjax.Scripts .Add(scr);
						//}
						
						// 08/29/2013 Paul.  Only include Asterisk code if enabled. 
						if ( Utils.CachedFileExists(Context, "~/Include/javascript/AsteriskManagerHubJS.aspx") )
						{
							SignalRUtils.RegisterSignalR(mgrAjax);
							// 09/07/2013 Paul.  IE is caching during development.  Add the version to the URL. 
							// 09/07/2013 Paul.  Put the labels in the javascript file because they will only change based on the language. 
							ScriptReference scrAsteriskManagerHub = new ScriptReference("~/Include/javascript/AsteriskManagerHubJS.aspx?" + Sql.ToString(Application["SplendidVersion"]) + "_" + Sql.ToString(Context.Session["USER_SETTINGS/CULTURE"]));
							if ( !mgrAjax.Scripts.Contains(scrAsteriskManagerHub) )
								mgrAjax.Scripts.Add(scrAsteriskManagerHub);
						}
					}
				}
			}
		}

		private void Start()
		{
			HttpApplicationState Application = this.Context.Application;
			string sAsteriskHost     = Sql.ToString (Application["CONFIG.Asterisk.Host"    ]);
			int    nAsteriskPort     = Sql.ToInteger(Application["CONFIG.Asterisk.Port"    ]);
			string sAsteriskUsername = Sql.ToString (Application["CONFIG.Asterisk.UserName"]);
			string sAsteriskPassword = Sql.ToString (Application["CONFIG.Asterisk.Password"]);
			if ( !Sql.IsEmptyString(sAsteriskHost) && nAsteriskPort > 0 && !Sql.IsEmptyString(sAsteriskUsername) && !Sql.IsEmptyString(sAsteriskPassword) )
			{
				try
				{
					if ( !Sql.IsEmptyString(sAsteriskPassword) )
					{
						Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
						Guid gINBOUND_EMAIL_IV  = Sql.ToGuid(Application["CONFIG.InboundEmailIV" ]);
						sAsteriskPassword = Security.DecryptPassword(sAsteriskPassword, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
					}
					_asteriskConnection.Hostname = sAsteriskHost    ;
					_asteriskConnection.Port     = nAsteriskPort    ;
					_asteriskConnection.Username = sAsteriskUsername;
					_asteriskConnection.Password = sAsteriskPassword;
				
					_asteriskConnection.Login();
					Debug.WriteLine("Asterisk login successful. " + DateTime.Now.ToString());
				}
#if DEBUG
				catch(Exception ex)
				{
					Debug.WriteLine("Asterisk login failed: " + ex.Message);
				}
#else
				catch
				{
				}
#endif
			}
		}
		#endregion

		#region disposal
		private bool disposed = false;
		
		~AsteriskManager()
		{
			Dispose(false);
		}
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if ( !disposed )
			{
				if ( disposing )
				{
					if ( _asteriskConnection != null )
					{
						if ( _asteriskConnection.IsConnected() )
						{
							_asteriskConnection.Logoff();
							_asteriskConnection = null;
						}
					}
				}
			}
			disposed = true;
		}
		#endregion

		#region Dump
		private void DumpAttributes(StringBuilder sbLog, Dictionary<string, string> Attributes)
		{
			if ( Attributes != null )
			{
				sbLog.Append(", Attributes(");
				bool bFirst = true;
				foreach ( string sName in Attributes.Keys )
				{
					if ( !bFirst )
						sbLog.Append(", ");
					sbLog.Append(sName + ": " + Attributes[sName]);
					bFirst = false;
				}
				sbLog.Append(")");
			}
		}

		private void DumpManagerEvent(StringBuilder sbLog, ManagerEvent e)
		{
#if DEBUG
			sbLog.Append("DateReceived: " + e.DateReceived.ToString());
			sbLog.Append(", Server: "     + e.Server                 );
			sbLog.Append(", UniqueId: "   + e.UniqueId               );
			sbLog.Append(", Channel: "    + e.Channel                );
#endif
		}

		private void DumpAbstractChannelEvent(StringBuilder sbLog, AbstractChannelEvent e)
		{
#if DEBUG
			sbLog.Append(", ChannelState: "     + e.ChannelState    );
			sbLog.Append(", ChannelStateDesc: " + e.ChannelStateDesc);
			sbLog.Append(", CallerId: "         + e.CallerId        );
			sbLog.Append(", CallerIdNum: "      + e.CallerIdNum     );
			sbLog.Append(", CallerIdName: "     + e.CallerIdName    );
			sbLog.Append(", AccountCode: "      + e.AccountCode     );
			sbLog.Append(", State: "            + e.State           );
#endif
		}

		private void DumpBridgeEvent(StringBuilder sbLog, BridgeEvent e)
		{
#if DEBUG
			sbLog.Append(", BridgeState: " + e.BridgeState);
			sbLog.Append(", BridgeType: "  + e.BridgeType );
			sbLog.Append(", Response: "    + e.Response   );
			sbLog.Append(", Reason: "      + e.Reason     );
			sbLog.Append(", Channel1: "    + e.Channel1   );
			sbLog.Append(", Channel2: "    + e.Channel2   );
			sbLog.Append(", UniqueId1: "   + e.UniqueId1  );
			sbLog.Append(", UniqueId2: "   + e.UniqueId2  );
			sbLog.Append(", CallerId1: "   + e.CallerId1  );
			sbLog.Append(", CallerId2: "   + e.CallerId2  );
#endif
		}

		private void DumpOriginateResponseEvent(StringBuilder sbLog, OriginateResponseEvent e)
		{
#if DEBUG
			sbLog.Append(", ActionId: "         + e.ActionId         );
			sbLog.Append(", InternalActionId: " + e.InternalActionId );
			sbLog.Append(", Response: "         + e.Response         );
			sbLog.Append(", Context: "          + e.Context          );
			sbLog.Append(", Exten: "            + e.Exten            );
			sbLog.Append(", Reason: "           + e.Reason.ToString());
			sbLog.Append(", CallerId: "         + e.CallerId         );
			sbLog.Append(", CallerIdNum: "      + e.CallerIdNum      );
			sbLog.Append(", CallerIdName: "     + e.CallerIdName     );
#endif
		}

		private void DumpManagerResponse(StringBuilder sbLog, ManagerResponse e)
		{
#if DEBUG
			sbLog.Append("DateReceived: " + e.DateReceived.ToString());
			sbLog.Append(", Server: "     + e.Server                 );
			sbLog.Append(", UniqueId: "   + e.UniqueId               );
			sbLog.Append(", ActionId: "   + e.ActionId               );
			sbLog.Append(", Response: "   + e.Response               );
			sbLog.Append(", Message: "    + e.Message                );
#endif
		}

		// CDR        - DateReceived: 9/10/2013 3:37:09 AM, Server: , UniqueId: 1378798628.8, Channel: SIP/100-00000008, AccountCode: 24dc30e6-d934-40c9-94d2-0ff8182ae946, StartTime: 2013-09-10 03:37:08, AnswerTime: 2013-09-10 03:37:23, EndTime: 2013-09-10 03:37:42, Duration: 34, BillableSeconds: 19, Src: 100, Destination: 9196041258, DestinationChannel: SIP/Triad-00000009, CallerId: 100, DestinationContext: from-internal, Disposition: ANSWERED, AmaFlags: DOCUMENTATION, LastApplication: Dial, LastData: SIP/Triad/9196041258,300,Tt, UserField: 
		private void DumpCdrEvent(StringBuilder sbLog, CdrEvent e)
		{
#if DEBUG
			sbLog.Append(", AccountCode: "        + e.AccountCode               );
			sbLog.Append(", StartTime: "          + e.StartTime                 );
			sbLog.Append(", AnswerTime: "         + e.AnswerTime                );
			sbLog.Append(", EndTime: "            + e.EndTime                   );
			sbLog.Append(", Duration: "           + e.Duration.ToString()       );
			sbLog.Append(", BillableSeconds: "    + e.BillableSeconds.ToString());
			sbLog.Append(", Src: "                + e.Src                       );
			sbLog.Append(", Destination: "        + e.Destination               );
			sbLog.Append(", DestinationChannel: " + e.DestinationChannel        );
			sbLog.Append(", CallerId: "           + e.CallerId                  );
			sbLog.Append(", DestinationContext: " + e.DestinationContext        );
			sbLog.Append(", Disposition: "        + e.Disposition               );
			sbLog.Append(", AmaFlags: "           + e.AmaFlags                  );
			sbLog.Append(", LastApplication: "    + e.LastApplication           );
			sbLog.Append(", LastData: "           + e.LastData                  );
			sbLog.Append(", UserField: "          + e.UserField                 );
#endif
		}
		#endregion

		#region Login
		public void Login()
		{
			if ( !_asteriskConnection.IsConnected() )
			{
				this._outgoingCalls = new Dictionary<string, NewChannelEvent>();
				this._incomingCalls = new Dictionary<string, NewChannelEvent>();
				
				HttpApplicationState Application = this.Context.Application;
				string sAsteriskHost     = Sql.ToString (Application["CONFIG.Asterisk.Host"    ]);
				int    nAsteriskPort     = Sql.ToInteger(Application["CONFIG.Asterisk.Port"    ]);
				string sAsteriskUsername = Sql.ToString (Application["CONFIG.Asterisk.UserName"]);
				string sAsteriskPassword = Sql.ToString (Application["CONFIG.Asterisk.Password"]);
				if ( Sql.IsEmptyString(sAsteriskHost    ) ) throw(new Exception("Asterisk host not specified."));
				if ( nAsteriskPort == 0                   ) throw(new Exception("Asterisk port not specified."));
				if ( Sql.IsEmptyString(sAsteriskUsername) ) throw(new Exception("Asterisk username not specified."));
				if ( Sql.IsEmptyString(sAsteriskPassword) ) throw(new Exception("Asterisk password not specified."));
				
				if ( !Sql.IsEmptyString(sAsteriskPassword) )
				{
					Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
					Guid gINBOUND_EMAIL_IV  = Sql.ToGuid(Application["CONFIG.InboundEmailIV" ]);
					sAsteriskPassword = Security.DecryptPassword(sAsteriskPassword, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
				}
				_asteriskConnection.Hostname = sAsteriskHost    ;
				_asteriskConnection.Port     = nAsteriskPort    ;
				_asteriskConnection.Username = sAsteriskUsername;
				_asteriskConnection.Password = sAsteriskPassword;
				_asteriskConnection.Login();
			}
		}

		public void Logout()
		{
			if ( _asteriskConnection.IsConnected() )
			{
				_asteriskConnection.Logoff();
			}
			this._outgoingCalls = new Dictionary<string, NewChannelEvent>();
			this._incomingCalls = new Dictionary<string, NewChannelEvent>();
		}

		public string ValidateLogin(string sAsteriskHost, int nAsteriskPort, string sAsteriskUsername, string sAsteriskPassword)
		{
			string sResult = String.Empty;
			try
			{
				if ( Sql.IsEmptyString(sAsteriskHost    ) ) throw(new Exception("Asterisk host not specified."));
				if ( nAsteriskPort == 0                   ) throw(new Exception("Asterisk port not specified."));
				if ( Sql.IsEmptyString(sAsteriskUsername) ) throw(new Exception("Asterisk username not specified."));
				if ( Sql.IsEmptyString(sAsteriskPassword) ) throw(new Exception("Asterisk password not specified."));
				if ( !Sql.IsEmptyString(sAsteriskPassword) )
				{
					Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Context.Application["CONFIG.InboundEmailKey"]);
					Guid gINBOUND_EMAIL_IV  = Sql.ToGuid(Context.Application["CONFIG.InboundEmailIV" ]);
					sAsteriskPassword = Security.DecryptPassword(sAsteriskPassword, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
				}
				
				ManagerConnection asteriskConnection = new ManagerConnection();
				asteriskConnection.Hostname = sAsteriskHost    ;
				asteriskConnection.Port     = nAsteriskPort    ;
				asteriskConnection.Username = sAsteriskUsername;
				asteriskConnection.Password = sAsteriskPassword;
				asteriskConnection.Login();
				asteriskConnection.Logoff();
			}
			catch(Exception ex)
			{
				sResult = ex.Message;
			}
			return sResult;
		}
		#endregion

		#region Helpers
		private object NullID(Guid gID)
		{
			return Sql.IsEmptyGuid(gID) ? null : gID.ToString();
		}

		private void GetCaller(HttpApplicationState Application, string sPhoneNumber, ref Guid gPARENT_ID, ref string sPARENT_TYPE)
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL;
				sSQL = "select PARENT_ID                             " + ControlChars.CrLf
				     + "     , PARENT_TYPE                           " + ControlChars.CrLf
				     + "  from vwPHONE_NUMBERS                       " + ControlChars.CrLf
				     + " where NORMALIZED_NUMBER = @NORMALIZED_NUMBER" + ControlChars.CrLf
				     + "   and PHONE_TYPE in ('Work', 'Mobile', 'Office', 'Home')" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@NORMALIZED_NUMBER", sPhoneNumber);
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						using ( DataTable dt = new DataTable() )
						{
							da.Fill(dt);
							if ( dt.Rows.Count > 0 )
							{
								DataView vw = dt.DefaultView;
								vw.RowFilter = "PARENT_TYPE = 'Contacts'";
								if ( vw.Count > 0 )
								{
									gPARENT_ID   = Sql.ToGuid  (vw[0]["PARENT_ID"  ]);
									sPARENT_TYPE = Sql.ToString(vw[0]["PARENT_TYPE"]);
									return;
								}
								vw.RowFilter = "PARENT_TYPE = 'Leads'";
								if ( vw.Count > 0 )
								{
									gPARENT_ID   = Sql.ToGuid  (vw[0]["PARENT_ID"  ]);
									sPARENT_TYPE = Sql.ToString(vw[0]["PARENT_TYPE"]);
									return;
								}
								vw.RowFilter = "PARENT_TYPE = 'Prospects'";
								if ( vw.Count > 0 )
								{
									gPARENT_ID   = Sql.ToGuid  (vw[0]["PARENT_ID"  ]);
									sPARENT_TYPE = Sql.ToString(vw[0]["PARENT_TYPE"]);
									return;
								}
								vw.RowFilter = "PARENT_TYPE = 'Accounts'";
								if ( vw.Count > 0 )
								{
									gPARENT_ID   = Sql.ToGuid  (vw[0]["PARENT_ID"  ]);
									sPARENT_TYPE = Sql.ToString(vw[0]["PARENT_TYPE"]);
									return;
								}
							}
						}
					}
				}
			}
		}

		private void CreateCall(HttpApplicationState Application, ref Guid gCALL_ID, string sNAME, string sSTATUS, string sDIRECTION, Guid gUSER_ID, Guid gTEAM_ID, string sINVITEE_LIST, Guid gPARENT_ID, string sPARENT_TYPE)
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				using ( IDbTransaction trn = Sql.BeginTransaction(con) )
				{
					try
					{
						// 12/23/2013 Paul.  Add SMS_REMINDER_TIME. 
						// 09/14/2015 Paul.  Default for reminders should be 0. 
						SqlProcs.spCALLS_Update
							( ref gCALL_ID
							, gUSER_ID          // @ASSIGNED_USER_ID    uniqueidentifier
							, sNAME             // @NAME                nvarchar(50)
							, 0                 // @DURATION_HOURS      int
							, 0                 // @DURATION_MINUTES    int
							, DateTime.Now      // @DATE_TIME           datetime
							, sPARENT_TYPE      // @PARENT_TYPE         nvarchar(25)
							, gPARENT_ID        // @PARENT_ID           uniqueidentifier
							, sSTATUS           // @STATUS              nvarchar(25)
							, sDIRECTION        // @DIRECTION           nvarchar(25)
							, 0                 // @REMINDER_TIME       int
							, null              // @DESCRIPTION         nvarchar(max)
							, sINVITEE_LIST     // @INVITEE_LIST        varchar(8000)
							, gTEAM_ID          // @TEAM_ID             uniqueidentifier = null
							, null              // @TEAM_SET_LIST       varchar(8000) = null
							, 0                 // @EMAIL_REMINDER_TIME int = null
							, false             // @ALL_DAY_EVENT       bit = null
							, null              // @REPEAT_TYPE         nvarchar(25) = null
							, 0                 // @REPEAT_INTERVAL     int = null
							, null              // @REPEAT_DOW          nvarchar(7) = null
							, DateTime.MinValue // @REPEAT_UNTIL        datetime = null
							, 0                 // @REPEAT_COUNT        int = null
							, 0                 // @SMS_REMINDER_TIME   int = null
							// 05/17/2017 Paul.  Add Tags module. 
							, String.Empty      // TAG_SET_NAME
							// 11/07/2017 Paul.  Add IS_PRIVATE for use by a large customer. 
							, false             // IS_PRIVATE
							// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
							, String.Empty      // ASSIGNED_SET_LIST
							, trn
							);
						trn.Commit();
					}
					catch(Exception ex)
					{
						trn.Rollback();
						throw(new Exception(ex.Message, ex.InnerException));
					}
				}
			}
		}
		#endregion

		private AsteriskManager(HttpContext Context, IHubConnectionContext<dynamic> clients)
		{
			this._outgoingCalls = new Dictionary<string, NewChannelEvent>();
			this._incomingCalls = new Dictionary<string, NewChannelEvent>();
			this.Context = Context;
			this.Clients = clients;
			
			_sAsteriskTrunk    = Sql.ToString (this.Context.Application["CONFIG.Asterisk.Trunk"  ]);
			_sOriginateContext = Sql.ToString (this.Context.Application["CONFIG.Asterisk.Context"]);
			if ( Sql.IsEmptyString(_sOriginateContext) ) _sOriginateContext = "from-internal";
			
			_asteriskConnection = new ManagerConnection();
			// 09/02/2013 Paul.  We need to be able to disable this function as the initial thread call from Global.asax will die. 
			_asteriskConnection.TraceCallerThread = false;
			_asteriskConnection.ConnectionState   += new ConnectionStateEventHandler  (_asteriskConnection_ConnectionState  );
			_asteriskConnection.NewChannel        += new NewChannelEventHandler       (_asteriskConnection_NewChannel       );
			_asteriskConnection.NewState          += new NewStateEventHandler         (_asteriskConnection_NewState         );
			_asteriskConnection.Hangup            += new HangupEventHandler           (_asteriskConnection_Hangup           );
			_asteriskConnection.OriginateResponse += new OriginateResponseEventHandler(_asteriskConnection_OriginateResponse);
			_asteriskConnection.Cdr               += new CdrEventHandler              (_asteriskConnection_CallDataRecord   );
			//_asteriskConnection.Link              += new LinkEventHandler             (_asteriskConnection_Link             );
			//_asteriskConnection.Dial              += new DialEventHandler             (_asteriskConnection_Dial             );
			//_asteriskConnection.ExtensionStatus   += new ExtensionStatusEventHandler  (_asteriskConnection_ExtensionStatus  );
			//_asteriskConnection.NewExten          += new NewExtenEventHandler         (_asteriskConnection_NewExten         );
		}

		// 09/08/2015 Paul.  We need to lock the collection as the entries can get removed before it is retrieved. 
		private NewChannelEvent SafeOutgoingNewChannelEvent(string sUniqueId)
		{
			NewChannelEvent channel = null;
			lock ( _outgoingCalls )
			{
				if ( _outgoingCalls.ContainsKey(sUniqueId) )
				{
					channel = _outgoingCalls[sUniqueId];
				}
			}
			return channel;
		}

		private NewChannelEvent SafeIncomingNewChannelEvent(string sUniqueId)
		{
			NewChannelEvent channel = null;
			lock ( _incomingCalls )
			{
				if ( _incomingCalls.ContainsKey(sUniqueId) )
				{
					channel = _incomingCalls[sUniqueId];
				}
			}
			return channel;
		}

		// 09/09/2013 Paul.  Call-Data-Record is typically used for billing purposes. 
		// http://www.asteriskdocs.org/en/3rd_Edition/asterisk-book-html-chunk/asterisk-SysAdmin-SECT-1.html
		private void _asteriskConnection_CallDataRecord(object sender, CdrEvent e)
		{
			StringBuilder sbLog = new StringBuilder();
			try
			{
				// 09/09/2013 Paul.  The Account Code may need to be pulled from the originating record. 
				if ( Sql.IsEmptyString(e.AccountCode) )
				{
					NewChannelEvent channel = SafeOutgoingNewChannelEvent(e.UniqueId);
					if ( channel != null )
					{
						e.AccountCode = channel.AccountCode;
					}
					else
					{
						channel = SafeIncomingNewChannelEvent(e.UniqueId);
						if ( channel != null )
							e.AccountCode = channel.AccountCode;
					}
				}
				sbLog.Append("CDR        - ");
				DumpManagerEvent(sbLog, e);
				DumpCdrEvent(sbLog, e);
				Debug.WriteLine(sbLog.ToString());
				
				HttpApplicationState Application = Context.Application;
				bool bLogCallDetails = Sql.ToBoolean(Application["CONFIG.Asterisk.LogCallDetails"]);
				if ( bLogCallDetails )
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						using ( IDbTransaction trn = Sql.BeginTransaction(con) )
						{
							try
							{
								SqlProcs.spCALL_DETAIL_RECORDS_InsertOnly
									( e.UniqueId                          // @UNIQUEID             nvarchar(32)
									, Sql.ToGuid(e.AccountCode)           // @ACCOUNT_CODE_ID      uniqueidentifier
									, e.Src                               // @SOURCE               nvarchar(80)
									, e.Destination                       // @DESTINATION          nvarchar(80)
									, e.DestinationContext                // @DESTINATION_CONTEXT  nvarchar(80)
									, e.CallerId                          // @CALLERID             nvarchar(80)
									, e.Channel                           // @SOURCE_CHANNEL       nvarchar(80)
									, e.DestinationChannel                // @DESTINATION_CHANNEL  nvarchar(80)
									, Sql.ToDateTime (e.StartTime      )  // @START_TIME           datetime
									, Sql.ToDateTime (e.AnswerTime     )  // @ANSWER_TIME          datetime
									, Sql.ToDateTime (e.EndTime        )  // @END_TIME             datetime
									, Convert.ToInt32(e.Duration       )  // @DURATION             int
									, Convert.ToInt32(e.BillableSeconds)  // @BILLABLE_SECONDS     int
									, e.Disposition                       // @DISPOSITION          nvarchar(45)
									, e.AmaFlags                          // @AMA_FLAGS            nvarchar(80)
									, e.LastApplication                   // @LAST_APPLICATION     nvarchar(80)
									, e.LastData                          // @LAST_DATA            nvarchar(80)
									, e.UserField                         // @USER_FIELD           nvarchar(255)
									, trn
									);
								trn.Commit();
							}
							catch(Exception ex)
							{
								trn.Rollback();
								throw(new Exception(ex.Message, ex.InnerException));
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex) + " " + sbLog.ToString());
			}
		}

		private void _asteriskConnection_ConnectionState(object sender, ConnectionStateEvent e)
		{
			StringBuilder sbLog = new StringBuilder();
			try
			{
				sbLog.Append("ConnectionState - ");
				DumpManagerEvent(sbLog, e);
				// Connection state has changed
				if ( _asteriskConnection.IsConnected() )
					sbLog.Append(String.Format( ", Connected to: {0}@{1}", _asteriskConnection.Username, _asteriskConnection.Hostname ));
				else
					sbLog.Append(String.Format( ", Disconnected, reconnecting to {0}...", _asteriskConnection.Hostname ));
			
				Debug.WriteLine(sbLog.ToString());
				//Clients.All.newState(sbLog.ToString());
			}
			catch(Exception ex)
			{
				SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex) + " " + sbLog.ToString());
			}
		}

		private void _asteriskConnection_NewChannel(object sender, NewChannelEvent e)
		{
			StringBuilder sbLog = new StringBuilder();
			try
			{
				sbLog.Append("NewChannel - ");
				DumpManagerEvent        (sbLog, e);
				DumpAbstractChannelEvent(sbLog, e);
				DumpAttributes          (sbLog, e.Attributes);
				Debug.WriteLine(sbLog.ToString());
			
				// 09/03/2013 Paul.  First event on outgoing call is a NewChannel with context: from-internal. 
				// 09/03/2013 Paul.  First event on incoming call is a NewChannel with context: from-trunk-sip-*. 
				if ( e.ChannelStateDesc == "Down" && e.Attributes != null && e.Attributes.ContainsKey("context") )
				{
					if ( e.Attributes.ContainsKey("exten") )
					{
						HttpApplicationState Application = Context.Application;
						string sCallerIdFormat = Sql.ToString(Application["CONFIG.Asterisk.CallerIdFormat"]);
						if ( Sql.IsEmptyString(sCallerIdFormat) )
							sCallerIdFormat = "{0} {1}";
						if ( e.Attributes["context"] == _sOriginateContext )
						{
							_outgoingCalls.Add(e.UniqueId, e);
							e.Attributes.Add("DateReceived", DateTime.Now.ToString());
						
							// 09/07/2013 Paul.  Get the internal extension from the NewChannel event. 
							// 09/07/2013 Paul.  Get the phone number from the NewChannel event. 
							string sConnectedLineNum  = e.CallerIdNum ;
							string sConnectedLineName = e.CallerIdName;
							string sCallerID          = e.Attributes["exten"];
							if ( !Sql.IsEmptyString(sConnectedLineNum) )
							{
								Guid   gCALL_ID      = Guid.Empty;
								Guid   gUSER_ID      = Guid.Empty;
								Guid   gTEAM_ID      = Guid.Empty;
								Guid   gCALLER_ID    = Guid.Empty;
								string sCALLER_TYPE  = String.Empty;
								string sINVITEE_LIST = String.Empty;
								Crm.Users.GetUserByExtension(Application, sConnectedLineNum, ref gUSER_ID, ref gTEAM_ID);
								GetCaller(Application, sCallerID, ref gCALLER_ID, ref sCALLER_TYPE);
								if ( !Sql.IsEmptyGuid(gCALLER_ID) )
									sINVITEE_LIST = gCALLER_ID.ToString();
							
								// NewChannel - UniqueId: 1378269245.86, Channel: SIP/100-00000056, ChannelState: 0, ChannelStateDesc: Down, CallerIdNum: 100       , CallerIdName: Paul Rony, AccountCode: , Attributes(exten: 9196041258, context: from-internal)
								//string sNAME = String.Format(L10N.Term(Application, Sql.ToString(Application["CONFIG.default_language"]), "Asterisk.LBL_NEW_OUTGOING_CALL_TEMPLATE"), sConnectedLineName, sCallerID);
								// 09/03/2013 Paul.  We know the exact extension that originated the call, so send message just to that extension. 
								Clients.Group(sConnectedLineNum).outgoingCall(e.UniqueId, sConnectedLineName, sCallerID, NullID(gCALL_ID));
							}
						}
						else
						{
							_incomingCalls.Add(e.UniqueId, e);
							e.Attributes.Add("DateReceived", DateTime.Now.ToString());
							// NewChannel - UniqueId: 1378268323.82, Channel: SIP/Triad-00000052, ChannelState: 0, ChannelStateDesc: Down, CallerIdNum: 9196041258, CallerIdName: WIRELESS CALLER, AccountCode: , Attributes(exten: 9193241532, context: from-trunk-sip-Triad)
						
							// 09/03/2013 Paul.  Incoming calls go to all extensions. 
							// 09/07/2013 Paul.  We will likely want the global alert to be a configurable feature.  For now, disable and use the NewState event to send message to correct extension. 
							//Clients.All.incomingCall(e.UniqueId, sConnectedLineName, sPhoneNumber, NullID(gCALL_ID));
						}
					}
					// 09/08/2013 Paul.  Click-to-Call will start with an empty channel. 
					else if ( Sql.IsEmptyString(e.CallerIdNum) && !e.Attributes.ContainsKey("connectedlinenum") )
					{
						_outgoingCalls.Add(e.UniqueId, e);
						e.Attributes.Add("DateReceived", DateTime.Now.ToString());
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex) + " " + sbLog.ToString());
			}
		}

		private void _asteriskConnection_NewState(object sender, NewStateEvent e)
		{
			StringBuilder sbLog = new StringBuilder();
			try
			{
				sbLog.Append("NewState   - ");
				DumpManagerEvent        (sbLog, e);
				DumpAbstractChannelEvent(sbLog, e);
				DumpAttributes          (sbLog, e.Attributes);
				Debug.WriteLine(sbLog.ToString());
				
				// http://lists.digium.com/pipermail/asterisk-users/2008-March/207108.html
				// UniqueID is composed of the epoch when a call starts, plus a monotonically incrementing integer.
				if ( e.ChannelStateDesc == "Ringing" )  // Down, Ring, Ringing
				{
					if ( e.Attributes != null && e.Attributes.ContainsKey("connectedlinenum") )
					{
						string sCallerID = e.Attributes["connectedlinenum"];
						foreach ( string sDownUniqueId in _incomingCalls.Keys )
						{
							NewChannelEvent channel = _incomingCalls[sDownUniqueId];
							if ( channel.ChannelStateDesc == "Down" && channel.CallerIdNum == sCallerID && !channel.Attributes.ContainsKey("connectedlinenum") )
							{
								// 09/08/2013 Paul.  It is important that we copy the connectedline information as it is not normally available in the inbound NewChannel event. 
								channel.Attributes["connectedlinenum" ] = e.CallerIdNum ;
								channel.Attributes["connectedlinename"] = e.CallerIdName;
								
								HttpApplicationState Application = Context.Application;
								string sCallerIdFormat = Sql.ToString(Application["CONFIG.Asterisk.CallerIdFormat"]);
								if ( Sql.IsEmptyString(sCallerIdFormat) )
									sCallerIdFormat = "{0} {1}";
								// 09/07/2013 Paul.  Get the internal extension from the Hangup event. 
								// 09/07/2013 Paul.  Get the phone number from the NewChannel event. 
								string sConnectedLineNum  = e.CallerIdNum ;
								string sConnectedLineName = e.CallerIdName;
								sCallerID = String.Format(sCallerIdFormat, channel.CallerIdNum, channel.CallerIdName).Trim();
								if ( !Sql.IsEmptyString(sConnectedLineName) )
								{
									Guid   gCALL_ID      = Guid.Empty;
									Guid   gUSER_ID      = Guid.Empty;
									Guid   gTEAM_ID      = Guid.Empty;
									Guid   gCALLER_ID    = Guid.Empty;
									string sCALLER_TYPE  = String.Empty;
									string sINVITEE_LIST = String.Empty;
									Crm.Users.GetUserByExtension(Application, sConnectedLineNum, ref gUSER_ID, ref gTEAM_ID);
									GetCaller(Application, channel.CallerIdNum, ref gCALLER_ID, ref sCALLER_TYPE);
									if ( !Sql.IsEmptyGuid(gCALLER_ID) )
										sINVITEE_LIST = gCALLER_ID.ToString();
									
									// NewChannel - UniqueId: 1378268323.82, Channel: SIP/Triad-00000052, ChannelState: 0, ChannelStateDesc: Down, CallerIdNum: 9196041258, CallerIdName: WIRELESS CALLER, AccountCode: , Attributes(exten: 9193241532, context: from-trunk-sip-Triad)
									// NewState   - UniqueId: 1378268323.82, Channel: SIP/Triad-00000052, ChannelState: 6, ChannelStateDesc: Up  , CallerIdNum: 9196041258, CallerIdName: WIRELESS CALLER, AccountCode: , Attributes(connectedlinenum: 100, connectedlinename: Paul Rony)
									//string sNAME = String.Format(L10N.Term(Application, Sql.ToString(Application["CONFIG.default_language"]), "Asterisk.LBL_NEW_INCOMING_CALL_TEMPLATE"), sCallerID, sConnectedLineName);
									Clients.Group(sConnectedLineNum).incomingCall(channel.UniqueId, sConnectedLineName, sCallerID, NullID(gCALL_ID));
								}
								break;
							}
						}
					}
				}
				else if ( e.ChannelStateDesc == "Up" )
				{
					NewChannelEvent channel = SafeOutgoingNewChannelEvent(e.UniqueId);
					if ( channel != null )
					{
						channel.Attributes["connected"] = "true";
					}
					channel = SafeIncomingNewChannelEvent(e.UniqueId);
					if ( channel != null )
					{
						channel.Attributes["connected"] = "true";
						// 02/09/2016 Paul.  Checking for attributes is being done on Paperless site. 
						if ( !channel.Attributes.ContainsKey("connectedlinenum") && e.Attributes != null && e.Attributes.ContainsKey("connectedlinenum") )
						{
							// 09/07/2013 Paul.  It is important that we copy the connectedline information as it is not normally available in the inbound NewChannel event. 
							channel.Attributes["connectedlinenum" ] = e.Attributes["connectedlinenum" ];
							if ( e.Attributes.ContainsKey("connectedlinename") )
								channel.Attributes["connectedlinename"] = e.Attributes["connectedlinename"];
							
							HttpApplicationState Application = Context.Application;
							string sCallerIdFormat = Sql.ToString(Application["CONFIG.Asterisk.CallerIdFormat"]);
							if ( Sql.IsEmptyString(sCallerIdFormat) )
								sCallerIdFormat = "{0} {1}";
							// 09/07/2013 Paul.  Get the internal extension from the Hangup event. 
							// 09/07/2013 Paul.  Get the phone number from the NewChannel event. 
							string sConnectedLineNum  = e.Attributes["connectedlinenum" ];
							string sConnectedLineName = e.Attributes["connectedlinename"];
							string sCallerID          = String.Format(sCallerIdFormat, channel.CallerIdNum, channel.CallerIdName).Trim();
							if ( !Sql.IsEmptyString(sConnectedLineName) )
							{
								Guid   gCALL_ID      = Guid.Empty;
								Guid   gUSER_ID      = Guid.Empty;
								Guid   gTEAM_ID      = Guid.Empty;
								Guid   gCALLER_ID    = Guid.Empty;
								string sCALLER_TYPE  = String.Empty;
								string sINVITEE_LIST = String.Empty;
								Crm.Users.GetUserByExtension(Application, sConnectedLineNum, ref gUSER_ID, ref gTEAM_ID);
								GetCaller(Application, channel.CallerIdNum, ref gCALLER_ID, ref sCALLER_TYPE);
								if ( !Sql.IsEmptyGuid(gCALLER_ID) )
									sINVITEE_LIST = gCALLER_ID.ToString();
							
								// NewChannel - UniqueId: 1378268323.82, Channel: SIP/Triad-00000052, ChannelState: 0, ChannelStateDesc: Down, CallerIdNum: 9196041258, CallerIdName: WIRELESS CALLER, AccountCode: , Attributes(exten: 9193241532, context: from-trunk-sip-Triad)
								// NewState   - UniqueId: 1378268323.82, Channel: SIP/Triad-00000052, ChannelState: 6, ChannelStateDesc: Up  , CallerIdNum: 9196041258, CallerIdName: WIRELESS CALLER, AccountCode: , Attributes(connectedlinenum: 100, connectedlinename: Paul Rony)
								//string sNAME = String.Format(L10N.Term(Application, Sql.ToString(Application["CONFIG.default_language"]), "Asterisk.LBL_NEW_INCOMING_CALL_TEMPLATE"), sCallerID, sConnectedLineName);
								Clients.Group(sConnectedLineNum).incomingCall(e.UniqueId, sConnectedLineName, sCallerID, NullID(gCALL_ID));
							}
						}
					}
				}
				//Clients.All.newState(sbLog.ToString());
				//Clients.Group(e.Channel).newState(sbLog.ToString());
			}
			catch(Exception ex)
			{
				SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex) + " " + sbLog.ToString());
			}
		}

		private void _asteriskConnection_Hangup(object sender, HangupEvent e)
		{
			StringBuilder sbLog = new StringBuilder();
			try
			{
				sbLog.Append("Hangup     - ");
				DumpManagerEvent        (sbLog, e);
				DumpAbstractChannelEvent(sbLog, e);
				DumpAttributes          (sbLog, e.Attributes);
				Debug.WriteLine(sbLog.ToString());
				
				HttpApplicationState Application = Context.Application;
				string sCallerIdFormat = Sql.ToString(Application["CONFIG.Asterisk.CallerIdFormat"]);
				if ( Sql.IsEmptyString(sCallerIdFormat) )
					sCallerIdFormat = "{0} {1}";
				NewChannelEvent channel = SafeOutgoingNewChannelEvent(e.UniqueId);
				if ( channel != null )
				{
					_outgoingCalls.Remove(e.UniqueId);
					
					// 09/07/2013 Paul.  Get the internal extension from the NewChannel event. 
					// 09/07/2013 Paul.  Get the phone number from the NewChannel event. 
					string sConnectedLineNum  = channel.CallerIdNum ;
					string sConnectedLineName = channel.CallerIdName;
					string sCallerID          = String.Empty;
					// 09/08/2015 Paul.  Attribute might not exist, like a pre-event firing. 
					if ( channel.Attributes.ContainsKey("exten") )
						sCallerID = channel.Attributes["exten"];
					if ( !Sql.IsEmptyString(sConnectedLineNum) && !Sql.IsEmptyString(sCallerID) )
					{
						Guid   gCALL_ID      = Sql.ToGuid(channel.AccountCode);
						Guid   gUSER_ID      = Guid.Empty;
						Guid   gTEAM_ID      = Guid.Empty;
						Guid   gCALLER_ID    = Guid.Empty;
						string sCALLER_TYPE  = String.Empty;
						string sINVITEE_LIST = String.Empty;
						Crm.Users.GetUserByExtension(Application, sConnectedLineNum, ref gUSER_ID, ref gTEAM_ID);
						GetCaller(Application, sCallerID, ref gCALLER_ID, ref sCALLER_TYPE);
						if ( !Sql.IsEmptyGuid(gCALLER_ID) )
							sINVITEE_LIST = gCALLER_ID.ToString();
						
						Guid   gPARENT_ID   = Guid.Empty;
						string sPARENT_TYPE = String.Empty;
						if ( channel.Attributes.ContainsKey("PARENT_ID") )
						{
							gPARENT_ID = Sql.ToGuid(channel.Attributes["PARENT_ID"]);
							if ( channel.Attributes.ContainsKey("PARENT_TYPE") )
								sPARENT_TYPE = channel.Attributes["PARENT_TYPE"];
						}
						else
						{
							gPARENT_ID   = gCALLER_ID  ;
							sPARENT_TYPE = sCALLER_TYPE;
						}
						
						if ( channel.Attributes.ContainsKey("connected") )
						{
							// NewChannel - UniqueId: 1378269245.86, Channel: SIP/100-00000056, ChannelState: 0, ChannelStateDesc: Down, CallerIdNum: 100       , CallerIdName: Paul Rony, AccountCode: , Attributes(exten: 9196041258, context: from-internal)
							// Hangup     - UniqueId: 1378269245.86, Channel: SIP/100-00000056, ChannelState:  , ChannelStateDesc:     , CallerIdNum: 9193241532, CallerIdName: <unknown>, AccountCode: , Attributes(connectedlinenum: 9196041258, connectedlinename: CID:9193241532)
							//string sNAME = String.Format(L10N.Term(Application, Sql.ToString(Application["CONFIG.default_language"]), "Asterisk.LBL_ANSWERED_OUTGOING_CALL_TEMPLATE"), sConnectedLineName, sCallerID);
							int nDURATION_HOURS   = 0;
							int nDURATION_MINUTES = 0;
							try
							{
								DateTime dtStart    = DateTime.Parse(channel.Attributes["DateReceived"]);
								DateTime dtEnd      = DateTime.Now;
								TimeSpan tsDuration = dtEnd - dtStart;
								nDURATION_HOURS     = tsDuration.Hours  ;
								nDURATION_MINUTES   = tsDuration.Minutes;
							}
							catch
							{
							}
							Clients.Group(sConnectedLineNum).outgoingComplete(e.UniqueId, sConnectedLineName, sCallerID, NullID(gCALL_ID), nDURATION_HOURS, nDURATION_MINUTES);
						}
						else if ( Sql.ToBoolean(Application["CONFIG.Asterisk.LogOutgoingMissedCalls"]) )
						{
							// NewChannel - UniqueId: 1378275624.88, Channel: SIP/100-00000058, ChannelState: 0, ChannelStateDesc: Down, CallerIdNum: 100       , CallerIdName: Paul Rony, AccountCode: , Attributes(exten: 9196041258, context: from-internal)
							// Hangup     - UniqueId: 1378275624.88, Channel: SIP/100-00000058, ChannelState:  , ChannelStateDesc:     , CallerIdNum: 9193241532, CallerIdName: <unknown>, AccountCode: , Attributes(connectedlinenum: 9196041258, connectedlinename: CID:9193241532)
							string sNAME = String.Format(L10N.Term(Application, Sql.ToString(Application["CONFIG.default_language"]), "Asterisk.LBL_MISSED_OUTGOING_CALL_TEMPLATE"), sConnectedLineName, sCallerID);
							CreateCall(Application, ref gCALL_ID, sNAME, "Not Answered", "Outbound", gUSER_ID, gTEAM_ID, sINVITEE_LIST, gPARENT_ID, sPARENT_TYPE);
							Clients.Group(sConnectedLineNum).outgoingIncomplete(e.UniqueId, sConnectedLineName, sCallerID, NullID(gCALL_ID));
						}
					}
				}
				else
				{
					channel = SafeIncomingNewChannelEvent(e.UniqueId);
					if ( channel != null )
					{
						_incomingCalls.Remove(e.UniqueId);
					
						// 09/07/2013 Paul.  Get the internal extension from the Hangup event. 
						// 09/07/2013 Paul.  Get the phone number from the NewChannel event. 
						string sConnectedLineNum  = e.Attributes.ContainsKey("connectedlinenum" ) ? e.Attributes["connectedlinenum" ] : String.Empty;
						string sConnectedLineName = e.Attributes.ContainsKey("connectedlinename") ? e.Attributes["connectedlinename"] : sConnectedLineNum;
						string sCallerID          = String.Format(sCallerIdFormat, channel.CallerIdNum, channel.CallerIdName).Trim();
						if ( !Sql.IsEmptyString(sConnectedLineName) && !Sql.IsEmptyString(channel.CallerIdNum) )
						{
							Guid   gCALL_ID      = Sql.ToGuid(channel.AccountCode);
							Guid   gUSER_ID      = Guid.Empty;
							Guid   gTEAM_ID      = Guid.Empty;
							Guid   gCALLER_ID    = Guid.Empty;
							string sCALLER_TYPE  = String.Empty;
							string sINVITEE_LIST = String.Empty;
							Crm.Users.GetUserByExtension(Application, sConnectedLineNum, ref gUSER_ID, ref gTEAM_ID);
							GetCaller(Application, channel.CallerIdNum, ref gCALLER_ID, ref sCALLER_TYPE);
							if ( !Sql.IsEmptyGuid(gCALLER_ID) )
								sINVITEE_LIST = gCALLER_ID.ToString();
						
							Guid   gPARENT_ID   = Guid.Empty;
							string sPARENT_TYPE = String.Empty;
							if ( channel.Attributes.ContainsKey("PARENT_ID") )
							{
								gPARENT_ID = Sql.ToGuid(channel.Attributes["PARENT_ID"]);
								if ( channel.Attributes.ContainsKey("PARENT_TYPE") )
									sPARENT_TYPE = channel.Attributes["PARENT_TYPE"];
							}
							else
							{
								gPARENT_ID   = gCALLER_ID  ;
								sPARENT_TYPE = sCALLER_TYPE;
							}
						
							if ( channel.Attributes.ContainsKey("connected") )
							{
								// NewChannel - UniqueId: 1378268323.82, Channel: SIP/Triad-00000052, ChannelState: 0, ChannelStateDesc: Down, CallerIdNum: 9196041258, CallerIdName: WIRELESS CALLER, AccountCode: , Attributes(exten: 9193241532, context: from-trunk-sip-Triad)
								// Hangup     - UniqueId: 1378268323.82, Channel: SIP/Triad-00000052, ChannelState:  , ChannelStateDesc:     , CallerIdNum: 9196041258, CallerIdName: WIRELESS CALLER, AccountCode: , Attributes(connectedlinenum: 100, connectedlinename: Paul Rony)
								//string sNAME = String.Format(L10N.Term(Application, Sql.ToString(Application["CONFIG.default_language"]), "Asterisk.LBL_ANSWERED_INCOMING_CALL_TEMPLATE"), sCallerID, sConnectedLineName);
								int nDURATION_HOURS   = 0;
								int nDURATION_MINUTES = 0;
								try
								{
									DateTime dtStart    = DateTime.Parse(channel.Attributes["DateReceived"]);
									DateTime dtEnd      = DateTime.Now;
									TimeSpan tsDuration = dtEnd - dtStart;
									nDURATION_HOURS     = tsDuration.Hours  ;
									nDURATION_MINUTES   = tsDuration.Minutes;
								}
								catch
								{
								}
								Clients.Group(sConnectedLineNum).incomingComplete(e.UniqueId, sConnectedLineName, channel.CallerIdNum, NullID(gCALL_ID), nDURATION_HOURS, nDURATION_MINUTES);
							}
							else if ( Sql.ToBoolean(Application["CONFIG.Asterisk.LogIncomingMissedCalls"]) )
							{
								// NewChannel - UniqueId: 1378279438.90, Channel: SIP/Triad-0000005a, ChannelState: 0, ChannelStateDesc: Down, CallerIdNum: 9196041258, CallerIdName: WIRELESS CALLER, AccountCode: , Attributes(exten: 9193241532, context: from-trunk-sip-Triad)
								// Hangup     - UniqueId: 1378279438.90, Channel: SIP/Triad-0000005a, ChannelState:  , ChannelStateDesc:     , CallerIdNum: 9196041258, CallerIdName: WIRELESS CALLER, AccountCode: , Attributes(connectedlinenum: 100, connectedlinename: Paul Rony)
								string sNAME = String.Format(L10N.Term(Application, Sql.ToString(Application["CONFIG.default_language"]), "Asterisk.LBL_MISSED_INCOMING_CALL_TEMPLATE"), sCallerID, sConnectedLineName);
								CreateCall(Application, ref gCALL_ID, sNAME, "Not Answered", "Inbound", gUSER_ID, gTEAM_ID, sINVITEE_LIST, gPARENT_ID, sPARENT_TYPE);
								Clients.Group(sConnectedLineNum).incomingIncomplete(e.UniqueId, sConnectedLineName, channel.CallerIdNum, NullID(gCALL_ID));
							}
						}
					}
				}
				//Clients.All.newState(sbLog.ToString());
			}
			catch(Exception ex)
			{
				SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex) + " " + sbLog.ToString());
			}
		}

		private void _asteriskConnection_OriginateResponse(object sender, OriginateResponseEvent e)
		{
			StringBuilder sbLog = new StringBuilder();
			try
			{
				sbLog.Append("Originate  - ");
				DumpManagerEvent          (sbLog, e);
				DumpOriginateResponseEvent(sbLog, e);
				DumpAttributes            (sbLog, e.Attributes);
				Debug.WriteLine(sbLog.ToString());
				Clients.All.newState(sbLog.ToString());
				
				string sPHONE       = String.Empty;
				Guid   gPARENT_ID   = Guid.Empty;
				string sPARENT_TYPE = String.Empty;
				string[] arrActionId = e.ActionId.Split(' ');  // DateTime.Now.Ticks.ToString() + " " + sPHONE + " " + sPARENT_ID + " " + sPARENT_TYPE;
				if ( arrActionId.Length > 1 ) sPHONE       = arrActionId[1];
				if ( arrActionId.Length > 2 ) gPARENT_ID   = Sql.ToGuid(arrActionId[2]);
				if ( arrActionId.Length > 3 ) sPARENT_TYPE = arrActionId[3];
				
				HttpApplicationState Application = Context.Application;
				string sCallerIdFormat = Sql.ToString(Application["CONFIG.Asterisk.CallerIdFormat"]);
				if ( Sql.IsEmptyString(sCallerIdFormat) )
					sCallerIdFormat = "{0} {1}";
				if ( e.Response == "Failure" )
				{
					// 09/07/2013 Paul.  Get the internal extension from the NewChannel event. 
					// 09/07/2013 Paul.  Get the phone number from the NewChannel event. 
					// 09/01/2015 Paul.  Allow Extension to be dialed first. 
					string sConnectedLineNum  = String.Empty;
					string sConnectedLineName = e.CallerIdName;
					string sCallerID          = String.Empty;
					bool bOriginateExtensionFirst = Sql.ToBoolean(Context.Application["CONFIG.Asterisk.OriginateExtensionFirst"]);
					if ( bOriginateExtensionFirst )
					{
						sConnectedLineNum  = e.Channel.Replace("SIP/", "");
						sCallerID          = e.Exten.Replace("@" + this._sAsteriskTrunk, "");
					}
					else
					{
						sConnectedLineNum  = e.Exten;
						sCallerID          = e.Channel.Replace("SIP/", "").Replace("@" + this._sAsteriskTrunk, "");
					}

					if ( !Sql.IsEmptyString(sConnectedLineNum) )
					{
						Guid   gCALL_ID      = Guid.Empty;
						Guid   gUSER_ID      = Guid.Empty;
						Guid   gTEAM_ID      = Guid.Empty;
						Guid   gCALLER_ID    = Guid.Empty;
						string sCALLER_TYPE  = String.Empty;
						string sINVITEE_LIST = String.Empty;
						Crm.Users.GetUserByExtension(Application, sConnectedLineNum, ref gUSER_ID, ref gTEAM_ID);
						GetCaller(Application, sCallerID, ref gCALLER_ID, ref sCALLER_TYPE);
						if ( !Sql.IsEmptyGuid(gCALLER_ID) )
							sINVITEE_LIST = gCALLER_ID.ToString();
						if ( Sql.IsEmptyGuid(gPARENT_ID) )
						{
							gPARENT_ID   = gCALLER_ID  ;
							sPARENT_TYPE = sCALLER_TYPE;
						}
						
						if ( Sql.ToBoolean(Application["CONFIG.Asterisk.LogOutgoingMissedCalls"]) )
						{
							// NewChannel - UniqueId: 1378669472.236, Channel: SIP/Triad-000000ec  , ChannelState: 0, ChannelStateDesc: Down, CallerId: , CallerIdNum:           , CallerIdName:          , AccountCode: , Attributes(context: from-trunk-sip-Triad)
							// Hangup     - UniqueId: 1378669472.236, Channel: SIP/Triad-000000ec  , ChannelState:  , ChannelStateDesc:     , CallerId: , CallerIdNum: 9193241532, CallerIdName: Paul Rony, AccountCode: from 100 to 9198964319, Attributes(connectedlinenum: 9193241532, connectedlinename: Paul Rony)
							// Originate  - UniqueId:               , Channel: SIP/9198964319@Triad, Response: Failure, Context: from-internal, Exten: 100, Reason: 3, CallerId: , CallerIdNum: 9193241532, CallerIdName: Paul Rony
							string sNAME = String.Format(L10N.Term(Application, Sql.ToString(Application["CONFIG.default_language"]), "Asterisk.LBL_MISSED_OUTGOING_CALL_TEMPLATE"), sConnectedLineName, sCallerID);
							CreateCall(Application, ref gCALL_ID, sNAME, "Not Answered", "Outbound", gUSER_ID, gTEAM_ID, sINVITEE_LIST, gPARENT_ID, sPARENT_TYPE);
						}
						// 12/26/2013 Paul.  Send the error to the client. 
						Clients.Group(sConnectedLineNum).outgoingIncomplete(e.UniqueId, sConnectedLineName, sCallerID, NullID(gCALL_ID));
					}
				}
				else if ( e.Response == "Success" && !Sql.IsEmptyString(sPHONE) )
				{
					NewChannelEvent channel = SafeOutgoingNewChannelEvent(e.UniqueId);
					if ( channel != null )
					{
						channel.CallerIdNum  = e.Exten;
						channel.CallerIdName = e.CallerIdName;
						channel.Attributes["context"  ] = e.Context;
						channel.Attributes["originate"] = "true";
						channel.Attributes["exten"    ] = sPHONE;
						if ( !Sql.IsEmptyGuid(gPARENT_ID) )
						{
							channel.Attributes["PARENT_ID"  ] = gPARENT_ID.ToString();
							channel.Attributes["PARENT_TYPE"] = sPARENT_TYPE;
						}
						Guid gCALL_ID = Guid.Empty;
						Clients.Group(e.Exten).outgoingCall(e.UniqueId, e.Exten, sPHONE, NullID(gCALL_ID));
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex) + " " + sbLog.ToString());
			}
		}

		public string OriginateCall(string sUSER_EXTENSION, string sUSER_FULL_NAME, string sUSER_PHONE_WORK, string sPHONE, string sPARENT_ID, string sPARENT_TYPE)
		{
			if ( !_asteriskConnection.IsConnected() )
				Login();
			
			Regex r = new Regex(@"[^0-9_]");
			sPHONE = r.Replace(sPHONE, "");
			// http://www.voip-info.org/wiki/view/Asterisk+Manager+API+Action+Originate
			OriginateAction action = new OriginateAction();
			// 09/01/2015 Paul.  Allow Extension to be dialed first. 
			bool bOriginateExtensionFirst = Sql.ToBoolean(Context.Application["CONFIG.Asterisk.OriginateExtensionFirst"]);
			if ( bOriginateExtensionFirst )
			{
				action.Channel  = "SIP/" + sUSER_EXTENSION;
				action.Exten    = sPHONE;  // + (Sql.IsEmptyString(_sAsteriskTrunk) ? String.Empty : "@" + _sAsteriskTrunk);
			}
			else
			{
				action.Channel  = "SIP/" + sPHONE + (Sql.IsEmptyString(_sAsteriskTrunk) ? String.Empty : "@" + _sAsteriskTrunk);
				action.Exten    = sUSER_EXTENSION;
				// 02/09/2016 Paul.  Caller ID not getting set properly when originate first, so try only setting for normal case. 
				// 02/10/2016 Paul.  Remove the setting altogether is it was preventing call recordings from working. 
				//action.CallerId = sUSER_FULL_NAME; // + (Sql.IsEmptyString(sUSER_PHONE_WORK) ? String.Empty : " <" + sUSER_PHONE_WORK + ">");
			}
			action.Context  = _sOriginateContext;
			action.Priority = 1;
			action.Async    = true;
			action.Timeout  = 30000;
			action.ActionId = DateTime.Now.Ticks.ToString() + " " + sPHONE + " " + sPARENT_ID + " " + sPARENT_TYPE;
			// 09/08/2013 Paul.  The account does not appear until the hangup event. 
			//action.Account = "from " + sUSER_EXTENSION + " to " + sPHONE;
			//action.Variable = "var_exten=" + sUSER_EXTENSION + "|phone=" + sPHONE;
			
			try
			{
				ManagerResponse response = _asteriskConnection.SendAction(action, action.Timeout);
				StringBuilder sbLog = new StringBuilder();
				sbLog.Append("ManagerRes - ");
				DumpManagerResponse(sbLog, response);
				DumpAttributes(sbLog, response.Attributes);
				Debug.WriteLine(sbLog.ToString());
				if ( !response.IsSuccess() )
				{
					return response.Message;
				}
				return "Connected";
			}
			catch(Exception ex)
			{
				return ex.Message;
			}
		}

		public Guid CreateCall(string sUniqueId)
		{
			Guid gCALL_ID = Guid.Empty;
			HttpApplicationState Application = Context.Application;
			string sCallerIdFormat = Sql.ToString(Application["CONFIG.Asterisk.CallerIdFormat"]);
			if ( Sql.IsEmptyString(sCallerIdFormat) )
				sCallerIdFormat = "{0} {1}";
			NewChannelEvent channel = SafeOutgoingNewChannelEvent(sUniqueId);
			if ( channel != null )
			{
				if ( channel.Attributes.ContainsKey("exten") )
				{
					// 09/07/2013 Paul.  Get the internal extension from the NewChannel event. 
					// 09/07/2013 Paul.  Get the phone number from the NewChannel event. 
					string sConnectedLineNum  = channel.CallerIdNum ;
					string sConnectedLineName = channel.CallerIdName;
					string sCallerID          = channel.Attributes["exten"];
					gCALL_ID = Sql.ToGuid(channel.AccountCode);
					if ( Sql.IsEmptyGuid(gCALL_ID) )
					{
						Guid   gUSER_ID      = Guid.Empty;
						Guid   gTEAM_ID      = Guid.Empty;
						Guid   gCALLER_ID    = Guid.Empty;
						string sCALLER_TYPE  = String.Empty;
						string sINVITEE_LIST = String.Empty;
						Crm.Users.GetUserByExtension(Application, sConnectedLineNum, ref gUSER_ID, ref gTEAM_ID);
						GetCaller(Application, sCallerID, ref gCALLER_ID, ref sCALLER_TYPE);
						if ( !Sql.IsEmptyGuid(gCALLER_ID) )
							sINVITEE_LIST = gCALLER_ID.ToString();
						
						Guid   gPARENT_ID   = Guid.Empty;
						string sPARENT_TYPE = String.Empty;
						if ( channel.Attributes.ContainsKey("PARENT_ID") )
						{
							gPARENT_ID = Sql.ToGuid(channel.Attributes["PARENT_ID"]);
							if ( channel.Attributes.ContainsKey("PARENT_TYPE") )
								sPARENT_TYPE = channel.Attributes["PARENT_TYPE"];
						}
						else
						{
							gPARENT_ID   = gCALLER_ID  ;
							sPARENT_TYPE = sCALLER_TYPE;
						}
						
						string sNAME = String.Format(L10N.Term(Application, Sql.ToString(Application["CONFIG.default_language"]), "Asterisk.LBL_ANSWERED_OUTGOING_CALL_TEMPLATE"), sConnectedLineName, sCallerID);
						CreateCall(Application, ref gCALL_ID, sNAME, "Held", "Outbound", gUSER_ID, gTEAM_ID, sINVITEE_LIST, gPARENT_ID, sPARENT_TYPE);
						channel.AccountCode = gCALL_ID.ToString();
					}
				}
			}
			else
			{
				channel = SafeIncomingNewChannelEvent(sUniqueId);
				if ( channel != null )
				{
					// 09/07/2013 Paul.  Get the internal extension from the Hangup event. 
					// 09/07/2013 Paul.  Get the phone number from the NewChannel event. 
					string sConnectedLineNum  = channel.Attributes.ContainsKey("connectedlinenum" ) ? channel.Attributes["connectedlinenum" ] : String.Empty;
					string sConnectedLineName = channel.Attributes.ContainsKey("connectedlinename") ? channel.Attributes["connectedlinename"] : sConnectedLineNum;
					string sCallerID          = String.Format(sCallerIdFormat, channel.CallerIdNum, channel.CallerIdName).Trim();
					gCALL_ID = Sql.ToGuid(channel.AccountCode);
					if ( Sql.IsEmptyGuid(gCALL_ID) )
					{
						Guid   gUSER_ID      = Guid.Empty;
						Guid   gTEAM_ID      = Guid.Empty;
						Guid   gCALLER_ID    = Guid.Empty;
						string sCALLER_TYPE  = String.Empty;
						string sINVITEE_LIST = String.Empty;
						Crm.Users.GetUserByExtension(Application, sConnectedLineNum, ref gUSER_ID, ref gTEAM_ID);
						GetCaller(Application, channel.CallerIdNum, ref gCALLER_ID, ref sCALLER_TYPE);
						if ( !Sql.IsEmptyGuid(gCALLER_ID) )
							sINVITEE_LIST = gCALLER_ID.ToString();
					
						Guid   gPARENT_ID   = Guid.Empty;
						string sPARENT_TYPE = String.Empty;
						if ( channel.Attributes.ContainsKey("PARENT_ID") )
						{
							gPARENT_ID = Sql.ToGuid(channel.Attributes["PARENT_ID"]);
							if ( channel.Attributes.ContainsKey("PARENT_TYPE") )
								sPARENT_TYPE = channel.Attributes["PARENT_TYPE"];
						}
						else
						{
							gPARENT_ID   = gCALLER_ID  ;
							sPARENT_TYPE = sCALLER_TYPE;
						}
					
						string sNAME = String.Format(L10N.Term(Application, Sql.ToString(Application["CONFIG.default_language"]), "Asterisk.LBL_ANSWERED_INCOMING_CALL_TEMPLATE"), sCallerID, sConnectedLineName);
						CreateCall(Application, ref gCALL_ID, sNAME, "Held", "Inbound", gUSER_ID, gTEAM_ID, sINVITEE_LIST, gPARENT_ID, sPARENT_TYPE);
						channel.AccountCode = gCALL_ID.ToString();
					}
				}
				else
				{
					string sERR_CALL_NOT_FOUND = L10N.Term(Application, Sql.ToString(Application["CONFIG.default_language"]), "Asterisk.ERR_CALL_NOT_FOUND");
					throw(new Exception(sERR_CALL_NOT_FOUND));
				}
			}
			return gCALL_ID;
		}

		#region Unused events
		//private void _asteriskConnection_Link(object sender, LinkEvent e)
		//{
		//	StringBuilder sbLog = new StringBuilder();
		//	sbLog.Append("Link       - ");
		//	DumpManagerEvent        (sbLog, e);
		//	DumpBridgeEvent         (sbLog, e);
		//	DumpAttributes          (sbLog, e.Attributes);
		//	Debug.WriteLine(sbLog.ToString());
		//}

		//private void _asteriskConnection_Dial(object sender, DialEvent e)
		//{
		//	StringBuilder sbLog = new StringBuilder();
		//	sbLog.Append("Dial       - ");
		//	DumpManagerEvent        (sbLog, e);
		//	DumpAttributes          (sbLog, e.Attributes);
		//	Debug.WriteLine(sbLog.ToString());
		//	Clients.All.newState(sbLog.ToString());
		//}

		//private void _asteriskConnection_NewExten(object sender, NewExtenEvent e)
		//{
		//	StringBuilder sbLog = new StringBuilder();
		//	sbLog.Append("NewExten - ");
		//	DumpManagerEvent        (sbLog, e);
		//	DumpAbstractChannelEvent(sbLog, e);
		//	DumpAttributes          (sbLog, e.Attributes);
		//	Debug.WriteLine(sbLog.ToString());
		//	Clients.All.newState(sbLog.ToString());
		//}

		//private void _asteriskConnection_ExtensionStatus(object sender, ExtensionStatusEvent e)
		//{
		//	StringBuilder sbLog = new StringBuilder();
		//	sbLog.Append("ExtensionStatus - ");
		//	DumpManagerEvent        (sbLog, e);
		//	DumpAbstractChannelEvent(sbLog, e);
		//	DumpAttributes          (sbLog, e.Attributes);
		//	Debug.WriteLine(sbLog.ToString());
		//	Clients.All.newState(sbLog.ToString());
		//}
		#endregion
	}
}

