using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(AICamera))]
public class Enemy : MonoBehaviour, IDamagable
{
    [Tooltip("The hand transform of this enemy. This stores the position of the weapon")]
    [SerializeField] Transform hands;
    [Tooltip("The starting maximum health of this enemy")]
    [SerializeField] float maxHealth = 100f;

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

    bool isFloating, isAttacking;
    float timer, nextTimeToAttack;

    void Start()
    {
        player = PlayerController.Instance;
        motor = GetComponent<PlayerMotor>();
        orientation = GetComponent<AICamera>();
        rb = GetComponent<Rigidbody>();

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
    /// Rotates the enemy to a given rotation
    /// </summary>
    /// <param name="_dir">The rotation to rotate towards</param>
    void Rotate(Quaternion _dir)
    {
        orientation.Rotate(_dir);
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
            //Attack if close enough and looking at player
            if (_distToPlayer <= attackRange) {
                //Target player and attempt to attack
                LookAtPlayer();
                Move(Vector3.zero);
                if (timer >= nextTimeToAttack) {
                    StartCoroutine(Attack());
                }
            }
            //Rotate and move to player
            else {
                LookAtPlayer();
                Move(motor.GetOrientation().transform.forward);
            }
        }
        else {
            Idle();
        }
    }

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
        Rotate(Quaternion.LookRotation(GetDirFromPlayer()));
    }

    void Idle()
    {
        Move(Vector3.zero);
        //
    }

    /// <summary>
    /// Apply damage to this enemy by a given value
    /// </summary>
    /// <param name="_value">Amount of damage to apply</param>
    public void TakeDamage(float _value)
    {
        Health -= _value;
    }

    public void Die()
    {
        //
        gameObject.SetActive(false);
    }

}
