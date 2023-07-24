<%@ Control Language="c#" AutoEventWireup="false" Codebehind="NewRecord.ascx.cs" Inherits="SplendidCRM.ActivityStream.NewRecord" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
<div id="divNewRecord">
	<asp:Panel ID="pnlMain" Width="100%" CssClass="leftColumnModuleS3" runat="server">
		<asp:Table Width="100%" runat="server">
			<asp:TableRow>
				<asp:TableCell style="vertical-align: top; width: 1%;">
					<asp:Image CssClass="ActivityStreamPicture" SkinID="ActivityStreamUser"   Visible='<%#  Sql.IsEmptyString(Security.PICTURE) %>' runat="server" />
					<asp:Image CssClass="ActivityStreamPicture" src='<%# Security.PICTURE %>' Visible='<%# !Sql.IsEmptyString(Security.PICTURE) %>' runat="server" />
				</asp:TableCell>
				<asp:TableCell>
					<asp:Panel ID="pnlEdit" CssClass="" style="margin-bottom: 4px;" Width="100%" runat="server">
						<table ID="tblMain" class="tabEditView" runat="server">
						</table>
					</asp:Panel>
				</asp:TableCell>
				<asp:TableCell style="vertical-align: top; width: 5%; padding: 8px;">
					<asp:Button ID="btnSubmit" CommandName="NewRecord"        OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term(".LBL_SUBMIT_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_SUBMIT_BUTTON_LABEL") %>' Runat="server" /><br />
					<asp:Button ID="btnCancel" CommandName="NewRecord.Cancel" OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term(".LBL_CANCEL_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_CANCEL_BUTTON_LABEL") %>' Runat="server" />
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
</div>

