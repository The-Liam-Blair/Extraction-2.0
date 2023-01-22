using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

// Saves on using 'UnityEngine.Random.Range' every time the random function is used.
// Note: The first value is inclusive, the second value is exclusive: Random.Range(inclusiveMin, exclusiveMax);
using Random = UnityEngine.Random;

public class GenerateFloor : MonoBehaviour
{
    [SerializeField] private GameObject floorTile;

    // Object pool of tiles: set to 128 for efficiency.
    private readonly GameObject[] tilePool = new GameObject[128];
    // Points to the next-selected tile in the tile pool, to allow reuse of the tile objects in the pool. Byte used for maximum storage efficiency.
    private byte poolPointer = 0;

    // Delay between generating a new floor tile. Int value represents the number of frames in-between each tile generation.
    private int floorDrawDelay = 5;
    // Holds the current y position for generating floor tiles. Varies throughout game play to create uneven terrain.
    private int CurrentFloorYPos = -90;

    // Spawn width (x) position for all tiles and enemies, just outside of the camera's view.
    private const float SPAWNPOINT_WIDTH = 550.0f;
    // Height value boundaries for both standard terrain and hills.
    private const float TERRAIN_MAX_HEIGHT = -60;
    private const float TERRAIN_MIN_HEIGHT = -130;
    private const float HILLS_MAX_HEIGHT = -70;
    private const float HILLS_MIN_HEIGHT = -110;

    // Timer that checks if a hill is being drawn currently. Negative/0.0 indicates no hill is being drawn.
    // >4.0 indicates an upward slope is being drawn, while <4.0 indicates a downward slope is being drawn.
    private float HillTerrainTimer = 0.0f;
    // Timer that prevents the height of terrain from changing. Used normally to set a small flat piece of land to spawn an enemy without it clipping
    // into the ground.
    private float FlatTerrainTimer = 0.0f;

    private void Awake()
    {
        // For each tile in the tile pool:
        // - Calculate the new y position of the current tile. Used to generate initially uneven terrain rather than oddly flat terrain on level start.
        // - Instantiate the object (with pre-attached update script to make it move).
        // - Spread out each tile on the x axis, and randomly generate y axis using the standard random height generator to populate the game with 
        //   an already-built ground.
        for (var i = 0; i < tilePool.Length; i++)
        {
            CalculateNewYPos();
            tilePool[i] = Instantiate(floorTile, new Vector3(-1, -1, 0), Quaternion.identity);
            tilePool[i].transform.position = new Vector3(i*4.5f, CurrentFloorYPos, 0);
            tilePool[i].transform.SetParent(GameObject.Find("_FLOOR").transform, true);
            tilePool[i].name = "FloorTile [" + i + "]";
        }
    }

