using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BlockOutKit
{
    [ExecuteInEditMode]
    public class IcoSphereGenerator : MonoBehaviour
    {
        public bool flatShading = false;
        [Header("Parameters")]
        [Range(0.1f, 15f)]
        public float radius = 0.5f;
        [Range(0, 3)]
        public int details = 2;

        public void Generate()
        {
            GetComponent<MeshFilter>().mesh = icoSphereGenerate(radius, details, flatShading);
        }

        Mesh icoSphereGenerate(float radius, int details, bool flatShading = false)
        {
            details = Math.Min(details, 5); // More than 5 will freeze Unity

            float t = (1 + Mathf.Sqrt(5)) / 2f;

            Vector3[] vertices = new Vector3[] {
         new Vector3(- 1, 0, t),
         new Vector3(1, 0, t),
         new Vector3(-1, 0, -t),
         new Vector3(1, 0, -t),
         new Vector3(0, t, 1),
         new Vector3(0, t, -1),
         new Vector3(0, -t, 1),
         new Vector3(0, -t, -1),
         new Vector3(t, 1, 0),
         new Vector3(-t, 1, 0),
         new Vector3(t, -1, 0),
         new Vector3(-t, -1, 0) };

            int[] indices = new int[] {
            0,1,4, 0,4,9 ,9,4,5 ,4,8,5, 4,1,8,
              8,1,10, 8,10,3, 5,8,3, 5,3,2, 2,3,7,
              7,3,10, 7,10,6, 7,6,11, 11,6,0, 0,6,1,
              6,10,1, 9,11,0, 9,2,11, 9,5,2, 7,11,2
        };

            Mesh generatedMesh = PolyhedronGenerate(vertices, indices, radius, details);

            if (flatShading == true)
            {
                List<Vector3> newVertices = new List<Vector3>();
                List<int> newTriangles = new List<int>();
                List<Vector3> newNormals = new List<Vector3>();
                List<Vector2> newUvs = new List<Vector2>();

                for (int i = 0; i < generatedMesh.triangles.Length; i += 3)
                {
                    newVertices.Add(generatedMesh.vertices[generatedMesh.triangles[i + 0]]);
                    newVertices.Add(generatedMesh.vertices[generatedMesh.triangles[i + 1]]);
                    newVertices.Add(generatedMesh.vertices[generatedMesh.triangles[i + 2]]);

                    newTriangles.Add(i + 0);
                    newTriangles.Add(i + 1);
                    newTriangles.Add(i + 2);

                    newUvs.Add(generatedMesh.uv[generatedMesh.triangles[i + 0]]);
                    newUvs.Add(generatedMesh.uv[generatedMesh.triangles[i + 1]]);
                    newUvs.Add(generatedMesh.uv[generatedMesh.triangles[i + 2]]);

                    // Face normal Calculation
                    Vector3 faceNormal = GetNormal(generatedMesh.vertices[generatedMesh.triangles[i + 0]], generatedMesh.vertices[generatedMesh.triangles[i + 1]], generatedMesh.vertices[generatedMesh.triangles[i + 2]]);

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

            Mesh PolyhedronGenerate(Vector3[] polyVertices, int[] polyIndices, float polyRadius, int polyDetail)
            {
                // default buffer data
                List<Vector3> vertexBuffer = new List<Vector3>();
                List<Vector2> uvBuffer = new List<Vector2>();

                // the subdivision creates the vertex buffer data

                subdivide(polyDetail);

                // all vertices should lie on a conceptual sphere with a given radius

                applyRadius(polyRadius);

                // finally, create the uv data

                generateUVs();

                // build non-indexed geometry

                Mesh polyhedronMesh = new Mesh();

                polyhedronMesh.vertices = vertexBuffer.ToArray();
                polyhedronMesh.uv = uvBuffer.ToArray();
                polyhedronMesh.normals = vertexBuffer.ToArray();

                List<int> tris = new List<int>();
                for (int q = 0; q < polyhedronMesh.vertices.Length; q++)
                {
                    tris.Add(q);
                }
                polyhedronMesh.triangles = tris.ToArray();

                for (int i = 0, il = polyhedronMesh.triangles.Length / 3; i < il; i++)
                {
                    polyhedronMesh.normals[polyhedronMesh.triangles[i]].Normalize();
                }

                return polyhedronMesh;

                // helper functions

                void subdivide(int Sdetail)
                {
                    // Iterate over all faces and apply a subdivison with the given detail value

                    for (int i = 0; i < polyIndices.Length; i += 3)
                    {
                        // Get the vertices of the triangle
                        // Perform subdivision

                        subdivideFace(polyVertices[polyIndices[i + 0]], polyVertices[polyIndices[i + 1]], polyVertices[polyIndices[i + 2]], Sdetail);

                    }

                }

                void subdivideFace(Vector3 a, Vector3 b, Vector3 c, int SFdetail)
                {

                    float cols = Mathf.Pow(2, SFdetail);

                    // We use this multidimensional array as a data structure for creating the subdivision

                    List<List<Vector3>> v = new List<List<Vector3>>();

                    // Construct all of the vertices for this subdivision

                    for (int i = 0; i <= cols; i++)
                    {

                        v.Add(new List<Vector3>());

                        Vector3 aj = Vector3.Lerp(a, c, (float)i / cols);
                        Vector3 bj = Vector3.Lerp(b, c, (float)i / cols);

                        int rows = (int)cols - i;

                        for (int j = 0; j <= rows; j++)
                        {

                            if (j == 0 && i == cols)
                            {
                                v[i].Add(aj);
                            }
                            else
                            {
                                v[i].Add(Vector3.Lerp(aj, bj, (float)j / rows));
                            }

                        }

                    }

                    // Construct all of the faces

                    for (int i = 0; i < cols; i++)
                    {

                        for (int j = 0; j < 2 * (cols - i) - 1; j++)
                        {

                            int k = (int)Mathf.Floor(j / 2);

                            if (j % 2 == 0)
                            {
                                vertexBuffer.Add((v[i][k + 1]));
                                vertexBuffer.Add((v[i + 1][k]));
                                vertexBuffer.Add((v[i][k]));
                            }
                            else
                            {
                                vertexBuffer.Add((v[i][k + 1]));
                                vertexBuffer.Add((v[i + 1][k + 1]));
                                vertexBuffer.Add((v[i + 1][k]));
                            }

                        }

                    }

                }

                void applyRadius(float Aradius)
                {

                    Vector3 vertex = new Vector3();

                    // iterate over the entire buffer and apply the radius to each vertex

                    for (int i = 0; i < vertexBuffer.Count; i++)
                    {
                        vertex = vertexBuffer[i].normalized;
                        vertex *= Aradius;

                        vertexBuffer[i] = vertex;
                    }

                }

                void generateUVs()
                {

                    Vector3 vertex = new Vector3();

                    for (int i = 0; i < vertexBuffer.Count; i++)
                    {

                        vertex = vertexBuffer[i];

                        float u = azimuth(vertex) / 2 / Mathf.PI + 0.5f;
                        float v = inclination(vertex) / Mathf.PI + 0.5f;
                        uvBuffer.Add(new Vector2(u, 1 - v));

                    }

                    // correctUVs();

                    // correctSeam();

                }

                float azimuth(Vector3 vector)
                {
                    return (float)Math.Atan2(vector.z, -vector.x);
                }


                // Angle above the XZ plane.
                float inclination(Vector3 vector)
                {
                    return (float)Math.Atan2(-vector.y, Math.Sqrt((vector.x * vector.x) + (vector.z * vector.z)));
                }
            }
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
