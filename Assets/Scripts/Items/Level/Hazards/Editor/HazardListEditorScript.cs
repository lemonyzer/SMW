using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Hazards)), CanEditMultipleObjects]
public class HazardListEditorScript : Editor {

//	[MenuItem("")]
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		if (Event.current.type == EventType.Layout)
			return;

		Rect position = new Rect (100,70, Screen.width, Screen.height);

		foreach (var item in targets)
		{
			if (position.height < EditorGUIUtility.singleLineHeight*2)
				continue;

			Hazards hazardList = item as Hazards;
			Rect usedRect = InspectHazards (position, hazardList);
		}
	}

	public static Rect InspectHazards (Rect position, Hazards hazards)
	{
//		GUI.changed = false;

//		EditorGUI.IntField (new Rect(position.x, position.y, position.width *0.5f, EditorGUIUtility.singleLineHeight), hazards.GetAmount ());

		if (GUI.Button (new Rect(position.x, position.y, position.width *0.5f, EditorGUIUtility.singleLineHeight), "init"))
		{
			hazards.Init ();
		}

		return position;
	}

}
