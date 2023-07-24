<%@ Control CodeBehind="TopicsView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Help.TopicsView" %>
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
<div id="divDetailView" runat="server">
	<asp:XmlDataSource ID="xdsViews" EnableCaching="false" runat="server" />
	<asp:TreeView ID="treeMain" ExpandDepth="1" ImageSet="XPFileExplorer" PopulateNodesFromClient="true" EnableClientScript="true" runat="server">
		<LeafNodeStyle     CssClass="leafStudioFolderLink"     />
		<ParentNodeStyle   CssClass="parentStudioFolderLink"   />
		<SelectedNodeStyle CssClass="selectedStudioFolderLink" />
		<NodeStyle         CssClass="nodeStudioFolderLink"     />
		<DataBindings>
			<asp:TreeNodeBinding DataMember="Modules" TextField="Name" Depth="0" SelectAction="None" />
			<asp:TreeNodeBinding DataMember="Module"  TextField="Name" Depth="1" SelectAction="SelectExpand" />
			<asp:TreeNodeBinding DataMember="View"    TextField="DisplayName" ValueField="Name" NavigateUrlField="URL"  />
		</DataBindings>
	</asp:TreeView>
	</div>
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />

