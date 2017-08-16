using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class NoiseFilter : IHeightMapFilter{
	[Range(0.1f, 5f)]
	public float incoherence;
	[Range(1f, 5f)]
	public float overtoneShift;
	[Range(1f, 50f)]
	public float height;
	[Range(1, 5)]
	public int overtones;

	private readonly List<IHeightMapFilter> overtoneFilters = new List<IHeightMapFilter>();

	public void AddOvertoneFilter(IHeightMapFilter filter){
		overtoneFilters.Add (filter);
	}

	public void ClearOvertoneFilters(){
		overtoneFilters.Clear ();
	}

	private static void ApplyOvertone(HeightMap input, float height, float incoherence){
		float offsetX = UnityEngine.Random.value * 4000000f;
		float offsetY = UnityEngine.Random.value * 4000000f;
		int size = input.Size;
		float zScale = 1f / size;
		for (int y = 0, offset = 0; y != size; ++y) {
			for (int x = 0; x != size; ++x, ++offset) {
				input[offset] += Mathf.PerlinNoise (offsetX + x * incoherence, offsetY + y * incoherence) * zScale * height;
			}
		}
	}

	public HeightMap Filter(HeightMap input){
		float height = this.height, incoherence = this.incoherence;
		for (int i = 0; i != overtones; ++i, height *= 0.5f, incoherence *= overtoneShift){
			ApplyOvertone (input, height, incoherence);
			foreach (IHeightMapFilter f in overtoneFilters){
				input = f.Filter (input);
			}
		}
		return input;
	}
}