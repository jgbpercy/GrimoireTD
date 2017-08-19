namespace GrimoireTD.Economy
{
    public interface IResource : IReadOnlyResource
    {
        bool CanDoTransaction(int amount);

        void DoTransaction(int amount);
    }
}