using System;
using System.Linq;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.AttackEffects;

/* optimisation: some sort of cache and callback-update pattern for saving resistance values
 * rather than recalculating on each call
 */
namespace GrimoireTD.Creeps
{
    [Serializable]
    public class SResistances : IResistances
    {
        [Serializable]
        private class MetaDamageResistance
        {
            [SerializeField]
            private BasicMetaDamageEffectType damageType;

            [SerializeField]
            private float amount;

            public BasicMetaDamageEffectType DamageType
            {
                get
                {
                    return damageType;
                }
            }

            public float Amount
            {
                get
                {
                    return amount;
                }
            }
        }

        [Serializable]
        private class SpecificDamageResistance
        {
            [SerializeField]
            private SpecificDamageEffectType damageType;

            [SerializeField]
            private float amount;

            public SpecificDamageEffectType DamageType
            {
                get
                {
                    return damageType;
                }
            }

            public float Amount
            {
                get
                {
                    return amount;
                }
            }
        }

        [Serializable]
        private class MetaDamageBlock
        {
            [SerializeField]
            private BasicMetaDamageEffectType damageType;

            [SerializeField]
            private int amount;

            public BasicMetaDamageEffectType DamageType
            {
                get
                {
                    return damageType;
                }
            }

            public int Amount
            {
                get
                {
                    return amount;
                }
            }
        }

        [Serializable]
        private class SpecificDamageBlock
        {
            [SerializeField]
            private SpecificDamageEffectType damageType;

            [SerializeField]
            private int amount;

            public SpecificDamageEffectType DamageType
            {
                get
                {
                    return damageType;
                }
            }

            public int Amount
            {
                get
                {
                    return amount;
                }
            }
        }

        [SerializeField]
        private int armor;

        [SerializeField]
        private MetaDamageResistance[] metaDamageResistances;

        [SerializeField]
        private SpecificDamageResistance[] specificDamageResistances;

        [SerializeField]
        private MetaDamageBlock[] metaDamageBlocks;

        [SerializeField]
        private SpecificDamageBlock[] specificDamageBlocks;

        public int Armor
        {
            get
            {
                return armor;
            }
        }

        public float GetResistance(DamageEffectType damageEffectType, int currentMinusArmor)
        {
            int actualArmor = armor - currentMinusArmor;

            SpecificDamageEffectType specificDamageEffectType = damageEffectType as SpecificDamageEffectType;
            if (specificDamageEffectType != null)
            {
                return GetSpecificResistance(specificDamageEffectType, actualArmor);
            }

            BasicMetaDamageEffectType basicDamageEffectType = damageEffectType as BasicMetaDamageEffectType;
            if (basicDamageEffectType != null)
            {
                return GetBasicResistance(basicDamageEffectType, actualArmor);
            }

            WeakMetaDamageEffectType weakDamageEffectType = damageEffectType as WeakMetaDamageEffectType;
            if (weakDamageEffectType != null)
            {
                return GetWeakResistance(weakDamageEffectType, actualArmor);
            }

            StrongMetaDamageEffectType strongDamageEffectType = damageEffectType as StrongMetaDamageEffectType;
            if (strongDamageEffectType != null)
            {
                return GetStrongResistance(strongDamageEffectType, actualArmor);
            }

            throw new Exception("Unhandled damageEffectType");
        }

        private float GetSpecificResistance(SpecificDamageEffectType specificDamageEffectType, int actualArmor)
        {
            float resistance = 0;

            foreach (SpecificDamageResistance specificDamageResistance in specificDamageResistances)
            {
                if (specificDamageEffectType == specificDamageResistance.DamageType)
                {
                    resistance += specificDamageResistance.Amount;
                }
            }

            foreach (MetaDamageResistance metaDamageResistance in metaDamageResistances)
            {
                if (specificDamageEffectType.BasicMetaDamageEffectType == metaDamageResistance.DamageType)
                {
                    resistance += metaDamageResistance.Amount;
                }
            }

            for (int i = 0; i < actualArmor; i++)
            {
                resistance += resistance + (1 - resistance) * specificDamageEffectType.BasicMetaDamageEffectType.EffectOfArmor;
            }

            return resistance;
        }

        private float GetBasicResistance(BasicMetaDamageEffectType basicDamageEffectType, int actualArmor)
        {
            return basicDamageEffectType.SpecificDamageTypes
                .Select(x => GetSpecificResistance(x, actualArmor))
                .Average();
        }

        private float GetWeakResistance(WeakMetaDamageEffectType weakDamageEffectType, int actualArmor)
        {
            return weakDamageEffectType.SpecificDamageTypes
                .Select(x => GetSpecificResistance(x, actualArmor))
                .Max();
        }

        private float GetStrongResistance(StrongMetaDamageEffectType strongDamageEffectType, int actualArmor)
        {
            return strongDamageEffectType.SpecificDamageTypes
                .Select(x => GetSpecificResistance(x, actualArmor))
                .Min();
        }

        public int GetBlock(DamageEffectType damageEffectType)
        {
            SpecificDamageEffectType specificDamageEffectType = damageEffectType as SpecificDamageEffectType;
            if (specificDamageEffectType != null)
            {
                return GetSpecificBlock(specificDamageEffectType);
            }

            BasicMetaDamageEffectType basicDamageEffectType = damageEffectType as BasicMetaDamageEffectType;
            if (basicDamageEffectType != null)
            {
                return GetBasicBlock(basicDamageEffectType);
            }

            WeakMetaDamageEffectType weakDamageEffectType = damageEffectType as WeakMetaDamageEffectType;
            if (weakDamageEffectType != null)
            {
                return GetWeakBlock(weakDamageEffectType);
            }

            StrongMetaDamageEffectType strongDamageEffectType = damageEffectType as StrongMetaDamageEffectType;
            if (strongDamageEffectType != null)
            {
                return GetStrongBlock(strongDamageEffectType);
            }

            throw new Exception("Unhandled damageEffectType");
        }

        private int GetSpecificBlock(SpecificDamageEffectType specificDamageEffectType)
        {
            int block = 0;

            foreach (SpecificDamageBlock specificDamageBlock in specificDamageBlocks)
            {
                if (specificDamageEffectType == specificDamageBlock.DamageType)
                {
                    block += specificDamageBlock.Amount;
                }
            }

            foreach (MetaDamageBlock metaDamageBlock in metaDamageBlocks)
            {
                if (specificDamageEffectType.BasicMetaDamageEffectType == metaDamageBlock.DamageType)
                {
                    block += metaDamageBlock.Amount;
                }
            }

            return block;
        }

        private int GetBasicBlock(BasicMetaDamageEffectType basicDamageEffectType)
        {
            return Mathf.RoundToInt(
                (float)
                (
                    basicDamageEffectType.SpecificDamageTypes
                    .Select(x => GetSpecificBlock(x))
                    .Average()
                )
            );
        }

        private int GetWeakBlock(WeakMetaDamageEffectType weakDamageEffectType)
        {
            return weakDamageEffectType.SpecificDamageTypes
                .Select(x => GetSpecificBlock(x))
                .Max();
        }

        private int GetStrongBlock(StrongMetaDamageEffectType strongDamageEffectType)
        {
            return strongDamageEffectType.SpecificDamageTypes
                .Select(x => GetSpecificBlock(x))
                .Min();
        }
    }
}