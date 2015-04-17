//#define NUM_POWERUPS 26
//#define NUM_BLOCK_SETTINGS NUM_POWERUPS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct MapTile
{
	public TileType iType;
	public int iFlags;
};

public struct MapBlock
{
	public short iType;
//	public short iSettings[NUM_BLOCK_SETTINGS];
	public short[] iSettings;
	public bool fHidden;

	public MapBlock(short iType)
	{
		this.iType = iType;
		iSettings = new short[Globals.NUM_BLOCK_SETTINGS];
		fHidden = false;
	}
};

public struct TilesetTile
{
	public short iID;
	public short iCol;
	public short iRow;
};

public class CTiletset {

	private string name;
	private int height;
	private int width;

	public string Name {
		get {
			return this.name;
		}
		set {
			name = value;
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

public enum TilesetIndex {
	TILESETUNKNOWN = -3,
};

public class TilesetManager {

	// bei Programmstart lade alle Tilesets
	List<CTiletset> tilesetList;

	public void Init()
	{
		tilesetList = new List<CTiletset>();
	}

//	public short GetIndexFromName(const char * szName)
	public int GetIndexFromName(string Name)
	{
//		short iLength = tilesetlist.size();
		int iLength = tilesetList.Count;
		
		for(int i = 0; i < iLength; i++)
		{

			if(!tilesetList[i].Name.Equals(Name))
				return i;
		}
		
		return (int) TilesetIndex.TILESETUNKNOWN;
	}

	public CTiletset GetTileset(int index)
	{
		return tilesetList[index];
	}
	
}
