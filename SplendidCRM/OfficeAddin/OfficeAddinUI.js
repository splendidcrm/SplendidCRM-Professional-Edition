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

function OfficeAddinUI(sExchangeFromDisplayName, sExchangeFromEmailAddress, sExchangeItemSubject, arrExchangeFromEmailList)
{
	this.ExchangeFromDisplayName    = sExchangeFromDisplayName ;
	this.ExchangeFromEmailAddress   = sExchangeFromEmailAddress;
	this.ExchangeItemSubject        = sExchangeItemSubject     ;
	this.ExchangeFromEmailList      = arrExchangeFromEmailList ;
}

OfficeAddinUI.prototype.Clear = function(sLayoutPanel, sActionsPanel)
{
	try
	{
		SplendidUI_Clear(sLayoutPanel, sActionsPanel);
		SplendidUI_ModuleHeader(sLayoutPanel, sActionsPanel, 'OfficeAddin', null);
		var divOfficeAddin = document.createElement('div');
		divOfficeAddin.id          = 'divOfficeAddin';
		divOfficeAddin.style.width = '100%';
		var divMainLayoutPanel = document.getElementById(sLayoutPanel);
		divMainLayoutPanel.appendChild(divOfficeAddin);
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'OfficeAddinUI.Clear');
	}
}

