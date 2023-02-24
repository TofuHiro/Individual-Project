using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(AICamera))]
public class Enemy : MonoBehaviour, IDamagable
{
    [Tooltip("The name for this enemy")]
    [SerializeField] new string name;
    [Tooltip("The hand transform of this enemy. This stores the position of the weapon")]
    [SerializeField] Transform hands;
    [Tooltip("The starting maximum health of this enemy")]
    [SerializeField] float maxHealth = 100f;
    [Tooltip("The prefab instantiated upon death")]
    [SerializeField] GameObject corpsePrefab;

    [Header("Attack")]
    [Tooltip("The starting weapon for this enemy")]
    [SerializeField] Weapon currentWeapon;
    [Tooltip("The distance between the enemy and player where the enemy will attempt to attack")]
    [SerializeField] float attackRange = 5f;
    [Tooltip("The distance between the enemy and player where the enemy will start chasing the player. ")]
    [SerializeField] float aggroRange = 10f;
    [Tooltip("The time required before carrying out an attack")]
    [SerializeField] float attackDelay = .5f;
    [Tooltip("The time required before attempting to perform an attack")]
    [SerializeField] float attackRate = 2f;

    [Header("Idle")]
    [Tooltip("The minimum time possible to walk when picking a random time to idle walk")]
    [SerializeField] float minWalkTime = 3f;
    [Tooltip("The maximum time possible to walk when picking a random time to idle walk")]
    [SerializeField] float maxWalkTime = 6f;
    [Tooltip("The time to wait before walking upon each idle walk")]
    [SerializeField] float moveDelay = 2f;
    [Tooltip("The minimum angle to turn when idling")]
    [SerializeField] float minTurnAngle = 60f;
    [Tooltip("The maximum angle to turn when idling")]
    [SerializeField] float maxTurnAngle = 360f;
    [Tooltip("The time to wait while rotating to complete")]
    [SerializeField] float rotateTime = 1f;

    public float Health { get { return health; }
        set {
            health = value;

            if (health <= 0f) {
                Die();
            }
        }
    }
    float health;

    public bool IsAggro { get; private set; }
    public bool IsActive { get; private set; } = true;

    PlayerController player;
    PlayerMotor motor;
    AICamera orientation;
    Rigidbody rb;
    ObjectPooler objectPooler;
    Coroutine lastIdleRoutine;

    bool isFloating, isAttacking, isIdling;
    float timer, nextTimeToAttack;

    void Start()
    {
        player = PlayerController.Instance;
        motor = GetComponent<PlayerMotor>();
        orientation = GetComponent<AICamera>();
        rb = GetComponent<Rigidbody>();
        objectPooler = ObjectPooler.Instance;

        health = maxHealth;
        nextTimeToAttack = attackRate;

        currentWeapon.Equip(hands);
    }

    /// <summary>
    /// Sets the state of this enemy
    /// </summary>
    /// <param name="_state">If true, the enemy will be able to move</param>
    public void SetActive(bool _state)
    {
        IsActive = _state;
    }

