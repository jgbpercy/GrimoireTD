using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Dependencies;

namespace GrimoireTD.Economy
{
    public class CEconomyTransaction : IEconomyTransaction
    {
        private IDictionary<IReadOnlyResource, IResourceTransaction> transactionsDictionary;

        public CEconomyTransaction()
        {
            transactionsDictionary = new Dictionary<IReadOnlyResource, IResourceTransaction>();

            foreach (var resource in DepsProv.TheEconomyManager.Resources)
            {
                transactionsDictionary.Add(resource, new CResourceTransaction(resource, 0));
            }
        }

        public CEconomyTransaction(IEnumerable<IResourceTransaction> resourceTransactions)
        {
            transactionsDictionary = new Dictionary<IReadOnlyResource, IResourceTransaction>();

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

                transactionsDictionary.Add(resource, new CResourceTransaction(resource, amount));
            }
        }

        public CEconomyTransaction(IDictionary<IReadOnlyResource, IResourceTransaction> transactionsDictionary)
        {
            this.transactionsDictionary = transactionsDictionary;

            foreach (var resource in DepsProv.TheEconomyManager.Resources)
            {
                if (!transactionsDictionary.ContainsKey(resource))
                {
                    transactionsDictionary.Add(resource, new CResourceTransaction(resource, 0));
                }
            }
        }

        public IResourceTransaction GetResourceTransaction(IReadOnlyResource resource)
        {
            return transactionsDictionary[resource];
        }

        public int GetTransactionAmount(IReadOnlyResource resource)
        {
            return transactionsDictionary[resource].Amount;
        }

        public bool CanDoTransaction()
        {
            return transactionsDictionary.Values.All(x => x.CanDoTransaction());
        }

        public override string ToString()
        {
            return ToString(this);
        }

        public string ToString(bool absolute)
        {
            return ToString(this, absolute);
        }

        public string ToString(EconomyTransactionStringFormat format, bool absolute)
        {
            return ToString(this, format, absolute);
        }

        public IEconomyTransaction Abs()
        {
            return Abs(this);
        }

        public IEconomyTransaction Add(IEconomyTransaction otherTransaction)
        {
            return Add(this, otherTransaction);
        }

        public IEconomyTransaction Subtract(IEconomyTransaction otherTransaction)
        {
            return Subtract(this, otherTransaction);
        }

        public IEconomyTransaction Multiply(int factor)
        {
            return Multiply(this, factor);
        }

        public IEconomyTransaction Multiply(float factor)
        {
            return Multiply(this, factor);
        }

        public IEconomyTransaction Multiply(float factor, RoundingMode roundingMode)
        {
            return Multiply(this, factor, roundingMode);
        }

        public static string ToString(IEconomyTransaction economyTransaction)
        {
            return ToString(economyTransaction, false);
        }

        public static string ToString(IEconomyTransaction economyTransaction, bool absolute)
        {
            return ToString(economyTransaction, EconomyTransactionStringFormat.FullNameSingleLine, absolute);
        }

        public static string ToString(IEconomyTransaction economyTransaction, EconomyTransactionStringFormat format, bool absolute)
        {
            IEconomyTransaction displayTransaction;

            var resources = DepsProv.TheEconomyManager.Resources;

            if (absolute)
            {
                displayTransaction = Abs(economyTransaction);
            }
            else
            {
                displayTransaction = economyTransaction;
            }

            string transactionAsString = "";

            if (format == EconomyTransactionStringFormat.FullNameSingleLine)
            {
                foreach (var resource in resources)
                {
                    transactionAsString += resource.NameInGame + ": " + displayTransaction.GetTransactionAmount(resource) + ", ";
                }
                return transactionAsString;
            }
            else if (format == EconomyTransactionStringFormat.FullNameLineBreaks)
            {
                foreach (var resource in resources)
                {
                    transactionAsString += resource.NameInGame + ": " + displayTransaction.GetTransactionAmount(resource) + "\n";
                }
                return transactionAsString;
            }
            else if (format == EconomyTransactionStringFormat.ShortNameSingleLine)
            {
                foreach (var resource in resources)
                {
                    transactionAsString += resource.ShortName + ": " + displayTransaction.GetTransactionAmount(resource) + ", ";
                }
                return transactionAsString;
            }
            else if (format == EconomyTransactionStringFormat.ShortNameLineBreaks)
            {
                foreach (var resource in resources)
                {
                    transactionAsString += resource.ShortName + ": " + displayTransaction.GetTransactionAmount(resource) + "\n";
                }
                return transactionAsString;
            }
            else
            {
                return ToString(economyTransaction, absolute);
            }
        }

