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

var _mailbox;
var _document;
var sExchangeFromDisplayName    = '';
var sExchangeFromEmailAddress   = '';
var arrExchangeFromEmailAddress = new Array();
var sExchangeItemType           = '';
var sExchangeItemSubject        = '';
var sExchangeItemID             = '';
var sExchangeUserEmailAddress   = '';
var sExchangeUserEmailDomain    = '';
var sExchangeUniqueUserID       = '';
// 03/21/2016 Paul.  SplendidCRM Exchange Sync uses the InternetMessageId when archiving. 
var sExchangeInternetMessageID  = '';
var sExchangeSplendidID         = '';
var oExchangeSplendidRelated    = new Array();
var bExchangeSentItemsFolder    = false;
var sExchangeMessageLayout      = 'AppRead';

var app = (function()
{
	"use strict";
	var app = {};
	// Common initialization function (to be called from each page)
	app.initialize = function ()
	{
		$('body').append(
			'<div id="notification-message">' +
				'<div class="padding">' +
					'<div id="notification-message-close"></div>' +
					'<div id="notification-message-header"></div>' +
					'<div id="notification-message-body"></div>' +
				'</div>' +
			'</div>');

		$('#notification-message-close').click(function()
		{
			$('#notification-message').hide();
		});

		// After initialization, expose a common notification function
		app.showNotification = function ( header, text )
		{
			$('#notification-message-header').text(header);
			$('#notification-message-body'  ).text(text  );
			$('#notification-message'       ).slideDown('fast');
		};
	};
	return app;
})();

