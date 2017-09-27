using UnityEngine;

namespace GrimoireTD.Abilities.BuildMode
{
    [CreateAssetMenu(fileName = "NewPlayerTargetsDefendingEntityComponent", menuName = "Build Mode Abilities/Player Targeted/Defending Entity Targeted Component")]
    public class SoPlayerTargetsDefendingEntityComponent : SoPlayerTargetedComponentTemplate, IPlayerTargetsDefendingEntityComponentTemplate
    {
        [SerializeField]
        private SoPlayerTargetsDefendingEntityArgsTemplate targetingRule;

        public IPlayerTargetsDefendingEntityArgsTemplate TargetingRule
        {
            get
            {
                return targetingRule;
            }
        }

        public override IBuildModeTargetingComponent GenerateTargetingComponent()
        {
            return new CPlayerTargetsDefendingEntityComponent(this);
        }
    }
}