<%@ Control Language="c#" AutoEventWireup="false" Codebehind="PostView.ascx.cs" Inherits="SplendidCRM.Threads.PostView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divPostView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlDynamicButtons" Runat="Server" />

	<asp:HiddenField ID="txtPOST_ID" runat="server" />
	<asp:Table ID="tblMain" SkinID="tabDetailView" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="80%" VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="txtTITLE" Font-Bold="true" runat="server" /></asp:TableCell>
			<asp:TableCell Width="10%" VerticalAlign="Top" CssClass="tabDetailViewDL" Wrap="false"><asp:Label ID="txtCREATED_BY" runat="server" />:</asp:TableCell>
			<asp:TableCell Width="10%" VerticalAlign="Top" CssClass="tabDetailViewDL" Wrap="false"><asp:Label ID="txtDATE_ENTERED" runat="server" /></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell ColumnSpan="3" style="background-color: #ffffff; padding-left: 3mm; padding-right: 3mm;">
				<asp:Literal ID="txtDESCRIPTION" runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow id="trModified" Visible="false" runat="server">
			<asp:TableCell Width="80%" VerticalAlign="Top" CssClass="tabDetailViewDF">&nbsp;</asp:TableCell>
			<asp:TableCell Width="10%" VerticalAlign="Top" CssClass="tabDetailViewDL" Wrap="false"><%# L10n.Term(".LBL_MODIFIED_BY") %>&nbsp;<asp:Label ID="txtMODIFIED_BY" runat="server" />:</asp:TableCell>
			<asp:TableCell Width="10%" VerticalAlign="Top" CssClass="tabDetailViewDL" Wrap="false"><asp:Label ID="txtDATE_MODIFIED" runat="server" /></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<br />
</div>
