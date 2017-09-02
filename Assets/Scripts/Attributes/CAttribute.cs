using System;
using System.Collections.Generic;

namespace GrimoireTD.Attributes
{
    public abstract class CAttribute : IAttribute
    {
        public string DisplayName { get; }

        protected List<IAttributeModifier> modifiers;

        public event EventHandler<EAOnAttributeChanged> OnAttributeChanged;

        public CAttribute(string displayName)
        {
            modifiers = new List<IAttributeModifier>();
            DisplayName = displayName;
        }

        public void AddModifier(IAttributeModifier modifier)
        {
            modifiers.Add(modifier);

            OnAttributeChanged?.Invoke(this, new EAOnAttributeChanged(Value()));
        }

        public bool TryRemoveModifier(IAttributeModifier modifier)
        {
            if (!modifiers.Contains(modifier))
            {
                return false;
            }

            modifiers.Remove(modifier);

            OnAttributeChanged?.Invoke(this, new EAOnAttributeChanged(Value()));

            return true;
        }

        public abstract float Value();
    }
}