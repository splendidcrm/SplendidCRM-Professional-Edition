<%@ Control Language="c#" AutoEventWireup="false" Codebehind="CRON.ascx.cs" Inherits="SplendidCRM._controls.CRON" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<asp:Table CellPadding="0" CellSpacing="0" runat="server">
	<asp:TableRow>
		<asp:TableCell VerticalAlign="Top" style="border-right: solid 1px black; padding-right: 10px; padding-top: 5px;">
			<asp:RadioButtonList ID="radFREQUENCY" DataValueField="NAME" DataTextField="DISPLAY_NAME" OnSelectedIndexChanged="radFREQUENCY_SelectedIndexChanged" AutoPostBack="true" CssClass="radio" style="white-space: nowrap;" runat="server" />
		</asp:TableCell>
		<asp:TableCell style="padding-left: 10px;">
			<asp:Table CellPadding="5" CellSpacing="0" runat="server">
				<asp:TableRow>
					<asp:TableCell VerticalAlign="Top">
						<asp:Label ID="lblMINUTES" Text='<%# L10n.Term("Schedulers.LBL_MINS"        ) %>' runat="server" /><br />
						<asp:ListBox ID="lstMINUTES" SelectionMode="Multiple" Rows="4" OnSelectedIndexChanged="lstMINUTES_SelectedIndexChanged" AutoPostBack="true" runat="server" />
					</asp:TableCell>
					<asp:TableCell VerticalAlign="Top">
						<asp:Label ID="lblHOURS" Text='<%# L10n.Term("Schedulers.LBL_HOURS"       ) %>' runat="server" /><br />
						<asp:ListBox ID="lstHOURS" SelectionMode="Multiple" Rows="4" OnSelectedIndexChanged="lstHOURS_SelectedIndexChanged" AutoPostBack="true" runat="server" />
					</asp:TableCell>
					<asp:TableCell VerticalAlign="Top">
						<asp:Label ID="lblDAYOFMONTH" Text='<%# L10n.Term("Schedulers.LBL_DAY_OF_MONTH") %>' runat="server" /><br />
						<asp:ListBox ID="lstDAYOFMONTH" SelectionMode="Multiple" Rows="4" OnSelectedIndexChanged="lstDAYOFMONTH_SelectedIndexChanged" AutoPostBack="true" runat="server" />
					</asp:TableCell>
					<asp:TableCell VerticalAlign="Top">
						<br />
						<asp:CheckBoxList ID="chkDAYOFWEEK" DataValueField="NAME" DataTextField="DISPLAY_NAME" OnSelectedIndexChanged="chkDAYOFWEEK_SelectedIndexChanged" RepeatColumns="4" AutoPostBack="true" CssClass="checkbox" RepeatDirection="Horizontal" style="white-space: nowrap; vertical-align: top;" runat="server" />
						<asp:CheckBoxList ID="chkMONTHS"    DataValueField="NAME" DataTextField="DISPLAY_NAME" OnSelectedIndexChanged="chkMONTHS_SelectedIndexChanged"    RepeatColumns="4" AutoPostBack="true" CssClass="checkbox" RepeatDirection="Horizontal" style="white-space: nowrap; vertical-align: top; padding-bottom: 10px;" runat="server" />
					</asp:TableCell>
				</asp:TableRow>
			</asp:Table>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>

<SplendidCRM:InlineScript runat="server">
<script type="text/javascript">
function ToggleCRONShow()
{
	try
	{
		var chkCRONShow = document.getElementById('<%# chkCRONShow.ClientID %>');
		chkCRONShow.checked = !chkCRONShow.checked;
		toggleDisplay('<%# pnlCRONValue.ClientID  %>');
	}
	catch(e)
	{
		alert(e.message);
	}
}
</script>
</SplendidCRM:InlineScript>
<asp:Table CellPadding="0" CellSpacing="0" runat="server">
	<asp:TableRow>
		<asp:TableCell VerticalAlign="Top" style="padding-right: 5px;">
			<asp:HyperLink NavigateUrl="javascript: ToggleCRONShow();" CssClass="utilsLink" runat="server">
				<asp:Image SkinID="advanced_search" runat="server" />
			</asp:HyperLink>
			<asp:CheckBox ID="chkCRONShow" style="display:none" CssClass="checkbox" runat="server" />
			&nbsp;<asp:Label ID="lblCRON_MESSAGE" Font-Italic="true" runat="server" /><br />
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow>
		<asp:TableCell VerticalAlign="Top">
			<asp:Panel ID="pnlCRONValue" style='<%# (chkCRONShow.Checked ? "display:inline" : "display:none") %>' runat="server">
				<asp:Table CellPadding="0" CellSpacing="0" runat="server">
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Schedulers.LBL_MINS"        ) %>' runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Schedulers.LBL_HOURS"       ) %>' runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Schedulers.LBL_DAY_OF_MONTH") %>' runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Schedulers.LBL_MONTHS"      ) %>' runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Schedulers.LBL_DAY_OF_WEEK" ) %>' runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataField"><asp:TextBox ID="CRON_MINUTES"    Text="0"  size="3" MaxLength="25" OnTextChanged="CRON_Changed" AutoPostBack="true" runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:TextBox ID="CRON_HOURS"      Text="23" size="3" MaxLength="25" OnTextChanged="CRON_Changed" AutoPostBack="true" runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:TextBox ID="CRON_DAYOFMONTH" Text="*"  size="3" MaxLength="25" OnTextChanged="CRON_Changed" AutoPostBack="true" runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:TextBox ID="CRON_MONTHS"     Text="*"  size="3" MaxLength="25" OnTextChanged="CRON_Changed" AutoPostBack="true" runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:TextBox ID="CRON_DAYOFWEEK"  Text="*"  size="3" MaxLength="25" OnTextChanged="CRON_Changed" AutoPostBack="true" runat="server" /></asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:Panel>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>

