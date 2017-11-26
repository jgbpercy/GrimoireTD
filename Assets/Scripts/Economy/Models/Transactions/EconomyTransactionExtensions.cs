using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GrimoireTD.Dependencies;
using GrimoireTD.Technical;

namespace GrimoireTD.Economy
{
    public enum EconomyTransactionStringFormat
    {
        FullNameSingleLine,
        FullNameLineBreaks,
        ShortNameSingleLine,
        ShortNameLineBreaks
    }

    public static class EconomyTransactionExtensions
    {        
        public static int GetTransactionAmount(this IEconomyTransaction transaction, IReadOnlyResource resource)
        {
            return transaction.TransactionDictionary[resource].Amount;
        }

        public static bool CanDoTransaction(this IEconomyTransaction transaction)
        {
            return transaction.TransactionDictionary.Values.All(x => x.CanDoTransaction());
        }

        public static string ToString(IEconomyTransaction transaction)
        {
            return transaction.ToString(false);
        }

        public static string ToString(this IEconomyTransaction transaction, bool absolute)
        {
            return transaction.ToString(EconomyTransactionStringFormat.FullNameSingleLine, absolute);
        }

        public static string ToString(this IEconomyTransaction transaction, EconomyTransactionStringFormat format)
        {
            return transaction.ToString(format, false);
        }

        public static string ToString(this IEconomyTransaction transaction, EconomyTransactionStringFormat format, bool absolute)
        {
            IEconomyTransaction displayTransaction;

            var resources = DepsProv.TheEconomyManager.Resources;

            if (absolute)
            {
                displayTransaction = transaction.Abs();
            }
            else
            {
                displayTransaction = transaction;
            }

            var transactionAsString = "";

            if (format == EconomyTransactionStringFormat.FullNameSingleLine)
            {
                var index = 0;
                foreach (var resource in resources)
                {
                    transactionAsString += resource.NameInGame + ": " + displayTransaction.GetTransactionAmount(resource) + (index == resources.Count - 1 ? "" : ", ");
                    index++;
                }
                return transactionAsString;
            }
            else if (format == EconomyTransactionStringFormat.FullNameLineBreaks)
            {
                var index = 0;
                foreach (var resource in resources)
                {
                    transactionAsString += resource.NameInGame + ": " + displayTransaction.GetTransactionAmount(resource) + (index == resources.Count - 1 ? "" : "\n");
                    index++;
                }
                return transactionAsString;
            }
            else if (format == EconomyTransactionStringFormat.ShortNameSingleLine)
            {
                var index = 0;
                foreach (var resource in resources)
                {
                    transactionAsString += resource.ShortName + ": " + displayTransaction.GetTransactionAmount(resource) + (index == resources.Count - 1 ? "" : ", ");
                    index++;
                }
                return transactionAsString;
            }
            else if (format == EconomyTransactionStringFormat.ShortNameLineBreaks)
            {
                var index = 0;
                foreach (var resource in resources)
                {
                    transactionAsString += resource.ShortName + ": " + displayTransaction.GetTransactionAmount(resource) + (index == resources.Count - 1 ? "" : "\n");
                    index++;
                }
                return transactionAsString;
            }
            else
            {
                return ToString(transaction, absolute);
            }
        }

        public static IEconomyTransaction Abs(this IEconomyTransaction transaction)
        {
            var absDictionary = new Dictionary<IReadOnlyResource, IResourceTransaction>();

            foreach (var resource in DepsProv.TheEconomyManager.Resources)
            {
                absDictionary.Add(resource, new CResourceTransaction(resource, Mathf.Abs(transaction.GetTransactionAmount(resource))));
            }

            return new CEconomyTransaction(absDictionary);
        }

        public static IEconomyTransaction Add(this IEconomyTransaction transaction, IEconomyTransaction otherTransaction)
        {
            var addDictionary = new Dictionary<IReadOnlyResource, IResourceTransaction>();

            foreach (var resource in DepsProv.TheEconomyManager.Resources)
            {
                addDictionary.Add(resource, new CResourceTransaction(resource, transaction.GetTransactionAmount(resource) + otherTransaction.GetTransactionAmount(resource)));
            }

            return new CEconomyTransaction(addDictionary);
        }

        public static IEconomyTransaction Subtract(this IEconomyTransaction transaction, IEconomyTransaction otherTransaction)
        {
            var subtractDictionary = new Dictionary<IReadOnlyResource, IResourceTransaction>();

            foreach (var resource in DepsProv.TheEconomyManager.Resources)
            {
                subtractDictionary.Add(resource, new CResourceTransaction(resource, transaction.GetTransactionAmount(resource) - otherTransaction.GetTransactionAmount(resource)));
            }

            return new CEconomyTransaction(subtractDictionary);
        }

        public static IEconomyTransaction Multiply(this IEconomyTransaction transaction, int factor)
        {
            var multiplyDictionary = new Dictionary<IReadOnlyResource, IResourceTransaction>();

            foreach (var resource in DepsProv.TheEconomyManager.Resources)
            {
                multiplyDictionary.Add(resource, new CResourceTransaction(resource, transaction.GetTransactionAmount(resource) * factor));
            }

            return new CEconomyTransaction(multiplyDictionary);
        }

        public static IEconomyTransaction Multiply(this IEconomyTransaction transaction, float factor, RoundingMode roundingMode)
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

        public static IEconomyTransaction Multiply(this IEconomyTransaction transaction, float factor)
        {
            return Multiply(transaction, factor, RoundingMode.NEAREST);
        }
    }
}