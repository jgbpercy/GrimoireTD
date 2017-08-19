using System.Collections.Generic;
using GrimoireTD.Abilities;
using GrimoireTD.DefendingEntities.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Attributes;

namespace GrimoireTD.DefendingEntities
{
    public class CDefendingEntityImprovement : IDefendingEntityImprovement
    {
        private INamedAttributeModifier<DefendingEntityAttributeName>[] attributeModifiers;

        private IHexOccupationBonus[] flatHexOccupationBonuses;

        private IAbilityTemplate[] abilities;

        private IDefenderAuraTemplate[] auras;

        public IEnumerable<INamedAttributeModifier<DefendingEntityAttributeName>> AttributeModifiers
        {
            get
            {
                return attributeModifiers;
            }
        }

        public IEnumerable<IHexOccupationBonus> FlatHexOccupationBonuses
        {
            get
            {
                return flatHexOccupationBonuses;
            }
        }

        public IEnumerable<IAbilityTemplate> Abilities
        {
            get
            {
                return abilities;
            }
        }

        public IEnumerable<IDefenderAuraTemplate> Auras
        {
            get
            {
                return auras;
            }
        }

        public CDefendingEntityImprovement(
            ICollection<INamedAttributeModifier<DefendingEntityAttributeName>> attributeModifiers,
            ICollection<IHexOccupationBonus> flatHexOccupationBonuses,
            ICollection<IAbilityTemplate> abilities,
            ICollection<IDefenderAuraTemplate> auras)
        {
            this.attributeModifiers = new INamedAttributeModifier<DefendingEntityAttributeName>[attributeModifiers.Count];
            this.flatHexOccupationBonuses = new IHexOccupationBonus[flatHexOccupationBonuses.Count];
            this.abilities = new IAbilityTemplate[abilities.Count];
            this.auras = new IDefenderAuraTemplate[auras.Count];

            attributeModifiers.CopyTo(this.attributeModifiers, 0);
            flatHexOccupationBonuses.CopyTo(this.flatHexOccupationBonuses, 0);
            abilities.CopyTo(this.abilities, 0);
            auras.CopyTo(this.auras, 0);
        }

        public IDefendingEntityImprovement Combine(IDefendingEntityImprovement otherImprovement)
        {
            return SoDefendingEntityImprovement.Combine(this, otherImprovement);
        }
    }
}