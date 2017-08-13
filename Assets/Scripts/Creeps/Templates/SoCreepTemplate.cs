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
        private GameObject creepPrefab;

        [SerializeField]
        private SEconomyTransaction bounty;

        [SerializeField]
        private SResistances resistances;

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

        public GameObject CreepPrefab
        {
            get
            {
                return creepPrefab;
            }
        }

        public IEconomyTransaction Bounty
        {
            get
            {
                return bounty;
            }
        }

        public IResistances Resistances
        {
            get
            {
                return resistances;
            }
        }

        public virtual Creep GenerateCreep(Vector3 spawnPosition)
        {
            return new Creep(this, spawnPosition);
        }
    }
}