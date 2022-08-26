using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Implementation of the mine enemy, a large floating mine that explodes when destroyed.
public class EnemyMine : Enemy
{
    // When object is active again, re-initialise it's properties.
    public override void OnEnable()
    {
        Health = 10;
        Speed = 25;
        ScoreOnDeath = 100;
        transform.position = new Vector3(CameraRight, 40, 0);
        isExploding = false;

        // Reset box collider back to it's initial, non-exploded value.
        GetComponent<BoxCollider2D>().size = new Vector2(0.55f, 0.55f);
    }

    public override void Update() { MoveLeft(); }

    // When exploding, adjust the box collider to match the explosion radius (approximately) and call the base function to play the animation.
    public override void Explode()
    {
        base.Explode();
        GetComponent<BoxCollider2D>().size = new Vector2(1, 1);
    }
}