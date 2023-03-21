using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceGame;

public class EnemySpawner : MonoBehaviour
{
    public static bool EnemiesAllowed = true;
    public static float SpawnNumberMultiplier = 1f;

    public int EnemyNumber { get { return enemyNumber; }
        set {
            enemyNumber = value;

            if (enemyNumber >= maxEnemyActive)
                CanSpawn = false;
            else
                CanSpawn = true;
        }
    }
    int enemyNumber = 0;

    public bool CanSpawn { get; set; } = false;

    [SerializeField] Enemy enemyPrefab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] int maxEnemyActive = 5;
    [SerializeField] float spawnInterval = 5f;

    float timer, nextTimeToSpawn;

    void OnEnable()
    {
        GameManager.OnGameStart += Init;
    }

    void OnDisable()
    {
        GameManager.OnGameStart -= Init;
    }

    void Init()
    {
        maxEnemyActive = (int)(maxEnemyActive * SpawnNumberMultiplier);
        nextTimeToSpawn = spawnInterval;
        CanSpawn = true;
    }

    void Update()
    {
        if (!EnemiesAllowed || !CanSpawn)
            return;

        timer += Time.deltaTime;

        if (timer >= nextTimeToSpawn) {
            Spawn();
        }
    }

    void Spawn()
    {
        Enemy _newEnemy = ObjectPooler.SpawnObject(enemyPrefab.Name, enemyPrefab.gameObject, spawnPoint.position, spawnPoint.rotation).GetComponent<Enemy>();
        _newEnemy.SetSpawner(this);
        EnemyNumber++;
        nextTimeToSpawn = timer + spawnInterval;
    }
}
