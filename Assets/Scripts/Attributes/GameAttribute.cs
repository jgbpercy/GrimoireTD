using System.Collections.Generic;

namespace GrimoireTD.Attributes
{
    public abstract class GameAttribute
    {
        private string displayName;

        protected List<IAttributeModifier> modifiers;

        public string DisplayName
        {
            get
            {
                return displayName;
            }
        }

        public GameAttribute(string displayName)
        {
            modifiers = new List<IAttributeModifier>();
            this.displayName = displayName;
        }

        public void AddModifier(IAttributeModifier modifier)
        {
            modifiers.Add(modifier);
        }

        public bool TryRemoveModifier(IAttributeModifier modifier)
        {
            if (!modifiers.Contains(modifier))
            {
                return false;
            }

            modifiers.Remove(modifier);
            return true;
        }

        public abstract float Value();
    }
}