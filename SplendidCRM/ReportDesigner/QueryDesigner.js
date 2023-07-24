/*
 * Copyright (C) 2005-2023 SplendidCRM Software, Inc. All rights reserved.
 *
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License 
 * Agreement, or other written agreement between you and SplendidCRM ("License"). By installing or 
 * using this file, you have unconditionally agreed to the terms and conditions of the License, 
 * including but not limited to restrictions on the number of users therein, and you may not use this 
 * file except in compliance with the License. 
 * 
 */

// 07/05/2016 Paul.  Use more unique name for modules definition array. 
var arrReportDesignerModules = null;
// 07/04/2016 Paul.  Special case when not showing selected fields. 
var bReportDesignerWorkflowMode = false;
// 07/17/2016 Paul.  Allow the filter operator to be changed to a workflow version. 
var report_filter_operator_dom = 'report_filter_operator_dom';

function ReportDesigner_FindModuleByTable(sTableName)
{
	if ( arrReportDesignerModules != null )
	{
		for ( var i = 0; i < arrReportDesignerModules.length; i++ )
		{
			if ( sTableName == arrReportDesignerModules[i].TableName )
			{
				return arrReportDesignerModules[i];
			}
		}
	}
	return null;
}

function ReportDesigner_FindModuleByName(sModuleName)
{
	if ( arrReportDesignerModules != null )
	{
		for ( var i = 0; i < arrReportDesignerModules.length; i++ )
		{
			if ( sModuleName == arrReportDesignerModules[i].ModuleName )
			{
				return arrReportDesignerModules[i];
			}
		}
	}
	return null;
}

// 07/11/2016 Paul.  Find field by table or module. 
function ReportDesigner_FindFieldByTable(sTableName, sColumnName)
{
	if ( arrReportDesignerModules != null )
	{
		var module = ReportDesigner_FindModuleByTable(sTableName);
		if ( module != null )
		{
			var arrFields = module.Fields;
			for ( var j = 0; j < arrFields.length; j++ )
			{
				if ( sColumnName == arrFields[j].ColumnName )
				{
					return arrFields[j];
				}
			}
		}
	}
	return null;
}

function ReportDesigner_FindFieldByModule(sModuleName, sColumnName)
{
	if ( arrReportDesignerModules != null )
	{
		var module = ReportDesigner_FindModuleByName(sModuleName);
		if ( module != null )
		{
			var arrFields = module.Fields;
			for ( var j = 0; j < arrFields.length; j++ )
			{
				if ( sColumnName == arrFields[j].ColumnName )
				{
					return arrFields[j];
				}
			}
		}
	}
	return null;
}

function ReportDesigner_CreateTableNodes(oReportFilter, oSelectedNode)
{
	var zNodes = new Array();
	try
	{
		var oCurrentTables = new Object();
		oCurrentTables.name     = L10n.Term('ReportDesigner.LBL_TABLES_IN_QUERY');
		oCurrentTables.open     = true;
		oCurrentTables.children = new Array();
		zNodes.push(oCurrentTables);
		for ( var i = 0; i < oReportDesign.Tables.length; i++ )
		{
			var table = new ReportTable(oReportDesign.Tables[i].Module);
			// 07/15/2016 Paul.  Set the id for the table to the table name. 
			table.id       = table.TableName;
			table.name     = table.DisplayName;
			table.children = new Array();
			if ( oReportFilter.Field != null && oReportFilter.Field.Module.TableName == oReportDesign.Tables[i].Module.TableName )
			{
				table.open = true;
			}
			else if ( oReportFilter.Field == null && i == 0 )
			{
				table.open = true;
			}
		
			var arrModuleFields = oReportDesign.ModuleFields(table.TableName);
			for ( var j = 0; j < arrModuleFields.length; j++ )
			{
				var field = arrModuleFields[j];
				var oFieldNode = new Object();
				oFieldNode.id    = field.TableName + '.' + field.ColumnName;
				oFieldNode.Field = field;
				oFieldNode.name  = field.DisplayName;
				if ( bDebug )
					oFieldNode.name += ' (' + field.TableName + '.' + field.ColumnName + ')';
				if ( oReportFilter.Field != null && oReportFilter.Field.TableName == field.TableName && oReportFilter.Field.ColumnName == field.ColumnName )
				{
					oSelectedNode = oFieldNode;
					//oSelectedNode.curSelectedNode = true;
				}
				table.children.push(oFieldNode);
			}
			oCurrentTables.children.push(table);
		}
	}
	catch(e)
	{
		alert('ReportDesigner_CreateTableNodes: ' + e.message);
	}
//MAX_DUMP_DEPTH = 4;
//alert(dumpObj(zNodes, 'zNodes'));
	return zNodes;
}

function pad(str, len, padChar)
{
	str = str.toString();
	if ( typeof padChar === "undefined")
	{
		padChar = ' ';
	}
	while ( str.length < len )
	{
		str = padChar + str;
	}
	return str;
}

function ReportField(field, bGroupAndAggregate)
{
	if ( bGroupAndAggregate === undefined )
		bGroupAndAggregate = false;
	this.Field         = field                  ;
	this.TableName     = field.TableName        ;
	this.ColumnName    = field.ColumnName       ;
	this.FieldName     = field.Module.TableName + '.' + field.ColumnName;
	this.AggregateType = (bGroupAndAggregate ? 'group by' : null);
	this.DisplayName   = field.Module.DisplayName + ' ' + field.DisplayName;
	this.DisplayWidth  = null;
	this.SortDirection = null;
}

function ReportTable(module)
{
	this.Module      = module            ;
	this.ModuleName  = module.ModuleName ;
	this.DisplayName = module.DisplayName;
	this.TableName   = module.TableName  ;
	if ( bDebug )
		this.DisplayName += ' (' + module.TableName + ')';
}

function ReportRelationship()
{
	this.LeftTable  = null;
	this.JoinType   = 'inner';
	this.RightTable = null;
	this.JoinFields = new Array();
}

function JoinFieldsDisplayText(arrJoinFields)
{
	var sJoinFields = L10n.Term('ReportDesigner.LBL_EDIT_INSTRUCTIONS');
	if ( arrJoinFields != null && arrJoinFields.length > 0 )
	{
		sJoinFields = '';
		for ( var i = 0; i < arrJoinFields.length; i++ )
		{
			var oJoinField = arrJoinFields[i];
			if ( sJoinFields.length > 0 )
				sJoinFields += '<br /> and ';
			if ( bDebug )
				sJoinFields += oJoinField.LeftField.DisplayName + ' (' + oJoinField.LeftField.FieldName + ') ' + oJoinField.OperatorType + ' ' + oJoinField.RightField.DisplayName + ' (' + oJoinField.RightField.FieldName + ') ';
			else
				sJoinFields += oJoinField.LeftField.DisplayName + ' ' + oJoinField.OperatorType + ' ' + oJoinField.RightField.DisplayName;
		}
	}
	return sJoinFields;
}

function ReportJoinField(oJoinField)
{
	if ( oJoinField !== undefined )
	{
		this.LeftField    = oJoinField.LeftField   ;
		this.OperatorType = oJoinField.OperatorType;
		this.RightField   = oJoinField.RightField  ;
	}
	else
	{
		this.LeftField    = null;
		this.OperatorType = '=';
		this.RightField   = null;
	}
}

ReportJoinField.prototype.SetLeftFieldName = function(sFieldName)
{
	var arrFieldName = sFieldName.split('.');
	var nRelationshipIndex = tRelationships_SelectedIndex();
	var oReportRelationship = oReportDesign.Relationships_GetAt(nRelationshipIndex);
	var arrModuleFields = oReportDesign.ModuleFields(oReportRelationship.LeftTable.TableName);
	for ( var i = 0; i < arrModuleFields.length; i++ )
	{
		if ( arrFieldName[1] == arrModuleFields[i].ColumnName )
		{
			this.LeftField = new ReportField(arrModuleFields[i]);
			break;
		}
	}
}

ReportJoinField.prototype.SetRightFieldName = function(sFieldName)
{
	var arrFieldName = sFieldName.split('.');
	var nRelationshipIndex = tRelationships_SelectedIndex();
	var oReportRelationship = oReportDesign.Relationships_GetAt(nRelationshipIndex);
	var arrModuleFields = oReportDesign.ModuleFields(oReportRelationship.RightTable.TableName);
	for ( var i = 0; i < arrModuleFields.length; i++ )
	{
		if ( arrFieldName[1] == arrModuleFields[i].ColumnName )
		{
			this.RightField = new ReportField(arrModuleFields[i]);
			break;
		}
	}
}

function ReportFilter()
{
	this.Field     = null;
	this.Operator  = '=';
	this.Value     = null;
	this.Parameter = false;
}

ReportFilter.prototype.IsNumericField = function()
{
	var b = false;
	if ( this.Field != null )
	{
		switch ( this.Field.DataType )
		{
			case 'ansistring':  b = false;  break;
			case 'bool'      :  b = false;  break;
			case 'byte[]'    :  b = false;  break;
			case 'DateTime'  :  b = false;  break;
			case 'decimal'   :  b = true ;  break;
			case 'float'     :  b = true ;  break;
			case 'Guid'      :  b = false;  break;
			case 'Int16'     :  b = true ;  break;
			case 'Int32'     :  b = true ;  break;
			case 'Int64'     :  b = true ;  break;
			case 'short'     :  b = true ;  break;
			case 'string'    :  b = false;  break;
		}
	}
	return b;
}

// 02/11/2018 Paul.  We need to determine if the string should be treated as a enum. 
ReportFilter.prototype.IsEnum = function()
{
	var b = false;
	if ( this.Field != null && this.Field.Module != null )
	{
		var lay = ReportDesign_EditView_Layout(this.Field.Module.ModuleName, this.Field.ColumnName);
		if ( lay != null )
		{
			if ( !Sql.IsEmptyString(lay.LIST_NAME) )
				b = true;
		}
	}
	return b;
}

ReportFilter.prototype.CsType = function()
{
	var sCsType = this.Field.DataType.toLowerCase();
	if ( this.IsEnum() )
		sCsType = 'enum';
	return sCsType;
}

ReportFilter.prototype.IsDateField = function()
{
	var b = false;
	if ( this.Field != null )
	{
		if ( this.Field.DataType == 'DateTime' )
			b = true;
	}
	return b;
}

ReportFilter.prototype.IsBooleanField = function()
{
	var b = false;
	if ( this.Field != null )
	{
		if ( this.Field.DataType == 'bool' )
			b = true;
	}
	return b;
}

ReportFilter.prototype.EscapedValue = function(sValue)
{
	var sSQLValue = null;
	if ( this.IsNumericField() )
		sSQLValue = sValue;
	else
		sSQLValue = '\'' + Sql.EscapeSQL(sValue) + '\'';
	return sSQLValue;
}

ReportFilter.prototype.EscapedLikeValue = function(sValue)
{
	var sSQLValue = '\'' + Sql.EscapeSQLLike(sValue) + '\'';
	return sSQLValue;
}

function ReportDesign()
{
	this.GroupAndAggregate = false;
	this.Tables            = new Array();
	this.SelectedFields    = new Array();
	this.Relationships     = new Array();
	this.AppliedFilters    = new Array();
}

// 08/03/2014 Paul.  Simplfy the object module by returning just the raw fields, not deep object references. 
ReportDesign.prototype.Stringify = function()
{
	var oRAW = new Object();
	oRAW.GroupAndAggregate = false;
	oRAW.Tables            = new Array();
	oRAW.SelectedFields    = new Array();
	oRAW.Relationships     = new Array();
	oRAW.AppliedFilters    = new Array();
	
	oRAW.GroupAndAggregate = oReportDesign.GroupAndAggregate;
	if ( oReportDesign.Tables != null )
	{
		for ( var i = 0; i < oReportDesign.Tables.length; i++ )
		{
			var table = new Object();
			table.ModuleName = oReportDesign.Tables[i].ModuleName ;
			table.TableName  = oReportDesign.Tables[i].TableName  ;
			oRAW.Tables.push(table);
		}
	}
	if ( oReportDesign.SelectedFields != null )
	{
		for ( var i = 0; i < oReportDesign.SelectedFields.length; i++ )
		{
			if ( oReportDesign.SelectedFields[i].Field != null )
			{
				var field = new Object();
				field.TableName     = oReportDesign.SelectedFields[i].Field.TableName;
				field.ColumnName    = oReportDesign.SelectedFields[i].ColumnName     ;
				field.FieldName     = oReportDesign.SelectedFields[i].FieldName      ;
				field.AggregateType = oReportDesign.SelectedFields[i].AggregateType  ;
				field.DisplayName   = oReportDesign.SelectedFields[i].DisplayName    ;
				field.DisplayWidth  = oReportDesign.SelectedFields[i].DisplayWidth   ;
				field.SortDirection = oReportDesign.SelectedFields[i].SortDirection  ;
				oRAW.SelectedFields.push(field);
			}
		}
	}
	if ( oReportDesign.Relationships != null )
	{
		for ( var i = 0; i < oReportDesign.Relationships.length; i++ )
		{
			if ( oReportDesign.Relationships[i].LeftTable != null && oReportDesign.Relationships[i].RightTable != null )
			{
				var relationship = new Object();
				relationship.LeftTableName  = oReportDesign.Relationships[i].LeftTable.TableName ;
				relationship.JoinType       = oReportDesign.Relationships[i].JoinType            ;
				relationship.RightTableName = oReportDesign.Relationships[i].RightTable.TableName;
				relationship.JoinFields = new Array();
				var joins = oReportDesign.Relationships[i].JoinFields;
				for ( var j = 0; j < joins.length; j++ )
				{
					var join = new Object();
					join.LeftTableName   = joins[j].LeftField.TableName  ;
					join.LeftColumnName  = joins[j].LeftField.ColumnName ;
					join.OperatorType    = joins[j].OperatorType         ;
					join.RightTableName  = joins[j].RightField.TableName ;
					join.RightColumnName = joins[j].RightField.ColumnName;
					relationship.JoinFields.push(join);
				}
				oRAW.Relationships.push(relationship);
			}
		}
	}
	if ( oReportDesign.AppliedFilters != null )
	{
		for ( var i = 0; i < oReportDesign.AppliedFilters.length; i++ )
		{
			if ( oReportDesign.AppliedFilters[i].Field != null )
			{
				var filter = new Object();
				filter.TableName  = oReportDesign.AppliedFilters[i].Field.TableName ;
				filter.ColumnName = oReportDesign.AppliedFilters[i].Field.ColumnName;
				filter.Operator   = oReportDesign.AppliedFilters[i].Operator        ;
				filter.Value      = oReportDesign.AppliedFilters[i].Value           ;
				filter.Parameter  = oReportDesign.AppliedFilters[i].Parameter       ;
				oRAW.AppliedFilters.push(filter);
			}
		}
	}
	return JSON.stringify(oRAW);
}

// 08/03/2014 Paul.  Simplfy the object module by returning just the raw fields, not deep object references. 
ReportDesign.prototype.Parse = function(sReportJson)
{
	this.GroupAndAggregate = false;
	this.Tables            = new Array();
	this.SelectedFields    = new Array();
	this.Relationships     = new Array();
	this.AppliedFilters    = new Array();
	if ( sReportJson !== undefined )
	{
		var oRAW = jQuery.parseJSON(sReportJson);
		if ( oRAW.GroupAndAggregate !== undefined )
			oReportDesign.GroupAndAggregate = oRAW.GroupAndAggregate;
		if ( oRAW.Tables !== undefined )
		{
			for ( var i = 0; i < oRAW.Tables.length; i++ )
			{
				var module = ReportDesigner_FindModuleByTable(oRAW.Tables[i].TableName);
				var table = new ReportTable(module);
				oReportDesign.Tables.push(table);
			}
		}
		if ( oRAW.SelectedFields !== undefined )
		{
			for ( var i = 0; i < oRAW.SelectedFields.length; i++ )
			{
				if ( oRAW.SelectedFields[i].TableName !== undefined && oRAW.SelectedFields[i].ColumnName !== undefined )
				{
					var oBaseField = ReportDesigner_FindFieldByTable(oRAW.SelectedFields[i].TableName, oRAW.SelectedFields[i].ColumnName);
					if ( oBaseField != null )
					{
						var field = new ReportField(oBaseField, oReportDesign.GroupAndAggregate);
						if ( oRAW.SelectedFields[i].AggregateType !== undefined ) field.AggregateType = oRAW.SelectedFields[i].AggregateType;
						if ( oRAW.SelectedFields[i].DisplayName   !== undefined ) field.DisplayName   = oRAW.SelectedFields[i].DisplayName  ;
						if ( oRAW.SelectedFields[i].DisplayWidth  !== undefined ) field.DisplayWidth  = oRAW.SelectedFields[i].DisplayWidth ;
						if ( oRAW.SelectedFields[i].SortDirection !== undefined ) field.SortDirection = oRAW.SelectedFields[i].SortDirection;
						oReportDesign.SelectedFields.push(field);
					}
				}
			}
		}
		if ( oRAW.Relationships !== undefined )
		{
			for ( var i = 0; i < oRAW.Relationships.length; i++ )
			{
				var oLeftModule  = ReportDesigner_FindModuleByTable(oRAW.Relationships[i].LeftTableName );
				var oRightModule = ReportDesigner_FindModuleByTable(oRAW.Relationships[i].RightTableName);
				if ( oLeftModule != null && oRightModule != null )
				{
					var relationship = new ReportRelationship();
					relationship.LeftTable  = new ReportTable(oLeftModule );
					relationship.JoinType   = oRAW.Relationships[i].JoinType;
					relationship.RightTable = new ReportTable(oRightModule);
					relationship.JoinFields = new Array();
					var joins = oRAW.Relationships[i].JoinFields;
					for ( var j = 0; j < joins.length; j++ )
					{
						var oLeftField  = ReportDesigner_FindFieldByTable(joins[j].LeftTableName , joins[j].LeftColumnName );
						var oRightField = ReportDesigner_FindFieldByTable(joins[j].RightTableName, joins[j].RightColumnName);
						if ( oLeftField != null && oRightField != null )
						{
							var join = new ReportJoinField();
							join.LeftField    = new ReportField(oLeftField );
							join.OperatorType = joins[j].OperatorType       ;
							join.RightField   = new ReportField(oRightField);
							relationship.JoinFields.push(join);
						}
					}
					oReportDesign.Relationships.push(relationship);
				}
			}
		}
		if ( oRAW.AppliedFilters !== undefined )
		{
			for ( var i = 0; i < oRAW.AppliedFilters.length; i++ )
			{
				//if ( Sql.IsEmptyString(oRAW.AppliedFilters[i].TableName) )
				//	alert('Missing table name in filter ' + i.toString() + ', ' + oRAW.AppliedFilters[i].ColumnName);
				var oBaseField = ReportDesigner_FindFieldByTable(oRAW.AppliedFilters[i].TableName, oRAW.AppliedFilters[i].ColumnName);
				if ( oBaseField != null )
				{
					var filter = new ReportFilter();
					filter.Field     = oBaseField;
					filter.Operator  = oRAW.AppliedFilters[i].Operator ;
					filter.Value     = oRAW.AppliedFilters[i].Value    ;
					filter.Parameter = oRAW.AppliedFilters[i].Parameter;
					oReportDesign.AppliedFilters.push(filter);
					
					//alert(dumpObj(filter.Field, 'Field'));
				}
			}
		}
	}
	for ( var i = 0; i < oReportDesign.SelectedFields.length; i++ )
	{
		tSelectedFields_AddField(oReportDesign.SelectedFields[i].Field, oReportDesign.SelectedFields[i])
	}
	for ( var i = 0; i < oReportDesign.Relationships.length; i++ )
	{
		tRelationships_AddRelationship(oReportDesign.Relationships[i])
	}
	for ( var i = 0; i < oReportDesign.AppliedFilters.length; i++ )
	{
		tAppliedFilters_AddFilter(oReportDesign.AppliedFilters[i])
	}
}

ReportDesign.prototype.Tables_AddTable = function(module)
{
	var bFound = false;
	for ( var i = 0; i < this.Tables.length; i++ )
	{
		if ( this.Tables[i].TableName == module.TableName )
		{
			bFound = true;
			break;
		}
	}
	if ( !bFound )
	{
		var table = new ReportTable(module);
		this.Tables.push(table);
	}
}

ReportDesign.prototype.Tables_RemoveTable = function(sTableName)
{
	for ( var i = 0; i < this.Tables.length; i++ )
	{
		if ( this.Tables[i].TableName == sTableName )
		{
			this.Tables.splice(i, 1);
			break;
		}
	}
}

ReportDesign.prototype.SelectedField_AddField = function(field)
{
	var sFieldName = field.TableName + '.' + field.ColumnName;
	
	var oReportField = null;
	var bFound = false;
	for ( var i = 0; i < this.SelectedFields.length; i++ )
	{
		oReportField = this.SelectedFields[i];
		if ( oReportField.FieldName == sFieldName )
		{
			bFound = true;
			break;
		}
	}
	if ( !bFound )
	{
		oReportField = new ReportField(field, this.GroupAndAggregate);
		this.SelectedFields.push(oReportField);
		this.Tables_AddTable(field.Module);
	}
	return oReportField;
}

ReportDesign.prototype.Tables_UpdateAll = function()
{
	this.Tables = new Array();
	for ( var i = 0; i < this.SelectedFields.length; i++ )
	{
		var field = this.SelectedFields[i].Field;
		this.Tables_AddTable(field.Module);
	}
	for ( var i = 0; i < this.Relationships.length; i++ )
	{
		var relationship = this.Relationships[i];
		if ( relationship.LeftTable != null )
			this.Tables_AddTable(relationship.LeftTable.Module );
		if ( relationship.RightTable != null )
			this.Tables_AddTable(relationship.RightTable.Module);
	}
}

ReportDesign.prototype.SelectedField_RemoveField = function(sFieldName)
{
	for ( var i = 0; i < this.SelectedFields.length; i++ )
	{
		var oReportField = this.SelectedFields[i];
		if ( oReportField.FieldName == sFieldName )
		{
			this.SelectedFields.splice(i, 1);
			break;
		}
	}
	this.Tables_UpdateAll();
}

ReportDesign.prototype.SelectedField_GetAt = function(nSelectedIndex)
{
	return this.SelectedFields[nSelectedIndex];
}

ReportDesign.prototype.SelectedField_Delete = function(nSelectedIndex)
{
	this.SelectedFields.splice(nSelectedIndex, 1);
}

ReportDesign.prototype.SelectedField_MoveUp = function(nSelectedIndex)
{
	var oReportField = this.SelectedFields[nSelectedIndex];
	this.SelectedFields.splice(nSelectedIndex, 1);
	this.SelectedFields.splice(nSelectedIndex - 1, 0, oReportField);
}

ReportDesign.prototype.SelectedField_MoveDown = function(nSelectedIndex)
{
	var oReportField = this.SelectedFields[nSelectedIndex];
	this.SelectedFields.splice(nSelectedIndex, 1);
	this.SelectedFields.splice(nSelectedIndex + 1, 0, oReportField);
}

ReportDesign.prototype.Relationships_AddRelationship = function()
{
	var oRelationship = new ReportRelationship();
	this.Relationships.push(oRelationship);
	return oRelationship;
}

ReportDesign.prototype.Relationships_GetAt = function(nSelectedIndex)
{
	return this.Relationships[nSelectedIndex];
}

ReportDesign.prototype.Relationships_Delete = function(nSelectedIndex)
{
	this.Relationships.splice(nSelectedIndex, 1);
}

ReportDesign.prototype.Relationships_MoveUp = function(nSelectedIndex)
{
	var oRelationship = this.Relationships[nSelectedIndex];
	this.Relationships.splice(nSelectedIndex, 1);
	this.Relationships.splice(nSelectedIndex - 1, 0, oRelationship);
}

