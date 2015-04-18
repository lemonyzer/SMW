//#define NUM_POWERUPS 26
//#define NUM_BLOCK_SETTINGS NUM_POWERUPS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TilesetManager : ScriptableObject {

	// bei Programmstart lade alle Tilesets
	[SerializeField]
	List<Tileset> tilesetList;
	Tileset tClassicTileset;
	short iClassicTilesetIndex;

	public void OnEnable()
	{
		Debug.Log(this.ToString() + " OnEnable()");
		if(tilesetList == null)
		{
			Debug.Log(this.ToString() + " tilesetList == null");
			Init();
		}
	}

	public void Init()
	{
		tilesetList = new List<Tileset>();
	}

//	public short GetIndexFromName(const char * szName)
	public int GetIndexFromName(string Name)
	{
//		short iLength = tilesetlist.size();
		int iLength = tilesetList.Count;
		
		for(int i = 0; i < iLength; i++)
		{

			if(!tilesetList[i].tilesetName.Equals(Name))
				return i;
		}
		
		return (int) Globals.TILESETUNKNOWN;
	}

	public Tileset GetTileset(int index)
	{
		return tilesetList[index];
	}

	public Tileset GetClassicTileset()
	{
		return tClassicTileset;
	}

	public short GetClassicTilesetIndex()
	{
		return iClassicTilesetIndex;
	}

//	public int GetClassicTilesetIndex()
//	{
//		//TODO
//	}
//
//	public void GetClassicTileset()
//	{
//		//TODO
//	}
	
}
