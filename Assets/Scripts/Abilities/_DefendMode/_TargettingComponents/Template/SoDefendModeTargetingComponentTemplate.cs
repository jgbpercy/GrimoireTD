using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode
{
    [CreateAssetMenu(fileName = "NewDefendModeTargetingComponent", menuName = "Defend Mode Abilities/Targeting Components/Targeting Component")]
    public class SoDefendModeTargetingComponentTemplate : ScriptableObject, IDefendModeTargetingComponentTemplate
    {
        [SerializeField]
        private SoDefendModeTargetingArgsTemplate targetingRule;

        public IDefendModeTargetingArgsTemplate TargetingRule
        {
            get
            {
                return targetingRule;
            }
        }

        public IDefendModeTargetingComponent GenerateTargetingComponent()
        {
            return new CDefendModeTargetingComponent(this);
        }
    }
}