using UnityEngine;
using System.Collections.Generic;
 
/*
class Point {
    public Vector3 p;
    public Point next;
}
*/
 
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class MeshLineRenderer : MonoBehaviour {
 
    public Material lineMaterial;
 
    private Mesh lineMesh;
 
    private Vector3 startPoint;
 
    private float lineSize = .1f;
 
    private bool firstQuad = true;
 
    void Start() {
        lineMesh = GetComponent<MeshFilter>().mesh;
        GetComponent<MeshRenderer>().material = lineMaterial;
    }
 
    public void setWidth(float width) {
        lineSize = width;
    }
 
    public void AddPoint(Vector3 point) {
        if(startPoint != Vector3.zero) {
            AddLine(lineMesh, MakeQuad(startPoint, point, lineSize, firstQuad));
            firstQuad = false;
        }
 
        startPoint = point;
    }
 
    Vector3[] MakeQuad(Vector3 start, Vector3 end, float width, bool firstQuadorNot) {
        width = width / 2;
 
        Vector3[] quadCreation;
        if(firstQuadorNot) {
            quadCreation = new Vector3[4];
        } else {
            quadCreation = new Vector3[2];
        }
 
        Vector3 n = Vector3.Cross(start, end);
        Vector3 l = Vector3.Cross(n, end - start);
        l.Normalize();
 
        if(firstQuadorNot) {
            quadCreation[0] = transform.InverseTransformPoint(start + l * width);
            quadCreation[1] = transform.InverseTransformPoint(start + l * -width);
            quadCreation[2] = transform.InverseTransformPoint(end + l * width);
            quadCreation[3] = transform.InverseTransformPoint(end + l * -width);
        } else {
            quadCreation[0] = transform.InverseTransformPoint(start + l * width);
            quadCreation[1] = transform.InverseTransformPoint(start + l * -width);
        }
        return quadCreation;
    }
 
    void AddLine(Mesh m, Vector3[] quad) {
        int lineVertexCount = m.vertices.Length;
 
        Vector3[] vertices = m.vertices;
        vertices = resizeVertices(vertices, 2 * quad.Length);
 
        //Assign new vertices
        for(int i = 0; i < 2*quad.Length; i += 2) {
            vertices[lineVertexCount + i] = quad[i / 2];
            vertices[lineVertexCount + i + 1] = quad[i / 2];
        }
 
        Vector2[] uvs = m.uv;
        uvs = resizeUVs(uvs, 2 * quad.Length);
 
        if(quad.Length == 4) {
            uvs[lineVertexCount] = Vector2.zero;
            uvs[lineVertexCount + 1] = Vector2.zero;
            uvs[lineVertexCount + 2] = Vector2.right;
            uvs[lineVertexCount + 3] = Vector2.right;
            uvs[lineVertexCount + 4] = Vector2.up;
            uvs[lineVertexCount + 5] = Vector2.up;
            uvs[lineVertexCount + 6] = Vector2.one;
            uvs[lineVertexCount + 7] = Vector2.one;
        } else {
            if(lineVertexCount % 8 == 0) {
                uvs[lineVertexCount] = Vector2.zero;
                uvs[lineVertexCount + 1] = Vector2.zero;
                uvs[lineVertexCount + 2] = Vector2.right;
                uvs[lineVertexCount + 3] = Vector2.right;
 
            } else {
                uvs[lineVertexCount] = Vector2.up;
                uvs[lineVertexCount + 1] = Vector2.up;
                uvs[lineVertexCount + 2] = Vector2.one;
                uvs[lineVertexCount + 3] = Vector2.one;
            }
        }
 
        int trianglesLength = m.triangles.Length;
 
        int[] triangles = m.triangles;
        triangles = resizeTriangles(triangles, 12);
 
        if(quad.Length == 2) {
            lineVertexCount -= 4;
        }
 
        // front-facing quad
        triangles[trianglesLength] = lineVertexCount;
        triangles[trianglesLength + 1] = lineVertexCount + 2;
        triangles[trianglesLength + 2] = lineVertexCount + 4;
 
        triangles[trianglesLength + 3] = lineVertexCount + 2;
        triangles[trianglesLength + 4] = lineVertexCount + 6;
        triangles[trianglesLength + 5] = lineVertexCount + 4;
 
        // back-facing quad
        triangles[trianglesLength + 6] = lineVertexCount + 5;
        triangles[trianglesLength + 7] = lineVertexCount + 3;
        triangles[trianglesLength + 8] = lineVertexCount + 1;
 
        triangles[trianglesLength + 9] = lineVertexCount + 5;
        triangles[trianglesLength + 10] = lineVertexCount + 7;
        triangles[trianglesLength + 11] = lineVertexCount + 3;
 
        m.vertices = vertices;
        m.uv = uvs;
        m.triangles = triangles;
        m.RecalculateBounds();
        m.RecalculateNormals();
    }
 
    Vector3[] resizeVertices(Vector3[] oldVertices, int newSize) {
        Vector3[] newVertices = new Vector3[oldVertices.Length + newSize];
        for(int i = 0; i < oldVertices.Length; i++) {
            newVertices[i] = oldVertices[i];
        }
 
        return newVertices;
    }
 
    Vector2[] resizeUVs(Vector2[] oldUVs, int newSize) {
        Vector2[] nvs = new Vector2[oldUVs.Length + newSize];
        for(int i = 0; i < oldUVs.Length; i++) {
            nvs[i] = oldUVs[i];
        }
 
        return nvs;
    }
 
    int[] resizeTriangles(int[] ovs, int ns) {
        int[] nvs = new int[ovs.Length + ns];
        for(int i = 0; i < ovs.Length; i++) {
            nvs[i] = ovs[i];
        }
 
        return nvs;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            AddPoint(transform.position);
        }
    }
}