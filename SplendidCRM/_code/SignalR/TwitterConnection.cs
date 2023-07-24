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
using System.Threading;
using System.Collections;
using System.Text.RegularExpressions;
using System.Text;
using System.Net;
using System.Web;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Diagnostics;

namespace SplendidCRM
{
	public class NewTweetEvent
	{
		public TweetinCore.Interfaces.ITweet Tweet { get; set; }
		public string Track { get; set; }
		
		public NewTweetEvent(TweetinCore.Interfaces.ITweet tweet, string sTrack)
		{
			this.Tweet = tweet ;
			this.Track = sTrack;
		}
	}

	public delegate void NewTweetEventHandler(NewTweetEvent e);

	public class TwitterConnection
	{
		#region Variables

		public event NewTweetEventHandler NewTweet;

		internal HttpContext                  context       ;
		private TwitterToken.Token            mrToken       ;
		private Streaminvi.FilteredStream     mrSocket      ;
		private Asterisk.NET.Util.ThreadClass mrReaderThread;
		private TwitterReader                 mrReader      ;

		private bool   keepAlive       = true;
		private object lockSocket      = new object();
		private bool   reconnected     = false;
		private bool   reconnectEnable = false;

		private Thread callerThread;
		#endregion

		public TwitterConnection(HttpContext Context)
		{
			this.context      = Context;
			this.callerThread = Thread.CurrentThread;
			this.mrToken      = new TwitterToken.Token(String.Empty, String.Empty, String.Empty, String.Empty);
			ServicePointManager.Expect100Continue = false;
		}

		public TwitterConnection(HttpContext Context, string sConsumerKey, string sConsumerSecret, string sAccessToken, string sAccessTokenSecret) : this(Context)
		{
			mrToken = new TwitterToken.Token(sAccessToken, sAccessTokenSecret, sConsumerKey, sConsumerSecret);
		}

		internal HttpContext Context
		{
			get { return context; }
		}

		internal Thread CallerThread
		{
			get { return callerThread; }
		}

		internal TwitterToken.Token Token
		{
			get { return mrToken; }
		}

		public string AccessToken
		{
			get { return mrToken.AccessToken; }
			set { mrToken.AccessToken = value; }
		}

		public string AccessTokenSecret
		{
			get { return mrToken.AccessTokenSecret; }
			set { mrToken.AccessTokenSecret = value; }
		}

		public string ConsumerKey
		{
			get { return mrToken.ConsumerKey; }
			set { mrToken.ConsumerKey = value; }
		}

		public string ConsumerSecret
		{
			get { return mrToken.ConsumerSecret; }
			set { mrToken.ConsumerSecret = value; }
		}

		internal void DispatchTweet(TweetinCore.Interfaces.ITweet tweet, List<string> matches)
		{
			if ( NewTweet != null )
			{
				foreach ( string sTrack in matches )
				{
					Debug.WriteLine(sTrack + " ==> " + tweet.Text);
					NewTweet(new NewTweetEvent(tweet, sTrack));
				}
			}
		}

		public void Start(string[] arrTracks)
		{
			if ( reconnected )
			{
				Debug.WriteLine("Start during reconnect state.");
				throw new Exception("Unable start during reconnect state.");
			}
			reconnectEnable = false;

			if ( connect(arrTracks) )
				reconnectEnable = keepAlive;
		}

		protected internal bool connect(string[] arrTracks)
		{
			// 11/15/2013 Paul.  Provide logging of twitter connections. 
			bool bVerboseStatus = Sql.ToBoolean(Context.Application["CONFIG.Twitter.VerboseStatus"]);
			if ( bVerboseStatus )
			{
				StringBuilder sbTracks = new StringBuilder();
				foreach ( string sTrack in arrTracks )
				{
					if ( sbTracks.Length > 0 )
						sbTracks.Append(", ");
					sbTracks.Append(sTrack);
				}
				SyncError.SystemMessage(Context, "Warning", new StackTrace(true).GetFrame(0), "Twitter Tracking: " + sbTracks.ToString());
			}
			
			bool startReader = false;
			lock ( lockSocket )
			{
				if ( mrSocket == null )
				{
					mrSocket = new Streaminvi.FilteredStream();
					foreach ( string sTrack in arrTracks )
					{
						mrSocket.AddTrack(sTrack);
					}

					if ( this.mrReader == null )
					{
						// 12/23/2013 Paul.  All tracing to be disabled. 
						mrReader = new TwitterReader(this, bVerboseStatus);
						mrReaderThread  = new Asterisk.NET.Util.ThreadClass(new ThreadStart(this.mrReader.Run), "ManagerReader-" + DateTime.Now.Second);
						mrReader.Socket = mrSocket;
						startReader = true;
					}
					else
					{
						mrReader.Socket = mrSocket;
					}
				}
			}
			if ( startReader )
				mrReaderThread.Start();

			//return IsConnected();
			return true;
		}

		private void disconnect(bool withDie)
		{
			lock ( lockSocket )
			{
				if ( withDie )
				{
					reconnectEnable = false;
					reconnected     = false;
				}

				if ( mrReader != null )
				{
					if ( withDie )
					{
						mrReader.Die = true;
						mrReader = null;
					}
					else
					{
						mrReader.Socket = null;
					}
				}

				if ( this.mrSocket != null )
				{
					mrSocket.StopStream();
					mrSocket = null;
				}
			}
		}

		public bool IsConnected()
		{
			bool result = false;
			lock ( lockSocket )
			{
				if ( mrSocket != null )
				{
					result = !(mrSocket.StreamState == TweetinCore.Enum.StreamState.Stop);
				}
			}
			return result;
		}

		public void Stop()
		{
			lock ( lockSocket )
			{
				// stop reconnecting when we got disconnected
				reconnectEnable = false;
				if ( mrReader != null && mrSocket != null )
				{
					try
					{
						mrReader.Die = true;
					}
					catch
					{
					}
				}
			}
			disconnect(true);
		}
	}
}