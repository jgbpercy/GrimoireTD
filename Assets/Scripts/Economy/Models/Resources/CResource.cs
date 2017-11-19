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

        //Currently the responsibility of the caller to check CanDoTransaction
        public void DoTransaction(int amount)
        {
            AmountOwned += amount;

            OnResourceChanged?.Invoke(this, new EAOnResourceChanged(amount, AmountOwned));
        }
    }
}