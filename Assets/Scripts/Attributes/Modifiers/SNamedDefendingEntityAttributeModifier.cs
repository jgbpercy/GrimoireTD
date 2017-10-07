using System;
using UnityEngine;

namespace GrimoireTD.Attributes
{
    [Serializable]
    public class SNamedDefendingEntityAttributeModifier : SAttributeModifier, INamedAttributeModifier<DEAttrName>
    {
        [SerializeField]
        private DEAttrName attributeName;

        public SNamedDefendingEntityAttributeModifier(float magnitude, DEAttrName attributeName) : base(magnitude)
        {
            this.attributeName = attributeName;
        }

        public DEAttrName AttributeName
        {
            get
            {
                return attributeName;
            }
        }
    }
}