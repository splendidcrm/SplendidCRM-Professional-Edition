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
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Diagnostics;

namespace SplendidCRM.Administration.SyncSchema
{
	/// <summary>
	/// Summary description for Default.
	/// </summary>
	public class Default : SplendidAdminPage
	{
		protected PlaceHolder     plcSyncStep;
		protected XmlDocument     xml        ;
		protected HtmlInputHidden txtStep    ;
		protected HtmlInputHidden txtXML     ;

		protected void SetStep(string sStep)
		{
			SyncControl ctlStep = null;
			switch ( sStep )
			{
				case "SpecifyDatabases" :  ctlStep = (SyncControl) LoadControl("SpecifyDatabases.ascx" );  break;
				case "VerifyTables"     :  ctlStep = (SyncControl) LoadControl("VerifyTables.ascx"     );  break;
				case "VerifyColumns"    :  ctlStep = (SyncControl) LoadControl("VerifyColumns.ascx"    );  break;
				case "VerifyViews"      :  ctlStep = (SyncControl) LoadControl("VerifyViews.ascx"      );  break;
				case "VerifyProcedures" :  ctlStep = (SyncControl) LoadControl("VerifyProcedures.ascx" );  break;
				case "VerifyFunctions"  :  ctlStep = (SyncControl) LoadControl("VerifyFunctions.ascx"  );  break;
				case "Summary"          :  ctlStep = (SyncControl) LoadControl("SpecifyDatabases.ascx" );  break;
				default                 :  ctlStep = (SyncControl) LoadControl("SpecifyDatabases.ascx" );  break;
			}
			plcSyncStep.Controls.Clear();
			plcSyncStep.Controls.Add(ctlStep);
			ctlStep.Command += new CommandEventHandler(Page_Command);
			txtStep.Value = sStep;
			// 07/10/2006 Paul.  We can't bind here if SetStep() is to be called within InitializeComponent(). 
			//Page.DataBind();
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				XmlDocument xml = new XmlDocument();
				// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
				// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
				// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
				xml.XmlResolver = null;
				try
				{
					xml.LoadXml(Server.HtmlDecode(txtXML.Value));
				}
				catch
				{
				}
				string sStep = Sql.ToString(txtStep.Value);
				if ( e.CommandName == "Next" )
				{
					switch ( sStep )
					{
						case "Summary"          :  sStep = "VerifyTables"     ;  break;
						case ""                 :  sStep = "VerifyTables"     ;  break;
						case "SpecifyDatabases" :  sStep = "VerifyTables"     ;  break;
						case "VerifyTables"     :  sStep = "VerifyColumns"    ;  break;
						case "VerifyColumns"    :  sStep = "VerifyViews"      ;  break;
						case "VerifyViews"      :  sStep = "VerifyProcedures" ;  break;
						case "VerifyProcedures" :  sStep = "VerifyFunctions"  ;  break;
						case "VerifyFunctions"  :  sStep = "Summary"          ;  break;
					}
				}
				else if ( e.CommandName == "Previous" )
				{
					switch ( sStep )
					{
						case "Summary"          :  sStep = "SpecifyDatabases" ;  break;
						case ""                 :  sStep = "SpecifyDatabases" ;  break;
						case "VerifyTables"     :  sStep = "SpecifyDatabases" ;  break;
						case "VerifyColumns"    :  sStep = "VerifyTables"     ;  break;
						case "VerifyViews"      :  sStep = "VerifyColumns"    ;  break;
						case "VerifyProcedures" :  sStep = "VerifyViews"      ;  break;
						case "VerifyFunctions"  :  sStep = "VerifyProcedures" ;  break;
					}
				}
				XmlUtil.SetSingleNode(xml, "Step", sStep);
				txtXML.Value = Server.HtmlEncode(xml.OuterXml);
				SetStep(sStep);
				// 07/10/2006 Paul.  Move DataBind here. 
				Page.DataBind();
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( !IsPostBack )
			{
				// 06/09/2006 Paul.  The primary data binding will now only occur in the ASPX pages so that this is only one per cycle. 
				// 03/11/2008 Paul.  Move the primary binding to SplendidPage. 
				//Page DataBind();
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
			this.IsAdminPage = true;
			this.Load += new System.EventHandler(this.Page_Load);
			string sStep = Sql.ToString(Request[txtStep.ClientID]);
			SetStep(sStep);
		}
		#endregion
	}
}
