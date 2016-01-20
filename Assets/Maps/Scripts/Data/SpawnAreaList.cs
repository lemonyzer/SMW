using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SpawnAreaList
{
	[SerializeField]
	public List<SpawnArea> spawnAreaList;

	public SpawnAreaList ()
	{
		spawnAreaList = new List<SpawnArea> ();
	}
};