<%@ Page language="c#" Codebehind="view_embedded.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Reports.ViewEmbedded" %>
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
<!DOCTYPE HTML>
<html>
<head runat="server">
	<script type="text/javascript">
	var sDebugSQL = '';
	</script>
	<script type="text/javascript" src="<%# Application["scriptURL"] %>ModulePopupScripts.aspx?LastModified=<%# Server.UrlEncode(Sql.ToString(Application["Modules.LastModified"])) + "&UserID=" + Security.USER_ID.ToString() %>"></script>
	<script type="text/javascript" src="<%# Application["scriptURL"] %>jquery-1.9.1.min.js"></script>
	<script type="text/javascript" src="<%# Application["scriptURL"] %>SplendidCRM.js"></script>
</head>
<body style="background-color: white;">
<form id="frmMain" method="post" runat="server">
	<div id="divEmbeddedFrame">
		<ajaxToolkit:ToolkitScriptManager ID="mgrAjax" CombineScripts="true" EnableScriptGlobalization="true" EnableScriptLocalization="false" ScriptMode="Release" runat="server" />

		<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
		<SplendidCRM:DynamicButtons ID="ctlDynamicButtons" ShowRequired="false" Runat="Server" />

		<%@ Register TagPrefix="SplendidCRM" Tagname="ParameterView" Src="ParameterView.ascx" %>
		<SplendidCRM:ParameterView ID="ctlParameterView" Runat="Server" />

		<%@ Register TagPrefix="SplendidCRM" Tagname="ReportView" Src="ReportView.ascx" %>
		<SplendidCRM:ReportView ID="ctlReportView" Visible='<%# SplendidCRM.Security.GetUserAccess("Reports", "view") >= 0 %>' Runat="Server" />
		<asp:Label ID="lblAccessError" ForeColor="Red" EnableViewState="false" Text='<%# L10n.Term("ACL.LBL_NO_ACCESS") %>' Visible="<%# !ctlReportView.Visible %>" Runat="server" />
	</div>
</form>
<script type="text/javascript">
function getQuerystring(key, default_)
{
	if ( default_ == null || typeof(default_) == 'undefined' )
		default_ = '';
	key = key.replace(/[\[]/,"\\\[").replace(/[\]]/,"\\\]");
	// 04/13/2012 Paul.  For some odd reason, facebook is using # and not ? as the separator. 
	var regex = new RegExp("[\\?&#]"+key+"=([^&#]*)");
	var qs = regex.exec(window.location.href);
	if ( qs == null )
		return default_;
	else
		return qs[1];
}

window.onload = function()
{
	try
	{
		// 03/09/2021 Paul.  Multi-selection listboxes are not displaying their selected values, so manually correct. 
		<%
		foreach ( System.Data.DataRow row in dtCorrected.Rows )
		{
		%>
			SelectOption('<%= Sql.ToString(row["NAME"]) %>', '<%= Sql.EscapeJavaScript(Sql.ToString(row["VALUE"])) %>');
		<%
		}
		%>
		var nHeight = $('#divEmbeddedFrame').height() + 30;
		var sParentFrame  = getQuerystring('ParentFrame');
		if ( sParentFrame != null && sParentFrame != '' )
		{
			if ( window.parent != null )
			{
				var ctlParentFrame = window.parent.document.getElementById(sParentFrame);
				if ( ctlParentFrame != null )
				{
					ctlParentFrame.style.height = nHeight.toString() + 'px';
				}
			}
		}
	}
	catch(e)
	{
		var ctlReportView_lblError = document.getElementById('ctlReportView_lblError');
		ctlReportView_lblError.innerHTML = e.message;
	}
}
</script>
</body>
</html>
