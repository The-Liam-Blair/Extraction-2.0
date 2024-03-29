using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorUpdate : MonoBehaviour
{
    private void Update()
    {
        // If the floor tile reaches the leftmost screen boundary, set it inactive as it's no longer required currently.
        // Once the tiles ahead of it have been exhausted and it's needed again to draw the next section of terrain, it will
        // be set to active again.
        if (transform.position.x < 0.0)  { gameObject.SetActive(false); }
        transform.Translate(-40f * Time.deltaTime, 0.0f, 0.0f);
    }
}
