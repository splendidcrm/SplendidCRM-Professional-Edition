<%@ Page language="c#" MasterPageFile="~/DetailView.Master" Codebehind="view.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Reports.View" %>
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

</script>
<asp:Content ID="cntSidebar" ContentPlaceHolderID="cntSidebar" runat="server">
	<%@ Register TagPrefix="SplendidCRM" Tagname="Shortcuts" Src="~/_controls/Shortcuts.ascx" %>
	<SplendidCRM:Shortcuts ID="ctlShortcuts" SubMenu="Reports" Runat="Server" />
</asp:Content>

<asp:Content ID="cntBody" ContentPlaceHolderID="cntBody" runat="server">
<SplendidCRM:InlineScript runat="server">
<script type="text/javascript">
// 03/09/2021 Paul.  Multi-selection listboxes are not displaying their selected values, so manually correct. 
window.onload = function()
{
	<%
	foreach ( System.Data.DataRow row in dtCorrected.Rows )
	{
	%>
		SelectOption('<%= Sql.ToString(row["NAME"]) %>', '<%= Sql.EscapeJavaScript(Sql.ToString(row["VALUE"])) %>');
	<%
	}
	%>
}
</script>
</SplendidCRM:InlineScript>

	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true" EditView="true" Module="Reports" HelpName="ReportView" EnableHelp="true" EnableFavorites="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="ParameterView" Src="ParameterView.ascx" %>
	<SplendidCRM:ParameterView ID="ctlParameterView" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="ReportView" Src="ReportView.ascx" %>
	<SplendidCRM:ReportView ID="ctlReportView" Visible='<%# SplendidCRM.Security.GetUserAccess("Reports", "view") >= 0 %>' Runat="Server" />
	<asp:Label ID="lblAccessError" ForeColor="Red" EnableViewState="false" Text='<%# L10n.Term("ACL.LBL_NO_ACCESS") %>' Visible="<%# !ctlReportView.Visible %>" Runat="server" />
</asp:Content>
