<%@ Page language="c#" MasterPageFile="~/PopupView.Master" Codebehind="SignaturePopup.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Reports.SignaturePopup" %>
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
	<div id="divSignaturePanel" style="width: 600pt; height: 100pt;"></div>

	<div>
		<asp:Button ID="btnSubmit" OnClientClick="SubmitSignature(); return false;" CssClass="EditHeaderFirstButton" runat="server" />&nbsp;
		<asp:Button ID="btnClear"  OnClientClick="ClearSignature() ; return false;" CssClass="EditHeaderFirstButton" runat="server" />&nbsp;
		<asp:Button ID="btnCancel" OnClientClick="CancelSignature(); return false;" CssClass="EditHeaderFirstButton" runat="server" />&nbsp;
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" runat="server" />
	</div>
	
	<%@ Register TagPrefix="SplendidCRM" Tagname="ReportView" Src="ReportView.ascx" %>
	<SplendidCRM:ReportView ID="ctlReportView" Visible='<%# PortalCache.IsPortal() || SplendidCRM.Security.GetUserAccess("Reports", "view") >= 0 %>' Runat="Server" />
	<asp:Label ID="lblAccessError" ForeColor="Red" EnableViewState="false" Text='<%# L10n.Term("ACL.LBL_NO_ACCESS") %>' Visible="<%# !ctlReportView.Visible %>" Runat="server" />

<script type='text/javascript'>
function SubmitSignature()
{
	var lblError = document.getElementById('<%# lblError.ClientID %>');
	var sSignature = $('#divSignaturePanel').signature('toJSON');
	var oSignature = jQuery.parseJSON(sSignature);
	if ( oSignature.lines.length == 0 )
	{
		$(lblError).text('<%# HttpUtility.JavaScriptStringEncode(L10n.Term("Orders.ERR_SIGNATURE_NOT_PROVIDED")) %>');
	}
	else if ( window.opener != null && window.opener.SubmitSignature != null )
	{
		oSignature.width     = $('#divSignaturePanel').width();
		oSignature.height    = $('#divSignaturePanel').height();
		oSignature.REPORT_ID = '<%# gID %>';
		sSignature = JSON.stringify(oSignature);
		window.opener.SubmitSignature(sSignature);
		window.close();
	}
	else
	{
		$(lblError).text('Original window has closed.  Signature cannot be submitted.');
	}
}

function ClearSignature()
{
	$('#divSignaturePanel').signature('clear');
}

function CancelSignature()
{
	window.close();
}

window.onload = function()
{
	try
	{
		var rdlViewer_fixedTable = document.getElementById('<%# ctlReportView.ClientID + "_rdlViewer_fixedTable" %>');
		if (rdlViewer_fixedTable != null)
		{
			var nWidth = $(rdlViewer_fixedTable).width();
			if (nWidth < 600)
				nWidth = 600;
			$('#divSignaturePanel').width(nWidth + 'px');
			$('#divSignaturePanel').signature();
			var rect = rdlViewer_fixedTable.getBoundingClientRect();
			var nWidth = rect.right > 600 ? rect.right : 600;
			var nHeight = rect.bottom > 600 ? rect.bottom : 600;
			window.resizeTo(nWidth + 40, nHeight + 40);
		}
	}
	catch(e)
	{
	}
}
</script>
</asp:Content>

