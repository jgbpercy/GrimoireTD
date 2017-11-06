using System;
using UnityEngine;

namespace GrimoireTD.Attributes
{
    [Serializable]
    public class SNamedDefenderAttributeModifier : SAttributeModifier, INamedAttributeModifier<DeAttrName>
    {
        [SerializeField]
        private DeAttrName attributeName;

        public SNamedDefenderAttributeModifier(float magnitude, DeAttrName attributeName) : base(magnitude)
        {
            this.attributeName = attributeName;
        }

        public DeAttrName AttributeName
        {
            get
            {
                return attributeName;
            }
        }
    }
}