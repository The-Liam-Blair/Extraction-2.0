using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class PlayerMinigunProjectile : Projectile
{
    // Determines the deviation for each shot fired. Determined each time the bullet is enabled/fired.
    private float yDeviation;

    protected override void OnEnable()
    {
        base.OnEnable();

        // Determine the new deviation value for this bullet when it's fired. Changes every time the bullet is re-enabled.
        yDeviation = GameObject.Find("Player").GetComponent<PlayerCombat>().bulletDeviation;

        Angle = new Vector2(1, yDeviation);
        Angle.Normalize();
        Velocity = 125;
    }

    protected override void Update()
    {
        // Decrease Angle's Y value over time by a gravity force.
        Angle.y -= 0.0003f;

        // Move the bullet in the direction of the angle vector.
        transform.Translate(Angle * Velocity * Time.deltaTime);
    }
}
