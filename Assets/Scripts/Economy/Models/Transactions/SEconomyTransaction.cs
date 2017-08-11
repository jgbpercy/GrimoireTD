using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GrimoireTD.Economy
{
    [Serializable]
    public class SEconomyTransaction : IEconomyTransaction
    {
        [SerializeField]
        private SResourceTransaction[] resourceTransactions;

        private IDictionary<IResource, IResourceTransaction> _transactionsDictionary = null;

        private IDictionary<IResource, IResourceTransaction> transactionsDictionary
        {
            get
            {
                if (_transactionsDictionary == null)
                {
                    _transactionsDictionary = new Dictionary<IResource, IResourceTransaction>();

                    foreach (IResource resource in EconomyManager.Instance.Resources)
                    {
                        int amount = 0;

                        foreach (IResourceTransaction resourceTransaction in resourceTransactions)
                        {
                            if (resourceTransaction.Resource == resource)
                            {
                                amount += resourceTransaction.Amount;
                            }
                        }

                        _transactionsDictionary.Add(resource, new CResourceTransaction(resource, amount));
                    }
                }

                return _transactionsDictionary;
            }
        }

        public IResourceTransaction GetResourceTransaction(IResource resource)
        {
            return transactionsDictionary[resource];
        }

        public int GetTransactionAmount(IResource resource)
        {
            return transactionsDictionary[resource].Amount;
        }

        public bool CanDoTransaction()
        {
            return transactionsDictionary.Values.All(x => x.CanDoTransaction());
        }

        public void DoTransaction()
        {
            foreach (IResourceTransaction resourceTransaction in transactionsDictionary.Values)
            {
                resourceTransaction.DoTransaction();
            }
        }

        public override string ToString()
        {
            return CEconomyTransaction.ToString(this);
        }

        public string ToString(bool absolute)
        {
            return CEconomyTransaction.ToString(this, absolute);
        }

        public string ToString(EconomyTransactionStringFormat format, bool absolute)
        {
            return CEconomyTransaction.ToString(this, format, absolute);
        }

        public IEconomyTransaction Abs()
        {
            return CEconomyTransaction.Abs(this);
        }

        public IEconomyTransaction Add(IEconomyTransaction otherTransaction)
        {
            return CEconomyTransaction.Add(this, otherTransaction);
        }

        public IEconomyTransaction Subtract(IEconomyTransaction otherTransaction)
        {
            return CEconomyTransaction.Subtract(this, otherTransaction);
        }

        public IEconomyTransaction Multiply(int factor)
        {
            return CEconomyTransaction.Multiply(this, factor);
        }

        public IEconomyTransaction Multiply(float factor, RoundingMode roundingMode)
        {
            return CEconomyTransaction.Multiply(this, factor, roundingMode);
        }

        public IEconomyTransaction Multiply(float factor)
        {
            return CEconomyTransaction.Multiply(this, factor);
        }
    }
}