(function()
{
	// The Office initialize function must be run each time a new page is loaded
	Office.initialize = function(reason)
	{
		$(document).ready(function()
		{
			app.initialize();
			_mailbox  = Office.context.mailbox;
			_document = Office.context.document;
			if ( Office.context.document )
			{
				// 04/02/2016 Paul.  Simulate exchange login so that we can remember the credentials. 
				sExchangeUserID = Sql.ToString(localStorage['EXCHANGE_USER_ID']);
				if (Sql.IsEmptyString(sExchangeUserID))
				{
					sExchangeUserID = Math.uuid();
					localStorage['EXCHANGE_USER_ID'] = sExchangeUserID;
				}
				getUserIdentityTokenCallback({ value: sExchangeUserID });
				LoadSplendid();
			}
			else if ( Office.context.mailbox )
			{
				try
				{
					// http://dev.outlook.com/reference/add-ins/Office.context.mailbox.userProfile.html
					sExchangeUserEmailAddress = Office.context.mailbox.userProfile.emailAddress;
					sExchangeUserEmailDomain  = (sExchangeUserEmailAddress.indexOf('@') > 0 ? sExchangeUserEmailAddress.substring(sExchangeUserEmailAddress.indexOf('@')) : '');
				
					// https://msdn.microsoft.com/EN-US/library/office/dn568064.aspx
					// http://dev.outlook.com/reference/add-ins/Office.context.mailbox.item.html#from
					var item = Office.cast.item.toItemRead(Office.context.mailbox.item);
					sExchangeMessageLayout = 'AppRead';
					sExchangeItemType      = item.itemType;
				
					sExchangeItemID            = item.itemId ;
					sExchangeItemSubject       = item.subject;
					sExchangeInternetMessageID = item.internetMessageId;
					console.log('ItemType = ' + item.itemType);
					if ( item.itemType === Office.MailboxEnums.ItemType.Message )
					{
						var from = Office.cast.item.toMessageRead(item).from;
						// http://dev.outlook.com/reference/add-ins/simple-types.html#EmailAddressDetails
						sExchangeFromDisplayName  = from.displayName ;
						sExchangeFromEmailAddress = from.emailAddress;
						//if ( from )
						//{
						//	$('#status').text(from.emailAddress);
						//	$('#from').click(function()
						//	{
						//		app.showNotification( from.displayName, from.emailAddress );
						//	});
						//}
					}
					else if ( item.itemType === Office.MailboxEnums.ItemType.Appointment )
					{
						var from = Office.cast.item.toAppointmentRead(item).organizer;
						// http://dev.outlook.com/reference/add-ins/simple-types.html#EmailAddressDetails
						sExchangeFromDisplayName  = from.displayName ;
						sExchangeFromEmailAddress = from.emailAddress;
					}
				
					// 03/23/2016 Paul.  We cannot get the folder of the email, so use the User Email Address. 
					//console.log('ExchangeFromEmailAddress = ' + sExchangeFromEmailAddress);
					//console.log('ExchangeUserEmailAddress = ' + sExchangeUserEmailAddress);
					if ( sExchangeFromEmailAddress == sExchangeUserEmailAddress )
					{
						bExchangeSentItemsFolder = true;
						// 03/28/2016 Paul.  If sent from user, then clear the email so that it will get set. 
						sExchangeFromEmailAddress = '';
						if ( item.itemType === Office.MailboxEnums.ItemType.Message )
						{
							var to = Office.cast.item.toMessageRead(item).to;
							if ( to != null )
							{
								for ( var i = 0; i < to.length; i++ )
								{
									// 03/23/2016 Paul.  Exclude emails to other employees. 
									if ( to[i].emailAddress.indexOf(sExchangeUserEmailDomain) > 0 )
										continue;
									if ( Sql.IsEmptyString(sExchangeFromEmailAddress) )
									{
										sExchangeFromDisplayName  = to[i].displayName ;
										sExchangeFromEmailAddress = to[i].emailAddress;
									}
									else
									{
										arrExchangeFromEmailAddress.push(to[i].emailAddress);
									}
								}
							}
							to = Office.cast.item.toMessageRead(item).cc;
							if ( to != null )
							{
								for ( var i = 0; i < to.length; i++ )
								{
									// 03/23/2016 Paul.  Exclude emails to other employees. 
									if ( to[i].emailAddress.indexOf(sExchangeUserEmailDomain) > 0 )
										continue;
									if ( Sql.IsEmptyString(sExchangeFromEmailAddress) )
									{
										sExchangeFromDisplayName  = to[i].displayName ;
										sExchangeFromEmailAddress = to[i].emailAddress;
									}
									else
									{
										arrExchangeFromEmailAddress.push(to[i].emailAddress);
									}
								}
							}
							to = Office.cast.item.toMessageRead(item).bcc;
							if ( to != null )
							{
								for ( var i = 0; i < to.length; i++ )
								{
									// 03/23/2016 Paul.  Exclude emails to other employees. 
									if ( to[i].emailAddress.indexOf(sExchangeUserEmailDomain) > 0 )
										continue;
									if ( Sql.IsEmptyString(sExchangeFromEmailAddress) )
									{
										sExchangeFromDisplayName  = to[i].displayName ;
										sExchangeFromEmailAddress = to[i].emailAddress;
									}
									else
									{
										arrExchangeFromEmailAddress.push(to[i].emailAddress);
									}
								}
							}
						}
					}
					else
					{
						// 03/23/2016 Paul.  Exclude emails to other employees. 
						if ( !Sql.IsEmptyString(sExchangeFromEmailAddress) && sExchangeFromEmailAddress.indexOf(sExchangeUserEmailDomain) < 0 )
						{
							arrExchangeFromEmailAddress.push(sExchangeFromEmailAddress);
						}
					}
					//console.log('arrExchangeFromEmailAddress = ' + dumpObj(arrExchangeFromEmailAddress, ''));
				}
				catch(e)
				{
					console.log('toItemRead: ' + e.message);
				}
				try
				{
					var item = Office.cast.item.toItemCompose(Office.context.mailbox.item);
					sExchangeMessageLayout = 'AppCompose';
					sExchangeItemType      = item.itemType;
				
					if ( item.itemType === Office.MailboxEnums.ItemType.Message )
					{
						Office.cast.item.toMessageCompose(item).to.getAsync(function(result)
						{
							var to = result.value;
							for ( var i = 0; i < to.length; i++ )
							{
								console.log( to[i].displayName + ' <' + to[i].emailAddress + '>');
								// 03/23/2016 Paul.  Exclude emails to other employees. 
								if ( to[i].emailAddress.indexOf(sExchangeUserEmailDomain) > 0 )
									continue;
								if ( Sql.IsEmptyString(sExchangeFromEmailAddress) )
								{
									sExchangeFromDisplayName  = to[i].displayName ;
									sExchangeFromEmailAddress = to[i].emailAddress;
								}
								else
								{
									arrExchangeFromEmailAddress.push(to[i].emailAddress);
								}
							}
						});
					}

					//item.subject.setAsync( "Hello world!" );
					//item.subject.getAsync( function ( result ) { console.log(result.value) });
					/*
					var addressToAdd =
					{
						displayName: Office.context.mailbox.userProfile.displayName,
						emailAddress: Office.context.mailbox.userProfile.emailAddress
					};

					if ( item.itemType === Office.MailboxEnums.ItemType.Message )
					{
						Office.cast.item.toMessageCompose( item ).to.addAsync( [addressToAdd] );
					}
					else if ( item.itemType === Office.MailboxEnums.ItemType.Appointment )
					{
						Office.cast.item.toAppointmentCompose( item ).requiredAttendees.addAsync( [addressToAdd] );
					}
					*/
				}
				catch(e)
				{
					console.log('toItemCompose: ' + e.message);
				}
				_mailbox.getUserIdentityTokenAsync(getUserIdentityTokenCallback);
			}
			else
			{
				app.showNotification('Error!', 'Unsupported Office.context.');
			}
		});
	};

	function getUserIdentityTokenCallback(asyncResult)
	{
		var token   = asyncResult.value;
		var hostUri = window.location.href.split('?')[0];
		var ewsUrl  = (_mailbox != null ? _mailbox.ewsUrl : null);
		var url     = hostUri.replace('default.aspx', '');

		var xhr = new XMLHttpRequest();
		xhr.open('POST', url + 'Rest.svc/CreateAndValidateIdentityToken');
		xhr.setRequestHeader('Content-Type', 'application/json');
		xhr.onreadystatechange = function()
		{
			if ( xhr.readyState == 4 && xhr.status == 200 )
			{
				//console.log(xhr.responseText);
				var response = JSON.parse(xhr.responseText);
				if ( response.errorMessage == null )
				{
					sExchangeUserID       = response.token.ExchangeUserID;  // User Exchange ID
					sExchangeUniqueUserID = response.token.UniqueUserID  ;  // Unique identifier
					sExchangeSplendidID   = response.token.SplendidID    ;  // Email ID if found. 
					if ( response.token.SplendidRelated != null )  // Related records. 
					{
						//console.log('SplendidID = ' + response.token.SplendidID);
						//console.log(response.token.SplendidRelated);
						oExchangeSplendidRelated = new Object();
						for ( var i = 0; i < response.token.SplendidRelated.length; i++ )
						{
							oExchangeSplendidRelated[response.token.SplendidRelated[i]] = true;
						}
					}
					// 04/02/2016 Paul. Save Exchange User ID to local storage so that it can be used by Word and Excel. 
					localStorage['EXCHANGE_USER_ID']= sExchangeUserID;
				}
				else
				{
					sExchangeUserID       = '';
					sExchangeUniqueUserID = '';
					sExchangeSplendidID   = '';
					app.showNotification('Error!', response.errorMessage);
				}
				LoadSplendid();
				// 06/28/2016 Paul.  If we were not authenticated, but now AutoLoggedIn, then we need to reload the user profile. 
				if ( response.token.AutoLoggedIn && Sql.IsEmptyGuid(sUSER_ID) )
				{
					try
					{
						GetUserProfile(function(status, message)
						{
							//app.showNotification('Status', 'Already logged in. [' + Security.USER_ID() + ']');
						});
					}
					catch(e)
					{
						app.showNotification('Error', e.message);
					}
				}
			}
			else if ( xhr.status == 500 )
			{
				console.log('getUserIdentityTokenCallback ' + xhr.statusText);
			}
		};
		try
		{
			var request = new Object();
			request.token             = token  ;
			request.hostUri           = hostUri;
			request.ewsUrl            = ewsUrl ;
			request.internetMessageId = sExchangeInternetMessageID;
			xhr.send(JSON.stringify(request));
		}
		catch(e)
		{
			app.showNotification("Error!", e.message);
		}
	}
})();

