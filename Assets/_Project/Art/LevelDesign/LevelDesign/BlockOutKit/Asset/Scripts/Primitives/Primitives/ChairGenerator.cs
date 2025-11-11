using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BlockOutKit
{
    [ExecuteInEditMode]
    public class ChairGenerator : MonoBehaviour
    {
        public bool flatShading = false;
        [Header("Dimensions")]
        [Range(0.1f, 10f)]
        public float width = 0.45f;
        [Range(0.01f, 1f)]
        public float height = 0.075f;
        [Range(0.1f, 10f)]
        public float depth = 0.45f;

        [Header("Chair Back")]
        [Range(0.1f, 10f)]
        public float backHeight = 0.55f;

        [Header("Leg Size")]
        [Range(0.01f, 1f)]
        public float legWidth = 0.05f;
        [Range(0.01f, 1f)]
        public float legHeight = 0.4f;
        [Range(0.01f, 1f)]
        public float legDepth = 0.05f;

        public void Generate()
        {
            // Save initial transform
            Vector3 initialPosition = transform.localPosition;
            Vector3 initialRotation = transform.localEulerAngles;
            Vector3 initialScale = transform.localScale;

            // Set transform to zero to avoid miscalculation of the mesh positions
            transform.position = new Vector3(0, 0, 0);
            transform.localEulerAngles = new Vector3(0, 0, 0);
            transform.localScale = new Vector3(1, 1, 1);

            CombineInstance[] combinedMesh = new CombineInstance[6];

            // Chair Top
            GetComponent<MeshFilter>().mesh = BoxGenerate(width, height, depth, 1, 1, 1, flatShading);
            transform.position = new Vector3(0, legHeight + (height / 2), 0);
            combinedMesh[0].mesh = GetComponent<MeshFilter>().sharedMesh;
            combinedMesh[0].transform = GetComponent<MeshFilter>().transform.localToWorldMatrix;
            // Chair Back
            GetComponent<MeshFilter>().mesh = BoxGenerate(height, backHeight, depth, 1, 1, 1, flatShading);
            transform.position = new Vector3(-width / 2 + (height / 2), legHeight + height + (backHeight / 2), 0);
            combinedMesh[1].mesh = GetComponent<MeshFilter>().sharedMesh;
            combinedMesh[1].transform = GetComponent<MeshFilter>().transform.localToWorldMatrix;
            // First leg
            GetComponent<MeshFilter>().mesh = BoxGenerate(legWidth, legHeight, legDepth, 1, 1, 1, flatShading);
            transform.position = new Vector3(width / 2 - (legWidth / 2), legHeight / 2, depth / 2 - (legDepth / 2));
            combinedMesh[2].mesh = GetComponent<MeshFilter>().sharedMesh;
            combinedMesh[2].transform = GetComponent<MeshFilter>().transform.localToWorldMatrix;
            // Second Leg
            transform.position = new Vector3(width / 2 - (legWidth / 2), legHeight / 2, -depth / 2 + (legDepth / 2));
            combinedMesh[3].mesh = GetComponent<MeshFilter>().sharedMesh;
            combinedMesh[3].transform = GetComponent<MeshFilter>().transform.localToWorldMatrix;
            // Third Leg
            transform.position = new Vector3(-width / 2 + (legWidth / 2), legHeight / 2, depth / 2 - (legDepth / 2));
            combinedMesh[4].mesh = GetComponent<MeshFilter>().sharedMesh;
            combinedMesh[4].transform = GetComponent<MeshFilter>().transform.localToWorldMatrix;
            // Forth Leg
            transform.position = new Vector3(-width / 2 + (legWidth / 2), legHeight / 2, -depth / 2 + (legDepth / 2));
            combinedMesh[5].mesh = GetComponent<MeshFilter>().sharedMesh;
            combinedMesh[5].transform = GetComponent<MeshFilter>().transform.localToWorldMatrix;

            Mesh tableMesh = new Mesh();
            tableMesh.CombineMeshes(combinedMesh);

            GetComponent<MeshFilter>().mesh = tableMesh;
            transform.localPosition = initialPosition;
            transform.localEulerAngles = initialRotation;
            transform.localScale = initialScale;
        }

        Mesh BoxGenerate(float width, float height, float depth, int widthSegments, int heightSegments, int depthSegments, bool flatShading = false)
        {
            // Buffer
            List<int> triangles = new List<int>();
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();

            // Helpers
            int numberOfVertices = 0;
            int groupStart = 0;

            buildPlane("z", "y", "x", -1, -1, depth, height, width, depthSegments, heightSegments); // px
            buildPlane("z", "y", "x", 1, -1, depth, height, -width, depthSegments, heightSegments); // nx
            buildPlane("x", "z", "y", 1, 1, width, depth, height, widthSegments, depthSegments); // py
            buildPlane("x", "z", "y", 1, -1, width, depth, -height, widthSegments, depthSegments); // ny
            buildPlane("x", "y", "z", 1, -1, width, height, depth, widthSegments, heightSegments); // pz
            buildPlane("x", "y", "z", -1, -1, width, height, -depth, widthSegments, heightSegments); // nz

            // Build geometry
            Mesh generatedMesh = new Mesh();

            generatedMesh.vertices = vertices.ToArray();
            generatedMesh.uv = uvs.ToArray();
            generatedMesh.normals = normals.ToArray();
            generatedMesh.triangles = triangles.ToArray();

            if (flatShading == true)
            {
                generatedMesh.RecalculateNormals();
            }

            return generatedMesh;

            // Helper Functions

            void buildPlane(string u, string v, string w, int udir, int vdir, float planeWidth, float planeHeight, float planeDepth, int gridX, int gridY)
            {
                u = axisNumber(u);
                v = axisNumber(v);
                w = axisNumber(w);
                // For normal vector calculation - to access correct Vector3 property
                string axisNumber(string axis)
                {
                    if (axis == "x")
                    {
                        axis = "0";
                    }
                    else if (axis == "y")
                    {
                        axis = "1";
                    }
                    else
                    {
                        axis = "2";
                    }
                    return axis;
                }

                float segmentWidth = planeWidth / gridX;
                float segmentHeight = planeHeight / gridY;

                float widthHalf = planeWidth / 2;
                float heightHalf = planeHeight / 2;
                float depthHalf = planeDepth / 2;

                int gridX1 = gridX + 1;
                int gridY1 = gridY + 1;

                int vertexCounter = 0;
                int groupCount = 0;

                Vector3 normalVector = new Vector3();

                // Vertice, Normals and UVs generation
                for (int iy = 0; iy < gridY1; iy++)
                {
                    float y = iy * segmentHeight - heightHalf;

                    for (int ix = 0; ix < gridX1; ix++)
                    {
                        float x = ix * segmentWidth - widthHalf;

                        // Set values to correct vector component
                        normalVector[Int32.Parse(u)] = x * udir;
                        normalVector[Int32.Parse(v)] = y * vdir;
                        normalVector[Int32.Parse(w)] = depthHalf;

                        vertices.Add(new Vector3(normalVector.x, normalVector.y, normalVector.z));

                        // Set values to correct vector component
                        normalVector[Int32.Parse(u)] = 0;
                        normalVector[Int32.Parse(v)] = 0;
                        normalVector[Int32.Parse(w)] = planeDepth > 0 ? 1 : -1;

                        normals.Add(new Vector3(normalVector.x, normalVector.y, normalVector.z));

                        // UVs
                        uvs.Add(new Vector2(ix * (planeWidth / gridX), iy * (planeHeight / gridY)));

                        // Counters
                        vertexCounter += 1;
                    }
                }

                // Triangles
                // 1. You need three vertices to draw a single triangle
                // 2. A single polygon consists of two triangles
                // 3. So we need to generate six (2*3) vertices per segment

                for (int iy = 0; iy < gridY; iy++)
                {
                    for (int ix = 0; ix < gridX; ix++)
                    {
                        int a = numberOfVertices + ix + gridX1 * iy;
                        int b = numberOfVertices + ix + gridX1 * (iy + 1);
                        int c = numberOfVertices + (ix + 1) + gridX1 * (iy + 1);
                        int d = numberOfVertices + (ix + 1) + gridX1 * iy;

                        // Triangles
                        triangles.Add(a);
                        triangles.Add(b);
                        triangles.Add(d);

                        triangles.Add(b);
                        triangles.Add(c);
                        triangles.Add(d);

                        // Increase counter
                        groupCount += 6;
                    }
                }

                // Calculate new start value for groups
                groupStart += groupCount;

                // Update total number of vertices
                numberOfVertices += vertexCounter;
            }

        }


    }
}
