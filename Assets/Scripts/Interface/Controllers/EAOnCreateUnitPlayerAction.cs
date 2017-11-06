using System;
using GrimoireTD.Defenders.Units;
using GrimoireTD.Map;

namespace GrimoireTD.UI
{
    public class EAOnCreateUnitPlayerAction : EventArgs
    {
        public readonly Coord Position;

        public readonly IUnitTemplate UnitTemplate;

        public EAOnCreateUnitPlayerAction(Coord position, IUnitTemplate unitTemplate)
        {
            Position = position;
            UnitTemplate = unitTemplate;
        }
    }
}