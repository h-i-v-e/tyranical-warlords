using System;
using UnityEngine;

public struct NoiseLayer
{
	public float coherence;

	public NoiseLayer (float coherence)
	{
		this.coherence = coherence;
	}

	public void Apply(Vector3[] vertices, int size, float height){
		float offsetX = UnityEngine.Random.value * 1000000f;
		float offsetY = UnityEngine.Random.value * 1000000f;
		for (int y = 0, offset = 0; y != size; ++y, offset += size) {
			for (int x = 0; x != size; ++x) {
				vertices [offset + x].y += Mathf.PerlinNoise (offsetX + x * coherence, offsetY + y * coherence) * height;
			}
		}
	}
}

