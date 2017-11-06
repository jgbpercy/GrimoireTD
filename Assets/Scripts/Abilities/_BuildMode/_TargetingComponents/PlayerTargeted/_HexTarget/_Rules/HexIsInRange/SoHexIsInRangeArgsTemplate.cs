using UnityEngine;
using GrimoireTD.Defenders;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    [CreateAssetMenu(fileName = "NewSoHexIsInRangeArgsTemplate", menuName = "Build Mode Abilities/Player Targeted/Hex Targeted Rules/Hex Is In Range")]
    public class SoHexIsInRangeArgsTemplate : SoPlayerTargetsHexArgsTemplate
    {
        [SerializeField]
        private int range;

        public override PlayerTargetsHexArgs GenerateArgs(IDefender sourceDefender, Coord targetCoord)
        {
            return new HexIsInRangeArgs(sourceDefender, targetCoord, range);
        }
    }
}