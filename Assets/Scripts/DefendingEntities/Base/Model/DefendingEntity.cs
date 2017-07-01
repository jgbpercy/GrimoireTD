using System;
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
    protected int id;

    //Attributes
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

    private Dictionary<AttributeName, Action<float>> OnAttributeChangedCallbackDictionary;

    //Economy
    protected class OccupationBonusList<T> where T : OccupationBonus
    {
        private List<T> list;

        private Action AddCallback;
        private Action RemoveCallback;

        public List<T> List
        {
            get
            {
                return list;
            }
        }

        public OccupationBonusList(Action addCallback, Action removeCallback)
        {
            AddCallback = addCallback;
            RemoveCallback = removeCallback;

            list = new List<T>();
        }

        public void AddBonus(T occupationBonus)
        {
            list.Add(occupationBonus);

            if ( AddCallback != null )
            {
                AddCallback();
            }
        }

        public bool TryRemoveBonus(T occupationBonus)
        {
            if ( !list.Contains(occupationBonus) )
            {
                return false;
            }

            list.Remove(occupationBonus);

            if ( RemoveCallback != null)
            {
                RemoveCallback();
            }

            return true;
        }
    }

    private OccupationBonusList<HexOccupationBonus> flatHexOccupationBonuses;

    private Action OnFlatHexOccupationBonusChanged;

    //Abilities
    protected class AbilityList
    {
        private SortedList<int, Ability> list;

        private DefendingEntity attachedToDefendingEntity;

        public SortedList<int, Ability> List
        {
            get
            {
                return list;
            }
        }

        public AbilityList(DefendingEntity attachedToDefendingEntity)
        {
            this.attachedToDefendingEntity = attachedToDefendingEntity;

            list = new SortedList<int, Ability>();
        }

        public void AddAbility(Ability ability)
        {
            list.Add(list.Count, ability);
            
            if ( attachedToDefendingEntity.OnAbilityAddedCallback != null )
            {
                attachedToDefendingEntity.OnAbilityAddedCallback(ability);
            }
        }

        public bool TryRemoveAbility(Ability ability)
        {
            if ( !list.ContainsValue(ability) )
            {
                return false;
            }

            int indexOfAbility = list.IndexOfValue(ability);
            list.RemoveAt(indexOfAbility);

            if ( attachedToDefendingEntity.OnAbilityRemovedCallback != null )
            {
                attachedToDefendingEntity.OnAbilityRemovedCallback(ability);
            }

            return true;
        }

        //public bool TryReplaceAbility ?? (needed?)
    }

    protected AbilityList abilities;

    private Action<Ability> OnAbilityAddedCallback;
    private Action<Ability> OnAbilityRemovedCallback;

    //Template
    private DefendingEntityTemplate defendingEntityTemplate;

    //Properties
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
            return abilities.List;
        }
    }

    public DefendingEntityTemplate DefendingEntityTemplate
    {
        get
        {
            return defendingEntityTemplate;
        }
    }

    //Helper Properties
    public Coord CoordPosition
    {
        get
        {
            return MapGenerator.Instance.Map.WhereAmI(this);
        }
    }

    protected HexData OnHex
    {
        get
        {
            return MapGenerator.Instance.Map.GetHexAt(CoordPosition);
        }
    }

    protected HexType OnHexType
    {
        get
        {
            return OnHex.HexType;
        }
    }

    //Constructor
    public DefendingEntity(DefendingEntityTemplate template)
    {
        id = IdGen.GetNextId();

        defendingEntityTemplate = template;

        SetUpAttributes();

        SetUpBaseAbilities();

        SetUpBaseFlatHexOccupationBonuses();

        GameStateManager.Instance.RegisterForOnEnterBuildModeCallback(OnEnterBuildMode);
    }

    //Set up
    private void SetUpBaseAbilities()
    {
        abilities = new AbilityList(this);

        for (int i = 0; i < defendingEntityTemplate.BaseAbilities.Length; i++)
        {
            abilities.AddAbility(defendingEntityTemplate.BaseAbilities[i].GenerateAbility(this));
        }
    }

    private void SetUpBaseFlatHexOccupationBonuses()
    {
        flatHexOccupationBonuses = new OccupationBonusList<HexOccupationBonus>(OnFlatHexOccupationBonusChanged, OnFlatHexOccupationBonusChanged);

        foreach (HexOccupationBonus hexOccupationBonus in defendingEntityTemplate.BaseFlatHexOccupationBonuses)
        {
            flatHexOccupationBonuses.AddBonus(hexOccupationBonus);
        }
    }

    private void SetUpAttributes()
    {
        attributes = new Dictionary<AttributeName, Attribute>
        {
            { AttributeName.rangeBonus, new AdditiveAttribute("Range Bonus") },
            { AttributeName.damageBonus, new AdditiveAttribute("Damage Bonus") },
            { AttributeName.cooldownReduction, new DiminishingAttribute("Cooldown Reduction") }
        };

        OnAttributeChangedCallbackDictionary = new Dictionary<AttributeName, Action<float>>();

        foreach (KeyValuePair<AttributeName, Attribute> attributePair in attributes)
        {
            OnAttributeChangedCallbackDictionary.Add(attributePair.Key, null);
        }
    }

    //Public Ability Lists
    public List<DefendModeAbility> DefendModeAbilities()
    {
        List<DefendModeAbility> defendModeAbilities = new List<DefendModeAbility>();

        for (int i = 0; i < abilities.List.Count; i++)
        {
            if (abilities.List[i] is DefendModeAbility)
            {
                defendModeAbilities.Add((DefendModeAbility)abilities.List[i]);
            }
        }

        return defendModeAbilities;
    }

    public List<BuildModeAbility> BuildModeAbilities()
    {
        List<BuildModeAbility> buildModeAbilities = new List<BuildModeAbility>();

        for (int i = 0; i < abilities.List.Count; i++)
        {
            if (abilities.List[i] is BuildModeAbility)
            {
                buildModeAbilities.Add((BuildModeAbility)abilities.List[i]);
            }
        }

        return buildModeAbilities;
    }

    protected virtual void OnEnterBuildMode()
    {
        CDebug.Log(CDebug.hexEconomy, Id + " entered build mode:");

        EconomyTransaction flatHexOccupationBonus = GetHexOccupationBonus(OnHexType, flatHexOccupationBonuses);

        EconomyManager.Instance.DoTransaction(flatHexOccupationBonus);

        CDebug.Log(CDebug.hexEconomy, Id + " added flat occupation bonus " + flatHexOccupationBonus);
    }

    protected void ApplyImprovement(DefendingEntityImprovement improvement)
    {
        foreach (NamedAttributeModifier attributeModifier in improvement.AttributeModifiers)
        {
            AddAttributeModifier(attributeModifier.AttributeName, attributeModifier.AttributeModifier);
        }

        foreach (HexOccupationBonus hexOccupationBonus in improvement.FlatHexOccupationBonuses)
        {
            flatHexOccupationBonuses.AddBonus(hexOccupationBonus);
        }

        foreach (AbilityTemplate abilityTemplate in improvement.Abilities)
        {
            abilities.AddAbility(abilityTemplate.GenerateAbility(this));
        }
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

        if (OnAttributeChangedCallbackDictionary[attribute] != null)
        {
            OnAttributeChangedCallbackDictionary[attribute](GetAttribute(attribute));
        }
    }

    public bool TryRemoveAttributeModifier(AttributeName attribute, AttributeModifier modifier)
    {
        if (attributes[attribute].TryRemoveModifier(modifier))
        {
            if (OnAttributeChangedCallbackDictionary[attribute] != null)
            {
                OnAttributeChangedCallbackDictionary[attribute](GetAttribute(attribute));
            }
            return true;
        }

        return false;
    }

    public float GetAttribute(AttributeName attributeName)
    {
        return attributes[attributeName].Value();
    }

    protected EconomyTransaction GetHexOccupationBonus(HexType hexType, OccupationBonusList<HexOccupationBonus> occupationBonusList)
    {
        EconomyTransaction occupationBonusTransaction = new EconomyTransaction();

        foreach (HexOccupationBonus hexOccupationBonus in occupationBonusList.List)
        {
            if ( hexOccupationBonus.HexType == hexType)
            {
                occupationBonusTransaction += hexOccupationBonus.ResourceGain;
            }
        }

        return occupationBonusTransaction;
    }

    public abstract string UIText();

    public abstract string CurrentName();

    public string TempDebugGetAttributeDisplayName(AttributeName attributeName)
    {
        return attributes[attributeName].DisplayName;
    }

    //Callbacks
    public void RegisterForOnFlatHexOccupationBonusChangedCallback(Action callback)
    {
        OnFlatHexOccupationBonusChanged += callback;
    }

    public void DeregisterForOnFlatHexOccupationBonusChangedCallback(Action callback)
    {
        OnFlatHexOccupationBonusChanged -= callback;
    }

    public void RegisterForOnAbilityAddedCallback(Action<Ability> callback)
    {
        OnAbilityAddedCallback += callback;
    }

    public void DeregisterForOnAbilityAddedCallback(Action<Ability> callback)
    {
        OnAbilityAddedCallback -= callback;
    }

    public void RegisterForOnAbilityRemovedCallback(Action<Ability> callback)
    {
        OnAbilityRemovedCallback += callback;
    }

    public void DeregisterForOnAbilityRemovedCallback(Action<Ability> callback)
    {
        OnAbilityRemovedCallback -= callback;
    }

    public void RegisterForOnAttributeChangedCallback(Action<float> callback, AttributeName attribute)
    {
        OnAttributeChangedCallbackDictionary[attribute] += callback;
    }

    public void DeregisterForOnAttributeChangedCallback(Action<float> callback, AttributeName attribute)
    {
        OnAttributeChangedCallbackDictionary[attribute] -= callback;
    }

}
