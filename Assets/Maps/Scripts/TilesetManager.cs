//#define NUM_POWERUPS 26
//#define NUM_BLOCK_SETTINGS NUM_POWERUPS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TilesetManager : ScriptableObject {

	// bei Programmstart lade alle Tilesets
	[SerializeField]
	short iClassicTilesetIndex = 0;
	[SerializeField]
	List<Tileset> tilesetList;
	[SerializeField]
	Tileset animationTileset;
	[SerializeField]
	Tileset blockTileset;
	[SerializeField]
	Tileset unknownTileset;
//	Tileset tClassicTileset;

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
			Debug.Log((tilesetList[i].tilesetName.ToLower().Equals(Name.ToLower()) ? "<color=green>Check</color>" : "<color=red>Check</color>")+"\n"+
			          tilesetList[i].tilesetName.ToLower()+"|"+"\n"+
			          Name.ToLower()+"|"+"\n"+
			          (tilesetList[i].tilesetName.ToLower().Equals(Name.ToLower()) ? "<color=green>true</color>" : "<color=red>false</color>") );
			if(tilesetList[i].tilesetName.ToLower().Equals(Name.ToLower()))
				return i;
		}
		
		return (int) Globals.TILESETUNKNOWN;
	}

	public Tileset GetTileset(int index)
	{

		if(index == Globals.TILESETNONE)
		{
			Debug.LogError("GetTileset() spezial: TILESETNONE: " + Globals.TILESETNONE);
			return null;
		}
		if(index == Globals.TILESETUNKNOWN)
		{
			Debug.LogError("GetTileset() spezial: TILESETUNKNOWN:" + Globals.TILESETUNKNOWN);
			if(unknownTileset == null)
				Debug.LogError(this.ToString() + " TilesetManager unknownTileset missing");
				
			return unknownTileset;
		}
		if(index == Globals.TILESETANIMATED)
		{
			Debug.LogError("GetTileset() spezial: TILESETANIMATED: " + Globals.TILESETANIMATED);
			if(animationTileset == null)
				Debug.LogError(this.ToString() + " TilesetManager animationTileset missing");

			return animationTileset;
		}

		if(index < 0 || index > tilesetList.Count)
		{
			Debug.LogError(this.ToString() + " Index " + index + " > tilesetList.Count " + tilesetList.Count);
			return null;
//			return (int) Globals.TILESETUNKNOWN;
		}
		return tilesetList[index];
	}

	public Tileset GetClassicTileset()
	{
		return GetTileset(iClassicTilesetIndex);
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
