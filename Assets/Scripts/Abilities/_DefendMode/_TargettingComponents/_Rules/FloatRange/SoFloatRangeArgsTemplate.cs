using UnityEngine;
using GrimoireTD.Defenders;
using GrimoireTD.Attributes;

namespace GrimoireTD.Abilities.DefendMode
{
    public class SoFloatRangeArgsTemplate : SoDefendModeTargetingArgsTemplate
    {
        [SerializeField]
        private float baseRange;

        //TODO: logic in SO - think of a way to clean up?
        public float GetActualRange(IDefender attachedToDefender)
        {
            if (attachedToDefender == null)
            {
                return baseRange;
            }

            return baseRange * (1 + attachedToDefender.Attributes.Get(DeAttrName.rangeBonus).Value());
        }
    }
}