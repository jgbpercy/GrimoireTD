using GrimoireTD.Abilities.DefendMode.Projectiles;

namespace GrimoireTD.Abilities.DefendMode
{
    public interface IProjectileLauncherComponentTemplate : IDefendModeEffectComponentTemplate
    {
        IProjectileTemplate ProjectileToFireTemplate { get; }
    }
}