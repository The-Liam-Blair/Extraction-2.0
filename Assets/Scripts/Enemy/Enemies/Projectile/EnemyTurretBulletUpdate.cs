using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurretBulletUpdate : MonoBehaviour
{
    private Vector2 firingAngle;
    
    private void OnBecameInvisible()
    {
        // When bullet is off-screen, disable it as it's no longer useful.
        gameObject.SetActive(false);
    }

    private void Start()
    {
        // Firing angle is normalized: scaled to set its speed.
        gameObject.GetComponent<Rigidbody2D>().velocity = firingAngle * 3f;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Disable if the projectile hits a player or another enemy (Friendly fire).
        if (other.transform.tag == "Player" || other.transform.tag == "Terrain" || other.transform.tag == "EnemyProjectile") { gameObject.SetActive(false); }
        else if (other.transform.tag == "Enemy"){ other.gameObject.GetComponent<Enemy>().ExplodeEnemy(); gameObject.SetActive(false); }
    }

    /// <summary>
    /// Sets the firing angle of the projectile on spawn.
    /// </summary>
    /// <param name="angle">Normalized angle of travel for this projectile</param>
    public void SetFiringAngle(Vector2 angle)
    {
        firingAngle = angle;
    }
}
