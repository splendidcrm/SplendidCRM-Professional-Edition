<%@ Page language="c#" Codebehind="manifest.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.OfficeAddin.Manifest" %>
<head visible="false" runat="server" /><?xml version="1.0" encoding="UTF-8"?>
<OfficeApp xmlns="http://schemas.microsoft.com/office/appforoffice/1.1" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:type="TaskPaneApp">
	<Id>A7B62EFD-1D1F-4785-90F9-E65E135F3688</Id>
	<Version><%# Application["SplendidVersion"] %></Version>
	<ProviderName>SplendidCRM Software, Inc.</ProviderName>
	<DefaultLocale>en-US</DefaultLocale>
	<DisplayName DefaultValue="SplendidCRM" />
	<Description DefaultValue="SplendidCRM for Office 365"/>
	<IconUrl DefaultValue="<%# m_sAddinRootPath %>Images/SplendidCRM_64x64.png" />
	<!-- https://dev.office.com/docs/add-ins/overview/specify-office-hosts-and-api-requirements -->
	<Hosts>
		<Host Name="Document"/>
		<Host Name="Workbook"/>
	</Hosts>
	<DefaultSettings>
		<SourceLocation DefaultValue="<%# m_sAddinRootPath %>default.aspx" />
	</DefaultSettings>
	<Permissions>ReadWriteDocument</Permissions>
</OfficeApp>
