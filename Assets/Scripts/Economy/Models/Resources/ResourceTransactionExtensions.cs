namespace GrimoireTD.Economy
{
    public static class ResourceTransactionExtensions
    {
        public static bool CanDoTransaction(this IResourceTransaction resourceTransaction)
        {
            return resourceTransaction.Resource.CanDoTransaction(resourceTransaction.Amount);
        }
    }
}