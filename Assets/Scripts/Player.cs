using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float runSpeed = 20.0f;
    public float distanceToLinePickup = 5.0f;

    [Header("References")]
    public GameObject lineStart;

    private GameObject currentLineStart;
    private List<GameObject> poles; // <---- polaki biedaki, specjalnie z malej
    //wybacz mi wpisanie tych dwoch lineRendererow, ale nie wiem co robi ten pierwszy do konca to nie chcialem usuwac
    private List<Renderer> poleRenderer;
    private LineRenderer lineRenderer2; // idk what the first one is for tbh
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
        lineRenderer2 = GetComponent<LineRenderer>();
        poles = new List<GameObject>();
        poleRenderer = new List<Renderer>();
    }

    private void FixedUpdate()
    {
        rigidbody.velocity = new Vector3(horizontal * runSpeed, 0.0f, vertical * runSpeed);

        if (HasLine)
        {
            
            Debug.DrawRay(transform.position, poles[0].transform.position - transform.position, Color.red);
            Vector3[] lineVertices = new Vector3[poles.Count+1];
            for(int i = 0; i < poles.Count; i++)
            {
                lineVertices[i] = poles[i].transform.position;
            }
            lineVertices[poles.Count] = transform.position;
            lineRenderer2.positionCount = poles.Count + 1;
            lineRenderer2.SetPositions(lineVertices);
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

            poleRenderer[poleRenderer.Count-1].SetPropertyBlock(lineMaterialPropertyBlock);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (HasLine)
            {
                Vector2 linePos = new Vector2(poles[poles.Count - 1].transform.position.x, poles[poles.Count-1].transform.position.z);
                Vector2 playerPos = new Vector2(transform.position.x, transform.position.z);
                Debug.Log(Vector2.Distance(linePos, playerPos));
                if (Vector2.Distance(linePos, playerPos) > distanceToLinePickup)
                {
                    poles.Add(Instantiate(lineStart, transform.position, Quaternion.identity));
                    poleRenderer.Add(poles[poles.Count - 1].GetComponent<Renderer>());
                    return;
                }
                lineRenderer2.positionCount = poles.Count;
                Destroy(poles[poles.Count - 1]);
                poles.RemoveAt(poles.Count - 1);
                poleRenderer.RemoveAt(poleRenderer.Count - 1);
            }
            else
            {
                poles.Add(Instantiate(lineStart, transform.position, Quaternion.identity));
                poleRenderer.Add(poles[poles.Count - 1].GetComponent<Renderer>());
            }
        }
    }
}
