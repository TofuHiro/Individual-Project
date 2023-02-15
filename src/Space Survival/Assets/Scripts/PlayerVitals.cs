using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VitalsUI))]
public class PlayerVitals : MonoBehaviour, IDamagable
{
    [Header("Shield")]
    [SerializeField] float maxShield = 100f;

    [Header("Health")]
    [SerializeField] float maxHealth = 100f;

    [Header("Water")]
    [SerializeField] float maxWater = 100f;
    [SerializeField] float waterDecayRate = 15f;
    [SerializeField] float parchedDamageRate = 1f;
    [SerializeField] float parchDamage = 1f;
    bool isParched;
    float nextParchTick, nextWaterTick;

    [Header("Food")]
    [SerializeField] float maxFood = 100f;
    [SerializeField] float foodDecayRate = 10f;
    [SerializeField] float starveDamageRate = 2f;
    [SerializeField] float starveDamage = 1f;
    bool isStarving;
    float nextStarveTick, nextFoodTick;

    [Header("Oxygen")]
    [SerializeField] float baseOxygenTime = 60f;
    [SerializeField] float suffocateDamageRate = .5f;
    [SerializeField] float suffocateDamage = 10f;
    [SerializeField] float oxygenRecoverRate = 10f;
    bool isSuffocating, inAir;
    float nextSuffocateTick, currentMaxOxygenTime;

    VitalsUI UI;
    float timer;

    public float Shield { get { return shield; }
        set {
            shield = value;
            shield = Mathf.Clamp(shield, 0f, maxShield);
            UI.SetShield(shield);

            if (shield <= 0f) {
                Health -= value;
            }
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
        UI = GetComponent<VitalsUI>();

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
        timer += Time.deltaTime;

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

    public void TakeDamage(float _value)
    {
        Shield -= _value;
    }

    public void Die()
    {
        //Die
    }

    public void AddShield(float _value)
    {
        Shield += _value;
    }

    public void AddHealth(float _value)
    {
        Health += _value;
    }

    public void AddWater(float _value)
    {
        Water += _value;
    }

    public void AddFood(float _value)
    {
        Food += _value;
    }

    public void AddOxygen(float _value)
    {
        Oxygen += _value;
    }

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
