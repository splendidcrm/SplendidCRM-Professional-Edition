<%@ Control Language="c#" AutoEventWireup="false" Codebehind="PreviewView.ascx.cs" Inherits="SplendidCRM.Emails.PreviewView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divPreviewView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Emails" EnablePrint="true" HelpName="DetailView" EnableHelp="true" Runat="Server" />

	<table ID="tblMain" class="tabDetailView" runat="server">
	</table>
	<table ID="tblAttachments" class="tabDetailView" runat="server">
		<tr>
			<td width="15%" class="tabDetailViewDL" valign="top">
				<%= L10n.Term("Emails.LBL_ATTACHMENTS") %>
			</td>
			<td colspan="3" class="tabDetailViewDF" valign="top">
				<asp:Repeater id="ctlAttachments" runat="server">
					<HeaderTemplate />
					<ItemTemplate>
							<asp:HyperLink NavigateUrl='<%# "~/Notes/Attachment.aspx?ID=" + DataBinder.Eval(Container.DataItem, "NOTE_ATTACHMENT_ID") %>' Target="_blank" Runat="server" >
							<%# DataBinder.Eval(Container.DataItem, "FILENAME") %>
							</asp:HyperLink><br />
					</ItemTemplate>
					<FooterTemplate />
				</asp:Repeater>
			</td>
		</tr>
	</table>

	<script type="text/javascript">
	// 08/26/2010 Paul.  We need to count the visible search panels in JavaScript as we do not have an easy way to get the visible count in the code-behind. 
	var nUnifiedSearchVisibleCount = 0;
	</script>

	<br />
	<p></p>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlDynamicButtons" Visible="<%# !PrintView %>" ShowRequired="true" Runat="Server" />
	<div id="divDetailSubPanel">
		<asp:PlaceHolder ID="plcSubPanel" Runat="server" />
	</div>
	<asp:Label ID="lblNoResults" Text='<%# L10n.Term(".LBL_EMAIL_SEARCH_NO_RESULTS") %>' CssClass="error" style="display:none" Runat="server" />
	<script type="text/javascript">
	if ( nUnifiedSearchVisibleCount == 0 )
	{
		var lblNoResults = document.getElementById('<%# lblNoResults.ClientID %>');
		lblNoResults.style.display = 'inline';
	}
	</script>
	<p></p>
	<SplendidCRM:DynamicButtons ID="ctlFooterButtons" Visible="<%# !PrintView %>" ShowRequired="false" Runat="Server" />
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />

