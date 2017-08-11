namespace GrimoireTD.Attributes
{
    public interface IAttributeModifier
    {
        float Magnitude { get; }

        string AsPercentString { get; }
    }
}