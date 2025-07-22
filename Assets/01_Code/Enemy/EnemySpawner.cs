using UnityEngine;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // Array von Prefabs
    public Transform[] spawnPoints;
    public int minEnemies = 1;
    public int maxEnemies = 5;

    void Awake()
    {
        spawnPoints = GetComponentsInChildren<Transform>().Where(t => t != transform).ToArray();
    }

    void Start()
    {
        SpawnEnemies();
    }

    private void OnEnable()
    {
        EnemyHealth.OnEnemyKilled += SpawnEnemies;
    }

    private void SpawnEnemies()
    {
        if (spawnPoints.Length >= 0 && enemyPrefabs.Length >= 0)
        {
            int enemyCount = Random.Range(minEnemies, maxEnemies + 1);

            for (int i = 0; i < enemyCount; i++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

                Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            }
        }
    }


}