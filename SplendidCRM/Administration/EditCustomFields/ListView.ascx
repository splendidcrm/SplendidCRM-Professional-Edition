<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.EditCustomFields.ListView" %>
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
function ExportSQL()
{
		return window.open('export.aspx?MODULE_NAME=<%= sMODULE_NAME %>','ExportSQL','width=1200,height=600,resizable=1,scrollbars=1');
}
</script>
<div id="divListView">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlModuleHeader" Module="EditCustomFields" Title="EditCustomFields.LBL_MODULE_TITLE" EnableModuleLabel="false" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchBasic" Src="SearchBasic.ascx" %>
	<SplendidCRM:SearchBasic ID="ctlSearch" Runat="Server" />
	<br />
	<%@ Register TagPrefix="SplendidCRM" Tagname="RestUtils" Src="~/_controls/RestUtils.ascx" %>
	<SplendidCRM:RestUtils Runat="Server" />

	<SplendidCRM:InlineScript runat="server">
		<script type="text/javascript">
function SplendidUI_ProgressBar(sActionsPanel)
{
	if ( !Sql.IsEmptyString(sActionsPanel) )
	{
		var divActionsPanel = document.getElementById(sActionsPanel);
		if ( divActionsPanel != null )
		{
			if ( divActionsPanel.childNodes != null )
			{
				while ( divActionsPanel.childNodes.length > 0 )
				{
					divActionsPanel.removeChild(divActionsPanel.firstChild);
				}
			}
			var divProgressBarFrame = document.createElement('div');
			divProgressBarFrame.id                    = 'divSplendidUI_ProgressBarFrame';
			divProgressBarFrame.style.margin          = '4px';
			divProgressBarFrame.style.padding         = '2px';
			divProgressBarFrame.style.border          = '1px solid #cccccc';
			//divProgressBarFrame.style.width           = '100%';
			divProgressBarFrame.style.backgroundColor = '#ffffff';
			divActionsPanel.appendChild(divProgressBarFrame);
			var divProgressStatusText = document.createElement('div');
			divProgressStatusText.id = 'divSplendidUI_ProgressStatusText';
			divProgressBarFrame.appendChild(divProgressStatusText);
			// 12/31/2014 Paul.  Firefox does not like innerText. Use createTextNode. 
			divProgressStatusText.innerHTML = '';
			var tblProgressBar = document.createElement('table');
			tblProgressBar.id                    = 'tblSplendidUI_ProgressBar';
			tblProgressBar.cellSpacing           = 0;
			tblProgressBar.style.width           = '100%';
			tblProgressBar.style.backgroundColor = '#000000';
			divProgressBarFrame.appendChild(tblProgressBar);
			var tbodyProgressBar = document.createElement('tbody');
			tbodyProgressBar.className = 'SplendidProgressBar';
			tblProgressBar.appendChild(tbodyProgressBar);
			var trProgressBar = document.createElement('tr');
			tbodyProgressBar.appendChild(trProgressBar);
			var tdProgressBar = document.createElement('td');
			tdProgressBar.align                = 'center';
			tdProgressBar.style.padding        = '2px';
			tdProgressBar.style.color          = '#ffffff';
			tdProgressBar.style.fontSize       = '12px';
			tdProgressBar.style.fontStyle      = 'normal';
			tdProgressBar.style.fontWeight     = 'normal';
			tdProgressBar.style.textDecoration = 'none';
			trProgressBar.appendChild(tdProgressBar);
			var divProgressBarText = document.createElement('div');
			divProgressBarText.id = 'divSplendidUI_ProgressBarText';
			tdProgressBar.appendChild(divProgressBarText);
			divProgressBarText.innerHTML = '0%';
		}
	}
}

function SplendidUI_UpdateProgressBar(nProgress, nTotal, sStatusText)
{
	if ( nTotal > 1 )
	{
		var nProgress = Math.round(100 * nProgress / nTotal);
		if ( nProgress >= 100 )
			nProgress = 99;
		
		var sProgress = nProgress.toString() + '%';
		var tblProgressBar        = document.getElementById('tblSplendidUI_ProgressBar'       );
		var divProgressBarText    = document.getElementById('divSplendidUI_ProgressBarText'   );
		var divProgressStatusText = document.getElementById('divSplendidUI_ProgressStatusText');
		divProgressBarText.innerHTML    = sProgress;
		tblProgressBar.style.width      = sProgress;
		divProgressStatusText.innerHTML = sStatusText;
	}
}

