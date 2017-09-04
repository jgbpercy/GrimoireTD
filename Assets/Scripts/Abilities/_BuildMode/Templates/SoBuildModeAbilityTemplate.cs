using UnityEngine;
using GrimoireTD.DefendingEntities;
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
        private SoBuildModeTargetingComponent targetingComponent;

        [SerializeField]
        private SoBuildModeEffectComponentTemplate[] effectComponents;

        public IEconomyTransaction Cost
        {
            get
            {
                return cost;
            }
        }

        public IBuildModeTargetingComponent TargetingComponent
        {
            get
            {
                return targetingComponent;
            }
        }

        public IEnumerable<IBuildModeEffectComponentTemplate> EffectComponents
        {
            get
            {
                return effectComponents;
            }
        }

        public override IAbility GenerateAbility(IDefendingEntity attachedToDefendingEntity)
        {
            return new CBuildModeAbility(this);
        }
    }
}