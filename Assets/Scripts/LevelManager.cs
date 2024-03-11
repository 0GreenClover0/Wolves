using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Animator transitionAnimator;

    private List<EnemySpawnsManager> enemySpawnsManagers = new List<EnemySpawnsManager>();
    private Player player;
    private Vector3 playerStartPosition;
    private int currentLevel = 0;

    private void Awake()
    {
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
        transitionAnimator.SetTrigger("Transition");

        yield return new WaitForSeconds(1.0f);

        player.transform.position = playerStartPosition;
        player.DestroyAllPoles();

        if (level > 0)
        {
            enemySpawnsManagers[level - 1].gameObject.SetActive(false);
            enemySpawnsManagers[level - 1].OnDone -= OnLevelDone;
        }

        enemySpawnsManagers[level].gameObject.SetActive(true);
        enemySpawnsManagers[level].OnDone += OnLevelDone;

        currentLevel = level;
    }
}
