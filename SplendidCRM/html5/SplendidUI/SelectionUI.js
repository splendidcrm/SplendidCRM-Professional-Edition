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

var arrSELECTED = new Object();

function SelectionUI_chkMain_Clicked(chk, sMODULE, sID, sNAME)
{
	if ( chk.checked )
	{
		//alert('SelectionUI_chkMain_Clicked Add ' + sMODULE + ' ' + sID + ' ' + sNAME);
		arrSELECTED[sID] = { 'Module': sMODULE, 'ID': sID, 'Name': sNAME };
	}
	else
	{
		//alert('SelectionUI_chkMain_Clicked Remove ' + sMODULE + ' ' + sID + ' ' + sNAME);
		if ( arrSELECTED[sID] != null )
			delete arrSELECTED[sID];
	}
	//alert(dumpObj(arrSELECTED, 'arrSELECTED'));
	SelectionUI_Render();
}

function SelectionUI_RemoveItem(sID)
{
	if ( arrSELECTED[sID] != null )
	{
		delete arrSELECTED[sID];
		SelectionUI_Render();
		var arrMain = document.getElementsByName('chkMain');
		for ( var i in arrMain )
		{
			var chkMain = arrMain[i];
			if ( typeof(chkMain) == 'object' && chkMain.tagName == 'INPUT' )
			{
				if ( chkMain.value == sID )
					chkMain.checked = false;
			}
		}
	}
}

// 09/04/2011 Paul.  RemoveAll is similar to clear, but also clears checkboxes. 
function SelectionUI_RemoveAll()
{
	var arrMain = document.getElementsByName('chkMain');
	for ( var i in arrMain )
	{
		var chkMain = arrMain[i];
		if ( typeof(chkMain) == 'object' && chkMain.tagName == 'INPUT' )
		{
			chkMain.checked = false;
		}
	}
	SelectionUI_Clear();
}

function SelectionUI_IsSelected(sID)
{
	return (arrSELECTED[sID] != null);
}

function SelectionUI_Render()
{
	var divSelection = document.getElementById('divSelection');
	if ( divSelection != null )
	{
		if ( divSelection.childNodes != null )
		{
			while ( divSelection.childNodes.length > 0 )
			{
				divSelection.removeChild(divSelection.firstChild);
			}
		}
		for ( var sID in arrSELECTED )
		{
			var oSelected = arrSELECTED[sID];
			var divItem = document.createElement('div');
			divSelection.appendChild(divItem);
			
			var sModuleLabel = L10n.ListTerm('moduleList', oSelected.Module);
			if ( Right(sModuleLabel, 3) == 'ies' && sModuleLabel.length > 3 )
				sModuleLabel = sModuleLabel.substring(0, sModuleLabel.length - 3) + 'y';
			else if ( Right(sModuleLabel, 1) == 's' )
				sModuleLabel = sModuleLabel.substring(0, sModuleLabel.length - 1);
			
			var aDelete = document.createElement('a');
			divItem.appendChild(aDelete);
			aDelete.href      = BindArguments(SelectionUI_RemoveItem, sID);
			
			var imgDelete = document.createElement('img');
			aDelete.appendChild(imgDelete);
			imgDelete.align             = 'absmiddle';
			imgDelete.style.height      = '16px';
			imgDelete.style.width       = '16px';
			imgDelete.style.borderWidth = '0px';
			imgDelete.src               = sIMAGE_SERVER + 'App_Themes/Six/images/delete_inline.gif';
			imgDelete.alt               = L10n.Term('.LBL_REMOVE');
			imgDelete.style.padding     = '1px';
			imgDelete.style.border      = 'none';

			var spnItem = document.createElement('span');
			divItem.appendChild(spnItem);
			spnItem.innerHTML = sModuleLabel + ': ' + oSelected.Name;
			spnItem.style.padding = '2px';
		}
	}
}

function SelectionUI_Clear()
{
	arrSELECTED = new Object();
	var divSelection = document.getElementById('divSelection');
	if ( divSelection != null && divSelection.childNodes != null )
	{
		while ( divSelection.childNodes.length > 0 )
		{
			divSelection.removeChild(divSelection.firstChild);
		}
	}
}

