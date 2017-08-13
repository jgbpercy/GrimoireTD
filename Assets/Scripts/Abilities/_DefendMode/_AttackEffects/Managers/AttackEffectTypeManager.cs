using System;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Technical;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public class AttackEffectTypeManager : SingletonMonobehaviour<AttackEffectTypeManager>
    {
        [SerializeField]
        private AttackEffectType[] attackEffectTypes;

        private Dictionary<MetaDamageEffectType, List<SpecificDamageEffectType>> metaToSpecificDictionary = new Dictionary<MetaDamageEffectType, List<SpecificDamageEffectType>>();

        private Dictionary<SpecificDamageEffectType, WeakMetaDamageEffectType> specificToWeakDictionary = new Dictionary<SpecificDamageEffectType, WeakMetaDamageEffectType>();
        private Dictionary<SpecificDamageEffectType, StrongMetaDamageEffectType> specificToStrongDictionary = new Dictionary<SpecificDamageEffectType, StrongMetaDamageEffectType>();

        private List<SpecificDamageEffectType> specificDamageTypes = new List<SpecificDamageEffectType>();
        private List<MetaDamageEffectType> metaDamageTypes = new List<MetaDamageEffectType>();
        private List<BasicMetaDamageEffectType> basicMetaDamageTypes = new List<BasicMetaDamageEffectType>();
        private List<WeakMetaDamageEffectType> weakMetaDamageTypes = new List<WeakMetaDamageEffectType>();
        private List<StrongMetaDamageEffectType> strongMetaDamageTypes = new List<StrongMetaDamageEffectType>();

        private List<ModifierEffectType> modifierEffectTypes = new List<ModifierEffectType>();
        private List<AttributeModifierEffectType> attributeEffectTypes = new List<AttributeModifierEffectType>();
        private List<ResistanceModifierEffectType> resistanceEffectTypes = new List<ResistanceModifierEffectType>();

        public IReadOnlyList<SpecificDamageEffectType> SpecificDamageTypes
        {
            get
            {
                return specificDamageTypes;
            }
        }
        public IReadOnlyList<MetaDamageEffectType> MetaDamageTypes
        {
            get
            {
                return metaDamageTypes;
            }
        }
        public IReadOnlyList<BasicMetaDamageEffectType> BasicMetaDamageTypes
        {
            get
            {
                return basicMetaDamageTypes;
            }
        }
        public IReadOnlyList<WeakMetaDamageEffectType> WeakMetaDamageTypes
        {
            get
            {
                return weakMetaDamageTypes;
            }
        }
        public IReadOnlyList<StrongMetaDamageEffectType> StrongMetaDamageTypes
        {
            get
            {
                return strongMetaDamageTypes;
            }
        }

        public IReadOnlyList<ModifierEffectType> ModifierEffectTypes
        {
            get
            {
                return modifierEffectTypes;
            }
        }

        public IReadOnlyList<AttributeModifierEffectType> AttributeEffectTypes
        {
            get
            {
                return attributeEffectTypes;
            }
        }

        public IReadOnlyList<ResistanceModifierEffectType> ResistanceEffectTypes
        {
            get
            {
                return resistanceEffectTypes;
            }
        }

        private void Awake()
        {
            foreach (AttackEffectType attackEffectType in attackEffectTypes)
            {
                DamageEffectType damageEffectType = attackEffectType as DamageEffectType;
                if (damageEffectType != null)
                {
                    AddDamageEffectType(damageEffectType);
                    continue;
                }

                ModifierEffectType modifierEffectType = attackEffectType as ModifierEffectType;
                if (modifierEffectType != null)
                {
                    AddModifierEffect(modifierEffectType);
                    continue;
                }
            }

            foreach (BasicMetaDamageEffectType basicMetaDamageType in basicMetaDamageTypes)
            {
                metaToSpecificDictionary.Add(basicMetaDamageType, new List<SpecificDamageEffectType>());
            }

            foreach (SpecificDamageEffectType specificDamageType in specificDamageTypes)
            {
                metaToSpecificDictionary[specificDamageType.BasicMetaDamageEffectType].Add(specificDamageType);
            }

            foreach (WeakMetaDamageEffectType weakMetaDamageType in weakMetaDamageTypes)
            {
                metaToSpecificDictionary.Add(weakMetaDamageType, metaToSpecificDictionary[weakMetaDamageType.BasicMetaDamageType]);

                foreach (SpecificDamageEffectType specificDamageType in metaToSpecificDictionary[weakMetaDamageType])
                {
                    specificToWeakDictionary.Add(specificDamageType, weakMetaDamageType);
                }
            }

            foreach (StrongMetaDamageEffectType strongMetaDamageType in strongMetaDamageTypes)
            {
                metaToSpecificDictionary.Add(strongMetaDamageType, metaToSpecificDictionary[strongMetaDamageType.BasicMetaDamageType]);

                foreach (SpecificDamageEffectType specificDamageType in metaToSpecificDictionary[strongMetaDamageType])
                {
                    specificToStrongDictionary.Add(specificDamageType, strongMetaDamageType);
                }
            }
        }

        private void AddDamageEffectType(DamageEffectType damageEffectType)
        {
            SpecificDamageEffectType specificDamageEffectType = damageEffectType as SpecificDamageEffectType;
            if (specificDamageEffectType != null)
            {
                specificDamageTypes.Add(specificDamageEffectType);
                return;
            }

            BasicMetaDamageEffectType basicMetaDamageEffectType = damageEffectType as BasicMetaDamageEffectType;
            if (basicMetaDamageEffectType != null)
            {
                basicMetaDamageTypes.Add(basicMetaDamageEffectType);
                return;
            }

            WeakMetaDamageEffectType weakMetaDamageEffectType = damageEffectType as WeakMetaDamageEffectType;
            if (weakMetaDamageEffectType != null)
            {
                weakMetaDamageTypes.Add(weakMetaDamageEffectType);
                return;
            }

            StrongMetaDamageEffectType strongMetaDamageEffectType = damageEffectType as StrongMetaDamageEffectType;
            if (strongMetaDamageEffectType != null)
            {
                strongMetaDamageTypes.Add(strongMetaDamageEffectType);
                return;
            }

            throw new Exception("Unhandled damageEffectType");
        }

        private void AddModifierEffect(ModifierEffectType modifierEffectType)
        {
            modifierEffectTypes.Add(modifierEffectType);

            AttributeModifierEffectType attributeModifierEffectType = modifierEffectType as AttributeModifierEffectType;
            if (attributeModifierEffectType != null)
            {
                attributeEffectTypes.Add(attributeModifierEffectType);
                return;
            }

            ResistanceModifierEffectType resistanceModifierEffectType = modifierEffectType as ResistanceModifierEffectType;
            if (resistanceModifierEffectType != null)
            {
                resistanceEffectTypes.Add(resistanceModifierEffectType);
                return;
            }

            throw new Exception("Unhandled instantEffectType");
        }

        public IReadOnlyList<SpecificDamageEffectType> GetSpecificDamageTypes(MetaDamageEffectType metaDamageEffectType)
        {
            return metaToSpecificDictionary[metaDamageEffectType];
        }

        public BasicMetaDamageEffectType GetBasicMetaDamageType(SpecificDamageEffectType specificDamageType)
        {
            return specificDamageType.BasicMetaDamageEffectType;
        }

        public WeakMetaDamageEffectType GetWeakMetaDamageType(SpecificDamageEffectType specificDamageType)
        {
            return specificToWeakDictionary[specificDamageType];
        }

        public StrongMetaDamageEffectType GetStrongMetaDamageType(SpecificDamageEffectType specificDamageType)
        {
            return specificToStrongDictionary[specificDamageType];
        }
    }
}