﻿using System;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Creeps
{
    [Serializable]
    public class BaseResistances
    {
        //These are just shorthand for setting all resistances/blocks within a meta type
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

        [SerializeField]
        private MetaDamageResistance[] metaDamageResistances;

        [SerializeField]
        private SResistanceModifier[] specificDamageResistances;

        [SerializeField]
        private MetaDamageBlock[] metaDamageBlocks;

        [SerializeField]
        private SBlockModifier[] specificDamageBlocks;

        public IResistanceModifier GetResistanceModifier(SpecificDamageEffectType specificDamageEffectType)
        {
            float resistance = 0;

            foreach (IResistanceModifier specificDamageResistance in specificDamageResistances)
            {
                if (specificDamageEffectType == specificDamageResistance.DamageType)
                {
                    resistance += specificDamageResistance.Magnitude;
                }
            }

            foreach (MetaDamageResistance metaDamageResistance in metaDamageResistances)
            {
                if (specificDamageEffectType.BasicMetaDamageEffectType == metaDamageResistance.DamageType)
                {
                    resistance += metaDamageResistance.Amount;
                }
            }

            return new SResistanceModifier(resistance, specificDamageEffectType);
        }

        public IBlockModifier GetBlockModifier(SpecificDamageEffectType specificDamageEffectType)
        {
            int block = 0;

            foreach (SBlockModifier specificDamageBlock in specificDamageBlocks)
            {
                if (specificDamageEffectType == specificDamageBlock.DamageType)
                {
                    block += specificDamageBlock.Magnitude;
                }
            }

            foreach (MetaDamageBlock metaDamageBlock in metaDamageBlocks)
            {
                if (specificDamageEffectType.BasicMetaDamageEffectType == metaDamageBlock.DamageType)
                {
                    block += metaDamageBlock.Amount;
                }
            }

            return new SBlockModifier(block, specificDamageEffectType);
        }
    }
}