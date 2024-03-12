using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float distanceToKill = 1.5f;
    public float distanceToGrab = 1.5f;

    private bool isHoldingSheep = false;
    private NavMeshAgent agent;
    private Sheep stolenSheep;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (SheepManager.instance.sheeps.Count == 0)
            return;

        if(isHoldingSheep)
        {
            // Move towards the end using NavMesh
            stolenSheep.transform.position = new Vector3(transform.position.x, ExitPoint.instance.transform.position.y, transform.position.z);
            if(Vector3.Distance(transform.position, ExitPoint.instance.transform.position) < distanceToKill)
            {
                SheepManager.KillSheep(stolenSheep);
                stolenSheep = null;
                isHoldingSheep = false;
            }
            return;
        }
        else
        {
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
            if (shortestDistance < distanceToGrab)
            {
                isHoldingSheep = true;
                stolenSheep = closestSheep;
                stolenSheep.agent.enabled = false;
                agent.SetDestination(new Vector3(ExitPoint.instance.transform.position.x, transform.position.y, ExitPoint.instance.transform.position.z));
                
            }
        }
    }
}
