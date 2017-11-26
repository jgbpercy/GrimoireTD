using System;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Defenders;
using GrimoireTD.Technical;
using GrimoireTD.Attributes;
using GrimoireTD.Dependencies;

namespace GrimoireTD.Abilities.DefendMode
{
    public class CDefendModeAbility : CAbility, IDefendModeAbility 
    {
        private int id;

        public IDefendModeAbilityTemplate DefendModeAbilityTemplate { get; }

        private IDefendModeTargetingComponent targetingComponent; 

        private List<IDefendModeEffectComponent> effectComponents;

        public float TimeSinceExecuted { get; private set; }
        public float ActualCooldown { get; private set; }

        public event EventHandler<EAOnAbilityOffCooldown> OnAbilityOffCooldown;

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

        public bool IsOffCooldown
        {
            get
            {
                return TimeSinceExecuted > ActualCooldown;
            }
        }

        public string Id
        {
            get
            {
                return "DefAb-" + id;
            }
        }

        public CDefendModeAbility(IDefendModeAbilityTemplate template, IDefender attachedToDefender) : base(template)
        {
            DepsProv.TheModelObjectFrameUpdater().Register(ModelObjectFrameUpdate);

            DepsProv.TheGameStateManager.OnEnterDefendMode += OnEnterDefendMode;

            id = IdGen.GetNextId();

            TimeSinceExecuted = 0f;
            DefendModeAbilityTemplate = template;

            ActualCooldown = GetCooldownAfterReduction(
                attachedToDefender.Attributes.Get(DeAttrName.cooldownReduction).Value()
            );

            attachedToDefender.Attributes.Get(DeAttrName.cooldownReduction)
                .OnAttributeChanged += OnAttachedDefenderCooldownReductionChange;

            targetingComponent = template.TargetingComponentTemplate.GenerateTargetingComponent();

            effectComponents = new List<IDefendModeEffectComponent>();

            foreach (var effectComponentTemplate in template.EffectComponentTemplates)
            {
                effectComponents.Add(effectComponentTemplate.GenerateEffectComponent());
            }
        }

        private void ModelObjectFrameUpdate(float deltaTime)
        {
            if (DepsProv.TheGameStateManager.CurrentGameMode == GameMode.BUILD)
            {
                return;
            }

            var wasOffCooldown = IsOffCooldown;

            TimeSinceExecuted += deltaTime;

            if (!wasOffCooldown && IsOffCooldown)
            {
                OnAbilityOffCooldown?.Invoke(this, new EAOnAbilityOffCooldown(this));
            }
        }

        public override string UIText()
        {
            return DefendModeAbilityTemplate.NameInGame;
        }

        public bool ExecuteAbility(IDefender attachedToDefender)
        {
            var targetList = targetingComponent.FindTargets(
                attachedToDefender
            );

            if (targetList == null)
            {
                return false;
            }

            foreach (var effectComponent in effectComponents)
            {
                effectComponent.ExecuteEffect(attachedToDefender, targetList);
            }

            TimeSinceExecuted = 0f;

            OnAbilityExecutedVirtual(new EAOnAbilityExecuted(this));

            return true;
        }

        private float GetCooldownAfterReduction(float cooldownReduction)
        {
            return DefendModeAbilityTemplate.BaseCooldown * (1 - cooldownReduction);
        }

        private void OnAttachedDefenderCooldownReductionChange(object sender, EAOnAttributeChanged args)
        {
            float newCooldown = GetCooldownAfterReduction(args.NewValue);

            TimeSinceExecuted = newCooldown * PercentOfCooldownPassed;

            ActualCooldown = newCooldown;
        }

        private void OnEnterDefendMode(object sender, EAOnEnterDefendMode args)
        {
            if (!IsOffCooldown)
            {
                OnAbilityOffCooldown?.Invoke(this, new EAOnAbilityOffCooldown(this));

                TimeSinceExecuted = ActualCooldown + 0.01f;
            }
        }
    }
}