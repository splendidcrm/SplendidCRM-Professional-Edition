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
using System.IO;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Xml;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.ComponentModel;
using System.Diagnostics;

namespace SplendidCRM
{
	public class WF4ApprovalResponse
	{
		public string BookmarkName { get; set; }
		public Guid   USER_ID      { get; set; }
		public string RESPONSE     { get; set; }
		public string XML          { get; set; }
	}

	public class WF4ApprovalActivity
	{
		public static void Approve(HttpApplicationState Application, L10N L10n, Guid gID, Guid gUSER_ID)
		{
		}

		public static void Reject(HttpApplicationState Application, Guid gID, Guid gUSER_ID)
		{
		}

		public static void Route(HttpApplicationState Application, L10N L10n, Guid gID, Guid gUSER_ID)
		{
		}

		public static void Claim(HttpApplicationState Application, Guid gID, Guid gUSER_ID)
		{
		}

		public static void Cancel(HttpApplicationState Application, Guid gID, Guid gUSER_ID)
		{
		}

		public static void ChangeProcessUser(HttpApplicationState Application, Guid gID, Guid gPROCESS_USER_ID, string sPROCESS_NOTES)
		{
		}

		public static void ChangeAssignedUser(HttpApplicationState Application, Guid gID, Guid gASSIGNED_USER_ID, string sPROCESS_NOTES)
		{
		}

		public static void Filter(HttpApplicationState Application, IDbCommand cmd, Guid gUSER_ID)
		{
		}

		public static bool GetProcessStatus(HttpApplicationState Application, L10N L10n, Guid gPENDING_PROCESS_ID, ref string sProcessStatus, ref bool bShowApprove, ref bool bShowReject, ref bool bShowRoute, ref bool bShowClaim, ref string sUSER_TASK_TYPE, ref Guid gPROCESS_USER_ID, ref Guid gASSIGNED_TEAM_ID, ref Guid gPROCESS_TEAM_ID)
		{
			return false;
		}

		public static bool IsProcessPending(System.Web.UI.WebControls.DataGridItem Container)
		{
			return false;
		}

		public static void ApplyEditViewPostLoadEventRules(HttpApplicationState Application, L10N L10n, string sEDIT_NAME, SplendidControl parent, DataRow row)
		{
		}

		public static void ApplyEditViewPreSaveEventRules(HttpApplicationState Application, L10N L10n, string sEDIT_NAME, SplendidControl parent, DataRow row)
		{
		}

		public static void ValidateRequiredFields(HttpApplicationState Application, L10N L10n, Guid gPENDING_PROCESS_ID)
		{
		}
	}
}
