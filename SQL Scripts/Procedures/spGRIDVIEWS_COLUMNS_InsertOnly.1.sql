if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spGRIDVIEWS_COLUMNS_InsertOnly' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spGRIDVIEWS_COLUMNS_InsertOnly;
GO
 
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
 *********************************************************************************************************************/
Create Procedure dbo.spGRIDVIEWS_COLUMNS_InsertOnly
	( @GRID_NAME                   nvarchar( 50)
	, @COLUMN_INDEX                int
	, @COLUMN_TYPE                 nvarchar( 25)
	, @DEFAULT_VIEW                bit
	, @HEADER_TEXT                 nvarchar(150)
	, @SORT_EXPRESSION             nvarchar( 50)
	, @ITEMSTYLE_WIDTH             nvarchar( 10)
	, @ITEMSTYLE_CSSCLASS          nvarchar( 50)
	, @ITEMSTYLE_HORIZONTAL_ALIGN  nvarchar( 10)
	, @ITEMSTYLE_VERTICAL_ALIGN    nvarchar( 10)
	, @ITEMSTYLE_WRAP              bit
	, @DATA_FIELD                  nvarchar( 50)
	, @DATA_FORMAT                 nvarchar( 25)
	, @URL_FIELD                   nvarchar(max)
	, @URL_FORMAT                  nvarchar(max)
	, @URL_TARGET                  nvarchar( 60)
	, @LIST_NAME                   nvarchar( 50)
	, @URL_MODULE                  nvarchar( 25)
	, @URL_ASSIGNED_FIELD          nvarchar( 30)
	, @MODULE_TYPE                 nvarchar( 25)
	, @PARENT_FIELD                nvarchar( 30)
	)
as
  begin

	declare @ID uniqueidentifier;
	
	declare @TEMP_COLUMN_INDEX int;	
	set @TEMP_COLUMN_INDEX = @COLUMN_INDEX;
	if @COLUMN_INDEX is null or @COLUMN_INDEX = -1 begin -- then
		-- BEGIN Oracle Exception
			select @TEMP_COLUMN_INDEX = isnull(max(COLUMN_INDEX), 0) + 1
			  from GRIDVIEWS_COLUMNS
			 where GRID_NAME    = @GRID_NAME
			   and DELETED      = 0            
			   and DEFAULT_VIEW = 0            ;
		-- END Oracle Exception
		-- BEGIN Oracle Exception
			select @ID = ID
			  from GRIDVIEWS_COLUMNS
			 where GRID_NAME    = @GRID_NAME
			   and DATA_FIELD   = @DATA_FIELD
			   and DELETED      = 0            
			   and DEFAULT_VIEW = 0            ;
		-- END Oracle Exception
	end else begin
		-- BEGIN Oracle Exception
			select @ID = ID
			  from GRIDVIEWS_COLUMNS
			 where GRID_NAME    = @GRID_NAME
			   and COLUMN_INDEX = @COLUMN_INDEX
			   and DELETED      = 0            
			   and DEFAULT_VIEW = 0            ;
		-- END Oracle Exception
	end -- if;
	if not exists(select * from GRIDVIEWS_COLUMNS where ID = @ID) begin -- then
		set @ID = newid();
		insert into GRIDVIEWS_COLUMNS
			( ID                        
			, CREATED_BY                
			, DATE_ENTERED              
			, MODIFIED_USER_ID          
			, DATE_MODIFIED             
			, GRID_NAME                 
			, COLUMN_INDEX              
			, COLUMN_TYPE               
			, DEFAULT_VIEW              
			, HEADER_TEXT               
			, SORT_EXPRESSION           
			, ITEMSTYLE_WIDTH           
			, ITEMSTYLE_CSSCLASS        
			, ITEMSTYLE_HORIZONTAL_ALIGN
			, ITEMSTYLE_VERTICAL_ALIGN  
			, ITEMSTYLE_WRAP            
			, DATA_FIELD                
			, DATA_FORMAT               
			, URL_FIELD                 
			, URL_FORMAT                
			, URL_TARGET                
			, LIST_NAME                 
			, URL_MODULE                
			, URL_ASSIGNED_FIELD        
			, MODULE_TYPE               
			, PARENT_FIELD              
			)
		values 
			( @ID                        
			, null                       
			,  getdate()                 
			, null                       
			,  getdate()                 
			, @GRID_NAME                 
			, @TEMP_COLUMN_INDEX         
			, @COLUMN_TYPE               
			, @DEFAULT_VIEW              
			, @HEADER_TEXT               
			, @SORT_EXPRESSION           
			, @ITEMSTYLE_WIDTH           
			, @ITEMSTYLE_CSSCLASS        
			, @ITEMSTYLE_HORIZONTAL_ALIGN
			, @ITEMSTYLE_VERTICAL_ALIGN  
			, @ITEMSTYLE_WRAP            
			, @DATA_FIELD                
			, @DATA_FORMAT               
			, @URL_FIELD                 
			, @URL_FORMAT                
			, @URL_TARGET                
			, @LIST_NAME                 
			, @URL_MODULE                
			, @URL_ASSIGNED_FIELD        
			, @MODULE_TYPE               
			, @PARENT_FIELD              
			);
	end -- if;
  end
GO

Grant Execute on dbo.spGRIDVIEWS_COLUMNS_InsertOnly to public;
GO

