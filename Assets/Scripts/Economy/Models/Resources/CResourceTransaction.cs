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
    }
}