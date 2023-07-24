<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ImportView.ascx.cs" Inherits="SplendidCRM.Surveys.ImportView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script type="text/javascript">
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
// 06/23/2010 Paul.  Automatically populate the Survey name based on the file name. 
function FileNameChanged(fld)
{
	var sNAME = fld.value;
	if ( sNAME.lastIndexOf('\\') >= 0 )
	{
		sNAME = sNAME.substring(sNAME.lastIndexOf('\\') + 1, sNAME.length);
	}
	else if ( sNAME.lastIndexOf('/') >= 0 )
	{
		sNAME = sNAME.substring(sNAME.lastIndexOf('/') + 1, sNAME.length);
	}
	// 10/30/2011 Paul.  Remove xml and rdl. 
	if ( Right(sNAME, 4).toLowerCase() == '.rdl' || Right(sNAME, 4).toLowerCase() == '.xml' )
	{
		sNAME = sNAME.substring(0, sNAME.length - 4);
	}
	var txtNAME = document.getElementById('<%= txtNAME.ClientID %>');
	if ( txtNAME != null )
		txtNAME.value = sNAME;
}
</script>
<div id="divListView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Surveys" Title=".moduleList.Home" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlDynamicButtons" Visible="<%# !PrintView %>" ShowRequired="true" Runat="Server" />

	<asp:Table Width="100%" CellPadding="0" CellSpacing="1" CssClass="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table ID="tblMain" Width="100%" CellSpacing="4" CellPadding="0" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="15%" VerticalAlign="Top" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Import.LBL_SELECT_FILE") %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" VerticalAlign="Top" CssClass="dataField">
							<input id="fileIMPORT" type="file" size="60" MaxLength="255" onchange="FileNameChanged(this)" runat="server" />
							&nbsp;
							<asp:RequiredFieldValidator ID="reqFILENAME" ControlToValidate="fileIMPORT" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" VerticalAlign="Top" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Surveys.LBL_NAME") %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" VerticalAlign="Top" CssClass="dataField">
							<asp:TextBox ID="txtNAME" TabIndex="2" size="35" MaxLength="150" Runat="server" />
							&nbsp;
							<asp:RequiredFieldValidator ID="reqNAME" ControlToValidate="txtNAME" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell VerticalAlign="Top" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Teams.LBL_TEAM") %>' Visible="<%# SplendidCRM.Crm.Config.enable_team_management() %>" runat="server" /></asp:TableCell>
						<asp:TableCell VerticalAlign="Top" CssClass="dataField">
							<asp:Panel Visible="<%# SplendidCRM.Crm.Config.enable_team_management() && !SplendidCRM.Crm.Config.enable_dynamic_teams() %>" runat="server">
								<asp:TextBox     ID="TEAM_NAME"     ReadOnly="True" Runat="server" />
								<asp:HiddenField ID="TEAM_ID"       runat="server" />&nbsp;
								<asp:Button      ID="btnChangeTeam" UseSubmitBehavior="false" OnClientClick=<%# "return ModulePopup('Teams', '" + TEAM_ID.ClientID + "', '" + TEAM_NAME.ClientID + "', null, false, null);" %> Text='<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>' CssClass="button" runat="server" />
							</asp:Panel>
							<%@ Register TagPrefix="SplendidCRM" Tagname="TeamSelect" Src="~/_controls/TeamSelect.ascx" %>
							<SplendidCRM:TeamSelect ID="ctlTeamSelect" Visible="<%# SplendidCRM.Crm.Config.enable_team_management() && SplendidCRM.Crm.Config.enable_dynamic_teams() %>" Runat="Server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" VerticalAlign="Top" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term(".LBL_ASSIGNED_USER_ID") %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" VerticalAlign="Top" CssClass="dataField">
							<asp:TextBox     ID="txtASSIGNED_TO" ReadOnly="True" Runat="server" />
							<asp:HiddenField ID="txtASSIGNED_USER_ID" runat="server" />&nbsp;
							<asp:Button      ID="btnChangeUser" UseSubmitBehavior="false" OnClientClick=<%# "return ModulePopup('Users', '" + txtASSIGNED_USER_ID.ClientID + "', '" + txtASSIGNED_TO.ClientID + "', null, false, null);" %> Text='<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>' CssClass="button" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>
