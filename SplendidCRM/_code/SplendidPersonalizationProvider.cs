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
using System.Data;
using System.Web.UI.WebControls.WebParts;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System.Diagnostics;

namespace SplendidCRM
{
	public class SplendidPersonalizationProvider : PersonalizationProvider
	{
		protected string sApplicationName;

		public override string ApplicationName
		{
			get { return ApplicationName; }
			set { sApplicationName = value; }
		}

		/*
		public override void Initialize(string name, NameValueCollection config)
		{
			// Assign the provider a default name if it doesn't have one
			if ( String.IsNullOrEmpty(name) )
				name = "SplendidPersonalizationProvider";

			// Add a default "description" attribute to config if the
			// attribute doesn't exist or is empty
			if ( String.IsNullOrEmpty(config["description"]) )
			{
				config.Remove("description");
				config.Add("description", "Text file personalization provider");
			}

			// Call the base class's Initialize method
			base.Initialize(name, config);
		}
		*/

		public static void USER_PREFERENCES_Write(Guid gID, byte[] blob, IDbTransaction trn)
		{
			using ( MemoryStream stm = new MemoryStream(blob) )
			{
				const int BUFFER_LENGTH = 4*1024;
				byte[] binFILE_POINTER = new byte[16];
				
				SqlProcs.spUSER_PREFERENCES_InitPointer(gID, ref binFILE_POINTER, trn);
				using ( BinaryReader reader = new BinaryReader(stm) )
				{
					int nFILE_OFFSET = 0 ;
					byte[] binBYTES = reader.ReadBytes(BUFFER_LENGTH);
					while ( binBYTES.Length > 0 )
					{
						SqlProcs.spUSER_PREFERENCES_WriteOffset(gID, binFILE_POINTER, nFILE_OFFSET, binBYTES, trn);
						nFILE_OFFSET += binBYTES.Length;
						binBYTES = reader.ReadBytes(BUFFER_LENGTH);
					}
				}
			}
		}

