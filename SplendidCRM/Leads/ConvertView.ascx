<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ConvertView.ascx.cs" Inherits="SplendidCRM.Leads.ConvertView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true" EditView="true" Module="Leads" EnablePrint="false" HelpName="ConvertView" EnableHelp="true" Runat="Server" />
	<p></p>

	<asp:HiddenField ID="LAYOUT_EDIT_VIEW" Runat="server" />
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<table ID="tblMain" class="tabEditView" runat="server">
					<tr>
						<th colspan="4"><h4><%= L10n.Term("Leads.LNK_NEW_CONTACT") %></h4></th>
					</tr>
				</table>
				<div id="divCreateContactNoteLink">
					&nbsp;<asp:CheckBox ID="chkCreateNote" CssClass="checkbox" Runat="server" />
					&nbsp;<%= L10n.Term("Leads.LNK_NEW_NOTE") %>
				</div>
				<div id="divCreateContactNote" style="display:<%= chkCreateNote.Checked ? "inline" : "none" %>">
					<p></p>
					<%@ Register TagPrefix="SplendidCRM" Tagname="ConvertViewNote" Src="ConvertViewNote.ascx" %>
					<SplendidCRM:ConvertViewNote ID="ctlConvertViewNote" Runat="Server" />
				</div>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	
	<p></p>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableHeaderCell CssClass="dataLabel">
							<h4 CssClass="dataLabel"><%= L10n.Term(".LBL_RELATED_RECORDS") %></h4>
						</asp:TableHeaderCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell VerticalAlign="top">
							<%@ Register TagPrefix="SplendidCRM" Tagname="ConvertViewAccount" Src="ConvertViewAccount.ascx" %>
							<SplendidCRM:ConvertViewAccount ID="ctlConvertViewAccount" Runat="Server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell VerticalAlign="top">
							<%@ Register TagPrefix="SplendidCRM" Tagname="ConvertViewOpportunity" Src="ConvertViewOpportunity.ascx" %>
							<SplendidCRM:ConvertViewOpportunity ID="ctlConvertViewOpportunity" Runat="Server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell VerticalAlign="top">
							<%@ Register TagPrefix="SplendidCRM" Tagname="ConvertViewAppointment" Src="ConvertViewAppointment.ascx" %>
							<SplendidCRM:ConvertViewAppointment ID="ctlConvertViewAppointment" Runat="Server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>

