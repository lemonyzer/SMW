using UnityEngine;
using System;

[Serializable]
public class FallingPath : MovingPlatformPath {
	
	[SerializeField]
	public float startX;
	[SerializeField]
	public float startY;
	
	public FallingPath (float startX, float startY) : base (startX, startY)
	{
		this.startX = startX;
		this.startY = startY;
	}
}