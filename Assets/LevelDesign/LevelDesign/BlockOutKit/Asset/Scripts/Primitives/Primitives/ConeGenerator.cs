using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BlockOutKit
{
    [ExecuteInEditMode]
    public class ConeGenerator : MonoBehaviour
    {
        public bool flatShading = false;
        [Header("Dimensions")]
        [Range(0.1f, 50f)]
        public float height = 1;
        [Range(0.1f, 50f)]
        public float radiusBottom = 1;

        [Header("Segments")]
        [Range(1, 50)]
        public int radialSegments = 16;
        [Range(1, 50)]
        public int heightSegments = 4;

        [Header("Trim")]
        public bool openEnded = false;
        [Range(0.0f, 2f)]
        public float thetaStart = 0;
        [Range(0.0f, 2f)]
        public float thetaLength = 2;

        public void Generate()
        {
            GetComponent<MeshFilter>().mesh = CylinderGenerate(0, radiusBottom, height, radialSegments, heightSegments, openEnded, thetaStart, thetaLength, flatShading);
        }

        public Mesh CylinderGenerate(float radiusTop, float radiusBottom, float height, int radialSegments, int heightSegments, bool openEnded = false, float thetaStart = 0, float thetaLength = 2, bool flatShading = false)
        {
            radiusBottom = Mathf.Max(radiusBottom, 0.01f);
            height = Mathf.Max(height, 0.01f);

            radialSegments = Mathf.Max(radialSegments, 3);
            heightSegments = Mathf.Max(heightSegments, 1);

            thetaStart = (float)Math.PI * thetaStart;
            thetaLength = (float)Math.PI * thetaLength;

            // Buffer
            List<int> triangles = new List<int>();
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();

            // Helpers
            int index = 0;
            List<List<int>> indexArray = new List<List<int>>();
            float halfHeight = height / 2;
            int groupStart = 0;

            // generate geometry

            generateTorso();

            if (openEnded == false)
            {

                if (radiusTop > 0) generateCap(true);
                if (radiusBottom > 0) generateCap(false);

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

            // Mesh generation functions

            void generateTorso()
            {

                Vector3 normal = new Vector3();
                Vector3 vertex = new Vector3();

                int groupCount = 0;

                // This will be used to calculate the normal
                float slope = (radiusBottom - radiusTop) / height;

                // Generate vertices, normals and uvs
                for (float y = 0; y <= heightSegments; y++)
                {
                    List<int> indexRow = new List<int>();

                    float v = y / heightSegments;

                    // Calculate the radius of the current row

                    float radius = v * (radiusBottom - radiusTop) + radiusTop;

                    for (float x = 0; x <= radialSegments; x++)
                    {

                        float u = x / radialSegments;

                        float theta = u * thetaLength + thetaStart;

                        float sinTheta = Mathf.Sin(theta);
                        float cosTheta = Mathf.Cos(theta);

                        // Vertex
                        vertex.x = radius * sinTheta;
                        vertex.y = -v * height + halfHeight;
                        vertex.z = radius * cosTheta;
                        vertices.Add(new Vector3(vertex.x, vertex.y, vertex.z));

                        // Normal
                        normal = new Vector3(sinTheta, slope, cosTheta);
                        normal.Normalize();

                        normals.Add(new Vector3(normal.x, normal.y, normal.z));

                        // UVs

                        uvs.Add(new Vector2(u, 1 - v));

                        // Save index of vertex in respective row

                        indexRow.Add(index++);

                    }

                    // Now save vertices of the row in our index array

                    indexArray.Add(indexRow);

                }

                // Generate indices

                for (int x = 0; x < radialSegments; x++)
                {

                    for (int y = 0; y < heightSegments; y++)
                    {
                        // We use the index array to access the correct indices

                        int a = indexArray[y][x];
                        int b = indexArray[y + 1][x];
                        int c = indexArray[y + 1][x + 1];
                        int d = indexArray[y][x + 1];

                        // Faces

                        triangles.Add(a);
                        triangles.Add(b);
                        triangles.Add(d);

                        triangles.Add(b);
                        triangles.Add(c);
                        triangles.Add(d);

                        // Update group counter

                        groupCount += 6;

                    }

                }

                // Calculate new start value for groups

                groupStart += groupCount;

            }

            void generateCap(bool top)
            {

                int centerIndexStart, centerIndexEnd;

                Vector2 uv = new Vector2();
                Vector3 vertex = new Vector3();

                int groupCount = 0;

                float radius = (top == true) ? radiusTop : radiusBottom;
                float sign = (top == true) ? 1 : -1;

                // Save the index of the first center vertex
                centerIndexStart = index;

                // First we generate the center vertex data of the cap.
                // because the geometry needs one set of uvs per face,
                // we must generate a center vertex per face/segment

                for (float x = 1; x <= radialSegments; x++)
                {

                    // Vertex
                    vertices.Add(new Vector3(0, halfHeight * sign, 0));

                    // Normal
                    normals.Add(new Vector3(0, sign, 0));

                    // uv
                    uvs.Add(new Vector2(0.5f, 0.5f));

                    // increase index
                    index++;

                }

                // save the index of the last center vertex

                centerIndexEnd = index;

                // now we generate the surrounding vertices, normals and uvs

                for (float x = 0; x <= radialSegments; x++)
                {

                    float u = x / radialSegments;
                    float theta = u * thetaLength + thetaStart;

                    float cosTheta = (float)Math.Cos(theta);
                    float sinTheta = (float)Math.Sin(theta);

                    // vertex

                    vertex.x = radius * sinTheta;
                    vertex.y = halfHeight * sign;
                    vertex.z = radius * cosTheta;
                    vertices.Add(new Vector3(vertex.x, vertex.y, vertex.z));

                    // normal

                    normals.Add(new Vector3(0, sign, 0));

                    // uv

                    uv.x = (cosTheta * 0.5f) + 0.5f;
                    uv.y = (sinTheta * 0.5f * sign) + 0.5f;
                    uvs.Add(new Vector2(uv.x, uv.y));

                    // increase index

                    index++;

                }

                // generate indices

                for (int x = 0; x < radialSegments; x++)
                {

                    int c = centerIndexStart + x;
                    int i = centerIndexEnd + x;

                    if (top == true)
                    {

                        // face top
                        triangles.Add(i);
                        triangles.Add(i + 1);
                        triangles.Add(c);

                    }
                    else
                    {

                        // face bottom
                        triangles.Add(i + 1);
                        triangles.Add(i);
                        triangles.Add(c);

                    }

                    groupCount += 3;

                }

                // calculate new start value for groups

                groupStart += groupCount;

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