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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Optimization;
using System.Diagnostics;

namespace SplendidCRM.Home.html5
{
	/// <summary>
	///		Summary description for DashboardView.
	/// </summary>
	public class DashboardView : SplendidControl
	{
		public void RegisterScripts(Page Page)
		{
			try
			{
				AjaxControlToolkit.ToolkitScriptManager mgrAjax = ScriptManager.GetCurrent(Page) as AjaxControlToolkit.ToolkitScriptManager;
#if DEBUG
				//mgrAjax.CombineScripts = false;
#endif
				// 07/01/2017 Paul.  We cannot bundle jquery-ui or zTreeStyle.css as it will change its relative path to images. 
				Sql.AddStyleSheet(this.Page, "~/html5/jQuery/jquery-ui-1.9.1.custom.css"                   );
				// 07/01/2017 Paul.  Cannot combine bootstrap as it pevents automatic loading of font files. 
				Sql.AddStyleSheet(this.Page, "~/html5/bootstrap/3.3.7/css/bootstrap.css"                   );
				// 07/01/2017 Paul.  Cannot combine font-awesome as it pevents automatic loading of font files. 
				Sql.AddStyleSheet(this.Page, "~/html5/fonts/font-awesome.css"                              );
				
				// 07/01/2017 Paul.  Use Microsoft ASP.NET Web Optimization 1.1.3 to combine stylesheets and javascript. 
				// 01/24/2018 Paul.  Include version in url to ensure updates of combined files. 
				string sBundleName = "~/Home/html5/StylesCombined" + "_" + Sql.ToString(Application["SplendidVersion"]);
				Bundle bndStyles = new Bundle(sBundleName);
				bndStyles.Include("~/html5/jQuery/jquery.jqplot.min.css"                                   );
				bndStyles.Include("~/html5/bootstrap/3.3.7/css/bootstrap-theme.css"                        );
				//bndStyles.Include("~/html5/Themes/Six/style.css"                                           );
				bndStyles.Include("~/html5/Atlantic.css"                                                   );
				//bndStyles.Include("~/html5/Themes/Seven/styleModuleHeader.css"                             );
				bndStyles.Include("~/html5/bootstrap/datatables.net/dataTables.bootstrap.css"              );
				bndStyles.Include("~/html5/bootstrap/datatables.net-responsive/responsive.bootstrap.css"   );
				bndStyles.Include("~/html5/bootstrap/gentelella/custom.css"                                );
				BundleTable.Bundles.Add(bndStyles);
				Sql.AddStyleSheet(this.Page, sBundleName);

				// 07/01/2017 Paul.  Use Microsoft ASP.NET Web Optimization 1.1.3 to combine stylesheets and javascript. 
				// 01/24/2018 Paul.  Include version in url to ensure updates of combined files. 
				sBundleName = "~/Home/html5/ScriptsCombined" + "_" + Sql.ToString(Application["SplendidVersion"]);
				Bundle bndScripts = new Bundle(sBundleName);
				bndScripts.Include("~/html5/jQuery/jquery-ui-timepicker-addon.js"                        );
				bndScripts.Include("~/html5/jQuery/jquery.paging.min.js"                                 );
				bndScripts.Include("~/html5/jQuery/jquery.jqplot.min.js"                                 );
				bndScripts.Include("~/html5/jQuery/jquery.jqplot.plugins.min.js"                         );
				//bndScripts.Include("~/html5/FullCalendar/fullcalendar.js"                                );
				BundleTable.Bundles.Add(bndScripts);
				Sql.AddScriptReference(mgrAjax, sBundleName);

				// 04/08/2017 Paul.  Use Bootstrap for responsive design. 
				// 01/24/2018 Paul.  Include version in url to ensure updates of combined files. 
				sBundleName = "~/Home/html5/BootstrapCombined" + "_" + Sql.ToString(Application["SplendidVersion"]);
				Bundle bndBootstrap = new Bundle(sBundleName);
				bndBootstrap.Include("~/html5/bootstrap/3.3.7/js/bootstrap.min.js"                         );
				bndBootstrap.Include("~/html5/bootstrap/datatables.net/jquery.dataTables.js"               );
				bndBootstrap.Include("~/html5/bootstrap/datatables.net/dataTables.bootstrap.js"            );
				bndBootstrap.Include("~/html5/bootstrap/datatables.net-responsive/dataTables.responsive.js");
				bndBootstrap.Include("~/html5/bootstrap/datatables.net-responsive/responsive.bootstrap.js" );
				BundleTable.Bundles.Add(bndBootstrap);
				Sql.AddScriptReference(mgrAjax, sBundleName);
				
				// 07/01/2017 Paul.  Use Microsoft ASP.NET Web Optimization 1.1.3 to combine stylesheets and javascript. 
				// 01/24/2018 Paul.  Include version in url to ensure updates of combined files. 
				sBundleName = "~/Home/html5/SplendidScriptsCombined" + "_" + Sql.ToString(Application["SplendidVersion"]);
				Bundle bndSplendidScripts = new Bundle(sBundleName);
				bndSplendidScripts.Include("~/html5/consolelog.min.js"                    );
				bndSplendidScripts.Include("~/html5/Utility.js"                           );
				bndSplendidScripts.Include("~/html5/SplendidScripts/SystemCacheRequest.js");
				bndSplendidScripts.Include("~/html5/SplendidScripts/SplendidCache.js"     );
				bndSplendidScripts.Include("~/html5/SplendidScripts/Application.js"       );
				bndSplendidScripts.Include("~/html5/SplendidScripts/DetailView.js"        );
				bndSplendidScripts.Include("~/html5/SplendidScripts/ListView.js"          );
				bndSplendidScripts.Include("~/html5/SplendidScripts/EditView.js"          );
				bndSplendidScripts.Include("~/html5/SplendidScripts/Terminology.js"       );
				bndSplendidScripts.Include("~/html5/SplendidScripts/AutoComplete.js"      );
				bndSplendidScripts.Include("~/html5/SplendidScripts/Dashboard.js"         );
				BundleTable.Bundles.Add(bndSplendidScripts);
				Sql.AddScriptReference(mgrAjax, sBundleName);
				
				// 07/01/2017 Paul.  Use Microsoft ASP.NET Web Optimization 1.1.3 to combine stylesheets and javascript. 
				// 01/24/2018 Paul.  Include version in url to ensure updates of combined files. 
				sBundleName = "~/Home/html5/SplendidUICombined" + "_" + Sql.ToString(Application["SplendidVersion"]);
				Bundle bndSplendidUI = new Bundle(sBundleName);
				bndSplendidUI.Include("~/html5/SplendidUI/SplendidErrorUI.js"             );
				bndSplendidUI.Include("~/html5/SplendidUI/SearchBuilder.js"               );
				bndSplendidUI.Include("~/html5/SplendidUI/Sql.js"                         );
				bndSplendidUI.Include("~/html5/SplendidUI/Crm.js"                         );
				bndSplendidUI.Include("~/html5/SplendidUI/Formatting.js"                  );
				bndSplendidUI.Include("~/html5/SplendidUI/TabMenuUI.js"                   );
				bndSplendidUI.Include("~/html5/SplendidUI/TerminologyUI.js"               );
				bndSplendidUI.Include("~/html5/SplendidUI/ListViewUI.js"                  );
				bndSplendidUI.Include("~/html5/SplendidUI/PopupViewUI.js"                 );
				bndSplendidUI.Include("~/html5/SplendidUI/EditViewUI.js"                  );
				bndSplendidUI.Include("~/html5/SplendidUI/SearchViewUI.js"                );
				bndSplendidUI.Include("~/html5/SplendidUI/SplendidInitUI.js"              );
				bndSplendidUI.Include("~/html5/SplendidUI/SelectionUI.js"                 );
				bndSplendidUI.Include("~/html5/SplendidUI/DynamicButtonsUI.js"            );
				bndSplendidUI.Include("~/html5/SplendidUI/LoginViewUI.js"                 );
				bndSplendidUI.Include("~/html5/SplendidUI/DashboardUI.js"                 );
				bndSplendidUI.Include("~/html5/SplendidUI/DashboardEditUI.js"             );
				BundleTable.Bundles.Add(bndSplendidUI);
				Sql.AddScriptReference(mgrAjax, sBundleName);
				
				// 05/19/2017 Paul.  The Dashboard uses RequireJs to load panels. 
				// 05/20/2017 Paul.  Must place require after bootstrap otherwise we get: Mismatched anonymous define() module: function ( $ )  
				Sql.AddScriptReference(mgrAjax, "~/html5/require-2.3.3.min.js"            );
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
