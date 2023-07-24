<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ListHeader.ascx.cs" Inherits="SplendidCRM._controls.ListHeader" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
	<asp:Table SkinID="tabFrame" CssClass="h3Row" runat="server">
		<asp:TableRow>
			<asp:TableCell Wrap="false">
				<h3>
					<span Visible="<%# !Sql.IsEmptyString(SubPanel) %>" runat="server">
						<asp:HyperLink ID="lnkShowSubPanel" NavigateUrl=<%# "javascript:ShowSubPanel(\'" + lnkShowSubPanel.ClientID + "\',\'" + lnkHideSubPanel.ClientID + "\',\'" + SubPanel + "\');" %> style=<%# "display:" + (CookieValue(SubPanel) == "1" ? "inline" : "none") %> runat="server"><asp:Image SkinID="advanced_search" runat="server" /></asp:HyperLink>
						<asp:HyperLink ID="lnkHideSubPanel" NavigateUrl=<%# "javascript:HideSubPanel(\'" + lnkShowSubPanel.ClientID + "\',\'" + lnkHideSubPanel.ClientID + "\',\'" + SubPanel + "\');" %> style=<%# "display:" + (CookieValue(SubPanel) != "1" ? "inline" : "none") %> runat="server"><asp:Image SkinID="basic_search"    runat="server" /></asp:HyperLink>
					</span>
					<asp:Image SkinID="h3Arrow" Visible="<%# Sql.IsEmptyString(SubPanel) %>" Runat="server" />
					&nbsp;<asp:Label Text='<%# L10n.Term(Title) %>' runat="server" />
				</h3>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

