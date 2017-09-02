using System;
using System.Collections.Generic;

namespace GrimoireTD.Map
{
    public class EAOnPathGeneratedOrChanged : EventArgs
    {
        public readonly IReadOnlyList<Coord> NewPath;

        public EAOnPathGeneratedOrChanged(IReadOnlyList<Coord> newPath)
        {
            NewPath = newPath;
        }
    }
}