using System;
using UnityEngine;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class SoBuildModeHexTargetedArgsTemplate : ScriptableObject, IBuildModeHexTargetedArgsTemplate
    {
        public virtual BuildModeHexTargetedArgs GenerateArgs(
            IDefendingEntity sourceEntity, 
            Coord targetCoord, 
            IReadOnlyMapData mapData
        )
        {
            throw new Exception("SoBuildModeAbilityHexTargetedArgsTemplate is pseudo abstract. Use inheriting classes to generate args.");
        }
    }
}