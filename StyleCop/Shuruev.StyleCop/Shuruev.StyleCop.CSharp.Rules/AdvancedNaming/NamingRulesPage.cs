using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.StyleCop;

namespace Shuruev.StyleCop.CSharp.AdvancedNaming
{
	/// <summary>
	/// Control displaying naming rules page.
	/// </summary>
	public partial class NamingRulesPage : UserControl
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public NamingRulesPage()
		{
			InitializeComponent();
		}

		#region Properties

		/// <summary>
		/// Gets or sets parent property page.
		/// </summary>
		public PropertyPage Page { get; set; }

		#endregion

		/// <summary>
		/// Initializes the specified property control with the StyleCop settings file.
		/// </summary>
		public void Initialize()
		{
		}

		/// <summary>
		/// Apply the modifications to the StyleCop settings file.
		/// </summary>
		public bool Apply()
		{
			return true;
		}

		/// <summary>
		/// Refreshes the state of the settings override.
		/// </summary>
		public void RefreshSettingsOverrideState()
		{
		}
	}
}
