using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPoint : MonoBehaviour
{
    public static ExitPoint instance;
    public List<Transform> exitPoints = new List<Transform>();
    public void Awake()
    {
        instance = this;
    }

    public Vector3 getClosestExitPoint(Vector3 position)
    {
        // finding the closest exit point
        Transform closestExitPoint = exitPoints[0];
        float shortestDistance = Vector3.Distance(position, closestExitPoint.position);
        for (int i = 1; i < exitPoints.Count; i++)
        {
            float distanceToExitPoint = Vector3.Distance(position, exitPoints[i].position);
            if (distanceToExitPoint < shortestDistance)
            {
                closestExitPoint = exitPoints[i];
                shortestDistance = distanceToExitPoint;
            }
        }
        return closestExitPoint.position;
    }

}
