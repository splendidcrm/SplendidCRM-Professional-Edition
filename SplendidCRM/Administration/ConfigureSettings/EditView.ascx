<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditView.ascx.cs" Inherits="SplendidCRM.Administration.ConfigureSettings.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divEditView" runat="server">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true" EditView="true" Module="Administration" EnablePrint="false" HelpName="EditView" EnableHelp="true" Runat="Server" />

	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableHeaderCell ColumnSpan="4"><h4><asp:Label Text='<%# L10n.Term("Administration.LBL_NOTIFY_TITLE") %>' runat="server" /></h4></asp:TableHeaderCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><%= L10n.Term("Administration.LBL_NOTIFY_FROMNAME") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField">
							<asp:TextBox ID="txtNOTIFY_FROMNAME" TabIndex="1" size="25" MaxLength="128" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top"><%= L10n.Term("Administration.LBL_NOTIFY_ON") %></asp:TableCell>
						<asp:TableCell Width="40%" CssClass="dataField" VerticalAlign="top">
							<asp:CheckBox ID="chkNOTIFY_ON" TabIndex="1" CssClass="checkbox" Runat="server" />
							<em><%= L10n.Term("Administration.LBL_NOTIFICATION_ON_DESC") %></em>
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Administration.LBL_NOTIFY_FROMADDRESS") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField">
							<asp:TextBox ID="txtNOTIFY_FROMADDRESS" TabIndex="1" size="25" MaxLength="128" Runat="server" />
						</asp:TableCell>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Administration.LBL_NOTIFY_SEND_BY_DEFAULT") %></asp:TableCell>
						<asp:TableCell CssClass="dataField">
							<asp:CheckBox ID="chkNOTIFY_SEND_BY_DEFAULT" TabIndex="1" CssClass="checkbox" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Administration.LBL_MAIL_SENDTYPE") %></asp:TableCell>
						<asp:TableCell CssClass="dataField">
							<asp:DropDownList ID="lstMAIL_SENDTYPE" DataValueField="NAME" DataTextField="DISPLAY_NAME" TabIndex="1" onChange="notify_setrequired(document.ConfigureSettings);" Runat="server" />
						</asp:TableCell>
						<asp:TableCell CssClass="dataLabel"></asp:TableCell>
						<asp:TableCell CssClass="dataField"></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell ColumnSpan="4">
							<div ID="smtp_settings">
								<asp:Table SkinID="tabEditView" runat="server">
									<asp:TableRow>
										<asp:TableCell Width="15%" CssClass="dataLabel"><%= L10n.Term("Administration.LBL_MAIL_SMTPSERVER") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
										<asp:TableCell Width="35%" CssClass="dataField">
											<asp:TextBox ID="txtMAIL_SMTPSERVER" TabIndex="1" size="25" MaxLength="64" Runat="server" />
										</asp:TableCell>
										<asp:TableCell Width="15%" CssClass="dataLabel"><%= L10n.Term("Administration.LBL_MAIL_SMTPPORT") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
										<asp:TableCell Width="35%" CssClass="dataField">
											<asp:TextBox ID="txtMAIL_SMTPPORT" TabIndex="1" size="5" MaxLength="5" Runat="server" />
										</asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell Width="15%" CssClass="dataLabel"><%= L10n.Term("Administration.LBL_MAIL_SMTPAUTH_REQ") %></asp:TableCell>
										<asp:TableCell ColumnSpan="3">
											<asp:CheckBox ID="chkMAIL_SMTPAUTH_REQ" TabIndex="1" CssClass="checkbox" onclick="notify_setrequired(document.ConfigureSettings);" Runat="server" />
										</asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell ColumnSpan="4">
											<div ID="smtp_auth">
												<asp:Table SkinID="tabEditView" runat="server">
													<asp:TableRow>
														<asp:TableCell Width="15%" CssClass="dataLabel"><%= L10n.Term("Administration.LBL_MAIL_SMTPUSER") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
														<asp:TableCell Width="35%" CssClass="dataField">
															<asp:TextBox ID="txtMAIL_SMTPUSER" TabIndex="1" size="25" MaxLength="64" Runat="server" />
														</asp:TableCell>
														<asp:TableCell Width="15%" CssClass="dataLabel"><%= L10n.Term("Administration.LBL_MAIL_SMTPPASS") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
														<asp:TableCell Width="35%" CssClass="dataField">
															<asp:TextBox ID="txtMAIL_SMTPPASS" TextMode="Password" TabIndex="1" size="25" MaxLength="64" Runat="server" />
														</asp:TableCell>
													</asp:TableRow>
												</asp:Table>
											</div>
										</asp:TableCell>
									</asp:TableRow>
								</asp:Table>
							</div>
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableHeaderCell ColumnSpan="4"><h4><asp:Label Text='<%# L10n.Term("Administration.LBL_PORTAL_TITLE") %>' runat="server" /></h4></asp:TableHeaderCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="25%" CssClass="dataLabel" VerticalAlign="middle"><%= L10n.Term("Administration.LBL_PORTAL_ON") %></asp:TableCell>
						<asp:TableCell Width="75%" CssClass="dataField" VerticalAlign="middle">
							<asp:CheckBox ID="chkPORTAL_ON" TabIndex="1" CssClass="checkbox" Runat="server" />
							<em><%= L10n.Term("Administration.LBL_PORTAL_ON_DESC") %></em>
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell ColumnSpan="4">
							<div id="portal_config">
								<asp:Table SkinID="tabEditView" runat="server">
									<asp:TableRow>
										<asp:TableCell Width="15%" CssClass="dataLabel">&nbsp;</asp:TableCell>
										<asp:TableCell Width="35%" CssClass="dataField">&nbsp;</asp:TableCell>
									</asp:TableRow>
								</asp:Table>
							</div>
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlFooterButtons" Visible="<%# !SplendidDynamic.StackedLayout(this.Page.Theme) && !PrintView %>" ShowRequired="false" Runat="Server" />
</div>
