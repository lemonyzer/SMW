using UnityEngine;
using System;

[Serializable]
public class SwitchTargetBlock
{
	[SerializeField]
	public int x;
	
	[SerializeField]
	public int y;

	[SerializeField]
	MapBlock mapBlock;

	public SwitchTargetBlock (int x, int y, MapBlock block)
	{
		this.x = x;
		this.y = y;
		this.mapBlock = block;
	}
}