OfficeAddinUI.prototype.PageCommand = function(sLayoutPanel, sActionsPanel, sCommandName, oCommandArguments)
{
	try
	{
		sCommandName = Sql.ToString(sCommandName);
		if ( sCommandName.indexOf('.Create') > 0 )
		{
			var sMODULE_NAME = sCommandName.split('.')[0];
			var sEDIT_NAME   = sMODULE_NAME + '.EditView' + sPLATFORM_LAYOUT;
			var oEditViewUI  = new EditViewUI();
			var rowInitialValues = new Object();
			// 06/10/2016 Paul.  Set this special flag so that ASSIGNED_USER_ID and TEAM_ID layout values will get initilaized. 
			rowInitialValues['DetailViewRelationshipCreate'] = true;
			if ( sMODULE_NAME == 'Accounts' )
			{
				rowInitialValues.NAME   = this.ExchangeFromDisplayName ;
				rowInitialValues.EMAIL1 = this.ExchangeFromEmailAddress;
			}
			else if ( sMODULE_NAME == 'Contacts' || sMODULE_NAME == 'Leads' )
			{
				if ( this.ExchangeFromDisplayName.indexOf(',') > 0 )
				{
					var arrName = this.ExchangeFromDisplayName.split(',');
					if ( arrName.length == 1 )
					{
						rowInitialValues.LAST_NAME = arrName[0];
					}
					else if ( arrName.length >= 2 )
					{
						rowInitialValues.LAST_NAME  = Trim(arrName[0]);
						rowInitialValues.FIRST_NAME = Trim(arrName[arrName.length - 1]);
					}
				}
				else
				{
					var arrName = this.ExchangeFromDisplayName.split(' ');
					if ( arrName.length == 1 )
					{
						rowInitialValues.LAST_NAME = arrName[0];
					}
					else if ( arrName.length >= 2 )
					{
						rowInitialValues.FIRST_NAME = Trim(arrName[0]);
						rowInitialValues.LAST_NAME  = Trim(arrName[arrName.length - 1]);
					}
				}
				rowInitialValues.EMAIL1 = this.ExchangeFromEmailAddress;
			}
			else if ( sMODULE_NAME == 'Bugs' )
			{
				rowInitialValues.NAME = this.ExchangeItemSubject;
			}
			else if ( sMODULE_NAME == 'Cases' )
			{
				if ( oCommandArguments != null )
				{
					if ( oCommandArguments.COMMAND_MODULE == 'Accounts' )
					{
						rowInitialValues.ACCOUNT_ID   = oCommandArguments.ID  ;
						rowInitialValues.ACCOUNT_NAME = oCommandArguments.NAME;
					}
					else if ( oCommandArguments.COMMAND_MODULE == 'Contacts' || oCommandArguments.COMMAND_MODULE == 'Leads' )
					{
						rowInitialValues.ACCOUNT_ID   = oCommandArguments.ACCOUNT_ID  ;
						rowInitialValues.ACCOUNT_NAME = oCommandArguments.ACCOUNT_NAME;
					}
				}
				rowInitialValues.NAME = this.ExchangeItemSubject;
			}
			else if ( sMODULE_NAME == 'Opportunities' )
			{
				rowInitialValues.NAME = this.ExchangeItemSubject;
			}
			var self = this;
			var sLayoutPanel  = 'divMainLayoutPanel';
			var sActionsPanel = 'divMainActionsPanel';
			oEditViewUI.cbCancel = function()
			{
				// 06/28/2016 Paul.  We need to use a self object as we don't call cbSaveComplete with proper THIS. 
				var oOfficeAddinUI = new OfficeAddinUI(self.ExchangeFromDisplayName, self.ExchangeFromEmailAddress, self.ExchangeItemSubject, self.ExchangeFromEmailList);
				oOfficeAddinUI.Render(sLayoutPanel, sActionsPanel, function(status, message)
				{
					if ( status == 0 || status == 1 )
					{
						callback(1, null);
					}
					else
					{
						SplendidError.SystemMessage(message);
					}
				});
			};
			oEditViewUI.cbSaveComplete = function(sID, sMODULE_NAME)
			{
				var sPRIMARY_MODULE = (oCommandArguments != null ? oCommandArguments.COMMAND_MODULE : null);
				var sPRIMARY_ID     = (oCommandArguments != null ? oCommandArguments.ID             : null);
				var sRELATED_MODULE = sMODULE_NAME  ;
				var sRELATED_ID     = sID           ;
				var bgPage = chrome.extension.getBackgroundPage();
				bgPage.UpdateRelatedItem(sPRIMARY_MODULE, sPRIMARY_ID, sRELATED_MODULE, sRELATED_ID, function(status, message)
				{
					// 06/28/2016 Paul.  We need to use a self object as we don't call cbSaveComplete with proper THIS. 
					var oOfficeAddinUI = new OfficeAddinUI(self.ExchangeFromDisplayName, self.ExchangeFromEmailAddress, self.ExchangeItemSubject, self.ExchangeFromEmailList);
					oOfficeAddinUI.Render(sLayoutPanel, sActionsPanel, function(status, message)
					{
						if ( status == 0 || status == 1 )
						{
							callback(1, null);
						}
						else
						{
							SplendidError.SystemMessage(message);
						}
					});
				}, self);
				/*
				OfficeAddinUI_ArchiveEmail(sMODULE_NAME, sID, function(status, message)
				{
					if ( status === 1 )
					{
						oExchangeSplendidRelated[oCommandArguments.ID] = true;
						var btn = document.getElementById('ArchiveEmail_' + oCommandArguments.ID);
						if ( btn != null )
						{
							btn.style.textDecoration = 'line-through';
							btn.style.fontStyle      = 'italic';
							btn.disabled             = true;
						}
					}
					else
					{
						SplendidError.SystemMessage('Archive Email: ' + message);
					}
				}, this);
				*/
			};
			oEditViewUI.LoadObject(sLayoutPanel, sActionsPanel, sEDIT_NAME, sMODULE_NAME, rowInitialValues, 'btnDynamicButtons_Save', oEditViewUI.PageCommand, function(status, message)
			{
			});
		}
		else if ( sCommandName == 'Edit' )
		{
			var sID          = (oCommandArguments != null ? oCommandArguments.ID             : null);
			var sMODULE_NAME = (oCommandArguments != null ? oCommandArguments.COMMAND_MODULE : null);
			if ( !Sql.IsEmptyString(sID) && !Sql.IsEmptyString(sMODULE_NAME) )
			{
				var oEditViewUI = new EditViewUI();
				var sLayoutPanel  = 'divMainLayoutPanel';
				var sActionsPanel = 'divMainActionsPanel';
				oEditViewUI.Load(sLayoutPanel, sActionsPanel, sMODULE_NAME, sID, false);
			}
		}
		else if ( sCommandName == 'View' )
		{
			var sID          = (oCommandArguments != null ? oCommandArguments.ID             : null);
			var sMODULE_NAME = (oCommandArguments != null ? oCommandArguments.COMMAND_MODULE : null);
			if ( !Sql.IsEmptyString(sID) && !Sql.IsEmptyString(sMODULE_NAME) )
			{
				var oDetailViewUI = new DetailViewUI();
				var sLayoutPanel  = 'divMainLayoutPanel';
				var sActionsPanel = 'divMainActionsPanel';
				oDetailViewUI.Load(sLayoutPanel, sActionsPanel, sMODULE_NAME, sID, function(status, message)
				{
				}, this);
			}
		}
		else if ( sCommandName == 'SearchText' )
		{
			var sSearchText = Trim(Sql.ToString(oCommandArguments));
			if ( !Sql.IsEmptyString(sSearchText) )
			{
				// 03/29/2016 Paul.  Null as the address list means to use like search. 
				var oOfficeAddinUI = new OfficeAddinUI('', sSearchText, this.ExchangeItemSubject, null);
				oOfficeAddinUI.Render(sLayoutPanel, sActionsPanel, function(status, message)
				{
					if ( status < 0 )
					{
						SplendidError.SystemMessage(message);
					}
				});
			}
		}
		else if ( sCommandName == 'ArchiveEmail' )
		{
			var sID          = (oCommandArguments != null ? oCommandArguments.ID             : null);
			var sMODULE_NAME = (oCommandArguments != null ? oCommandArguments.COMMAND_MODULE : null);
			if ( !Sql.IsEmptyString(sID) && !Sql.IsEmptyString(sMODULE_NAME) )
			{
				OfficeAddinUI_ArchiveEmail(sMODULE_NAME, sID, function(status, message)
				{
					if ( status === 1 )
					{
						oExchangeSplendidRelated[oCommandArguments.ID] = true;
						var btn = document.getElementById('ArchiveEmail_' + oCommandArguments.ID);
						if ( btn != null )
						{
							btn.style.textDecoration = 'line-through';
							btn.style.fontStyle      = 'italic';
							btn.disabled             = true;
						}
					}
					else
					{
						SplendidError.SystemMessage('Archive Email: ' + message);
					}
					OfficeAddinUI_HidePopup();
				}, this);
			}
		}
		else if ( sCommandName == 'ViewSplendidCRM' )
		{
			var sID          = (oCommandArguments != null ? oCommandArguments.ID             : null);
			var sMODULE_NAME = (oCommandArguments != null ? oCommandArguments.COMMAND_MODULE : null);
			if ( !Sql.IsEmptyString(sID) && !Sql.IsEmptyString(sMODULE_NAME) )
			{
				window.open(sREMOTE_SERVER + sMODULE_NAME + '\\view.aspx?ID=' + sID, 'ViewSplendidCRM');
				OfficeAddinUI_HidePopup();
			}
		}
		else if ( sCommandName == 'AddRecipient' )
		{
			var addressToAdd = null;
			if ( oCommandArguments != null )
			{
				if ( oCommandArguments['EMAIL1'] !== undefined )
				{
					addressToAdd = { displayName: oCommandArguments['NAME'], emailAddress: oCommandArguments['EMAIL1'] };
				}
				else if ( oCommandArguments['BILLING_CONTACT_EMAIL1'] !== undefined )
				{
					addressToAdd = { displayName: oCommandArguments['BILLING_CONTACT_NAME'], emailAddress: oCommandArguments['BILLING_CONTACT_EMAIL1'] };
				}
			}
			if ( addressToAdd != null )
			{
				var item = Office.cast.item.toItemCompose(_mailbox.item);
				if ( item.itemType === Office.MailboxEnums.ItemType.Message )
				{
					Office.cast.item.toMessageCompose(item).to.addAsync( [addressToAdd] );
				}
				else if ( item.itemType === Office.MailboxEnums.ItemType.Appointment )
				{
					Office.cast.item.toAppointmentCompose(item).requiredAttendees.addAsync( [addressToAdd] );
				}
			}
			else
			{
				SplendidError.SystemMessage('Nothing to add.');
			}
			OfficeAddinUI_HidePopup();
		}
		else if ( sCommandName == 'AttachPDF' )
		{
			var sID          = (oCommandArguments != null ? oCommandArguments.ID             : null);
			var sNAME        = (oCommandArguments != null ? oCommandArguments.NAME           : null);
			var sMODULE_NAME = (oCommandArguments != null ? oCommandArguments.COMMAND_MODULE : null);
			if ( !Sql.IsEmptyString(sID) && !Sql.IsEmptyString(sMODULE_NAME) )
			{
				if ( Sql.IsEmptyString(sNAME) )
					sNAME = sMODULE_NAME + ' ' + sID;
				var item = Office.cast.item.toItemCompose(_mailbox.item);
				if ( item.itemType === Office.MailboxEnums.ItemType.Message )
				{
					var sDETAIL_NAME = sMODULE_NAME + '.DetailView';
					var bgPage = chrome.extension.getBackgroundPage();
					bgPage.DynamicButtons_LoadLayout(sDETAIL_NAME, function(status, message)
					{
						if ( status == 1 )
						{
							var arrButtons = message;
							for ( var i = 0; i < arrButtons.length; i++ )
							{
								if ( arrButtons[i].COMMAND_NAME == 'Report' && arrButtons[i].URL_FORMAT.indexOf('render.aspx') > 0 )
								{
									// https://dev.outlook.com/reference/add-ins/Office.context.mailbox.item.html#addFileAttachmentAsync
									var sURL_FORMAT = arrButtons[i].URL_FORMAT.replace('{0}', sID).replace('../', sREMOTE_SERVER);
									Office.cast.item.toMessageCompose(item).addFileAttachmentAsync(sURL_FORMAT, sNAME, this, function(result)
									{
										// result.asyncContext should be the this object. 
										if ( result.error )
										{
											SplendidError.SystemMessage(result.error);
										}
										else
										{
											console.log(result.value);
										}
									});
								}
							}
						}
						else
						{
							SplendidError.SystemMessage(message);
						}
					}, this);
				}
			}
			else
			{
				SplendidError.SystemMessage('Nothing to attach.');
			}
			OfficeAddinUI_HidePopup();
		}
		else
		{
			SplendidError.SystemMessage('OfficeAddinUI.PageCommand: Unknown command ' + sCommandName);
		}
	}
	catch(e)
	{
		SplendidError.SystemError(e, 'OfficeAddinUI.PageCommand');
	}
}

