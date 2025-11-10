using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BlockOutKit
{
	[ExecuteInEditMode]
	public class TorusKnotGenerator : MonoBehaviour
	{
		public bool flatShading = false;
		[Header("Dimensions")]
		[Range(0.1f, 15f)]
		public float radius = 0.5f;
		[Range(0.01f, 1f)]
		public float tubeRadius = 0.15f;

		[Header("Segments")]
		[Range(3, 50)]
		public int tubularSegments = 32;
		[Range(3, 50)]
		public int radialSegments = 12;

		[Header("Knot Winds")]
		[Range(1, 15)]
		public int p = 2;
		[Range(1, 15)]
		public int q = 3;

		public void Generate()
		{
			GetComponent<MeshFilter>().mesh = TorusKnotGenerate(radius, tubeRadius, tubularSegments, radialSegments, p, q, flatShading);
		}

		Mesh TorusKnotGenerate(float radius, float tube, int tubularSegments, int radialSegments, int p, int q, bool flatShading = false)
		{
			radius = Mathf.Max(radius, 0.1f);
			tube = Mathf.Max(tube, 0.01f);
			tubularSegments = Mathf.Max(tubularSegments, 2);
			radialSegments = Mathf.Max(radialSegments, 2);
			p = Mathf.Max(p, 1);
			q = Mathf.Max(q, 1);

			// Buffer
			List<int> triangles = new List<int>();
			List<Vector3> vertices = new List<Vector3>();
			List<Vector3> normals = new List<Vector3>();
			List<Vector2> uvs = new List<Vector2>();

			// helper variables

			Vector3 vertex = new Vector3();
			Vector3 normal = new Vector3();

			Vector3 P1 = new Vector3();
			Vector3 P2 = new Vector3();

			Vector3 B = new Vector3();
			Vector3 T = new Vector3();
			Vector3 N = new Vector3();

			// generate vertices, normals and uvs

			for (float i = 0; i <= tubularSegments; ++i)
			{

				// the radian "u" is used to calculate the position on the torus curve of the current tubular segement

				float u = i / tubularSegments * p * Mathf.PI * 2f;

				// now we calculate two points. P1 is our current position on the curve, P2 is a little farther ahead.
				// these points are used to create a special "coordinate space", which is necessary to calculate the correct vertex positions

				P1 = calculatePositionOnCurve(u, p, q, radius);
				P2 = calculatePositionOnCurve(u + 0.01f, p, q, radius);

				// calculate orthonormal basis

				T = P2 - P1;
				N = P2 + P1;
				B = crossVectors(T, N);
				N = crossVectors(B, T);

				// normalize B, N. T can be ignored, we don't use it

				B.Normalize();
				N.Normalize();

				for (int j = 0; j <= radialSegments; ++j)
				{

					// now calculate the vertices. they are nothing more than an extrusion of the torus curve.
					// because we extrude a shape in the xy-plane, there is no need to calculate a z-value.

					float v = (float)j / radialSegments * Mathf.PI * 2f;
					float cx = (float)-tube * Mathf.Cos(v);
					float cy = (float)tube * Mathf.Sin(v);

					// now calculate the final vertex position.
					// first we orient the extrusion with our basis vectos, then we add it to the current position on the curve

					vertex.x = P1.x + (cx * N.x + cy * B.x);
					vertex.y = P1.y + (cx * N.y + cy * B.y);
					vertex.z = P1.z + (cx * N.z + cy * B.z);

					vertices.Add(new Vector3(vertex.x, vertex.y, vertex.z));

					// normal (P1 is always the center/origin of the extrusion, thus we can use it to calculate the normal)
					normal = (vertex - P1).normalized;

					normals.Add(new Vector3(normal.x, normal.y, normal.z));

					// uv

					uvs.Add(new Vector2(i / tubularSegments, j / radialSegments));

				}

			}

			// generate indices

			for (int j = 1; j <= tubularSegments; j++)
			{

				for (int i = 1; i <= radialSegments; i++)
				{

					// indices

					int a = (radialSegments + 1) * (j - 1) + (i - 1);
					int b = (radialSegments + 1) * j + (i - 1);
					int c = (radialSegments + 1) * j + i;
					int d = (radialSegments + 1) * (j - 1) + i;

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

		// this function calculates the current position on the torus curve
		Vector3 calculatePositionOnCurve(float u, int p, int q, float radius)
		{
			Vector3 position = new Vector3();
			float cu = Mathf.Cos(u);
			float su = Mathf.Sin(u);
			float quOverP = (float)q / p * u;
			float cs = Mathf.Cos(quOverP);

			position.x = radius * (2f + cs) * 0.5f * cu;
			position.y = radius * (2f + cs) * su * 0.5f;
			position.z = radius * Mathf.Sin(quOverP) * 0.5f;

			return position;
		}

		Vector3 crossVectors(Vector3 a, Vector3 b)
		{
			Vector3 crossVector = new Vector3();
			float ax = a.x, ay = a.y, az = a.z;
			float bx = b.x, by = b.y, bz = b.z;

			crossVector.x = ay * bz - az * by;
			crossVector.y = az * bx - ax * bz;
			crossVector.z = ax * by - ay * bx;

			return crossVector;
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