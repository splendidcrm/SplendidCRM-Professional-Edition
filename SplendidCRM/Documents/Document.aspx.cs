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
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace SplendidCRM.Documents
{
	/// <summary>
	/// Summary description for Document.
	/// </summary>
	public class Document : SplendidPage
	{
		// 10/20/2009 Paul.  Move blob logic to WriteStream. 
		public static void WriteStream(Guid gID, IDbConnection con, BinaryWriter writer)
		{
			// 09/06/2008 Paul.  PostgreSQL does not require that we stream the bytes, so lets explore doing this for all platforms. 
			if ( Sql.StreamBlobs(con) )
			{
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = "spDOCUMENTS_CONTENT_ReadOffset";
					cmd.CommandType = CommandType.StoredProcedure;
					
					const int BUFFER_LENGTH = 4*1024;
					int idx  = 0;
					int size = 0;
					byte[] binData = new byte[BUFFER_LENGTH];  // 10/20/2005 Paul.  This allocation is only used to set the parameter size. 
					IDbDataParameter parID          = Sql.AddParameter(cmd, "@ID"         , gID    );
					IDbDataParameter parFILE_OFFSET = Sql.AddParameter(cmd, "@FILE_OFFSET", idx    );
					// 01/21/2006 Paul.  Field was renamed to READ_SIZE. 
					IDbDataParameter parREAD_SIZE   = Sql.AddParameter(cmd, "@READ_SIZE"  , size   );
					IDbDataParameter parBYTES       = Sql.AddParameter(cmd, "@BYTES"      , binData);
					parBYTES.Direction = ParameterDirection.InputOutput;
					do
					{
						parID         .Value = gID          ;
						parFILE_OFFSET.Value = idx          ;
						parREAD_SIZE  .Value = BUFFER_LENGTH;
						size = 0;
						// 08/14/2005 Paul.  Oracle returns the bytes in a field.
						// SQL Server can only return the bytes in a resultset. 
						// 10/20/2005 Paul.  MySQL works returning bytes in an output parameter. 
						// 02/05/2006 Paul.  DB2 returns bytse in a field. 
						if ( Sql.IsOracle(cmd) || Sql.IsDB2(cmd) ) // || Sql.IsMySQL(cmd) )
						{
							cmd.ExecuteNonQuery();
							binData = Sql.ToByteArray(parBYTES);
							if ( binData != null )
							{
								size = binData.Length;
								writer.Write(binData);
								idx += size;
							}
						}
						else
						{
							using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
							{
								if ( rdr.Read() )
								{
									// 10/20/2005 Paul.  MySQL works returning a record set, but it cannot be cast to a byte array. 
									// binData = (byte[]) rdr[0];
									binData = Sql.ToByteArray((System.Array) rdr[0]);
									if ( binData != null )
									{
										size = binData.Length;
										writer.Write(binData);
										idx += size;
									}
								}
							}
						}
					}
					while ( size == BUFFER_LENGTH );
				}
			}
			else
			{
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					string sSQL;
					sSQL = "select CONTENT                     " + ControlChars.CrLf
					     + "  from vwDOCUMENT_REVISIONS_CONTENT" + ControlChars.CrLf
					     + " where ID = @ID                    " + ControlChars.CrLf;
					Sql.AddParameter(cmd, "@ID", gID);
					cmd.CommandText = sSQL;
					//object oBlob = cmd.ExecuteScalar();
					//byte[] binData = Sql.ToByteArray(oBlob);
					//writer.Write(binData);
					using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
					{
						if ( rdr.Read() )
						{
							// 10/20/2009 Paul.  Try to be more efficient by using a reader. 
							Sql.WriteStream(rdr, 0, writer);
						}
					}
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				// 05/05/2010 Paul.  Lets make it easy to get the current revision of a document. 
				Guid gID = Sql.ToGuid(Request["ID"]);
				Guid gDOCUMENT_ID = Sql.ToGuid(Request["DOCUMENT_ID"]);
				if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDOCUMENT_ID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL ;
							Guid gDOCUMENT_REVISION_ID = Guid.Empty;
							if ( !Sql.IsEmptyGuid(gDOCUMENT_ID) )
							{
								sSQL = "select DOCUMENT_REVISION_ID" + ControlChars.CrLf
								     + "     , FILE_MIME_TYPE      " + ControlChars.CrLf
								     + "     , FILENAME            " + ControlChars.CrLf
								     + "  from vwDOCUMENTS         " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Security.Filter(cmd, "Documents", "view");
									Sql.AppendParameter(cmd, gDOCUMENT_ID, "ID", false);
									using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
									{
										if ( rdr.Read() )
										{
											gDOCUMENT_REVISION_ID = Sql.ToGuid(rdr["DOCUMENT_REVISION_ID"]);
											Response.ContentType = Sql.ToString(rdr["FILE_MIME_TYPE"]);
											// 01/27/2011 Paul.  Don't use GetFileName as the name may contain reserved directory characters, but expect them to be removed in Utils.ContentDispositionEncode. 
											string sFileName = Sql.ToString(rdr["FILENAME"]);
											// 08/06/2008 yxy21969.  Make sure to encode all URLs.
											// 12/20/2009 Paul.  Use our own encoding so that a space does not get converted to a +. 
											Response.AddHeader("Content-Disposition", "attachment;filename=" + Utils.ContentDispositionEncode(Request.Browser, sFileName));
										}
									}
								}
							}
							else
							{
								sSQL = "select ID                  " + ControlChars.CrLf
								     + "     , FILE_MIME_TYPE      " + ControlChars.CrLf
								     + "     , FILENAME            " + ControlChars.CrLf
								     + "  from vwDOCUMENT_REVISIONS" + ControlChars.CrLf
								     + " where ID = @ID            " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@ID", gID);
									using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
									{
										if ( rdr.Read() )
										{
											gDOCUMENT_REVISION_ID = Sql.ToGuid(rdr["ID"]);
											Response.ContentType = Sql.ToString(rdr["FILE_MIME_TYPE"]);
											// 01/27/2011 Paul.  Don't use GetFileName as the name may contain reserved directory characters, but expect them to be removed in Utils.ContentDispositionEncode. 
											string sFileName = Sql.ToString(rdr["FILENAME"]);
											// 08/06/2008 yxy21969.  Make sure to encode all URLs.
											// 12/20/2009 Paul.  Use our own encoding so that a space does not get converted to a +. 
											Response.AddHeader("Content-Disposition", "attachment;filename=" + Utils.ContentDispositionEncode(Request.Browser, sFileName));
										}
									}
								}
							}
							if ( !Sql.IsEmptyGuid(gDOCUMENT_REVISION_ID) )
							{
								using ( BinaryWriter writer = new BinaryWriter(Response.OutputStream) )
								{
									WriteStream(gDOCUMENT_REVISION_ID, con, writer);
								}
							}
							else
							{
								string sMessage = "Image not found.";
								// 04/30/2010 Paul.  Image not found is correct, unless we are an Offline Client. 
								if ( !Sql.IsEmptyString(Sql.ToString(Context.Session["SystemSync.Server"])) )
									sMessage = "Must be online to retrieve image.";
								byte[] byImage = SplendidCRM.Images.Image.RenderAsImage(Response, 300, 100, "Error: " + sMessage, ImageFormat.Gif);
								Response.ContentType = "image/gif";
								Response.BinaryWrite(byImage);
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				Response.Write(ex.Message);
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}

