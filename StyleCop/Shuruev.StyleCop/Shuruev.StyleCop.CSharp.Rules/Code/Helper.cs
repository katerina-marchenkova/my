using System.Collections.Generic;
using System.Reflection;
using Microsoft.StyleCop;
using Microsoft.StyleCop.CSharp;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Helper methods.
	/// </summary>
	public static class Helper
	{
		/// <summary>
		/// Checks whether specified element describes windows forms event handler.
		/// </summary>
		public static bool IsWindowsFormsEventHandler(CsElement element)
		{
			if (element.ElementType == ElementType.Method)
			{
				if (element.AccessModifier == AccessModifierType.Private)
				{
					if (element.Declaration.Name.Contains("_"))
					{
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
					}
				}
			}

			return false;
		}
	}
}
