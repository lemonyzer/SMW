#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapList : MonoBehaviour {
	
	public int currentMap = 0;
	public List<GameObject> maps;

	public Color32 myColor = Color.magenta;
	public float handleSize = 1f;

	public void Awake()
	{
		if(maps == null)
		{
			maps = new List<GameObject>();
			currentMap = 0;
		}
	}

	public void ClearAndAddAllGameObjectWithTagMaps()
	{
		if(maps == null)
			maps = new List<GameObject>();

		maps.Clear();

		GameObject[] allMapsInscene = GameObject.FindGameObjectsWithTag(Tags.tag_Map);

		if(allMapsInscene == null)
		{
			Debug.LogError("No Map found! (No ACTIVE GameObject with Tag "+Tags.tag_Map+")");
		}

		foreach(GameObject mapGO in allMapsInscene)
		{
			maps.Add(mapGO);
		}
	}

	public void DeactiveAll()
	{
		if(maps == null)
			return;

		for(int i=0; i<maps.Count; i++)
		{
			if(maps[i] != null)
				maps[i].SetActive(false);
		}
	}

	public void Previouse()
	{
		int next = currentMap-1;
//		Debug.Log(next);
		if(next <0)
			next = maps.Count-1;	// (0) bis (maps.Count-1)

		if(next > maps.Count)
			next = 0;
//		Debug.Log(next);
		if(maps[currentMap] != null)
		{
			if(maps[next] != null)
			{
				maps[next].SetActive(true);
				maps[currentMap].SetActive(false);
				currentMap = next;
			}
		}
	}

	public void Next()
	{
		bool nextFound = false;
		int tryNum = 0;

		if(maps.Count == 0)
			return;

		if(currentMap >= maps.Count)
		{
			currentMap = 0;
		}

		if(maps[currentMap] != null)
		{
			maps[currentMap].SetActive(false);
		}
		while(!nextFound)
		{
			tryNum++;
			currentMap++;

			if(currentMap >= maps.Count)
			{
				currentMap = 0;
			}
//			if(currentMap < 0)
//			{
//				currentMap = maps.Count-1;
//			}

			if(maps[currentMap] != null)
			{
				maps[currentMap].SetActive(true);
				nextFound = true;
			}

			if(maps.Count <= 0)
				return;
			
			if(tryNum >= maps.Count)
				return;
		}

	}

	public void SelectCurrentMap()
	{
		GameObject currMap = GetCurrentMap();
		if(currMap != null)
			UnityEditor.Selection.activeGameObject = currMap;
	}

	public GameObject GetCurrentMap()
	{
		if(maps == null)
			return null;

		if( currentMap < 0 || currentMap > maps.Count)
			return null;

		return maps[currentMap];
	}
	
}
#endif