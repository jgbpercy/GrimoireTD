using System;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.AttackEffects;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Technical;
using GrimoireTD.Map;
using GrimoireTD.Attributes;
using GrimoireTD.TemporaryEffects;
using GrimoireTD.Dependencies;

namespace GrimoireTD.Creeps
{
    public class CCreep : ICreep
    {
        public const float TARGET_POSITION_Z_OFFSET = -0.4f;

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
        private IAttributes<CreepAttrName> attributes;

        public float CurrentSpeed { get; private set; }
        public float CurrentArmor { get; private set; }

        //Resistances
        private IResistances resistances;

        //Health
        public int CurrentHitpoints { get; private set; }

        public event EventHandler<EventArgs> OnDied;
        public event EventHandler<EAOnHealthChanged> OnHealthChanged;

        public event EventHandler<EAOnAttributeChanged> OnArmorChanged;

        public event EventHandler<EAOnAttributeChanged> OnSpeedChanged;

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
        public IReadOnlyAttributes<CreepAttrName> Attributes
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

        //Constructor
        public CCreep(ICreepTemplate template, Vector3 spawnPosition)
        {
            id = IdGen.GetNextId();
            
            //Template
            CreepTemplate = template;

            //Temporary Effects
            temporaryEffects = DependencyProvider.TemporaryEffectsManager();

            //Pathing/Position
            Position = spawnPosition;

            currentDestinationPathNode = GameModels.Models[0].MapData.CreepPath.Count - 2;
            currentDestinationVector = GameModels.Models[0].MapData.CreepPath[currentDestinationPathNode].ToPositionVector();

            float distanceFromCurrentDestination = GetDistanceFromCurrentDestination();

            DistanceFromEnd = GetDistanceFromEnd(distanceFromCurrentDestination);

            //Attributes
            attributes = DependencyProvider.CreepAttributes();

            EventHandler<EAOnAttributeChanged> OnArmorAttributeChanged = ((object sender, EAOnAttributeChanged args) =>
            {
                CurrentArmor = attributes.Get(CreepAttrName.rawArmor).Value() * (1 + attributes.Get(CreepAttrName.armorMultiplier).Value());
                OnArmorChanged?.Invoke(this, new EAOnAttributeChanged(CurrentArmor));
            });
            attributes.Get(CreepAttrName.armorMultiplier).OnAttributeChanged += OnArmorAttributeChanged;
            attributes.Get(CreepAttrName.rawArmor).OnAttributeChanged += OnArmorAttributeChanged;

            EventHandler<EAOnAttributeChanged> OnSpeedAttributeChanged = ((object sender, EAOnAttributeChanged args) =>
            {
                CurrentSpeed = attributes.Get(CreepAttrName.rawSpeed).Value() * (1 + attributes.Get(CreepAttrName.speedMultiplier).Value());
                OnSpeedChanged?.Invoke(this, new EAOnAttributeChanged(CurrentSpeed));
            });
            attributes.Get(CreepAttrName.speedMultiplier).OnAttributeChanged += OnSpeedAttributeChanged;
            attributes.Get(CreepAttrName.rawSpeed).OnAttributeChanged += OnSpeedAttributeChanged;

            foreach (INamedAttributeModifier<CreepAttrName> attributeModifier in CreepTemplate.BaseAttributes)
            {
                attributes.AddModifier(attributeModifier);
            }

            //Resistances
            resistances = DependencyProvider.Resistances(this, CreepTemplate.BaseResistances);

            //Health
            CurrentHitpoints = template.MaxHitpoints;

            //Bullshit
            ModelObjectFrameUpdater.Instance.RegisterAsModelObjectFrameUpdatee(this);
        }

        public Vector3 TargetPosition()
        {
            //TODO unhardcode this height
            return new Vector3(Position.x, Position.y, TARGET_POSITION_Z_OFFSET);
        }

        public void ModelObjectFrameUpdate(float deltaTime)
        {
            float distanceFromCurrentDestination = GetDistanceFromCurrentDestination();

            if (distanceFromCurrentDestination <= CurrentSpeed * deltaTime)
            {
                currentDestinationPathNode = currentDestinationPathNode - 1 < 0 ? 0 : currentDestinationPathNode - 1;
                currentDestinationVector = GameModels.Models[0].MapData.CreepPath[currentDestinationPathNode].ToPositionVector();
            }

            float movementAmount = CurrentSpeed * deltaTime;

            distanceFromCurrentDestination -= movementAmount;

            Position = Vector3.MoveTowards(Position, currentDestinationVector, movementAmount);

            DistanceFromEnd = GetDistanceFromEnd(distanceFromCurrentDestination);
        }

        private float GetDistanceFromCurrentDestination()
        {
            return Vector3.Magnitude(Position - currentDestinationVector);
        }

        private float GetDistanceFromEnd(float distanceFromCurrentDestination)
        {
            return currentDestinationPathNode * MapRenderer.HEX_OFFSET + distanceFromCurrentDestination;
        }

        public void ApplyAttackEffects(IEnumerable<IAttackEffect> attackEffects, IDefendingEntity sourceDefendingEntity)
        {
            foreach (IAttackEffect attackEffect in attackEffects)
            {
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

        //TODO: #optimisation total up damage from all effects and apply?
        private void ApplyDamageEffect(IAttackEffect attackEffect, IDefendingEntity sourceDefendingEntity, IDamageEffectType damageEffectType)
        {
            float magnitude = attackEffect.GetActualMagnitude(sourceDefendingEntity);
            int block = resistances.GetBlock(damageEffectType).Value;
            float resistance = resistances.GetResistance(damageEffectType).Value;

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

                return;
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
                    blockModifierEffectType.BlockTypeToModify
                );

                resistances.AddBlockModifier(blockModifier);

                return;
            }

            throw new Exception("Unhandled ModifierEffectType");
        }

        private void TakeDamage(int damage)
        {
            CurrentHitpoints -= damage;

            if (CurrentHitpoints < 1)
            {
                CurrentHitpoints = 0;
            }

            OnHealthChanged?.Invoke(this, new EAOnHealthChanged(CurrentHitpoints));

            if (CurrentHitpoints == 0)
            {
                OnDied?.Invoke(this, EventArgs.Empty);

                ModelObjectFrameUpdater.Instance.DeregisterAsModelObjectFrameUpdatee(this);
            }
        }
    }
}