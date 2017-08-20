namespace GrimoireTD.Economy
{
    public interface IResource : IReadOnlyResource
    {
        void DoTransaction(int amount);
    }
}