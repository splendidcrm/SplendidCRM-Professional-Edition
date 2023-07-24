<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditView.ascx.cs" Inherits="SplendidCRM.ReportDesigner.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="SplendidCRM" Tagname="DatePicker" Src="~/_controls/DatePicker.ascx" %>
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
<div id="divEditView" runat="server">
<SplendidCRM:InlineScript runat="server">
<script type="text/javascript">
// 09/03/2012 Paul.  Must place within InlineScript to prevent error. The Controls collection cannot be modified because the control contains code blocks (i.e. ).
var sDynamicLayoutModule = '<%= ctlQueryBuilder.MODULE %>';
function PreLoadEventPopup()
{
	return ModulePopup('ReportRules', '<%= new SplendidCRM.DynamicControl(this, "PRE_LOAD_EVENT_ID").ClientID %>', '<%= new SplendidCRM.DynamicControl(this, "PRE_LOAD_EVENT_NAME").ClientID %>', 'Module=' + sDynamicLayoutModule, false, null);
}
function PostLoadEventPopup()
{
	return ModulePopup('ReportRules', '<%= new SplendidCRM.DynamicControl(this, "POST_LOAD_EVENT_ID").ClientID %>', '<%= new SplendidCRM.DynamicControl(this, "POST_LOAD_EVENT_NAME").ClientID %>', 'Module=' + sDynamicLayoutModule, false, null);
}
function toggleDisplay(sID)
{
	var fld = document.getElementById(sID);
	if ( fld != undefined )
		fld.style.display = (fld.style.display == 'none') ? 'inline' : 'none';
}
</script>
</SplendidCRM:InlineScript>
<asp:UpdatePanel UpdateMode="Conditional" runat="server">
	<ContentTemplate>
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%-- 03/16/2016 Paul.  HeaderButtons must be inside UpdatePanel in order to display errors. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true" EditView="true" Module="Reports" EnablePrint="false" HelpName="EditView" EnableHelp="true" Runat="Server" />

	<%-- 03/27/2020 Paul.  Convert to dynamic layout to support React Client. --%>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<table ID="tblMain" class="tabEditView" runat="server">
				</table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

	<%@ Register TagPrefix="SplendidCRM" Tagname="QueryDesigner" Src="QueryDesigner.ascx" %>
	<SplendidCRM:QueryDesigner ID="ctlQueryBuilder" UseSQLParameters="true" ShowRelated="true" Runat="Server" />
	
	<div id="divSHOW_QUERY" style="display: <%= new SplendidCRM.DynamicControl(this, "SHOW_QUERY").Checked ? "inline" : "none" %>">
		<table id="tblReportDesignerSQL" border="1" cellpadding="3" cellspacing="0" width="100%" bgcolor="LightGrey">
			<tr>
				<td>
					<pre id="divReportDesignerSQL"></pre>
				</td>
			</tr>
		</table>
		<table id="tblReportDesignerJSON" border="1" cellpadding="3" cellspacing="0" width="100%" bgcolor="LightGrey" style="display: inline">
			<tr>
				<td>
					<div id="divReportDesignerJSON"></div>
				</td>
			</tr>
		</table>
	</div>
	<asp:Literal ID="litREPORT_QUERY" EnableViewState="false" Visible="false" runat="server" />
	<asp:Literal ID="litREPORT_RDL"   EnableViewState="false" Visible="false" runat="server" />
	
	<table cellpadding="0" cellspacing="0" width="90%" style="margin-top: 4px; margin-bottom: 4px">
		<tr>
			<td Width="90%">
				<%@ Register TagPrefix="SplendidCRM" Tagname="ParameterView" Src="~/Reports/ParameterView.ascx" %>
				<SplendidCRM:ParameterView ID="ctlParameterView" Runat="Server" />
			</td>
			<td valign="bottom" style="padding-left: 4px;">
				<asp:Button ID="btnSubmitParameters" Text='<%# L10n.Term(".LBL_SUBMIT_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_SUBMIT_BUTTON_TITLE") %>' OnCommand="Page_Command" CommandName="Submit" runat="server" />
			</td>
		</tr>
	</table>
	<asp:HiddenField ID="hidPARAMETERS_EDITVIEW" runat="server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="ReportView" Src="~/Reports/ReportView.ascx" %>
	<SplendidCRM:ReportView ID="ctlReportView" Runat="Server" />
	
	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />

	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%-- 03/16/2016 Paul.  HeaderButtons must be inside UpdatePanel in order to display errors. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlFooterButtons" Visible="<%# !SplendidDynamic.StackedLayout(this.Page.Theme) && !PrintView %>" ShowRequired="false" Runat="Server" />
	</ContentTemplate>
</asp:UpdatePanel>
</div>


