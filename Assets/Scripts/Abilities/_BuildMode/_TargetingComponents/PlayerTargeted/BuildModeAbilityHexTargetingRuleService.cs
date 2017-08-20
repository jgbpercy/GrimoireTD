using System;
using UnityEngine.Assertions;
using GrimoireTD.DefendingEntities;
using GrimoireTD.DefendingEntities.Units;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public static class BuildModeAbilityHexTargetingRuleService
    {
        private static IReadOnlyMapData mapData = GameModels.Models[0].MapData;

        //Rule names
        public enum RuleName
        {
            ValidMove,
            HexIsInRange
        }

        //Wrapper function
        public static bool RunRule(RuleName ruleName, IDefendingEntity sourceDefendingEntity, Coord targetCoord, int range)
        {
            switch (ruleName)
            {
                case RuleName.ValidMove:
                    return ValidMove(sourceDefendingEntity, targetCoord, range);
                case RuleName.HexIsInRange:
                    return HexIsInRange(sourceDefendingEntity, targetCoord, range);
                default:
                    throw new Exception("BuildModeAbilityHexTargetingRuleService was passed a rule name to run that does not have a rule configured.");
            }
        }

        //Rules
        private static bool ValidMove(IDefendingEntity sourceDefendingEntity, Coord targetCoord, int range)
        {
            IUnit sourceUnit = sourceDefendingEntity as IUnit;
            Assert.IsTrue(sourceUnit != null);

            if (!HexIsInRange(sourceDefendingEntity, targetCoord, range))
            {
                return false;
            }

            return mapData.CanMoveUnitTo(targetCoord, sourceUnit.CachedDisallowedMovementDestinations);
        }

        private static bool DefendingEntityAtTarget(Coord targetCoord)
        {
            return UnitAtTarget(targetCoord) || StructureAtTarget(targetCoord);
        }

        private static bool UnitAtTarget(Coord targetCoord)
        {
            return mapData.GetHexAt(targetCoord).UnitHere != null;
        }

        private static bool StructureAtTarget(Coord targetCoord)
        {
            return mapData.GetHexAt(targetCoord).StructureHere != null;
        }

        private static bool HexIsInRange(IDefendingEntity sourceDefendingEntity, Coord targetCoord, int range)
        {
            return CMapData.HexIsInRange(range, sourceDefendingEntity.CoordPosition, targetCoord);
        }
    }
}