ReportDesign.prototype.Relationships_MoveDown = function(nSelectedIndex)
{
	var oRelationship = this.Relationships[nSelectedIndex];
	this.Relationships.splice(nSelectedIndex, 1);
	this.Relationships.splice(nSelectedIndex + 1, 0, oRelationship);
}

ReportDesign.prototype.Relationships_JoinField_GetAt = function(nRelationshipIndex, nSelectedIndex)
{
	var arrRelationships = this.Relationships[nRelationshipIndex];
	return arrRelationships.JoinFields[nSelectedIndex];
}

ReportDesign.prototype.ModuleFields = function(sTableName)
{
	var arrFields = null;
	for ( var i = 0; i < arrReportDesignerModules.length; i++ )
	{
		if ( arrReportDesignerModules[i].TableName == sTableName )
		{
			arrFields = arrReportDesignerModules[i].Fields;
			break;
		}
	}
	return arrFields;
}

ReportDesign.prototype.AppliedFilters_AddFilter = function()
{
	var oFilter = new ReportFilter();
	this.AppliedFilters.push(oFilter);
	return oFilter;
}

ReportDesign.prototype.AppliedFilters_GetAt = function(nSelectedIndex)
{
	return this.AppliedFilters[nSelectedIndex];
}

ReportDesign.prototype.AppliedFilters_Delete = function(nSelectedIndex)
{
	this.AppliedFilters.splice(nSelectedIndex, 1);
}

ReportDesign.prototype.AppliedFilters_MoveUp = function(nSelectedIndex)
{
	var oFilter = this.AppliedFilters[nSelectedIndex];
	this.AppliedFilters.splice(nSelectedIndex, 1);
	this.AppliedFilters.splice(nSelectedIndex - 1, 0, oFilter);
}

ReportDesign.prototype.AppliedFilters_MoveDown = function(nSelectedIndex)
{
	var oFilter = this.AppliedFilters[nSelectedIndex];
	this.AppliedFilters.splice(nSelectedIndex, 1);
	this.AppliedFilters.splice(nSelectedIndex + 1, 0, oFilter);
}

