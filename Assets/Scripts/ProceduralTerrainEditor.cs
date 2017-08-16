using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(ProceduralTerrain))]
public class ProceduralTerrainEditor : Editor
{
	private static readonly string[] typeOptions = new string[] {
		"Island", "Coast"
	};

	private int index = 0;

	private void Generate (ProceduralTerrain terrain)
	{
		switch (index) {
		case 0:
			terrain.typeFilter = new IslandFilter ();
			break;
		default:
			terrain.typeFilter = new CoastFilter ();
			break;
		}
		terrain.GenerateSharedMesh ();
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		ProceduralTerrain terrain = (ProceduralTerrain)target;
		index = EditorGUILayout.Popup ("Type", index, typeOptions);
		if (GUILayout.Button ("Generate")) {
			Generate (terrain);
		} else if (GUILayout.Button ("Randomize")) {
			terrain.seed = Random.Range (0, int.MaxValue);
		}
	}
}

