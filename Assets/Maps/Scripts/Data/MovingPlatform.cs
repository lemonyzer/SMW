using UnityEngine;
using System;

[Serializable]
public class MovingPlatform {

//	[SerializeField]
//	public int iPathType;
	
	[SerializeField]
	public int iPlatformWidth;
	[SerializeField]
	public int iPlatformHeight;
	[SerializeField]
	public MovingPlatformPath path;
	[SerializeField]
	public MapLayer platformTiles;
	[SerializeField]
	public MapLayer platformTilesRaw;
	[SerializeField]
	public MapTopLayer platformTileTypes;
//	[SerializeField]
//	public short iWidth;
//	[SerializeField]
//	public short iHeight;
	[SerializeField]
	public short iDrawLayer;
	[SerializeField]
	public bool fPreview;
	
	public MovingPlatform(MapLayer platformTiles, MapLayer platformTilesRaw, MapTopLayer platformTileTypes, short iWidth, short iHeight, short iDrawLayer, MovingPlatformPath path, bool fPreview)
	{
		this.platformTiles = platformTiles;
		this.platformTilesRaw = platformTilesRaw;
		this.platformTileTypes = platformTileTypes;
		this.iPlatformWidth = iWidth;
		this.iPlatformHeight = iHeight;
//		this.iWidth = iWidth;
//		this.iHeight = iHeight;
		this.iDrawLayer = iDrawLayer;
		this.path = path;
		this.fPreview = fPreview;
	}
}