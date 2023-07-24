<%@ Page language="c#" EnableTheming="false" Codebehind="default.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.OfficeAddin.Default" %>
<!--
/*
 * Copyright (C) 2005-2023 SplendidCRM Software, Inc. All rights reserved.
 *
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License 
 * Agreement, or other written agreement between you and SplendidCRM ("License"). By installing or 
 * using this file, you have unconditionally agreed to the terms and conditions of the License, 
 * including but not limited to restrictions on the number of users therein, and you may not use this 
 * file except in compliance with the License. 
 * 
 */
-->
<!DOCTYPE HTML>
<html id="htmlRoot" runat="server">
<head runat="server">
	<!-- 02/01/2018 Paul.  Prevent IE compatibility mode from disabling HTML5. -->
	<!-- 02/14/2018 Paul.  Moved to MetaHeader control so that it is used everywhere. -->
	<meta charset="utf-8">
	<meta http-equiv="X-UA-Compatible" content="IE=edge">
	<title><%# L10n.Term(".LBL_BROWSER_TITLE") %></title>

	<!-- // 07/01/2017 Paul.  Cannot combine font-awesome as it pevents automatic loading of font files.  -->
	<link type="text/css" rel="stylesheet" href="../html5/fonts/font-awesome.css" />
	<link id="lnkThemeStyle" type="text/css" rel="stylesheet" href="../html5/Themes/Six/style.css" />
	<!-- 01/24/2018 Paul.  Include version in url to ensure updates of combined files. -->
	<!-- 05/09/2018 Paul.  Use # instead of =. -->
	<link type="text/css" rel="stylesheet" href="StylesCombined<%# "_" + Sql.ToString(Application["SplendidVersion"]) %>" />
	<script type="text/javascript" src="ScriptsCombined<%#         "_" + Sql.ToString(Application["SplendidVersion"]) %>"></script>
	<script type="text/javascript" src="SplendidScriptsCombined<%# "_" + Sql.ToString(Application["SplendidVersion"]) %>"></script>
	<script type="text/javascript" src="SplendidUICombined<%#      "_" + Sql.ToString(Application["SplendidVersion"]) %>"></script>
	<script type="text/javascript" src="<%# Application["scriptURL"] %>ModulePopupScripts.aspx?LastModified=<%# Server.UrlEncode(Sql.ToString(Application["Modules.LastModified"])) + "&UserID=" + Security.USER_ID.ToString() %>"></script>

	<script type="text/javascript" src="Scripts/Office/MicrosoftAjax.js"></script>
	<script type="text/javascript" src="Scripts/Office/1.1/office.debug.js"   ></script>
	<script type="text/javascript" src="../html5/default.js"></script>
	<!-- 01/17/2018 Paul.  Add DATA_FORMAT to ListBox support multi-select CSV.  Combining causes javascript error. -->
	<script type="text/javascript" src="../html5/jQuery/multiple-select.js"></script>

	<%@ Register TagPrefix="SplendidCRM" Tagname="LoadSplendid" Src="LoadSplendid.ascx" %>
	<SplendidCRM:LoadSplendid ID="ctlLoadSplendid" Runat="Server" />
	<script type="text/javascript" src="OfficeAddinUI.js"></script>
	<script type="text/javascript" src="SplendidAddin.js"></script>
</head>
<body style="background-color: white;">
<div id="ctlAtlanticToolbar"></div>
<div id="ctlHeader"></div>
<div width="100%" style="background-color: White">
	<div id="ctlTabMenu"></div>
	
	<div style="padding-left: 10px; padding-right: 10px; padding-bottom: 5px;">
		<div id="divMainLayoutPanel_Header"></div>
		
		<div id="divMainActionsPanel"></div>
		
		<div id="divMainLayoutPanel"></div>
	</div>
</div>
</body>
</html>
