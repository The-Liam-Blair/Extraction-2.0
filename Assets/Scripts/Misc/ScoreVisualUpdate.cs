using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ScoreVisualUpdate : MonoBehaviour
{
    public void ScoreVisualEnd()
    {
        gameObject.SetActive(false);
    }
}
