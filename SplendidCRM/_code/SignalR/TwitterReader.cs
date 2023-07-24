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
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Diagnostics;

namespace SplendidCRM
{
	public class TwitterReader
	{
		private TwitterConnection         mrConnector;
		private Streaminvi.FilteredStream mrSocket   ;

		private int  wait       = 250;
		private bool die        = false;
		private bool bVerboseStatus = false;

		public TwitterReader(TwitterConnection connection, bool bVerboseStatus)
		{
			this.mrConnector = connection;
			this.die         = false;
			this.bVerboseStatus = bVerboseStatus;
		}

		internal Streaminvi.FilteredStream Socket
		{
			set
			{
				this.mrSocket = value;
			}
		}

		internal bool Die
		{
			get { return die; }
			set
			{
				die = value;
				if ( die )
					this.mrSocket = null;
			}
		}

		// Func<TweetinCore.Interfaces.ITweet, List<string>, bool>
		private bool ProcessTweet(TweetinCore.Interfaces.ITweet tweet, List<string> matches)
		{
			if ( tweet != null )
			{
				mrConnector.DispatchTweet(tweet, matches);
			}
			wait = 250;
			//Debug.WriteLine(tweet.Text);
			if ( die )
				mrSocket.StopStream();
			return true;
		}

		private void StartStream()
		{
			try
			{
				//mrSocket.StartStream(mrConnector.Token, (x, matches) => ProcessTweet(x, matches));
				Func<TweetinCore.Interfaces.ITweet, List<string>, bool> processDelegate = ProcessTweet;
				mrSocket.StartStream(mrConnector.Token, processDelegate);
			}
			catch (WebException ex)
			{
				string sError = "TwitterReader Exception: " + ex.Message;
				if ( bVerboseStatus )
					Debug.WriteLine(sError);
				// https://dev.twitter.com/docs/error-codes-responses
				// 420 means rate limited. 
				HttpWebResponse resp = ex.Response as HttpWebResponse;
				if ( resp != null && resp.StatusCode.ToString() == "420" )
				{
					SplendidError.SystemMessage(mrConnector.Context, "Warning", new StackTrace(true).GetFrame(0), sError);
					if ( wait < 10000)
					{
						wait = 10000;
					}
					else
					{
						if ( wait < 240000 )
						{
							wait = wait * 2;
						}
					}
				}
				else
				{
					SplendidError.SystemMessage(mrConnector.Context, "Error", new StackTrace(true).GetFrame(0), sError);
				}
			}
		}

		internal void Run()
		{
			if ( mrSocket == null )
				throw new SystemException("TwitterReader is unable to run: stream is null.");

			Tweetinvi.TwitterContext context = new Tweetinvi.TwitterContext();
			while ( !die && mrSocket != null )
			{
				try
				{
					while ( !die && mrSocket != null )
					{
						if ( !context.TryInvokeAction(StartStream) )
						{
							if ( mrConnector.CallerThread != null && mrConnector.CallerThread.ThreadState == System.Threading.ThreadState.Stopped )
							{
								die = true;
								break;
							}
							if ( context.LastActionTwitterException != null && context.LastActionTwitterException.Status == WebExceptionStatus.ProtocolError )
							{
								// -- From Twitter Docs -- 
								// When a HTTP error (> 200) is returned, back off exponentially. 
								// Perhaps start with a 10 second wait, double on each subsequent failure, 
								// and finally cap the wait at 240 seconds. 
								// Exponential Backoff
								if ( wait < 10000)
								{
									wait = 10000;
								}
								else
								{
									if ( wait < 240000 )
									{
										wait = wait * 2;
									}
								}
							}
							else
							{
								// -- From Twitter Docs -- 
								// When a network error (TCP/IP level) is encountered, back off linearly. 
								// Perhaps start at 250 milliseconds and cap at 16 seconds.
								// Linear Backoff
								if ( wait < 16000 )
								{
									wait += 250;
								}
							}
							Thread.Sleep(wait);
						}
					}
					if ( mrSocket != null )
						mrSocket.StopStream();
					break;
				}
				catch (Exception ex)
				{
					string sError = "TwitterReader Exception: " + ex.Message;
					if ( bVerboseStatus )
						Debug.WriteLine(sError);
					SplendidError.SystemMessage(mrConnector.Context, "Error", new StackTrace(true).GetFrame(0), sError);
				}
			}
		}
	}
}
