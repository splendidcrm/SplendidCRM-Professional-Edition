<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MergeView.ascx.cs" Inherits="SplendidCRM.Import.MergeView" %>
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
<div id="divMergeView">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true" EnableModuleLabel="false" EnablePrint="true" HelpName="MergeView" EnableHelp="true" Runat="Server" />
	
	<script type="text/javascript">
	function CopyTextField(idDest, idSrc)
	{
		try
		{
			//alert('CopyTextField(' + idDest + ',' + idSrc + ')');
			var fldDest = document.getElementById(idDest);
			var fldSrc  = document.getElementById(idSrc );
			if ( fldDest == null ) alert('Could not find destiation field: ' + idDest);
			if ( fldSrc  == null ) alert('Could not find source field: ' + idSrc);
			if ( fldDest != null && fldSrc != null )
			{
				fldDest.value = fldSrc.innerHTML;
			}
		}
		catch(e)
		{
			alert(e);
		}
		return false;
	}
	function CopyHtmlField(idDest, idSrc)
	{
		try
		{
			//alert('CopyHtmlField(' + idDest + ',' + idSrc + ')');
			// 09/18/2011 Paul.  Upgrade to CKEditor 3.6.2. 
			var fldDest = CKEDITOR.instances[idDest];
			var fldSrc  = document.getElementById(idSrc );
			if ( fldDest == null ) alert('Could not find destiation field: ' + idDest);
			if ( fldSrc  == null ) alert('Could not find source field: ' + idSrc);
			if ( fldDest != null && fldSrc != null )
			{
				if ( fldDest.EditMode == 0 )
					fldDest.SetHTML(fldSrc.innerHTML);
				else
					fldDest.SetData(fldSrc.innerHTML);
			}
		}
		catch(e)
		{
			alert(e);
		}
		return false;
	}
	function CopyInputField(idDest, idSrc)
	{
		try
		{
			//alert('CopyTextField(' + idDest + ',' + idSrc + ')');
			var fldDest = document.getElementById(idDest);
			var fldSrc  = document.getElementById(idSrc );
			if ( fldDest == null ) alert('Could not find destiation field: ' + idDest);
			if ( fldSrc  == null ) alert('Could not find source field: ' + idSrc);
			if ( fldDest != null && fldSrc != null )
			{
				fldDest.value = fldSrc.value;
			}
		}
		catch(e)
		{
			alert(e);
		}
		return false;
	}
	function CopyListField(idDest, idSrc)
	{
		try
		{
			//alert('CopyListField(' + idDest + ',' + idSrc + ')');
			var fldDest = document.getElementById(idDest);
			var fldSrc  = document.getElementById(idSrc );
			if ( fldDest == null ) alert('Could not find destiation field: ' + idDest);
			if ( fldSrc  == null ) alert('Could not find source field: ' + idSrc);
			for ( i=0; i < fldDest.options.length; i++ )
			{
				if ( fldDest.options[i].value == fldSrc.innerHTML )
				{
					fldDest.options.selectedIndex = i;
					break;
				}
			}
		}
		catch(e)
		{
			alert(e);
		}
		return false;
	}
	function SetListFields(idDest, arrValues)
	{
		try
		{
			//alert('SetListFields(' + idDest + ',' + idSrc + ')');
			var fldDest = document.getElementById(idDest);
			if ( fldDest == null ) alert('Could not find destiation field: ' + idDest);
			for ( i=0; i < fldDest.options.length; i++ )
			{
				fldDest.options[i].selected = false;
				for ( j=0; j < arrValues.length; j++ )
				{
					if ( arrValues[j] == fldDest.options[i].value )
						fldDest.options[i].selected = true;
				}
			}
		}
		catch(e)
		{
			alert(e);
		}
		return false;
	}
	function SetCheckBoxListFields(idDest, arrValues)
	{
		try
		{
			//alert('SetCheckBoxListFields(' + idDest + ')');
			for ( i=0; ; i++ )
			{
				var fldDest = document.getElementById(idDest + '_' + i);
				if ( fldDest == null )
					break;
				fldDest.checked = false;
				for ( j=0; j < arrValues.length; j++ )
				{
					if ( arrValues[j] == fldDest.nextSibling.innerHTML )
						fldDest.checked = true;
				}
			}
		}
		catch(e)
		{
			alert(e);
		}
		return false;
	}
	function SetRadioFields(idDest, sValue)
	{
		try
		{
			//alert('SetRadioFields(' + idDest + ')');
			var fldDest = document.getElementsByName(idDest);
			if ( fldDest == null ) alert('Could not find destiation field: ' + idDest);
			for ( i=0; i < fldDest.length; i++ )
			{
				fldDest[i].checked = false;
				if ( sValue == fldDest[i].value )
					fldDest[i].checked = true;
			}
		}
		catch(e)
		{
			alert(e);
		}
		return false;
	}
	function CopyCheckboxField(idDest, idSrc)
	{
		try
		{
			var fldDest = document.getElementById(idDest);
			var fldSrc  = document.getElementById(idSrc );
			if ( fldDest == null ) alert('Could not find destiation field: ' + idDest);
			if ( fldSrc  == null ) alert('Could not find source field: ' + idSrc);
			if ( fldDest != null && fldSrc != null )
			{
				fldDest.checked = (fldSrc.innerHTML == 'true');
			}
		}
		catch(e)
		{
			alert(e);
		}
		return false;
	}
	function SetPrimaryRecord(id)
	{
		var fldPrimaryRecord = document.getElementById('<%= hidPrimaryRecord.ClientID %>');
		var fldSetPrimary    = document.getElementById('<%= btnSetPrimary.ClientID %>');
		fldPrimaryRecord.value = id;
		fldSetPrimary.click();
		return false;
	}
	function RemoveRecord(id)
	{
		var fldRemoveRecord = document.getElementById('<%= hidRemoveRecord.ClientID %>');
		var fldRemove       = document.getElementById('<%= btnRemove.ClientID %>');
		fldRemoveRecord.value = id;
		fldRemove.click();
		return false;
	}
	</script>

	<asp:Button ID="btnSetPrimary" Text="SetPrimary" CommandName="SetPrimary" OnCommand="Page_Command" style="display:none" runat="server" />
	<asp:Button ID="btnRemove"     Text="Remove"     CommandName="Remove"     OnCommand="Page_Command" style="display:none" runat="server" />
	<asp:HiddenField ID="hidRecords"         runat="server" />
	<asp:HiddenField ID="hidPrimaryRecord"   runat="server" />
	<asp:HiddenField ID="hidRemoveRecord"    EnableViewState="false" runat="server" />
	<asp:HiddenField ID="hidRecordCount"     runat="server" />
	<asp:HiddenField ID="hidDifferentFields" runat="server" />
	<asp:HiddenField ID="hidSimilarFields"   runat="server" />
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<div><asp:Label Text='<%# L10n.Term("Merge.LBL_DIFF_COL_VALUES") %>' Font-Bold="true" runat="server" /></div>
				<table ID="tblMain" class="tabEditView" runat="server">
				</table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<div><asp:Label Text='<%# L10n.Term("Merge.LBL_SAME_COL_VALUES") %>' Font-Bold="true" runat="server" /></div>
				<table ID="tblSimilar" class="tabEditView" runat="server">
				</table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

</div>

