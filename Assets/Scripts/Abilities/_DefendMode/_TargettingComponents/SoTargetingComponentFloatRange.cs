using UnityEngine;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Attributes;

namespace GrimoireTD.Abilities.DefendMode
{
    public class SoTargetingComponentFloatRange : SoDefendModeTargetingComponent, ITargetingComponentFloatRange
    {
        [SerializeField]
        private float baseRange;

        public float BaseRange
        {
            get
            {
                return baseRange;
            }
        }

        public float GetActualRange(DefendingEntity attachedToDefendingEntity)
        {
            return baseRange * (1 + attachedToDefendingEntity.Attributes.GetAttribute(DefendingEntityAttributeName.rangeBonus));
        }
    }
}