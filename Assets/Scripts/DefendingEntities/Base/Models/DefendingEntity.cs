﻿using System;
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

    private Action<AttributeName, float> OnAnyAttributeChangedCallback;

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

        //public bool TryReplaceAbility ?? (needed?) TODO?
    }

    protected AbilityList abilities;

    private Action<Ability> OnAbilityAddedCallback;
    private Action<Ability> OnAbilityRemovedCallback;

    //Auras (that this is the source of)
    protected CallbackList<DefenderAura> aurasEmitted;

    //Affected By Auras
    protected CallbackList<DefenderAura> affectedByDefenderAuras;

    //Template
    private IDefendingEntityTemplate defendingEntityTemplate;

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

    public IReadOnlyDictionary<int, Ability> Abilities
    {
        get
        {
            return abilities.List;
        }
    }

    public IDefendingEntityTemplate DefendingEntityTemplate
    {
        get
        {
            return defendingEntityTemplate;
        }
    }

    public IEnumerable<DefenderAura> AurasEmitted
    {
        get
        {
            return aurasEmitted;
        }
    }

    public IEnumerable<DefenderAura> AffectedByDefenderAuras
    {
        get
        {
            return affectedByDefenderAuras;
        }
    }

    public IEnumerable<HexOccupationBonus> FlatHexOccupationBonuses
    {
        get
        {
            return flatHexOccupationBonuses;
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

    protected IHexType OnHexType
    {
        get
        {
            return OnHex.HexType;
        }
    }

    //Constructor
    public DefendingEntity(IDefendingEntityTemplate template, Coord position)
    {
        id = IdGen.GetNextId();

        defendingEntityTemplate = template;

        SetUpAttributes();

        abilities = new AbilityList(this);

        flatHexOccupationBonuses = new CallbackList<HexOccupationBonus>();

        aurasEmitted = new CallbackList<DefenderAura>();
        RegisterForOnAuraEmittedAddedCallback(OnInitialiseAura);
        RegisterForOnAuraEmittedRemovedCallback(OnRemoveAura);

        coordPosition = position;
        OnHex.RegisterForOnDefenderAuraAddedCallback(OnNewDefenderAuraInCurrentHex);
        OnHex.RegisterForOnDefenderAuraRemovedCallback(OnClearDefenderAuraInCurrentHex);

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

    protected virtual void SetUpAffectedByDefenderAuras()
    {
        affectedByDefenderAuras = new CallbackList<DefenderAura>();

        affectedByDefenderAuras.RegisterForAdd(x => ApplyImprovement(x.DefenderEffectTemplate.Improvement));
        affectedByDefenderAuras.RegisterForRemove(x => RemoveImprovement(x.DefenderEffectTemplate.Improvement));

        GetDefenderAurasFromCurrentHex();
    }

    protected void GetDefenderAurasFromCurrentHex()
    {
        foreach (DefenderAura defenderAura in OnHex.DefenderAurasHere)
        {
            affectedByDefenderAuras.Add(defenderAura);
        }
    }

    //Public Ability Lists
    public IReadOnlyList<DefendModeAbility> DefendModeAbilities()
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

    public IReadOnlyList<BuildModeAbility> BuildModeAbilities()
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

        foreach (IAbilityTemplate abilityTemplate in improvement.Abilities)
        {
            abilities.AddAbility(abilityTemplate.GenerateAbility(this));
        }

        foreach (IDefenderAuraTemplate auraTemplate in improvement.Auras)
        {
            aurasEmitted.Add(auraTemplate.GenerateDefenderAura(this));
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

        foreach (IAbilityTemplate abilityTemplate in improvement.Abilities)
        {
            wasPresent = abilities.TryRemoveAbility(abilityTemplate.GenerateAbility(this));
            Assert.IsTrue(wasPresent);
        }

        foreach (IDefenderAuraTemplate auraTemplate in improvement.Auras)
        {
            wasPresent = aurasEmitted.TryRemove(auraTemplate.GenerateDefenderAura(this));
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
            throw new Exception("Attempted to add a modifier to attribute " + attribute + ", which " + id + " does not have.");
        }

        attributeToModify.AddModifier(modifier);

        float newAttributeValue = GetAttribute(attribute);

        OnAttributeChangedCallbackDictionary[attribute]?.Invoke(newAttributeValue);

        OnAnyAttributeChangedCallback?.Invoke(attribute, newAttributeValue);
    }

    public bool TryRemoveAttributeModifier(AttributeName attribute, AttributeModifier modifier)
    {
        if (attributes[attribute].TryRemoveModifier(modifier))
        {
            float newAttributeValue = GetAttribute(attribute);

            OnAttributeChangedCallbackDictionary[attribute]?.Invoke(GetAttribute(attribute));

            OnAnyAttributeChangedCallback?.Invoke(attribute, newAttributeValue);

            return true;
        }

        return false;
    }

    public float GetAttribute(AttributeName attributeName)
    {
        return attributes[attributeName].Value();
    }

    //Economy
    protected EconomyTransaction GetHexOccupationBonus(IHexType hexType, CallbackList<HexOccupationBonus> occupationBonusList)
    {
        EconomyTransaction occupationBonusTransaction = new EconomyTransaction();

        foreach (HexOccupationBonus hexOccupationBonus in occupationBonusList)
        {
            if ( hexOccupationBonus.HexType == hexType)
            {
                occupationBonusTransaction += hexOccupationBonus.ResourceGain;
            }
        }

        return occupationBonusTransaction;
    }

    public EconomyTransaction GetFlatHexOccupationBonus(IHexType hexType)
    {
        return GetHexOccupationBonus(hexType, flatHexOccupationBonuses);
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
    protected abstract void OnNewDefenderAuraInCurrentHex(DefenderAura aura);

    protected abstract void OnClearDefenderAuraInCurrentHex(DefenderAura aura);

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

    public void RegisterForOnAttributeChangedCallback(Action<AttributeName, float> callback)
    {
        OnAnyAttributeChangedCallback += callback;
    }

    public void DeregisterForOnAttributeChangedCallback(Action<AttributeName, float> callback)
    {
        OnAnyAttributeChangedCallback -= callback;
    }

    //  Auras
    public void RegisterForOnAuraEmittedAddedCallback(Action<DefenderAura> callback)
    {
        aurasEmitted.RegisterForAdd(callback);
    }

    public void DeregisterForOnAuraEmittedAddedCallback(Action<DefenderAura> callback)
    {
        aurasEmitted.DeregisterForAdd(callback);
    }

    public void RegisterForOnAuraEmittedRemovedCallback(Action<DefenderAura> callback)
    {
        aurasEmitted.RegisterForRemove(callback);
    }

    public void DeregisterForOnAuraEmittedRemovedCallback(Action<DefenderAura> callback)
    {
        aurasEmitted.DeregisterForRemove(callback);
    }

    public void RegisterForOnAffectedByDefenderAuraAddedCallback(Action<DefenderAura> callback)
    {
        affectedByDefenderAuras.RegisterForAdd(callback);
    }

    public void DeregisterForOnAffectedByDefenderAuraAddedCallback(Action<DefenderAura> callback)
    {
        affectedByDefenderAuras.DeregisterForAdd(callback);
    }

    public void RegisterForOnAffectedByDefenderAuraRemovedCallback(Action<DefenderAura> callback)
    {
        affectedByDefenderAuras.RegisterForRemove(callback);
    }

    public void DeregisterForOnAffectedByDefenderAuraRemovedCallback(Action<DefenderAura> callback)
    {
        affectedByDefenderAuras.DeregisterForRemove(callback);
    }
}
