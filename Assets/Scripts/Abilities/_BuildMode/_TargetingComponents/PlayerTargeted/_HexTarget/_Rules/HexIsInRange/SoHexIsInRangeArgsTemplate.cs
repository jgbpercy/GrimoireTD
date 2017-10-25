using UnityEngine;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    [CreateAssetMenu(fileName = "NewSoHexIsInRangeArgsTemplate", menuName = "Build Mode Abilities/Player Targeted/Hex Targeted Rules/Hex Is In Range")]
    public class SoHexIsInRangeArgsTemplate : SoPlayerTargetsHexArgsTemplate
    {
        [SerializeField]
        private int range;

        public override PlayerTargetsHexArgs GenerateArgs(IDefendingEntity sourceEntity, Coord targetCoord)
        {
            return new HexIsInRangeArgs(sourceEntity, targetCoord, range);
        }
    }
}