namespace Microsoft.StyleCop
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows.Forms;

    public static class AlertDialog
    {
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification="The default MessageBoxOptions are acceptable.")]
        private static DialogResult DisplayMessageBox(Control parent, string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            Control owner = parent;
            while (owner != null)
            {
                if (owner.RightToLeft == RightToLeft.Inherit)
                {
                    owner = owner.Parent;
                }
                else
                {
                    if (owner.RightToLeft != RightToLeft.Yes)
                    {
                        break;
                    }
                    return MessageBox.Show(owner, message, title, buttons, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                }
            }
            return MessageBox.Show(parent, message, title, buttons, icon);
        }

        private static void SendToOutput(StyleCopCore core, string message, MessageBoxIcon icon)
        {
            string format = "{0}";
            if (((icon & MessageBoxIcon.Hand) != MessageBoxIcon.None) || ((icon & MessageBoxIcon.Hand) != MessageBoxIcon.None))
            {
                format = Strings.ErrorTag;
            }
            else if ((icon & MessageBoxIcon.Exclamation) != MessageBoxIcon.None)
            {
                format = Strings.WarningTag;
            }
            core.SignalOutput(string.Format(CultureInfo.CurrentCulture, format, new object[] { message }));
        }

        public static DialogResult Show(StyleCopCore core, Control parent, string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            Param.RequireNotNull(core, "core");
            Param.RequireValidString(message, "message");
            Param.RequireValidString(title, "title");
            if (core.DisplayUI)
            {
                return DisplayMessageBox(parent, message, title, buttons, icon);
            }
            if (buttons != MessageBoxButtons.OK)
            {
                throw new InvalidOperationException(Strings.AlertDialogWithOptionsInNonUIState);
            }
            SendToOutput(core, message, icon);
            return DialogResult.OK;
        }
    }
}

