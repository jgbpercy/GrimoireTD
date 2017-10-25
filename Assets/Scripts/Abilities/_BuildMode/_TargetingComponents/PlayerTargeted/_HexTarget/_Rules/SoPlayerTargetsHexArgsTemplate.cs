using System;
using UnityEngine;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class SoPlayerTargetsHexArgsTemplate : ScriptableObject, IPlayerTargetsHexArgsTemplate
    {
        public virtual PlayerTargetsHexArgs GenerateArgs(
            IDefendingEntity sourceEntity, 
            Coord targetCoord
        )
        {
            throw new Exception("SoBuildModeAbilityHexTargetedArgsTemplate is pseudo abstract. Use inheriting classes to generate args.");
        }
    }
}