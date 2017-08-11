using System;
using UnityEngine;

namespace GrimoireTD.Attributes
{
    [Serializable]
    public class SAttributeModifier : IAttributeModifier
    {
        [SerializeField]
        private float magnitude;

        public float Magnitude
        {
            get
            {
                return magnitude;
            }
        }

        public SAttributeModifier(float magnitude)
        {
            this.magnitude = magnitude;
        }

        public string AsPercentString
        {
            get
            {
                return magnitude * 100 + "%";
            }
        }
    }
}