using System.Collections;
using UnityEngine;
using SpaceGame;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(AICamera))]
public class Enemy : MonoBehaviour, IDamagable
{
    public static float GlobalDamageMultiplier = 1f;
    public static float GlobalHealthMultiplier = 1f;

    public string Name { get { return name; } }

    [Tooltip("Animator of this enemy")]
    [SerializeField] Animator animator;
    [Tooltip("The name for this enemy")]
    [SerializeField] new string name;
    [Tooltip("The hand transform of this enemy. This stores the position of the weapon")]
    [SerializeField] Transform hands;
    [Tooltip("The starting maximum health of this enemy")]
    [SerializeField] float maxHealth = 100f;
    [Tooltip("The prefab instantiated upon death")]
    [SerializeField] GameObject corpsePrefab;
    [Tooltip("The minimum distance the players needs to be for this enemy to be active")]
    [SerializeField] float activeRadius;

    [Header("Attack")]
    [Tooltip("The starting weapon for this enemy")]
    [SerializeField] Weapon currentWeapon;
    [Tooltip("The distance between the enemy and player where the enemy will attempt to attack")]
    [SerializeField] float attackRange = 5f;
    [Tooltip("The distance between the enemy and player where the enemy will start chasing the player. ")]
    [SerializeField] float aggroRange = 10f;
    [Tooltip("The time required before attempting to perform an attack")]
    [SerializeField] float attackDelay = .5f;
    [Tooltip("The required time to pass while attacking to stop")]
    [SerializeField] float attackBurstTime = .3f;
    [Tooltip("The time required before carrying out an attack")]
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
    [Tooltip("The max distance the enemy can travel from its starting point")]
    [SerializeField] float idleRadius = 25f;

    [Header("Sounds")]
    [Tooltip("The delay between each idle sound")]
    [SerializeField] Vector2 idleSoundDelay;
    [Tooltip("Sounds to play when idling")]
    [SerializeField] string[] idleSounds;
    [Tooltip("The delay between each aggro sound")]
    [SerializeField] Vector2 aggroSoundDelay;
    [Tooltip("Sounds to play when aggro")]
    [SerializeField] string[] aggroSounds;
    [Tooltip("The delay between each hurt sound")]
    [SerializeField] float hurtSoundDelay;
    [Tooltip("Sounds to play when damaged")]
    [SerializeField] string[] hurtSounds;
    [Tooltip("Sounds to play when dying")]
    [SerializeField] string[] deathSounds;

    public float Health { get { return health; }
        set {
            health = value;
            healthDisplayer.ShowHealthBar(transform, maxHealth, health);

            //Damage sounds
            if (timer >= nextTimeToHurtSound) {
                foreach (string _sound in hurtSounds) {
                    audioManager.PlayClip(_sound, transform.position);
                }
                nextTimeToHurtSound = timer + hurtSoundDelay;
            }

            if (health <= 0f) {
                Die();

                //Death sounds
                foreach (string _sound in deathSounds) {
                    audioManager.PlayClip(_sound, transform.position);
                }
            }
        }
    }
    float health;

    public bool IsAggro { get; private set; }
    public bool IsActive { get; private set; } = true;

    AudioManager audioManager;
    EnemySpawner spawner;
    PlayerController player;
    HealthDisplayer healthDisplayer;
    PlayerMotor motor;
    AICamera orientation;
    Coroutine lastIdleRoutine;

    bool isAttacking, isIdling;
    float timer, nextTimeToAttack, nextTimeToIdleSound, nextTimeToAggroSound, nextTimeToHurtSound;

    Vector3 startPos;

    void Start()
    {
        audioManager = AudioManager.Instance;
        player = PlayerController.Instance;
        healthDisplayer = HealthDisplayer.Instance;
        motor = GetComponent<PlayerMotor>();
        orientation = GetComponent<AICamera>();

        startPos = transform.position;
        nextTimeToIdleSound = Random.Range(idleSoundDelay.x, idleSoundDelay.y);
        nextTimeToAggroSound = Random.Range(aggroSoundDelay.x, aggroSoundDelay.y);
        nextTimeToHurtSound = hurtSoundDelay;

        currentWeapon.Equip(hands);
        currentWeapon.ApplyDamageMultiplier(GlobalDamageMultiplier);

        maxHealth *= GlobalHealthMultiplier;
        ResetEnemy();
    }

    void ResetEnemy()
    {
        health = maxHealth;
        nextTimeToAttack = attackRate;
        Enable();
    }

    void OnEnable()
    {
        ResetEnemy();
        GameManager.OnPlayerDie += Disable;
        GameManager.OnPlayerRespawn += Enable;
    }

    void OnDisable()
    {
        GameManager.OnPlayerDie -= Disable;
        GameManager.OnPlayerRespawn -= Enable;
    }

    void OnDestroy()
    {
        GameManager.OnPlayerDie -= Disable;
        GameManager.OnPlayerRespawn -= Enable;
    }

