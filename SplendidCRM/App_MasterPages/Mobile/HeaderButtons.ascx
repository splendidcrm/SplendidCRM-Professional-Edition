<%@ Control Language="c#" AutoEventWireup="false" Codebehind="HeaderButtons.ascx.cs" Inherits="SplendidCRM.Themes.Sugar.HeaderButtons" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
<div id="divModuleHeader<%= sModule %>">
	<SplendidCRM:DynamicImage ID="ctlDynamicImage" Visible="false" ImageSkinID="<%# sModule %>" AlternateText='<%# L10n.Term(".moduleList." + sModule) %>' style="margin-top: 3px" Runat="server" />
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="ProcessButtons" Src="~/_controls/ProcessButtons.ascx" %>
<SplendidCRM:ProcessButtons ID="ctlProcessButtons" Runat="Server" />

<script type="text/javascript">
function ConfirmDelete()
{
	return confirm('<%= L10n.TermJavaScript(".NTC_DELETE_CONFIRMATION") %>');
}
</script>
<asp:Table SkinID="tabEditViewButtons" Visible="<%# !PrintView %>" runat="server">
	<asp:TableRow>
		<asp:TableCell ID="tdButtons" Width="10%" Wrap="false">
			<asp:Panel CssClass="button-panel" runat="server">
				<asp:PlaceHolder ID="pnlDynamicButtons" runat="server" />
				<asp:PlaceHolder ID="pnlProcessButtons" runat="server" />
			</asp:Panel>
		</asp:TableCell>
		<asp:TableCell ID="tdError">
			<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
		</asp:TableCell>
		<asp:TableCell ID="tdDynamicLinks" HorizontalAlign="Right" Wrap="false" Visible="false">
			<asp:Panel CssClass="button-panel" runat="server">
				<asp:PlaceHolder ID="pnlDynamicLinks" runat="server" />
			</asp:Panel>
		</asp:TableCell>
		<asp:TableCell ID="tdRequired" HorizontalAlign="Right" Wrap="false" Visible="false">
			<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" />
			&nbsp;
			<asp:Label Text='<%# L10n.Term(".NTC_REQUIRED") %>' Runat="server" />
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>

