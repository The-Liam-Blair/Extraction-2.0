using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorUpdate : MonoBehaviour
{
    void Update()
    {
        // If the object is in the inactive state (x position is -1 or x position is beyond the camera range), do not move the object.
        // Tile will be teleported to a valid position when it's needed, so will be 'active' and will move.
        if (transform.position.x < 0.0)  { return; }
        transform.Translate(-66f * Time.deltaTime, 0.0f, 0.0f);
    }
}
