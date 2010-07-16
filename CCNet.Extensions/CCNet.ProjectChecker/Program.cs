using System;
using System.Collections.Generic;
using System.IO;
using CCNet.Common;
using CCNet.ProjectChecker.Properties;

namespace CCNet.ProjectChecker
{
	/// <summary>
	/// Checks project during build process.
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// Main program.
		/// </summary>
		public static int Main(string[] args)
		{
			args = new[]
			{
				@"ProjectName=ConsoleApplication1",
				@"ProjectPath=C:\Users\oshuruev\Documents\Visual Studio 2010\Projects\ConsoleApplication1\ConsoleApplication1",
				@"ProjectType=ConsoleApplication",
				@"VisualStudioVersion=2010",
				@"TargetFramework=Net40",
				@"TargetPlatform=x86",
				@"RootNamespace=ConsoleApplication1",
			};

			if (args == null || args.Length == 0)
			{
				DisplayUsage();
				return 0;
			}

			Arguments.Default = ArgumentProperties.Parse(args);

			CheckWrongProjectFileLocation();
			if (RaiseError.ExitCode > 0)
				return RaiseError.ExitCode;

			ProjectHelper.LoadProject(Paths.ProjectFile);
			CheckWrongVisualStudioVersion();
			CheckUnknownConfiguration();
			CheckWrongPlatform();
			CheckWrongCommonProperties();
			CheckWrongDebugProperties();
			CheckWrongReleaseProperties();

			return RaiseError.ExitCode;
		}

		/// <summary>
		/// Displays usage text.
		/// </summary>
		private static void DisplayUsage()
		{
			Console.WriteLine();
			Console.WriteLine(Resources.UsageInfo);
			Console.WriteLine();
		}

		#region Checking file structure

		/// <summary>
		/// Checks "WrongProjectFileLocation" condition.
		/// </summary>
		private static void CheckWrongProjectFileLocation()
		{
			string[] files = Directory.GetFiles(Arguments.ProjectPath, "*.csproj", SearchOption.AllDirectories);
			if (files.Length == 1)
			{
				if (files[0] == Paths.ProjectFile)
					return;
			}

			RaiseError.WrongProjectFileLocation();
		}

		#endregion

		#region Checking project properties

		/// <summary>
		/// Checks "WrongVisualStudioVersion" condition.
		/// </summary>
		private static void CheckWrongVisualStudioVersion()
		{
			string currentVersion = ProjectHelper.GetVisualStudioVersion();
			if (currentVersion == Arguments.VisualStudioVersion)
				return;

			RaiseError.WrongVisualStudioVersion(currentVersion);
		}

		/// <summary>
		/// Checks "UnknownConfiguration" condition.
		/// </summary>
		private static void CheckUnknownConfiguration()
		{
			List<string> configurations = ProjectHelper.GetUsedConfigurations();
			configurations.Remove("Debug");
			configurations.Remove("Release");

			if (configurations.Count == 0)
				return;

			RaiseError.UnknownConfiguration(configurations[0]);
		}

		/// <summary>
		/// Checks "WrongPlatform" condition.
		/// </summary>
		private static void CheckWrongPlatform()
		{
			List<string> platforms = ProjectHelper.GetUsedPlatforms();

			platforms.Remove(Arguments.TargetPlatform);
			if (platforms.Count == 0)
				return;

			RaiseError.WrongPlatform(platforms[0]);
		}

		/// <summary>
		/// Checks "WrongCommonProperties" condition.
		/// </summary>
		private static void CheckWrongCommonProperties()
		{
			Dictionary<string, string> properties = ProjectHelper.GetCommonProperties();
			Dictionary<string, string> required = new Dictionary<string, string>();
			Dictionary<string, string> allowed = new Dictionary<string, string>();

			required.Add("AppDesignerFolder", "Properties");

			switch (Arguments.ProjectType)
			{
				case ProjectType.ConsoleApplication:
					allowed.Add("ApplicationIcon", String.Empty);
					break;
				case ProjectType.WindowsApplication:
					required.Add("ApplicationIcon", null);
					break;
			}

			required.Add("AssemblyName", Arguments.ProjectName);
			allowed.Add("CodeContractsAssemblyMode", null);
			allowed.Add("Configuration", null);
			required.Add("FileAlignment", "512");
			allowed.Add("GenerateResourceNeverLockTypeAssemblies", "true");

			switch (Arguments.ProjectType)
			{
				case ProjectType.ConsoleApplication:
					required.Add("OutputType", "Exe");
					break;
				case ProjectType.WindowsApplication:
					required.Add("OutputType", "WinExe");
					break;
			}

			allowed.Add("Platform", null);
			allowed.Add("PostBuildEvent", String.Empty);
			allowed.Add("PreBuildEvent", String.Empty);
			allowed.Add("ProductVersion", null);
			allowed.Add("ProjectGuid", null);
			required.Add("RootNamespace", Arguments.RootNamespace);
			allowed.Add("RunPostBuildEvent", "OnBuildSuccess");
			allowed.Add("SccAuxPath", null);
			allowed.Add("SccLocalPath", null);
			allowed.Add("SccProjectName", null);
			allowed.Add("SccProvider", null);
			allowed.Add("SchemaVersion", null);
			allowed.Add("SignAssembly", "false");
			allowed.Add("StartupObject", String.Empty);
			allowed.Add("TargetFrameworkProfile", null);

			switch (Arguments.TargetFramework)
			{
				case TargetFramework.Net20:
					allowed.Add("TargetFrameworkVersion", "v2.0");
					break;
				case TargetFramework.Net35:
					required.Add("TargetFrameworkVersion", "v3.5");
					break;
				case TargetFramework.Net40:
					required.Add("TargetFrameworkVersion", "v4.0");
					break;
			}

			allowed.Add("UseVSHostingProcess", null);
			allowed.Add("Win32Resource", String.Empty);

			string description;
			if (ValidationHelper.CheckProperties(
				properties,
				required,
				allowed,
				out description))
				return;

			RaiseError.WrongCommonProperties(description);
		}

