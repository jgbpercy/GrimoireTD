using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.AttackEffects;
using GrimoireTD.Technical;

namespace GrimoireTD.Creeps
{
    public class CResistances : IResistances
    {
        private Dictionary<ISpecificDamageEffectType, RecalculatorList<IResistanceModifier, float>> positiveResistanceModifiers;
        private Dictionary<ISpecificDamageEffectType, RecalculatorList<IResistanceModifier, float>> negativeResistanceModifiers;

        private Dictionary<ISpecificDamageEffectType, RecalculatorList<IBlockModifier, int>> blockModifiers;

        private Dictionary<ISpecificDamageEffectType, Action<float>> OnResistanceChangedCallbackDictionary;
        private Action<ISpecificDamageEffectType, float> OnAnyResistanceChangedCallback;

        private Dictionary<ISpecificDamageEffectType, Action<int>> OnBlockChangedCallbackDictionary;
        private Action<ISpecificDamageEffectType, int> OnAnyBlockChangedCallback;

        public CResistances(IBaseResistances baseResistances)
        {
            positiveResistanceModifiers = new Dictionary<ISpecificDamageEffectType, RecalculatorList<IResistanceModifier, float>>();
            negativeResistanceModifiers = new Dictionary<ISpecificDamageEffectType, RecalculatorList<IResistanceModifier, float>>();
            blockModifiers = new Dictionary<ISpecificDamageEffectType, RecalculatorList<IBlockModifier, int>>();

            OnResistanceChangedCallbackDictionary = new Dictionary<ISpecificDamageEffectType, Action<float>>();
            OnBlockChangedCallbackDictionary = new Dictionary<ISpecificDamageEffectType, Action<int>>();

            foreach (ISpecificDamageEffectType specificDamageType in AttackEffectTypeManager.Instance.SpecificDamageTypes)
            {
                positiveResistanceModifiers.Add(specificDamageType, new RecalculatorList<IResistanceModifier, float>(CalculatePositiveResistance));
                positiveResistanceModifiers[specificDamageType].Add(baseResistances.GetResistanceModifier(specificDamageType));

                negativeResistanceModifiers.Add(specificDamageType, new RecalculatorList<IResistanceModifier, float>(CalculateNegativeResistanceMultiplier));

                blockModifiers.Add(specificDamageType, new RecalculatorList<IBlockModifier, int>(CalculateBlock));
                blockModifiers[specificDamageType].Add(baseResistances.GetBlockModifier(specificDamageType));
            }
        }

        private float CalculatePositiveResistance(List<IResistanceModifier> modifierList)
        {
            float resistance = 0;

            foreach (IResistanceModifier modifier in modifierList)
            {
                resistance += (1 - resistance) * modifier.Magnitude;
            }

            return resistance;
        }

        private float CalculateNegativeResistanceMultiplier(List<IResistanceModifier> modifierList)
        {
            float resistanceMultiplier = 1;

            foreach (IResistanceModifier modifier in modifierList)
            {
                resistanceMultiplier = resistanceMultiplier * (1 + modifier.Magnitude);
            }

            return resistanceMultiplier;
        }

        private int CalculateBlock(IList<IBlockModifier> modifierList)
        {
            int block = 0;

            foreach (IBlockModifier modifier in modifierList)
            {
                block += modifier.Magnitude;
            }

            return Math.Min(block, 0);
        }
        

        public void AddResistanceModifier(IResistanceModifier modifier)
        {
            if (modifier.Magnitude >= 0)
            {
                positiveResistanceModifiers[modifier.DamageType].Add(modifier);
            }

            negativeResistanceModifiers[modifier.DamageType].Add(modifier);
        }

        public void RemoveResistanceModifer(IResistanceModifier modifier)
        {
            if (modifier.Magnitude >= 0)
            {
                positiveResistanceModifiers[modifier.DamageType].Remove(modifier);
            }

            negativeResistanceModifiers[modifier.DamageType].Remove(modifier);
        }

        public void AddBlockModifier(IBlockModifier modifier)
        {
            blockModifiers[modifier.DamageType].Add(modifier);
        }

        public void RemoveBlockModifier(IBlockModifier modifier)
        {
            blockModifiers[modifier.DamageType].Remove(modifier);
        }

        public float GetResistance(IDamageEffectType damageEffectType, float currentArmor)
        {
            ISpecificDamageEffectType specificDamageEffectType = damageEffectType as ISpecificDamageEffectType;
            if (specificDamageEffectType != null)
            {
                return GetSpecificResistance(specificDamageEffectType, currentArmor);
            }

            IBasicMetaDamageEffectType basicDamageEffectType = damageEffectType as IBasicMetaDamageEffectType;
            if (basicDamageEffectType != null)
            {
                return GetBasicResistance(basicDamageEffectType, currentArmor);
            }

            IWeakMetaDamageEffectType weakDamageEffectType = damageEffectType as IWeakMetaDamageEffectType;
            if (weakDamageEffectType != null)
            {
                return GetWeakResistance(weakDamageEffectType, currentArmor);
            }

            IStrongMetaDamageEffectType strongDamageEffectType = damageEffectType as IStrongMetaDamageEffectType;
            if (strongDamageEffectType != null)
            {
                return GetStrongResistance(strongDamageEffectType, currentArmor);
            }

            throw new Exception("Unhandled damageEffectType");
        }

