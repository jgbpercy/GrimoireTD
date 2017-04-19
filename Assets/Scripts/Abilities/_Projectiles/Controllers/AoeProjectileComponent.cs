using UnityEngine;
using UnityEngine.Assertions;

public class AoeProjectileComponent : ProjectileComponent {

    private bool isExploding = false;

    private AoeProjectile aoeProjectileModel;

    public override void SetUp(Projectile projectileModel)
    {
        base.SetUp(projectileModel);

        aoeProjectileModel = (AoeProjectile)projectileModel;
        Assert.IsTrue(aoeProjectileModel != null);

        aoeProjectileModel.RegisterForOnExplosionCallback( () => { isExploding = true; } );
        aoeProjectileModel.RegisterForOnExplosionFinishedCallback(() => { ownCollider.enabled = false; isExploding = false; });

    }

    protected override void OnTriggerEnter(Collider other)
    {
        if ( !isExploding )
        {
            base.OnTriggerEnter(other);
            ownCollider.enabled = true;
        }
        else
        {
            if ( other.CompareTag("Creep") )
            {
                Creep hitCreep = other.GetComponent<CreepComponent>().CreepModel;
                aoeProjectileModel.HitCreepInAoe(hitCreep);
            }
        }
    }

    protected override void Update () {

        base.Update();

        if ( isExploding )
        {
            ownCollider.radius = aoeProjectileModel.CurrentAoeRadius;
        }
	}
}
