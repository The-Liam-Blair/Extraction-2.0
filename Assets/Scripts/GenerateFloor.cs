using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class GenerateFloor : MonoBehaviour
{
    [SerializeField] private GameObject floorTile;

    // Object pooling: 128 (4bit) sufficient for floor tile generation.

    // Delay (By frame count) between generating a new floor tile
    private int floorDrawDelay = 10;

    // X position of the right of the camera.
    private const float CAMERA_WIDTH = 550.0f;

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        if (floorDrawDelay <= 0)
        {
            DrawNewFloorTile();
            floorDrawDelay = 10;
        }

        floorDrawDelay--;
    }

    // Create a new floor tile object and attach it's appropriate update component, which handles it's movement and collisions.
    private void DrawNewFloorTile()
    {
       var newFloorTile =  Instantiate(floorTile, new Vector3(CAMERA_WIDTH, 0, 0), Quaternion.identity);
       newFloorTile.AddComponent<FloorUpdate>();
    }

}
