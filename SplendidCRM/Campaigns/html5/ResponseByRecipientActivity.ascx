<%@ Control CodeBehind="ResponseByRecipientActivity.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Dashboard.html5.ResponseByRecipientActivity" %>
<script runat="server">
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
</script>
<div id="divHtml5ResponseByRecipientActivity">
	<div align="center">
		<asp:HiddenField ID="hidSERIES_DATA" Value="{}" runat="server" />
		<div style="display: none">
			<asp:ListBox ID="lstACTIVITY_TYPE" DataValueField="NAME" DataTextField="DISPLAY_NAME" SelectionMode="Multiple" Rows="3" Runat="server" />
		</div>

		<SplendidCRM:InlineScript runat="server">
		<script type="text/javascript">
		$(document).ready(function()
		{
			var arrActivityType = new Array();
			var arrActivityTypeValue = new Array();
			var lstACTIVITY_TYPE = document.getElementById('<%# lstACTIVITY_TYPE.ClientID %>');
			for ( var i = 0; i < lstACTIVITY_TYPE.options.length; i++ )
			{
				if ( lstACTIVITY_TYPE.options[i].selected )
				{
					arrActivityType.unshift(lstACTIVITY_TYPE.options[i].text);
					arrActivityTypeValue.unshift(lstACTIVITY_TYPE.options[i].value);
				}
			}

			var data    = $.parseJSON(document.getElementById('<%# hidSERIES_DATA.ClientID %>').value);

			var options = 
			{ stackSeries: true
			, width: 600
			, height: 600
			, title: 
				{ show: true
				}
			, cursor: 
				{ show: true
				, zoom: true
				}
			, seriesDefaults: 
				{ renderer: $.jqplot.BarRenderer
				, rendererOptions: 
					{ barDirection: 'horizontal'
					, fillToZero: true
					}
				, pointLabels: 
					{ show: true
					, stackedValue: true
					, edgeTolerence: -15
					}
				}
			, series: 
				[ { label: '<%# Sql.EscapeJavaScript(Sql.ToString(L10n.Term(".moduleList.", "Contacts" ))) %>', value: 'Contacts' }
				, { label: '<%# Sql.EscapeJavaScript(Sql.ToString(L10n.Term(".moduleList.", "Leads"    ))) %>', value: 'Leads'    }
				, { label: '<%# Sql.EscapeJavaScript(Sql.ToString(L10n.Term(".moduleList.", "Prospects"))) %>', value: 'Prospects'}
				, { label: '<%# Sql.EscapeJavaScript(Sql.ToString(L10n.Term(".moduleList.", "Users"    ))) %>', value: 'Users'    }
				]
			, legend: 
				{ show: true
				, placement: 'outsideGrid'
				, location: 'e'
				}
			, axes: 
				{ yaxis: 
					{ show: true
					, tickRenderer: $.jqplot.CanvasAxisTickRenderer
					, label: ''
					, renderer: $.jqplot.CategoryAxisRenderer
					, ticks: arrActivityType
					}
				, xaxis: 
					{ show: false
					, tickOptions: 
						{ formatString: '%d'
						}
					}
				}
			};
	
			try
			{
				//options.axes.xaxis.label = '<%# Sql.EscapeJavaScript(L10n.Term("Campaigns.LBL_ROLLOVER_VIEW")) %>';
				options.title.text       = '<%# Sql.EscapeJavaScript(L10n.Term("Campaigns.LBL_CAMPAIGN_RESPONSE_BY_RECIPIENT_ACTIVITY")) %>';
				var plot1 = $.jqplot('html5ResponseByRecipientActivity', data, options);
				
				$('#html5ResponseByRecipientActivity').bind('jqplotDataClick', function(ev, seriesIndex, pointIndex, data)
				{
					window.location.href = '#CampaignBookmark_' + arrActivityTypeValue[pointIndex].replace(' ', '_');
				});
				$("#html5ResponseByRecipientActivity").bind('jqplotDataHighlight', function(ev, seriesIndex, pointIndex, data)
				{
					var $this = $(this);
					var sValue = $.jqplot.DefaultTickFormatter(options.axes.xaxis.tickOptions.formatString, data[0]);
					$this.attr('title', sValue + '\n' + options.series[seriesIndex].label + '\n' + arrActivityType[pointIndex]);
				}); 
				$("#html5ResponseByRecipientActivity").bind('jqplotDataUnhighlight', function(ev, seriesIndex, pointIndex, data)
				{
					var $this = $(this);
					$this.attr('title', '');
				});
			}
			catch(e)
			{
				var divChartError = document.getElementById('divChartError_ResponseByRecipientActivity');
				divChartError.innerHTML = 'Chart error: ' + e.message;
			}
		});
		</script>
		</SplendidCRM:InlineScript>
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
		<div id="divChartError_ResponseByRecipientActivity" class="error"></div>
		<div id="html5ResponseByRecipientActivity" style="width: 700px; height: 400px; margin-top:20px; margin-left: auto; margin-right: auto; ">
		</div>
	</div>
</div>

