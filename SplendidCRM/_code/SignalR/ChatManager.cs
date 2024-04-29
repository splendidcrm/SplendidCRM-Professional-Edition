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
using System.Web.SessionState;
using System.Diagnostics;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for ChatManager.
	/// </summary>
	public class ChatManager
	{
		#region Properties
		private HttpContext                Context ;
		private IHubConnectionContext<dynamic>      Clients { get; set; }

		// Singleton instance
		private static ChatManager _instance = null;

		public static ChatManager Instance
		{
			get { return _instance; }
		}
		#endregion

		#region Initialization
		public static void InitApp(HttpContext Context)
		{
			_instance = new ChatManager(Context, GlobalHost.ConnectionManager.GetHubContext<ChatManagerHub>().Clients);
		}

		public static void RegisterScripts(HttpContext Context, ScriptManager mgrAjax)
		{
			if ( mgrAjax != null )
			{
				if ( Utils.CachedFileExists(Context, "~/Include/javascript/ChatManagerHubJS.aspx") )
				{
					SignalRUtils.RegisterSignalR(mgrAjax);
					// 11/25/2014 Paul.  There is no reason to have teh Chat hub code in a file separate from ChatDashboardUI.js. 
					//ScriptReference scrChatManagerHub = new ScriptReference("~/Include/javascript/ChatManagerHubJS.aspx?" + Sql.ToString(Context.Application["SplendidVersion"]) + "_" + Sql.ToString(Context.Session["USER_SETTINGS/CULTURE"]));
					//if ( !mgrAjax.Scripts.Contains(scrChatManagerHub) )
					//	mgrAjax.Scripts.Add(scrChatManagerHub);
				}
			}
		}
		#endregion

		private object NullID(Guid gID)
		{
			return Sql.IsEmptyGuid(gID) ? null : gID.ToString();
		}

		private ChatManager(HttpContext Context, IHubConnectionContext<dynamic> clients)
		{
			this.Context = Context;
			this.Clients = clients;
		}

		// 04/01/2020 Paul.  Move json utils to RestUtil. 

		public void NewMessage(Guid gID)
		{
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory(this.Context.Application);
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					if ( !Sql.IsEmptyGuid(gID) )
					{
						string sSQL ;
						sSQL = "select *              " + ControlChars.CrLf
						     + "  from vwCHAT_MESSAGES" + ControlChars.CrLf
						     + " where ID = @ID       " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@ID", gID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									if ( dt.Rows.Count > 0 )
									{
										DataRow row = dt.Rows[0];
										Guid     gCHAT_CHANNEL_ID    = Sql.ToGuid    (row["CHAT_CHANNEL_ID"   ]);
										string   sNAME               = Sql.ToString  (row["NAME"              ]);
										string   sDESCRIPTION        = Sql.ToString  (row["DESCRIPTION"       ]);
										DateTime dtDATE_ENTERED      = Sql.ToDateTime(row["DATE_ENTERED"      ]);
										Guid     gCREATED_BY_ID      = Sql.ToGuid    (row["CREATED_BY_ID"     ]);
										string   sCREATED_BY         = Sql.ToString  (row["CREATED_BY"        ]);
										string   sCREATED_BY_PICTURE = Sql.ToString  (row["CREATED_BY_PICTURE"]);
										Guid     gPARENT_ID          = Sql.ToGuid    (row["PARENT_ID"         ]);
										string   sPARENT_TYPE        = Sql.ToString  (row["PARENT_TYPE"       ]);
										string   sPARENT_NAME        = Sql.ToString  (row["PARENT_NAME"       ]);
										Guid     gNOTE_ATTACHMENT_ID = Sql.ToGuid    (row["NOTE_ATTACHMENT_ID"]);
										string   sFILENAME           = Sql.ToString  (row["FILENAME"          ]);
										string   sFILE_EXT           = Sql.ToString  (row["FILE_EXT"          ]);
										string   sFILE_MIME_TYPE     = Sql.ToString  (row["FILE_MIME_TYPE"    ]);
										long     lFILE_SIZE          = Sql.ToLong    (row["FILE_SIZE"         ]);
										bool     bATTACHMENT_READY   = Sql.ToBoolean (row["ATTACHMENT_READY"  ]);
										
										Guid     gTIMEZONE        = Sql.ToGuid  (HttpContext.Current.Session["USER_SETTINGS/TIMEZONE"]);
										TimeZone T10n             = TimeZone.CreateTimeZone(gTIMEZONE);
										// 04/01/2020 Paul.  Move json utils to RestUtil. 
										string   sDATE_ENTERED    = RestUtil.ToJsonDate(T10n.FromServerTime(dtDATE_ENTERED));
										//Clients.Group(gCHAT_CHANNEL_ID.ToString()).newMessage(gCHAT_CHANNEL_ID, gID, sNAME, sDESCRIPTION, sDATE_ENTERED, NullID(gPARENT_ID), sPARENT_TYPE, sPARENT_NAME, NullID(gCREATED_BY_ID), sCREATED_BY, sCREATED_BY_PICTURE, NullID(gNOTE_ATTACHMENT_ID), sFILENAME, sFILE_EXT, sFILE_MIME_TYPE, lFILE_SIZE, bATTACHMENT_READY);
										//Clients.All.allMessage(gCHAT_CHANNEL_ID, gID, sDESCRIPTION, dtDATE_ENTERED, gUSER_ID, sCREATED_BY, NullID(gPARENT_ID), sPARENT_TYPE);
										// 04/27/2024 Paul.  SignalR core does not support more than 10 parameters, so convert to dictionary. 
										Dictionary<string, object> dict = RestUtil.ToJson("", "ChatMessages", dt.Rows[0], T10n);
										Clients.Group(gCHAT_CHANNEL_ID.ToString()).newMessage((dict["d"] as Dictionary<string, object>)["results"]);
									}
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
			}
		}
	}
}

