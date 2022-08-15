using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base enemy class implementation.
public abstract class Enemy : MonoBehaviour
{
    // Health of the enemy.
    public int Health
    {
        get;
        protected set; // Protected: Only the derived enemy classes can set their own health points. Getter is still public however.
    }

    // Movement speed multiplier of the enemy. (The actual, final movement speed of enemies is calculated as Speed * dt).
    protected int Speed
    {
        get;
        set;
    }

    // Score awarded to the player when the enemy is defeated.
    public int ScoreOnDeath
    {
        get;
        protected set; // Protected: Only the derived enemy classes can set their own score. Getter is still public however.
    }

    // Right-Most side of the screen + 70 units further to the right, to serve as the enemy spawn point on the x axis.
    protected int CameraRight = 430;


    // virtual methods ensure each enemy has an onEnable and update method.
    public virtual void OnEnable() {}
    
    public virtual void Update() {}
    
    // De-activate the object if it leaves the screen boundaries.
    protected void OnBecameInvisible()
    {
        // Don't deactivate objects on the rightmost side of the screen, which have just re-spawned.
        if (transform.position.x > 370) { return; }

        gameObject.SetActive(false);
    }

    // Handle collisions with other entities.
    protected void OnCollisionEnter2D(Collision2D other)
    {
        // If collided with a player's projectile, inflict projectile damage to that enemy.
        if (other.gameObject.tag == "PlayerProjectile")
        {
            Health -= GameObject.Find("Player").GetComponent<PlayerCombat>().Damage;

        }
        
        // After the (potential) bullet collision handling, destroy the enemy if it's health was set to or below 0 from being shot or if it
        // collided with the terrain or the player.
        if (Health <= 0 || other.gameObject.tag == "Player" || other.gameObject.tag == "Terrain")
        {
            Explode();
        }
    }


    // Will be called by all update functions: Makes objects move leftward with the terrain and other objects to give the illusion of
    // the player flying through the level.
    protected void MoveLeft() { transform.Translate(-Speed * Time.deltaTime, 0 ,0);}


    // Each enemy defines it's own explosion as each enemy's explosion is unique.
    /// (after-thought: make each enemy store an animation clip of their explosion and make the base explosion function implement this for all
    /// enemies).
    //
    // For each enemy's derived and implemented explosion:
    // - Get and play the explode animation from the animation manager.
    // - Adjust the hitbox/collider to fit the explosion radius. (Dynamically?)
    // - Get the frame-count of the animation and set the enemy to inactive once the explosion
    //   animation fully plays (Count each frame until it's equal to animation time or use some magic unity pre-defined function :) ).
    public virtual void Explode() {}
}
