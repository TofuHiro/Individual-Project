using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestableResource : MonoBehaviour, IDamagable, IHarvestable
{
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

    [SerializeField] float maxHealth = 100f;
    [SerializeField] HarvestTiers minTier;
    [SerializeField] HarvestTypes harvestType;
    [SerializeField] ResourceDrop[] drops;

    ObjectPooler objectPooler;

    void Start()
    {
        health = maxHealth;
        objectPooler = ObjectPooler.Instance;
    }

    public void TakeDamage(float _value) { return; }

    public void TakeDamage(float _value, HarvestTypes _harvestType, HarvestTiers _tier)
    {
        if (_harvestType == harvestType && _tier >= minTier) {
            Health -= _value;
        }
    }

    public void Die()
    {
        float _chance = Random.Range(.01f, 100f);

        foreach (ResourceDrop _drop in drops) {
            if (_chance <= _drop.spawnChance) {
                objectPooler.SpawnObject(_drop.resource.name, _drop.resource, transform.position, transform.rotation);
            }
        }

        gameObject.SetActive(false);
    }
}
