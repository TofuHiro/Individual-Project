using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    ObjectPooler objectPooler;
    Rigidbody rb;

    string projectileTag;
    float damage, lifeTime, explosionRadius, explosionForce;
    bool explodeOnContact;
    float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > lifeTime) {
            Explode();
        }
    }

    public void Init(string _tag, float _damage, float _speed, float _explosionRadius, float _explosionForce, float _lifeTime, bool _onContact, bool _gravity)
    {
        projectileTag = _tag;
        damage = _damage;
        lifeTime = _lifeTime;
        explosionRadius = _explosionRadius;
        explosionForce = _explosionForce;
        explodeOnContact = _onContact;

        objectPooler = ObjectPooler.Instance;
        rb = GetComponent<Rigidbody>();

        rb.AddForce(transform.forward * _speed, ForceMode.VelocityChange);
        rb.useGravity = _gravity;

        timer = 0f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!explodeOnContact)
            return;

        if (!other.isTrigger) {
            Explode();
        }
    }

    void Explode()
    {
        Collider[] _colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider _col in _colliders) {
            IDamagable _damagle = _col.transform.GetComponent<IDamagable>();
            if (_damagle != null) {
                _damagle.TakeDamage(damage);
            }
            Rigidbody _rb = _col.GetComponent<Rigidbody>();
            if (_rb != null) {
                _rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        objectPooler.PoolObject(projectileTag, gameObject);
    }
}
