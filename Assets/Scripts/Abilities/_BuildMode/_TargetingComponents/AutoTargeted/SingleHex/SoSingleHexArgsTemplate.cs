using UnityEngine;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    [CreateAssetMenu(fileName = "NewSoSingleHexArgsTemplate", menuName = "Build Mode Abilities/Auto Targeted/Rule Args/Single Hex")]
    public class SoSingleHexArgsTemplate : SoBuildModeAutoTargetedArgsTemplate
    {
        public override BuildModeAutoTargetedArgs GenerateArgs(Coord targetCoord, IReadOnlyMapData mapData)
        {
            return new SingleHexArgs(targetCoord, mapData);
        }
    }
}