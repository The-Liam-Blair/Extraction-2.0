using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Implementation of the mine enemy, a large floating mine that explodes when destroyed.
public class EnemyMine : Enemy
{

    // Start is called before the first frame update
    public override void OnEnable()
    {
        Health = 10;
        Speed = 50;
        ScoreOnDeath = 100;
        Cooldown = 4;
        transform.position = new Vector3(370, 40, 0);
    }

    // Update is called once per frame
    public override void Update()
    {
        transform.Translate(-Speed * Time.deltaTime, 0, 0);
    }

    void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }
}
