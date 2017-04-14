using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creep : MonoBehaviour {

    private Vector3 currentDestinationVector;
    private int currentDestinationPathNode;

    private CreepManager creepManager;
    private float distanceFromEnd = Mathf.Infinity;

    [SerializeField]
    private float baseSpeed = 1;
    private float currentSpeed;

    [SerializeField]
    private int maxHitpoints = 4;
    private int hitpoints;

    private Slider healthBar;

    public float DistanceFromEnd
    {
        get
        {
            return distanceFromEnd;
        }
    }

	void Start () {

        

        creepManager = FindObjectOfType<CreepManager>();

        currentSpeed = baseSpeed;

        healthBar = GetComponentInChildren<Slider>();
        healthBar.maxValue = maxHitpoints;
        healthBar.value = maxHitpoints;

        hitpoints = maxHitpoints;

        currentDestinationPathNode = creepManager.Path.Count - 2;
        currentDestinationVector = creepManager.Path[currentDestinationPathNode].ToPositionVector();

    }
	
	void Update () {

        float distanceFromCurrentDestination = Vector3.Magnitude(transform.position - currentDestinationVector);

        distanceFromEnd = currentDestinationPathNode * MapLoader.HEX_OFFSET + distanceFromCurrentDestination;

        if ( distanceFromCurrentDestination < currentSpeed * Time.deltaTime )
        {
            currentDestinationPathNode = currentDestinationPathNode - 1 < 0 ? 0 : currentDestinationPathNode - 1;
            currentDestinationVector = creepManager.Path[currentDestinationPathNode].ToPositionVector();
        }

        transform.position = Vector3.MoveTowards(transform.position, currentDestinationVector, currentSpeed * Time.deltaTime);

        if ( hitpoints < 1 )
        {
            Destroy(gameObject);
        }

    }

    public void TakeDamage(int damage)
    {
        hitpoints -= damage;
        healthBar.value = hitpoints;
    }

    private void OnDestroy()
    {
        creepManager.CreepDied(this);
    }
}
