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
using System.Data;
using System.Data.Common;
using System.Reflection;
//using iAnywhere.Data.AsaClient;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for SQLAnywhereDataAdapter.
	/// 04/21/2006 Paul.  SQL Anywhere requires a boxed data adapter that inherits DbDataAdapter.
	/// </summary>
	public class SQLAnywhereDataAdapter : DbDataAdapter, IDbDataAdapter
	{
		protected Assembly       m_asmSqlClient      ;
		protected System.Type    m_typSqlDataAdapter ;
		protected IDbDataAdapter m_dbDataAdapter     ;
		private const string m_sAssemblyName    = "iAnywhere.Data.AsaClient";
		private const string m_sDataAdapterName = "iAnywhere.Data.AsaClient.AsaDataAdapter";

		public SQLAnywhereDataAdapter()
		{
			#pragma warning disable 618
			m_asmSqlClient      = Assembly.LoadWithPartialName(m_sAssemblyName);
			#pragma warning restore 618

			if ( m_asmSqlClient == null )
				throw(new Exception("Could not load " + m_sAssemblyName));
			m_typSqlDataAdapter = m_asmSqlClient.GetType(m_sDataAdapterName);
			
			ConstructorInfo info = m_typSqlDataAdapter.GetConstructor(new Type[0]); 
			m_dbDataAdapter = info.Invoke(null) as IDbDataAdapter; 
			if ( m_dbDataAdapter == null )
				throw(new Exception("Failed to invoke database adapter constructor."));
		}

		/*
		// 04/21/2006 Paul.  There does not need to be a dispose 
		// as neither m_asmSqlClient, nor m_typSqlDataAdapter have a dispose method. 
		private bool disposed = false;

		~SQLAnywhereDataAdapter()
		{
			Dispose(false);
		}
		
		public new void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected override void Dispose(bool disposing)
		{
			if ( !disposed )
			{
				if ( disposing )
				{
					m_asmSqlClient      = null;
					m_typSqlDataAdapter = null;
				}
			}
			base.Dispose(disposing);
			disposed = true;
		}
		*/


		#region DbDataAdapter Abstract Members
		protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow , IDbCommand command , StatementType statementType , DataTableMapping tableMapping)
		{
			// 04/24/2006 Paul.  I don't like seeing the unreachable code warning. 
			//throw new NotImplementedException();
			return null;
		}
		
		protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow , IDbCommand command , StatementType statementType , DataTableMapping tableMapping)
		{
			// 04/24/2006 Paul.  I don't like seeing the unreachable code warning. 
			//throw new NotImplementedException();
			return null;
		}

		protected override void OnRowUpdated(RowUpdatedEventArgs value)
		{
			throw new NotImplementedException();
		}

		protected override void OnRowUpdating(RowUpdatingEventArgs value)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region IDbDataAdapter Members
		// 05/18/2006 Paul.  .NET 2.0 defines IDbDataAdapter overrides differently. 
		IDbCommand IDbDataAdapter.UpdateCommand
		{
			get { return m_dbDataAdapter.UpdateCommand; }
			set { m_dbDataAdapter.UpdateCommand = value; }
		}

		IDbCommand IDbDataAdapter.SelectCommand
		{
			get { return m_dbDataAdapter.SelectCommand; }
			set { m_dbDataAdapter.SelectCommand = value; }
		}

		IDbCommand IDbDataAdapter.DeleteCommand
		{
			get { return m_dbDataAdapter.DeleteCommand; }
			set { m_dbDataAdapter.DeleteCommand = value; }
		}

		IDbCommand IDbDataAdapter.InsertCommand
		{
			get { return m_dbDataAdapter.InsertCommand; }
			set { m_dbDataAdapter.InsertCommand = value; }
		}
		#endregion
	}
}
