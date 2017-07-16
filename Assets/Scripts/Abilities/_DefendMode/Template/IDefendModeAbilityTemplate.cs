public interface IDefendModeAbilityTemplate : IAbilityTemplate {

    float BaseCooldown { get; }

    IDefendModeTargetingComponent TargetingComponent { get; }

    IDefendModeEffectComponent EffectComponent { get; }
}
