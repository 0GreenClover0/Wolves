using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;

    private float distanceToKill = 0.75f;

    private void Update()
    {
        if (SheepManager.instance.sheeps.Count == 0)
            return;

        Sheep closestSheep = SheepManager.instance.sheeps.Aggregate((c, d) => Vector3.Distance(c.transform.position, transform.position) < Vector3.Distance(d.transform.position, transform.position) ? c : d);

        Vector3 enemyPos = new Vector3(transform.position.x, 0.0f, transform.position.z);
        Vector3 sheepPos = new Vector3(closestSheep.transform.position.x, 0.0f, closestSheep.transform.position.z);
        Vector3 newPos = Vector3.MoveTowards(enemyPos, sheepPos, Time.deltaTime * speed);
        newPos.y = transform.position.y;
        transform.position = newPos;

        if (Vector3.Distance(enemyPos, sheepPos) < distanceToKill)
            SheepManager.KillSheep(closestSheep);
    }
}
