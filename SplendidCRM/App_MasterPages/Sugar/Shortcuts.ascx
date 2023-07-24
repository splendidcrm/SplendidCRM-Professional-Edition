<%@ Control Language="c#" AutoEventWireup="false" Codebehind="Shortcuts.ascx.cs" Inherits="SplendidCRM.Themes.Sugar.Shortcuts" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Import Namespace="System.Data" %>
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
<div id="divShortcuts">
<p></p>
<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderLeft" Src="~/_controls/HeaderLeft.ascx" %>
<SplendidCRM:HeaderLeft ID="ctlHeaderLeft" runat="Server" Title=".LBL_SHORTCUTS" />


<div style="width: 180;">
	<ul class="subMenu">
<%
string sApplicationPath = Request.ApplicationPath;
if ( !sApplicationPath.EndsWith("/") )
	sApplicationPath += "/";
DataTable dt = SplendidCache.Shortcuts(sSubMenu);
if ( (SplendidCRM.Security.AdminUserAccess(sSubMenu, "access") >= 0) || !AdminShortcuts )
{
	foreach(DataRow row in dt.Rows)
	{
		// 09/26/2017 Paul.  Add Archive access right. 
		string sSHORTCUT_ACLTYPE = Sql.ToString(row["SHORTCUT_ACLTYPE"]);
		if ( sSHORTCUT_ACLTYPE == "archive" )
		{
			// 09/26/2017 Paul.  If the module does not have an archive table, then hide the link. 
			bool bArchiveEnabled = Sql.ToBoolean(Application["Modules." + Sql.ToString(row["MODULE_NAME"]) + ".ArchiveEnabled"]);
			if ( !bArchiveEnabled )
				continue;
		}
		string sRELATIVE_PATH = Sql.ToString(row["RELATIVE_PATH"]);
		string sIMAGE_NAME    = Sql.ToString(row["IMAGE_NAME"   ]);
		string sID            = Sql.ToString(row["DISPLAY_NAME" ]).Replace(" ", "_");
		string sDISPLAY_NAME  = L10n.Term(Sql.ToString(row["DISPLAY_NAME"]));
		if ( sRELATIVE_PATH.StartsWith("~/") )
			sRELATIVE_PATH = sRELATIVE_PATH.Replace("~/", sApplicationPath);
		%>
		<li><a href="<%= sRELATIVE_PATH %>"><img src="<%= Sql.ToString(Session["themeURL"]) + "images/" + sIMAGE_NAME %>" alt="<%= L10n.Term(sDISPLAY_NAME) %>" border="0" width="16" height="16" align="absmiddle">&nbsp;<%= L10n.Term(sDISPLAY_NAME) %></a></li>
		<%
	}
}
%>
	</ul>
</div>
<p></p>
<asp:Image SkinID="spacer" Height="1" Width="180" runat="server" /><br />
</div>

