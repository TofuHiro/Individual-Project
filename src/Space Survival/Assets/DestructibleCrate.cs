using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour, IDamagable
{
    public float Health { get { return health; }
        set {
            health -= value;

            if (health <= 0) {
                Die();
            }
        }
    }
    float health;

    [SerializeField] float maxHealth;

    void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(float _value)
    {
        Health = _value;
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }
}
