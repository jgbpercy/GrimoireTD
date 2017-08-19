using System;

namespace GrimoireTD.Attributes
{
    public interface IAttributes<T> : IReadOnlyAttributes<T> where T : struct, IConvertible
    {
        void AddModifier(INamedAttributeModifier<T> attributeModifier);

        bool TryRemoveModifier(INamedAttributeModifier<T> attributeModifier);
    }
}