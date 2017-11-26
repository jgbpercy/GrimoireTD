using System.Collections.Generic;
using GrimoireTD.Dependencies;

namespace GrimoireTD.Economy
{
    public class CEconomyTransaction : IEconomyTransaction
    {
        private Dictionary<IReadOnlyResource, IResourceTransaction> transactionDictionary;

        public CEconomyTransaction()
        {
            transactionDictionary = new Dictionary<IReadOnlyResource, IResourceTransaction>();

            foreach (var resource in DepsProv.TheEconomyManager.Resources)
            {
                transactionDictionary.Add(resource, new CResourceTransaction(resource, 0));
            }
        }

        public CEconomyTransaction(IEnumerable<IResourceTransaction> resourceTransactions)
        {
            transactionDictionary = new Dictionary<IReadOnlyResource, IResourceTransaction>();

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

                transactionDictionary.Add(resource, new CResourceTransaction(resource, amount));
            }
        }

        public CEconomyTransaction(IDictionary<IReadOnlyResource, IResourceTransaction> transactionDictionary)
        {
            this.transactionDictionary = new Dictionary<IReadOnlyResource, IResourceTransaction>(transactionDictionary);

            foreach (var resource in DepsProv.TheEconomyManager.Resources)
            {
                if (!transactionDictionary.ContainsKey(resource))
                {
                    transactionDictionary.Add(resource, new CResourceTransaction(resource, 0));
                }
            }
        }

        public IReadOnlyDictionary<IReadOnlyResource, IResourceTransaction> TransactionDictionary
        {
            get
            {
                return transactionDictionary;
            }
        }

        public override string ToString()
        {
            return EconomyTransactionExtensions.ToString(this);
        }
    }
}