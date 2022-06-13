using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletUpdate : MonoBehaviour
{
    private const float cameraRight = 550f;
    void Update()
    {
        // Projectile rapidly moves towards the right of the screen. When it reaches just outside the rightside of the camera, it will stop moving as it's already passed
        // the visual range for the player. It is then teleported to 0f on the x axis to prevent it interacting with newly-spawned enemies.
        if (transform.position.x > cameraRight) { gameObject.SetActive(false); }
        transform.Translate(100.0f * Time.deltaTime, 0, 0);
    }
}
