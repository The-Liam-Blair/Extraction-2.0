using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

// Base enemy class implementation.

/// TERMINOLOGY:
/// ------------
/// L2 - Level 2, denotes an 'upgraded' enemy that has improved stats and visually more striking.
/// Variant - Alternative enemy that is similar to a basic enemy with stark, unique difference in their attack. Can also be L2.
/// 
///
/// LIST OF ENEMIES:
/// ---------------------------------------
/// 
/// Legacy Enemies (Present in Extraction 1):
///
///   DUN :)
/// - Mine: Stationary, floating object that explodes on death or on collision with a miniature nuclear blast.
///     L2: No change to the mine, but 3 mines will spawn together in a random, tight group.
/// 
/// - Turret: Stationary ground enemy that fires a slow laser at the player.
///       L2: Increased fire rate, projectile velocity.
///
/// - Bomber Ship: Small ship on the top of the screen that drops slow bombs.
///            L2: Increased bomb drop rate and fall speed.
///
/// - Enemy Structure: Defenseless, vulnerable and critical target that can be optionally destroyed for a large sum of points. No L2 option present.
///
///
///
/// New Enemies:
/// 
/// - Rocket Ship: Flies in from the right of the screen, firing a large, wild, unpredictable rocket before retreating to the right of the screen again.
///            L2: Increased rocket velocity, ship health.
///
/// - Interceptor Fighter: Small, weak ship that stays on the far right of the screen, firing a small laser projectile every few seconds.
///                    L2: Laser fired more often and travels more quickly.
///
///
///
/// Enemy Variants:
///
/// 
/// - EMP Mine (Mine): Similar to the standard mine, but produces an EMP explosion than a nuclear explosion,
///             which temporarily disables weapons and shields within it's radius.
///   L2: Increased EMP explosion radius. Slowly drifts towards the player when the mine is close to the player.
///
/// - Anti-Air Turret (Turret): Like the turret, but fires one homing missile that loses it's homing capabilities after a few seconds.
///   L2: Rocket speed is increased.
///
/// - Planet Cracker (Bomber): Massive ship that flies on the top of the screen. After a lengthy, visual charging phase, it will fire a
///                            massive beam into the ground for a couple seconds.
///   L2: Beam width (And ship size) increased. Beam-ground interception point will now also spew debris randomly in all directions for the duration of the attack.
///
/// - Gravity Well (Structure): Ground-based structure that creates a gravitational field when approached by the player, weakly
///                             pulling in all space-faring objects and ships to it.
///   L2: Increased pull strength and duration.
///
/// - Mine Launcher (Rocket Ship): Medium sized, low hp ship that flies in from the right of the screen, firing single mines occasionally at the player.
///   L2: Increased ship health, and fires a spread of 3 mines instead of 1.
///
/// - Shield Battery (Interceptor Ship): Small ship with very low health and moves unpredictably. Gives nearby enemies 50% damage resistance.
///   L2: Increased ship health and damage resist aura radius.
///
///
///
/// Boss Enemies:
///
/// - Planetary Guardian (Level 1 Boss):
///   - Medium sized, high hp square ship that spins rapidly.
///   - Fires a massive, piercing laser every few seconds.
///   - Phase 2 (50% HP): Massive laser splits into 3 smaller lasers in a burst fashion after traveling for a few seconds.
///
/// - The Swarm (Level 1 Boss):
///   - Massive group of low hp, small, fast ships.
///   - Ships move as a collective group.
///   - Each ship has a low chance of firing a small laser projectile every few seconds. Chance of firing increases as more ships are destroyed.
///   - Once there are 5 or less drones left, they will always fire a projectile at least once a second, increasing further as more drones are destroyed.


