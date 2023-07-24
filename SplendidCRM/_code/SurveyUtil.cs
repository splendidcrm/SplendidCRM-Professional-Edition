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
using System.Web.UI.HtmlControls;
using System.Web.Optimization;
using System.Diagnostics;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for SurveyUtil.
	/// </summary>
	public class SurveyUtil
	{
		public static void RegisterScripts(Page Page)
		{
			try
			{
				AjaxControlToolkit.ToolkitScriptManager mgrAjax = ScriptManager.GetCurrent(Page) as AjaxControlToolkit.ToolkitScriptManager;

				// 07/01/2017 Paul.  Use Microsoft ASP.NET Web Optimization 1.1.3 to combine stylesheets and javascript. 
				// 01/24/2018 Paul.  Include version in url to ensure updates of combined files. 
				string sBundleName = "~/Surveys/ScriptsCombined" + "_" + Sql.ToString(HttpContext.Current.Application["SplendidVersion"]);
				Bundle bndScripts = new Bundle(sBundleName);
				// 08/25/2013 Paul.  jQuery now registered in the master pages. 
				// 08/28/2013 Paul.  Also register jQuery in run.aspx as it does not use a master page. 
				bndScripts.Include("~/html5/jQuery/jquery-ui-timepicker-addon.js");
				bndScripts.Include("~/html5/jQuery/jquery.paging.min.js"         );
				// 08/28/2013 Paul.  json2.js now registered in the master pages. 
				//bndScripts.Include("~/html5/JSON.js"                             );
				bndScripts.Include("~/html5/Utility.js"                          );
				// 06/10/2013 Paul.  Fast MD5 algorithm in JavaScript. 
				// http://www.myersdaily.org/joseph/javascript/md5-text.html
				bndScripts.Include("~/html5/md5.js"                              );
				bndScripts.Include("~/html5/SplendidUI/Formatting.js"            );
				bndScripts.Include("~/html5/SplendidUI/Sql.js"                   );
				BundleTable.Bundles.Add(bndScripts);
				Sql.AddScriptReference(mgrAjax, sBundleName);
				
				// 08/25/2013 Paul.  jQuery now registered in the master pages, except for Surveys since they do not use the master pages. 
				// 09/07/2013 Paul.  The jQuery UI stylesheet will now be manually embedded in the page. 
				//Sql.AddStyleSheet(Page, "~/html5/jQuery/jquery-ui-1.9.1.custom.css");
				
				// 07/01/2017 Paul.  Use Microsoft ASP.NET Web Optimization 1.1.3 to combine stylesheets and javascript. 
				// 01/24/2018 Paul.  Include version in url to ensure updates of combined files. 
				sBundleName = "~/Surveys/SurveyScriptsCombined" + "_" + Sql.ToString(HttpContext.Current.Application["SplendidVersion"]);
				Bundle bndSurveyScripts = new Bundle(sBundleName);
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SplendidRequest.js"                );
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/Survey.js"                         );
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyPage.js"                     );
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyQuestion.js"                 );
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyQuestion_Radio.js"           );
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyQuestion_Checkbox.js"        );
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyQuestion_Dropdown.js"        );
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyQuestion_Ranking.js"         );
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyQuestion_RatingScale.js"     );
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyQuestion_RadioMatrix.js"     );
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyQuestion_CheckboxMatrix.js"  );
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyQuestion_DropdownMatrix.js"  );
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyQuestion_TextArea.js"        );
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyQuestion_Textbox.js"         );
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyQuestion_TextboxMultiple.js" );
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyQuestion_TextboxNumerical.js");
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyQuestion_PlainText.js"       );
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyQuestion_Image.js"           );
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyQuestion_Date.js"            );
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyQuestion_Demographic.js"     );
				// 10/08/2014 Paul.  Add Range question type. 
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyQuestion_Range.js"           );
				// 11/07/2018 Paul.  Provide a way to get a single numerical value for lead population.
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyQuestion_SingleNumerical.js" );
				// 11/07/2018 Paul.  Provide a way to get a single date for lead population.
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyQuestion_SingleDate.js"      );
				// 11/10/2018 Paul.  Provide a way to get a single checkbox for lead population.
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyQuestion_SingleCheckbox.js"  );
				// 11/10/2018 Paul.  Provide a way to get a hidden value for lead population.
				bndSurveyScripts.Include("~/Surveys/SurveyScripts/SurveyQuestion_Hidden.js"          );
				BundleTable.Bundles.Add(bndSurveyScripts);
				Sql.AddScriptReference(mgrAjax, sBundleName);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}
	}
}

