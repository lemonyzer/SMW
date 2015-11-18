using UnityEngine;
using UnityEditor;
using System.Collections;

public class WarpArrowEditorScript : EditorWindow {

	[MenuItem("SMW/WarpArrows")]
	public static WarpArrow CreateScriptableObject () {

		WarpArrow newAsset = ScriptableObject.CreateInstance<WarpArrow>();

		AssetDatabase.CreateAsset(newAsset, "Assets/warparrow.asset");
		AssetDatabase.SaveAssets();
		
		//		EditorUtility.FocusProjectWindow();
		Selection.activeObject = newAsset;
		
		return newAsset;

	}
}
