using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    public float maxDistance = 3f;
    public float moveSpeed = 1f;

    private float currentTime = 0;
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindAnyObjectByType<Player>();
        StartCoroutine(MoveToRandomLocation());
    }

    private IEnumerator MoveToRandomLocation()
    {
        while (true)
        {
            float newTime = Random.Range(5f, 8f);

            Vector3 v0 = Vector3.zero;
            v0.y = 1;

            Vector2 v = Random.insideUnitCircle * Random.Range(0, maxDistance);
            Vector3 randomDirection = new Vector3(v.x, 1, v.y);
            Vector3 targetPosition;

            if (Vector2.Distance(v0, transform.position) > 6)
                 targetPosition = transform.position + randomDirection;
            else
                 targetPosition = randomDirection;

            targetPosition.y = 1;

            while (currentTime < newTime)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
                currentTime += Time.deltaTime;
            }

            currentTime = 0;
        }
    }
}
