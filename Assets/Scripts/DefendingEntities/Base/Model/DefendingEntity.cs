using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public enum AttributeName
{
    rangeBonus,
    damageBonus,
    cooldownReduction
}

public abstract class DefendingEntity : IBuildModeTargetable
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
    private CallbackList<HexOccupationBonus> flatHexOccupationBonuses;

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

    //Auras (that this is the source of)
    protected CallbackList<DefenderAura> auras;

    //Affected By Auras
    protected List<DefenderAura> affectedByDefenderAuras;

    //Template
    private DefendingEntityTemplate defendingEntityTemplate;

    //Position
    protected Coord coordPosition;

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
            return coordPosition;
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
    public DefendingEntity(DefendingEntityTemplate template, Coord position)
    {
        id = IdGen.GetNextId();

        defendingEntityTemplate = template;

        SetUpAttributes();

        abilities = new AbilityList(this);

        flatHexOccupationBonuses = new CallbackList<HexOccupationBonus>();

        auras = new CallbackList<DefenderAura>();
        RegisterForOnAuraAddedCallback(OnInitialiseAura);
        RegisterForOnAuraRemovedCallback(OnRemoveAura);

        coordPosition = position;
        OnHex.RegisterForOnDefenderAuraAddedCallback(OnNewDefenderAura);
        OnHex.RegisterForOnDefenderAuraRemovedCallback(OnClearDefenderAura);

        SetUpAffectedByDefenderAuras();

        GameStateManager.Instance.RegisterForOnEnterBuildModeCallback(OnEnterBuildMode);
    }

    //Set Up
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

    private void SetUpAffectedByDefenderAuras()
    {
        affectedByDefenderAuras = new List<DefenderAura>();

        foreach (DefenderAura defenderAura in OnHex.DefenderAurasHere)
        {

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

    //Enter Build Mode
    protected virtual void OnEnterBuildMode()
    {
        CDebug.Log(CDebug.hexEconomy, Id + " entered build mode:");

        EconomyTransaction flatHexOccupationBonus = GetHexOccupationBonus(OnHexType, flatHexOccupationBonuses);

        EconomyManager.Instance.DoTransaction(flatHexOccupationBonus);

        CDebug.Log(CDebug.hexEconomy, Id + " added flat occupation bonus " + flatHexOccupationBonus);
    }

    //Improvements
    protected void ApplyImprovement(IDefendingEntityImprovement improvement)
    {
        foreach (NamedAttributeModifier attributeModifier in improvement.AttributeModifiers)
        {
            AddAttributeModifier(attributeModifier.AttributeName, attributeModifier.AttributeModifier);
        }

        foreach (HexOccupationBonus hexOccupationBonus in improvement.FlatHexOccupationBonuses)
        {
            flatHexOccupationBonuses.Add(hexOccupationBonus);
        }

        foreach (AbilityTemplate abilityTemplate in improvement.Abilities)
        {
            abilities.AddAbility(abilityTemplate.GenerateAbility(this));
        }

        foreach (DefenderAuraTemplate auraTemplate in improvement.Auras)
        {
            auras.Add(auraTemplate.GenerateDefenderAura(this));
        }
    }

    protected void RemoveImprovement(IDefendingEntityImprovement improvement)
    {
        bool wasPresent;

        foreach (NamedAttributeModifier attributeModifier in improvement.AttributeModifiers)
        {
            wasPresent = TryRemoveAttributeModifier(attributeModifier.AttributeName, attributeModifier.AttributeModifier);
            Assert.IsTrue(wasPresent);
        }

        foreach (HexOccupationBonus hexOccupationBonus in improvement.FlatHexOccupationBonuses)
        {
            wasPresent = flatHexOccupationBonuses.TryRemove(hexOccupationBonus);
            Assert.IsTrue(wasPresent);
        }

        foreach (AbilityTemplate abilityTemplate in improvement.Abilities)
        {
            wasPresent = abilities.TryRemoveAbility(abilityTemplate.GenerateAbility(this));
            Assert.IsTrue(wasPresent);
        }

        foreach (DefenderAuraTemplate auraTemplate in improvement.Auras)
        {
            wasPresent = auras.TryRemove(auraTemplate.GenerateDefenderAura(this));
            Assert.IsTrue(wasPresent);
        }
    }

    //Attributes
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

    //Economy
    protected EconomyTransaction GetHexOccupationBonus(HexType hexType, CallbackList<HexOccupationBonus> occupationBonusList)
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

    //UI Stuff
    public abstract string UIText();

    public abstract string CurrentName();

    public string TempDebugGetAttributeDisplayName(AttributeName attributeName)
    {
        return attributes[attributeName].DisplayName;
    }

    //Defender Auras that this is the source of
    protected void OnInitialiseAura(DefenderAura aura)
    {
        List<Coord> affectedCoords = MapData.CoordsInRange(aura.Range, CoordPosition);

        foreach (Coord coord in affectedCoords)
        {
            MapGenerator.Instance.Map.GetHexAt(coord).AddDefenderAura(aura);
        }
    }

    private void OnRemoveAura(DefenderAura aura)
    {
        aura.ClearAura();
    }

    //Defender Auras Affected By
    protected abstract void OnNewDefenderAura(DefenderAura aura);

    protected abstract void OnClearDefenderAura(DefenderAura aura);

    //Callbacks

    //  Flat Hex Occupation Bonus
    public void RegisterForOnFlatHexOccupationBonusAddedCallback(Action<HexOccupationBonus> callback)
    {
        flatHexOccupationBonuses.RegisterForAdd(callback);
    }

    public void DeregisterForOnFlatHexOccupationBonusAddedCallback(Action<HexOccupationBonus> callback)
    {
        flatHexOccupationBonuses.DeregisterForAdd(callback);
    }

    public void RegisterForOnFlatHexOccupationBonusRemovedCallback(Action<HexOccupationBonus> callback)
    {
        flatHexOccupationBonuses.RegisterForRemove(callback);
    }

    public void DeregisterForOnFlatHexOccupationBonusRemovedCallback(Action<HexOccupationBonus> callback)
    {
        flatHexOccupationBonuses.DeregisterForRemove(callback);
    }

    //  Abilities
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

    //  Attributes
    public void RegisterForOnAttributeChangedCallback(Action<float> callback, AttributeName attribute)
    {
        OnAttributeChangedCallbackDictionary[attribute] += callback;
    }

    public void DeregisterForOnAttributeChangedCallback(Action<float> callback, AttributeName attribute)
    {
        OnAttributeChangedCallbackDictionary[attribute] -= callback;
    }

    //  Auras
    public void RegisterForOnAuraAddedCallback(Action<DefenderAura> callback)
    {
        auras.RegisterForAdd(callback);
    }

    public void DeregisterForOnAuraAddedCallback(Action<DefenderAura> callback)
    {
        auras.DeregisterForAdd(callback);
    }

    public void RegisterForOnAuraRemovedCallback(Action<DefenderAura> callback)
    {
        auras.RegisterForRemove(callback);
    }

    public void DeregisterForOnAuraRemovedCallback(Action<DefenderAura> callback)
    {
        auras.DeregisterForRemove(callback);
    }
}
