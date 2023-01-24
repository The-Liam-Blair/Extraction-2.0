using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

// Implementation of the mine enemy, a large floating mine that explodes when destroyed.
public class EnemyTurret : Enemy
{
    // Gun is a separate object so it can be rotated/aimed independently.
    private GameObject gun;

    // Turret can shoot once per spawn.
    private bool hasShot;

    // Is the enemy currently charging and about to fire?
    private bool isChargingShot;

    // Like the inherited hurtSprite but for the gun as well, who also has it's own hurt sprite.
    private GameObject gunHurtSprite;


    //todo: FOR RED TINT TEXTURING:
    // If shader method fails: cheap way of emulating: Have child object follow parent, and have child object be a sprite with a red tint. Kept invisible, made briefly
    // visible when enemy is hurt, does not have collision etc. Used so that other animations will not be interrupted and the hurt effect will still persist.
    // all in all fuck shaders

    private void Awake()
    {
        canHitTerrain = true;
        gun = gameObject.transform.GetChild(1).gameObject;
        gunHurtSprite = gun.transform.GetChild(0).gameObject;
        ScoreOnDeath = 1000;

    }

    protected override void OnEnable()
    {
        Health = 10;
        MaxHealth = Health;

        Speed = 40; // Speed matches terrain movement speed so it appears to be stationary.

        transform.position = new Vector3(CameraRight + 50, 60.0f, transform.position.z);

        hasShot = false;

        isChargingShot = false;

        gunHurtSprite.SetActive(false);


        // Turrets spawn sitting on top of the floor, whose height needs to be calculated at runtime.
        StartCoroutine(SetTurretYPos());
    }

    protected override void Update()
    {
        MoveLeft();
        
        if(!isChargingShot) { AimGun(); }

        test_Shoot();
    }

    protected override void Explode()
    {
        isChargingShot = true; // Gun will not move or attempt to fire after turret destruction.
        hasShot = true;
        
        base.Explode();
    }

    protected override void OnChargeEnd()
    {
        isChargingShot = false;
        base.OnChargeEnd();
    }

    protected override void OnExplodeEnd()
    {
        // Stops call to base.OnExplodeEnd() which will disable the enemy object instance. Makes destroyed turret persist after animation as it has a crumbling animation
        // instead of a disappearing explosion animation.
    }

    protected override void Hurt()
    {
        gunHurtSprite.SetActive(true); Invoke("HurtTurretGunFinish", 0.05f);
        base.Hurt();
    }

    private void HurtTurretGunFinish()
    {
        gunHurtSprite.SetActive(false);
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
        
        return hit.point.y + 5f;
    }

    /// <summary>
    /// Set turret's y position after 2 seconds. Required to wait for terrain to spawn under the turret so that it can perform a raycast
    /// operation to determine the point it will be placed to look like it's directly sitting on the terrain.
    /// </summary>
    IEnumerator SetTurretYPos()
    {
        GameObject.Find("_GAMEMANAGER").GetComponent<GenerateFloor>().GenerateFlatTerrain(1f); // Ensure terrain under turret is flat to minimize clipping.
        yield return new WaitForSeconds(1); // Wait for flat terrain to spawn.
        transform.position = new Vector2(transform.position.x,
            GetYFloorPosition()); // Find correct position to place the turret by performing a downwards ray cast, which is the flat terrain below.
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
        if (!hasShot && Vector3.Distance(player.transform.position, transform.position) < 40)
        {
            hasShot = true;
            isChargingShot = true; // Prevents gun aiming after firing, indicates that the turret is now inactive after firing.
            GetComponent<Animator>().Play("Fire");
        }
    }
}