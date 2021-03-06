﻿using System;
using GrimoireTD.Defenders.Structures;

namespace GrimoireTD.Map
{
    public class EAOnStructureCreated : EventArgs
    {
        public readonly Coord Position;

        public readonly IStructure StructureCreated;

        public EAOnStructureCreated(Coord position, IStructure structureCreated)
        {
            Position = position;
            StructureCreated = structureCreated;
        }
    }
}