		/// <summary>
		/// Checks "WrongDebugProperties" condition.
		/// </summary>
		private static void CheckWrongDebugProperties()
		{
			Dictionary<string, string> properties = ProjectHelper.GetDebugProperties();
			Dictionary<string, string> required = new Dictionary<string, string>();
			Dictionary<string, string> allowed = new Dictionary<string, string>();

			required.Add("DebugSymbols", "true");
			required.Add("DebugType", "full");
			required.Add("DefineConstants", "DEBUG;TRACE");
			required.Add("DocumentationFile", @"bin\Debug\{0}.xml".Display(Arguments.ProjectName));
			required.Add("ErrorReport", "prompt");
			required.Add("Optimize", "false");
			required.Add("OutputPath", @"bin\Debug\");
			allowed.Add("PlatformTarget", Arguments.TargetPlatform);
			required.Add("WarningLevel", "4");
			allowed.Add("CodeContractsArithmeticObligations", null);
			allowed.Add("CodeContractsBaseLineFile", null);
			allowed.Add("CodeContractsBoundsObligations", null);
			allowed.Add("CodeContractsCustomRewriterAssembly", null);
			allowed.Add("CodeContractsCustomRewriterClass", null);
			allowed.Add("CodeContractsEmitXMLDocs", null);
			allowed.Add("CodeContractsEnableRuntimeChecking", null);
			allowed.Add("CodeContractsExtraAnalysisOptions", null);
			allowed.Add("CodeContractsExtraRewriteOptions", null);
			allowed.Add("CodeContractsLibPaths", null);
			allowed.Add("CodeContractsNonNullObligations", null);
			allowed.Add("CodeContractsRedundantAssumptions", null);
			allowed.Add("CodeContractsReferenceAssembly", null);
			allowed.Add("CodeContractsRunCodeAnalysis", null);
			allowed.Add("CodeContractsRunInBackground", null);
			allowed.Add("CodeContractsRuntimeCallSiteRequires", null);
			allowed.Add("CodeContractsRuntimeCheckingLevel", null);
			allowed.Add("CodeContractsRuntimeOnlyPublicSurface", null);
			allowed.Add("CodeContractsRuntimeThrowOnFailure", null);
			allowed.Add("CodeContractsShowSquigglies", null);
			allowed.Add("CodeContractsUseBaseLine", null);

			string description;
			if (ValidationHelper.CheckProperties(
				properties,
				required,
				allowed,
				out description))
				return;

			RaiseError.WrongDebugProperties(description);
		}

		/// <summary>
		/// Checks "WrongReleaseProperties" condition.
		/// </summary>
		private static void CheckWrongReleaseProperties()
		{
			Dictionary<string, string> properties = ProjectHelper.GetReleaseProperties();
			Dictionary<string, string> required = new Dictionary<string, string>();
			Dictionary<string, string> allowed = new Dictionary<string, string>();

			required.Add("DebugSymbols", "true");
			required.Add("DebugType", "pdbonly");
			required.Add("DefineConstants", "TRACE");
			required.Add("DocumentationFile", @"bin\Release\{0}.xml".Display(Arguments.ProjectName));
			required.Add("ErrorReport", "prompt");
			required.Add("Optimize", "true");
			required.Add("OutputPath", @"bin\Release\");
			allowed.Add("PlatformTarget", Arguments.TargetPlatform);
			required.Add("WarningLevel", "4");
			allowed.Add("CodeContractsArithmeticObligations", null);
			allowed.Add("CodeContractsBaseLineFile", null);
			allowed.Add("CodeContractsBoundsObligations", null);
			allowed.Add("CodeContractsCustomRewriterAssembly", null);
			allowed.Add("CodeContractsCustomRewriterClass", null);
			allowed.Add("CodeContractsEmitXMLDocs", null);
			allowed.Add("CodeContractsEnableRuntimeChecking", null);
			allowed.Add("CodeContractsExtraAnalysisOptions", null);
			allowed.Add("CodeContractsExtraRewriteOptions", null);
			allowed.Add("CodeContractsLibPaths", null);
			allowed.Add("CodeContractsNonNullObligations", null);
			allowed.Add("CodeContractsRedundantAssumptions", null);
			allowed.Add("CodeContractsReferenceAssembly", null);
			allowed.Add("CodeContractsRunCodeAnalysis", null);
			allowed.Add("CodeContractsRunInBackground", null);
			allowed.Add("CodeContractsRuntimeCallSiteRequires", null);
			allowed.Add("CodeContractsRuntimeCheckingLevel", null);
			allowed.Add("CodeContractsRuntimeOnlyPublicSurface", null);
			allowed.Add("CodeContractsRuntimeThrowOnFailure", null);
			allowed.Add("CodeContractsShowSquigglies", null);
			allowed.Add("CodeContractsUseBaseLine", null);

			string description;
			if (ValidationHelper.CheckProperties(
				properties,
				required,
				allowed,
				out description))
				return;

			RaiseError.WrongReleaseProperties(description);
		}

		#endregion
	}
}
