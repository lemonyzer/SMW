using UnityEngine;
using System;

[Serializable]
public class MapTopLayer
{
	[SerializeField]
	int height;
	[SerializeField]
	int width;
	[SerializeField]
	MapTile[] mapTiles;
	
	public MapTopLayer (int x, int y)
	{
		width = x;
		height = y;
		mapTiles = new MapTile[x * y];
		int length = x * y;
		for (int i=0; i< length; i++)
		{
			mapTiles[i] = new MapTile ();
		}
		Debug.Log (this.ToString () + " width = " + width);
	}
	
	public MapTile GetTile (int x, int y) {
		return mapTiles [x + y*width];
	}
	
	public void SetTile (int x, int y, MapTile mapTile) {
		mapTiles [x + y*width] = mapTile;
	}
};