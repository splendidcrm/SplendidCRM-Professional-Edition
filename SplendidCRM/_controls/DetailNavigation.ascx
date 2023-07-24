<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DetailNavigation.ascx.cs" Inherits="SplendidCRM._controls.DetailNavigation" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
// 07/28/2010 Paul.  The View Change Log link has been moved to a dynamic button. 
</script>
<div id="divPopupAudit">
	<script type="text/javascript">
	function PopupAudit()
	{
		window.open('<%= Application["rootURL"] %>Audit/Popup.aspx?ID=<%= gID %>&Module=<%= sModule %>', 'Audit', '<%= SplendidCRM.Crm.Config.PopupWindowOptions() %>,status=0,toolbar=0,location=0');
		return false;
	}
	function PopupPersonalInfo()
	{
		window.open('<%= Application["rootURL"] %>Audit/PopupPersonalInfo.aspx?ID=<%= gID %>&Module=<%= sModule %>', 'PersonalInfo', '<%= SplendidCRM.Crm.Config.PopupWindowOptions() %>,status=0,toolbar=0,location=0');
		return false;
	}
	</script>
	<asp:Table Width="100%" CellPadding="0" CellSpacing="0" CssClass="" Visible="false" runat="server">
		<asp:TableRow>
			<asp:TableCell CssClass="listViewPaginationTdS1">
				<asp:LinkButton ID="lnkViewChangeLog" OnClientClick="PopupAudit(); return false;" Text='<%# L10n.Term(".LNK_VIEW_CHANGE_LOG") %>' CssClass="listViewPaginationLinkS1" runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="listViewPaginationTdS1" HorizontalAlign="Right">
				<asp:HyperLink ID="lnkReturnToList" Text='<%# L10n.Term(".LNK_LIST_RETURN") %>' CssClass="listViewPaginationLinkS1" runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>

