using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewScript : MonoBehaviour
{
    [SerializeField] private LayerMask layermask;
    private Mesh mesh;
    private Vector3 origin;
    private Vector3 startingPos;
    public float startingAngle;
    public float angle;
    private float fov;
    public bool canSeePlayer;
    public float suspicionMultiplier;
    public GameObject progressBar;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        fov = 90f;
        transform.position += new Vector3(-transform.parent.position.x, -transform.parent.position.y, 0f);
        startingPos = transform.position;
    }

    private void LateUpdate()
    {

        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshFilter>().sharedMesh.RecalculateBounds();
        GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();

        transform.position = transform.parent.position - transform.parent.position;
        int rayCount = 250;
        angle = startingAngle;
        float angleIncrease = fov / rayCount;
        float viewDistance = 12f;

        Vector3[] vertices = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            RaycastHit2D rayCastHit = Physics2D.Raycast(origin, new Vector3(Mathf.Cos(angle * (Mathf.PI / 180f)), Mathf.Sin(angle * Mathf.PI / 180f)), viewDistance, layermask);
            if (rayCastHit.collider == null)
            {
                vertex = origin + (new Vector3(Mathf.Cos(angle * (Mathf.PI / 180f)), Mathf.Sin(angle * Mathf.PI / 180f)) * viewDistance);
            }
            else if (rayCastHit.collider.gameObject.layer == 3)
            {
                canSeePlayer = true;
                StartCoroutine("canSeePlayerDeterminer");
                vertex = rayCastHit.point;
                suspicionMultiplier = viewDistance-Vector3.Distance(origin, rayCastHit.point);
            }
            else
            {
                vertex = rayCastHit.point;
            }
            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;

            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        if (transform.parent.GetComponent<AIBase>().aiState == AIBase.AIState.aggro)
        {
            GetComponent<MeshRenderer>().enabled = false;
            progressBar.gameObject.SetActive(false);
        }
        progressBar.gameObject.transform.position = origin + new Vector3 (0,2,0);
    }

    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }

    public void SetAimDirection(string aimDirection)
    {
        if (aimDirection == "right")
        {
            startingAngle = Mathf.Lerp(startingAngle, 45, Time.deltaTime * 20);
        }
        else if (aimDirection == "left")
        {
            startingAngle = Mathf.Lerp(startingAngle, 225, Time.deltaTime * 20);
        }

        //Vector3 dir = aimDirection;
        //
        //dir = aimDirection.normalized;
        //float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //if (n < 0) 
        //{
        //    n += 360;
        //}
        //Debug.Log(n);
        //startingAngle = n + fov / 2f;

    }

    private IEnumerator canSeePlayerDeterminer()
    {
        canSeePlayer = true;
        yield return new WaitForSeconds(0.02f);
        canSeePlayer = false;
    }
}
