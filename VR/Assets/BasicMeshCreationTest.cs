using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeshCreationTest : MonoBehaviour
{
    private Mesh triangleMesh;
    bool firstTime = false;

    // Start is called before the first frame update
    void Start()
    {
        triangleMesh = GetComponent<MeshFilter>().mesh;
    }

    public void SpawnTriangle(Vector3 position)
    {
        triangleMesh.Clear();
        print("hi");
        Vector3[] vertices = new Vector3[8];
        int[] triangles = new int[36];
        //Vector3[] normals = new Vector3[3];

        //vertices[0] = transform.InverseTransformPoint(transform.position);
        //vertices[1] = transform.InverseTransformPoint(transform.position + new Vector3(1,0,0));
        //vertices[2] = transform.InverseTransformPoint(transform.position + new Vector3(0, 1, 0));
        //vertices[3] = transform.InverseTransformPoint(transform.position + new Vector3(1, 1, 0));
        //vertices[4] = transform.InverseTransformPoint(transform.position + new Vector3(0, 0, 1));
        //vertices[5] = transform.InverseTransformPoint(transform.position + new Vector3(1, 0, 1));
        //vertices[6] = transform.InverseTransformPoint(transform.position + new Vector3(0, 1, 1));
        //vertices[7] = transform.InverseTransformPoint(transform.position + new Vector3(1, 1, 1));

        vertices[0] = transform.InverseTransformPoint(transform.position);
        vertices[1] = transform.InverseTransformPoint(transform.position + transform.forward);
        vertices[2] = transform.InverseTransformPoint(transform.position + transform.up);
        vertices[3] = transform.InverseTransformPoint(transform.position + transform.forward + transform.up);


        //normals[0] = Vector3.up;
        //normals[1] = Vector3.up;
        //normals[2] = Vector3.up;

        if (firstTime)
        {
                triangles[0] = 2;
                triangles[1] = 1;
                triangles[2] = 0;

                triangles[3] = 1;
                triangles[4] = 2;
                triangles[5] = 3;
        }
        else
        {
            for(int i=0;i < triangles.Length / 6; i++)
            {

            }
        }

        //triangles[0] = 2;
        //triangles[1] = 1;
        //triangles[2] = 0;

        //triangles[3] = 1;
        //triangles[4] = 2;
        //triangles[5] = 3;

        //triangles[6] = 3;
        //triangles[7] = 5;
        //triangles[8] = 1;

        //triangles[9] = 5;
        //triangles[10] = 3;
        //triangles[11] = 7;

        //triangles[12] = 7;
        //triangles[13] = 4;
        //triangles[14] = 5;

        //triangles[15] = 4;
        //triangles[16] = 7;
        //triangles[17] = 6;

        //triangles[18] = 6;
        //triangles[19] = 0;
        //triangles[20] = 4;

        //triangles[21] = 0;
        //triangles[22] = 6;
        //triangles[23] = 2;

        //triangles[24] = 6;
        //triangles[25] = 3;
        //triangles[26] = 2;

        //triangles[27] = 3;
        //triangles[28] = 6;
        //triangles[29] = 7;

        //triangles[30] = 0;
        //triangles[31] = 5;
        //triangles[32] = 4;

        //triangles[33] = 5;
        //triangles[34] = 0;
        //triangles[35] = 1;

        triangleMesh.vertices = vertices;
        triangleMesh.triangles = triangles;
        firstTime = false;
        //triangleMesh.normals = normals;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnTriangle(transform.position);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            triangleMesh.Clear();
            firstTime = true;
        }
    }
}
