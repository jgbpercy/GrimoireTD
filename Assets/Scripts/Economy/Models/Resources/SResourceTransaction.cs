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

        public IResource Resource
        {
            get
            {
                return EconomyManager.Instance.GetResourceFromTemplate(resource);
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

        public void DoTransaction()
        {
            CResourceTransaction.DoTransaction(this);
        }
    }
}