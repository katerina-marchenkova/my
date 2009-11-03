namespace Microsoft.StyleCop
{
    using System;
    using System.Diagnostics;
    using System.IO;

    internal class Log : IDisposable
    {
        private Listener listener;
        private StyleCopLogLevel logLevel;

        public Log(StyleCopCore core)
        {
            object obj2 = core.Registry.CUGetValue("Logging");
            if (obj2 != null)
            {
                try
                {
                    int num = (int) obj2;
                    if (num > 0)
                    {
                        this.logLevel = StyleCopLogLevel.High;
                    }
                }
                catch (FormatException)
                {
                }
            }
            if (this.logLevel != StyleCopLogLevel.None)
            {
                this.listener = new Listener();
                Trace.Listeners.Add(this.listener);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (this.listener != null)
            {
                Trace.Listeners.Remove(this.listener);
                this.listener.Dispose();
                this.listener = null;
            }
        }

        public static void Write(string text, params string[] stringParameters)
        {
            Trace.TraceInformation(text, stringParameters);
        }

        private class Listener : TraceListener
        {
            private StreamWriter writer;

            public Listener()
            {
                this.OpenLogFile();
            }

            private void CloseLogFile()
            {
                if (this.writer != null)
                {
                    this.writer.Close();
                    this.writer.Dispose();
                    this.writer = null;
                }
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (disposing)
                {
                    this.CloseLogFile();
                }
            }

            private void OpenLogFile()
            {
                string path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft"), "StyleCop");
                try
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string str2 = Path.Combine(path, "StyleCopLog.log");
                    if (File.Exists(str2))
                    {
                        File.SetAttributes(str2, FileAttributes.Normal);
                        File.Delete(str2);
                    }
                    this.writer = new StreamWriter(str2, false);
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }

            public override void Write(string message)
            {
                if ((message != null) && (this.writer != null))
                {
                    this.writer.Write(message);
                    this.writer.Flush();
                }
            }

            public override void WriteLine(string message)
            {
                if ((message != null) && (this.writer != null))
                {
                    this.writer.WriteLine(message);
                    this.writer.Flush();
                }
            }
        }
    }
}

