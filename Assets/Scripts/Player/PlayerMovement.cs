using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //todo checklist:
    // --> (later in development) controls can be changed in a settings menu, reflected in this movement script.


    // Width and Height of the player.
    private Vector2 playerSize;

    // Component that handles the firing and handling of the player's weapons.
    private PlayerCombat pCombat;

    // Camera dimensions, better for readability when calling sides of the camera than using uninformative values.
    private enum CameraPositions
    {
        Left = 15,
        Right = 538,
        Top = 95
    }

    
    private void Start()
    {
        // Calculate the player's size from it's connected box collider component.
        playerSize = transform.GetComponent<BoxCollider2D>().size;
        pCombat = GetComponent<PlayerCombat>();
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

        if (Input.GetAxisRaw("PrimaryFire") > 0 && pCombat.GetCooldown() < 0.0f)
        {
            GetComponent<PlayerCombat>().GenerateProjectile(transform.position);
        }
    }


    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag.Equals("Terrain") || 
            other.transform.tag.Equals("Enemy")   ||
            other.transform.tag.Equals("EnemyProjectile"))
        {
            // Collided with terrain
            if (other.gameObject.tag.Equals("Terrain"))
            {
                // die
            }
            
            // Collided with an enemy or one of it's projectiles.
            else
            {
                {
                    // lose 1 hp
                }
            }
        }
    }
}
