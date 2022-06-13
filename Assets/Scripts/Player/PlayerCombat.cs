using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    private GameObject Bullet;

    private float cooldown;

    private void Start()
    {
        cooldown = 0.33f;
    }

    private void Update()
    {
        cooldown -= Time.deltaTime;
    }

    public void GenerateProjectile()
    {
        Instantiate(Bullet, transform.position, Quaternion.identity);
        cooldown = 0.33f;
    }

    public float GetCooldown()
    {
        return cooldown;
    }

}
