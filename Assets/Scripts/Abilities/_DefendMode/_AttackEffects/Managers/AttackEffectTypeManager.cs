using System;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Technical;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public class AttackEffectTypeManager : SingletonMonobehaviour<AttackEffectTypeManager>
    {
        [SerializeField]
        private SoAttackEffectType[] attackEffectTypes;

        private Dictionary<IMetaDamageEffectType, List<ISpecificDamageEffectType>> metaToSpecificDictionary = new Dictionary<IMetaDamageEffectType, List<ISpecificDamageEffectType>>();

        private Dictionary<ISpecificDamageEffectType, IWeakMetaDamageEffectType> specificToWeakDictionary = new Dictionary<ISpecificDamageEffectType, IWeakMetaDamageEffectType>();
        private Dictionary<ISpecificDamageEffectType, IStrongMetaDamageEffectType> specificToStrongDictionary = new Dictionary<ISpecificDamageEffectType, IStrongMetaDamageEffectType>();

        private Dictionary<IBasicMetaDamageEffectType, IWeakMetaDamageEffectType> basicToWeakDictionary = new Dictionary<IBasicMetaDamageEffectType, IWeakMetaDamageEffectType>();
        private Dictionary<IBasicMetaDamageEffectType, IStrongMetaDamageEffectType> basicToStrongDictionary = new Dictionary<IBasicMetaDamageEffectType, IStrongMetaDamageEffectType>();

        private List<ISpecificDamageEffectType> specificDamageTypes = new List<ISpecificDamageEffectType>();
        private List<IMetaDamageEffectType> metaDamageTypes = new List<IMetaDamageEffectType>();
        private List<IBasicMetaDamageEffectType> basicMetaDamageTypes = new List<IBasicMetaDamageEffectType>();
        private List<IWeakMetaDamageEffectType> weakMetaDamageTypes = new List<IWeakMetaDamageEffectType>();
        private List<IStrongMetaDamageEffectType> strongMetaDamageTypes = new List<IStrongMetaDamageEffectType>();

        private List<IModifierEffectType> modifierEffectTypes = new List<IModifierEffectType>();
        private List<IAttributeModifierEffectType> attributeEffectTypes = new List<IAttributeModifierEffectType>();
        private List<IResistanceModifierEffectType> resistanceEffectTypes = new List<IResistanceModifierEffectType>();

        public IReadOnlyList<ISpecificDamageEffectType> SpecificDamageTypes
        {
            get
            {
                return specificDamageTypes;
            }
        }
        public IReadOnlyList<IMetaDamageEffectType> MetaDamageTypes
        {
            get
            {
                return metaDamageTypes;
            }
        }
        public IReadOnlyList<IBasicMetaDamageEffectType> BasicMetaDamageTypes
        {
            get
            {
                return basicMetaDamageTypes;
            }
        }
        public IReadOnlyList<IWeakMetaDamageEffectType> WeakMetaDamageTypes
        {
            get
            {
                return weakMetaDamageTypes;
            }
        }
        public IReadOnlyList<IStrongMetaDamageEffectType> StrongMetaDamageTypes
        {
            get
            {
                return strongMetaDamageTypes;
            }
        }

        public IReadOnlyList<IModifierEffectType> ModifierEffectTypes
        {
            get
            {
                return modifierEffectTypes;
            }
        }

        public IReadOnlyList<IAttributeModifierEffectType> AttributeEffectTypes
        {
            get
            {
                return attributeEffectTypes;
            }
        }

        public IReadOnlyList<IResistanceModifierEffectType> ResistanceEffectTypes
        {
            get
            {
                return resistanceEffectTypes;
            }
        }

        private void Awake()
        {
            foreach (IAttackEffectType attackEffectType in attackEffectTypes)
            {
                IDamageEffectType damageEffectType = attackEffectType as IDamageEffectType;
                if (damageEffectType != null)
                {
                    AddDamageEffectType(damageEffectType);
                    continue;
                }

                IModifierEffectType modifierEffectType = attackEffectType as IModifierEffectType;
                if (modifierEffectType != null)
                {
                    AddModifierEffect(modifierEffectType);
                    continue;
                }
            }

            foreach (IBasicMetaDamageEffectType basicMetaDamageType in basicMetaDamageTypes)
            {
                metaToSpecificDictionary.Add(basicMetaDamageType, new List<ISpecificDamageEffectType>());
            }

            foreach (ISpecificDamageEffectType specificDamageType in specificDamageTypes)
            {
                metaToSpecificDictionary[specificDamageType.BasicMetaDamageEffectType].Add(specificDamageType);
            }

            foreach (IWeakMetaDamageEffectType weakMetaDamageType in weakMetaDamageTypes)
            {
                metaToSpecificDictionary.Add(weakMetaDamageType, metaToSpecificDictionary[weakMetaDamageType.BasicMetaDamageType]);

                basicToWeakDictionary.Add(weakMetaDamageType.BasicMetaDamageType, weakMetaDamageType);

                foreach (ISpecificDamageEffectType specificDamageType in metaToSpecificDictionary[weakMetaDamageType])
                {
                    specificToWeakDictionary.Add(specificDamageType, weakMetaDamageType);
                }
            }

            foreach (IStrongMetaDamageEffectType strongMetaDamageType in strongMetaDamageTypes)
            {
                metaToSpecificDictionary.Add(strongMetaDamageType, metaToSpecificDictionary[strongMetaDamageType.BasicMetaDamageType]);

                basicToStrongDictionary.Add(strongMetaDamageType.BasicMetaDamageType, strongMetaDamageType);

                foreach (ISpecificDamageEffectType specificDamageType in metaToSpecificDictionary[strongMetaDamageType])
                {
                    specificToStrongDictionary.Add(specificDamageType, strongMetaDamageType);
                }
            }
        }

        private void AddDamageEffectType(IDamageEffectType damageEffectType)
        {
            ISpecificDamageEffectType specificDamageEffectType = damageEffectType as ISpecificDamageEffectType;
            if (specificDamageEffectType != null)
            {
                specificDamageTypes.Add(specificDamageEffectType);
                return;
            }

            IBasicMetaDamageEffectType basicMetaDamageEffectType = damageEffectType as IBasicMetaDamageEffectType;
            if (basicMetaDamageEffectType != null)
            {
                basicMetaDamageTypes.Add(basicMetaDamageEffectType);
                return;
            }

            IWeakMetaDamageEffectType weakMetaDamageEffectType = damageEffectType as IWeakMetaDamageEffectType;
            if (weakMetaDamageEffectType != null)
            {
                weakMetaDamageTypes.Add(weakMetaDamageEffectType);
                return;
            }

            IStrongMetaDamageEffectType strongMetaDamageEffectType = damageEffectType as IStrongMetaDamageEffectType;
            if (strongMetaDamageEffectType != null)
            {
                strongMetaDamageTypes.Add(strongMetaDamageEffectType);
                return;
            }

            throw new Exception("Unhandled damageEffectType");
        }

        private void AddModifierEffect(IModifierEffectType modifierEffectType)
        {
            modifierEffectTypes.Add(modifierEffectType);

            IAttributeModifierEffectType attributeModifierEffectType = modifierEffectType as IAttributeModifierEffectType;
            if (attributeModifierEffectType != null)
            {
                attributeEffectTypes.Add(attributeModifierEffectType);
                return;
            }

            IResistanceModifierEffectType resistanceModifierEffectType = modifierEffectType as IResistanceModifierEffectType;
            if (resistanceModifierEffectType != null)
            {
                resistanceEffectTypes.Add(resistanceModifierEffectType);
                return;
            }

            throw new Exception("Unhandled instantEffectType");
        }

        public IReadOnlyList<ISpecificDamageEffectType> GetSpecificDamageTypes(IMetaDamageEffectType metaDamageEffectType)
        {
            return metaToSpecificDictionary[metaDamageEffectType];
        }

        public IBasicMetaDamageEffectType GetBasicMetaDamageType(ISpecificDamageEffectType specificDamageType)
        {
            return specificDamageType.BasicMetaDamageEffectType;
        }

        public IWeakMetaDamageEffectType GetWeakMetaDamageType(ISpecificDamageEffectType specificDamageType)
        {
            return specificToWeakDictionary[specificDamageType];
        }

        public IWeakMetaDamageEffectType GetWeakMetaDamageType(IBasicMetaDamageEffectType basicDamageType)
        {
            return basicToWeakDictionary[basicDamageType];
        }

        public IStrongMetaDamageEffectType GetStrongMetaDamageType(ISpecificDamageEffectType specificDamageType)
        {
            return specificToStrongDictionary[specificDamageType];
        }

        public IStrongMetaDamageEffectType GetStrongMetaDamageType(IBasicMetaDamageEffectType basicDamageType)
        {
            return basicToStrongDictionary[basicDamageType];
        }
    }
}