using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class manages the bullets for all enemies that fire projectiles.
/// Each projectile type has it's own pool and pointer.
/// </summary>
public class EnemyProjectileManager : MonoBehaviour, IManager
{
    // Turret projectile pool
    [SerializeField] private GameObject turretBullet;
    private GameObject[] tBulletPool = new GameObject[32];
    private byte tBulletPointer = 0;

    public string ManagerName { get; set; }

    void Start()
    {
        // Initialise the pool per projectile type.
        // todo: maybe homogenize into a large singular pool that can be used by all enemies with projectile attacks.

        // Enemy turret bullet pool.
        for (int i = 0; i < tBulletPool.Length; i++)
        {
            tBulletPool[i] = Instantiate(turretBullet, new Vector3(-1, -1, -1), Quaternion.identity); // Spawn the bullet offscreen.
            tBulletPool[i].SetActive(false); // Disable it for now.
            tBulletPool[i].transform.SetParent(GameObject.Find("_ENEMYPROJECTILE").transform, true); // Set parent to an empty game object for organization.
            tBulletPool[i].name = turretBullet.name + " [" + i + "]"; // Set the name of the bullet by it's index for organization.
        }
    }

    /// <summary>
    /// Instructs the enemy projectile manager to launch a new projectile, called by any enemy with projectile-based attacks.
    /// </summary>
    /// <param name="enemyPos">Position of the enemy (To set the projectile spawn location).</param>
    /// <param name="projectileType">Identifier that's used to determine which pool to use, and so which projectile to fire.</param>
    /// <param name="direction">Direction the projectile will move.</param>
    /// <param name="velocity">Speed of the projectile.</param>
    public void FireNewProjectile(Vector3 enemyPos, int projectileType, Vector2 direction, int velocity)
    {
        switch (projectileType)
        {
            // Turret projectile
            case 0:
                tBulletPool[tBulletPointer].transform.position = enemyPos + (Vector3) direction * 10f; // Spawn projectile at enemy position (plus direction vector to align it with turret barrel).
                tBulletPool[tBulletPointer].GetComponent<EnemyTurretProjectile>().Init(direction, velocity); // Set this projectile's direction and velocity.
                tBulletPool[tBulletPointer].SetActive(true); // Set the projectile to be active (To enable it and to call it's OnEnable function).

                // Handle the pool pointer.
                tBulletPointer++;
                if (tBulletPointer >= tBulletPool.Length) { tBulletPointer = 0; }

                break;
        }
    }
}
