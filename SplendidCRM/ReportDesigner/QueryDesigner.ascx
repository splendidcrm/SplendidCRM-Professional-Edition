<%@ Control CodeBehind="QueryDesigner.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.ReportDesigner.QueryDesigner" %>
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
<div id="divQueryDesigner">
	<div id="lblError" class="error"></div>
	<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	<table cellspacing="0" cellpadding="0" width="100%" style="border-left: 1px solid #cccccc; border-right: 1px solid #cccccc; border-bottom: 1px solid #cccccc;">
		<tbody>
			<tr>
				<td valign="top" rowspan="3" width="300px" style="border: 1px solid #cccccc;">
					<table cellspacing="0" cellpadding="4" width="100%">
						<tbody>
							<tr class="listViewThS1">
								<td><%# L10n.Term("ReportDesigner.LBL_MODULES") %></td>
							</tr>
						</tbody>
					</table>
					<div style="height: 640px; overflow-y: auto; width: 300px; overflow-x: auto;">
						<ul id="treeModules" class="ztree"></ul>
					</div>
				</td>
				<td valign="top" style="height: 240px; border: 1px solid #cccccc;">
					<table cellspacing="0" cellpadding="4" width="100%">
						<tbody>
							<tr class="listViewThS1">
								<td width="20%"><%# L10n.Term("ReportDesigner.LBL_SELECTED_FIELDS") %></td>
								<td width="80%" align="right">
									<input id="chkGroupAndAggregate" type="checkbox" onclick="return chkGroupAndAggregate_Clicked(this);" class="checkbox" />&nbsp;<label for="chkGroupAndAggregate"><%# L10n.Term("ReportDesigner.LBL_GROUP_AND_AGGREGATE") %></label>&nbsp;
									<a href="#" onclick="return tSelectedFields_SelectedDelete();"  ><asp:Image SkinID="ReportDesignerItemDelete"   runat="server" /></a>
									<a href="#" onclick="return tSelectedFields_SelectedMoveUp();"  ><asp:Image SkinID="ReportDesignerItemMoveUp"   runat="server" /></a>
									<a href="#" onclick="return tSelectedFields_SelectedMoveDown();"><asp:Image SkinID="ReportDesignerItemMoveDown" runat="server" /></a>
								</td>
							</tr>
						</tbody>
					</table>
					<div style="height: 240px; overflow-y: auto;">
						<table cellspacing="0" cellpadding="4" width="100%">
							<tbody id="tSelectedFields">
								<tr id="" class="listViewThS1">
									<td width="25%"><%# L10n.Term("ReportDesigner.LBL_FIELD"         ) %></td>
									<td width="30%"><%# L10n.Term("ReportDesigner.LBL_DISPLAY_NAME"  ) %></td>
									<td width="15%"><%# L10n.Term("ReportDesigner.LBL_DISPLAY_WIDTH" ) %></td>
									<td width="15%"><%# L10n.Term("ReportDesigner.LBL_AGGREGATE"     ) %></td>
									<td width="15%"><%# L10n.Term("ReportDesigner.LBL_SORT_DIRECTION") %></td>
								</tr>
							</tbody>
						</table>
					</div>
				</td>
			</tr>
			<tr>
				<td valign="top" style="height: 200px; border: 1px solid #cccccc;">
					<table cellspacing="0" cellpadding="4" width="100%">
						<tbody>
							<tr class="listViewThS1">
								<td width="20%"><%# L10n.Term("ReportDesigner.LBL_RELATIONSHIPS") %></td>
								<td width="80%" align="right">
									<a href="#" onclick="return tRelationships_AddRelationship();" ><asp:Image SkinID="ReportDesignerRelationshipCreate" runat="server" /></a>
									<a href="#" onclick="return tRelationships_SelectedDelete();"  ><asp:Image SkinID="ReportDesignerItemDelete"         runat="server" /></a>
									<a href="#" onclick="return tRelationships_SelectedMoveUp();"  ><asp:Image SkinID="ReportDesignerItemMoveUp"         runat="server" /></a>
									<a href="#" onclick="return tRelationships_SelectedMoveDown();"><asp:Image SkinID="ReportDesignerItemMoveDown"       runat="server" /></a>
								</td>
							</tr>
						</tbody>
					</table>
					<table cellspacing="0" cellpadding="4" width="100%">
						<tbody id="tRelationships">
							<tr id="" class="listViewThS1">
								<td width="30%"><%# L10n.Term("ReportDesigner.LBL_LEFT_TABLE" ) %></td>
								<td width="10%"><%# L10n.Term("ReportDesigner.LBL_JOIN_TYPE"  ) %></td>
								<td width="30%"><%# L10n.Term("ReportDesigner.LBL_RIGHT_TABLE") %></td>
								<td width="30%"><%# L10n.Term("ReportDesigner.LBL_JOIN_FIELDS") %></td>
							</tr>
						</tbody>
					</table>
				</td>
			</tr>
			<tr>
				<td valign="top" style="height: 200px; border: 1px solid #cccccc;">
					<table cellspacing="0" cellpadding="4" width="100%">
						<tbody>
							<tr class="listViewThS1">
								<td width="20%"><%# L10n.Term("ReportDesigner.LBL_APPLIED_FILTERS") %></td>
								<td width="80%" align="right">
									<a href="#" onclick="return tAppliedFilters_AddFilter();"       ><asp:Image SkinID="ReportDesignerFilterCreate" runat="server" /></a>
									<a href="#" onclick="return tAppliedFilters_SelectedDelete();"  ><asp:Image SkinID="ReportDesignerItemDelete"   runat="server" /></a>
									<a href="#" onclick="return tAppliedFilters_SelectedMoveUp();"  ><asp:Image SkinID="ReportDesignerItemMoveUp"   runat="server" /></a>
									<a href="#" onclick="return tAppliedFilters_SelectedMoveDown();"><asp:Image SkinID="ReportDesignerItemMoveDown" runat="server" /></a>
								</td>
							</tr>
						</tbody>
					</table>
					<table cellspacing="0" cellpadding="4" width="100%">
						<tbody id="tAppliedFilters">
							<tr id="" class="listViewThS1">
								<td width="40%"><%# L10n.Term("ReportDesigner.LBL_FIELD_NAME") %></td>
								<td width="10%"><%# L10n.Term("ReportDesigner.LBL_OPERATOR"  ) %></td>
								<td width="40%"><%# L10n.Term("ReportDesigner.LBL_VALUE"     ) %></td>
								<td width="10%"><%# L10n.Term("ReportDesigner.LBL_PARAMETER" ) %></td>
							</tr>
						</tbody>
					</table>
				</td>
			</tr>
		</tbody>
	</table>

	<div id="divReportDesignerSQLError" class="error"></div>
	<br />