function UpdateStatus()
{
	var xhr = CreateSplendidRequest('Administration/Rest.svc/GetRecompileStatus', 'GET');
	xhr.onreadystatechange = function()
	{
		if ( xhr.readyState == 4 )
		{
			GetSplendidResult(xhr, function(result)
			{
				try
				{
					if ( result.status == 200 )
					{
						//console.log(dumpObj(result.d, ''));
						var oStatus = result.d;
						if ( oStatus != null )
						{
							var divProgressPanel = document.getElementById('divProgressPanel');
							divProgressPanel.style.display = '';
							var nMaximum = Sql.ToInteger(oStatus.TotalPasses) * Sql.ToInteger(oStatus.TotalViews);
							var nCurrent = Sql.ToInteger(oStatus.CurrentPass) * Sql.ToInteger(oStatus.TotalViews) + Sql.ToInteger(oStatus.CurrentView);
							var sStatus  = oStatus.StartDate + ', Pass ' + oStatus.CurrentPass + ' of ' + oStatus.TotalPasses + ', ' + oStatus.RemainingSeconds + ' seconds remaining, ' + oStatus.CurrentViewName + '. ';
							SplendidUI_UpdateProgressBar(nCurrent, nMaximum, sStatus);
							setTimeout(function()
							{
								UpdateStatus();
							}, 1000);
						}
						else
						{
							var divProgressBarFrame = document.getElementById('divSplendidUI_ProgressBarFrame');
							if ( divProgressBarFrame != null )
							{
								divProgressBarFrame.parentNode.removeChild(divProgressBarFrame);
							}
						}
					}
					else
					{
						if ( result.ExceptionDetail !== undefined )
							SplendidError.SystemMessage(result.ExceptionDetail.Message);
						else
							SplendidError.SystemMessage(xhr.responseText);
					}
				}
				catch(e)
				{
					SplendidError.SystemMessage(SplendidError.FormatError(e, 'CalendarView_GetCalendar'));
				}
			});
		}
	}
	try
	{
		xhr.send();
	}
	catch(e)
	{
		if ( e.number != -2146697208 )
			SplendidError.SystemMessage(SplendidError.FormatError(e, 'CalendarView_GetCalendar'));
	}
}

$(document).ready(function()
{
	SplendidUI_ProgressBar('divProgressPanel');
	SplendidUI_UpdateProgressBar(0, 100, 'Getting status');
	UpdateStatus();
});
		</script>
	</SplendidCRM:InlineScript>

	<div id="divProgressPanel" style="display: none;"></div>

	<asp:Table SkinID="tabFrame" CssClass="h3Row" runat="server">
		<asp:TableRow>
			<asp:TableCell Wrap="false">
				<h3><asp:Image SkinID="h3Arrow" Runat="server" />&nbsp;<asp:Label ID="Label1" Text='<%# L10n.Term("EditCustomFields.LBL_CUSTOM_FIELDS") %>' runat="server" /></h3>
			</asp:TableCell>
			<asp:TableCell HorizontalAlign="Right">
				<asp:Button ID="btnExport" Visible='<%# SplendidCRM.Security.AdminUserAccess(m_sMODULE, "export") >= 0 %>' UseSubmitBehavior="false" OnClientClick="ExportSQL(); return false;"    CssClass="button" Text='<%# L10n.Term(".LBL_EXPORT_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_EXPORT_BUTTON_TITLE"  ) %>' Runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
	
	<SplendidCRM:SplendidGrid id="grdMain" AllowPaging="false" AllowSorting="true" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn  HeaderText="EditCustomFields.COLUMN_TITLE_NAME" SortExpression="NAME" ItemStyle-Width="22%" ItemStyle-CssClass="listViewTdLinkS1">
				<ItemTemplate>
					<asp:HyperLink Enabled='<%# SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0 %>' Text='<%# DataBinder.Eval(Container.DataItem, "NAME") %>' NavigateUrl='<%# "edit.aspx?ID=" + DataBinder.Eval(Container.DataItem, "ID") %>' CssClass="listViewTdLinkS1" Runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn     HeaderText="EditCustomFields.COLUMN_TITLE_LABEL"           DataField="LABEL"           SortExpression="LABEL"           ItemStyle-Width="22%" />
			<asp:BoundColumn     HeaderText="EditCustomFields.COLUMN_TITLE_DATA_TYPE"       DataField="DATA_TYPE"       SortExpression="DATA_TYPE"       ItemStyle-Width="10%" />
			<asp:BoundColumn     HeaderText="EditCustomFields.COLUMN_TITLE_MAX_SIZE"        DataField="MAX_SIZE"        SortExpression="MAX_SIZE"        ItemStyle-Width="10%" />
			<asp:BoundColumn     HeaderText="EditCustomFields.COLUMN_TITLE_REQUIRED_OPTION" DataField="REQUIRED_OPTION" SortExpression="REQUIRED_OPTION" ItemStyle-Width="10%" />
			<asp:BoundColumn     HeaderText="EditCustomFields.COLUMN_TITLE_DEFAULT_VALUE"   DataField="DEFAULT_VALUE"   SortExpression="DEFAULT_VALUE"   ItemStyle-Width="10%" />
			<asp:BoundColumn     HeaderText="EditCustomFields.COLUMN_TITLE_DROPDOWN"        DataField="EXT1"            SortExpression="EXT1"            ItemStyle-Width="10%" />
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
				<ItemTemplate>
					<span onclick="return confirm('<%= L10n.TermJavaScript(".NTC_DELETE_CONFIRMATION") %>')">
						<asp:ImageButton Visible='<%# SplendidCRM.Security.AdminUserAccess(m_sMODULE, "delete") >= 0 %>' CommandName="EditCustomFields.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_DELETE") %>' SkinID="delete_inline" Runat="server" />
						<asp:LinkButton  Visible='<%# SplendidCRM.Security.AdminUserAccess(m_sMODULE, "delete") >= 0 %>' CommandName="EditCustomFields.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_DELETE") %>' Runat="server" />
					</span>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
	
	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>

