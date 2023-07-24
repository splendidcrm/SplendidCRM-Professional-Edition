<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TeamTree.ascx.cs" Inherits="SplendidCRM._controls.TeamTree" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
<asp:XmlDataSource ID="xdsMain" EnableCaching="false" runat="server" />

<div style="padding-top: 10px; padding-left: 10px;">
	<asp:Label ID="lblScope" CssClass="lastViewLink" Text='<%# L10n.Term("Teams.LBL_CURRENT") %>' runat="server" />&nbsp;
	<asp:Label ID="txtScope" CssClass="lastViewLink" Font-Bold="true" runat="server" />
</div>

<asp:TreeView ID="treeMain" OnSelectedNodeChanged="treeMain_SelectedNodeChanged" 
	CollapseImageUrl="~/App_Themes/Sugar2006/images/uparrow.gif" 
	ExpandImageUrl="~/App_Themes/Sugar2006/images/down_arrowRed.gif" 
	SelectedNodeStyle-Font-Overline="true" ShowLines="True" runat="server">
	<DataBindings>
		<asp:TreeNodeBinding DataMember="TEAM" ValueField="ID" TextField="NAME" />
	</DataBindings>
	<ParentNodeStyle   ChildNodesPadding="2px" HorizontalPadding="2px" VerticalPadding="2px" />
	<SelectedNodeStyle ChildNodesPadding="2px" Font-Italic="True" Font-Overline="False" Font-Bold="true" />
	<NodeStyle         ChildNodesPadding="2px" HorizontalPadding="2px" VerticalPadding="2px" CssClass="lastViewLink" />
	<LeafNodeStyle     ChildNodesPadding="2px" HorizontalPadding="2px" VerticalPadding="2px" CssClass="lastViewLink" />
</asp:TreeView>

