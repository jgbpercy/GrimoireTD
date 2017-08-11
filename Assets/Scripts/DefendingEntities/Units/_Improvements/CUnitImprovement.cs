﻿using System.Collections.Generic;
using GrimoireTD.Abilities;
using GrimoireTD.DefendingEntities.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Attributes;

namespace GrimoireTD.DefendingEntities.Units
{
    public class CUnitImprovement : CDefendingEntityImprovement, IUnitImprovement
    {
        private HexOccupationBonus[] conditionalHexOccupationBonuses;

        private StructureOccupationBonus[] conditionalStructureOccupationBonuses;

        public IEnumerable<HexOccupationBonus> ConditionalHexOccupationBonuses
        {
            get
            {
                return conditionalHexOccupationBonuses;
            }
        }

        public IEnumerable<StructureOccupationBonus> ConditionalStructureOccupationBonuses
        {
            get
            {
                return conditionalStructureOccupationBonuses;
            }
        }

        public CUnitImprovement(
            ICollection<INamedAttributeModifier<DefendingEntityAttributeName>> attributeModifiers,
            ICollection<HexOccupationBonus> flatHexOccupationBonuses,
            ICollection<IAbilityTemplate> abilities,
            ICollection<IDefenderAuraTemplate> auras,
            ICollection<HexOccupationBonus> conditionalHexOccupationBonuses,
            ICollection<StructureOccupationBonus> conditionalStructureOccupationBonuses
            )
            : base(attributeModifiers, flatHexOccupationBonuses, abilities, auras)
        {
            this.conditionalHexOccupationBonuses = new HexOccupationBonus[conditionalHexOccupationBonuses.Count];
            this.conditionalStructureOccupationBonuses = new StructureOccupationBonus[conditionalStructureOccupationBonuses.Count];

            conditionalHexOccupationBonuses.CopyTo(this.conditionalHexOccupationBonuses, 0);
            conditionalStructureOccupationBonuses.CopyTo(this.conditionalStructureOccupationBonuses, 0);
        }
    }
}