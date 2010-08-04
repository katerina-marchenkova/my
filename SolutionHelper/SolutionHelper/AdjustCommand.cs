using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using EnvDTE80;

namespace SolutionHelper
{
	/// <summary>
	/// "Adjust" command.
	/// </summary>
	public class AdjustCommand : AddInCommand
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public AdjustCommand()
		{
			AddInName = "SolutionHelper";
			ClassName = "Connect";
			CommandName = "Adjust";
			CommandText = "Adjust Solution (Beta)";
			CommandTooltip = "Adjusts solution according to the internal development practices";
			CommandBarCode = "Solution";
		}
	}
}
