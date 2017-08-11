namespace GrimoireTD.Economy
{
    public class CResourceTransaction : IResourceTransaction
    {
        private IResource resource;

        private int amount;

        public IResource Resource
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

        public CResourceTransaction(IResource resource, int amount)
        {
            this.resource = resource;
            this.amount = amount;
        }

        public bool CanDoTransaction()
        {
            return CanDoTransaction(this);
        }

        public void DoTransaction()
        {
            DoTransaction(this);
        }

        public static bool CanDoTransaction(IResourceTransaction transaction)
        {
            return transaction.Resource.CanDoTransaction(transaction.Amount);
        }

        public static void DoTransaction(IResourceTransaction transaction)
        {
            transaction.Resource.DoTransaction(transaction.Amount);
        }
    }
}