using UnityEngine;
using System.Collections;

public class LevelLibrary : MonoBehaviour {

	public GameObject[] levels;

	public GameObject GetLevel(int index)
	{
		if(index >=0 && index < levels.Length)
		{
			return levels[index];
		}

		return null;
	}
}
