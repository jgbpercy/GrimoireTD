using System;

namespace GrimoireTD.Attributes
{
    public interface IReadOnlyAttribute
    {
        string DisplayName { get; }

        event EventHandler<EAOnAttributeChanged> OnAttributeChanged;

        float Value();
    }
}