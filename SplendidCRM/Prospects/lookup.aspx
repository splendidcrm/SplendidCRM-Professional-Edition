<%@ Page language="c#" MasterPageFile="~/DefaultView.Master" Codebehind="lookup.aspx.cs" AutoEventWireup="true" Inherits="SplendidCRM.SplendidPage" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.Common" %>
<%@ Import Namespace="System.Diagnostics" %>
<script runat="server">
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

private void Page_Load(object sender, System.EventArgs e)
{
	string m_sMODULE = "Prospects";
	SetMenu(m_sMODULE);
	
	lblError.Text = L10n.Term("ACL.LBL_NO_ACCESS");
	try
	{
		if ( SplendidCRM.Security.GetUserAccess(m_sMODULE, "list") >= 0 )
		{
			SplendidCRM.DbProviderFactory dbf = SplendidCRM.DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				string sTABLE_NAME = SplendidCRM.Crm.Modules.TableName(m_sMODULE);
				sSQL = "select ID             " + ControlChars.CrLf
				     + "  from vw" + sTABLE_NAME + "_List" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Security.Filter(cmd, m_sMODULE, "list");
					cmd.CommandText += "   and (     1 = 0" + ControlChars.CrLf;
					
					string sNAME  = Sql.ToString(Request["NAME" ]);
					SearchBuilder sb = new SearchBuilder(sNAME, cmd);
					cmd.CommandText += sb.BuildQuery("         or ", "NAME"           );
					cmd.CommandText += sb.BuildQuery("         or ", "ACCOUNT_NAME"   );
					
					string sPHONE = Sql.ToString(Request["PHONE"]);
					sb = new SearchBuilder(sPHONE, cmd);
					cmd.CommandText += sb.BuildQuery("         or ", "PHONE_HOME"     );
					cmd.CommandText += sb.BuildQuery("         or ", "PHONE_MOBILE"   );
					cmd.CommandText += sb.BuildQuery("         or ", "PHONE_WORK"     );
					cmd.CommandText += sb.BuildQuery("         or ", "PHONE_OTHER"    );
					cmd.CommandText += sb.BuildQuery("         or ", "PHONE_FAX"      );
					cmd.CommandText += sb.BuildQuery("         or ", "ASSISTANT_PHONE");
					cmd.CommandText += "       )" + ControlChars.CrLf;
					cmd.CommandText += " order by NAME";

					if ( bDebug )
						ClientScript.RegisterClientScriptBlock(typeof(String), "vw" + sTABLE_NAME + "_List", Sql.ClientScriptBlock(cmd));

					using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
					{
						if ( rdr.Read() )
						{
							if ( SplendidCRM.Security.GetUserAccess(m_sMODULE, "view") >= 0 )
							{
								lblError.Text = "Record Found";
								Guid gID = Sql.ToGuid(rdr["ID"]);
								Response.Redirect("view.aspx?ID=" + gID.ToString());
							}
						}
						else
						{
							if ( SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit") >= 0 )
							{
								lblError.Text = "Not Found";
								Response.Redirect("edit.aspx?NAME=" + Server.UrlEncode(sNAME) + "&PHONE=" + Server.UrlEncode(sPHONE));
							}
						}
					}
				}
			}
		}
	}
	catch(Exception ex)
	{
		SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
		lblError.Text = ex.Message;
	}
}
</script>
<asp:Content ID="cntSidebar" ContentPlaceHolderID="cntSidebar" runat="server">
	<%@ Register TagPrefix="SplendidCRM" Tagname="Shortcuts" Src="~/_controls/Shortcuts.ascx" %>
	<SplendidCRM:Shortcuts ID="ctlShortcuts" SubMenu="Prospects" Runat="Server" />
</asp:Content>

<asp:Content ID="cntBody" ContentPlaceHolderID="cntBody" runat="server">
	<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	<p></p>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
	<p></p>
</asp:Content>

