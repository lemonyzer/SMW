#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.IO;
using UnityEditor;

public class TilesetManagerWindow : EditorWindow {

	#region Variables
	static TilesetManagerWindow currWindow;
	#endregion

	#region Main Methods

	[MenuItem("SMW/Tileset Manager Create")]
	public static void Create()
	{
		TilesetManager newTilesetAsset = ScriptableObject.CreateInstance<TilesetManager>();

		AssetDatabase.CreateAsset(newTilesetAsset, "Assets/Maps/newTiletsetManagerSO.asset");
		AssetDatabase.SaveAssets();
		
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = newTilesetAsset;
	}

	[MenuItem("SMW/Tileset Manager Window")]
	public static void Init()
	{
		if(currWindow == null)
		{
			currWindow = (TilesetManagerWindow) EditorWindow.GetWindow(typeof(TilesetManagerWindow));
			currWindow.title = "Tileset Manager";
//			currWindow.minSize = new Vector2(256,512);
		}
		else
		{
			currWindow.Show();
		}
	}

	void OnGUI()
	{
		//EditorGUILayout.Space(10);
		GUILayout.BeginHorizontal();
		GUILayout.Space(10);
		GUILayout.BeginVertical();
		GUILayout.Space(10);
		GUILayout.Label("SMW Tileset Properties", EditorStyles.boldLabel);

		GUILayout.Label ("Auto Import", EditorStyles.boldLabel);

		if (GUILayout.Button("Create Tileset Manager", GUILayout.ExpandWidth(false)))
		{
			Create();
		}

		GUILayout.EndVertical();
		GUILayout.BeginHorizontal();


		Repaint();
	}
	#endregion

	string EP_LastWorkingMapImportPath = "EP_LastWorkingMapImportPath";
	string m_LastWorkingMapImportPath = "";
	string m_LastMapPath = "";
	bool m_FileOpened = false;
	Map currentMap;

	bool OnGUI_OpenFile(out string absPath)
	{
		// open folder dialog
		absPath = EditorUtility.OpenFilePanel ("Select SMW Map", m_LastWorkingMapImportPath, "map");
		if(!string.IsNullOrEmpty(absPath))
		{
			m_LastWorkingMapImportPath = absPath;
			//absolutenPath in EditorPrefs speichern 
			EditorPrefs.SetString(EP_LastWorkingMapImportPath, m_LastWorkingMapImportPath);

			return true;
		}
		else
		{
			return false;
			
		}
	}

}
#endif