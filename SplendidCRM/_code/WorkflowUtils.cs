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
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM
{
	public class WorkflowUtils
	{
		private static bool bInsideWorkflow = false;

		#region spWORKFLOW_EVENTS_Delete
		/// <summary>
		/// spWORKFLOW_EVENTS_Delete
		/// </summary>
		public static void spWORKFLOW_EVENTS_Delete(HttpApplicationState Application, Guid gID)
		{
			if ( HttpContext.Current != null && HttpContext.Current.Application != null )
			{
				// 12/22/2007 Paul.  By calling the SqlProcs version, we will ensure a compile-time error if the parameters change. 
				SqlProcs.spWORKFLOW_EVENTS_Delete(gID);
			}
			else
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
					using ( IDbTransaction trn = Sql.BeginTransaction(con) )
					{
						try
						{
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.Transaction = trn;
								cmd.CommandType = CommandType.StoredProcedure;
								cmd.CommandText = "spWORKFLOW_EVENTS_Delete";
								IDbDataParameter parID                 = Sql.AddParameter(cmd, "@ID"                , gID       );
								IDbDataParameter parMODIFIED_USER_ID   = Sql.AddParameter(cmd, "@MODIFIED_USER_ID"  , Guid.Empty);
								cmd.ExecuteNonQuery();
							}
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
		#endregion

		#region spWORKFLOW_EVENTS_ProcessAll
		/// <summary>
		/// spWORKFLOW_EVENTS_ProcessAll
		/// </summary>
		public static void spWORKFLOW_EVENTS_ProcessAll(HttpApplicationState Application)
		{
			if ( HttpContext.Current != null && HttpContext.Current.Application != null )
			{
				// 12/22/2007 Paul.  By calling the SqlProcs version, we will ensure a compile-time error if the parameters change. 
				SqlProcs.spWORKFLOW_EVENTS_ProcessAll();
			}
			else
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
					using ( IDbTransaction trn = Sql.BeginTransaction(con) )
					{
						try
						{
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.Transaction = trn;
								cmd.CommandType = CommandType.StoredProcedure;
								cmd.CommandText = "spWORKFLOW_EVENTS_ProcessAll";
								cmd.ExecuteNonQuery();
							}
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
		#endregion

		public static void Process(HttpContext Context)
		{
			if ( !bInsideWorkflow )
			{
				bInsideWorkflow = true;
				try
				{
					//SplendidError.SystemMessage(Context, "Warning", new StackTrace(true).GetFrame(0), "WorkflowUtils.Process Begin");

					spWORKFLOW_EVENTS_ProcessAll(Context.Application);
					/*
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						string sSQL ;
						sSQL = "select *                " + ControlChars.CrLf
						     + "  from vwWORKFLOW_EVENTS" + ControlChars.CrLf
						     + " order by AUDIT_VERSION " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							con.Open();

							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									if ( dt.Rows.Count > 0 )
										SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), "Processing " + dt.Rows.Count.ToString() + " workflow events");
									foreach ( DataRow row in dt.Rows )
									{
										Guid gID = Sql.ToGuid(row["ID"]);
										// 12/30/2007 Paul.  We are not going to do anything yet, but we do need to clean up the table. 
										spWORKFLOW_EVENTS_Delete(Application, gID);
									}
								}
							}
						}
					}
					*/
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
				}
				finally
				{
					bInsideWorkflow = false;
				}
			}
		}
	}
}
