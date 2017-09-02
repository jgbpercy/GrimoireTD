using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using GrimoireTD.Abilities;
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
        //Id
        protected int id;

        //Template
        public IDefendingEntityTemplate DefendingEntityTemplate { get; }

        //Attributes
        protected IAttributes<DefendingEntityAttributeName> attributes;

        //Position
        public Coord CoordPosition { get; protected set; }

        //Economy
        private CallbackList<IHexOccupationBonus> flatHexOccupationBonuses;

        public event EventHandler<EAOnTriggeredFlatHexOccupationBonus> OnTriggeredFlatHexOccupationBonus;

        //Abilities
        protected IAbilities abilities;

        //Auras Emitted
        protected CallbackList<IDefenderAura> aurasEmitted;

        //Auras Affected By
        protected CallbackList<IDefenderAura> affectedByDefenderAuras;

        //Projectiles
        public event EventHandler<EAOnProjectileCreated> OnProjectileCreated;

        //UI Stuff
        public abstract string UIText { get; protected set; }

        public abstract string CurrentName { get; protected set; }

        //Properties
        //Id
        public virtual string Id
        {
            get
            {
                return "?-" + id;
            }
        }

        //Attributes
        public IReadOnlyAttributes<DefendingEntityAttributeName> Attributes
        {
            get
            {
                return attributes;
            }
        }
        
        //Economy
        public IReadOnlyCallbackList<IHexOccupationBonus> FlatHexOccupationBonuses
        {
            get
            {
                return flatHexOccupationBonuses;
            }
        }

        //Abilities
        public IReadOnlyAbilities Abilities
        {
            get
            {
                return abilities;
            }
        }

        //Auras Emitted
        public IReadOnlyCallbackList<IDefenderAura> AurasEmitted
        {
            get
            {
                return aurasEmitted;
            }
        }

        //Auras Affected By
        public IReadOnlyCallbackList<IDefenderAura> AffectedByDefenderAuras
        {
            get
            {
                return affectedByDefenderAuras;
            }
        }

        //Helper Properties
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

            DefendingEntityTemplate = template;

            attributes = new CAttributes<DefendingEntityAttributeName>(DefendingEntityAttributes.NewAttributesDictionary());

            abilities = new Abilities.Abilities();

            flatHexOccupationBonuses = new CallbackList<IHexOccupationBonus>();

            aurasEmitted = new CallbackList<IDefenderAura>();
            aurasEmitted.OnAdd += OnInitialiseAura;
            aurasEmitted.OnRemove -= OnRemoveAura;

            CoordPosition = position;
            OnHex.DefenderAurasHere.OnAdd += OnNewDefenderAuraInCurrentHex;
            OnHex.DefenderAurasHere.OnRemove += OnClearDefenderAuraInCurrentHex;

            SetUpAffectedByDefenderAuras();

            GameModels.Models[0].GameStateManager.OnEnterBuildMode += OnEnterBuildMode;
        }

        //Set Up
        protected virtual void SetUpAffectedByDefenderAuras()
        {
            affectedByDefenderAuras = new CallbackList<IDefenderAura>();

            affectedByDefenderAuras.OnAdd += 
                (object sender, EAOnCallbackListAdd<IDefenderAura> args) => ApplyImprovement(args.AddedItem.DefenderEffectTemplate.Improvement);
            affectedByDefenderAuras.OnRemove -= 
                (object sender, EAOnCallbackListRemove<IDefenderAura> args) => RemoveImprovement(args.RemovedItem.DefenderEffectTemplate.Improvement);

            GetDefenderAurasFromCurrentHex();
        }

        protected void GetDefenderAurasFromCurrentHex()
        {
            foreach (IDefenderAura defenderAura in OnHex.DefenderAurasHere)
            {
                affectedByDefenderAuras.Add(defenderAura);
            }
        }

        //Enter Build Mode
        protected virtual void OnEnterBuildMode(object sender, EAOnEnterBuildMode args)
        {
            CDebug.Log(CDebug.hexEconomy, Id + " entered build mode:");

            IEconomyTransaction flatHexOccupationBonus = GetHexOccupationBonus(OnHexType, flatHexOccupationBonuses);

            OnTriggeredFlatHexOccupationBonus?.Invoke(this, new EAOnTriggeredFlatHexOccupationBonus(this, flatHexOccupationBonus));

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
            OnProjectileCreated?.Invoke(this, new EAOnProjectileCreated(projectile));
        }

        //Defender Auras that this is the source of
        protected void OnInitialiseAura(object sender, EAOnCallbackListAdd<IDefenderAura> args)
        {
            List<Coord> affectedCoords = GameModels.Models[0].MapData.CoordsInRange(args.AddedItem.Range, CoordPosition);

            foreach (Coord coord in affectedCoords)
            {
                GameModels.Models[0].MapData.GetHexAt(coord).AddDefenderAura(args.AddedItem);
            }
        }

        private void OnRemoveAura(object sender, EAOnCallbackListRemove<IDefenderAura> args)
        {
            args.RemovedItem.ClearAura();
        }

        //Defender Auras Affected By
        protected abstract void OnNewDefenderAuraInCurrentHex(object sender, EAOnCallbackListAdd<IDefenderAura> args);

        protected abstract void OnClearDefenderAuraInCurrentHex(object sender, EAOnCallbackListRemove<IDefenderAura> args);
    }
}