ReportDesign.prototype.PreviewSQL = function()
{
	var sSQL = '';
	var CrLf = '\r\n';
	var sErrors = '';
	if ( this.Tables.length > 0 )
	{
		var oUsedTables = new Object();
		for ( var i = 0; i < this.Tables.length; i++ )
		{
			oUsedTables[this.Tables[i].TableName] = 0;
		}
		// 07/04/2016 Paul.  Special case when not showing selected fields. 
		if ( bReportDesignerWorkflowMode )
		{
			if ( this.SelectedFields.length == 0 )
			{
				for ( var i = 0; i < this.Tables.length; i++ )
				{
					sSQL = 'select ' + this.Tables[i].TableName + '.ID' + CrLf;
					break;
				}
			}
			else
			{
				for ( var i = 0; i < this.SelectedFields.length; i++ )
				{
					var oReportField = this.SelectedFields[i];
					sSQL = 'select ' + oReportField.FieldName + CrLf;
					break;
				}
			}
		}
		else if ( this.SelectedFields.length == 0 )
		{
			sSQL += 'select *' + CrLf;
		}
		else
		{
			var nMaxLen = 0;
			for ( var i = 0; i < this.SelectedFields.length; i++ )
			{
				var oReportField = this.SelectedFields[i];
				nMaxLen = Math.max(nMaxLen, oReportField.FieldName.length);
			}
			for ( var i = 0; i < this.SelectedFields.length; i++ )
			{
				var oReportField = this.SelectedFields[i];
				sSQL += (i == 0 ? 'select ' : '     , ');
				if ( !Sql.IsEmptyString(oReportField.AggregateType) )
				{
					switch ( oReportField.AggregateType )
					{
						case 'group by'        :  sSQL += oReportField.FieldName + pad('', nMaxLen - oReportField.FieldName.length, ' ');  break;
						case 'avg'             :  sSQL += 'avg'    + '('          + oReportField.FieldName + ')';  break;
						case 'count'           :  sSQL += 'count'  + '('          + oReportField.FieldName + ')';  break;
						case 'min'             :  sSQL += 'min'    + '('          + oReportField.FieldName + ')';  break;
						case 'max'             :  sSQL += 'max'    + '('          + oReportField.FieldName + ')';  break;
						case 'stdev'           :  sSQL += 'stdev'  + '('          + oReportField.FieldName + ')';  break;
						case 'stdevp'          :  sSQL += 'stdevp' + '('          + oReportField.FieldName + ')';  break;
						case 'sum'             :  sSQL += 'sum'    + '('          + oReportField.FieldName + ')';  break;
						case 'var'             :  sSQL += 'var'    + '('          + oReportField.FieldName + ')';  break;
						case 'varp'            :  sSQL += 'varp'   + '('          + oReportField.FieldName + ')';  break;
						case 'avg distinct'    :  sSQL += 'avg'    + '(distinct ' + oReportField.FieldName + ')';  break;
						case 'count distinct'  :  sSQL += 'count'  + '(distinct ' + oReportField.FieldName + ')';  break;
						case 'stdev distinct'  :  sSQL += 'stdev'  + '(distinct ' + oReportField.FieldName + ')';  break;
						case 'stdevp distinct' :  sSQL += 'stdevp' + '(distinct ' + oReportField.FieldName + ')';  break;
						case 'sum distinct'    :  sSQL += 'sum'    + '(distinct ' + oReportField.FieldName + ')';  break;
						case 'var distinct'    :  sSQL += 'var'    + '(distinct ' + oReportField.FieldName + ')';  break;
						case 'varp distinct'   :  sSQL += 'varp'   + '(distinct ' + oReportField.FieldName + ')';  break;
						default                :  sSQL += '\'Unknown AggregateType\'';  break;
					}
				}
				else
				{
					sSQL += oReportField.FieldName + pad('', nMaxLen - oReportField.FieldName.length, ' ');
				}
				sSQL += ' as \"' + oReportField.FieldName + '\"';
				sSQL += CrLf;
			}
		}
		if ( this.Relationships.length == 0 )
		{
			sSQL += '  from vw' + this.Tables[0].TableName + ' ' + this.Tables[0].TableName + CrLf;
			oUsedTables[this.Tables[0].TableName] += 1;
		}
		else
		{
			for ( var i = 0; i < this.Relationships.length; i++ )
			{
				var sJoinType = '';
				var sJoinTypeSpacer = '';
				var oReportRelationship = this.Relationships[i];
				switch ( oReportRelationship.JoinType )
				{
					case 'inner'      :  sJoinType = ' inner join '      ;  sJoinTypeSpacer = '        '      ;  break;
					case 'left outer' :  sJoinType = '  left outer join ';  sJoinTypeSpacer = '              ';  break;
					case 'right outer':  sJoinType = ' right outer join ';  sJoinTypeSpacer = '              ';  break;
					case 'full outer' :  sJoinType = '  full outer join ';  sJoinTypeSpacer = '              ';  break;
				}
				// 04/08/2020 Paul.  PreviewSQL may be called before the join tables have been specified. 
				if ( oReportRelationship.LeftTable == null || oReportRelationship.RightTable == null )
				{
					sErrors += L10n.Term('ReportDesigner.LBL_MISSING_JOIN_TABLE');
					if ( i == 0 )
					{
						sSQL += '  from vw' + this.Tables[0].TableName + ' ' + this.Tables[0].TableName + CrLf;
						oUsedTables[this.Tables[0].TableName] += 1;
					}
					continue;
				}
				if ( i == 0 )
				{
					if ( oReportRelationship.LeftTable != null && oReportRelationship.RightTable != null )
					{
						sSQL += '  from vw' + oReportRelationship.LeftTable.TableName + ' ' + oReportRelationship.LeftTable.TableName + CrLf;
						// 02/24/2015 Paul.  Need to prime the object list before incrementing. 
						if ( oUsedTables[oReportRelationship.LeftTable.TableName] === undefined )
							oUsedTables[oReportRelationship.LeftTable.TableName] = 0;
						oUsedTables[oReportRelationship.LeftTable.TableName] += 1;
						sSQL += sJoinType + 'vw' + oReportRelationship.RightTable.TableName + ' ' + oReportRelationship.RightTable.TableName + CrLf;
						// 02/24/2015 Paul.  Need to prime the object list before incrementing. 
						if ( oUsedTables[oReportRelationship.RightTable.TableName] === undefined )
							oUsedTables[oReportRelationship.RightTable.TableName] = 0;
						oUsedTables[oReportRelationship.RightTable.TableName] += 1;
						if ( oReportRelationship.JoinFields == null || oReportRelationship.JoinFields.length == 0 )
						{
							sErrors += L10n.Term('ReportDesigner.LBL_MISSING_JOIN_FIELDS').replace('{0}', oReportRelationship.LeftTable.TableName).replace('{1}', oReportRelationship.RightTable.TableName) + '<br />' + CrLf;
						}
						else
						{
							for ( var j = 0; j < oReportRelationship.JoinFields.length; j++ )
							{
								var oJoinField = oReportRelationship.JoinFields[j];
								sSQL += sJoinTypeSpacer + (j == 0 ? ' on ' : 'and ') + oJoinField.RightField.FieldName + ' ' + oJoinField.OperatorType + ' ' + oJoinField.LeftField.FieldName + CrLf;
							}
						}
					}
				}
				else if ( oUsedTables[oReportRelationship.LeftTable.TableName] > 0 && oUsedTables[oReportRelationship.RightTable.TableName] > 0 )
				{
					sErrors += L10n.Term('ReportDesigner.LBL_COMBINE_RELATIONSHIPS').replace('{0}', oReportRelationship.LeftTable.TableName).replace('{1}', oReportRelationship.RightTable.TableName) + '<br />' + CrLf;
				}
				else if ( oUsedTables[oReportRelationship.LeftTable.TableName] > 0 )
				{
					sSQL += sJoinType + 'vw' + oReportRelationship.RightTable.TableName + " " + oReportRelationship.RightTable.TableName + CrLf;
					// 02/24/2015 Paul.  Need to prime the object list before incrementing. 
					if ( oUsedTables[oReportRelationship.RightTable.TableName] === undefined )
						oUsedTables[oReportRelationship.RightTable.TableName] = 0;
					oUsedTables[oReportRelationship.RightTable.TableName] += 1;
					if ( oReportRelationship.JoinFields == null || oReportRelationship.JoinFields.length == 0 )
					{
						sErrors += L10n.Term('ReportDesigner.LBL_MISSING_JOIN_FIELDS').replace('{0}', oReportRelationship.LeftTable.TableName).replace('{1}', oReportRelationship.RightTable.TableName) + '<br />' + CrLf;
					}
					else
					{
						for ( var j = 0; j < oReportRelationship.JoinFields.length; j++ )
						{
							var oJoinField = oReportRelationship.JoinFields[j];
							sSQL += sJoinTypeSpacer + (j == 0 ? ' on ' : 'and ') + oJoinField.RightField.FieldName + ' ' + oJoinField.OperatorType + ' ' + oJoinField.LeftField.FieldName + CrLf;
						}
					}
				}
				else if ( oUsedTables[oReportRelationship.RightTable.TableName] > 0 )
				{
					// 01/06/2014 Paul.  If left table does not exist in query, then switch the join type. 
					switch ( oReportRelationship.JoinType )
					{
						case 'left outer' :  sJoinType = ' right outer join ';  break;
						case 'right outer':  sJoinType = '  left outer join ';  break;
					}
					sSQL += sJoinType + 'vw' + oReportRelationship.LeftTable.TableName + ' ' + oReportRelationship.LeftTable.TableName + CrLf;
					// 02/24/2015 Paul.  Need to prime the object list before incrementing. 
					if ( oUsedTables[oReportRelationship.LeftTable.TableName] === undefined )
						oUsedTables[oReportRelationship.LeftTable.TableName] = 0;
					oUsedTables[oReportRelationship.LeftTable.TableName] += 1;
					if ( oReportRelationship.JoinFields == null || oReportRelationship.JoinFields.length == 0 )
					{
						sErrors += L10n.Term('ReportDesigner.LBL_MISSING_JOIN_FIELDS').replace('{0}', oReportRelationship.LeftTable.TableName).replace('{1}', oReportRelationship.RightTable.TableName) + '<br />' + CrLf;
					}
					else
					{
						for ( var j = 0; j < oReportRelationship.JoinFields.length; j++ )
						{
							var oJoinField = oReportRelationship.JoinFields[j];
							sSQL += sJoinTypeSpacer + (j == 0 ? ' on ' : 'and ') + oJoinField.RightField.FieldName + ' ' + oJoinField.OperatorType + ' ' + oJoinField.LeftField.FieldName + CrLf;
						}
					}
				}
				else
				{
					alert(dumpObj(oUsedTables, null));
					alert('Missing case RightTable: ' + oReportRelationship.RightTable.TableName + ', LeftTable: ' + oReportRelationship.LeftTable.TableName);
				}
			}
		}
		if ( this.AppliedFilters.length > 0 )
		{
			// 07/17/2016 Paul.  Add support for changed to support workflow. 
			// Look for the first occurence of a changed field, then add the audit join. 
			for ( var i = 0; i < this.AppliedFilters.length; i++ )
			{
				var oReportFilter = this.AppliedFilters[i];
				var field = oReportFilter.Field;
				// 07/17/2016 Paul.  Change event only applies to first table. 
				if ( oReportFilter.Operator == 'changed' && field.TableName == this.Tables[0].TableName )
				{
					//  left outer join vwACCOUNTS_AUDIT      ACCOUNTS_AUDIT_OLD
					//               on ACCOUNTS_AUDIT_OLD.ID = ACCOUNTS.ID
					//              and ACCOUNTS_AUDIT_OLD.AUDIT_VERSION = (select max(vwACCOUNTS_AUDIT.AUDIT_VERSION)
					//                                                  from vwACCOUNTS_AUDIT
					//                                                 where vwACCOUNTS_AUDIT.ID            =  ACCOUNTS.ID
					//                                                   and vwACCOUNTS_AUDIT.AUDIT_VERSION <  ACCOUNTS.AUDIT_VERSION
					//                                                   and vwACCOUNTS_AUDIT.AUDIT_TOKEN   <> ACCOUNTS.AUDIT_TOKEN
					//                                               )
					sSQL += '  left outer join vw' + field.TableName + '_AUDIT        '   + field.TableName + '_AUDIT_OLD' + CrLf;
					sSQL += '               on '   + field.TableName + '_AUDIT_OLD.ID = ' + field.TableName + '.ID' + CrLf;
					sSQL += '              and '   + field.TableName + '_AUDIT_OLD.AUDIT_VERSION = (select max(vw' + field.TableName + '_AUDIT.AUDIT_VERSION)' + CrLf;
					sSQL += '                                                  from vw' + field.TableName + '_AUDIT' + CrLf;
					sSQL += '                                                 where vw' + field.TableName + '_AUDIT.ID            =  ' + field.TableName + '.ID' + CrLf;
					sSQL += '                                                   and vw' + field.TableName + '_AUDIT.AUDIT_VERSION <  ' + field.TableName + '.AUDIT_VERSION' + CrLf;
					sSQL += '                                                   and vw' + field.TableName + '_AUDIT.AUDIT_TOKEN   <> ' + field.TableName + '.AUDIT_TOKEN' + CrLf;
					sSQL += '                                               )' + CrLf;
					break;
				}
			}
			for ( var i = 0; i < this.AppliedFilters.length; i++ )
			{
				var oReportFilter = this.AppliedFilters[i];
				var field = oReportFilter.Field;
				if ( field == null )
				{
					sErrors += L10n.Term('ReportDesigner.LBL_MISSING_FILTER_FIELD').replace('{0}', i.toString()) + '<br />' + CrLf;
				}
				else if ( oReportFilter.Operator == null || oReportFilter.Operator == '' )
				{
					sErrors += L10n.Term('ReportDesigner.LBL_MISSING_FILTER_OPERATOR').replace('{0}', field.TableName + '.' + field.ColumnName) + '<br />' + CrLf;
				}
				// 07/17/2016 Paul.  Add support for changed to support workflow. 
				// 08/17/2018 Paul.  Need to include empty and not_empty for workflow mode. 
				else if ( oReportFilter.Value == null && (oReportFilter.Operator != 'empty' && oReportFilter.Operator != 'not_empty' && oReportFilter.Operator != 'is null' && oReportFilter.Operator != 'is not null' && oReportFilter.Operator != 'changed') && !oReportFilter.Parameter )
				{
					sErrors += L10n.Term('ReportDesigner.LBL_MISSING_FILTER_VALUE').replace('{0}', field.TableName + '.' + field.ColumnName) + '<br />' + CrLf;
				}
				else if ( (oReportFilter.Value == null || oReportFilter.Value == '') && (oReportFilter.IsNumericField() || oReportFilter.IsDateField() || oReportFilter.IsBooleanField()) && !oReportFilter.Parameter )
				{
					sErrors += L10n.Term('ReportDesigner.LBL_MISSING_FILTER_VALUE').replace('{0}', field.TableName + '.' + field.ColumnName) + '<br />' + CrLf;
				}
				else
				{
					if ( i == 0 )
						sSQL += ' where ';
					else
						sSQL += '   and ';
					// 07/17/2016 Paul.  Add support for changed to support workflow. 
					if ( oReportFilter.Operator == 'changed' )
					{
						// 07/17/2016 Paul.  Change event only applies to first table. 
						if ( field.TableName == this.Tables[0].TableName )
						{
							//   and (ACCOUNTS_AUDIT_OLD.AUDIT_ID is null or (not(ACCOUNTS.ASSIGNED_USER_ID is null and ACCOUNTS_AUDIT_OLD.ASSIGNED_USER_ID is null) and (ACCOUNTS.ASSIGNED_USER_ID <> ACCOUNTS_AUDIT_OLD.ASSIGNED_USER_ID or ACCOUNTS.ASSIGNED_USER_ID is null or ACCOUNTS_AUDIT_OLD.ASSIGNED_USER_ID is null)))
							sSQL += '(' + field.TableName + '_AUDIT_OLD.AUDIT_ID is null or (not(' + field.TableName + '.' + field.ColumnName + ' is null and ' + field.TableName + '_AUDIT_OLD.' + field.ColumnName + ' is null    ) and (' + field.TableName + '.' + field.ColumnName + ' <> ' + field.TableName + '_AUDIT_OLD.' + field.ColumnName + ' or ' + field.TableName + '.' + field.ColumnName + ' is null or ' + field.TableName + '_AUDIT_OLD.' + field.ColumnName + ' is null)))' + CrLf;
						}
					}
					// 02/11/2018 Paul.  Workflow mode uses older style of operators. 
					else if ( bReportDesignerWorkflowMode )
					{
						var bIsOracle     = false;
						var bIsDB2        = false;
						var bIsMySQL      = false;
						var bIsPostgreSQL = false;
						var sCAT_SEP = (bIsOracle ? " || " : " + ");
						var sOPERATOR         = oReportFilter.Operator;
						var sCOMMON_DATA_TYPE = oReportFilter.Field.DataType.toLowerCase();
						if ( sCOMMON_DATA_TYPE == "ansistring" )
							sCOMMON_DATA_TYPE = "string";
						// 02/11/2018 Paul.  We need to determine if the string should be treated as a enum. 
						if ( oReportFilter.IsEnum() )
							sCOMMON_DATA_TYPE = "enum";
						var sSEARCH_TEXT1 = '@' + field.ColumnName;
						switch ( sCOMMON_DATA_TYPE )
						{
							case "string":
							{
								switch ( sOPERATOR )
								{
									case "equals"         :  sSQL += field.TableName + '.' + field.ColumnName + " = "    + sSEARCH_TEXT1;  break;
									case "less"           :  sSQL += field.TableName + '.' + field.ColumnName + " < "    + sSEARCH_TEXT1;  break;
									case "less_equal"     :  sSQL += field.TableName + '.' + field.ColumnName + " <= "   + sSEARCH_TEXT1;  break;
									case "greater"        :  sSQL += field.TableName + '.' + field.ColumnName + " > "    + sSEARCH_TEXT1;  break;
									case "greater_equal"  :  sSQL += field.TableName + '.' + field.ColumnName + " >= "   + sSEARCH_TEXT1;  break;
									case "contains"       :  sSQL += field.TableName + '.' + field.ColumnName + " like " + "N'%'" + sCAT_SEP + sSEARCH_TEXT1 + sCAT_SEP + "N'%'";  break;
									case "starts_with"    :  sSQL += field.TableName + '.' + field.ColumnName + " like " +                     sSEARCH_TEXT1 + sCAT_SEP + "N'%'";  break;
									case "ends_with"      :  sSQL += field.TableName + '.' + field.ColumnName + " like " + "N'%'" + sCAT_SEP + sSEARCH_TEXT1;  break;
									case "like"           :  sSQL += field.TableName + '.' + field.ColumnName + " like " + "N'%'" + sCAT_SEP + sSEARCH_TEXT1 + sCAT_SEP + "N'%'";  break;
									case "empty"          :  sSQL += field.TableName + '.' + field.ColumnName + " is null"    ;  break;
									case "not_empty"      :  sSQL += field.TableName + '.' + field.ColumnName + " is not null";  break;
									// 10/25/2014 Paul.  Filters that use NOT should protect against NULL values. 
									case "not_equals_str" :  sSQL += 'coalesce(' + field.TableName + '.' + field.ColumnName + ", N'')" + " <> "   + sSEARCH_TEXT1;  break;
									case "not_contains"   :  sSQL += 'coalesce(' + field.TableName + '.' + field.ColumnName + ", N'')" + " not like " + "N'%'" + sCAT_SEP + sSEARCH_TEXT1 + sCAT_SEP + "N'%'";  break;
									case "not_starts_with":  sSQL += 'coalesce(' + field.TableName + '.' + field.ColumnName + ", N'')" + " not like " +                     sSEARCH_TEXT1 + sCAT_SEP + "N'%'";  break;
									case "not_ends_with"  :  sSQL += 'coalesce(' + field.TableName + '.' + field.ColumnName + ", N'')" + " not like " + "N'%'" + sCAT_SEP + sSEARCH_TEXT1;  break;
									case "not_like"       :  sSQL += 'coalesce(' + field.TableName + '.' + field.ColumnName + ", N'')" + " not like " + "N'%'" + sCAT_SEP + sSEARCH_TEXT1 + sCAT_SEP + "N'%'";  break;
								}
								break;
							}
							case "datetime":
							{
								var fnPrefix = "dbo.";
								if ( bIsOracle || bIsDB2 || bIsMySQL || bIsPostgreSQL )
								{
									fnPrefix = "";
								}
								switch ( sOPERATOR )
								{
									case "on"               :  sSQL += fnPrefix + 'fnDateOnly(' + field.TableName + '.' + field.ColumnName + ") = "  + sSEARCH_TEXT1;  break;
									case "before"           :  sSQL += fnPrefix + 'fnDateOnly(' + field.TableName + '.' + field.ColumnName + ") < "  + sSEARCH_TEXT1;  break;
									case "after"            :  sSQL += fnPrefix + 'fnDateOnly(' + field.TableName + '.' + field.ColumnName + ") > "  + sSEARCH_TEXT1;  break;
									case "not_equals_str"   :  sSQL += fnPrefix + 'fnDateOnly(' + field.TableName + '.' + field.ColumnName + ") <> " + sSEARCH_TEXT1;  break;
									case "between_dates"    :  sSQL += fnPrefix + 'fnDateOnly(' + field.TableName + '.' + field.ColumnName + ") between " + ' @' + field.ColumnName + '_AFTER' + " and " + '@' + field.ColumnName + '_BEFORE';  break;
									case "tp_days_after"    :  sSQL += "TODAY()"   + " > "       + fnPrefix + "fnDateAdd('day', "    +       sSEARCH_TEXT1        + ", " + fnPrefix + 'fnDateOnly(' + field.TableName + '.' + field.ColumnName + "))";  break;
									case "tp_weeks_after"   :  sSQL += "TODAY()"   + " > "       + fnPrefix + "fnDateAdd('week', "   +       sSEARCH_TEXT1        + ", " + fnPrefix + 'fnDateOnly(' + field.TableName + '.' + field.ColumnName + "))";  break;
									case "tp_months_after"  :  sSQL += "TODAY()"   + " > "       + fnPrefix + "fnDateAdd('month', "  +       sSEARCH_TEXT1        + ", " + fnPrefix + 'fnDateOnly(' + field.TableName + '.' + field.ColumnName + "))";  break;
									case "tp_years_after"   :  sSQL += "TODAY()"   + " > "       + fnPrefix + "fnDateAdd('year', "   +       sSEARCH_TEXT1        + ", " + fnPrefix + 'fnDateOnly(' + field.TableName + '.' + field.ColumnName + "))";  break;
									case "tp_days_before"   :  sSQL += "TODAY()"   + " between " + fnPrefix + "fnDateAdd('day', "    + "-" + sSEARCH_TEXT1        + ", " + fnPrefix + 'fnDateOnly(' + field.TableName + '.' + field.ColumnName + ")) and " + fnPrefix + 'fnDateOnly(' + field.TableName + '.' + field.ColumnName + ")";  break;
									case "tp_weeks_before"  :  sSQL += "TODAY()"   + " between " + fnPrefix + "fnDateAdd('week', "   + "-" + sSEARCH_TEXT1        + ", " + fnPrefix + 'fnDateOnly(' + field.TableName + '.' + field.ColumnName + ")) and " + fnPrefix + 'fnDateOnly(' + field.TableName + '.' + field.ColumnName + ")";  break;
									case "tp_months_before" :  sSQL += "TODAY()"   + " between " + fnPrefix + "fnDateAdd('month', "  + "-" + sSEARCH_TEXT1        + ", " + fnPrefix + 'fnDateOnly(' + field.TableName + '.' + field.ColumnName + ")) and " + fnPrefix + 'fnDateOnly(' + field.TableName + '.' + field.ColumnName + ")";  break;
									case "tp_years_before"  :  sSQL += "TODAY()"   + " between " + fnPrefix + "fnDateAdd('year', "   + "-" + sSEARCH_TEXT1        + ", " + fnPrefix + 'fnDateOnly(' + field.TableName + '.' + field.ColumnName + ")) and " + fnPrefix + 'fnDateOnly(' + field.TableName + '.' + field.ColumnName + ")";  break;
									case "tp_minutes_after" :  sSQL += "GETDATE()" + " between " + fnPrefix + "fnDateAdd('minute', " +       sSEARCH_TEXT1        + ", " + field.TableName + '.' + field.ColumnName                             + ") and " + fnPrefix + "fnDateAdd('minute', " + "1+" + sSEARCH_TEXT1 + ", " + field.TableName + '.' + field.ColumnName + ")";  break;
									case "tp_hours_after"   :  sSQL += "GETDATE()" + " between " + fnPrefix + "fnDateAdd('hour', "   +       sSEARCH_TEXT1        + ", " + field.TableName + '.' + field.ColumnName                             + ") and " + fnPrefix + "fnDateAdd('hour', "   + "1+" + sSEARCH_TEXT1 + ", " + field.TableName + '.' + field.ColumnName + ")";  break;
									case "tp_minutes_before":  sSQL += "GETDATE()" + " between " + fnPrefix + "fnDateAdd('minute', " + "-" + sSEARCH_TEXT1 + "-1" + ", " + field.TableName + '.' + field.ColumnName                             + ") and " + fnPrefix + "fnDateAdd('minute', " + "-"  + sSEARCH_TEXT1 + ", " + field.TableName + '.' + field.ColumnName + ")";  break;
									case "tp_hours_before"  :  sSQL += "GETDATE()" + " between " + fnPrefix + "fnDateAdd('hour', "   + "-" + sSEARCH_TEXT1 + "-1" + ", " + field.TableName + '.' + field.ColumnName                             + ") and " + fnPrefix + "fnDateAdd('hour', "   + "-"  + sSEARCH_TEXT1 + ", " + field.TableName + '.' + field.ColumnName + ")";  break;
									case "tp_days_old"      :  sSQL += "TODAY()"   + " = "       + fnPrefix + "fnDateAdd('day', "    +       sSEARCH_TEXT1        + ", " + fnPrefix + 'fnDateOnly(' + field.TableName + '.' + field.ColumnName + "))";  break;
									case "tp_weeks_old"     :  sSQL += "TODAY()"   + " = "       + fnPrefix + "fnDateAdd('week', "   +       sSEARCH_TEXT1        + ", " + fnPrefix + 'fnDateOnly(' + field.TableName + '.' + field.ColumnName + "))";  break;
									case "tp_months_old"    :  sSQL += "TODAY()"   + " = "       + fnPrefix + "fnDateAdd('month', "  +       sSEARCH_TEXT1        + ", " + fnPrefix + 'fnDateOnly(' + field.TableName + '.' + field.ColumnName + "))";  break;
									case "tp_years_old"     :  sSQL += "TODAY()"   + " = "       + fnPrefix + "fnDateAdd('year', "   +       sSEARCH_TEXT1        + ", " + fnPrefix + 'fnDateOnly(' + field.TableName + '.' + field.ColumnName + "))";  break;
								}
								break;
							}
							case "int32":
							{
								switch ( sOPERATOR )
								{
									case "equals"       :  sSQL += field.TableName + '.' + field.ColumnName + " = "    + sSEARCH_TEXT1;  break;
									case "less"         :  sSQL += field.TableName + '.' + field.ColumnName + " < "    + sSEARCH_TEXT1;  break;
									case "greater"      :  sSQL += field.TableName + '.' + field.ColumnName + " > "    + sSEARCH_TEXT1;  break;
									case "not_equals"   :  sSQL += field.TableName + '.' + field.ColumnName + " <> "   + sSEARCH_TEXT1;  break;
									case "between"      :  sSQL += field.TableName + '.' + field.ColumnName + " between "   + ' @' + field.ColumnName + '_AFTER' + " and " + '@' + field.ColumnName + '_BEFORE';  break;
									case "empty"        :  sSQL += field.TableName + '.' + field.ColumnName + " is null"    ;  break;
									case "not_empty"    :  sSQL += field.TableName + '.' + field.ColumnName + " is not null";  break;
									case "less_equal"   :  sSQL += field.TableName + '.' + field.ColumnName + " <= "    + sSEARCH_TEXT1;  break;
									case "greater_equal":  sSQL += field.TableName + '.' + field.ColumnName + " >= "    + sSEARCH_TEXT1;  break;
								}
								break;
							}
							case "decimal":
							{
								switch ( sOPERATOR )
								{
									case "equals"       :  sSQL += field.TableName + '.' + field.ColumnName + " = "    + sSEARCH_TEXT1;  break;
									case "less"         :  sSQL += field.TableName + '.' + field.ColumnName + " < "    + sSEARCH_TEXT1;  break;
									case "greater"      :  sSQL += field.TableName + '.' + field.ColumnName + " > "    + sSEARCH_TEXT1;  break;
									case "not_equals"   :  sSQL += field.TableName + '.' + field.ColumnName + " <> "   + sSEARCH_TEXT1;  break;
									case "between"      :  sSQL += field.TableName + '.' + field.ColumnName + " between "   + ' @' + field.ColumnName + '_AFTER' + " and " + '@' + field.ColumnName + '_BEFORE';  break;
									case "empty"        :  sSQL += field.TableName + '.' + field.ColumnName + " is null"    ;  break;
									case "not_empty"    :  sSQL += field.TableName + '.' + field.ColumnName + " is not null";  break;
									case "less_equal"   :  sSQL += field.TableName + '.' + field.ColumnName + " <= "    + sSEARCH_TEXT1;  break;
									case "greater_equal":  sSQL += field.TableName + '.' + field.ColumnName + " >= "    + sSEARCH_TEXT1;  break;
								}
								break;
							}
							case "float":
							{
								switch ( sOPERATOR )
								{
									case "equals"       :  sSQL += field.TableName + '.' + field.ColumnName + " = "    + sSEARCH_TEXT1;  break;
									case "less"         :  sSQL += field.TableName + '.' + field.ColumnName + " < "    + sSEARCH_TEXT1;  break;
									case "greater"      :  sSQL += field.TableName + '.' + field.ColumnName + " > "    + sSEARCH_TEXT1;  break;
									case "not_equals"   :  sSQL += field.TableName + '.' + field.ColumnName + " <> "   + sSEARCH_TEXT1;  break;
									case "between"      :  sSQL += field.TableName + '.' + field.ColumnName + " between "   + ' @' + field.ColumnName + '_AFTER' + " and " + '@' + field.ColumnName + '_BEFORE';  break;
									case "empty"        :  sSQL += field.TableName + '.' + field.ColumnName + " is null"    ;  break;
									case "not_empty"    :  sSQL += field.TableName + '.' + field.ColumnName + " is not null";  break;
									case "less_equal"   :  sSQL += field.TableName + '.' + field.ColumnName + " <= "    + sSEARCH_TEXT1;  break;
									case "greater_equal":  sSQL += field.TableName + '.' + field.ColumnName + " >= "    + sSEARCH_TEXT1;  break;
								}
								break;
							}
							case "bool":
							{
								switch ( sOPERATOR )
								{
									case "equals"    :  sSQL += field.TableName + '.' + field.ColumnName + " = "    + sSEARCH_TEXT1;  break;
									case "empty"     :  sSQL += field.TableName + '.' + field.ColumnName + " is null"    ;  break;
									case "not_empty" :  sSQL += field.TableName + '.' + field.ColumnName + " is not null";  break;
								}
								break;
							}
							case "guid":
							{
								switch ( sOPERATOR )
								{
									case "is"             :  sSQL += field.TableName + '.' + field.ColumnName + " = "    + sSEARCH_TEXT1;  break;
									case "equals"         :  sSQL += field.TableName + '.' + field.ColumnName + " = "    + sSEARCH_TEXT1;  break;
									case "contains"       :  sSQL += field.TableName + '.' + field.ColumnName + " like " + "N'%'" + sCAT_SEP + sSEARCH_TEXT1 + sCAT_SEP + "N'%'";  break;
									case "starts_with"    :  sSQL += field.TableName + '.' + field.ColumnName + " like " +                     sSEARCH_TEXT1 + sCAT_SEP + "N'%'";  break;
									case "ends_with"      :  sSQL += field.TableName + '.' + field.ColumnName + " like " + "N'%'" + sCAT_SEP + sSEARCH_TEXT1;  break;
									case "not_equals_str" :  sSQL += field.TableName + '.' + field.ColumnName + " <> "   + sSEARCH_TEXT1;  break;
									case "empty"          :  sSQL += field.TableName + '.' + field.ColumnName + " is null"    ;  break;
									case "not_empty"      :  sSQL += field.TableName + '.' + field.ColumnName + " is not null";  break;
									case "one_of"         :  sSQL += field.TableName + '.' + field.ColumnName + ' in (@' + field.ColumnName + ')';  break;
								}
								break;
							}
							case "enum":
							{
								switch ( sOPERATOR )
								{
									// 02/09/2007 Paul.  enum uses is instead of equals operator. 
									case "is"             :  sSQL += field.TableName + '.' + field.ColumnName + " = "   + sSEARCH_TEXT1;  break;
									case "one_of"         :  sSQL += field.TableName + '.' + field.ColumnName + ' in (@' + field.ColumnName + ')';  break;
									case "empty"          :  sSQL += field.TableName + '.' + field.ColumnName + " is null"    ;  break;
									case "not_empty"      :  sSQL += field.TableName + '.' + field.ColumnName + " is not null";  break;
								}
								break;
							}
						}
					}
					else if ( oReportFilter.Operator == 'is null' || oReportFilter.Operator == 'is not null' )
					{
						sSQL += field.TableName + '.' + field.ColumnName + ' ';
						sSQL += oReportFilter.Operator;
					}
					else if ( oReportFilter.Parameter )
					{
						if ( oReportFilter.Operator == 'in' )
						{
							sSQL += field.TableName + '.' + field.ColumnName + ' ';
							sSQL += oReportFilter.Operator;
							sSQL += ' (@' + field.ColumnName + ')';
						}
						else if ( oReportFilter.Operator == 'not in' )
						{
							// 10/25/2014 Paul.  Filters that use NOT should protect against NULL values. 
							sSQL += 'coalesce(' + field.TableName + '.' + field.ColumnName + ', N\'\') ';
							sSQL += oReportFilter.Operator;
							sSQL += ' (@' + field.ColumnName + ')';
						}
						else if ( oReportFilter.Operator == '<>' )
						{
							// 10/25/2014 Paul.  Filters that use NOT should protect against NULL values. 
							sSQL += 'coalesce(' + field.TableName + '.' + field.ColumnName + ', N\'\') ';
							sSQL += oReportFilter.Operator;
							sSQL += ' @' + field.ColumnName;
						}
						// 04/11/2016 Paul.  Special support for between clause as a parameter. Needed to be separated into 2 report parameters. 
						else if ( oReportFilter.Operator == 'between' )
						{
							sSQL += field.TableName + '.' + field.ColumnName + ' ';
							sSQL += oReportFilter.Operator;
							sSQL += ' @' + field.ColumnName + '_AFTER' + ' and ' + '@' + field.ColumnName + '_BEFORE';
						}
						else
						{
							sSQL += field.TableName + '.' + field.ColumnName + ' ';
							sSQL += oReportFilter.Operator;
							sSQL += ' @' + field.ColumnName;
						}
					}
					else if ( oReportFilter.Operator == 'in' )
					{
						if ( oReportFilter.Value != null && $.isArray(oReportFilter.Value) )
						{
							sSQL += field.TableName + '.' + field.ColumnName + ' ';
							sSQL += oReportFilter.Operator + ' (';
							for ( var j = 0; j < oReportFilter.Value.length; j++ )
							{
								if ( j > 0 )
									sSQL += ', ';
								sSQL += oReportFilter.EscapedValue(oReportFilter.Value[j]);
							}
							sSQL += ')';
						}
						else
						{
							// 07/17/2016 Paul.  Allow the filter operator to be changed to a workflow version. 
							sErrors += L10n.Term('ReportDesigner.LBL_INVALID_ARRAY_VALUE').replace('{0}', field.TableName + '.' + field.ColumnName).replace('{1}', L10n.ListTerm(report_filter_operator_dom, oReportFilter.Operator)) + '<br />' + CrLf;
						}
					}
					else if ( oReportFilter.Operator == 'not in' )
					{
						if ( oReportFilter.Value != null && $.isArray(oReportFilter.Value) )
						{
							// 10/25/2014 Paul.  Filters that use NOT should protect against NULL values. 
							sSQL += 'coalesce(' + field.TableName + '.' + field.ColumnName + ', N\'\') ';
							sSQL += oReportFilter.Operator + ' (';
							for ( var j = 0; j < oReportFilter.Value.length; j++ )
							{
								if ( j > 0 )
									sSQL += ', ';
								sSQL += oReportFilter.EscapedValue(oReportFilter.Value[j]);
							}
							sSQL += ')';
						}
						else
						{
							// 07/17/2016 Paul.  Allow the filter operator to be changed to a workflow version. 
							sErrors += L10n.Term('ReportDesigner.LBL_INVALID_ARRAY_VALUE').replace('{0}', field.TableName + '.' + field.ColumnName).replace('{1}', L10n.ListTerm(report_filter_operator_dom, oReportFilter.Operator)) + '<br />' + CrLf;
						}
					}
					// 02/24/2015 Paul.  Add support for between filter clause. 
					else if ( oReportFilter.Operator == 'between' )
					{
						if ( oReportFilter.Value != null && $.isArray(oReportFilter.Value) && oReportFilter.Value.length >= 2 )
						{
							sSQL += field.TableName + '.' + field.ColumnName + ' ';
							sSQL += oReportFilter.Operator + ' ';
							sSQL += oReportFilter.EscapedValue(oReportFilter.Value[0]);
							sSQL += ' and ';
							sSQL += oReportFilter.EscapedValue(oReportFilter.Value[1]);
						}
						else
						{
							// 07/17/2016 Paul.  Allow the filter operator to be changed to a workflow version. 
							sErrors += L10n.Term('ReportDesigner.LBL_INVALID_ARRAY_VALUE').replace('{0}', field.TableName + '.' + field.ColumnName).replace('{1}', L10n.ListTerm(report_filter_operator_dom, oReportFilter.Operator)) + '<br />' + CrLf;
						}
					}
					else if ( oReportFilter.Value == null )
					{
						sErrors += L10n.Term('ReportDesigner.LBL_MISSING_FILTER_VALUE').replace('{0}', field.TableName + '.' + field.ColumnName) + '<br />' + CrLf;
					}
					else if ( oReportFilter.Operator == 'like' )
					{
						sSQL += field.TableName + '.' + field.ColumnName + ' ';
						sSQL += oReportFilter.Operator;
						sSQL += ' ';
						sSQL += oReportFilter.EscapedLikeValue(oReportFilter.Value);
					}
					else if ( oReportFilter.Operator == 'not like' )
					{
						// 10/25/2014 Paul.  Filters that use NOT should protect against NULL values. 
						sSQL += 'coalesce(' + field.TableName + '.' + field.ColumnName + ', N\'\') ';
						sSQL += oReportFilter.Operator;
						sSQL += ' ';
						sSQL += oReportFilter.EscapedLikeValue(oReportFilter.Value);
					}
					else if ( oReportFilter.Operator == '<>' )
					{
						// 10/25/2014 Paul.  Filters that use NOT should protect against NULL values. 
						sSQL += 'coalesce(' + field.TableName + '.' + field.ColumnName + ', N\'\') ';
						sSQL += oReportFilter.Operator;
						sSQL += ' ';
						sSQL += oReportFilter.EscapedValue(oReportFilter.Value);
					}
					else
					{
						sSQL += field.TableName + '.' + field.ColumnName + ' ';
						sSQL += oReportFilter.Operator;
						sSQL += ' ';
						sSQL += oReportFilter.EscapedValue(oReportFilter.Value);
					}
					sSQL += CrLf;
				}
			}
		}
		var nGroupBy = 0;
		for ( var i = 0; i < this.SelectedFields.length; i++ )
		{
			var oReportField = this.SelectedFields[i];
			if ( !Sql.IsEmptyString(oReportField.AggregateType) )
			{
				if ( oReportField.AggregateType == 'group by' )
				{
					sSQL += (nGroupBy == 0 ? ' group by ' : ', ');
					sSQL += oReportField.FieldName;
					nGroupBy++;
				}
			}
		}
		if ( nGroupBy > 0 )
			sSQL += CrLf;
		
		var nOrderBy = 0;
		for ( var i = 0; i < this.SelectedFields.length; i++ )
		{
			var oReportField = this.SelectedFields[i];
			if ( !Sql.IsEmptyString(oReportField.SortDirection) )
			{
				sSQL += (nOrderBy == 0 ? ' order by ' : ', ');
				if ( !Sql.IsEmptyString(oReportField.AggregateType) )
				{
					switch ( oReportField.AggregateType )
					{
						case 'group by'        :  sSQL +=                           oReportField.FieldName       + ' ' + oReportField.SortDirection;  nOrderBy++;  break;
						case 'avg'             :  sSQL += 'avg'    + '('          + oReportField.FieldName + ')' + ' ' + oReportField.SortDirection;  nOrderBy++;  break;
						case 'count'           :  sSQL += 'count'  + '('          + oReportField.FieldName + ')' + ' ' + oReportField.SortDirection;  nOrderBy++;  break;
						case 'min'             :  sSQL += 'min'    + '('          + oReportField.FieldName + ')' + ' ' + oReportField.SortDirection;  nOrderBy++;  break;
						case 'max'             :  sSQL += 'max'    + '('          + oReportField.FieldName + ')' + ' ' + oReportField.SortDirection;  nOrderBy++;  break;
						case 'stdev'           :  sSQL += 'stdev'  + '('          + oReportField.FieldName + ')' + ' ' + oReportField.SortDirection;  nOrderBy++;  break;
						case 'stdevp'          :  sSQL += 'stdevp' + '('          + oReportField.FieldName + ')' + ' ' + oReportField.SortDirection;  nOrderBy++;  break;
						case 'sum'             :  sSQL += 'sum'    + '('          + oReportField.FieldName + ')' + ' ' + oReportField.SortDirection;  nOrderBy++;  break;
						case 'var'             :  sSQL += 'var'    + '('          + oReportField.FieldName + ')' + ' ' + oReportField.SortDirection;  nOrderBy++;  break;
						case 'varp'            :  sSQL += 'varp'   + '('          + oReportField.FieldName + ')' + ' ' + oReportField.SortDirection;  nOrderBy++;  break;
						case 'avg distinct'    :  sSQL += 'avg'    + '(distinct ' + oReportField.FieldName + ')' + ' ' + oReportField.SortDirection;  nOrderBy++;  break;
						case 'count distinct'  :  sSQL += 'count'  + '(distinct ' + oReportField.FieldName + ')' + ' ' + oReportField.SortDirection;  nOrderBy++;  break;
						case 'stdev distinct'  :  sSQL += 'stdev'  + '(distinct ' + oReportField.FieldName + ')' + ' ' + oReportField.SortDirection;  nOrderBy++;  break;
						case 'stdevp distinct' :  sSQL += 'stdevp' + '(distinct ' + oReportField.FieldName + ')' + ' ' + oReportField.SortDirection;  nOrderBy++;  break;
						case 'sum distinct'    :  sSQL += 'sum'    + '(distinct ' + oReportField.FieldName + ')' + ' ' + oReportField.SortDirection;  nOrderBy++;  break;
						case 'var distinct'    :  sSQL += 'var'    + '(distinct ' + oReportField.FieldName + ')' + ' ' + oReportField.SortDirection;  nOrderBy++;  break;
						case 'varp distinct'   :  sSQL += 'varp'   + '(distinct ' + oReportField.FieldName + ')' + ' ' + oReportField.SortDirection;  nOrderBy++;  break;
					}
				}
				else
				{
					sSQL += oReportField.FieldName + ' ' + oReportField.SortDirection;
					nOrderBy++;
				}
			}
		}
		if ( nOrderBy > 0 )
			sSQL += CrLf;
		
		var sUnusedTables = '';
		for ( var sTableName in oUsedTables )
		{
			if ( oUsedTables[sTableName] == 0 )
			{
				if ( sUnusedTables.length > 0 )
					sUnusedTables += ', ';
				sUnusedTables += sTableName;
			}
		}
		if ( sUnusedTables.length > 0 )
		{
			sErrors += L10n.Term('ReportDesigner.LBL_UNRELATED_ERROR').replace('{0}', sUnusedTables);
		}
	}
	sSQL += CrLf;
	//sSQL += dumpObj(this.Tables        , 'Tables'        );
	//sSQL += dumpObj(this.Relationships , 'Relationships' );
	//sSQL += dumpObj(this.AppliedFilters, 'AppliedFilters');
	
	var divReportDesignerSQL = document.getElementById('divReportDesignerSQL');
	if ( divReportDesignerSQL != null )
	{
		while ( divReportDesignerSQL.childNodes.length > 0 )
			divReportDesignerSQL.removeChild(divReportDesignerSQL.firstChild);
		divReportDesignerSQL.appendChild(document.createTextNode(sSQL));
	}
	
	var divReportDesignerSQLError = document.getElementById('divReportDesignerSQLError');
	divReportDesignerSQLError.innerHtml = sErrors;
	
	if ( bDebug )
	{
		var divReportDesignerJSON = document.getElementById('divReportDesignerJSON');
		if ( divReportDesignerJSON != null )
		{
			MAX_DUMP_DEPTH = 4;
			divReportDesignerJSON.innerHTML = dumpObj(jQuery.parseJSON(this.Stringify()), '').replace(/\n/g, '<br>\n').replace(/\t/g, '&nbsp;&nbsp;&nbsp;');
		}
	}
}

var oReportDesign = new ReportDesign();
var oCurrentJoinFields = null;

