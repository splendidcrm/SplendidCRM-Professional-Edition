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
using System.Data;
using System.Data.Common;
using System.Web.UI.WebControls;

namespace SplendidCRM
{
	public class NewRecordControl : InlineEditControl
	{
		protected string sEditView          = "NewRecord";
		protected Unit   uWidth             = new Unit("100%");
		protected bool   bShowTopButtons    = false;
		protected bool   bShowBottomButtons = true ;
		protected bool   bShowHeader        = true ;
		protected bool   bShowInlineHeader  = false;
		protected bool   bShowFullForm      = false;
		protected bool   bShowCancel        = false;

		// 05/06/2010 Paul.  We need a common way to attach a command from the Toolbar. 
		public CommandEventHandler Command     ;
		// 06/04/2010 Paul.  Generate a load event so that the fields can be populated. 
		public EventHandler        EditViewLoad;

		// 05/05/2010 Paul.  We need a common way to access the parent from the Toolbar. 
		public Guid PARENT_ID
		{
			get
			{
				// 02/21/2010 Paul.  An EditView Inline will use the ViewState, and a NewRecord Inline will use the Request. 
				Guid gPARENT_ID = Sql.ToGuid(ViewState["PARENT_ID"]);
				if ( Sql.IsEmptyGuid(gPARENT_ID) )
					gPARENT_ID = Sql.ToGuid(Request["PARENT_ID"]);
				return gPARENT_ID;
			}
			set
			{
				ViewState["PARENT_ID"] = value;
			}
		}

		public string PARENT_TYPE
		{
			get { return Sql.ToString(ViewState["PARENT_TYPE"]); }
			set { ViewState["PARENT_TYPE"] = value; }
		}

		// 04/19/2010 Paul.  Allow the EditView to be redefined. 
		public string EditView
		{
			get { return sEditView; }
			set { sEditView = value; }
		}

		public Unit Width
		{
			get { return uWidth; }
			set { uWidth = value; }
		}

		public bool ShowTopButtons
		{
			get { return bShowTopButtons; }
			set { bShowTopButtons = value; }
		}

		public bool ShowBottomButtons
		{
			get { return bShowBottomButtons; }
			set { bShowBottomButtons = value; }
		}

		public bool ShowHeader
		{
			get { return bShowHeader; }
			set { bShowHeader = value; }
		}

		public bool ShowInlineHeader
		{
			get { return bShowInlineHeader; }
			set { bShowInlineHeader = value; }
		}

		public bool ShowCancel
		{
			get { return bShowCancel; }
			set { bShowCancel = value; }
		}

		public bool ShowFullForm
		{
			get { return bShowFullForm; }
			set { bShowFullForm = value; }
		}
	}
}