/// EVENTS
///
///
/// 
/// Level 1 (On planet surface) - 1-2 Random events then boss encounter.
/// ---------------------------
/// - Cave system (Random Event):
///   - Player must navigate through a cave, which has a roof and floor terrain generation system.
///   - Only few mines and interceptor ships spawn during this phase.
///
/// - Enemy Fleet Encounter (Random Event):
///   - Mines and ground-based enemies stop spawning.
///   - All enemy ships can spawn (Bombers, Rocket Ships and Interceptor Fighters).
///   - Low chance for variants to spawn.
///   - Normal enemies are much more likely to spawn as L2, and variants have a low chance to spawn as L2.
///
/// - City Fly-By (Random Event):
///   - Mines no longer spawn.
///   - Only interceptor and rocket ships can spawn as fliers.
///   - Enemy structures spawn abundantly with some Turrets.
///   - Very low chance of Turrets/Enemy structures spawning as variants.
///   - Low chance for interceptors, rocket ships and variants to spawn as L2.
///
/// - Abandoned Minefield (Random Event):
///   - No ground-based structures will spawn.
///   - Massive amounts of mines will spawn with a very high chance to spawn as L2 and medium chance for variants.
///
///
/// Level 2 (In Space) - 1 Random events then boss encounter.
/// --------------------
/// - Enemy Interceptor Squadron (Random Event):
///   - Mostly interceptors with some rocket ships.
///   - Very high chance for enemies to spawn as L2, medium chance for variants. Variants have a low chance to spawn as L2.
///
/// - Enemy Ambush (Random Event):
///   - Large amounts of mines, interceptors, rocket ships and bomber ships.
///   - Low chance of variants to spawn.
///   - All enemies spawn as L2.
///
/// - Enemy Dreadfleet (Rare Random Event):
///   - Rocket ships, interceptors and bombers.
///   - 50% variant chance (Extremely high).
///   - Every and all enemies spawn as L2.
///   - Increased enemy spawn rate.
public abstract class Enemy : MonoBehaviour
{
    /// <summary>
    /// Health of the enemy.
    /// </summary>
    public int Health
    {
        get;
        protected set; // Protected: Only the derived enemy classes can set their own health points. Getter is still public however.
    }

    public int MaxHealth
    {
        get;
        protected set;
    }

    
    /// <summary>
    /// Movement speed multiplier of the enemy. (The actual, final movement speed of enemies is calculated as Speed * dt).
    /// </summary>
    protected int Speed { get; set; }

    
    /// <summary>
    /// Score awarded to the player when the enemy is defeated.
    /// </summary>
    public int ScoreOnDeath
    {
        get;
        protected set;
    }

    
    /// <summary>
    /// Is the enemy exploding (And so the relevant animations are playing and triggers have been disabled)?
    /// </summary>
    public bool isExploding
    {
        get;
        protected set;
    }

    /// <summary>
    /// Can this enemy touch terrain without being destroyed?
    /// </summary>
    protected bool canHitTerrain
    {
        get;
        set;
    } = false;

    /// <summary>
    /// Game object which generates smoke at a location.
    /// </summary>
    protected virtual GameObject SmokeParticle
    {
        get;
        set;
    }

    //todo: either incorporate or get rid of particle system handlers below
    
    /// <summary>
    /// Bool checks if enemy is damaged enough that it's visibly smoking.
    /// </summary>
    protected bool isSmoking
    {
        get;
        set;
    }

    /// <summary>
    /// Game object which generates flames at a location.
    /// </summary>
    protected virtual GameObject FlameParticle
    {
        get;
        set;
    }

    /// <summary>
    /// Bool checks if enemy is severely damaged enough that it's on fire.
    /// </summary>
    protected bool isOnFire
    {
        get;
        set;
    }


    /// <summary>
    /// Spawn point of the enemy.
    /// Spawn point is slightly ahead of terrain spawn point so that runtime terrain distance calculations can be completed with already-spawned terrain.
    /// </summary>
    protected const int CameraRight = 500;

    protected GameObject player;

    public void Start()
    {
        // UNUSED PARTICLE SYSTEM INSTANTIATION: KEPT FOR POTENTIAL FUTURE REFERENCE.
        // Load smoke and flame particle (game objects) from resources folder within the Particle prefab.
        //SmokeParticle = Resources.Load("P_Smoke") as GameObject;
        //FlameParticle = Resources.Load("P_Fire") as GameObject;

        player = GameObject.FindGameObjectWithTag("Player"); // Faster to search via tag than search via object names due to reduced search space.
    }

    // virtual methods ensure each enemy has an onEnable and update method.
    public virtual void OnEnable() {}

    
    public virtual void Update() {}

    
    // De-activate the object if it leaves the screen boundaries.
    protected void OnBecameInvisible()
    {
        // Don't deactivate objects on the rightmost side of the screen, which have just re-spawned.
        if (transform.position.x > 350 | transform.position.x > -50) { return; }
        
        gameObject.SetActive(false);
    }

