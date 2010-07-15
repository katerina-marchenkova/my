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
				@"TargetFramework=4.0",
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

			required.Add("RootNamespace", Arguments.RootNamespace);
			required.Add("AssemblyName", Arguments.ProjectName);
			required.Add("AppDesignerFolder", "Properties");
			required.Add("FileAlignment", "512");

			switch (Arguments.ProjectType)
			{
				case "ConsoleApplication":
					required.Add("OutputType", "Exe");
					break;
				default:
					throw new InvalidOperationException("Invalid project type specified.");
			}

			switch (Arguments.TargetFramework)
			{
				case "2.0":
					allowed.Add("TargetFrameworkVersion", "v2.0");
					break;
				case "3.5":
					required.Add("TargetFrameworkVersion", "v3.5");
					break;
				case "4.0":
					required.Add("TargetFrameworkVersion", "v4.0");
					break;
				default:
					throw new InvalidOperationException("Invalid target framework specified.");
			}

			allowed.Add("PreBuildEvent", String.Empty);
			allowed.Add("PostBuildEvent", String.Empty);
			allowed.Add("Configuration", null);
			allowed.Add("Platform", null);
			allowed.Add("ProductVersion", null);
			allowed.Add("SchemaVersion", null);
			allowed.Add("ProjectGuid", null);
			allowed.Add("TargetFrameworkProfile", null);
			allowed.Add("CodeContractsAssemblyMode", null);

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
			required.Add("Optimize", "false");
			required.Add("OutputPath", @"bin\Debug\");
			required.Add("DefineConstants", "DEBUG;TRACE");
			required.Add("ErrorReport", "prompt");
			required.Add("WarningLevel", "4");
			required.Add("DocumentationFile", @"bin\Debug\{0}.xml".Display(Arguments.ProjectName));

			allowed.Add("PlatformTarget", Arguments.TargetPlatform);
			allowed.Add("CodeContractsEnableRuntimeChecking", null);
			allowed.Add("CodeContractsRuntimeOnlyPublicSurface", null);
			allowed.Add("CodeContractsRuntimeThrowOnFailure", null);
			allowed.Add("CodeContractsRuntimeCallSiteRequires", null);
			allowed.Add("CodeContractsRunCodeAnalysis", null);
			allowed.Add("CodeContractsNonNullObligations", null);
			allowed.Add("CodeContractsBoundsObligations", null);
			allowed.Add("CodeContractsArithmeticObligations", null);
			allowed.Add("CodeContractsRedundantAssumptions", null);
			allowed.Add("CodeContractsRunInBackground", null);
			allowed.Add("CodeContractsShowSquigglies", null);
			allowed.Add("CodeContractsUseBaseLine", null);
			allowed.Add("CodeContractsEmitXMLDocs", null);
			allowed.Add("CodeContractsCustomRewriterAssembly", null);
			allowed.Add("CodeContractsCustomRewriterClass", null);
			allowed.Add("CodeContractsLibPaths", null);
			allowed.Add("CodeContractsExtraRewriteOptions", null);
			allowed.Add("CodeContractsExtraAnalysisOptions", null);
			allowed.Add("CodeContractsBaseLineFile", null);
			allowed.Add("CodeContractsRuntimeCheckingLevel", null);
			allowed.Add("CodeContractsReferenceAssembly", null);

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
			required.Add("Optimize", "true");
			required.Add("OutputPath", @"bin\Release\");
			required.Add("DefineConstants", "TRACE");
			required.Add("ErrorReport", "prompt");
			required.Add("WarningLevel", "4");
			required.Add("DocumentationFile", @"bin\Release\{0}.xml".Display(Arguments.ProjectName));

			allowed.Add("PlatformTarget", Arguments.TargetPlatform);
			allowed.Add("CodeContractsEnableRuntimeChecking", null);
			allowed.Add("CodeContractsRuntimeOnlyPublicSurface", null);
			allowed.Add("CodeContractsRuntimeThrowOnFailure", null);
			allowed.Add("CodeContractsRuntimeCallSiteRequires", null);
			allowed.Add("CodeContractsRunCodeAnalysis", null);
			allowed.Add("CodeContractsNonNullObligations", null);
			allowed.Add("CodeContractsBoundsObligations", null);
			allowed.Add("CodeContractsArithmeticObligations", null);
			allowed.Add("CodeContractsRedundantAssumptions", null);
			allowed.Add("CodeContractsRunInBackground", null);
			allowed.Add("CodeContractsShowSquigglies", null);
			allowed.Add("CodeContractsUseBaseLine", null);
			allowed.Add("CodeContractsEmitXMLDocs", null);
			allowed.Add("CodeContractsCustomRewriterAssembly", null);
			allowed.Add("CodeContractsCustomRewriterClass", null);
			allowed.Add("CodeContractsLibPaths", null);
			allowed.Add("CodeContractsExtraRewriteOptions", null);
			allowed.Add("CodeContractsExtraAnalysisOptions", null);
			allowed.Add("CodeContractsBaseLineFile", null);
			allowed.Add("CodeContractsRuntimeCheckingLevel", null);
			allowed.Add("CodeContractsReferenceAssembly", null);

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
