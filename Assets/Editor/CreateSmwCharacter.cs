using UnityEngine;
using System.Collections;

using UnityEditor;

public class CreateSmwCharacter : MonoBehaviour {

	[MenuItem("Assets/Create/SMW Character SO")]
	public static void CreateAsset()
	{
		SmwCharacter asset = ScriptableObject.CreateInstance<SmwCharacter>();

		AssetDatabase.CreateAsset(asset, "Assets/newSmwCharacterSO.asset");
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
	}

	public static SmwCharacter CreateAssetAndSetup()
	{
		SmwCharacter asset = ScriptableObject.CreateInstance<SmwCharacter>();
		
		AssetDatabase.CreateAsset(asset, "Assets/newSmwCharacterSO.asset");
		AssetDatabase.SaveAssets();
		
//		EditorUtility.FocusProjectWindow();
//		Selection.activeObject = asset;

		return asset;
	}

	public static SmwCharacter CreateAssetWithPathAndName(string relPath, string name)
	{
		SmwCharacter asset = ScriptableObject.CreateInstance<SmwCharacter>();
		asset.charName = name;
		AssetDatabase.CreateAsset(asset, relPath + "/" + name + ".asset");
		AssetDatabase.SaveAssets();
		
//		EditorUtility.FocusProjectWindow();
//		Selection.activeObject = asset;
		
		return asset;
	}
}
