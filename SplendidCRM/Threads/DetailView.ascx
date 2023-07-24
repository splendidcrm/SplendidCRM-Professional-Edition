<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DetailView.ascx.cs" Inherits="SplendidCRM.Threads.DetailView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" Module="Threads" EnablePrint="true" HelpName="DetailView" EnableHelp="true" Runat="Server" />

	<asp:HyperLink ID="lnkForum" Text='<%# L10n.Term("Threads.LBL_BACK_TO_PARENT") %>' NavigateUrl="~/Forums/" Visible="false" runat="server" />

	<table ID="tblMain" class="tabDetailView" runat="server">
		<tr>
			<td class="tabDetailViewDF"><asp:Label ID="txtTITLE" Font-Bold="true" runat="server" /></td>
			<td width="10%" class="tabDetailViewDL"><asp:Label ID="txtCREATED_BY" runat="server" />:</td>
			<td width="10%" class="tabDetailViewDL" nowrap><asp:Label ID="txtDATE_ENTERED" runat="server" /></td>
		</tr>
		<tr>
			<td colspan="3" style="background-color: #ffffff; padding-left: 3mm; padding-right: 3mm; ">
				<asp:Literal ID="txtDESCRIPTION" runat="server" />
			</td>
		</tr>
		<tr id="trModified" visible="false" runat="server">
			<td class="tabDetailViewDF">&nbsp;</td>
			<td width="10%" class="tabDetailViewDL" nowrap><%# L10n.Term(".LBL_MODIFIED_BY") %>&nbsp;<asp:Label ID="txtMODIFIED_BY" runat="server" />:</td>
			<td width="10%" class="tabDetailViewDL" nowrap><asp:Label ID="txtDATE_MODIFIED" runat="server" /></td>
		</tr>
	</table>

	<div id="divDetailSubPanel">
		<%@ Register TagPrefix="SplendidCRM" Tagname="Posts" Src="Posts.ascx" %>
		<SplendidCRM:Posts ID="ctlPosts" Runat="Server" />
		<%@ Register TagPrefix="SplendidCRM" Tagname="ThreadView" Src="ThreadView.ascx" %>
		<SplendidCRM:ThreadView ID="ctlThreadView" Runat="Server" />

		<asp:PlaceHolder ID="plcSubPanel" Runat="server" />
	</div>
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