		protected override void LoadPersonalizationBlobs(WebPartManager webPartManager, string path, string userName, ref byte[] sharedDataBlob, ref byte[] userDataBlob)
		{
			sharedDataBlob = null;
			userDataBlob   = null;
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL ;
					sSQL = "select ID                      " + ControlChars.CrLf
					     + "  from vwUSER_PREFERENCES      " + ControlChars.CrLf
					     + " where CATEGORY = @CATEGORY    " + ControlChars.CrLf
					     + "   and ASSIGNED_USER_ID is null" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@CATEGORY", path);
						Guid gID = Sql.ToGuid(cmd.ExecuteScalar());
						if ( !Sql.IsEmptyGuid(gID) )
						{
							// 09/06/2008 Paul.  PostgreSQL does not require that we stream the bytes, so lets explore doing this for all platforms. 
							if ( Sql.StreamBlobs(con) )
							{
								// 07/15/2008 Paul.  Move USER_PREFERENCES_Read to Sql so that it can be reused. 
								sharedDataBlob = Sql.ReadImage(gID, con, "spUSER_PREFERENCES_ReadOffset");
							}
							else
							{
								cmd.Parameters.Clear();
								sSQL = "select CONTENT                   " + ControlChars.CrLf
								     + "  from vwUSER_PREFERENCES_CONTENT" + ControlChars.CrLf
								     + " where ID = @ID                  " + ControlChars.CrLf;
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@ID", gID);
								object oBlob = cmd.ExecuteScalar();
								sharedDataBlob = Sql.ToByteArray(oBlob);
							}
						}
					}
					// Load private state if userName holds a user name
					if ( !String.IsNullOrEmpty(userName) )
					{
						if ( userName.IndexOf('\\') >= 0 )
							userName = userName.Substring(userName.IndexOf('\\')+1);
						
						sSQL = "select ID                                      " + ControlChars.CrLf
						     + "  from vwUSER_PREFERENCES                      " + ControlChars.CrLf
						     + " where CATEGORY           = @CATEGORY          " + ControlChars.CrLf
						     + "   and ASSIGNED_USER_NAME = @ASSIGNED_USER_NAME" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@CATEGORY"          , path.ToLower()    );
							Sql.AddParameter(cmd, "@ASSIGNED_USER_NAME", userName.ToLower());
							Guid gID = Sql.ToGuid(cmd.ExecuteScalar());
							if ( !Sql.IsEmptyGuid(gID) )
							{
								// 09/06/2008 Paul.  PostgreSQL does not require that we stream the bytes, so lets explore doing this for all platforms. 
								if ( Sql.StreamBlobs(con) )
								{
									// 07/15/2008 Paul.  Move USER_PREFERENCES_Read to Sql so that it can be reused. 
									userDataBlob = Sql.ReadImage(gID, con, "spUSER_PREFERENCES_ReadOffset");
								}
								else
								{
									cmd.Parameters.Clear();
									sSQL = "select CONTENT                   " + ControlChars.CrLf
									     + "  from vwUSER_PREFERENCES_CONTENT" + ControlChars.CrLf
									     + " where ID = @ID                  " + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@ID", gID);
									object oBlob = cmd.ExecuteScalar();
									userDataBlob = Sql.ToByteArray(oBlob);
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}

		protected override void ResetPersonalizationBlob(WebPartManager webPartManager, string path, string userName)
		{
			if ( userName != null )
			{
				if ( userName.IndexOf('\\') >= 0 )
					userName = userName.Substring(userName.IndexOf('\\')+1);
			}
			try
			{
				SqlProcs.spUSER_PREFERENCES_DeleteByUser( userName, path);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}

		protected override void SavePersonalizationBlob(WebPartManager webPartManager, string path, string userName, byte[] dataBlob)
		{
			if ( userName != null )
			{
				if ( userName.IndexOf('\\') >= 0 )
					userName = userName.Substring(userName.IndexOf('\\')+1);
				userName = userName.ToLower();
			}
			path = path.ToLower();
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
					using ( IDbTransaction trn = Sql.BeginTransaction(con) )
					{
						try
						{
							Guid gID = Guid.Empty;
							SqlProcs.spUSER_PREFERENCES_InsertByUser(ref gID, userName, path, trn);
							// 09/06/2008 Paul.  PostgreSQL does not require that we stream the bytes, so lets explore doing this for all platforms. 
							if ( Sql.StreamBlobs(con) )
							{
								USER_PREFERENCES_Write(gID, dataBlob, trn);
							}
							else
							{
								SqlProcs.spUSER_PREFERENCES_CONTENT_Update(gID, dataBlob, trn);
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
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}

		public override PersonalizationStateInfoCollection FindState(PersonalizationScope scope, PersonalizationStateQuery query, int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotSupportedException();
		}

		public override int GetCountOfState(PersonalizationScope scope, PersonalizationStateQuery query)
		{
			throw new NotSupportedException();
		}

		public override int ResetState(PersonalizationScope scope, string[] paths, string[] usernames)
		{
			// 01/25/2007 Paul.  Strip the domain from the user names. 
			for ( int i=0; i < usernames.Length; i++ )
			{
				string sUserName = usernames[i];
				if ( sUserName.IndexOf('\\') >= 0 )
					usernames[i] = sUserName.Substring(sUserName.IndexOf('\\')+1);
				// 01/25/2007 Paul.  Convert to lowercase to support Oracle. 
				usernames[i] = sUserName.ToLower();
			}
			// 01/25/2007 Paul.  Convert to lowercase to support Oracle. 
			for ( int i=0; i < paths.Length; i++ )
			{
				paths[i] = paths[i].ToLower();
			}
			
			int nResetCount = 0;
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
					using ( IDbTransaction trn = Sql.BeginTransaction(con) )
					{
						try
						{
							string sSQL ;
							sSQL = "select ID                " + ControlChars.CrLf
							     + "  from vwUSER_PREFERENCES" + ControlChars.CrLf
							     + " where 1 = 1             " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.Transaction = trn;
								cmd.CommandText = sSQL;
								Sql.AppendParameter(cmd, paths, "CATEGORY");
								if ( scope == PersonalizationScope.User )
									Sql.AppendParameter(cmd, usernames, "ASSIGNED_USER_NAME");
								using ( IDataReader rdr = cmd.ExecuteReader() )
								{
									while ( rdr.Read() )
									{
										Guid gID = Sql.ToGuid(rdr["ID"]);
										SqlProcs.spUSER_PREFERENCES_Delete(gID, trn);
										nResetCount++;
									}
								}
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
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
			return nResetCount;
		}

		public override int ResetUserState(string path, DateTime userInactiveSinceDate)
		{
			int nResetCount = 0;
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
					using ( IDbTransaction trn = Sql.BeginTransaction(con) )
					{
						try
						{
							string sSQL ;
							sSQL = "select ID                            " + ControlChars.CrLf
							     + "  from vwUSER_PREFERENCES            " + ControlChars.CrLf
							     + " where CATEGORY      = @CATEGORY     " + ControlChars.CrLf
							     + "   and DATE_MODIFIED < @DATE_MODIFIED" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.Transaction = trn;
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@CATEGORY"     , path.ToLower());
								Sql.AddParameter(cmd, "@DATE_MODIFIED", userInactiveSinceDate);
								using ( IDataReader rdr = cmd.ExecuteReader() )
								{
									while ( rdr.Read() )
									{
										Guid gID = Sql.ToGuid(rdr["ID"]);
										SqlProcs.spUSER_PREFERENCES_Delete(gID, trn);
										nResetCount++;
									}
								}
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
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
			return nResetCount;
		}
	}
}

