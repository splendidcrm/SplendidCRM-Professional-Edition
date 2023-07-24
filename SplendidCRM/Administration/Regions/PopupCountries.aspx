<%@ Page language="c#" MasterPageFile="~/PopupView.Master" Codebehind="PopupCountries.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Administration.Regions.PopupCountries" %>
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
	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchBasic" Src="SearchBasic.ascx" %>
	<SplendidCRM:SearchBasic ID="ctlSearch" Runat="Server" />
	<br />

<script type="text/javascript">
function SelectCountry(sPARENT_NAME)
{
	if (window.opener != null && window.opener.ChangeCountry != null)
	{
		window.opener.ChangeCountry(sPARENT_NAME);
		window.close();
	}
	else
	{
		alert('Original window has closed.  Country cannot be assigned.');
	}
}
function SelectChecked()
{
	if ( window.opener != null && window.opener.ChangeCountry != null )
	{
		var sACLRoles = '';
		for ( var i = 0 ; i < document.all.length ; i++ )
		{
			if ( document.all[i].name == 'chkMain' )
			{
				if ( document.all[i].checked )
				{
					if ( sACLRoles.length > 0 )
						sACLRoles += '|';
					sACLRoles += document.all[i].value;
				}
			}
		}
		window.opener.ChangeCountry(sACLRoles);
		window.close();
	}
	else
	{
		alert('Original window has closed.  Country cannot be assigned.');
	}
}
function Cancel()
{
	window.close();
}
</script>
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="Regions.LBL_COUNTRIES" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlDynamicButtons" Runat="Server" />

	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdPopupView" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="2%">
				<ItemTemplate>
					<input name="chkMain" class="checkbox" type="checkbox" value="<%# DataBinder.Eval(Container.DataItem, "NAME") %>" />
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:HyperLinkColumn HeaderText="Regions.LBL_COUNTRY"             DataTextField="DISPLAY_NAME" SortExpression="DISPLAY_NAME" ItemStyle-CssClass="listViewTdLinkS1" DataNavigateUrlField="NAME" DataNavigateUrlFormatString="javascript:SelectCountry('{0}');" />
			<asp:BoundColumn     HeaderText="Terminology.LBL_LIST_LIST_ORDER" DataField="LIST_ORDER"       SortExpression="LIST_ORDER"   ItemStyle-Width="5%" />
		</Columns>
	</SplendidCRM:SplendidGrid>
	<%@ Register TagPrefix="SplendidCRM" Tagname="CheckAll" Src="~/_controls/CheckAll.ascx" %>
	<SplendidCRM:CheckAll Visible="<%# !PrintView %>" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</asp:Content>
