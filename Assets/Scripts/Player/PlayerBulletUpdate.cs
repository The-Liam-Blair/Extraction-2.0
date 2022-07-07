using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletUpdate : MonoBehaviour
{
    // Determines the deviation for each shot fired. Determined each time the bullet is enabled/fired.
    private float yDeviation;

    // Runs each time the object is activated.
    private void OnEnable()
    {
        // Determine the new deviation value for this bullet when it's fired. Changes every time the bullet is re-enabled.
        yDeviation = Random.Range(-6f, 6f);
    }

    private void OnBecameInvisible()
    {
        // When bullet is off-screen, disable it again so it cannot collide with objects off-screen and removes the need
        // to update the projectile for efficiency.
        gameObject.SetActive(false);
    }

    private void Update()
    {
        // Projectile rapidly travels rightwards with a set deviation. Also influenced by gravity from the rigidbody component.
        transform.Translate(100.0f * Time.deltaTime, yDeviation * Time.deltaTime, 0);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Disable bullet on colliding with terrain.
        if(other.transform.tag == "Terrain") { gameObject.SetActive(false); }
    }
}