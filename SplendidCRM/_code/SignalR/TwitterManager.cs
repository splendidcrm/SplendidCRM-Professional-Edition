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
	/// Summary description for TwitterManager.
	/// </summary>
	public class TwitterManager : IDisposable
	{
		#region Properties
		private List<string>               _lstTracks        ;
		private List<long>                 _lstRecentTweets  ;
		private Dictionary<string, string> _dictTrackType    ;
		private TwitterConnection          _twitterConnection;
		private HttpContext                Context ;
		private IHubConnectionContext<dynamic>      Clients { get; set; }

		// Singleton instance
		private static TwitterManager _instance = null;

		public static TwitterManager Instance
		{
			get { return _instance; }
		}
		#endregion

		#region Initialization
		public static void InitApp(HttpContext Context)
		{
			_instance = new TwitterManager(Context, GlobalHost.ConnectionManager.GetHubContext<TwitterManagerHub>().Clients);
			System.Threading.Thread t = new System.Threading.Thread(_instance.AppStart);
			t.Start();
		}

		public static void RegisterScripts(HttpContext Context, ScriptManager mgrAjax)
		{
			if ( mgrAjax != null )
			{
				HttpApplicationState Application = Context.Application;
				string sTwitterConsumerKey    = Sql.ToString(Application["CONFIG.Twitter.ConsumerKey"      ]);
				string sTwitterConsumerSecret = Sql.ToString(Application["CONFIG.Twitter.ConsumerSecret"   ]);
				string sTwitterAccessToken    = Sql.ToString(Application["CONFIG.Twitter.AccessToken"      ]);
				string sTwitterAccessSecret   = Sql.ToString(Application["CONFIG.Twitter.AccessTokenSecret"]);
				// 02/26/2015 Paul.  Provide a way to disable twitter without clearing values. 
				if ( Sql.ToBoolean(Application["CONFIG.Twitter.EnableTracking"]) && !Sql.IsEmptyString(sTwitterConsumerKey) && !Sql.IsEmptyString(sTwitterConsumerSecret) && !Sql.IsEmptyString(sTwitterAccessToken) && !Sql.IsEmptyString(sTwitterAccessSecret) )
				{
					if ( Utils.CachedFileExists(Context, "~/Include/javascript/TwitterManagerHubJS.aspx") )
					{
						SignalRUtils.RegisterSignalR(mgrAjax);
						ScriptReference scrTwitterManagerHub = new ScriptReference("~/Include/javascript/TwitterManagerHubJS.aspx?" + Sql.ToString(Application["SplendidVersion"]) + "_" + Sql.ToString(Context.Session["USER_SETTINGS/CULTURE"]));
						if ( !mgrAjax.Scripts.Contains(scrTwitterManagerHub) )
							mgrAjax.Scripts.Add(scrTwitterManagerHub);
					}
				}
			}
		}

		private void UpdateTracks()
		{
			try
			{
				DataTable dt = SplendidCache.TwitterTracks(this.Context.Application);
				if ( dt != null )
				{
					_lstTracks      .Clear();
					// 10/27/2013 Paul.  Don't need to clear the recent tweet list because a processed tweet state still applies. 
					//_lstRecentTweets.Clear();
					_dictTrackType  .Clear();
					foreach ( DataRow row in dt.Rows )
					{
						string sNAME   = Sql.ToString(row["NAME"]).ToLower().Trim();
						string sTYPE = Sql.ToString(row["TYPE"]);
						if ( !Sql.IsEmptyString(sNAME) )
						{
							// 'Archive Original', 'Archive Retweets', 'Monitor Only'
							// 10/26/2013 Paul.  Check for duplicate entries.  If not in dictionary, then add to list and dictionary. 
							if ( !_dictTrackType.ContainsKey(sNAME) )
							{
								_lstTracks.Add(sNAME);
								_dictTrackType[sNAME] = sTYPE;
							}
							else if ( sTYPE == "Archive All" )
							{
								// If already in dictionary, then overwrite if new type is All. 
								// If existing type is All or Original, then no change needs to be made. 
								_dictTrackType[sNAME] = sTYPE;
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

		private void AppStart()
		{
			UpdateTracks();
			if ( _lstTracks.Count > 0 )
			{
				HttpApplicationState Application = this.Context.Application;
				string sTwitterConsumerKey       = Sql.ToString(Application["CONFIG.Twitter.ConsumerKey"      ]);
				string sTwitterConsumerSecret    = Sql.ToString(Application["CONFIG.Twitter.ConsumerSecret"   ]);
				string sTwitterAccessToken       = Sql.ToString(Application["CONFIG.Twitter.AccessToken"      ]);
				string sTwitterAccessTokenSecret = Sql.ToString(Application["CONFIG.Twitter.AccessTokenSecret"]);
				// 02/26/2015 Paul.  Provide a way to disable twitter without clearing values. 
				if ( Sql.ToBoolean(Application["CONFIG.Twitter.EnableTracking"]) && !Sql.IsEmptyString(sTwitterConsumerKey) && !Sql.IsEmptyString(sTwitterConsumerSecret) && !Sql.IsEmptyString(sTwitterAccessToken) && !Sql.IsEmptyString(sTwitterAccessTokenSecret) )
				{
					try
					{
						_twitterConnection.ConsumerKey       = sTwitterConsumerKey      ;
						_twitterConnection.ConsumerSecret    = sTwitterConsumerSecret   ;
						_twitterConnection.AccessToken       = sTwitterAccessToken      ;
						_twitterConnection.AccessTokenSecret = sTwitterAccessTokenSecret;
				
						_twitterConnection.Start(_lstTracks.ToArray());
						Debug.WriteLine("Twitter Streaming start successful. " + DateTime.Now.ToString());
					}
					catch(Exception ex)
					{
						Debug.WriteLine("Twitter Streaming start failed: " + ex.Message);
					}
				}
			}
		}
		#endregion

		#region disposal
		private bool disposed = false;
		
		~TwitterManager()
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
					if ( _twitterConnection != null )
					{
						if ( _twitterConnection.IsConnected() )
						{
							_twitterConnection.Stop();
							_twitterConnection = null;
						}
					}
				}
			}
			disposed = true;
		}
		#endregion

		#region Start
		public void Start()
		{
			if ( !_twitterConnection.IsConnected() )
			{
				UpdateTracks();
				if ( _lstTracks.Count > 0 )
				{
					HttpApplicationState Application = this.Context.Application;
					string sTwitterConsumerKey       = Sql.ToString (Application["CONFIG.Twitter.ConsumerKey"      ]);
					string sTwitterConsumerSecret    = Sql.ToString (Application["CONFIG.Twitter.ConsumerSecret"   ]);
					string sTwitterAccessToken       = Sql.ToString (Application["CONFIG.Twitter.AccessToken"      ]);
					string sTwitterAccessTokenSecret = Sql.ToString (Application["CONFIG.Twitter.AccessTokenSecret"]);
					bool   bTwitterEnableTracking    = Sql.ToBoolean(Application["CONFIG.Twitter.EnableTracking"   ]);
					if ( Sql.IsEmptyString(sTwitterConsumerKey      ) ) throw(new Exception("Twitter Consumer Key not specified."       ));
					if ( Sql.IsEmptyString(sTwitterConsumerSecret   ) ) throw(new Exception("Twitter Consumer Secret not specified."    ));
					if ( Sql.IsEmptyString(sTwitterAccessToken      ) ) throw(new Exception("Twitter Access Token not specified."       ));
					if ( Sql.IsEmptyString(sTwitterAccessTokenSecret) ) throw(new Exception("Twitter Access Token Secret not specified."));
					// 02/26/2015 Paul.  Provide a way to disable twitter without clearing values. 
					if ( !bTwitterEnableTracking                      ) throw(new Exception("Twitter tracking is not enabled."          ));
				
					_twitterConnection.ConsumerKey       = sTwitterConsumerKey      ;
					_twitterConnection.ConsumerSecret    = sTwitterConsumerSecret   ;
					_twitterConnection.AccessToken       = sTwitterAccessToken      ;
					_twitterConnection.AccessTokenSecret = sTwitterAccessTokenSecret;
					_twitterConnection.Start(_lstTracks.ToArray());
				}
			}
		}

		public void Stop()
		{
			// 11/05/2013 Paul.  Always call stop.  This should delete socket even if it is stopped. 
			//if ( _twitterConnection.IsConnected() )
			{
				_twitterConnection.Stop();
			}
		}

		public void Restart()
		{
			Stop();
			Start();
		}

		public string ValidateLogin(string sTwitterConsumerKey, string sTwitterConsumerSecret, string sTwitterAccessToken, string sTwitterAccessTokenSecret)
		{
			string sResult = String.Empty;
			try
			{
				if ( Sql.IsEmptyString(sTwitterConsumerKey      ) ) throw(new Exception("Twitter Consumer Key not specified."));
				if ( Sql.IsEmptyString(sTwitterConsumerSecret   ) ) throw(new Exception("Twitter Consumer Secret not specified."));
				if ( Sql.IsEmptyString(sTwitterAccessToken      ) ) throw(new Exception("Twitter Access Token not specified."));
				if ( Sql.IsEmptyString(sTwitterAccessTokenSecret) ) throw(new Exception("Twitter Access Token Secret not specified."));
				
				TwitterConnection twitterConnection = new TwitterConnection(Context, sTwitterConsumerKey, sTwitterConsumerSecret, sTwitterAccessToken, sTwitterAccessTokenSecret);
				twitterConnection.Start(new string[] { "SplendidCRM" });
				twitterConnection.Stop();
			}
			catch(Exception ex)
			{
				sResult = ex.Message;
			}
			return sResult;
		}
		#endregion

		private object NullID(Guid gID)
		{
			return Sql.IsEmptyGuid(gID) ? null : gID.ToString();
		}

		private TwitterManager(HttpContext Context, IHubConnectionContext<dynamic> clients)
		{
			this.Context = Context;
			this.Clients = clients;
			
			_lstTracks         = new List<string>();
			_lstRecentTweets   = new List<Int64>();
			_dictTrackType     = new Dictionary<string, string>();
			_twitterConnection = new TwitterConnection(Context);
			_twitterConnection.NewTweet += new NewTweetEventHandler(_twitterConnection_NewTweet);
		}

		private void _twitterConnection_NewTweet(NewTweetEvent e)
		{
			try
			{
				StringBuilder sbLog = new StringBuilder();
				//sbLog.Append("NewTweet - " + e.Track + " ==> " + e.Tweet.Text);
				//Debug.WriteLine(sbLog.ToString());
				
				Guid     gTWITTER_MESSAGE_ID   = Guid.Empty;
				string   sTRACK                = e.Track                          ;
				string   sNAME                 = e.Tweet.Text                     ;
				string   sDESCRIPTION          = SocialImport.FormatTweet(e.Tweet);
				DateTime dtDATE_START          = e.Tweet.CreatedAt                ;
				string   sDATE_START           = e.Tweet.CreatedAt.ToShortTimeString() + ' ' + e.Tweet.CreatedAt.ToShortDateString();
				long     lTWITTER_ID           = e.Tweet.Id.Value                 ;
				long     lTWITTER_USER_ID      = e.Tweet.Creator.Id.Value         ;
				string   sTWITTER_FULL_NAME    = e.Tweet.Creator.Name             ;
				string   sTWITTER_SCREEN_NAME  = e.Tweet.Creator.ScreenName       ;
				string   sTWITTER_AVATAR       = e.Tweet.Creator.ProfileImageURL  ;
				
				bool bRecentTweet = false;
				if ( e.Tweet.Retweeting != null )
				{
					long lORIGINAL_ID = e.Tweet.Retweeting.Id.Value;
					bRecentTweet = _lstRecentTweets.Contains(lORIGINAL_ID);
					if ( !bRecentTweet )
						_lstRecentTweets.Add(lORIGINAL_ID);
				}
				else
				{
					bRecentTweet = _lstRecentTweets.Contains(lTWITTER_ID);
					if ( !bRecentTweet )
						_lstRecentTweets.Add(lTWITTER_ID);
				}
				if ( _dictTrackType.ContainsKey(sTRACK) )
				{
					try
					{
						string sTYPE = _dictTrackType[sTRACK];
						if ( sTYPE == "Archive All" || (sTYPE == "Archive Original" && !bRecentTweet) )
						{
							string   sORIGINAL_NAME        = String.Empty;
							string   sORIGINAL_DESCRIPTION = String.Empty;
							long     lORIGINAL_ID          = 0;
							long     lORIGINAL_USER_ID     = 0;
							string   sORIGINAL_FULL_NAME   = String.Empty;
							string   sORIGINAL_SCREEN_NAME = String.Empty;
							DateTime dtORIGINAL_DATE_TIME  = DateTime.MinValue;
							if ( e.Tweet.Retweeting != null )
							{
								sORIGINAL_NAME        = e.Tweet.Retweeting.Text                     ;
								sORIGINAL_DESCRIPTION = SocialImport.FormatTweet(e.Tweet.Retweeting);
								lORIGINAL_ID          = e.Tweet.Retweeting.Id.Value                 ;
								lORIGINAL_USER_ID     = e.Tweet.Retweeting.Creator.Id.Value         ;
								sORIGINAL_FULL_NAME   = e.Tweet.Retweeting.Creator.Name             ;
								sORIGINAL_SCREEN_NAME = e.Tweet.Retweeting.Creator.ScreenName       ;
								dtORIGINAL_DATE_TIME  = e.Tweet.Retweeting.CreatedAt                ;
							}
							DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								using ( IDbTransaction trn = Sql.BeginTransaction(con) )
								{
									try
									{
										SqlProcs.spTWITTER_MESSAGES_InsertTrack
											( ref gTWITTER_MESSAGE_ID  // @ID                  
											, sTRACK                   // @TRACK               
											, sNAME                    // @NAME                
											, sDESCRIPTION             // @DESCRIPTION         
											, dtDATE_START             // @DATE_TIME           
											, lTWITTER_ID              // @TWITTER_ID          
											, lTWITTER_USER_ID         // @TWITTER_USER_ID     
											, sTWITTER_FULL_NAME       // @TWITTER_FULL_NAME   
											, sTWITTER_SCREEN_NAME     // @TWITTER_SCREEN_NAME 
											, lORIGINAL_ID             // @ORIGINAL_ID         
											, lORIGINAL_USER_ID        // @ORIGINAL_USER_ID    
											, sORIGINAL_FULL_NAME      // @ORIGINAL_FULL_NAME  
											, sORIGINAL_SCREEN_NAME    // @ORIGINAL_SCREEN_NAME
											, dtORIGINAL_DATE_TIME     // @ORIGINAL_DATE_TIME
											, sORIGINAL_NAME           // @ORIGINAL_NAME
											, sORIGINAL_DESCRIPTION    // @ORIGINAL_DESCRIPTION
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
						SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
					}
				}
				Clients.Group(sTRACK).newTweet(sTRACK, sNAME, sDESCRIPTION, sDATE_START, lTWITTER_ID, lTWITTER_USER_ID, sTWITTER_FULL_NAME, sTWITTER_SCREEN_NAME, sTWITTER_AVATAR, NullID(gTWITTER_MESSAGE_ID));
				//Clients.All.allTweet(sTRACK, sNAME, sDESCRIPTION, sDATE_START, lTWITTER_ID, lTWITTER_USER_ID, sTWITTER_FULL_NAME, sTWITTER_SCREEN_NAME, sTWITTER_AVATAR, NullID(gTWITTER_MESSAGE_ID));
			}
			catch(Exception ex)
			{
				SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
			}
		}
	}
}

