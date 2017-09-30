using System;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.AttackEffects;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Technical;
using GrimoireTD.Map;
using GrimoireTD.ChannelDebug;
using GrimoireTD.Attributes;
using GrimoireTD.TemporaryEffects;

namespace GrimoireTD.Creeps
{
    public class CCreep : ICreep
    {
        private int id;
        
        //Template
        public ICreepTemplate CreepTemplate { get; }

        //TemporaryEffects
        private ITemporaryEffectsManager temporaryEffects;

        //Pathing/Position
        private Vector3 currentDestinationVector;
        private int currentDestinationPathNode;

        public Vector3 Position { get; private set; }
        public float DistanceFromEnd { get; private set; }

        //Attributes
        private IAttributes<CreepAttributeName> attributes;

        //Resistances
        private IResistances resistances;

        //Health
        public int CurrentHitpoints { get; private set; }

        public event EventHandler OnDied;
        public event EventHandler<EAOnHealthChanged> OnHealthChanged;
        
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
                return CreepTemplate.NameInGame;
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

        //Attributes
        public IReadOnlyAttributes<CreepAttributeName> Attributes
        {
            get
            {
                return attributes;
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
        //TODO: cache these two and update on change event
        public float CurrentSpeed
        {
            get
            {
                return attributes.GetAttribute(CreepAttributeName.rawSpeed).Value() * (1 + attributes.GetAttribute(CreepAttributeName.speedMultiplier).Value());
            }
        }

        public float CurrentArmor
        {
            get
            {
                return attributes.GetAttribute(CreepAttributeName.rawArmor).Value() * (1 + attributes.GetAttribute(CreepAttributeName.armorMultiplier).Value());
            }
        }

        //Constructor
        public CCreep(ICreepTemplate template, Vector3 spawnPosition)
        {
            id = IdGen.GetNextId();
            
            //Template
            CreepTemplate = template;

            //Temporary Effects
            temporaryEffects = new CTemporaryEffectsManager();

            //Pathing/Position
            Position = spawnPosition;

            currentDestinationPathNode = GameModels.Models[0].MapData.CreepPath.Count - 2;
            currentDestinationVector = GameModels.Models[0].MapData.CreepPath[currentDestinationPathNode].ToPositionVector();
            
            //Resistances
            resistances = new CResistances(CreepTemplate.BaseResistances);

            //Attributes
            attributes = new CAttributes<CreepAttributeName>(CreepAttributes.NewAttributesDictionary());

            //  TODO: put this inside the attributes ctor like resistances
            foreach (INamedAttributeModifier<CreepAttributeName> attributeModifier in CreepTemplate.BaseAttributes)
            {
                attributes.AddModifier(attributeModifier);
            }

            //Health
            CurrentHitpoints = template.MaxHitpoints;

            //Bullshit
            ModelObjectFrameUpdater.Instance.RegisterAsModelObjectFrameUpdatee(this);
        }

        public Vector3 TargetPosition()
        {
            //TODO unhardcode this height
            return new Vector3(Position.x, Position.y, -0.4f);
        }

        public void ModelObjectFrameUpdate(float deltaTime)
        {
            float distanceFromCurrentDestination = Vector3.Magnitude(Position - currentDestinationVector);

            DistanceFromEnd = currentDestinationPathNode * MapRenderer.HEX_OFFSET + distanceFromCurrentDestination;

            if (distanceFromCurrentDestination < CurrentSpeed * deltaTime)
            {
                currentDestinationPathNode = currentDestinationPathNode - 1 < 0 ? 0 : currentDestinationPathNode - 1;
                currentDestinationVector = GameModels.Models[0].MapData.CreepPath[currentDestinationPathNode].ToPositionVector();
            }

            Position = Vector3.MoveTowards(Position, currentDestinationVector, CurrentSpeed * deltaTime);
        }

        public void ApplyAttackEffects(IEnumerable<IAttackEffect> attackEffects, IDefendingEntity sourceDefendingEntity)
        {
            CDebug.Log(CDebug.combatLog, Id + " receiving attack effects:");

            foreach (IAttackEffect attackEffect in attackEffects)
            {
                CDebug.Log(CDebug.combatLog, "Applying attack effect: " + attackEffect.EffectName +
                    ", base magnitude: " + attackEffect.BaseMagnitude +
                    ", base duration: " + attackEffect.BaseDuration +
                    ", actual magnitude: " + attackEffect.GetActualMagnitude(sourceDefendingEntity) +
                    ", actual duration: " + attackEffect.GetActualDuration(sourceDefendingEntity)
                );

                IDamageEffectType damageEffectType = attackEffect.AttackEffectType as IDamageEffectType;
                if (damageEffectType != null)
                {
                    ApplyDamageEffect(attackEffect, sourceDefendingEntity, damageEffectType);
                    continue;
                }

                IModifierEffectType modifierEffectType = attackEffect.AttackEffectType as IModifierEffectType;
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

        private void ApplyDamageEffect(IAttackEffect attackEffect, IDefendingEntity sourceDefendingEntity, IDamageEffectType damageEffectType)
        {
            float magnitude = attackEffect.GetActualMagnitude(sourceDefendingEntity);
            int block = resistances.GetBlock(damageEffectType).Value;
            float resistance = resistances.GetResistanceAfterArmor(damageEffectType, CurrentArmor);

            CDebug.Log(CDebug.combatLog, "Bl: " + block + ", Res: " + resistance);

            TakeDamage(
                Mathf.RoundToInt(
                    (magnitude - block) * (1 - resistance)
                )
            );
        }

        private void ApplyTemporaryEffect(IAttackEffect attackEffect, IDefendingEntity sourceDefendingEntity, IModifierEffectType modifierEffectType)
        {
            float newEffectMagnitude = attackEffect.GetActualMagnitude(sourceDefendingEntity);
            float newEffectDuration = attackEffect.GetActualDuration(sourceDefendingEntity);

            IAttributeModifierEffectType attributeModifierEffectType = modifierEffectType as IAttributeModifierEffectType;
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
                    (object sender, EAOnTemporaryEffectEnd args) => { attributes.TryRemoveModifier(attributeModifier); }
                );

                return;
            }

            IResistanceModifierEffectType resistanceModifierEffectType = modifierEffectType as IResistanceModifierEffectType;
            if (resistanceModifierEffectType != null)
            {
                IResistanceModifier resistanceModifier = new CResistanceModifier(
                    newEffectMagnitude, 
                    resistanceModifierEffectType.ResistanceToModify
                );

                temporaryEffects.ApplyEffect(
                    attackEffect.AttackEffectType,
                    newEffectMagnitude,
                    newEffectDuration,
                    attackEffect.AttackEffectType.EffectName(),
                    () => { resistances.AddResistanceModifier(resistanceModifier); },
                    (object sender, EAOnTemporaryEffectEnd args) => { resistances.RemoveResistanceModifer(resistanceModifier); }
                );

                return;
            }

            IBlockModifierEffectType blockModifierEffectType = modifierEffectType as IBlockModifierEffectType;
            if (blockModifierEffectType != null)
            {
                IBlockModifier blockModifier = new CBlockModifier(
                    Mathf.RoundToInt(newEffectMagnitude),
                    blockModifierEffectType.BlockTypeToModify
                );

                temporaryEffects.ApplyEffect(
                    attackEffect.AttackEffectType,
                    newEffectMagnitude,
                    newEffectDuration,
                    attackEffect.AttackEffectType.EffectName(),
                    () => { resistances.AddBlockModifier(blockModifier); },
                    (object sender, EAOnTemporaryEffectEnd args) => { resistances.RemoveBlockModifier(blockModifier); }
                );
            }

                throw new Exception("Unhandled ModifierEffectType");
        }

        private void ApplyPermanentEffect(IAttackEffect attackEffect, IDefendingEntity sourceDefendingEntity, IModifierEffectType modifierEffectType)
        {
            float actualMagnitude = attackEffect.GetActualMagnitude(sourceDefendingEntity);

            IAttributeModifierEffectType attributeModifierEffectType = modifierEffectType as IAttributeModifierEffectType;
            if (attributeModifierEffectType != null)
            {
                SNamedCreepAttributeModifier attributeModifier = new SNamedCreepAttributeModifier(
                    actualMagnitude,
                    attributeModifierEffectType.CreepAttributeName
                );

                attributes.AddModifier(attributeModifier);

                return;
            }

            IResistanceModifierEffectType resistanceModifierEffectType = modifierEffectType as IResistanceModifierEffectType;
            if (resistanceModifierEffectType != null)
            {
                IResistanceModifier resistanceModifier = new CResistanceModifier(
                    actualMagnitude,
                    resistanceModifierEffectType.ResistanceToModify
                );

                resistances.AddResistanceModifier(resistanceModifier);

                return;
            }

            IBlockModifierEffectType blockModifierEffectType = modifierEffectType as IBlockModifierEffectType;
            if (blockModifierEffectType != null)
            {
                IBlockModifier blockModifier = new CBlockModifier(
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
            CurrentHitpoints -= damage;

            CDebug.Log(CDebug.combatLog, Id + " takes " + damage + " damage, leaving " + CurrentHitpoints);

            if (CurrentHitpoints < 1)
            {
                CurrentHitpoints = 0;
            }

            OnHealthChanged?.Invoke(this, new EAOnHealthChanged(CurrentHitpoints));

            if (CurrentHitpoints == 0)
            {
                CDebug.Log(CDebug.combatLog, Id + " died");

                OnDied?.Invoke(this, EventArgs.Empty);
            }
        }

        //TODO: make callback? Or change? Seems bad
        public void GameObjectDestroyed()
        {
            ModelObjectFrameUpdater.Instance.DeregisterAsModelObjectFrameUpdatee(this);
        }
    }
}