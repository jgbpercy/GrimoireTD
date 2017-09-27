using UnityEngine;
using GrimoireTD.Map;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.BuildMode
{
    [CreateAssetMenu(fileName = "NewSoValidMoveArgsTemplate", menuName = "Build Mode Abilities/Player Targeted/Hex Targeted Rules/Valid Move")]
    public class SoValidMoveArgsTemplate : SoPlayerTargetsHexArgsTemplate
    {
        [SerializeField]
        private int range;

        public override PlayerTargetsHexArgs GenerateArgs(IDefendingEntity sourceEntity, Coord targetCoord, IReadOnlyMapData mapData)
        {
            return new ValidMoveArgs(sourceEntity, targetCoord, mapData, range);
        }
    }
}