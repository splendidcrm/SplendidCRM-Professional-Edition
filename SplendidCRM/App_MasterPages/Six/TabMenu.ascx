<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TabMenu.ascx.cs" Inherits="SplendidCRM.Themes.Six.TabMenu" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
<div id="divTabMenu">
	<table ID="tblSixMenu" class="tabFrame" cellspacing="0" cellpadding="0" runat="server" />
	<asp:Panel ID="pnlTabMenuMore" style="display: none;" runat="server">
		<table cellpadding="0" cellspacing="0" class="MoreActionsShadingTable">
			<tr>
				<td colspan="3" class="ModuleActionsShadingHorizontal"></td>
			</tr>
			<tr>
				<td class="ModuleActionsShadingVertical"></td>
				<td>
					<table cellpadding="0" cellspacing="0" class="ModuleActionsInnerTable">
						<tr>
							<td class="ModuleActionsInnerCell"><asp:PlaceHolder ID="phMoreInnerCell" runat="server" /></td>
						</tr>
					</table>
				</td>
				<td class="ModuleActionsShadingVertical"></td>
			</tr>
			<tr>
				<td colspan="3" class="ModuleActionsShadingHorizontal"></td>
			</tr>
		</table>
	</asp:Panel>
	<asp:PlaceHolder ID="phHoverControls" runat="server" />
	<%@ Register TagPrefix="SplendidCRM" Tagname="Actions" Src="../Sugar/Actions.ascx" %>
	<SplendidCRM:Actions ID="ctlActions" Visible='<%# !PrintView && SplendidCRM.Utils.SupportsTouch %>' Runat="Server" />
</div>

