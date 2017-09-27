using UnityEngine;

namespace GrimoireTD.Abilities.BuildMode
{
    [CreateAssetMenu(fileName = "NewAutoTargetedComponentTemplate", menuName = "Build Mode Abilities/Auto Targeted/Auto Targeted Component")]
    public class SoAutoTargetedComponentTemplate : SoBuildModeTargetingComponentTemplate, IAutoTargetedComponentTemplate
    {
        [SerializeField]
        private SoBuildModeAutoTargetedArgsTemplate targetingRule;

        public IBuildModeAutoTargetedArgsTemplate TargetingRule
        {
            get
            {
                return targetingRule;
            }
        }

        public override IBuildModeTargetingComponent GenerateTargetingComponent()
        {
            return new CAutoTargetedComponent(this);
        }
    }
}
