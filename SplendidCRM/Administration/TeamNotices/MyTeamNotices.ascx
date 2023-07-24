<%@ Control CodeBehind="MyTeamNotices.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.TeamNotices.MyTeamNotices" %>
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
<div id="divTeamNoticesMyTeamNotices">
	<br />
	<%@ Register TagPrefix="SplendidCRM" Tagname="DashletHeader" Src="~/_controls/DashletHeader.ascx" %>
	<SplendidCRM:DashletHeader ID="ctlDashletHeader" Title="Home.LBL_TEAM_NOTICES_TITLE" Runat="Server" />

	<marquee behavior="scroll" scrollamount="1" scrolldelay="100" direction="up" height="60" width="100%" class="monthBox">
		<div  style="margin: 4px">
			<asp:Repeater id="ctlRepeater" runat="server">
				<ItemTemplate>
					<b><%# DataBinder.Eval(Container.DataItem, "NAME") %></b><br />
					<%# DataBinder.Eval(Container.DataItem, "DESCRIPTION") %><br />
					<asp:HyperLink NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "URL") %>' Text='<%# DataBinder.Eval(Container.DataItem, "URL_TITLE") %>' Runat="server" /><br />
				</ItemTemplate>
			</asp:Repeater>
		</div>
	</marquee>
</div>
