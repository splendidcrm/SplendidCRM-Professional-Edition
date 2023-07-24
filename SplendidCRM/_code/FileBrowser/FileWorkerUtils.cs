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
using System.Data;
using System.Web;
using System.Diagnostics;

namespace SplendidCRM.FileBrowser
{
	public class FileWorkerUtils
	{
		// 11/06/2010 Paul.  Move LoadFile() to Crm.EmailImages. 

		public static void LoadImage(ref Guid gImageID, ref string sFILENAME, IDbTransaction trn)
		{
			// 04/26/2012 Paul.  CKEditor change the name to upload. 
			LoadImage(ref gImageID, ref sFILENAME, "upload", trn);
		}

		// 08/09/2009 Paul.  We need to allow the field name to be a parameter so that this code can be reused. 
		public static void LoadImage(ref Guid gImageID, ref string sFILENAME, string sHTML_FIELD_NAME, IDbTransaction trn)
		{
			gImageID = Guid.Empty;
			HttpPostedFile pstIMAGE  = HttpContext.Current.Request.Files[sHTML_FIELD_NAME];
			if ( pstIMAGE != null )
			{
				long lFileSize      = pstIMAGE.ContentLength;
				long lUploadMaxSize = Sql.ToLong(HttpContext.Current.Application["CONFIG.upload_maxsize"]);
				if ( (lUploadMaxSize > 0) && (lFileSize > lUploadMaxSize) )
				{
					throw(new Exception("ERROR: uploaded file was too big: max filesize: " + lUploadMaxSize.ToString()));
				}
				// 04/13/2005 Paul.  File may not have been provided. 
				if ( pstIMAGE.FileName.Length > 0 )
				{
					sFILENAME              = Path.GetFileName (pstIMAGE.FileName);
					string sFILE_EXT       = Path.GetExtension(sFILENAME);
					string sFILE_MIME_TYPE = pstIMAGE.ContentType;
					
					SqlProcs.spEMAIL_IMAGES_Insert
						( ref gImageID
						, Guid.Empty // gParentID
						, sFILENAME
						, sFILE_EXT
						, sFILE_MIME_TYPE
						, trn
						);
					// 09/06/2008 Paul.  PostgreSQL does not require that we stream the bytes, so lets explore doing this for all platforms. 
					// 10/18/2009 Paul.  Move blob logic to LoadFile. 
					Crm.EmailImages.LoadFile(gImageID, pstIMAGE.InputStream, trn);
				}
			}
		}
	}
}

