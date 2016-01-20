#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.IO;
using UnityEditor;

public class MapWindow : EditorWindow {

	#region Variables
	static MapWindow currWindow;
	TilesetManager g_TilesetManager;
	#endregion

	#region Main Methods

	[MenuItem("SMW/Map/Create")]
	public static Map Create()
	{
		Map newTilesetAsset = ScriptableObject.CreateInstance<Map>();
		
		AssetDatabase.CreateAsset(newTilesetAsset, "Assets/Maps/newMapSO.asset");
		AssetDatabase.SaveAssets();
		
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = newTilesetAsset;

		return newTilesetAsset;
	}

	[MenuItem("SMW/Map Window")]
	public static void Init()
	{
		if(currWindow == null)
		{
			currWindow = (MapWindow) EditorWindow.GetWindow(typeof(MapWindow));
			currWindow.title = "SMW Map";
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
		GUILayout.Label("SMW Map Properties", EditorStyles.boldLabel);

		GUILayout.Label ("Auto Import", EditorStyles.boldLabel);

		g_TilesetManager = (TilesetManager) EditorGUILayout.ObjectField("TilesetManager", g_TilesetManager, typeof(TilesetManager), false, GUILayout.ExpandWidth(true));

//		if (GUILayout.Button("Select TileManager", GUILayout.ExpandWidth(false)))
//		{
//		}

		if(g_TilesetManager == null)
			GUI.enabled = false;
		else
			GUI.enabled = true;

		if (GUILayout.Button("Open File", GUILayout.ExpandWidth(false)))
		{
			if(OnGUI_OpenFile(out m_LastMapPath))
			{
				m_FileOpened = true;
				// Class
//				currentMap = new Map(g_TilesetManager);		// on time (on button clicked)

				// ScriptableObject
//				currentMap = ScriptableObject.CreateInstance<Map>();
//				currentMap.SetTiletsetManager(g_TilesetManager);
//				currentMap.loadMap(m_LastWorkingMapImportPath, ReadType.read_type_preview);

				// Asset - ScripableObject // TODO savepath+name Create(path);
				currentMap = Create();
//				currentMap.SetTiletsetManager(g_TilesetManager);
				currentMap.loadMap(m_LastWorkingMapImportPath, ReadType.read_type_preview, g_TilesetManager);
			}
			else
			{
				m_FileOpened = false;
			}
		}
		if(m_FileOpened)
		{
			GUILayout.Label ("Path = " + m_LastWorkingMapImportPath, GUILayout.ExpandWidth(false));
			GUILayout.Label ("Path = " + @m_LastWorkingMapImportPath, GUILayout.ExpandWidth(false));
			if(currentMap != null)
				currentMap.OnGUI();
		}
		else
		{
			GUILayout.Label ("Path = " + "nothing selected", GUILayout.ExpandWidth(false));
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


//	FileInfo[] GetFileList (string absPath)
//	{
//		if (!string.IsNullOrEmpty(absPath))
//		{
//			DirectoryInfo dir = new DirectoryInfo(absPath);
//			FileInfo[] info = dir.GetFiles("*.png");
//			
//			
//			// Einmalige ausgabe auf Console
//			foreach (FileInfo f in info)
//			{
//				//				Debug.Log("Found " + f.Name);
//				//				Debug.Log("f.DirectoryName=" + f.DirectoryName);
//				//				Debug.Log("f.FullName=" + f.FullName);
//				//				Debug.Log("modified=" + f.FullName.Substring(Application.dataPath.Length - "Assets".Length));
//				// relative pfad angabe
//				string currentSpritePath = f.FullName.Substring(Application.dataPath.Length - "Assets".Length);
//				Debug.Log("currentSpritePath=" + currentSpritePath);
//				
////				string charName = GetCharNameFromFileName(f.Name);
//				string charName = f.Name;
//				if(charName != null)
//				{
//					Debug.Log(charName);
//				}
//				else
//				{
//					Debug.LogError(f.Name + " konnte Character Name nicht extrahieren");
//				}
//			}
//			return info;
//		}
//		else
//		{
//			Debug.LogError("absPath == \"\" or NULL ");
//			return null;
//		}
//	}

}
#endif