function OfficeAddinUI_Picture(sID, sPICTURE)
{
	var spnPicture = document.getElementById('OfficeAddinUI_Picture_' + sID);
	if ( spnPicture != null )
	{
		while ( spnPicture.childNodes.length > 0 )
		{
			spnPicture.removeChild(spnPicture.firstChild);
		}
		var img = document.createElement('img');
		img.className = 'ModulePicture';
		img.src = (!Sql.IsEmptyString(sPICTURE) ? sPICTURE : sREMOTE_SERVER + 'App_Themes/Six/images/ActivityStreamUser.gif');
		spnPicture.appendChild(img);
		if ( spnPicture.className.indexOf('ListHeaderModule') > 0 )
			img.className = 'ListModulePicture';
		spnPicture.className = '';
	}
}

OfficeAddinUI.prototype.SearchRenderPaginated = function (sLayoutPanel, sActionsPanel, sMODULE_NAME, bRelationship, arrOfficeAddinButtons, results, nStartIndex, callback, context)
{
	var nMaxEntries = Crm.Config.ToInteger('list_max_entries_per_page')
	var divLayoutPanel = document.getElementById(sLayoutPanel);
	//console.log('SearchRenderPaginated ' + nStartIndex + ' to ' + results.length);
	for (var i = 0; (nStartIndex + i) < results.length && i < nMaxEntries; i++)
	{
		var row = results[nStartIndex + i];
		var sID = row.ID;
		var oDetailViewUI = new DetailViewUI();
		oDetailViewUI.ActivateTab = false;
		oDetailViewUI.FrameClass = '';

		var tblHeader = document.createElement('table');
		tblHeader.id = sLayoutPanel + sID + '_Header';
		tblHeader.cellSpacing = 1;
		tblHeader.cellPadding = 0;
		tblHeader.border = 0;
		tblHeader.width = '100%';
		if (bRelationship)
			tblHeader.className = 'tabOfficeAddinRelationship';
		//else
		//	tblHeader.className   = 'moduleTitle ModuleHeaderFrame';
		divLayoutPanel.appendChild(tblHeader);
		var tr = tblHeader.insertRow(-1);
		var td = tr.insertCell(-1);
		td.vAlign = 'top';
		var spanModule = document.createElement('span');
		spanModule.id = 'OfficeAddinUI_Picture_' + sID;
		spanModule.className = 'ModuleHeaderModule ModuleHeaderModule' + sMODULE_NAME;
		spanModule.innerHTML = L10n.Term(sMODULE_NAME + '.LBL_MODULE_ABBREVIATION');
		spanModule.style.marginTop = '2px';
		td.appendChild(spanModule);
		if (bRelationship)
			spanModule.className += ' ListHeaderModule';

		var divActionsPopup = document.createElement('div');
		divActionsPopup.id = tblHeader.id + '_Actions';
		divActionsPopup.className = 'button-panel';
		td.appendChild(divActionsPopup);
		DynamicButtonsUI_OfficeAddinActions(sLayoutPanel, sActionsPanel, divActionsPopup, arrOfficeAddinButtons, row, sMODULE_NAME, this.PageCommand, this);

		td = document.createElement('td');
		tr.appendChild(td);
		td.style.width = '99%';
		// 03/24/2016 Paul.  We want to allow wrap as Order subject may be long. 
		//td.style.whiteSpace = 'nowrap';

		// 02/26/2016 Paul. oDetailViewUI.Load() will clear the actions panel, so create a bogus panel. 
		var divDetailViewLayout = document.createElement('div');
		divDetailViewLayout.id = sLayoutPanel + sID + '_Layout';
		td.appendChild(divDetailViewLayout);
		var divDetailViewActions = document.createElement('div');
		divDetailViewActions.id = sLayoutPanel + sID + '_Actions';
		td.appendChild(divDetailViewActions);
		oDetailViewUI.LoadOfficeAddin(divDetailViewLayout.id, divDetailViewActions.id, sMODULE_NAME + '.Home' + sPLATFORM_LAYOUT, sMODULE_NAME, sID, function (status, message) {
			callback.call(context || this, 1, message);
		}, this);
		// 03/19/2016 Paul.  The primary entry will only use the first. 
		if (!bRelationship)
			break;
		if ( (nStartIndex + i + 1) < results.length && (i + 1) == nMaxEntries )
		{
			var divMore = document.createElement('div');
			divMore.id = sLayoutPanel + '_' + sMODULE_NAME + '_More' + (nStartIndex + i + 1).toString();
			divMore.className = 'OfficeAddinMoreRecords';
			divLayoutPanel.appendChild(divMore);

			var aMore = document.createElement('a');
			aMore.style.display = 'inline';
			aMore.href = '#';
			aMore.appendChild(document.createTextNode(L10n.Term('.LBL_MORE_RECORDS')));
			divMore.appendChild(aMore);
			aMore.onclick = function (e)
			{
				divMore.style.display = 'none';
				context.SearchRenderPaginated(sLayoutPanel, sActionsPanel, sMODULE_NAME, bRelationship, arrOfficeAddinButtons, results, (nStartIndex + i), callback, context);
				return false;
			};
		}
	}
}

