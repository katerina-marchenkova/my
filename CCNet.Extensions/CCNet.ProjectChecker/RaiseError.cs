using System;
using CCNet.Common;
using CCNet.ProjectChecker.Properties;

namespace CCNet.ProjectChecker
{
	/// <summary>
	/// Raises all errors.
	/// </summary>
	public static class RaiseError
	{
		/// <summary>
		/// Gets an appropriate exit code.
		/// </summary>
		public static int ExitCode { get; private set; }

		#region Internal methods

		/// <summary>
		/// Raises an error.
		/// </summary>
		private static void RaiseInternal(string message)
		{
			ExitCode = 1;

			Console.Error.WriteLine(
				Resources.DescriptionHtml,
				message.ToHtml());
		}

		#endregion

		#region File structure errors

		/// <summary>
		/// Raises "WrongProjectFileLocation" error.
		/// </summary>
		public static void WrongProjectFileLocation()
		{
			RaiseInternal(
				Errors.WrongProjectFileLocation
				.Display(Arguments.ProjectName));
		}

		#endregion

		#region Project properties errors

		/// <summary>
		/// Raises "WrongVisualStudioVersion" error.
		/// </summary>
		public static void WrongVisualStudioVersion(string currentVersion)
		{
			RaiseInternal(
				Errors.WrongVisualStudioVersion
				.Display(Arguments.VisualStudioVersion, currentVersion));
		}

		/// <summary>
		/// Raises "UnknownConfiguration" error.
		/// </summary>
		public static void UnknownConfiguration(string configurationName)
		{
			RaiseInternal(
				Errors.UnknownConfiguration
				.Display(configurationName));
		}

		/// <summary>
		/// Raises "WrongPlatform" error.
		/// </summary>
		public static void WrongPlatform(string platformName)
		{
			RaiseInternal(
				Errors.WrongPlatform
				.Display(Arguments.TargetPlatform, platformName));
		}

		/// <summary>
		/// Raises "WrongCommonProperties" error.
		/// </summary>
		public static void WrongCommonProperties(string description)
		{
			RaiseInternal(
				Errors.WrongCommonProperties
				.Display(description));
		}

		/// <summary>
		/// Raises "WrongDebugProperties" error.
		/// </summary>
		public static void WrongDebugProperties(string description)
		{
			RaiseInternal(
				Errors.WrongDebugProperties
				.Display(description));
		}

		/// <summary>
		/// Raises "WrongReleaseProperties" error.
		/// </summary>
		public static void WrongReleaseProperties(string description)
		{
			RaiseInternal(
				Errors.WrongReleaseProperties
				.Display(description));
		}

		#endregion
	}
}
