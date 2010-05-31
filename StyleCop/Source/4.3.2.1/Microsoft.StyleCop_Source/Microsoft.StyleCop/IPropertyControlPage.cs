namespace Microsoft.StyleCop
{
    using System;

    public interface IPropertyControlPage
    {
        void Activate(bool activated);
        bool Apply();
        void Initialize(PropertyControl propertyControl);
        void PostApply(bool wasDirty);
        bool PreApply();
        void RefreshSettingsOverrideState();

        bool Dirty { get; set; }

        string TabName { get; }
    }
}

