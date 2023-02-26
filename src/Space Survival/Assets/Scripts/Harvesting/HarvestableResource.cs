using UnityEngine;

public class HarvestableResource : MonoBehaviour, IDamagable, IHarvestable
{
    //Class for resource spawn chances on harvest
    [System.Serializable]
    [SerializeField] class ResourceDrop
    {
        public GameObject resource;
        public float spawnChance;
    }

    public float Health { get { return health; }
        set {
            health = value;

            if (health <= 0f) {
                Die();
            }
        } 
    }
    float health;

    [Tooltip("Starting health of resource")]
    [SerializeField] float maxHealth = 100f;
    [Tooltip("The minimum tool tier required to be harvested")]
    [SerializeField] int minTier;
    [Tooltip("The tool type required to be harvested")]
    [SerializeField] HarvestTypes harvestType;
    [Tooltip("Array of item to drop with specified spawn rates")]
    [SerializeField] ResourceDrop[] drops;

    void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(float _value) { return; }

    /// <summary>
    /// Applies damage to the resource node depending on the tool used.
    /// </summary>
    /// <param name="_value">Damage to apply</param>
    /// <param name="_harvestType">The tool type used</param>
    /// <param name="_tier">The tier of the tool used</param>
    public void TakeDamage(float _value, HarvestTypes _harvestType, int _tier)
    {
        if (_harvestType == harvestType && _tier >= minTier) {
            Health -= _value;
        }
    }

    /// <summary>
    /// Spawns resources upon death
    /// </summary>
    public void Die()
    {
        //Roll for chance out of 100%
        float _chance = Random.Range(.01f, 100f);

        //Spawns all items within chance
        foreach (ResourceDrop _drop in drops) {
            if (_chance <= _drop.spawnChance) {
                ObjectPooler.SpawnObject(_drop.resource.name, _drop.resource, transform.position, transform.rotation, transform.localScale);
            }
        }

        gameObject.SetActive(false);
    }
}
