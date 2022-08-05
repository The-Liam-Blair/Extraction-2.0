using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Implementation of the mine enemy, a large floating mine that explodes when destroyed.
public class EnemyMine : Enemy
{
    // Right-Most side of the screen + 70 units further to the right so that they don't visually teleport in, but travel from the right of the screen fully.
    private int CameraRight = 430;

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
}
