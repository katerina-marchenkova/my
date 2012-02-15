using System;
using System.Windows.Forms;
using EnvDTE80;
using SolutionHelper.Properties;

namespace SolutionHelper
{
	/// <summary>
	/// Dialog for running add-in.
	/// </summary>
	public partial class RunForm : Form
	{
		/// <summary>
		/// Gets or sets an application object.
		/// </summary>
		public DTE2 ApplicationObject { get; set; }

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public RunForm()
		{
			InitializeComponent();
		}

		#region Event handlers

		private void RunForm_Load(object sender, EventArgs e)
		{
			Text = Messages.AddInName;
			textBaseReferencePaths.Text = Settings.Default.BaseReferencePaths;
			txtLocalPathInternal.Text = Settings.Default.InternalReferencesTargetPath;
			txtLocalPathExternal.Text = Settings.Default.ExternalReferencesTargetPath;
		}

		private void btnAdjust_Click(object sender, EventArgs e)
		{
			Enabled = false;

			Refresh();
			Application.DoEvents();

			Adjust();
			Enabled = true;

			Close();
		}


		private void btnDownloadLatestLibraries_Click(object sender, EventArgs e)
		{
			Enabled = false;

			Refresh();
			Application.DoEvents();

			DownloadLatestReferences();
			Enabled = true;
		}

		private void RunForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Close();
		}

		#endregion

		#region Adjusting process

		/// <summary>
		/// Performs adjusting.
		/// </summary>
		private void Adjust()
		{
			try
			{
				SaveSettings();

				ProjectAdapter adapter = new ProjectAdapter();
				adapter.Adjust(ApplicationObject, Settings.Default.BaseReferencePaths);
			}
			catch (Exception e)
			{
				Messages.ShowWarning(this, e.Message);
				return;
			}

			Messages.ShowInformation(this, Resources.AdjustDone);
		}

		/// <summary>
		/// Reloads latest version of references.
		/// </summary>
		private void DownloadLatestReferences()
		{
			try
			{
				SaveSettings();

				ProjectAdapter adapter = new ProjectAdapter();
				adapter.DownloadLatestReferences(
					ApplicationObject,
					Settings.Default.InternalReferencesSourcePath,
					Settings.Default.ExternalReferencesSourcePath,
					Settings.Default.InternalReferencesTargetPath,
					Settings.Default.ExternalReferencesTargetPath);
			}
			catch (Exception e)
			{
				Messages.ShowWarning(this, e.Message);
				return;
			}

			Messages.ShowInformation(this, Resources.AdjustDone);
		}

		/// <summary>
		/// Saves user settings.
		/// </summary>
		private void SaveSettings()
		{
			Settings.Default.BaseReferencePaths = textBaseReferencePaths.Text;
			Settings.Default.InternalReferencesTargetPath = txtLocalPathInternal.Text;
			Settings.Default.ExternalReferencesTargetPath = txtLocalPathExternal.Text;
			Settings.Default.Save();
		}

		#endregion
	}
}
