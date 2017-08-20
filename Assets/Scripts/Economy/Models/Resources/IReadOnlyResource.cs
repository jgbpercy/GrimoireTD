using System;

namespace GrimoireTD.Economy
{
    public interface IReadOnlyResource
    {
        string NameInGame { get; }

        string ShortName { get; }

        int AmountOwned { get; }

        bool CanDoTransaction(int amount);

        void RegisterForOnResourceChangedCallback(Action<int, int> callback);

        void DeregisterForOnResourceChangedCallback(Action<int, int> callback);
    }
}