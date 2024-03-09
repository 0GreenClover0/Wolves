using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sheep : MonoBehaviour
{
    public float maxDistance = 3f;
    public float minWaitTime = 0.5f;
    public float maxWaitTime = 5f;
    public float centerThreshold = 10f; // Distance threshold from the center to trigger going back
    private NavMeshAgent agent;
    private Vector3 centerPosition;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        centerPosition = new Vector3(0, 1, 0);
        StartCoroutine(MoveToRandomLocation());
    }

    // Update is called once per frame
    void Update()
    {
        // Ensure that the sheep stays at a constant height
        Vector3 position = transform.position;
        position.y = 1;
        transform.position = position;

        // Check distance from center
        if (Vector3.Distance(transform.position, centerPosition) > centerThreshold)
        {
            GoBackToCenter();
        }
    }

    private IEnumerator MoveToRandomLocation()
    {
        while (true)
        {
            // Calculate random direction within maxDistance
            Vector3 randomDirection = Random.insideUnitCircle.normalized * Random.Range(0, maxDistance);
            Vector3 randomDestination = transform.position + randomDirection;
            randomDestination.z = randomDestination.y;
            randomDestination.y = 1;

            // Check if the random destination is on the NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDestination, out hit, maxDistance, NavMesh.AllAreas))
            {
                // Set the destination for the NavMeshAgent
                agent.SetDestination(hit.position);

                // Wait for the agent to reach the destination
                yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance < 0.1f);

                // Wait for a random duration before selecting a new destination
                yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            }
            else
            {
                // Retry with a new random destination if the current one is not on the NavMesh
                yield return null;
            }
        }
    }

    private void GoBackToCenter()
    {
        // Set destination to the center position
        Vector2 v = Random.insideUnitCircle.normalized;
        Vector3 v3 = centerPosition + new Vector3(v.x, 1, v.y);
        agent.SetDestination(v3);
    }
}
