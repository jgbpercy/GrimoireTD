using UnityEngine;
using System.Collections.Generic;
using GrimoireTD.Abilities;
using GrimoireTD.Defenders.DefenderEffects;
using GrimoireTD.Economy;
using GrimoireTD.Attributes;

namespace GrimoireTD.Defenders
{
    [CreateAssetMenu(fileName = "NewDefenderImprovement", menuName = "Structures and Units/Defender Improvement")]
    public class SoDefenderImprovement : ScriptableObject, IDefenderImprovement
    {
        [SerializeField]
        private SNamedDefenderAttributeModifier[] attributeModifiers;

        [SerializeField]
        private SHexOccupationBonus[] flatHexOccupationBonuses;

        [SerializeField]
        private SoAbilityTemplate[] abilities;

        [SerializeField]
        private SoDefenderAuraTemplate[] auras;

        public IEnumerable<INamedAttributeModifier<DeAttrName>> AttributeModifiers
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

        public IDefenderImprovement Combine(IDefenderImprovement otherImprovement)
        {
            return CDefenderImprovement.Combine(this, otherImprovement);
        }
    }
}