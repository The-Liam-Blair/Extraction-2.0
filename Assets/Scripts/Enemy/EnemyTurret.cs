using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Implementation of the mine enemy, a large floating mine that explodes when destroyed.
public class EnemyTurret : Enemy
{
    // Gun is a separate object so it can be rotated/aimed independently.
    private GameObject gun;

    private bool shot;

    public void Awake()
    {
        canHitTerrain = true;
        gun = gameObject.transform.GetChild(0).gameObject;
    }

    public override void OnEnable()
    {
        Health = 10;
        MaxHealth = Health;

        Speed = 60; // Speed matches terrain movement speed so it appears to be stationary.

        ScoreOnDeath = 1000;

        transform.position = new Vector3(CameraRight + 50, 60.0f, transform.position.z);

        shot = false;

        // Turrets spawn sitting on top of the floor, whose height needs to be calculated at runtime.
        StartCoroutine(SetTurretYPos());
    }

    public override void Update()
    {
        MoveLeft();
        AimGun();

        test_Shoot();
    }

    public override void Explode()
    {
        base.Explode();
    }

    /// <summary>
    /// Using a raycast, get the intersection point of the terrain directly under a newly-spawned turret to find the position to place
    /// the turret such that it's ontop of the terrain without visible clipping.
    /// </summary>
    /// <returns>The y value used to correctly position the turret.</returns>
    private float GetYFloorPosition()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
            Vector3.down,
            500f,
            LayerMask.GetMask("Terrain"));

        Debug.DrawLine(transform.position, hit.point, Color.red, 3);

        return hit.point.y + 5f;
    }

    /// <summary>
    /// Set turret's y position after 2 seconds. Required to wait for terrain to spawn under the turret so that it can perform a raycast
    /// operation to determine the point it will be placed to look like it's directly sitting on the terrain.
    /// </summary>
    IEnumerator SetTurretYPos()
    {
        GameObject.Find("_GAMEMANAGER").GetComponent<GenerateFloor>().GenerateFlatTerrain(1f); // Ensure terrain under turret is flat to minimize clipping.
        yield return new WaitForSeconds(2);
        transform.position = new Vector2(transform.position.x,
            GetYFloorPosition());
        yield return null;

    }

    /// <summary>
    /// Rotate turret gun on the z axis to face and aim at the player
    /// </summary>
    private void AimGun()
    {
        Vector3 playerPos = player.transform.position;

        gun.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(playerPos.y - gun.transform.position.y, playerPos.x - gun.transform.position.x) * Mathf.Rad2Deg - 135f);
    }

    private void test_Shoot()
    {
        if (!shot && Vector3.Distance(player.transform.position, transform.position) < 40)
        {
            shot = true;
            GetComponent<Animator>().Play("Fire");
            Debug.Log("firin");
        }
    }
}