SplendidError.SystemAlert = function(e, method)
{
	var message = SplendidError.FormatError(e, method);
	SplendidError.arrErrorLog.push(message);
	app.showNotification('Error!', message);
}

function Login(callback, context)
{
	if ( !ValidateCredentials() )
	{
		callback.call(context||this, -1, 'Invalid connection information.');
		return;
	}
	var xhr = CreateSplendidRequest('OfficeAddin/Rest.svc/Login');
	xhr.onreadystatechange = function()
	{
		if ( xhr.readyState == 4 )
		{
			GetSplendidResult(xhr, function(result)
			{
				try
				{
					if ( result.status == 200 )
					{
						if ( result.d !== undefined )
						{
							if ( result.d.length == 36 )
							{
								sUSER_ID = result.d;
								// 05/07/2013 Paul.  Replace GetUserLanguage with GetUserProfile. 
								GetUserProfile(function(status, message)
								{
									if ( status == 1 )
									{
										// 09/09/2014 Paul.  Reset after getting the language. 
										SplendidCache.Reset();
										callback.call(context||this, 1, null);
									}
									else
									{
										callback.call(context||this, status, message);
									}
								});
							}
							else
								callback.call(context||this, -1, 'Login should return Guid.');
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
					callback.call(context||this, -1, SplendidError.FormatError(e, 'Login'));
				}
			});
		}
	}
	try
	{
		xhr.send('{"UserName": ' + JSON.stringify(sUSER_NAME) + ', "Password": ' + JSON.stringify(sPASSWORD) + ', "ExchangeUserID": ' + JSON.stringify(sExchangeUserID) + ', "Version": "6.0"}');
	}
	catch(e)
	{
		//alert('Login: ' + e.message);
		callback.call(context||this, -1, SplendidError.FormatError(e, 'Login'));
	}
}

function Logout(callback, context)
{
	if ( !ValidateCredentials() )
	{
		callback.call(context||this, -1, 'Invalid connection information.');
		return;
	}
	var xhr = CreateSplendidRequest('OfficeAddin/Rest.svc/Logout');
	xhr.onreadystatechange = function()
	{
		if ( xhr.readyState == 4 )
		{
			GetSplendidResult(xhr, function(result)
			{
				try
				{
					if ( result.status == 200 )
					{
						if ( result.d !== undefined )
						{
							sUSER_ID = '';
							// 04/02/2016 Paul.  Clear ID used for Word and Excel. 
							localStorage['EXCHANGE_USER_ID'] = '';
							callback.call(context||this, 1, null);
						}
						else
						{
							callback.call(context||this, -1, xhr.responseText);
						}
					}
					else
					{
						if ( result.ExceptionDetail !== undefined )
							callback.call(context||this, -1, result.ExceptionDetail.Message);
						else
							callback.call(context||this, -1, xhr.responseText);
					}
				}
				catch(e)
				{
					callback.call(context||this, -1, SplendidError.FormatError(e, 'Logout'));
				}
			});
		}
	}
	try
	{
		xhr.send();
	}
	catch(e)
	{
		//alert('Logout: ' + e.message);
		// 03/28/2012 Paul.  IE9 is returning -2146697208 when working offline. 
		if ( e.number != -2146697208 )
			callback.call(context||this, -1, SplendidError.FormatError(e, 'Logout'));
	}
}

chrome.extension.getBackgroundPage().Logout = Logout;
chrome.extension.getBackgroundPage().Login  = Login;

