using System;
using GrimoireTD.Abilities;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Attributes;
using GrimoireTD.TemporaryEffects;
using GrimoireTD.Creeps;
using GrimoireTD.Technical;
using GrimoireTD.Map;
using GrimoireTD.Economy;
using GrimoireTD.Abilities.DefendMode.AttackEffects;

// *** It should go without saying that changing any of this stuff outside of tests is inadvisable ***

namespace GrimoireTD.Dependencies
{
    public static class DepsProv
    {
        //***Game Model***

        private static IReadOnlyGameModel theGameModel;

        public static void SetTheGameModel(IReadOnlyGameModel gameModel)
        {
            theGameModel = gameModel;
        }

        public static IReadOnlyGameModel TheGameModel
        {
            get
            {
                return theGameModel;
            }
        }

        public static IReadOnlyGameStateManager TheGameStateManager
        {
            get
            {
                return theGameModel.GameStateManager;
            }
        }

        public static IReadOnlyMapData TheMapData
        {
            get
            {
                return theGameModel.MapData;
            }
        }

        public static IReadOnlyCreepManager TheCreepManager
        {
            get
            {
                return theGameModel.CreepManager;
            }
        }

        public static IReadOnlyEconomyManager TheEconomyManager
        {
            get
            {
                return theGameModel.EconomyManager;
            }
        }

        public static IReadOnlyAttackEffectTypeManager TheAttackEffectTypeManager 
        {
            get
            {
                return theGameModel.AttackEffectTypeManager;
            }
        }

        //***Model Object Frame Updater***

        public static Func<IModelObjectFrameUpdater> TheModelObjectFrameUpdater = () =>
        {
            return ModelObjectFrameUpdater.Instance;
        };

        //***Instance Dependencies***

        public static Func<IAbilities, IDefendingEntity, IDefendModeAbilityManager> DefendModeAbilityManager = 
            (abilities, defendingEntity) =>
        {
            return new CDefendModeAbilityManager(abilities, defendingEntity);
        };

        public static Func<IAttributes<CreepAttrName>> CreepAttributes = () =>
        {
            return new CAttributes<CreepAttrName>(CreepAttributeDefinitions.NewAttributesDictionary());
        };

        public static Func<ITemporaryEffectsManager> TemporaryEffectsManager = () =>
        {
            return new CTemporaryEffectsManager();
        };

        public static Func<ICreep, IBaseResistances, IResistances> Resistances = (attachedToCreep, baseResistances) =>
        {
            return new CResistances(attachedToCreep, baseResistances);
        };

        public static Func<IAttributes<DEAttrName>> DefendingEntityAttributes = () =>
        {
            return new CAttributes<DEAttrName>(DefendingEntityAttributeDefinitions.NewAttributesDictionary());
        };

        public static Func<IDefendingEntity, IAbilities> Abilities = (attachedToDefendingEntity) =>
        {
            return new CAbilities(attachedToDefendingEntity);
        };
    }
}