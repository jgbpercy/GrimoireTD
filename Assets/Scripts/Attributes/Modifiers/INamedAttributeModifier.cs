using System;

namespace GrimoireTD.Attributes
{
    public interface INamedAttributeModifier<T> : IAttributeModifier where T : struct, IConvertible
    {
        T AttributeName { get; }
    }
}