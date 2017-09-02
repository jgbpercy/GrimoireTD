using System;

namespace GrimoireTD.Economy
{
    public interface IReadOnlyResource
    {
        string NameInGame { get; }

        string ShortName { get; }

        int AmountOwned { get; }

        event EventHandler<EAOnResourceChanged> OnResourceChanged;

        bool CanDoTransaction(int amount);
    }
}