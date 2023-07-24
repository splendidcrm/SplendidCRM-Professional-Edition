<%@ Page language="c#" MasterPageFile="~/DefaultView.Master" Codebehind="SetTimezone.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Users.SetTimezone" %>
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
function SetBrowserDefaultTimezone()
{
	var lstTIMEZONE  = document.getElementById('<%= lstTIMEZONE.ClientID %>');
	if ( lstTIMEZONE != null )
	{
		if ( lstTIMEZONE.options.selectedIndex == 0 )
		{
			var dtJanuary = new Date((new Date()).getFullYear(), 0, 1, 0, 0, 0);
			
			var sDefaultOffset;
			if ( dtJanuary.getTimezoneOffset() > 0 )
				sDefaultOffset = '(GMT-' + ('0' +    dtJanuary.getTimezoneOffset()/60 + ':00').substring(0, 5) + ')';
			else
				sDefaultOffset = '(GMT+' + ('0' + -1*dtJanuary.getTimezoneOffset()/60 + ':00').substring(0, 5) + ')';

			for ( i = 0; i < lstTIMEZONE.options.length; i++ )
			{
				if ( lstTIMEZONE.options[i].text.substring(0, sDefaultOffset.length) == sDefaultOffset )
				{
					lstTIMEZONE.options.selectedIndex = i;
					break;
				}
			}
		}
	}
}
</script>
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
	
	
	<asp:Table Width="400" BorderWidth="1" BorderColor="#444444" CellPadding="8" CellSpacing="2" HorizontalAlign="Center" CssClass="" runat="server">
		<asp:TableRow>
			<asp:TableCell />
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell style="padding-bottom: 5px;">
				<asp:Label Text='<%# L10n.Term("Users.LBL_PICK_TZ_WELCOME") %>' runat="server" />
				<br />
				<br />
				<asp:Label Text='<%# L10n.Term("Users.LBL_PICK_TZ_DESCRIPTION") %>' runat="server" />
				<br />
				<br />
				<asp:DropDownList ID="lstTIMEZONE" DataValueField="ID" DataTextField="NAME" TabIndex="3" Runat="server" />
				&nbsp;<asp:Button ID="btnSave" CommandName="Save" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SAVE_BUTTON_LABEL"  ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_SAVE_BUTTON_TITLE"  ) %>' AccessKey='<%# L10n.AccessKey(".LBL_SAVE_BUTTON_KEY"  ) %>' Runat="server" /><br />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<br />
	<script type="text/javascript">
		SetBrowserDefaultTimezone();
	</script>
</asp:Content>

