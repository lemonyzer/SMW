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

	public static Map Create(string mapName)
	{
		Map newMapAsset = ScriptableObject.CreateInstance<Map>();
		newMapAsset.mapName = mapName;
		AssetDatabase.CreateAsset(newMapAsset, "Assets/Maps/map_" + mapName + ".asset");
		AssetDatabase.SaveAssets();
		
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = newMapAsset;
		
		return newMapAsset;
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
				string mapName = Path.GetFileNameWithoutExtension(m_LastMapPath);
				if(string.IsNullOrEmpty(mapName))
					mapName = "noMapName";
				m_CurrentMap = Create(mapName);
				m_CurrentMap.mapName = mapName;
//				m_CurrentMap.SetTiletsetManager(g_TilesetManager);
				m_CurrentMap.loadMap(m_LastWorkingMapImportPath, ReadType.read_type_full, g_TilesetManager);
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
//			m_CurrentMap.m_Tileset = (List<Tileset>) EditorGUILayout.ObjectField("Map", m_CurrentMap.m_Tileset, typeof(List<Tileset>), false, GUILayout.ExpandWidth(true));
			EditorGUILayout.LabelField(m_CurrentMap.mapName);
			
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
		GUILayout.EndHorizontal();


//		Repaint();
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
			Debug.LogError("mapSO.GetMapData() == NULL -> keine Informationen über Tile<->SubSprite vorhanden");
			return;
		}

		bool[,,] customMapData = mapSO.GetCustomMapData();
		if(customMapData == null)
		{
			Debug.LogError("mapSO.GetCustomMapData() == NULL -> keine Informationen welches Tile ein Sprite enthält");
			return;
		}

		GameObject mapRootGO = new GameObject(mapSO.mapName);
		mapRootGO.transform.position = Vector3.zero;
		for(int l=0; l<Globals.MAPLAYERS; l++)
		{
			GameObject currentMapLayerGO = new GameObject("Layer " + l);
			currentMapLayerGO.transform.SetParent(mapRootGO.transform);
			currentMapLayerGO.transform.localPosition = new Vector3(0f,0f,l*-2f);
			for(int y=0; y<Globals.MAPHEIGHT; y++)
			{
				for(int x=0; x<Globals.MAPWIDTH; x++)
				{
//					GameObject currentTileGO = new GameObject("Tile_" + x.ToString("D2") + " " + y.ToString("D2"));
//					Transform currentTileTransform = currentTileGO.transform;
//					currentTileTransform.SetParent(currentMapLayerGO.transform);
//					//currentTileTransform.position
//					Vector3 tilePos = new Vector3(-Globals.MAPWIDTH*0.5f +x,
//					                              Globals.MAPHEIGHT*0.5f -y,
//					                              0f);
//					currentTileTransform.localPosition = tilePos;
					if(customMapData[x,y,l] == true)
					{
						GameObject currentTileGO = new GameObject("Tile " + x.ToString("D2") + " " + y.ToString("D2"));
						Transform currentTileTransform = currentTileGO.transform;
						currentTileTransform.SetParent(currentMapLayerGO.transform);
						//currentTileTransform.position
						Vector3 tilePos = new Vector3(-Globals.MAPWIDTH*0.5f +x,
						                              Globals.MAPHEIGHT*0.5f -y,
						                              0f);
						currentTileTransform.localPosition = tilePos;
						SpriteRenderer currentSpriteRenderer = currentTileGO.AddComponent<SpriteRenderer>();
						int iTileSetId = mapData[x,y,l].iTilesetID;
						int tilePosX = mapData[x,y,l].iCol;
						int tilePosY = mapData[x,y,l].iRow;
						Tileset tileSet = g_TilesetManager.GetTileset(iTileSetId);
						Sprite tileSprite = tileSet.GetNewCreatetTileSprite(tilePosX,tilePosY);
						currentSpriteRenderer.sprite = tileSprite;
					}
					else
					{
//						DestroyImmediate(currentTileGO);
					}
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
