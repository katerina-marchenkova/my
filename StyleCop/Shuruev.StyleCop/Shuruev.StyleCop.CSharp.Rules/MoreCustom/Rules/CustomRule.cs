﻿using System;
using System.Drawing;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Represents a single custom rule.
	/// </summary>
	public abstract class CustomRule
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		internal CustomRule(
			Rules rule,
			string code,
			string settingName,
			string description,
			Image exampleImage)
		{
			Rule = rule;
			Code = code;
			SettingName = settingName;
			Description = description;
			ExampleImage = exampleImage;
		}

		#region Properties

		/// <summary>
		/// Gets or sets rule object.
		/// </summary>
		private Rules Rule { get; set; }

		/// <summary>
		/// Gets rule code.
		/// </summary>
		public string Code { get; private set; }

		/// <summary>
		/// Gets rule name.
		/// </summary>
		public string RuleName
		{
			get
			{
				return Rule.ToString();
			}
		}

		/// <summary>
		/// Gets rule setting name.
		/// </summary>
		public string SettingName { get; private set; }

		/// <summary>
		/// Gets rule description.
		/// </summary>
		public string Description { get; private set; }

		/// <summary>
		/// Gets rule details URL.
		/// </summary>
		public string DetailsUrl
		{
			get
			{
				return String.Format(CustomRulesResources.DetailsUrl, Code);
			}
		}

		/// <summary>
		/// Gets rule example image.
		/// </summary>
		public Image ExampleImage { get; private set; }

		#endregion
	}
}
