using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldBounds : MonoBehaviour
{
    [SerializeField] GameObject[] asteroidProjectilePrefab;
    [SerializeField] float gracePeriod;
    [SerializeField] float attackRate;
    [SerializeField] float asteroidDamage;
    [SerializeField] float asteroidSpeed;
    [SerializeField] float asteroidLifeTime;
    [SerializeField] float spawnDistance;
    [SerializeField] float spawnAngularVel;
    [SerializeField] float explosionRadius;
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
