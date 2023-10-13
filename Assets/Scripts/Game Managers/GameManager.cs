using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton implementation
    private static GameManager _instance;

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
    }

    public void AddScore(int score)
    {
        totalScore += score;
    }

}
