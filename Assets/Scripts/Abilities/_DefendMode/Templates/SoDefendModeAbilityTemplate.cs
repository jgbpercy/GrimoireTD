﻿using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.DefendMode
{
    [CreateAssetMenu(fileName = "NewDefendModeAbilityTemplate", menuName = "Defend Mode Abilities/Defend Mode Ability")]
    public class SoDefendModeAbilityTemplate : SoAbilityTemplate, IDefendModeAbilityTemplate
    {
        [SerializeField]
        protected float baseCooldown;

        [SerializeField]
        protected SoDefendModeTargetingComponent targetingComponent;

        [SerializeField]
        protected SoDefendModeEffectComponentTemplate[] effectComponentTemplates;

        public float BaseCooldown
        {
            get
            {
                return baseCooldown;
            }
        }

        public IDefendModeTargetingComponent TargetingComponent
        {
            get
            {
                return targetingComponent;
            }
        }

        public IEnumerable<IDefendModeEffectComponentTemplate> EffectComponentTemplates
        {
            get
            {
                return effectComponentTemplates;
            }
        }

        public override IAbility GenerateAbility(IDefendingEntity attachedToDefendingEntity)
        {
            return new CDefendModeAbility(this, attachedToDefendingEntity);
        }
    }
}