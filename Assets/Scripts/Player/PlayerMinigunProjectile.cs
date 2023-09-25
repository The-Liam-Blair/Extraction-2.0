using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMinigunProjectile : Projectile
{
    // Determines the deviation for each shot fired. Determined each time the bullet is enabled/fired.
    private float yDeviation;

    protected override void OnEnable()
    {
        base.OnEnable();

        // Determine the new deviation value for this bullet when it's fired. Changes every time the bullet is re-enabled.
        yDeviation = Random.Range(-0.09f, 0.09f);

        // temp
        Angle = new Vector2(1, yDeviation);
        Angle.Normalize();
        Velocity = 75;

        gameObject.GetComponent<Rigidbody2D>().velocity = Angle * Velocity;
    }
}
