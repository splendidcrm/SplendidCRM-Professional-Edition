<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DetailView.ascx.cs" Inherits="SplendidCRM.Administration.Config.DetailView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" Module="Administration" EnablePrint="true" HelpName="DetailView" EnableHelp="true" Runat="Server" />

	<table class="tabDetailView">
		<tr>
			<td width="15%"  valign="top" class="tabDetailViewDL"><%= L10n.Term("Config.LBL_NAME"    ) %></td>
			<td width="35%"  valign="top" class="tabDetailViewDF"><asp:Label ID="txtNAME"     Runat="server" /></td>
			<td width="15%"  valign="top" class="tabDetailViewDL"><%= L10n.Term("Config.LBL_CATEGORY") %></td>
			<td width="35%"  valign="top" class="tabDetailViewDF"><asp:Label ID="txtCATEGORY" Runat="server" /></td>
		</tr>
		<tr>
			<td valign="top" class="tabDetailViewDL"><%= L10n.Term("Config.LBL_VALUE") %></td>
			<td colspan="3" class="tabDetailViewDF"><asp:Label ID="txtVALUE" Runat="server" /></td>
		</tr>
	</table>
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />

