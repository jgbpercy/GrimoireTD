﻿using System;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.Abilities.DefendMode.AttackEffects;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Technical;
using GrimoireTD.Map;
using GrimoireTD.ChannelDebug;
using GrimoireTD.Attributes;
using GrimoireTD.TemporaryEffects;

namespace GrimoireTD.Creeps
{
    public class Creep : IDefendModeTargetable, IFrameUpdatee
    {
        private int id;
        
        //Template
        private ICreepTemplate creepTemplate;

        //TemporaryEffects
        private TemporaryEffectsManager temporaryEffects;

        //Pathing/Position
        private Vector3 currentDestinationVector;
        private int currentDestinationPathNode;

        private Vector3 position;
        private float distanceFromEnd;

        //Attributes
        private Attributes<CreepAttributeName> attributes;

        //Resistances
        private Resistances resistances;

        //Health
        private int currentHitpoints;

        private Action OnHealthChangedCallback;
        private Action OnDiedCallback;

        //Properties
        //Name/Id
        public string Id
        {
            get
            {
                return "C-" + id;
            }
        }
        
        public string NameInGame
        {
            get
            {
                return creepTemplate.NameInGame;
            }
        }

        //Template
        public ICreepTemplate CreepTemplate
        {
            get
            {
                return creepTemplate;
            }
        }

        //Temporary Effects
        public IReadOnlyTemporaryEffectsManager TemporaryEffects
        {
            get
            {
                return temporaryEffects;
            }
        }

        //Pathing/Position
        public Vector3 Position
        {
            get
            {
                return position;
            }
        }

        public float DistanceFromEnd
        {
            get
            {
                return distanceFromEnd;
            }
        }

        //Resistances
        public IReadOnlyResistances Resistances
        {
            get
            {
                return resistances;
            }
        }

        //Attributes
        public float CurrentSpeed
        {
            get
            {
                return attributes.GetAttribute(CreepAttributeName.rawSpeed) * (1 + attributes.GetAttribute(CreepAttributeName.speedMultiplier));
            }
        }

        public float CurrentArmor
        {
            get
            {
                return attributes.GetAttribute(CreepAttributeName.rawArmor) * (1 + attributes.GetAttribute(CreepAttributeName.armorMultiplier));
            }
        }

        //Health
        public int CurrentHitpoints
        {
            get
            {
                return currentHitpoints;
            }
        }

        //Constructor
        public Creep(ICreepTemplate template, Vector3 spawnPosition)
        {
            id = IdGen.GetNextId();
            
            //Template
            creepTemplate = template;

            //Temporary Effects
            temporaryEffects = new TemporaryEffectsManager();

            //Pathing/Position
            position = spawnPosition;

            currentDestinationPathNode = MapGenerator.Instance.Map.CreepPath.Count - 2;
            currentDestinationVector = MapGenerator.Instance.Map.CreepPath[currentDestinationPathNode].ToPositionVector();
            
            //Resistances
            resistances = new Resistances(creepTemplate.BaseResistances);

            //Attributes
            attributes = new Attributes<CreepAttributeName>(CreepAttributes.NewAttributesDictionary());

            //  TODO: put this inside the attributes ctor like resistances
            foreach (INamedAttributeModifier<CreepAttributeName> attributeModifier in creepTemplate.BaseAttributes)
            {
                attributes.AddModifier(attributeModifier);
            }

            //Health
            currentHitpoints = template.MaxHitpoints;

            //Bullshit
            ModelObjectFrameUpdater.Instance.RegisterAsModelObjectFrameUpdatee(this);

            CreepView.Instance.CreateCreep(this);
        }

        public Vector3 TargetPosition()
        {
            //TODO unhardcode this height
            return new Vector3(position.x, position.y, -0.4f);
        }

        public void ModelObjectFrameUpdate()
        {
            float distanceFromCurrentDestination = Vector3.Magnitude(position - currentDestinationVector);

            distanceFromEnd = currentDestinationPathNode * MapRenderer.HEX_OFFSET + distanceFromCurrentDestination;

            if (distanceFromCurrentDestination < CurrentSpeed * Time.deltaTime)
            {
                currentDestinationPathNode = currentDestinationPathNode - 1 < 0 ? 0 : currentDestinationPathNode - 1;
                currentDestinationVector = MapGenerator.Instance.Map.CreepPath[currentDestinationPathNode].ToPositionVector();
            }

            position = Vector3.MoveTowards(position, currentDestinationVector, CurrentSpeed * Time.deltaTime);
        }

        public void ApplyAttackEffects(IEnumerable<AttackEffect> attackEffects, DefendingEntity sourceDefendingEntity)
        {
            CDebug.Log(CDebug.combatLog, Id + " receiving attack effects:");

            foreach (AttackEffect attackEffect in attackEffects)
            {
                CDebug.Log(CDebug.combatLog, "Applying attack effect: " + attackEffect.EffectName +
                    ", base magnitude: " + attackEffect.BaseMagnitude +
                    ", base duration: " + attackEffect.BaseDuration +
                    ", actual magnitude: " + attackEffect.GetActualMagnitude(sourceDefendingEntity) +
                    ", actual duration: " + attackEffect.GetActualDuration(sourceDefendingEntity)
                );

                DamageEffectType damageEffectType = attackEffect.AttackEffectType as DamageEffectType;
                if (damageEffectType != null)
                {
                    ApplyDamageEffect(attackEffect, sourceDefendingEntity, damageEffectType);
                    continue;
                }

                ModifierEffectType modifierEffectType = attackEffect.AttackEffectType as ModifierEffectType;
                if (modifierEffectType != null)
                {
                    if (modifierEffectType.Temporary)
                    {
                        ApplyTemporaryEffect(attackEffect, sourceDefendingEntity, modifierEffectType);
                    }
                    else
                    {
                        ApplyPermanentEffect(attackEffect, sourceDefendingEntity, modifierEffectType);
                    }
                    continue;
                }

                throw new Exception("Unhandled AttackEffectType");
            }
        }

        private void ApplyDamageEffect(AttackEffect attackEffect, DefendingEntity sourceDefendingEntity, DamageEffectType damageEffectType)
        {
            float magnitude = attackEffect.GetActualMagnitude(sourceDefendingEntity);
            int block = resistances.GetBlock(damageEffectType);
            float resistance = resistances.GetResistance(damageEffectType, CurrentArmor);

            CDebug.Log(CDebug.combatLog, "Bl: " + block + ", Res: " + resistance);

            TakeDamage(
                Mathf.RoundToInt(
                    (magnitude - block) * (1 - resistance)
                )
            );
        }

        private void ApplyTemporaryEffect(AttackEffect attackEffect, DefendingEntity sourceDefendingEntity, ModifierEffectType modifierEffectType)
        {
            float newEffectMagnitude = attackEffect.GetActualMagnitude(sourceDefendingEntity);
            float newEffectDuration = attackEffect.GetActualDuration(sourceDefendingEntity);

            AttributeModifierEffectType attributeModifierEffectType = modifierEffectType as AttributeModifierEffectType;
            if (attributeModifierEffectType != null)
            {
                SNamedCreepAttributeModifier attributeModifier = new SNamedCreepAttributeModifier(
                    newEffectMagnitude,
                    attributeModifierEffectType.CreepAttributeName
                );

                temporaryEffects.ApplyEffect(
                    attackEffect.AttackEffectType,
                    newEffectMagnitude,
                    newEffectDuration,
                    attackEffect.AttackEffectType.EffectName(),
                    () => { attributes.AddModifier(attributeModifier); },
                    () => { attributes.TryRemoveModifier(attributeModifier); }
                );

                return;
            }

            ResistanceModifierEffectType resistanceModifierEffectType = modifierEffectType as ResistanceModifierEffectType;
            if (resistanceModifierEffectType != null)
            {
                SResistanceModifier resistanceModifier = new SResistanceModifier(
                    newEffectMagnitude, 
                    resistanceModifierEffectType.ResistanceToModify
                );

                temporaryEffects.ApplyEffect(
                    attackEffect.AttackEffectType,
                    newEffectMagnitude,
                    newEffectDuration,
                    attackEffect.AttackEffectType.EffectName(),
                    () => { resistances.AddResistanceModifier(resistanceModifier); },
                    () => { resistances.RemoveResistanceModifer(resistanceModifier); }
                );

                return;
            }

            BlockModifierEffectType blockModifierEffectType = modifierEffectType as BlockModifierEffectType;
            if (blockModifierEffectType != null)
            {
                SBlockModifier blockModifier = new SBlockModifier(
                    Mathf.RoundToInt(newEffectMagnitude),
                    blockModifierEffectType.BlockTypeToModify
                );

                temporaryEffects.ApplyEffect(
                    attackEffect.AttackEffectType,
                    newEffectMagnitude,
                    newEffectDuration,
                    attackEffect.AttackEffectType.EffectName(),
                    () => { resistances.AddBlockModifier(blockModifier); },
                    () => { resistances.RemoveBlockModifier(blockModifier); }
                );
            }

                throw new Exception("Unhandled ModifierEffectType");
        }

