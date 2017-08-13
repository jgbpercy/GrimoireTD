using System;
using System.Collections.Generic;
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

namespace GrimoireTD.DefendingEntities
{
    public abstract class DefendingEntity : IBuildModeTargetable
    {
        protected int id;

        //Attributes
        private Attributes<DefendingEntityAttributeName> attributes;

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

                if (attachedToDefendingEntity.OnAbilityAddedCallback != null)
                {
                    attachedToDefendingEntity.OnAbilityAddedCallback(ability);
                }
            }

            public bool TryRemoveAbility(Ability ability)
            {
                if (!list.ContainsValue(ability))
                {
                    return false;
                }

                int indexOfAbility = list.IndexOfValue(ability);
                list.RemoveAt(indexOfAbility);

                if (attachedToDefendingEntity.OnAbilityRemovedCallback != null)
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

        public Attributes<DefendingEntityAttributeName> Attributes
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

            attributes = new Attributes<DefendingEntityAttributeName>(DefendingEntityAttributes.NewAttributesDictionary());

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

            IEconomyTransaction flatHexOccupationBonus = GetHexOccupationBonus(OnHexType, flatHexOccupationBonuses);

            flatHexOccupationBonus.DoTransaction();

            CDebug.Log(CDebug.hexEconomy, Id + " added flat occupation bonus " + flatHexOccupationBonus);
        }

        //Improvements
        protected void ApplyImprovement(IDefendingEntityImprovement improvement)
        {
            foreach (INamedAttributeModifier<DefendingEntityAttributeName> attributeModifier in improvement.AttributeModifiers)
            {
                attributes.AddModifier(attributeModifier);
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

            foreach (INamedAttributeModifier<DefendingEntityAttributeName> attributeModifier in improvement.AttributeModifiers)
            {
                wasPresent = attributes.TryRemoveModifier(attributeModifier);
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

        //Economy
        protected IEconomyTransaction GetHexOccupationBonus(IHexType hexType, CallbackList<HexOccupationBonus> occupationBonusList)
        {
            IEconomyTransaction occupationBonusTransaction = new CEconomyTransaction();

            foreach (HexOccupationBonus hexOccupationBonus in occupationBonusList)
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

        //UI Stuff
        public abstract string UIText();

        public abstract string CurrentName();

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
}