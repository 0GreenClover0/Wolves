using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public float delay = 0.0f;

    [Header("Spawn Time")]
    [SerializeField]
    private float timeToSpawnLow = 15f;
    [SerializeField]
    private float timeToSpawnHigh = 30f;

    private static int enemyCount = 0;

    private float currentTime = 0.0f;

    private float timeToSpawn = 0.0f;

    private void Awake()
    {
        timeToSpawn = Random.Range(timeToSpawnLow, timeToSpawnHigh);
    }

    private void Update()
    {
        currentTime += Time.deltaTime;

        if (delay > 0.0f)
        {
            if (currentTime > delay)
            {
                delay = 0.0f;
                Spawn();
            }

            return;
        }

        if (currentTime < timeToSpawn)
            return;

        Spawn();
    }

    private void Spawn()
    {
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        currentTime = 0.0f;
        timeToSpawn = Random.Range(timeToSpawnLow, timeToSpawnHigh);
    }
}
