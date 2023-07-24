<%@ Control CodeBehind="PopupView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Products.ProductCatalog.PopupView" %>
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
// 05/14/2012 Paul.  ChangeProductCatalog to match ModulePopupScripts generated code. 
function SelectProductTemplate(sPARENT_ID, sPARENT_NAME)
{
	var bEnableOptions = '<%= bEnableOptions ? 1 : 0 %>';
	if ( bEnableOptions == '1' )
	{
		var chk = document.getElementById('PRODUCT_CATALOG_ID_' + sPARENT_ID);
		if ( chk != null )
		{
			chk.checked = true;
			SelectChecked();
		}
		else
		{
			alert('Could not find ' + sPARENT_ID);
		}
	}
	else
	{
		if ( window.opener != null && window.opener.ChangeProductCatalog != null )
		{
			window.opener.ChangeProductCatalog(sPARENT_ID, sPARENT_NAME);
			window.close();
		}
		else
		{
			alert('Original window has closed.  Product Template cannot be assigned.' + '\n' + sPARENT_ID + '\n' + sPARENT_NAME);
		}
	}
}
function VerifyOptions(sID)
{
	var bVerified = false;
	try
	{
		var fldMINIMUM_OPTIONS = document.getElementById('MINIMUM_OPTIONS_' + sID);
		var fldMAXIMUM_OPTIONS = document.getElementById('MAXIMUM_OPTIONS_' + sID);
		if ( fldMINIMUM_OPTIONS != null && fldMAXIMUM_OPTIONS != null )
		{
			var nMINIMUM_OPTIONS = parseInt(fldMINIMUM_OPTIONS.value);
			var nMAXIMUM_OPTIONS = parseInt(fldMAXIMUM_OPTIONS.value);
			if ( nMINIMUM_OPTIONS > 0 && nMAXIMUM_OPTIONS > 0 )
			{
				var nSELECTED_OPTIONS = 0;
				var form = document.forms[0];
				for ( var i = 0 ; i < form.elements.length ; i++ )
				{
					if ( form.elements[i].name == 'chkMain' )
					{
						if ( form.elements[i].checked )
						{
							// 07/10/2010 Paul.  We can't lookup the parent as it might find the stand-alone product. 
							// Make sure to position the parent as the next input field. 
							//var fldPARENT_ID = document.getElementById('PARENT_ID_' + form.elements[i].value);
							var fldPARENT_ID = form.elements[i+1];
							if ( fldPARENT_ID != null )
							{
								if ( fldPARENT_ID.id == 'PARENT_ID_' + form.elements[i].value )
								{
									if ( fldPARENT_ID.value == sID )
										nSELECTED_OPTIONS++;
								}
								else
								{
									alert('Next input field should be PARENT_ID.');
								}
							}
						}
					}
				}
				if ( nSELECTED_OPTIONS < nMINIMUM_OPTIONS )
					alert('<%= Sql.EscapeJavaScript(L10n.Term("ProductCatalog.ERR_MINIMUM_OPTIONS")) %>');
				else if ( nSELECTED_OPTIONS > nMAXIMUM_OPTIONS )
					alert('<%= Sql.EscapeJavaScript(L10n.Term("ProductCatalog.ERR_MAXIMUM_OPTIONS")) %>');
				else
					bVerified = true;
			}
			else
			{
				bVerified = true;
			}
		}
		else
		{
			bVerified = true;
		}
	}
	catch(e)
	{
		alert(e);
	}
	return bVerified;
}
function SelectChecked()
{
	var bEnableOptions = '<%= bEnableOptions ? 1 : 0 %>';
	if ( window.opener != null && window.opener.ChangeProductCatalog != null )
	{
		var sProductTemplate = '';
		var form = document.forms[0];
		for ( var i = 0 ; i < form.elements.length ; i++ )
		{
			if ( form.elements[i].name == 'chkMain' )
			{
				if ( form.elements[i].checked )
				{
					if ( sProductTemplate.length > 0 )
						sProductTemplate += ',';
					if ( bEnableOptions == '1' )
					{
						if ( !VerifyOptions(form.elements[i].value) )
							return;
					}
					sProductTemplate += form.elements[i].value;
					// 07/11/2010 Paul.  We need to append the Parent ID if it exists. 
					var fldPARENT_ID = form.elements[i+1];
					if ( fldPARENT_ID != null )
					{
						if ( fldPARENT_ID.id == 'PARENT_ID_' + form.elements[i].value )
						{
							if ( fldPARENT_ID.value.length > 0 )
								sProductTemplate += '|' + fldPARENT_ID.value;
						}
					}
				}
			}
		}
		if ( sProductTemplate == '' )
		{
			alert('<%= Sql.EscapeJavaScript(L10n.Term(".LBL_LISTVIEW_NO_SELECTED")) %>');
		}
		else
		{
			window.opener.ChangeProductCatalog(sProductTemplate, '');
			window.close();
		}
	}
	else
	{
		alert('Original window has closed.  Product Template cannot be assigned.');
	}
}
function Clear()
{
	if ( window.opener != null && window.opener.ChangeProductCatalog != null )
	{
		window.opener.ChangeProductCatalog('', '');
		window.close();
	}
	else
	{
		alert('Original window has closed.  Product Template cannot be assigned.');
	}
}
function Cancel()
{
	window.close();
}
</script>
<div id="divPopupView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchView" Src="~/_controls/SearchView.ascx" %>
	<SplendidCRM:SearchView ID="ctlSearchView" Module="Products" IsPopupSearch="true" ShowSearchTabs="false" Visible="<%# !PrintView %>" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="ProductTemplates.LBL_LIST_FORM_TITLE" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlDynamicButtons" Runat="Server" />

	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdPopupView" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="2%">
				<ItemTemplate>
					<div Visible="<%# !PrintView && (bMultiSelect || bEnableOptions) %>" runat="server">
						<asp:Literal Text="&nbsp;&nbsp;&nbsp;" Visible='<%# !Sql.IsEmptyGuid(Eval("PARENT_ID")) %>' runat="server" />
						<input name="chkMain" id="PRODUCT_CATALOG_ID_<%# Eval("ID") %>" class="checkbox" type="checkbox" value="<%# Eval("ID") %>" />
						<input type="hidden" id="PARENT_ID_<%# Eval("ID") %>" value="<%# Eval("PARENT_ID") %>" />
						<input type="hidden" id="MINIMUM_OPTIONS_<%# Eval("ID") %>" value="<%# Sql.ToInteger(Eval("MINIMUM_OPTIONS")) %>" />
						<input type="hidden" id="MAXIMUM_OPTIONS_<%# Eval("ID") %>" value="<%# Sql.ToInteger(Eval("MAXIMUM_OPTIONS")) %>" />
					</div>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
	<%@ Register TagPrefix="SplendidCRM" Tagname="CheckAll" Src="~/_controls/CheckAll.ascx" %>
	<SplendidCRM:CheckAll ID="ctlCheckAll" Visible="<%# !PrintView && bMultiSelect %>" ShowSelectAll="false" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>
