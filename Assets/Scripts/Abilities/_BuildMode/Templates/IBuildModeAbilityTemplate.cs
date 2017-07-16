public interface IBuildModeAbilityTemplate : IAbilityTemplate {

    EconomyTransaction Cost { get; }

    IBuildModeTargetingComponent TargetingComponent { get; }

    IBuildModeEffectComponent EffectComponent { get; }
}
