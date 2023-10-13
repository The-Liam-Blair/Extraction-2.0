using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreOnKill : MonoBehaviour
{
    [SerializeField] private GameObject scoreObject;

    private GameObject[] ScoreOutput = new GameObject[8];
    private int pointer;

    private GameManager gameManager;

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

    // TODO: Adjust game manager functions calls with a mediator pattern (see chat gpt for example).

    public void DisplayScore(int score, Vector3 position)
    {
        ScoreOutput[pointer].SetActive(true);
        ScoreOutput[pointer].GetComponent<RectTransform>().position = position + new Vector3(0, 10f, 0);
        ScoreOutput[pointer].GetComponent<Text>().text = "+" + score + "!";

        Debug.DrawLine(position, ScoreOutput[pointer].GetComponent<RectTransform>().position, Color.red, 3f);
        Debug.DrawLine(Vector3.zero, ScoreOutput[pointer].GetComponent<RectTransform>().position, Color.cyan, 3f);
        Debug.Log(position + "\n" + ScoreOutput[pointer].GetComponent<RectTransform>().position);

        pointer++;
        if (pointer >= ScoreOutput.Length) { pointer = 0; }


       AddScore(score);
    }

    private void AddScore(int score)
    {
        GameManager.Instance.AddScore(score);
    }
}