OfficeAddinUI.prototype.SearchModule = function(sLayoutPanel, sActionsPanel, sMODULE_NAME, sPRIMARY_FIELD, bRelationship, arrOfficeAddinButtons, callback, context)
{
	var sSEARCH_FILTER = '';
	var rowDefaultSearch = null;
	//console.log('SearchModule ' + sMODULE_NAME + ' for ' + this.ExchangeFromEmailAddress);
	// 03/23/2016 Paul.  Email could be in the Sent Items folder and be sent to multiple recipients. 
	if ( this.ExchangeFromEmailList != null && this.ExchangeFromEmailList.length > 0 )
	{
		for ( var i = 0; i < this.ExchangeFromEmailList.length; i++ )
		{
			var sEmailAddress = this.ExchangeFromEmailList[i];
			var sEmailDomain  = (sEmailAddress.indexOf('@') > 0 ? sEmailAddress.substring(sEmailAddress.indexOf('@')) : '');
			var arrPRIMARY_FIELD = sPRIMARY_FIELD.split(' ');
			for ( var j = 0; j < arrPRIMARY_FIELD.length; j++ )
			{
				var sPRIMARY_FIELD = arrPRIMARY_FIELD[j];
				if ( sPRIMARY_FIELD.indexOf('EMAIL1') >= 0 )
				{
					if ( sSEARCH_FILTER.length > 0 )
						sSEARCH_FILTER += ' or ';
					sSEARCH_FILTER += sPRIMARY_FIELD + ' eq \'' + sEmailAddress + '\'';
					if ( sEmailDomain.length > 0 )
					{
						if ( (sMODULE_NAME == 'Accounts' && sPRIMARY_FIELD == 'EMAIL1') || (sPRIMARY_FIELD == 'ACCOUNT_EMAIL1') )
						{
							sSEARCH_FILTER += ' or ' + sPRIMARY_FIELD + ' like \'' + sEmailDomain + '\'';
						}
					}
				}
			}
		}
	}
	else if ( !Sql.IsEmptyString(this.ExchangeFromEmailAddress) )
	{
		var sEmailDomain = (this.ExchangeFromEmailAddress.indexOf('@') > 0 ? this.ExchangeFromEmailAddress.substring(this.ExchangeFromEmailAddress.indexOf('@')) : '');
		var arrPRIMARY_FIELD = sPRIMARY_FIELD.split(' ');
		for ( var j = 0; j < arrPRIMARY_FIELD.length; j++ )
		{
			var sPRIMARY_FIELD = arrPRIMARY_FIELD[j];
			if ( sPRIMARY_FIELD.indexOf('EMAIL1') >= 0 )
			{
				if ( sSEARCH_FILTER.length > 0 )
					sSEARCH_FILTER += ' or ';
				// 03/29/2016 Paul.  Use like clause if there are no address list entries as this means it is a search. 
				sSEARCH_FILTER += sPRIMARY_FIELD + ' like \'%' + this.ExchangeFromEmailAddress + '%\'';
				if ( sEmailDomain.length > 0 )
				{
					if ( (sMODULE_NAME == 'Accounts' && sPRIMARY_FIELD == 'EMAIL1') || (sPRIMARY_FIELD == 'ACCOUNT_EMAIL1') )
					{
						sSEARCH_FILTER += ' or ' + sPRIMARY_FIELD + ' like \'%' + sEmailDomain + '%\'';
					}
				}
			}
		}
	}
	// 06/08/2016 Paul.  ExchangeItemSubject might be null. 
	if ( !Sql.IsEmptyString(this.ExchangeItemSubject) && this.ExchangeItemSubject.indexOf('[CASE:') >= 0 && sMODULE_NAME == 'Cases' )
	{
		var nStart = this.ExchangeItemSubject.indexOf('[CASE:') + 6;
		var nEnd   = this.ExchangeItemSubject.indexOf(']', nStart);
		if ( nStart >=0 && nEnd > nStart && (nEnd - nStart == 36) )
		{
			// 03/19/2016 Paul.  If the special key is in the subject, then convert to a specific ID search. 
			//if ( sSEARCH_FILTER.length > 0 )
			//	sSEARCH_FILTER += ' or ';
			sSEARCH_FILTER = 'ID eq \'' + this.ExchangeItemSubject.substring(nStart, nEnd) + '\'';
		}
	}
	//console.log('SearchModule: ' + sMODULE_NAME + ' ' + sSEARCH_FILTER)
	var bgPage = chrome.extension.getBackgroundPage();
	if ( !Sql.IsEmptyString(sSEARCH_FILTER) )
	{
		var sSORT_FIELD      = 'DATE_ENTERED';
		var sSORT_DIRECTION  = 'desc';
		var sSELECT_FIELDS   = 'ID, NAME';
		// 03/26/2016 Paul.  We don't have a way to specify this list of fields in a layout, so just manually add. 
		if ( sMODULE_NAME == 'Contacts' || sMODULE_NAME == 'Leads' )
		{
			sSELECT_FIELDS += ',ACCOUNT_ID,ACCOUNT_NAME,EMAIL1';
		}
		if ( sMODULE_NAME == 'Quotes' || sMODULE_NAME == 'Orders' || sMODULE_NAME == 'Invoices' )
		{
			sSELECT_FIELDS += ',BILLING_CONTACT_NAME,BILLING_CONTACT_EMAIL1';
		}
		var rowSEARCH_VALUES = null;
		bgPage.ListView_LoadModule(sMODULE_NAME, sSORT_FIELD, sSORT_DIRECTION, sSELECT_FIELDS, sSEARCH_FILTER, rowSEARCH_VALUES, function(status, message)
		{
			if ( status == 1 )
			{
				results = message;
				//console.log(sMODULE_NAME + ' ' + results.length);
				this.SearchRenderPaginated(sLayoutPanel, sActionsPanel, sMODULE_NAME, bRelationship, arrOfficeAddinButtons, results, 0, callback, context);
				if ( results.length == 0 )
				{
					callback.call(context||this, 2, null);
				}
			}
			else
			{
				callback.call(context||this, status, message);
			}
		}, this);
	}
}

