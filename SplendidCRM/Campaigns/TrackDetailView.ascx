<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TrackDetailView.ascx.cs" Inherits="SplendidCRM.Campaigns.TrackDetailView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divTrackDetailView">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" Module="Campaigns" EnablePrint="true" HelpName="TrackDetailView" EnableHelp="true" Runat="Server" />

	<table ID="tblMain" class="tabDetailView" runat="server">
	</table>

	<div Visible="<%# false %>" runat="server">
		<asp:HyperLink ID="lnkXML" NavigateUrl=<%# Application["rootURL"] + "Campaigns/xml/ResponseByRecipientActivity.aspx?ID=" + Request["ID"] + "&" %> Text="XML" Target="xml" Visible="<%# bDebug %>" runat="server" /><br />
	</div>
	<p></p>
	<%@ Register TagPrefix="SplendidCRM" Tagname="ResponseByRecipientActivityHtml5" Src="~/Campaigns/html5/ResponseByRecipientActivity.ascx" %>
	<SplendidCRM:ResponseByRecipientActivityHtml5 Runat="Server" />


<%@ Register TagPrefix="SplendidCRM" Tagname="ResponseByRecipientActivity" Src="~/Campaigns/xaml/ResponseByRecipientActivity.ascx" %>
<script type="text/xaml" id="xamlResponseByRecipientActivity"><?xml version="1.0"?>
<SplendidCRM:ResponseByRecipientActivity Visible="<%# false && SplendidCRM.Crm.Config.enable_silverlight() %>" Runat="Server" />
</script>
	<asp:Panel Visible="<%# false && SplendidCRM.Crm.Config.enable_silverlight() %>" runat="server">
		<div id="hostResponseByRecipientActivity" style="width: 800x; height: 400px; padding-bottom: 2px;"></div>
<SplendidCRM:InlineScript runat="server">
			<script type="text/javascript">
			Silverlight.createObjectEx({
				source: "#xamlResponseByRecipientActivity",
				parentElement: document.getElementById("hostResponseByRecipientActivity"),
				id: "SilverlightControl",
				properties: {
					width: "800",
					height: "400",
					version: "1.0",
					enableHtmlAccess: "true",
					isWindowless: "true" /* 05/08/2010 Paul.  The isWindowless allows HTML to appear over a silverlight app. */
				},
				events: {}
			});
			</script>
</SplendidCRM:InlineScript>
	</asp:Panel>
	<p></p>
	<div Visible="<%# SplendidCRM.Crm.Config.enable_flash() %>" runat="server">
	<object id="hBarF" width="800" height="400" align="" classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" codebase="https://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,0,0" viewastext>
		<param name="movie"   value="<%= Application["chartURL" ] %>hBarF.swf?filename=<%= Server.UrlEncode(Application["rootURL"] + "Campaigns/xml/ResponseByRecipientActivity.aspx?ID=" + gID.ToString() + "&") %>">
		<param name="bgcolor" value="#FFFFFF" />
		<param name="wmode"   value="transparent" />
		<param name="quality" value="high" />
		<embed src="<%= Application["chartURL" ] %>hBarF.swf?filename=<%= Server.UrlEncode(Application["rootURL"] + "Campaigns/xml/ResponseByRecipientActivity.aspx?ID=" + gID.ToString() + "&") %>" wmode="transparent" quality="high" bgcolor="#FFFFFF" height="400" width="800" name="hBarF" align="" type="application/x-shockwave-flash" pluginspage="https://www.macromedia.com/go/getflashplayer" />
	</object>
	</div>

	<div id="divDetailSubPanel">
		<asp:PlaceHolder ID="plcSubPanel" Runat="server" />
	</div>
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />

