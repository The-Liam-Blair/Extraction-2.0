using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    // Stores the angle of the gun's rotation from the start of the charge attack, used to calculate the firing angle of the projectile.
    private Vector2 AimAngle;

    // Like the inherited hurtSprite but for the gun as well, who also has it's own hurt sprite.
    private GameObject gunHurtSprite;

    private void Awake()
    {
        canHitTerrain = true;
        gun = gameObject.transform.GetChild(1).gameObject;
        gunHurtSprite = gun.transform.GetChild(0).gameObject;
        ScoreOnDeath = 1000;
        AimAngle = Vector2.zero;
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

        if (!isChargingShot) { AimGun(); } // Gun constantly aims at the player until it has expended it's shot.


        if (!hasShot && Vector3.Distance(player.transform.position, transform.position) < 100) { Fire(); } // Once player is close enough, fire the gun once. Does not fire again.
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

    // Stops call to base.OnExplodeEnd() which will disable the enemy object instance. Makes destroyed turret persist after animation as it has a crumbling animation
    // instead of a disappearing explosion animation.
    protected override void OnExplodeEnd() {}

    protected override void Attack()
    {
        isChargingShot = true;
        
        // Instruct the projectile manager to fire a new turret projectile.
        // Also passes the Vector object that represents the firing angle, which is normalized and directed at the player's location.
        AimAngle = (player.transform.position - gun.transform.position).normalized * 50f;

        var velocityScalar = 3;
        projectileLauncher.FireNewProjectile(gun.transform.position, 0, AimAngle, velocityScalar);

    }

    protected override void Hurt()
    {
        gunHurtSprite.SetActive(true); 
        Invoke("HurtTurretGunFinish", 0.05f);
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

        return hit.point.y + 5f; // +5 to account for the turret's height.
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

        // God knows whats happening here but it rotates to face the player so yay (+180 degree offset otherwise it points the wrong way).
        // Note to self: Make sprites point up (if rotatable) so that they don't need to be rotated 180 degrees to face the player.
        gun.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(playerPos.y - gun.transform.position.y, playerPos.x - gun.transform.position.x) * Mathf.Rad2Deg + 180f);
    }

    /// <summary>
    /// Fire the turret at the player. The turret will only attack once, in which case it will not fire again and will not rotate it's turret to face the player anymore.
    /// Note that the turret does not fire immediately: this calls the turret to begin charging an attack, which afterwards it will immediately fire.
    /// </summary>
    private void Fire()
    {
       hasShot = true;
       GetComponent<Animator>().Play("Fire");
    }
}