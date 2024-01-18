using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using Mirror;
using System.Linq;


#region REQUIRE COMPONENTS
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(DealContactDamage))]
[RequireComponent(typeof(ReceiveContactDamage))]
[RequireComponent(typeof(DestroyedEvent))]
[RequireComponent(typeof(Destroyed))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
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
[RequireComponent(typeof(AnimatePlayer))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
#endregion REQUIRE COMPONENTS



public class Player : NetworkBehaviour
{
    [HideInInspector] public PlayerDetailsSO playerDetails;
    [HideInInspector] public HealthEvent healthEvent;
    [HideInInspector] public Health health;
    [HideInInspector] public DestroyedEvent destroyedEvent;
    [HideInInspector] public PlayerControl playerControl;
    [HideInInspector] public MovementByVelocityEvent movementByVelocityEvent;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public FireWeaponEvent fireWeaponEvent;
    [HideInInspector] public SetActiveWeaponEvent setActiveWeaponEvent;
    [HideInInspector] public ActiveWeapon activeWeapon;
    [HideInInspector] public WeaponFiredEvent weaponFiredEvent;
    [HideInInspector] public ReloadWeaponEvent reloadWeaponEvent;
    [HideInInspector] public WeaponReloadedEvent weaponReloadedEvent;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;

    public List<Weapon> weaponList = new List<Weapon>();

    public AllWeaponSO allWeapon;
    [SyncVar(hook = nameof(SyncWeapon))] public int weapon_index = 0;
    [SyncVar] public int chr_index = 0;

    private void Awake()
    {
        // Load components
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        destroyedEvent = GetComponent<DestroyedEvent>();
        playerControl = GetComponent<PlayerControl>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        idleEvent = GetComponent<IdleEvent>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        activeWeapon = GetComponent<ActiveWeapon>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    //Fix
    public override void OnStartAuthority()
    {
        C_Data.Instance.character = this;

        Initialize(GameResources.Instance.currentPlayer.playerDetails);

        setActiveWeaponEvent.OnSetActiveWeapon += SetWeaponEvent_OnSetActiveWeapon;

        SceneManager.LoadScene("MainGameScene");
    }

    public override void OnStartClient()
    {
        FindObjectOfType<Minimap>()?.AddPlayer(this);
    }

    public void OpenDoor(string id)
    {
        if (isOwned) Cmd_OpenDoor(id);
    }

    [Command]
    void Cmd_OpenDoor(string id)
    {
        Rpc_OpenDoor(id);
    }

    [ClientRpc]
    void Rpc_OpenDoor(string id)
    {
        try
        {
            EnemySpawner.Instance.OpenDoor(FindObjectsOfType<InstantiatedRoom>().FirstOrDefault(n => n.room.id == id).room);
        }
        catch
        {
        }
    }

    //Fix
    public void SyncWeapon(int _, int n)
    {
        if (isOwned) return;

        WeaponDetailsSO weaponDetails = allWeapon.weapons[n];

        Weapon weapon = new Weapon() { weaponDetails = weaponDetails, weaponReloadTimer = 0f, weaponClipRemainingAmmo = weaponDetails.weaponClipAmmoCapacity, weaponRemainingAmmo = weaponDetails.weaponAmmoCapacity, isWeaponReloading = false };

        setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);
    }

    //Fix
    private void SetWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        int a = 0;
        foreach (var i in allWeapon.weapons)
        {
            if (i == setActiveWeaponEventArgs.weapon.weaponDetails)
            {
                break;
            }

            a++;
        }

        weapon_index = a;
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
        if (isOwned) return;

        fireWeaponEvent.CallFireWeaponEvent(
                true,
                fireWeaponEventArgs.firePreviousFrame,
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

    // fix long
    public void AimWeaponEvent_OnWeaponAim(AimWeaponEventArgs aimWeaponEventArgs)
    {
        Cmd_Aim(aimWeaponEventArgs);
    }

    [Command]
    void Cmd_Aim(AimWeaponEventArgs aimWeaponEventArgs)
    {
        Rpc_Aim(aimWeaponEventArgs);
    }

    [ClientRpc]
    void Rpc_Aim(AimWeaponEventArgs aimWeaponEventArgs)
    {
        if (isOwned) return;

        aimWeaponEvent.CallAimWeaponEvent(aimWeaponEventArgs.aimDirection, aimWeaponEventArgs.aimAngle, aimWeaponEventArgs.weaponAimAngle, aimWeaponEventArgs.weaponAimDirectionVector);
    }




    // Initialize the player
    public void Initialize(PlayerDetailsSO playerDetails)
    {
        this.playerDetails = playerDetails;

        //Create player starting weapons
        CreatePlayerStartingWeapons();

        // Set player starting health
        SetPlayerHealth();
    }

    ///Fix
    public void Sv_Initialize(PlayerDetailsSO playerDetails)
    {
        this.playerDetails = playerDetails;

        //Create player starting weapons
        CreatePlayerStartingWeapons();
    }

    private void OnEnable()
    {
        // Subscribe to player health event
        healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void OnDisable()
    {
        // Unsubscribe from player health event
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;

        //Fix
        if (isOwned)
        {
            setActiveWeaponEvent.OnSetActiveWeapon -= SetWeaponEvent_OnSetActiveWeapon;
        }
    }

    // Handle health changed event
    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        // If player has died
        if (healthEventArgs.healthAmount <= 0f)
        {
            destroyedEvent.CallDestroyedEvent(true, 0);
        }

    }


    // Set the player starting weapon
    private void CreatePlayerStartingWeapons()
    {
        // Clear list
        weaponList.Clear();

        // Populate weapon list from starting weapons
        foreach (WeaponDetailsSO weaponDetails in playerDetails.startingWeaponList)
        {
            // Add weapon to player
            AddWeaponToPlayer(weaponDetails);
        }
    }

    // Set player health from playerDetails SO
    private void SetPlayerHealth()
    {
        health.SetStartingHealth(playerDetails.playerHealthAmount);
    }

    // Returns the player position
    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    // Add a weapon to the player weapon dictionary
    public Weapon AddWeaponToPlayer(WeaponDetailsSO weaponDetails)
    {
        Weapon weapon = new Weapon() { weaponDetails = weaponDetails, weaponReloadTimer = 0f, weaponClipRemainingAmmo = weaponDetails.weaponClipAmmoCapacity, weaponRemainingAmmo = weaponDetails.weaponAmmoCapacity, isWeaponReloading = false };

        // Add the weapon to the list
        weaponList.Add(weapon);

        // Set weapon position in list
        weapon.weaponListPosition = weaponList.Count;

        // Set the added weapon as active
        setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);

        return weapon;

    }

    // Returns true if the weapon is held by the player - otherwise returns false
    public bool IsWeaponHeldByPlayer(WeaponDetailsSO weaponDetails)
    {

        foreach (Weapon weapon in weaponList)
        {
            if (weapon.weaponDetails == weaponDetails) return true;
        }

        return false;
    }
}