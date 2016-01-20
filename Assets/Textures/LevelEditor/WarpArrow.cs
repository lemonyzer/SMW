using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarpArrow : ScriptableObject {

//	private static WarpArrow instance = null;
//
//	public static WarpArrow Instance {
//		get {
//			if (instance == null)
//				instance = new WarpArrow ();
//			return instance;
//		}
//	}
	public List<Sprite> warpArrows;
	public bool init = false;
	public Sprite[] north;
	public Sprite[] east;
	public Sprite[] south;
	public Sprite[] west;

	public void OnEnable ()
	{
		if (init)
		{
			return;
		}
		for (int i=0; i< 10; i++)
		{
			warpArrows.Add (north[i]);
		}
		for (int i=0; i< 10; i++)
		{
			warpArrows.Add (east[i]);
		}
		for (int i=0; i< 10; i++)
		{
			warpArrows.Add (south[i]);
		}
		for (int i=0; i< 10; i++)
		{
			warpArrows.Add (west[i]);
		}
		init = true;
				#if UNITY_EDITOR
		UnityEditor.EditorUtility.SetDirty (this);
				#endif
	}

	public Sprite GetExitArrow (int direction, int connection)
	{
		return warpArrows [direction*10 + connection];
	}
	public Sprite GetEnterArrow (int direction, int connection)
	{
		if (direction != -1)
		{
			int dir = (direction + 2) % 4;
			return GetExitArrow (dir, connection);
		}
		else
			return null;
	}
}
