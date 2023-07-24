<%@ Control Language="c#" AutoEventWireup="false" Codebehind="SixToolbar.ascx.cs" Inherits="SplendidCRM.Themes.Six.SixToolbar" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
<div id="divSixToolbar">
	<asp:UpdatePanel UpdateMode="Conditional" runat="server">
		<ContentTemplate>
			<table cellspacing="0" cellpadding="0" border="0" class="<%# L10n.IsLanguageRTL() ? "SixToolbarRTL" : "SixToolbar" %>">
				<tr>
					<td width="45px">
						<script runat="server">
						// 05/08/2010 Paul.  We need an image button at the top be a sink for the ENTER key.
						// This will prevent the first toolbar button from getting selected inadvertantly. 
						</script>
						<asp:ImageButton SkinID="blank" Width="0" Height="0" OnClientClick="return false;" runat="server" />
					</td>
					<td nowrap>
						<asp:PlaceHolder ID="plcSubPanel" runat="server" />
						<asp:HiddenField ID="hidDynamicNewRecord" Value="" runat="server" />
					</td>
					<td align="<%# L10n.IsLanguageRTL() ? "left" : "right" %>" valign="middle">
						<asp:Panel ID="cntUnifiedSearch" runat="server">
							<div id="divUnifiedSearch">
								<script type="text/javascript">
								function UnifiedSearch()
								{
									var frm = document.forms[0];
									// 01/21/2014 Paul.  Need to escape the query value to allow for symbols in the query. 
									var sUrl = '<%= Application["rootURL"] %>Home/UnifiedSearch.aspx?txtUnifiedSearch=' + escape(frm['<%= txtUnifiedSearch.ClientID %>'].value);
									window.location.href = sUrl;
									return false;
								}
								</script>
								<nobr>
								&nbsp;<asp:TextBox ID="txtUnifiedSearch" CssClass="searchField" size="30" Text='<%# Request["txtUnifiedSearch"] %>' runat="server" />
								<asp:ImageButton ID="btnUnifiedSearch" SkinID="searchButton" AlternateText='<%# L10n.Term(".LBL_SEARCH") %>' OnClientClick="return UnifiedSearch();" CssClass="searchButton" runat="server" />
								&nbsp;
								</nobr>
							</div>
						</asp:Panel>
					</td>
				</tr>
			</table>
			<div style="height: 45px; width: 100%"></div>
			<asp:PlaceHolder ID="plcDynamicNewRecords" runat="server" />
		</ContentTemplate>
	</asp:UpdatePanel>
</div>

