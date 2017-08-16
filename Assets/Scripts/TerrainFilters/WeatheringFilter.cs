using UnityEngine;

[System.Serializable]
public class WeatheringFilter
{

	public HeightMap Filter(HeightMap input){
		Particle particle = new Particle ();
		particle.carryingCapacity = carryingCapacity;
		particle.HeightMap = input;
		for (int i = 0; i != particles; ++i){
			particle.Track ();
		}
		return input;
	}

	public int particles;
	[Range(0f, 1f)]
	public float carryingCapacity;

	private struct Particle{
		public float carryingCapacity;
		private HeightMap heightMap;
		private int last;
		private Coord coord;

		public HeightMap HeightMap{
			get{
				return heightMap;
			}
			set{
				last = value.Size - 1;
				heightMap = value;
			}
		}

		float velocity, carrying;


		int GetLast(int value){
			return --value < 0 ? 0 : value;
		}

		int GetNext(int value){
			return ++value > last ? last : value;
		}

		float Gouge(float slope){
			float newVelocity = (velocity - slope) * 0.5f;
			float canCarry = (newVelocity * newVelocity) * carryingCapacity;
			float o = carrying - canCarry;
			carrying = canCarry;
			velocity = newVelocity;
			return o;
		}

		void AddDiagonal(Coord dst, float adding){
			//float lesser = adding * 0.25f;
			//heightMap [coord.x, dst.y] += lesser;
			//heightMap [dst.x, coord.y] += lesser;
			heightMap [coord] += adding/* * 0.5f*/;
		}

		bool Iterate(){
			int size = heightMap.Size;
			float steepestSlope = 1f, centre = heightMap[coord];
			int lastX = GetLast(coord.x), nextX = GetNext(coord.x), lastY = GetLast(coord.y), nextY = GetNext(coord.y);
			Coord next = coord;
			for (int y = lastY; y <= nextY; ++y){
				int yoff = y * size;
				for (int x = lastX; x <= nextX; ++x){
					int pos = yoff + x;
					float dif = heightMap[pos] - centre;
					if (dif >= 0){
						continue;
					}
					if (dif < steepestSlope){
						steepestSlope = dif;
						next.Set(x, y);
					}
				}
			}
			if (coord == next) {
				//Debug.Log ("Adding: " + carrying);
				//Debug.Log ("Before: " + heightMap [coord]);
				heightMap [coord] += carrying;
				//Debug.Log ("After: " + heightMap [coord]);
				return false;
			}
			float adding = Gouge (steepestSlope);
			if (next.x != coord.x && next.y != coord.y){
				AddDiagonal (next, adding);
			}
			else{
				heightMap [next] += adding;
			}
			//Debug.Log (coord + " -> " + next + ", adding: " + adding + ", steepest: " + steepestSlope + " velocity: "+ velocity + ", carrying: " + carrying);
			coord = next;
			return true;
		}

		public void Track(){
			coord = new Coord (Random.Range (0, heightMap.Size), Random.Range (0, heightMap.Size));
			velocity = 0f;
			carrying = 0f;
			while (Iterate());
		}
	}
}

