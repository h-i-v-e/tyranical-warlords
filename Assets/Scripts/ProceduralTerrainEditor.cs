using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ProceduralTerrain))]
public class ProceduralTerrainEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		ProceduralTerrain terrain = (ProceduralTerrain)target;
		if(GUILayout.Button("Generate"))
		{
			terrain.GenerateSharedMesh ();
		}
		else if (GUILayout.Button("Randomize")){
			terrain.seed = Random.Range(0, int.MaxValue);
		}
	}
}

