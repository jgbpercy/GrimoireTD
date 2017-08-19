namespace GrimoireTD.TemporaryEffects
{
    public interface ITemporaryEffect : IReadOnlyTemporaryEffect
    {
        object Key { get; }

        void EndNow();
    }
}