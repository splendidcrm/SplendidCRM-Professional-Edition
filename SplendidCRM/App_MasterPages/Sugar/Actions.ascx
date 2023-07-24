<%@ Control Language="c#" AutoEventWireup="false" Codebehind="Actions.ascx.cs" Inherits="SplendidCRM.Themes.Sugar.Shortcuts" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
	private void Page_Load(object sender, System.EventArgs e)
	{
		// 12/04/2010 Paul.  A customer reported an exception with sSubMenu being empty. Rebind just in case. 
		divShortcuts.DataBind();
	}

	#region Web Form Designer generated code
	override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);
	}
	
	/// <summary>
	///		Required method for Designer support - do not modify
	///		the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
		this.Load += new System.EventHandler(this.Page_Load);
	}
	#endregion
</script>
<div id="divShortcuts" width="100%" class="lastView" visible='<%# SplendidCRM.Security.IsAuthenticated() && ((!Sql.IsEmptyString(sSubMenu) && SplendidCRM.Security.AdminUserAccess(sSubMenu, "access") >= 0) || !AdminShortcuts || Sql.IsEmptyString(sSubMenu)) %>' runat="server">
	<b><%= L10n.Term(".LBL_ACTIONS") %>:&nbsp;&nbsp;</b>
	<asp:Repeater id="ctlRepeater" DataSource='<%# SplendidCache.Shortcuts(Sql.ToString(Page.Items["ActiveTabMenu"])) %>' runat="server">
		<ItemTemplate>
			<%-- 09/26/2017 Paul.  Add Archive access right.  --%>
			<nobr Visible='<%# Sql.ToString(Eval("SHORTCUT_ACLTYPE")) != "archive" || Sql.ToBoolean(Application["Modules." + Sql.ToString(Eval("MODULE_NAME")) + ".ArchiveEnabled"]) %>' runat="server">
				<asp:HyperLink NavigateUrl='<%# Eval("RELATIVE_PATH") %>' 
					ToolTip='<%# L10n.Term(Sql.ToString(Eval("DISPLAY_NAME"))) %>' CssClass="lastViewLink" Runat="server">
					<img src="<%# Sql.ToString(Session["themeURL"]) + "images/" + Sql.ToString(Eval("IMAGE_NAME")) %>" border="0" width="16" height="16" align="absmiddle">
					&nbsp;<%# L10n.Term(Sql.ToString(Eval("DISPLAY_NAME"))) %></asp:HyperLink>&nbsp;
			</nobr>
		</ItemTemplate>
	</asp:Repeater>
</div>

