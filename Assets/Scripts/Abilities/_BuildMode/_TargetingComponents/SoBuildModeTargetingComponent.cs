using System;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class SoBuildModeTargetingComponent : ScriptableObject, IBuildModeTargetingComponent
    {
        public virtual IReadOnlyList<IBuildModeTargetable> FindTargets(Coord position, IReadOnlyMapData mapData)
        {
            throw new NotImplementedException("Base BuildModeTargetingComponent cannot find targets and should not be used.");
        }
    }
}