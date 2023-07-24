<%@ Control CodeBehind="AboutView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Home.AboutSugarCRM" %>
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
<span class="body">
	<div style="margin-top: 15px;"><b>SplendidCRM <%= Application["CONFIG.service_level"] %> Version <asp:Label ID="lblBuildNumber" Runat="server" /></b></div>

	<div>Copyright &copy; 2005 -2021 <asp:HyperLink NavigateUrl="http://www.splendidcrm.com" Target="_blank" CssClass="body" Text="SplendidCRM Software, Inc." runat="server" /> All Rights Reserved.</div>
	<asp:Label ID="lblLicense" runat="server" />
	
	<div style="margin-top: 15px;"><b>SplendidCRM Software, Inc.</b></div>
	<div>Web:     <asp:HyperLink NavigateUrl="http://www.splendidcrm.com" Target="_blank" CssClass="body" Text="http://www.splendidcrm.com" runat="server" /></div>
	<div>Sales:   <asp:HyperLink NavigateUrl="mailto:sales@splendidcrm.com"               CssClass="body" Text="sales@splendidcrm.com"      runat="server" /></div>
	<div>Support: <asp:HyperLink NavigateUrl="mailto:support@splendidcrm.com"             CssClass="body" Text="support@splendidcrm.com"    runat="server" /></div>
	
	<div style="margin-top: 15px;"><b>Open-Source libraries</b></div>
	<ul style="margin-top: 0px;">
		<li><a href="http://www.mimekit.net/" target="_blank">MimeKit</a> - MimeKit is a C# library which may be used for the creation and parsing of messages using the Multipurpose Internet Mail Extension (MIME).</li>
		<li><a href="http://bpmn.io/" target="_blank">bpmn.io</a> - Initiated by Camunda, the creators of Camunda BPM and Zalando, a German e-commerce champion, bpmn.io aims to bring business processes to everyone.</li>
		<li><a href="http://asternet.codeplex.com/" target="_blank">Aster.NET/Asterisk.NET</a> - Aster.NET library consists of a set of C# classes that allow you to easily build .NET applications that interact with an Asterisk PBX Server.</li>
		<li><a href="https://tweetinvi.codeplex.com/" target="_blank">Tweetinvi</a> - Tweetinvi is an intuitive .NET C# SDK that provides an easy and intuitive access to the Twitter REST and STREAM API 1.1.</li>
		<li><a href="http://www.twilio.com/docs/api/rest" target="_blank">Twilio REST API</a> - The Twilio REST API allows you to query meta-data about your account, phone numbers, calls, text messages, and recordings.</li>
		<li><a href="http://www.asp.net/signalr" target="_blank">ASP.NET SignalR</a> - ASP.NET SignalR is a new library for ASP.NET developers that makes developing real-time web functionality easy.</li>
		<li><a href="http://arshaw.com/fullcalendar/" target="_blank">FullCalendar</a> - FullCalendar is a jQuery plugin that provides a full-sized, drag &amp; drop calendar. </li>
		<li><a href="http://www.springframework.net/" target="_blank">Spring.NET</a> - Spring.NET is an open source application framework that makes building enterprise .NET applications easier. </li>
		<li><a href="http://ckeditor.com/" target="_blank">CKEditor</a> - CKEditor is a text editor to be used inside web pages. It's a WYSIWYG editor, which means that the text being edited on it looks as similar as possible to the results users have when publishing it.</li>
		<li><a href="http://www.jqplot.com/" target="_blank">jqPlot</a> - jqPlot is a plotting and charting plugin for the jQuery Javascript framework.</li>
		<li><a href="http://jqueryui.com/" target="_blank">jQuery UI</a> - jQuery UI provides abstractions for low-level interaction and animation, advanced effects and high-level, themeable widgets, built on top of the jQuery JavaScript Library, that you can use to build highly interactive web applications.</li>
		<li><a href="http://jquery.com/" target="_blank">jQuery</a> - jQuery is a fast and concise JavaScript Library that simplifies HTML document traversing, event handling, animating, and Ajax interactions for rapid web development.</li>
		<li><a href="http://getbootstrap.com/" target="_blank">Bootstrap</a> - Bootstrap is the most popular HTML, CSS, and JS framework for developing responsive, mobile first projects on the web.</li>
		<li><a href="http://datatables.net/" target="_blank">DataTables</a> - DataTables is a highly flexible tool, based upon the foundations of progressive enhancement, and will add advanced interaction controls to any HTML table.</li>
		<li><a href="http://imapnet.codeplex.com/" target="_blank">Koolwired.IMAP</a> - Koolwired.IMAP is a C# implementation of IMAP.</li>
		<li><a href="http://xamarin.com/" target="_blank">Mono</a> - Mono is an open source implementation of Microsoft's .NET Framework based on the ECMA standards for C# and the Common Language Runtime.</li>
	</ul>
</span>

