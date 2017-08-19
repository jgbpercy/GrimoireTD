using System.Collections.Generic;
using GrimoireTD.Abilities;
using GrimoireTD.DefendingEntities.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Attributes;

namespace GrimoireTD.DefendingEntities.Units
{
    public class CUnitImprovement : CDefendingEntityImprovement, IUnitImprovement
    {
        private IHexOccupationBonus[] conditionalHexOccupationBonuses;

        private IStructureOccupationBonus[] conditionalStructureOccupationBonuses;

        public IEnumerable<IHexOccupationBonus> ConditionalHexOccupationBonuses
        {
            get
            {
                return conditionalHexOccupationBonuses;
            }
        }

        public IEnumerable<IStructureOccupationBonus> ConditionalStructureOccupationBonuses
        {
            get
            {
                return conditionalStructureOccupationBonuses;
            }
        }

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
            this.conditionalHexOccupationBonuses = new IHexOccupationBonus[conditionalHexOccupationBonuses.Count];
            this.conditionalStructureOccupationBonuses = new IStructureOccupationBonus[conditionalStructureOccupationBonuses.Count];

            conditionalHexOccupationBonuses.CopyTo(this.conditionalHexOccupationBonuses, 0);
            conditionalStructureOccupationBonuses.CopyTo(this.conditionalStructureOccupationBonuses, 0);
        }
    }
}