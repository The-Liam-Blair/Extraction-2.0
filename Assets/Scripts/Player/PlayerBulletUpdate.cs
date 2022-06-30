using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletUpdate : MonoBehaviour
{
    // Right edge of the camera.
    private const float cameraRight = 435f;

    // Determines the deviation for each shot fired. Determined once, each time the bullet is enabled.
    private float yDeviation;

    // Runs each time the object is activated.
    private void OnEnable()
    {
        // Determine the new deviation value for this bullet when it is first fired.
        yDeviation = Random.Range(-6f, 6f);
    }

    private void OnDisable()
    {
        transform.position = new Vector3(-1, -1, -1);
    }

    private void Update()
    {
        // Projectile rapidly travels rightwards with deviation and gravity influence. When it reaches the right screen edge, it will be set as inactive
        // as it is outside the game boundaries.
        if (transform.position.x > cameraRight) { gameObject.SetActive(false); }
        transform.Translate(100.0f * Time.deltaTime, yDeviation * Time.deltaTime, 0);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Bullet de-spawns when hitting terrain.
        if(other.transform.tag == "Terrain") { gameObject.SetActive(false); }
    }
}