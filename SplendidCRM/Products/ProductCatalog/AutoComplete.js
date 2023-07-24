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

// AutoComplete.js

// 05/18/2011 Paul.  .NET 4.0 appends a row index to the field IDs. 
var sItemNameUserContext = '';
var sItemNameUserSuffix  = '';
function NamePrefix(s, sSeparator)
{
	var nSeparatorIndex = s.lastIndexOf(sSeparator);
	if ( nSeparatorIndex > 0 )
	{
		return s.substring(0, nSeparatorIndex);
	}
}

function NameSuffix(s, sSeparator)
{
	var nSeparatorIndex = s.lastIndexOf(sSeparator);
	if ( nSeparatorIndex > 0 )
	{
		return s.substring(nSeparatorIndex + sSeparator.length, s.length);
	}
	return '';
}

// 03/07/2008 Paul.  Use Microsoft AJAX number localization functions to format currency. 

function GetCurrencyID(sCURRENCY_ID)
{
	// 03/15/2007 Paul.  The Price Level is required to lookup the item price.
	// 08/03/2010 Paul.  We can't send null or an empty string as it will generate a Guid exception. 
	var sCURRENCY = '00000000-0000-0000-0000-000000000000';
	var fldCURRENCY = document.getElementById(sCURRENCY_ID);
	if ( fldCURRENCY != null )
		sCURRENCY = fldCURRENCY.options[fldCURRENCY.selectedIndex].value;
	return sCURRENCY;
}