    // Handle collisions with other entities.
    protected void OnCollisionEnter2D(Collision2D other)
    {
        // Do not perform collision checking if enemy is already exploding -> already dead.
        if (isExploding) { return; }
        
        // If collided with a player's projectile, inflict projectile damage to that enemy. If this attack was fatal, the
        // enemy explodes. Otherwise, it is only hurt.
        if (other.gameObject.tag == "PlayerProjectile")
        {
            Health -= GameObject.Find("Player").GetComponent<PlayerCombat>().Damage;

            if (Health <= 0) { Explode(); } // todo weird bug: After enemy explodes and respawns, explosion first frame animation plays alongside/sometimes replacing hurt animation.
            else { Hurt(); }
        }

        // If collided with the player itself or terrain, the enemy explodes regardless of it's hp.
        // If collided with terrain, it will only explode if the enemy is unable to hit terrain safely; buildings etc do not explode on terrain contact.
        else if (other.gameObject.tag == "Player" || (other.gameObject.tag == "Terrain" && !canHitTerrain))
        {
            Health = 0;
        }
    }


    /// <summary>
    /// Will be called by all update functions: Makes objects move leftward with the terrain and other objects to give the illusion of
    /// the player flying through the level.
    /// </summary>
    protected void MoveLeft() { transform.Translate(-Speed * Time.deltaTime, 0, 0); }


    /// <summary>
    /// Enemy is dead when this is called, so play the Explode animation from the enemy's animator component.
    /// OnExplodeEnd() is automatically called when the explode animation ends so that the enemy will be set to inactive as soon 
    /// as the exploding animation completes.
    ///
    /// Each enemy has a derived Explode() function but will always call this base function to play the exploding animation. The
    /// derived animations are to modify components during explosions, such as the collision.
    /// </summary>
    public virtual void Explode() { 
        isExploding = true;
        GetComponent<Animator>().Play("Explode");
    }

    
    /// <summary>
    /// Called by the animator component when an enemy's explosion animation completes: disable the enemy.
    /// </summary>
    public void OnExplodeEnd() { gameObject.SetActive(false); }

    /// <summary>
    /// Called by the animator component when an enemy's attack (which requires charging) is finished charging: Execute the attack.
    /// </summary>
    public void OnChargeEnd() { Attack();}

    /// <summary>
    /// Called by the animator component when the enemy will execute an attack (May or may not require charging).
    /// </summary>
    public void Attack() { Debug.Log("kek"); }

    /// <summary>
    /// Enemy has taken non-fatal damage when this is called, so an animation plays that very quickly places a red tint on the 
    /// enemy sprite to signify that it has taken damage.
    /// </summary>
    public void Hurt() { GetComponent<Animator>().Play("Hurt"); }



    // UNUSED PARTICLE CALLS: KEPT FOR (POTENTIAL) FUTURE REFERENCE.
    
    /// <summary>
    /// Make an enemy's ship start to smoke by instantiating a smoke particle game object and attach to enemy as a child.
    /// </summary>
    public void EnemyShipSmoking()
    {
        Bounds enemyBounds = GetComponent<Collider2D>().bounds;

        Vector3 smokePos = new Vector3(Random.Range(enemyBounds.min.x + 0.5f, enemyBounds.max.x),  // Set position of game object to be 
            Random.Range(enemyBounds.min.y, enemyBounds.max.y),                                    // Within the centre (50% inwards) of the bounds.
            -2); // -2 z position to move smoke particle in front the enemy.
        

        GameObject smoke = Instantiate(SmokeParticle,
            smokePos,
            Quaternion.identity, 
            transform);

        smoke.GetComponent<ParticleSystem>().Play();
    }

    /// <summary>
    /// Make an enemy's ship be set on fire by instantiating a flame particle game object and attach to enemy as a child.
    /// </summary>
    public void EnemyShipOnFire()
    {
        Bounds enemyBounds = GetComponent<Collider2D>().bounds;

        Vector3 firePos = new Vector3(Random.Range(enemyBounds.min.x + 0.5f, enemyBounds.max.x - 0.5f),
            Random.Range(enemyBounds.min.y + 0.5f, enemyBounds.max.y - 0.5f),
            -2);
        GameObject flame = Instantiate(FlameParticle,
            firePos,
            Quaternion.identity,
            transform);

        flame.GetComponent<ParticleSystem>().Play();
    }

}
