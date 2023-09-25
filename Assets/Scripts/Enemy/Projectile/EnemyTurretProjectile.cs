using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurretProjectile : Projectile
{
    protected override void Start()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = Angle * Velocity;
    }
}
