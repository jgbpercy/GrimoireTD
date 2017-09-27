using UnityEngine;

namespace GrimoireTD.Abilities.BuildMode
{
    [CreateAssetMenu(fileName = "NewPlayerTargetsHexComponent", menuName = "Build Mode Abilities/Player Targeted/Hex Targeted Component")]
    public class SoPlayerTargetsHexComponent : SoPlayerTargetedComponentTemplate, IPlayerTargetsHexComponentTemplate
    {
        [SerializeField]
        private SoPlayerTargetsHexArgsTemplate targetingRule;

        public IPlayerTargetsHexArgsTemplate TargetingRule
        {
            get
            {
                return targetingRule;
            }
        }

        public override IBuildModeTargetingComponent GenerateTargetingComponent()
        {
            return new CPlayerTargetsHexComponent(this);
        }
    }
}