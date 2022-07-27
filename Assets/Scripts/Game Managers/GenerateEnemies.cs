using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// For each enemy, init it with the enemy class for behaviours and yadda yadda.
public class GenerateEnemies : MonoBehaviour
{
    // List of enemies as game objects.
    [SerializeField] private GameObject[] EnemyPrefabs;

    // Holds the pool of enemies, all entirely stored on this 2D list.
    private List<List<GameObject>> Enemies = new List<List<GameObject>>();


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
            for (int j = 0; j < 5; j++)
            {
                Enemies[i].Add(Instantiate(EnemyPrefabs[i], new Vector3(-1, -1, 0), Quaternion.identity));
                Enemies[i][j].transform.SetParent(GameObject.Find("_ENEMYMANAGER").transform, true);
            }
        }
    }
}
