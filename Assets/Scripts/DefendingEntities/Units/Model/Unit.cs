using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public enum UnitAttributeName
{
    rangeBonus,
    damageBonus,
    cooldownReduction
}

public class Unit : DefendingEntity
{

    private abstract class Attribute
    {
        private string displayName;

        protected List<UnitAttributeModifier> modifiers;

        public string DisplayName
        {
            get
            {
                return displayName;
            }
        }

        public Attribute(string displayName)
        {
            modifiers = new List<UnitAttributeModifier>();
            this.displayName = displayName;
        }

        public void AddModifier(UnitAttributeModifier modifier)
        {
            modifiers.Add(modifier);
        }

        public bool TryRemoveModifier(UnitAttributeModifier modifier)
        {
            if (!modifiers.Contains(modifier) )
            {
                return false;
            }

            modifiers.Remove(modifier);
            return true;
        }

        public abstract float Value();
    }

    private class MultiplicativeAttribute : Attribute
    {
        public MultiplicativeAttribute(string displayName) : base(displayName)
        {

        }

        public override float Value()
        {
            float multiplier = 1;

            foreach (UnitAttributeModifier modifier in modifiers)
            {
                multiplier = multiplier * (1 + modifier.Magnitude);
            }

            return multiplier - 1;
        }
    }

    private class AdditiveAttribute : Attribute
    {
        public AdditiveAttribute(string displayName) : base(displayName)
        {

        }

        public override float Value()
        {
            float value = 0;

            foreach (UnitAttributeModifier modifier in modifiers)
            {
                value += modifier.Magnitude;
            }

            return value;
        }
    }

    private class DiminishingAttribute : Attribute
    {
        public DiminishingAttribute(string displayName) : base(displayName)
        {

        }

        public override float Value()
        {
            float multiplier = 1;

            foreach (UnitAttributeModifier modifier in modifiers)
            {
                multiplier = multiplier * (1 - modifier.Magnitude);
            }

            return 1 - multiplier;
        }
    }

    private Dictionary<UnitAttributeName, Attribute> attributes;

    private Dictionary<UnitTalent, int> levelledTalents;

    private UnitTemplate unitTemplate;

    private Action<Coord> onMovedCallback;

    private float timeIdle;
    private float timeActive;

    private int experience;
    private int fatigue;
    private int levelUpsPending;
    private int level;

    private Action OnExperienceFatigueLevelChangedCallback;

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

        SetUpAttributes();

        SetUpLevelUpsAchieved();

        timeIdle = 0f;
        timeActive = 0f;
        experience = 0;
        fatigue = 0;
        levelUpsPending = 0;
        level = 0;

        GameStateManager.Instance.RegisterForOnEnterBuildModeCallback(OnEnterBuildMode);
    }

    private void SetUpAttributes()
    {
        attributes = new Dictionary<UnitAttributeName, Attribute>();

        attributes.Add(UnitAttributeName.rangeBonus, new AdditiveAttribute("Range Bonus"));
        attributes.Add(UnitAttributeName.damageBonus, new AdditiveAttribute("Damage Bonus"));
        attributes.Add(UnitAttributeName.cooldownReduction, new DiminishingAttribute("Cooldown Reduction"));
    }

    private void SetUpLevelUpsAchieved()
    {
        levelledTalents = new Dictionary<UnitTalent, int>();

        foreach (UnitTalent levelUp in unitTemplate.UnitTalents) 
        {
            levelledTalents.Add(levelUp, 0);
        }
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

    private void OnEnterBuildMode()
    {
        CDebug.Log(CDebug.experienceAndFatigue, Id + " entered build mode:");
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

        timeIdle = 0f;
        timeActive = 0f;
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

    public float GetAttribute(UnitAttributeName attributeName)
    {
        return attributes[attributeName].Value();
    }

    public string TempDebugGetAttributeDisplayName(UnitAttributeName attributeName)
    {
        return attributes[attributeName].DisplayName;
    }

    public void AddAttributeModifier(UnitAttributeName attribute, UnitAttributeModifier modifier)
    {
        attributes[attribute].AddModifier(modifier);
    }

    public bool TryRemoveAttributeModifier(UnitAttributeName attribute, UnitAttributeModifier modifier)
    {
        if (attributes[attribute].TryRemoveModifier(modifier))
        {
            return true;
        }

        return false;
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
            UnitAttributeNamedModifier outgoingNamedModifier = talentChosen.AttributeBonuses[levelledTalents[talentChosen] - 1];
            bool removedModifier = TryRemoveAttributeModifier(outgoingNamedModifier.UnitAttributeName, outgoingNamedModifier.UnitAttributeModifier);
            Assert.IsTrue(removedModifier);
        }

        UnitAttributeNamedModifier incomingNamedModifier = talentChosen.AttributeBonuses[levelledTalents[talentChosen]];
        AddAttributeModifier(incomingNamedModifier.UnitAttributeName, incomingNamedModifier.UnitAttributeModifier);

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
