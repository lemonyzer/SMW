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
	bool w_UseAssetSubSpritesToggle = true;
	string EP_TilesetManagerKey = "EP_tilesetManagerKey";
	public static string EP_BackgroundAssetFolderPathKey = "EP_BackgroundAssetFolderPathKey";
	string w_BackgroundAssetFolderPath = "";
	string m_LastBackgroundAssetFolderPath = "";
//	string lastUsedTilesetManagerPath = "";
	#endregion

	#region Main Methods

	public static Map Create(string mapName)
	{
		Map newMapAsset = ScriptableObject.CreateInstance<Map>();
		newMapAsset.mapName = mapName;
		AssetDatabase.CreateAsset(newMapAsset, "Assets/Maps/map_" + mapName + ".asset");
		AssetDatabase.SaveAssets();
		
//		EditorUtility.FocusProjectWindow();
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

	void OnEnable()
	{
		// load last used List
		if(EditorPrefs.HasKey(EP_TilesetManagerKey))
		{
			string objectPath = EditorPrefs.GetString(EP_TilesetManagerKey);
			g_TilesetManager = AssetDatabase.LoadAssetAtPath(objectPath, typeof(TilesetManager)) as TilesetManager;
		}

		if(EditorPrefs.HasKey(EP_BackgroundAssetFolderPathKey))
		{
			w_BackgroundAssetFolderPath = EditorPrefs.GetString(EP_BackgroundAssetFolderPathKey);
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
		EditorGUI.BeginChangeCheck();
		g_TilesetManager = (TilesetManager) EditorGUILayout.ObjectField("TilesetManager", g_TilesetManager, typeof(TilesetManager), false, GUILayout.ExpandWidth(true));
		if(EditorGUI.EndChangeCheck())
		{
			if(g_TilesetManager != null)
			{
				EditorPrefs.SetString(EP_TilesetManagerKey, AssetDatabase.GetAssetPath(g_TilesetManager));
				Debug.Log("Path " + AssetDatabase.GetAssetPath(g_TilesetManager)+ " saved in EditorPrefs("+EP_TilesetManagerKey+")");
			}
		}
		GUILayout.BeginHorizontal();
		{
			EditorGUILayout.LabelField("Background Asset Folder: " + w_BackgroundAssetFolderPath);
			if(GUILayout.Button("Select Background Asset Folder", GUILayout.ExpandWidth(false)))
			{
				if(OnGUI_OpenAssetFolder(out m_LastBackgroundAssetFolderPath))
				{
					// folder selected
					w_BackgroundAssetFolderPath = m_LastBackgroundAssetFolderPath;
				}
				else
				{
					// nothing selected
				}
			}
		}
		GUILayout.EndHorizontal();
		if(AssetDatabase.IsValidFolder(w_BackgroundAssetFolderPath) == false)
		{
			EditorGUILayout.LabelField("select existing Background Asset Folder!", EditorStyles.boldLabel);
			GUI.enabled = false;
		}
		else
		{
			GUI.enabled = true;
		}
		if(g_TilesetManager == null)
			GUI.enabled = false;
//		else
//			GUI.enabled = true;

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
				bool loadWithoutError = m_CurrentMap.loadMap(m_LastWorkingMapImportPath, ReadType.read_type_full, g_TilesetManager);
				if(!loadWithoutError)
				{
					// import mit Fehler
					string currentAssetPath = AssetDatabase.GetAssetPath(m_CurrentMap);
					string newAssetName = "_import_error_"+mapName;
					AssetDatabase.RenameAsset(currentAssetPath, newAssetName);
				}
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
			w_UseAssetSubSpritesToggle = GUILayout.Toggle(w_UseAssetSubSpritesToggle, "Use Sprite from Asset, if false Sprites get sliced from Tileset-Sprite");
			if (GUILayout.Button("Create Unity Map", GUILayout.ExpandWidth(false)))
			{
				CreateUnityMap(m_CurrentMap, w_BackgroundAssetFolderPath);
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

	void CreateUnityMap(Map mapSO, string backgroundAssetFolderPath)
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

		GameObject mapRootGO = new GameObject(mapSO.mapName + (w_UseAssetSubSpritesToggle ? "_AssetSubSprites" : "_no"));
		mapRootGO.transform.position = Vector3.zero;
		// Map Background
		SpriteRenderer backgroundSpriteRenderer = mapRootGO.AddComponent<SpriteRenderer>();
		backgroundSpriteRenderer.sortingLayerName = "MapBackgroundLayer";
		Sprite backgroundSprite;
		string backgroundFilename = mapSO.GetBackgroundFilename();
		if(string.IsNullOrEmpty(mapSO.GetBackgroundFilename()))
		{
			backgroundFilename = "Land_Classic.png";
		}
		backgroundSprite = (Sprite) AssetDatabase.LoadAssetAtPath(backgroundAssetFolderPath +"/"+ backgroundFilename, typeof(Sprite));

		if(backgroundSprite == null)
			Debug.LogError("Map: " + mapSO.mapName + " Background " + backgroundFilename + " not Found in " + backgroundAssetFolderPath );

		backgroundSpriteRenderer.sprite = backgroundSprite;

		for(int l=0; l<Globals.MAPLAYERS; l++)
		{
			GameObject currentMapLayerGO = new GameObject("Layer " + l);
			currentMapLayerGO.transform.SetParent(mapRootGO.transform);
			currentMapLayerGO.transform.localPosition = new Vector3(0f,0f,(l+1)*-2f);		//(l+1) (trenne layer von background)
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
						Vector3 tilePos = new Vector3(-Globals.MAPWIDTH*0.5f +x,		// x+1: pivot Right , x+0.5f: pivor Center, x: pivot Left
						                              Globals.MAPHEIGHT*0.5f -(y+1),	// y-1: pivot Bottom, y-0.5f: pivot Center, y: pivot Top //TODO Tileset SlicedSprite Pivot setzen!
						                              0f);
						currentTileTransform.localPosition = tilePos;
						SpriteRenderer currentSpriteRenderer = currentTileGO.AddComponent<SpriteRenderer>();
						currentSpriteRenderer.sortingLayerName = "MapTileLayer"+l;
						int iTileSetId = mapData[x,y,l].iTilesetID;
						int tilePosX = mapData[x,y,l].iCol;
						int tilePosY = mapData[x,y,l].iRow;
						Tileset tileSet = g_TilesetManager.GetTileset(iTileSetId);
						Sprite tileSprite;
						if(w_UseAssetSubSpritesToggle)
							tileSprite = tileSet.GetTileSprite(tilePosX, tilePosY);
						else
							tileSprite = tileSet.GetNewCreatetTileSprite(tilePosX, tilePosY);	
						currentSpriteRenderer.sprite = tileSprite;

						TileType currentTileType = tileSet.GetTileType((short)tilePosX, (short)tilePosY);

						TileTypeToUnityTranslation(currentTileType, currentTileGO);

						TileScript currenTileScript = currentTileGO.AddComponent<TileScript>();
						currenTileScript.tileSet = tileSet;
						currenTileScript.tileType = currentTileType;
                        
                    }
					else
					{
//						DestroyImmediate(currentTileGO);
					}
				}
			}
		}

		// ObjectData
		int layer = Globals.MAPLAYERS;			
		layer++;									// +1 because Background and Layer 0 are two different layers 
		MapBlock[,] mapObjectData = mapSO.GetObjectData();
		if (mapObjectData != null)	
		{
			GameObject mapObjectDataLayerGO = new GameObject("ObjectData");
			mapObjectDataLayerGO.transform.SetParent(mapRootGO.transform);
			mapObjectDataLayerGO.transform.localPosition = new Vector3(0f,0f,(layer)*-2f);		//(l+1) (trenne layer von background)

			for(int y=0; y<Globals.MAPHEIGHT; y++)
			{
				for(int x=0; x<Globals.MAPWIDTH; x++)
				{
					MapBlock currenObjectDataMapBlock = mapObjectData[x,y];
					if(currenObjectDataMapBlock != null)
					{
						string tileTypeString = ""+ ((TileType) currenObjectDataMapBlock.iType);
						GameObject currentTileGO = new GameObject(x.ToString("D2") + " " + y.ToString("D2") + " " + tileTypeString );
						currentTileGO.transform.SetParent(mapObjectDataLayerGO.transform);
						Vector3 tileLocalPos = new Vector3(-Globals.MAPWIDTH*0.5f +x,		// x+1: pivot Right , x+0.5f: pivor Center, x: pivot Left
						                                   Globals.MAPHEIGHT*0.5f -(y+1),	// y-1: pivot Bottom, y-0.5f: pivot Center, y: pivot Top //TODO Tileset SlicedSprite Pivot setzen!
						                                   0f);
						currentTileGO.transform.localPosition = tileLocalPos;

						TileScript currenTileScript = currentTileGO.AddComponent<TileScript>();
						currenTileScript.mapBlock = currenObjectDataMapBlock;
						//currenTileScript.Add(currentMapBlock)
						//currenTileScript.Add(currentTilesetTile)
						//currenTileScript.Add(currentMapTile)
						//currenTileScript.Add(currentMovingPlatform)
					}
				}
			}
		}
		else
			Debug.LogError("Map: " + mapSO.mapName + " GetObjectData == NULL");

		layer++;
		MapTile[,] mapDataTop = mapSO.GetMapDataTop();
		if (mapDataTop != null)
		{
			// mapDataTop
			GameObject mapDataTopLayerGO = new GameObject("MapDataTop");
			mapDataTopLayerGO.transform.SetParent(mapRootGO.transform);
			mapDataTopLayerGO.transform.localPosition = new Vector3(0f,0f,(layer)*-2f);		//(l+1) (trenne layer von background)
			
			for(int y=0; y<Globals.MAPHEIGHT; y++)
			{
				for(int x=0; x<Globals.MAPWIDTH; x++)
				{
					MapTile currentMapDataTopTile = mapDataTop[x,y];
					if (currentMapDataTopTile != null)
					{
						string tileTypeString = "" + currentMapDataTopTile.iType;
						GameObject currentTileGO = new GameObject(x.ToString("D2") + " " + y.ToString("D2") + " " + tileTypeString );
						currentTileGO.transform.SetParent(mapDataTopLayerGO.transform);
						Vector3 tileLocalPos = new Vector3(-Globals.MAPWIDTH*0.5f +x,		// x+1: pivot Right , x+0.5f: pivor Center, x: pivot Left
						                                   Globals.MAPHEIGHT*0.5f -(y+1),	// y-1: pivot Bottom, y-0.5f: pivot Center, y: pivot Top //TODO Tileset SlicedSprite Pivot setzen!
						                                   0f);
						currentTileGO.transform.localPosition = tileLocalPos;

						TileScript currenTileScript = currentTileGO.AddComponent<TileScript>();
						currenTileScript.mapTile = currentMapDataTopTile;
					}
				}
			}
		}
		else
			Debug.LogError("Map: " + mapSO.mapName + " GetMapDataTop == NULL");

		// Platforms
		layer++;
		MovingPlatform[] platforms = mapSO.GetPlatforms();
		if (platforms != null)
		{
			// Platforms
			GameObject mapPlatformsLayerGO = new GameObject("Platforms");
			mapPlatformsLayerGO.transform.SetParent(mapRootGO.transform);
			mapPlatformsLayerGO.transform.localPosition = new Vector3(0f,0f,(layer)*-2f);		//(l+1) (trenne layer von background)

			for(int i=0; i<platforms.Length; i++)
			{
				// Single Platform
				MovingPlatform currentPlatform = platforms[i];
				if (currentPlatform != null)
				{
					GameObject currenPlatformGO = new GameObject("Platform " + i.ToString("D2"));
					currenPlatformGO.transform.SetParent(mapPlatformsLayerGO.transform, false);		// World Position stays = false.
//					currenPlatformGO.transform.localPosition = Vector3.zero;
					
					for(int y=0; y<currentPlatform.iPlatformHeight; y++)
					{
						for(int x=0; x<currentPlatform.iPlatformWidth; x++)
						{
							TilesetTile currentTilesetTile = currentPlatform.platformTiles[x,y];
							MapTile currentMapTile = currentPlatform.platformTileTypes[x,y];
							if(currentTilesetTile != null)
							{
								string tileTypeString = "";
								if (currentMapTile != null)
									tileTypeString += currentMapTile.iType;
								else
									tileTypeString += "NULL";
									
								GameObject currentPlatformTileGO = new GameObject(x.ToString("D2") + " " + y.ToString("D2") + " " + tileTypeString );
								currentPlatformTileGO.transform.SetParent(currenPlatformGO.transform);
								Vector3 tileLocalPos = new Vector3(-Globals.MAPWIDTH*0.5f +x,		// x+1: pivot Right , x+0.5f: pivor Center, x: pivot Left
								                              Globals.MAPHEIGHT*0.5f -(y+1),	// y-1: pivot Bottom, y-0.5f: pivot Center, y: pivot Top //TODO Tileset SlicedSprite Pivot setzen!
								                              0f);
								currentPlatformTileGO.transform.localPosition = tileLocalPos;

								SpriteRenderer tileRenderer = currentPlatformTileGO.AddComponent<SpriteRenderer>();
								tileRenderer.sortingLayerName = "MapPlatformLayer";
								
								int iTileSetId = currentTilesetTile.iTilesetID;
								int tilePosX = currentTilesetTile.iCol;
								int tilePosY = currentTilesetTile.iRow;

								Tileset tileSet = g_TilesetManager.GetTileset(iTileSetId);
								Sprite tileSprite;
								if(w_UseAssetSubSpritesToggle)
									tileSprite = tileSet.GetTileSprite(tilePosX, tilePosY);
								else
									tileSprite = tileSet.GetNewCreatetTileSprite(tilePosX, tilePosY);	
								tileRenderer.sprite = tileSprite;
								
								TileType currentTileType = tileSet.GetTileType((short)tilePosX, (short)tilePosY);
								
								TileTypeToUnityTranslation(currentTileType, currentPlatformTileGO);
							}

						}
					}
				}
			}
		}
		else
			Debug.Log("Map: " + mapSO.mapName + " Platforms == NULL -> Map hat keine MovingPlatform");
	}

	void TileTypeToUnityTranslation(TileType tileType, GameObject tileGO)		// Polymorphy!
	{
		// Polymorphie
		// tileType.AddComponent(tileGO);
		if(tileType == TileType.tile_solid)
		{
			// Block
			BoxCollider2D box = tileGO.AddComponent<BoxCollider2D>();
			box.isTrigger = false;
		}
		else if(tileType == TileType.tile_solid_on_top)
		{
			// JumpOnPlatform
			tileGO.layer = LayerMask.NameToLayer(Layer.jumpAblePlatformLayerName);
			BoxCollider2D box = tileGO.AddComponent<BoxCollider2D>();
			box.isTrigger = false;
		}
		else if(tileType == TileType.tile_nonsolid)
		{
			// nothing but Sprite
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

	string m_LastWorkingBackgroundAssetFolderPath = "";

	bool OnGUI_OpenAssetFolder(out string relPath)
	{
		// open folder dialog
		string absPath = EditorUtility.OpenFolderPanel ("Select Backgrounds Asset Folder", m_LastWorkingBackgroundAssetFolderPath, "Backgrounds");
		if(!string.IsNullOrEmpty(absPath))
		{
			int subStringStart = Application.dataPath.Length;
			if(!absPath.StartsWith(Application.dataPath))
			{
				// nicht im aktuell Projekt-AssetPath
				relPath = null;
				return false;
			}
			relPath = "Assets" + absPath.Substring(subStringStart);

			m_LastWorkingBackgroundAssetFolderPath = relPath;
//			m_LastWorkingBackgroundAssetFolderPath = absPath;
			//absolutenPath in EditorPrefs speichern 
			EditorPrefs.SetString(EP_BackgroundAssetFolderPathKey, m_LastWorkingBackgroundAssetFolderPath);
			Debug.Log("Path " + m_LastWorkingBackgroundAssetFolderPath+ " saved in EditorPrefs("+EP_BackgroundAssetFolderPathKey+")");
			
			return true;
		}
		else
		{
			relPath = null;
			return false;
		}
	}
}
