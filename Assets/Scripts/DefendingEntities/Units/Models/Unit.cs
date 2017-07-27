using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class Unit : DefendingEntity
{
    //Template
    private IUnitTemplate unitTemplate;

    //Movement
    private Action<Coord> OnMovedCallback;

    private List<Coord> cachedDisallowedMovementDestinations = new List<Coord>();

    //Talents & levelling
    private Dictionary<IUnitTalent, int> levelledTalents;

    private float timeIdle;
    private float timeActive;

    private int experience;
    private int fatigue;
    private int levelUpsPending;
    private int level;

    private Action OnExperienceFatigueLevelChangedCallback;

    //Economy
    private CallbackList<HexOccupationBonus> conditionalHexOccupationBonuses;

    private CallbackList<StructureOccupationBonus> conditionalStructureOccupationBonuses;

    //Public Properties
    public override string Id
    {
        get
        {
            return "U-" + id;
        }
    }

    public Dictionary<IUnitTalent, int> LevelledTalents
    {
        get
        {
            return levelledTalents;
        }
    }

    public IUnitTemplate UnitTemplate
    {
        get
        {
            return unitTemplate;
        }
    }

    public List<Coord> CachedDisallowedMovementDestinations
    {
        get
        {
            return cachedDisallowedMovementDestinations;
        }
    }

    public float TimeIdle
    {
        get
        {
            return timeIdle;
        }
    }

    public float TimeActive
    {
        get
        {
            return timeActive;
        }
    }

    public int Experience
    {
        get
        {
            return experience;
        }
    }

    public int Fatigue
    {
        get
        {
            return fatigue;
        }
    }

    public int LevelUpsPending
    {
        get
        {
            return levelUpsPending;
        }
    }

    public int Level
    {
        get
        {
            return level;
        }
    }

    public IEnumerable<HexOccupationBonus> ConditionalHexOccupationBonuses
    {
        get
        {
            return conditionalHexOccupationBonuses;
        }
    }

    public IEnumerable<StructureOccupationBonus> ConditionalStructureOccupationBonuses
    {
        get
        {
            return conditionalStructureOccupationBonuses;
        }
    }

    //Constructor
    public Unit(IUnitTemplate unitTemplate, Coord coordPosition) : base(unitTemplate, coordPosition)
    {
        Assert.IsTrue(unitTemplate.BaseCharacteristics is IUnitImprovement);

        this.unitTemplate = unitTemplate;

        DefendingEntityView.Instance.CreateUnit(this, coordPosition.ToPositionVector());

        SetUpTalentsAchieved();

        conditionalHexOccupationBonuses = new CallbackList<HexOccupationBonus>();
        conditionalStructureOccupationBonuses = new CallbackList<StructureOccupationBonus>();

        ApplyUnitImprovement(unitTemplate.BaseUnitCharacteristics);

        timeIdle = 0f;
        timeActive = 0f;
        experience = 0;
        fatigue = 0;
        levelUpsPending = 0;
        level = 0;
    }

    //Set Up
    private void SetUpTalentsAchieved()
    {
        levelledTalents = new Dictionary<IUnitTalent, int>();

        foreach (IUnitTalent talent in unitTemplate.UnitTalents) 
        {
            levelledTalents.Add(talent, 0);
        }
    }

    protected override void SetUpAffectedByDefenderAuras()
    {
        affectedByDefenderAuras = new CallbackList<DefenderAura>();

        affectedByDefenderAuras.RegisterForAdd(aura =>
        {
            IUnitImprovement unitImprovement = aura.DefenderEffectTemplate.Improvement as IUnitImprovement;
            if (unitImprovement != null)
            {
                ApplyUnitImprovement(unitImprovement);
            }
            else
            {
                ApplyImprovement(aura.DefenderEffectTemplate.Improvement);
            }
        });

        affectedByDefenderAuras.RegisterForRemove(aura =>
        {
            IUnitImprovement unitImprovement = aura.DefenderEffectTemplate.Improvement as IUnitImprovement;
            if (unitImprovement != null)
            {
                RemoveUnitImprovement(unitImprovement);
            }
            else
            {
                RemoveImprovement(aura.DefenderEffectTemplate.Improvement);
            }
        });

        GetDefenderAurasFromCurrentHex();
    }

    //UI
    public override string CurrentName()
    {
        return unitTemplate.NameInGame;
    }

    public override string UIText()
    {
        return unitTemplate.Description;
    }

    //Time Tracking
    public void TrackTime(bool wasIdle, float time)
    {
        if (wasIdle)
        {
            timeIdle += time;
        }
        else
        {
            timeActive += time;
        }
    }

    //Enter Build Mode
    protected override void OnEnterBuildMode()
    {
        base.OnEnterBuildMode();

        OnEnterBuildModeEconomyChanges();

        OnEnterBuildModeExperienceAndFatigueChanges();

        timeIdle = 0f;
        timeActive = 0f;
    }

    //Economy
    private void OnEnterBuildModeEconomyChanges()
    {
        CDebug.Log(CDebug.hexEconomy, "Time Active: " + timeActive.ToString("0.0"));
        CDebug.Log(CDebug.hexEconomy, "Time Idle: " + timeIdle.ToString("0.0"));

        float activeProportion = timeActive / (timeActive + timeIdle);
        CDebug.Log(CDebug.hexEconomy, "Active Proportion: " + activeProportion.ToString("0.000"));

        EconomyTransaction grossConditionalHexOccuationBonus = GetHexOccupationBonus(OnHexType, conditionalHexOccupationBonuses);
        CDebug.Log(CDebug.hexEconomy, "Gross Hex Oc Bonus: " + grossConditionalHexOccuationBonus);

        EconomyTransaction netConditionalHexOccupationBonus = grossConditionalHexOccuationBonus.Multiply(activeProportion);
        CDebug.Log(CDebug.hexEconomy, "Net Hex Oc Bonus: " + netConditionalHexOccupationBonus);

        EconomyManager.Instance.DoTransaction(netConditionalHexOccupationBonus);

        EconomyTransaction grossConditionalStructureOccupationBonus = GetStructureOccupationBonus(OnHex.StructureHere, conditionalStructureOccupationBonuses);
        CDebug.Log(CDebug.hexEconomy, "Gross Structure Oc Bonus: " + grossConditionalStructureOccupationBonus);

        EconomyTransaction netConditionalStructureOccupationBonus = grossConditionalStructureOccupationBonus.Multiply(activeProportion);
        CDebug.Log(CDebug.hexEconomy, "Net Structure Oc Bonus: " + netConditionalStructureOccupationBonus);

        EconomyManager.Instance.DoTransaction(netConditionalStructureOccupationBonus);
    }

    private EconomyTransaction GetStructureOccupationBonus(Structure structure, CallbackList<StructureOccupationBonus> structureOccupationBonuses)
    {
        EconomyTransaction occupationBonusTransaction = new EconomyTransaction();

        IStructureTemplate template = structure != null ? structure.StructureTemplate : null;
        IStructureUpgrade upgrade = structure != null ? structure.CurrentUpgradeLevel() : null;

        foreach (StructureOccupationBonus structureOccupationBonus in structureOccupationBonuses)
        {
            if ( structureOccupationBonus.StructureTemplate == template && structureOccupationBonus.StructureUpgradeLevel == upgrade )
            {
                occupationBonusTransaction += structureOccupationBonus.ResourceGain;
            }
        }

        return occupationBonusTransaction;
    }

    public EconomyTransaction GetConditionalHexOccupationBonus(HexType hexType)
    {
        return GetHexOccupationBonus(hexType, conditionalHexOccupationBonuses);
    }

    //Experience, Fatigue and Talents
    private void OnEnterBuildModeExperienceAndFatigueChanges()
    {
        CDebug.Log(CDebug.experienceAndFatigue, "Time Active: " + timeActive.ToString("0.0"));
        CDebug.Log(CDebug.experienceAndFatigue, "Time Idle: " + timeIdle.ToString("0.0"));

        float rawExperienceGain = (timeActive / (timeActive + timeIdle)) * 100;

        CDebug.Log(CDebug.experienceAndFatigue, "Raw Experience: " + rawExperienceGain.ToString("0.000"));

        float fatigueFactor = FatigueFactor();

        int experienceGain = Mathf.RoundToInt(rawExperienceGain * fatigueFactor);
        experience += experienceGain;

        CDebug.Log(CDebug.experienceAndFatigue, "Fatigue: " + fatigue + ", Factor: " + fatigueFactor);
        CDebug.Log(CDebug.experienceAndFatigue, "Experience gain: " + experienceGain + ", new Experience: " + experience);

        fatigue += Mathf.RoundToInt((timeActive / (timeActive + timeIdle)) * 10) - 5;

        fatigue = Mathf.Max(fatigue, 0);

        CDebug.Log(CDebug.experienceAndFatigue, "New fatigue: " + fatigue);

        levelUpsPending = (experience - level * UnitTemplate.ExperienceToLevelUp) / UnitTemplate.ExperienceToLevelUp;

        if (OnExperienceFatigueLevelChangedCallback != null)
        {
            OnExperienceFatigueLevelChangedCallback();
        }
    }

    private float FatigueFactor()
    {
        float inflectionPoint = TempSettings.Instance.UnitFatigueFactorInfelctionPoint;
        float shallownessMultiplier = TempSettings.Instance.UnitFatigueFactorShallownessMultiplier;

        CDebug.Log(CDebug.experienceAndFatigue, "Inflection Point: " + inflectionPoint);
        CDebug.Log(CDebug.experienceAndFatigue, "Shallowness Multiplier: " + shallownessMultiplier);

        float rawInverserFactor = CustomMath.SignedOddRoot((fatigue - inflectionPoint) / shallownessMultiplier, 3) + Mathf.Pow(inflectionPoint / shallownessMultiplier, 1f / 3f);

        CDebug.Log(CDebug.experienceAndFatigue, "Calculated raw inverse factor: " + rawInverserFactor);

        return Mathf.Clamp(1 - rawInverserFactor, 0f, 1f);
    }

    public void TempDebugAddExperience()
    {
        experience += 30;
        levelUpsPending = (experience - level * UnitTemplate.ExperienceToLevelUp) / UnitTemplate.ExperienceToLevelUp;
        OnExperienceFatigueLevelChangedCallback();
    }

    public bool TryLevelUp(IUnitTalent talentChosen)
    {
        if ( levelUpsPending <= 0 )
        {
            return false;
        }

        if ( levelledTalents[talentChosen] >= talentChosen.UnitImprovements.Count)
        {
            return false;
        }

        if (levelledTalents[talentChosen] != 0)
        {
            foreach (NamedAttributeModifier outgoingNamedModifier in talentChosen.UnitImprovements[levelledTalents[talentChosen] - 1].AttributeModifiers)
            {
                bool removedModifier = TryRemoveAttributeModifier(outgoingNamedModifier.AttributeName, outgoingNamedModifier.AttributeModifier);
                Assert.IsTrue(removedModifier);
            }
        }

        ApplyUnitImprovement(talentChosen.UnitImprovements[LevelledTalents[talentChosen]]);

        levelledTalents[talentChosen] += 1;

        level += 1;
        levelUpsPending -= 1;

        if ( OnExperienceFatigueLevelChangedCallback != null )
        {
            OnExperienceFatigueLevelChangedCallback();
        }

        return true;
    }

    //Improvement
    private void ApplyUnitImprovement(IUnitImprovement improvement)
    {
        ApplyImprovement(improvement);

        foreach (StructureOccupationBonus occupationBonus in improvement.ConditionalStructureOccupationBonuses)
        {
            conditionalStructureOccupationBonuses.Add(occupationBonus);
        }

        foreach (HexOccupationBonus occupationBonus in improvement.ConditionalHexOccupationBonuses)
        {
            conditionalHexOccupationBonuses.Add(occupationBonus);
        }
    }

    private void RemoveUnitImprovement(IUnitImprovement improvement)
    {
        RemoveImprovement(improvement);

        bool wasPresent;

        foreach (StructureOccupationBonus occupationBonus in improvement.ConditionalStructureOccupationBonuses) 
        {
            wasPresent = conditionalStructureOccupationBonuses.TryRemove(occupationBonus);
            Assert.IsTrue(wasPresent);
        }

        foreach (HexOccupationBonus occupationBonus in improvement.ConditionalHexOccupationBonuses)
        {
            wasPresent = conditionalHexOccupationBonuses.TryRemove(occupationBonus);
            Assert.IsTrue(wasPresent);
        }
    }

    //Defender Auras Affected By
    protected override void OnNewDefenderAuraInCurrentHex(DefenderAura aura)
    {
        if ( aura.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.BOTH || aura.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.UNITS )
        {
            affectedByDefenderAuras.Add(aura);
        }
    }

    protected override void OnClearDefenderAuraInCurrentHex(DefenderAura aura)
    {
        if (aura.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.BOTH || aura.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.UNITS)
        {
            bool wasPresent = affectedByDefenderAuras.Contains(aura);
            Assert.IsTrue(wasPresent);

            affectedByDefenderAuras.TryRemove(aura);
        }
    }

    //Movement
    public void Move(Coord targetCoord)
    {
        if ( !MapGenerator.Instance.Map.TryMoveUnitTo(coordPosition, targetCoord, this, cachedDisallowedMovementDestinations) )
        {
            throw new Exception("Invalid unit movement attempted");
        }

        OnHex.DeregisterForOnDefenderAuraAddedCallback(OnNewDefenderAuraInCurrentHex);
        OnHex.DeregisterForOnDefenderAuraRemovedCallback(OnClearDefenderAuraInCurrentHex);

        coordPosition = targetCoord;

        OnHex.RegisterForOnDefenderAuraAddedCallback(OnNewDefenderAuraInCurrentHex);
        OnHex.RegisterForOnDefenderAuraRemovedCallback(OnClearDefenderAuraInCurrentHex);

        OnMovedCallback(targetCoord);

        foreach (DefenderAura auraEmitted in aurasEmitted)
        {
            auraEmitted.ClearAura();

            OnInitialiseAura(auraEmitted);
        }

        affectedByDefenderAuras.Clear();
        
        GetDefenderAurasFromCurrentHex();
    }

    public void RegenerateCachedDisallowedMovementDestinations()
    {
        cachedDisallowedMovementDestinations = MapGenerator.Instance.Map.GetDisallowedCoordsAfterUnitMove(CoordPosition);
    }

    //Callbacks
    //  Moves
    public void RegisterForOnMovedCallback(Action<Coord> callback)
    {
        OnMovedCallback += callback;
    }

    public void DeregisterForOnMovedCallback(Action<Coord> callback)
    {
        OnMovedCallback -= callback;
    }

    //  Experience/Fatigue Change
    public void RegisterForExperienceFatigueChangedCallback(Action callback)
    {
        OnExperienceFatigueLevelChangedCallback += callback;
    }

    public void DeregisterForExperienceFatigueChangedCallback(Action callback)
    {
        OnExperienceFatigueLevelChangedCallback -= callback;
    }

    //  Hex Occupation Bonus
    public void RegisterForOnConditionalHexOccupationBonusAddedCallback(Action<HexOccupationBonus> callback)
    {
        conditionalHexOccupationBonuses.RegisterForAdd(callback);
    }

    public void DeregisterForOnConditionalHexOccupationBonusAddedCallback(Action<HexOccupationBonus> callback)
    {
        conditionalHexOccupationBonuses.DeregisterForAdd(callback);
    }

    public void RegisterForOnConditionalHexOccupationBonusRemovedCallback(Action<HexOccupationBonus> callback)
    {
        conditionalHexOccupationBonuses.RegisterForRemove(callback);
    }

    public void DeregisterForOnConditionalHexOccupationBonusRemovedCallback(Action<HexOccupationBonus> callback)
    {
        conditionalHexOccupationBonuses.DeregisterForRemove(callback);
    }

    //  Structure Occupation Bonus
    public void RegisterForOnConditionalStructureOccupationBonusAddedCallback(Action<StructureOccupationBonus> callback)
    {
        conditionalStructureOccupationBonuses.RegisterForAdd(callback);
    }

    public void DeregisterForOnConditionalStructureOccupationBonusAddedCallback(Action<StructureOccupationBonus> callback)
    {
        conditionalStructureOccupationBonuses.DeregisterForAdd(callback);
    }

    public void RegisterForOnConditionalStructureOccupationBonusRemovedCallback(Action<StructureOccupationBonus> callback)
    {
        conditionalStructureOccupationBonuses.RegisterForRemove(callback);
    }

    public void DeregisterForOnConditionalStructureOccupationBonusRemovedCallback(Action<StructureOccupationBonus> callback)
    {
        conditionalStructureOccupationBonuses.DeregisterForRemove(callback);
    }

}
