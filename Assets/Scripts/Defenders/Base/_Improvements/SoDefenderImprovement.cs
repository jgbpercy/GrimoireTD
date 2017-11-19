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

        public ICollection<INamedAttributeModifier<DeAttrName>> AttributeModifiers
        {
            get
            {
                return attributeModifiers;
            }
        }

        public ICollection<IHexOccupationBonus> FlatHexOccupationBonuses
        {
            get
            {
                return flatHexOccupationBonuses;
            }
        }

        public ICollection<IAbilityTemplate> Abilities
        {
            get
            {
                return abilities;
            }
        }

        public ICollection<IDefenderAuraTemplate> Auras
        {
            get
            {
                return auras;
            }
        }
    }
}