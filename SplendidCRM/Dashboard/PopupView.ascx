<%@ Control CodeBehind="PopupView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Dashboard.PopupView" %>
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
<div id="divPopupView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchView" Src="~/_controls/SearchView.ascx" %>
	<SplendidCRM:SearchView ID="ctlSearchView" Module="Dashboard" IsPopupSearch="true" ShowSearchTabs="false" Visible="<%# !PrintView %>" Runat="Server" />

	<script type="text/javascript">
	function SelectDashboard(sDASHBOARD_ID, sCATEGORY)
	{
		if ( window.opener != null && window.opener.ChangeDashboard != null )
		{
			var sREPORT_ID = '<%# Request["REPORT_ID"] %>';
			window.opener.ChangeDashboard(sREPORT_ID, sDASHBOARD_ID, sCATEGORY);
			window.close();
		}
		else
		{
			alert('Original window has closed.  Dashboard cannot be assigned.' + '\n' + sDASHBOARD_ID + '\n' + sCATEGORY);
		}
		return false;
	}
	function Cancel()
	{
		window.close();
	}
	</script>

	<asp:Label ID="lblError" CssClass="error" EnableViewState="false" runat="server" />

	<asp:Panel ID="pnlMain" runat="server">
		<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
		<SplendidCRM:ListHeader Title="Dashboard.LBL_HOME_PAGE_DASHBOARDS" Runat="Server" />

		<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdPopupView" EnableViewState="true" runat="server">
			<Columns>
				<asp:TemplateColumn HeaderText="Dashboard.LBL_LIST_NAME" SortExpression="NAME" ItemStyle-Width="80%" ItemStyle-HorizontalAlign="Left">
					<ItemTemplate>
						<asp:HyperLink Text='<%# Eval("NAME") %>' NavigateUrl='<%# "javascript:SelectDashboard(null, \"" + Eval("NAME") + "\")" %>' Runat="server" />
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:BoundColumn HeaderText="Dashboard.LBL_LIST_CATEGORY" DataField="CATEGORY" ItemStyle-Width="80%" ItemStyle-HorizontalAlign="Left" />
			</Columns>
		</SplendidCRM:SplendidGrid>
	</asp:Panel>

	<asp:Panel ID="pnlHome" runat="server">
		<SplendidCRM:ListHeader Title="Dashboard.LBL_HOME_PAGE_DASHBOARDS" Runat="Server" />
		<asp:Button Text='<%# L10n.Term("Dashboard.LBL_CREATE_NEW_DASHBOARD") %>' OnClientClick='<%# "SelectDashboard(null, \"Home\")" %>' CssClass="button" style="margin-bottom: 4px;" runat="server" />

		<SplendidCRM:SplendidGrid id="grdHome" SkinID="grdPopupView" EnableViewState="true" runat="server">
			<Columns>
				<asp:TemplateColumn HeaderText="Dashboard.LBL_LIST_NAME" SortExpression="NAME" ItemStyle-Width="80%" ItemStyle-HorizontalAlign="Left">
					<ItemTemplate>
						<asp:HyperLink Text='<%# Eval("NAME") %>' NavigateUrl='<%# "javascript:SelectDashboard(\"" + Eval("ID") + "\", \"" + Eval("CATEGORY") + "\")" %>' Runat="server" />
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:BoundColumn HeaderText="Dashboard.LBL_LIST_CATEGORY" DataField="CATEGORY" ItemStyle-Width="80%" ItemStyle-HorizontalAlign="Left" />
			</Columns>
		</SplendidCRM:SplendidGrid>
	</asp:Panel>

	<asp:Panel ID="pnlDashboard" runat="server">
		<SplendidCRM:ListHeader Title="Dashboard.LBL_DASHBOARDS" Runat="Server" />
		<asp:Button Text='<%# L10n.Term("Dashboard.LBL_CREATE_NEW_DASHBOARD") %>' OnClientClick='<%# "SelectDashboard(null, \"Dashboard\")" %>' CssClass="button" style="margin-bottom: 4px;" runat="server" />

		<SplendidCRM:SplendidGrid id="grdDashboard" SkinID="grdPopupView" EnableViewState="true" runat="server">
			<Columns>
				<asp:TemplateColumn HeaderText="Dashboard.LBL_LIST_NAME" SortExpression="NAME" ItemStyle-Width="80%" ItemStyle-HorizontalAlign="Left">
					<ItemTemplate>
						<asp:HyperLink Text='<%# Eval("NAME") %>' NavigateUrl='<%# "javascript:SelectDashboard(\"" + Eval("ID") + "\", \"" + Eval("CATEGORY") + "\")" %>' Runat="server" />
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:BoundColumn HeaderText="Dashboard.LBL_LIST_CATEGORY" DataField="CATEGORY" ItemStyle-Width="80%" ItemStyle-HorizontalAlign="Left" />
			</Columns>
		</SplendidCRM:SplendidGrid>
	</asp:Panel>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>

