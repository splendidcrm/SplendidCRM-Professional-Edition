<%@ Page language="c#" Codebehind="manifest.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.OfficeAddin.Manifest" %>
<head visible="false" runat="server" /><?xml version="1.0" encoding="UTF-8"?>
<OfficeApp xmlns="http://schemas.microsoft.com/office/appforoffice/1.1" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:type="MailApp">
	<Id>E8EB3829-FA9D-455F-A59C-7A4951D24BCE</Id>
	<Version><%# Application["SplendidVersion"] %></Version>
	<ProviderName>SplendidCRM Software, Inc.</ProviderName>
	<DefaultLocale>en-US</DefaultLocale>
	<DisplayName DefaultValue="SplendidCRM" />
	<Description DefaultValue="SplendidCRM for Office 365"/>
	<IconUrl DefaultValue="<%# m_sAddinRootPath %>Images/SplendidCRM_64x64.png" />
	<Hosts>
		<Host Name="Mailbox" />
	</Hosts>
	<Requirements>
		<Sets>
			<Set Name="MailBox" MinVersion="1.1" />
		</Sets>
	</Requirements>
	<FormSettings>
		<Form xsi:type="ItemRead">
			<DesktopSettings>
				<SourceLocation DefaultValue="<%# m_sAddinRootPath %>default.aspx"/>
				<RequestedHeight>450</RequestedHeight>
			</DesktopSettings>
		</Form>
		<Form xsi:type="ItemEdit">
			<DesktopSettings>
				<SourceLocation DefaultValue="<%# m_sAddinRootPath %>default.aspx"/>
			</DesktopSettings>
		</Form>
	</FormSettings>
	<Permissions>ReadWriteItem</Permissions>
	<Rule xsi:type="RuleCollection" Mode="Or">
		<Rule xsi:type="ItemIs" ItemType="Message"     FormType="Edit" />
		<Rule xsi:type="ItemIs" ItemType="Appointment" FormType="Edit" />
		<Rule xsi:type="ItemIs" ItemType="Message"     FormType="Read" />
		<Rule xsi:type="ItemIs" ItemType="Appointment" FormType="Read" />
	</Rule>
	<DisableEntityHighlighting>false</DisableEntityHighlighting>
</OfficeApp>
