namespace GrimoireTD.Economy
{
    public class CResourceTransaction : IResourceTransaction
    {
        private IReadOnlyResource resource;

        private int amount;

        public IReadOnlyResource Resource
        {
            get
            {
                return resource;
            }
        }

        public int Amount
        {
            get
            {
                return amount;
            }
        }

        public CResourceTransaction(IReadOnlyResource resource, int amount)
        {
            this.resource = resource;
            this.amount = amount;
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