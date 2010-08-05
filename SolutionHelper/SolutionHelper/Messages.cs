using System;
using System.Windows.Forms;

namespace SolutionHelper
{
	/// <summary>
	/// Displays messages.
	/// </summary>
	public static class Messages
	{
		/// <summary>
		/// Gets or sets add-in name.
		/// </summary>
		public static string AddInName { get; set; }

		/// <summary>
		/// Displays information message.
		/// </summary>
		public static void ShowInformation(IWin32Window owner, string format, params object[] args)
		{
			string message = String.Format(format, args);
			MessageBox.Show(
				owner,
				message,
				AddInName,
				MessageBoxButtons.OK,
				MessageBoxIcon.Information);
		}

		/// <summary>
		/// Displays warning message.
		/// </summary>
		public static void ShowWarning(IWin32Window owner, string format, params object[] args)
		{
			string message = String.Format(format, args);
			MessageBox.Show(
				owner,
				message,
				AddInName,
				MessageBoxButtons.OK,
				MessageBoxIcon.Warning);
		}

		/// <summary>
		/// Displays error message.
		/// </summary>
		public static void ShowError(IWin32Window owner, string format, params object[] args)
		{
			string message = String.Format(format, args);
			MessageBox.Show(
				owner,
				message,
				AddInName,
				MessageBoxButtons.OK,
				MessageBoxIcon.Error);
		}
	}
}
