using UnityEngine;
using System;

[Serializable]
public class MapTile
{
	[SerializeField]
	public TileType iType;
	[SerializeField]
	public int iFlags;
};