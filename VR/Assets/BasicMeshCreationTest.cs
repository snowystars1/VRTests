using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeshCreationTest : MonoBehaviour
{
    private Mesh triangleMesh;

    // Start is called before the first frame update
    void Start()
    {
        triangleMesh = GetComponent<MeshFilter>().mesh;
    }

    public void SpawnTriangle(Vector3 position)
    {
        triangleMesh.Clear();
        print("hi");
        Vector3[] vertices = new Vector3[3];
        int[] triangles = new int[3];
        Vector3[] normals = new Vector3[3];

        vertices[0] = transform.InverseTransformPoint(transform.position);
        vertices[1] = transform.InverseTransformPoint(transform.position + new Vector3(1,0,0));
        vertices[2] = transform.InverseTransformPoint(transform.position + new Vector3(0, 0, 1));

        normals[0] = Vector3.up;
        normals[1] = Vector3.up;
        normals[2] = Vector3.up;

        triangles[0] = 2;
        triangles[1] = 1;
        triangles[2] = 0;

        triangleMesh.vertices = vertices;
        triangleMesh.triangles = triangles;
        triangleMesh.normals = normals;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnTriangle(transform.position);
        }
    }
}
