using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Shuruev.StyleCop.Run
{
	/// <summary>
	/// This is a class 2.
	/// </summary>
	public class Class2
	{
		private event CancelEventHandler _onCancel;

		/// <summary>
		/// Adds or removes Cancel event.
		/// </summary>
		[Category("Action")]
		[Description("Cancel")]
		public event CancelEventHandler Cancel
		{
			add { _onCancel += value; }
			remove { _onCancel -= value; }
		}

		/// <summary>
		/// Fires cancel event.
		/// </summary>
		protected virtual void OnCancel(CancelEventArgs e)
		{
			if (_onCancel != null)
			{
				_onCancel(this, e);
			}
		}
	}
}
