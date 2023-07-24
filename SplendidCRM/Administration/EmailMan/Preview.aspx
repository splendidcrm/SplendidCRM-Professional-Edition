<%@ Page language="c#" MasterPageFile="~/PopupView.Master" Codebehind="Preview.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Administration.EmailMan.Preview" %>
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
<asp:Content ID="cntBody" ContentPlaceHolderID="cntBody" runat="server">
	<script type="text/javascript">
	function UpdateParent()
	{
		if ( window.opener != null )
		{
			window.opener.Refresh();
			window.close();
		}
		else
		{
			window.close();
		}
	}
	</script>

	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true"  Module="EmailMan" EnableModuleLabel="false" EnablePrint="true" EnableHelp="false" Runat="Server" />

	<asp:Table Width="100%" BorderWidth="0" CellSpacing="0" CellPadding="0" CssClass="tabDetailView" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="15%" CssClass="tabDetailViewDL" VerticalAlign="top"><asp:Label Text='<%# L10n.Term("Emails.LBL_DATE_SENT") %>' runat="server" /></asp:TableCell>
			<asp:TableCell Width="85%" CssClass="tabDetailViewDF" VerticalAlign="top"><asp:Label ID="txtSEND_DATE_TIME" runat="server" /></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL" VerticalAlign="top"><asp:Label Text='<%# L10n.Term("Emails.LBL_FROM") %>' runat="server" /></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF" VerticalAlign="top"><asp:Label ID="txtFROM" runat="server" /></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL" VerticalAlign="top"><asp:Label Text='<%# L10n.Term("Emails.LBL_TO") %>' runat="server" /></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF" VerticalAlign="top"><asp:Label ID="txtTO" runat="server" /></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL" VerticalAlign="top"><asp:Label Text='<%# L10n.Term("Emails.LBL_SUBJECT") %>' runat="server" /></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF" VerticalAlign="top"><asp:Label ID="txtSUBJECT" runat="server" /></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL">&nbsp;</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF">&nbsp;</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL" VerticalAlign="top"><asp:Label Text='<%# L10n.Term("Emails.LBL_BODY") %>' runat="server" /></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF" VerticalAlign="top"><asp:Label ID="txtBODY_HTML" runat="server" /></asp:TableCell>
		</asp:TableRow>
	</asp:Table>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</asp:Content>

