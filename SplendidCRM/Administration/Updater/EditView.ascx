<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditView.ascx.cs" Inherits="SplendidCRM.Administration.Updater.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true" EditView="true" Module="Administration" Title="Administration.LBL_CONFIGURE_UPDATER" EnablePrint="false" EnableHelp="true" Runat="Server" />
	
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableHeaderCell CssClass="dataLabel"><h4><asp:Label Text='<%# L10n.Term("Administration.LBL_SPLENDIDCRM_UPDATE_TITLE") %>' runat="server" /></h4></asp:TableHeaderCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell>
							<asp:Table CellPadding="4" runat="server">
								<asp:TableRow>
									<asp:TableCell Width="1%" CssClass="dataField" VerticalAlign="Top">
										<asp:CheckBox ID="SEND_USAGE_INFO" CssClass="checkbox" Runat="server" />
									</asp:TableCell>
									<asp:TableCell Width="99%" CssClass="dataField">
										<asp:Label Text='<%# L10n.Term("Administration.LBL_SEND_STAT") %>' runat="server" />
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell>
							<asp:Table CellPadding="4" runat="server">
								<asp:TableRow>
									<asp:TableCell Width="1%" CssClass="dataField" VerticalAlign="Top">
										<asp:CheckBox ID="CHECK_UPDATES" CssClass="checkbox" Runat="server" />
									</asp:TableCell>
									<asp:TableCell Width="99%" CssClass="dataField">
										<asp:Label Text='<%# L10n.Term("Administration.LBL_UPDATE_CHECK_TYPE") %>' runat="server" />
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell>
							<asp:Table CellPadding="4" runat="server">
								<asp:TableRow>
									<asp:TableCell CssClass="dataField">
										<asp:Button OnCommand="Page_Command" CommandName="CheckNow" Text='<%# "  " + L10n.Term("Administration.LBL_CHECK_NOW_LABEL") + "  " %>' ToolTip='<%# L10n.Term("Administration.LBL_CHECK_NOW_TITLE") %>' CssClass="buttonOn" runat="server" />
										&nbsp;
										<b>Version <%= Application["SplendidVersion"] %></b>
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataField">
							<asp:Label ID="NO_UPDATES" Text='<%# L10n.Term("Administration.LBL_UPTODATE") %>' Visible="false" runat="server" />

							<SplendidCRM:SplendidGrid id="grdMain" AllowPaging="false" AllowSorting="false" EnableViewState="true" runat="server">
								<Columns>
									<asp:BoundColumn     HeaderText="Build"       DataField="Build"       ItemStyle-Width="15%" ItemStyle-Wrap="false" />
									<asp:BoundColumn     HeaderText="Date"        DataField="Date"        ItemStyle-Width="15%" ItemStyle-Wrap="false" />
									<asp:BoundColumn     HeaderText="Description" DataField="Description" ItemStyle-Width="60%" ItemStyle-Wrap="true" />
									<asp:TemplateColumn  HeaderText="" ItemStyle-Width="1%">
										<ItemTemplate>
											<asp:HyperLink  NavigateUrl='<%# Eval("URL") %>' SkinID="Backup" runat="server" />
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
							</SplendidCRM:SplendidGrid>
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlFooterButtons" Visible="<%# !SplendidDynamic.StackedLayout(this.Page.Theme) && !PrintView %>" ShowRequired="false" Runat="Server" />
</div>

