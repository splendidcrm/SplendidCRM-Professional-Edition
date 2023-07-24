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

function ItemNameChanged(sCURRENCY_ID, fldINVOICE_NAME)
{
	var fldAjaxErrors = document.getElementById('AjaxErrors');
	fldAjaxErrors.innerHTML = '';
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	var userContext = fldINVOICE_NAME.id.replace('INVOICE_NAME', '');
	
	var fldPREVIOUS_NAME = document.getElementById(userContext + 'PREVIOUS_NAME');
	if ( fldPREVIOUS_NAME.value != fldINVOICE_NAME.value )
	{
		if ( fldINVOICE_NAME.value.length > 0 )
		{
			try
			{
				SplendidCRM.Invoices.AutoComplete.GetInvoiceByName(GetCurrencyID(sCURRENCY_ID), fldINVOICE_NAME.value, ItemChanged_OnSucceededWithContext, ItemChanged_OnFailed, userContext);
			}
			catch(e)
			{
				alert('ItemNameChanged: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var fldINVOICE_ID    = document.getElementById(userContext + 'INVOICE_ID'   );
			var fldPREVIOUS_NAME = document.getElementById(userContext + 'PREVIOUS_NAME');
			if ( fldINVOICE_ID    != null ) fldINVOICE_ID.value    = '';
			if ( fldPREVIOUS_NAME != null ) fldPREVIOUS_NAME.value = '';
		}
	}
}

function ItemChanged_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID                  = result.ID                 ;
		var sNAME                = result.NAME               ;
		var dAMOUNT_DUE          = result.AMOUNT_DUE         ;
		var dAMOUNT_DUE_USDOLLAR = result.AMOUNT_DUE_USDOLLAR;
		
		var fldAjaxErrors          = document.getElementById('AjaxErrors');
		var fldINVOICE_ID          = document.getElementById(userContext + 'INVOICE_ID'         );
		var fldINVOICE_NAME        = document.getElementById(userContext + 'INVOICE_NAME'       );
		var fldPREVIOUS_NAME       = document.getElementById(userContext + 'PREVIOUS_NAME'      );
		var fldAMOUNT_DUE          = document.getElementById(userContext + 'AMOUNT_DUE'         );
		var fldAMOUNT_DUE_USDOLLAR = document.getElementById(userContext + 'AMOUNT_DUE_USDOLLAR');
		var fldAMOUNT              = document.getElementById(userContext + 'AMOUNT'             );
		var fldAMOUNT_USDOLLAR     = document.getElementById(userContext + 'AMOUNT_USDOLLAR'    );
		if ( fldINVOICE_ID          != null ) fldINVOICE_ID.value          = sID                 ;
		if ( fldINVOICE_NAME        != null ) fldINVOICE_NAME.value        = sNAME               ;
		if ( fldPREVIOUS_NAME       != null ) fldPREVIOUS_NAME.value       = sNAME               ;
		if ( fldAMOUNT_DUE          != null ) fldAMOUNT_DUE.value          = dAMOUNT_DUE.localeFormat('c');
		if ( fldAMOUNT_DUE_USDOLLAR != null ) fldAMOUNT_DUE_USDOLLAR.value = dAMOUNT_DUE_USDOLLAR;
		if ( fldAMOUNT              != null ) fldAMOUNT.value              = dAMOUNT_DUE.localeFormat('c');
		if ( fldAMOUNT_USDOLLAR     != null ) fldAMOUNT_USDOLLAR.value     = dAMOUNT_DUE_USDOLLAR;
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

if ( typeof(Sys) !== 'undefined' )
	Sys.Application.notifyScriptLoaded();

