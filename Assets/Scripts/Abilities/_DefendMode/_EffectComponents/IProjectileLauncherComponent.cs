using GrimoireTD.Abilities.DefendMode.Projectiles;

namespace GrimoireTD.Abilities.DefendMode
{
    public interface IProjectileLauncherComponent
    {
        IProjectileTemplate ProjectileToFireTemplate { get; }
    }
}