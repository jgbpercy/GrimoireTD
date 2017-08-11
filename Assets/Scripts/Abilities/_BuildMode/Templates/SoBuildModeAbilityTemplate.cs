using UnityEngine;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Economy;

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
        private SoBuildModeEffectComponent effectComponent;

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

        public IBuildModeEffectComponent EffectComponent
        {
            get
            {
                return effectComponent;
            }
        }

        public override Ability GenerateAbility(DefendingEntity attachedToDefendingEntity)
        {
            return new BuildModeAbility(this, attachedToDefendingEntity);
        }
    }
}