using UnityEngine;
using GrimoireTD.Map;
using GrimoireTD.Defenders;

namespace GrimoireTD.Abilities.BuildMode
{
    [CreateAssetMenu(fileName = "NewSoValidMoveArgsTemplate", menuName = "Build Mode Abilities/Player Targeted/Hex Targeted Rules/Valid Move")]
    public class SoValidMoveArgsTemplate : SoPlayerTargetsHexArgsTemplate
    {
        [SerializeField]
        private int range;

        public override PlayerTargetsHexArgs GenerateArgs(IDefender sourceDefender, Coord targetCoord)
        {
            return new ValidMoveArgs(sourceDefender, targetCoord, range);
        }
    }
}