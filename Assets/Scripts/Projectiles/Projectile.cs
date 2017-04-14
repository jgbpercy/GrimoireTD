using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    [HideInInspector]
    public GameObject target;

    [SerializeField]
    private float speed;
    [SerializeField]
    protected int damage;

    [SerializeField]
    private ParticleSystem hitExplosion;
    [SerializeField]
    private MeshRenderer ownRenderer;
    protected SphereCollider ownCollider;

    private Vector3 currentDirection = Vector3.zero;
    private bool destroyingForNoTarget;
    protected bool destroyingForHitTarget;

    private void Start()
    {
        ownCollider = gameObject.GetComponent<SphereCollider>();
        destroyingForNoTarget = false;
        destroyingForHitTarget = false;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if ( other.CompareTag("Creep") )
        {
            Creep hitCreep = other.GetComponent<Creep>();
            hitCreep.TakeDamage(damage);
            hitExplosion.Play();
            ownRenderer.enabled = false;
            ownCollider.enabled = false;
            destroyingForHitTarget = true;
            Destroy(gameObject, hitExplosion.main.duration);
        }
    }

    protected virtual void Update ()
    {
        if ( destroyingForHitTarget )
        {
            return;
        }

        if ( target == null )
        {
            if ( !destroyingForNoTarget)
            {
                Destroy(gameObject, 5f);
                destroyingForNoTarget = true;
            }

            if ( currentDirection == Vector3.zero )
            {
                Destroy(gameObject);
            }
            transform.position = transform.position + currentDirection * speed * Time.deltaTime;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            currentDirection = (target.transform.position - transform.position).normalized;
        }
        
	}

}
