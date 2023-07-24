<%@ Control Language="c#" AutoEventWireup="false" Codebehind="CheckAll.ascx.cs" Inherits="SplendidCRM._controls.CheckAll" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
var sSelectedLabelFormat = '<%= L10n.Term(".LBL_SELECTED") %>';

function ValidateOne()
{
	if ( SelectedCount('<%= sFieldName %>') < 1 )
	{
		alert('<%= Sql.EscapeJavaScript(L10n.Term(".LBL_LISTVIEW_NO_SELECTED")) %>');
		return false;
	}
	return true;
}
function ValidateTwo()
{
	if ( SelectedCount('<%= sFieldName %>') < 2 )
	{
		alert('<%= Sql.EscapeJavaScript(L10n.Term(".LBL_LISTVIEW_TWO_REQUIRED")) %>');
		return false;
	}
	return true;
}

function SplendidGrid_CheckAll(value)
{
	var fld = document.forms[0]['<%= sFieldName %>'];
	if ( fld != undefined )
	{
		if ( fld.length == undefined )
		{
			if ( fld.type == 'checkbox' )
			{
				fld.checked = value;
				SplendidGrid_ToggleCheckbox(fld);
			}
		}
		else
		{
			for (i = 0; i < fld.length; i++)
			{
				if ( fld[i].type == 'checkbox' )
				{
					fld[i].checked = value;
					SplendidGrid_ToggleCheckbox(fld[i]);
				}
			}
		}
		if ( !value )
		{
			var fldSelectedLabelClientID = document.getElementById('<%= lblSelectedLabel.ClientID %>');
			var fldSelectedItemsClientID = document.getElementById('<%= hidSelectedItems.ClientID %>');
			if ( fldSelectedItemsClientID != null )
				fldSelectedItemsClientID.value = '';
			if ( fldSelectedLabelClientID != null )
				fldSelectedLabelClientID.innerHTML = sSelectedLabelFormat.replace('{0}', Math.floor((fldSelectedItemsClientID.value.length+1)/37));
		}
	}
}

// 11/27/2010 Paul.  Special functions to add and remove checkbox values from a hidden field. 
function SplendidGrid_ToggleCheckbox(chk)
{
	var fldSelectedLabelClientID = document.getElementById('<%= lblSelectedLabel.ClientID %>');
	var fldSelectedItemsClientID = document.getElementById('<%= hidSelectedItems.ClientID %>');
	if ( fldSelectedItemsClientID != null )
	{
		if ( chk.checked )
		{
			if ( fldSelectedItemsClientID.value.indexOf(chk.value) < 0 )
			{
				if ( fldSelectedItemsClientID.value.length > 0 )
					fldSelectedItemsClientID.value += ',';
				fldSelectedItemsClientID.value += chk.value;
			}
		}
		else
		{
			// 09/20/2013 Paul.  New method of removing an item from a comma-separated string. 
			var arr = fldSelectedItemsClientID.value.split(',');
			var i = arr.indexOf(chk.value);
			if ( i != -1 )
				arr.splice(i, 1);
			fldSelectedItemsClientID.value = arr.join(',');
		}
		if ( fldSelectedLabelClientID != null )
		{
			// 09/20/2013 Paul.  Need a new method for counting items that is not guid-specific. 
			var nItems = 0;
			if ( chk.name == 'chkMain' )
				nItems = Math.floor((fldSelectedItemsClientID.value.length+1)/37);
			else if ( fldSelectedItemsClientID.value.length > 0 )
				nItems = fldSelectedItemsClientID.value.split(',').length;
			fldSelectedLabelClientID.innerHTML = sSelectedLabelFormat.replace('{0}', nItems);
		}
	}
}

</script>
<asp:Panel ID="pnlCheckAll" Visible="<%# !SplendidDynamic.StackedLayout(Page.Theme) %>" style="display: inline-block;" runat="server">
	<asp:HyperLink ID="lnkSelectPage" NavigateUrl='<%# "javascript:SplendidGrid_CheckAll(1);" %>' Text='<%# L10n.Term(".LBL_SELECT_PAGE") %>' CssClass="listViewCheckLink" runat="server" />
	<asp:Literal Text=" - " runat="server" />
	<asp:LinkButton ID="btnSelectAll" Visible="<%# bShowSelectAll %>" Text='<%# L10n.Term(".LBL_SELECT_ALL") %>' CommandName="SelectAll" OnCommand="Page_Command" CssClass="listViewCheckLink" runat="server" />
	<asp:Literal Text=" - " Visible="<%# bShowSelectAll %>" runat="server" />
	<asp:HyperLink ID="lnkDeselectAll" NavigateUrl='<%# "javascript:SplendidGrid_CheckAll(0);" %>' Text='<%# L10n.Term(".LBL_DESELECT_ALL") %>' CssClass="listViewCheckLink" runat="server" />
	&nbsp;
</asp:Panel>
<asp:Label ID="lblSelectedLabel" runat="server" />
<asp:HiddenField ID="hidSelectedItems" runat="server" />
<div class="CheckAllPaddingBottom">
</div>

