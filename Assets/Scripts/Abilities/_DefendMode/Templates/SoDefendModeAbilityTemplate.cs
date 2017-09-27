using System.Collections.Generic;
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
        protected SoDefendModeTargetingComponentTemplate targetingComponentTemplate;

        [SerializeField]
        protected SoDefendModeEffectComponentTemplate[] effectComponentTemplates;

        public float BaseCooldown
        {
            get
            {
                return baseCooldown;
            }
        }

        public IDefendModeTargetingComponentTemplate TargetingComponentTemplate
        {
            get
            {
                return targetingComponentTemplate;
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