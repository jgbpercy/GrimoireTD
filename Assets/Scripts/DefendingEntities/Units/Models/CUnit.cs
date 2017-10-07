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
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.Abilities;

namespace GrimoireTD.DefendingEntities.Units
{
    public class CUnit : CDefendingEntity, IUnit
    {
        //Template
        public IUnitTemplate UnitTemplate { get; }

        //Movement
        public event EventHandler<EAOnMoved> OnMoved;

        private List<Coord> cachedDisallowedMovementDestinations;

        //Talents & levelling
        private Dictionary<IUnitTalent, int> levelledTalents;

        public float TimeIdle { get; private set; }
        public float TimeActive { get; private set; }

        private bool isIdle = true;

        public int Experience { get; private set; }
        public int Fatigue { get; private set; }
        public int LevelUpsPending { get; private set; }
        public int Level { get; private set; }

        public event EventHandler<EAOnExperienceFatigueLevelChange> OnExperienceFatigueLevelChanged;

        private float inflectionPoint;
        private float shallownessMultiplier;

        //Economy
        private CallbackList<IHexOccupationBonus> conditionalHexOccupationBonuses;

        private CallbackList<IStructureOccupationBonus> conditionalStructureOccupationBonuses;

        public event EventHandler<EAOnTriggeredConditionalOccupationBonus> OnTriggeredConditionalOccupationBonuses;

