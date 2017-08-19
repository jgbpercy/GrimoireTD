using GrimoireTD.Map;

namespace GrimoireTD.Economy
{
    public interface IHexOccupationBonus : IOccupationBonus
    {
        IHexType HexType { get; }
    }
}