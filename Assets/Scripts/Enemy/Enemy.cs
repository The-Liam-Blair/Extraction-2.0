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
        protected set;
    }

    // Movement speed multiplier of the enemy. (Speed is calculated as Speed * dt).
    protected int Speed
    {
        get;
        set;
    }

    // Score awarded to the player when the enemy is defeated.
    public int ScoreOnDeath
    {
        get;
        protected set; // Protected: Only the derived enemy classes can set their on score. Getter is still public however.
    }

    // Right-Most side of the screen + 70 units further to the right, to serve as the enemy spawn point on the x axis.
    protected int CameraRight = 430;


    // virtual methods ensure each enemy has an onEnable and update method.
    public virtual void OnEnable() {}
    
    public virtual void Update() {}
    
    // De-activate the object if it leaves the screen boundaries
    void OnBecameInvisible()
    {
        // Don't deactivate objects on the rightmost side of the screen, which have just respawned.
        if (transform.position.x > 370) { return; }

        gameObject.SetActive(false);
    }


    // Will be called by all update functions: Makes objects move leftward with the terrain and other objects to give the illusion of
    // the player flying through the level.
    protected void MoveLeft() { transform.Translate(-Speed * Time.deltaTime, 0 ,0);}

    // If the enemy collides with the player or is defeated, they will explode, potentially inflicting damage to anything nearby it.
    public virtual void Explode() {}
}
