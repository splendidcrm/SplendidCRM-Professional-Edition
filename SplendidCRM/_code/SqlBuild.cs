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
using System.Web;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace SplendidCRM
{
	// 08/01/2015 Paul.  The Microsoft Web Platform Installer is unable to deploy due to a timeout when applying the Build.sql file. 
	// Increasing the timeout in the Manifest.xml does not solve the problem. 
	// 06/30/2018 Paul.  Move SqlBuild to separate file. 
	public class SqlBuild
	{
		// http://stackoverflow.com/questions/3773857/escape-curly-brace-in-string-format
		protected const string sProgressTemplate = @"
<html>
<head>
<style type=""text/css"">
.ProgressBarFrame {{ padding: 2px; border: 1px solid #cccccc; width: 60%; background-color: #ffffff; }}
.ProgressBar      {{ background-color: #000000; }}
.ProgressBar td   {{ color: #ffffff; font-size: 12px; font-style: normal; font-weight: normal; text-decoration: none; }}
.QuestionError    {{ color: #e00000; font-size: 11px; font-style: normal; font-weight: bold; text-decoration: none; background-color: inherit; }}
</style>
</head>
<script type=""text/javascript"">
setTimeout(function()
{{
	location.reload();
}}, 3000);
</script>
<body>
The SplendidCRM database is being built.
<div class=""ProgressBarFrame"" align=""left"">
	<table cellspacing=""0"" width=""100%"" class=""ProgressBar"" style=""width: {0}%;"">
		<tbody class=""ProgressBar"">
			<tr>
				<td align=""center"" style=""padding: 2px;"">{1}%</td>
			</tr>
		</tbody>
	</table>
</div>
<div class=""QuestionError"">{2}</div>
<pre>{3}</pre>
</body>
</html>";
		protected const string sErrorTemplate = @"<html>
<head>
<style type=""text/css"">
</style>
</head>
<body>
There were errors during the SplendidCRM database build process. 
To manually enable SplendidCRM, you will need to delete the app_offline.htm file at the root of the web site. 
<pre>%0</pre>
</body>
</html>";

		public class BuildState
		{
			private HttpContext Context;
			private string[]    arrSQL ;
			
			public BuildState(HttpContext Context, string[] arrSQL)
			{
				this.Context = Context;
				this.arrSQL  = arrSQL;
			}
			
			public void Start()
			{
				string sBuildLogPath = Context.Server.MapPath("~/App_Data/Build.log");
				try
				{
					string sOfflinePath = Context.Server.MapPath("~/app_offline.htm");
					try
					{
						Debug.WriteLine(DateTime.Now.ToString() + " Begin");
						File.AppendAllText(sBuildLogPath, DateTime.Now.ToString() + " Begin" + ControlChars.CrLf);
					}
					catch
					{
						// The App_Data folder may be read-only, so protect against exception. 
					}
					
					int nErrors = 0;
					StringBuilder sbLogText = new StringBuilder();
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						for ( int i = 0; i < arrSQL.Length; i++ )
						{
							string sSQL = arrSQL[i].Trim();
							if ( !String.IsNullOrEmpty(sSQL) )
							{
								int nProgress = (100 * i) / arrSQL.Length;
								try
								{
									// 08/02/2015 Paul.  Do not include the SQL as it would confuse users. 
									string sOfflineHtml = String.Format(sProgressTemplate, nProgress, nProgress, sbLogText.ToString(), String.Empty);
									try
									{
										File.WriteAllText(sOfflinePath, sOfflineHtml);
									}
									catch(Exception ex)
									{
										// There may be an exception if we try and write the file and IIS is trying to deliver the file. Just ignore. 
										Debug.WriteLine(ex.Message);
									}
#if DEBUG
									int nEndOfLine = sSQL.IndexOf(ControlChars.CrLf);
									string sFirstLine = (nEndOfLine > 0) ? sSQL.Substring(0, nEndOfLine) : sSQL;
									Debug.WriteLine(sFirstLine);
#endif
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandTimeout = 0;
										cmd.CommandText = sSQL;
										cmd.ExecuteNonQuery();
									}
								}
								catch(Exception ex)
								{
									nErrors++;
									string sThisError = i.ToString() + ": " + ex.Message + ControlChars.CrLf;
									sbLogText.Append(sThisError);
									try
									{
										File.AppendAllText(sBuildLogPath, DateTime.Now.ToString() + " - " + sThisError + sSQL + ControlChars.CrLf + ControlChars.CrLf);
									}
									catch
									{
										// The App_Data folder may be read-only, so protect against exception. 
									}
								}
							}
						}
					}
					try
					{
						Debug.WriteLine(DateTime.Now.ToString() + " End");
						File.AppendAllText(sBuildLogPath, DateTime.Now.ToString() + " End" + ControlChars.CrLf);
					}
					catch
					{
						// The App_Data folder may be read-only, so protect against exception. 
					}
					if ( nErrors > 0 )
					{
						SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), sbLogText.ToString());
						string sOfflineHtml = String.Format(sErrorTemplate, sbLogText.ToString());
						File.WriteAllText(sOfflinePath, sOfflineHtml);
					}
					else
					{
						if ( File.Exists(sOfflinePath) )
							File.Delete(sOfflinePath);
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), ex);
					try
					{
						File.AppendAllText(sBuildLogPath, DateTime.Now.ToString() + " - " + ex.Message + ControlChars.CrLf + ControlChars.CrLf);
					}
					catch
					{
						// The App_Data folder may be read-only, so protect against exception. 
					}
				}
			}
		}

		public static void BuildDatabase(HttpContext Context)
		{
			string sBuildSqlPath = Context.Server.MapPath("~/App_Data/Build.sql");
			try
			{
				// 08/01/2015 Paul.  If Build.log exists, then we have already processed the build.sql file, so skip. 
				if ( File.Exists(sBuildSqlPath) )
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							string sSQL = "select count(*) from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'CONFIG'";
							cmd.CommandTimeout = 0;
							cmd.CommandText = sSQL;
							int nTables = Sql.ToInteger(cmd.ExecuteScalar());
							if ( nTables == 0 )
							{
								// 08/12/2015 Paul.  Read the file after checking for a valid database. 
								string sBuildSQL = File.ReadAllText(sBuildSqlPath);
								if ( !String.IsNullOrEmpty(sBuildSQL) )
								{
									sBuildSQL = sBuildSQL.Replace(ControlChars.CrLf + "go" + ControlChars.CrLf, ControlChars.CrLf + "GO" + ControlChars.CrLf);
									sBuildSQL = sBuildSQL.Replace(ControlChars.CrLf + "Go" + ControlChars.CrLf, ControlChars.CrLf + "GO" + ControlChars.CrLf);
									string[] arrSQL = Microsoft.VisualBasic.Strings.Split(sBuildSQL, ControlChars.CrLf + "GO" + ControlChars.CrLf, -1, Microsoft.VisualBasic.CompareMethod.Text);
									if ( arrSQL.Length > 1 )
									{
										string sOfflinePath = Context.Server.MapPath("~/app_offline.htm");
										try
										{
											string sOfflineHtml = String.Format(sProgressTemplate, 0, 0, String.Empty, String.Empty);
											File.WriteAllText(sOfflinePath, sOfflineHtml);
											// 08/01/2015 Paul.  Send content and flush so that the browser will refresh. 
											Context.Response.Write(sOfflineHtml);
											Context.Response.Flush();
										}
										catch(Exception ex)
										{
											// There may be an exception if we try and write the file and IIS is trying to deliver the file. Just ignore. 
											Debug.WriteLine(ex.Message);
										}
										BuildState build = new BuildState(Context, arrSQL);
										//System.Threading.Thread t = new System.Threading.Thread(build.Start);
										//t.Start();
										// 08/01/2015 Paul.  Can't use a thread as IIS will terminate it. 
										build.Start();
									}
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), ex);
			}
		}
	}
}
