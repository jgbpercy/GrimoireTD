using System;
using GrimoireTD.Abilities;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.Defenders;
using GrimoireTD.Attributes;
using GrimoireTD.TemporaryEffects;
using GrimoireTD.Creeps;
using GrimoireTD.Technical;
using GrimoireTD.Map;
using GrimoireTD.Economy;
using GrimoireTD.Abilities.DefendMode.AttackEffects;
using GrimoireTD.UI;

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

        public static IReadOnlyGameModeManager TheGameModeManager
        {
            get
            {
                return theGameModel.GameModeManager;
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

        //***Controllers***

        public static Func<IModelInterfaceController> TheInterfaceController = () =>
        {
            return InterfaceController.Instance;
        };

        //***Instance Dependencies***

        public static Func<IAbilities, IDefender, IDefendModeAbilityManager> DefendModeAbilityManager = 
            (abilities, defender) =>
        {
            return new CDefendModeAbilityManager(abilities, defender);
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

        public static Func<IAttributes<DeAttrName>> DefenderAttributes = () =>
        {
            return new CAttributes<DeAttrName>(DefenderAttributeDefinitions.NewAttributesDictionary());
        };

        public static Func<IDefender, IAbilities> Abilities = (attachedToDefender) =>
        {
            return new CAbilities(attachedToDefender);
        };

        public static Func<object, float, float, string, EventHandler<EAOnTemporaryEffectEnd>, ITemporaryEffect> TemporaryEffect =
            (key, magnitude, duration, effectName, onEndEvent) =>
        {
            return new CTemporaryEffect(key, magnitude, duration, effectName, onEndEvent);
        };
    }
}