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
using System.Reflection;
using System.Workflow.Activities.Rules;
using System.Collections.Generic;

namespace SplendidCRM
{
	public class RulesParser
	{
		private static ConstructorInfo m_ctorParser               = null;
		private static MethodInfo      m_methParseCondition       = null;
		private static MethodInfo      m_methParseStatementList   = null;
		private static MethodInfo      m_methParseSingleStatement = null;
		private object objParser = null;

		static RulesParser()
		{
			//Assembly asmActivities = Assembly.GetAssembly(typeof(System.Workflow.Activities.Rules.RuleValidation));
			//Type     typParser     = asmActivities.GetType("System.Workflow.Activities.Rules.Parser");
			// 11/29/2010 Paul.  The Parser may not be available in .NET 4.0, so specify the 3.0 version. 
			Type     typParser     = Type.GetType("System.Workflow.Activities.Rules.Parser, System.Workflow.Activities, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			m_ctorParser               = typParser.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(System.Workflow.Activities.Rules.RuleValidation) }, null);
			m_methParseCondition       = typParser.GetMethod("ParseCondition"      , BindingFlags.Instance | BindingFlags.NonPublic);
			m_methParseStatementList   = typParser.GetMethod("ParseStatementList"  , BindingFlags.Instance | BindingFlags.NonPublic);
			m_methParseSingleStatement = typParser.GetMethod("ParseSingleStatement", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		public RulesParser(RuleValidation validation)
		{
			objParser = m_ctorParser.Invoke(new object[] { validation } );
		}

		public RuleExpressionCondition ParseCondition(string expressionString)
		{
			try
			{
				return m_methParseCondition.Invoke(objParser, new object[] { expressionString } ) as RuleExpressionCondition;
			}
			catch(TargetInvocationException ex)
			{
				// 10/22/2010 Paul.   Instead of displaying "Exception has been thrown by the target of an invocation.", 
				// catch the error and return the more useful inner exception. 
				throw ex.InnerException;
			}
		}

		public List<RuleAction> ParseStatementList(string statementString)
		{
			try
			{
				return m_methParseStatementList.Invoke(objParser, new object[] { statementString } ) as List<RuleAction>;
			}
			catch(TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}

		public RuleAction ParseSingleStatement(string statementString)
		{
			try
			{
				return m_methParseSingleStatement.Invoke(objParser, new object[] { statementString } ) as RuleAction;
			}
			catch(TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}
	}
}
