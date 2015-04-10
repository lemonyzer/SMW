using UnityEngine;
using System.Collections;
using UnityEditor;

public class LevelDesignerMenu : Editor {

	[MenuItem("GameObject/Create Other/00Level Designer")]
	public static void ShowLevelDesigner()
	{
		GameObject go = new GameObject();
		go.name = "00Level Designer";
		go.AddComponent<LevelDesigner>();
		GameObject[] selected = new GameObject[1];
		selected[0]=go;
		Selection.objects = selected;		//makierte Elemente überschreiben!
	}

}
