<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DetailView.ascx.cs" Inherits="SplendidCRM.Administration.Schedulers.DetailView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divDetailView" runat="server">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" Module="Schedulers" Title="Schedulers.LBL_MODULE_TITLE" EnablePrint="true" HelpName="DetailView" EnableHelp="true" Runat="Server" />

	<asp:Table Width="100%" CellPadding="0" CellSpacing="0" CssClass="tabDetailView" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="15%" VerticalAlign="Top" CssClass="tabDetailViewDL"><%= L10n.Term("Schedulers.LBL_JOB"   ) %></asp:TableCell>
			<asp:TableCell Width="35%" VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="JOB"    Runat="server" /></asp:TableCell>
			<asp:TableCell Width="15%" VerticalAlign="Top" CssClass="tabDetailViewDL"><%= L10n.Term("Schedulers.LBL_STATUS") %></asp:TableCell>
			<asp:TableCell Width="35%" VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="STATUS" Runat="server" /></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDL"><%= L10n.Term("Schedulers.LBL_DATE_TIME_START") %></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="DATE_TIME_START" Runat="server" /></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDL"><%= L10n.Term("Schedulers.LBL_TIME_FROM"      ) %></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="TIME_FROM"       Runat="server" /></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDL"><%= L10n.Term("Schedulers.LBL_DATE_TIME_END") %></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="DATE_TIME_END" Runat="server" /></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDL"><%= L10n.Term("Schedulers.LBL_TIME_TO"      ) %></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="TIME_TO"       Runat="server" /></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDL"><%= L10n.Term("Schedulers.LBL_LAST_RUN") %></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="LAST_RUN"     Runat="server" /></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDL"><%= L10n.Term("Schedulers.LBL_INTERVAL") %></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="JOB_INTERVAL" Runat="server" /></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDL"><%= L10n.Term("Schedulers.LBL_CATCH_UP"    ) %></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="CATCH_UP"     Runat="server" /></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDL">&nbsp;</asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDF">&nbsp;</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDL"><%= L10n.Term(".LBL_DATE_ENTERED" ) %></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="DATE_ENTERED"  Runat="server" /></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDL"><%= L10n.Term(".LBL_DATE_MODIFIED") %></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="DATE_MODIFIED" Runat="server" /></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />

