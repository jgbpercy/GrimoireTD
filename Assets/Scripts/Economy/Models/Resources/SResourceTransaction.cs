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
                return GameModels.Models[0].EconomyManager.GetResourceFromTemplate(resource);
            }
        }

        public int Amount
        {
            get
            {
                return amount;
            }
        }

        public bool CanDoTransaction()
        {
            return CResourceTransaction.CanDoTransaction(this);
        }
    }
}