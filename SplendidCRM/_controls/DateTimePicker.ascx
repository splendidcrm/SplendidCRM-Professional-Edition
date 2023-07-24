<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DateTimePicker.ascx.cs" Inherits="SplendidCRM._controls.DateTimePicker" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
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
// 08/12/2014 Paul.  Add format to calendar. 
</script>
<asp:Table BorderWidth="0" CellPadding="0" CellSpacing="0" runat="server">
	<asp:TableRow>
		<asp:TableCell Wrap="false">
			<asp:TextBox ID="txtDATE" TabIndex="1" size="11" MaxLength="40" OnTextChanged="Date_Changed" Runat="server" />
			<ajaxToolkit:CalendarExtender ID="extDATE" TargetControlID="txtDATE" PopupButtonID="imgCalendar" Animated="false" Format="<%# System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern %>" runat="server" />
			&nbsp;<asp:Image ID="imgCalendar" AlternateText='<%# L10n.Term(".LBL_ENTER_DATE") %>' SkinID="Calendar" Runat="server" />
			&nbsp;
		</asp:TableCell>
		<asp:TableCell Wrap="false">
			<asp:DropDownList ID="lstHOUR"     TabIndex="1" OnSelectedIndexChanged="Date_Changed" Runat="server" />
			<asp:DropDownList ID="lstMINUTE"   TabIndex="1" OnSelectedIndexChanged="Date_Changed" Runat="server" />
			<asp:DropDownList ID="lstMERIDIEM" TabIndex="1" OnSelectedIndexChanged="Date_Changed" Visible="false" Runat="server" />
		</asp:TableCell>
		<asp:TableCell>
			<!-- 08/31/2006 Paul.  We cannot use a regular expression validator because there are just too many date formats. -->
			<SplendidCRM:DateValidator ID="valDATE" ControlToValidate="txtDATE" CssClass="required" EnableClientScript="false" EnableViewState="false" Enabled="false" Runat="server" />
			<asp:RequiredFieldValidator     ID="reqDATE" ControlToValidate="txtDATE" CssClass="required" EnableClientScript="false" EnableViewState="false" Enabled="false" Runat="server" />
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow>
		<asp:TableCell Wrap="false"><asp:Label ID="lblDATEFORMAT" CssClass="dateFormat" Runat="server" /></asp:TableCell>
		<asp:TableCell Wrap="false"><asp:Label ID="lblTIMEFORMAT" CssClass="dateFormat" Runat="server" /></asp:TableCell>
		<asp:TableCell Wrap="false">&nbsp;</asp:TableCell>
	</asp:TableRow>
</asp:Table>

