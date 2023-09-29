using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurretProjectile : Projectile
{
    protected override void Update()
    {
        // Move the bullet in the direction of the angle vector.
        transform.Translate(Angle * Velocity * Time.deltaTime);
    }
}
