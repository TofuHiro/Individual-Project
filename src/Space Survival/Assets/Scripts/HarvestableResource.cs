using UnityEngine;

//Class for resource spawn chances on harvest
[System.Serializable]
public class ObjectChance
{
    public string nameTag;
    public GameObject resource;
    [Range(.01f, 100f)]
    public float spawnChance;
}
public class HarvestableResource : MonoBehaviour, IHarvestable, IDamagable
{
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
    [Tooltip("Destroys this object when harvested")]
    [SerializeField] bool destroyOnHarvest = true;
    [Tooltip("Array of item to drop with specified spawn rates")]
    [SerializeField] ObjectChance[] drops;

    bool isDead = false;

    void Start()
    {
        health = maxHealth;
    }

    /// <summary>
    /// Applies damage to the resource node depending on the tool used.
    /// </summary>
    /// <param name="_damage">Damage to apply</param>
    /// <param name="_toolType">The tool type used</param>
    /// <param name="_toolTier">The tier of the tool used</param>
    public void TakeDamage(float _damage, HarvestTypes _toolType, int _toolTier)
    {
        if (isDead)
            return;

        if (_toolType == harvestType && _toolTier >= minTier) {
            Health -= _damage;
        }
    }

    public void TakeDamage(float _damage)
    {
        if (harvestType == HarvestTypes.Any) {
            Health -= _damage;
        }
    }

    /// <summary>
    /// Spawns resources upon death
    /// </summary>
    public void Die()
    {
        int _spawnNo = 0;

        //Spawns all items within chance
        foreach (ObjectChance _drop in drops) {
            //Roll for chance out of 100%
            float _chance = Random.Range(.01f, 100f);
            if (_chance <= _drop.spawnChance) {
                //For single item spawns
                if (_spawnNo == 0) {
                    ObjectPooler.SpawnObject(
                    _drop.nameTag,
                    _drop.resource,
                    transform.position,
                    transform.rotation,
                    _drop.resource.transform.localScale);
                }
                //For multiple item spawns
                else {
                    ObjectPooler.SpawnObject(
                    _drop.nameTag,
                    _drop.resource,
                    //Spreaded spawn 
                    transform.position + new Vector3(
                        Random.Range(-transform.localScale.x / 1.2f, transform.localScale.x / 1.2f),
                        Random.Range(-transform.localScale.y / 1.2f, transform.localScale.y / 1.2f),
                        Random.Range(-transform.localScale.z / 1.2f, transform.localScale.z / 1.2f)),
                    transform.rotation,
                     _drop.resource.transform.localScale);
                }

                _spawnNo++;
            }
        }

        isDead = true;

        if (destroyOnHarvest) {
            gameObject.SetActive(false);
        }
    }
}