OfficeAddinUI.prototype.Render = function(sLayoutPanel, sActionsPanel, callback, context)
{
	try
	{
		console.log('sExchangeMessageLayout = ' + sExchangeMessageLayout);
		SplendidError.SystemMessage('');
		this.Clear(sLayoutPanel, sActionsPanel);
		// 12/06/2014 Paul.  LayoutMode is used on the Mobile view. 
		if ( ctlActiveMenu != null )
			ctlActiveMenu.ActivateTab('OfficeAddin', null, 'OfficeAddinView');
		
		var divOfficeAddin = document.getElementById('divOfficeAddin');
		var divSearchText = document.createElement('div');
		divSearchText.id = 'divOfficeAddin_divSearchText';
		divOfficeAddin.appendChild(divSearchText);
		
		var txtSearch = document.createElement('input');
		txtSearch.id                  = 'divOfficeAddin_divSearchText_txtSearch';
		txtSearch.type                = 'search';
		txtSearch.className           = 'ChatInputText';
		txtSearch.style.marginTop     = '3px';
		txtSearch.style.verticalAlign = 'top';
		txtSearch.value               = (this.ExchangeFromEmailList == null ? this.ExchangeFromEmailAddress : '');
		divSearchText.appendChild(txtSearch);
		var btnSearch = document.createElement('button');
		btnSearch.id                  = 'divChatDashboard_btnSearch';
		btnSearch.className           = 'ChatInputSubmit';
		btnSearch.innerHTML           = L10n.Term('.LBL_SEARCH_BUTTON_LABEL');
		btnSearch.style.display       = 'none';
		divSearchText.appendChild(btnSearch);
		txtSearch.onkeypress = function(e)
		{
			return RegisterEnterKeyPress(e, btnSearch.id);
		};
		btnSearch.onclick = BindArguments(function(Page_Command, sLayoutPanel, sActionsPanel, context)
		{
			Page_Command.call(context, sLayoutPanel, sActionsPanel, 'SearchText', txtSearch.value);
		}, this.PageCommand, sLayoutPanel, sActionsPanel, context||this);

		var aSearch = document.createElement('a');
		divSearchText.appendChild(aSearch);
		var iSearch = document.createElement('i');
		iSearch.id        = 'divOfficeAddin_divSearchText_iSearch';
		iSearch.className = 'fa fa-2x fa-search navButton';
		aSearch.appendChild(iSearch);
		aSearch.style.cursor      = 'pointer';
		aSearch.onclick = btnSearch.onclick;
		txtSearch.style.width = ($(divOfficeAddin).width() - $(aSearch).width() - 60).toString() + 'px';

		var divEmailInfo = document.createElement('h2');
		divEmailInfo.id = 'divOfficeAddin_divEmailInfo';
		// 03/19/2016 Paul.  Start with email info hidden. 
		divEmailInfo.style.display = 'block';
		divOfficeAddin.appendChild(divEmailInfo);
		if ( !Sql.IsEmptyString(this.ExchangeFromEmailAddress) )
		{
			if ( !Sql.IsEmptyString(this.ExchangeFromDisplayName) && this.ExchangeFromDisplayName != this.ExchangeFromEmailAddress )
				divEmailInfo.appendChild(document.createTextNode(this.ExchangeFromDisplayName + ' <' + this.ExchangeFromEmailAddress + '>'));
			else
				divEmailInfo.appendChild(document.createTextNode(this.ExchangeFromEmailAddress));
		}
		
		var sDETAIL_NAME = 'Home.OfficeAddin.AppRead';
		var bgPage = chrome.extension.getBackgroundPage();
		if ( !Sql.IsEmptyString(this.ExchangeFromEmailAddress) && sExchangeMessageLayout != 'AppCompose' )
		{
			bgPage.DynamicButtons_LoadLayout(sDETAIL_NAME, function(status, message)
			{
				if ( status == 1 )
				{
					var arrOfficeAddinButtons = message;
				
					var divActionsPopup = document.createElement('span');
					divActionsPopup.id               = divEmailInfo.id + '_Actions';
					divActionsPopup.className        = 'button-panel';
					divActionsPopup.style.marginLeft = '4px';
					divEmailInfo.appendChild(divActionsPopup);
					DynamicButtonsUI_OfficeAddinActions(sLayoutPanel, sActionsPanel, divActionsPopup, arrOfficeAddinButtons, null, null, this.PageCommand, this);
				}
				else
				{
					callback(status, message);
				}
			}, this);
		}
		
		var divActions = document.createElement('div');
		var divLayout  = document.createElement('div');
		divActions.id = 'divOfficeAddin_ListView'  ;
		divLayout .id = 'divOfficeAddin_DetailView';
		divOfficeAddin.appendChild(divActions);
		divOfficeAddin.appendChild(divLayout );
		
		var sMODULE_NAME   = 'Contacts';
		var sPRIMARY_FIELD = 'EMAIL1';
		sDETAIL_NAME = sMODULE_NAME + '.OfficeAddin.' + sExchangeMessageLayout;
		bgPage.DynamicButtons_LoadLayout(sDETAIL_NAME, function(status, message)
		{
			if ( status == 1 )
			{
				var arrOfficeAddinButtons = message;
				// 03/18/2016 Paul.  First search contacts. 
				this.SearchModule(divLayout .id, divActions.id, sMODULE_NAME, sPRIMARY_FIELD, false, arrOfficeAddinButtons, function(status, message)
				{
					if ( status == 2 )
					{
						// 03/18/2016 Paul.  If contact not found, then search leads. 
						var sMODULE_NAME   = 'Leads';
						var sPRIMARY_FIELD = 'EMAIL1';
						var sDETAIL_NAME = sMODULE_NAME + '.OfficeAddin.' + sExchangeMessageLayout;
						bgPage.DynamicButtons_LoadLayout(sDETAIL_NAME, function(status, message)
						{
							if ( status == 1 )
							{
								var arrOfficeAddinButtons = message;
								this.SearchModule(divLayout .id, divActions.id, sMODULE_NAME, sPRIMARY_FIELD, false, arrOfficeAddinButtons, function(status, message)
								{
									if ( status == 2 )
									{
										// 03/18/2016 Paul.  If lead not found, then search account by domain only. 
										var sMODULE_NAME   = 'Accounts';
										var sPRIMARY_FIELD = 'EMAIL1';
										var sDETAIL_NAME = sMODULE_NAME + '.OfficeAddin.' + sExchangeMessageLayout;
										bgPage.DynamicButtons_LoadLayout(sDETAIL_NAME, function(status, message)
										{
											if ( status == 1 )
											{
												var arrOfficeAddinButtons = message;
												this.SearchModule(divLayout .id, divActions.id, sMODULE_NAME, sPRIMARY_FIELD, false, arrOfficeAddinButtons, function(status, message)
												{
													if ( status == 2 )
													{
														divEmailInfo.style.display = 'block';
													}
												});
											}
										}, this);
									}
									else if ( status == 1 )
									{
										// record found. 
									}
									else if ( status < 0 )
									{
										callback.call(context||this, status, message);
									}
								}, this);
							}
						}, this);
					}
					else if ( status == 1 )
					{
						// record found. 
					}
					else if ( status < 0 )
					{
						callback.call(context||this, status, message);
					}
				}, this);
				
				var sDETAIL_NAME = 'Home.OfficeAddin.' + sExchangeMessageLayout;
				bgPage.DetailViewRelationships_LoadLayout(sDETAIL_NAME, function(status, message)
				{
					if ( status == 1 )
					{
						var arrDetailViewRelationship = message;
						//console.log(dumpObj(arrDetailViewRelationship, 'arrDetailViewRelationship'));
						for ( var i = 0; i < arrDetailViewRelationship.length; i++ )
						{
							var sMODULE_NAME   = arrDetailViewRelationship[i].MODULE_NAME  ;
							var sPRIMARY_FIELD = arrDetailViewRelationship[i].PRIMARY_FIELD;
							var sTITLE         = arrDetailViewRelationship[i].TITLE        ;
							var sCONTROL_ID    = sLayoutPanel + '_div' + sMODULE_NAME;
							// 03/28/2016 Paul.  We need to bind the arguments as the button load will be slow and will allow moduile name to be changed before executed. 
							BindArguments(function(sMODULE_NAME, sPRIMARY_FIELD, sCONTROL_ID, context)
							{
								var sDETAIL_NAME   = sMODULE_NAME + '.OfficeAddin.' + sExchangeMessageLayout;
								bgPage.DynamicButtons_LoadLayout(sDETAIL_NAME, function(status, message)
								{
									if ( status == 1 )
									{
										var arrOfficeAddinButtons = message;
										var divSubPanel = document.createElement('div');
										divSubPanel.id = 'divOfficeAddin_' + sCONTROL_ID;
										divOfficeAddin.appendChild(divSubPanel);
										
										var divActions = document.createElement('div');
										var divLayout  = document.createElement('div');
										divActions.id = divSubPanel.id + '_ListView'  ;
										divLayout .id = divSubPanel.id + '_DetailView';
										divOfficeAddin.appendChild(divActions);
										divOfficeAddin.appendChild(divLayout );
										//console.log('Render DetailViewRelationship: ' + sMODULE_NAME + ' ' + sPRIMARY_FIELD);
										try
										{
											this.SearchModule(divLayout.id, divActions.id, sMODULE_NAME, sPRIMARY_FIELD, true, arrOfficeAddinButtons, function(status, message)
											{
												if ( status < 0 )
												{
													callback.call(context||this, status, message);
												}
											}, context||this);
										}
										catch(e)
										{
											console.log(e.message);
										}
									}
								}, context||this);
							}, sMODULE_NAME, sPRIMARY_FIELD, sCONTROL_ID, this)();
						}
					}
					else
					{
						callback(status, message);
					}
				}, this);
			}
			else
			{
				callback(status, message);
			}
		}, this);
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'OfficeAddinUI.Render');
	}
}

