using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public interface IPlayerTargetsDefendingEntityArgsTemplate
    {
        PlayerTargetsDefendingEntityArgs GenerateArgs(
            IDefendingEntity sourceEntity, 
            IDefendingEntity targetEntity
        );
    }
}