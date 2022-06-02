using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorUpdate : MonoBehaviour
{
    void Update()
    {
        transform.Translate(-1f, 0.0f, 0.0f);
        if (transform.position.x < 0) { Destroy(gameObject); }
    }
}
