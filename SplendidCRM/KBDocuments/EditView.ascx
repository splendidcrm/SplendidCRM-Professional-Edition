<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditView.ascx.cs" Inherits="SplendidCRM.KBDocuments.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<script type="text/javascript">
var nAttachmentCount = 0;
var nImageCount = 0;
function AddAttachment()
{
	nAttachmentCount++;
	var attachments_div = document.getElementById('attachments_div');
	var fileAttachment = document.createElement('input');
	fileAttachment.setAttribute('type' , 'file'  );
	fileAttachment.setAttribute('size' , '40'    );
	fileAttachment.setAttribute('id'   , 'attachment' + nAttachmentCount);
	fileAttachment.setAttribute('name' , 'attachment' + nAttachmentCount);
	attachments_div.appendChild(fileAttachment);

	var nbsp = document.createTextNode('\u00A0');
	attachments_div.appendChild( nbsp );

	var btnRemove = document.createElement('input');
	btnRemove.setAttribute('id'     , 'remove_attachment' + nAttachmentCount);
	btnRemove.setAttribute('type'   , 'button');
	btnRemove.className = 'button';
	btnRemove.onclick   = Function('DeleteAttachment(' + nAttachmentCount + ');');
	btnRemove.setAttribute('value'  , '<%= Sql.EscapeJavaScript(L10n.Term(".LBL_REMOVE")) %>');
	attachments_div.appendChild(btnRemove);

	var br = document.createElement('br');
	attachments_div.appendChild(br);
}
function AddImage()
{
	nImageCount++;
	var images_div = document.getElementById('images_div');
	var fileAttachment = document.createElement('input');
	fileAttachment.setAttribute('type' , 'file'  );
	fileAttachment.setAttribute('size' , '40'    );
	fileAttachment.setAttribute('id'   , 'image' + nImageCount);
	fileAttachment.setAttribute('name' , 'image' + nImageCount);
	images_div.appendChild(fileAttachment);

	var nbsp = document.createTextNode('\u00A0');
	images_div.appendChild( nbsp );

	var btnRemove = document.createElement('input');
	btnRemove.setAttribute('id'     , 'remove_image' + nImageCount);
	btnRemove.setAttribute('type'   , 'button');
	btnRemove.className = 'button';
	btnRemove.onclick   = Function('DeleteImage(' + nImageCount + ');');
	btnRemove.setAttribute('value'  , '<%= Sql.EscapeJavaScript(L10n.Term(".LBL_REMOVE")) %>');
	images_div.appendChild(btnRemove);

	var br = document.createElement('br');
	images_div.appendChild(br);
}
function DeleteAttachment(index)
{
	var attachment        = document.getElementById('attachment' + index);
	var remove_attachment = document.getElementById('remove_attachment' + index);
	if ( attachment != null )
	{
		var nbsp = attachment.nextSibling;
		if ( nbsp != null )
		{
			nbsp.parentNode.removeChild(nbsp);
		}
		attachment.parentNode.removeChild(attachment);
	}
	if ( remove_attachment != null )
	{
		var br = remove_attachment.nextSibling;
		if ( br != null )
		{
			br.parentNode.removeChild(br);
		}
		remove_attachment.parentNode.removeChild(remove_attachment);
	}
	var attachments_div = document.getElementById('attachments_div');
	if ( attachments_div.childNodes.length == 0 )
		AddAttachment();
}
function DeleteImage(index)
{
	var image        = document.getElementById('image' + index);
	var remove_image = document.getElementById('remove_image' + index);
	if ( image != null )
	{
		var nbsp = image.nextSibling;
		if ( nbsp != null )
		{
			nbsp.parentNode.removeChild(nbsp);
		}
		image.parentNode.removeChild(image);
	}
	if ( remove_image != null )
	{
		var br = remove_image.nextSibling;
		if ( br != null )
		{
			br.parentNode.removeChild(br);
		}
		remove_image.parentNode.removeChild(remove_image);
	}
	var images_div = document.getElementById('images_div');
	if ( images_div.childNodes.length == 0 )
		AddImage();
}
</script>
<div id="divEditView" runat="server">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true" EditView="true" Module="KBDocuments" EnablePrint="false" HelpName="EditView" EnableHelp="true" Runat="Server" />

	<asp:HiddenField ID="LAYOUT_EDIT_VIEW" Runat="server" />
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<table ID="tblMain" class="tabEditView" runat="server">
				</table>
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableCell class="dataLabel" VerticalAlign="Top"><%= L10n.Term("KBDocuments.LBL_ATTACHMENTS") %></asp:TableCell>
						<asp:TableCell class="dataField" VerticalAlign="Top" HorizontalAlign="Left">
							<asp:Repeater id="ctlAttachments" runat="server">
								<HeaderTemplate />
								<ItemTemplate>
									<asp:HyperLink Text='<%# DataBinder.Eval(Container.DataItem, "FILENAME") %>' NavigateUrl='<%# "~/KBDocuments/Attachment.aspx?ID=" + Eval("ID") %>' Target="_blank" Runat="server" />
									&nbsp;<asp:ImageButton OnCommand="Page_Command" CommandName="Attachments.Delete" CommandArgument='<%# Eval("ID") %>' ImageUrl='<%# Session["themeURL"] + "images/delete_inline.gif"  %>' ImageAlign="Middle" runat="server" /><br />
								</ItemTemplate>
								<FooterTemplate />
							</asp:Repeater>
							<div id="attachments_div"></div>
							<div style="display: none"><input id="dummy_attachment" type="file" tabindex="0" size="40" runat="server" /></div>
							<input type="button" CssClass="button" onclick="AddAttachment();" value="<%= L10n.Term("KBDocuments.LBL_ADD_FILE") %>" />
						</asp:TableCell>
						<asp:TableCell class="dataLabel" VerticalAlign="Top"><%= L10n.Term("KBDocuments.LBL_IMAGES") %></asp:TableCell>
						<asp:TableCell class="dataField" VerticalAlign="Top" HorizontalAlign="Left">
							<asp:Repeater id="ctlImages" runat="server">
								<HeaderTemplate />
								<ItemTemplate>
									<asp:HyperLink Text='<%# DataBinder.Eval(Container.DataItem, "FILENAME") %>' NavigateUrl='<%# "~/KBDocuments/Image.aspx?ID=" + Eval("ID") %>' Target="_blank" Runat="server" />
									&nbsp;<asp:ImageButton OnCommand="Page_Command" CommandName="Images.Delete" CommandArgument='<%# Eval("ID") %>' ImageUrl='<%# Session["themeURL"] + "images/delete_inline.gif"  %>' ImageAlign="Middle" runat="server" /><br />
								</ItemTemplate>
								<FooterTemplate />
							</asp:Repeater>
							<div id="images_div"></div>
							<input type="button" CssClass="button" onclick="AddImage();" value="<%= L10n.Term("KBDocuments.LBL_ADD_FILE") %>" />
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

<script type="text/javascript">
AddAttachment();
AddImage();
</script>