        public static IEconomyTransaction Abs(IEconomyTransaction economyTransaction)
        {
            var absDictionary = new Dictionary<IReadOnlyResource, IResourceTransaction>();

            foreach (var resource in DepsProv.TheEconomyManager.Resources)
            {
                absDictionary.Add(resource, new CResourceTransaction(resource, Mathf.Abs(economyTransaction.GetTransactionAmount(resource))));
            }

            return new CEconomyTransaction(absDictionary);
        }

        public static IEconomyTransaction Add(IEconomyTransaction firstTransaction, IEconomyTransaction secondTransaction)
        {
            var addDictionary = new Dictionary<IReadOnlyResource, IResourceTransaction>();

            foreach (var resource in DepsProv.TheEconomyManager.Resources)
            {
                addDictionary.Add(resource, new CResourceTransaction(resource, firstTransaction.GetTransactionAmount(resource) + secondTransaction.GetTransactionAmount(resource)));
            }

            return new CEconomyTransaction(addDictionary);
        }

        public static IEconomyTransaction Subtract(IEconomyTransaction firstTransaction, IEconomyTransaction secondTransaction)
        {
            var subtractDictionary = new Dictionary<IReadOnlyResource, IResourceTransaction>();

            foreach (var resource in DepsProv.TheEconomyManager.Resources)
            {
                subtractDictionary.Add(resource, new CResourceTransaction(resource, firstTransaction.GetTransactionAmount(resource) - secondTransaction.GetTransactionAmount(resource)));
            }

            return new CEconomyTransaction(subtractDictionary);
        }

        public static IEconomyTransaction Multiply(IEconomyTransaction transaction, int factor)
        {
            var multiplyDictionary = new Dictionary<IReadOnlyResource, IResourceTransaction>();

            foreach (var resource in DepsProv.TheEconomyManager.Resources)
            {
                multiplyDictionary.Add(resource, new CResourceTransaction(resource, transaction.GetTransactionAmount(resource) * factor));
            }

            return new CEconomyTransaction(multiplyDictionary);
        }

        public static IEconomyTransaction Multiply(IEconomyTransaction transaction, float factor, RoundingMode roundingMode)
        {
            var multiplyDictionary = new Dictionary<IReadOnlyResource, IResourceTransaction>();

            var economyManager = DepsProv.TheEconomyManager;

            if (roundingMode == RoundingMode.NEAREST)
            {
                foreach (var resource in economyManager.Resources)
                {
                    multiplyDictionary.Add(resource, new CResourceTransaction(resource, Mathf.RoundToInt(transaction.GetTransactionAmount(resource) * factor)));
                }

                return new CEconomyTransaction(multiplyDictionary);
            }
            else if (roundingMode == RoundingMode.UP)
            {
                foreach (var resource in economyManager.Resources)
                {
                    multiplyDictionary.Add(resource, new CResourceTransaction(resource, Mathf.CeilToInt(transaction.GetTransactionAmount(resource) * factor)));
                }

                return new CEconomyTransaction(multiplyDictionary);
            }
            else if (roundingMode == RoundingMode.DOWN)
            {
                foreach (var resource in economyManager.Resources)
                {
                    multiplyDictionary.Add(resource, new CResourceTransaction(resource, Mathf.FloorToInt(transaction.GetTransactionAmount(resource) * factor)));
                }

                return new CEconomyTransaction(multiplyDictionary);
            }
            else
            {
                return Multiply(transaction, factor);
            }
        }

        public static IEconomyTransaction Multiply(IEconomyTransaction transaction, float factor)
        {
            return Multiply(transaction, factor, RoundingMode.NEAREST);
        }
    }
}