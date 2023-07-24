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
using System.Drawing;
using System.Net;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Text;
using System.Diagnostics;

namespace SplendidCRM.Administration.SyncSchema
{
	/// <summary>
	///		Summary description for VerifyProcedures.
	/// </summary>
	public class VerifyProcedures : SyncControl
	{
		protected _controls.ModuleHeader ctlModuleHeader;
		protected WizardButtons          ctlWizardButtons;

		protected Label   lblSOURCE_PROVIDER       ;
		protected Label   lblDESTINATION_PROVIDER  ;
		protected Label   lblSOURCE_CONNECTION     ;
		protected Label   lblDESTINATION_CONNECTION;
		protected Label   lblSourceError           ;
		protected Label   lblDestinationError      ;
		protected Literal litSOURCE_UNIQUE         ;
		protected Literal litDESTINATION_UNIQUE    ;
		protected Literal litSOURCE_LIST           ;
		protected Literal litDESTINATION_LIST      ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Next" )
			{
				if ( Command != null )
					Command(sender, e);
			}
			else if ( e.CommandName == "Previous" )
			{
				if ( Command != null )
					Command(sender, e);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				SetPageTitle(L10n.Term(".moduleList.Administration"));
				XmlDocument xml = GetXml();

				lblSOURCE_PROVIDER.Text        = XmlUtil.SelectSingleNode(xml, "Source/Provider"             );
				lblDESTINATION_PROVIDER.Text   = XmlUtil.SelectSingleNode(xml, "Destination/Provider"        );
				lblSOURCE_CONNECTION.Text      = XmlUtil.SelectSingleNode(xml, "Source/ConnectionString"     );
				lblDESTINATION_CONNECTION.Text = XmlUtil.SelectSingleNode(xml, "Destination/ConnectionString");

				XmlNode nodeSource      = xml.DocumentElement.SelectSingleNode("Source"     );
				XmlNode nodeDestination = xml.DocumentElement.SelectSingleNode("Destination");
				try
				{
					litSOURCE_LIST.Text = LoadNames(nodeSource, "Procedures", "Procedure", lblSOURCE_PROVIDER.Text, lblSOURCE_CONNECTION.Text, GetProceduresCommand(lblSOURCE_PROVIDER.Text));
				}
				catch(Exception ex)
				{
					lblSourceError.Text = ex.Message;
				}
				try
				{
					litDESTINATION_LIST.Text = LoadNames(nodeDestination, "Procedures", "Procedure", lblDESTINATION_PROVIDER.Text, lblDESTINATION_CONNECTION.Text, GetProceduresCommand(lblDESTINATION_PROVIDER.Text));
				}
				catch(Exception ex)
				{
					lblDestinationError.Text = ex.Message;
				}
				StringBuilder sbSourceUnique      = new StringBuilder();
				StringBuilder sbDestinationUnique = new StringBuilder();
				CompareNames(nodeSource, nodeDestination, "Procedures", "Procedure", ref sbSourceUnique, ref sbDestinationUnique);
				litSOURCE_UNIQUE.Text = sbSourceUnique.ToString();
				litDESTINATION_UNIQUE.Text = sbDestinationUnique.ToString();
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlWizardButtons.ErrorText = ex.Message;
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
			ctlWizardButtons.Command += new CommandEventHandler(Page_Command);
		}
		#endregion
	}
}
