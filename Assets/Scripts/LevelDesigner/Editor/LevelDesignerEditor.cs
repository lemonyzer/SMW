using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LevelDesigner))]

public class LevelDesignerEditor : Editor {

	LevelDesigner script;
	BatchMode batchmode = BatchMode.None;
	bool leftControl = false;
	Vector2 oldTilePos = new Vector2();
	bool usePrefix = true;

	enum BatchMode
	{
		Create,
		Delete,
		None
	}

	//wenn das aktuelle Skript aktiv wird
	void OnEnable()
	{
		script = (LevelDesigner) target;

		if(!Application.isPlaying)
		{
			if(SceneView.lastActiveSceneView != null)
			{
				Tools.current = Tool.View;
				//SceneView.lastActiveSceneView.orthographic = true;
				SceneView.lastActiveSceneView.in2DMode = true;
				SceneView.lastActiveSceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, Quaternion.identity);
				leftControl = false;
				batchmode = BatchMode.None;
			}

		}

	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Prefab");
		script.prefab = (GameObject) EditorGUILayout.ObjectField (script.prefab, typeof(GameObject), false);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Sprite");
		script.sprite = (Sprite) EditorGUILayout.ObjectField (script.sprite, typeof(Sprite), false);
		if(script.sprite != null)
		{
			if(script.sprite.bounds.center != Vector3.zero)
			{
				script.useOffset = true;
				script.offsetX = script.sprite.bounds.center.x;
				script.offsetY = script.sprite.bounds.center.y;
//				Debug.Log("Center: " + script.sprite.bounds.center);
			}
			else
			{
				script.useOffset = false;
			}
			script.sizeX = script.sprite.bounds.extents.x*2;
			script.sizeY = script.sprite.bounds.extents.y*2;
//			Debug.Log("Extends: " + script.sprite.bounds.extents);
		}
		else
		{
			script.sizeX = 1;
			script.sizeY = 1;
//			Debug.Log("Extends: " + script.sprite.bounds.extents);
			script.useOffset = false;
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Use Name Prefix");
		usePrefix = EditorGUILayout.Toggle (usePrefix);							// Prefix Checkbox / Toggle
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Depth");
		script.depth = EditorGUILayout.Slider (script.depth, -5,5);
		EditorGUILayout.EndHorizontal();

		script.rotation = EditorGUILayout.Vector3Field("Rotation", script.rotation);

//		script.layer = EditorGUILayout.LayerField("Sprite Layer",

		if(GUI.changed)
			EditorUtility.SetDirty(target);
	}

	//wenn Sceneview gezeichnet wird
	void OnSceneGUI()
	{
		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		Vector2 tilePos = new Vector2();
		tilePos.x = Mathf.RoundToInt(ray.origin.x);
		tilePos.y = Mathf.RoundToInt(ray.origin.y);

		if (tilePos != oldTilePos)
		{
			script.gizmoPosition = tilePos;
			SceneView.RepaintAll();
		}

		Event current = Event.current;

		/* left CTRL Key */
		if(current.keyCode == KeyCode.LeftControl)
		{
			if(current.type == EventType.keyDown)
			{
				leftControl = true;
			}
			else if(current.type == EventType.keyUp)
			{
				leftControl = false;
				batchmode = BatchMode.None;
			}
		}

		/* Mouse */
		if(leftControl)
		{
			if(current.type == EventType.mouseDown)
			{
				if(current.button == 0)
				{
					//(linke Maustaste)
					batchmode = BatchMode.Create;
				}
				else if(current.button == 1)
				{
					//(rechte Maustaste)
					batchmode = BatchMode.Delete;
				}

			}

		}

		/* Use Input */
		if((current.type == EventType.mouseDown) || (batchmode != BatchMode.None))
		{
			if(script.prefab != null)
			{
				string name;
				if(usePrefix)
				{
					name = string.Format(script.prefab.name + "_{0}_{1}_{2}",script.depth,tilePos.y,tilePos.x);
				}
				else
				{
					name = string.Format(script.prefab.name);
				}
				if((current.button == 0) || (batchmode == BatchMode.Create))
				{
					//Create (linke Maustaste)
					CreateTile(tilePos,name);
				}
				
				if((current.button == 1) || (batchmode == BatchMode.Delete))
				{
					//Delete (rechte Maustaste)
					DeleteTile(name);
				}
			}
			else if(script.sprite != null)
			{
				string name;
				if(usePrefix)
				{
					name = string.Format(script.sprite.name + "_{0}_{1}_{2}",script.depth,tilePos.y,tilePos.x);
				}
				else
				{
					name = string.Format(script.sprite.name);
				}
				if((current.button == 0) || (batchmode == BatchMode.Create))
				{
					//Create (linke Maustaste)
					CreateSprite(tilePos,name);
				}
				
				if((current.button == 1) || (batchmode == BatchMode.Delete))
				{
					//Delete (rechte Maustaste)
					DeleteSprite(name);
				}
			}
			else
			{
				Debug.LogWarning("Prefab and Sprite selected, de-select one!");
			}
		}

		SetGizmosColor();

		if(GUI.changed)
			EditorUtility.SetDirty(target);	
	}

	void CreateTile(Vector2 tilePos, string name)
	{
		if(!GameObject.Find(name))
		{
			Vector3 pos = new Vector3(tilePos.x, tilePos.y, script.depth);
			Quaternion quat = new Quaternion();
			quat.eulerAngles = script.rotation;
			GameObject go = (GameObject) Instantiate (script.prefab,pos,quat);
			go.name = name;
		}
	}
	
	void DeleteTile(string name)
	{
		GameObject go = GameObject.Find (name);
		
		if(go!=null)
		{
			DestroyImmediate(go);	//Destroy() klappt im Editormodus nicht!
		}
		
	}

	void CreateSprite(Vector2 tilePos, string name)
	{
		if(!GameObject.Find(name))
		{
			Vector3 pos = new Vector3(tilePos.x, tilePos.y, script.depth);
			Quaternion quat = new Quaternion();
			quat.eulerAngles = script.rotation;
			GameObject go = new GameObject();
			go.AddComponent<SpriteRenderer>();
			SpriteRenderer renderer = go.GetComponent<SpriteRenderer>();
			renderer.sprite = script.sprite;
//			if(script.sprite.bounds.center != Vector3.zero)
//			{
//				script.useOffset = true;
//				script.offsetX = script.sprite.bounds.center.x;
//				script.offsetY = script.sprite.bounds.center.y;
//				Debug.Log(script.sprite.bounds.center);
//				Debug.Log(script.sprite.bounds.extents);
//			}
//			else
//			{
//				script.useOffset = false;
//			}
			//Instantiate (go,pos,quat);
			go.name = name;
			go.transform.position = pos;
		}
	}
	
	void DeleteSprite(string name)
	{
		GameObject go = GameObject.Find (name);
		
		if(go!=null)
		{
			DestroyImmediate(go);	//Destroy() klappt im Editormodus nicht!
		}
		
	}

	void SetGizmosColor()
	{
		switch(batchmode)
		{
		case BatchMode.None:
			script.gizmoColor = Color.gray;
			break;
		case BatchMode.Create:
			script.gizmoColor = Color.green;
			break;
		case BatchMode.Delete:
			script.gizmoColor = Color.red;
			break;
		}
	}

}