    /// <summary>
    /// Set the spawner origin of this enemy
    /// </summary>
    /// <param name="_spawner"></param>
    public void SetSpawner(EnemySpawner _spawner)
    {
        spawner = _spawner;
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
        return (player.transform.position - new Vector3(0f, .2f, 0f) - transform.position).normalized;
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
        SetActive(GetPlayerDistance() <= activeRadius);
        timer += Time.deltaTime;

        if (!IsActive)
            return;
        if (isAttacking) {
            return;
        }

        float _distToPlayer = GetPlayerDistance();
        //If player is near enemy
        IsAggro = _distToPlayer <= aggroRange;

        //Aggressive
        if (IsAggro) {
            //Aggro sounds
            if (timer >= nextTimeToAggroSound) {
                foreach (string _sound in aggroSounds) {
                    audioManager.PlayClip(_sound, transform.position);
                }
                nextTimeToAggroSound = timer + Random.Range(aggroSoundDelay.x, aggroSoundDelay.y);
            }

            animator.SetBool("IsTargeting", true);

            //Stop idle moving
            if (lastIdleRoutine != null) {
                StopCoroutine(lastIdleRoutine);
            }

            StopMovement();
            StopRotation();
            isIdling = false;
            animator.SetBool("IsIdleMoving", false);

            //Attack if close enough and looking at player
            if (_distToPlayer <= attackRange) {
                animator.SetBool("IsAttacking", true);
                //Target player and attempt to attack
                LookAtPlayer();
                StopMovement();
                if (timer >= nextTimeToAttack) {
                    if (!isAttacking)
                    StartCoroutine(Attack());
                }
            }
            //Rotate and move to player
            else {
                animator.SetBool("IsAttacking", false);
                LookAtPlayer();
                Move(orientation.GetOrientation().transform.forward);
            }
        }

        //Passive
        else {
            //Start idling
            if (!isIdling) {
                animator.SetBool("IsTargeting", false);
                isIdling = true;
                StopMovement();
                lastIdleRoutine = StartCoroutine(Idle());
            }

            //Idle sounds
            if (timer >= nextTimeToIdleSound) {
                foreach (string _sound in idleSounds) {
                    audioManager.PlayClip(_sound, transform.position);
                }
                nextTimeToIdleSound = timer + Random.Range(idleSoundDelay.x, idleSoundDelay.y);
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
        //Wait before attack
        yield return new WaitForSeconds(attackDelay);

        //Start attacking
        currentWeapon.SetPrimaryAttack(true);
        animator.SetTrigger("Attack");

        //Keep attacking
        yield return new WaitForSeconds(attackBurstTime);

        //Stop attacking
        isAttacking = false;
        currentWeapon.SetPrimaryAttack(false);
        nextTimeToAttack = timer + attackRate;
    }

    /// <summary>
    /// Rotates the enemy towards the player
    /// </summary>
    void LookAtPlayer()
    {
        orientation.SetRotation(Quaternion.LookRotation(GetDirFromPlayer()), motor.IsFloating);
    }

    /// <summary>
    /// Moves and rotates randomly at invervals
    /// </summary>
    /// <returns></returns>
    IEnumerator Idle()
    {
        //Look towards start pos if too far off
        if (Vector3.Distance(startPos, transform.position) > idleRadius) {
            orientation.LookAt(Quaternion.LookRotation((startPos - transform.position).normalized), motor.IsFloating);
        }

        yield return new WaitForSeconds(moveDelay);

        //Move
        animator.SetBool("IsIdleMoving", true);
        Move(orientation.GetOrientation().forward);
        yield return new WaitForSeconds(Random.Range(minWalkTime, maxWalkTime));

        //Stop
        StopMovement();
        animator.SetBool("IsIdleMoving", false);

        //Rotate
        if (motor.IsFloating) {
            orientation.SetRotation(Quaternion.Euler(orientation.GetOrientation().rotation.eulerAngles + new Vector3(GetRandomLookAngle(), GetRandomLookAngle(), 0f)), motor.IsFloating);
        }
        else {
            orientation.SetRotation(Quaternion.Euler(orientation.GetOrientation().rotation.eulerAngles + new Vector3(0f, GetRandomLookAngle(), 0f)), motor.IsFloating);
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
        Disable();
        spawner.EnemyNumber--;
        
        //Change to corpse
        ObjectPooler.PoolObject(Name, gameObject);
        ObjectPooler.SpawnObject(Name + "_corpse", corpsePrefab, orientation.GetOrientation().position, orientation.GetOrientation().rotation);
    }

    /// <summary>
    /// Returns a negative or positive range between the min and max turn angle. Chance between a positive and negative is 50/50
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
        orientation.SetRotation(orientation.GetOrientation().rotation, motor.IsFloating);
    }

    void StopMovement()
    {
        Move(Vector3.zero);
    }

    /// <summary>
    /// Disabled the enemy
    /// </summary>
    void Disable()
    {
        if (lastIdleRoutine != null) {
            StopCoroutine(lastIdleRoutine);
        }
        StopRotation();
        StopMovement();
        SetActive(false);
        isAttacking = false;
        isIdling = false;
        IsAggro = false;
        currentWeapon.SetPrimaryAttack(false);
        currentWeapon.SetSecondaryAttack(false);
    }

    /// <summary>
    /// Enables the enemy
    /// </summary>
    void Enable()
    {
        SetActive(true);
    }
}