        // Public Properties
        //Id
        public override string Id
        {
            get
            {
                return "U-" + id;
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

        //Economy
        public IReadOnlyCallbackList<IHexOccupationBonus> ConditionalHexOccupationBonuses
        {
            get
            {
                return conditionalHexOccupationBonuses;
            }
        }

        public IReadOnlyCallbackList<IStructureOccupationBonus> ConditionalStructureOccupationBonuses
        {
            get
            {
                return conditionalStructureOccupationBonuses;
            }
        }

        //UI Name etc
        public override string CurrentName
        {
            get
            {
                return UnitTemplate.NameInGame;
            }

            protected set
            {
                return;
            }
        }

        public override string UIText
        {
            get
            {
                return UnitTemplate.Description;
            }

            //TODO? Hack cos Structures need to set but this should exist at DE level?
            protected set
            {
                return;
            }
        }

        //Constructor
        public CUnit(IUnitTemplate unitTemplate, Coord coordPosition) : base(unitTemplate, coordPosition)
        {
            Assert.IsTrue(unitTemplate.BaseCharacteristics is IUnitImprovement);

            UnitTemplate = unitTemplate;

            SetUpTalentsAchieved();

            conditionalHexOccupationBonuses = new CallbackList<IHexOccupationBonus>();
            conditionalStructureOccupationBonuses = new CallbackList<IStructureOccupationBonus>();

            abilities.OnDefendModeAbilityAdded += (object sender, EAOnDefendModeAbilityAdded args) =>
            {
                args.DefendModeAbility.OnAbilityExecuted += OnDefendModeAbilityExecuted;
            };

            abilities.OnDefendModeAbilityRemoved += (object sender, EAOnDefendModeAbilityRemoved args) =>
            {
                args.DefendModeAbility.OnAbilityExecuted -= OnDefendModeAbilityExecuted;
            };

            abilities.DefendModeAbilityManager.OnAllDefendModeAbilitiesOffCooldown += OnAllDefendModeAbilitiesOffCooldown;

            ApplyUnitImprovement(unitTemplate.BaseUnitCharacteristics);

            TimeIdle = 0f;
            TimeActive = 0f;
            Experience = 0;
            Fatigue = 0;
            LevelUpsPending = 0;
            Level = 0;

            cachedDisallowedMovementDestinations = new List<Coord>();

            if (GameModels.Models[0].IsSetUp)
            {
                SetUpFatigueVars(GameModels.Models[0].UnitFatigueFactorInfelctionPoint, GameModels.Models[0].UnitFatigueFactorShallownessMultiplier);
            }
            else
            {
                GameModels.Models[0].OnGameModelSetUp += OnGameModelSetUp;
            }

            ModelObjectFrameUpdater.Instance.RegisterAsModelObjectFrameUpdatee(this);
        }

        //Set Up
        private void OnGameModelSetUp(object sender, EAOnGameModelSetUp args)
        {
            SetUpFatigueVars(args.GameModel.UnitFatigueFactorInfelctionPoint, args.GameModel.UnitFatigueFactorShallownessMultiplier);
        }

        private void SetUpFatigueVars(float inflectionPoint, float shallownessMultiplier)
        {
            this.inflectionPoint = inflectionPoint;
            this.shallownessMultiplier = shallownessMultiplier;
        }

        private void SetUpTalentsAchieved()
        {
            levelledTalents = new Dictionary<IUnitTalent, int>();

            foreach (IUnitTalent talent in UnitTemplate.UnitTalents)
            {
                levelledTalents.Add(talent, 0);
            }
        }

        protected override void SetUpAffectedByDefenderAuras()
        {
            affectedByDefenderAuras = new CallbackList<IDefenderAura>();

            affectedByDefenderAuras.OnAdd += (object sender, EAOnCallbackListAdd<IDefenderAura> args) =>
            {
                IUnitImprovement unitImprovement = args.AddedItem.DefenderEffectTemplate.Improvement as IUnitImprovement;
                if (unitImprovement != null)
                {
                    ApplyUnitImprovement(unitImprovement);
                }
                else
                {
                    ApplyImprovement(args.AddedItem.DefenderEffectTemplate.Improvement);
                }
            };

            affectedByDefenderAuras.OnRemove += (object sender, EAOnCallbackListRemove<IDefenderAura> args) =>
            {
                IUnitImprovement unitImprovement = args.RemovedItem.DefenderEffectTemplate.Improvement as IUnitImprovement;
                if (unitImprovement != null)
                {
                    RemoveUnitImprovement(unitImprovement);
                }
                else
                {
                    RemoveImprovement(args.RemovedItem.DefenderEffectTemplate.Improvement);
                }
            };

            GetDefenderAurasFromCurrentHex();
        }

        //Update loop
        public void ModelObjectFrameUpdate(float deltaTime)
        {
            if (GameModels.Models[0].GameStateManager.CurrentGameMode == GameMode.BUILD)
            {
                return;
            }

            if (isIdle)
            {
                TimeIdle += deltaTime;
            }
            else
            {
                TimeActive += deltaTime;
            }
        }

        //Time Tracking
        private void OnAllDefendModeAbilitiesOffCooldown(object sender, EAOnAllDefendModeAbilitiesOffCooldown args)
        {
            isIdle = true;
        }

        private void OnDefendModeAbilityExecuted(object sender, EAOnAbilityExecuted args)
        {
            isIdle = false;
        }

        //Enter Build Mode
        protected override void OnEnterBuildMode(object sender, EAOnEnterBuildMode args)
        {
            base.OnEnterBuildMode(sender, args);

            OnEnterBuildModeEconomyChanges();

            OnEnterBuildModeExperienceAndFatigueChanges();

            TimeIdle = 0f;
            TimeActive = 0f;
        }

        //Economy
        private void OnEnterBuildModeEconomyChanges()
        {
            CDebug.Log(CDebug.hexEconomy, "Time Active: " + TimeActive.ToString("0.0"));
            CDebug.Log(CDebug.hexEconomy, "Time Idle: " + TimeIdle.ToString("0.0"));

            float activeProportion = TimeActive / (TimeActive + TimeIdle);
            CDebug.Log(CDebug.hexEconomy, "Active Proportion: " + activeProportion.ToString("0.000"));

            IEconomyTransaction grossConditionalHexOccuationBonus = GetHexOccupationBonus(OnHexType, conditionalHexOccupationBonuses);
            CDebug.Log(CDebug.hexEconomy, "Gross Hex Oc Bonus: " + grossConditionalHexOccuationBonus);

            IEconomyTransaction netConditionalHexOccupationBonus = grossConditionalHexOccuationBonus.Multiply(activeProportion);
            CDebug.Log(CDebug.hexEconomy, "Net Hex Oc Bonus: " + netConditionalHexOccupationBonus);

            IEconomyTransaction grossConditionalStructureOccupationBonus = GetStructureOccupationBonus(OnHex.StructureHere, conditionalStructureOccupationBonuses);
            CDebug.Log(CDebug.hexEconomy, "Gross Structure Oc Bonus: " + grossConditionalStructureOccupationBonus);

            IEconomyTransaction netConditionalStructureOccupationBonus = grossConditionalStructureOccupationBonus.Multiply(activeProportion);
            CDebug.Log(CDebug.hexEconomy, "Net Structure Oc Bonus: " + netConditionalStructureOccupationBonus);

            OnTriggeredConditionalOccupationBonuses?.Invoke(this, new EAOnTriggeredConditionalOccupationBonus(this, netConditionalHexOccupationBonus, netConditionalStructureOccupationBonus));
        }

        private IEconomyTransaction GetStructureOccupationBonus(IStructure structure, CallbackList<IStructureOccupationBonus> structureOccupationBonuses)
        {
            IEconomyTransaction occupationBonusTransaction = new CEconomyTransaction();

            IStructureTemplate template = structure?.StructureTemplate;
            IStructureUpgrade upgrade = structure?.CurrentUpgradeLevel();

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
            CDebug.Log(CDebug.experienceAndFatigue, "Time Active: " + TimeActive.ToString("0.0"));
            CDebug.Log(CDebug.experienceAndFatigue, "Time Idle: " + TimeIdle.ToString("0.0"));

            float rawExperienceGain = (TimeActive / (TimeActive + TimeIdle)) * 100;

            CDebug.Log(CDebug.experienceAndFatigue, "Raw Experience: " + rawExperienceGain.ToString("0.000"));

            float fatigueFactor = FatigueFactor();

            int experienceGain = Mathf.RoundToInt(rawExperienceGain * fatigueFactor);
            Experience += experienceGain;

            CDebug.Log(CDebug.experienceAndFatigue, "Fatigue: " + Fatigue + ", Factor: " + fatigueFactor);
            CDebug.Log(CDebug.experienceAndFatigue, "Experience gain: " + experienceGain + ", new Experience: " + Experience);

            Fatigue += Mathf.RoundToInt((TimeActive / (TimeActive + TimeIdle)) * 10) - 5;

            Fatigue = Mathf.Max(Fatigue, 0);

            CDebug.Log(CDebug.experienceAndFatigue, "New fatigue: " + Fatigue);

            LevelUpsPending = (Experience - Level * UnitTemplate.ExperienceToLevelUp) / UnitTemplate.ExperienceToLevelUp;

            //TODO: break up this event?
            OnExperienceFatigueLevelChanged?.Invoke(this, new EAOnExperienceFatigueLevelChange(Experience, Fatigue, Level, LevelUpsPending));
        }

        private float FatigueFactor()
        {
            CDebug.Log(CDebug.experienceAndFatigue, "Inflection Point: " + inflectionPoint);
            CDebug.Log(CDebug.experienceAndFatigue, "Shallowness Multiplier: " + shallownessMultiplier);

            float rawInverserFactor = CustomMath.SignedOddRoot((Fatigue - inflectionPoint) / shallownessMultiplier, 3) + Mathf.Pow(inflectionPoint / shallownessMultiplier, 1f / 3f);

            CDebug.Log(CDebug.experienceAndFatigue, "Calculated raw inverse factor: " + rawInverserFactor);

            return Mathf.Clamp(1 - rawInverserFactor, 0f, 1f);
        }

        public void TempDebugAddExperience()
        {
            Experience += 30;
            LevelUpsPending = (Experience - Level * UnitTemplate.ExperienceToLevelUp) / UnitTemplate.ExperienceToLevelUp;
            OnExperienceFatigueLevelChanged?.Invoke(this, new EAOnExperienceFatigueLevelChange(Experience, Fatigue, Level, LevelUpsPending));
        }

        public bool TryLevelUp(IUnitTalent talentChosen)
        {
            if (LevelUpsPending <= 0)
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
                foreach (INamedAttributeModifier<DEAttrName> outgoingNamedModifier in talentChosen.UnitImprovements[levelledTalents[talentChosen] - 1].AttributeModifiers)
                {
                    bool removedModifier = attributes.TryRemoveModifier(outgoingNamedModifier);
                    Assert.IsTrue(removedModifier);
                }
            }

            ApplyUnitImprovement(talentChosen.UnitImprovements[LevelledTalents[talentChosen]]);

            levelledTalents[talentChosen] += 1;

            Level += 1;
            LevelUpsPending -= 1;

            OnExperienceFatigueLevelChanged?.Invoke(this, new EAOnExperienceFatigueLevelChange(Experience, Fatigue, Level, LevelUpsPending));

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
        protected override void OnNewDefenderAuraInCurrentHex(object sender, EAOnCallbackListAdd<IDefenderAura> args)
        {
            if (args.AddedItem.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.BOTH || args.AddedItem.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.UNITS)
            {
                affectedByDefenderAuras.Add(args.AddedItem);
            }
        }

        protected override void OnClearDefenderAuraInCurrentHex(object sender, EAOnCallbackListRemove<IDefenderAura> args)
        {
            if (args.RemovedItem.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.BOTH || args.RemovedItem.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.UNITS)
            {
                bool wasPresent = affectedByDefenderAuras.Contains(args.RemovedItem);
                Assert.IsTrue(wasPresent);

                affectedByDefenderAuras.TryRemove(args.RemovedItem);
            }
        }

        //Movement
        public void Move(Coord targetCoord)
        {
            OnHex.DefenderAurasHere.OnAdd -= OnNewDefenderAuraInCurrentHex;
            OnHex.DefenderAurasHere.OnRemove -= OnClearDefenderAuraInCurrentHex;

            CoordPosition = targetCoord;

            OnHex.DefenderAurasHere.OnAdd += OnNewDefenderAuraInCurrentHex;
            OnHex.DefenderAurasHere.OnRemove -= OnClearDefenderAuraInCurrentHex;

            OnMoved?.Invoke(this, new EAOnMoved(targetCoord));

            foreach (IDefenderAura auraEmitted in aurasEmitted)
            {
                auraEmitted.ClearAura();

                OnInitialiseAura(this, new EAOnCallbackListAdd<IDefenderAura>(auraEmitted));
            }

            affectedByDefenderAuras.Clear();

            GetDefenderAurasFromCurrentHex();
        }

        public void RegenerateCachedDisallowedMovementDestinations()
        {
            cachedDisallowedMovementDestinations = GameModels.Models[0].MapData.GetDisallowedCoordsAfterUnitMove(CoordPosition);
        }
    }
}