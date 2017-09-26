using UnityEngine;

namespace GrimoireTD.Abilities.BuildMode
{
    [CreateAssetMenu(fileName = "NewMoveEffectComponent", menuName = "Build Mode Abilities/Effect Components/Move")]
    public class SoMoveEffectComponentTemplate : SoBuildModeEffectComponentTemplate, IMoveEffectComponentTemplate
    {
        public override IBuildModeEffectComponent GenerateEffectComponent()
        {
            return new CMoveEffectComponent(this);
        }
    }
}