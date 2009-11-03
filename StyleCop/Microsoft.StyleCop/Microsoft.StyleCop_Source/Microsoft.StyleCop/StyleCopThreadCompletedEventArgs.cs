namespace Microsoft.StyleCop
{
    using System;

    internal class StyleCopThreadCompletedEventArgs : EventArgs
    {
        private Microsoft.StyleCop.StyleCopThread.Data data;

        public StyleCopThreadCompletedEventArgs(Microsoft.StyleCop.StyleCopThread.Data data)
        {
            this.data = data;
        }

        public Microsoft.StyleCop.StyleCopThread.Data Data
        {
            get
            {
                return this.data;
            }
        }
    }
}

