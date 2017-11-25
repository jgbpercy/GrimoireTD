using System;

namespace GrimoireTD.Economy
{
    public class CResource : IResource
    {
        private IResourceTemplate resourceTemplate;

        public event EventHandler<EAOnResourceChanged> OnResourceChanged;

        public int AmountOwned { get; private set; }

        public string NameInGame
        {
            get
            {
                return resourceTemplate.NameInGame;
            }
        }

        public string ShortName
        {
            get
            {
                return resourceTemplate.ShortName;
            }
        }

        public CResource(IResourceTemplate template)
        {
            resourceTemplate = template;
            AmountOwned = 0;
        }

        public bool CanDoTransaction(int amount)
        {
            var resultingAmount = AmountOwned + amount;

            if (resultingAmount > resourceTemplate.MaxAmount || resultingAmount < 0)
            {
                return false;
            }

            return true;
        }

        public void DoTransaction(int amount)
        {
            //TODO: remove in release
            if (!CanDoTransaction(amount))
            {
                throw new Exception("Resource was asked to do a transaction that can't be done. Some code didn't check the transaction could be done.");
            }

            AmountOwned += amount;

            OnResourceChanged?.Invoke(this, new EAOnResourceChanged(amount, AmountOwned));
        }
    }
}