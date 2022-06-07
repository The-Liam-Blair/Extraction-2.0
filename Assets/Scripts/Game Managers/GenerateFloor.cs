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

    // Timer that checks if a hill is being drawn currently. Negative/0.0 indicates no hill is being drawn.
    // >4.0 indicates an upward slope is being drawn, while <4.0 indicates a downward slope is being drawn.
    private float HillTerrainTimer = 0.0f;
    // Timer that prevents the height of terrain from changing. Used normally to set a small flat piece of land to spawn an enemy without it clipping
    // into the ground.
    private float FlatTerrainTimer = 0.0f;

    private void Start()
    {
        // For each tile in the tile pool:
        // - Calculate the new y position of the current tile. Used to generate initially uneven terrain rather than oddly flat terrain on level start.
        // - Instantiate the object,
        // - Attach the FloorUpdate script, which controls it's movement.
        // - Set it's initial position to the standard y position and it's x position moving up from the start to the end of the screen
        //   to populate the screen with terrain on game start.
        for (var i = 0; i < tilePool.Length; i++)
        {
            CalculateNewYPos();
            tilePool[i] = Instantiate(floorTile, new Vector3(-1, -1, 0), Quaternion.identity);
            tilePool[i].AddComponent<FloorUpdate>();
            tilePool[i].transform.position = new Vector3(i*4.5f, CurrentFloorYPos, 0);
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
            // Roll 1/100 chance to draw a hill.
            switch (Random.Range(0, 101))
            {
                case 0:
                    // Standard duration to draw a full hill is 8 seconds, +/- 2 (6-10 seconds) to make hills more varied and lopsided (As hill down slope draw
                    // time is always 4 seconds. For example, a 6 second hill will have 2 seconds to draw the uphill then 4 seconds to draw the downhill).
                    HillTerrainTimer = 8.0f + Random.Range(-2, 5);
                    break;
            }
            DrawNewFloorTile();
            floorDrawDelay = 5;
        }
        // Decrement floor draw delay by 1 frame (For more precision to generate floor tiles consistently rather than use dt).
        floorDrawDelay--;

        // Decrement hill draw duration and flat terrain draw duration by dt.
        FlatTerrainTimer -= Time.deltaTime;
        HillTerrainTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Teleport the oldest terrain tile from the tile pool and 'reactivate' it by teleporting it back to the right of the screen.
    /// <br>Also calls <see cref="CalculateNewYPos"/> to calculate a new y position to assign to this reactivated piece of terrain.</br>
    /// </summary>
    private void DrawNewFloorTile()
    {
        // Calculate the next height value for this piece of terrain.
        CalculateNewYPos();
        // Teleport the terrain last in the pool queue to just outside the screen boundary on the right side with the new calculated height value,
        // reactivating the terrain from the pool and making it move leftwards again.
        tilePool[poolPointer].transform.position = new Vector3(SPAWNPOINT_WIDTH, CurrentFloorYPos, 0);
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
        // Hills terrain: Draw an upward slope. After either the height level is reached or after 1 second, draw a downward slope.
        // Can also create terrain such as cliff edges if the height level is already at the upper height limit.
        if (HillTerrainTimer > 0.0f)
        {
            if (FlatTerrainTimer > 0.0f)
            {
                return;
            }
            GenerateHillsTerrain();
        }
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
        // Note that the actual height change is extremely small, so this will mostly create rough, small bumps in the ground. Hills terrain (above) produce more drastic
        // effects to the terrain.
        switch (Random.Range(0, 6))
        {
            case 0:
                if (CurrentFloorYPos <= -150) break;
                CurrentFloorYPos -= 1;
                break;

            case 5:
                if (CurrentFloorYPos >= -60) break;
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
        // These two lines build the hill. The first line controls the downward slope of the hill (which runs after the hill has been going upwards
        // for 4 seconds, or halfway done) so perform the downward slope to complete the hill. The second line builds the upward portion of the slope
        // with a check to ensure it does not go too high up.
        if (HillTerrainTimer <= 4.0f && CurrentFloorYPos >= -130) { CurrentFloorYPos -= Random.Range(-1, 4); }
        else if (CurrentFloorYPos <= -70) { CurrentFloorYPos += Random.Range(-1, 4); }

        // Calls the standard random terrain method. This can only be invoked if the current terrain height is out of bounds for generating a hill
        // (Hills have a boundary of -70 < y < -130, while terrain generation has a boundary of -60 < y < -150).
        else { GenerateRandomTerrain(); }
    }


    /// <summary>
    /// Generate purposely flat terrain. Overrides the creation of all other terrain, as it's normally used to prepare terrain for an enemy structure or weapon
    /// to sit on.
    /// </summary>
    public void GenerateFlatTerrain()
    {
        FlatTerrainTimer = 3.0f;
    }
}