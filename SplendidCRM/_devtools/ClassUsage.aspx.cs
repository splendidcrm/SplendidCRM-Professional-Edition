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
using System.Collections;
using System.Diagnostics;

namespace SplendidCRM._devtools
{
	struct Usage : IComparable
	{
		private string sName ;
		private int    nValue;

		public Usage(string sName, int nValue)
		{
			this.sName  = sName;
			this.nValue = nValue;
		}

		public int Value
		{
			get { return nValue; }
		}

		public string Name
		{
			get { return sName; }
		}

		public int CompareTo(object obj)
		{
			if ( obj == null )
			{
				return 1;
			}
			Usage that = (Usage)obj;
			return -1 * this.Value.CompareTo(that.Value);
		}
	}

	/// <summary>
	/// Summary description for ClassUsage.
	/// </summary>
	public class ClassUsage : System.Web.UI.Page
	{
		Hashtable hash;

		void ExtractClass(string sAllText)
		{
			string ssAllTextLOWER = sAllText.ToLower();
			int nStart = ssAllTextLOWER.IndexOf("class=\"");
			while ( nStart >= 0 )
			{
				nStart += 7;
				int nEnd = ssAllTextLOWER.IndexOf("\"", nStart);
				if ( nEnd - nStart > 0 )
				{
					string sName = sAllText.Substring(nStart, nEnd - nStart);
					sName = sName.Trim();
					if ( sName.StartsWith("<%= sTAB_CLASS %>") )
					{
						string currentTab = sName.Replace("<%= sTAB_CLASS %>", "currentTab");
						if ( hash.ContainsKey(currentTab) )
						{
							hash[currentTab] = Sql.ToInteger(hash[currentTab]) + 1;
						}
						else
						{
							hash.Add(currentTab, 1);
						}
						string otherTab = sName.Replace("<%= sTAB_CLASS %>", "otherTab");
						if ( hash.ContainsKey(otherTab) )
						{
							hash[otherTab] = Sql.ToInteger(hash[otherTab]) + 1;
						}
						else
						{
							hash.Add(otherTab, 1);
						}
					}
					else if ( sName.Length > 0 && !sName.Contains(".") )
					{
						if ( hash.ContainsKey(sName) )
						{
							hash[sName] = Sql.ToInteger(hash[sName]) + 1;
						}
						else
						{
							hash.Add(sName, 1);
						}
					}
				}
				nStart = ssAllTextLOWER.IndexOf("class=\"", nEnd + 1);;
				
			}
		}

		void RecursiveReadAll(string strDirectory)
		{
			FileInfo objInfo = null;

			string[] arrFiles = Directory.GetFiles(strDirectory);
			for ( int i = 0 ; i < arrFiles.Length ; i++ )
			{
				objInfo = new FileInfo(arrFiles[i]);
				if ( (String.Compare(objInfo.Extension, ".cs", true) == 0 || String.Compare(objInfo.Extension, ".aspx", true) == 0 || String.Compare(objInfo.Extension, ".ascx", true) == 0 || String.Compare(objInfo.Extension, ".asmx", true) == 0 || String.Compare(objInfo.Extension, ".master", true) == 0 || String.Compare(objInfo.Extension, ".skin", true) == 0) && Response.IsClientConnected )
				{
					using (StreamReader rdr = objInfo.OpenText() )
					{
						ExtractClass(rdr.ReadToEnd());
					}
				}
			}
			string[] arrDirectories = Directory.GetDirectories(strDirectory);
			for ( int i = 0 ; i < arrDirectories.Length ; i++ )
			{
				objInfo = new FileInfo(arrDirectories[i]);
				if ( (String.Compare(objInfo.Name, "_vti_cnf", true) != 0) && (String.Compare(objInfo.Name, "_sgbak", true) != 0) )
					RecursiveReadAll(objInfo.FullName);
			}
		}

		void Page_Load(object sender, System.EventArgs e)
		{
			if ( !SplendidCRM.Security.IS_ADMIN || Request.ServerVariables["SERVER_NAME"] != "localhost" )
				return;

			hash = new Hashtable();
			// 08/23/2008 Paul.  Prime the counts using values from the database. 
			/*
				select CONTROL_CSSCLASS, count(*)
				  from DYNAMIC_BUTTONS
				 group by CONTROL_CSSCLASS

				select ITEMSTYLE_CSSCLASS, count(*)
				  from GRIDVIEWS_COLUMNS
				 group by ITEMSTYLE_CSSCLASS
			*/
			hash.Add("button", 665);
			hash.Add("listViewTdLinkS1", 738);
			hash.Add("CreditCardsTdLinkS1", 2);

			RecursiveReadAll(Server.MapPath(".."));
			try
			{
				Response.Write("<html><body><table border=1 cellpadding=0 cellspacing=0>");
				ArrayList sorted = new ArrayList();

				foreach ( DictionaryEntry entry in hash )
				{
					sorted.Add(new Usage(entry.Key.ToString(), Sql.ToInteger(entry.Value)));
				}
				sorted.Sort();
				foreach ( Usage entry in sorted )
				{
					Response.Write("<tr><td>" + entry.Name + "</td><td>" + entry.Value + "</td></tr>" + ControlChars.CrLf);
				}
				Response.Write("</table></body></html>");
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				Response.Write(ex.Message + ControlChars.CrLf);
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
