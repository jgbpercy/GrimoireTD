using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Creeps
{
    public class CResistances : IResistances
    {
        private Dictionary<ISpecificDamageEffectType, RecalculatorList<IResistanceModifier, float>> resistanceModifiers;

        private Dictionary<IMetaDamageEffectType, MutableRecalculatorList<RecalculatorList<IResistanceModifier, float>, float, EARecalculatorListChange<float>>> metaResistanceDict;

        private Dictionary<ISpecificDamageEffectType, RecalculatorList<IBlockModifier, int>> blockModifiers;

        private Dictionary<IMetaDamageEffectType, MutableRecalculatorList<RecalculatorList<IBlockModifier, int>, int, EARecalculatorListChange<int>>> metaBlockDict;

        public event EventHandler<EAOnAnyResistanceChanged> OnAnyResistanceChanged;

        public event EventHandler<EAOnAnyBlockChanged> OnAnyBlockChanged;

        public CResistances(IBaseResistances baseResistances)
        {
            resistanceModifiers = new Dictionary<ISpecificDamageEffectType, RecalculatorList<IResistanceModifier, float>>();
            metaResistanceDict = new Dictionary<IMetaDamageEffectType, MutableRecalculatorList<RecalculatorList<IResistanceModifier, float>, float, EARecalculatorListChange<float>>>();
            
            blockModifiers = new Dictionary<ISpecificDamageEffectType, RecalculatorList<IBlockModifier, int>>();
            metaBlockDict = new Dictionary<IMetaDamageEffectType, MutableRecalculatorList<RecalculatorList<IBlockModifier, int>, int, EARecalculatorListChange<int>>>();

            foreach (var specificDamageType in GameModels.Models[0].AttackEffectTypeManager.SpecificDamageTypes)
            {
                resistanceModifiers.Add(specificDamageType, new RecalculatorList<IResistanceModifier, float>(CalculateSpecificResistance));
                resistanceModifiers[specificDamageType].Add(baseResistances.GetResistanceModifier(specificDamageType));

                blockModifiers.Add(specificDamageType, new RecalculatorList<IBlockModifier, int>(CalculateSpecificBlock));
                blockModifiers[specificDamageType].Add(baseResistances.GetBlockModifier(specificDamageType));
            }

            foreach (var basicMetaDamageType in GameModels.Models[0].AttackEffectTypeManager.BasicMetaDamageTypes)
            {
                var specificDamageTypeResistanceLists = basicMetaDamageType.SpecificDamageTypes.Select(x => resistanceModifiers[x]).ToList();
                var specificDamageTypeBlockLists = basicMetaDamageType.SpecificDamageTypes.Select(x => blockModifiers[x]).ToList();

                metaResistanceDict.Add(
                    basicMetaDamageType,
                    new MutableRecalculatorList<RecalculatorList<IResistanceModifier, float>, float, EARecalculatorListChange<float>>(
                        specificDamageTypeResistanceLists,
                        CalculateBasicResistance
                    )
                );

                metaResistanceDict.Add(
                    basicMetaDamageType.WeakMetaDamageType,
                    new MutableRecalculatorList<RecalculatorList<IResistanceModifier, float>, float, EARecalculatorListChange<float>>(
                        specificDamageTypeResistanceLists,
                        CalculateWeakResistance
                    )
                );

                metaResistanceDict.Add(
                    basicMetaDamageType.StrongMetaDamageType,
                    new MutableRecalculatorList<RecalculatorList<IResistanceModifier, float>, float, EARecalculatorListChange<float>>(
                        specificDamageTypeResistanceLists,
                        CalculateStrongResistance
                    )
                );

                metaBlockDict.Add(
                    basicMetaDamageType,
                    new MutableRecalculatorList<RecalculatorList<IBlockModifier, int>, int, EARecalculatorListChange<int>>(
                        specificDamageTypeBlockLists,
                        CalculateBasicBlock
                    )
                );

                metaBlockDict.Add(
                    basicMetaDamageType.WeakMetaDamageType,
                    new MutableRecalculatorList<RecalculatorList<IBlockModifier, int>, int, EARecalculatorListChange<int>>(
                        specificDamageTypeBlockLists,
                        CalculateWeakBlock
                    )
                );

                metaBlockDict.Add(
                    basicMetaDamageType.StrongMetaDamageType,
                    new MutableRecalculatorList<RecalculatorList<IBlockModifier, int>, int, EARecalculatorListChange<int>>(
                        specificDamageTypeBlockLists,
                        CalculateStrongBlock
                    )
                );
            }
        }

        public IReadOnlyRecalculatorList<float> GetBaseResistance(IDamageEffectType damageEffectType)
        {
            var specificDamageEffectType = damageEffectType as ISpecificDamageEffectType;
            if (specificDamageEffectType != null)
            {
                return resistanceModifiers[specificDamageEffectType];
            }

            var metaDamageEffectType = damageEffectType as IMetaDamageEffectType;
            if (metaDamageEffectType != null)
            {
                return metaResistanceDict[metaDamageEffectType];
            }

            throw new Exception("Unhandled damageEffectType");
        }

        public IReadOnlyRecalculatorList<int> GetBlock(IDamageEffectType damageEffectType)
        {
            var specificDamageEffectType = damageEffectType as ISpecificDamageEffectType;
            if (specificDamageEffectType != null)
            {
                return blockModifiers[specificDamageEffectType];
            }

            var metaDamageEffectType = damageEffectType as IMetaDamageEffectType;
            if (metaDamageEffectType != null)
            {
                return metaBlockDict[metaDamageEffectType];
            }

            throw new Exception("Unhandled damageEffectType");
        }

        private float CalculateSpecificResistance(List<IResistanceModifier> modifierList)
        {
            var positiveModifiers = modifierList.Where(x => x.Magnitude >= 0).ToArray();
            var negativeModifiers = modifierList.Where(x => x.Magnitude < 0).ToArray();

            return CalculatePositiveResistance(positiveModifiers) * CalculateNegativeResistanceMultiplier(negativeModifiers);
        }

        private float CalculatePositiveResistance(IResistanceModifier[] modifierList)
        {
            float resistance = 0;

            foreach (IResistanceModifier modifier in modifierList)
            {
                resistance += (1 - resistance) * modifier.Magnitude;
            }

            return resistance;
        }

        private float CalculateNegativeResistanceMultiplier(IResistanceModifier[] modifierList)
        {
            float resistanceMultiplier = 1;

            foreach (IResistanceModifier modifier in modifierList)
            {
                resistanceMultiplier = resistanceMultiplier * (1 + modifier.Magnitude);
            }

            return resistanceMultiplier;
        }

        private float CalculateBasicResistance(List<RecalculatorList<IResistanceModifier, float>> specificResistanceList)
        {
            return specificResistanceList.Select(x => x.Value).Average();
        }

        private float CalculateWeakResistance(List<RecalculatorList<IResistanceModifier, float>> specificResistanceList)
        {
            return specificResistanceList.Select(x => x.Value).Max();
        }

        private float CalculateStrongResistance(List<RecalculatorList<IResistanceModifier, float>> specificResistanceList)
        {
            return specificResistanceList.Select(x => x.Value).Min();
        }

        private int CalculateSpecificBlock(IList<IBlockModifier> modifierList)
        {
            int block = 0;

            foreach (IBlockModifier modifier in modifierList)
            {
                block += modifier.Magnitude;
            }

            return Math.Min(block, 0);
        }

        private int CalculateBasicBlock(List<RecalculatorList<IBlockModifier, int>> specificBlockList)
        {
            return Mathf.RoundToInt(
                (float)
                (
                    specificBlockList
                    .Select(x => x.Value)
                    .Average()
                )
            );
        }

        private int CalculateWeakBlock(List<RecalculatorList<IBlockModifier, int>> specificBlockList)
        {
            return specificBlockList
                .Select(x => x.Value)
                .Max();
        }

        private int CalculateStrongBlock(List<RecalculatorList<IBlockModifier, int>> specificBlockList)
        {
            return specificBlockList
                .Select(x => x.Value)
                .Min();
        }


        public void AddResistanceModifier(IResistanceModifier modifier)
        {
            var newValue = resistanceModifiers[modifier.DamageType].Add(modifier);

            OnAnyResistanceChanged?.Invoke(this, new EAOnAnyResistanceChanged(modifier.DamageType, newValue));
        }

        public void RemoveResistanceModifer(IResistanceModifier modifier)
        {
            var newValue = resistanceModifiers[modifier.DamageType].Remove(modifier);

            OnAnyResistanceChanged?.Invoke(this, new EAOnAnyResistanceChanged(modifier.DamageType, newValue));
        }

        public void AddBlockModifier(IBlockModifier modifier)
        {
            var newValue = blockModifiers[modifier.DamageType].Add(modifier);

            OnAnyBlockChanged?.Invoke(this, new EAOnAnyBlockChanged(modifier.DamageType, newValue));
        }

        public void RemoveBlockModifier(IBlockModifier modifier)
        {
            var newValue = blockModifiers[modifier.DamageType].Remove(modifier);

            OnAnyBlockChanged?.Invoke(this, new EAOnAnyBlockChanged(modifier.DamageType, newValue));
        }

        public float GetResistanceAfterArmor(IDamageEffectType damageEffectType, float currentArmor)
        {
            ISpecificDamageEffectType specificDamageEffectType = damageEffectType as ISpecificDamageEffectType;
            if (specificDamageEffectType != null)
            {
                return GetSpecificResistanceAfterArmor(specificDamageEffectType, currentArmor);
            }

            IBasicMetaDamageEffectType basicDamageEffectType = damageEffectType as IBasicMetaDamageEffectType;
            if (basicDamageEffectType != null)
            {
                return GetBasicResistanceAfterArmor(basicDamageEffectType, currentArmor);
            }

            IWeakMetaDamageEffectType weakDamageEffectType = damageEffectType as IWeakMetaDamageEffectType;
            if (weakDamageEffectType != null)
            {
                return GetWeakResistanceAfterArmor(weakDamageEffectType, currentArmor);
            }

            IStrongMetaDamageEffectType strongDamageEffectType = damageEffectType as IStrongMetaDamageEffectType;
            if (strongDamageEffectType != null)
            {
                return GetStrongResistanceAfterArmor(strongDamageEffectType, currentArmor);
            }

            throw new Exception("Unhandled damageEffectType");
        }

        private float GetSpecificResistanceAfterArmor(ISpecificDamageEffectType specificDamageEffectType, float actualArmor)
        {
            float resistanceBeforeArmor = resistanceModifiers[specificDamageEffectType].Value;

            return GetResistanceAfterArmor(resistanceBeforeArmor, actualArmor, specificDamageEffectType.BasicMetaDamageEffectType.EffectOfArmor);
        }

        //TODO: put the relevant stuff on a meta damage type so this can be one method
        private float GetBasicResistanceAfterArmor(IBasicMetaDamageEffectType basicDamageEffectType, float actualArmor)
        {
            float resistanceBeforeArmor = metaResistanceDict[basicDamageEffectType].Value;

            return GetResistanceAfterArmor(resistanceBeforeArmor, actualArmor, basicDamageEffectType.EffectOfArmor);
        }

        private float GetWeakResistanceAfterArmor(IWeakMetaDamageEffectType weakDamageEffectType, float actualArmor)
        {
            float resistanceBeforeArmor = metaResistanceDict[weakDamageEffectType].Value;

            return GetResistanceAfterArmor(resistanceBeforeArmor, actualArmor, weakDamageEffectType.BasicMetaDamageType.EffectOfArmor);
        }

        private float GetStrongResistanceAfterArmor(IStrongMetaDamageEffectType strongDamageEffectType, float actualArmor)
        {
            float resistanceBeforeArmor = metaResistanceDict[strongDamageEffectType].Value;

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
    }
}