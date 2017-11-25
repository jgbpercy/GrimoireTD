

namespace GrimoireTD.Economy
{
    public interface IResourceTemplate
    {
        string NameInGame { get; }

        string ShortName { get; }

        int MaxAmount { get; }

        IResource GenerateResource();
    }
}