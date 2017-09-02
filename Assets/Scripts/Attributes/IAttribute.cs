namespace GrimoireTD.Attributes
{
    public interface IAttribute : IReadOnlyAttribute
    {
        void AddModifier(IAttributeModifier modifier);

        bool TryRemoveModifier(IAttributeModifier modifier);
    }
}