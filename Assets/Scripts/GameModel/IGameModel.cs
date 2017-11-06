using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.AttackEffects;
using GrimoireTD.Economy;
using GrimoireTD.Levels;
using GrimoireTD.Map;
using GrimoireTD.Defenders.Structures;

namespace GrimoireTD
{
    public interface IGameModel : IReadOnlyGameModel
    {
        void SetUp(
            ILevel level, 
            IEnumerable<IResourceTemplate> resourceTemplates, 
            IEnumerable<IHexType> hexTypes, 
            IEnumerable<IAttackEffectType> attackEffectTypes,
            IDictionary<Color32, IHexType> colorToHexTypeDictionary,
            IEnumerable<IStructureTemplate> buildableStructureTemplates,
            float trackIdleTimeAfterSpawns,
            float unitFatigueFactorInfelctionPoint,
            float unitFatigueFactorShallownessMultiplier
        );
    }
}