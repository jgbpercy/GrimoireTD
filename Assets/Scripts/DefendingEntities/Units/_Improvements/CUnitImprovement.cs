using System.Collections.Generic;
using GrimoireTD.Abilities;
using GrimoireTD.DefendingEntities.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Attributes;

namespace GrimoireTD.DefendingEntities.Units
{
    public class CUnitImprovement : CDefendingEntityImprovement, IUnitImprovement
    {
        public IEnumerable<IHexOccupationBonus> ConditionalHexOccupationBonuses { get; }

        public IEnumerable<IStructureOccupationBonus> ConditionalStructureOccupationBonuses { get; }

        public CUnitImprovement(
            ICollection<INamedAttributeModifier<DefendingEntityAttributeName>> attributeModifiers,
            ICollection<IHexOccupationBonus> flatHexOccupationBonuses,
            ICollection<IAbilityTemplate> abilities,
            ICollection<IDefenderAuraTemplate> auras,
            ICollection<IHexOccupationBonus> conditionalHexOccupationBonuses,
            ICollection<IStructureOccupationBonus> conditionalStructureOccupationBonuses
            )
            : base(attributeModifiers, flatHexOccupationBonuses, abilities, auras)
        {
            var tempConditionalHexOccupationBonuses = new IHexOccupationBonus[conditionalHexOccupationBonuses.Count];
            var tempConditionalStructureOccupationBonuses = new IStructureOccupationBonus[conditionalStructureOccupationBonuses.Count];

            conditionalHexOccupationBonuses.CopyTo(tempConditionalHexOccupationBonuses, 0);
            conditionalStructureOccupationBonuses.CopyTo(tempConditionalStructureOccupationBonuses, 0);

            ConditionalHexOccupationBonuses = tempConditionalHexOccupationBonuses;
            ConditionalStructureOccupationBonuses = tempConditionalStructureOccupationBonuses;
        }
    }
}