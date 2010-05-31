namespace Microsoft.StyleCop
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Xml;

    public class FileBasedEnvironment : StyleCopEnvironment
    {
        private string defaultSettingsFilePath;
        private Dictionary<string, List<SourceParser>> fileTypes = new Dictionary<string, List<SourceParser>>();

        public override void AddParser(SourceParser parser)
        {
            Param.RequireNotNull(parser, "parser");
            ICollection<string> fileTypes = parser.FileTypes;
            if (fileTypes != null)
            {
                foreach (string str in fileTypes)
                {
                    if (str != null)
                    {
                        List<SourceParser> list = null;
                        if (!this.fileTypes.TryGetValue(str, out list))
                        {
                            list = new List<SourceParser>(1);
                            this.fileTypes.Add(str, list);
                        }
                        list.Add(parser);
                    }
                }
            }
        }

        public override bool AddSourceCode(CodeProject project, string path, object context)
        {
            Param.RequireNotNull(project, "project");
            Param.RequireValidString(path, "path");
            bool flag = false;
            string extension = Path.GetExtension(path);
            if ((extension != null) && (extension.Length > 0))
            {
                extension = extension.Substring(1).ToUpperInvariant();
                ICollection<SourceParser> parsersForFileType = this.GetParsersForFileType(extension);
                if (parsersForFileType == null)
                {
                    return flag;
                }
                foreach (SourceParser parser in parsersForFileType)
                {
                    SourceCode sourceCode = this.CreateCodeFile(path, project, parser, context);
                    project.AddSourceCode(sourceCode);
                    flag = true;
                }
            }
            return flag;
        }

        protected virtual CodeFile CreateCodeFile(string path, CodeProject project, SourceParser parser, object context)
        {
            return new CodeFile(path, project, parser);
        }

        private WritableSettings CreateSettingsDocument(string path, out Exception exception)
        {
            exception = null;
            try
            {
                XmlDocument contents = WritableSettings.NewDocument();
                contents.Save(path);
                return new WritableSettings(base.Core, path, Path.GetDirectoryName(path), contents, File.GetLastWriteTime(path));
            }
            catch (ArgumentException exception2)
            {
                exception = exception2;
            }
            catch (IOException exception3)
            {
                exception = exception3;
            }
            catch (SecurityException exception4)
            {
                exception = exception4;
            }
            catch (UnauthorizedAccessException exception5)
            {
                exception = exception5;
            }
            catch (XmlException exception6)
            {
                exception = exception6;
            }
            return null;
        }

        public override string GetDefaultSettingsPath()
        {
            if (this.defaultSettingsFilePath == null)
            {
                this.defaultSettingsFilePath = string.Empty;
                string location = Assembly.GetExecutingAssembly().Location;
                if (!string.IsNullOrEmpty(location))
                {
                    string directoryName = Path.GetDirectoryName(location);
                    if (!string.IsNullOrEmpty(directoryName) && Directory.Exists(directoryName))
                    {
                        string path = Path.Combine(directoryName, "Settings.StyleCop");
                        if (File.Exists(path))
                        {
                            this.defaultSettingsFilePath = path;
                        }
                        else
                        {
                            path = Path.Combine(directoryName, "Settings.SourceAnalysis");
                            if (File.Exists(path))
                            {
                                this.defaultSettingsFilePath = path;
                            }
                        }
                    }
                }
            }
            return this.defaultSettingsFilePath;
        }

        public override string GetParentSettingsPath(string settingsPath)
        {
            Param.RequireValidString(settingsPath, "settingsPath");
            string fullName = settingsPath;
            while (!string.IsNullOrEmpty(fullName))
            {
                DirectoryInfo parent = Directory.GetParent(fullName);
                if (parent == null)
                {
                    break;
                }
                fullName = parent.FullName;
                string path = Path.Combine(fullName, "Settings.StyleCop");
                if (!File.Exists(path))
                {
                    string str3 = Path.Combine(fullName, "Settings.SourceAnalysis");
                    if (File.Exists(str3))
                    {
                        path = str3;
                    }
                    else
                    {
                        str3 = Path.Combine(fullName, "StyleCop.Settings");
                        if (File.Exists(str3))
                        {
                            path = str3;
                        }
                    }
                }
                if (File.Exists(path))
                {
                    return path;
                }
            }
            return null;
        }

        public ICollection<SourceParser> GetParsersForFileType(string fileType)
        {
            List<SourceParser> list;
            Param.RequireValidString(fileType, "fileType");
            if (this.fileTypes.TryGetValue(fileType, out list))
            {
                return list;
            }
            return null;
        }

        public override Settings GetProjectSettings(CodeProject project, bool merge, out Exception exception)
        {
            Param.RequireNotNull(project, "project");
            string path = Path.Combine(project.Location, "Settings.StyleCop");
            if (!File.Exists(path))
            {
                string str2 = Path.Combine(project.Location, "Settings.SourceAnalysis");
                if (File.Exists(str2))
                {
                    path = str2;
                }
                else
                {
                    str2 = Path.Combine(project.Location, "StyleCop.Settings");
                    if (File.Exists(str2))
                    {
                        path = str2;
                    }
                }
            }
            return this.GetSettings(path, merge, out exception);
        }

        private static string GetResultsCachePath(string location)
        {
            if (File.Exists(location))
            {
                location = Path.GetDirectoryName(location);
            }
            return Path.GetFullPath(Path.Combine(location, "StyleCop.Cache"));
        }

        public override Settings GetSettings(string settingsPath, bool merge, out Exception exception)
        {
            Param.RequireValidString(settingsPath, "settingsPath");
            Settings localSettings = this.LoadSettingsDocument(settingsPath, true, out exception);
            if (!merge)
            {
                return localSettings;
            }
            if (localSettings == null)
            {
                localSettings = new Settings(base.Core, settingsPath, Path.GetDirectoryName(settingsPath));
            }
            SettingsMerger merger = new SettingsMerger(localSettings, this);
            return merger.MergedSettings;
        }

        public override WritableSettings GetWritableSettings(string settingsPath, out Exception exception)
        {
            Param.RequireValidString(settingsPath, "settingsPath");
            WritableSettings settings = this.LoadSettingsDocument(settingsPath, false, out exception) as WritableSettings;
            if ((settings == null) && (exception is FileNotFoundException))
            {
                settings = this.CreateSettingsDocument(settingsPath, out exception);
            }
            return settings;
        }

        public override XmlDocument LoadResultsCache(string location)
        {
            Param.RequireValidString(location, "location");
            try
            {
                string resultsCachePath = GetResultsCachePath(location);
                if (File.Exists(resultsCachePath))
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(resultsCachePath);
                    return document;
                }
            }
            catch (XmlException)
            {
            }
            catch (IOException)
            {
            }
            catch (SecurityException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            return null;
        }

        private Settings LoadSettingsDocument(string settingsFilePath, bool readOnly, out Exception exception)
        {
            exception = null;
            try
            {
                if (!File.Exists(settingsFilePath))
                {
                    exception = new FileNotFoundException();
                }
                else
                {
                    XmlDocument contents = new XmlDocument();
                    contents.Load(settingsFilePath);
                    string directoryName = Path.GetDirectoryName(settingsFilePath);
                    DateTime lastWriteTime = File.GetLastWriteTime(settingsFilePath);
                    return (readOnly ? new Settings(base.Core, settingsFilePath, directoryName, contents, lastWriteTime) : new WritableSettings(base.Core, settingsFilePath, directoryName, contents, lastWriteTime));
                }
            }
            catch (IOException exception2)
            {
                exception = exception2;
            }
            catch (SecurityException exception3)
            {
                exception = exception3;
            }
            catch (UnauthorizedAccessException exception4)
            {
                exception = exception4;
            }
            catch (XmlException exception5)
            {
                exception = exception5;
            }
            return null;
        }

        public override void RemoveAnalysisResults(string location)
        {
            Param.RequireValidString(location, "location");
            try
            {
                if (File.Exists(location))
                {
                    File.SetAttributes(location, FileAttributes.Normal);
                    File.Delete(location);
                }
            }
            catch (IOException)
            {
            }
            catch (SecurityException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        public override bool SaveAnalysisResults(string location, XmlDocument analysisResults, out Exception exception)
        {
            Param.RequireValidString(location, "location");
            Param.RequireNotNull(analysisResults, "analysisResults");
            exception = null;
            try
            {
                analysisResults.Save(location);
                return true;
            }
            catch (SecurityException exception2)
            {
                exception = exception2;
            }
            catch (UnauthorizedAccessException exception3)
            {
                exception = exception3;
            }
            catch (IOException exception4)
            {
                exception = exception4;
            }
            catch (XmlException exception5)
            {
                exception = exception5;
            }
            catch (ArgumentException exception6)
            {
                exception = exception6;
            }
            return false;
        }

        public override void SaveResultsCache(string location, XmlDocument resultsCache)
        {
            Param.RequireValidString(location, "location");
            Param.RequireNotNull(resultsCache, "resultsCache");
            try
            {
                string resultsCachePath = GetResultsCachePath(location);
                try
                {
                    if (File.Exists(resultsCachePath))
                    {
                        File.SetAttributes(resultsCachePath, FileAttributes.Normal);
                        File.Delete(resultsCachePath);
                    }
                    resultsCache.Save(resultsCachePath);
                }
                catch (ArgumentException)
                {
                }
                catch (IOException)
                {
                }
                catch (SecurityException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
                try
                {
                    if (File.Exists(resultsCachePath))
                    {
                        File.SetAttributes(resultsCachePath, FileAttributes.Hidden);
                    }
                }
                catch (ArgumentException)
                {
                }
                catch (IOException)
                {
                }
                catch (SecurityException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
            catch (XmlException)
            {
            }
        }

        public override bool SaveSettings(WritableSettings settings, out Exception exception)
        {
            Param.RequireNotNull(settings, "settings");
            exception = null;
            if ((settings.Path == null) || (settings.Contents == null))
            {
                throw new InvalidOperationException(Strings.SettingsFileHasNotBeenLoaded);
            }
            XmlDocument document = settings.WriteSettingsToDocument(this);
            try
            {
                document.Save(settings.Path);
                settings.WriteTime = File.GetLastWriteTime(settings.Path);
                return true;
            }
            catch (IOException exception2)
            {
                exception = exception2;
            }
            catch (SecurityException exception3)
            {
                exception = exception3;
            }
            catch (UnauthorizedAccessException exception4)
            {
                exception = exception4;
            }
            catch (XmlException exception5)
            {
                exception = exception5;
            }
            return false;
        }

        public override bool SupportsLinkedSettings
        {
            get
            {
                return true;
            }
        }

        public override bool SupportsResultsCache
        {
            get
            {
                return true;
            }
        }
    }
}

