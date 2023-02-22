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

    /// <summary>
    /// Initialize this projectile
    /// </summary>
    /// <param name="_tag">Name tag to pool this projectile with</param>
    /// <param name="_damage">Damage this projectile deals</param>
    /// <param name="_speed">Initial speed this projectile starts with</param>
    /// <param name="_explosionRadius">Explosive radius of this projectile</param>
    /// <param name="_explosionForce">Explosive force this projectile exerts when exploding</param>
    /// <param name="_lifeTime">The life time of this projectile before exploding</param>
    /// <param name="_onContact">If true, this projectile explode upon contact with another object</param>
    /// <param name="_gravity">If true, this projectile is affected by gravity</param>
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

    /// <summary>
    /// Explode this projectile exerting explosive force and damage to nearby objects
    /// </summary>
    void Explode()
    {
        //Get nearby objects
        Collider[] _colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider _col in _colliders) {
            //Apply damage
            IDamagable _damagle = _col.transform.GetComponent<IDamagable>();
            if (_damagle != null) {
                _damagle.TakeDamage(damage);
            }

            //Apply explosive force
            Rigidbody _rb = _col.GetComponent<Rigidbody>();
            if (_rb != null) {
                _rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        //Pool projectile
        objectPooler.PoolObject(projectileTag, gameObject);
    }
}
