namespace Microsoft.StyleCop
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security;

    public class CodeFile : SourceCode
    {
        private string fileType;
        private string folder;
        private string name;
        private string path;

        public CodeFile(string path, CodeProject project, SourceParser parser) : this(path, project, parser, null)
        {
        }

        public CodeFile(string path, CodeProject project, SourceParser parser, IEnumerable<Configuration> configurations) : base(project, parser, configurations)
        {
            Param.RequireNotNull(path, "path");
            Param.RequireNotNull(project, "project");
            Param.RequireNotNull(parser, "parser");
            this.path = path;
            if ((!path.StartsWith(@"\\", StringComparison.Ordinal) && (path.Length >= 2)) && (path[1] != ':'))
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                if (currentDirectory.EndsWith(@"\", StringComparison.Ordinal))
                {
                    currentDirectory = currentDirectory.Substring(0, currentDirectory.Length - 1);
                }
                if (path.StartsWith(@"\", StringComparison.Ordinal))
                {
                    path = currentDirectory.Substring(0, 2) + path;
                }
                else
                {
                    path = currentDirectory + @"\" + path;
                }
            }
            int length = path.LastIndexOf(@"\", StringComparison.Ordinal);
            if (-1 == length)
            {
                this.name = this.path;
            }
            else
            {
                this.name = path.Substring(length + 1, (path.Length - length) - 1);
                this.folder = path.Substring(0, length);
                if (this.folder != null)
                {
                    this.folder = StyleCopCore.CleanPath(this.folder);
                }
            }
            length = this.name.LastIndexOf(".", StringComparison.Ordinal);
            if (-1 == length)
            {
                this.fileType = string.Empty;
            }
            else
            {
                this.fileType = this.name.Substring(length + 1, (this.name.Length - length) - 1).ToUpperInvariant();
            }
        }

        public override TextReader Read()
        {
            if (this.Exists)
            {
                try
                {
                    return new StreamReader(this.path, true);
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (IOException)
                {
                }
            }
            return null;
        }

        public override bool Exists
        {
            get
            {
                return (!string.IsNullOrEmpty(this.path) && File.Exists(this.path));
            }
        }

        public string Folder
        {
            get
            {
                return this.folder;
            }
        }

        public string FullPathName
        {
            get
            {
                char[] chArray = this.name.ToCharArray();
                for (int i = 0; i < chArray.Length; i++)
                {
                    if (((chArray[i] == '\\') || (chArray[i] == '.')) || (chArray[i] == ':'))
                    {
                        chArray[i] = '_';
                    }
                }
                return new string(chArray);
            }
        }

        public override string Name
        {
            get
            {
                return this.name;
            }
        }

        public override string Path
        {
            get
            {
                return this.path;
            }
        }

        public override DateTime TimeStamp
        {
            get
            {
                try
                {
                    if (this.Exists)
                    {
                        return File.GetLastWriteTime(this.path);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (SecurityException)
                {
                }
                catch (IOException)
                {
                }
                return new DateTime();
            }
        }

        public override string Type
        {
            get
            {
                return this.fileType;
            }
        }
    }
}

