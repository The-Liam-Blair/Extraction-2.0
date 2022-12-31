using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Implementation of the mine enemy, a large floating mine that explodes when destroyed.
public class EnemyTurret : Enemy
{
    // When object is active again, re-initialise it's properties.
    public override void OnEnable()
    {
        Health = 10;
        MaxHealth = Health;

        Speed = 66; // Speed matches terrain movement speed so it appears to be stationary.

        ScoreOnDeath = 100;

        // Turrets spawn sitting on top of the floor, whose height needs to be calculated at runtime.
        transform.position = new Vector3(CameraRight, GetYFloorPosition(), transform.position.z);
    }

    public override void Update()
    {
        MoveLeft();
    }

    // When exploding, adjust the box collider to match the explosion radius (approximately) and call the base function for general post-explosion events.
    public override void Explode()
    {
        base.Explode();
    }

    private float GetYFloorPosition()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, 50f, LayerMask.GetMask("Terrain")).point.y;
    }
}