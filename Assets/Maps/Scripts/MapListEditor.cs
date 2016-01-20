#if UNITY_EDITOR
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

	int x = 0;
	int y = 0;
	bool pixelFormat = false;
	bool hazard = false;
	bool hazardOrbit = false;

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

		x = EditorGUILayout.IntField ("X=" + x,x);
		y = EditorGUILayout.IntField ("Y=" + y,y);
		int.TryParse (GUILayout.TextField (x + ""),out x);
		int.TryParse (GUILayout.TextField (y + ""),out y);
		//float.TryParse

		pixelFormat = GUILayout.Toggle (pixelFormat, "Pixel Format");
		hazard = GUILayout.Toggle (hazard, "Hazard");
		hazardOrbit = GUILayout.Toggle (hazardOrbit, "Hazard Orbit");

		if(GUILayout.Button("Draw Position"))
		{
			Vector3 position = Vector3.zero;
			if (hazard)
			{
				if (hazardOrbit)
					position = TransformOrbitHazardPositionToPixelPosition (new Vector3 (x,y,0));
				else
					position = TransformHazardPositionToPixelPosition (new Vector3 (x,y,0));

				position = TransformPixelPositionToUnityWorldPosition (position);
			}
			else 
			{
				if (pixelFormat)
					position = TransformPixelPositionToUnityWorldPosition (new Vector2 (x,y));
				else 
					position = TransformPositionToUnityPosition (new Vector2 (x,y));
			}

			Vector3 help = new Vector3 (1,1,0);
			Debug.DrawLine (position - help * 0.5f, position + help * 0.5f, Color.green, 2f);
			Debug.DrawLine (position + help * 0.5f, position - help * 0.5f, Color.green, 2f);

			Debug.DrawLine (position +Vector3.left+Vector3.up, position +Vector3.right+Vector3.down, Color.green, 2f);
			Debug.DrawLine (position +Vector3.left+Vector3.down, position +Vector3.right+Vector3.up, Color.green, 2f);
				
		}
	}

	public static Vector3 TransformOrbitHazardPositionToPixelPosition (Vector3 originalHazardPosition)
	{
		Vector3 pixelPosition = Vector3.zero;
		pixelPosition.x = (((int) originalHazardPosition.x) << 4) + 16;
		pixelPosition.y = (((int) originalHazardPosition.y) << 4) + 16;
		Debug.Log ("pixelPosition = " + pixelPosition);
		return pixelPosition;
	}

	public static Vector3 TransformHazardPositionToPixelPosition (Vector3 originalHazardPosition)
	{
		Vector3 pixelPosition = Vector3.zero;
		pixelPosition.x = ((int) originalHazardPosition.x) << 4;
		pixelPosition.y = +32 + (((int) originalHazardPosition.y) << 4);
		Debug.Log ("pixelPosition = " + pixelPosition);
		return pixelPosition;
	}
	
	public static Vector3 TransformPixelPositionToUnityWorldPosition (Vector3 originalPixelPosition)
	{
		Vector3 unityPosition = Vector3.zero;
		unityPosition.x = TransformXPosToUnityXPosition (originalPixelPosition.x /32.0f);
		unityPosition.y = TransformYPosToUnityYPosition (originalPixelPosition.y /32.0f);
		return unityPosition;
	}
	
	public static Vector3 TransformPositionToUnityPosition (Vector3 originalPosition)
	{
		Vector3 unityPosition = Vector3.zero;
		unityPosition.x = TransformXPosToUnityXPosition (originalPosition.x);
		unityPosition.y = TransformYPosToUnityYPosition (originalPosition.y);
		return unityPosition;
	}
	
	public static float TransformXPosToUnityXPosition (float originalXPosition)
	{
		return originalXPosition - 10.0f;
	}
	
	public static float TransformYPosToUnityYPosition (float originalYPosition)
	{
		return 15f - originalYPosition - 7.5f;
	}

}
#endif