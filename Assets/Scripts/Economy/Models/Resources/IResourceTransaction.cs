namespace GrimoireTD.Economy
{
    public interface IResourceTransaction
    {
        IReadOnlyResource Resource { get; }

        int Amount { get; }

        bool CanDoTransaction();
    }
}