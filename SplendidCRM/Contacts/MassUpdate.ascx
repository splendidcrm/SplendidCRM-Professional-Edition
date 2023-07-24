<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MassUpdate.ascx.cs" Inherits="SplendidCRM.Contacts.MassUpdate" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<script type="text/javascript">
function MassUpdateContactPopup()
{
	return ModulePopup('Contacts', '<%= txtREPORTS_TO_ID.ClientID %>', '<%= txtREPORTS_TO_NAME.ClientID %>', null, false, null);
}
function MassUpdateAccountPopup()
{
	return ModulePopup('Accounts', '<%= txtACCOUNT_ID.ClientID %>', '<%= txtACCOUNT_NAME.ClientID %>', null, false, null);
}
</script>
<%-- 06/06/2015 Paul.  MassUpdateButtons combines ListHeader and DynamicButtons. --%>
<%@ Register TagPrefix="SplendidCRM" Tagname="MassUpdateButtons" Src="~/_controls/MassUpdateButtons.ascx" %>
<SplendidCRM:MassUpdateButtons ID="ctlDynamicButtons" SubPanel="divContactsMassUpdate" Title=".LBL_MASS_UPDATE_TITLE" Runat="Server" />

<%-- 03/26/2018 Paul.  Hide mass update fields in ArchiveView. --%>
<div id="divContactsMassUpdate" style='<%= "display:" + (CookieValue("divContactsMassUpdate") != "1" && !ArchiveView() ? "inline" : "none") %>'>
	<asp:Table Width="100%" CellPadding="0" CellSpacing="0" CssClass="tabForm tabMassUpdate" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<%@ Register TagPrefix="SplendidCRM" Tagname="TeamAssignedMassUpdate" Src="~/_controls/TeamAssignedMassUpdate.ascx" %>
				<SplendidCRM:TeamAssignedMassUpdate ID="ctlTeamAssignedMassUpdate" Runat="Server" />
				<asp:Table Width="100%" CellPadding="0" CellSpacing="0" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Contacts.LBL_LEAD_SOURCE") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><asp:DropDownList ID="lstLEAD_SOURCE" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel">
							<asp:Label Text='<%# L10n.Term("Contacts.LBL_ACCOUNT_NAME") %>' Visible='<%# Sql.ToString(Application["CONFIG.BusinessMode"]) != "B2C" %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox     ID="txtACCOUNT_NAME" ReadOnly="True" Visible='<%# Sql.ToString(Application["CONFIG.BusinessMode"]) != "B2C" %>' Runat="server" />
							<asp:HiddenField ID="txtACCOUNT_ID" runat="server" />&nbsp;
							<asp:Button      ID="btnACCOUNT_ID" UseSubmitBehavior="false" OnClientClick="return MassUpdateAccountPopup();" Text='<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>' CssClass="button" Visible='<%# Sql.ToString(Application["CONFIG.BusinessMode"]) != "B2C" %>' runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term(".LBL_TAG_SET_NAME") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<%@ Register TagPrefix="SplendidCRM" Tagname="TagMassUpdate" Src="~/_controls/TagMassUpdate.ascx" %>
							<SplendidCRM:TagMassUpdate ID="ctlTagMassUpdate" Runat="Server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Contacts.LBL_REPORTS_TO") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox     ID="txtREPORTS_TO_NAME" ReadOnly="True" Runat="server" />
							<asp:HiddenField ID="txtREPORTS_TO_ID" runat="server" />&nbsp;
							<asp:Button      ID="btnREPORTS_TO_ID" UseSubmitBehavior="false" OnClientClick="return MassUpdateContactPopup();" Text='<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>' CssClass="button" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>

