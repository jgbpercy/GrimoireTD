using System.Collections;
using System.Collections.Generic;

public enum AttributeName
{
    rangeBonus,
    damageBonus,
    cooldownReduction
}

public abstract class DefendingEntity
{
    protected abstract class Attribute
    {
        private string displayName;

        protected List<AttributeModifier> modifiers;

        public string DisplayName
        {
            get
            {
                return displayName;
            }
        }

        public Attribute(string displayName)
        {
            modifiers = new List<AttributeModifier>();
            this.displayName = displayName;
        }

        public void AddModifier(AttributeModifier modifier)
        {
            modifiers.Add(modifier);
        }

        public bool TryRemoveModifier(AttributeModifier modifier)
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

    protected class MultiplicativeAttribute : Attribute
    {
        public MultiplicativeAttribute(string displayName) : base(displayName)
        {

        }

        public override float Value()
        {
            float multiplier = 1;

            foreach (AttributeModifier modifier in modifiers)
            {
                multiplier = multiplier * (1 + modifier.Magnitude);
            }

            return multiplier - 1;
        }
    }

    protected class AdditiveAttribute : Attribute
    {
        public AdditiveAttribute(string displayName) : base(displayName)
        {

        }

        public override float Value()
        {
            float value = 0;

            foreach (AttributeModifier modifier in modifiers)
            {
                value += modifier.Magnitude;
            }

            return value;
        }
    }

    protected class DiminishingAttribute : Attribute
    {
        public DiminishingAttribute(string displayName) : base(displayName)
        {

        }

        public override float Value()
        {
            float multiplier = 1;

            foreach (AttributeModifier modifier in modifiers)
            {
                multiplier = multiplier * (1 - modifier.Magnitude);
            }

            return 1 - multiplier;
        }
    }

    protected Dictionary<AttributeName, Attribute> attributes;

    protected int id;

    protected SortedList<int, Ability> abilities;

    private DefendingEntityTemplate defendingEntityTemplate;

    public virtual string Id
    {
        get
        {
            return "?-" + id;
        }
    }

    public SortedList<int, Ability> Abilities
    {
        get
        {
            return abilities;
        }
    }

    public DefendingEntityTemplate DefendingEntityTemplate
    {
        get
        {
            return defendingEntityTemplate;
        }
    }

    public DefendingEntity(DefendingEntityTemplate template)
    {
        abilities = new SortedList<int, Ability>();

        for (int i = 0; i < template.BaseAbilities.Length; i++)
        {
            abilities.Add(i, template.BaseAbilities[i].GenerateAbility());
        }

        defendingEntityTemplate = template;
    }

    public List<DefendModeAbility> DefendModeAbilities()
    {
        List<DefendModeAbility> defendModeAbilities = new List<DefendModeAbility>();

        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i] is DefendModeAbility)
            {
                defendModeAbilities.Add((DefendModeAbility)abilities[i]);
            }
        }

        return defendModeAbilities;
    }

    public List<BuildModeAbility> BuildModeAbilities()
    {
        List<BuildModeAbility> buildModeAbilities = new List<BuildModeAbility>();

        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i] is BuildModeAbility)
            {
                buildModeAbilities.Add((BuildModeAbility)abilities[i]);
            }
        }

        return buildModeAbilities;
    }

    public void AddAttributeModifier(AttributeName attribute, AttributeModifier modifier)
    {
        Attribute attributeToModify = null;

        bool hasAttribute = attributes.TryGetValue(attribute, out attributeToModify);

        if ( !hasAttribute )
        {
            throw new System.Exception("Attempted to add a modifier to attribute " + attribute + ", which " + id + " does not have.");
        }

        attributeToModify.AddModifier(modifier);
    }

    public bool TryRemoveAttributeModifier(AttributeName attribute, AttributeModifier modifier)
    {
        if (attributes[attribute].TryRemoveModifier(modifier))
        {
            return true;
        }

        return false;
    }

    public float GetAttribute(AttributeName attributeName)
    {
        return attributes[attributeName].Value();
    }

    public string TempDebugGetAttributeDisplayName(AttributeName attributeName)
    {
        return attributes[attributeName].DisplayName;
    }

    public abstract string UIText();

    public abstract string CurrentName();
}
