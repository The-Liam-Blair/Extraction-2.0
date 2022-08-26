using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using UnityEngine;

// For each enemy, init it with the enemy class for behaviours and yadda yadda.
public class GenerateEnemies : MonoBehaviour
{
    // List of enemies as game objects.
    [SerializeField] private GameObject[] EnemyPrefabs;

    // Holds the pool of enemies, all entirely stored on this 2D list.
    private List<List<GameObject>> Enemies = new List<List<GameObject>>();
    
    // Cooldown time for spawning each enemy type.
    private List<float> EnemySpawnCooldowns = new List<float>();
    // Starter cooldown value for each enemy after the enemy has just spawned.
    private List<float> EnemySpawnCooldownsReset = new List<float>();

    // Pointers for each enemy type to enable object pooling.
    private List<int> EnemySpawnPointers = new List<int>();

    // Player position to be used for spawning enemies aligned with the player on the y-axis.
    private Vector3 playerPos;

    private void Start()
    {
        // Lists are added in order of the enemies in the prefab list.
        foreach (GameObject enemy in EnemyPrefabs)
        {
            Enemies.Add(new List<GameObject>());
        }


        // For each enemy type...
        for (int i = 0; i < Enemies.Count; i++)
        {
            // Generate 5 game objects of that enemy type for the enemy pool.
            // Because of how objects are instantiated, the enemies are created and grouped in order of appearance in the enemy prefab
            // list.
            for (int j = 0; j < 5; j++)
            {
                Enemies[i].Add(Instantiate(EnemyPrefabs[i], new Vector3(-1, -1, 0), Quaternion.identity));
                Enemies[i][j].transform.SetParent(GameObject.Find("_ENEMYMANAGER").transform, true);
                Enemies[i][j].SetActive(false);
                Enemies[i][j].name = EnemyPrefabs[i].name + " [" + j + "]";
            }
        }

        // Really crap method of setting the spawning cooldown required for each enemy.
        //
        // Like the enemy pool, the cooldowns list is populated in order of the enemy prefabs list, which means it will
        // match the ordering of the enemy pool.
        for (int i = 0; i < Enemies.Count; i++)
        {
            EnemySpawnPointers.Add(0);

            switch (EnemyPrefabs[i].name)
            {
                // If the enemy was a mine...
                case "Mine":
                    // Add the cooldown interval to spawn this mine enemy to the list of enemy spawn cooldowns. (in this case, 4 seconds).
                    // SpawnCooldowns is the current cooldown timer, SpawnCooldownsReset is the 'reset' value which the spawn variable is set to after spawning an enemy.
                    EnemySpawnCooldowns.Add(4);
                    EnemySpawnCooldownsReset.Add(4);
                    break;

                // repeat for each enemy type...
            }
        }
    }

    private void Update()
    {
        // Update property that tracks the player's position.
        playerPos = GameObject.Find("Player").transform.position;

        // For each enemy type...
        for (int i = 0; i < Enemies.Count; i++)
        {
            // If the cooldown for that enemy is below 0, it can be spawned, so...
            if (EnemySpawnCooldowns[i] < 0)
            {
                // - Make that enemy active again so it will update.
                // - Increment the pointer for that enemy type.
                // - Reset the cooldown to the starter value.
                //
                // NOTE: Each enemy handles it's own position setting when set active. This is because
                // enemies will massively vary in how they spawn (For example, mines may align themselves with the
                // player, while turrets may be situated on the ground).
                Enemies[i][EnemySpawnPointers[i]].SetActive(true);
                
                // Increment the pool pointer and reset it's position if it overflows.
                EnemySpawnPointers[i]++;
                if (EnemySpawnPointers[i] > 4)
                {
                    EnemySpawnPointers[i] = 0;
                }
                
                // Reset spawn cooldown to the reset value.
                EnemySpawnCooldowns[i] = EnemySpawnCooldownsReset[i];
            }
        }

        // For each enemy type, decrement it's spawn timer by dt.
        for (int i = 0; i < EnemySpawnCooldowns.Count; i++)
        {
            EnemySpawnCooldowns[i] -= Time.deltaTime;
        }
    }
}
