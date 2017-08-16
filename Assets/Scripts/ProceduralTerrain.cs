using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTerrain : MonoBehaviour {

	//public WeatheringFilter weathering;
	public ExpensiveWeatheringFilter weathering;
	public NoiseFilter noise;
	public IHeightMapFilter typeFilter;

	[Range(4, 255)]
	public int size;
	public float textureSize;

	public int seed;

	[Range(0, 5)]
	public int smoothings;

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

	private static float GetOverflow(float val){
		int floor = (int)val;
		float o = val - floor;
		return val < 0 ? 1f + o : o;
	}

	private Vector2[] GenerateUV (Vector3[] vertices){
		Vector2[] uv = new Vector2[size * size];
		for (int y = 0, offset = 0; y != size; ++y) {
			for (int x = 0; x != size; ++x, ++offset) {
				uv [offset] = new Vector2 ((float)(x & 1), (float)(y & 1));
			}
		}
		return uv;
	}

	private void PrintVertices(Vector3[] vertices){
		for (int y = 0, offset = 0; y != size; ++y) {
			string line = "";
			for (int x = 0; x != size; ++x, ++offset) {
				line += ", " + vertices [offset];
			}
			Debug.Log (line);
		}
	}

	private void ClampToOrigin(Vector3[] vertices){
		float minY = float.MaxValue;
		foreach (Vector3 v in vertices){
			if (v.y < minY){
				minY = v.y;
			}
		}
		for (int i = 0, j = vertices.Length; i != j; vertices[i++].y -= minY);
	}

	private Vector3[] GenerateVertices(){
		int smoothingPasses = (noise.overtones * smoothings) + 1;
		HeightMap map = new HeightMap (SmoothFilter.ComputeStartSize (size + smoothingPasses, smoothingPasses));
		SmoothFilter smooth = new SmoothFilter ();
		noise.ClearOvertoneFilters ();
		for (int i = 0; i != smoothings; ++i) {
			noise.AddOvertoneFilter (smooth);
		}
		map = smooth.Filter (weathering.Filter (typeFilter.Filter(noise.Filter (map))));
		int dif = map.Size - size;
		int offset = dif >> 1;
		Vector3[] vertices = map.ToVertices (offset, offset, size);
		ClampToOrigin(vertices);
		return vertices;
	}

	private static Vector3 NormalFromVertices (Vector3 a, Vector3 b, Vector3 c){
		return Vector3.Normalize(Vector3.Cross (a - b, a - c)) * -1f;
	}

	private static Vector3 NormalFromVertices (Vector3 a, Vector3 b, Vector3 c, Vector3 d){
		return Vector3.Normalize(Vector3.Cross (a - b, c - d)) * -1f;
	}

	private Vector3[] GenerateNormals(Vector3[] vertices){
		Vector3[] normals = new Vector3[size * size];
		int lastIndex = size - 1, b;
		for (int x = 0; x != lastIndex; ++x) {
			b = lastIndex * size + x;
			normals [x] = NormalFromVertices (vertices[x], vertices[x + 1], vertices[x + size]);
		}
		normals [lastIndex] = normals [lastIndex - 1];
		for (int y = 1, offset = size; y != lastIndex; ++y, offset += size) {
			for (int x = 1; x != lastIndex; ++x) {
				b = offset + x;
				normals [b] = NormalFromVertices (vertices[b - 1], vertices[b + 1], vertices[b - size], vertices[b + size]);
			}
			normals[offset] = NormalFromVertices (vertices[offset], vertices[offset + 1], vertices[offset + size]);
			b = offset + lastIndex;
			normals [b] = normals [b - 1];
		}
		for (int x = 0; x != size; ++x) {
			b = lastIndex * size + x;
			normals [b] = normals[b - size];
		}
		return normals;
	}

	private void AssignVertices(Mesh mesh, Vector3[] vertices){
		mesh.Clear ();
		mesh.vertices = vertices;
		mesh.uv = GenerateUV (vertices);
		mesh.triangles = GenerateTriangles ();
		//mesh.normals = GenerateNormals (vertices);
		mesh.RecalculateNormals();
		mesh.RecalculateBounds ();
	}

	private void GenerateMesh(Mesh mesh){
		InitializeValues ();
		AssignVertices(mesh, GenerateVertices ());
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
