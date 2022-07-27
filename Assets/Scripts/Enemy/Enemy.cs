using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base enemy class implementation.
public abstract class Enemy : MonoBehaviour
{
    // Health of the enemy.
    protected int Health
    {
        get;
        set;
    }

    // Movement speed of the enemy.
    protected int Speed
    {
        get;
        set;
    }

    // Score awarded to the player when the enemy is defeated.
    public int ScoreOnDeath
    {
        get;
        set;
    }

    public float Cooldown
    {
        get;
        set;
    }

    // virtual methods ensure each enemy has an onEnable and update method.
    public virtual void OnEnable() {}
    public virtual void Update() {}

    // If the enemy collides with the player or is defeated, they will explode, potentially inflicting damage to anything nearby it.
    public virtual void Explode() {}
}
