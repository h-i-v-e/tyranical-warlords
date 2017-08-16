using UnityEngine;

public class CoastFilter : IHeightMapFilter{
	public HeightMap Filter(HeightMap map){
		int size = map.Size;
		float drop = (float)size;
		float piInvRadius = Mathf.PI * 0.5f / drop, yDist = drop;
		for (int y = 0, offset = 0; y != size; ++y, yDist -= 1f){
			for (int x = 0; x != size; ++x){
				float rise = Mathf.Cos(yDist * piInvRadius);
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
