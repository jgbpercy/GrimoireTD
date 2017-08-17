using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.AttackEffects;
using GrimoireTD.Technical;

namespace GrimoireTD.Creeps
{
    public class Resistances : IReadOnlyResistances
    {
        private Dictionary<SpecificDamageEffectType, RecalculatorList<IResistanceModifier, float>> positiveResistanceModifiers;
        private Dictionary<SpecificDamageEffectType, RecalculatorList<IResistanceModifier, float>> negativeResistanceModifiers;

        private Dictionary<SpecificDamageEffectType, RecalculatorList<IBlockModifier, int>> blockModifiers;

        private Dictionary<SpecificDamageEffectType, Action<float>> OnResistanceChangedCallbackDictionary;
        private Action<SpecificDamageEffectType, float> OnAnyResistanceChangedCallback;

        private Dictionary<SpecificDamageEffectType, Action<int>> OnBlockChangedCallbackDictionary;
        private Action<SpecificDamageEffectType, int> OnAnyBlockChangedCallback;

        public Resistances(BaseResistances baseResistances)
        {
            positiveResistanceModifiers = new Dictionary<SpecificDamageEffectType, RecalculatorList<IResistanceModifier, float>>();
            negativeResistanceModifiers = new Dictionary<SpecificDamageEffectType, RecalculatorList<IResistanceModifier, float>>();
            blockModifiers = new Dictionary<SpecificDamageEffectType, RecalculatorList<IBlockModifier, int>>();

            OnResistanceChangedCallbackDictionary = new Dictionary<SpecificDamageEffectType, Action<float>>();
            OnBlockChangedCallbackDictionary = new Dictionary<SpecificDamageEffectType, Action<int>>();

            foreach (SpecificDamageEffectType specificDamageType in AttackEffectTypeManager.Instance.SpecificDamageTypes)
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

        public float GetResistance(DamageEffectType damageEffectType, float currentArmor)
        {
            SpecificDamageEffectType specificDamageEffectType = damageEffectType as SpecificDamageEffectType;
            if (specificDamageEffectType != null)
            {
                return GetSpecificResistance(specificDamageEffectType, currentArmor);
            }

            BasicMetaDamageEffectType basicDamageEffectType = damageEffectType as BasicMetaDamageEffectType;
            if (basicDamageEffectType != null)
            {
                return GetBasicResistance(basicDamageEffectType, currentArmor);
            }

            WeakMetaDamageEffectType weakDamageEffectType = damageEffectType as WeakMetaDamageEffectType;
            if (weakDamageEffectType != null)
            {
                return GetWeakResistance(weakDamageEffectType, currentArmor);
            }

            StrongMetaDamageEffectType strongDamageEffectType = damageEffectType as StrongMetaDamageEffectType;
            if (strongDamageEffectType != null)
            {
                return GetStrongResistance(strongDamageEffectType, currentArmor);
            }

            throw new Exception("Unhandled damageEffectType");
        }

        private float GetSpecificResistance(SpecificDamageEffectType specificDamageEffectType, float actualArmor)
        {
            float resistanceBeforeArmor = positiveResistanceModifiers[specificDamageEffectType].Value * negativeResistanceModifiers[specificDamageEffectType].Value;

            return GetResistanceAfterArmor(resistanceBeforeArmor, actualArmor, specificDamageEffectType.BasicMetaDamageEffectType.EffectOfArmor);
        }

        private float GetBasicResistance(BasicMetaDamageEffectType basicDamageEffectType, float actualArmor)
        {
            float resistanceBeforeArmor = basicDamageEffectType.SpecificDamageTypes
                .Select(x => GetSpecificResistance(x, 0f))
                .Average();

            return GetResistanceAfterArmor(resistanceBeforeArmor, actualArmor, basicDamageEffectType.EffectOfArmor);
        }

        private float GetWeakResistance(WeakMetaDamageEffectType weakDamageEffectType, float actualArmor)
        {
            float resistanceBeforeArmor = weakDamageEffectType.SpecificDamageTypes
                .Select(x => GetSpecificResistance(x, 0f))
                .Max();

            return GetResistanceAfterArmor(resistanceBeforeArmor, actualArmor, weakDamageEffectType.BasicMetaDamageType.EffectOfArmor);
        }

        private float GetStrongResistance(StrongMetaDamageEffectType strongDamageEffectType, float actualArmor)
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
            return blockModifiers[specificDamageEffectType].Value;
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

        public void RegisterForOnResistanceChangedCallback(Action<float> callback, SpecificDamageEffectType specificDamageEffectType)
        {
            OnResistanceChangedCallbackDictionary[specificDamageEffectType] += callback;
        }

        public void DeregisterForOnResistanceChangedCallback(Action<float> callback, SpecificDamageEffectType specificDamageEffectType)
        {
            OnResistanceChangedCallbackDictionary[specificDamageEffectType] -= callback;
        }

        public void RegisterForOnAnyResistanceChangedCallback(Action<SpecificDamageEffectType, float> callback)
        {
            OnAnyResistanceChangedCallback += callback;
        }

        public void DeregisterForOnAnyResistanceChangedCallback(Action<SpecificDamageEffectType, float> callback)
        {
            OnAnyResistanceChangedCallback -= callback;
        }

        public void RegisterForOnBlockChanged(Action<int> callback, SpecificDamageEffectType specificDamageEffectType)
        {
            OnBlockChangedCallbackDictionary[specificDamageEffectType] += callback;
        }

        public void DeregisterForOnBlockChanged(Action<int> callback, SpecificDamageEffectType specificDamageEffectType)
        {
            OnBlockChangedCallbackDictionary[specificDamageEffectType] -= callback;
        }

        public void RegisterForOnAnyBlockChangedCallback(Action<SpecificDamageEffectType, int> callback)
        {
            OnAnyBlockChangedCallback += callback;
        }

        public void DeregisterForOnAnyBlockChangeCallback(Action<SpecificDamageEffectType, int> callback)
        {
            OnAnyBlockChangedCallback -= callback;
        }
    }
}