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

namespace SplendidCRM
{
	public class SubPanelControl : InlineEditControl
	{
		protected bool bEditView;

		public bool IsEditView
		{
			get { return bEditView; }
			set { bEditView = value; }
		}

		protected UniqueGuidCollection GetDeletedEditViewRelationships()
		{
			UniqueGuidCollection arrDELETED = ViewState[m_sMODULE + ".Deleted"] as UniqueGuidCollection;
			if ( arrDELETED == null )
				arrDELETED = new UniqueGuidCollection();
			return arrDELETED;
		}

		protected UniqueGuidCollection GetUpdatedEditViewRelationships()
		{
			UniqueGuidCollection arrUPDATED = ViewState[m_sMODULE + ".Updated"] as UniqueGuidCollection;
			if ( arrUPDATED == null )
				arrUPDATED = new UniqueGuidCollection();
			return arrUPDATED;
		}

		protected void DeleteEditViewRelationship(Guid gDELETE_ID)
		{
			// 01/27/2010 Paul.  Keep a separate list of removed items. 
			UniqueGuidCollection arrDELETED = GetDeletedEditViewRelationships();
			arrDELETED.Add(gDELETE_ID);
			ViewState[m_sMODULE + ".Deleted"] = arrDELETED;
			
			UniqueGuidCollection arrUPDATED = ViewState[m_sMODULE + ".Updated"] as UniqueGuidCollection;
			if ( arrUPDATED != null )
			{
				arrUPDATED.Remove(gDELETE_ID);
				ViewState[m_sMODULE + ".Updated"] = arrUPDATED;
			}
		}

		protected void UpdateEditViewRelationship(Guid gUPDATE_ID)
		{
			UniqueGuidCollection arrUPDATED = GetUpdatedEditViewRelationships();
			arrUPDATED.Add(gUPDATE_ID);
			ViewState[m_sMODULE + ".Updated"] = arrUPDATED;
			
			// 01/27/2010 Paul.  Just in case the user is adding back a record that he previous removed. 
			UniqueGuidCollection arrDELETED = ViewState[m_sMODULE + ".Deleted"] as UniqueGuidCollection;
			if ( arrDELETED != null )
			{
				arrDELETED.Remove(gUPDATE_ID);
				ViewState[m_sMODULE + ".Deleted"] = arrDELETED;
			}
		}

		protected void UpdateEditViewRelationship(string[] arrID)
		{
			UniqueGuidCollection arrUPDATED = GetUpdatedEditViewRelationships();
			foreach(string item in arrID)
			{
				Guid gUPDATE_ID = Sql.ToGuid(item);
				arrUPDATED.Add(gUPDATE_ID);
			}
			ViewState[m_sMODULE + ".Updated"] = arrUPDATED;
			
			UniqueGuidCollection arrDELETED = ViewState[m_sMODULE + ".Deleted"] as UniqueGuidCollection;
			if ( arrDELETED != null )
			{
				foreach(string item in arrID)
				{
					Guid gUPDATE_ID = Sql.ToGuid(item);
					arrDELETED.Remove(gUPDATE_ID);
				}
				ViewState[m_sMODULE + ".Deleted"] = arrDELETED;
			}
		}

		protected void CreateEditViewRelationships(DataTable dt, string sPrimaryField)
		{
			UniqueGuidCollection arrUPDATED = new UniqueGuidCollection();
			foreach ( DataRow row in dt.Rows )
			{
				Guid gUPDATE_ID = Sql.ToGuid(row[sPrimaryField]);
				arrUPDATED.Add(gUPDATE_ID);
			}
			ViewState[m_sMODULE + ".Updated"] = arrUPDATED;
		}
	}
}

