using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
				@"ProjectName=VortexCommander",
				@"ReferencesDirectory=\\rufrt-vxbuild\e$\CCNET\VortexCommander\References",
				@"WorkingDirectorySource=\\rufrt-vxbuild\e$\CCNET\VortexCommander\WorkingDirectory\Source",
				@"ExternalReferencesPath=\\rufrt-vxbuild\ExternalReferences",
				@"InternalReferencesPath=\\rufrt-vxbuild\InternalReferences",
				@"ProjectType=ClickOnce",
				@"AssemblyName=VortexCommander",
				@"FriendlyName=Vortex Commander",
				@"DownloadZone=Public",
				@"VisualStudioVersion=2010",
				@"TargetFramework=Net20",
				@"TargetPlatform=AnyCPU",
				@"RootNamespace=VortexCommander",
				@"SuppressWarnings=",
				@"ExpectedVersion=1.0.0.0"
			};*/

			/*xxxargs = new[]
			{
				@"ProjectName=VXStudioWindowsControls",
				@"ReferencesDirectory=C:\Users\Public\VSS\SED\TFS\VXStudio\VXStudioWindowsControls-Refs",
				@"WorkingDirectorySource=C:\Users\Public\VSS\SED\TFS\VXStudio\VXStudioWindowsControls",
				@"ExternalReferencesPath=\\rufrt-vxbuild\ExternalReferences",
				@"InternalReferencesPath=\\rufrt-vxbuild\InternalReferences",
				@"ProjectType=Library",
				@"AssemblyName=VXStudioWindowsControls",
				@"VisualStudioVersion=2008",
				@"TargetFramework=Net20",
				@"TargetPlatform=AnyCPU",
				@"RootNamespace=VX.Studio.WindowsControls",
				@"SuppressWarnings=1591",
				@"ExpectedVersion=1.0.0.0"
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
				return ErrorHandler.Runtime(e);
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
			CheckWrongAssemblyInfoFileLocation();
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
			CheckWrongAssemblyInfoContents();

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

		/// <summary>
		/// Checks "WrongAssemblyInfoFileLocation" condition.
		/// </summary>
		private static void CheckWrongAssemblyInfoFileLocation()
		{
			string[] files = Directory.GetFiles(Arguments.WorkingDirectorySource, "AssemblyInfo.cs", SearchOption.AllDirectories);
			if (files.Length == 1)
			{
				if (files[0] == Paths.AssemblyInfoFile)
					return;
			}

			RaiseError.WrongAssemblyInfoFileLocation();
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

			allowed.Add("AppDesignerFolder", "Properties");

			switch (Arguments.ProjectType)
			{
				case ProjectType.ClickOnce:
					required.Add("ApplicationIcon", null);
					break;
				default:
					allowed.Add("ApplicationIcon", String.Empty);
					break;
			}

			required.Add("AssemblyName", Arguments.AssemblyName);
			allowed.Add("AssemblyKeyContainerName", String.Empty);
			allowed.Add("AssemblyOriginatorKeyFile", String.Empty);
			allowed.Add("CodeContractsAssemblyMode", null);
			allowed.Add("Configuration", null);
			allowed.Add("DefaultClientScript", null);
			allowed.Add("DefaultHTMLPageLayout", null);
			allowed.Add("DefaultTargetSchema", null);
			allowed.Add("DelaySign", "false");
			allowed.Add("FileAlignment", "512");
			allowed.Add("FileUpgradeFlags", null);
			allowed.Add("GenerateResourceNeverLockTypeAssemblies", "true");
			allowed.Add("MvcBuildViews", null);
			allowed.Add("OldToolsVersion", null);

			switch (Arguments.ProjectType)
			{
				case ProjectType.ClickOnce:
					required.Add("OutputType", "WinExe");
					break;
				case ProjectType.Console:
					required.Add("OutputType", "Exe");
					break;
				case ProjectType.WebSite:
				case ProjectType.Library:
					required.Add("OutputType", "Library");
					break;
			}

			allowed.Add("Platform", null);
			allowed.Add("PostBuildEvent", String.Empty);
			allowed.Add("PreBuildEvent", String.Empty);
			allowed.Add("ProductVersion", null);
			allowed.Add("ProjectGuid", null);
			allowed.Add("ProjectType", "Local");
			allowed.Add("ProjectTypeGuids", null);
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
					required.Add("GenerateManifests", "false");
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
					allowed.Add("GenerateManifests", "false");
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

			allowed.Add("AllowUnsafeBlocks", "false");
			allowed.Add("BaseAddress", "285212672");
			allowed.Add("CheckForOverflowUnderflow", "false");
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
			allowed.Add("ConfigurationOverrideFile", String.Empty);
			required.Add("DebugSymbols", "true");
			required.Add("DebugType", "full");
			required.Add("DefineConstants", "DEBUG;TRACE");
			required.Add("ErrorReport", "prompt");
			allowed.Add("FileAlignment", "512");
			allowed.Add("FxCopRules", null);
			allowed.Add("NoStdLib", "false");
			allowed.Add("NoWarn", Arguments.SuppressWarnings);
			required.Add("Optimize", "false");

			switch (Arguments.ProjectType)
			{
				case ProjectType.WebSite:
					required.Add("OutputPath", @"bin\");
					required.Add("DocumentationFile", @"bin\{0}.xml".Display(Arguments.AssemblyName));
					break;
				default:
					required.Add("OutputPath", @"bin\Debug\");
					required.Add("DocumentationFile", @"bin\Debug\{0}.xml".Display(Arguments.AssemblyName));
					break;
			}

			allowed.Add("PlatformTarget", Arguments.TargetPlatform);
			allowed.Add("RegisterForComInterop", "false");
			allowed.Add("RemoveIntegerChecks", "false");
			allowed.Add("TreatWarningsAsErrors", "false");
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

			allowed.Add("AllowUnsafeBlocks", "false");
			allowed.Add("BaseAddress", "285212672");
			allowed.Add("CheckForOverflowUnderflow", "false");
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
			allowed.Add("ConfigurationOverrideFile", String.Empty);
			allowed.Add("DebugSymbols", "true");
			required.Add("DebugType", "pdbonly");
			required.Add("DefineConstants", "TRACE");
			required.Add("ErrorReport", "prompt");
			allowed.Add("FileAlignment", "512");
			allowed.Add("FxCopRules", null);
			allowed.Add("NoStdLib", "false");
			allowed.Add("NoWarn", Arguments.SuppressWarnings);
			required.Add("Optimize", "true");

			switch (Arguments.ProjectType)
			{
				case ProjectType.WebSite:
					required.Add("OutputPath", @"bin\");
					required.Add("DocumentationFile", @"bin\{0}.xml".Display(Arguments.AssemblyName));
					break;
				default:
					required.Add("OutputPath", @"bin\Release\");
					required.Add("DocumentationFile", @"bin\Release\{0}.xml".Display(Arguments.AssemblyName));
					break;
			}

			allowed.Add("PlatformTarget", Arguments.TargetPlatform);
			allowed.Add("RegisterForComInterop", "false");
			allowed.Add("RemoveIntegerChecks", "false");
			allowed.Add("TreatWarningsAsErrors", "false");
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

		/// <summary>
		/// Checks "WrongAssemblyInfoContents" condition.
		/// </summary>
		public static void CheckWrongAssemblyInfoContents()
		{
			string[] lines = File.ReadAllLines(Paths.AssemblyInfoFile);

			Dictionary<string, string> properties = PropertiesHelper.ParseFromAssemblyInfo(lines);
			Dictionary<string, string> required = new Dictionary<string, string>();
			Dictionary<string, string> allowed = new Dictionary<string, string>();

			if (Arguments.ProjectType == ProjectType.ClickOnce)
			{
				required.Add("AssemblyTitle", Arguments.FriendlyName);
				required.Add("AssemblyProduct", Arguments.FriendlyName);
			}
			else
			{
				required.Add("AssemblyTitle", Arguments.AssemblyName);
				required.Add("AssemblyProduct", Arguments.AssemblyName);
			}

			required.Add("AssemblyDescription", String.Empty);
			required.Add("AssemblyConfiguration", String.Empty);
			required.Add("AssemblyCompany", "CNET Content Solutions");
			required.Add("AssemblyCopyright", "Copyright © CNET Content Solutions 2010");
			required.Add("AssemblyTrademark", String.Empty);
			required.Add("AssemblyCulture", String.Empty);
			required.Add("AssemblyVersion", Arguments.ExpectedVersion);

			allowed.Add("NeutralResourcesLanguage", "en");
			allowed.Add("ComVisible", "false");
			allowed.Add("Guid", null);

			string description;
			if (ValidationHelper.CheckProperties(
				properties,
				required,
				allowed,
				out description))
				return;

			RaiseError.WrongAssemblyInfoContents(description);
		}

		#endregion

		#region Checking project references

		/// <summary>
		/// Checks "WrongReferences" condition.
		/// </summary>
		public static void CheckWrongReferences()
		{
			StringBuilder message = new StringBuilder();
			List<string> projects = ProjectHelper.GetProjectReferences();
			foreach (string project in projects)
			{
				message.AppendLine(
					Strings.DontUseProjectReference
					.Display(project));
			}

			List<Reference> references = ProjectHelper.GetBinaryReferences();
			CheckReferenceProperties(references, message);

			List<ReferenceFile> allExternals = ReferenceFolder.GetAllFiles(Arguments.ExternalReferencesPath);
			List<ReferenceFile> allInternals = ReferenceFolder.GetAllFiles(Arguments.InternalReferencesPath);
			List<ReferenceFile> usedExternals = new List<ReferenceFile>();
			List<ReferenceFile> usedInternals = new List<ReferenceFile>();
			List<Reference> usedGac = new List<Reference>();

			foreach (Reference reference in references)
			{
				ReferenceFile usedExternal = allExternals.Where(file => file.AssemblyName == reference.Name).FirstOrDefault();
				ReferenceFile usedInternal = allInternals.Where(file => file.AssemblyName == reference.Name).FirstOrDefault();

				if (usedExternal != null)
				{
					usedExternals.Add(usedExternal);
				}
				else if (usedInternal != null)
				{
					usedInternals.Add(usedInternal);
				}
				else
				{
					usedGac.Add(reference);
				}
			}

			ReferenceMark.SetupActual(
				ReferenceType.External,
				Arguments.ReferencesDirectory,
				usedExternals.Select(item => item.ProjectName).Distinct());

			ReferenceMark.SetupActual(
				ReferenceType.Internal,
				Arguments.ReferencesDirectory,
				usedInternals.Select(item => item.ProjectName).Distinct());

			List<string> requiredGac = new List<string>();
			List<string> allowedGac = new List<string>();
			allowedGac.Add("Microsoft.CSharp");
			allowedGac.Add("Microsoft.mshtml");
			allowedGac.Add("System");
			allowedGac.Add("System.Core");
			allowedGac.Add("System.ComponentModel.DataAnnotations");
			allowedGac.Add("System.Configuration");
			allowedGac.Add("System.configuration");
			allowedGac.Add("System.Data");
			allowedGac.Add("System.Data.DataSetExtensions");
			allowedGac.Add("System.Deployment");
			allowedGac.Add("System.Design");
			allowedGac.Add("System.Drawing");
			allowedGac.Add("System.Web");
			allowedGac.Add("System.Web.Abstractions");
			allowedGac.Add("System.Web.ApplicationServices");
			allowedGac.Add("System.Web.DynamicData");
			allowedGac.Add("System.EnterpriseServices");
			allowedGac.Add("System.Web.Entity");
			allowedGac.Add("System.Web.Extensions");
			allowedGac.Add("System.Web.Routing");
			allowedGac.Add("System.Web.Services");
			allowedGac.Add("System.Windows.Forms");
			allowedGac.Add("System.XML");
			allowedGac.Add("System.Xml");
			allowedGac.Add("System.Xml.Linq");

			string entriesDescription;
			if (!ValidationHelper.CheckEntries(
				usedGac.Select(reference => reference.Name).ToList(),
				requiredGac,
				allowedGac,
				out entriesDescription))
			{
				message.Append(entriesDescription);
			}

			if (message.Length == 0)
				return;

			RaiseError.WrongReferences(message.ToString());
		}

		/// <summary>
		/// Checks properties that should not be specified directly.
		/// </summary>
		private static void CheckReferenceProperties(IEnumerable<Reference> references, StringBuilder message)
		{
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
