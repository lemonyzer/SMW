//#define NUM_POWERUPS 26
//#define NUM_BLOCK_SETTINGS NUM_POWERUPS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TilesetManager : ScriptableObject {

	// bei Programmstart lade alle Tilesets
	[SerializeField]
	List<CTiletset> tilesetList;

	public void OnEnable()
	{
		Debug.Log("OnEnable()");
		if(tilesetList == null)
		{
			Debug.Log("tilesetList == null");
			tilesetList = new List<CTiletset>();
		}
	}

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
