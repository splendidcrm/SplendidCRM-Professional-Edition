<%@ Control CodeBehind="AccessView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.ACLRoles.AccessView" %>
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
<script type="text/javascript">
function toggleDisplay(sID)
{
	var fld = document.getElementById(sID);
	fld.style.display = (fld.style.display == 'none') ? 'inline' : 'none';
	var fldLink = document.getElementById(sID + 'link');
	if ( fldLink != null )
	{
		// 02/28/2008 Paul.  The linked field is the opposite of the main. 
		fldLink.style.display = (fld.style.display == 'none') ? 'inline' : 'none';
	}
}
</script>
<div id="divListView">
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
	
	<SplendidCRM:ACLGrid id="grdACL" Width="100%" CssClass="tabDetailView"
		CellPadding="0" CellSpacing="1" border="0"
		AllowPaging="false" AllowSorting="false" 
		AutoGenerateColumns="false" EnableACLEditing="false"
		EnableViewState="true" runat="server">
		<ItemStyle            CssClass="tabDetailViewDF" />
		<AlternatingItemStyle CssClass="tabDetailViewDF" />
		<HeaderStyle          CssClass="tabDetailViewDL" />
	</SplendidCRM:ACLGrid>
	
	<asp:Panel ID="pnlAdmin" runat="server">
		<br />
		<SplendidCRM:ACLGrid id="grdACL_Admin" Width="100%" CssClass="tabDetailView"
			CellPadding="0" CellSpacing="1" border="0"
			AllowPaging="false" AllowSorting="false" 
			AutoGenerateColumns="false" EnableACLEditing="false"
			EnableViewState="true" runat="server">
			<ItemStyle            CssClass="tabDetailViewDF" />
			<AlternatingItemStyle CssClass="tabDetailViewDF" />
			<HeaderStyle          CssClass="tabDetailViewDL" />
		</SplendidCRM:ACLGrid>
	</asp:Panel>
</div>

