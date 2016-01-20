using UnityEngine;
using System;

[Serializable]
public class MapLayer
{

	//1D 		TilesetTile[] tiles;
	//2D 		TilesetTile[,] tiles;
	//3D 		TilesetTile[,,] tiles;
	//Jagged	TilesetTile[][] tiles;

	[SerializeField]
	int height;
	[SerializeField]
	int width;
	[SerializeField]
	TilesetTile[] tiles;

	public MapLayer (int x, int y)
	{
		width = x;
		height = y;
		tiles = new TilesetTile[x * y];
		int length = x * y;
		for (int i=0; i< length; i++)
		{
			tiles[i] = new TilesetTile ();
		}
//		Debug.Log (this.ToString () + " width = " + width);
	}

	public TilesetTile GetTile (int x, int y) {
		return tiles [x + y*width];
	}

	public void SetTile (int x, int y, TilesetTile tile) {
		tiles [x + y*width] = tile;
	}

}