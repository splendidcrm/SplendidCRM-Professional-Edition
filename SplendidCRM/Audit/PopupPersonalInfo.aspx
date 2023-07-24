<%@ Page language="c#" MasterPageFile="~/PopupView.Master" Codebehind="PopupPersonalInfo.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Audit.PopupPersonalInfo" %>
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
<asp:Content ID="cntBody" ContentPlaceHolderID="cntBody" runat="server">
	<p></p>
	<table width="100%" border="0" cellpadding="0" cellspacing="0" class="moduleTitle">
		<tr>
			<td valign="top"><SplendidCRM:DynamicImage ImageSkinID="<%# sModule %>" AlternateText='<%# L10n.Term(".moduleList." + sModule) %>' style="margin-top: 3px" Runat="server" />&nbsp;</td>
			<td width="100%"><h2><asp:Label ID="lblTitle" Runat="server" /></h2></td>
			<td valign="top" align="right" style="padding-top:3px; padding-left: 5px;" nowrap>
				<asp:ImageButton OnClientClick="print(); return false;" CssClass="utilsLink" AlternateText='<%# L10n.Term(".LNK_PRINT") %>' SkinID="print" Runat="server" />
				<asp:LinkButton  OnClientClick="print(); return false;" CssClass="utilsLink" Text='<%# L10n.Term(".LNK_PRINT") %>' Runat="server" />
			</td>
		</tr>
	</table>
	<p></p>

	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
	
	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdPopupView" EnableViewState="true" runat="server">
		<Columns>
			<asp:BoundColumn HeaderText="Audit.LBL_LIST_FIELD"        DataField="FIELD_NAME"   ItemStyle-Width="20%"  />
			<asp:BoundColumn HeaderText="Audit.LBL_LIST_VALUE"        DataField="VALUE"        ItemStyle-Width="45%" />
			<asp:BoundColumn HeaderText="Audit.LBL_LIST_LEAD_SOURCE"  DataField="LEAD_SOURCE"  ItemStyle-Width="20%"  />
			<asp:BoundColumn HeaderText="Audit.LBL_LIST_LAST_UPDATED" DataField="LAST_UPDATED" ItemStyle-Width="15%" ItemStyle-Wrap="false" />
		</Columns>
	</SplendidCRM:SplendidGrid>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</asp:Content>

