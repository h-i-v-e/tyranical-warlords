using UnityEngine;

public struct HeightMap{
	private int size;
	private float[] heights;

	public HeightMap(int size){
		heights = new float[size * size];
		this.size = size;
	}

	public float this[int x, int y]{
		get{
			return heights [y * size + x];
		}
		set{
			heights [y * size + x] = value;
		}
	}

	public float this[Coord coord]{
		get{
			return heights[coord.Offset (size)];
		}
		set{
			heights [coord.Offset (size)] = value;
		}
	}

	public int Size{
		get{
			return size;
		}
	}

	public Vector3[] ToVertices(int fromx, int fromy, int newsize){
		float stepSize = 1f / newsize;
		Vector3[] verts = new Vector3[newsize * newsize];
		float zOff = -0.5f;
		for (int y = fromy, yend = newsize + fromy, xend = newsize + fromx, offset = 0; y != yend; ++y, zOff += stepSize){
			float xOff = -0.5f;
			for (int x = fromx; x != xend; ++x, xOff += stepSize){
				verts [offset++] = new Vector3 (xOff, this[x, y], zOff);
			}
		}
		return verts;
	}

	public Vector3[] ToVertices(){
		return ToVertices (0, 0, size);
	}

	public float this[int i]{
		get{
			return heights [i];
		}
		set{
			heights [i] = value;
		}
	}
}