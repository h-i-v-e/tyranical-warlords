using UnityEngine;

[System.Serializable]
public class Weathering{

	public int particles;
	[Range(0f, 1f)]
	public float carryingCapacity;

	private struct Particle{
		public float carryingCapacity;
		public Vector3[] vertices;
		int x, y, size, last;
		float velocity, carrying;

		public int Size{
			get{
				return size;
			}
			set{
				size = value;
				last = size - 1;
			}
		}


		int GetLast(int value){
			return --value < 0 ? 0 : value;
		}

		int GetNext(int value){
			return ++value > last ? last : value;
		}

		float Gouge(Vector3 slope){
			float newVelocity = (velocity - slope.y) * 0.5f;
			float canCarry = newVelocity * carryingCapacity;
			float o = carrying - canCarry;
			carrying = canCarry;
			velocity = newVelocity;
			return o;
		}

		bool Iterate(){
			int myVertexOffset = y * size + x;
			Vector3 steepestSlope = Vector3.up, centre = vertices[myVertexOffset];
			int lastX = GetLast(x), nextX = GetNext(x), lastY = GetLast(y), nextY = GetNext(y);
			int ox = x, oy = y;
			for (int y = lastY; y <= nextY; ++y){
				int yoff = y * size;
				for (int x = lastX; x <= nextX; ++x){
					int pos = yoff + x;
					if (pos == myVertexOffset){
						continue;
					}
					Vector3 thisVertex = vertices[pos];
					if (thisVertex.y >= centre.y){
						continue;
					}
					Vector3 slope = Vector3.Normalize (thisVertex - centre);
					//Debug.Log (thisVertex + " - " + centre + " = " + slope);
					if (slope.y < steepestSlope.y){
						steepestSlope = slope;
						ox = x;
						oy = y;
					}
				}
			}
			if (ox == x && oy == y) {
				vertices [myVertexOffset].y += carrying;
				return false;
			}
			vertices [y * size + x].y += Gouge (steepestSlope);
			//Debug.Log (x + ", " + y + " -> " + ox + ", " + oy + ", " + steepestSlope + " velocity: "+ velocity + ", carrying: " + carrying);
			x = ox;
			y = oy;
			return true;
		}

		public void Track(){
			x = Random.Range (0, size);
			y = Random.Range (0, size);
			velocity = 0f;
			carrying = 0f;
			while (Iterate());
		}
	}

	public void Weather(Vector3[] input, int size){
		Particle particle = new Particle ();
		particle.vertices = input;
		particle.carryingCapacity = carryingCapacity / size;
		particle.Size = size;
		for (int i = 0; i != particles; ++i){
			particle.Track ();
		}
	}
}