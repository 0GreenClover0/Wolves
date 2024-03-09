using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float distanceToKill = 1.5f;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (SheepManager.instance.sheeps.Count == 0)
            return;

        Sheep closestSheep = SheepManager.instance.sheeps[0];
        float shortestDistance = Vector3.Distance(transform.position, closestSheep.transform.position);

        // Find the closest sheep
        for (int i = 1; i < SheepManager.instance.sheeps.Count; i++)
        {
            float distanceToSheep = Vector3.Distance(transform.position, SheepManager.instance.sheeps[i].transform.position);
            if (distanceToSheep < shortestDistance)
            {
                closestSheep = SheepManager.instance.sheeps[i];
                shortestDistance = distanceToSheep;
            }
        }

        // Move towards the closest sheep using NavMesh
        Vector3 targetPosition = new Vector3(closestSheep.transform.position.x, transform.position.y, closestSheep.transform.position.z);
        agent.SetDestination(targetPosition);

        // Check if the enemy is close enough to the sheep to kill it
        if (shortestDistance < distanceToKill)
            SheepManager.KillSheep(closestSheep);
    }
}
