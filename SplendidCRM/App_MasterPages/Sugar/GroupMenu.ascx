<%@ Control Language="c#" AutoEventWireup="false" Codebehind="GroupMenu.ascx.cs" Inherits="SplendidCRM.Themes.Sugar.GroupMenu" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
var sSplendidMenuActiveGroupName = '';
function GroupMenuActivateTab(sNewGroupName)
{
	if ( sSplendidMenuActiveGroupName != sNewGroupName )
	{
		if ( sSplendidMenuActiveGroupName != '' )
		{
			var tdOldTabLeft   = document.getElementById('GroupMenu' + sSplendidMenuActiveGroupName + 'Left'  );
			var tdOldTabMiddle = document.getElementById('GroupMenu' + sSplendidMenuActiveGroupName + 'Middle');
			var tdOldTabRight  = document.getElementById('GroupMenu' + sSplendidMenuActiveGroupName + 'Right' );
			tdOldTabLeft.className   = 'otherTabLeft'   ;
			tdOldTabMiddle.className = 'otherTab'       ;
			tdOldTabRight.className  = 'otherTabRight'  ;
		}
		// 02/25/2010 Paul.  There is a blank SubMenu, so we will always hide something. 
		var tblOldSubMenu  = document.getElementById('SubMenu'   + sSplendidMenuActiveGroupName);
		tblOldSubMenu.style.display = 'none'  ;
		
		var tdNewTabLeft   = document.getElementById('GroupMenu' + sNewGroupName + 'Left'  );
		var tdNewTabMiddle = document.getElementById('GroupMenu' + sNewGroupName + 'Middle');
		var tdNewTabRight  = document.getElementById('GroupMenu' + sNewGroupName + 'Right' );
		var tblNewSubMenu  = document.getElementById('SubMenu'   + sNewGroupName);

		tdNewTabLeft.className   = 'currentTabLeft' ;
		tdNewTabMiddle.className = 'currentTab'     ;
		tdNewTabRight.className  = 'currentTabRight';
		tblNewSubMenu.style.display = 'inline';
		sSplendidMenuActiveGroupName = sNewGroupName;
	}
}
</script>
<%= Session["SplendidGroupMenuHtml"] %>
<script type='text/javascript'>GroupMenuActivateTab('<%= sActiveGroup %>');</script>

