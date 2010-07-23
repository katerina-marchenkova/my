using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
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
			/*xxxargs = new[]
			{
				@"ProjectName=ConsoleApplication1",
				@"ProjectPath=C:\Users\oshuruev\Documents\Visual Studio 2010\Projects\ConsoleApplication1\ConsoleApplication1",
				@"ProjectType=ClickOnce",
				@"AssemblyName=ConsoleApplication12",
				@"FriendlyName=123",
				@"DownloadZone=Public",
				@"VisualStudioVersion=2010",
				@"TargetFramework=Net40",
				@"TargetPlatform=x86",
				@"RootNamespace=ConsoleApplication1",
				@"SuppressWarnings="
			};*/

			if (args == null || args.Length == 0)
			{
				DisplayUsage();
				return 0;
			}

			try
			{
				Arguments.Default = ArgumentProperties.Parse(args);
				PerformChecks();
			}
			catch (Exception e)
			{
				RaiseError.RuntimeException(e);
			}

			return RaiseError.ExitCode;
		}

		/// <summary>
		/// Performs all checks.
		/// </summary>
		private static void PerformChecks()
		{
			CheckWrongProjectFileLocation();
			CheckWrongManifestFileLocation();
			if (RaiseError.ExitCode > 0)
				return;

			ProjectHelper.LoadProject(Paths.ProjectFile);
			CheckWrongVisualStudioVersion();
			CheckUnknownConfiguration();
			CheckWrongPlatform();
			CheckWrongCommonProperties();
			CheckWrongDebugProperties();
			CheckWrongReleaseProperties();

			CheckWrongManifestContents();

			CheckWrongReferences();
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
			string[] files = Directory.GetFiles(Arguments.WorkingDirectorySource, "*.csproj", SearchOption.AllDirectories);
			if (files.Length == 1)
			{
				if (files[0] == Paths.ProjectFile)
					return;
			}

			RaiseError.WrongProjectFileLocation();
		}

		/// <summary>
		/// Checks "WrongManifestFileLocation" condition.
		/// </summary>
		private static void CheckWrongManifestFileLocation()
		{
			if (Arguments.ProjectType != ProjectType.ClickOnce)
				return;

			string[] files = Directory.GetFiles(Arguments.WorkingDirectorySource, "App.manifest", SearchOption.AllDirectories);
			if (files.Length == 1)
			{
				if (files[0] == Paths.ManifestFile)
					return;
			}

			RaiseError.WrongManifestFileLocation();
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
				case ProjectType.Console:
					allowed.Add("ApplicationIcon", String.Empty);
					break;
				case ProjectType.ClickOnce:
					required.Add("ApplicationIcon", null);
					break;
			}

			required.Add("AssemblyName", Arguments.AssemblyName);
			allowed.Add("AssemblyOriginatorKeyFile", null);
			allowed.Add("CodeContractsAssemblyMode", null);
			allowed.Add("Configuration", null);
			allowed.Add("DelaySign", "false");
			allowed.Add("FileAlignment", "512");
			allowed.Add("FileUpgradeFlags", null);
			allowed.Add("GenerateResourceNeverLockTypeAssemblies", "true");
			allowed.Add("OldToolsVersion", null);

			switch (Arguments.ProjectType)
			{
				case ProjectType.Console:
					required.Add("OutputType", "Exe");
					break;
				case ProjectType.ClickOnce:
					required.Add("OutputType", "WinExe");
					break;
			}

			allowed.Add("Platform", null);
			allowed.Add("PostBuildEvent", String.Empty);
			allowed.Add("PreBuildEvent", String.Empty);
			allowed.Add("ProductVersion", null);
			allowed.Add("ProjectGuid", null);
			allowed.Add("PublishWizardCompleted", null);
			required.Add("RootNamespace", Arguments.RootNamespace);
			allowed.Add("RunPostBuildEvent", "OnBuildSuccess");
			allowed.Add("SccAuxPath", null);
			allowed.Add("SccLocalPath", null);
			allowed.Add("SccProjectName", null);
			allowed.Add("SccProvider", null);
			allowed.Add("SchemaVersion", null);
			allowed.Add("SignAssembly", "false");

			switch (Arguments.ProjectType)
			{
				case ProjectType.ClickOnce:
					required.Add("ApplicationManifest", @"Properties\App.manifest");
					required.Add("ApplicationRevision", "0");
					required.Add("ApplicationVersion", "1.0.0.0");
					required.Add("BootstrapperEnabled", "true");
					required.Add("GenerateManifests", "true");
					required.Add("Install", "true");
					required.Add("InstallFrom", "Web");
					required.Add("InstallUrl", "http://download.cnetcontentsolutions.com/{0}/{1}/".Display(Arguments.DownloadZone, Arguments.AssemblyName));
					required.Add("IsWebBootstrapper", "true");
					required.Add("ManifestCertificateThumbprint", "51C48C1CDB83928A3A6F46ED8865E80BF5D0B5EF");
					required.Add("ManifestKeyFile", "vortex.pfx");
					required.Add("ManifestTimestampUrl", "http://timestamp.comodoca.com/authenticode");
					required.Add("MapFileExtensions", "true");
					required.Add("MinimumRequiredVersion", "1.0.0.0");
					required.Add("ProductName", Arguments.FriendlyName);
					required.Add("PublisherName", "CNET Content Solutions");
					required.Add("PublishUrl", @"D:\publish\{0}\".Display(Arguments.AssemblyName));
					required.Add("SignManifests", "true");
					required.Add("UpdateEnabled", "true");
					allowed.Add("UpdateInterval", null);
					allowed.Add("UpdateIntervalUnits", null);
					required.Add("UpdateMode", "Foreground");
					required.Add("UpdatePeriodically", "false");
					required.Add("UpdateRequired", "true");
					required.Add("UseApplicationTrust", "false");
					break;
				default:
					required.Add("GenerateManifests", "false");
					allowed.Add("SignManifests", "true");
					break;
			}

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

			allowed.Add("TargetZone", null);
			allowed.Add("UpgradeBackupLocation", null);
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

			allowed.Add("CodeAnalysisRuleSet", null);
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
			required.Add("DebugSymbols", "true");
			required.Add("DebugType", "full");
			required.Add("DefineConstants", "DEBUG;TRACE");
			required.Add("DocumentationFile", @"bin\Debug\{0}.xml".Display(Arguments.AssemblyName));
			required.Add("ErrorReport", "prompt");
			allowed.Add("FxCopRules", null);
			allowed.Add("NoWarn", Arguments.SuppressWarnings);
			required.Add("Optimize", "false");
			required.Add("OutputPath", @"bin\Debug\");
			allowed.Add("PlatformTarget", Arguments.TargetPlatform);
			required.Add("WarningLevel", "4");
			allowed.Add("UseVSHostingProcess", null);

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

			allowed.Add("CodeAnalysisRuleSet", null);
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
			allowed.Add("DebugSymbols", "true");
			required.Add("DebugType", "pdbonly");
			required.Add("DefineConstants", "TRACE");
			required.Add("DocumentationFile", @"bin\Release\{0}.xml".Display(Arguments.AssemblyName));
			required.Add("ErrorReport", "prompt");
			allowed.Add("FxCopRules", null);
			allowed.Add("NoWarn", Arguments.SuppressWarnings);
			required.Add("Optimize", "true");
			required.Add("OutputPath", @"bin\Release\");
			allowed.Add("PlatformTarget", Arguments.TargetPlatform);
			required.Add("WarningLevel", "4");
			allowed.Add("UseVSHostingProcess", null);

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

		#region Checking file contents

		/// <summary>
		/// Checks "WrongManifestContents" condition.
		/// </summary>
		public static void CheckWrongManifestContents()
		{
			if (Arguments.ProjectType != ProjectType.ClickOnce)
				return;

			string xml = File.ReadAllText(Paths.ManifestFile);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);

			Dictionary<string, string> properties = PropertiesHelper.ParseFromXml(doc);
			Dictionary<string, string> required = new Dictionary<string, string>();
			Dictionary<string, string> allowed = new Dictionary<string, string>();

			required.Add("/asmv1:assembly[@manifestVersion]", "1.0");
			required.Add("/asmv1:assembly/assemblyIdentity[@version]", "1.0.0.0");
			required.Add("/asmv1:assembly/assemblyIdentity[@name]", "{0}.app".Display(Arguments.AssemblyName));
			required.Add("/asmv1:assembly/trustInfo/security/requestedPrivileges/requestedExecutionLevel[@level]", "asInvoker");
			required.Add("/asmv1:assembly/trustInfo/security/requestedPrivileges/requestedExecutionLevel[@uiAccess]", "false");
			required.Add("/asmv1:assembly/trustInfo/security/applicationRequestMinimum/defaultAssemblyRequest[@permissionSetReference]", "Custom");
			required.Add("/asmv1:assembly/trustInfo/security/applicationRequestMinimum/PermissionSet[@class]", "System.Security.PermissionSet");
			required.Add("/asmv1:assembly/trustInfo/security/applicationRequestMinimum/PermissionSet[@version]", "1");
			required.Add("/asmv1:assembly/trustInfo/security/applicationRequestMinimum/PermissionSet[@ID]", "Custom");
			required.Add("/asmv1:assembly/trustInfo/security/applicationRequestMinimum/PermissionSet[@SameSite]", "site");
			required.Add("/asmv1:assembly/trustInfo/security/applicationRequestMinimum/PermissionSet[@Unrestricted]", "true");
			allowed.Add("/asmv1:assembly/compatibility/application", String.Empty);

			string description;
			if (ValidationHelper.CheckProperties(
				properties,
				required,
				allowed,
				out description))
				return;

			RaiseError.WrongManifestContents(description);
		}

		#endregion

		#region Checking project references

		/// <summary>
		/// Checks "WrongReferences" condition.
		/// </summary>
		public static void CheckWrongReferences()
		{
			StringBuilder message = new StringBuilder();
			List<Reference> references = ProjectHelper.GetAllReferences();

			foreach (Reference reference in references)
			{
				CheckDirectlySpecifiedProperties(reference, message);

				if (reference.Version != null && reference.SpecificVersion != "False")
				{
					message.AppendLine(
						Strings.DontUseSpecificVersion
						.Display(reference.Name));
				}
			}

			if (message.Length == 0)
				return;

			RaiseError.WrongReferences(message.ToString());
		}

		/// <summary>
		/// Checks properties that should not be specified directly.
		/// </summary>
		private static void CheckDirectlySpecifiedProperties(Reference reference, StringBuilder message)
		{
			if (reference.Aliases != null)
			{
				message.AppendLine(
					Strings.DontSpecifyPropertyDirectly
					.Display(reference.Name, "Aliases"));
			}

			if (reference.Private != null)
			{
				message.AppendLine(
					Strings.DontSpecifyPropertyDirectly
					.Display(reference.Name, "Copy Local"));
			}

			if (reference.EmbedInteropTypes != null)
			{
				message.AppendLine(
					Strings.DontSpecifyPropertyDirectly
					.Display(reference.Name, "Embed Interop Types"));
			}
		}

		#endregion
	}
}
