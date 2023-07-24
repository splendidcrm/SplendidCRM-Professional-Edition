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
using System.Web;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace SplendidCRM._controls
{
	/// <summary>
	///		Summary description for LastViewed.
	/// </summary>
	public class LastViewed : SplendidControl
	{
		protected DataView vwLastViewed;
		protected Repeater ctlRepeater ;

		public void Refresh()
		{
			// 05/04/2010 Paul.  LastViewed may not exist in the theme. 
			if ( ctlRepeater == null )
				return;

			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL;
				sSQL = "select *                   " + ControlChars.CrLf
				     + "  from vwTRACKER_LastViewed" + ControlChars.CrLf
				     + " where USER_ID = @USER_ID  " + ControlChars.CrLf
				     + " order by DATE_ENTERED desc" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataSet ds = new DataSet() )
							{
								using ( DataTable dt = new DataTable("vwTRACKER") )
								{
									ds.Tables.Add(dt);
									// 08/16/2005 Paul.  Instead of TOP, use Fill to restrict the records. 
									int nHistoryMaxViewed = Sql.ToInteger(Application["CONFIG.history_max_viewed"]);
									if ( nHistoryMaxViewed == 0 )
										nHistoryMaxViewed = 10;
									// 10/18/2005 Paul.  Start record should be 0. 
									da.Fill(ds, 0, nHistoryMaxViewed, "vwTRACKER");
									
									// 08/17/2005 Paul.  Oracle is having a problem returning an integer column. 
									DataColumn colROW_NUMBER = dt.Columns.Add("ROW_NUMBER", Type.GetType("System.Int32"));
									int nRowNumber = 1;
									foreach(DataRow row in dt.Rows)
									{
										// 10/18/2005 Paul.  AccessKey must be in range of 1 to 9. 
										row["ROW_NUMBER"] = Math.Min(nRowNumber, 9);
										nRowNumber++;
									}
									vwLastViewed = dt.DefaultView;
									ctlRepeater.DataSource = vwLastViewed ;
									ctlRepeater.DataBind();
								}
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					}
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 12/02/2005 Paul.  Always bind as the repeater does not save its state on postback. 
			Refresh();
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}

