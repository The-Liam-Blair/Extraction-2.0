using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurretProjectile : Projectile
{
    protected override void OnEnable()
    {
        base.OnEnable();
        gameObject.GetComponent<Rigidbody2D>().velocity = Angle * Velocity;
    }
}
