namespace Microsoft.StyleCop
{
    using Microsoft.Win32;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Windows.Forms;

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Utils", Justification="API has already been published and should not be changed.")]
    public class RegistryUtils
    {
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification="False positive")]
        private const string ApplicationAcronym = "StyleCop";
        private RegistryKey curoot;

        internal RegistryUtils()
        {
            Permissions.Demand();
            string name = @"Software\Microsoft\StyleCop";
            this.curoot = Registry.CurrentUser.OpenSubKey(name, true);
            if (this.curoot == null)
            {
                this.curoot = Registry.CurrentUser.CreateSubKey(name);
            }
        }

        private static RegistryKey AddKey(RegistryKey root, string name)
        {
            RegistryKey key = null;
            try
            {
                PathInfo info = CreatePath(root, name);
                if (info.Key == null)
                {
                    return null;
                }
                key = info.Key.CreateSubKey(info.Stub);
            }
            catch (SecurityException)
            {
            }
            catch (ArgumentException)
            {
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            return key;
        }

        private static PathInfo CreatePath(RegistryKey root, string name)
        {
            PathInfo info = new PathInfo();
            try
            {
                info.Key = root;
                string str = name;
                string subkey = null;
                while (true)
                {
                    int index = str.IndexOf(@"\", StringComparison.Ordinal);
                    if (-1 == index)
                    {
                        info.Stub = str;
                        return info;
                    }
                    subkey = str.Substring(0, index);
                    str = str.Substring(index + 1, (str.Length - index) - 1);
                    info.Key = info.Key.CreateSubKey(subkey);
                }
            }
            catch (ArgumentException)
            {
            }
            catch (SecurityException)
            {
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            info.Key = null;
            return info;
        }

        public RegistryKey CUAddKey(string name)
        {
            Param.RequireValidString(name, "name");
            return AddKey(this.curoot, name);
        }

        public void CUDeleteKey(string name)
        {
            Param.RequireValidString(name, "name");
            DeleteKey(this.curoot, name);
        }

        public void CUDeleteValue(string name)
        {
            Param.RequireValidString(name, "name");
            DeleteValue(this.curoot, name);
        }

        public object CUGetValue(string name)
        {
            Param.RequireValidString(name, "name");
            return GetValue(this.curoot, name);
        }

        public RegistryKey CUOpenKey(string name)
        {
            Param.RequireValidString(name, "name");
            return OpenKey(this.curoot, name);
        }

        public bool CUSetValue(string name, object value)
        {
            Param.RequireValidString(name, "name");
            Param.RequireNotNull(value, "value");
            return SetValue(this.curoot, name, value);
        }

        private static void DeleteKey(RegistryKey root, string name)
        {
            try
            {
                int num = name.LastIndexOf(@"\", StringComparison.Ordinal);
                if (-1 == num)
                {
                    root.DeleteSubKeyTree(name);
                }
                else
                {
                    string subkey = name.Substring(num + 1, (name.Length - num) - 1);
                    PathInfo path = GetPath(root, name);
                    if (path.Key != null)
                    {
                        path.Key.DeleteSubKeyTree(subkey);
                    }
                }
            }
            catch (ArgumentException)
            {
            }
            catch (SecurityException)
            {
            }
        }

        private static void DeleteValue(RegistryKey root, string name)
        {
            try
            {
                PathInfo path = GetPath(root, name);
                if (path.Key != null)
                {
                    path.Key.DeleteValue(path.Stub);
                }
            }
            catch (ArgumentException)
            {
            }
            catch (SecurityException)
            {
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        ~RegistryUtils()
        {
            if (this.curoot != null)
            {
                this.curoot.Close();
            }
        }

        private static PathInfo GetPath(RegistryKey root, string name)
        {
            PathInfo info = new PathInfo();
            try
            {
                info.Key = root;
                string str = name;
                string str2 = null;
                while (true)
                {
                    int index = str.IndexOf(@"\", StringComparison.Ordinal);
                    if (-1 == index)
                    {
                        info.Stub = str;
                        return info;
                    }
                    str2 = str.Substring(0, index);
                    str = str.Substring(index + 1, (str.Length - index) - 1);
                    info.Key = info.Key.OpenSubKey(str2, true);
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
            info.Key = null;
            return info;
        }

        private static object GetValue(RegistryKey root, string name)
        {
            object obj2 = null;
            try
            {
                PathInfo path = GetPath(root, name);
                if (path.Key == null)
                {
                    return null;
                }
                obj2 = path.Key.GetValue(path.Stub);
            }
            catch (SecurityException)
            {
            }
            catch (IOException)
            {
            }
            catch (ArgumentException)
            {
            }
            return obj2;
        }

        private static RegistryKey OpenKey(RegistryKey root, string name)
        {
            RegistryKey key = null;
            try
            {
                PathInfo info = CreatePath(root, name);
                if (info.Key == null)
                {
                    return null;
                }
                key = info.Key.OpenSubKey(info.Stub);
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
            return key;
        }

        public bool RestoreWindowPosition(string name, Form form)
        {
            Param.RequireValidString(name, "name");
            Param.RequireNotNull(form, "form");
            return this.RestoreWindowPosition(name, form, null, null);
        }

        public bool RestoreWindowPosition(string name, Form form, object location, object size)
        {
            Param.RequireValidString(name, "name");
            Param.RequireNotNull(form, "form");
            bool flag = false;
            object obj2 = this.CUGetValue(@"WindowLocation\" + name);
            if (obj2 == null)
            {
                if (location != null)
                {
                    form.Location = (Point) location;
                }
                if (size != null)
                {
                    form.Size = (Size) size;
                }
            }
            else
            {
                WindowLocation location2 = new WindowLocation((string) obj2);
                form.Location = location2.Location;
                form.Size = location2.Size;
                form.WindowState = location2.State;
                form.StartPosition = FormStartPosition.Manual;
                flag = true;
            }
            if (form.WindowState == FormWindowState.Normal)
            {
                Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
                if (form.Width > workingArea.Width)
                {
                    form.Width = workingArea.Width;
                }
                if (form.Height > workingArea.Height)
                {
                    form.Height = workingArea.Height;
                }
                int x = form.Location.X;
                if (x < 0)
                {
                    x = 0;
                }
                if ((x + form.Width) > workingArea.Right)
                {
                    x = workingArea.Right - form.Width;
                }
                int y = form.Location.Y;
                if (y < 0)
                {
                    y = 0;
                }
                if ((y + form.Height) > workingArea.Bottom)
                {
                    y = workingArea.Bottom - form.Height;
                }
                form.Location = new Point(x, y);
            }
            return flag;
        }

        public bool SaveWindowPosition(string name, Point location, Size size, FormWindowState state)
        {
            Param.RequireValidString(name, "name");
            WindowLocation location2 = new WindowLocation();
            location2.Location = location;
            location2.Size = size;
            location2.State = state;
            return this.CUSetValue(@"WindowLocation\" + name, location2);
        }

        public bool SaveWindowPositionByForm(string name, Form form)
        {
            Param.RequireValidString(name, "name");
            Param.RequireNotNull(form, "form");
            if (form.WindowState == FormWindowState.Normal)
            {
                return this.SaveWindowPosition(name, form.Location, form.Size, form.WindowState);
            }
            return true;
        }

        private static bool SetValue(RegistryKey root, string name, object value)
        {
            bool flag = false;
            try
            {
                PathInfo info = CreatePath(root, name);
                if (info.Key == null)
                {
                    return false;
                }
                info.Key.SetValue(info.Stub, value);
                flag = true;
            }
            catch (ArgumentException)
            {
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (SecurityException)
            {
            }
            return flag;
        }

        public RegistryKey CURoot
        {
            get
            {
                return this.curoot;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PathInfo
        {
            public RegistryKey Key;
            public string Stub;
        }

        internal sealed class Permissions
        {
            private Permissions()
            {
            }

            public static void Demand()
            {
                string pathList = @"HKEY_CURRENT_USER\Software\Microsoft\StyleCop";
                new RegistryPermission(RegistryPermissionAccess.AllAccess, pathList).Demand();
            }
        }

        private class WindowLocation
        {
            private Point location;
            private System.Drawing.Size size;
            private FormWindowState state;

            public WindowLocation()
            {
            }

            public WindowLocation(string input)
            {
                string[] strArray = input.Split(new char[] { ',' });
                if (strArray.Length < 5)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.InvalidWindowLocationInputString, new object[] { input }));
                }
                this.location.X = Convert.ToInt32(strArray[0], (IFormatProvider) null);
                this.location.Y = Convert.ToInt32(strArray[1], (IFormatProvider) null);
                this.size.Height = Convert.ToInt32(strArray[2], (IFormatProvider) null);
                this.size.Width = Convert.ToInt32(strArray[3], (IFormatProvider) null);
                int num = Convert.ToInt32(strArray[4], (IFormatProvider) null);
                if (num < 0)
                {
                    this.state = FormWindowState.Minimized;
                }
                else if (num > 0)
                {
                    this.state = FormWindowState.Maximized;
                }
                else if (num == 0)
                {
                    this.state = FormWindowState.Normal;
                }
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat(CultureInfo.CurrentUICulture, "{0},{1},{2},{3},", new object[] { this.location.X, this.location.Y, this.size.Height, this.size.Width });
                if (this.state == FormWindowState.Maximized)
                {
                    builder.Append("1");
                }
                else
                {
                    builder.Append("0");
                }
                return builder.ToString();
            }

            public Point Location
            {
                get
                {
                    return this.location;
                }
                set
                {
                    this.location = value;
                }
            }

            public System.Drawing.Size Size
            {
                get
                {
                    return this.size;
                }
                set
                {
                    this.size = value;
                }
            }

            public FormWindowState State
            {
                get
                {
                    return this.state;
                }
                set
                {
                    this.state = value;
                }
            }
        }
    }
}