        private void ApplyPermanentEffect(AttackEffect attackEffect, DefendingEntity sourceDefendingEntity, ModifierEffectType modifierEffectType)
        {
            float actualMagnitude = attackEffect.GetActualMagnitude(sourceDefendingEntity);

            AttributeModifierEffectType attributeModifierEffectType = modifierEffectType as AttributeModifierEffectType;
            if (attributeModifierEffectType != null)
            {
                SNamedCreepAttributeModifier attributeModifier = new SNamedCreepAttributeModifier(
                    actualMagnitude,
                    attributeModifierEffectType.CreepAttributeName
                );

                attributes.AddModifier(attributeModifier);

                return;
            }

            ResistanceModifierEffectType resistanceModifierEffectType = modifierEffectType as ResistanceModifierEffectType;
            if (resistanceModifierEffectType != null)
            {
                SResistanceModifier resistanceModifier = new SResistanceModifier(
                    actualMagnitude,
                    resistanceModifierEffectType.ResistanceToModify
                );

                resistances.AddResistanceModifier(resistanceModifier);

                return;
            }

            BlockModifierEffectType blockModifierEffectType = modifierEffectType as BlockModifierEffectType;
            if (blockModifierEffectType != null)
            {
                SBlockModifier blockModifier = new SBlockModifier(
                    Mathf.RoundToInt(actualMagnitude),
                    resistanceModifierEffectType.ResistanceToModify
                );

                resistances.AddBlockModifier(blockModifier);

                return;
            }

            throw new Exception("Unhandled ModifierEffectType");
        }

        private void TakeDamage(int damage)
        {
            currentHitpoints -= damage;

            CDebug.Log(CDebug.combatLog, Id + " takes " + damage + " damage, leaving " + CurrentHitpoints);

            if (currentHitpoints < 1)
            {
                currentHitpoints = 0;
            }

            OnHealthChangedCallback();

            if (currentHitpoints == 0)
            {
                CDebug.Log(CDebug.combatLog, Id + " died");
                OnDiedCallback();
                CreepTemplate.Bounty.DoTransaction();
            }
        }

        //TODO: make callback? Or change? Seems bad
        public void GameObjectDestroyed()
        {
            ModelObjectFrameUpdater.Instance.DeregisterAsModelObjectFrameUpdatee(this);
        }

        //Callbacks
        public void RegisterForOnHealthChangedCallback(Action callback)
        {
            OnHealthChangedCallback += callback;
        }

        public void DeregisterForOnHealthChangedCallback(Action callback)
        {
            OnHealthChangedCallback -= callback;
        }

        public void RegisterForOnDiedCallback(Action callback)
        {
            OnDiedCallback += callback;
        }

        public void DeregisterForOnDiedCallback(Action callback)
        {
            OnDiedCallback -= callback;
        }
    }
}