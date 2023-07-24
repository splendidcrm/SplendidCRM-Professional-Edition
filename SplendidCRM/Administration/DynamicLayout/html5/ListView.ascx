<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.DynamicLayout.html5.ListView" %>
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

<div id="divListView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlModuleHeader" Module="Administration" Title="Administration.LBL_MANAGE_LAYOUT" EnableModuleLabel="false" EnablePrint="false" HelpName="index" EnableHelp="true" Runat="Server" />
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>

	<%@ Register TagPrefix="SplendidCRM" Tagname="RestUtils" Src="~/_controls/RestUtils.ascx" %>
	<SplendidCRM:RestUtils Runat="Server" />

	<div id="divError" class="error"></div>
	<table id="tblLayoutFrame" cellpadding="0" cellspacing="4" border="0" style="height: 800px;">
		<tr>
			<td width="245" valign="top">
				<div id="divTreeModulesFrame" style="height: 100%; width: 235px; padding: 5px; overflow-y: auto; overflow-x: auto; border: 1px dotted black; vertical-align: top;">
					<ul id="treeModules" class="ztree"></ul>
				</div>
			</td>
			<td id="tdLayoutFrameFieldList" width="230" valign="top">
				<div id="divFieldListFrame" style="height: 100%; padding: 10px; overflow-y: auto; border: 1px dotted black;">
					<h2><%= L10n.Term("DynamicLayout.LBL_TOOLBOX") %></h2>
					<div id="divFieldList"></div>
				</div>
				<img width="240" height="1" src="../../../App_Themes/Six/images/blank.gif" />
			</td>
			<td id="tdLayoutFrameLayout" width="550" valign="top">
				<div id="tblLayoutTableFrame" style="height: 100%; padding: 10px; overflow-y: auto; border: 1px dotted black;">
					<table  cellpadding="0" cellspacing="0" border="0">
						<tr>
							<td width="1%"><h2><%= L10n.Term("DynamicLayout.LBL_LAYOUT") %></h2></td>
							<td style="padding-left: 10px;"><div id="divLayoutError" class="error"></div></td>
						</tr>
					</table>
					<div id="divLayoutButtons" style="margin-top: 2px; margin-bottom: 4px;"></div>
					<table id="tblEvents" style="width: 100%; border: 1px solid black; margin-bottom: 3px; display: none;">
						<tbody></tbody>
					</table>
					<table id="tblLayout" style="width: 100%; border: 1px solid black;">
						<tbody></tbody>
					</table>
				</div>
				<img width="550" height="1" src="../../../App_Themes/Six/images/blank.gif" />
			</td>
			<td id="tdLayoutFrameProperties" width="300" valign="top">
				<div id="tblPropertiesFrame" style="width: 100%; height: 100%; padding: 10px; overflow-y: auto; overflow-x: auto; border: 1px dotted black;">
					<h2><%= L10n.Term("DynamicLayout.LBL_PROPERTIES") %></h2>
					<div id="divPropertiesButtons" style="margin-top: 2px; margin-bottom: 4px; display: none;"></div>
					<table id="tblProperties" cellPadding="3" style="width: 100%; padding: 2px; border: 1px solid #cbcaca;">
						<tbody></tbody>
					</table>
				</div>
			</td>
			<td></td>
		</tr>
	</table>
</div>

<script type='text/javascript'>
var MAX_DUMP_DEPTH = 2;
var sPopupWindowOptions = '<%= SplendidCRM.Crm.Config.PopupWindowOptions() %>';

window.onload = function()
{
	var bgPage = chrome.extension.getBackgroundPage();
	bgPage.IsAuthenticated(function(status, message)
	{
		if ( status == 1 )
		{
			if ( status == 1 )
			{
				AdminLayout_GetTerminology(function(status, message)
				{
					if ( status == 0 || status == 1 )
					{
						AdminLayout_GetAllLists(function(status, message)
						{
							if ( status == 0 || status == 1 )
							{
								AdminLayout_GetModules(function(status, message)
								{
									if ( status == 0 || status == 1 )
									{
										var zNodes = zTree_BuildAdminLayoutModuleNodes(message);
										var setting =
										{
											callback:
											{
												onClick: zTree_AdminLayoutOnClick
											}
											, data:
											{
												simpleData:
												{
													enable: false
												}
											}
										};
										$.fn.zTree.init($('#treeModules'), setting, zNodes);
									}
									else
									{
										SplendidError.SystemMessage(message);
									}
								});
							}
							else
							{
								SplendidError.SystemMessage(message);
							}
						});
					}
					else
					{
						SplendidError.SystemMessage(message);
					}
				});
			}
			else
			{
				SplendidError.SystemMessage(message);
			}
		}
		else
		{
			SplendidError.SystemMessage(message);
		}
	});

	//var layout = new LayoutDetailViewUI();
	//layout.MODULE_NAME = 'Accounts';
	//layout.DETAIL_NAME   = layout.MODULE_NAME + '.DetailView';
	//layout.Load();

	//var layout = new LayoutListViewUI();
	//layout.MODULE_NAME = 'Accounts';
	//layout.GRID_NAME   = layout.MODULE_NAME + '.ListView';
	//layout.Load();

	//var layout = new LayoutDetailViewRelationshipUI();
	//layout.MODULE_NAME = 'Accounts';
	//layout.DETAIL_NAME   = layout.MODULE_NAME + '.DetailView';
	//layout.Load();

	//var layout = new LayoutTerminologyUI();
	//layout.MODULE_NAME = 'Accounts';
	//layout.LANG        = 'en-US';
	//layout.Load();

	AdminLayoutResize();
	$(window).resize(AdminLayoutResize);
}

</script>