function ItemNameChanged(sCURRENCY_ID, fldNAME)
{
	var fldAjaxErrors = document.getElementById('AjaxErrors');
	fldAjaxErrors.innerHTML = '';
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	// 05/18/2011 Paul.  .NET 4.0 appends a row index to the field IDs. 
	var userContext = NamePrefix(fldNAME.id, 'NAME');
	var userSuffix  = NameSuffix(fldNAME.id, 'NAME');
	
	var fldPREVIOUS_NAME = document.getElementById(userContext + 'PREVIOUS_NAME' + userSuffix);
	if ( fldPREVIOUS_NAME.value != fldNAME.value )
	{
		if ( fldNAME.value.length > 0 )
		{
			try
			{
				sItemNameUserContext = userContext;
				sItemNameUserSuffix  = userSuffix ;
				SplendidCRM.Products.ProductCatalog.AutoComplete.GetItemDetailsByName(GetCurrencyID(sCURRENCY_ID), fldNAME.value, ItemChanged_OnSucceededWithContext, ItemChanged_OnFailed, userContext);
			}
			catch(e)
			{
				alert('ItemNameChanged: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var fldPRODUCT_TEMPLATE_ID   = document.getElementById(userContext + 'PRODUCT_TEMPLATE_ID'   + userSuffix);
			var fldPARENT_TEMPLATE_ID    = document.getElementById(userContext + 'PARENT_TEMPLATE_ID'    + userSuffix);
			var fldPREVIOUS_NAME         = document.getElementById(userContext + 'PREVIOUS_NAME'         + userSuffix);
			var fldPREVIOUS_MFT_PART_NUM = document.getElementById(userContext + 'PREVIOUS_MFT_PART_NUM' + userSuffix);
			if ( fldPRODUCT_TEMPLATE_ID   != null ) fldPRODUCT_TEMPLATE_ID.value   = '';
			if ( fldPARENT_TEMPLATE_ID    != null ) fldPARENT_TEMPLATE_ID.value    = '';
			if ( fldPREVIOUS_NAME         != null ) fldPREVIOUS_NAME.value         = '';
			if ( fldPREVIOUS_MFT_PART_NUM != null ) fldPREVIOUS_MFT_PART_NUM.value = '';
		}
	}
}

function ItemPartNumberChanged(sCURRENCY_ID, fldMFT_PART_NUM)
{
	var fldAjaxErrors = document.getElementById('AjaxErrors');
	fldAjaxErrors.innerHTML = '';
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	// 05/18/2011 Paul.  .NET 4.0 appends a row index to the field IDs. 
	var userContext = NamePrefix(fldMFT_PART_NUM.id, 'MFT_PART_NUM');
	var userSuffix  = NameSuffix(fldMFT_PART_NUM.id, 'MFT_PART_NUM');

	var fldLastLineItemPartNumber = document.getElementById(userContext + 'PREVIOUS_MFT_PART_NUM' + userSuffix);
	if ( fldLastLineItemPartNumber.value != fldMFT_PART_NUM.value )
	{
		if ( fldMFT_PART_NUM.value.length > 0 )
		{
			try
			{
				sItemNameUserContext = userContext;
				sItemNameUserSuffix  = userSuffix ;
				SplendidCRM.Products.ProductCatalog.AutoComplete.GetItemDetailsByNumber(GetCurrencyID(sCURRENCY_ID), fldMFT_PART_NUM.value, ItemChanged_OnSucceededWithContext, ItemChanged_OnFailed, userContext);
			}
			catch(e)
			{
				alert('ItemNameChanged: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var fldPRODUCT_TEMPLATE_ID   = document.getElementById(userContext + 'PRODUCT_TEMPLATE_ID'   + userSuffix);
			var fldPARENT_TEMPLATE_ID    = document.getElementById(userContext + 'PARENT_TEMPLATE_ID'    + userSuffix);
			var fldPREVIOUS_NAME         = document.getElementById(userContext + 'PREVIOUS_NAME'         + userSuffix);
			var fldPREVIOUS_MFT_PART_NUM = document.getElementById(userContext + 'PREVIOUS_MFT_PART_NUM' + userSuffix);
			if ( fldPRODUCT_TEMPLATE_ID   != null ) fldPRODUCT_TEMPLATE_ID.value   = '';
			if ( fldPARENT_TEMPLATE_ID    != null ) fldPARENT_TEMPLATE_ID.value    = '';
			if ( fldPREVIOUS_NAME         != null ) fldPREVIOUS_NAME.value         = '';
			if ( fldPREVIOUS_MFT_PART_NUM != null ) fldPREVIOUS_MFT_PART_NUM.value = '';
		}
	}
}

function ItemChanged_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID              = result.ID             ;
		var sNAME            = result.NAME           ;
		var sMFT_PART_NUM    = result.MFT_PART_NUM   ;
		var sVENDOR_PART_NUM = result.VENDOR_PART_NUM;
		var sTAX_CLASS       = result.TAX_CLASS      ;
		// 12/16/2013 Paul.  Allow each product to have a default tax rate. 
		var sTAXRATE_ID      = result.TAXRATE_ID     ;
		var dCOST_PRICE      = result.COST_PRICE     ;
		var dCOST_USDOLLAR   = result.COST_USDOLLAR  ;
		var dLIST_PRICE      = result.LIST_PRICE     ;
		var dLIST_USDOLLAR   = result.LIST_USDOLLAR  ;
		var dUNIT_PRICE      = result.UNIT_PRICE     ;
		var dUNIT_USDOLLAR   = result.UNIT_USDOLLAR  ;
		// 05/13/2009 Paul.  Carry forward the description from product to quote, order and invoice. 
		var sDESCRIPTION     = result.DESCRIPTION    ;
		
		var fldAjaxErrors          = document.getElementById('AjaxErrors');
		var fldPRODUCT_TEMPLATE_ID = document.getElementById(userContext + 'PRODUCT_TEMPLATE_ID' + sItemNameUserSuffix);
		// 07/11/2010 Paul.  Add PARENT_TEMPLATE_ID. 
		var fldPARENT_TEMPLATE_ID  = document.getElementById(userContext + 'PARENT_TEMPLATE_ID'  + sItemNameUserSuffix);
		var fldNAME                = document.getElementById(userContext + 'NAME'                + sItemNameUserSuffix);
		var fldMFT_PART_NUM        = document.getElementById(userContext + 'MFT_PART_NUM'        + sItemNameUserSuffix);
		var fldVENDOR_PART_NUM     = document.getElementById(userContext + 'VENDOR_PART_NUM'     + sItemNameUserSuffix);
		var fldTAX_CLASS           = document.getElementById(userContext + 'TAX_CLASS'           + sItemNameUserSuffix);
		// 12/16/2013 Paul.  Allow each product to have a default tax rate. 
		var fldTAXRATE_ID          = document.getElementById(userContext + 'lstTAXRATE_ID'       + sItemNameUserSuffix);
		var fldCOST_PRICE          = document.getElementById(userContext + 'COST_PRICE'          + sItemNameUserSuffix);
		var fldCOST_USDOLLAR       = document.getElementById(userContext + 'COST_USDOLLAR'       + sItemNameUserSuffix);
		var fldLIST_PRICE          = document.getElementById(userContext + 'LIST_PRICE'          + sItemNameUserSuffix);
		var fldLIST_USDOLLAR       = document.getElementById(userContext + 'LIST_USDOLLAR'       + sItemNameUserSuffix);
		var fldUNIT_PRICE          = document.getElementById(userContext + 'UNIT_PRICE'          + sItemNameUserSuffix);
		var fldUNIT_USDOLLAR       = document.getElementById(userContext + 'UNIT_USDOLLAR'       + sItemNameUserSuffix);
		var fldDESCRIPTION         = document.getElementById(userContext + 'DESCRIPTION'         + sItemNameUserSuffix);
		if ( fldPRODUCT_TEMPLATE_ID != null ) fldPRODUCT_TEMPLATE_ID.value = sID             ;
		// 07/11/2010 Paul.  Always clear the Parent Template ID as it cannot be managed using AJAX. 
		if ( fldPARENT_TEMPLATE_ID  != null ) fldPARENT_TEMPLATE_ID.value  = ''              ;
		if ( fldNAME                != null ) fldNAME.value                = sNAME           ;
		if ( fldMFT_PART_NUM        != null ) fldMFT_PART_NUM.value        = sMFT_PART_NUM   ;
		if ( fldVENDOR_PART_NUM     != null ) fldVENDOR_PART_NUM.value     = sVENDOR_PART_NUM;
		if ( fldCOST_PRICE          != null ) fldCOST_PRICE.value          = dCOST_PRICE.localeFormat('c');
		if ( fldCOST_USDOLLAR       != null ) fldCOST_USDOLLAR.value       = dCOST_USDOLLAR  ;
		if ( fldLIST_PRICE          != null ) fldLIST_PRICE.value          = dLIST_PRICE.localeFormat('c');
		if ( fldLIST_USDOLLAR       != null ) fldLIST_USDOLLAR.value       = dLIST_USDOLLAR  ;
		if ( fldUNIT_PRICE          != null ) fldUNIT_PRICE.value          = dUNIT_PRICE.localeFormat('c');
		if ( fldUNIT_USDOLLAR       != null ) fldUNIT_USDOLLAR.value       = dUNIT_USDOLLAR  ;
		// 05/13/2009 Paul.  Carry forward the description from product to quote, order and invoice. 
		if ( fldDESCRIPTION         != null ) fldDESCRIPTION.value         = sDESCRIPTION    ;
		if ( fldTAX_CLASS           != null )
		{
			var lst = fldTAX_CLASS;
			if ( lst.options != null )
			{
				for ( i=0; i < lst.options.length ; i++ )
				{
					if ( lst.options[i].value == sTAX_CLASS )
					{
						lst.options[i].selected = true;
						break;
					}
				}
			}
		}
		// 12/16/2013 Paul.  Allow each product to have a default tax rate. 
		if ( fldTAXRATE_ID          != null )
		{
			var lst = fldTAXRATE_ID;
			if ( lst.options != null )
			{
				for ( i=0; i < lst.options.length ; i++ )
				{
					if ( lst.options[i].value == sTAXRATE_ID )
					{
						lst.options[i].selected = true;
						break;
					}
				}
			}
		}

		var fldQUANTITY               = document.getElementById(userContext + 'QUANTITY'              + sItemNameUserSuffix);
		var fldEXTENDED_PRICE         = document.getElementById(userContext + 'EXTENDED_PRICE'        + sItemNameUserSuffix);
		var fldEXTENDED_USDOLLAR      = document.getElementById(userContext + 'EXTENDED_USDOLLAR'     + sItemNameUserSuffix);
		var fldPREVIOUS_MFT_PART_NUM  = document.getElementById(userContext + 'PREVIOUS_MFT_PART_NUM' + sItemNameUserSuffix);
		var fldPREVIOUS_NAME          = document.getElementById(userContext + 'PREVIOUS_NAME'         + sItemNameUserSuffix);
		if ( fldPREVIOUS_MFT_PART_NUM  != null ) fldPREVIOUS_MFT_PART_NUM.value = sMFT_PART_NUM   ;
		if ( fldPREVIOUS_NAME          != null ) fldPREVIOUS_NAME.value         = sNAME           ;

		if ( fldQUANTITY != null && fldEXTENDED_PRICE != null )
		{
			// 02/10/2011 Paul.  Stop converting the Quantity to an integer. 
			var nQUANTITY = parseFloat(fldQUANTITY.value);
			// 03/29/2007 Paul.  Initialize the quantity. 
			if ( isNaN(nQUANTITY) )
			{
				nQUANTITY = 1;
				fldQUANTITY.value = nQUANTITY;
			}
			if ( !isNaN(nQUANTITY) )
			{
				if ( !isNaN(dUNIT_PRICE) )
				{
					fldEXTENDED_PRICE.value    = (nQUANTITY * dUNIT_PRICE).localeFormat('c');
				}
				if ( !isNaN(dUNIT_USDOLLAR) )
				{
					fldEXTENDED_USDOLLAR.value = nQUANTITY * dUNIT_USDOLLAR;
				}
			}
			fldQUANTITY.focus();
		}
	}
	else
	{
		alert('result from AutoComplete service is null');
	}
}
function ItemChanged_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById('AjaxErrors');
	fldAjaxErrors.innerHTML = 'Service Error: ' + error.get_message();
}

function ItemQuantityChanged(fldQUANTITY)
{
	var fldAjaxErrors = document.getElementById('AjaxErrors');
	fldAjaxErrors.innerHTML = '';
	
	// 02/10/2011 Paul.  Stop converting the Quantity to an integer. 
	var nQUANTITY = parseFloat(fldQUANTITY.value);
	if ( !isNaN(nQUANTITY) )
	{
		// 05/18/2011 Paul.  .NET 4.0 appends a row index to the field IDs. 
		var userContext = NamePrefix(fldQUANTITY.id, 'QUANTITY');
		var userSuffix  = NameSuffix(fldQUANTITY.id, 'QUANTITY');

		var fldUNIT_PRICE     = document.getElementById(userContext + 'UNIT_PRICE'     + userSuffix);
		var fldEXTENDED_PRICE = document.getElementById(userContext + 'EXTENDED_PRICE' + userSuffix);
		if ( fldUNIT_PRICE != null && fldEXTENDED_PRICE != null )
		{
			// 03/07/2008 Paul.  Use Microsoft AJAX number localization functions. 
			var dUNIT_PRICE = Number.parseLocale(fldUNIT_PRICE.value);
			if ( !isNaN(dUNIT_PRICE) )
			{
				fldEXTENDED_PRICE.value = (nQUANTITY * dUNIT_PRICE).localeFormat('c');
			}
			else
			{
				fldAjaxErrors.innerHTML = 'Unit Price is invalid';
			}
		}
	}
	else
	{
		fldAjaxErrors.innerHTML = 'Quantity is invalid';
	}
}

function ItemUnitPriceChanged(fldUNIT_PRICE)
{
	var fldAjaxErrors = document.getElementById('AjaxErrors');
	fldAjaxErrors.innerHTML = '';
	
	// 03/07/2008 Paul.  Use Microsoft AJAX number localization functions. 
	var dLIST_PRICE = Number.parseLocale(fldUNIT_PRICE.value);
	if ( !isNaN(dLIST_PRICE) )
	{
		fldUNIT_PRICE.value = dLIST_PRICE.localeFormat('c');
		// 05/18/2011 Paul.  .NET 4.0 appends a row index to the field IDs. 
		var userContext = NamePrefix(fldUNIT_PRICE.id, 'UNIT_PRICE');
		var userSuffix  = NameSuffix(fldUNIT_PRICE.id, 'UNIT_PRICE');

		var fldQUANTITY       = document.getElementById(userContext + 'QUANTITY'       + userSuffix);
		var fldEXTENDED_PRICE = document.getElementById(userContext + 'EXTENDED_PRICE' + userSuffix);
		if ( fldQUANTITY != null && fldEXTENDED_PRICE != null )
		{
			// 02/10/2011 Paul.  Stop converting the Quantity to an integer. 
			var nQUANTITY = parseFloat(fldQUANTITY.value);
			if ( !isNaN(nQUANTITY) )
			{
				fldEXTENDED_PRICE.value = (nQUANTITY * dLIST_PRICE).localeFormat('c');
			}
			else
			{
				fldAjaxErrors.innerHTML = 'Quantity is invalid';
			}
		}
	}
	else
	{
		fldAjaxErrors.innerHTML = 'Unit Price is invalid';
	}
}

if ( typeof(Sys) !== 'undefined' )
	Sys.Application.notifyScriptLoaded();

