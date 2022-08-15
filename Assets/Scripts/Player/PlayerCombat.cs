using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//todo: afterthought: Primary and Secondary customisable weapons.
// PRIMARIES:
//  - Automatic High spread, low damage autocannon (Current).
//  - High speed, explosive, medium damage, slow firing rocket launcher.
//  - Slow speed, high damage, piercing, slow firing laser cannon.

// SECONDARIES:
//  - Extremely high damage, slow reload, slow moving bomb bay.
//  - Low damage, high spread, medium fire rate automated small machine gun.
//  - Shield battery (Player shield is wider and respawns quicker after firing).

public class PlayerCombat : MonoBehaviour
{
    // Bullet template
    [SerializeField] private GameObject playerBullet;

    // Object pool of tiles and pointer for it.
    private readonly GameObject[] pBulletPool = new GameObject[64];
    private byte pBulletPointer = 0;

    // Cooldown determines the fire rate of the player's weapon.
    public float Cooldown
    {
        get;
        private set;
    }

    public int Damage
    {
        get;
        private set;
    }

    private void Start()
    {
        // Init starter cooldown.
        Cooldown = 0.15f;

        // Initialize the player bullet pool to an offscreen position in the inactive state.
        for (int i = 0; i < pBulletPool.Length; i++)
        {
            pBulletPool[i] = Instantiate(playerBullet, new Vector3(-1, -1, -1), Quaternion.identity);
            pBulletPool[i].SetActive(false);
            pBulletPool[i].transform.SetParent(GameObject.Find("_PLAYERPROJECTILE").transform, true);
            pBulletPool[i].name = playerBullet.name + " [" + i + "]";
            pBulletPool[i].tag = "PlayerProjectile";
        }

        // Init damage per projectile.
        Damage = 1;
    }

    private void Update()
    {
        Cooldown -= Time.deltaTime;
    }

    // Can only be called from the PlayerMovement script, a projectile is fired by the player.
    public void GenerateProjectile(Vector3 playerPos)
    {
        // Select the next bullet within the bullet pool, reset it's position to the player's position and fire it (set it active).
        // Increment the pointer for the bullet pool to prepare for the next bullet to be fired.
        pBulletPool[pBulletPointer].transform.position = playerPos + new Vector3(5, 0, 0);
        pBulletPool[pBulletPointer].SetActive(true);
        
        pBulletPointer++;
        if(pBulletPointer >= pBulletPool.Length) { pBulletPointer = 0; }
        
        // 0.15s cooldown per shot (Between 6 and 7 shots per second).
        Cooldown = 0.15f;
    }
}