function ReportDesigner_LoadModules(callback, context)
{
	var xhr = CreateSplendidRequest('ReportDesigner/Rest.svc/GetModules', 'GET');
	xhr.onreadystatechange = function()
	{
		if ( xhr.readyState == 4 )
		{
			GetSplendidResult(xhr, function(result)
			{
				try
				{
					if ( result.status == 200 )
					{
						if ( result.d !== undefined )
						{
							callback.call(context||this, 1, result.d.results);
						}
						else if ( result.ExceptionDetail !== undefined )
						{
							callback.call(context||this, -1, result.ExceptionDetail.Message);
						}
						else
						{
							callback.call(context||this, -1, xhr.responseText);
						}
					}
					else
					{
						if ( result.ExceptionDetail !== undefined )
							callback.call(context||this, -1, result.ExceptionDetail.Message);
						else
							callback.call(context||this, -1, xhr.responseText);
					}
				}
				catch(e)
				{
					callback.call(context||this, -1, SplendidError.FormatError(e, 'ReportDesigner_LoadModules'));
				}
			});
		}
	}
	try
	{
		xhr.send();
	}
	catch(e)
	{
		callback.call(context||this, -1, SplendidError.FormatError(e, 'ReportDesigner_LoadModules'));
	}
}

// 07/04/2016 Paul.  We need a separate method for workflow modules. 
function ReportDesigner_LoadBusinessProcessModules(callback, context)
{
	var xhr = CreateSplendidRequest('ReportDesigner/Rest.svc/GetBusinessProcessModules', 'GET');
	xhr.onreadystatechange = function()
	{
		if ( xhr.readyState == 4 )
		{
			GetSplendidResult(xhr, function(result)
			{
				try
				{
					if ( result.status == 200 )
					{
						if ( result.d !== undefined )
						{
							callback.call(context||this, 1, result.d.results);
						}
						else if ( result.ExceptionDetail !== undefined )
						{
							callback.call(context||this, -1, result.ExceptionDetail.Message);
						}
						else
						{
							callback.call(context||this, -1, xhr.responseText);
						}
					}
					else
					{
						if ( result.ExceptionDetail !== undefined )
							callback.call(context||this, -1, result.ExceptionDetail.Message);
						else
							callback.call(context||this, -1, xhr.responseText);
					}
				}
				catch(e)
				{
					callback.call(context||this, -1, SplendidError.FormatError(e, 'ReportDesigner_LoadBusinessProcessModules'));
				}
			});
		}
	}
	try
	{
		xhr.send();
	}
	catch(e)
	{
		callback.call(context||this, -1, SplendidError.FormatError(e, 'ReportDesigner_LoadBusinessProcessModules'));
	}
}

function zTree_onCheck(e, treeId, treeNode)
{
	//var zTree = $.fn.zTree.getZTreeObj(treeId);
	//var nodes = zTree.getCheckedNodes(true);
	
	//treeNode.checked, treeNode.checkedOld, treeNode.isParent, treeNode.level
	if ( treeNode.Field === undefined )
	{
		var bRemoveTable = !treeNode.checked;
		// 07/04/2016 Paul.  Special case when not showing selected fields. 
		if ( !bReportDesignerWorkflowMode )
		{
			for ( var j = 0; j < treeNode.children.length; j++ )
			{
				var oFieldNode = treeNode.children[j];
				if ( oFieldNode.checked )
				{
					tSelectedFields_AddField(oFieldNode.Field);
				}
				else
				{
					tSelectedFields_RemoveField(oFieldNode.Field, bRemoveTable);
				}
			}
		}
		else
		{
			var field = ReportDesigner_FindFieldByTable(treeNode.Module.TableName, 'ID');
			if ( treeNode.checked )
			{
				tSelectedFields_AddField(field);
			}
			else
			{
				tSelectedFields_RemoveField(field, bRemoveTable);
			}
		}
		oReportDesign.PreviewSQL();
	}
	else if ( treeNode.Field !== undefined )
	{
		if ( treeNode.checked )
		{
			tSelectedFields_AddField(treeNode.Field);
		}
		else
		{
			var bRemoveTable = !treeNode.getParentNode().checked;
			tSelectedFields_RemoveField(treeNode.Field, bRemoveTable);
		}
		oReportDesign.PreviewSQL();
	}
}

function zTree_BuildModuleNodes(message)
{
	var zNodes = new Array();
	var oTable = new Object();
	oTable.name        = L10n.Term('ReportDesigner.LBL_TABLES');
	oTable.open        = true;
	oTable.chkDisabled = true;
	oTable.children    = new Array();
	zNodes.push(oTable);
	var oRelationshipTables = new Object();
	oRelationshipTables.name        = L10n.Term('ReportDesigner.LBL_RELATIONSHIP_TABLES');
	oRelationshipTables.open        = true;
	oRelationshipTables.chkDisabled = true;
	oRelationshipTables.children    = new Array();
	// 07/04/2016 Paul.  Only add relationships if they exists. 
	if ( !bReportDesignerWorkflowMode )
		zNodes.push(oRelationshipTables);
	// 04/17/2018 Paul.  Add CustomReportView to simplify reporting. 
	var oCustomReportViews = new Object();
	oCustomReportViews.name        = L10n.Term('ReportDesigner.LBL_CUSTOM_REPORT_VIEWS');
	oCustomReportViews.open        = true;
	oCustomReportViews.chkDisabled = true;
	oCustomReportViews.children    = new Array();
	if ( !bReportDesignerWorkflowMode )
		zNodes.push(oCustomReportViews);
	if ( message instanceof Array )
	{
		arrReportDesignerModules = message;
		for ( var i = 0; i < arrReportDesignerModules.length; i++ )
		{
			var module = arrReportDesignerModules[i];
			var oModuleNode = new Object();
			oModuleNode.Module = module;
			// 07/15/2016 Paul.  Set the id for the table to the table name. 
			oModuleNode.id     = module.TableName;
			oModuleNode.name   = module.DisplayName;
			if ( bDebug )
				oModuleNode.name += ' (' + module.TableName + ')';
			oModuleNode.open = false;
			if ( module.Fields !== undefined && module.Fields instanceof Array )
			{
				oModuleNode.children = new Array();
				for ( var j = 0; j < module.Fields.length; j++ )
				{
					var field = module.Fields[j];
					// 01/05/2014 Paul.  Linking back to the original module causes an Out of stack space error.
					field.Module = new Object();
					field.Module.ModuleName  = module.ModuleName ;
					field.Module.DisplayName = module.DisplayName;
					field.Module.TableName   = module.TableName  ;
					// 07/04/2016 Paul.  Special case when not showing selected fields. 
					if ( !bReportDesignerWorkflowMode )
					{
						var oFieldNode = new Object();
						oFieldNode.Field = field;
						oFieldNode.name  = field.DisplayName;
						// 08/03/2014 Paul.  The full name will be used when selecting nodes during the load operation. 
						oFieldNode.FieldName = field.TableName + '.' + field.ColumnName;
						if ( bDebug )
							oFieldNode.name += ' (' + field.TableName + '.' + field.ColumnName + ')';
						oModuleNode.children.push(oFieldNode);
					}
				}
			}
			// 04/17/2018 Paul.  Add CustomReportView to simplify reporting. 
			if ( Sql.ToBoolean(module.CustomReportView) )
				oCustomReportViews.children.push(oModuleNode);
			else if ( module.Relationship == 0 )
				oTable.children.push(oModuleNode);
			else
				oRelationshipTables.children.push(oModuleNode);
		}
	}
	return zNodes;
}

function ReportDesigner_Cache(callback)
{
	try
	{
		var nCacheItemCount = 0;
		var bgPage = chrome.extension.getBackgroundPage();
		// 10/16/2012 Paul.  GLobal flag to indicate that the cache is being loaded. 
		if ( bGLOBAL_LAYOUT_CACHE )
		{
			// 02/01/2013 Paul.  Signal that we are done. 
			callback(1, null);
			return;
		}
		bGLOBAL_LAYOUT_CACHE = true;
		
		// 10/12/2012 Paul.  Instead of 300+ requests, use the new bulk load functions to populate the layout cache. 
		callback(2, 'loading all GridView layouts');
		bgPage.ListView_LoadAllLayouts(function(status, message)
		{
			//callback(2, 'loading all DetailView layouts');
			//bgPage.DetailView_LoadAllLayouts(function(status, message)
			//{
				callback(2, 'loading all EditView layouts');
				bgPage.EditView_LoadAllLayouts(function(status, message)
				{
					//callback(2, 'loading all DetailViewRelationship layouts');
					//bgPage.DetailViewRelationships_LoadAllLayouts(function(status, message)
					//{
						//callback(2, 'loading all DynamicButton layouts');
						//bgPage.DynamicButtons_LoadAllLayouts(function(status, message)
						//{
							callback(2, 'loading all Terminology Lists');
							bgPage.Terminology_LoadAllLists(function(status, message)
							{
								callback(2, 'loading all Terminology');
								bgPage.Terminology_LoadAllTerms(function(status, message)
								{
									callback(2, 'Done loading all layouts');
									// 10/16/2012 Paul.  Signal that we are done. 
									callback(1, null);
								});
							});
						//});
					//});
				});
			//});
		});
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'ReportDesigner_Cache');
	}
}

function ReportDesigner_InitUI(callback)
{
	try
	{
		var bgPage = chrome.extension.getBackgroundPage();
		callback(2, 'Loading config.');
		bgPage.Application_Config(function(status, message)
		{
			if ( status == 0 || status == 1 )
			{
				callback(2, 'Loading modules.');
				bgPage.Application_Modules(function(status, message)
				{
					if ( status == 0 || status == 1 )
					{
						callback(2, 'Loading global terminology.');
						ReportDesigner_Cache(function(status, message)
						{
							if ( status == 0 || status == 1 )
							{
								callback(status, null);
							}
							else
							{
								callback(status, message);
							}
						});
					}
					else
					{
						callback(status, message);
					}
				});
			}
			else
			{
				callback(status, message);
			}
		});
	}
	catch(e)
	{
		callback(-1, SplendidError.FormatError(e, 'ReportDesigner_InitUI'));
	}
}

SplendidError.SystemMessage = function(message)
{
	var lblError = document.getElementById('lblError');
	if ( lblError != null )
		lblError.innerHTML = message;
}

function tSelectedFields_SelectCell(td)
{
	var tSelectedFields = document.getElementById('tSelectedFields');
	$('#tSelectedFields td.QueryDesigner_Selected').removeClass('QueryDesigner_Selected').addClass('QueryDesigner');
	td.className = 'QueryDesigner_Selected';
}

function tSelectedFields_SelectedIndex()
{
	var nSelectedIndex = -1;
	var td = $('#tSelectedFields td.QueryDesigner_Selected');
	if ( td.length > 0 )
	{
		nSelectedIndex = td[0].parentNode.rowIndex - 1;
	}
	return nSelectedIndex;
}

function tSelectedFields_SelectedDelete()
{
	var nSelectedIndex = tSelectedFields_SelectedIndex();
	if ( nSelectedIndex != -1 )
	{
		var trCurrent  = tSelectedFields.rows[nSelectedIndex + 1];
		tSelectedFields.removeChild(trCurrent);
		
		oReportDesign.SelectedField_Delete(nSelectedIndex);
		oReportDesign.PreviewSQL();
	}
	return false;
}

function tSelectedFields_SelectedMoveUp()
{
	var nSelectedIndex = tSelectedFields_SelectedIndex();
	if ( nSelectedIndex > 0 )
	{
		var tSelectedFields = document.getElementById('tSelectedFields');
		// First row is header. 
		var trPrevious = tSelectedFields.rows[nSelectedIndex + 1 - 1];
		var trCurrent  = tSelectedFields.rows[nSelectedIndex + 1];
		tSelectedFields.removeChild(trCurrent);
		tSelectedFields.insertBefore(trCurrent, trPrevious);
		
		oReportDesign.SelectedField_MoveUp(nSelectedIndex);
		oReportDesign.PreviewSQL();
	}
	return false;
}

function tSelectedFields_SelectedMoveDown()
{
	var nSelectedIndex = tSelectedFields_SelectedIndex();
	if ( nSelectedIndex != -1 && nSelectedIndex < oReportDesign.SelectedFields.length )
	{
		var tSelectedFields = document.getElementById('tSelectedFields');
		// First row is header. 
		var trCurrent  = tSelectedFields.rows[nSelectedIndex + 1];
		var trNext     = tSelectedFields.rows[nSelectedIndex + 1 + 1];
		tSelectedFields.removeChild(trNext);
		// There is no insertAfter, so just use insertBefore, but switch the arguments. 
		tSelectedFields.insertBefore(trNext, trCurrent);
		
		oReportDesign.SelectedField_MoveDown(nSelectedIndex);
		oReportDesign.PreviewSQL();
	}
	return false;
}

function tSelectedFields_AddField(field, oReportField)
{
	if ( oReportField === undefined || oReportField == null )
		oReportField = oReportDesign.SelectedField_AddField(field);
	var sFieldName   = oReportField.FieldName  ;
	var sDisplayName = oReportField.DisplayName;

	if ( oReportField != null )
	{
		var tSelectedFields = document.getElementById('tSelectedFields');
		var tr = tSelectedFields.rows.namedItem(sFieldName);
		if ( tr == null )
		{
			tr = tSelectedFields.insertRow(-1);
			tr.id = sFieldName;
			tSelectedFields.appendChild(tr);
			var tdField         = document.createElement('td');
			var tdDisplayName   = document.createElement('td');
			var tdDisplayWidth  = document.createElement('td');
			var tdAggregate     = document.createElement('td');
			var tdSortDirection = document.createElement('td');
			tr.appendChild(tdField        );
			tr.appendChild(tdDisplayName  );
			tr.appendChild(tdDisplayWidth );
			tr.appendChild(tdAggregate    );
			tr.appendChild(tdSortDirection);
			// 12/31/2014 Paul.  Firefox does not like innerText. Use createTextNode
			tdField.appendChild(document.createTextNode(sFieldName));
			tdField.className        = 'QueryDesigner';
			tdDisplayName.appendChild(document.createTextNode(sDisplayName));
			tdDisplayName.className  = 'QueryDesigner';
			tdDisplayWidth.className = 'QueryDesigner';
			if ( oReportField.DisplayWidth == null )
				tdDisplayWidth.appendChild(document.createTextNode(''));
			else
				tdDisplayWidth.appendChild(document.createTextNode(oReportField.DisplayWidth));
			if ( oReportField.AggregateType == null )
				tdAggregate.appendChild(document.createTextNode(L10n.Term('ReportDesigner.LBL_NONE')));
			else
				tdAggregate.appendChild(document.createTextNode(L10n.ListTerm('report_aggregate_type_dom', oReportField.AggregateType)));
			tdAggregate.className = 'QueryDesigner';
			if ( oReportField.SortDirection == null )
				tdSortDirection.appendChild(document.createTextNode(L10n.Term('ReportDesigner.LBL_NONE')));
			else
				tdSortDirection.appendChild(document.createTextNode(L10n.ListTerm('report_sort_direction_dom', oReportField.SortDirection)));
			tdSortDirection.className = 'QueryDesigner';
			tdField.onclick = function(e)
			{
				tSelectedFields_SelectCell(tdField);
			};
			tdDisplayName.onclick = function(e)
			{
				if ( e.target.tagName == 'INPUT' )
					return;
				tSelectedFields_SelectCell(tdDisplayName);
				var nSelectedIndex = tdDisplayName.parentNode.rowIndex - 1;
				if ( tdDisplayName.childNodes.length > 0 && tdDisplayName.childNodes[0].tagName == 'DIV' )
				{
					while ( tdDisplayName.childNodes.length > 0 )
						tdDisplayName.removeChild(tdDisplayName.firstChild);
					
					if ( oReportField.DisplayName != null )
					{
						tdDisplayName.appendChild(document.createTextNode(oReportField.DisplayName));
					}
				}
				else
				{
					while ( tdDisplayName.childNodes.length > 0 )
						tdDisplayName.removeChild(tdDisplayName.firstChild);
					
					var div = document.createElement('div');
					//div.style.backgroundColor = 'white';
					div.className   = 'QueryDesigner';
					tdDisplayName.appendChild(div);
					div.onclick = function(e)
					{
						tAppliedFilters_SelectCell(tdDisplayName);
						// 02/14/2014 Paul.  Use e.preventDefault() and e.stopPropagation(). 
						e.preventDefault();
						e.stopPropagation();
					};
				
					var divDisplayName = document.createElement('div');
					div.appendChild(divDisplayName);
					var txt = document.createElement('input');
					txt.id          = 'tSelectedFields_DisplayName' + nSelectedIndex.toString();
					txt.type        = 'text' ;
					txt.style.width = ($(tdDisplayName).width() - 4).toString() + 'px';
					divDisplayName.appendChild(txt);
					txt.focus();
					if ( oReportField.DisplayName != null )
						txt.value = oReportField.DisplayName;
					txt.onblur = function(e)
					{
						oReportField.DisplayName = txt.value;
						if ( tdDisplayName.childNodes.length > 0 && tdDisplayName.childNodes[0].tagName == 'DIV' )
						{
							while ( tdDisplayName.childNodes.length > 0 )
								tdDisplayName.removeChild(tdDisplayName.firstChild);
							
							if ( oReportField.DisplayName != null )
							{
								tdDisplayName.appendChild(document.createTextNode(oReportField.DisplayName));
							}
						}
						oReportDesign.PreviewSQL();
					};
				}
			};
			tdDisplayWidth.onclick = function(e)
			{
				if ( e.target.tagName == 'INPUT' )
					return;
				tSelectedFields_SelectCell(tdDisplayWidth);
				var nSelectedIndex = tdDisplayWidth.parentNode.rowIndex - 1;
				if ( tdDisplayWidth.childNodes.length > 0 && tdDisplayWidth.childNodes[0].tagName == 'DIV' )
				{
					while ( tdDisplayWidth.childNodes.length > 0 )
						tdDisplayWidth.removeChild(tdDisplayWidth.firstChild);
					
					if ( oReportField.DisplayWidth != null )
					{
						tdDisplayWidth.appendChild(document.createTextNode(oReportField.DisplayWidth));
					}
				}
				else
				{
					while ( tdDisplayWidth.childNodes.length > 0 )
						tdDisplayWidth.removeChild(tdDisplayWidth.firstChild);
					
					var div = document.createElement('div');
					//div.style.backgroundColor = 'white';
					div.className   = 'QueryDesigner';
					//div.style.width = '90px';
					tdDisplayWidth.appendChild(div);
					div.onclick = function(e)
					{
						tAppliedFilters_SelectCell(tdDisplayWidth);
						// 02/14/2014 Paul.  Use e.preventDefault() and e.stopPropagation(). 
						e.preventDefault();
						e.stopPropagation();
					};
				
					var divDisplayWidth = document.createElement('div');
					div.appendChild(divDisplayWidth);
					var txt = document.createElement('input');
					txt.id          = 'tSelectedFields_DisplayWidth' + nSelectedIndex.toString();
					txt.type        = 'text' ;
					txt.style.width = ($(tdDisplayWidth).width() - 4).toString() + 'px';
					divDisplayWidth.appendChild(txt);
					txt.focus();
					if ( oReportField.DisplayWidth != null )
						txt.value = oReportField.DisplayWidth;
					txt.onblur = function(e)
					{
						oReportField.DisplayWidth = txt.value;
						if ( tdDisplayWidth.childNodes.length > 0 && tdDisplayWidth.childNodes[0].tagName == 'DIV' )
						{
							while ( tdDisplayWidth.childNodes.length > 0 )
								tdDisplayWidth.removeChild(tdDisplayWidth.firstChild);
							
							if ( oReportField.DisplayWidth != null )
							{
								tdDisplayWidth.appendChild(document.createTextNode(oReportField.DisplayWidth));
							}
						}
						oReportDesign.PreviewSQL();
					};
				}
			};
			tdAggregate.onclick = function(e)
			{
				// 12/29/2013 Paul.  Need to prevent the select click event from blocking the code.  stopPropagation() did not work. 
				if ( e.target.tagName == 'SELECT' )
					return;
				tSelectedFields_SelectCell(tdAggregate);
				if ( oReportDesign.GroupAndAggregate )
				{
					if ( tdAggregate.childNodes.length > 0 && tdAggregate.childNodes[0].tagName == 'SELECT' )
					{
						while ( tdAggregate.childNodes.length > 0 )
							tdAggregate.removeChild(tdAggregate.firstChild);
						
						var nSelectedIndex = tdAggregate.parentNode.rowIndex - 1;
						var oReportField = oReportDesign.SelectedField_GetAt(nSelectedIndex);
						tdAggregate.appendChild(document.createTextNode(L10n.ListTerm('report_aggregate_type_dom', oReportField.AggregateType)));
					}
					else
					{
						while ( tdAggregate.childNodes.length > 0 )
							tdAggregate.removeChild(tdAggregate.firstChild);
						
						var sel = document.createElement('select');
						tdAggregate.appendChild(sel);
						
						var nSelectedIndex = tdAggregate.parentNode.rowIndex - 1;
						var oReportField = oReportDesign.SelectedField_GetAt(nSelectedIndex);
						var arrAggregateTypes = L10n.GetList('report_aggregate_type_dom');
						for ( var i = 0; i < arrAggregateTypes.length; i++ )
						{
							var opt = document.createElement('option');
							opt.value     = arrAggregateTypes[i];
							opt.innerHTML = L10n.ListTerm('report_aggregate_type_dom', arrAggregateTypes[i]);
							sel.appendChild(opt);
							if ( oReportField.AggregateType == arrAggregateTypes[i] )
								sel.options.selectedIndex = i;
						}
						sel.focus();
						sel.onchange = function(e)
						{
							var nSelectedIndex = tdAggregate.parentNode.rowIndex - 1;
							var sValue         = sel.options[sel.options.selectedIndex].value;
							var oReportField   = oReportDesign.SelectedField_GetAt(nSelectedIndex);
							oReportField.AggregateType = sValue;
							while ( tdAggregate.childNodes.length > 0 )
								tdAggregate.removeChild(tdAggregate.firstChild);
							tdAggregate.appendChild(document.createTextNode(L10n.ListTerm('report_aggregate_type_dom', oReportField.AggregateType)));
							oReportDesign.PreviewSQL();
						};
					}
				}
			};
			tdSortDirection.onclick = function(e)
			{
				// 12/29/2013 Paul.  Need to prevent the select click event from blocking the code.  stopPropagation() did not work. 
				if ( e.target.tagName == 'SELECT' )
					return;
				tSelectedFields_SelectCell(tdSortDirection);
				if ( tdSortDirection.childNodes.length > 0 && tdSortDirection.childNodes[0].tagName == 'SELECT' )
				{
					while ( tdSortDirection.childNodes.length > 0 )
						tdSortDirection.removeChild(tdSortDirection.firstChild);
					
					var nSelectedIndex = tdSortDirection.parentNode.rowIndex - 1;
					var oReportField = oReportDesign.SelectedField_GetAt(nSelectedIndex);
					tdSortDirection.appendChild(document.createTextNode(L10n.ListTerm('report_sort_direction_dom', oReportField.SortDirection)));
				}
				else
				{
					while ( tdSortDirection.childNodes.length > 0 )
						tdSortDirection.removeChild(tdSortDirection.firstChild);
					
					var sel = document.createElement('select');
					tdSortDirection.appendChild(sel);
					
					var nSelectedIndex = tdSortDirection.parentNode.rowIndex - 1;
					var oReportField = oReportDesign.SelectedField_GetAt(nSelectedIndex);
					var arrSortDirections = L10n.GetList('report_sort_direction_dom');
					for ( var i = 0; i < arrSortDirections.length; i++ )
					{
						var opt = document.createElement('option');
						opt.value     = arrSortDirections[i];
						opt.innerHTML = L10n.ListTerm('report_sort_direction_dom', arrSortDirections[i]);
						sel.appendChild(opt);
						if ( oReportField.SortDirection == arrSortDirections[i] )
							sel.options.selectedIndex = i;
					}
					sel.focus();
					sel.onchange = function(e)
					{
						var nSelectedIndex = tdSortDirection.parentNode.rowIndex - 1;
						var sValue         = sel.options[sel.options.selectedIndex].value;
						var oReportField   = oReportDesign.SelectedField_GetAt(nSelectedIndex);
						oReportField.SortDirection = sValue;
						while ( tdSortDirection.childNodes.length > 0 )
							tdSortDirection.removeChild(tdSortDirection.firstChild);
						tdSortDirection.appendChild(document.createTextNode(L10n.ListTerm('report_sort_direction_dom', oReportField.SortDirection)));
						oReportDesign.PreviewSQL();
					};
				}
			};
		}
	}
}

