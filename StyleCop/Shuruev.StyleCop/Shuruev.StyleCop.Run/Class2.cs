using System;
using System.Collections.Generic;
using System.Text;

namespace Shuruev.StyleCop.Run
{
	/// <summary>
	/// BBB class.
	/// </summary>
	public class BBB
	{
		public int A = 10;

		/// <summary>
		/// Some action here.
		/// </summary>
		public void Zzz()
		{
			string text = String.Format(
				"oleg {0}{1}{2}",
				String.Format(
					"{0}{1}",
					"uh",
					"eh"),
				"aga",
				"ogo");
			/*string text = String.Format(
				"oleg {0}",
				String.Format(
					"*/

			/*XElement xml = new XElement(
				"contacts",
				new XElement(
					"contact",
					new XAttribute("contactId", "2"),
					new XElement("firstName", "Barry"),
					new XElement("lastName", "Gottshall")),
				new XElement("contact",
					new XAttribute("contactId", "3"),
					new XElement("firstName", "Armando"),
					new XElement("lastName", "Valdes")
				));

			Console.WriteLine(xml);*/
		}
	}
}
