<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditView.ascx.cs" Inherits="SplendidCRM.Administration.DynamicLayout.EditViews.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<SplendidCRM:InlineScript runat="server">
<script type="text/javascript">
var sDynamicLayoutModule = '<%= ViewState["MODULE_NAME"] %>';
function NewEventPopup()
{
	return ModulePopup('BusinessRules', '<%= new SplendidCRM.DynamicControl(this, "NEW_EVENT_ID").ClientID %>', '<%= new SplendidCRM.DynamicControl(this, "NEW_EVENT_NAME").ClientID %>', 'Module=' + sDynamicLayoutModule, false, null);
}
function PreLoadEventPopup()
{
	return ModulePopup('BusinessRules', '<%= new SplendidCRM.DynamicControl(this, "PRE_LOAD_EVENT_ID").ClientID %>', '<%= new SplendidCRM.DynamicControl(this, "PRE_LOAD_EVENT_NAME").ClientID %>', 'Module=' + sDynamicLayoutModule, false, null);
}
function PostLoadEventPopup()
{
	return ModulePopup('BusinessRules', '<%= new SplendidCRM.DynamicControl(this, "POST_LOAD_EVENT_ID").ClientID %>', '<%= new SplendidCRM.DynamicControl(this, "POST_LOAD_EVENT_NAME").ClientID %>', 'Module=' + sDynamicLayoutModule, false, null);
}
function ValidationEventPopup()
{
	return ModulePopup('BusinessRules', '<%= new SplendidCRM.DynamicControl(this, "VALIDATION_EVENT_ID").ClientID %>', '<%= new SplendidCRM.DynamicControl(this, "VALIDATION_EVENT_NAME").ClientID %>', 'Module=' + sDynamicLayoutModule, false, null);
}
function PreSaveEventPopup()
{
	return ModulePopup('BusinessRules', '<%= new SplendidCRM.DynamicControl(this, "PRE_SAVE_EVENT_ID").ClientID %>', '<%= new SplendidCRM.DynamicControl(this, "PRE_SAVE_EVENT_NAME").ClientID %>', 'Module=' + sDynamicLayoutModule, false, null);
}
function PostSaveEventPopup()
{
	return ModulePopup('BusinessRules', '<%= new SplendidCRM.DynamicControl(this, "POST_SAVE_EVENT_ID").ClientID %>', '<%= new SplendidCRM.DynamicControl(this, "POST_SAVE_EVENT_NAME").ClientID %>', 'Module=' + sDynamicLayoutModule, false, null);
}
function LayoutDragOver(event, nDropIndex)
{
	// 08/08/2013 Paul.  IE does not support preventDefault. 
	// http://stackoverflow.com/questions/1000597/event-preventdefault-function-not-working-in-ie
	event.preventDefault ? event.preventDefault() : event.returnValue = false;
}
function LayoutDropIndex(event, nDropIndex)
{
	// 08/08/2013 Paul.  IE does not support preventDefault. 
	event.preventDefault ? event.preventDefault() : event.returnValue = false;
	var hidDragStartIndex = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "hidDragStartIndex").ClientID %>');
	var hidDragEndIndex   = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "hidDragEndIndex"  ).ClientID %>');
	var btnDragComplete   = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "btnDragComplete"   ).ClientID %>');
	hidDragStartIndex.value = event.dataTransfer.getData('Text');
	hidDragEndIndex.value   = nDropIndex;
	if ( hidDragStartIndex.value != hidDragEndIndex.value )
	{
		btnDragComplete.click();
	}
}
</script>
</SplendidCRM:InlineScript>
<div id="divEditView" runat="server">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" Module="Administration" Title="DynamicLayout.LBL_EDIT_VIEW_LAYOUT" EnablePrint="false" HelpName="EditView" EnableHelp="true" Runat="Server" />

	<asp:HiddenField ID="hidDragStartIndex" runat="server" />
	<asp:HiddenField ID="hidDragEndIndex"   runat="server" />
	<asp:Button      ID="btnDragComplete" CommandName="Layout.DragIndex" OnCommand="Page_Command" style="display:none" runat="server" />

	<asp:Table Width="100%" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="200px" VerticalAlign="Top">
				<%@ Register TagPrefix="SplendidCRM" Tagname="SearchBasic" Src="../_controls/SearchBasic.ascx" %>
				<SplendidCRM:SearchBasic ID="ctlSearch" ViewTableName="vwEDITVIEWS_Layout" ViewFieldName="EDIT_NAME" Runat="Server" />
			</asp:TableCell>
			<asp:TableCell VerticalAlign="Top">
				<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
				<SplendidCRM:ListHeader ID="ctlListHeader" Runat="Server" />
				
				<%@ Register TagPrefix="SplendidCRM" Tagname="LayoutButtons" Src="../_controls/LayoutButtons.ascx" %>
				<SplendidCRM:LayoutButtons ID="ctlLayoutButtons" Visible="<%# !PrintView %>" Runat="Server" />

				<asp:Table ID="tblViewEventsPanel" Width="100%" CellPadding="0" CellSpacing="0" CssClass="" runat="server">
					<asp:TableRow>
						<asp:TableCell>
							<table ID="tblViewEvents" class="tabEditView" runat="server">
							</table>
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>

				<input type="hidden" id="txtFieldState" runat="server" />
				<asp:Panel ID="pnlDynamicMain" runat="server">
					<asp:Table ID="tblForm" Width="100%" CellPadding="0" CellSpacing="0" CssClass="" runat="server">
						<asp:TableRow>
							<asp:TableCell>
								<table ID="tblMain" class="tabEditView" runat="server">
								</table>
							</asp:TableCell>
						</asp:TableRow>
					</asp:Table>
				</asp:Panel>
				
				<br />
				<%@ Register TagPrefix="SplendidCRM" Tagname="NewRecord" Src="~/Administration/DynamicLayout/EditViews/NewRecord.ascx" %>
				<SplendidCRM:NewRecord ID="ctlNewRecord" Visible="false" Runat="Server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>

<%-- 03/21/2016 Paul.  Resize layout. --%>
<script type="text/javascript">
function AdminLayoutResize()
{
	try
	{
		var divDynamicLayoutSearchBasic  = document.getElementById('divDynamicLayoutSearchBasic');
		rect = divDynamicLayoutSearchBasic.getBoundingClientRect();
		nHeight = $(window).height() - rect.top;
		nHeight -= 42;
		divDynamicLayoutSearchBasic.style.height = nHeight.toString() + 'px';
	}
	catch(e)
	{
		alert(e.message);
	}
}

window.onload = function()
{
	AdminLayoutResize();
	$(window).resize(AdminLayoutResize);
}
// 03/21/2016 Paul.  Also resize after ajax update. 
Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function()
{
	AdminLayoutResize();
});
</script>

