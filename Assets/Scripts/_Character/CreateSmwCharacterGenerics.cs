#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateSmwCharacterGenerics {


	[MenuItem("Assets/Create/SMW Character Generics SO")]
	public static SmwCharacterGenerics Create()
	{
		SmwCharacterGenerics asset = ScriptableObject.CreateInstance<SmwCharacterGenerics>();

		AssetDatabase.CreateAsset(asset, "Assets/SMWCharacterGenerics.asset");
		AssetDatabase.SaveAssets();
		return asset;
	}
}
#endif