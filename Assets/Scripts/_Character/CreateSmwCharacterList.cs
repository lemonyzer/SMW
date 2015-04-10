#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateSmwCharacterList {


	[MenuItem("Assets/Create/SMW Character List SO")]
	public static SmwCharacterList Create()
	{
		SmwCharacterList asset = ScriptableObject.CreateInstance<SmwCharacterList>();

		AssetDatabase.CreateAsset(asset, "Assets/SMWCharacterList.asset");
		AssetDatabase.SaveAssets();
		return asset;
	}
}
#endif