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

    // Update is called once per frame
    void Update()
    {
        float hOffset = Input.GetAxisRaw("Horizontal") * 50 * Time.deltaTime;
        float vOffset = Input.GetAxisRaw("Vertical") * 50 * Time.deltaTime;

        transform.Translate(hOffset, vOffset, 0);
    }
}
