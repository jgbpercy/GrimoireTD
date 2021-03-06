﻿using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using GrimoireTD.Abilities;
using GrimoireTD.Defenders.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Map;
using GrimoireTD.Technical;
using GrimoireTD.Attributes;
using GrimoireTD.Abilities.DefendMode.Projectiles;
using GrimoireTD.Dependencies;

namespace GrimoireTD.Defenders
{
    public abstract class CDefender : IDefender
    {
        //Id
        protected int id;

        //Template
        public IDefenderTemplate DefenderTemplate { get; }

        //Attributes
        protected IAttributes<DeAttrName> attributes;

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
        public IReadOnlyAttributes<DeAttrName> Attributes
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
                return DepsProv.TheMapData.GetHexAt(CoordPosition);
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
        public CDefender(IDefenderTemplate template, Coord position)
        {
            id = IdGen.GetNextId();

            DefenderTemplate = template;

            attributes = DepsProv.DefenderAttributes();

            abilities = DepsProv.Abilities(this);

            flatHexOccupationBonuses = new CallbackList<IHexOccupationBonus>();

            aurasEmitted = new CallbackList<IDefenderAura>();
            aurasEmitted.OnAdd += OnInitialiseAura;
            aurasEmitted.OnRemove += OnRemoveAura;

            CoordPosition = position;
            OnHex.DefenderAurasHere.OnAdd += OnNewDefenderAuraInCurrentHex;
            OnHex.DefenderAurasHere.OnRemove += OnClearDefenderAuraInCurrentHex;

            SetUpAffectedByDefenderAuras();

            DepsProv.TheGameModeManager.OnEnterBuildMode += OnEnterBuildMode;
        }

        //Set Up
        protected virtual void SetUpAffectedByDefenderAuras()
        {
            affectedByDefenderAuras = new CallbackList<IDefenderAura>();

            affectedByDefenderAuras.OnAdd += 
                (object sender, EAOnCallbackListAdd<IDefenderAura> args) => ApplyImprovement(args.AddedItem.DefenderEffectTemplate.Improvement);
            affectedByDefenderAuras.OnRemove += 
                (object sender, EAOnCallbackListRemove<IDefenderAura> args) => RemoveImprovement(args.RemovedItem.DefenderEffectTemplate.Improvement);

            GetDefenderAurasFromCurrentHex();
        }

        protected void GetDefenderAurasFromCurrentHex()
        {
            foreach (var defenderAura in OnHex.DefenderAurasHere)
            {
                OnNewDefenderAuraInCurrentHex(this, new EAOnCallbackListAdd<IDefenderAura>(defenderAura));
            }
        }

        //Enter Build Mode
        protected virtual void OnEnterBuildMode(object sender, EAOnEnterBuildMode args)
        {
            var flatHexOccupationBonus = GetHexOccupationBonus(OnHexType, flatHexOccupationBonuses);

            OnTriggeredFlatHexOccupationBonus?.Invoke(this, new EAOnTriggeredFlatHexOccupationBonus(this, flatHexOccupationBonus));
        }

        //Improvements
        protected virtual void ApplyImprovement(IDefenderImprovement improvement)
        {
            foreach (var attributeModifier in improvement.AttributeModifiers)
            {
                attributes.AddModifier(attributeModifier);
            }

            foreach (var hexOccupationBonus in improvement.FlatHexOccupationBonuses)
            {
                flatHexOccupationBonuses.Add(hexOccupationBonus);
            }

            foreach (var abilityTemplate in improvement.Abilities)
            {
                abilities.AddAbility(abilityTemplate.GenerateAbility(this));
            }

            foreach (var auraTemplate in improvement.Auras)
            {
                aurasEmitted.Add(auraTemplate.GenerateDefenderAura(this));
            }
        }

        protected virtual void RemoveImprovement(IDefenderImprovement improvement)
        {
            bool wasPresent;

            foreach (var attributeModifier in improvement.AttributeModifiers)
            {
                wasPresent = attributes.TryRemoveModifier(attributeModifier);
                Assert.IsTrue(wasPresent);
            }

            foreach (var hexOccupationBonus in improvement.FlatHexOccupationBonuses)
            {
                wasPresent = flatHexOccupationBonuses.TryRemove(hexOccupationBonus);
                Assert.IsTrue(wasPresent);
            }

            foreach (var abilityTemplate in improvement.Abilities)
            {
                wasPresent = abilities.TryRemoveAbility(abilityTemplate);
                Assert.IsTrue(wasPresent);
            }

            foreach (var auraTemplate in improvement.Auras)
            {
                wasPresent = aurasEmitted.TryRemove(auraTemplate.GenerateDefenderAura(this), new CDefenderAura.TemplateEqualityComparer());
                Assert.IsTrue(wasPresent);
            }
        }

        //Economy
        protected IEconomyTransaction GetHexOccupationBonus(IHexType hexType, CallbackList<IHexOccupationBonus> occupationBonusList)
        {
            IEconomyTransaction occupationBonusTransaction = new CEconomyTransaction();

            foreach (var hexOccupationBonus in occupationBonusList)
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
            var mapData = DepsProv.TheMapData;

            var affectedCoords = mapData.GetCoordsInRange(args.AddedItem.Range, CoordPosition);

            foreach (var coord in affectedCoords)
            {
                mapData.GetHexAt(coord).AddDefenderAura(args.AddedItem);
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