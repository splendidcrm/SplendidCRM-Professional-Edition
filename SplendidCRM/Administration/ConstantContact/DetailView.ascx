<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DetailView.ascx.cs" Inherits="SplendidCRM.Administration.ConstantContact.DetailView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divEditView" runat="server">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" Module="ConstantContact" Title="ConstantContact.LBL_CONSTANTCONTACT_SETTINGS" EnableModuleLabel="false" EnablePrint="false" EnableHelp="true" Runat="Server" />
	
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabSearchView" runat="server">
					<asp:TableRow ID="trInstructions">
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top" ColumnSpan="4">
							<asp:Label Text='<%# L10n.Term("ConstantContact.LBL_INSTRUCTIONS") %>' runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("ConstantContact.LBL_CONSTANTCONTACT_ENABLED") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataLabel" VerticalAlign="top">
							<asp:CheckBox ID="ENABLED" CssClass="checkbox" Enabled="false" runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("ConstantContact.LBL_VERBOSE_STATUS") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataLabel" VerticalAlign="top">
							<asp:CheckBox ID="VERBOSE_STATUS" CssClass="checkbox" Enabled="false" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("ConstantContact.LBL_DIRECTION") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label ID="DIRECTION" runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("ConstantContact.LBL_CONFLICT_RESOLUTION") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label ID="CONFLICT_RESOLUTION" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("ConstantContact.LBL_SYNC_MODULES") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label ID="SYNC_MODULES" runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataLabel" VerticalAlign="top">
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

	<div id="divDetailSubPanel">
		<asp:PlaceHolder ID="plcSubPanel" Runat="server" />
	</div>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>
