using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using GrimoireTD.Abilities;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.DefendingEntities.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Map;
using GrimoireTD.Technical;
using GrimoireTD.ChannelDebug;
using GrimoireTD.Attributes;
using GrimoireTD.Abilities.DefendMode.Projectiles;

namespace GrimoireTD.DefendingEntities
{
    public abstract class CDefendingEntity : IDefendingEntity
    {
        protected int id;

        //Attributes
        protected IAttributes<DefendingEntityAttributeName> attributes;

        //Economy
        private CallbackList<IHexOccupationBonus> flatHexOccupationBonuses;

        private Action<IDefendingEntity, IEconomyTransaction> OnTriggeredFlatHexOccupationBonusCallback;

        //Abilities
        //TODO refactor out
        protected class AbilityList
        {
            private SortedList<int, IAbility> list;

            private CDefendingEntity attachedToDefendingEntity;

            public SortedList<int, IAbility> List
            {
                get
                {
                    return list;
                }
            }

            public AbilityList(CDefendingEntity attachedToDefendingEntity)
            {
                this.attachedToDefendingEntity = attachedToDefendingEntity;

                list = new SortedList<int, IAbility>();
            }

            public void AddAbility(IAbility ability)
            {
                list.Add(list.Count, ability);

                attachedToDefendingEntity.OnAbilityAddedCallback?.Invoke(ability);
            }

            public bool TryRemoveAbility(IAbility ability)
            {
                if (!list.ContainsValue(ability))
                {
                    return false;
                }

                int indexOfAbility = list.IndexOfValue(ability);
                list.RemoveAt(indexOfAbility);

                attachedToDefendingEntity.OnAbilityRemovedCallback?.Invoke(ability);

                return true;
            }

            public bool TryRemoveAbility(IAbilityTemplate template)
            {
                var abilityToRemove = list
                    .Select(kvp => kvp.Value)
                    .Where(x => x.Template == template)
                    .FirstOrDefault();

                if (abilityToRemove == null)
                {
                    return false;
                }

                int indexOfAbility = list.IndexOfValue(abilityToRemove);
                list.RemoveAt(indexOfAbility);

                return true;
            }

            //public bool TryReplaceAbility ?? (needed?) TODO?
        }

        protected AbilityList abilities;

        private Action<IAbility> OnAbilityAddedCallback;
        private Action<IAbility> OnAbilityRemovedCallback;

        private Action<IBuildModeAbility> OnBuildModeAbilityExecutedCallback;

        //Auras (that this is the source of)
        protected CallbackList<IDefenderAura> aurasEmitted;

        //Affected By Auras
        protected CallbackList<IDefenderAura> affectedByDefenderAuras;

        //Template
        private IDefendingEntityTemplate defendingEntityTemplate;

        //Position
        protected Coord coordPosition;

        //Projectiles
        private Action<IProjectile> OnProjectileCreatedCallback;

        //Properties
        public virtual string Id
        {
            get
            {
                return "?-" + id;
            }
        }

        public IDefendingEntityTemplate DefendingEntityTemplate
        {
            get
            {
                return defendingEntityTemplate;
            }
        }

        public IReadOnlyDictionary<int, IAbility> Abilities
        {
            get
            {
                return abilities.List;
            }
        }

        public IEnumerable<IDefenderAura> AurasEmitted
        {
            get
            {
                return aurasEmitted;
            }
        }

        public IEnumerable<IDefenderAura> AffectedByDefenderAuras
        {
            get
            {
                return affectedByDefenderAuras;
            }
        }

        public IEnumerable<IHexOccupationBonus> FlatHexOccupationBonuses
        {
            get
            {
                return flatHexOccupationBonuses;
            }
        }