    // FixedUpdate used so that tile generation is not affected by frame rate.
    private void FixedUpdate()
    {
        // When the interval for drawing a new section of the floor has expired...
        if (floorDrawDelay <= 0)
        {
            // Roll 1/100 chance to draw a hill. If successful, add time to the hill draw timer.
            switch (Random.Range(0, 101))
            {
                case 0:
                    // Standard duration to draw a full hill is 8 seconds, +/- 2 (6-10 seconds) to make hills more varied and lopsided (As hill down slope draw
                    // time is always 4 seconds. For example, a 6 second hill will have 2 seconds to draw the uphill then 4 seconds to draw the downhill).
                    HillTerrainTimer = 8.0f + Random.Range(-2, 5);
                    break;
            }
            // Run the floor tile generation function and reset the floor interval timer.
            DrawNewFloorTile();
            floorDrawDelay = 5;
        }
        // Decrement the delay timer by 1 frame (So a new floor tile is drawn every 5 frames).
        floorDrawDelay--;

        // Decrement hill draw duration and flat terrain draw duration by dt.
        FlatTerrainTimer -= Time.deltaTime;
        HillTerrainTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Teleport the oldest terrain tile from the tile pool and reactivate it by teleporting it back to the right of the screen.
    /// <br>Also calls <see cref="CalculateNewYPos"/> to calculate a new y position to assign to this reactivated piece of terrain.</br>
    /// </summary>
    private void DrawNewFloorTile()
    {
        // Calculate the next height value for this piece of terrain.
        CalculateNewYPos();
        // Teleport the terrain last in the pool queue (Which would be in an inactive state) to the rightmost section of the screen and set it active
        // again to make it move again. Also increment the pointer that tracks the current position for the pool of terrain objects.
        tilePool[poolPointer].transform.position = new Vector3(SPAWNPOINT_WIDTH, CurrentFloorYPos, 0);
        tilePool[poolPointer].SetActive(true);
        poolPointer++;

        // Reset tile pool pointer if it exceeds the array size.
        if (poolPointer == tilePool.Length) { poolPointer = 0; }
    }


    /// <summary>
    /// Determine the next y position of the tile being reused and reactivated. <para></para>
    /// <br>If hill drawing is enabled (<paramref name="HillTerrainTimer"></paramref> > 0.0) then <see cref="GenerateHillsTerrain"/> is called.</br>
    /// <br>If flatlands drawing is enabled (<paramref name="FlatTerrainTimer"></paramref> > 0.0) then <see cref="GenerateFlatTerrain"/> is called.</br>
    /// <br>Otherwise, <see cref="GenerateRandomTerrain"/> is called.</br>
    /// </summary>
    private void CalculateNewYPos()
    {
        // If the hill terrain timer is positive...
        if (HillTerrainTimer > 0.0f)
        {
            // But the flat terrain timer is also positive...
            if (FlatTerrainTimer > 0.0f)
            {
                // Flat terrain timer overrides all, so do not adjust the terrain height.
                return;
            }
            // Only hill terrain timer is positive, so use the hill terrain generation function.
            GenerateHillsTerrain();
        }
        // Neither hill or flat terrain timer is positive, so draw random terrain.
        GenerateRandomTerrain();
    }

    /// <summary>
    /// Generate a completely random height value. Creates bumps and rough terrain.
    /// <para>Called when neither a hill or flat lands is being created. </para>
    /// </summary>
    private void GenerateRandomTerrain()
    {
        // Random number generator between 0 and 5. If the value returned is 0, the terrain will shift downwards, if the value is 5, the terrain will
        // shift upwards, otherwise (1-4) terrain stays at the same elevation. All in all, 40% chance per floor tile spawn to adjust terrain height, with equal weighting
        // for it going up or down (20% each).
        // Note that the actual height change is extremely small, so this will mostly create rough, small bumps in the ground.
        switch (Random.Range(0, 6))
        {
            case 0:
                // If the terrain is out of bounds, don't adjust the height.
                if (CurrentFloorYPos <= TERRAIN_MIN_HEIGHT) break;
                CurrentFloorYPos -= 1;
                break;

            case 5:
                // If the terrain is out of bounds, don't adjust the height.
                if (CurrentFloorYPos >= TERRAIN_MAX_HEIGHT) break;
                CurrentFloorYPos += 1;
                break;
        }
    }

    /// <summary>
    /// Generate a height value according to the <paramref name="HillTerrainTimer"/> value. Creates hills and slopes.<para></para>
    /// <br>If <paramref name="HillTerrainTimer"/> > 4.0: Generate upward slope.</br>
    /// <br>Else if <paramref name="HillTerrainTimer"/> > 0.0: Generate downward slope. (Between 0.0 and 4.0)</br>
    /// <br>Else (<paramref name="HillTerrainTimer"/> is negative): Hill is not being drawn.</br>
    /// </summary>
    private void GenerateHillsTerrain()
    {
        // These two lines sets the hill height value. The first line controls the downward slope of the hill (which runs after the hill has been going upwards
        // for 4 seconds, or halfway done) so it performs the downward slope to complete the hill. The second line builds the upward portion of the slope
        // with a check to ensure it does not go too high up.
        if (HillTerrainTimer <= 4.0f && CurrentFloorYPos >= HILLS_MIN_HEIGHT) { CurrentFloorYPos -= Random.Range(-1, 4); }
        else if (CurrentFloorYPos <= HILLS_MAX_HEIGHT) { CurrentFloorYPos += Random.Range(-1, 4); }

        // Calls the standard random terrain method. This can only be invoked if the current terrain height is out of bounds for generating a hill
        // (Hills have a boundary of -70 < y < -110, while overall terrain generation has a boundary of -60 < y < -130).
        else { GenerateRandomTerrain(); }
    }


    /// <summary>
    /// Generate purposely flat terrain. Overrides the creation of all other terrain, as it's normally used to prepare terrain for an enemy structure or weapon
    /// to sit on.
    /// </summary>
    /// <param name="duration">Time to keep the flat terrain for in seconds. Defaults to 0.5 if no input is provided.</param>
    public void GenerateFlatTerrain(float duration = 0.5f)
    {
        FlatTerrainTimer = duration;
    }
}