using UnityEngine;
using System;

[Serializable]
public class WarpMap
{
	[SerializeField]
	int height;
	[SerializeField]
	int width;
	[SerializeField]
	Warp[] warpMap;

	public int GetHeight()
	{
		return height;
	}

	public int GetWidth()
	{
		return width;
	}
	
	public WarpMap (int x, int y)
	{
		width = x;
		height = y;
		warpMap = new Warp[x * y];
		int length = x * y;
		for (int i=0; i< length; i++)
		{
			warpMap[i] = new Warp ();
		}
		Debug.Log (this.ToString () + " width = " + width);
	}
	
	public Warp GetField (int x, int y) {
		return warpMap [x + y*width];
	}
	
	public void SetField (int x, int y, Warp warp) {
		warpMap [x + y*width] = warp;
	}
};