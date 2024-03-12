using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float runSpeed = 20.0f;
    public float distanceToLinePickup = 5.0f;
    public int maxPolesCount = 2;
    public float maxWireLength = 5.0f;

    private float usedWireLength = 0.0f;

    [Header("References")]
    public GameObject lineStart;

    private GameObject currentLineStart;
    private List<GameObject> poles = new List<GameObject>();

    private List<Pole> disattachedStartingPoles = new List<Pole>();

    private List<Renderer> poleRenderers = new List<Renderer>();
    private LineRenderer lineRenderer;
    private MaterialPropertyBlock lineMaterialPropertyBlock;
    private new Rigidbody rigidbody;
    private float horizontal;
    private float vertical;

    private bool HasLine => poles.Count > 0;

    private LayerMask enemyLayer;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>(); 

        enemyLayer = LayerMask.GetMask("Enemy");

        lineMaterialPropertyBlock = new MaterialPropertyBlock();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void FixedUpdate()
    {
        ClampPlayerDistanceFromPole();

        for (int i = 0; i < disattachedStartingPoles.Count; ++i)
        {
            Pole pole = disattachedStartingPoles[i];
            List<Vector3> disattachedLineVertices = new List<Vector3>();
            while (pole != null)
            {
                disattachedLineVertices.Add(pole.transform.position);

                if (pole.nextPole == null)
                {
                    pole = null;
                    break;
                }

                pole = pole.nextPole;
            }

            disattachedStartingPoles[i].lineRenderer.positionCount = disattachedLineVertices.Count;
            disattachedStartingPoles[i].lineRenderer.SetPositions(disattachedLineVertices.ToArray());
        }

        for (int i = 0; i < disattachedStartingPoles.Count; i++)
        {
            Pole pole = disattachedStartingPoles[i];
            while (pole != null)
            {
                if (pole.nextPole == null)
                    break;

                Ray ray = new Ray(pole.nextPole.transform.position, pole.transform.position - pole.nextPole.transform.position);
                if (Physics.Raycast(ray, out RaycastHit info, Vector3.Distance(pole.transform.position, pole.nextPole.transform.position), enemyLayer, QueryTriggerInteraction.Collide))
                {
                    Destroy(info.collider.gameObject);
                    DestroyFreestandingPoles(disattachedStartingPoles[i]);
                }

                pole = pole.nextPole;
            }
        }

        if (HasLine)
        {
            Vector3[] lineVertices = new Vector3[poles.Count+1];
            for (int i = 0; i < poles.Count; i++)
            {
                lineVertices[i] = poles[i].transform.position;
            }

            lineVertices[poles.Count] = transform.position;
            lineRenderer.positionCount = poles.Count + 1;
            lineRenderer.SetPositions(lineVertices);

            for (int i = 0; i < poles.Count - 1; i++)
            {
                Ray ray = new Ray(poles[i+1].transform.position, poles[i].transform.position - poles[i + 1].transform.position);
                if (Physics.Raycast(ray, out RaycastHit info, Vector3.Distance(poles[i].transform.position, poles[i+1].transform.position), enemyLayer, QueryTriggerInteraction.Collide))
                {
                    Destroy(info.collider.gameObject);
                }
            }
        }
    }

    private void ClampPlayerDistanceFromPole()
    {
        CalculateUsedWireLength();

        if (usedWireLength < maxWireLength)
        {
            rigidbody.velocity = new Vector3(horizontal * runSpeed, 0.0f, vertical * runSpeed);
        }
        else
        {
            Vector3 desiredVelocity = new Vector3(horizontal * runSpeed, 0.0f, vertical * runSpeed);
            Vector3 lastPolePosition = poles[poles.Count - 1].transform.position;
            Vector3 directionTowardsLastPole = lastPolePosition - transform.position;

            if (Mathf.Sign(desiredVelocity.x) != Mathf.Sign(directionTowardsLastPole.x))
            {
                desiredVelocity.x = 0.0f;
            }

            if (Mathf.Sign(desiredVelocity.z) != Mathf.Sign(directionTowardsLastPole.z))
            {
                desiredVelocity.z = 0.0f;
            }

            rigidbody.velocity = desiredVelocity;
        }
    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);

        if (HasLine)
        {
            Vector2 linePos = new Vector2(poles[poles.Count-1].transform.position.x, poles[poles.Count - 1].transform.position.z);
            Vector2 playerPos = new Vector2(transform.position.x, transform.position.z);
            if (Vector2.Distance(linePos, playerPos) > distanceToLinePickup)
            {
                lineMaterialPropertyBlock.SetColor("_Color", Color.white);
            }
            else
            {
                lineMaterialPropertyBlock.SetColor("_Color", Color.blue);
            }

            poleRenderers[poleRenderers.Count-1].SetPropertyBlock(lineMaterialPropertyBlock);

            // Cut
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (poles.Count < 1)
                    return;

                if (poles.Count == 1)
                {
                    SpawnNextPole();
                }

                lineRenderer.positionCount = 1;

                Pole firstPole = poles[0].GetComponent<Pole>();
                firstPole.lineRenderer.enabled = true;
                disattachedStartingPoles.Add(firstPole);

                poles.Clear();
                poleRenderers.Clear();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (HasLine)
            {
                Vector2 linePos = new Vector2(poles[poles.Count - 1].transform.position.x, poles[poles.Count-1].transform.position.z);
                Vector2 playerPos = new Vector2(transform.position.x, transform.position.z);

                if (Vector2.Distance(linePos, playerPos) > distanceToLinePickup)
                {
                    if (poles.Count >= maxPolesCount)
                        return;

                    SpawnNextPole();
                    return;
                }

                DestroyLastPole();
            }
            else
            {
                poles.Add(Instantiate(lineStart, transform.position, Quaternion.identity));
                poleRenderers.Add(poles[poles.Count - 1].GetComponent<Renderer>());
            }
        }
    }

    private void SpawnNextPole()
    {
        Pole previousPole = poles[poles.Count - 1].GetComponent<Pole>();
        poles.Add(Instantiate(lineStart, transform.position, Quaternion.identity));
        previousPole.nextPole = poles[poles.Count - 1].GetComponent<Pole>();
        poleRenderers.Add(poles[poles.Count - 1].GetComponent<Renderer>());
    }

    private void DestroyFreestandingPoles(Pole pole)
    {
        disattachedStartingPoles.Remove(pole);

        while (pole != null)
        {
            Pole nextPole = pole.nextPole;
            Destroy(pole.gameObject);
            pole = nextPole;
        }
    }

    public void DestroyAllPoles()
    {
        lineRenderer.positionCount = 1;

        for (int i = poles.Count - 1; i >= 0; --i)
        {
            Destroy(poles[i]);
        }

        poles.Clear();
        poleRenderers.Clear();
    }

    private void DestroyLastPole()
    {
        lineRenderer.positionCount = poles.Count;
        Destroy(poles[poles.Count - 1]);
        poles.RemoveAt(poles.Count - 1);
        poleRenderers.RemoveAt(poleRenderers.Count - 1);
    }

    private void CalculateUsedWireLength(){
        usedWireLength = 0.0f;
        for(int i = 0; i < poles.Count; i++)
        {
            if (i == poles.Count - 1)
            {
                usedWireLength += Vector3.Distance(poles[i].transform.position, transform.position);
            }
            else
            {
                usedWireLength += Vector3.Distance(poles[i].transform.position, poles[i + 1].transform.position);
            }
        }
    }
}
