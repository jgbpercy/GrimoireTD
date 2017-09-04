using UnityEngine;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    [CreateAssetMenu(fileName = "NewSoHexIsInRangeArgsTemplate", menuName = "Build Mode Abilities/Player Targeted/Hex Targeted/Rule Args/Hex Is In Range")]
    public class SoHexIsInRangeArgsTemplate : SoBuildModeHexTargetedArgsTemplate
    {
        [SerializeField]
        private int range;

        public override BuildModeHexTargetedArgs GenerateArgs(IDefendingEntity sourceEntity, Coord targetCoord, IReadOnlyMapData mapData)
        {
            return new HexIsInRangeArgs(sourceEntity, targetCoord, mapData, range);
        }
    }
}