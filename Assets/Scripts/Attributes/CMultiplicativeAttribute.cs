namespace GrimoireTD.Attributes
{
    public class CMultiplicativeAttribute : CAttribute
    {
        public CMultiplicativeAttribute(string displayName) : base(displayName) { }

        public override float Value()
        {
            float multiplier = 1;

            foreach (IAttributeModifier modifier in modifiers)
            {
                multiplier = multiplier * (1 + modifier.Magnitude);
            }

            return multiplier - 1;
        }
    }
}