function OfficeAddinResize()
{
	try
	{
		var divOfficeAddin = document.getElementById('divOfficeAddin');
		if ( divOfficeAddin != null )
		{
			var rect = divOfficeAddin.getBoundingClientRect();
			var nHeight = $(window).height() - rect.top - 8;
			divOfficeAddin.style.height = nHeight.toString() + 'px';
		}
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'OfficeAddinResize');
	}
}

function OfficeAddinUI_ArchiveEmail(sMODULE_NAME, sPARENT_ID, callback, context)
{
	if ( !ValidateCredentials() )
	{
		callback.call(context||this, -1, 'Invalid connection information.');
		return;
	}
	var ewsUrl = Office.context.mailbox.ewsUrl;
	Office.context.mailbox.getCallbackTokenAsync(function (ar)
	{
		var token = ar.value;
		//sendRequest("GetAttachment/SaveAttachments", attachmentData);
		var xhr = CreateSplendidRequest('OfficeAddin/Rest.svc/ArchiveEmail');
		xhr.onreadystatechange = function()
		{
			if ( xhr.readyState == 4 )
			{
				GetSplendidResult(xhr, function(result)
				{
					try
					{
						console.log(result);
						if ( result.status == 200 )
						{
							if ( result.d !== undefined )
							{
								callback.call(context||this, result.d, null);
							}
							else
							{
								callback.call(context||this, -1, xhr.responseText);
							}
						}
						else
						{
							if ( result.status == 0 )
								callback.call(context||this, 0, result.ExceptionDetail.Message);
							else if ( result.ExceptionDetail !== undefined )
								callback.call(context||this, -1, result.ExceptionDetail.Message);
							else
								callback.call(context||this, -1, xhr.responseText);
						}
					}
					catch(e)
					{
						callback.call(context||this, -1, SplendidError.FormatError(e, 'ArchiveEmail'));
					}
				});
			}
		}
		try
		{
			var request = new Object();
			request.authToken         = token                     ;
			request.ewsUrl            = ewsUrl                    ;
			request.itemID            = sExchangeItemID           ;
			request.internetMessageId = sExchangeInternetMessageID;
			request.sentItemsFolder   = bExchangeSentItemsFolder  ;
			request.MODULE_NAME       = sMODULE_NAME              ;
			request.PARENT_ID         = sPARENT_ID                ;
			//console.log(request);
			xhr.send(JSON.stringify(request));
		}
		catch(e)
		{
			//alert('Login: ' + e.message);
			callback.call(context||this, -1, SplendidError.FormatError(e, 'ArchiveEmail'));
		}
	});
}

