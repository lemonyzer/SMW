using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MapList))]
public class MapListEditor : Editor {
	
	MapList targetObject;

	void OnEnable()
	{
		targetObject = (MapList) target;
	}

	public override void OnInspectorGUI ()
	{
		//base.OnInspectorGUI ();
		DrawDefaultInspector();

		GUILayout.Space(10);
		GUIElemts();
	}

	void OnSceneGUI()
	{
		// Handles for Scene View
		Handles.color = targetObject.myColor;
//		Handles.CubeCap (0, targetObject.transform.position, targetObject.transform.rotation, targetObject.handleSize);
		Handles.SphereCap (0, targetObject.transform.position, targetObject.transform.rotation, targetObject.handleSize);
		Handles.Label (targetObject.transform.position + new Vector3(0f, targetObject.handleSize, 0f), targetObject.name);

		// 2D GUI for Scene View
		Handles.BeginGUI();
		GUILayout.BeginArea (new Rect (10f, 10f, 100f, 400f));
		//Handles.Button("Next Map");
		GUIElemts();
		GUILayout.EndArea();
		Handles.EndGUI();
	}

	void GUIElemts()
	{
		if(GUILayout.Button("Next Map"))
		{
			targetObject.Next();
		}
		if(GUILayout.Button("Previouse Map"))
		{
			targetObject.Previouse();
		}
		if(GUILayout.Button("Select Map"))
		{
			targetObject.SelectCurrentMap();
		}
		if(GUILayout.Button("Deactivate All"))
		{
			targetObject.DeactiveAll();
		}
		if(GUILayout.Button("Init MapsList (active GameObjects with Tag " + Tags.tag_Map))
		{
			targetObject.ClearAndAddAllGameObjectWithTagMaps();
		}
	}

}
