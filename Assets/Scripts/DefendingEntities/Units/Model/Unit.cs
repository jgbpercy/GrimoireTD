﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class Unit : DefendingEntity
{
    //Template
    private UnitTemplate unitTemplate;

    //Movement
    private Action<Coord> onMovedCallback;

    //Talents & levelling
    private Dictionary<UnitTalent, int> levelledTalents;

    private float timeIdle;
    private float timeActive;

    private int experience;
    private int fatigue;
    private int levelUpsPending;
    private int level;

    private Action OnExperienceFatigueLevelChangedCallback;

    //Economy
    private List<HexOccupationBonus> conditionalHexOccupationBonuses;

    //Public Properties
    public override string Id
    {
        get
        {
            return "U-" + id;
        }
    }

    public Dictionary<UnitTalent, int> LevelledTalents
    {
        get
        {
            return levelledTalents;
        }
    }

    public UnitTemplate UnitTemplate
    {
        get
        {
            return unitTemplate;
        }
    }

    public Action<Coord> OnMovedCallback
    {
        get
        {
            return onMovedCallback;
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

    public Unit(UnitTemplate unitTemplate, Vector3 position) : base(unitTemplate)
    {
        id = IdGen.GetNextId();

        this.unitTemplate = unitTemplate;

        DefendingEntityView.Instance.CreateUnit(this, position);

        SetUpTalentsAchieved();

        SetUpBaseConditionalHexOccupationBonuses();

        timeIdle = 0f;
        timeActive = 0f;
        experience = 0;
        fatigue = 0;
        levelUpsPending = 0;
        level = 0;
    }

    private void SetUpTalentsAchieved()
    {
        levelledTalents = new Dictionary<UnitTalent, int>();

        foreach (UnitTalent talent in unitTemplate.UnitTalents) 
        {
            levelledTalents.Add(talent, 0);
        }
    }

    private void SetUpBaseConditionalHexOccupationBonuses()
    {
        conditionalHexOccupationBonuses = new List<HexOccupationBonus>();

        foreach (HexOccupationBonus hexOccupationBonus in unitTemplate.BaseConditionalHexOccupationBonuses)
        {
            conditionalHexOccupationBonuses.Add(hexOccupationBonus);
        }
    }

    public override string CurrentName()
    {
        return unitTemplate.NameInGame;
    }

    public override string UIText()
    {
        return unitTemplate.Description;
    }

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

    protected override void OnEnterBuildMode()
    {
        base.OnEnterBuildMode();

        OnEnterBuildModeEconomyChanges();

        OnEnterBuildModeExperienceAndFatigueChanges();

        timeIdle = 0f;
        timeActive = 0f;
    }

    private void OnEnterBuildModeEconomyChanges()
    {
        CDebug.Log(CDebug.hexEconomy, "Time Active: " + timeActive.ToString("0.0"));
        CDebug.Log(CDebug.hexEconomy, "Time Idle: " + timeIdle.ToString("0.0"));

        float activeProportion = timeActive / (timeActive + timeIdle);
        CDebug.Log(CDebug.hexEconomy, "Active Proportion: " + activeProportion.ToString("0.000"));

        EconomyTransaction grossConditionalHexOccuationBonus = GetHexOccupationBonus(OnHexType, conditionalHexOccupationBonuses);
        CDebug.Log(CDebug.hexEconomy, "Gross Bonus: " + grossConditionalHexOccuationBonus);

        EconomyTransaction netConditionalHexOccupationBonus = grossConditionalHexOccuationBonus.Multiply(activeProportion);
        CDebug.Log(CDebug.hexEconomy, "Net Bonus: " + netConditionalHexOccupationBonus);

        EconomyManager.Instance.DoTransaction(grossConditionalHexOccuationBonus);
    }

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

    public bool TryLevelUp(UnitTalent talentChosen)
    {
        if ( levelUpsPending <= 0 )
        {
            return false;
        }

        if ( levelledTalents[talentChosen] >= talentChosen.AttributeBonuses.Length)
        {
            return false;
        }

        if (levelledTalents[talentChosen] != 0)
        {
            NamedAttributeModifier outgoingNamedModifier = talentChosen.AttributeBonuses[levelledTalents[talentChosen] - 1];
            bool removedModifier = TryRemoveAttributeModifier(outgoingNamedModifier.AttributeName, outgoingNamedModifier.AttributeModifier);
            Assert.IsTrue(removedModifier);
        }

        NamedAttributeModifier incomingNamedModifier = talentChosen.AttributeBonuses[levelledTalents[talentChosen]];
        AddAttributeModifier(incomingNamedModifier.AttributeName, incomingNamedModifier.AttributeModifier);

        levelledTalents[talentChosen] += 1;

        level += 1;
        levelUpsPending -= 1;

        if ( OnExperienceFatigueLevelChangedCallback != null )
        {
            OnExperienceFatigueLevelChangedCallback();
        }

        return true;
    }

    public void RegisterForOnMovedCallback(Action<Coord> callback)
    {
        onMovedCallback += callback;
    }

    public void DeregisterForOnMovedCallback(Action<Coord> callback)
    {
        onMovedCallback -= callback;
    }

    public void RegisterForExperienceFatigueChangedCallback(Action callback)
    {
        OnExperienceFatigueLevelChangedCallback += callback;
    }

    public void DeregisterForExperienceFatigueChangedCallback(Action callback)
    {
        OnExperienceFatigueLevelChangedCallback -= callback;
    }
}
