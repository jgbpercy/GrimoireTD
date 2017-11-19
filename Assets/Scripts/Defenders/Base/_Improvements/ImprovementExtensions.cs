using System.Collections.Generic;
using GrimoireTD.Defenders.Units;

namespace GrimoireTD.Defenders
{
    public static class ImprovementExtensions
    {
        public static IDefenderImprovement CombineWith(this IDefenderImprovement improvement, IDefenderImprovement otherImprovement)
        {
            var unitImprovement = improvement as IUnitImprovement;
            var otherUnitImprovement = otherImprovement as IUnitImprovement;

            if (unitImprovement != null && otherUnitImprovement != null)
            {
                return new CUnitImprovement(
                    CollectionExt.CombineCollection(unitImprovement.AttributeModifiers, otherUnitImprovement.AttributeModifiers),
                    CollectionExt.CombineCollection(unitImprovement.FlatHexOccupationBonuses, otherUnitImprovement.FlatHexOccupationBonuses),
                    CollectionExt.CombineCollection(unitImprovement.Abilities, otherUnitImprovement.Abilities),
                    CollectionExt.CombineCollection(unitImprovement.Auras, otherUnitImprovement.Auras),
                    CollectionExt.CombineCollection(unitImprovement.ConditionalHexOccupationBonuses, otherUnitImprovement.ConditionalHexOccupationBonuses),
                    CollectionExt.CombineCollection(unitImprovement.ConditionalStructureOccupationBonuses, otherUnitImprovement.ConditionalStructureOccupationBonuses)
                );
            }
            else if (unitImprovement != null)
            {
                return new CUnitImprovement(
                    CollectionExt.CombineCollection(unitImprovement.AttributeModifiers, otherImprovement.AttributeModifiers),
                    CollectionExt.CombineCollection(unitImprovement.FlatHexOccupationBonuses, otherImprovement.FlatHexOccupationBonuses),
                    CollectionExt.CombineCollection(unitImprovement.Abilities, otherImprovement.Abilities),
                    CollectionExt.CombineCollection(unitImprovement.Auras, otherImprovement.Auras),
                    unitImprovement.ConditionalHexOccupationBonuses,
                    unitImprovement.ConditionalStructureOccupationBonuses
                );
            }
            else if (otherUnitImprovement != null)
            {
                return new CUnitImprovement(
                    CollectionExt.CombineCollection(improvement.AttributeModifiers, otherUnitImprovement.AttributeModifiers),
                    CollectionExt.CombineCollection(improvement.FlatHexOccupationBonuses, otherUnitImprovement.FlatHexOccupationBonuses),
                    CollectionExt.CombineCollection(improvement.Abilities, otherUnitImprovement.Abilities),
                    CollectionExt.CombineCollection(improvement.Auras, otherUnitImprovement.Auras),
                    otherUnitImprovement.ConditionalHexOccupationBonuses,
                    otherUnitImprovement.ConditionalStructureOccupationBonuses
                );
            }
            else
            {
                return new CDefenderImprovement(
                    CollectionExt.CombineCollection(improvement.AttributeModifiers, otherImprovement.AttributeModifiers),
                    CollectionExt.CombineCollection(improvement.FlatHexOccupationBonuses, otherImprovement.FlatHexOccupationBonuses),
                    CollectionExt.CombineCollection(improvement.Abilities, otherImprovement.Abilities),
                    CollectionExt.CombineCollection(improvement.Auras, otherImprovement.Auras)
                );
            }
        }
    }
}