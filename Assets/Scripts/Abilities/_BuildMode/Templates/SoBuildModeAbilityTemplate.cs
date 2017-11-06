using UnityEngine;
using GrimoireTD.Defenders;
using GrimoireTD.Economy;
using System.Collections.Generic;

namespace GrimoireTD.Abilities.BuildMode
{
    [CreateAssetMenu(fileName = "NewBuildModeAbilityTemplate", menuName = "Build Mode Abilities/Build Mode Ability")]
    public class SoBuildModeAbilityTemplate : SoAbilityTemplate, IBuildModeAbilityTemplate
    {
        [SerializeField]
        private SEconomyTransaction cost;

        [SerializeField]
        private SoBuildModeTargetingComponentTemplate targetingComponentTemplate;

        [SerializeField]
        private SoBuildModeEffectComponentTemplate[] effectComponentTemplates;

        public IEconomyTransaction Cost
        {
            get
            {
                return cost;
            }
        }

        public IBuildModeTargetingComponentTemplate TargetingComponentTemplate
        {
            get
            {
                return targetingComponentTemplate;
            }
        }

        public IEnumerable<IBuildModeEffectComponentTemplate> EffectComponentTemplates
        {
            get
            {
                return effectComponentTemplates;
            }
        }

        public override IAbility GenerateAbility(IDefender attachedToDefender)
        {
            return new CBuildModeAbility(this);
        }
    }
}