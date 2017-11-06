using System;
using GrimoireTD.Defenders.Structures;
using GrimoireTD.Map;

namespace GrimoireTD.UI
{
    public class EAOnBuildStructurePlayerAction : EventArgs
    {
        public readonly Coord Position;

        public readonly IStructureTemplate StructureTemplate;

        public EAOnBuildStructurePlayerAction(Coord position, IStructureTemplate structureTemplate)
        {
            Position = position;
            StructureTemplate = structureTemplate;
        }
    }
}