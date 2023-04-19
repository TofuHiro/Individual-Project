using UnityEngine;
using SpaceGame;

public class EnemySpawner : MonoBehaviour
{
    /// <summary>
    /// If enemies are allowed to spawn
    /// </summary>
    public static bool EnemiesAllowed = true;
    /// <summary>
    /// The multiplier applied to the maximum number of enemies allowed to spawn per spawner
    /// </summary>
    public static float SpawnNumberMultiplier = 1f;

    /// <summary>
    /// The number of active enemies the spawner currently has
    /// </summary>
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

    /// <summary>
    /// If this spawner can currently spawn enemies
    /// </summary>
    public bool CanSpawn { get; private set; } = false;

    [Tooltip("The enemy to spawn")]
    [SerializeField] Enemy enemyPrefab;
    [Tooltip("The position to spawn the enemy at")]
    [SerializeField] Transform spawnPoint;
    [Tooltip("The maximum possible number of enemies that are currently active from this spawner")]
    [SerializeField] int maxEnemyActive = 5;
    [Tooltip("The time interval between each spawn")]
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

    void OnDestroy()
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

    /// <summary>
    /// Spawns the set enemy 
    /// </summary>
    void Spawn()
    {
        Enemy _newEnemy = ObjectPooler.SpawnObject(enemyPrefab.Name, enemyPrefab.gameObject, spawnPoint.position, spawnPoint.rotation).GetComponent<Enemy>();
        _newEnemy.SetSpawner(this);
        EnemyNumber++;
        nextTimeToSpawn = timer + spawnInterval;
    }
}
