using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.IO;
using UnityEditor;

public class MapPreviewWindow : EditorWindow {

	#region Variables
	static MapPreviewWindow currWindow;
	TilesetManager g_TilesetManager;
	Map m_CurrentMap;
	#endregion

	#region Main Methods

	public static Map Create()
	{
		Map newTilesetAsset = ScriptableObject.CreateInstance<Map>();
		
		AssetDatabase.CreateAsset(newTilesetAsset, "Assets/Maps/previewNewMapSO.asset");
		AssetDatabase.SaveAssets();
		
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = newTilesetAsset;
		
		return newTilesetAsset;
	}

	[MenuItem("SMW/Map/Preview Window")]
	public static void Init()
	{
		if(currWindow == null)
		{
			currWindow = (MapPreviewWindow) EditorWindow.GetWindow(typeof(MapPreviewWindow));
			currWindow.title = "SMW Map Preview";
//			currWindow.minSize = new Vector2(256,512);
		}
		else
		{
			currWindow.Show();
		}
	}

//	public GUISkin guiSkin;
//	public GUIStyle textFieldStlye;

	void OnGUI()
	{
		//EditorGUILayout.Space(10);
		GUILayout.BeginHorizontal();
		GUILayout.Space(10);
		GUILayout.BeginVertical();
		GUILayout.Space(10);
		GUILayout.Label("SMW Map Properties", EditorStyles.boldLabel);

		GUILayout.Label ("Auto Import", EditorStyles.boldLabel);

//		guiSkin = (GUISkin) EditorGUILayout.ObjectField("guiSkin", guiSkin, typeof(GUISkin), false, GUILayout.ExpandWidth(true));
//		if(guiSkin != null)
//			GUI.skin = guiSkin;
//		textFieldStlye = (GUIStyle) EditorGUILayout.ObjectField("GUIStyle", textFieldStlye, typeof(GUIStyle), false, GUILayout.ExpandWidth(true));

		g_TilesetManager = (TilesetManager) EditorGUILayout.ObjectField("TilesetManager", g_TilesetManager, typeof(TilesetManager), false, GUILayout.ExpandWidth(true));
		if(g_TilesetManager == null)
			GUI.enabled = false;
		else
			GUI.enabled = true;

		if (GUILayout.Button("Open Map to Import", GUILayout.ExpandWidth(false)))
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
				m_CurrentMap = Create();
				m_CurrentMap.SetTiletsetManager(g_TilesetManager);
				m_CurrentMap.loadMap(m_LastWorkingMapImportPath, ReadType.read_type_full);
				EditorUtility.SetDirty(m_CurrentMap);
//				EditorApplication.SaveAssets();
				AssetDatabase.SaveAssets();
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
			if(m_CurrentMap != null)
				m_CurrentMap.OnGUI();
		}
		else
		{
			GUILayout.Label ("Path = " + "nothing selected", GUILayout.ExpandWidth(false));
		}

		m_CurrentMap = (Map) EditorGUILayout.ObjectField("Map", m_CurrentMap, typeof(Map), false, GUILayout.ExpandWidth(true));

		if(m_CurrentMap == null)
		{
			EditorGUILayout.LabelField("no Map selected");
			GUI.enabled = false;
		}
		else
		{
			GUI.enabled = true;
			if (GUILayout.Button("Create Unity Map", GUILayout.ExpandWidth(false)))
			{
				CreateUnityMap(m_CurrentMap);
			}
			m_CurrentMap.OnGUI_Preview();
		}

//		if (GUILayout.Button("Select TileManager", GUILayout.ExpandWidth(false)))
//		{
//		}

		GUILayout.EndVertical();
		GUILayout.BeginHorizontal();


		Repaint();
	}

	void CreateUnityMap(Map mapSO)
	{
		if(mapSO == null)
		{
			Debug.LogError("mapSO == NULL");
			return;
		}

		TilesetTile[,,] mapData = mapSO.GetMapData();
		if(mapData == null)
		{
			Debug.LogError("mapSO.GetMapData() == NULL");
			return;
		}

		GameObject mapRoot = new GameObject("TestMap");
		for(int l=0; l<Globals.MAPLAYERS; l++)
		{
			GameObject currentMapLayer = new GameObject("Layer " + l);
			currentMapLayer.transform.SetParent(mapRoot.transform);

			for(int y=0; y<Globals.MAPHEIGHT; y++)
			{
				for(int x=0; x<Globals.MAPWIDTH; x++)
				{
					SpriteRenderer currentSpriteRenderer = currentMapLayer.AddComponent<SpriteRenderer>();
					//currentSpriteRenderer.sprite
				}
			}
		}
	}
	#endregion

	string EP_LastWorkingMapImportPath = "EP_LastWorkingMapImportPath";
	string m_LastWorkingMapImportPath = "";
	string m_LastMapPath = "";
	bool m_FileOpened = false;

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
