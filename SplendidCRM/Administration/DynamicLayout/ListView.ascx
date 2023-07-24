<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.FieldLayout.ListView" %>
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
<div id="divListView">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlModuleHeader" Module="Administration" Title="Administration.LBL_MANAGE_LAYOUT" EnableModuleLabel="false" EnablePrint="false" HelpName="index" EnableHelp="true" Runat="Server" />
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>

	<SplendidCRM:ListHeader Title="Administration.LBL_MANAGE_LAYOUT" Runat="Server" />
	<asp:Table Width="100%" CssClass="tabDetailView2" runat="server">
		<asp:TableRow>
			<asp:TableCell width="35%" CssClass="tabDetailViewDL2">
				<asp:Image AlternateText='<%# L10n.Term("Administration.LBL_LAYOUT_DETAILVIEW_TITLE") %>' SkinID="Administration" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_LAYOUT_DETAILVIEW_TITLE") %>' NavigateUrl="~/Administration/DynamicLayout/DetailViews/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_LAYOUT_DETAILVIEW") %></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell width="35%" CssClass="tabDetailViewDL2" Wrap="false">
				<asp:Image AlternateText='<%# L10n.Term("Administration.LBL_LAYOUT_EDITVIEW_TITLE") %>' SkinID="Administration" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_LAYOUT_EDITVIEW_TITLE") %>' NavigateUrl="~/Administration/DynamicLayout/EditViews/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_LAYOUT_EDITVIEW") %></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell  width="35%" CssClass="tabDetailViewDL2">
				<asp:Image AlternateText='<%# L10n.Term("Administration.LBL_LAYOUT_GRIDVIEW_TITLE") %>' SkinID="Administration" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_LAYOUT_GRIDVIEW_TITLE") %>' NavigateUrl="~/Administration/DynamicLayout/GridViews/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_LAYOUT_GRIDVIEW") %></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell  width="35%" CssClass="tabDetailViewDL2">
				<asp:Image AlternateText='<%# L10n.Term("Administration.LBL_LAYOUT_RELATIONSHIPS_TITLE") %>' SkinID="Administration" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_LAYOUT_RELATIONSHIPS_TITLE") %>' NavigateUrl="~/Administration/DynamicLayout/Relationships/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_LAYOUT_RELATIONSHIPS") %></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell  width="35%" CssClass="tabDetailViewDL2">
				<asp:Image AlternateText='<%# L10n.Term("Administration.LBL_LAYOUT_EDIT_RELATIONSHIPS_TITLE") %>' SkinID="Administration" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_LAYOUT_EDIT_RELATIONSHIPS_TITLE") %>' NavigateUrl="~/Administration/DynamicLayout/EditRelationships/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_LAYOUT_EDIT_RELATIONSHIPS") %></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>