function tSelectedFields_RemoveField(field, bRemoveTable)
{
	var sTableName   = field.TableName;
	var sFieldName   = field.TableName + '.' + field.ColumnName;
	var tSelectedFields = document.getElementById('tSelectedFields');
	var tr = tSelectedFields.rows.namedItem(sFieldName);
	if ( tr != null )
	{
		tSelectedFields.deleteRow(tr.rowIndex);
	}
	
	// If parent Table node is not checked, then all fields for the table will be removed. 
	if ( bRemoveTable )
	{
		oReportDesign.Tables_RemoveTable(sTableName);
	}
	oReportDesign.SelectedField_RemoveField(sFieldName);
}

function chkGroupAndAggregate_Clicked(chk)
{
	oReportDesign.GroupAndAggregate = chk.checked;
	var sLBL_NONE     = L10n.Term('ReportDesigner.LBL_NONE');
	var sLBL_GROUP_BY = L10n.ListTerm('report_aggregate_type_dom', 'group by')
	var tSelectedFields = document.getElementById('tSelectedFields');
	for ( var i = 0; i < oReportDesign.SelectedFields.length; i++ )
	{
		var oReportField = oReportDesign.SelectedFields[i];
		if ( oReportDesign.GroupAndAggregate )
			oReportField.AggregateType = 'group by';
		else
			oReportField.AggregateType = null;
	}
	for ( var i = 1; i < tSelectedFields.rows.length; i++ )
	{
		var tdAggregate = tSelectedFields.rows[i].cells[3];
		// 04/19/2018 Paul.  The old value first needs to be removed. 
		while ( tdAggregate.childNodes.length > 0 )
			tdAggregate.removeChild(tdAggregate.firstChild);

		if ( oReportDesign.GroupAndAggregate )
			tdAggregate.appendChild(document.createTextNode(sLBL_GROUP_BY));
		else
			tdAggregate.appendChild(document.createTextNode(sLBL_NONE));
	}
	oReportDesign.PreviewSQL();
}

function tRelationships_SelectCell(td)
{
	var tRelationships = document.getElementById('tRelationships');
	$('#tRelationships td.QueryDesigner_Selected').removeClass('QueryDesigner_Selected').addClass('QueryDesigner');
	td.className = 'QueryDesigner_Selected';
}

function tRelationships_SelectedIndex()
{
	var nSelectedIndex = -1;
	var td = $('#tRelationships td.QueryDesigner_Selected');
	if ( td.length > 0 )
	{
		nSelectedIndex = td[0].parentNode.rowIndex - 1;
	}
	return nSelectedIndex;
}

function tRelationships_SelectedDelete()
{
	var nSelectedIndex = tRelationships_SelectedIndex();
	if ( nSelectedIndex != -1 )
	{
		var trCurrent  = tRelationships.rows[nSelectedIndex + 1];
		tRelationships.removeChild(trCurrent);
		
		oReportDesign.Relationships_Delete(nSelectedIndex);
		oReportDesign.PreviewSQL();
	}
	return false;
}

function tRelationships_SelectedMoveUp()
{
	var nSelectedIndex = tRelationships_SelectedIndex();
	if ( nSelectedIndex > 0 )
	{
		var tRelationships = document.getElementById('tRelationships');
		// First row is header. 
		var trPrevious = tRelationships.rows[nSelectedIndex + 1 - 1];
		var trCurrent  = tRelationships.rows[nSelectedIndex + 1];
		tRelationships.removeChild(trCurrent);
		tRelationships.insertBefore(trCurrent, trPrevious);
		
		oReportDesign.Relationships_MoveUp(nSelectedIndex);
		oReportDesign.PreviewSQL();
	}
	return false;
}

function tRelationships_SelectedMoveDown()
{
	var nSelectedIndex = tRelationships_SelectedIndex();
	if ( nSelectedIndex != -1 && nSelectedIndex < oReportDesign.Relationships.length - 1 )
	{
		var tRelationships = document.getElementById('tRelationships');
		// First row is header. 
		var trCurrent  = tRelationships.rows[nSelectedIndex + 1];
		var trNext     = tRelationships.rows[nSelectedIndex + 1 + 1];
		tRelationships.removeChild(trNext);
		// There is no insertAfter, so just use insertBefore, but switch the arguments. 
		tRelationships.insertBefore(trNext, trCurrent);
		
		oReportDesign.Relationships_MoveDown(nSelectedIndex);
		oReportDesign.PreviewSQL();
	}
	return false;
}

function tRelationships_AddRelationship(oReportRelationship)
{
	if ( oReportRelationship === undefined || oReportRelationship == null )
		oReportRelationship = oReportDesign.Relationships_AddRelationship();
	if ( oReportRelationship != null )
	{
		var tRelationships = document.getElementById('tRelationships');
		var tr = tRelationships.insertRow(-1);
		tRelationships.appendChild(tr);
		var tdLeftTable  = document.createElement('td');
		var tdJoinType   = document.createElement('td');
		var tdRightTable = document.createElement('td');
		var tdJoinFields = document.createElement('td');
		tr.appendChild(tdLeftTable );
		tr.appendChild(tdJoinType  );
		tr.appendChild(tdRightTable);
		tr.appendChild(tdJoinFields);
		tdLeftTable .className = 'QueryDesigner';
		tdJoinType  .className = 'QueryDesigner';
		tdRightTable.className = 'QueryDesigner';
		tdJoinFields.className = 'QueryDesigner';
		if ( oReportRelationship.JoinType != null )
			tdJoinType  .appendChild(document.createTextNode(L10n.ListTerm('report_join_type_dom', oReportRelationship.JoinType)));
		if ( oReportRelationship.JoinFields != null )
			tdJoinFields.innerHTML = JoinFieldsDisplayText(oReportRelationship.JoinFields);
		if ( oReportRelationship.LeftTable != null )
			tdLeftTable.appendChild(document.createTextNode(oReportRelationship.LeftTable.DisplayName));
		if ( oReportRelationship.RightTable != null )
			tdRightTable.appendChild(document.createTextNode(oReportRelationship.RightTable.DisplayName));
		
		tdLeftTable.onclick = function(e)
		{
			var nSelectedIndex = tdLeftTable.parentNode.rowIndex - 1;
			var oReportRelationship = oReportDesign.Relationships_GetAt(nSelectedIndex);
			// 01/05/2014 Paul.  Use e.preventDefault() and e.stopPropagation(). 
			//if ( e.target.tagName == 'DIV' )
			//	return;
			tRelationships_SelectCell(tdLeftTable);
			if ( tdLeftTable.childNodes.length > 0 && tdLeftTable.childNodes[0].tagName == 'DIV' )
			{
				while ( tdLeftTable.childNodes.length > 0 )
					tdLeftTable.removeChild(tdLeftTable.firstChild);
				
				if ( oReportRelationship.LeftTable != null )
				{
					tdLeftTable.appendChild(document.createTextNode(oReportRelationship.LeftTable.DisplayName));
				}
			}
			else
			{
				while ( tdLeftTable.childNodes.length > 0 )
					tdLeftTable.removeChild(tdLeftTable.firstChild);
				
				var div = document.createElement('div');
				div.id = 'tRelationships_LeftTable' + nSelectedIndex.toString();
				div.className             = 'ztree';
				div.style.padding         = '6px';
				div.style.backgroundColor = 'white';
				tdLeftTable.appendChild(div);
				div.onclick = function(e)
				{
					// 01/05/2014 Paul.  Prevent tree click from disabling window.
					e.preventDefault();
					e.stopPropagation();
				};
				
				var zNodes = new Array();
				var oCurrentTables = new Object();
				var oAllTables     = new Object();
				oCurrentTables.name     = L10n.Term('ReportDesigner.LBL_TABLES_IN_QUERY');
				oCurrentTables.open     = true;
				oCurrentTables.children = new Array();
				oAllTables.name         = L10n.Term('ReportDesigner.LBL_TABLES');
				oAllTables.open         = false;
				oAllTables.children     = new Array();
				zNodes.push(oCurrentTables);
				zNodes.push(oAllTables    );
				for ( var i = 0; i < oReportDesign.Tables.length; i++ )
				{
					var table = new ReportTable(oReportDesign.Tables[i].Module);
					table.id   = table.TableName  ;
					table.name = table.DisplayName;
					oCurrentTables.children.push(table);
				}
				for ( var i = 0; i < arrReportDesignerModules.length; i++ )
				{
					var module = arrReportDesignerModules[i];
					var table = new ReportTable(module);
					table.id   = table.TableName  ;
					table.name = table.DisplayName;
					oAllTables.children.push(table);
				}
				
				var setting =
				{
					callback:
					{
						onClick: function(event, treeId, treeNode, clickFlag)
						{
							if ( treeNode.Module !== undefined )
							{
								var table = new ReportTable(treeNode.Module);
								oReportRelationship.LeftTable = table;
								while ( tdLeftTable.childNodes.length > 0 )
									tdLeftTable.removeChild(tdLeftTable.firstChild);
								
								tdLeftTable.appendChild(document.createTextNode(table.DisplayName));
								tdJoinFields.innerHTML = JoinFieldsDisplayText(oReportRelationship.JoinFields);
								oReportDesign.Tables_UpdateAll();
							}
						}
					}
					, data:
					{
						simpleData:
						{
							enable: false
						}
					}
				};
				var treeObj = $.fn.zTree.init($('#' + div.id), setting, zNodes);
				if ( oReportRelationship.LeftTable != null )
				{
					treeObj.selectNode(treeObj.getNodeByParam('id', oReportRelationship.LeftTable.TableName));
				}
			}
		};
		tdRightTable.onclick = function(e)
		{
			var nSelectedIndex = tdRightTable.parentNode.rowIndex - 1;
			var oReportRelationship = oReportDesign.Relationships_GetAt(nSelectedIndex);
			// 01/05/2014 Paul.  Use e.preventDefault() and e.stopPropagation(). 
			//if ( e.target.tagName == 'DIV' )
			//	return;
			tRelationships_SelectCell(tdRightTable);
			if ( tdRightTable.childNodes.length > 0 && tdRightTable.childNodes[0].tagName == 'DIV' )
			{
				while ( tdRightTable.childNodes.length > 0 )
					tdRightTable.removeChild(tdRightTable.firstChild);
				
				if ( oReportRelationship.RightTable != null )
				{
					tdRightTable.appendChild(document.createTextNode(oReportRelationship.RightTable.DisplayName));
				}
			}
			else
			{
				while ( tdRightTable.childNodes.length > 0 )
					tdRightTable.removeChild(tdRightTable.firstChild);
				
				var div = document.createElement('div');
				div.id = 'tRelationships_RightTable' + nSelectedIndex.toString();
				div.className             = 'ztree';
				div.style.padding         = '6px';
				div.style.backgroundColor = 'white';
				tdRightTable.appendChild(div);
				div.onclick = function(e)
				{
					// 01/05/2014 Paul.  Prevent tree click from disabling window.
					e.preventDefault();
					e.stopPropagation();
				};
				
				var zNodes = new Array();
				var oCurrentTables = new Object();
				var oAllTables = new Object();
				oCurrentTables.name     = L10n.Term('ReportDesigner.LBL_TABLES_IN_QUERY');
				oCurrentTables.open     = true;
				oCurrentTables.children = new Array();
				oAllTables.name         = L10n.Term('ReportDesigner.LBL_TABLES');
				oAllTables.open         = false;
				oAllTables.children     = new Array();
				zNodes.push(oCurrentTables);
				zNodes.push(oAllTables    );
				for ( var i = 0; i < oReportDesign.Tables.length; i++ )
				{
					var table = new ReportTable(oReportDesign.Tables[i].Module);
					table.id   = table.TableName  ;
					table.name = table.DisplayName;
					oCurrentTables.children.push(table);
				}
				for ( var i = 0; i < arrReportDesignerModules.length; i++ )
				{
					var module = arrReportDesignerModules[i];
					var table = new ReportTable(module);
					table.id   = table.TableName  ;
					table.name = table.DisplayName;
					oAllTables.children.push(table);
				}
				
				var setting =
				{
					callback:
					{
						onClick: function(event, treeId, treeNode, clickFlag)
						{
							if ( treeNode.Module !== undefined )
							{
								var table = new ReportTable(treeNode.Module);
								oReportRelationship.RightTable = table;
								while ( tdRightTable.childNodes.length > 0 )
									tdRightTable.removeChild(tdRightTable.firstChild);
								
								tdRightTable.appendChild(document.createTextNode(table.DisplayName));
								tdJoinFields.innerHTML = JoinFieldsDisplayText(oReportRelationship.JoinFields);
								oReportDesign.Tables_UpdateAll();
							}
						}
					}
					, data:
					{
						simpleData:
						{
							enable: false
						}
					}
				};
				var treeObj = $.fn.zTree.init($('#' + div.id), setting, zNodes);
				if ( oReportRelationship.RightTable != null )
				{
					treeObj.selectNode(treeObj.getNodeByParam('id', oReportRelationship.RightTable.TableName));
				}
			}
		};
		tdJoinFields.onclick = function(e)
		{
			tRelationships_SelectCell(tdJoinFields);
			var nSelectedIndex = tdJoinFields.parentNode.rowIndex - 1;
			var oReportRelationship = oReportDesign.Relationships_GetAt(nSelectedIndex);
			if ( oReportRelationship.LeftTable == null || oReportRelationship.RightTable == null )
			{
				tdJoinFields.appendChild(document.createTextNode(L10n.Term('ReportDesigner.LBL_MISSING_JOIN_TABLE')));
			}
			else
			{
				var sInstructions = L10n.Term('ReportDesigner.LBL_JOIN_FIELDS_INSTRUCTIONS');
				sInstructions = sInstructions.replace('{0}', oReportRelationship.LeftTable.DisplayName );
				sInstructions = sInstructions.replace('{1}', oReportRelationship.RightTable.DisplayName);
				var sDialogHtml = '';
				// 07/15/2016 Paul.  We need to allow report and workflow modes, and they have different sIMAGE_SERVER values. 
				var sIMAGE_ROOT = sIMAGE_SERVER;
				sIMAGE_ROOT = sIMAGE_ROOT.replace('App_Themes/Atlantic/images/', '');
				// 04/18/2017 Paul.  Alert should have been removed before production build. 
				//alert(sIMAGE_ROOT);
				sDialogHtml += '<div id="divEditJoinFields">';
				sDialogHtml += '	' + sInstructions;
				sDialogHtml += '	<table cellspacing="0" cellpadding="4" width="100%">';
				sDialogHtml += '		<tbody>';
				sDialogHtml += '			<tr class="listViewThS1">';
				sDialogHtml += '				<td width="20%">&nbsp;</td>';
				sDialogHtml += '				<td width="80%" align="right">';
				sDialogHtml += '					<a href="#" onclick="return tJoinFields_AddJoin();"         ><img border="0" src="' + sIMAGE_ROOT + 'App_Themes/Atlantic/images/ReportDesignerRelationshipCreate.gif" /></a>';
				sDialogHtml += '					<a href="#" onclick="return tJoinFields_SelectedDelete();"  ><img border="0" src="' + sIMAGE_ROOT + 'App_Themes/Atlantic/images/ReportDesignerItemDelete.gif"         /></a>';
				sDialogHtml += '					<a href="#" onclick="return tJoinFields_SelectedMoveUp();"  ><img border="0" src="' + sIMAGE_ROOT + 'App_Themes/Atlantic/images/ReportDesignerItemMoveUp.gif"         /></a>';
				sDialogHtml += '					<a href="#" onclick="return tJoinFields_SelectedMoveDown();"><img border="0" src="' + sIMAGE_ROOT + 'App_Themes/Atlantic/images/ReportDesignerItemMoveDown.gif"       /></a>';
				sDialogHtml += '				</td>';
				sDialogHtml += '			</tr>';
				sDialogHtml += '		</tbody>';
				sDialogHtml += '	</table>';
				sDialogHtml += '	<table cellspacing="0" cellpadding="4" width="100%">';
				sDialogHtml += '		<tbody id="tJoinFields">';
				sDialogHtml += '			<tr id="" class="listViewThS1">';
				sDialogHtml += '				<td width="40%">' + L10n.Term('ReportDesigner.LBL_LEFT_JOIN_FIELD' ) + '</td>';
				sDialogHtml += '				<td width="15%">' + L10n.Term('ReportDesigner.LBL_OPERATOR'        ) + '</td>';
				sDialogHtml += '				<td width="40%">' + L10n.Term('ReportDesigner.LBL_RIGHT_JOIN_FIELD') + '</td>';
				sDialogHtml += '			</tr>';
				sDialogHtml += '		</tbody>';
				sDialogHtml += '	</table>';
				sDialogHtml += '	<table cellspacing="0" cellpadding="4" width="100%">';
				sDialogHtml += '		<tbody>';
				sDialogHtml += '			<tr class="listViewThS1">';
				sDialogHtml += '				<td id="tJoinFields_Error" width="50%" style="error">&nbsp;</td>';
				sDialogHtml += '				<td width="50%" align="right">';
				sDialogHtml += '					<input id="tJoinFields_OK"     type="button" value="' + L10n.Term('ReportDesigner.LBL_OK'    ) + '" />';
				sDialogHtml += '					<input id="tJoinFields_Cancel" type="button" value="' + L10n.Term('ReportDesigner.LBL_CANCEL') + '" />';
				sDialogHtml += '				</td>';
				sDialogHtml += '			</tr>';
				sDialogHtml += '		</tbody>';
				sDialogHtml += '	</table>';
				sDialogHtml += '</div>';
				var $dialog = $(sDialogHtml);
				$dialog.dialog(
				{
					  modal    : true
					, resizable: true
					, width    : 700
					, height   : 300
					, position : { my: 'left top', at: 'left top+18', of: '#tRelationships' }
					, title    : L10n.Term('ReportDesigner.LBL_EDIT_RELATED_FIELDS')
					, create   : function(event, ui)
					{
						try
						{
							// 01/05/2014 Paul.  We need a global value for current join fields so that changes can be aborted. 
							oCurrentJoinFields = new Array();
							// 01/05/2014 Paul.  We need a deep copy of the existing join fields array. 
							for ( var i = 0; i < oReportRelationship.JoinFields.length; i++ )
							{
								var oJoinField = new ReportJoinField(oReportRelationship.JoinFields[i]);
								oCurrentJoinFields.push(oJoinField);
								tJoinFields_AddJoin(oJoinField);
							}
							// 07/27/2014 Paul.  If no join is specified, then at least create the first row for quick creation.  Might even want to auto-detect values. 
							if ( oReportRelationship.JoinFields.length == 0 )
							{
								tJoinFields_AddJoin();
							}
							
							var tJoinFields_OK = document.getElementById('tJoinFields_OK');
							tJoinFields_OK.onclick = function()
							{
								var bMissingField = false;
								for ( var i = 0; i < oCurrentJoinFields.length; i++ )
								{
									if ( oCurrentJoinFields[i].LeftField == null || oCurrentJoinFields[i].RightField == null )
									{
										bMissingField = true;
										break;
									}
								}
								if ( bMissingField )
								{
									var tJoinFields_Error = document.getElementById('tJoinFields_Error');
									tJoinFields_Error.appendChild(document.createTextNode(L10n.Term('ReportDesigner.LBL_MISSING_JOIN_FIELD')));
								}
								else
								{
									oReportRelationship.JoinFields = oCurrentJoinFields;
									oCurrentJoinFields = null;
									tdJoinFields.innerHTML = JoinFieldsDisplayText(oReportRelationship.JoinFields);
									$dialog.dialog('close');
									oReportDesign.PreviewSQL();
								}
							}
							var tJoinFields_Cancel = document.getElementById('tJoinFields_Cancel');
							tJoinFields_Cancel.onclick = function()
							{
								oCurrentJoinFields = null;
								tdJoinFields.innerHTML = JoinFieldsDisplayText(oReportRelationship.JoinFields);
								$dialog.dialog('close');
							}
						}
						catch(e)
						{
							alert(e.message);
						}
					}
					, close: function(event, ui)
					{
						$dialog.dialog('destroy');
						var divEditJoinFields = document.getElementById('divEditJoinFields');
						divEditJoinFields.parentNode.removeChild(divEditJoinFields);
					}
				});
			}
		};
		tdJoinType.onclick = function(e)
		{
			// 12/29/2013 Paul.  Need to prevent the select click event from blocking the code.  stopPropagation() did not work. 
			if ( e.target.tagName == 'SELECT' )
				return;
			tRelationships_SelectCell(tdJoinType);
			if ( tdJoinType.childNodes.length > 0 && tdJoinType.childNodes[0].tagName == 'SELECT' )
			{
				while ( tdJoinType.childNodes.length > 0 )
					tdJoinType.removeChild(tdJoinType.firstChild);
				
				var nSelectedIndex = tdJoinType.parentNode.rowIndex - 1;
				var oReportRelationship = oReportDesign.Relationships_GetAt(nSelectedIndex);
				tdJoinType.appendChild(document.createTextNode(L10n.ListTerm('report_join_type_dom', oReportRelationship.JoinType)));
			}
			else
			{
				while ( tdJoinType.childNodes.length > 0 )
					tdJoinType.removeChild(tdJoinType.firstChild);
				
				var sel = document.createElement('select');
				tdJoinType.appendChild(sel);
				
				var nSelectedIndex = tdJoinType.parentNode.rowIndex - 1;
				var oReportRelationship = oReportDesign.Relationships_GetAt(nSelectedIndex);
				var arrJoinTypes = L10n.GetList('report_join_type_dom');
				for ( var i = 0; i < arrJoinTypes.length; i++ )
				{
					var opt = document.createElement('option');
					opt.value     = arrJoinTypes[i];
					opt.innerHTML = L10n.ListTerm('report_join_type_dom', arrJoinTypes[i]);
					sel.appendChild(opt);
					if ( oReportRelationship.JoinType == arrJoinTypes[i] )
						sel.options.selectedIndex = i;
				}
				sel.focus();
				sel.onchange = function(e)
				{
					var nSelectedIndex = tdJoinType.parentNode.rowIndex - 1;
					var sValue = sel.options[sel.options.selectedIndex].value;
					var oReportRelationship = oReportDesign.Relationships_GetAt(nSelectedIndex);
					oReportRelationship.JoinType = sValue;
					while ( tdJoinType.childNodes.length > 0 )
						tdJoinType.removeChild(tdJoinType.firstChild);
					tdJoinType.appendChild(document.createTextNode(L10n.ListTerm('report_join_type_dom', oReportRelationship.JoinType)));
					oReportDesign.PreviewSQL();
				};
			}
		};
	}
}

function tJoinFields_SelectCell(td)
{
	var tJoinFields = document.getElementById('tJoinFields');
	$('#tJoinFields td.QueryDesigner_Selected').removeClass('QueryDesigner_Selected').addClass('QueryDesigner');
	td.className = 'QueryDesigner_Selected';
}

function tJoinFields_SelectedIndex()
{
	var nSelectedIndex = -1;
	var td = $('#tJoinFields td.QueryDesigner_Selected');
	if ( td.length > 0 )
	{
		nSelectedIndex = td[0].parentNode.rowIndex - 1;
	}
	return nSelectedIndex;
}

