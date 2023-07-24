<%@ Control Language="c#" AutoEventWireup="false" Codebehind="SubPanelButtons.ascx.cs" Inherits="SplendidCRM.Themes.Seven.SubPanelButtons" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
	<asp:Table ID="tblSubPanelFrame" SkinID="tabFrame" CssClass=<%# (CookieValue(SubPanel) == "1" ? "h3Row h3RowDisabled" : "h3Row") %> runat="server">
		<asp:TableRow>
			<asp:TableCell VerticalAlign="Top">
				<span class="ModuleHeaderModule ModuleHeaderModule<%# sModule %> ListHeaderModule"><%# L10n.Term(sModule + ".LBL_MODULE_ABBREVIATION") %></span>
			</asp:TableCell>
			<asp:TableCell Width="99%" Wrap="false">
				<asp:Label Text='<%# L10n.Term(Title) %>' CssClass="ListHeaderName" runat="server" />
			</asp:TableCell>
			<asp:TableCell>
				<span Visible="<%# !Sql.IsEmptyString(SubPanel) %>" runat="server">
					<asp:HyperLink ID="lnkShowSubPanel" NavigateUrl=<%# "javascript:ShowSubPanel(\'" + lnkShowSubPanel.ClientID + "\',\'" + lnkHideSubPanel.ClientID + "\',\'" + SubPanel + "\',\'" + tblSubPanelFrame.ClientID + "\');" %> style=<%# "display:" + (CookieValue(SubPanel) == "1" ? "inline" : "none") %> runat="server"><asp:Image SkinID="subpanel_expand"   runat="server" /></asp:HyperLink>
					<asp:HyperLink ID="lnkHideSubPanel" NavigateUrl=<%# "javascript:HideSubPanel(\'" + lnkShowSubPanel.ClientID + "\',\'" + lnkHideSubPanel.ClientID + "\',\'" + SubPanel + "\',\'" + tblSubPanelFrame.ClientID + "\');" %> style=<%# "display:" + (CookieValue(SubPanel) != "1" ? "inline" : "none") %> runat="server"><asp:Image SkinID="subpanel_collapse" runat="server" /></asp:HyperLink>
				</span>
			</asp:TableCell>
			<asp:TableCell ID="tdButtons" Wrap="false" onclick=<%# "javascript:ShowSubPanel(\'" + lnkShowSubPanel.ClientID + "\',\'" + lnkHideSubPanel.ClientID + "\',\'" + SubPanel + "\',\'" + tblSubPanelFrame.ClientID + "\');" %>>
				<asp:PlaceHolder ID="pnlDynamicButtons" runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>


<script type="text/javascript">
function ConfirmDelete()
{
	return confirm('<%= L10n.TermJavaScript(".NTC_DELETE_CONFIRMATION") %>');
}
</script>
<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />

<asp:Panel ID="phButtonHover" CssClass="PanelHoverHidden ListHeaderOtherPanel" runat="server" />
<ajaxToolkit:HoverMenuExtender ID="hexHoverMenuExtender" TargetControlID="tdButtons" PopupControlID="phButtonHover" PopupPosition="Right" OffsetY="30" OffsetX="-151" PopDelay="250" HoverDelay="500" runat="server" />

