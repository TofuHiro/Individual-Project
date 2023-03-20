using UnityEngine;

[RequireComponent(typeof(VitalsUI))]
public class PlayerVitals : MonoBehaviour, IDamagable
{
    #region Singleton
    public static PlayerVitals Instance;
    void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(Instance);
        }
        else {
            Instance = this;
        }
    }
    #endregion

    public static bool IsDead;

    [Header("Shield")]
    [Tooltip("The starting maximum shield of the player")]
    [SerializeField] float maxShield = 0f;
    [Tooltip("The rate that the player recharges their shields")]
    [SerializeField] float shieldRecoverRate = 10f;
    [Tooltip("The time delay required between taking damage to start recovering")]
    [SerializeField] float shieldRecoverStartDelay = 3f;
    float nextTimeToStartShieldRecover;

    [Header("Health")]
    [Tooltip("The starting maximum health")]
    [SerializeField] float maxHealth = 100f;

    [Header("Water")]
    [Tooltip("The starting maximum water levels")]
    [SerializeField] float maxWater = 100f;
    [Tooltip("The time between each decrement of water level")]
    [SerializeField] float waterDecayRate = 15f;
    [Tooltip("The time between taking damage from being parched")]
    [SerializeField] float parchedDamageRate = 1f;
    [Tooltip("The damage taken from being parched")]
    [SerializeField] float parchDamage = 1f;
    bool isParched;
    float nextParchTick, nextWaterTick;

    [Header("Food")]
    [Tooltip("The starting maximum food levels")]
    [SerializeField] float maxFood = 100f;
    [Tooltip("The time between each decrement of food level")]
    [SerializeField] float foodDecayRate = 10f;
    [Tooltip("The time between taking damage from starving")]
    [SerializeField] float starveDamageRate = 2f;
    [Tooltip("The damage taken from starving")]
    [SerializeField] float starveDamage = 1f;
    bool isStarving;
    float nextStarveTick, nextFoodTick;

    [Header("Oxygen")]
    [Tooltip("The starting maximum oxygen breathing time")]
    [SerializeField] float baseOxygenTime = 60f;
    [Tooltip("The time between each damage taken from suffocation")]
    [SerializeField] float suffocateDamageRate = .5f;
    [Tooltip("The damage taken from suffocating")]
    [SerializeField] float suffocateDamage = 10f;
    [Tooltip("The rate of oxygen recovery")]
    [SerializeField] float oxygenRecoverRate = 10f;
    bool isSuffocating, inAir;
    float nextSuffocateTick, currentMaxOxygenTime;

    DeathManager deathManager;
    VitalsUI UI;
    float timer;

    public float Shield { get { return shield; }
        set {
            shield = value;
            shield = Mathf.Clamp(shield, 0f, maxShield);
            UI.SetShield(shield);
        }
    }
    float shield;

    public float Health { get { return health; }
        set {
            health = value;
            health = Mathf.Clamp(health, 0f, maxHealth);
            UI.SetHealth(health);

            if (health <= 0f) {
                Die();
            }
        }
    }
    float health;

    public float Water { get { return water; }
        set {
            water = value;
            water = Mathf.Clamp(water, 0f, maxWater);
            UI.SetWater(water);

            isParched = (water <= 0f);
        }
    }
    float water;

    public float Food { get { return food; }
        set {
            food = value;
            food = Mathf.Clamp(food, 0f, maxFood);
            UI.SetFood(food);

            isStarving = (food <= 0f);
        }
    }
    float food;

    public float Oxygen
    {
        get { return oxygen; }
        set {
            oxygen = value;
            oxygen = Mathf.Clamp(oxygen, 0f, currentMaxOxygenTime);
            UI.SetOxygen(oxygen);

            isSuffocating = (oxygen <= 0f);
        }
    }
    float oxygen;

    void Start()
    {
        deathManager = DeathManager.Instance;
        UI = GetComponent<VitalsUI>();

        DeathManager.OnRespawn += Respawn;

        //Shield
        UI.SetMaxShield(maxShield);
        Shield = 0f;

        //Health
        UI.SetMaxHealth(maxHealth);
        Health = maxHealth;

        //Water
        UI.SetMaxWater(maxWater);
        Water = maxWater;
        nextWaterTick = waterDecayRate;
        nextParchTick = parchedDamageRate;

        //Food
        UI.SetMaxFood(maxFood);
        Food = maxFood;
        nextFoodTick = foodDecayRate;
        nextStarveTick = starveDamageRate;

        //Oxygen
        UI.SetMaxOxygen(baseOxygenTime);
        currentMaxOxygenTime = baseOxygenTime;
        Oxygen = baseOxygenTime;
        nextSuffocateTick = suffocateDamageRate;
    }

    void Update()
    {
        if (IsDead)
            return;

        timer += Time.deltaTime;

        //Recover shields
        if (timer >= nextTimeToStartShieldRecover) {
            Shield += Time.deltaTime * shieldRecoverRate;
        }

        //Decrease water
        if (timer >= nextWaterTick) {
            Water--;
            nextWaterTick = timer + waterDecayRate;
        }
        //Decrease health if parched
        if (isParched && timer >= nextParchTick) {
            Health -= parchDamage;
            nextParchTick = timer + parchedDamageRate;
        }

        //Decrease food
        if (timer >= nextFoodTick) {
            Food--;
            nextFoodTick = timer + foodDecayRate;
        }
        //Decrease health if starving
        if (isStarving && timer >= nextStarveTick ) {
            Health -= starveDamage;
            nextStarveTick = timer + starveDamageRate;
        }

        //Decrease oxygen
        if (!inAir) {
            Oxygen -= Time.deltaTime;
        }
        else {
            Oxygen += Time.deltaTime * oxygenRecoverRate;
        }
        //Decrease health is suffocating
        if (isSuffocating && timer >= nextSuffocateTick) {
            Health -= suffocateDamage;
            nextSuffocateTick = timer + suffocateDamageRate;
        }
    }

    /// <summary>
    /// Apply an amount of damage to the player
    /// </summary>
    /// <param name="_value">The amount of damage to apply</param>
    public void TakeDamage(float _value)
    {
        float _damage = _value;
        if (_damage > Shield) {
            _damage -= Shield;
            Shield = 0f;
            Health -= _damage;
        }
        else {
            Shield -= _value;
        }
        
        nextTimeToStartShieldRecover = timer + shieldRecoverStartDelay;
    }

    public void Die()
    {
        IsDead = true;
        deathManager.Die();
    }

    void Respawn()
    {
        IsDead = false;

        Health = maxHealth;
        Oxygen = currentMaxOxygenTime;
    }

    /// <summary>
    /// Add a value to the player's current shield levels
    /// </summary>
    /// <param name="_value">The value to add</param>
    public void AddShield(float _value)
    {
        Shield += _value;
    }

    /// <summary>
    /// Add a value to the player's current maximum shield levels
    /// </summary>
    /// <param name="_value">The value to add</param>
    public void AddMaxShield(float _value)
    {
        maxShield += _value;
        UI.SetMaxShield(maxShield);
    }

    /// <summary>
    /// Add a value to the player's current health
    /// </summary>
    /// <param name="_value">The value to add</param>
    public void AddHealth(float _value)
    {
        Health += _value;
    }

    /// <summary>
    /// Sets the player's current health to a value
    /// </summary>
    /// <param name="_value"></param>
    public void SetHealth(float _value)
    {
        Health = _value;
    }

    /// <summary>
    /// Add a value to the player's current maximum health
    /// </summary>
    /// <param name="_value">The value to add</param>
    public void AddMaxHealth(float _value)
    {
        maxHealth += _value;
        UI.SetMaxHealth(_value);
    }

    /// <summary>
    /// Add a value to the player's current water levels
    /// </summary>
    /// <param name="_value">The value to add</param>
    public void AddWater(float _value)
    {
        Water += _value;
    }

    /// <summary>
    /// Sets the player's current water levels to a value
    /// </summary>
    /// <param name="_value"></param>
    public void SetWater(float _value)
    {
        Water = _value;
    }

    /// <summary>
    /// Add a value to the player's current maximum water levels
    /// </summary>
    /// <param name="_value">The value to add</param>
    public void AddMaxWater(float _value)
    {
        maxWater += _value;
        UI.SetMaxWater(_value);
    }

    /// <summary>
    /// Add a value to the player's current food levels
    /// </summary>
    /// <param name="_value">The value to add</param>
    public void AddFood(float _value)
    {
        Food += _value;
    }

    /// <summary>
    /// Sets the player's current food levels to a value
    /// </summary>
    /// <param name="_value"></param>
    public void SetFood(float _value)
    {
        Food = _value;
    }

    /// <summary>
    /// Add a value to the player's current maximum food levels
    /// </summary>
    /// <param name="_value">The value to add</param>
    public void AddMaxFood(float _value)
    {
        maxFood += _value;
        UI.SetMaxFood(_value);
    }

    /// <summary>
    /// Add a value to the player's current oxygen levels
    /// </summary>
    /// <param name="_value">The value to add</param>
    public void AddOxygen(float _value)
    {
        Oxygen += _value;
    }

    /// <summary>
    /// Add a value to the player's current maximum oxygen levels
    /// </summary>
    /// <param name="_value">The value to add</param>
    public void AddMaxOxygen(float _value)
    {
        currentMaxOxygenTime += _value;
        UI.SetMaxOxygen(_value);
    }

    //Determine whether player is in oxygen or not
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Oxygen")) {
            inAir = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Oxygen")) {
            inAir = false;
        }
    }
}
