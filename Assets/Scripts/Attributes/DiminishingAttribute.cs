﻿namespace GrimoireTD.Attributes
{
    public class DiminishingAttribute : GameAttribute
    {
        public DiminishingAttribute(string displayName) : base(displayName) { }

        public override float Value()
        {
            float multiplier = 1;

            foreach (IAttributeModifier modifier in modifiers)
            {
                multiplier = multiplier * (1 - modifier.Magnitude);
            }

            return 1 - multiplier;
        }
    }
}