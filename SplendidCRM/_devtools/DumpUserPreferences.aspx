<%@ Page language="c#" Codebehind="DumpUserPreferences.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM._devtools.DumpUserPreferences" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.Common" %>
<%@ Import Namespace="System.Diagnostics" %>
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
<!DOCTYPE HTML>
<html>
<head runat="server">
	<title>DumpUserPreferences</title>
</head>
<body>
<%
// 01/11/2006 Paul.  Only a developer/administrator should see this. 
if ( SplendidCRM.Security.IS_ADMIN && Request.ServerVariables["SERVER_NAME"] == "localhost" )
{
	XmlDocument xml = new XmlDocument();
	try
	{
		xml.LoadXml(Sql.ToString(Session["USER_PREFERENCES"]));
		XmlUtil.Dump(xml);
		Response.Write("<pre>");
		string sPHP = XmlUtil.ConvertToPHP(xml.DocumentElement);
		foreach(char ch in sPHP.ToCharArray())
		{
			if ( ch == ';' )
			{
				Response.Write(";\r\n");
			}
			else if ( ch == '{' )
			{
				Response.Write("\r\n{\r\n");
			}
			else if ( ch == '}' )
			{
				Response.Write("}\r\n");
			}
			else
			{
				Response.Write(ch);
			}
		}
		Response.Write("</pre>");
	}
	catch
	{
	}
}
%>
</body>
</html>
