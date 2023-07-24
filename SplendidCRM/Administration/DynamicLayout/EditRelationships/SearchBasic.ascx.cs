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

namespace SplendidCRM.Administration.DynamicLayout.EditRelationships
{
	/// <summary>
	///		Summary description for SearchBasic.
	/// </summary>
	public class SearchBasic : SearchControl
	{
		protected DropDownList lstLAYOUT_VIEWS;
		protected string sViewTableName;
		protected string sViewFieldName;

		public string NAME
		{
			get
			{
				return lstLAYOUT_VIEWS.SelectedValue;
			}
		}

		public string ViewTableName
		{
			get { return sViewTableName; }
			set { sViewTableName = value; }
		}

		public string ViewFieldName
		{
			get { return sViewFieldName; }
			set { sViewFieldName = value; }
		}

		public override void SqlSearchClause(IDbCommand cmd)
		{
			Sql.AppendParameter(cmd, lstLAYOUT_VIEWS, sViewFieldName);
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 01/06/2006 Paul.  Try disabling viewstate of DetailView to prevent viewstate error. 
			if ( !this.IsPostBack || !Parent.EnableViewState )
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					// 05/04/2008 Paul.  Protect against SQL Injection. A table name will never have a space character.
					sViewTableName = sViewTableName.Replace(" ", "");
					// 04/19/2010 Paul.  The EditView Relationships will only apply to EditViews. 
					// We are going to specifically exclude PopupViews, and Mobile Views. 
					sSQL = "select *                      " + ControlChars.CrLf
					     + "  from " + sViewTableName       + ControlChars.CrLf
					     + " where NAME like N'%.EditView'" + ControlChars.CrLf
					     + " order by DISPLAY_NAME        " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						try
						{
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									lstLAYOUT_VIEWS.DataSource = dt;
									lstLAYOUT_VIEWS.DataBind();
									lstLAYOUT_VIEWS.Items.Insert(0, String.Empty);

									// 01/08/2006 Paul.  The viewstate is no longer disabled, so this is not necessary. 
									/*
									try
									{
										// 01/06/2006 Paul.  If viewstate has been disabled, then recall the submitted value. 
										if ( !Parent.EnableViewState )
										{
											string sNAME = Sql.ToString(Request[ListUniqueID]);
											// 08/19/2010 Paul.  Check the list before assigning the value. 
											Utils.SetSelectedValue(lstLAYOUT_VIEWS, sNAME);
										}
									}
									catch
									{
									}
									*/
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

