//#define NUM_POWERUPS 26
//#define NUM_BLOCK_SETTINGS NUM_POWERUPS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Eyecandy
{
	int id;
	// 1 = clouds
	// 2 = ghosts
	// 5 = leafs
	// 9 = snow
	// 16 = fish
	// 32 = rain
	// 80 = bubbles
}

[System.Serializable]
public class EyecandyManager
{
	[SerializeField]
	List<Eyecandy> list;
}


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
	Tileset levelEditorTileTypeTileset;
	[SerializeField]
	EyecandyManager eyecandyManager;
	[SerializeField]
	Tileset unknownTileset;
//	Tileset tClassicTileset;
	[SerializeField]
	WarpArrow warpArrows;

	[SerializeField]
	public List<string> typeConversions;

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
//			Debug.Log("GetTileset() spezial: TILESETANIMATED: " + Globals.TILESETANIMATED);
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

	public Tileset GetBlockTileset ()
	{
		return blockTileset;
	}

	public Tileset GetLevelEditorTileTypeTileset ()
	{
		return levelEditorTileTypeTileset;;
	}

	[SerializeField]
	int blockTileSetWidth = 15;

	public Sprite GetBlockSprite (int index)
	{
		int x = index % blockTileSetWidth;
		int y = 0;
//		Debug.LogWarning ("blockTileSetWidth= " + blockTileSetWidth);
//		Debug.LogWarning ("index= " + index);
//		Debug.LogWarning ("x= " + x);
//		Debug.LogWarning ("y= " + y);
		for (int i=0; i<3; i++) 
		{
			if (index >= blockTileSetWidth*i && 
			    index < blockTileSetWidth*(i+1))
			{
				y = i;
				break;
			}
		}
//		Debug.LogWarning ("yT= " + y);
		return blockTileset.GetTileSprite (x, y);
	}

	public WarpArrow GetWarpArrow ()
	{
		return warpArrows;
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
