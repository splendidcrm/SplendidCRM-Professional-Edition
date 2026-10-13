if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spEDITVIEWS_FIELDS_InsertOnly' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spEDITVIEWS_FIELDS_InsertOnly;
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
Create Procedure dbo.spEDITVIEWS_FIELDS_InsertOnly
	( @EDIT_NAME                          nvarchar( 50)
	, @FIELD_INDEX                        int
	, @FIELD_TYPE                         nvarchar( 25)
	, @DEFAULT_VIEW                       bit
	, @DATA_LABEL                         nvarchar(150)
	, @DATA_FIELD                         nvarchar(100)
	, @DATA_FORMAT                        nvarchar(100)
	, @DISPLAY_FIELD                      nvarchar(100)
	, @CACHE_NAME                         nvarchar( 50)
	, @DATA_REQUIRED                      bit
	, @UI_REQUIRED                        bit
	, @ONCLICK_SCRIPT                     nvarchar(max)
	, @FORMAT_SCRIPT                      nvarchar(255)
	, @FORMAT_TAB_INDEX                   int
	, @FORMAT_MAX_LENGTH                  int
	, @FORMAT_SIZE                        int
	, @FORMAT_ROWS                        int
	, @FORMAT_COLUMNS                     int
	, @COLSPAN                            int
	, @ROWSPAN                            int
	, @FIELD_VALIDATOR_ID                 uniqueidentifier
	, @FIELD_VALIDATOR_MESSAGE            nvarchar(150)
	, @MODULE_TYPE                        nvarchar( 25)
	, @TOOL_TIP                           nvarchar(150)
	, @RELATED_SOURCE_MODULE_NAME         nvarchar( 50)
	, @RELATED_SOURCE_VIEW_NAME           nvarchar( 50)
	, @RELATED_SOURCE_ID_FIELD            nvarchar( 30)
	, @RELATED_SOURCE_NAME_FIELD          nvarchar(100)
	, @RELATED_VIEW_NAME                  nvarchar( 50)
	, @RELATED_ID_FIELD                   nvarchar( 30)
	, @RELATED_NAME_FIELD                 nvarchar(100)
	, @RELATED_JOIN_FIELD                 nvarchar( 30)
	, @PARENT_FIELD                       nvarchar( 30)
	)
as
  begin
	set nocount on
	
	declare @ID uniqueidentifier;
	declare @TEMP_FIELD_INDEX int;	
	set @TEMP_FIELD_INDEX = @FIELD_INDEX;
	if @FIELD_INDEX is null or @FIELD_INDEX = -1 begin -- then
		-- BEGIN Oracle Exception
			select @TEMP_FIELD_INDEX = isnull(max(FIELD_INDEX), 0) + 1
			  from EDITVIEWS_FIELDS
			 where EDIT_NAME    = @EDIT_NAME
			   and DELETED      = 0            
			   and DEFAULT_VIEW = 0            ;
		-- END Oracle Exception
		-- BEGIN Oracle Exception
			select @ID = ID
			  from EDITVIEWS_FIELDS
			 where EDIT_NAME    = @EDIT_NAME
			   and DATA_FIELD   = @DATA_FIELD
			   and DELETED      = 0            
			   and DEFAULT_VIEW = 0            ;
		-- END Oracle Exception
	end else begin
		-- BEGIN Oracle Exception
			select @ID = ID
			  from EDITVIEWS_FIELDS
			 where EDIT_NAME    = @EDIT_NAME
			   and FIELD_INDEX  = @FIELD_INDEX
			   and DELETED      = 0            
			   and DEFAULT_VIEW = 0            ;
		-- END Oracle Exception
	end -- if;
	if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
		set @ID = newid();
		insert into EDITVIEWS_FIELDS
			( ID                        
			, CREATED_BY                
			, DATE_ENTERED              
			, MODIFIED_USER_ID          
			, DATE_MODIFIED             
			, EDIT_NAME                 
			, FIELD_INDEX               
			, FIELD_TYPE                
			, DEFAULT_VIEW              
			, DATA_LABEL                
			, DATA_FIELD                
			, DATA_FORMAT               
			, DISPLAY_FIELD             
			, CACHE_NAME                
			, DATA_REQUIRED             
			, UI_REQUIRED               
			, ONCLICK_SCRIPT            
			, FORMAT_SCRIPT             
			, FORMAT_TAB_INDEX          
			, FORMAT_MAX_LENGTH         
			, FORMAT_SIZE               
			, FORMAT_ROWS               
			, FORMAT_COLUMNS            
			, COLSPAN                   
			, ROWSPAN                   
			, FIELD_VALIDATOR_ID        
			, FIELD_VALIDATOR_MESSAGE   
			, MODULE_TYPE               
			, TOOL_TIP                  
			, RELATED_SOURCE_MODULE_NAME
			, RELATED_SOURCE_VIEW_NAME  
			, RELATED_SOURCE_ID_FIELD   
			, RELATED_SOURCE_NAME_FIELD 
			, RELATED_VIEW_NAME         
			, RELATED_ID_FIELD          
			, RELATED_NAME_FIELD        
			, RELATED_JOIN_FIELD        
			, PARENT_FIELD              
			)
		values 
			( @ID                        
			, null                       
			,  getdate()                 
			, null                       
			,  getdate()                 
			, @EDIT_NAME                 
			, @TEMP_FIELD_INDEX          
			, @FIELD_TYPE                
			, @DEFAULT_VIEW              
			, @DATA_LABEL                
			, @DATA_FIELD                
			, @DATA_FORMAT               
			, @DISPLAY_FIELD             
			, @CACHE_NAME                
			, @DATA_REQUIRED             
			, @UI_REQUIRED               
			, @ONCLICK_SCRIPT            
			, @FORMAT_SCRIPT             
			, @FORMAT_TAB_INDEX          
			, @FORMAT_MAX_LENGTH         
			, @FORMAT_SIZE               
			, @FORMAT_ROWS               
			, @FORMAT_COLUMNS            
			, @COLSPAN                   
			, @ROWSPAN                   
			, @FIELD_VALIDATOR_ID        
			, @FIELD_VALIDATOR_MESSAGE   
			, @MODULE_TYPE               
			, @TOOL_TIP                  
			, @RELATED_SOURCE_MODULE_NAME
			, @RELATED_SOURCE_VIEW_NAME  
			, @RELATED_SOURCE_ID_FIELD   
			, @RELATED_SOURCE_NAME_FIELD 
			, @RELATED_VIEW_NAME         
			, @RELATED_ID_FIELD          
			, @RELATED_NAME_FIELD        
			, @RELATED_JOIN_FIELD        
			, @PARENT_FIELD              
			);
	end -- if;
  end
GO
 
Grant Execute on dbo.spEDITVIEWS_FIELDS_InsertOnly to public;
GO
 
