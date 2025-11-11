using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BlockOutKit
{
    [ExecuteInEditMode]
    public class SphereGenerator : MonoBehaviour
    {
        public bool flatShading = false;
        [Header("Dimensions")]
        [Range(0.1f, 15f)]
        public float radius = 0.5f;

        [Header("Segments")]
        [Range(3, 50)]
        public int widthSegments = 32;
        [Range(2, 50)]
        public int heightSegments = 16;

        [Header("Vertical Trim")]
        [Range(0.0f, 2f)]
        public float phiStart = 0;
        [Range(0.0f, 2f)]
        public float phiLength = 2;

        [Header("Horizontal Trim")]
        [Range(0.0f, 1f)]
        public float thetaStart = 0;
        [Range(0.0f, 1f)]
        public float thetaLength = 1;

        public void Generate()
        {
            GetComponent<MeshFilter>().mesh = SphereGenerate(radius, widthSegments, heightSegments, phiStart, phiLength, thetaStart, thetaLength, flatShading);
        }

        Mesh SphereGenerate(float radius, int widthSegments, int heightSegments, float phiStart, float phiLength, float thetaStart, float thetaLength, bool flatShading = false)
        {
            radius = Mathf.Max(radius, 0.1f);

            widthSegments = Mathf.Max(widthSegments, 1);
            heightSegments = Mathf.Max(heightSegments, 1);

            phiStart = (float)Math.PI * phiStart;
            phiLength = (float)Math.PI * phiLength;

            thetaStart = (float)Math.PI * thetaStart;
            thetaLength = (float)Math.PI * thetaLength;

            float thetaEnd = Math.Min(thetaStart + thetaLength, Mathf.PI);

            int index = 0;
            List<List<int>> grid = new List<List<int>>();

            Vector3 vertex = new Vector3();
            Vector3 normal = new Vector3();

            // Buffer
            List<int> triangles = new List<int>();
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();

            // generate vertices, normals and uvs

            for (float iy = 0; iy <= heightSegments; iy++)
            {
                List<int> verticesRow = new List<int>();

                float v = iy / heightSegments;

                // special case for the poles

                float uOffset = 0;

                if (iy == 0 && thetaStart == 0)
                {

                    uOffset = 0.5f / widthSegments;

                }
                else if (iy == heightSegments && thetaEnd == Math.PI)
                {

                    uOffset = -0.5f / widthSegments;

                }

                for (float ix = 0; ix <= widthSegments; ix++)
                {

                    float u = ix / widthSegments;

                    // vertex

                    vertex.x = -radius * Mathf.Cos(phiStart + u * phiLength) * Mathf.Sin(thetaStart + v * thetaLength);
                    vertex.y = radius * Mathf.Cos(thetaStart + v * thetaLength);
                    vertex.z = radius * Mathf.Sin(phiStart + u * phiLength) * Mathf.Sin(thetaStart + v * thetaLength);

                    vertices.Add(new Vector3(vertex.x, vertex.y, vertex.z));

                    // normal
                    normal = vertex;
                    normal.Normalize();
                    normals.Add(new Vector3(normal.x, normal.y, normal.z));

                    // uv

                    uvs.Add(new Vector2(u + uOffset, 1 - v));

                    verticesRow.Add(index++);

                }

                grid.Add(verticesRow);

            }

            // indices

            for (int iy = 0; iy < heightSegments; iy++)
            {

                for (int ix = 0; ix < widthSegments; ix++)
                {

                    int a = grid[iy][ix + 1];
                    int b = grid[iy][ix];
                    int c = grid[iy + 1][ix];
                    int d = grid[iy + 1][ix + 1];

                    if (iy != 0 || thetaStart > 0)
                    {
                        triangles.Add(a);
                        triangles.Add(b);
                        triangles.Add(d);
                    }
                    if (iy != heightSegments - 1 || thetaEnd < Math.PI)
                    {
                        triangles.Add(b);
                        triangles.Add(c);
                        triangles.Add(d);
                    }

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