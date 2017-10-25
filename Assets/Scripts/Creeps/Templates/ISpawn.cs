namespace GrimoireTD.Creeps
{
    public interface ISpawn
    {
        float Timing { get; }

        ICreepTemplate Creep { get; }
    }
}