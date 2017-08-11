using System;
using UnityEngine;

namespace GrimoireTD.Attributes
{
    [Serializable]
    public class SNamedDefendingEntityAttributeModifier : SAttributeModifier, INamedAttributeModifier<DefendingEntityAttributeName>
    {
        [SerializeField]
        private DefendingEntityAttributeName attributeName;

        public SNamedDefendingEntityAttributeModifier(float magnitude, DefendingEntityAttributeName attributeName) : base(magnitude)
        {
            this.attributeName = attributeName;
        }

        public DefendingEntityAttributeName AttributeName
        {
            get
            {
                return attributeName;
            }
        }
    }
}