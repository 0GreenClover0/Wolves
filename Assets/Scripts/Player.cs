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
    private Renderer lineRenderer;
    private MaterialPropertyBlock lineMaterialPropertyBlock;
    private new Rigidbody rigidbody;
    private float horizontal;
    private float vertical;

    private bool HasLine => currentLineStart != null;

    private LayerMask enemyLayer;

    private void Start ()
    {
        rigidbody = GetComponent<Rigidbody>(); 

        enemyLayer = LayerMask.GetMask("Enemy");

        lineMaterialPropertyBlock = new MaterialPropertyBlock();
    }

    private void FixedUpdate()
    {
        rigidbody.velocity = new Vector3(horizontal * runSpeed, 0.0f, vertical * runSpeed);

        if (HasLine)
        {
            Ray ray = new Ray(transform.position, currentLineStart.transform.position - transform.position);
            Debug.DrawRay(transform.position, currentLineStart.transform.position - transform.position, Color.red);
            if (Physics.Raycast(ray, out RaycastHit info, Vector3.Distance(currentLineStart.transform.position, transform.position), enemyLayer, QueryTriggerInteraction.Collide))
            {
                Destroy(info.collider.gameObject);
            }
        }
    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (HasLine)
        {
            Vector2 linePos = new Vector2(currentLineStart.transform.position.x, currentLineStart.transform.position.z);
            Vector2 playerPos = new Vector2(transform.position.x, transform.position.z);
            if (Vector2.Distance(linePos, playerPos) > distanceToLinePickup)
            {
                lineMaterialPropertyBlock.SetColor("_Color", Color.white);
            }
            else
            {
                lineMaterialPropertyBlock.SetColor("_Color", Color.blue);
            }

            lineRenderer.SetPropertyBlock(lineMaterialPropertyBlock);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (HasLine)
            {
                Vector2 linePos = new Vector2(currentLineStart.transform.position.x, currentLineStart.transform.position.z);
                Vector2 playerPos = new Vector2(transform.position.x, transform.position.z);
                Debug.Log(Vector2.Distance(linePos, playerPos));
                if (Vector2.Distance(linePos, playerPos) > distanceToLinePickup)
                {
                    return;
                }

                Destroy(currentLineStart);
            }
            else
            {
                currentLineStart = Instantiate(lineStart, transform.position, Quaternion.identity);
                lineRenderer = currentLineStart.GetComponent<Renderer>();
            }
        }
    }
}
