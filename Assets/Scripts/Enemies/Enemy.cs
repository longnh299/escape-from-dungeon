using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Mirror;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(DealContactDamage))]
[RequireComponent(typeof(DestroyedEvent))]
[RequireComponent(typeof(Destroyed))]
[RequireComponent(typeof(EnemyWeaponAI))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(ReloadWeapon))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(EnemyMovementAI))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(AnimateEnemy))]
[RequireComponent(typeof(MaterializeEffect))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
#endregion REQUIRE COMPONENTS

[DisallowMultipleComponent]
public class Enemy : NetworkBehaviour
{
    [HideInInspector] [SyncVar] public int index = 0;
    [HideInInspector] public EnemyDetailsSO enemyDetails;
    private HealthEvent healthEvent;
    private Health health;
    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public FireWeaponEvent fireWeaponEvent;
    private FireWeapon fireWeapon;
    private SetActiveWeaponEvent setActiveWeaponEvent;
    private EnemyMovementAI enemyMovementAI;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public IdleEvent idleEvent;
    private MaterializeEffect materializeEffect;
    private CircleCollider2D circleCollider2D;
    private PolygonCollider2D polygonCollider2D;
    [HideInInspector] public SpriteRenderer[] spriteRendererArray;
    [HideInInspector] public Animator animator;
    public Transform player;
    bool isDied = false;

    [ClientCallback]
    private void Awake()
    {
        // Load components
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        fireWeapon = GetComponent<FireWeapon>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        enemyMovementAI = GetComponent<EnemyMovementAI>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        idleEvent = GetComponent<IdleEvent>();
        materializeEffect = GetComponent<MaterializeEffect>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        spriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    [ClientCallback]
    private void OnEnable()
    {
        //subscribe to health event
        healthEvent.OnHealthChanged += HealthEvent_OnHealthLost;
    }

    [ClientCallback]
    private void OnDisable()
    {
        //subscribe to health event
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthLost;
    }

    //Fix
    [Server]
    public void ChangeIndex(int i)
    {
        index = i;
    }

    public void Die()
    {
        Cmd_Die(EnemySpawner.Instance.currentRoom.id);
    }

    [Command(requiresAuthority = false)]
    public void Cmd_Die(string id)
    {
        if(isDied) return;
        else isDied = true;

        Destroy(gameObject, 1);

        Rpc_Die(id);
    }

    [ClientRpc]
    void Rpc_Die(string id)
    {
        if(id != EnemySpawner.Instance.currentRoom.id) return;

        EnemyDestroyed();
    }

    //Fix
    public override void OnStartClient()
    {
        // if(idroom == EnemySpawner.Instance.currentRoom.id)
        // {
        //     EnemySpawner.Instance.enemiesSpawnedSoFar++;
        //     // Add one to the current enemy count - this is reduced when an enemy is destroyed
        //     EnemySpawner.Instance.currentEnemyCount++;

        //     var i = C_Data.Instance.player.allenemy.enemyList[index];

        //     EnemyInitialization(i, EnemySpawner.Instance.enemiesSpawnedSoFar, C_Data.Instance.crr_dungeonLevel);

        //     EnemySpawner.Instance.Subscribe_DestroyEvent(GetComponent<DestroyedEvent>());
        // }
        // else
        // {
        //     var i = C_Data.Instance.player.allenemy.enemyList[index];

        //     EnemyInitialization(i, EnemySpawner.Instance.enemiesSpawnedSoFar, C_Data.Instance.crr_dungeonLevel);
        // }

        EnemySpawner.Instance.enemiesSpawnedSoFar++;
        // Add one to the current enemy count - this is reduced when an enemy is destroyed
        EnemySpawner.Instance.currentEnemyCount++;

        var i = C_Data.Instance.player.allenemy.enemyList[index];

        EnemyInitialization(i, EnemySpawner.Instance.enemiesSpawnedSoFar, GameManager.Instance.GetCurrentDungeonLevel());

        EnemySpawner.Instance.Subscribe_DestroyEvent(GetComponent<DestroyedEvent>());
    }

    public void FireWeaponEvent_OnFireWeapon(FireWeaponEventArgs fireWeaponEventArgs)
    {
        Cmd_Fire(fireWeaponEventArgs);
    }

    [Command]
    void Cmd_Fire(FireWeaponEventArgs fireWeaponEventArgs)
    {
        Rpc_Fire(fireWeaponEventArgs);
    }

    [ClientRpc]
    void Rpc_Fire(FireWeaponEventArgs fireWeaponEventArgs)
    {
        if(isOwned) return;

        fireWeaponEvent.CallFireWeaponEvent(
                true, 
                true, 
                fireWeaponEventArgs.aimDirection, 
                fireWeaponEventArgs.aimAngle, 
                fireWeaponEventArgs.weaponAimAngle, 
                fireWeaponEventArgs.weaponAimDirectionVector);

        aimWeaponEvent.CallAimWeaponEvent(
                fireWeaponEventArgs.aimDirection,
                fireWeaponEventArgs.aimAngle, 
                fireWeaponEventArgs.weaponAimAngle, 
                fireWeaponEventArgs.weaponAimDirectionVector);
    }

    float t = 5;
    float crr_t = 5;
    public Transform GetPlayer()
    {
        if(player != null)
        {
            crr_t -= Time.deltaTime;

            if(crr_t <= 0)
            {
                crr_t = t;

                player = null;

                return this.GetPlayer();
            }
            else
            {
                return player;
            }
        }
        else
        {
            Transform p = null;

            Player[] players = FindObjectsOfType<Player>();

            int l = players.Length;

            if(l == 1)
            {
                p = players[0].transform;
            }
            else if(l == 2)
            {
                float d1 = Vector2.Distance(transform.position, players[0].transform.position);
                float d2 = Vector2.Distance(transform.position, players[1].transform.position);

                if(d1 > d2)
                {
                    p = players[1].transform;
                }
                else
                {
                    p = players[0].transform;
                }
            }

            player = p;

            return p;
        }
    }

    // Handle health lost event
    private void HealthEvent_OnHealthLost(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if (healthEventArgs.healthAmount <= 0)
        {
            Die();
        }
    }

    // Enemy destroyed
    private void EnemyDestroyed()
    {
        DestroyedEvent destroyedEvent = GetComponent<DestroyedEvent>();
        destroyedEvent.CallDestroyedEvent(false, health.GetStartingHealth());

    }

    // Initialise the enemy
    public void EnemyInitialization(EnemyDetailsSO enemyDetails, int enemySpawnNumber, DungeonLevelSO dungeonLevel)
    {
        this.enemyDetails = enemyDetails;

        SetEnemyMovementUpdateFrame(enemySpawnNumber);

        SetEnemyStartingHealth(dungeonLevel);

        SetEnemyStartingWeapon();

        SetEnemyAnimationSpeed();

        // Materialise enemy
        StartCoroutine(MaterializeEnemy());
    }

    // Set enemy movement update frame
    private void SetEnemyMovementUpdateFrame(int enemySpawnNumber)
    {
        // Set frame number that enemy should process it's updates
        enemyMovementAI.SetUpdateFrameNumber(enemySpawnNumber % Settings.targetFrameRateToSpreadPathfindingOver);
    }


    // Set the starting health for the enemy
    private void SetEnemyStartingHealth(DungeonLevelSO dungeonLevel)
    {
        // Get the enemy health for the dungeon level
        foreach (EnemyHealthDetails enemyHealthDetails in enemyDetails.enemyHealthDetailsArray)
        {
            if (enemyHealthDetails.dungeonLevel == dungeonLevel)
            {
                health.SetStartingHealth(enemyHealthDetails.enemyHealthAmount);
                return;
            }
        }
        health.SetStartingHealth(Settings.defaultEnemyHealth);

    }


    // Set enemy starting weapon as per the weapon details SO
    private void SetEnemyStartingWeapon()
    {
        // Process if enemy has a weapon
        if (enemyDetails.enemyWeapon != null)
        {
            Weapon weapon = new Weapon() { weaponDetails = enemyDetails.enemyWeapon, weaponReloadTimer = 0f, weaponClipRemainingAmmo = enemyDetails.enemyWeapon.weaponClipAmmoCapacity, weaponRemainingAmmo = enemyDetails.enemyWeapon.weaponAmmoCapacity, isWeaponReloading = false };

            //Set weapon for enemy
            setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);

        }
    }

    // Set enemy animator speed to match movement speed
    private void SetEnemyAnimationSpeed()
    {
        // Set animator speed to match movement speed
        animator.speed = enemyMovementAI.moveSpeed / Settings.baseSpeedForEnemyAnimations;
    }

    private IEnumerator MaterializeEnemy()
    {
        // Disable collider, Movement AI and Weapon AI
        EnemyEnable(false);

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(enemyDetails.enemyMaterializeShader, enemyDetails.enemyMaterializeColor, enemyDetails.enemyMaterializeTime, spriteRendererArray, enemyDetails.enemyStandardMaterial));

        // Enable collider, Movement AI and Weapon AI
        EnemyEnable(true);

    }

    private void EnemyEnable(bool isEnabled)
    {
        // Enable/Disable colliders
        circleCollider2D.enabled = isEnabled;
        polygonCollider2D.enabled = isEnabled;

        // Enable/Disable movement AI
        enemyMovementAI.enabled = isEnabled;

        // Enable / Disable Fire Weapon
        fireWeapon.enabled = isEnabled;

    }
}