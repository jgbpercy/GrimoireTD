using UnityEngine;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Attributes;

namespace GrimoireTD.Abilities.DefendMode
{
    public class SoFloatRangeArgsTemplate : SoDefendModeTargetingArgsTemplate
    {
        [SerializeField]
        private float baseRange;

        //TODO: logic in SO - think of a way to clean up?
        public float GetActualRange(IDefendingEntity attachedToDefendingEntity)
        {
            if (attachedToDefendingEntity == null)
            {
                return baseRange;
            }

            return baseRange * (1 + attachedToDefendingEntity.Attributes.Get(DEAttrName.rangeBonus).Value());
        }
    }
}