        private float GetSpecificResistance(ISpecificDamageEffectType specificDamageEffectType, float actualArmor)
        {
            float resistanceBeforeArmor = positiveResistanceModifiers[specificDamageEffectType].Value * negativeResistanceModifiers[specificDamageEffectType].Value;

            return GetResistanceAfterArmor(resistanceBeforeArmor, actualArmor, specificDamageEffectType.BasicMetaDamageEffectType.EffectOfArmor);
        }

        private float GetBasicResistance(IBasicMetaDamageEffectType basicDamageEffectType, float actualArmor)
        {
            float resistanceBeforeArmor = basicDamageEffectType.SpecificDamageTypes
                .Select(x => GetSpecificResistance(x, 0f))
                .Average();

            return GetResistanceAfterArmor(resistanceBeforeArmor, actualArmor, basicDamageEffectType.EffectOfArmor);
        }

        private float GetWeakResistance(IWeakMetaDamageEffectType weakDamageEffectType, float actualArmor)
        {
            float resistanceBeforeArmor = weakDamageEffectType.SpecificDamageTypes
                .Select(x => GetSpecificResistance(x, 0f))
                .Max();

            return GetResistanceAfterArmor(resistanceBeforeArmor, actualArmor, weakDamageEffectType.BasicMetaDamageType.EffectOfArmor);
        }

        private float GetStrongResistance(IStrongMetaDamageEffectType strongDamageEffectType, float actualArmor)
        {
            float resistanceBeforeArmor = strongDamageEffectType.SpecificDamageTypes
                .Select(x => GetSpecificResistance(x, 0f))
                .Min();

            return GetResistanceAfterArmor(resistanceBeforeArmor, actualArmor, strongDamageEffectType.BasicMetaDamageType.EffectOfArmor);
        }

        private float GetResistanceAfterArmor(float resistanceBeforeArmor, float actualArmor, float effectOfArmor)
        {
            float resistanceAfterArmor = resistanceBeforeArmor;

            for (int i = 0; i < actualArmor; i++)
            {
                resistanceAfterArmor += resistanceAfterArmor + (1 - resistanceAfterArmor) * effectOfArmor;
            }

            return resistanceAfterArmor;
        }

        public int GetBlock(IDamageEffectType damageEffectType)
        {
            ISpecificDamageEffectType specificDamageEffectType = damageEffectType as ISpecificDamageEffectType;
            if (specificDamageEffectType != null)
            {
                return GetSpecificBlock(specificDamageEffectType);
            }

            IBasicMetaDamageEffectType basicDamageEffectType = damageEffectType as IBasicMetaDamageEffectType;
            if (basicDamageEffectType != null)
            {
                return GetBasicBlock(basicDamageEffectType);
            }

            IWeakMetaDamageEffectType weakDamageEffectType = damageEffectType as IWeakMetaDamageEffectType;
            if (weakDamageEffectType != null)
            {
                return GetWeakBlock(weakDamageEffectType);
            }

            IStrongMetaDamageEffectType strongDamageEffectType = damageEffectType as IStrongMetaDamageEffectType;
            if (strongDamageEffectType != null)
            {
                return GetStrongBlock(strongDamageEffectType);
            }

            throw new Exception("Unhandled damageEffectType");
        }

        private int GetSpecificBlock(ISpecificDamageEffectType specificDamageEffectType)
        {
            return blockModifiers[specificDamageEffectType].Value;
        }

        private int GetBasicBlock(IBasicMetaDamageEffectType basicDamageEffectType)
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

        private int GetWeakBlock(IWeakMetaDamageEffectType weakDamageEffectType)
        {
            return weakDamageEffectType.SpecificDamageTypes
                .Select(x => GetSpecificBlock(x))
                .Max();
        }

        private int GetStrongBlock(IStrongMetaDamageEffectType strongDamageEffectType)
        {
            return strongDamageEffectType.SpecificDamageTypes
                .Select(x => GetSpecificBlock(x))
                .Min();
        }

        public void RegisterForOnResistanceChangedCallback(Action<float> callback, ISpecificDamageEffectType specificDamageEffectType)
        {
            OnResistanceChangedCallbackDictionary[specificDamageEffectType] += callback;
        }

        public void DeregisterForOnResistanceChangedCallback(Action<float> callback, ISpecificDamageEffectType specificDamageEffectType)
        {
            OnResistanceChangedCallbackDictionary[specificDamageEffectType] -= callback;
        }

        public void RegisterForOnAnyResistanceChangedCallback(Action<ISpecificDamageEffectType, float> callback)
        {
            OnAnyResistanceChangedCallback += callback;
        }

        public void DeregisterForOnAnyResistanceChangedCallback(Action<ISpecificDamageEffectType, float> callback)
        {
            OnAnyResistanceChangedCallback -= callback;
        }

        public void RegisterForOnBlockChanged(Action<int> callback, ISpecificDamageEffectType specificDamageEffectType)
        {
            OnBlockChangedCallbackDictionary[specificDamageEffectType] += callback;
        }

        public void DeregisterForOnBlockChanged(Action<int> callback, ISpecificDamageEffectType specificDamageEffectType)
        {
            OnBlockChangedCallbackDictionary[specificDamageEffectType] -= callback;
        }

        public void RegisterForOnAnyBlockChangedCallback(Action<ISpecificDamageEffectType, int> callback)
        {
            OnAnyBlockChangedCallback += callback;
        }

        public void DeregisterForOnAnyBlockChangeCallback(Action<ISpecificDamageEffectType, int> callback)
        {
            OnAnyBlockChangedCallback -= callback;
        }
    }
}