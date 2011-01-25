using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.StyleCop;
using Microsoft.StyleCop.CSharp;
using Attribute = Microsoft.StyleCop.CSharp.Attribute;
using Delegate = Microsoft.StyleCop.CSharp.Delegate;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Helper methods for analyzing code.
	/// </summary>
	public static class CodeHelper
	{
		#region Identifying elements

		/// <summary>
		/// Checks whether specified element describes private event handler.
		/// </summary>
		public static bool IsPrivateEventHandler(CsElement element)
		{
			if (element.ElementType != ElementType.Method)
				return false;

			if (!IsPrivate(element))
				return false;

			Method method = (Method)element;
			if (method.Parameters.Count == 2)
			{
				Parameter sender = method.Parameters[0];
				Parameter args = method.Parameters[1];
				if (sender.Name == "sender"
					&& sender.Type.Text == "object"
					&& args.Name == "e"
					&& args.Type.Text.EndsWith("EventArgs"))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Checks whether specified element describes test method.
		/// </summary>
		public static bool IsTestMethod(CsElement element)
		{
			if (element.ElementType != ElementType.Method)
				return false;

			if (element.Attributes != null)
			{
				foreach (Attribute attr in element.Attributes)
				{
					for (Node<CsToken> node = attr.ChildTokens.First; node != null; node = node.Next)
					{
						if (node.Value.Text == "TestMethod")
							return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Checks whether specified element has public access modifier.
		/// </summary>
		public static bool IsPublic(CsElement element)
		{
			AccessModifierType modifier = element.AccessModifier;

			if (modifier == AccessModifierType.Public)
				return true;

			return false;
		}

		/// <summary>
		/// Checks whether specified element has protected access modifier.
		/// </summary>
		public static bool IsProtected(CsElement element)
		{
			AccessModifierType modifier = element.AccessModifier;

			if (modifier == AccessModifierType.Protected)
				return true;

			return false;
		}

		/// <summary>
		/// Checks whether specified element has private access modifier.
		/// </summary>
		public static bool IsPrivate(CsElement element)
		{
			AccessModifierType modifier = element.AccessModifier;

			if (modifier == AccessModifierType.Private)
				return true;

			return false;
		}

		/// <summary>
		/// Checks whether specified element has internal access modifier.
		/// </summary>
		public static bool IsInternal(CsElement element)
		{
			AccessModifierType modifier = element.AccessModifier;

			if (modifier == AccessModifierType.Internal)
				return true;

			if (modifier == AccessModifierType.ProtectedInternal)
				return true;

			return false;
		}

		/// <summary>
		/// Checks whether specified element is static.
		/// </summary>
		public static bool IsStatic(CsElement element)
		{
			return element.Declaration.ContainsModifier(CsTokenType.Static);
		}

		/// <summary>
		/// Checks whether specified element is operator.
		/// </summary>
		public static bool IsOperator(CsElement element)
		{
			return element.Declaration.Name.StartsWith("operator ");
		}

		#endregion

		#region Working with element names

		/// <summary>
		/// Extracts pure name from the declaration.
		/// </summary>
		public static string ExtractPureName(string declarationName)
		{
			string text = declarationName.TrimStart('@');

			string[] parts = text.Split('.');
			text = parts[parts.Length - 1];

			int index = text.IndexOf('<');
			if (index < 0)
				return text;

			return text.Substring(0, index);
		}

		#endregion

		#region Working with parameters

		/// <summary>
		/// Gets a list of parameters for an element.
		/// </summary>
		public static List<ParameterItem> GetParameters(CsElement element)
		{
			IEnumerable<Parameter> ownParameters = new List<Parameter>();
			switch (element.ElementType)
			{
				case ElementType.Constructor:
					ownParameters = ((Constructor)element).Parameters;
					break;
				case ElementType.Delegate:
					ownParameters = ((Delegate)element).Parameters;
					break;
				case ElementType.Indexer:
					ownParameters = ((Indexer)element).Parameters;
					break;
				case ElementType.Method:
					ownParameters = ((Method)element).Parameters;
					break;
			}

			List<ParameterItem> result = ConvertParameters(ownParameters);
			element.WalkElement(null, null, GetParametersExpressionVisitor, result);
			return result;
		}

		/// <summary>
		/// Converts native parameters to parameter items.
		/// </summary>
		private static List<ParameterItem> ConvertParameters(IEnumerable<Parameter> parameters)
		{
			List<ParameterItem> result = new List<ParameterItem>();
			foreach (Parameter parameter in parameters)
			{
				result.Add(new ParameterItem
					{
						Name = parameter.Name,
						LineNumber = parameter.LineNumber
					});
			}

			return result;
		}

		/// <summary>
		/// Analyzes expression for getting parameters.
		/// </summary>
		private static bool GetParametersExpressionVisitor(
			Expression expression,
			Expression parentExpression,
			Statement parentStatement,
			CsElement parentElement,
			List<ParameterItem> result)
		{
			IEnumerable<Parameter> innerParameters;
			switch (expression.ExpressionType)
			{
				case ExpressionType.AnonymousMethod:
					innerParameters = ((AnonymousMethodExpression)expression).Parameters;
					break;
				case ExpressionType.Lambda:
					innerParameters = ((LambdaExpression)expression).Parameters;
					break;
				default:
					return true;
			}

			result.AddRange(ConvertParameters(innerParameters));
			return true;
		}

		/// <summary>
		/// Gets a list of type parameters for an element.
		/// </summary>
		public static List<TypeParameterItem> GetTypeParameters(CsElement element)
		{
			List<TypeParameterItem> result = new List<TypeParameterItem>();

			for (Node<CsToken> node = element.Tokens.First; node != null; node = node.Next)
			{
				if (node.Value.CsTokenType == CsTokenType.OpenCurlyBracket
					|| node.Value.CsTokenType == CsTokenType.OpenParenthesis
					|| node.Value.CsTokenType == CsTokenType.BaseColon
					|| node.Value.CsTokenType == CsTokenType.Where)
					break;

				if (StyleCop43Compatibility.IsStyleCop43())
				{
					if (node.Value.Text == "where")
						break;
				}

				if (node.Value.CsTokenClass == CsTokenClass.GenericType)
				{
					GenericType type = (GenericType)node.Value;

					if (element.ElementType == ElementType.Method)
					{
						Method method = (Method)element;
						if (type == method.ReturnType)
							continue;
					}

					if (element.ElementType == ElementType.Delegate)
					{
						Delegate @delegate = (Delegate)element;
						if (type == @delegate.ReturnType)
							continue;
					}

					for (Node<CsToken> inner = type.ChildTokens.First; inner != null; inner = inner.Next)
					{
						if (inner.Value.CsTokenClass == CsTokenClass.Type)
						{
							result.Add(new TypeParameterItem
								{
									Name = inner.Value.Text,
									LineNumber = inner.Value.LineNumber
								});
						}
					}
				}
			}

			return result;
		}

		#endregion

		#region Working with local declarations and labels

		/// <summary>
		/// Gets local declarations.
		/// </summary>
		public static List<LocalDeclarationItem> GetLocalDeclarations(CsElement element)
		{
			List<LocalDeclarationItem> result = new List<LocalDeclarationItem>();
			element.WalkElement(null, GetLocalDeclarationsStatementVisitor, result);
			return result;
		}

		/// <summary>
		/// Analyzes statements for getting local declarations.
		/// </summary>
		private static bool GetLocalDeclarationsStatementVisitor(
			Statement statement,
			Expression parentExpression,
			Statement parentStatement,
			CsElement parentElement,
			List<LocalDeclarationItem> declarations)
		{
			if (statement.StatementType == StatementType.Block)
				return true;

			if (statement.Variables.Count > 0)
			{
				foreach (Variable variable in statement.Variables)
				{
					declarations.Add(new LocalDeclarationItem
						{
							Name = variable.Name,
							LineNumber = StyleCop43Compatibility.GetVariableLineNumber(variable)
						});
				}

				return true;
			}

			if (statement.StatementType == StatementType.VariableDeclaration)
			{
				VariableDeclarationStatement declaration = (VariableDeclarationStatement)statement;
				foreach (VariableDeclaratorExpression declarator in declaration.Declarators)
				{
					declarations.Add(new LocalDeclarationItem
						{
							Name = declarator.Identifier.Text,
							IsConstant = declaration.Constant,
							LineNumber = declarator.LineNumber
						});
				}
			}

			return true;
		}

		/// <summary>
		/// Gets labels.
		/// </summary>
		public static List<LabelItem> GetLabels(CsElement element)
		{
			List<LabelItem> result = new List<LabelItem>();
			element.WalkElement(null, GetLabelsStatementVisitor, result);
			return result;
		}

		/// <summary>
		/// Analyzes statements for getting labels.
		/// </summary>
		private static bool GetLabelsStatementVisitor(
			Statement statement,
			Expression parentExpression,
			Statement parentStatement,
			CsElement parentElement,
			List<LabelItem> result)
		{
			if (statement.StatementType != StatementType.Label)
				return true;

			LabelStatement label = (LabelStatement)statement;
			result.Add(new LabelItem
				{
					Name = label.Identifier.Text,
					LineNumber = label.LineNumber
				});

			return true;
		}

		#endregion

		#region Working with code tree

		/// <summary>
		/// Gets first node by specified line number.
		/// </summary>
		public static Node<CsToken> GetNodeByLine(CsDocument document, int lineNumber)
		{
			for (Node<CsToken> node = document.Tokens.First; node != null; node = node.Next)
			{
				if (node.Value.LineNumber == lineNumber)
					return node;
			}

			return null;
		}

		/// <summary>
		/// Finds previous valuable node.
		/// </summary>
		public static Node<CsToken> FindPreviousValueableNode(Node<CsToken> target)
		{
			for (Node<CsToken> node = target.Previous; node != null; node = node.Previous)
			{
				if (node.Value.CsTokenType == CsTokenType.WhiteSpace
					|| node.Value.CsTokenType == CsTokenType.EndOfLine)
					continue;

				return node;
			}

			return null;
		}

		/// <summary>
		/// Finds next valuable node.
		/// </summary>
		public static Node<CsToken> FindNextValueableNode(Node<CsToken> target)
		{
			for (Node<CsToken> node = target.Next; node != null; node = node.Next)
			{
				if (node.Value.CsTokenType == CsTokenType.WhiteSpace
					|| node.Value.CsTokenType == CsTokenType.EndOfLine)
					continue;

				return node;
			}

			return null;
		}

		/// <summary>
		/// Gets first code element at specified line number.
		/// </summary>
		/// <remarks>
		/// Returns object because it is ICodeElement for 4.4 and CodeElement for 4.3.
		/// </remarks>
		public static object GetElementByLine(CsDocument document, int lineNumber)
		{
			object[] args = new object[] { lineNumber, null };
			document.WalkDocument(FindByLineElementVisitor, null, args);

			return args[1];
		}

		/// <summary>
		/// Tries to find code element by line.
		/// </summary>
		private static bool FindByLineElementVisitor(
			CsElement element,
			CsElement parentElement,
			object context)
		{
			object[] args = (object[])context;
			int lineNumber = (int)args[0];

			if (element.Location.StartPoint.LineNumber > lineNumber)
				return false;

			if (element.Location.StartPoint.LineNumber <= lineNumber
				&& element.Location.EndPoint.LineNumber >= lineNumber)
			{
				args[1] = element;
			}

			return true;
		}

		/// <summary>
		/// Gets first expression at specified line number.
		/// </summary>
		public static Expression GetExpressionByLine(CsDocument document, int lineNumber)
		{
			object[] args = new object[] { lineNumber, null };
			document.WalkDocument(null, null, FindByLineExpressionVisitor, args);

			return (Expression)args[1];
		}

		/// <summary>
		/// Tries to find expression by line.
		/// </summary>
		private static bool FindByLineExpressionVisitor(
			Expression expression,
			Expression parentExpression,
			Statement parentStatement,
			CsElement parentElement,
			object context)
		{
			object[] args = (object[])context;
			int lineNumber = (int)args[0];

			if (expression.Location.StartPoint.LineNumber > lineNumber)
				return false;

			if (expression.Location.StartPoint.LineNumber <= lineNumber
				&& expression.Location.EndPoint.LineNumber >= lineNumber)
			{
				args[1] = expression;
			}

			return true;
		}

		/// <summary>
		/// Finds the root expression.
		/// </summary>
		public static Expression GetRootExpression(Expression expression)
		{
			Expression root = expression;

			while (root.Parent is Expression)
			{
				root = (Expression)root.Parent;
			}

			return root;
		}

		#endregion

		#region Working with documentation

		/// <summary>
		/// Gets summary text for specified element.
		/// </summary>
		public static string GetSummaryText(CsElement element)
		{
			XmlDocument header = MakeXml(element.Header.Text);
			if (header != null)
			{
				XmlNode node = header.SelectSingleNode("root/summary");
				if (node != null)
				{
					return node.InnerXml.Trim();
				}
			}

			return null;
		}

		/// <summary>
		/// Creates XML document from its inner text.
		/// </summary>
		public static XmlDocument MakeXml(string text)
		{
			XmlDocument document = new XmlDocument();

			try
			{
				StringBuilder xml = new StringBuilder();
				xml.Append("<root>");
				xml.Append(text);
				xml.Append("</root>");
				document.LoadXml(xml.ToString());
			}
			catch (XmlException)
			{
				return null;
			}

			return document;
		}

		#endregion

		#region Working with elements size

		/// <summary>
		/// Gets element size in line count.
		/// </summary>
		public static int GetElementSizeByDeclaration(CsElement element)
		{
			var open = FindNextValueableNode(element.Declaration.Tokens.Last);
			var close = element.Tokens.Last;
			return close.Value.LineNumber - open.Value.LineNumber + 1;
		}

		#endregion
	}
}
