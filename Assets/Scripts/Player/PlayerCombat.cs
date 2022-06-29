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

    // Object pool of tiles: set to 128 for efficiency.
    private readonly GameObject[] pBulletPool = new GameObject[64];
    private byte pBulletPointer = 0;

    private float cooldown;

    private void Start()
    {
        cooldown = 0.33f;

        // Initialize the player bullet pool to an offscreen position in the inactive state.
        for (int i = 0; i < pBulletPool.Length; i++)
        {
            pBulletPool[i] = Instantiate(playerBullet, new Vector3(-1, -1, -1), Quaternion.identity);
            pBulletPool[i].SetActive(false);
        }

        // Bullet template is no longer needed, so destroy the reference to the prefab.
        playerBullet = null;
    }

    private void Update()
    {
        cooldown -= Time.deltaTime;
    }

    public void GenerateProjectile(Vector3 playerPos)
    {
        // Select the next bullet within the bullet pool, reset it's position to the player's position and fire it (set it active).
        // Increment the pointer for the bullet pool to prepare for the next bullet to be fired.
        pBulletPool[pBulletPointer].transform.position = playerPos;
        pBulletPool[pBulletPointer].SetActive(true);
        pBulletPointer++;
        if(pBulletPointer >= pBulletPool.Length) { pBulletPointer = 0; }
        
        // 0.15s cooldown per shot (Between 6 and 7 shots per second).
        cooldown = 0.15f;
    }

    public float GetCooldown()
    {
        return cooldown;

    }
}
