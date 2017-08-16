using UnityEngine;

public class IslandFilter : IHeightMapFilter{
	public HeightMap Filter(HeightMap map){
		int size = map.Size;
		float radius = size * 0.5f;
		float piInvRadius = Mathf.PI * 0.5f / radius, yDist = radius;
		for (int y = 0, offset = 0; y != size; ++y, yDist -= 1f){
			float yy = yDist * yDist, xDist = radius;
			for (int x = 0; x != size; ++x, xDist -= 1f){
				float rise = Mathf.Cos(Mathf.Sqrt ((xDist * xDist) + yy) * piInvRadius);
				if (rise > 0f) {
					map [offset++] *= rise;
				} else {
					map [offset++] = 0f;
				}
			}
		}
		return map;
	}
}
