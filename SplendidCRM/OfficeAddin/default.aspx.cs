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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.Optimization;
using System.Diagnostics;

namespace SplendidCRM.OfficeAddin
{
	/// <summary>
	/// Summary description for Default.
	/// </summary>
	public class Default : SplendidPage
	{
		public Default()
		{
			this.PreInit += new EventHandler(Default_PreInit);
		}

		protected void Default_PreInit(object sender, EventArgs e)
		{
			this.Theme = "";
		}

		override protected bool AuthenticationRequired()
		{
			return false;
		}

		public void RegisterScripts(Page Page)
		{
			try
			{
				AjaxControlToolkit.ToolkitScriptManager mgrAjax = ScriptManager.GetCurrent(Page) as AjaxControlToolkit.ToolkitScriptManager;
				// 07/01/2017 Paul.  Use Microsoft ASP.NET Web Optimization 1.1.3 to combine stylesheets and javascript. 
				// 01/24/2018 Paul.  Include version in url to ensure updates of combined files. 
				string sBundleName = "~/OfficeAddin/StylesCombined" + "_" + Sql.ToString(Application["SplendidVersion"]);
				Bundle bndStyles = new Bundle(sBundleName);
				bndStyles.Include("~/html5/jQuery/contextMenu-1.2.0.css"      );
				bndStyles.Include("~/html5/FullCalendar/fullcalendar.css"     );
				// 07/01/2017 Paul.  Cannot combine font-awesome as it pevents automatic loading of font files. 
				//bndStyles.Include("~/html5/html5/fonts/font-awesome.css"            );
				bndStyles.Include("~/html5/mobile.css"                        );
				bndStyles.Include("~/html5/Atlantic.css"                      );
				bndStyles.Include("~/html5/Themes/Seven/styleModuleHeader.css");
				bndStyles.Include("~/OfficeAddin/Content/Office.css"          );
				bndStyles.Include("~/OfficeAddin/Content/App.css"             );
				bndStyles.Include("~/OfficeAddin/style.css"                   );
				BundleTable.Bundles.Add(bndStyles);
				//Sql.AddStyleSheet(this.Page, sBundleName);
				// 07/01/2017 Paul.  We cannot bundle jquery-ui or zTreeStyle.css as it will change its relative path to images. 
				Sql.AddStyleSheet(this.Page, "~/html5/jQuery/jquery-ui-1.9.1.custom.css");
				
				// 07/01/2017 Paul.  Use Microsoft ASP.NET Web Optimization 1.1.3 to combine stylesheets and javascript. 
				// 01/24/2018 Paul.  Include version in url to ensure updates of combined files. 
				sBundleName = "~/OfficeAddin/ScriptsCombined" + "_" + Sql.ToString(Application["SplendidVersion"]);
				Bundle bndScripts = new Bundle(sBundleName);
				bndScripts.Include("~/html5//jQuery/jquery-1.9.1.min.js"          );
				bndScripts.Include("~/html5//jQuery/jquery-ui-1.9.1.custom.js"    );
				bndScripts.Include("~/html5//jQuery/jquery-ui-timepicker-addon.js");
				bndScripts.Include("~/html5//jQuery/jquery.paging.min.js"         );
				bndScripts.Include("~/html5//jQuery/contextMenu-1.2.0.js"         );
				bndScripts.Include("~/html5//FullCalendar/fullcalendar.js"        );
				bndScripts.Include("~/html5//FullCalendar/gcal.js"                );
				bndScripts.Include("~/html5//JSON.js"                             );
				bndScripts.Include("~/html5//Math.uuid.js"                        );
				bndScripts.Include("~/html5//utility.js"                          );
				bndScripts.Include("~/html5//sha1.js"                             );
				// 01/10/2017 Paul.  Add support for ADFS or Azure AD Single Sign on. 
				bndScripts.Include("~/html5//adal.min.js"                         );
				BundleTable.Bundles.Add(bndScripts);
				//Sql.AddScriptReference(mgrAjax, sBundleName);
				
				// 07/01/2017 Paul.  Use Microsoft ASP.NET Web Optimization 1.1.3 to combine stylesheets and javascript. 
				// 01/24/2018 Paul.  Include version in url to ensure updates of combined files. 
				sBundleName = "~/OfficeAddin/SplendidScriptsCombined" + "_" + Sql.ToString(Application["SplendidVersion"]);
				Bundle bndSplendidScripts = new Bundle(sBundleName);
				// 03/02/2016 Paul.  Use generic console.log to support IE9. 
				bndSplendidScripts.Include("~/html5/consolelog.min.js"                         );
				bndSplendidScripts.Include("~/html5/SplendidScripts/SplendidStorage.js"        );
				bndSplendidScripts.Include("~/html5/SplendidScripts/Credentials.js"            );
				bndSplendidScripts.Include("~/html5/SplendidScripts/SplendidRequest.js"        );
				bndSplendidScripts.Include("~/html5/SplendidScripts/SystemCacheRequest.js"     );
				bndSplendidScripts.Include("~/html5/SplendidScripts/SplendidCache.js"          );
				bndSplendidScripts.Include("~/html5/SplendidScripts/Application.js"            );
				bndSplendidScripts.Include("~/html5/SplendidScripts/Login.js"                  );
				bndSplendidScripts.Include("~/html5/SplendidScripts/Logout.js"                 );
				bndSplendidScripts.Include("~/html5/SplendidScripts/Terminology.js"            );
				bndSplendidScripts.Include("~/html5/SplendidScripts/DetailViewRelationships.js");
				bndSplendidScripts.Include("~/html5/SplendidScripts/TabMenu.js"                );
				bndSplendidScripts.Include("~/html5/SplendidScripts/ListView.js"               );
				bndSplendidScripts.Include("~/html5/SplendidScripts/DetailView.js"             );
				bndSplendidScripts.Include("~/html5/SplendidScripts/EditView.js"               );
				bndSplendidScripts.Include("~/html5/SplendidScripts/DynamicButtons.js"         );
				// 08/20/2016 Paul.  Add Business Process buttons. 
				bndSplendidScripts.Include("~/html5/SplendidScripts/ProcessButtons.js"         );
				bndSplendidScripts.Include("~/html5/SplendidScripts/ModuleUpdate.js"           );
				bndSplendidScripts.Include("~/html5/SplendidScripts/AutoComplete.js"           );
				bndSplendidScripts.Include("~/html5/SplendidScripts/Options.js"                );
				bndSplendidScripts.Include("~/html5/SplendidScripts/CalendarView.js"           );
				BundleTable.Bundles.Add(bndSplendidScripts);
				//Sql.AddScriptReference(mgrAjax, sBundleName);
				
				// 07/01/2017 Paul.  Use Microsoft ASP.NET Web Optimization 1.1.3 to combine stylesheets and javascript. 
				// 01/24/2018 Paul.  Include version in url to ensure updates of combined files. 
				sBundleName = "~/OfficeAddin/SplendidUICombined" + "_" + Sql.ToString(Application["SplendidVersion"]);
				Bundle bndSplendidUI = new Bundle(sBundleName);
				bndSplendidUI.Include("~/html5/SplendidUI/chrome.js"                           );
				bndSplendidUI.Include("~/html5/SplendidUI/SplendidErrorUI.js"                  );
				bndSplendidUI.Include("~/html5/SplendidUI/SearchBuilder.js"                    );
				bndSplendidUI.Include("~/html5/SplendidUI/Sql.js"                              );
				bndSplendidUI.Include("~/html5/SplendidUI/Crm.js"                              );
				bndSplendidUI.Include("~/html5/SplendidUI/Formatting.js"                       );
				bndSplendidUI.Include("~/html5/SplendidUI/TerminologyUI.js"                    );
				bndSplendidUI.Include("~/html5/SplendidUI/TabMenuUI.js"                        );
				bndSplendidUI.Include("~/html5/SplendidUI/TabMenuUI_Six.js"                    );
				bndSplendidUI.Include("~/html5/SplendidUI/TabMenuUI_Atlantic.js"               );
				bndSplendidUI.Include("~/html5/SplendidUI/TabMenuUI_Mobile.js"                 );
				bndSplendidUI.Include("~/html5/SplendidUI/TabMenuUI_OfficeAddin.js"            );
				bndSplendidUI.Include("~/html5/SplendidUI/ListViewUI.js"                       );
				bndSplendidUI.Include("~/html5/SplendidUI/PopupViewUI.js"                      );
				bndSplendidUI.Include("~/html5/SplendidUI/DetailViewUI.js"                     );
				bndSplendidUI.Include("~/html5/SplendidUI/EditViewUI.js"                       );
				bndSplendidUI.Include("~/html5/SplendidUI/SearchViewUI.js"                     );
				bndSplendidUI.Include("~/html5/SplendidUI/SplendidInitUI.js"                   );
				bndSplendidUI.Include("~/html5/SplendidUI/DynamicButtonsUI.js"                 );
				// 08/20/2016 Paul.  Add Business Process buttons. 
				bndSplendidUI.Include("~/html5/SplendidUI/ProcessButtonsUI.js"                 );
				bndSplendidUI.Include("~/html5/SplendidUI/DetailViewRelationshipsUI.js"        );
				bndSplendidUI.Include("~/html5/SplendidUI/SelectionUI.js"                      );
				bndSplendidUI.Include("~/html5/SplendidUI/LoginViewUI.js"                      );
				bndSplendidUI.Include("~/html5/SplendidUI/ArchiveEmailUI.js"                   );
				bndSplendidUI.Include("~/html5/SplendidUI/CalendarViewUI.js"                   );
				bndSplendidUI.Include("~/html5/SplendidUI/EditLineItemsViewUI.js"              );
				BundleTable.Bundles.Add(bndSplendidUI);
				//Sql.AddScriptReference(mgrAjax, sBundleName);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
			RegisterScripts(this.Page);
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

