<%@ Control CodeBehind="FeedDetailView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Feeds.FeedDetailView" %>
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
	<asp:Table SkinID="tabFrame" CssClass="tabDetailView" runat="server">
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDF">
				<asp:Table SkinID="tabFrame" CssClass="mod" runat="server">
					<asp:TableRow>
						<asp:TableCell bgcolor="aaaaaa">
							<asp:Table Width="100%" BorderWidth="0" CellPadding="2" CellSpacing="0" runat="server">
								<asp:TableRow>
									<asp:TableCell CssClass="modtitle" width="98%">
										<asp:HyperLink Text='<%# sChannelTitle %>' NavigateUrl='<%# sChannelLink %>' Runat="server" />
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell>
							<asp:DataGrid id="grdMain" Width="100%" CellPadding="3" CellSpacing="0" border="0"
								AllowPaging="false" AllowSorting="false" AutoGenerateColumns="false" 
								ShowHeader="false" EnableViewState="false" runat="server">
								<Columns>
									<asp:TemplateColumn>
										<ItemTemplate>
											<asp:Table CellPadding="0" CellSpacing="2" runat="server">
												<asp:TableRow>
													<asp:TableCell CssClass="itemtitle">
														<asp:HyperLink Text='<%# DataBinder.Eval(Container.DataItem, "title") %>' NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "link") %>' Target="_new" Runat="server" />
													</asp:TableCell>
												</asp:TableRow>
												<asp:TableRow><asp:TableCell CssClass="itemdate"><%# DataBinder.Eval(Container.DataItem, "pubDate"    ) %></asp:TableCell></asp:TableRow>
												<asp:TableRow><asp:TableCell CssClass="itemdesc"><%# DataBinder.Eval(Container.DataItem, "description") %></asp:TableCell></asp:TableRow>
											</asp:Table>
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
							</asp:DataGrid>
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<br />
</div>

