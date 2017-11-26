namespace GrimoireTD.Map
{
    public interface IHexType
    {
        string NameInGame { get; }

        int TextureOffsetX { get; }
        int TextureOffsetY { get; }

        bool IsBuildable { get; }

        bool IsPathableByCreeps { get; }

        bool UnitCanOccupy { get; }
    }
}