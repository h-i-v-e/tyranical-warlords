using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTerrain : MonoBehaviour {

	public int size;
	[Range(1, 5)]
	public int octaves;
	[Range(0, 10)]
	public float height;
	[Range(0, 1)]
	public float coherence;

	public int seed;

	private void InitializeValues(){
		Random.InitState (seed);
	}

	private int[] GenerateTriangles(){
		int numLines = size - 1;
		int numTriangles = (numLines * numLines) << 1;
		int[] triangles = new int[numTriangles * 3];
		for (int y = 0, offset = 0; y != numLines; ++y){
			for (int x = 0; x != numLines; ++x){
				int y1 = y * size, y2 = (y + 1) * size;
				triangles [offset++] = x + y1;
				triangles [offset++] = x + y2;
				triangles [offset++] = x + 1 + y1;
				triangles [offset++] = x + y2;
				triangles [offset++] = x + 1 + y2;
				triangles [offset++] = x + 1 + y1;
			}
		}
		return triangles;
	}

	private Vector3[] GenerateVertices(Vector2[] uv){
		int numVertices = size * size;
		Vector3[] vertices = new Vector3[numVertices];
		for (int y = 0, offset = 0; y != size; ++y, offset += size) {
			for (int x = 0; x != size; ++x) {
				vertices [offset + x] = new Vector3 (x, 0f, y);
				uv [offset + x] = new Vector2 ((float)(x & 1), (float)(y & 1));
			}
		}
		NoiseLayer noise = new NoiseLayer (coherence);
		for (int i = 0; i != octaves; ++i){
			noise.coherence *= i + 1;
			noise.Apply(vertices, size, height / ((i + 1) * (i + 1)));
		}
		return vertices;
	}

	private static Vector3 NormalFromVertices (Vector3 a, Vector3 b, Vector3 c){
		return Vector3.Normalize(Vector3.Cross (b - a, c - a));
	}

	private Vector3[] GenerateNormals(Vector3[] vertices){
		Vector3[] normals = new Vector3[size * size];
		int lastIndex = size - 1, b;
		for (int y = 0, offset = 0; y != lastIndex; ++y, offset += size) {
			for (int x = 0; x != lastIndex; ++x) {
				b = offset + x;
				normals [b] = NormalFromVertices (vertices[b], vertices[b + 1], vertices[b + size]);
			}
			b = offset + lastIndex;
			normals [b] = normals [b - 1];
		}
		for (int x = 0; x != size; ++x) {
			b = lastIndex * size + x;
			normals [b] = normals[b - size];
		}
		return normals;
	}

	private void GenerateMesh(Mesh mesh){
		InitializeValues ();
		mesh.Clear ();
		Vector2[] uv = new Vector2[size * size];
		mesh.vertices = GenerateVertices (uv);
		mesh.uv = uv;
		mesh.triangles = GenerateTriangles ();
		mesh.normals = GenerateNormals (mesh.vertices);
	}

	public void GenerateSharedMesh(){
		MeshFilter filter = GetComponent<MeshFilter> ();
		GenerateMesh (filter.sharedMesh);
	}

	void Start () {
		MeshFilter filter = GetComponent<MeshFilter> ();
		GenerateMesh (filter.mesh);
	}
}
