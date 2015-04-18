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


	short iWidth, iHeight;
	short iTileTypeSize;
	TileType[] tiletypes;

	public void OnEnable()
	{
		Debug.Log(this.ToString() + " OnEnable()");
	}

	public TileType GetTileType(short iTileCol, short iTileRow)
	{
		return tiletypes[iTileCol + iTileRow * iWidth];
	}

	void SetTileType(short iTileCol, short iTileRow, TileType type)
	{
		tiletypes[iTileCol + iTileRow * iWidth] = type;
	}

	public TileType IncrementTileType(short iTileCol, short iTileRow)
	{
		short iTile =((short)((int) iTileCol + (int) iTileRow * (int) iWidth));
		tiletypes[iTile] = GetIncrementedTileType(tiletypes[iTile]);
		
		return tiletypes[iTile];
	}

	public TileType GetIncrementedTileType(TileType type)
	{
		if(type == TileType.tile_nonsolid)
			return TileType.tile_solid;
		else if(type == TileType.tile_solid)
			return TileType.tile_solid_on_top;
		else if(type == TileType.tile_solid_on_top)
			return TileType.tile_ice;
		else if(type == TileType.tile_ice)
			return TileType.tile_death;
		else if(type == TileType.tile_death)
			return TileType.tile_death_on_top;
		else if(type == TileType.tile_death_on_top)
			return TileType.tile_death_on_bottom;
		else if(type == TileType.tile_death_on_bottom)
			return TileType.tile_death_on_left;
		else if(type == TileType.tile_death_on_left)
			return TileType.tile_death_on_right;
		else if(type == TileType.tile_death_on_right)
			return TileType.tile_ice_on_top;
		else if(type == TileType.tile_ice_on_top)
			return TileType.tile_ice_death_on_bottom;
		else if(type == TileType.tile_ice_death_on_bottom)
			return TileType.tile_ice_death_on_left;
		else if(type == TileType.tile_ice_death_on_left)
			return TileType.tile_ice_death_on_right;
		else if(type == TileType.tile_ice_death_on_right)
			return TileType.tile_super_death;
		else if(type == TileType.tile_super_death)
			return TileType.tile_super_death_top;
		else if(type == TileType.tile_super_death_top)
			return TileType.tile_super_death_bottom;
		else if(type == TileType.tile_super_death_bottom)
			return TileType.tile_super_death_left;
		else if(type == TileType.tile_super_death_left)
			return TileType.tile_super_death_right;
		else if(type == TileType.tile_super_death_right)
			return TileType.tile_player_death;
		else if(type == TileType.tile_player_death)
			return TileType.tile_nonsolid;
		
		return TileType.tile_nonsolid;
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
