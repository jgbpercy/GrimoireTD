using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GrimoireTD.DefendingEntities.Structures;
using GrimoireTD.DefendingEntities.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Map;
using GrimoireTD.ChannelDebug;
using GrimoireTD.Technical;
using GrimoireTD.Attributes;

namespace GrimoireTD.DefendingEntities.Units
{
    public class CUnit : CDefendingEntity, IUnit
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
        private CallbackList<IHexOccupationBonus> conditionalHexOccupationBonuses;

        private CallbackList<IStructureOccupationBonus> conditionalStructureOccupationBonuses;

        // Public Properties
        //Id
        public override string Id
        {
            get
            {
                return "U-" + id;
            }
        }
        
        //Template
        public IUnitTemplate UnitTemplate
        {
            get
            {
                return unitTemplate;
            }
        }

        //Movement
        public IReadOnlyList<Coord> CachedDisallowedMovementDestinations
        {
            get
            {
                return cachedDisallowedMovementDestinations;
            }
        }

        //Talents and Levelling
        public IReadOnlyDictionary<IUnitTalent, int> LevelledTalents
        {
            get
            {
                return levelledTalents;
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

        //Economy
        public IEnumerable<IHexOccupationBonus> ConditionalHexOccupationBonuses
        {
            get
            {
                return conditionalHexOccupationBonuses;
            }
        }

        public IEnumerable<IStructureOccupationBonus> ConditionalStructureOccupationBonuses
        {
            get
            {
                return conditionalStructureOccupationBonuses;
            }
        }

        //Constructor
        public CUnit(IUnitTemplate unitTemplate, Coord coordPosition) : base(unitTemplate, coordPosition)
        {
            Assert.IsTrue(unitTemplate.BaseCharacteristics is IUnitImprovement);

            this.unitTemplate = unitTemplate;

            DefendingEntityView.Instance.CreateUnit(this, coordPosition.ToPositionVector());

            SetUpTalentsAchieved();

            conditionalHexOccupationBonuses = new CallbackList<IHexOccupationBonus>();
            conditionalStructureOccupationBonuses = new CallbackList<IStructureOccupationBonus>();

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
            affectedByDefenderAuras = new CallbackList<IDefenderAura>();

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

            IEconomyTransaction grossConditionalHexOccuationBonus = GetHexOccupationBonus(OnHexType, conditionalHexOccupationBonuses);
            CDebug.Log(CDebug.hexEconomy, "Gross Hex Oc Bonus: " + grossConditionalHexOccuationBonus);

            IEconomyTransaction netConditionalHexOccupationBonus = grossConditionalHexOccuationBonus.Multiply(activeProportion);
            CDebug.Log(CDebug.hexEconomy, "Net Hex Oc Bonus: " + netConditionalHexOccupationBonus);

            netConditionalHexOccupationBonus.DoTransaction();

            IEconomyTransaction grossConditionalStructureOccupationBonus = GetStructureOccupationBonus(OnHex.StructureHere, conditionalStructureOccupationBonuses);
            CDebug.Log(CDebug.hexEconomy, "Gross Structure Oc Bonus: " + grossConditionalStructureOccupationBonus);

            IEconomyTransaction netConditionalStructureOccupationBonus = grossConditionalStructureOccupationBonus.Multiply(activeProportion);
            CDebug.Log(CDebug.hexEconomy, "Net Structure Oc Bonus: " + netConditionalStructureOccupationBonus);

            netConditionalStructureOccupationBonus.DoTransaction();
        }

        private IEconomyTransaction GetStructureOccupationBonus(IStructure structure, CallbackList<IStructureOccupationBonus> structureOccupationBonuses)
        {
            IEconomyTransaction occupationBonusTransaction = new CEconomyTransaction();

            IStructureTemplate template = structure != null ? structure.StructureTemplate : null;
            IStructureUpgrade upgrade = structure != null ? structure.CurrentUpgradeLevel() : null;

            foreach (IStructureOccupationBonus structureOccupationBonus in structureOccupationBonuses)
            {
                if (structureOccupationBonus.StructureTemplate == template && structureOccupationBonus.StructureUpgradeLevel == upgrade)
                {
                    occupationBonusTransaction = occupationBonusTransaction.Add(structureOccupationBonus.ResourceGain);
                }
            }

            return occupationBonusTransaction;
        }

        public IEconomyTransaction GetConditionalHexOccupationBonus(IHexType hexType)
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

            OnExperienceFatigueLevelChangedCallback?.Invoke();
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
            if (levelUpsPending <= 0)
            {
                return false;
            }

            if (levelledTalents[talentChosen] >= talentChosen.UnitImprovements.Count)
            {
                return false;
            }

            if (levelledTalents[talentChosen] != 0)
            {
                //TODO: remove entire previous level improvement, not just attributes
                foreach (INamedAttributeModifier<DefendingEntityAttributeName> outgoingNamedModifier in talentChosen.UnitImprovements[levelledTalents[talentChosen] - 1].AttributeModifiers)
                {
                    bool removedModifier = attributes.TryRemoveModifier(outgoingNamedModifier);
                    Assert.IsTrue(removedModifier);
                }
            }

            ApplyUnitImprovement(talentChosen.UnitImprovements[LevelledTalents[talentChosen]]);

            levelledTalents[talentChosen] += 1;

            level += 1;
            levelUpsPending -= 1;

            OnExperienceFatigueLevelChangedCallback?.Invoke();

            return true;
        }

        //Improvement
        private void ApplyUnitImprovement(IUnitImprovement improvement)
        {
            ApplyImprovement(improvement);

            foreach (IStructureOccupationBonus occupationBonus in improvement.ConditionalStructureOccupationBonuses)
            {
                conditionalStructureOccupationBonuses.Add(occupationBonus);
            }

            foreach (IHexOccupationBonus occupationBonus in improvement.ConditionalHexOccupationBonuses)
            {
                conditionalHexOccupationBonuses.Add(occupationBonus);
            }
        }

        private void RemoveUnitImprovement(IUnitImprovement improvement)
        {
            RemoveImprovement(improvement);

            bool wasPresent;

            foreach (IStructureOccupationBonus occupationBonus in improvement.ConditionalStructureOccupationBonuses)
            {
                wasPresent = conditionalStructureOccupationBonuses.TryRemove(occupationBonus);
                Assert.IsTrue(wasPresent);
            }

            foreach (IHexOccupationBonus occupationBonus in improvement.ConditionalHexOccupationBonuses)
            {
                wasPresent = conditionalHexOccupationBonuses.TryRemove(occupationBonus);
                Assert.IsTrue(wasPresent);
            }
        }

        //Defender Auras Affected By
        protected override void OnNewDefenderAuraInCurrentHex(IDefenderAura aura)
        {
            if (aura.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.BOTH || aura.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.UNITS)
            {
                affectedByDefenderAuras.Add(aura);
            }
        }

        protected override void OnClearDefenderAuraInCurrentHex(IDefenderAura aura)
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
            if (!MapGenerator.Instance.Map.TryMoveUnitTo(coordPosition, targetCoord, this, cachedDisallowedMovementDestinations))
            {
                throw new Exception("Invalid unit movement attempted");
            }

            OnHex.DeregisterForOnDefenderAuraAddedCallback(OnNewDefenderAuraInCurrentHex);
            OnHex.DeregisterForOnDefenderAuraRemovedCallback(OnClearDefenderAuraInCurrentHex);

            coordPosition = targetCoord;

            OnHex.RegisterForOnDefenderAuraAddedCallback(OnNewDefenderAuraInCurrentHex);
            OnHex.RegisterForOnDefenderAuraRemovedCallback(OnClearDefenderAuraInCurrentHex);

            OnMovedCallback(targetCoord);

            foreach (IDefenderAura auraEmitted in aurasEmitted)
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
        public void RegisterForOnConditionalHexOccupationBonusAddedCallback(Action<IHexOccupationBonus> callback)
        {
            conditionalHexOccupationBonuses.RegisterForAdd(callback);
        }

        public void DeregisterForOnConditionalHexOccupationBonusAddedCallback(Action<IHexOccupationBonus> callback)
        {
            conditionalHexOccupationBonuses.DeregisterForAdd(callback);
        }

        public void RegisterForOnConditionalHexOccupationBonusRemovedCallback(Action<IHexOccupationBonus> callback)
        {
            conditionalHexOccupationBonuses.RegisterForRemove(callback);
        }

        public void DeregisterForOnConditionalHexOccupationBonusRemovedCallback(Action<IHexOccupationBonus> callback)
        {
            conditionalHexOccupationBonuses.DeregisterForRemove(callback);
        }

        //  Structure Occupation Bonus
        public void RegisterForOnConditionalStructureOccupationBonusAddedCallback(Action<IStructureOccupationBonus> callback)
        {
            conditionalStructureOccupationBonuses.RegisterForAdd(callback);
        }

        public void DeregisterForOnConditionalStructureOccupationBonusAddedCallback(Action<IStructureOccupationBonus> callback)
        {
            conditionalStructureOccupationBonuses.DeregisterForAdd(callback);
        }

        public void RegisterForOnConditionalStructureOccupationBonusRemovedCallback(Action<IStructureOccupationBonus> callback)
        {
            conditionalStructureOccupationBonuses.RegisterForRemove(callback);
        }

        public void DeregisterForOnConditionalStructureOccupationBonusRemovedCallback(Action<IStructureOccupationBonus> callback)
        {
            conditionalStructureOccupationBonuses.DeregisterForRemove(callback);
        }
    }
}