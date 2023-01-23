using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Implementation of the mine enemy, a large floating mine that explodes when destroyed.
public class EnemyMine : Enemy
{

    private void Awake()
    {
        canHitTerrain = false;
        ScoreOnDeath = 100;
    }

    // When object is active again, re-initialise it's properties.
    protected override void OnEnable()
    {
        base.OnEnable();
        
        Health = 10;
        MaxHealth = Health;
        
        Speed = 25;
        
        // Mines spawn practically anywhere on the battlefield randomly. Minimum height is at 25, where it's unlikely to hit terrain and maximum height is 105, which is the upper
        // screen boundary.
        transform.position = new Vector3(CameraRight, Random.Range(0f, 85f) + 25f, 0);
        
        isExploding = false;
        isSmoking = false;
        isOnFire = false;

        // Reset box collider back to it's initial, non-exploded value.
        GetComponent<BoxCollider2D>().size = new Vector2(0.55f, 0.55f);
    }

    protected override void Update() { MoveLeft(); }

    // When exploding, adjust the box collider to match the explosion radius (approximately) and call the base function for general post-explosion events.
    protected override void Explode()
    {
        base.Explode();
        GetComponent<BoxCollider2D>().size = new Vector2(1, 1);
    }
}