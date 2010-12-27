using System;
using System.Reflection;

namespace SomeNamespace
{
	public class SP2100
	{
		public void SomeMethod()
		{
			// very long code line
			object result = typeof(Type).InvokeMember("valueType", BindingFlags.Static | BindingFlags.GetField | BindingFlags.NonPublic, null, null, null);

			// ...
		}
	}
}
