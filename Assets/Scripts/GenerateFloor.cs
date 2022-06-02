using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = System.Random;

public class GenerateFloor : MonoBehaviour
{
    [SerializeField] private GameObject floorTile;

    // Object pool of tiles: set to 128 for efficiency.
    private GameObject[] tilePool = new GameObject[128];
    // Points to the next-selected tile in the tile pool, to allow reuse of the tile objects in the pool.
    private byte poolPointer = 0;

    // Delay between generating a new floor tile. Int value represents the number of frames in-between each tile generation.
    private int floorDrawDelay = 5;

    // Holds the current y position for generating floor tiles. Varies throughout game play to create uneven terrain.
    private int CurrentFloorYPos = -70;

    // Spawn width (x) position for all tiles and enemies, just outside of the camera's view.
    private const float SPAWNPOINT_WIDTH = 550.0f;

    void Start()
    {
        for (int i = 0; i < tilePool.Length; i++)
        {
            tilePool[i] = Instantiate(floorTile, new Vector3(-1, -1, 0), Quaternion.identity);
            tilePool[i].AddComponent<FloorUpdate>();
        }
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        if (floorDrawDelay <= 0)
        {
            DrawNewFloorTile();
            floorDrawDelay = 5;
        }

        floorDrawDelay--;
    }

    // Summon a new floor tile object from the object pool.
    private void DrawNewFloorTile()
    {
        CalculateNewYPos();
        tilePool[poolPointer].transform.position = new Vector3(SPAWNPOINT_WIDTH, CurrentFloorYPos, 0);
        poolPointer++;

        // Reset tile pool pointer if it exceeds the array size.
        if (poolPointer == tilePool.Length) { poolPointer = 0; }
    }

    private void CalculateNewYPos()
    {
        // Random number generator between 0 and 10. If the value returned is 0, the terrain will shift downwards, if the value is 10, the terrain will
        // shift upwards, otherwise (1-9) terrain stays at the same elevation. All in all, 20% chance per frame to adjust terrain height, with equal weighting
        // for it going up or down (10% each).
        switch(UnityEngine.Random.Range(0, 11))
        {
            case 0:
                if (CurrentFloorYPos == -100) break;
                CurrentFloorYPos -= 5;
                break;

            case 5:
                if (CurrentFloorYPos == -70) break;
                CurrentFloorYPos += 5;
                break;
        }
    }

}
