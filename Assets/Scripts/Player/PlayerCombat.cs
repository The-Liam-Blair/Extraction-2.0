using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//todo: potential primary and secondary customizable weapons (for later in development).
// PRIMARIES:
//  - Automatic High spread, low damage autocannon (Current).
//  - High speed, explosive, medium damage, slow firing rocket launcher.
//  - Slow speed, high damage, piercing, slow firing laser cannon.
//  - Multi-projectile, low range, devastating at close range.
//  - Slow firing, medium damage, arcing projectile which bounces off terrain, enemies and screen boundaries.

// SECONDARIES:
//  - Extremely high damage bomb bay which glides downwards and forwards.
//  - Upgrade to player shield which widens it's arc and reactivates faster after pimrary weapon usage.
//  - Extremely short duration, medium cooldown ship phase that allows the player to pass through enemy ships and projectiles (Like Dark Souls rolling).
//  - Medium cooldown short range teleportation in the current direction of travel (Defaults to backwards).

public class PlayerCombat : MonoBehaviour
{
    // Bullet template
    [SerializeField] private GameObject playerBullet;

    // Object pool of tiles and pointer for it.
    private GameObject[] pBulletPool = new GameObject[64];
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

    // Called from the PlayerMovement script, a projectile is fired by the player.
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
