using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BlockOutKit
{
    [ExecuteInEditMode]
    public class TorusGenerator : MonoBehaviour
    {
        public bool flatShading = false;
        [Header("Dimensions")]
        [Range(0.1f, 15f)]
        public float radius = 1;
        [Range(0.01f, 1f)]
        public float tubeRadius = 0.2f;

        [Header("Segments")]
        [Range(2, 50)]
        public int radialSegments = 32;
        [Range(2, 50)]
        public int tubularSegments = 16;

        [Header("Trim")]
        [Range(0.0f, 2f)]
        public float arc = 2;

        public void Generate()
        {
            GetComponent<MeshFilter>().mesh = TorusGenerate(radius, tubeRadius, radialSegments, tubularSegments, arc, flatShading);
        }

        Mesh TorusGenerate(float radius, float tube, int radialSegments, int tubularSegments, float arc = 2, bool flatShading = false)
        {
            radius = Mathf.Max(radius, 0.01f);
            tube = Mathf.Max(tube, 0.01f);
            radialSegments = Mathf.Max(radialSegments, 2);
            tubularSegments = Mathf.Max(tubularSegments, 2);
            arc = (float)Math.PI * arc;

            // Buffer
            List<int> triangles = new List<int>();
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();

            // Helper variables

            Vector3 center = new Vector3();
            Vector3 vertex = new Vector3();
            Vector3 normal = new Vector3();

            // generate vertices, normals and uvs

            for (float j = 0; j <= radialSegments; j++)
            {

                for (float i = 0; i <= tubularSegments; i++)
                {

                    float u = i / tubularSegments * arc;
                    float v = j / radialSegments * Mathf.PI * 2f;

                    // vertex

                    vertex.x = (radius + tube * Mathf.Cos(v)) * Mathf.Cos(u);
                    vertex.y = (radius + tube * Mathf.Cos(v)) * Mathf.Sin(u);
                    vertex.z = tube * Mathf.Sin(v);

                    vertices.Add(new Vector3(vertex.x, vertex.y, vertex.z));

                    // normal

                    center.x = radius * Mathf.Cos(u);
                    center.y = radius * Mathf.Sin(u);

                    normal = vertex - center;

                    normal.Normalize();

                    normals.Add(new Vector3(normal.x, normal.y, normal.z));

                    // uv

                    uvs.Add(new Vector2(i / tubularSegments, j / radialSegments));
                }

            }

            // generate indices

            for (int j = 1; j <= radialSegments; j++)
            {

                for (int i = 1; i <= tubularSegments; i++)
                {

                    // indices

                    int a = (tubularSegments + 1) * j + i - 1;
                    int b = (tubularSegments + 1) * (j - 1) + i - 1;
                    int c = (tubularSegments + 1) * (j - 1) + i;
                    int d = (tubularSegments + 1) * j + i;

                    // faces

                    triangles.Add(a);
                    triangles.Add(b);
                    triangles.Add(d);

                    triangles.Add(b);
                    triangles.Add(c);
                    triangles.Add(d);

                }

            }

            // Build geometry
            Mesh generatedMesh = new Mesh();

            generatedMesh.vertices = vertices.ToArray();
            generatedMesh.uv = uvs.ToArray();
            generatedMesh.normals = normals.ToArray();
            generatedMesh.triangles = triangles.ToArray();

            if (flatShading == true)
            {
                List<Vector3> newVertices = new List<Vector3>();
                List<int> newTriangles = new List<int>();
                List<Vector3> newNormals = new List<Vector3>();
                List<Vector2> newUvs = new List<Vector2>();

                for (int i = 0; i < triangles.Count; i += 3)
                {
                    newVertices.Add(vertices[triangles[i + 0]]);
                    newVertices.Add(vertices[triangles[i + 1]]);
                    newVertices.Add(vertices[triangles[i + 2]]);

                    newTriangles.Add(i + 0);
                    newTriangles.Add(i + 1);
                    newTriangles.Add(i + 2);

                    newUvs.Add(uvs[triangles[i + 0]]);
                    newUvs.Add(uvs[triangles[i + 1]]);
                    newUvs.Add(uvs[triangles[i + 2]]);

                    // Face normal Calculation
                    Vector3 faceNormal = GetNormal(vertices[triangles[i + 0]], vertices[triangles[i + 1]], vertices[triangles[i + 2]]);

                    newNormals.Add(faceNormal);
                    newNormals.Add(faceNormal);
                    newNormals.Add(faceNormal);
                }

                generatedMesh.vertices = newVertices.ToArray();
                generatedMesh.uv = newUvs.ToArray();
                generatedMesh.normals = newNormals.ToArray();
                generatedMesh.triangles = newTriangles.ToArray();
            }

            return generatedMesh;
        }

        Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c)
        {
            // Find vectors corresponding to two of the sides of the triangle.
            Vector3 side1 = b - a;
            Vector3 side2 = c - a;

            // Cross the vectors to get a perpendicular vector, then normalize it.
            return Vector3.Cross(side1, side2).normalized;
        }
    }
}
