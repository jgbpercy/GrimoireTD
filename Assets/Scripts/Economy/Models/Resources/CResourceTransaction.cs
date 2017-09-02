namespace GrimoireTD.Economy
{
    public class CResourceTransaction : IResourceTransaction
    {
        public IReadOnlyResource Resource { get; }

        public int Amount { get; }

        public CResourceTransaction(IReadOnlyResource resource, int amount)
        {
            Resource = resource;
            Amount = amount;
        }

        public bool CanDoTransaction()
        {
            return CanDoTransaction(this);
        }

        public static bool CanDoTransaction(IResourceTransaction transaction)
        {
            return transaction.Resource.CanDoTransaction(transaction.Amount);
        }
    }
}