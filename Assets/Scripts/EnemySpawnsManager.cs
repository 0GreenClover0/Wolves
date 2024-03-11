using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemySpawnsManager : MonoBehaviour
{
    public event Action OnDone;

    [SerializeField] private List<Spawner> spawners = new List<Spawner>();

    [HideInInspector] public bool isDone = false;

    private List<EnemySpawner> enemySpawners = new List<EnemySpawner>();

    private int randomOffset = 0;

    private void Awake()
    {
        enemySpawners = GetComponentsInChildren<EnemySpawner>(true).ToList();

        Assert.AreEqual(enemySpawners.Count, spawners.Count);

        randomOffset = UnityEngine.Random.Range(0, 4);
    }

    private void Start()
    {
        for (int i = 0; i < enemySpawners.Count; ++i)
        {
            enemySpawners[i].spawnTimes = spawners[(i + randomOffset) % spawners.Count].spawnTimes;
        }
    }

    private void Update()
    {
        if (isDone)
            return;

        foreach (var enemySpawner in enemySpawners)
        {
            if (!enemySpawner.isDone)
                return;

            if (enemySpawner.enemies.FirstOrDefault(x => x != null) != null)
                return;
        }

        isDone = true;

        OnDone?.Invoke();
    }
}

[Serializable]
public class Spawner
{
    public List<float> spawnTimes = new List<float>();
}
