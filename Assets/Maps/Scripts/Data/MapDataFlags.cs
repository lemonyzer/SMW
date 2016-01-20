using UnityEngine;
using System;

[Serializable]
public class MapDataFlags
{
	[SerializeField]
	int height;
	[SerializeField]
	int width;
	[SerializeField]
	bool[] mapData;
	
	public MapDataFlags (int x, int y)
	{
		width = x;
		height = y;
		mapData = new bool[x * y];
		int length = x * y;
		for (int i=0; i< length; i++)
		{
			mapData[i] = new bool ();
		}
//		Debug.Log (this.ToString () + " width = " + width);
	}
	
	public bool GetField (int x, int y) {
		return mapData [x + y*width];
	}
	
	public void SetField (int x, int y, bool field) {
		mapData [x + y*width] = field;
	}
};