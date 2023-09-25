using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMinigunProjectile : Projectile
{
    // Determines the deviation for each shot fired. Determined each time the bullet is enabled/fired.
    private float yDeviation;

    // Runs each time the object is activated.
    private void OnEnable()
    {
        // Determine the new deviation value for this bullet when it's fired. Changes every time the bullet is re-enabled.
        yDeviation = Random.Range(-0.1f, 0.1f);
    }

    protected override void Start()
    {
        // temp
        Angle = new Vector2(1, yDeviation);
        Angle.Normalize();
        Velocity = 3000;

        gameObject.GetComponent<Rigidbody2D>().velocity = Angle * Velocity * Time.fixedDeltaTime;
    }
}
