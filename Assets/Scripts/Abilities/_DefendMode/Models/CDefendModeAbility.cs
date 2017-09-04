﻿using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Technical;
using GrimoireTD.ChannelDebug;
using GrimoireTD.Attributes;

namespace GrimoireTD.Abilities.DefendMode
{
    public class CDefendModeAbility : CAbility, IDefendModeAbility, IFrameUpdatee 
    {
        private int id;

        public IDefendModeAbilityTemplate DefendModeAbilityTemplate { get; }

        public float TimeSinceExecuted { get; private set; }
        public float ActualCooldown { get; private set; }

        private IDefendingEntity attachedToDefendingEntity;

        public float TimeSinceExecutedClamped
        {
            get
            {
                return Mathf.Clamp(TimeSinceExecuted, 0f, ActualCooldown);
            }
        }

        public float PercentOfCooldownPassed
        {
            get
            {
                return Mathf.Clamp(TimeSinceExecuted / ActualCooldown, 0f, 1f);
            }
        }

        public string Id
        {
            get
            {
                return "DefAb-" + id;
            }
        }

        public CDefendModeAbility(IDefendModeAbilityTemplate template, IDefendingEntity attachedToDefendingEntity) : base(template)
        {
            id = IdGen.GetNextId();

            TimeSinceExecuted = 0f;
            DefendModeAbilityTemplate = template;

            this.attachedToDefendingEntity = attachedToDefendingEntity;

            ActualCooldown = GetActualCooldown();

            attachedToDefendingEntity.Attributes.GetAttribute(DefendingEntityAttributeName.cooldownReduction).OnAttributeChanged += OnAttachedDefendingEntityCooldownReductionChange;

            ModelObjectFrameUpdater.Instance.RegisterAsModelObjectFrameUpdatee(this);
        }

        public override string UIText()
        {
            return DefendModeAbilityTemplate.NameInGame;
        }

        public bool ExecuteAbility(IDefendingEntity attachedToDefendingEntity)
        {
            IReadOnlyList<IDefendModeTargetable> targetList = DefendModeAbilityTemplate.TargetingComponent.FindTargets(attachedToDefendingEntity);

            if (targetList == null) { return false; }

            DefendModeAbilityTemplate.EffectComponent.ExecuteEffect(attachedToDefendingEntity, targetList);
            return true;
        }

        private float GetActualCooldown()
        {
            float cooldown = GetCooldownAfterReduction(attachedToDefendingEntity.Attributes.GetAttribute(DefendingEntityAttributeName.cooldownReduction).Value());

            CDebug.Log(CDebug.unitAttributes, 
                Id + 
                " (" + DefendModeAbilityTemplate.NameInGame + ") " + 
                "cooldown: " + cooldown + 
                " = " + DefendModeAbilityTemplate.BaseCooldown + 
                " * " + (1 - attachedToDefendingEntity.Attributes.GetAttribute(DefendingEntityAttributeName.cooldownReduction).Value())
            );

            return cooldown;
        }

        private float GetCooldownAfterReduction(float cooldownReduction)
        {
            return DefendModeAbilityTemplate.BaseCooldown * (1 - cooldownReduction);
        }

        private void OnAttachedDefendingEntityCooldownReductionChange(object sender, EAOnAttributeChanged args)
        {
            float newCooldown = GetCooldownAfterReduction(args.NewValue);

            TimeSinceExecuted = newCooldown * PercentOfCooldownPassed;

            ActualCooldown = newCooldown;
        }

        public bool OffCooldown()
        {
            return TimeSinceExecuted > ActualCooldown;
        }

        public void ModelObjectFrameUpdate()
        {
            TimeSinceExecuted += Time.deltaTime;
        }

        public void WasExecuted()
        {
            TimeSinceExecuted = 0f;
        }
    }
}