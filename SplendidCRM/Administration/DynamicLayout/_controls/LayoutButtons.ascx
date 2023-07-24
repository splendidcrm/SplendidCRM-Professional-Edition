<%@ Control Language="c#" AutoEventWireup="false" Codebehind="LayoutButtons.ascx.cs" Inherits="SplendidCRM.Administration.DynamicLayout._controls.LayoutButtons" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
// 07/14/2010 Paul.  We need to use InlineScript because this user control is inside an UpdatePanel 
// and the JavaScript needs to be registered when made visible. 
</script>
<SplendidCRM:InlineScript runat="server">
	<script type="text/javascript">
	function ExportSQL()
	{
			return window.open('export.aspx?NAME=<%= sVIEW_NAME %>','ExportSQL','width=1200,height=600,resizable=1,scrollbars=1');
	}
	</script>
</SplendidCRM:InlineScript>
<asp:Table Width="100%" CellPadding="0" CellSpacing="0" style="padding-bottom: 2px;" CssClass="button-panel" runat="server">
	<asp:TableRow>
		<asp:TableCell HorizontalAlign="Left">
			<asp:Button   ID="btnSave"       CommandName="Save"        OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# "  " + L10n.Term(".LBL_SAVE_BUTTON_LABEL"             ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_SAVE_BUTTON_TITLE"             ) %>' Runat="server" />
			<asp:Button   ID="btnCancel"     CommandName="Cancel"      OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# "  " + L10n.Term(".LBL_CANCEL_BUTTON_LABEL"           ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_CANCEL_BUTTON_TITLE"           ) %>' Runat="server" />
			<asp:Button   ID="btnNew"        CommandName="New"         OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# "  " + L10n.Term(".LBL_NEW_BUTTON_LABEL"              ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_NEW_BUTTON_TITLE"              ) %>' Runat="server" />
			<asp:Button   ID="btnCopyLayout" CommandName="Layout.Copy" OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# "  " + L10n.Term("DynamicLayout.LBL_COPY_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term("DynamicLayout.LBL_COPY_BUTTON_TITLE") %>' Runat="server" />
			<asp:TextBox  ID="txtCopyLayout" Visible="false" Width="200"                                          style="margin-right: 3px;" Runat="server" />
			<asp:Button   ID="btnDefaults"   CommandName="Defaults"    OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# "  " + L10n.Term(".LBL_DEFAULTS_BUTTON_LABEL"         ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_DEFAULTS_BUTTON_TITLE"         ) %>' Runat="server" />
			<asp:Button   ID="btnExport"     OnClientClick="ExportSQL(); return false;"         CssClass="button" style="margin-right: 3px;" Text='<%# "  " + L10n.Term(".LBL_EXPORT_BUTTON_LABEL"           ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_EXPORT_BUTTON_TITLE"           ) %>' UseSubmitBehavior="false" Runat="server" />
			<asp:Checkbox ID="chkPreview"    Text='<%# L10n.Term("DynamicLayout.LBL_PREVIEW") %>' OnCheckedChanged="chkPreview_CheckedChanged" CssClass="checkbox" AutoPostBack="true" runat="server" />
			<asp:HiddenField ID="hidPreviousPreview" runat="server" />
			<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
		</asp:TableCell>
		<asp:TableCell HorizontalAlign="Right" Wrap="false">
			<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" />
			&nbsp;
			<asp:Label Text='<%# L10n.Term(".NTC_REQUIRED") %>' runat="server" />
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>

