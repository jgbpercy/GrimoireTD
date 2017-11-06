using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.AttackEffects;
using GrimoireTD.Defenders;

namespace GrimoireTD.Abilities.DefendMode.Projectiles
{
    [CreateAssetMenu(fileName = "NewAoeProjectile", menuName = "Defend Mode Abilities/Projectiles/AOE Projectile")]
    public class SoAoeProjectileTemplate : SoProjectileTemplate, IAoeProjectileTemplate
    {
        [SerializeField]
        protected SAttackEffect[] aoeAttackEffects;

        [SerializeField]
        protected float aoeRadius;

        [SerializeField]
        protected float aoeExplosionTime;

        public IEnumerable<IAttackEffect> AoeAttackEffects
        {
            get
            {
                return aoeAttackEffects;
            }
        }

        public float AoeRadius
        {
            get
            {
                return aoeRadius;
            }
        }

        public float AoeExplosionTime
        {
            get
            {
                return aoeExplosionTime;
            }
        }

        public override IProjectile GenerateProjectile(Vector3 startPosition, IDefendModeTargetable target, IDefender sourceDefender)
        {
            return new CAoeProjectile(startPosition, target, this, sourceDefender);
        }
    }
}