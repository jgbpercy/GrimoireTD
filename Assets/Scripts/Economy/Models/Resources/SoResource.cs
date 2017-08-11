using System;
using UnityEngine;

namespace GrimoireTD.Economy
{
    [CreateAssetMenu(fileName = "NewResource", menuName = "Economy/Resource")]
    public class SoResource : ScriptableObject, IResource
    {
        [SerializeField]
        private string nameInGame;

        [SerializeField]
        private string shortName;

        private int amountOwned = 0;

        [SerializeField]
        private int maxAmount;

        private Action<int, int> OnResourceChangedCallback = null;

        public string NameInGame
        {
            get
            {
                return nameInGame;
            }
        }

        public string ShortName
        {
            get
            {
                return shortName;
            }
        }

        public int AmountOwned
        {
            get
            {
                return amountOwned;
            }
        }

        public bool CanDoTransaction(int amount)
        {
            int resultingAmount = amountOwned + amount;
            if (resultingAmount > maxAmount || resultingAmount < 0)
            {
                return false;
            }

            return true;
        }

        public void DoTransaction(int amount)
        {
            amountOwned += amount;

            OnResourceChangedCallback?.Invoke(amount, amountOwned);
        }

        public void RegisterForOnResourceChangedCallback(Action<int, int> callback)
        {
            OnResourceChangedCallback += callback;
        }

        public void DeregisterForOnResourceChangedCallback(Action<int, int> callback)
        {
            OnResourceChangedCallback -= callback;
        }
    }
}