using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

// TODO CHECKLIST:
// --> Terrain generated at start of the game fully rather than wait for terrain to fill up the screen -- Required.
// --> Interaction with enemy spawning system such that smoothed terrain is generated to place a ground-based enemy without it clipping into the ground -- Required.
// --> Colour -- Required.
// --> Random Chance to create a specific terrain type, such as a hill, mountain range, or flat lands -- Optional.
// --> Sloped terrain (If terrain moves up/down, change from a rigid square to triangular shape to create smoothness, chance based etc) -- Optional.

public class GenerateFloor : MonoBehaviour
{
    [SerializeField] private GameObject floorTile;

    // Object pool of tiles: set to 128 for efficiency.
    private readonly GameObject[] tilePool = new GameObject[128];
    // Points to the next-selected tile in the tile pool, to allow reuse of the tile objects in the pool.
    private byte poolPointer = 0;

    // Delay between generating a new floor tile. Int value represents the number of frames in-between each tile generation.
    private int floorDrawDelay = 5;

    // Holds the current y position for generating floor tiles. Varies throughout game play to create uneven terrain.
    private int CurrentFloorYPos = -70;

    // Spawn width (x) position for all tiles and enemies, just outside of the camera's view.
    private const float SPAWNPOINT_WIDTH = 550.0f;

    private float DrawFlatTerrainTimer = 2.0f;

    private void Start()
    {
        for (var i = 0; i < tilePool.Length; i++)
        {
            tilePool[i] = Instantiate(floorTile, new Vector3(-1, -1, 0), Quaternion.identity);
            tilePool[i].AddComponent<FloorUpdate>();
        }

        // Force FPS to be at 60FPS to (try) to keep dt values consistent, so that the floor tiles don't move out of position from a sudden change in
        // dt, resulting in gaps forming in the terrain.
        // todo: stick this in appropriate loading or game state script.
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        if (floorDrawDelay <= 0)
        {
            DrawNewFloorTile();
            floorDrawDelay = 5;
        }

        floorDrawDelay--;
        DrawFlatTerrainTimer -= Time.deltaTime;
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
        if (DrawFlatTerrainTimer < 0.0f)
        {
            // Random number generator between 0 and 10. If the value returned is 0, the terrain will shift downwards, if the value is 10, the terrain will
            // shift upwards, otherwise (1-9) terrain stays at the same elevation. All in all, 20% chance per floor tile spawn to adjust terrain height, with equal weighting
            // for it going up or down (10% each).
            switch (UnityEngine.Random.Range(0, 11))
            {
                case 0:
                    if (CurrentFloorYPos <= -100) break;
                    CurrentFloorYPos -= 5;
                    break;

                case 10:
                    if (CurrentFloorYPos >= -70) break;
                    CurrentFloorYPos += 5;
                    break;
            }
        }
    }

    // Called by enemy manager when a ground-based enemy is about to spawn to ensure terrain is smooth temporarily so the enemy does not
    // clip into the ground from potentially uneven terrain.
    public void DrawFlatTerrain()
    {
        DrawFlatTerrainTimer = 3.0f;
    }
}