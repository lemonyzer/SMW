using UnityEngine;
using System.Collections;
using UnityEditor;

public class HazardEditorScript : Editor {

	[MenuItem("SMW/Hazards/Create")]
	public static Hazards Create()
	{
		Hazards newTilesetAsset = ScriptableObject.CreateInstance<Hazards>();
		
		AssetDatabase.CreateAsset(newTilesetAsset, "Assets/newHazardListSO.asset");
		AssetDatabase.SaveAssets();
		
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = newTilesetAsset;
		
		return newTilesetAsset;
	}
}
