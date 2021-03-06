﻿using System;
using GrimoireTD.Defenders.Units;

namespace GrimoireTD.Map
{
    public class EAOnUnitCreated : EventArgs
    {
        public readonly Coord Position;

        public readonly IUnit UnitCreated;

        public EAOnUnitCreated(Coord postion, IUnit unitCreated)
        {
            Position = postion;
            UnitCreated = unitCreated;
        }
    }
}