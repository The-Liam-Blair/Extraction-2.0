using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Implementation of the mine enemy, a large floating mine that explodes when destroyed.
public class EnemyMine : Enemy
{
    // When object is active again, re-initialise it's properties.
    public override void OnEnable()
    {
        Health = 10;
        Speed = 50;
        ScoreOnDeath = 100;
        transform.position = new Vector3(CameraRight, 40, 0);
    }

    public override void Update()
    {
        MoveLeft();
    }

    // Example, temporary implementation of explode.
    public override void Explode()
    {
        gameObject.SetActive(false);
        Debug.Log("ouch");
    }
}
