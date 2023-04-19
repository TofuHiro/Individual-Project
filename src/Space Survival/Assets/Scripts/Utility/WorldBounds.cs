using UnityEngine;

public class WorldBounds : MonoBehaviour
{
    [Tooltip("Asteroid objects to spawn as hazards")]
    [SerializeField] GameObject[] asteroidProjectilePrefab;
    [Tooltip("The time delay before starting to attack")]
    [SerializeField] float gracePeriod;
    [Tooltip("The time delay before attacks")]
    [SerializeField] float attackRate;
    [Tooltip("The damage each asteroid deals")]
    [SerializeField] float asteroidDamage;
    [Tooltip("The initial speed the asteroid start with")]
    [SerializeField] float asteroidSpeed;
    [Tooltip("The maximum time the asteroid stay active")]
    [SerializeField] float asteroidLifeTime;
    [Tooltip("The distance asteroids are spawned from the player")]
    [SerializeField] float spawnDistance;
    [Tooltip("The angular velocity applied to asteroids on spawn")]
    [SerializeField] float spawnAngularVel;
    [Tooltip("The damaging radius of asteroids upon impact")]
    [SerializeField] float explosionRadius;
    [Tooltip("The UI object displaying a warning for hazards")]
    [SerializeField] GameObject warningUI;

    PlayerController player;

    bool playerInBounds = true, attackFromTop;
    float timer, nextTimeToAttack;

    void Start()
    {
        player = PlayerController.Instance;
    }

    void Update()
    {
        if (playerInBounds)
            return;

        timer += Time.deltaTime;

        if (timer >= nextTimeToAttack) {
            if (attackFromTop)
                AttackPlayerFromTop();
            else
                AttackPlayerFromBot();

            attackFromTop = !attackFromTop;
            nextTimeToAttack = timer + attackRate;
        }
    }

    void AttackPlayerFromTop()
    {
        Projectile _newProjectile = ObjectPooler.SpawnObject("Boundary Asteroid", asteroidProjectilePrefab[Random.Range(0, asteroidProjectilePrefab.Length)], player.GetPlayerPosition() + (Vector3.up * spawnDistance), Quaternion.identity).GetComponent<Projectile>();
        _newProjectile.Init("Boundary Asteroid", asteroidDamage, 0f, explosionRadius, 1f, asteroidLifeTime, true, false, Vector3.forward * spawnAngularVel);
        _newProjectile.GetComponent<Rigidbody>().AddForce(-Vector3.up * asteroidSpeed, ForceMode.VelocityChange);
    }

    void AttackPlayerFromBot()
    {
        Projectile _newProjectile = ObjectPooler.SpawnObject("Boundary Asteroid", asteroidProjectilePrefab[Random.Range(0, asteroidProjectilePrefab.Length)], player.GetPlayerPosition() - (Vector3.up * spawnDistance), Quaternion.identity).GetComponent<Projectile>();
        _newProjectile.Init("Boundary Asteroid", asteroidDamage, 0f, explosionRadius, 1f, asteroidLifeTime, true, false, Vector3.forward * spawnAngularVel);
        _newProjectile.GetComponent<Rigidbody>().AddForce(Vector3.up * asteroidSpeed, ForceMode.VelocityChange);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) {
            playerInBounds = false;
            nextTimeToAttack = gracePeriod;
            timer = 0f;
            warningUI.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            playerInBounds = true;
            warningUI.SetActive(false);
        }
    }
}
