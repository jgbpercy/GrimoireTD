using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GrimoireTD.Defenders.Structures;
using GrimoireTD.Defenders.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Map;
using GrimoireTD.Technical;
using GrimoireTD.Attributes;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.Abilities;
using GrimoireTD.Dependencies;

namespace GrimoireTD.Defenders.Units
{
    public class CUnit : CDefender, IUnit
    {
        //Template
        public IUnitTemplate UnitTemplate { get; }

        //Movement
        public event EventHandler<EAOnMoved> OnMoved;

        private List<Coord> cachedDisallowedMovementDestinations;

        //Talents & levelling
        private Dictionary<IUnitTalent, int> talentsLevelled;

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
        public IReadOnlyDictionary<IUnitTalent, int> TalentsLevelled
        {
            get
            {
                return talentsLevelled;
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
            var baseUnitCharacteristics = unitTemplate.BaseCharacteristics as IUnitImprovement;

            //TODO: remove in release
            if (baseUnitCharacteristics == null) throw new ArgumentException("UnitTemplate BaseCharacteristics is not an IUnitImprovement");

            UnitTemplate = unitTemplate;

            SetUpTalentsLevelled();

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

            ApplyImprovement(baseUnitCharacteristics);

            TimeIdle = 0f;
            TimeActive = 0f;
            Experience = 0;
            Fatigue = 0;
            LevelUpsPending = 0;
            Level = 0;

            cachedDisallowedMovementDestinations = new List<Coord>();

            var gameModel = DepsProv.TheGameModel;

            if (gameModel.IsSetUp)
            {
                SetUpFatigueVars(gameModel.UnitFatigueFactorInfelctionPoint, gameModel.UnitFatigueFactorShallownessMultiplier);
            }
            else
            {
                gameModel.OnGameModelSetUp += OnGameModelSetUp;
            }

            DepsProv.TheModelObjectFrameUpdater().Register(ModelObjectFrameUpdate);
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

        private void SetUpTalentsLevelled()
        {
            talentsLevelled = new Dictionary<IUnitTalent, int>();

            foreach (IUnitTalent talent in UnitTemplate.UnitTalents)
            {
                talentsLevelled.Add(talent, 0);
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
                    ApplyImprovement(unitImprovement);
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
                    RemoveImprovement(unitImprovement);
                }
                else
                {
                    RemoveImprovement(args.RemovedItem.DefenderEffectTemplate.Improvement);
                }
            };

            GetDefenderAurasFromCurrentHex();
        }

        //Update loop
        private void ModelObjectFrameUpdate(float deltaTime)
        {
            if (DepsProv.TheGameStateManager.CurrentGameMode == GameMode.BUILD)
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
            float idleProportion = TimeIdle / (TimeActive + TimeIdle);

            IEconomyTransaction grossConditionalHexOccuationBonus = GetHexOccupationBonus(OnHexType, conditionalHexOccupationBonuses);

            IEconomyTransaction netConditionalHexOccupationBonus = grossConditionalHexOccuationBonus.Multiply(idleProportion);

            IEconomyTransaction grossConditionalStructureOccupationBonus = GetStructureOccupationBonus(OnHex.StructureHere, conditionalStructureOccupationBonuses);

            IEconomyTransaction netConditionalStructureOccupationBonus = grossConditionalStructureOccupationBonus.Multiply(idleProportion);

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
            float rawExperienceGain = (TimeActive / (TimeActive + TimeIdle)) * 100;

            float fatigueFactor = FatigueFactor();

            int experienceGain = Mathf.RoundToInt(rawExperienceGain * fatigueFactor);
            Experience += experienceGain;

            Fatigue += Mathf.RoundToInt((TimeActive / (TimeActive + TimeIdle)) * 10) - 5;

            Fatigue = Mathf.Max(Fatigue, 0);

            LevelUpsPending = (Experience - Level * UnitTemplate.ExperienceToLevelUp) / UnitTemplate.ExperienceToLevelUp;

            //TODO: break up this event?
            OnExperienceFatigueLevelChanged?.Invoke(this, new EAOnExperienceFatigueLevelChange(Experience, Fatigue, Level, LevelUpsPending));
        }

        private float FatigueFactor()
        {
            float rawInverserFactor = CustomMath.SignedOddRoot((Fatigue - inflectionPoint) / shallownessMultiplier, 3) + Mathf.Pow(inflectionPoint / shallownessMultiplier, 1f / 3f);

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

            if (talentsLevelled[talentChosen] >= talentChosen.UnitImprovements.Count)
            {
                return false;
            }

            if (talentsLevelled[talentChosen] != 0)
            {
                RemoveImprovement(talentChosen.UnitImprovements[talentsLevelled[talentChosen] - 1]);
            }

            ApplyImprovement(talentChosen.UnitImprovements[TalentsLevelled[talentChosen]]);

            talentsLevelled[talentChosen] += 1;

            Level += 1;
            LevelUpsPending -= 1;

            OnExperienceFatigueLevelChanged?.Invoke(this, new EAOnExperienceFatigueLevelChange(Experience, Fatigue, Level, LevelUpsPending));

            return true;
        }

        //Improvement
        protected override void ApplyImprovement(IDefenderImprovement improvement)
        {
            base.ApplyImprovement(improvement);

            var unitImprovement = improvement as IUnitImprovement;
            if (unitImprovement != null)
            {
                foreach (var occupationBonus in unitImprovement.ConditionalStructureOccupationBonuses)
                {
                    conditionalStructureOccupationBonuses.Add(occupationBonus);
                }

                foreach (var occupationBonus in unitImprovement.ConditionalHexOccupationBonuses)
                {
                    conditionalHexOccupationBonuses.Add(occupationBonus);
                }
            }
        }

        protected override void RemoveImprovement(IDefenderImprovement improvement)
        {
            base.RemoveImprovement(improvement);

            var unitImprovement = improvement as IUnitImprovement;
            if (unitImprovement != null)
            {
                bool wasPresent;

                foreach (IStructureOccupationBonus occupationBonus in unitImprovement.ConditionalStructureOccupationBonuses)
                {
                    wasPresent = conditionalStructureOccupationBonuses.TryRemove(occupationBonus);
                    Assert.IsTrue(wasPresent);
                }

                foreach (IHexOccupationBonus occupationBonus in unitImprovement.ConditionalHexOccupationBonuses)
                {
                    wasPresent = conditionalHexOccupationBonuses.TryRemove(occupationBonus);
                    Assert.IsTrue(wasPresent);
                }
            }
        }

        //Defender Auras Affected By
        protected override void OnNewDefenderAuraInCurrentHex(object sender, EAOnCallbackListAdd<IDefenderAura> args)
        {
            if (
                args.AddedItem.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.BOTH || 
                args.AddedItem.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.UNITS)
            {
                affectedByDefenderAuras.Add(args.AddedItem);
            }
        }

        protected override void OnClearDefenderAuraInCurrentHex(object sender, EAOnCallbackListRemove<IDefenderAura> args)
        {
            if (
                args.RemovedItem.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.BOTH || 
                args.RemovedItem.DefenderEffectTemplate.Affects == DefenderEffectAffectsType.UNITS)
            {
                bool wasPresent = affectedByDefenderAuras.TryRemove(args.RemovedItem);
                Assert.IsTrue(wasPresent);
            }
        }

        //Movement
        public void Move(Coord targetCoord)
        {
            OnHex.DefenderAurasHere.OnAdd -= OnNewDefenderAuraInCurrentHex;
            OnHex.DefenderAurasHere.OnRemove -= OnClearDefenderAuraInCurrentHex;

            CoordPosition = targetCoord;

            OnHex.DefenderAurasHere.OnAdd += OnNewDefenderAuraInCurrentHex;
            OnHex.DefenderAurasHere.OnRemove += OnClearDefenderAuraInCurrentHex;

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
            cachedDisallowedMovementDestinations = DepsProv.TheMapData.GetDisallowedMovementDestinationCoords(CoordPosition);
        }
    }
}