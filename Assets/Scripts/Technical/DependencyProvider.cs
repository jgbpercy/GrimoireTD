using System;
using GrimoireTD.Abilities;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Attributes;
using GrimoireTD.TemporaryEffects;
using GrimoireTD.Creeps;
using GrimoireTD.Technical;

namespace GrimoireTD.Dependencies
{
    public static class DependencyProvider
    {
        //Model Object Frame Updater
        public static Func<IModelObjectFrameUpdater> TheModelObjectFrameUpdater = () =>
        {
            return ModelObjectFrameUpdater.Instance;
        };

        //Instance Dependencies
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
    }
}