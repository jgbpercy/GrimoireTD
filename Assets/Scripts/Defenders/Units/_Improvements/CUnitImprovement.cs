using System.Collections.Generic;
using GrimoireTD.Abilities;
using GrimoireTD.Defenders.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Attributes;

namespace GrimoireTD.Defenders.Units
{
    public class CUnitImprovement : CDefenderImprovement, IUnitImprovement
    {
        public ICollection<IHexOccupationBonus> ConditionalHexOccupationBonuses { get; }

        public ICollection<IStructureOccupationBonus> ConditionalStructureOccupationBonuses { get; }

        public CUnitImprovement(
            ICollection<INamedAttributeModifier<DeAttrName>> attributeModifiers,
            ICollection<IHexOccupationBonus> flatHexOccupationBonuses,
            ICollection<IAbilityTemplate> abilities,
            ICollection<IDefenderAuraTemplate> auras,
            ICollection<IHexOccupationBonus> conditionalHexOccupationBonuses,
            ICollection<IStructureOccupationBonus> conditionalStructureOccupationBonuses
            )
            : base(attributeModifiers, flatHexOccupationBonuses, abilities, auras)
        {
            ConditionalHexOccupationBonuses = conditionalHexOccupationBonuses;
            ConditionalStructureOccupationBonuses = conditionalStructureOccupationBonuses;
        }
    }
}