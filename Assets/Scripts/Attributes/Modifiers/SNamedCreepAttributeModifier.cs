using System;
using UnityEngine;

namespace GrimoireTD.Attributes
{
    [Serializable]
    public class SNamedCreepAttributeModifier : SAttributeModifier, INamedAttributeModifier<CreepAttrName>
    {
        [SerializeField]
        private CreepAttrName attributeName;

        public SNamedCreepAttributeModifier(float magnitude, CreepAttrName attributeName) : base(magnitude)
        {
            this.attributeName = attributeName;
        }

        public CreepAttrName AttributeName
        {
            get
            {
                return attributeName;
            }
        }
    }
}