using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SpawnAreaMap
{
	[SerializeField]
	//	SpawnArea[] map;
	List<SpawnAreaList> map;
	
	public SpawnAreaMap (int x)
	{
		map = new List<SpawnAreaList>();

		for (int i=0; i< x; i++)
		{
			map.Add (new SpawnAreaList ());
//			map[i] = new SpawnArea ();
		}
//		Debug.Log (this.ToString () + " width = " + width);
	}
	
	public SpawnArea GetElement (int x, int y) {
		return map [x].spawnAreaList[y];
	}
	
	public void SetElement (int x, int y, SpawnArea element) {
		map[x].spawnAreaList[y] = element;
	}

	public void AddElement (int x, SpawnArea element) {
		map[x].spawnAreaList.Add (element);
	}

};