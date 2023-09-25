using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using Enum = Google.Protobuf.WellKnownTypes.Enum;

public abstract class Projectile : MonoBehaviour
{

    /// <summary>
    /// Defines the tag of the other object colliding with this projectile, based on existing tags for game objects.
    /// Enum is used in <see cref="Projectile.OnCollisionEnter2D"/> to determine how to handle collisions between 2 objects with given tags.
    /// </summary>
    protected enum collisionType
    {
        Player,
        PlayerProjectile,
        Terrain,
        EnemyProjectile,
        Enemy
    };

    // (Normalized) fixed direction of travel.
    protected Vector2 Angle;

    // Speed scalar value of the projectile.
    protected int Velocity;

    protected void OnBecameInvisible()
    {
        // When bullet is off-screen, disable it again so it cannot collide with objects off-screen and removes the need
        // to update the projectile for efficiency.
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Each projectile requires a start function, primarily to give it an initial velocity.
    /// </summary>
    protected virtual void Start() {}


    /// <summary>
    /// Handle collision events between this projectile and the other entity.
    /// Being a projectile, the current projectile's tag can either be a player or enemy projectile. The other collider
    /// may be any tag from the <see cref="collisionType"/> set.
    /// </summary>
    /// <param name="other">Other collider</param>
    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        // Attempt to convert this projectile's and the other entity's tags into their respective collisionType enum objects. Throw an error if this operation fails.
        if (System.Enum.TryParse(transform.tag, out collisionType thisProjectile) && // Parse this projectile, send result to thisProjectile.
            System.Enum.TryParse(other.transform.tag, out collisionType otherEntity)) // Parse other collision object, send result to otherEntity.
        {
            switch (otherEntity)
            {
                // Projectile hit player
                case collisionType.Player:

                    // If an enemy's projectile hit the player, hurt the player. Player projectile to player collisions are ignored.
                    if(thisProjectile == collisionType.EnemyProjectile) { other.transform.GetComponent<PlayerMovement>().HurtPlayer(); }
                    gameObject.SetActive(false);
                    break;


                // Projectile hit a player's projectile
                case collisionType.PlayerProjectile:

                    // If an enemy's projectile hit the player's projectile, they destroy each other. Player projectiles do not intercept each other.
                    if (thisProjectile == collisionType.EnemyProjectile)
                    {
                        gameObject.SetActive(false);
                        other.gameObject.SetActive(false);
                    }
                    break;


                // Projectile hit an enemy
                case collisionType.Enemy:

                    // If the player's projectile hits an enemy, hurt the enemy.
                    // If an enemy's projectile hits an enemy, destroy the enemy. (Pass in a massive value through the hurt method).
                    if (thisProjectile == collisionType.PlayerProjectile) { other.transform.GetComponent<Enemy>().HurtEnemy(1); }
                    else if (thisProjectile == collisionType.EnemyProjectile) { other.transform.GetComponent<Enemy>().HurtEnemy(9999); }
                    gameObject.SetActive(false);
                    break;


                // Projectile hit an enemy's projectile
                case collisionType.EnemyProjectile:

                    // If a player's projectile or an enemy's projectile hits an enemy projectile, they destroy each other.
                    gameObject.SetActive(false);
                    other.gameObject.SetActive(false);
                    break;


                // Projectile hit terrain
                case collisionType.Terrain:

                    // Player and enemy projectiles are destroyed when impacting terrain.
                    gameObject.SetActive(false);
                    break;
            }
        }
        else
        {
            Debug.Log("ERROR: UNKNOWN COLLISION TYPE DETECTED (ENUM CONVERSION FAILED)." + "\n OBJECT: " + gameObject.name + "\n SCRIPT: " + name);

        }
    }
    public void Init(Vector2 angle, int velocity)
    {
        Angle = angle;
        Velocity = velocity;
    }
}