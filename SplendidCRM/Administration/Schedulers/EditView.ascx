<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditView.ascx.cs" Inherits="SplendidCRM.Administration.Schedulers.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true" EditView="true" Module="Schedulers" EnablePrint="false" HelpName="EditView" EnableHelp="true" Runat="Server" />

	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableHeaderCell ColumnSpan="4"><h4><asp:Label Text='<%# L10n.Term("Schedulers.LBL_BASIC_OPTIONS") %>' runat="server" /></h4></asp:TableHeaderCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_NAME"  ) %> <asp:Label Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' CssClass="required" runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox ID="NAME" size="35" MaxLength="255" Runat="server" />
							<asp:RequiredFieldValidator ID="NAME_REQUIRED" ControlToValidate="NAME" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableViewState="false" EnableClientScript="false" Enabled="false" runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_STATUS") %> <asp:Label Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' CssClass="required" runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><asp:DropDownList ID="STATUS" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell VerticalAlign="Top" CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_JOB") %> <asp:Label Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' CssClass="required" runat="server" /></asp:TableCell>
						<asp:TableCell VerticalAlign="Top" CssClass="dataField"><asp:DropDownList ID="JOB" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /></asp:TableCell>
						<asp:TableCell VerticalAlign="Top" CssClass="dataLabel">&nbsp;</asp:TableCell>
						<asp:TableCell VerticalAlign="Top" CssClass="dataField">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell VerticalAlign="Top" CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_INTERVAL") %> <asp:Label Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' CssClass="required" runat="server" /></asp:TableCell>
						<asp:TableCell>
							<asp:UpdatePanel UpdateMode="Conditional" runat="server">
								<ContentTemplate>
									<%@ Register TagPrefix="SplendidCRM" Tagname="CRON" Src="~/_controls/CRON.ascx" %>
									<SplendidCRM:CRON ID="ctlCRON" Runat="Server" />
								</ContentTemplate>
							</asp:UpdatePanel>
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DateTimePicker" Src="~/_controls/DateTimePicker.ascx" %>
	<%@ Register TagPrefix="SplendidCRM" Tagname="TimePicker" Src="~/_controls/TimePicker.ascx" %>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableHeaderCell ColumnSpan="4"><h4><asp:Label Text='<%# L10n.Term("Schedulers.LBL_ADV_OPTIONS") %>' runat="server" /></h4></asp:TableHeaderCell>
					</asp:TableRow>
					<asp:TableRow Visible="false">
						<asp:TableCell Width="15%" VerticalAlign="Top" CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_CATCH_UP") %></asp:TableCell>
						<asp:TableCell Width="35%" VerticalAlign="Top" CssClass="dataField"><asp:CheckBox ID="CATCH_UP" CssClass="checkbox" runat="server" /></asp:TableCell>
						<asp:TableCell Width="15%" VerticalAlign="Top" CssClass="dataLabel">&nbsp;</asp:TableCell>
						<asp:TableCell Width="35%" VerticalAlign="Top" CssClass="dataField">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell VerticalAlign="Top" CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_DATE_TIME_START") %></asp:TableCell>
						<asp:TableCell VerticalAlign="Top" CssClass="dataField"><SplendidCRM:DateTimePicker ID="DATE_TIME_START" MinutesIncrement="5" Runat="server" /></asp:TableCell>
						<asp:TableCell VerticalAlign="Top" CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_TIME_FROM"      ) %></asp:TableCell>
						<asp:TableCell VerticalAlign="Top" CssClass="dataField"><SplendidCRM:TimePicker ID="TIME_FROM" MinutesIncrement="5" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell VerticalAlign="Top" CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_DATE_TIME_END") %></asp:TableCell>
						<asp:TableCell VerticalAlign="Top" CssClass="dataField"><SplendidCRM:DateTimePicker ID="DATE_TIME_END" MinutesIncrement="5" Runat="server" /></asp:TableCell>
						<asp:TableCell VerticalAlign="Top" CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_TIME_TO"      ) %></asp:TableCell>
						<asp:TableCell VerticalAlign="Top" CssClass="dataField"><SplendidCRM:TimePicker ID="TIME_TO" MinutesIncrement="5" Runat="server" /></asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlFooterButtons" Visible="<%# !SplendidDynamic.StackedLayout(this.Page.Theme) && !PrintView %>" ShowRequired="false" Runat="Server" />
</div>

