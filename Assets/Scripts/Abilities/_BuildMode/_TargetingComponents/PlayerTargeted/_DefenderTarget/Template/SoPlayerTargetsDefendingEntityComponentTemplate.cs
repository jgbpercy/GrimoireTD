using UnityEngine;

namespace GrimoireTD.Abilities.BuildMode
{
    [CreateAssetMenu(fileName = "NewPlayerTargetsDefenderComponent", menuName = "Build Mode Abilities/Player Targeted/Defender Targeted Component")]
    public class SoPlayerTargetsDefenderComponent : SoPlayerTargetedComponentTemplate, IPlayerTargetsDefenderComponentTemplate
    {
        [SerializeField]
        private SoPlayerTargetsDefenderArgsTemplate targetingRule;

        public IPlayerTargetsDefenderArgsTemplate TargetingRule
        {
            get
            {
                return targetingRule;
            }
        }

        public override IBuildModeTargetingComponent GenerateTargetingComponent()
        {
            return new CPlayerTargetsDefenderComponent(this);
        }
    }
}