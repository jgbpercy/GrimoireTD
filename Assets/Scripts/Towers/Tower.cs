using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {

    [SerializeField]
    private Transform firePoint;

    [SerializeField]
    private GameObject projectile;
    [SerializeField]
    private float shotCooldown;
    [SerializeField]
    private float range = 50f;
    [SerializeField]
    private string nameInGame;

    /*
    public float RateOfFire
    {
        get
        {
            return 1 / shotCooldown;
        }
    }*/

    public float Range
    {
        get
        {
            return range;
        }
    }

    public string NameInGame
    {
        get
        {
            return nameInGame;
        }
    }

    private float timeSinceShot = 0f;

    private CreepManager creepManager;

    private void Start()
    {
        creepManager = GameObject.Find("GameManager").GetComponent<CreepManager>();
    }

    private void Update () {

        timeSinceShot += Time.deltaTime;
        if ( timeSinceShot > shotCooldown)
        {
            Shoot();
        }
	}

    private void Shoot()
    {
        Transform potentialTarget = creepManager.CreepInRangeNearestToEnd(transform.position, range);

        if ( potentialTarget != null )
        {
            Projectile shot = Instantiate(projectile, firePoint.position, Quaternion.identity).GetComponent<Projectile>();

            shot.target = potentialTarget.GetChild(0).gameObject;

            timeSinceShot = 0f;
        }
    }

    public string TowerStatsUIText()
    {
        return "Range: <i>" + range + "</i>, RoF: <i>" + (1 / shotCooldown) + "</i>";
    }
}