        public IReadOnlyAttributes<DefendingEntityAttributeName> Attributes
        {
            get
            {
                return attributes;
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

        protected IHexData OnHex
        {
            get
            {
                return GameModels.Models[0].MapData.GetHexAt(CoordPosition);
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
        public CDefendingEntity(IDefendingEntityTemplate template, Coord position)
        {
            id = IdGen.GetNextId();

            defendingEntityTemplate = template;

            attributes = new CAttributes<DefendingEntityAttributeName>(DefendingEntityAttributes.NewAttributesDictionary());

            abilities = new AbilityList(this);

            //TODO: unfuck this when refactoring out ability list
            OnAbilityAddedCallback += OnAbilityAdded;

            flatHexOccupationBonuses = new CallbackList<IHexOccupationBonus>();

            aurasEmitted = new CallbackList<IDefenderAura>();
            RegisterForOnAuraEmittedAddedCallback(OnInitialiseAura);
            RegisterForOnAuraEmittedRemovedCallback(OnRemoveAura);

            coordPosition = position;
            OnHex.RegisterForOnDefenderAuraAddedCallback(OnNewDefenderAuraInCurrentHex);
            OnHex.RegisterForOnDefenderAuraRemovedCallback(OnClearDefenderAuraInCurrentHex);

            SetUpAffectedByDefenderAuras();

            GameModels.Models[0].GameStateManager.RegisterForOnEnterBuildModeCallback(OnEnterBuildMode);
        }

        //Set Up
        protected virtual void SetUpAffectedByDefenderAuras()
        {
            affectedByDefenderAuras = new CallbackList<IDefenderAura> ();

            affectedByDefenderAuras.RegisterForAdd(x => ApplyImprovement(x.DefenderEffectTemplate.Improvement));
            affectedByDefenderAuras.RegisterForRemove(x => RemoveImprovement(x.DefenderEffectTemplate.Improvement));

            GetDefenderAurasFromCurrentHex();
        }

        protected void GetDefenderAurasFromCurrentHex()
        {
            foreach (IDefenderAura defenderAura in OnHex.DefenderAurasHere)
            {
                affectedByDefenderAuras.Add(defenderAura);
            }
        }

        //Public Ability Lists
        public IReadOnlyList<IDefendModeAbility> DefendModeAbilities()
        {
            List<IDefendModeAbility> defendModeAbilities = new List<IDefendModeAbility>();

            for (int i = 0; i < abilities.List.Count; i++)
            {
                if (abilities.List[i] is IDefendModeAbility)
                {
                    defendModeAbilities.Add((IDefendModeAbility)abilities.List[i]);
                }
            }

            return defendModeAbilities;
        }

        public IReadOnlyList<IBuildModeAbility> BuildModeAbilities()
        {
            List<IBuildModeAbility> buildModeAbilities = new List<IBuildModeAbility>();

            for (int i = 0; i < abilities.List.Count; i++)
            {
                if (abilities.List[i] is IBuildModeAbility)
                {
                    buildModeAbilities.Add((IBuildModeAbility)abilities.List[i]);
                }
            }

            return buildModeAbilities;
        }

        //Enter Build Mode
        protected virtual void OnEnterBuildMode()
        {
            CDebug.Log(CDebug.hexEconomy, Id + " entered build mode:");

            IEconomyTransaction flatHexOccupationBonus = GetHexOccupationBonus(OnHexType, flatHexOccupationBonuses);

            OnTriggeredFlatHexOccupationBonusCallback?.Invoke(this, flatHexOccupationBonus);

            CDebug.Log(CDebug.hexEconomy, Id + " added flat occupation bonus " + flatHexOccupationBonus);
        }

        //Improvements
        protected void ApplyImprovement(IDefendingEntityImprovement improvement)
        {
            foreach (INamedAttributeModifier<DefendingEntityAttributeName> attributeModifier in improvement.AttributeModifiers)
            {
                attributes.AddModifier(attributeModifier);
            }

            foreach (IHexOccupationBonus hexOccupationBonus in improvement.FlatHexOccupationBonuses)
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

            foreach (INamedAttributeModifier<DefendingEntityAttributeName> attributeModifier in improvement.AttributeModifiers)
            {
                wasPresent = attributes.TryRemoveModifier(attributeModifier);
                Assert.IsTrue(wasPresent);
            }

            foreach (IHexOccupationBonus hexOccupationBonus in improvement.FlatHexOccupationBonuses)
            {
                wasPresent = flatHexOccupationBonuses.TryRemove(hexOccupationBonus);
                Assert.IsTrue(wasPresent);
            }

            foreach (IAbilityTemplate abilityTemplate in improvement.Abilities)
            {
                wasPresent = abilities.TryRemoveAbility(abilityTemplate);
                Assert.IsTrue(wasPresent);
            }

            foreach (IDefenderAuraTemplate auraTemplate in improvement.Auras)
            {
                wasPresent = aurasEmitted.TryRemove(auraTemplate.GenerateDefenderAura(this), new CDefenderAura.TemplateEqualityComparer());
                Assert.IsTrue(wasPresent);
            }
        }

        //Economy
        protected IEconomyTransaction GetHexOccupationBonus(IHexType hexType, CallbackList<IHexOccupationBonus> occupationBonusList)
        {
            IEconomyTransaction occupationBonusTransaction = new CEconomyTransaction();

            foreach (IHexOccupationBonus hexOccupationBonus in occupationBonusList)
            {
                if (hexOccupationBonus.HexType == hexType)
                {
                    occupationBonusTransaction = occupationBonusTransaction.Add(hexOccupationBonus.ResourceGain);
                }
            }

            return occupationBonusTransaction;
        }

        public IEconomyTransaction GetFlatHexOccupationBonus(IHexType hexType)
        {
            return GetHexOccupationBonus(hexType, flatHexOccupationBonuses);
        }

        //Projectiles
        public void CreatedProjectile(IProjectile projectile)
        {
            OnProjectileCreatedCallback?.Invoke(projectile);
        }

        //UI Stuff
        public abstract string UIText();

        public abstract string CurrentName();

        //Defender Auras that this is the source of
        protected void OnInitialiseAura(IDefenderAura aura)
        {
            List<Coord> affectedCoords = GameModels.Models[0].MapData.CoordsInRange(aura.Range, CoordPosition);

            foreach (Coord coord in affectedCoords)
            {
                GameModels.Models[0].MapData.GetHexAt(coord).AddDefenderAura(aura);
            }
        }

        private void OnRemoveAura(IDefenderAura aura)
        {
            aura.ClearAura();
        }

        //Defender Auras Affected By
        protected abstract void OnNewDefenderAuraInCurrentHex(IDefenderAura aura);

        protected abstract void OnClearDefenderAuraInCurrentHex(IDefenderAura aura);

        //Callbacks

        //  Flat Hex Occupation Bonus
        public void RegisterForOnFlatHexOccupationBonusAddedCallback(Action<IHexOccupationBonus> callback)
        {
            flatHexOccupationBonuses.RegisterForAdd(callback);
        }

        public void DeregisterForOnFlatHexOccupationBonusAddedCallback(Action<IHexOccupationBonus> callback)
        {
            flatHexOccupationBonuses.DeregisterForAdd(callback);
        }

        public void RegisterForOnFlatHexOccupationBonusRemovedCallback(Action<IHexOccupationBonus> callback)
        {
            flatHexOccupationBonuses.RegisterForRemove(callback);
        }

        public void DeregisterForOnFlatHexOccupationBonusRemovedCallback(Action<IHexOccupationBonus> callback)
        {
            flatHexOccupationBonuses.DeregisterForRemove(callback);
        }

        public void RegisterForOnTriggeredFlatHexOccupationBonusCallback(Action<IDefendingEntity, IEconomyTransaction> callback)
        {
            OnTriggeredFlatHexOccupationBonusCallback += callback;
        }

        public void DregisterForOnTriggeredFlatHexOccupationBonusCallback(Action<IDefendingEntity, IEconomyTransaction> callback)
        {
            OnTriggeredFlatHexOccupationBonusCallback -= callback;
        }

        //  Abilities
        public void RegisterForOnAbilityAddedCallback(Action<IAbility> callback)
        {
            OnAbilityAddedCallback += callback;
        }

        public void DeregisterForOnAbilityAddedCallback(Action<IAbility> callback)
        {
            OnAbilityAddedCallback -= callback;
        }

        public void RegisterForOnAbilityRemovedCallback(Action<IAbility> callback)
        {
            OnAbilityRemovedCallback += callback;
        }

        public void DeregisterForOnAbilityRemovedCallback(Action<IAbility> callback)
        {
            OnAbilityRemovedCallback -= callback;
        }

        //  Auras
        public void RegisterForOnAuraEmittedAddedCallback(Action<IDefenderAura> callback)
        {
            aurasEmitted.RegisterForAdd(callback);
        }

        public void DeregisterForOnAuraEmittedAddedCallback(Action<IDefenderAura> callback)
        {
            aurasEmitted.DeregisterForAdd(callback);
        }

        public void RegisterForOnAuraEmittedRemovedCallback(Action<IDefenderAura> callback)
        {
            aurasEmitted.RegisterForRemove(callback);
        }

        public void DeregisterForOnAuraEmittedRemovedCallback(Action<IDefenderAura> callback)
        {
            aurasEmitted.DeregisterForRemove(callback);
        }

        public void RegisterForOnAffectedByDefenderAuraAddedCallback(Action<IDefenderAura> callback)
        {
            affectedByDefenderAuras.RegisterForAdd(callback);
        }

        public void DeregisterForOnAffectedByDefenderAuraAddedCallback(Action<IDefenderAura> callback)
        {
            affectedByDefenderAuras.DeregisterForAdd(callback);
        }

        public void RegisterForOnAffectedByDefenderAuraRemovedCallback(Action<IDefenderAura> callback)
        {
            affectedByDefenderAuras.RegisterForRemove(callback);
        }

        public void DeregisterForOnAffectedByDefenderAuraRemovedCallback(Action<IDefenderAura> callback)
        {
            affectedByDefenderAuras.DeregisterForRemove(callback);
        }

        //Projectile spawning
        public void RegisterForOnProjectileCreatedCallback(Action<IProjectile> callback)
        {
            OnProjectileCreatedCallback += callback;
        }

        public void DeregisterForOnProjectileCreatedCallback(Action<IProjectile> callback)
        {
            OnProjectileCreatedCallback -= callback;
        }

        //Ability Execution
        private void OnAbilityAdded(IAbility ability) 
        {
            IBuildModeAbility buildModeAbility = ability as IBuildModeAbility;
            if (buildModeAbility != null)
            {
                buildModeAbility.RegisterForOnExecutedCallback(OnBuildModeAbilityExecuted);
            }
        }

        private void OnBuildModeAbilityExecuted(IBuildModeAbility ability)
        {
            OnBuildModeAbilityExecutedCallback?.Invoke(ability);
        }

        public void RegisterForOnBuildModeAbilityExecutedCallback(Action<IBuildModeAbility> callback)
        {
            OnBuildModeAbilityExecutedCallback += callback;
        }

        public void DeregisterForOnBuildModeAbilityExecutedCallback(Action<IBuildModeAbility> callback)
        {
            OnBuildModeAbilityExecutedCallback -= callback;
        }
    }
}