using System.Collections.Generic;

public interface IHexType {

    string NameInGame { get; }

    BaseHexTypeEnum BaseHexType { get; }

    int TextureOffsetX { get; }
    int TextureOffsetY { get; }

    bool IsBuildable { get; }

    bool TypeIsPathableByCreeps { get; }

    bool UnitCanOccupy { get; }
}
