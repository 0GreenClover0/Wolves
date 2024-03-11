using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float runSpeed = 20.0f;
    public float distanceToLinePickup = 5.0f;
    public int maxPolesCount = 1;

    [Header("References")]
    public GameObject lineStart;

    private GameObject currentLineStart;
    private List<GameObject> poles; // <---- polaki biedaki, specjalnie z malej

    private List<Renderer> poleRenderers;
    private LineRenderer lineRenderer;
    private MaterialPropertyBlock lineMaterialPropertyBlock;
    private new Rigidbody rigidbody;
    private float horizontal;
    private float vertical;

    private bool HasLine => poles.Count > 0;

    private LayerMask enemyLayer;

    private void Start ()
    {
        rigidbody = GetComponent<Rigidbody>(); 

        enemyLayer = LayerMask.GetMask("Enemy");

        lineMaterialPropertyBlock = new MaterialPropertyBlock();
        lineRenderer = GetComponent<LineRenderer>();
        poles = new List<GameObject>();
        poleRenderers = new List<Renderer>();
    }

    private void FixedUpdate()
    {
        rigidbody.velocity = new Vector3(horizontal * runSpeed, 0.0f, vertical * runSpeed);

        if (HasLine)
        {
            Vector3[] lineVertices = new Vector3[poles.Count+1];
            for(int i = 0; i < poles.Count; i++)
            {
                lineVertices[i] = poles[i].transform.position;
            }
            lineVertices[poles.Count] = transform.position;
            lineRenderer.positionCount = poles.Count + 1;
            lineRenderer.SetPositions(lineVertices);
            for(int i = 0; i < poles.Count; i++)
            {
                if (i == poles.Count - 1)
                {
                    Ray ray = new Ray(transform.position, poles[i].transform.position - this.transform.position);
                    if (Physics.Raycast(ray, out RaycastHit info, Vector3.Distance(poles[i].transform.position, transform.position), enemyLayer, QueryTriggerInteraction.Collide))
                    {
                        Destroy(info.collider.gameObject);
                    }
                }
                else
                {
                    Ray ray = new Ray(poles[i+1].transform.position, poles[i].transform.position - poles[i + 1].transform.position);
                    if (Physics.Raycast(ray, out RaycastHit info, Vector3.Distance(poles[i].transform.position, poles[i+1].transform.position), enemyLayer, QueryTriggerInteraction.Collide))
                    {
                        Destroy(info.collider.gameObject);
                    }
                }

            }
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

                    poles.Add(Instantiate(lineStart, transform.position, Quaternion.identity));
                    poleRenderers.Add(poles[poles.Count - 1].GetComponent<Renderer>());
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
}
