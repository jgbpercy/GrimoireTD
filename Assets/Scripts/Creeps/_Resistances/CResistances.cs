using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.AttackEffects;
using GrimoireTD.Attributes;

namespace GrimoireTD.Creeps
{
    public class CResistances : IResistances
    {
        private Dictionary<
            ISpecificDamageEffectType, 
            RecalculatorList<IResistanceModifier, float>> 
            resistanceModifiers = new Dictionary<ISpecificDamageEffectType, RecalculatorList<IResistanceModifier, float>>();

        private Dictionary<
            IMetaDamageEffectType, 
            MutableRecalculatorList<
                RecalculatorList<IResistanceModifier, float>, 
                float, 
                EARecalculatorListChange<float>>> 
            metaResistanceDict = new Dictionary<IMetaDamageEffectType, MutableRecalculatorList<RecalculatorList<IResistanceModifier, float>, float, EARecalculatorListChange<float>>>();

        private Dictionary<
            ISpecificDamageEffectType, 
            RecalculatorList<IBlockModifier, int>> 
            blockModifiers = new Dictionary<ISpecificDamageEffectType, RecalculatorList<IBlockModifier, int>>();

        private Dictionary<
            IMetaDamageEffectType, 
            MutableRecalculatorList<
                RecalculatorList<IBlockModifier, int>, 
                int, 
                EARecalculatorListChange<int>>> 
            metaBlockDict = new Dictionary<IMetaDamageEffectType, MutableRecalculatorList<RecalculatorList<IBlockModifier, int>, int, EARecalculatorListChange<int>>>();

        private Dictionary<ISpecificDamageEffectType, IResistanceModifier> modifiersFromArmor = new Dictionary<ISpecificDamageEffectType, IResistanceModifier>();

        public event EventHandler<EAOnAnyResistanceChanged> OnAnyResistanceChanged;

        public event EventHandler<EAOnAnyBlockChanged> OnAnyBlockChanged;

        public CResistances(ICreep attachedToCreep, IBaseResistances baseResistances)
        {
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

            AddArmorResistanceModifiers(attachedToCreep.CurrentArmor);

            attachedToCreep.OnArmorChanged += OnArmorChanged;
        }

        public IReadOnlyRecalculatorList<float> GetResistance(IDamageEffectType damageEffectType)
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

        /* TODO: #optimisation: can definitely way reduce number of calcs here by returning a dto like thing where we only calculate
         * each specific damage type's value once as an optimised call when all are needed (e.g. on armor change) 
         * with the ability to still just get a specific one when a specific resistance changes
         * */
        public float GetResistanceWithoutArmor(IDamageEffectType damageEffectType)
        {
            ISpecificDamageEffectType specificDamageEffectType = damageEffectType as ISpecificDamageEffectType;
            if (specificDamageEffectType != null)
            {
                return GetSpecificResistanceWithoutArmor(specificDamageEffectType);
            }

            IBasicMetaDamageEffectType basicDamageEffectType = damageEffectType as IBasicMetaDamageEffectType;
            if (basicDamageEffectType != null)
            {
                return GetBasicResistanceWithoutArmor(basicDamageEffectType);
            }

            IWeakMetaDamageEffectType weakDamageEffectType = damageEffectType as IWeakMetaDamageEffectType;
            if (weakDamageEffectType != null)
            {
                return GetWeakResistanceWithoutArmor(weakDamageEffectType);
            }

            IStrongMetaDamageEffectType strongDamageEffectType = damageEffectType as IStrongMetaDamageEffectType;
            if (strongDamageEffectType != null)
            {
                return GetStrongResistanceWithoutArmor(strongDamageEffectType);
            }

            throw new Exception("Unhandled damageEffectType");
        }

        private void OnArmorChanged(object sender, EAOnAttributeChanged args)
        {
            RemoveArmorResistanceModifiers();

            AddArmorResistanceModifiers(args.NewValue);
        }

        private void RemoveArmorResistanceModifiers()
        {
            foreach (var specificDamageType in GameModels.Models[0].AttackEffectTypeManager.SpecificDamageTypes)
            {
                //TODO #optimisation: surpress event on these removes as we will always be firing on the adds?
                RemoveResistanceModifer(modifiersFromArmor[specificDamageType]);
                modifiersFromArmor.Remove(specificDamageType);
            }
        }

        private void AddArmorResistanceModifiers(float newArmorValue)
        {
            foreach (var basicDamageType in GameModels.Models[0].AttackEffectTypeManager.BasicMetaDamageTypes)
            {
                float armorModifierMagnitude = 1 - Mathf.Pow((1 - basicDamageType.EffectOfArmor), newArmorValue);

                foreach (var specificDamageType in basicDamageType.SpecificDamageTypes)
                {
                    var newResistanceModifier = new CResistanceModifier(armorModifierMagnitude, specificDamageType);

                    AddResistanceModifier(newResistanceModifier);

                    modifiersFromArmor.Add(specificDamageType, newResistanceModifier);
                }
            }
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

            return Math.Max(block, 0);
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

        private float GetSpecificResistanceWithoutArmor(ISpecificDamageEffectType specificDamageEffectType)
        {
            float resistance = GetResistance(specificDamageEffectType).Value;

            float armorModifierMagnitude = modifiersFromArmor[specificDamageEffectType].Magnitude;

            return 1 - ((1 - resistance) / (1 - armorModifierMagnitude));
        }

        private float GetBasicResistanceWithoutArmor(IBasicMetaDamageEffectType basicDamageEffectType)
        {
            return basicDamageEffectType.SpecificDamageTypes
                .Select(x => GetResistanceWithoutArmor(x))
                .Average();
        }

        private float GetWeakResistanceWithoutArmor(IWeakMetaDamageEffectType weakDamageEffectType)
        {
            return weakDamageEffectType.SpecificDamageTypes
                .Select(x => GetResistanceWithoutArmor(x))
                .Max();
        }

        private float GetStrongResistanceWithoutArmor(IStrongMetaDamageEffectType strongDamageEffectType)
        {
            return strongDamageEffectType.SpecificDamageTypes
                .Select(x => GetResistanceWithoutArmor(x))
                .Min();
        }
    }
}