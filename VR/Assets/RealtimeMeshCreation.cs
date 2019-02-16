using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealtimeMeshCreation : MonoBehaviour
{
    public float paintBrushLength=1.0f;
    public float depth = 5.0f;
    public float time = 10.0f;
    public float minDistance = 0.1f;

    Vector3[] newVertices;
    Vector2[] newUV;
    int[] newTriangles;

    public Color startColor = Color.white;
    public Color endColor = new Color(1, 1, 1, 0);

    private List<PaintSection> sections = new List<PaintSection>();

    private void LateUpdate()
    {
        //Vector3 position = transform.position;// + new Vector3(0f, 0f, paintBrushLength);

        Vector3 position = transform.TransformPoint(transform.localPosition + new Vector3(0f, 0f, paintBrushLength));
        //Vector3 position = transform.localPosition;
        float now = Time.time;

        // Remove old sections
        while (sections.Count > 0 && now > sections[sections.Count - 1].time + time)
        {
            sections.RemoveAt(sections.Count - 1);
        }

        // Add a new trail section
        if (sections.Count == 0 || (sections[0].point - position).sqrMagnitude > minDistance * minDistance)
        {
            PaintSection section = new PaintSection();
            section.point = position;

            section.dir = Vector3.forward;//transform.TransformDirection(Vector3.forward);

            section.time = now;

            sections.Insert(0, section);
        }

        // Rebuild the mesh
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        // We need at least 2 sections to create the line
        if (sections.Count < 2)
        {
            return;
        }

        Vector3[] vertices = new Vector3[sections.Count * 2];
        Color[] colors = new Color[sections.Count * 2];
        Vector2[] uv = new Vector2[sections.Count * 2];

        PaintSection previousSection = sections[0];
        PaintSection currentSection = sections[0];

        // Use matrix instead of transform.TransformPoint for performance reasons
        Matrix4x4 localSpaceTransform = transform.worldToLocalMatrix;

        // Generate vertex, uv and colors
        for (int i = 0; i < sections.Count; i++)
        {
            previousSection = currentSection;
            currentSection = sections[i];
            // Calculate u for texture uv and color interpolation
            float u = 0.0f;
            if (i != 0)
            {
                u = Mathf.Clamp01((Time.time - currentSection.time) / time);
            }

            // Calculate upwards direction
            Vector3 upDir = currentSection.dir;

            // Generate vertices
            vertices[i * 2 + 0] = localSpaceTransform.MultiplyPoint(currentSection.point);
            vertices[i * 2 + 1] = localSpaceTransform.MultiplyPoint(currentSection.point + upDir * depth);
            //vertices[i * 2 + 0] = currentSection.point;
            //vertices[i * 2 + 1] = currentSection.point + upDir * depth;

            uv[i * 2 + 0] = new Vector2(u, 0);
            uv[i * 2 + 1] = new Vector2(u, 1);

            // fade colors out over time
            //colors[i * 2 + 0] = Color.black;
            //colors[i * 2 + 1] = Color.black;
            Color interpolatedColor = Color.Lerp(startColor, endColor, u);
            colors[i * 2 + 0] = interpolatedColor;
            colors[i * 2 + 1] = interpolatedColor;
        }

        // Generate triangles indices
        int[] triangles = new int[(sections.Count - 1) * 2 * 3];
        for (int i = 0; i < triangles.Length / 6; i++)
        {
            triangles[i * 6 + 0] = i * 2;
            triangles[i * 6 + 1] = i * 2 + 1;
            triangles[i * 6 + 2] = i * 2 + 2;

            triangles[i * 6 + 3] = i * 2 + 2;
            triangles[i * 6 + 4] = i * 2 + 1;
            triangles[i * 6 + 5] = i * 2 + 3;
        }

        // Assign to mesh	
        mesh.vertices = vertices;
        //mesh.colors = colors;
        mesh.colors = colors;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawSphere(transform.TransformPoint(transform.localPosition + new Vector3(0f, 0f, paintBrushLength)), .1f);
    }


    class PaintSection
    {
        public Vector3 point;
        public Vector3 dir;
        public  float time;
    }
}
