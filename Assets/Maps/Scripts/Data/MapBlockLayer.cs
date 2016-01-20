using UnityEngine;
using System;

[Serializable]
public class MapBlockLayer
{
	[SerializeField]
	int height;
	[SerializeField]
	int width;
	[SerializeField]
	MapBlock[] mapBlocksData;

	public int Height {
		get { return height; }
	}
	public int Width {
		get { return width; }
	}
	
	public MapBlockLayer (int x, int y)
	{
		width = x;
		height = y;
		mapBlocksData = new MapBlock[x * y];
		int length = x * y;
		for (int i=0; i< length; i++)
		{
			mapBlocksData[i] = new MapBlock ();
		}
//		Debug.Log (this.ToString () + " width = " + width);
	}
	
	public MapBlock GetBlock (int x, int y) {
		if (x >= 0 &&
		    x < width &&
		    y >= 0 &&
		    y < height)
			return mapBlocksData [x + y*width];
		else
			return null;
	}
	
	public void SetBlock (int x, int y, MapBlock mapBlock) {
		mapBlocksData [x + y*width] = mapBlock;
	}
};