</div>

<asp:HiddenField ID="hidDESIGNER_JSON" runat="server" />

<%-- Document Ready --%>
<SplendidCRM:InlineScript runat="server">
	<script type="text/javascript">
var bDebug = '<%# bDebug %>';

function QueryDesigner_OnSubmit()
{
	var hidDESIGNER_JSON = document.getElementById('<%# hidDESIGNER_JSON.ClientID %>');
	hidDESIGNER_JSON.value = oReportDesign.Stringify();
	//alert('QueryDesigner_OnSubmit\n' + hidDESIGNER_JSON.value);
}

$(document).ready(function()
{
	try
	{
		// 06/24/2017 Paul.  We need a way to turn off bootstrap for BPMN, ReportDesigner and ChatDashboard. 
		bDESKTOP_LAYOUT   = true;
		// 04/03/2018 Paul.  Allow proxy to so mask https. 
		// 04/23/2018 Paul.  Build in javascript to allow proxy handling. 
		sREMOTE_SERVER    = window.location.protocol + "//" + window.location.hostname + (window.location.port ? ':' + window.location.port: '') + '<%# Sql.ToString(Application["rootURL"]) %>';
		// 06/29/2017 Paul.  AssemblyVersion is needed for HTML5 Dashboard. 
		sAssemblyVersion  = '<%# Sql.ToString(Application["SplendidVersion"]) %>';
		sIMAGE_SERVER     = sREMOTE_SERVER;
		sAUTHENTICATION   = '<%# Security.IsWindowsAuthentication() || Security.IsAuthenticated() ? "Windows" : "CRM" %>';
		bWINDOWS_AUTH     =  <%# Security.IsWindowsAuthentication() ? "true" : "false" %>;
		sUSER_LANG        = '<%# L10n.NAME          %>';
		sUSER_DATE_FORMAT = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["USER_SETTINGS/DATEFORMAT"])) %>';
		sUSER_TIME_FORMAT = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["USER_SETTINGS/TIMEFORMAT"])) %>';
		// 10/16/2016 Paul.  Remove offline ability by treating like mobile client. 
		// 04/30/2017 Paul.  Mobile Client must be treated separately. 
		bMOBILE_CLIENT    = false;
		bENABLE_OFFLINE   = false;
		
		<%= !this.Visible ? "return;" : String.Empty %>
		var bgPage = chrome.extension.getBackgroundPage();
		bgPage.IsAuthenticated(function(status, message)
		{
			try
			{
				if ( status == 1 )
				{
					ReportDesigner_InitUI(function(status, message)
					{
						if ( status == 1 )
						{
							SplendidError.SystemMessage('Loading modules.');
							ReportDesigner_LoadModules(function(status, message)
							{
								if ( status == 1 )
								{
									try
									{
										SplendidError.SystemMessage('Building module node tree.');
										var zNodes = zTree_BuildModuleNodes(message);
										var setting =
										{
											callback:
											{
												onCheck: zTree_onCheck
											}
											, check:
											{
												enable: true
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
										SplendidError.SystemMessage('');
										
										var hidDESIGNER_JSON = document.getElementById('<%# hidDESIGNER_JSON.ClientID %>');
										if ( !Sql.IsEmptyString(hidDESIGNER_JSON.value) )
										{
											if ( bDebug )
											{
												var divReportDesignerJSON = document.getElementById('divReportDesignerJSON');
												if ( divReportDesignerJSON != null )
													divReportDesignerJSON.innerHTML = dumpObj(jQuery.parseJSON(hidDESIGNER_JSON.value), null).replace(/\n/g, '<br>\n').replace(/\t/g, '&nbsp;&nbsp;&nbsp;');
											}
											
											oReportDesign = new ReportDesign();
											oReportDesign.Parse(hidDESIGNER_JSON.value);
											oReportDesign.PreviewSQL();
											
											var treeObj = $.fn.zTree.getZTreeObj("treeModules");
											for ( var i = 0; i < oReportDesign.SelectedFields.length; i++ )
											{
												var field = oReportDesign.SelectedFields[i];
												var sFieldName = field.FieldName;
												var node = treeObj.getNodeByParam('FieldName', sFieldName);
												if ( node != null )
													treeObj.checkNode(node, true, true, false);
											}
										}
									}
									catch(e)
									{
										SplendidError.SystemMessage('Modules: ' + e.message);
									}
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
			catch(e)
			{
				SplendidError.SystemError(e, 'QueryDesigner/default.aspx IsAuthenticated()');
			}
		});
	}
	catch(e)
	{
		SplendidError.SystemMessage(e.message);
	}
});
	</script>
</SplendidCRM:InlineScript>

