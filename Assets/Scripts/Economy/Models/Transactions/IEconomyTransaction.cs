using System.Collections.Generic;

namespace GrimoireTD.Economy
{
    public interface IEconomyTransaction
    {
        IReadOnlyDictionary<IReadOnlyResource, IResourceTransaction> TransactionDictionary { get; }
    }
}