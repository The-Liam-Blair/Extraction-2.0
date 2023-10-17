using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton implementation
    private static GameManager _instance;

    [NotNull] private List<IManager> managers = new List<IManager>();

    public static GameManager Instance
    {
        get
        {
            if(_instance == null) { _instance = FindObjectOfType(typeof(GameManager)) as GameManager; }
            return _instance;
        }

        private set { _instance = value; }
    }

    private int totalScore;

    private void Awake()
    {
        totalScore = 0;

        // Add all managers to the list.
        //
        // This is a temporary solution to collecting manager references: Later on, a better way would be to
        // instantiate the relevant managers as per scene/level requirements instead of having all managers active constantly.

        // Enemy Projectile Manager: Handles spawning and initialization of enemy projectiles.
        var projMan = GameObject.Find("_GAMEMANAGER").GetComponent<EnemyProjectileManager>();
        projMan.ManagerName = "EnemyProjectileManager";
        managers.Add(projMan);

        // Score Manager: Handles retrieving score values for enemies and displaying them on death, and accumulating the player's total score.
        var scoreMan = GameObject.Find("_GAMEMANAGER").GetComponent<ScoreManager>();
        scoreMan.ManagerName = "ScoreManager";
        managers.Add(scoreMan);

        // Floor Manager: Handles the constant generation of floor tiles.
        var FloorMan = GameObject.Find("_GAMEMANAGER").GetComponent<GenerateFloor>();
        FloorMan.ManagerName = "GenerateFloor";
        managers.Add(FloorMan);
    }

    public void AddScore(int score)
    {
        totalScore += score;
        Debug.Log($"Score: {totalScore}");
    }

    /// <summary>
    /// Mediator pattern: Centralised game manager handles method requests to other specialised managers.
    /// </summary>
    /// <typeparam name="T">Return type of the requested method, if applicable.</typeparam>
    /// <param name="managerName">Name of the manager to be accessed.</param>
    /// <param name="methodName">Name of the method to be requested.</param>
    /// <param name="parameters">List of parameters to pass to the method, if applicable.</param>
    /// <returns>Return value of the requested method, if it has one, otherwise it doesn't return anything.</returns>
    public T InvokeManagerMethod<T>(string managerName, string methodName, params object[] parameters)
    {
        // Find the manager by name.
        IManager manager = managers.Find(m => m.ManagerName == managerName);

        if (manager != null)
        {
            // Find the method by name, for the related manager.
            System.Reflection.MethodInfo method = manager.GetType().GetMethod(methodName); // Scary reflection stuff! (Also really fucking inefficient but it'll do for now).

            if (method != null)
            {
                // Call the requested method on the selected manager, and return the method's return value is there is one.
                var result = method.Invoke(manager, parameters);

                if (result != null)
                {
                    return (T)result;
                }
            }

            else
            {
                Debug.LogError($"ERROR: Method '{methodName}' not found on manager '{managerName}'.");
            }
        }

        else
        {
            Debug.LogError($"ERROR: Manager '{managerName}' not found.");
        }

        return default;
    }

}

public interface IManager
{
    public string ManagerName 
    { 
        get;
        set;
    }
}