function tJoinFields_SelectedDelete()
{
	var nSelectedIndex = tJoinFields_SelectedIndex();
	if ( nSelectedIndex != -1 )
	{
		var trCurrent  = tJoinFields.rows[nSelectedIndex + 1];
		tJoinFields.removeChild(trCurrent);
		
		// 01/05/2014 Paul.  We need a global value for current join fields so that changes can be aborted. 
		oCurrentJoinFields.splice(nSelectedIndex, 1);
	}
	return false;
}

function tJoinFields_SelectedMoveUp()
{
	var nSelectedIndex = tJoinFields_SelectedIndex();
	if ( nSelectedIndex > 0 )
	{
		var tJoinFields = document.getElementById('tJoinFields');
		// First row is header. 
		var trPrevious = tJoinFields.rows[nSelectedIndex + 1 - 1];
		var trCurrent  = tJoinFields.rows[nSelectedIndex + 1];
		tJoinFields.removeChild(trCurrent);
		tJoinFields.insertBefore(trCurrent, trPrevious);
		
		// 01/05/2014 Paul.  We need a global value for current join fields so that changes can be aborted. 
		var oJoinField = oCurrentJoinFields[nSelectedIndex];
		oCurrentJoinFields.splice(nSelectedIndex, 1);
		oCurrentJoinFields.splice(nSelectedIndex - 1, 0, oJoinField);
	}
	return false;
}

function tJoinFields_SelectedMoveDown()
{
	var nSelectedIndex = tJoinFields_SelectedIndex();
	if ( nSelectedIndex != -1 && nSelectedIndex < oReportDesign.Relationships.length - 1 )
	{
		var tJoinFields = document.getElementById('tJoinFields');
		// First row is header. 
		var trCurrent  = tJoinFields.rows[nSelectedIndex + 1];
		var trNext     = tJoinFields.rows[nSelectedIndex + 1 + 1];
		tJoinFields.removeChild(trNext);
		// There is no insertAfter, so just use insertBefore, but switch the arguments. 
		tJoinFields.insertBefore(trNext, trCurrent);
		
		// 01/05/2014 Paul.  We need a global value for current join fields so that changes can be aborted. 
		var oJoinField = oCurrentJoinFields[nSelectedIndex];
		oCurrentJoinFields.splice(nSelectedIndex, 1);
		oCurrentJoinFields.splice(nSelectedIndex + 1, 0, oJoinField);
	}
	return false;
}

function tJoinFields_AddJoin(oJoinField)
{
	try
	{
		var nRelationshipIndex = tRelationships_SelectedIndex();
		var oReportRelationship = oReportDesign.Relationships_GetAt(nRelationshipIndex);
		if ( oJoinField === undefined )
		{
			oJoinField = new ReportJoinField();
			oCurrentJoinFields.push(oJoinField);
		}
		else
		{
			//alert(dumpObj(oJoinField, 'oJoinField'));
		}

		var tJoinFields = document.getElementById('tJoinFields');
		var tr = tJoinFields.insertRow(-1);
		tJoinFields.appendChild(tr);
		var tdLeftJoinField  = document.createElement('td');
		var tdJoinOperator   = document.createElement('td');
		var tdRightJoinField = document.createElement('td');
		tr.appendChild(tdLeftJoinField );
		tr.appendChild(tdJoinOperator  );
		tr.appendChild(tdRightJoinField);
		tdLeftJoinField .className = 'QueryDesigner';
		tdJoinOperator  .className = 'QueryDesigner';
		tdRightJoinField.className = 'QueryDesigner';
		tdLeftJoinField .appendChild(document.createTextNode(oJoinField.LeftField  != null ? oJoinField.LeftField.DisplayName  : ''));
		tdJoinOperator  .appendChild(document.createTextNode(oJoinField.OperatorType));
		tdRightJoinField.appendChild(document.createTextNode(oJoinField.RightField != null ? oJoinField.RightField.DisplayName : ''));
	
		tdJoinOperator.onclick = function(e)
		{
			tJoinFields_SelectCell(tdJoinOperator);
		};
		tdLeftJoinField.onclick = function(e)
		{
			// 12/29/2013 Paul.  Need to prevent the select click event from blocking the code.  stopPropagation() did not work. 
			if ( e.target.tagName == 'SELECT' )
				return;
			tJoinFields_SelectCell(tdLeftJoinField);
			if ( tdLeftJoinField.childNodes.length > 0 && tdLeftJoinField.childNodes[0].tagName == 'SELECT' )
			{
				while ( tdLeftJoinField.childNodes.length > 0 )
					tdLeftJoinField.removeChild(tdLeftJoinField.firstChild);
				
				var nSelectedIndex = tdLeftJoinField.parentNode.rowIndex - 1;
				var oJoinField = oCurrentJoinFields[nSelectedIndex];
				if ( oJoinField.LeftField != null )
					tdLeftJoinField.appendChild(document.createTextNode(oJoinField.LeftField.DisplayName));
			}
			else
			{
				while ( tdLeftJoinField.childNodes.length > 0 )
					tdLeftJoinField.removeChild(tdLeftJoinField.firstChild);
				
				var sel = document.createElement('select');
				tdLeftJoinField.appendChild(sel);
				
				var nSelectedIndex = tdLeftJoinField.parentNode.rowIndex - 1;
				var oJoinField = oCurrentJoinFields[nSelectedIndex];
				var arrModuleFields = oReportDesign.ModuleFields(oReportRelationship.LeftTable.TableName);
				for ( var i = 0; i < arrModuleFields.length; i++ )
				{
					var opt = document.createElement('option');
					opt.value     = arrModuleFields[i].TableName + '.' + arrModuleFields[i].ColumnName;
					opt.appendChild(document.createTextNode(oReportRelationship.LeftTable.Module.DisplayName + ' ' + arrModuleFields[i].DisplayName));
					if ( bDebug )
						opt.appendChild(document.createTextNode(oReportRelationship.LeftTable.Module.DisplayName + ' ' + arrModuleFields[i].DisplayName + ' (' + opt.value + ')'));
					sel.appendChild(opt);
					// 01/05/2014 Paul.  If field does not have a value, then default to first item in list. This will allow OK without selecting item. 
					if ( oJoinField.LeftField == null )
						oJoinField.SetLeftFieldName(opt.value);
					else if ( oJoinField.LeftField.FieldName == opt.value )
						sel.options.selectedIndex = i;
				}
				sel.focus();
				sel.onchange = function(e)
				{
					var nSelectedIndex = tdLeftJoinField.parentNode.rowIndex - 1;
					var sValue = sel.options[sel.options.selectedIndex].value;
					var oJoinField = oCurrentJoinFields[nSelectedIndex];
					oJoinField.SetLeftFieldName(sValue);
					while ( tdLeftJoinField.childNodes.length > 0 )
						tdLeftJoinField.removeChild(tdLeftJoinField.firstChild);
					tdLeftJoinField.appendChild(document.createTextNode(oJoinField.LeftField.DisplayName));
				};
			}
		};
		tdRightJoinField.onclick = function(e)
		{
			// 12/29/2013 Paul.  Need to prevent the select click event from blocking the code.  stopPropagation() did not work. 
			if ( e.target.tagName == 'SELECT' )
				return;
			tJoinFields_SelectCell(tdRightJoinField);
			if ( tdRightJoinField.childNodes.length > 0 && tdRightJoinField.childNodes[0].tagName == 'SELECT' )
			{
				while ( tdRightJoinField.childNodes.length > 0 )
					tdRightJoinField.removeChild(tdRightJoinField.firstChild);
				
				var nSelectedIndex = tdRightJoinField.parentNode.rowIndex - 1;
				var oJoinField = oCurrentJoinFields[nSelectedIndex];
				if ( oJoinField.RightField != null )
					tdRightJoinField.appendChild(document.createTextNode(oJoinField.RightField.DisplayName));
			}
			else
			{
				while ( tdRightJoinField.childNodes.length > 0 )
					tdRightJoinField.removeChild(tdRightJoinField.firstChild);
				
				var sel = document.createElement('select');
				tdRightJoinField.appendChild(sel);
				
				var nSelectedIndex = tdRightJoinField.parentNode.rowIndex - 1;
				var oJoinField = oCurrentJoinFields[nSelectedIndex];
				var arrModuleFields = oReportDesign.ModuleFields(oReportRelationship.RightTable.TableName);
				for ( var i = 0; i < arrModuleFields.length; i++ )
				{
					var opt = document.createElement('option');
					opt.value     = arrModuleFields[i].TableName + '.' + arrModuleFields[i].ColumnName;
					opt.appendChild(document.createTextNode(oReportRelationship.RightTable.Module.DisplayName + ' ' + arrModuleFields[i].DisplayName));
					if ( bDebug )
						opt.appendChild(document.createTextNode(oReportRelationship.RightTable.Module.DisplayName + ' ' + arrModuleFields[i].DisplayName + ' (' + opt.value + ')'));
					sel.appendChild(opt);
					// 01/05/2014 Paul.  If field does not have a value, then default to first item in list. This will allow OK without selecting item. 
					if ( oJoinField.RightField == null )
						oJoinField.SetRightFieldName(opt.value);
					else if ( oJoinField.RightField.FieldName == opt.value )
						sel.options.selectedIndex = i;
				}
				sel.focus();
				sel.onchange = function(e)
				{
					var nSelectedIndex = tdRightJoinField.parentNode.rowIndex - 1;
					var sValue = sel.options[sel.options.selectedIndex].value;
					var oJoinField = oCurrentJoinFields[nSelectedIndex];
					oJoinField.SetRightFieldName(sValue);
					while ( tdRightJoinField.childNodes.length > 0 )
						tdRightJoinField.removeChild(tdRightJoinField.firstChild);
					tdRightJoinField.appendChild(document.createTextNode(oJoinField.RightField.DisplayName));
				};
			}
		};
	}
	catch(e)
	{
		alert(e.message);
	}
}

function tAppliedFilters_SelectCell(td)
{
	var tAppliedFilters = document.getElementById('tAppliedFilters');
	$('#tAppliedFilters td.QueryDesigner_Selected').removeClass('QueryDesigner_Selected').addClass('QueryDesigner');
	td.className = 'QueryDesigner_Selected';
}

function tAppliedFilters_SelectedIndex()
{
	var nSelectedIndex = -1;
	var td = $('#tAppliedFilters td.QueryDesigner_Selected');
	if ( td.length > 0 )
	{
		nSelectedIndex = td[0].parentNode.rowIndex - 1;
	}
	return nSelectedIndex;
}

function tAppliedFilters_SelectedDelete()
{
	var nSelectedIndex = tAppliedFilters_SelectedIndex();
	if ( nSelectedIndex != -1 )
	{
		var trCurrent  = tAppliedFilters.rows[nSelectedIndex + 1];
		tAppliedFilters.removeChild(trCurrent);
		
		oReportDesign.AppliedFilters_Delete(nSelectedIndex);
		oReportDesign.PreviewSQL();
	}
	return false;
}

function tAppliedFilters_SelectedMoveUp()
{
	var nSelectedIndex = tAppliedFilters_SelectedIndex();
	if ( nSelectedIndex > 0 )
	{
		var tAppliedFilters = document.getElementById('tAppliedFilters');
		// First row is header. 
		var trPrevious = tAppliedFilters.rows[nSelectedIndex + 1 - 1];
		var trCurrent  = tAppliedFilters.rows[nSelectedIndex + 1];
		tAppliedFilters.removeChild(trCurrent);
		tAppliedFilters.insertBefore(trCurrent, trPrevious);
		
		oReportDesign.AppliedFilters_MoveUp(nSelectedIndex);
		oReportDesign.PreviewSQL();
	}
	return false;
}

function tAppliedFilters_SelectedMoveDown()
{
	var nSelectedIndex = tAppliedFilters_SelectedIndex();
	if ( nSelectedIndex != -1 && nSelectedIndex < oReportDesign.AppliedFilters.length - 1 )
	{
		var tAppliedFilters = document.getElementById('tAppliedFilters');
		// First row is header. 
		var trCurrent  = tAppliedFilters.rows[nSelectedIndex + 1];
		var trNext     = tAppliedFilters.rows[nSelectedIndex + 1 + 1];
		tAppliedFilters.removeChild(trNext);
		// There is no insertAfter, so just use insertBefore, but switch the arguments. 
		tAppliedFilters.insertBefore(trNext, trCurrent);
		
		oReportDesign.AppliedFilters_MoveDown(nSelectedIndex);
		oReportDesign.PreviewSQL();
	}
	return false;
}

