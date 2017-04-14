using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeProjectile : Projectile {

    [SerializeField]
    private int aoeDamage; 

    [SerializeField]
    private int aoeRadius;

    [SerializeField]
    private float aoeExpansionLerpFactor = 0.15f;
    
    protected override void OnTriggerEnter(Collider other)
    {
        if ( !destroyingForHitTarget )
        {
            base.OnTriggerEnter(other);
            ownCollider.enabled = true;
        }
        else
        {
            if ( other.CompareTag("Creep") )
            {
                Creep hitCreep = other.GetComponent<Creep>();
                hitCreep.TakeDamage(aoeDamage);
            }
        }
    }

    protected override void Update () {
        base.Update();

        if ( destroyingForHitTarget )
        {
            ownCollider.radius = Mathf.Lerp(ownCollider.radius, aoeRadius, aoeExpansionLerpFactor);

            if (aoeRadius - ownCollider.radius < 0.01f)
            {
                ownCollider.enabled = false;
            }
        }
	}
}
