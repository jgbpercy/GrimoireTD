namespace GrimoireTD.Attributes
{
    public class CAdditiveAttribute : CAttribute
    {
        public CAdditiveAttribute(string displayName) : base(displayName) { }

        public override float Value()
        {
            float value = 0;

            foreach (IAttributeModifier modifier in modifiers)
            {
                value += modifier.Magnitude;
            }

            return value;
        }
    }
}