function tAppliedFilters_AddFilter(oReportFilter)
{
	if ( oReportFilter === undefined || oReportFilter == null )
		oReportFilter = oReportDesign.AppliedFilters_AddFilter();
	if ( oReportFilter != null )
	{
		var tAppliedFilters = document.getElementById('tAppliedFilters');
		var tr = tAppliedFilters.insertRow(-1);
		tAppliedFilters.appendChild(tr);
		var tdField     = document.createElement('td');
		var tdOperator  = document.createElement('td');
		var tdValue     = document.createElement('td');
		var tdParameter = document.createElement('td');
		tr.appendChild(tdField    );
		tr.appendChild(tdOperator );
		tr.appendChild(tdValue    );
		tr.appendChild(tdParameter);
		tdField    .className = 'QueryDesigner';
		tdOperator .className = 'QueryDesigner';
		tdValue    .className = 'QueryDesigner';
		tdParameter.className = 'QueryDesigner';
		tdOperator.style.whiteSpace = 'nowrap';
		tdValue   .style.whiteSpace = 'nowrap';
		// 07/17/2016 Paul.  Hide the parameter column in workflow mode. 
		if ( bReportDesignerWorkflowMode )
			tdParameter.style.display = 'none';
		
		if ( oReportFilter.Field != null )
		{
			var field = oReportFilter.Field;
			// 03/30/2020 Paul.  Displayname was being diplayed twice. 
			if ( bDebug )
			{
				tdField.appendChild(document.createTextNode(field.DisplayName + ' (' + field.TableName + '.' + field.ColumnName + ')'));
			}
			else
			{
				tdField.appendChild(document.createTextNode(field.DisplayName));
			}
		}
		if ( oReportFilter.Operator != null )
		{
			// 07/17/2016 Paul.  Allow the filter operator to be changed to a workflow version. 
			// 02/11/2018 Paul.  Workflow mode uses older style of operators. 
			if ( bReportDesignerWorkflowMode && oReportFilter.Field != null && oReportFilter.Field.DataType != null )
			{
				// 02/11/2018 Paul.  We need to determine if the string should be treated as a enum. 
				report_filter_operator_dom = oReportFilter.CsType() + '_operator_dom';
			}
			tdOperator.appendChild(document.createTextNode(L10n.ListTerm(report_filter_operator_dom, oReportFilter.Operator)));
		}
		if ( oReportFilter.Value != null )
		{
			while ( tdValue.childNodes.length > 0 )
				tdValue.removeChild(tdValue.firstChild);
			if ( $.isArray(oReportFilter.Value) )
				tdValue.appendChild(document.createTextNode(oReportFilter.Value.join()));
			else
				tdValue.appendChild(document.createTextNode(oReportFilter.Value));
		}
		
		var chkParameter = document.createElement('input');
		chkParameter.type      = 'checkbox';
		chkParameter.className = 'checkbox';
		chkParameter.value     = '1';
		tdParameter.appendChild(chkParameter);
		if ( oReportFilter.Parameter != null )
			chkParameter.checked   = oReportFilter.Parameter;
		
		tdField.onclick = function(e)
		{
			var nSelectedIndex = tdField.parentNode.rowIndex - 1;
			var oReportFilter = oReportDesign.AppliedFilters_GetAt(nSelectedIndex);
			// 01/05/2014 Paul.  Use e.preventDefault() and e.stopPropagation(). 
			//if ( e.target.tagName == 'DIV' )
			//	return;
			tAppliedFilters_SelectCell(tdField);
			if ( tdField.childNodes.length > 0 && tdField.childNodes[0].tagName == 'DIV' )
			{
				while ( tdField.childNodes.length > 0 )
					tdField.removeChild(tdField.firstChild);
				
				if ( oReportFilter.Field != null )
				{
					var field = oReportFilter.Field;
					// 03/30/2020 Paul.  Displayname was being diplayed twice. 
					if ( bDebug )
						tdField.appendChild(document.createTextNode(field.DisplayName + ' (' + field.TableName + '.' + field.ColumnName + ')'));
					else
						tdField.appendChild(document.createTextNode(field.DisplayName));
				}
			}
			else
			{
				while ( tdField.childNodes.length > 0 )
					tdField.removeChild(tdField.firstChild);
				
				var div = document.createElement('div');
				div.id = 'tAppliedFilters_Field' + nSelectedIndex.toString();
				div.className             = 'ztree';
				div.style.padding         = '6px';
				div.style.backgroundColor = 'white';
				tdField.appendChild(div);
				div.onclick = function(e)
				{
					// 01/05/2014 Paul.  Prevent tree click from disabling window.
					e.preventDefault();
					e.stopPropagation();
				};
				
				var oSelectedNode = null;
				var zNodes = ReportDesigner_CreateTableNodes(oReportFilter, oSelectedNode);
				
				var setting =
				{
					view:
					{
						txtSelectedEnable: true
						, dblClickExpand: function(treeId, treeNode)
						{
							return treeNode.level > 0;
						}
					}
					, callback:
					{
						onClick: function(event, treeId, treeNode, clickFlag)
						{
							if ( treeNode.Field !== undefined )
							{
								var field = treeNode.Field;
								oReportFilter.Field = field;
								while ( tdField.childNodes.length > 0 )
									tdField.removeChild(tdField.firstChild);
								
								tdField.appendChild(document.createTextNode(field.DisplayName));
								if ( bDebug )
									tdField.appendChild(document.createTextNode(field.DisplayName + ' (' + field.TableName + '.' + field.ColumnName + ')'));
								oReportDesign.PreviewSQL();
							}
							else
							{
								var treeObj = $.fn.zTree.getZTreeObj(div.id);
								treeObj.destroy();
								
								var oSelectedNode = null;
								var zNodes = ReportDesigner_CreateTableNodes(oReportFilter, oSelectedNode);
								var treeObj = $.fn.zTree.init($('#' + div.id), setting, zNodes);
								if ( oSelectedNode != null )
								{
									treeObj.selectNode(treeObj.getNodeByParam('id', oSelectedNode.id));
								}
							}
						}
					}
					, data:
					{
						simpleData:
						{
							enable: false
						}
					}
				};
				var treeObj = $.fn.zTree.init($('#' + div.id), setting, zNodes);
				if ( oSelectedNode != null )
				{
					treeObj.selectNode(treeObj.getNodeByParam('id', oSelectedNode.id));
				}
			}
		};
		tdOperator.onclick = function(e)
		{
			// 12/29/2013 Paul.  Need to prevent the select click event from blocking the code.  stopPropagation() did not work. 
			if ( e.target.tagName == 'SELECT' )
				return;
			tAppliedFilters_SelectCell(tdOperator);
			if ( tdOperator.childNodes.length > 0 && tdOperator.childNodes[0].tagName == 'SELECT' )
			{
				while ( tdOperator.childNodes.length > 0 )
					tdOperator.removeChild(tdOperator.firstChild);
				
				var nSelectedIndex = tdOperator.parentNode.rowIndex - 1;
				var oReportFilter = oReportDesign.AppliedFilters_GetAt(nSelectedIndex);
				
				if ( oReportFilter.Operator != null )
				{
					// 07/17/2016 Paul.  Allow the filter operator to be changed to a workflow version. 
					// 02/11/2018 Paul.  Workflow mode uses older style of operators. 
					if ( bReportDesignerWorkflowMode && oReportFilter.Field != null && oReportFilter.Field.DataType != null )
					{
						// 02/11/2018 Paul.  We need to determine if the string should be treated as a enum. 
						report_filter_operator_dom = oReportFilter.CsType() + '_operator_dom';
					}
					tdOperator.appendChild(document.createTextNode(L10n.ListTerm(report_filter_operator_dom, oReportFilter.Operator)));
				}
			}
			else
			{
				while ( tdOperator.childNodes.length > 0 )
					tdOperator.removeChild(tdOperator.firstChild);
				
				var sel = document.createElement('select');
				tdOperator.appendChild(sel);
				
				var nSelectedIndex = tdOperator.parentNode.rowIndex - 1;
				var oReportFilter = oReportDesign.AppliedFilters_GetAt(nSelectedIndex);
				// 02/11/2018 Paul.  Workflow mode uses older style of operators. 
				if ( bReportDesignerWorkflowMode && oReportFilter.Field != null && oReportFilter.Field.DataType != null )
				{
					// 02/11/2018 Paul.  We need to determine if the string should be treated as a enum. 
					report_filter_operator_dom = oReportFilter.CsType() + '_operator_dom';
				}
				// 07/17/2016 Paul.  Allow the filter operator to be changed to a workflow version. 
				var arrOperators = L10n.GetList(report_filter_operator_dom);
				for ( var i = 0; i < arrOperators.length; i++ )
				{
					var opt = document.createElement('option');
					opt.value     = arrOperators[i];
					// 07/17/2016 Paul.  Allow the filter operator to be changed to a workflow version. 
					opt.innerHTML = L10n.ListTerm(report_filter_operator_dom, arrOperators[i]);
					sel.appendChild(opt);
					if ( oReportFilter.Operator == arrOperators[i] )
						sel.options.selectedIndex = i;
				}
				// 02/11/2018 Paul.  Workflow mode uses older style of operators. 
				if ( bReportDesignerWorkflowMode )
				{
					var opt = document.createElement('option');
					opt.value = 'changed';
					opt.innerHTML = L10n.ListTerm('workflow_filter_operator_dom', opt.value);
					sel.appendChild(opt);
					if ( oReportFilter.Operator == opt.value )
						sel.options.selectedIndex = sel.options.length - 1;
				}
				sel.focus();
				sel.onchange = function(e)
				{
					var nSelectedIndex = tdOperator.parentNode.rowIndex - 1;
					var sValue = sel.options[sel.options.selectedIndex].value;
					var oReportFilter = oReportDesign.AppliedFilters_GetAt(nSelectedIndex);
					oReportFilter.Operator = sValue;
					while ( tdOperator.childNodes.length > 0 )
						tdOperator.removeChild(tdOperator.firstChild);
					// 07/17/2016 Paul.  Add support for changed to support workflow. 
					// 08/17/2018 Paul.  Need to include empty and not_empty for workflow mode. 
					if ( oReportFilter.Operator == 'empty' || oReportFilter.Operator == 'not_empty' || oReportFilter.Operator == 'is null' || oReportFilter.Operator == 'is not null' || oReportFilter.Operator == 'changed' )
					{
						oReportFilter.Value = null;
						while ( tdValue.childNodes.length > 0 )
							tdValue.removeChild(tdValue.firstChild);
					}
					else if ( oReportFilter.Operator == 'in' || oReportFilter.Operator == 'not in' )
					{
						if ( oReportFilter.Value != null && !$.isArray(oReportFilter.Value) )
							oReportFilter.Value = null;
					}
					// 02/24/2015 Paul.  Add support for between filter clause. 
					else if ( oReportFilter.Operator == 'between' )
					{
						if ( oReportFilter.Value != null && !$.isArray(oReportFilter.Value) && oReportFilter.Value.length != 2 )
							oReportFilter.Value = null;
					}
					else
					{
						if ( oReportFilter.Value != null && $.isArray(oReportFilter.Value) )
							oReportFilter.Value = null;
					}
					if ( oReportFilter.Value != null )
					{
						while ( tdValue.childNodes.length > 0 )
							tdValue.removeChild(tdValue.firstChild);
						if ( $.isArray(oReportFilter.Value) )
							tdValue.appendChild(document.createTextNode(oReportFilter.Value.join()));
						else
							tdValue.appendChild(document.createTextNode(oReportFilter.Value));
					}
					
					// 07/17/2016 Paul.  Allow the filter operator to be changed to a workflow version. 
					// 02/11/2018 Paul.  Workflow mode uses older style of operators. 
					if ( bReportDesignerWorkflowMode && oReportFilter.Field != null && oReportFilter.Field.DataType != null )
					{
						// 02/11/2018 Paul.  We need to determine if the string should be treated as a enum. 
						report_filter_operator_dom = oReportFilter.CsType() + '_operator_dom';
					}
					tdOperator.appendChild(document.createTextNode(L10n.ListTerm(report_filter_operator_dom, oReportFilter.Operator)));
					oReportDesign.PreviewSQL();
				};
				sel.onblur = sel.onchange;
			}
		};
		tdParameter.onclick = function(e)
		{
			tAppliedFilters_SelectCell(tdParameter);
		};
		chkParameter.onclick = function(e)
		{
			tAppliedFilters_SelectCell(chkParameter.parentNode);
			var nSelectedIndex = chkParameter.parentNode.parentNode.rowIndex - 1;
			var oReportFilter = oReportDesign.AppliedFilters_GetAt(nSelectedIndex);
			oReportFilter.Parameter = chkParameter.checked;
			oReportDesign.PreviewSQL();
		};
		tdValue.onclick = function(e)
		{
			var nSelectedIndex = tdValue.parentNode.rowIndex - 1;
			var oReportFilter = oReportDesign.AppliedFilters_GetAt(nSelectedIndex);
			tAppliedFilters_SelectCell(tdValue);
			// 07/17/2016 Paul.  Add support for changed to support workflow. 
			if ( oReportFilter.Operator == 'is null' || oReportFilter.Operator == 'is not null' || oReportFilter.Operator == 'changed' )
				return;

			if ( tdValue.childNodes.length > 0 && tdValue.childNodes[0].tagName == 'DIV' )
			{
				while ( tdValue.childNodes.length > 0 )
					tdValue.removeChild(tdValue.firstChild);
				
				if ( oReportFilter.Value != null )
				{
					while ( tdValue.childNodes.length > 0 )
						tdValue.removeChild(tdValue.firstChild);
					if ( $.isArray(oReportFilter.Value) )
						tdValue.appendChild(document.createTextNode(oReportFilter.Value.join()));
					else
						tdValue.appendChild(document.createTextNode(oReportFilter.Value));
				}
			}
			else
			{
				while ( tdValue.childNodes.length > 0 )
					tdValue.removeChild(tdValue.firstChild);
				
				var div = document.createElement('div');
				//div.style.backgroundColor = 'white';
				div.className   = 'QueryDesigner';
				tdValue.appendChild(div);
				div.onclick = function(e)
				{
					tAppliedFilters_SelectCell(tdValue);
					// 02/14/2014 Paul.  Use e.preventDefault() and e.stopPropagation(). 
					e.preventDefault();
					e.stopPropagation();
				};
				
				var bPopup   = false;
				var bTextBox = false;
				var bListBox = false;
				// 02/11/2018 Paul.  Workflow mode uses older style of operators. 
				if ( bReportDesignerWorkflowMode )
				{
					var sCOMMON_DATA_TYPE = oReportFilter.Field.DataType.toLowerCase();
					if ( sCOMMON_DATA_TYPE == "ansistring" )
						sCOMMON_DATA_TYPE = "string";
					// 02/11/2018 Paul.  We need to determine if the string should be treated as a enum. 
					if ( oReportFilter.Field != null )
					{
						var lay = ReportDesign_EditView_Layout(oReportFilter.Field.Module.ModuleName, oReportFilter.Field.ColumnName);
						if ( lay != null )
						{
							if ( !Sql.IsEmptyString(lay.LIST_NAME) )
								sCOMMON_DATA_TYPE = "enum";
						}
					}
					switch ( sCOMMON_DATA_TYPE )
					{
						case "string":
						{
							switch ( oReportFilter.Operator )
							{
								case "equals"        :  bTextBox = true ;  break;
								case "contains"      :  bTextBox = true ;  break;
								case "starts_with"   :  bTextBox = true ;  break;
								case "ends_with"     :  bTextBox = true ;  break;
								case "not_equals_str":  bTextBox = true ;  break;
								case "empty"         :  break;
								case "not_empty"     :  break;
								case "changed"       :  break;
								case "unchanged"     :  break;
								case "increased"     :  break;
								case "decreased"     :  break;
								// 08/25/2011 Paul.  A customer wants more use of NOT in string filters. 
								case "not_contains"   :  bTextBox = true ;  break;
								case "not_starts_with":  bTextBox = true ;  break;
								case "not_ends_with"  :  bTextBox = true ;  break;
								// 02/14/2013 Paul.  A customer wants to use like in string filters. 
								case "like"           :  bTextBox = true ;  break;
								case "not_like"       :  bTextBox = true ;  break;
								// 07/23/2013 Paul.  Add greater and less than conditions. 
								case "less"          :  bTextBox = true ;  break;
								case "less_equal"    :  bTextBox = true ;  break;
								case "greater"       :  bTextBox = true ;  break;
								case "greater_equal" :  bTextBox = true ;  break;
							}
							break;
						}
						case "datetime":
						{
							switch ( oReportFilter.Operator )
							{
								case "on"               :  bTextBox = true;  break;
								case "before"           :  bTextBox = true;  break;
								case "after"            :  bTextBox = true;  break;
								case "between_dates"    :  bTextBox = true;  break;
								case "not_equals_str"   :  bTextBox = true;  break;
								case "empty"            :  break;
								case "not_empty"        :  break;
								case "is_before"        :  break;
								case "is_after"         :  break;
								case "tp_yesterday"     :  break;
								case "tp_today"         :  break;
								case "tp_tomorrow"      :  break;
								case "tp_last_7_days"   :  break;
								case "tp_next_7_days"   :  break;
								case "tp_last_month"    :  break;
								case "tp_this_month"    :  break;
								case "tp_next_month"    :  break;
								case "tp_last_30_days"  :  break;
								case "tp_next_30_days"  :  break;
								case "tp_last_year"     :  break;
								case "tp_this_year"     :  break;
								case "tp_next_year"     :  break;
								case "changed"          :  break;
								case "unchanged"        :  break;
								case "increased"        :  break;
								case "decreased"        :  break;
								case "tp_minutes_after" :  bTextBox = true ;  break;
								case "tp_hours_after"   :  bTextBox = true ;  break;
								case "tp_days_after"    :  bTextBox = true ;  break;
								case "tp_weeks_after"   :  bTextBox = true ;  break;
								case "tp_months_after"  :  bTextBox = true ;  break;
								case "tp_years_after"   :  bTextBox = true ;  break;
								case "tp_minutes_before":  bTextBox = true ;  break;
								case "tp_hours_before"  :  bTextBox = true ;  break;
								case "tp_days_before"   :  bTextBox = true ;  break;
								case "tp_weeks_before"  :  bTextBox = true ;  break;
								case "tp_months_before" :  bTextBox = true ;  break;
								case "tp_years_before"  :  bTextBox = true ;  break;
								// 12/04/2008 Paul.  We need to be able to do an an equals. 
								case "tp_days_old"      :  bTextBox = true ;  break;
								case "tp_weeks_old"     :  bTextBox = true ;  break;
								case "tp_months_old"    :  bTextBox = true ;  break;
								case "tp_years_old"     :  bTextBox = true ;  break;
							}
							break;
						}
						case "int32":
						{
							switch ( oReportFilter.Operator )
							{
								case "equals"    :  bTextBox = true ;  break;
								case "less"      :  bTextBox = true ;  break;
								case "greater"   :  bTextBox = true ;  break;
								case "between"   :  bTextBox = true ;  break;
								case "not_equals":  bTextBox = true ;  break;
								case "empty"     :  break;
								case "not_empty" :  break;
								case "changed"   :  break;
								case "unchanged" :  break;
								case "increased" :  break;
								case "decreased" :  break;
								// 07/23/2013 Paul.  Add greater and less than conditions. 
								case "less_equal"    :  bTextBox = true ;  break;
								case "greater_equal" :  bTextBox = true ;  break;
							}
							break;
						}
						case "decimal":
						{
							switch ( oReportFilter.Operator )
							{
								case "equals"    :  bTextBox = true ;  break;
								case "less"      :  bTextBox = true ;  break;
								case "greater"   :  bTextBox = true ;  break;
								case "between"   :  bTextBox = true ;  break;
								case "not_equals":  bTextBox = true ;  break;
								case "empty"     :  break;
								case "not_empty" :  break;
								case "changed"   :  break;
								case "unchanged" :  break;
								case "increased" :  break;
								case "decreased" :  break;
								// 07/23/2013 Paul.  Add greater and less than conditions. 
								case "less_equal"    :  bTextBox = true ;  break;
								case "greater_equal" :  bTextBox = true ;  break;
							}
							break;
						}
						case "float":
						{
							switch ( oReportFilter.Operator )
							{
								case "equals"    :  bTextBox = true ;  break;
								case "less"      :  bTextBox = true ;  break;
								case "greater"   :  bTextBox = true ;  break;
								case "between"   :  bTextBox = true ;  break;
								case "not_equals":  bTextBox = true ;  break;
								case "empty"     :  break;
								case "not_empty" :  break;
								case "changed"   :  break;
								case "unchanged" :  break;
								case "increased" :  break;
								case "decreased" :  break;
								// 07/23/2013 Paul.  Add greater and less than conditions. 
								case "less_equal"    :  bTextBox = true ;  break;
								case "greater_equal" :  bTextBox = true ;  break;
							}
							break;
						}
						case "bool":
						{
							switch ( oReportFilter.Operator )
							{
								case "equals"    :  bTextBox = true ;  break;
								case "empty"     :  break;
								case "not_empty" :  break;
								case "changed"   :  break;
								case "unchanged" :  break;
								case "increased" :  break;
								case "decreased" :  break;
							}
							break;
						}
						case "guid":
						{
							switch ( oReportFilter.Operator )
							{
								// 05/05/2010 Paul.  The Select button was not being made visible. 
								case "is"            :  break;
								case "equals"        :  bTextBox = true ;  break;
								case "contains"      :  bTextBox = true ;  break;
								case "starts_with"   :  bTextBox = true ;  break;
								case "ends_with"     :  bTextBox = true ;  break;
								case "not_equals_str":  bTextBox = true ;  break;
								case "empty"         :  break;
								case "not_empty"     :  break;
								case "changed"       :  break;
								case "unchanged"     :  break;
								case "increased"     :  break;
								case "decreased"     :  break;
								case "one_of"        :  bListBox = true;  break;
							}
							break;
						}
						case "enum":
						{
							switch ( oReportFilter.Operator )
							{
								case "is"            :  bListBox = true;  break;
								case "one_of"        :  bListBox = true;  break;
								case "empty"         :  break;
								case "not_empty"     :  break;
								case "changed"       :  break;
								case "unchanged"     :  break;
								case "increased"     :  break;
								case "decreased"     :  break;
							}
							break;
						}
					}
				}
				else
				{
					switch ( oReportFilter.Operator )
					{
						case 'in'         :  bListBox = true ;  break;
						case 'not in'     :  bListBox = true ;  break;
						case 'is null'    :  bTextBox = false;  break;
						case 'is not null':  bTextBox = false;  break;
						// 07/17/2016 Paul.  Add support for changed to support workflow. 
						case 'changed'    :  bTextBox = false;  break;
						default           :  bTextBox = true ;  break;
					}
				}
//debugger;
				if ( (oReportFilter.Operator == '=' || oReportFilter.Operator == 'is') && oReportFilter.Field != null && oReportFilter.Field.DataType == 'Guid' )
				{
					var lay = ReportDesign_EditView_Layout(oReportFilter.Field.Module.ModuleName, oReportFilter.Field.ColumnName);
					if ( lay != null && (lay.FIELD_TYPE == 'ModulePopup' || lay.FIELD_TYPE == 'ChangeButton') && !Sql.IsEmptyString(lay.MODULE_TYPE) )
					{
						bTextBox = false;
						bListBox = false;
						bPopup   = true ;
						
						var sEDIT_NAME                  = lay.EDIT_NAME                 ;
						var sFIELD_TYPE                 = lay.FIELD_TYPE                ;
						var sDATA_LABEL                 = lay.DATA_LABEL                ;
						var sDATA_FIELD                 = lay.DATA_FIELD                ;
						var sDATA_FORMAT                = lay.DATA_FORMAT               ;
						var sDISPLAY_FIELD              = lay.DISPLAY_FIELD             ;
						var nFORMAT_TAB_INDEX           = Sql.ToInteger(lay.FORMAT_TAB_INDEX );
						var nFORMAT_MAX_LENGTH          = Sql.ToInteger(lay.FORMAT_MAX_LENGTH);
						var nFORMAT_SIZE                = Sql.ToInteger(lay.FORMAT_SIZE      );
						var sMODULE_TYPE                = lay.MODULE_TYPE               ;
						
						var row           = null;
						var sLayoutPanel  = 'tAppliedFilters' + nSelectedIndex.toString();
						var divPopupValue = document.createElement('div');
						divPopupValue.style.whiteSpace = 'nowrap';
						div.appendChild(divPopupValue);
						var sTEMP_DISPLAY_FIELD = Sql.IsEmptyString(sDISPLAY_FIELD) ? sDATA_FIELD + '_NAME' : sDISPLAY_FIELD;
						var txt = document.createElement('input');
						txt.id = sLayoutPanel + '_ctlEditView_' + sTEMP_DISPLAY_FIELD;
						txt.type      = 'text';
						if ( nFORMAT_MAX_LENGTH > 0 ) txt.maxlength = nFORMAT_MAX_LENGTH;
						if ( nFORMAT_TAB_INDEX  > 0 ) txt.tabindex  = nFORMAT_TAB_INDEX ;
						if ( nFORMAT_SIZE       > 0 ) txt.size      = nFORMAT_SIZE      ;
						if ( oReportFilter.Value != null && !$.isArray(oReportFilter.Value) )
						{
							BindArguments(function(sMODULE_NAME, sID, txt, context)
							{
								Crm.Modules.ItemName(sMODULE_NAME, sID, function(status, message)
								{
									if ( status == 1 )
										txt.value = message;
								}, context);
							}, sMODULE_TYPE, oReportFilter.Value, txt, this)();
						}
						divPopupValue.appendChild(txt);
					
						var hid = document.createElement('input');
						hid.id   = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD;
						hid.type = 'hidden';
						if ( oReportFilter.Value != null && !$.isArray(oReportFilter.Value) )
						{
							hid.value = oReportFilter.Value;
						}
						divPopupValue.appendChild(hid);
					
						var btnChange = document.createElement('input');
						btnChange.type      = 'button';
						btnChange.className = 'button';
						btnChange.title     = L10n.Term('.LBL_SELECT_BUTTON_TITLE');
						btnChange.value     = L10n.Term('.LBL_SELECT_BUTTON_LABEL');
						btnChange.style.marginLeft  = '4px';
						btnChange.style.marginRight = '2px';
						btnChange.onclick = function(e) // BindArguments(function(txt, hid, sMODULE_TYPE, sDATA_LABEL, sFIELD_TYPE, sDATA_FIELD, tdValue, oReportFilter)
						{
							var $dialog = $('<div id="' + hid.id + '_divPopup"><div id="divPopupActionsPanel" /><div id="divPopupLayoutPanel" /></div>');
							$dialog.dialog(
							{
								  modal    : true
								, resizable: true
								, width    : $(window).width() > 0 ? ($(window).width() - 60) : 800
								, height   : (navigator.userAgent.indexOf('iPad') > 0 ? 'auto' : ($(window).height() > 0 ? $(window).height() - 60 : 800))
								, title    : L10n.Term(sMODULE_TYPE + '.LBL_LIST_FORM_TITLE')
								, create   : function(event, ui)
								{
									try
									{
										var oPopupViewUI = new PopupViewUI();
										oPopupViewUI.Load('divPopupLayoutPanel', 'divPopupActionsPanel', sMODULE_TYPE, false, function(status, message)
										{
											if ( status == 1 )
											{
												hid.value = message.ID  ;
												txt.value = message.NAME;
												while ( tdValue.childNodes.length > 0 )
													tdValue.removeChild(tdValue.firstChild);
												
												oReportFilter.Value = message.ID;
												if ( oReportFilter.Value != null )
												{
													if ( $.isArray(oReportFilter.Value) )
														tdValue.appendChild(document.createTextNode(oReportFilter.Value.join()));
													else
														tdValue.appendChild(document.createTextNode(oReportFilter.Value));
												}
												// 02/21/2013 Paul.  Use close instead of destroy. 
												$dialog.dialog('close');
												oReportDesign.PreviewSQL();
											}
											else if ( status == -2 )
											{
												// 02/21/2013 Paul.  Use close instead of destroy. 
												$dialog.dialog('close');
											}
											else if ( status == -1 )
											{
												SplendidError.SystemMessage(message);
											}
										});
									}
									catch(e)
									{
										SplendidError.SystemError(e, 'PopupViewUI dialog');
									}
								}
								, close    : function(event, ui)
								{
									$dialog.dialog('destroy');
									// 10/17/2011 Paul.  We have to remove the new HTML, otherwise there will be multiple definitions for divPopupLayoutPanel. 
									var divPopup = document.getElementById(hid.id + '_divPopup');
									divPopup.parentNode.removeChild(divPopup);
								}
							});
						//}, txt, hid, sMODULE_TYPE, sDATA_LABEL, sFIELD_TYPE, sDATA_FIELD, tdValue, oReportFilter);
						};
						divPopupValue.appendChild(btnChange);
						
						var btnClear = document.createElement('input');
						btnClear.type      = 'button';
						btnClear.className = 'button';
						btnClear.title     = L10n.Term('.LBL_CLEAR_BUTTON_TITLE');
						btnClear.value     = L10n.Term('.LBL_CLEAR_BUTTON_LABEL');
						btnClear.style.marginLeft  = '2px';
						btnClear.style.marginRight = '4px';
						btnClear.onclick = function(e)  // BindArguments(function(txt, hid, tdValue, oReportFilter)
						{
							oReportFilter.Value = null;
							while ( tdValue.childNodes.length > 0 )
								tdValue.removeChild(tdValue.firstChild);
							oReportDesign.PreviewSQL();
						//}, txt, hid, tdValue, oReportFilter);
						};
						divPopupValue.appendChild(btnClear);
						
						var btnCancel = document.createElement('input');
						btnCancel.type      = 'button';
						btnCancel.className = 'button';
						btnCancel.title     = L10n.Term('.LBL_CANCEL_BUTTON_TITLE');
						btnCancel.value     = L10n.Term('.LBL_CANCEL_BUTTON_LABEL');
						btnCancel.style.marginLeft  = '2px';
						btnCancel.style.marginRight = '4px';
						btnCancel.onclick = function(e)
						{
							while ( tdValue.childNodes.length > 0 )
								tdValue.removeChild(tdValue.firstChild);
							if ( oReportFilter.Value != null )
							{
								if ( $.isArray(oReportFilter.Value) )
									tdValue.appendChild(document.createTextNode(oReportFilter.Value.join()));
								else
									tdValue.appendChild(document.createTextNode(oReportFilter.Value));
							}
						};
						divPopupValue.appendChild(btnCancel);
					}
				}
				// 02/24/2015 Paul.  Add support for between filter clause. 
				// 04/09/2020 Paul.  else clause as cannot be Operator is and Operatory between. 
				else if ( oReportFilter.Operator == 'between' || oReportFilter.Operator == 'between_dates' )
				{
					var divListValue = document.createElement('div');
					divListValue.style.whiteSpace = 'nowrap';
					div.appendChild(divListValue);
					
					var nSelectedIndex = tdValue.parentNode.rowIndex - 1;
					var oReportFilter  = oReportDesign.AppliedFilters_GetAt(nSelectedIndex);
					
					var nTextWidth = Math.max($(tdValue).width()/2, 100);
					var txt1 = document.createElement('input');
					txt1.id          = 'tAppliedFilters_TextValue' + nSelectedIndex.toString() + '_1';
					txt1.type        = 'text' ;
					txt1.style.width = (nTextWidth - 4).toString() + 'px';
					divListValue.appendChild(txt1);
					txt1.focus();
					var txt2 = document.createElement('input');
					txt2.id          = 'tAppliedFilters_TextValue' + nSelectedIndex.toString() + '_2';
					txt2.type        = 'text' ;
					txt2.style.width = (nTextWidth - 4).toString() + 'px';
					divListValue.appendChild(txt2);
					
					if ( oReportFilter.Value != null && $.isArray(oReportFilter.Value) )
					{
						if ( oReportFilter.Value.length >= 1 )
							txt1.value = oReportFilter.Value[0];
						if ( oReportFilter.Value.length >= 2 )
							txt2.value = oReportFilter.Value[1];
					}
					var br = document.createElement('br');
					divListValue.appendChild(br);
					
					var btnSave = document.createElement('input');
					btnSave.type      = 'button';
					btnSave.className = 'button';
					btnSave.title     = L10n.Term('.LBL_SAVE_BUTTON_TITLE');
					btnSave.value     = L10n.Term('.LBL_SAVE_BUTTON_LABEL');
					btnSave.style.marginLeft  = '2px';
					btnSave.style.marginRight = '4px';
					btnSave.onclick = function(e)
					{
						var nSelectedIndex = tdValue.parentNode.rowIndex - 1;
						var txt1 = document.getElementById('tAppliedFilters_TextValue' + nSelectedIndex.toString() + '_1');
						var txt2 = document.getElementById('tAppliedFilters_TextValue' + nSelectedIndex.toString() + '_2');
						// 02/24/2015 Paul.  Store the between items as an array. 
						var arr = new Array();
						arr.push(txt1.value);
						arr.push(txt2.value);
						var oReportFilter = oReportDesign.AppliedFilters_GetAt(nSelectedIndex);
						oReportFilter.Value = arr;
						
						while ( tdValue.childNodes.length > 0 )
							tdValue.removeChild(tdValue.firstChild);
						
						if ( oReportFilter.Value != null && $.isArray(oReportFilter.Value) )
							tdValue.appendChild(document.createTextNode(oReportFilter.Value.join()));
						oReportDesign.PreviewSQL();
					};
					divListValue.appendChild(btnSave);
					
					var btnCancel = document.createElement('input');
					btnCancel.type      = 'button';
					btnCancel.className = 'button';
					btnCancel.title     = L10n.Term('.LBL_CANCEL_BUTTON_TITLE');
					btnCancel.value     = L10n.Term('.LBL_CANCEL_BUTTON_LABEL');
					btnCancel.style.marginLeft  = '2px';
					btnCancel.style.marginRight = '4px';
					btnCancel.onclick = function(e)
					{
						while ( tdValue.childNodes.length > 0 )
							tdValue.removeChild(tdValue.firstChild);
						if ( oReportFilter.Value != null && $.isArray(oReportFilter.Value) )
							tdValue.appendChild(document.createTextNode(oReportFilter.Value.join()));
					};
					divListValue.appendChild(btnCancel);
				}
				else if ( bTextBox )
				{
					var divTextValue = document.createElement('div');
					div.appendChild(divTextValue);
					var txt = document.createElement('input');
					txt.id            = 'tAppliedFilters_Value' + nSelectedIndex.toString();
					txt.type          = 'text' ;
					txt.style.width = ($(tdValue).width() - 4).toString() + 'px';
					divTextValue.appendChild(txt);
					txt.focus();
					if ( oReportFilter.Value != null )
						txt.value = oReportFilter.Value;
					txt.onblur = function(e)
					{
						oReportFilter.Value = txt.value;
						if ( tdValue.childNodes.length > 0 && tdValue.childNodes[0].tagName == 'DIV' )
						{
							while ( tdValue.childNodes.length > 0 )
								tdValue.removeChild(tdValue.firstChild);
							
							if ( oReportFilter.Value != null )
							{
								tdValue.appendChild(document.createTextNode(oReportFilter.Value));
							}
						}
						oReportDesign.PreviewSQL();
					};
				}
				else if ( bListBox )
				{
					var divListValue = document.createElement('div');
					divListValue.style.whiteSpace = 'nowrap';
					div.appendChild(divListValue);
					var lst = document.createElement('select');
					lst.id       = 'tAppliedFilters_ListValue' + nSelectedIndex.toString();
					lst.multiple = 'multiple';
					lst.size     = 6;
					
					var nSelectedIndex = tdValue.parentNode.rowIndex - 1;
					var oReportFilter  = oReportDesign.AppliedFilters_GetAt(nSelectedIndex);
					var sListName = null;
					if ( oReportFilter.Field != null )
					{
						var lay = ReportDesign_EditView_Layout(oReportFilter.Field.Module.ModuleName, oReportFilter.Field.ColumnName);
						if ( lay != null )
							sListName = lay.LIST_NAME;
					}
					if ( sListName != null )
					{
						var arrListItems = L10n.GetList(sListName);
						for ( var i = 0; i < arrListItems.length; i++ )
						{
							var opt = document.createElement('option');
							opt.value     = arrListItems[i];
							opt.innerHTML = L10n.ListTerm(sListName, arrListItems[i]);
							lst.appendChild(opt);
							if ( oReportFilter.Value != null && $.isArray(oReportFilter.Value) && ($.inArray(arrListItems[i], oReportFilter.Value) >= 0) )
							{
								opt.selected = true;
							}
						}
						divListValue.appendChild(lst);
					}
					else
					{
						var txt = document.createElement('input');
						txt.id          = 'tAppliedFilters_TextValue' + nSelectedIndex.toString();
						txt.type        = 'text' ;
						txt.style.width = ($(tdValue).width() - 34).toString() + 'px';
						divListValue.appendChild(txt);
						txt.focus();
						
						var btnAdd = document.createElement('input');
						btnAdd.type      = 'button';
						btnAdd.className = 'button';
						btnAdd.value     = '+';
						divListValue.appendChild(btnAdd);
						btnAdd.onclick = function(e)
						{
							var txt = document.getElementById('tAppliedFilters_TextValue' + nSelectedIndex.toString());
							var lst = document.getElementById('tAppliedFilters_ListValue' + nSelectedIndex.toString());
							if ( txt != null && lst != null )
							{
								if ( !Sql.IsEmptyString(txt.value) )
								{
									var opt = document.createElement('option');
									opt.value     = txt.value;
									opt.innerHTML = txt.value;
									opt.selected  = true;
									lst.appendChild(opt);
									txt.value     = '';
								}
							}
						};
						
						if ( oReportFilter.Value != null && $.isArray(oReportFilter.Value) )
						{
							for ( var nValueIndex in oReportFilter.Value )
							{
								var opt = document.createElement('option');
								opt.value     = oReportFilter.Value[nValueIndex];
								opt.innerHTML = oReportFilter.Value[nValueIndex];
								opt.selected  = true;
								lst.appendChild(opt);
							}
						}
						var br = document.createElement('br');
						divListValue.appendChild(br);
						// 07/28/2014 Paul.  Add text field add ability to add items. 
						divListValue.appendChild(lst);
					}
					lst.focus();
					
					var btnSave = document.createElement('input');
					btnSave.type      = 'button';
					btnSave.className = 'button';
					btnSave.title     = L10n.Term('.LBL_SAVE_BUTTON_TITLE');
					btnSave.value     = L10n.Term('.LBL_SAVE_BUTTON_LABEL');
					btnSave.style.marginLeft  = '2px';
					btnSave.style.marginRight = '4px';
					btnSave.onclick = function(e)
					{
						var arr = new Array();
						var nSelectedIndex = tdValue.parentNode.rowIndex - 1;
						for ( var j = 0; j < lst.options.length; j++ )
						{
							if ( lst.options[j].selected )
							{
								arr.push(lst.options[j].value);
							}
						}
						var oReportFilter = oReportDesign.AppliedFilters_GetAt(nSelectedIndex);
						oReportFilter.Value = arr;
						
						while ( tdValue.childNodes.length > 0 )
							tdValue.removeChild(tdValue.firstChild);
						
						if ( oReportFilter.Value != null && $.isArray(oReportFilter.Value) )
							tdValue.appendChild(document.createTextNode(oReportFilter.Value.join()));
						oReportDesign.PreviewSQL();
					};
					divListValue.appendChild(btnSave);
					
					var btnCancel = document.createElement('input');
					btnCancel.type      = 'button';
					btnCancel.className = 'button';
					btnCancel.title     = L10n.Term('.LBL_CANCEL_BUTTON_TITLE');
					btnCancel.value     = L10n.Term('.LBL_CANCEL_BUTTON_LABEL');
					btnCancel.style.marginLeft  = '2px';
					btnCancel.style.marginRight = '4px';
					btnCancel.onclick = function(e)
					{
						while ( tdValue.childNodes.length > 0 )
							tdValue.removeChild(tdValue.firstChild);
						if ( oReportFilter.Value != null && $.isArray(oReportFilter.Value) )
							tdValue.appendChild(document.createTextNode(oReportFilter.Value.join()));
					};
					divListValue.appendChild(btnCancel);
				}
			}
		};
	}
}

