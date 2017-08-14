﻿using UnityEngine;


[System.Serializable]
public class Weathering{

	public int particles;
	[Range(0f, 1f)]
	public float carryingCapacity;

	private struct Particle{
		public float carryingCapacity;
		public Vector3[] vertices;
		private int size, last;
		private Coord coord;

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

		void AddDiagonal(Coord dst, float adding){
			float lesser = adding * 0.166f;
			vertices [dst.y * size + coord.x].y += lesser;
			vertices [coord.y * size + dst.x].y += lesser;
			vertices [coord.Offset(size)].y += adding * 0.667f;
		}

		bool Iterate(){
			if (coord.x == coord.y){
				return false;
			}
			int myVertexOffset = coord.y * size + coord.x;
			Vector3 steepestSlope = Vector3.up, centre = vertices[myVertexOffset];
			int lastX = GetLast(coord.x), nextX = GetNext(coord.x), lastY = GetLast(coord.y), nextY = GetNext(coord.y);
			Coord next = new Coord (coord.x, coord.y);
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
						next.Set(x, y);
					}
				}
			}
			if (next.x == next.y){
				return false;
			}
			if (coord == next) {
				vertices [myVertexOffset].y += carrying;
				return false;
			}
			float adding = Gouge (steepestSlope);
			if (next.x != centre.x && next.y != centre.y){
				AddDiagonal (next, adding);
			}
			else{
				vertices [myVertexOffset].y += adding;
			}
			//Debug.Log (x + ", " + y + " -> " + ox + ", " + oy + ", " + steepestSlope + " velocity: "+ velocity + ", carrying: " + carrying);
			coord = next;
			return true;
		}

		public void Track(){
			coord = new Coord (Random.Range (0, size), Random.Range (0, size));
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