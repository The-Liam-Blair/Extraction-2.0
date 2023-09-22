using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using Enum = Google.Protobuf.WellKnownTypes.Enum;

public abstract class Projectile : MonoBehaviour
{
    protected enum collisionType
    {
        Player,
        PlayerProjectile,
        Terrain,
        EnemyProjectile,
        Enemy
    };

    private Vector2 projectileAngle;


    private void OnBecameInvisible()
    {
        // When bullet is off-screen, disable it again so it cannot collide with objects off-screen and removes the need
        // to update the projectile for efficiency.
        gameObject.SetActive(false);
    }

    protected virtual void Start() {}

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        collisionType col = (collisionType) System.Enum.Parse(typeof(collisionType), other.transform.tag);

        if (System.Enum.TryParse(other.transform.tag, out col))
        {
            switch (col)
            {
                case collisionType.Enemy:
                    other.gameObject.GetComponent<Enemy>().ExplodeEnemy();
                    gameObject.SetActive(false);
                    break;

                case collisionType.EnemyProjectile:
                case collisionType.PlayerProjectile: 
                    other.gameObject.SetActive(false);
                    gameObject.SetActive(false);
                    break;

                case collisionType.Player:
                case collisionType.Terrain:
                    gameObject.SetActive(false);
                    break;
            }
        }
        else
        {
            Debug.Log("ERROR: UNKNOWN COLLISION TYPE DETECTED (ENUM CONVERSION FAILED)." + "\n OBJECT: " + gameObject.name + "\n SCRIPT: " + name);
        }
    }

    /// <summary>
    /// Sets the firing angle of the projectile on spawn.
    /// </summary>
    /// <param name="angle">Normalized angle of travel for this projectile</param>
    public void SetFiringAngle(Vector2 angle)
    {
        projectileAngle = angle;
    }
}