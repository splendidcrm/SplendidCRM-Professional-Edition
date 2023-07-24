<%@ Control CodeBehind="MyTwitterTracks.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.TwitterTracks.MyTwitterTracks" %>
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
var sUSER_TWITTER_TRACKS = '<%= sUSER_TWITTER_TRACKS %>';
</script>

<div id="divTwitterTracksMyTwitterTracks">
	<%@ Register TagPrefix="SplendidCRM" Tagname="DashletHeader" Src="~/_controls/DashletHeader.ascx" %>
	<SplendidCRM:DashletHeader ID="ctlDashletHeader" Title="TwitterTracks.LBL_LIST_MY_TWITTER_TRACKS" DivEditName="my_twittertracks_edit" Runat="Server" />
	
	<div ID="my_twittertracks_edit" style="DISPLAY: <%= bShowEditDialog ? "inline" : "none" %>">
		<%@ Register TagPrefix="SplendidCRM" Tagname="SearchView" Src="~/_controls/SearchView.ascx" %>
		<SplendidCRM:SearchView ID="ctlSearchView" Module="TwitterTracks" SearchMode="SearchHome" IsDashlet="true" AutoSaveSearch="true" ShowSearchTabs="false" ShowSearchViews="false" ShowDuplicateSearch="false" Visible="<%# !PrintView %>" Runat="Server" />
	</div>
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
	
	<div id="divMyTwitterTracks" class="twitter-stream">
		<ol id="olMyTwitterTracks" class="twitter-stream-items"></ol>
	</div>
</div>
<br />

