using GrimoireTD.Dependencies;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GrimoireTD.Economy
{
    [Serializable]
    public class SEconomyTransaction : IEconomyTransaction
    {
        [SerializeField]
        private SResourceTransaction[] resourceTransactions;

        private Dictionary<IReadOnlyResource, IResourceTransaction> _transactionDictionary = null;

        public IReadOnlyDictionary<IReadOnlyResource, IResourceTransaction> TransactionDictionary
        {
            get
            {
                if (_transactionDictionary == null)
                {
                    _transactionDictionary = new Dictionary<IReadOnlyResource, IResourceTransaction>();

                    foreach (var resource in DepsProv.TheEconomyManager.Resources)
                    {
                        var amount = 0;

                        foreach (var resourceTransaction in resourceTransactions)
                        {
                            if (resourceTransaction.Resource == resource)
                            {
                                amount += resourceTransaction.Amount;
                            }
                        }

                        _transactionDictionary.Add(resource, new CResourceTransaction(resource, amount));
                    }
                }

                return _transactionDictionary;
            }
        }

        public override string ToString()
        {
            return EconomyTransactionExtensions.ToString(this);
        }
    }
}