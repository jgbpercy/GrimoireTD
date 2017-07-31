public interface IHexType {

    string NameInGame { get; }

    int TextureOffsetX { get; }
    int TextureOffsetY { get; }

    bool IsBuildable { get; }

    bool TypeIsPathableByCreeps { get; }

    bool UnitCanOccupy { get; }
}
