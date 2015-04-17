using UnityEngine;
using System.Collections;

[System.Serializable]
public class CTiletset : ScriptableObject {

	[SerializeField]
	private Texture tilesetTexture;
	[SerializeField]
	private Texture2D tilesetTexture2D;
	[SerializeField]
	private Sprite tileset;
	[SerializeField]
	private GUITexture tilesetGUITexture;
	[SerializeField]
	private string tilesetName;
	[SerializeField]
	private int height;
	[SerializeField]
	private int width;
	
	public string Name {
		get {
			return this.tilesetName;
		}
		set {
			tilesetName = value;
		}
	}
	
	public int Height {
		get {
			return this.height;
		}
		set {
			height = value;
		}
	}
	
	public int Width {
		get {
			return this.width;
		}
		set {
			width = value;
		}
	}
	
}
