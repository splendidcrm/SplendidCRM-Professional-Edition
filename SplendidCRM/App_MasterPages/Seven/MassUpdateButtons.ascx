<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MassUpdateButtons.ascx.cs" Inherits="SplendidCRM.Themes.Seven.MassUpdateButtons" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
	<asp:Table SkinID="tabFrame" CssClass="MassUpdateHeaderFrame" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="99%" Wrap="false">
				<asp:Label Text='<%# L10n.Term(Title) %>' CssClass="MassUpdateHeaderName" runat="server" />
			</asp:TableCell>
			<asp:TableCell ID="tdButtons" Wrap="false">
				<asp:PlaceHolder ID="pnlDynamicButtons" runat="server" />
				<asp:ImageButton CssClass="MassUpdateHeaderClose" SkinID="subpanel_close" OnCommand="Page_Command" CommandName="ToggleMassUpdate" runat="server" />
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

