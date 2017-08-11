using System;
using UnityEngine;

namespace GrimoireTD.Attributes
{
    [Serializable]
    public class SNamedCreepAttributeModifier : SAttributeModifier, INamedAttributeModifier<CreepAttributeName>
    {
        [SerializeField]
        private CreepAttributeName attributeName;

        public SNamedCreepAttributeModifier(float magnitude, CreepAttributeName attributeName) : base(magnitude)
        {
            this.attributeName = attributeName;
        }

        public CreepAttributeName AttributeName
        {
            get
            {
                return attributeName;
            }
        }
    }
}