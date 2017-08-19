namespace GrimoireTD.Attributes
{
    public interface IAttribute
    {
        string DisplayName { get; }

        float Value();

        void AddModifier(IAttributeModifier modifier);

        bool TryRemoveModifier(IAttributeModifier modifier);
    }
}