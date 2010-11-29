using System;
using System.Windows.Forms;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Notification area that displays warnings.
	/// </summary>
	public partial class WarningArea : UserControl
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public WarningArea()
		{
			InitializeComponent();
		}

		#region Event handlers

		private void WarningArea_Load(object sender, EventArgs e)
		{
			if (DesignMode)
				return;

			Clear();
		}

		#endregion

		#region Displaying warnings

		/// <summary>
		/// Clears all warnings.
		/// </summary>
		public void Clear()
		{
			tableWarnings.Controls.Clear();
			tableWarnings.RowCount = 0;
			tableWarnings.RowStyles.Clear();
		}

		/// <summary>
		/// Adds specified warning.
		/// </summary>
		public void Add(string warningText, string warningUrl)
		{
			tableWarnings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			tableWarnings.RowCount = tableWarnings.RowCount + 1;

			WarningItem item = new WarningItem(warningText, warningUrl);
			item.Margin = new Padding(4, 2, 4, 2);
			item.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			item.AutoSize = true;
			tableWarnings.Controls.Add(item, 0, tableWarnings.RowCount - 1);
		}

		#endregion
	}
}
