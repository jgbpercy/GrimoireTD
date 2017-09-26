using UnityEngine;
using GrimoireTD.Abilities.DefendMode.Projectiles;

namespace GrimoireTD.Abilities.DefendMode
{
    [CreateAssetMenu(fileName = "NewProjectileLauncher", menuName = "Defend Mode Abilities/Effect Components/Projectile Launcher")]
    public class SoProjectileLauncherComponentTemplate : SoDefendModeEffectComponentTemplate, IProjectileLauncherComponentTemplate
    {
        [SerializeField]
        protected SoProjectileTemplate projectileToFireTemplate;

        public IProjectileTemplate ProjectileToFireTemplate
        {
            get
            {
                return projectileToFireTemplate;
            }
        }

        public override IDefendModeEffectComponent GenerateEffectComponent()
        {
            return new CProjectileLauncherComponent(this);
        }
    }
}