using UnityEngine;
//using UnityEditor;
using System.Collections;
//using System.Collections.Generic;


[System.Serializable]
public class Tileset : ScriptableObject {

//	[SerializeField]
//	private Texture tilesetTexture;
//	[SerializeField]
//	private Texture2D tilesetTexture2D;
//	[SerializeField]
//	private GUITexture tilesetGUITexture;

//	[SerializeField]
//	public SpriteMetaData[] spriteMetaData;
//	[SerializeField]
//	public Sprite[] tilesetArray;

	[SerializeField]
	public string tilesetName;
	[SerializeField]
	public Sprite tileset;
	[SerializeField]
	public int width;
	[SerializeField]
	public int height;

	public void OnEnable()
	{
		Debug.Log(this.ToString() + " OnEnable()");
	}
	
//	public string Name {
//		get {
//			return this.tilesetName;
//		}
//		set {
//			tilesetName = value;
//		}
//	}
//	
//	public int Height {
//		get {
//			return this.height;
//		}
//		set {
//			height = value;
//		}
//	}
//	
//	public int Width {
//		get {
//			return this.width;
//		}
//		set {
//			width = value;
//		}
//	}
	
}
