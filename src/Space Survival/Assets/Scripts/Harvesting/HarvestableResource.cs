using UnityEngine;

//Class for resource spawn chances on harvest
[System.Serializable]
public class ObjectChance
{
    public GameObject resource;
    [Range(.01f, 100f)]
    public float spawnChance;
}
public class HarvestableResource : MonoBehaviour, IHarvestable
{
    public float Health { get { return health; }
        private set {
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
    [SerializeField] ObjectChance[] drops;

    void Start()
    {
        health = maxHealth;
    }

    /// <summary>
    /// Applies damage to the resource node depending on the tool used.
    /// </summary>
    /// <param name="_value">Damage to apply</param>
    /// <param name="_harvestType">The tool type used</param>
    /// <param name="_tier">The tier of the tool used</param>
    public void TakeDamage(float _damage, HarvestTypes _toolType, int _toolTier)
    {
        if (_toolType == harvestType && _toolTier >= minTier) {
            Health -= _damage;
        }
    }

    /// <summary>
    /// Spawns resources upon death
    /// </summary>
    public void Die()
    {
        //Roll for chance out of 100%
        float _chance = Random.Range(.01f, 100f);

        int _spawnNo = 0;

        //Spawns all items within chance
        foreach (ObjectChance _drop in drops) {
            if (_chance <= _drop.spawnChance) {
                //For single item spawns
                if (_spawnNo == 0) {
                    ObjectPooler.SpawnObject(
                    _drop.resource.name,
                    _drop.resource,
                    transform.position,
                    transform.rotation,
                    transform.localScale);
                }
                //For multiple item spawns
                else {
                    ObjectPooler.SpawnObject(
                    _drop.resource.name,
                    _drop.resource,
                    //Spreaded spawn 
                    transform.position + new Vector3(
                        Random.Range(-transform.localScale.x / 1.2f, transform.localScale.x / 1.2f),
                        Random.Range(-transform.localScale.y / 1.2f, transform.localScale.y / 1.2f),
                        Random.Range(-transform.localScale.z / 1.2f, transform.localScale.z / 1.2f)),
                    transform.rotation,
                    transform.localScale);
                }

                _spawnNo++;
            }
        }

        gameObject.SetActive(false);
    }
}
