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
using System;

namespace SplendidCRM
{
	public class KeySortDropDownList : System.Web.UI.WebControls.DropDownList
	{
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.Attributes.Add("onkeypress", "return KeySortDropDownList_onkeypress(this, false)");
			// 10/16/2015 Paul.  fireEvent is not supported in IE 11. 
			// https://msdn.microsoft.com/en-us/library/ff986080(v=vs.85).aspx
			string sFireEvent = "if(document.createEvent) {var evt = document.createEvent('HTMLEvents'); evt.initEvent('change',true,false); this.dispatchEvent(evt); } else if ( this.fireEvent ) { this.fireEvent('onChange'); }";
			this.Attributes.Add("onkeydown" , "if (window.event.keyCode == 13||window.event.keyCode == 9||window.event.keyCode == 27){" + sFireEvent + " onchangefired=true;}");
			this.Attributes.Add("onclick"   , "if (this.selectedIndex!=" + this.SelectedIndex + " && onchangefired==false) {" + sFireEvent + " onchangefired=true;}");
			// 01/13/2010 Paul.  KeySortDropDownList is causing OnChange will always fire when tabbed-away. 
			// This onblur could be the cause, but we are not ready to research the issue further.  
			// It was only an issue in the PARENT_TYPE dropdown, so we will simply not use the KeySort in the Parent Type area. 
			this.Attributes.Add("onblur"    , "if (this.selectedIndex!=" + this.SelectedIndex + " && onchangefired==false) {" + sFireEvent + "}");
		}
	}
}


