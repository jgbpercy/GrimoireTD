using UnityEngine;
using System.Collections.Generic;
using GrimoireTD.Abilities.DefendMode.AttackEffects;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.DefendMode.Projectiles
{
    [CreateAssetMenu(fileName = "NewProjectile", menuName = "Defend Mode Abilities/Projectiles/Projectile")]
    public class SoProjectileTemplate : ScriptableObject, IProjectileTemplate
    {
        [SerializeField]
        protected SAttackEffect[] attackEffects;

        [SerializeField]
        protected float speed;

        public IEnumerable<IAttackEffect> AttackEffects
        {
            get
            {
                return attackEffects;
            }
        }

        public float Speed
        {
            get
            {
                return speed;
            }
        }

        public virtual IProjectile GenerateProjectile(Vector3 startPosition, IDefendModeTargetable target, IDefendingEntity sourceDefendingEntity)
        {
            return new CProjectile(startPosition, target, this, sourceDefendingEntity);
        }
    }
}