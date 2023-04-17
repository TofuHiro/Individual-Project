using UnityEngine;

[CreateAssetMenu(fileName = "New Difficulty", menuName = "Difficulty")]
public class DifficultyModes : ScriptableObject
{
    [Header("Health")]
    [Tooltip("The starting maximum health of the player")]
    public float startingMaxHealth;
    [Tooltip("The health regenerated over time per second")]
    public float healthOverTime;
    [Tooltip("The multiplier applied on the effects of consuming nutritions")]
    public float sustenanceEffectMultiplier;
    [Tooltip("The time between each water level decrement")]
    public float waterDecayRate;
    [Tooltip("The time between each damage taken by dehydration")]
    public float dehydrationDamageRate;
    [Tooltip("The damage taken when dehydrating")]
    public float dehydrationDamage;
    [Tooltip("The time between each food level decrement")]
    public float foodDecayRate;
    [Tooltip("The time between each damage taken by starvation")]
    public float starveDamageRate;
    [Tooltip("The damage taken when starving")]
    public float starveDamage;
    [Tooltip("Whether the player's oxygen is drained")]
    public bool useOxygen;
    [Tooltip("The maximum starting oxygen level of the player")]
    public float startingMaxOxygen;
    [Tooltip("The time between each damage taken by suffocation")]
    public float suffocateDamageRate;
    [Tooltip("The damage taken when suffocating")]
    public float suffocationDamage;
    /*[Tooltip("Whether the player has one life or not.")]
    public bool oneLife;*/
    [Tooltip("The health given to the player when respawned")]
    public float respawnHealth;
    [Tooltip("The water level given to the player when respawned")]
    public float respawnWater;
    [Tooltip("The food level given to the player when respawned")]
    public float respawnFood;

    [Header("Enemies")]
    [Tooltip("To use enemies or not")]
    public bool enemies;
    [Tooltip("The multiplier applied to the number of enemies used")]
    public float enemySpawnMultiplier;
    [Tooltip("The multiplier applied to enemy's health")]
    public float enemyHealthMultiplier;
    [Tooltip("The multiplier applied to enemy's attacks")]
    public float enemyDamageMultiplier;

    [Header("Settings")]
    [Tooltip("Whether building structure requires any ingredients")]
    public bool buildingRequirements;
    [Tooltip("Whether crafting requires any ingredients")]
    public bool craftingRequirements;
    [Tooltip("Whether the player drops all their items in their main inventory on death")]
    public bool dropItemsOnDeath;
    [Tooltip("Whether the player drops all their weapons on death")]
    public bool dropWeaponsOnDeath;
    [Tooltip("Whether the player drops all their upgrades on death")]
    public bool dropUpgradesOnDeath;
    [Tooltip("Whether the player drops all their armour on death")]
    public bool dropArmoursOnDeath;
}
