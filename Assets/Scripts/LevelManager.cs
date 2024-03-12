using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [NonSerialized] public bool levelIsRunning = false;

    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private Animator transitionAnimator;

    private List<EnemySpawnsManager> enemySpawnsManagers = new List<EnemySpawnsManager>();
    private Player player;
    private Vector3 playerStartPosition;
    private int currentLevel = 0;

    private void Awake()
    {
        instance = this;
        enemySpawnsManagers = GetComponentsInChildren<EnemySpawnsManager>(true).ToList();
        player = FindAnyObjectByType<Player>();
        playerStartPosition = player.transform.position;
    }

    private void Start()
    {
        StartCoroutine(StartLevel(0));
    }

    private void OnLevelDone()
    {
        if (currentLevel >= enemySpawnsManagers.Count)
            return;

        StartCoroutine(StartLevel(currentLevel + 1));
    }

    private IEnumerator StartLevel(int level)
    {
        levelIsRunning = false;
        transitionAnimator.SetTrigger("Transition");

        yield return new WaitForSeconds(1.0f);

        player.transform.position = playerStartPosition + new Vector3(0.0f, 500.0f, 0.0f);
        player.DestroyAllPoles();

        SheepManager.RemoveSheeps();

        if (level > 0)
        {
            enemySpawnsManagers[level - 1].gameObject.SetActive(false);
            enemySpawnsManagers[level - 1].OnDone -= OnLevelDone;
        }

        enemySpawnsManagers[level].gameObject.SetActive(true);
        enemySpawnsManagers[level].OnDone += OnLevelDone;

        SheepManager.instance.sheeps = enemySpawnsManagers[level].Sheeps;

        navMeshSurface.BuildNavMesh();

        player.transform.position = playerStartPosition;

        currentLevel = level;
        levelIsRunning = true;
    }
}
