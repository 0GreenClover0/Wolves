using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    [HideInInspector] public List<GameObject> enemies = new List<GameObject>();
    [HideInInspector] public List<float> spawnTimes = new List<float>();
    [HideInInspector] public bool isDone = false;

    private float currentTime = 0.0f;
    private int lastSpawned = 0;

    private void Update()
    {
        if (lastSpawned >= spawnTimes.Count)
        {
            isDone = true;
            return;
        }

        currentTime += Time.deltaTime;

        if (currentTime < spawnTimes[lastSpawned])
            return;

        Spawn();
    }

    private void Spawn()
    {
        enemies.Add(Instantiate(enemyPrefab, transform.position, Quaternion.identity));
        lastSpawned++;
    }
}
