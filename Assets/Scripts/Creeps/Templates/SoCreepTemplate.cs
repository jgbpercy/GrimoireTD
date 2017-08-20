using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Economy;
using GrimoireTD.Attributes;

namespace GrimoireTD.Creeps
{
    [CreateAssetMenu(fileName = "NewCreep", menuName = "Creeps/Creep")]
    public class SoCreepTemplate : ScriptableObject, ICreepTemplate
    {
        [SerializeField]
        private string nameInGame;

        [SerializeField]
        private SNamedCreepAttributeModifier[] baseAttributes;

        [SerializeField]
        private int maxHitpoints;

        [SerializeField]
        private SEconomyTransaction bounty;

        [SerializeField]
        private SBaseResistances resistances;

        public string NameInGame
        {
            get
            {
                return nameInGame;
            }
        }

        public IEnumerable<INamedAttributeModifier<CreepAttributeName>> BaseAttributes
        {
            get
            {
                return baseAttributes;
            }
        }

        public int MaxHitpoints
        {
            get
            {
                return maxHitpoints;
            }
        }

        public IEconomyTransaction Bounty
        {
            get
            {
                return bounty;
            }
        }

        public IBaseResistances BaseResistances
        {
            get
            {
                return resistances;
            }
        }

        public virtual ICreep GenerateCreep(Vector3 spawnPosition)
        {
            return new CCreep(this, spawnPosition);
        }
    }
}