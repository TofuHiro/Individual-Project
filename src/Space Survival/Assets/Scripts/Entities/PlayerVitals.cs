using UnityEngine;
using SpaceGame;

[RequireComponent(typeof(VitalsUI))]
[RequireComponent(typeof(OxygenConsumer))]
public class PlayerVitals : MonoBehaviour, IDamagable, IDataPersistance
{
    public static PlayerVitals Instance;

    public delegate void PlayerVitalStateChange(bool _state);
    public PlayerVitalStateChange OnOxygenTriggerChange;

    /// <summary>
    /// If this player is dead
    /// </summary>
    public static bool IsDead { get; private set; }

    /// <summary>
    /// The shield levels of the player
    /// </summary>
    public float Shield
    {
        get { return shield; }
        set {
            shield = value;
            shield = Mathf.Clamp(shield, 0f, maxShield);
            UI.SetShield(shield);
        }
    }
    float shield;

    /// <summary>
    /// The maximum shield levels of the player
    /// </summary>
    public float MaxShield
    {
        get { return maxShield; }
        set {
            maxShield = value;
            UI.SetMaxShield(value);
            if (Shield > maxShield)
                Shield = maxShield;
        }
    }

    /// <summary>
    /// The health of the player
    /// </summary>
    public float Health
    {
        get { return health; }
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

    /// <summary>
    /// The maximum health of the player
    /// </summary>
    public float MaxHealth
    {
        get { return maxHealth; }
        set {
            maxHealth = value;
            UI.SetMaxHealth(value);
            if (Health > maxHealth)
                Health = maxHealth;
        }
    }

    /// <summary>
    /// The water levels of the player
    /// </summary>
    public float Water
    {
        get { return water; }
        set {
            water = value;
            water = Mathf.Clamp(water, 0f, maxWater);
            UI.SetWater(water);

            isDehydrating = (water <= 0f);
        }
    }
    float water;

    /// <summary>
    /// The maximum water levels of the player
    /// </summary>
    public float MaxWater
    {
        get { return maxWater; }
        set {
            maxWater = value;
            UI.SetMaxWater(value);
            if (Water > maxWater)
                Water = maxWater;
        }
    }

    /// <summary>
    /// The food levels of the player
    /// </summary>
    public float Food
    {
        get { return food; }
        set {
            food = value;
            food = Mathf.Clamp(food, 0f, maxFood);
            UI.SetFood(food);

            isStarving = (food <= 0f);
        }
    }
    float food;

    /// <summary>
    /// The maximum food levels of the player
    /// </summary>
    public float MaxFood
    {
        get { return maxFood; }
        set {
            maxFood = value;
            UI.SetMaxFood(value);
            if (Food > maxFood)
                Food = maxFood;
        }
    }

    /// <summary>
    /// The food levels of the player
    /// </summary>
    public float Oxygen
    {
        get { return playerOxygen.GetOxygen(); }
        set {
            playerOxygen.SetOxygen(value);
            UI.SetOxygen(playerOxygen.GetOxygen());
        }
    }

    /// <summary>
    /// The maximum food levels of the player
    /// </summary>
    public float MaxOxygen
    {
        get { return playerOxygen.GetMaxOxygen(); }
        set {
            playerOxygen.SetMaxOxygenTime(value);
            UI.SetMaxOxygen(playerOxygen.GetMaxOxygen());
        }
    }

    [Tooltip("The multiplier applied on the effects of consuming nutritions")]
    [SerializeField] float sustenanceMultiplier = 1f;

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
    [Tooltip("The health regenerated per second")]
    [SerializeField] float healthRegen = .1f;
    float nextHealTick;
    float respawnHealth;

    [Header("Water")]
    [Tooltip("The starting maximum water levels")]
    [SerializeField] float maxWater = 100f;
    [Tooltip("The time between each decrement of water level")]
    [SerializeField] float waterDecayRate = 15f;
    [Tooltip("The time between taking damage from being parched")]
    [SerializeField] float dehydrationDamageRate = 1f;
    [Tooltip("The damage taken from being parched")]
    [SerializeField] float dehydrationDamage = 1f;
    bool isDehydrating;
    float nextDehydrateTick, nextWaterTick;
    float respawnWater;

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
    float respawnFood;

    [Header("Oxygen")]
    [Tooltip("Whether to consume oxygen")]
    [SerializeField] bool useOxygen = true;
    [Tooltip("The time between each damage taken from suffocation")]
    [SerializeField] float suffocateDamageRate = .5f;
    [Tooltip("The damage taken from suffocating")]
    [SerializeField] float suffocateDamage = 10f;
    float nextSuffocateTick;

    OxygenConsumer playerOxygen;
    GameManager gameManager;
    VitalsUI UI;
    float timer;
    bool inOxygen;

    void Awake()
    {
        //Singleton init
        if (Instance != null && Instance != this) {
            Destroy(Instance);
        }
        Instance = this;

        UI = GetComponent<VitalsUI>();
        playerOxygen = GetComponent<OxygenConsumer>();
    }

    void Start()
    {
        gameManager = GameManager.Instance;
        GameManager.OnPlayerRespawn += Respawn;

        nextWaterTick = waterDecayRate;
        nextDehydrateTick = dehydrationDamageRate;

        nextFoodTick = foodDecayRate;
        nextStarveTick = starveDamageRate;

        UI.SetMaxOxygen(playerOxygen.GetMaxOxygen());
        UI.SetOxygen(playerOxygen.GetOxygen());
        nextSuffocateTick = suffocateDamageRate;
    }

    void OnDisable()
    {
        GameManager.OnPlayerRespawn -= Respawn;
        IsDead = false;
    }

    void OnDestroy()
    {
        GameManager.OnPlayerRespawn -= Respawn;
        IsDead = false;
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

        //Recover health
        if (healthRegen > 0) {
            if (timer >= nextHealTick) {
                Health += healthRegen;
                nextHealTick = timer + 1;
            }
        }

        //Decrease water
        if (timer >= nextWaterTick) {
            Water--;
            nextWaterTick = timer + waterDecayRate;
        }
        //Decrease health if parched
        if (isDehydrating && timer >= nextDehydrateTick) {
            Health -= dehydrationDamage;
            nextDehydrateTick = timer + dehydrationDamageRate;
        }

        //Decrease food
        if (timer >= nextFoodTick) {
            Food--;
            nextFoodTick = timer + foodDecayRate;
        }
        //Decrease health if starving
        if (isStarving && timer >= nextStarveTick) {
            Health -= starveDamage;
            nextStarveTick = timer + starveDamageRate;
        }

        if (useOxygen) {
            UI.SetOxygen(playerOxygen.GetOxygen());
            //Decrease health is suffocating
            if (playerOxygen.IsSuffocating && timer >= nextSuffocateTick) {
                Health -= suffocateDamage;
                nextSuffocateTick = timer + suffocateDamageRate;
            }
        }

        //Modify sounds based on oxygen
        if (playerOxygen.InOxygen != inOxygen) {
            OnOxygenTriggerChange?.Invoke(playerOxygen.InOxygen);
            inOxygen = playerOxygen.InOxygen;
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
        gameManager.Die();
    }

    void Respawn()
    {
        IsDead = false;
        Health = respawnHealth;
        Water = respawnWater;
        Food = respawnFood;
        playerOxygen.SetMax();
    }

    public void FullHeal()
    {
        Health = MaxHealth;
        Water = MaxWater;
        Food = MaxFood;
        Oxygen = MaxOxygen;
    }

    /// <summary>
    /// Add a value to the maximum oxygen
    /// </summary>
    /// <param name="_value"></param>
    public void AddMaxOxygen(float _value)
    {
        MaxOxygen += _value;
    }

    /// <summary>
    /// Add a value to current shield level
    /// </summary>
    /// <param name="_value"></param>
    public void AddShield(float _value)
    {
        Shield += _value;
    }

    /// <summary>
    /// Add a value to current health level
    /// </summary>
    /// <param name="_value"></param>
    public void AddHealth(float _value)
    {
        Health += _value;
    }

    /// <summary>
    /// Add a value to current water level
    /// </summary>
    /// <param name="_value"></param>
    public void AddWater(float _value)
    {
        Water += _value * sustenanceMultiplier;
    }

    /// <summary>
    /// Add a value to current food level
    /// </summary>
    /// <param name="_value"></param>
    public void AddFood(float _value)
    {
        Food += _value * sustenanceMultiplier;
    }

    /// <summary>
    /// Add a value to current oxygen level
    /// </summary>
    /// <param name="_value"></param>
    public void AddOxygen(float _value)
    {
        Oxygen += _value;
    }

    /// <summary>
    /// Set the player's health regen per second
    /// </summary>
    /// <param name="_value"></param>
    public void SetHealthRegen(float _value)
    {
        healthRegen = _value;
    }

    /// <summary>
    /// Set the multiplier applied when consuming sustenance
    /// </summary>
    /// <param name="_value"></param>
    public void SetSustenanceMultipler(float _value)
    {
        sustenanceMultiplier = _value;
    }

    /// <summary>
    /// Set the time taken for water to decrement
    /// </summary>
    /// <param name="_value"></param>
    public void SetWaterDecayRate(float _value)
    {
        waterDecayRate = _value;
    }

    /// <summary>
    /// Set the time between damage while dehydrating
    /// </summary>
    /// <param name="_value"></param>
    public void SetDehydrationRate(float _value)
    {
        dehydrationDamageRate = _value;
    }

    /// <summary>
    /// Set the damage taken by dehyrdating
    /// </summary>
    /// <param name="_value"></param>
    public void SetDehydrationDamage(float _value)
    {
        dehydrationDamage = _value;
    }

    /// <summary>
    /// Set the time taken for food to decrement
    /// </summary>
    /// <param name="_value"></param>
    public void SetFoodDecayRate(float _value)
    {
        foodDecayRate = _value;
    }

    /// <summary>
    /// Set the time between damage while starving
    /// </summary>
    /// <param name="_value"></param>
    public void SetStarveRate(float _value)
    {
        starveDamageRate = _value;
    }

    /// <summary>
    /// Set the damage taken by starving
    /// </summary>
    /// <param name="_value"></param>
    public void SetStarveDamage(float _value)
    {
        starveDamage = _value;
    }

    /// <summary>
    /// Set whether the player will use oxygen or not
    /// </summary>
    /// <param name="_state"></param>
    public void UseOxygen(bool _state)
    {
        useOxygen = _state;
    }

    /// <summary>
    /// Set the time taken between each damage from suffocation
    /// </summary>
    /// <param name="_value"></param>
    public void SetSuffocateRate(float _value)
    {
        suffocateDamageRate = _value;
    }

    /// <summary>
    /// Set the damage taken by suffocating
    /// </summary>
    /// <param name="_value"></param>
    public void SetSuffocateDamage(float _value)
    {
        suffocateDamage = _value;
    }

    /// <summary>
    /// Set the value of health given when respawned
    /// </summary>
    /// <param name="_value"></param>
    public void SetRespawnHealth(float _value)
    {
        respawnHealth = _value;
    }

    /// <summary>
    /// Set the water level given when respawned
    /// </summary>
    /// <param name="_value"></param>
    public void SetRespawnWater(float _value)
    {
        respawnWater = _value;
    }

    /// <summary>
    /// Set the food level given when respawned
    /// </summary>
    /// <param name="_value"></param>
    public void SetRespawnFood(float _value)
    {
        respawnFood = _value;
    }

    public void SaveData(ref GameData _data)
    {
        _data.playerHealth = Health;
        _data.playerShields = Shield;
        _data.playerWater = Water;
        _data.playerFood = Food;
        _data.playerOxygen = Oxygen;
    }

    public void LoadData(GameData _data)
    {
        //
    }
}
