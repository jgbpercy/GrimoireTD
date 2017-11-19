using GrimoireTD.Dependencies;
using System;
using UnityEngine;

namespace GrimoireTD.Economy
{
    [Serializable]
    public class SResourceTransaction : IResourceTransaction
    {
        [SerializeField]
        private SoResourceTemplate resource;

        [SerializeField]
        private int amount;

        public IReadOnlyResource Resource
        {
            get
            {
                return DepsProv.TheEconomyManager.GetResourceFromTemplate(resource);
            }
        }

        public int Amount
        {
            get
            {
                return amount;
            }
        }
    }
}