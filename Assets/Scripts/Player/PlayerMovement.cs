using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    //todo checklist:
    // --> (later in development) controls can be changed in a settings menu, reflected in this movement script.


    // Width and Height of the player.
    private Vector2 playerSize;

    // Component that handles the firing and handling of the player's weapons.
    private PlayerCombat pCombat;

    private int health;

    // Player is given temporary immunity to enemy projectile and ship collision damage after receiving damage from either of
    // those sources. Colliding with terrain will always kill the player regardless.
    private float invulnerableTimer;

    // Camera dimensions, better for readability when calling sides of the camera than using uninformative values.
    private enum CameraPositions
    {
        Left = 38,
        Right = 377,
        Top = 121
    }

    
    private void Start()
    {
        // Calculate the player's size from it's connected box collider component.
        playerSize = transform.GetComponent<BoxCollider2D>().size;
        pCombat = GetComponent<PlayerCombat>();
        
        health = 3;
    }


    private void Update()
    {
        // Read input from input manager to calculate which key has been pressed, it's axis (horizontal/vertical) and scale it by a scalar and dt
        // to generate a movement vector.
        float hOffset = Input.GetAxisRaw("Horizontal") * 50 * Time.deltaTime;
        float vOffset = Input.GetAxisRaw("Vertical") * 50 * Time.deltaTime;
        transform.Translate(hOffset, vOffset, 0);


        // After movement has been calculated and applied, check if the player is within the screen boundaries. First checks the left and right of the screen
        // then checks the top of the screen (Bottom checking is omitted as terrain blocks passage to the bottom of the screen).

        if (transform.position.x < (float) CameraPositions.Left + playerSize.x)
        {
            transform.position = new Vector3((float) CameraPositions.Left + playerSize.x, transform.position.y, 0);
        }
        else if (transform.position.x > (float) CameraPositions.Right - playerSize.x)
        {
            transform.position = new Vector3((float) CameraPositions.Right - playerSize.x, transform.position.y, 0);
        }

        if (transform.position.y > (float) CameraPositions.Top - playerSize.y)
        {
            transform.position = new Vector3(transform.position.x, (float) CameraPositions.Top - playerSize.y, 0);
        }

        
        // Check if the player has pressed the primary fire key, and fire a bullet if so.

        if (Input.GetAxisRaw("PrimaryFire") > 0 && pCombat.Cooldown < 0.0f)
        {
            GetComponent<PlayerCombat>().GenerateProjectile(transform.position);
        }

        // Invulnerability decrements over time. Negative time means that it is inactive.
        invulnerableTimer -= Time.deltaTime;
    }


    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag.Equals("Terrain") || 
            other.transform.tag.Equals("Enemy")   ||
            other.transform.tag.Equals("EnemyProjectile"))
        {
            // Collided with terrain
            if (other.gameObject.tag.Equals("Terrain"))  { SceneManager.LoadScene("LoseGame"); }
            
            // Collided with enemy or enemy projectile, resist damage if invulnerable.
            else if (invulnerableTimer < 0f) /** && if tag == "Enemy" || tag == "EnemyProjectile" **/
            {
                // Player is immune to all damage (Apart from colliding with terrain) for 2 seconds.
                invulnerableTimer = 2f;
                health--;
                if (health <= 0)  { SceneManager.LoadScene("LoseGame"); }
            }
        }
    }
}