function ReportDesign_EditView_Layout(sModuleName, sColumnName)
{
	sColumnName = sColumnName.toUpperCase();
	
	var sEDIT_NAME = sModuleName + '.EditView';
	var bgPage  = chrome.extension.getBackgroundPage();
	var layout  = bgPage.SplendidCache.EditViewFields(sEDIT_NAME);
	for ( var nLayoutIndex in layout )
	{
		var lay = layout[nLayoutIndex];
		var sFIELD_TYPE = lay.FIELD_TYPE;
		var sDATA_FIELD = lay.DATA_FIELD;
		if ( sDATA_FIELD != null && sDATA_FIELD.toUpperCase() == sColumnName )
		{
			return lay;
		}
	}
	return null;
}

function ReportDesign_EditDialog(jsonReportDesign, bWorkflowMode, cbDialogSave)
{
	bReportDesignerWorkflowMode = bWorkflowMode;
	try
	{
		var sDialogHtml = '';
		sDialogHtml += '<div id="divQueryDesigner">';
		sDialogHtml += '	<div class="button-panel">';
		sDialogHtml += '		<button id="btnReportDesignSave"   class="button" style="margin-right: 3px;">' + L10n.Term('.LBL_SAVE_BUTTON_LABEL'  ) + '</button>';
		sDialogHtml += '		<button id="btnReportDesignCancel" class="button" style="margin-right: 3px;">' + L10n.Term('.LBL_CANCEL_BUTTON_LABEL') + '</button>';
		sDialogHtml += '		<div id="lblError" class="error"></div>';
		sDialogHtml += '	</div>';
		sDialogHtml += '	<table cellspacing="0" cellpadding="0" width="100%" style="border-left: 1px solid #cccccc; border-right: 1px solid #cccccc; border-bottom: 1px solid #cccccc;">';
		sDialogHtml += '		<tbody>';
		sDialogHtml += '			<tr>';
		sDialogHtml += '				<td valign="top" rowspan="3" width="300px" style="border: 1px solid #cccccc;">';
		sDialogHtml += '					<table cellspacing="0" cellpadding="4" width="100%">';
		sDialogHtml += '						<tbody>';
		sDialogHtml += '							<tr class="listViewThS1">';
		sDialogHtml += '								<td>' + L10n.Term("ReportDesigner.LBL_MODULES") + '</td>';
		sDialogHtml += '							</tr>';
		sDialogHtml += '						</tbody>';
		sDialogHtml += '					</table>';
		sDialogHtml += '					<div style="height: ' + (bReportDesignerWorkflowMode ? '440px' : '640px') + '; overflow-y: auto; width: 300px; overflow-x: auto;">';
		sDialogHtml += '						<ul id="treeModules" class="ztree"></ul>';
		sDialogHtml += '					</div>';
		sDialogHtml += '				</td>';
		sDialogHtml += '				<td valign="top" style="height: 240px; border: 1px solid #cccccc;' + (bReportDesignerWorkflowMode ? ' display: none;' : '') + '">';
		sDialogHtml += '					<table cellspacing="0" cellpadding="4" width="100%">';
		sDialogHtml += '						<tbody>';
		sDialogHtml += '							<tr class="listViewThS1">';
		sDialogHtml += '								<td width="20%">' + L10n.Term("ReportDesigner.LBL_SELECTED_FIELDS") + '</td>';
		sDialogHtml += '								<td width="80%" align="right">';
		sDialogHtml += '									<input id="chkGroupAndAggregate" type="checkbox" onclick="return chkGroupAndAggregate_Clicked(this);" class="checkbox" />&nbsp;<label for="chkGroupAndAggregate">' + L10n.Term("ReportDesigner.LBL_GROUP_AND_AGGREGATE") + '</label>&nbsp;';
		sDialogHtml += '									<a href="#" id="aSelectedFields_SelectedDelete"  ><img border="0" src="'+ sIMAGE_SERVER + 'ReportDesignerItemDelete.gif"   width="18" height="18" /></a>';
		sDialogHtml += '									<a href="#" id="aSelectedFields_SelectedMoveUp"  ><img border="0" src="'+ sIMAGE_SERVER + 'ReportDesignerItemMoveUp.gif"   width="18" height="18" /></a>';
		sDialogHtml += '									<a href="#" id="aSelectedFields_SelectedMoveDown"><img border="0" src="'+ sIMAGE_SERVER + 'ReportDesignerItemMoveDown.gif" width="18" height="18" /></a>';
		sDialogHtml += '								</td>';
		sDialogHtml += '							</tr>';
		sDialogHtml += '						</tbody>';
		sDialogHtml += '					</table>';
		sDialogHtml += '					<div style="height: 240px; overflow-y: auto;">';
		sDialogHtml += '						<table cellspacing="0" cellpadding="4" width="100%">';
		sDialogHtml += '							<tbody id="tSelectedFields">';
		sDialogHtml += '								<tr id="" class="listViewThS1">';
		sDialogHtml += '									<td width="25%">' + L10n.Term("ReportDesigner.LBL_FIELD"         ) + '</td>';
		sDialogHtml += '									<td width="30%">' + L10n.Term("ReportDesigner.LBL_DISPLAY_NAME"  ) + '</td>';
		sDialogHtml += '									<td width="15%">' + L10n.Term("ReportDesigner.LBL_DISPLAY_WIDTH" ) + '</td>';
		sDialogHtml += '									<td width="15%">' + L10n.Term("ReportDesigner.LBL_AGGREGATE"     ) + '</td>';
		sDialogHtml += '									<td width="15%">' + L10n.Term("ReportDesigner.LBL_SORT_DIRECTION") + '</td>';
		sDialogHtml += '								</tr>';
		sDialogHtml += '							</tbody>';
		sDialogHtml += '						</table>';
		sDialogHtml += '					</div>';
		sDialogHtml += '				</td>';
		sDialogHtml += '			</tr>';
		sDialogHtml += '			<tr>';
		sDialogHtml += '				<td valign="top" style="height: 200px; border: 1px solid #cccccc;">';
		sDialogHtml += '					<table cellspacing="0" cellpadding="4" width="100%">';
		sDialogHtml += '						<tbody>';
		sDialogHtml += '							<tr class="listViewThS1">';
		sDialogHtml += '								<td width="20%">' + L10n.Term("ReportDesigner.LBL_RELATIONSHIPS") + '</td>';
		sDialogHtml += '								<td width="80%" align="right">';
		sDialogHtml += '									<a href="#" id="aRelationships_AddRelationship" ><img border="0" src="'+ sIMAGE_SERVER + 'ReportDesignerRelationshipCreate.gif" width="18" height="18" /></a>';
		sDialogHtml += '									<a href="#" id="aRelationships_SelectedDelete"  ><img border="0" src="'+ sIMAGE_SERVER + 'ReportDesignerItemDelete.gif"         width="18" height="18" /></a>';
		sDialogHtml += '									<a href="#" id="aRelationships_SelectedMoveUp"  ><img border="0" src="'+ sIMAGE_SERVER + 'ReportDesignerItemMoveUp.gif"         width="18" height="18" /></a>';
		sDialogHtml += '									<a href="#" id="aRelationships_SelectedMoveDown"><img border="0" src="'+ sIMAGE_SERVER + 'ReportDesignerItemMoveDown.gif"       width="18" height="18" /></a>';
		sDialogHtml += '								</td>';
		sDialogHtml += '							</tr>';
		sDialogHtml += '						</tbody>';
		sDialogHtml += '					</table>';
		sDialogHtml += '					<table cellspacing="0" cellpadding="4" width="100%">';
		sDialogHtml += '						<tbody id="tRelationships">';
		sDialogHtml += '							<tr id="" class="listViewThS1">';
		sDialogHtml += '								<td width="30%">' + L10n.Term("ReportDesigner.LBL_LEFT_TABLE" ) + '</td>';
		sDialogHtml += '								<td width="10%">' + L10n.Term("ReportDesigner.LBL_JOIN_TYPE"  ) + '</td>';
		sDialogHtml += '								<td width="30%">' + L10n.Term("ReportDesigner.LBL_RIGHT_TABLE") + '</td>';
		sDialogHtml += '								<td width="30%">' + L10n.Term("ReportDesigner.LBL_JOIN_FIELDS") + '</td>';
		sDialogHtml += '							</tr>';
		sDialogHtml += '						</tbody>';
		sDialogHtml += '					</table>';
		sDialogHtml += '				</td>';
		sDialogHtml += '			</tr>';
		sDialogHtml += '			<tr>';
		sDialogHtml += '				<td valign="top" style="height: 200px; border: 1px solid #cccccc;">';
		sDialogHtml += '					<table cellspacing="0" cellpadding="4" width="100%">';
		sDialogHtml += '						<tbody>';
		sDialogHtml += '							<tr class="listViewThS1">';
		sDialogHtml += '								<td width="20%">' + L10n.Term("ReportDesigner.LBL_APPLIED_FILTERS") + '</td>';
		sDialogHtml += '								<td width="80%" align="right">';
		sDialogHtml += '									<a href="#" id="aAppliedFilters_AddFilter"       ><img border="0" src="'+ sIMAGE_SERVER + 'ReportDesignerFilterCreate.gif" width="18" height="18" /></a>';
		sDialogHtml += '									<a href="#" id="aAppliedFilters_SelectedDelete"  ><img border="0" src="'+ sIMAGE_SERVER + 'ReportDesignerItemDelete.gif"   width="18" height="18" /></a>';
		sDialogHtml += '									<a href="#" id="aAppliedFilters_SelectedMoveUp"  ><img border="0" src="'+ sIMAGE_SERVER + 'ReportDesignerItemMoveUp.gif"   width="18" height="18" /></a>';
		sDialogHtml += '									<a href="#" id="aAppliedFilters_SelectedMoveDown"><img border="0" src="'+ sIMAGE_SERVER + 'ReportDesignerItemMoveDown.gif" width="18" height="18" /></a>';
		sDialogHtml += '								</td>';
		sDialogHtml += '							</tr>';
		sDialogHtml += '						</tbody>';
		sDialogHtml += '					</table>';
		sDialogHtml += '					<table cellspacing="0" cellpadding="4" width="100%">';
		sDialogHtml += '						<tbody id="tAppliedFilters">';
		sDialogHtml += '							<tr id="" class="listViewThS1">';
		sDialogHtml += '								<td width="40%">' + L10n.Term("ReportDesigner.LBL_FIELD_NAME") + '</td>';
		sDialogHtml += '								<td width="10%">' + L10n.Term("ReportDesigner.LBL_OPERATOR"  ) + '</td>';
		sDialogHtml += '								<td width="40%">' + L10n.Term("ReportDesigner.LBL_VALUE"     ) + '</td>';
		sDialogHtml += '								<td width="10%">' + L10n.Term("ReportDesigner.LBL_PARAMETER" ) + '</td>';
		sDialogHtml += '							</tr>';
		sDialogHtml += '						</tbody>';
		sDialogHtml += '					</table>';
		sDialogHtml += '				</td>';
		sDialogHtml += '			</tr>';
		sDialogHtml += '		</tbody>';
		sDialogHtml += '	</table>';
		sDialogHtml += '';
		sDialogHtml += '	<div id="divReportDesignerSQLError" class="error"></div>';
		sDialogHtml += '	<br />';
		sDialogHtml += '	<div id="divSHOW_QUERY" style="display: inline">';
		sDialogHtml += '		<table id="tblReportDesignerSQL" border="1" cellpadding="3" cellspacing="0" width="100%" bgcolor="LightGrey">';
		sDialogHtml += '			<tr>';
		sDialogHtml += '				<td>';
		sDialogHtml += '					<pre id="divReportDesignerSQL"></pre>';
		sDialogHtml += '				</td>';
		sDialogHtml += '			</tr>';
		sDialogHtml += '		</table>';
		sDialogHtml += '		<table id="tblReportDesignerJSON" border="1" cellpadding="3" cellspacing="0" width="100%" bgcolor="LightGrey" style="display: inline">';
		sDialogHtml += '			<tr>';
		sDialogHtml += '				<td>';
		sDialogHtml += '					<div id="divReportDesignerJSON"></div>';
		sDialogHtml += '				</td>';
		sDialogHtml += '			</tr>';
		sDialogHtml += '		</table>';
		sDialogHtml += '	</div>';
		sDialogHtml += '</div>';

		var nWidth  = Math.floor(85 * $(window).width () / 100);
		var nHeight = Math.floor(85 * $(window).height() / 100);
		if ( nWidth < 1200 )
			nWidth = 1200;
		if ( nHeight < 700 )
			nHeight = 700;
		if ( nHeight > 900 )
			nHeight = 900;
		if ( bReportDesignerWorkflowMode )
			nHeight -= 200;

		var $dialog = $(sDialogHtml);
		$dialog.dialog(
		{
				modal    : true
			, resizable: true
			, width    : nWidth
			, height   : nHeight
			//, position : { my: 'left top', at: 'left top+18', of: '#tRelationships' }
			, title    : L10n.Term('.moduleList.ReportDesigner')
			, create   : function(event, ui)
			{
				try
				{
					SplendidError.SystemMessage('Building module node tree.');
					var zNodes = zTree_BuildModuleNodes(arrReportDesignerModules);
					var setting =
					{
						callback:
						{
							onCheck: zTree_onCheck
						}
						, check:
						{
							enable: true
						}
						, data:
						{
							simpleData:
							{
								enable: false
							}
						}
					};
					$.fn.zTree.init($('#treeModules'), setting, zNodes);
					SplendidError.SystemMessage('');
					
					try
					{
						var aSelectedFields_SelectedDelete   = document.getElementById('aSelectedFields_SelectedDelete'  );
						var aSelectedFields_SelectedMoveUp   = document.getElementById('aSelectedFields_SelectedMoveUp'  );
						var aSelectedFields_SelectedMoveDown = document.getElementById('aSelectedFields_SelectedMoveDown');
						var aRelationships_AddRelationship   = document.getElementById('aRelationships_AddRelationship'  );
						var aRelationships_SelectedDelete    = document.getElementById('aRelationships_SelectedDelete'   );
						var aRelationships_SelectedMoveUp    = document.getElementById('aRelationships_SelectedMoveUp'   );
						var aRelationships_SelectedMoveDown  = document.getElementById('aRelationships_SelectedMoveDown' );
						var aAppliedFilters_AddFilter        = document.getElementById('aAppliedFilters_AddFilter'       );
						var aAppliedFilters_SelectedDelete   = document.getElementById('aAppliedFilters_SelectedDelete'  );
						var aAppliedFilters_SelectedMoveUp   = document.getElementById('aAppliedFilters_SelectedMoveUp'  );
						var aAppliedFilters_SelectedMoveDown = document.getElementById('aAppliedFilters_SelectedMoveDown');
						
						aSelectedFields_SelectedDelete  .onclick = function() { return tSelectedFields_SelectedDelete  (); };
						aSelectedFields_SelectedMoveUp  .onclick = function() { return tSelectedFields_SelectedMoveUp  (); };
						aSelectedFields_SelectedMoveDown.onclick = function() { return tSelectedFields_SelectedMoveDown(); };
						aRelationships_AddRelationship  .onclick = function() { return tRelationships_AddRelationship  (); };
						aRelationships_SelectedDelete   .onclick = function() { return tRelationships_SelectedDelete   (); };
						aRelationships_SelectedMoveUp   .onclick = function() { return tRelationships_SelectedMoveUp   (); };
						aRelationships_SelectedMoveDown .onclick = function() { return tRelationships_SelectedMoveDown (); };
						aAppliedFilters_AddFilter       .onclick = function() { return tAppliedFilters_AddFilter       (); };
						aAppliedFilters_SelectedDelete  .onclick = function() { return tAppliedFilters_SelectedDelete  (); };
						aAppliedFilters_SelectedMoveUp  .onclick = function() { return tAppliedFilters_SelectedMoveUp  (); };
						aAppliedFilters_SelectedMoveDown.onclick = function() { return tAppliedFilters_SelectedMoveDown(); };
						
						var btnReportDesignSave   = document.getElementById('btnReportDesignSave'  );
						var btnReportDesignCancel = document.getElementById('btnReportDesignCancel');
						btnReportDesignSave.onclick = function()
						{
							var divReportDesignerSQL = document.getElementById('divReportDesignerSQL');
							var oDesign = new Object();
							oDesign.json = oReportDesign.Stringify();
							oDesign.sql  = divReportDesignerSQL.innerHTML;
							cbDialogSave(1, oDesign);
							
							$dialog.dialog('destroy');
							var divQueryDesigner = document.getElementById('divQueryDesigner');
							divQueryDesigner.parentNode.removeChild(divQueryDesigner);
						};
						btnReportDesignCancel.onclick = function()
						{
							$dialog.dialog('destroy');
							var divQueryDesigner = document.getElementById('divQueryDesigner');
							divQueryDesigner.parentNode.removeChild(divQueryDesigner);
						};
					}
					catch(e)
					{
						SplendidError.SystemMessage('ReportDesigner buttons: ' + e.message);
					}
					
					if ( !Sql.IsEmptyString(jsonReportDesign) )
					{
						if ( bDebug )
						{
							var divReportDesignerJSON = document.getElementById('divReportDesignerJSON');
							if ( divReportDesignerJSON != null )
								divReportDesignerJSON.innerHTML = dumpObj(jQuery.parseJSON(jsonReportDesign), '').replace(/\n/g, '<br>\n').replace(/\t/g, '&nbsp;&nbsp;&nbsp;');
						}
						
						oReportDesign = new ReportDesign();
						oReportDesign.Parse(jsonReportDesign);
						oReportDesign.PreviewSQL();
						
						var treeObj = $.fn.zTree.getZTreeObj("treeModules");
						for ( var i = 0; i < oReportDesign.SelectedFields.length; i++ )
						{
							var field = oReportDesign.SelectedFields[i];
							var sFieldName = field.FieldName;
							var node = treeObj.getNodeByParam('FieldName', sFieldName);
							if ( node != null )
								treeObj.checkNode(node, true, true, false);
						}
						// 07/15/2016 Paul.  We need to check the tables when in workflow mode. 
						for ( var i = 0; i < oReportDesign.Tables.length; i++ )
						{
							var table = oReportDesign.Tables[i];
							var sTableName = table.TableName;
							var node = treeObj.getNodeByParam('id', sTableName);
							if ( node != null )
								treeObj.checkNode(node, true, true, false);
						}
					}
				}
				catch(e)
				{
					SplendidError.SystemMessage('ReportDesigner Modules: ' + e.message);
				}
			}
			, close: function(event, ui)
			{
				$dialog.dialog('destroy');
				var divQueryDesigner = document.getElementById('divQueryDesigner');
				divQueryDesigner.parentNode.removeChild(divQueryDesigner);
			}
		});
	}
	catch(e)
	{
		SplendidError.SystemMessage('ReportDesigner EditDialog: ' + e.message);
		cbDialogSave(-1, e.message);
	}
}

