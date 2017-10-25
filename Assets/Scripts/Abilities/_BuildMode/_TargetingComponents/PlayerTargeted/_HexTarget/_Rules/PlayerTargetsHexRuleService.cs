using System;
using GrimoireTD.Map;
using GrimoireTD.DefendingEntities.Units;
using GrimoireTD.Dependencies;

namespace GrimoireTD.Abilities.BuildMode
{
    public static class PlayerTargetsHexRuleService
    {
        //Wrapper function
        public static Func<PlayerTargetsHexArgs, bool> RunRule = (args) =>
        {
            var validMoveArgs = args as ValidMoveArgs;
            if (validMoveArgs != null)
            {
                return ValidMove(validMoveArgs);
            }

            var hexIsInRangeArgs = args as HexIsInRangeArgs;
            if (hexIsInRangeArgs != null)
            {
                return HexIsInRange(hexIsInRangeArgs);
            }

            throw new ArgumentException("BuildModeAbilityHexTargetingRuleService was passed a rule args for which there was no rule.");
        };

        //Rules
        private static bool ValidMove(ValidMoveArgs args)
        {
            var sourceUnit = args.SourceEntity as IUnit;
            if (sourceUnit == null) throw new ArgumentException("ValidMove was passed a non-Unit"); //optimisation: disable in release build

            if (!HexIsInRange(new HexIsInRangeArgs(args.SourceEntity, args.TargetCoord, args.Range)))
            {
                return false;
            }

            return DepsProv.TheMapData.CanMoveUnitTo(args.TargetCoord, sourceUnit.CachedDisallowedMovementDestinations);
        }

        private static bool DefendingEntityAtTarget(Coord targetCoord, IMapData mapData)
        {
            return UnitAtTarget(targetCoord, mapData) || StructureAtTarget(targetCoord, mapData);
        }

        private static bool UnitAtTarget(Coord targetCoord, IMapData mapData)
        {
            return mapData.GetHexAt(targetCoord).UnitHere != null;
        }

        private static bool StructureAtTarget(Coord targetCoord, IMapData mapData)
        {
            return mapData.GetHexAt(targetCoord).StructureHere != null;
        }

        private static bool HexIsInRange(HexIsInRangeArgs args)
        {
            return CMapData.HexIsInRange(args.Range, args.SourceEntity.CoordPosition, args.TargetCoord);
        }
    }
}