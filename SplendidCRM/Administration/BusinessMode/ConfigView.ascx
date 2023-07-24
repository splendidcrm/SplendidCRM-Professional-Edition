<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ConfigView.ascx.cs" Inherits="SplendidCRM.Administration.BusinessMode.ConfigView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true" EditView="true" Module="Administration" Title="Administration.LBL_BUSINESS_MODE_TITLE" EnableModuleLabel="false" EnablePrint="false" EnableHelp="true" Runat="Server" />
	
	<p></p>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="50%" VerticalAlign="Top">
				<asp:Label Text='<%# L10n.Term("Administration.LBL_BUSINESS_MODE_INSTRUCTIONS") %>' runat="server" /><br />
			</asp:TableCell>
			<asp:TableCell Width="50%" VerticalAlign="Top">
				<asp:Label Text='<%# L10n.Term("Administration.LBL_OPPORTUNITIES_MODE_INSTRUCTIONS") %>' runat="server" /><br />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell>
				<asp:RadioButton ID="radBUSINESS_MODE_B2B" GroupName="BusinessMode" Text='<%# L10n.Term("Administration.LBL_BUSINESS_MODE_B2B") %>' CssClass="radio" runat="server" /><br />
				<asp:RadioButton ID="radBUSINESS_MODE_B2C" GroupName="BusinessMode" Text='<%# L10n.Term("Administration.LBL_BUSINESS_MODE_B2C") %>' CssClass="radio" runat="server" /><br />
			</asp:TableCell>
			<asp:TableCell>
				<asp:RadioButton ID="radOPPORTUNITIES_MODE_OPPORTUNITIES" GroupName="OpportunitiesMode" Text='<%# L10n.Term("Administration.LBL_OPPORTUNITIES_MODE_OPPORTUNITIES") %>' CssClass="radio" runat="server" /><br />
				<asp:RadioButton ID="radOPPORTUNITIES_MODE_REVENUE"       GroupName="OpportunitiesMode" Text='<%# L10n.Term("Administration.LBL_OPPORTUNITIES_MODE_REVENUE"      ) %>' CssClass="radio" runat="server" /><br />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<p></p>
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlFooterButtons" Visible="<%# !SplendidDynamic.StackedLayout(this.Page.Theme) && !PrintView %>" ShowRequired="false" Runat="Server" />
</div>
