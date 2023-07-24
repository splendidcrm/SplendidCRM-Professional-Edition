<%@ Page language="c#" MasterPageFile="~/PopupView.Master" Codebehind="Popup.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Administration.Dropdown.Popup" %>
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
function ChangeItem()
{
	if ( window.opener != null && window.opener.ChangeItem != null )
	{
		window.opener.ChangeItem(document.getElementById('<%= txtKEY.ClientID %>').value, document.getElementById('<%= txtVALUE.ClientID %>').value, <%= nINDEX %>);
		window.close();
	}
}
function Cancel()
{
	window.close();
}
// 08/30/2006 Paul.  Fix onload to support Firefox. 
window.onload = function()
{
	document.getElementById('<%= txtKEY.ClientID %>').focus();
}
</script>

<asp:Table ID="tblMain" SkinID="tabForm" runat="server">
	<asp:TableRow>
		<asp:TableCell>
			<table width="100%" border="0" cellspacing="0" cellpadding="0">
				<tr>
					<td class="dataLabel" noWrap><asp:Label Text='<%# L10n.Term("Dropdown.LBL_KEY"  ) %>' runat="server" />&nbsp;&nbsp;<asp:TextBox ID="txtKEY"   CssClass="dataField" Runat="server" /></td>
					<td class="dataLabel" noWrap><asp:Label Text='<%# L10n.Term("Dropdown.LBL_VALUE") %>' runat="server" />&nbsp;&nbsp;<asp:TextBox ID="txtVALUE" CssClass="dataField" Runat="server" /></td>
					<td align="right">
						<asp:Button ID="btnPopupSelect" OnClientClick="ChangeItem(); return false;" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SAVE_BUTTON_LABEL"  ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_SAVE_BUTTON_LABEL"  ) %>' AccessKey='<%# L10n.AccessKey(".LBL_SAVE_BUTTON_KEY"  ) %>' runat="server" />
						<asp:Button ID="btnPopupCancel" OnClientClick="Cancel(); return false;"     CssClass="button" Text='<%# "  " + L10n.Term(".LBL_CANCEL_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_CANCEL_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_CANCEL_BUTTON_KEY") %>' runat="server" />
					</td>
				</tr>
			</table>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
</asp:Content>

