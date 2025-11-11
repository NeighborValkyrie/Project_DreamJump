using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BlockOutKit
{
    [ExecuteInEditMode]
    public class WedgeGenerator : MonoBehaviour
    {
        [Header("Dimensions")]
        [Range(0.1f, 50f)]
        public float width = 1;
        [Range(0.1f, 50f)]
        public float height = 1;
        [Range(0.1f, 50f)]
        public float depth = 1;

        public void Generate()
        {
            GetComponent<MeshFilter>().mesh = WedgeGenerate(width, height, depth);
        }

        Mesh WedgeGenerate(float width, float height, float depth)
        {
            Vector3[] vertices = new Vector3[] {
         new Vector3(0, 0, 0),
         new Vector3(width, 0, 0),
         new Vector3(0, height, 0),

         new Vector3(0, 0, depth),
         new Vector3(width, 0, depth),
         new Vector3(0, height, depth),

         new Vector3(width, 0, 0),
         new Vector3(0, height, 0),
         new Vector3(width, 0, depth),

         new Vector3(0, height, depth),
         new Vector3(0, height, 0),
         new Vector3(width, 0, depth),

         new Vector3(0, height, depth),
         new Vector3(0, height, 0),
         new Vector3(0, 0, depth),

         new Vector3(0, 0, 0),
         new Vector3(0, height, 0),
         new Vector3(0, 0, depth),

         new Vector3(0, 0, 0),
         new Vector3(0, 0, depth),
         new Vector3(width, 0, 0),

         new Vector3(width, 0, depth),
         new Vector3(0, 0, depth),
         new Vector3(width, 0, 0)};

            int[] indices = new int[] {
            0,2,1, 3,4,5 ,6,7,8 ,9,11,10, 12,13,14,
              15,17,16, 18,20,19, 21,22,23
        };

            Vector2[] uvs = new Vector2[] {
         new Vector2(0, 0),
         new Vector2(width, 0),
         new Vector2(0, height),

         new Vector2(0, 0),
         new Vector2(width, 0),
         new Vector2(0, height),

         new Vector2(0, -width),
         new Vector2(0, height),
         new Vector2(depth, -width),

         new Vector2(depth, height),
         new Vector2(0, height),
         new Vector2(depth, -width),

         new Vector2(0, height),
         new Vector2(depth, height),
         new Vector2(0, 0),

         new Vector2(depth, 0),
         new Vector2(depth, height),
         new Vector2(0, 0),

         new Vector2(depth, width),
         new Vector2(0, width),
         new Vector2(depth, 0),

         new Vector2(0, 0),
         new Vector2(0, width),
         new Vector2(depth, 0)};

            // Build geometry
            Mesh generatedMesh = new Mesh();

            generatedMesh.vertices = vertices;
            generatedMesh.uv = uvs;
            generatedMesh.triangles = indices;
            generatedMesh.RecalculateNormals();

            return generatedMesh;

        }

    }
}