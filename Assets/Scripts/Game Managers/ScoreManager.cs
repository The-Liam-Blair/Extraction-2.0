using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour, IManager
{
    [SerializeField] private GameObject scoreObject;

    private GameObject[] ScoreOutput = new GameObject[8];
    private int pointer;

    private GameManager gameManager;
    public string ManagerName { get; set; }

    private void Start()
    {
        for (int i = 0; i < ScoreOutput.Length; i++)
        {
            ScoreOutput[i] = Instantiate(scoreObject, new Vector3(0, 0, 0), Quaternion.identity);
            ScoreOutput[i].transform.SetParent(GameObject.Find("_SCORES").transform.GetChild(0));
            ScoreOutput[i].SetActive(false);
        }

        pointer = 0;

        gameManager = GameManager.Instance;
    }

    /// <summary>
    /// Called by an enemy on death to display the score value as an indicator of the score reward the player received for defeating that enemy.
    /// Also adds that score to the player's total.
    /// </summary>
    /// <param name="score">Score value.</param>
    /// <param name="position">Enemy position (to place the score relatively next to).</param>
    public void DisplayScore(int score, Vector3 position)
    {
        ScoreOutput[pointer].SetActive(true);
        ScoreOutput[pointer].GetComponent<RectTransform>().position = position + new Vector3(0, 10f, 0);
        ScoreOutput[pointer].GetComponent<Text>().text = "+" + score + "!";

        pointer++;
        if (pointer >= ScoreOutput.Length) { pointer = 0; }


       AddScore(score);
    }

    private void AddScore(int score)
    {
        GameManager.Instance.AddScore(score);
    }
}
