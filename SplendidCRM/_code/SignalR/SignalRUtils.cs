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
using System.Diagnostics;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SplendidCRM.SignalRUtils))]
namespace SplendidCRM
{
	/// <summary>
	/// Summary description for SignalRUtils.
	/// </summary>
	// 09/14/2020 Paul.  Convert to SignalR 2.4.1 
	// https://docs.microsoft.com/en-us/aspnet/signalr/overview/releases/upgrading-signalr-1x-projects-to-20
	public class SignalRUtils
	{
		public void Configuration(IAppBuilder app)
		{
			app.MapSignalR();
		}

		public static void InitApp()
		{
			//IDependencyResolver dependencyResolver = GlobalHost.DependencyResolver;
			//IHubPipeline hubPipeline = GlobalHost.HubPipeline;
			// Uncomment the following line to enable scale-out using SQL Server
			//dependencyResolver.UseSqlServer(System.Configuration.ConfigurationManager.ConnectionStrings["SignalRSamples"].ConnectionString);

			// Uncomment the following line to enable scale-out using Redis
			//var config = new RedisScaleoutConfiguration("127.0.0.1", 6379, "", "SignalRSamples");
			//config.RetryOnError = true;
			//dependencyResolver.UseRedis(config);
			//dependencyResolver.UseRedis("127.0.0.1", 6379, "", "SignalRSamples");

			// Uncomment the following line to enable scale-out using service bus
			//dependencyResolver.UseServiceBus("connection string", "Microsoft.AspNet.SignalR.Samples");

			//hubPipeline.AddModule(new SplendidPipelineModule());
			// Register the default hubs route /signalr
			
			//RouteTable.Routes.MapHubs("/signalr", new HubConfiguration() { EnableDetailedErrors = true } );
			try
			{
				// 12/02/2014 Paul.  Enable Cross Domain for the Mobile Client. 
				// 09/14/2020 Paul.  Convert to SignalR 2.4.1 
				//Microsoft.AspNet.SignalR.HubConfiguration config = new Microsoft.AspNet.SignalR.HubConfiguration();
				//config.EnableCrossDomain = true;
				//RouteTable.Routes.MapHubs(config);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		public static void RegisterSignalR(ScriptManager mgrAjax)
		{
			if ( mgrAjax != null )
			{
				ScriptReference scrSignalR     = new ScriptReference("~/Include/javascript/jquery.signalR-2.4.1.min.js"  );
				ScriptReference scrSignalRHubs = new ScriptReference("~/signalr/hubs"                          );
				ScriptReference scrConnection  = new ScriptReference("~/Include/javascript/connection.start.js");
				if ( !mgrAjax.Scripts.Contains(scrSignalR    ) ) mgrAjax.Scripts.Add(scrSignalR    );
				if ( !mgrAjax.Scripts.Contains(scrSignalRHubs) ) mgrAjax.Scripts.Add(scrSignalRHubs);
				if ( !mgrAjax.Scripts.Contains(scrConnection ) ) mgrAjax.Scripts.Add(scrConnection );
			}
		}
	}
}

