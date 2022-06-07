using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //todo checklist:
    // --> Basic player input for movement.
    // --> Referencing (soon to be made) attack script when the attack key is pressed.
    // --> Handle collisions with terrain, enemies, and enemy projectiles. 
    // --> (later in development) controls can be changed in a settings menu, reflected in this movement script.

    private Vector2 playerSize;


    void Start()
    {
        playerSize = transform.GetComponent<BoxCollider2D>().size;
    }
    void Update()
    {
        float hOffset = Input.GetAxisRaw("Horizontal") * 50 * Time.deltaTime;
        float vOffset = Input.GetAxisRaw("Vertical") * 50 * Time.deltaTime;
        transform.Translate(hOffset, vOffset, 0);

        if(transform.position.x < 15.0 + playerSize.x) { transform.position = new Vector3(15 + playerSize.x, transform.position.y, 0); }
        else if(transform.position.x > 545 - playerSize.x) {transform.position = new Vector3(545 - playerSize.x, transform.position.y, 0); }

        if(transform.position.y > 95 - playerSize.y) { transform.position = new Vector3(transform.position.x, 95 - playerSize.y, 0);}
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
