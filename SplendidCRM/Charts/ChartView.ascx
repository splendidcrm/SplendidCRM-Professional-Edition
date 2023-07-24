<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ChartView.ascx.cs" Inherits="SplendidCRM.Charts.ChartView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
	<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
</asp:Panel>

<div id="divChartView">
<script type="text/javascript">
function formatDate(dtValue, sFormat)
{
	if ( dtValue instanceof Date && dtValue.getFullYear() > 1 && sFormat !== undefined && sFormat != null )
	{
		var short_months = new Array('<%= String.Join("', '", System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames) %>');
		var long_months  = new Array('<%= String.Join("', '", System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.MonthNames) %>');
		var yyyy = dtValue.getFullYear();
		var yy   = yyyy.toString().substring(2);
		var M    = dtValue.getMonth() + 1;
		var MM   = M < 10 ? '0' + M : M;
		var MMM  = short_months[M];
		var MMMM = long_months[M];
		var d    = dtValue.getDate();
		var dd   = d < 10 ? '0' + d : d;
		
		var H  = dtValue.getHours();
		var HH = H < 10 ? '0' + H : H;
		var m  = dtValue.getMinutes();
		var mm = m < 10 ? '0' + m : m;
		var s  = dtValue.getSeconds();
		var ss = s < 10 ? '0' + s : s;
		var tt = H < 12 ? 'am' : 'pm';
		var TT = H < 12 ? 'AM' : 'PM';
		var h = H;
		if ( H == 0 )
			h = 12;
		else if ( H > 12 )
			h = H - 12;
		var hh = h < 10 ? '0' + h : h;

		sFormat = sFormat.replace('HH'  , HH  );
		sFormat = sFormat.replace('H'   , H   );
		sFormat = sFormat.replace('yyyy', yyyy);
		sFormat = sFormat.replace('yy'  , yy  );
		sFormat = sFormat.replace('MMMM', MMMM);
		sFormat = sFormat.replace('MMM' , MMM );
		sFormat = sFormat.replace('MM'  , MM  );
		sFormat = sFormat.replace('M'   , M   );
		sFormat = sFormat.replace('dd'  , dd  );
		sFormat = sFormat.replace('d'   , d   );
		sFormat = sFormat.replace('hh'  , hh  );
		sFormat = sFormat.replace('h'   , h   );
		sFormat = sFormat.replace('mm'  , mm  );
		sFormat = sFormat.replace('m'   , m   );
		sFormat = sFormat.replace('ss'  , ss  );
		sFormat = sFormat.replace('s'   , s   );
		sFormat = sFormat.replace('tt'  , tt  );
		sFormat = sFormat.replace('TT'  , TT  );
		return sFormat;
	}
	else
	{
		return '';
	}
}

function FromJsonDate(sDATA_VALUE, formatString)
{
	try
	{
		if ( typeof(sDATA_VALUE) == 'string' )
		{
			// 10/30/2011 Paul.  Not sure why the leading slash is missing. 
			if ( sDATA_VALUE.substr(0, 6) == '/Date(' )
			{
				//alert(sDATA_VALUE.substr(sDATA_VALUE.length - 2, 2) + ' = ' + ')/');
				if ( sDATA_VALUE.substr(sDATA_VALUE.length - 2, 2) == ')/' )
				{
					sDATA_VALUE = sDATA_VALUE.substr(6, sDATA_VALUE.length - 6 - 2);
					var utcTime = parseInt(sDATA_VALUE);
					var dt = new Date(utcTime);
					var off = dt.getTimezoneOffset();
					dt.setMinutes(dt.getMinutes() + off);
					return formatDate(dt, formatString);
					//return dt;
				}
			}
		}
	}
	catch(e)
	{
		alert(dumpObj(e, 'FromJsonDate'));
	}
	return sDATA_VALUE;
}
</script>

<SplendidCRM:InlineScript runat="server">
<script type="text/javascript">
$(document).ready(function()
{
	var data    = <%= GetSeriesData() %>;
	var options = <%= GetChartData()  %>;
	
	for ( var i = 0; i < data.length; i++ )
	{
		for ( var j = 0; j < data[i].length; j++ )
		{
			for ( var k = 0; k < data[i][j].length; k++ )
			{
				data[i][j][k] = FromJsonDate(data[i][j][k], '<%= System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern %>');
			}
		}
	}
	var chart1 = document.getElementById('chart1');
	chart1.style.width  = 600;
	chart1.style.height = 300;
	if ( options.width !== undefined )
		chart1.style.width = options.width;
	if ( options.height !== undefined )
		chart1.style.height = options.height;
	try
	{
		var plot1 = $.jqplot('chart1', data, options);
	}
	catch(e)
	{
		var divChartError = document.getElementById('divChartError');
		divChartError.innerHTML = e.message;
	}
});
</script>
</SplendidCRM:InlineScript>
	<div id="divChartError" class="error"></div>
	<div id="chart1" style="margin-top:20px; margin-left:20px;"></div>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>
