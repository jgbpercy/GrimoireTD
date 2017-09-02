using UnityEngine;
using System.Collections.Generic;
using GrimoireTD.Abilities;
using GrimoireTD.DefendingEntities.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Attributes;

namespace GrimoireTD.DefendingEntities
{
    [CreateAssetMenu(fileName = "NewDefendingEntityImprovement", menuName = "Structures and Units/Defending Entity Improvement")]
    public class SoDefendingEntityImprovement : ScriptableObject, IDefendingEntityImprovement
    {
        [SerializeField]
        private SNamedDefendingEntityAttributeModifier[] attributeModifiers;

        [SerializeField]
        private SHexOccupationBonus[] flatHexOccupationBonuses;

        [SerializeField]
        private SoAbilityTemplate[] abilities;

        [SerializeField]
        private SoDefenderAuraTemplate[] auras;

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

        public IDefendingEntityImprovement Combine(IDefendingEntityImprovement otherImprovement)
        {
            return CDefendingEntityImprovement.Combine(this, otherImprovement);
        }
    }
}