    /// <summary>
    /// Sets the movement mode of the enemy from floating to walking
    /// </summary>
    /// <param name="_state">If true, the enemy will move in a 3D axis ignoring gravity</param>
    public void SetFloatingMode(bool _state)
    {
        isFloating = _state;
        
        //motor.SetFloatingMode(_state);

        if (_state) {
            rb.constraints = RigidbodyConstraints.None;
        }
        else {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    /// <summary>
    /// Returns the distance between this enemy and the player
    /// </summary>
    /// <returns>The distance of this enemy from the player</returns>
    float GetPlayerDistance()
    {
        return Vector3.Distance(player.transform.position, transform.position);
    }

    /// <summary>
    /// Returns the direction of the enemy from the player
    /// </summary>
    /// <returns>Returns a normalized Vector3 of the direction of the enemy from the player</returns>
    Vector3 GetDirFromPlayer()
    {
        return (player.transform.position + new Vector3(0f, 1f, 0f) - transform.position).normalized;
    }

    /// <summary>
    /// Moves the enemy to a given position
    /// </summary>
    /// <param name="_dir">The Vector3 position to move the enemy to</param>
    void Move(Vector3 _dir)
    {
        motor.SetDirection(_dir);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!IsActive)
            return;
        if (isAttacking) {
            return;
        }

        float _distToPlayer = GetPlayerDistance();
        //If player is near enemy
        IsAggro = _distToPlayer <= aggroRange;

        if (IsAggro) {
            //Stop idle moving
            StopCoroutine(lastIdleRoutine);
            StopMovement();
            StopRotation();
            isIdling = false;

            //Attack if close enough and looking at player
            if (_distToPlayer <= attackRange) {
                //Target player and attempt to attack
                LookAtPlayer();
                StopMovement();
                if (timer >= nextTimeToAttack) {
                    StartCoroutine(Attack());
                }
            }
            //Rotate and move to player
            else {
                LookAtPlayer();
                Move(orientation.GetOrientation().transform.forward);
            }
        }
        else {
            //Start idling
            if (!isIdling) {
                isIdling = true;
                StopMovement();
                lastIdleRoutine = StartCoroutine(Idle());
            }
        }
    }

    /// <summary>
    /// Attacks with the currently equipped weapon
    /// </summary>
    /// <returns></returns>
    IEnumerator Attack()
    {
        isAttacking = true;
        currentWeapon.SetPrimaryAttack(true);

        yield return new WaitForSeconds(attackDelay);

        isAttacking = false;
        currentWeapon.SetPrimaryAttack(false);

        nextTimeToAttack = timer + attackRate;
    }

    /// <summary>
    /// Rotates the enemy towards the player
    /// </summary>
    void LookAtPlayer()
    {
        orientation.SetRotation(Quaternion.LookRotation(GetDirFromPlayer()));
    }

    /// <summary>
    /// Moves and rotates randomly at invervals
    /// </summary>
    /// <returns></returns>
    IEnumerator Idle()
    {
        yield return new WaitForSeconds(moveDelay);

        //Move
        Move(orientation.GetOrientation().forward);
        yield return new WaitForSeconds(Random.Range(minWalkTime, maxWalkTime));

        //Stop
        StopMovement();

        //Rotate
        if (isFloating) {
            orientation.SetRotation(Quaternion.Euler(orientation.GetOrientation().rotation.eulerAngles + new Vector3(GetRandomLookAngle(), GetRandomLookAngle(), 0f)));
        }
        else {
            orientation.SetRotation(Quaternion.Euler(orientation.GetOrientation().rotation.eulerAngles + new Vector3(0f, GetRandomLookAngle(), 0f)));
        }
        //To finish rotating
        yield return new WaitForSeconds(rotateTime);

        isIdling = false;
    }

    /// <summary>
    /// Apply damage to this enemy by a given value
    /// </summary>
    /// <param name="_value">Amount of damage to apply</param>
    public void TakeDamage(float _value)
    {
        Health -= _value;
    }

    /// <summary>
    /// Spawns the enemy's corpse and drops their weapon
    /// </summary>
    public void Die()
    {
        StopAllCoroutines();
        StopMovement();
        StopRotation();

        isAttacking = false;
        currentWeapon.SetPrimaryAttack(false);
        currentWeapon.Holster();

        IsActive = false;
        objectPooler.PoolObject(name, gameObject);
        objectPooler.SpawnObject(name + "_corpse", corpsePrefab, orientation.GetOrientation().position, orientation.GetOrientation().rotation);
    }

    /// <summary>
    /// Returns a negative or positive range between the min and max turn angle.
    /// </summary>
    /// <returns>An angle between the minimum and maximum turn angle</returns>
    float GetRandomLookAngle()
    {
        //50/50 for negative or positive
        bool _negative = Random.Range(0f, 1f) >= .5f;
        if (_negative) {
            return -Random.Range(minTurnAngle, maxTurnAngle);
        }
        else {
            return Random.Range(minTurnAngle, maxTurnAngle);
        }
    }

    void StopRotation()
    {
        orientation.SetRotation(orientation.GetOrientation().rotation);
    }

    void StopMovement()
    {
        Move(Vector3.zero);
    }
}
