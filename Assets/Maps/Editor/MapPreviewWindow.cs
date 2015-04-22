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
	bool w_DontTranslationUnknown = true;
	bool w_SetNotValidToUnknown = true;
	bool w_SetTileTypeForNonValidTiles = true;
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

		GUILayout.Label ("Single Import", EditorStyles.boldLabel);

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
				m_CurrentMap = OpenMapFile(m_LastMapPath, false);
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
			w_DontTranslationUnknown = GUILayout.Toggle(w_DontTranslationUnknown, "leave Tile.TilesetID's == Globals.TILESETUNKNOWN (iCol&iRow)=0, else Tile.TilesetID's -> 0 (first Tileset in Map) -> lokal translated");
			w_SetNotValidToUnknown = GUILayout.Toggle(w_SetNotValidToUnknown, "Non Valid Tile.TilesetID's -> Globals.TILESETUNKNOWN, else Tile.TilesetID's -> 0 (first Tileset in Map) -> lokal translated");
			if(!w_SetNotValidToUnknown)
				w_SetTileTypeForNonValidTiles = GUILayout.Toggle(w_SetTileTypeForNonValidTiles, "use Tileset TileTyp for non Valid Tiles");
			else
			{
				bool temp = GUI.enabled;
				GUI.enabled = false;
				w_SetTileTypeForNonValidTiles = GUILayout.Toggle(w_SetTileTypeForNonValidTiles, "use Tileset TileTyp for non Valid Tiles");
				GUI.enabled = temp;
			}

			if (GUILayout.Button("Create Unity Map", GUILayout.ExpandWidth(false)))
			{
				CreateUnityMap(m_CurrentMap, w_BackgroundAssetFolderPath, w_UseAssetSubSpritesToggle, w_DontTranslationUnknown, w_SetNotValidToUnknown, w_SetTileTypeForNonValidTiles);
			}
			m_CurrentMap.OnGUI_Preview();
		}

//		if (GUILayout.Button("Select TileManager", GUILayout.ExpandWidth(false)))
//		{
//		}
		if(g_TilesetManager != null)
		{
			GUI.enabled = true;
		}
		else
		{
			EditorGUILayout.LabelField("no g_TilesetManager selected");
			GUI.enabled = false;
		}
		if(AssetDatabase.IsValidFolder(w_BackgroundAssetFolderPath) == false)
		{
			EditorGUILayout.LabelField("select existing Background Asset Folder!", EditorStyles.boldLabel);
			GUI.enabled = false;
		}
		else
		{
			GUI.enabled = true;
		}

		OnGUI_AutoImport();

		GUILayout.EndVertical();
		GUILayout.EndHorizontal();


//		Repaint();
	}

	Map OpenMapFile(string mapFilePath, bool isBatch)
	{
		// Class
		//				currentMap = new Map(g_TilesetManager);		// on time (on button clicked)
		
		// ScriptableObject
		//				currentMap = ScriptableObject.CreateInstance<Map>();
		//				currentMap.SetTiletsetManager(g_TilesetManager);
		//				currentMap.loadMap(m_LastWorkingMapImportPath, ReadType.read_type_preview);
		
		// Asset - ScripableObject // TODO savepath+name Create(path);
		string mapName = Path.GetFileNameWithoutExtension(mapFilePath);
		if(string.IsNullOrEmpty(mapName))
			mapName = "noMapName";
		Map currentMap = Create(mapName);
		currentMap.mapName = mapName;
		//				currentMap.SetTiletsetManager(g_TilesetManager);
		bool loadWithoutError = currentMap.loadMap(mapFilePath, ReadType.read_type_full, g_TilesetManager);
		if(!loadWithoutError)
		{
			// import mit Fehler
			string currentAssetPath = AssetDatabase.GetAssetPath(currentMap);
			string newAssetName = "_import_error_"+mapName;
			AssetDatabase.RenameAsset(currentAssetPath, newAssetName);
		}

		if(!isBatch)
		{
			EditorUtility.SetDirty(currentMap);
			AssetDatabase.SaveAssets();
		}
		return currentMap;
		//				EditorApplication.SaveAssets();
	}

	GameObject CreateUnityMap(Map mapSO, string backgroundAssetFolderPath, bool useAssetSubSprites, bool dontTranslateUnknown, bool setNotValidToUnknown, bool setTileTypeForNonValidTiles)
	{
		if(mapSO == null)
		{
			Debug.LogError("mapSO == NULL");
			return null;
		}

		TilesetTile[,,] mapDataRaw = mapSO.GetMapDataRaw();
		if(mapDataRaw == null)
		{
			Debug.LogError("mapSO.GetMapDataRaw() == NULL -> keine Informationen über Tile<->SubSprite vorhanden");
			return null;
		}

		TilesetTile[,,] mapData = mapSO.GetMapData();
		if(mapData == null)
		{
			Debug.LogError("mapSO.GetMapData() == NULL -> keine Informationen über Tile<->SubSprite vorhanden");
			return null;
		}

		bool[,,] customMapData = mapSO.GetCustomMapData();
		if(customMapData == null)
		{
			Debug.LogError("mapSO.GetCustomMapData() == NULL -> keine Informationen welches Tile ein Sprite enthält");
			return null;
		}

		/**
		 * 
		 * MapRootGO
		 * 
		 **/

		GameObject mapRootGO = new GameObject(mapSO.mapName + (w_UseAssetSubSpritesToggle ? "_AssetSubSprites" : "_no"));
		mapRootGO.tag = Tags.tag_Map;
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
					//TilesetTile currenTilesetTile = mapData[x,y,l];
					TilesetTile currentRawTilesetTile = mapDataRaw[x,y,l];
					if(currentRawTilesetTile != null)
					{
						// erzeuge (kopiere) currenTilesetTile neu um RawDaten nicht zu manipulieren!
						TilesetTile translatedTile = new TilesetTile();
						translatedTile.iTilesetID = currentRawTilesetTile.iTilesetID;
						translatedTile.iCol = currentRawTilesetTile.iCol;
						translatedTile.iRow = currentRawTilesetTile.iRow;

						if(translatedTile.iTilesetID == Globals.TILESETNONE)		// geht nur mit mapDataRaw !!!
						{
							// TILESET NONE

						}
						else
						{
							bool useTileType = true;
							if(translatedTile.iTilesetID == Globals.TILESETANIMATED)	// geht nur mit mapDataRaw !!!
							{
								// TILESET ANIMATED
								useTileType = false;
								translatedTile.iTilesetID = currentRawTilesetTile.iTilesetID;
								translatedTile.iCol = currentRawTilesetTile.iCol;
								translatedTile.iRow = currentRawTilesetTile.iRow;
							}
							else if(translatedTile.iTilesetID == Globals.TILESETUNKNOWN)	// geht nur mit mapDataRaw !!!
							{
								// TILESET unknown
								// erzeuge currenTilesetTile neu um RawDaten nicht zu manipulieren!

								Debug.LogError( mapSO.mapName + " non Valid Tile (TileID > maxTilesetID) found " + (setNotValidToUnknown ? "set UNKNOWN" : "set TileID = 0" ));


								if(dontTranslateUnknown)
								{
									translatedTile.iTilesetID = Globals.TILESETUNKNOWN;	// TODO ID darf nicht lokal Translated werden!!! (array out of bounds error)
									translatedTile.iCol = 0;
									translatedTile.iRow = 0;
									
									useTileType = false;
								}
								else
								{
									translatedTile.iTilesetID = 0;

									//Make sure the column and row we read in is within the bounds of the tileset
									if(translatedTile.iCol < 0 || translatedTile.iCol >= mapSO.tilesetwidths[translatedTile.iTilesetID])
									{
										translatedTile.iCol = 0;
									}
									
									if(translatedTile.iRow < 0 || translatedTile.iRow >= mapSO.tilesetheights[translatedTile.iTilesetID])
									{
										translatedTile.iRow = 0;
									}

									if(setTileTypeForNonValidTiles)		
										useTileType = true;				// obwohl aktuelles Tile einem Tileste angehört das in der Map-Datei nicht gelistet wurde wird der TileTyp vom ersten Tileset verwendet um dem Tile Collider/Movement/Friction zu geben
									else
										useTileType = false;

									translatedTile.iTilesetID = (short) mapSO.translationid[currentRawTilesetTile.iTilesetID];
								}

							}
							else if(translatedTile.iTilesetID > mapSO.GetMaxTilesetID())
							{
								// TilesetID passt nicht zu den in der Map angegebenen Tilesets!
								// setzte TilesetID von aktuellem Tile auf das Tileset das als erstes in Map angegebenen wurde
								
								Debug.LogError( mapSO.mapName + " non Valid Tile (TileID > maxTilesetID) found " + (setNotValidToUnknown ? "set UNKNOWN" : "set TileID = 0" ));
								
								if(setNotValidToUnknown)
								{
									translatedTile.iTilesetID = Globals.TILESETUNKNOWN;	// TODO ID darf nicht lokal Translated werden!!! (array out of bounds error)
									translatedTile.iCol = 0;
									translatedTile.iRow = 0;
									
									useTileType = false;
								}
								else
								{
									translatedTile.iTilesetID = 0;		// TODO ID muss noch Translated werden!!!
									
									//Make sure the column and row we read in is within the bounds of the tileset
									if(translatedTile.iCol < 0 || translatedTile.iCol >= mapSO.tilesetwidths[translatedTile.iTilesetID])
									{
										translatedTile.iCol = 0;
									}
									
									if(translatedTile.iRow < 0 || translatedTile.iRow >= mapSO.tilesetheights[translatedTile.iTilesetID])
									{
										translatedTile.iRow = 0;
									}
									
									if(setTileTypeForNonValidTiles)		
										useTileType = true;				// obwohl aktuelles Tile einem Tileste angehört das in der Map-Datei nicht gelistet wurde wird der TileTyp vom ersten Tileset verwendet um dem Tile Collider/Movement/Friction zu geben
									else
										useTileType = false;
									
									// translate from Map TilesetID to Lokal TilesetManager TilesetID
									// Map.TilesetID -> Lokal TilesetManager.GetTilesetIDByName(Map.Tileset)TilesetID)
									translatedTile.iTilesetID = (short) mapSO.translationid[currentRawTilesetTile.iTilesetID];	// TODO lokal translation non valid tile translated to first Map-File Tileset
									
								}
							}
							else
							{
								// in diesem ELSE darf translatedTile.iTilesetID 0 bis eingeschlossen mapSO.GetMaxTilesetID laufen
								// normales Tile (inerhalb der erlaubten 0-iMaxTilesetID
								// translate from Map TilesetID to Lokal TilesetManager TilesetID
								// Map.TilesetID -> Lokal TilesetManager.GetTilesetIDByName(Map.Tileset)TilesetID)
								translatedTile.iTilesetID = (short) mapSO.translationid[currentRawTilesetTile.iTilesetID];		// TODO lokal translation normal tile
								useTileType = true;	
							}

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
							int iTileSetId = translatedTile.iTilesetID;
							int tilePosX = translatedTile.iCol;
							int tilePosY = translatedTile.iRow;
							Tileset tileSet = g_TilesetManager.GetTileset(iTileSetId);
							Sprite tileSprite;
							Sprite[] animationSprites;
							if(useAssetSubSprites)
							{
								if(translatedTile.iTilesetID == Globals.TILESETANIMATED)
								{
									//Animations Script an Gameobject fügen
									// mit Sprites füllen
									//TODO
								}
								else
									tileSprite = tileSet.GetTileSprite(tilePosX, tilePosY);
							}
							else
								tileSprite = tileSet.GetNewCreatetTileSprite(tilePosX, tilePosY);	
							currentSpriteRenderer.sprite = tileSprite;

							if (useTileType)
							{
								TileType currentTileType = tileSet.GetTileType((short)tilePosX, (short)tilePosY);
								
								TileTypeToUnityTranslation(currentTileType, currentTileGO);
								
								TileScript currenTileScript = currentTileGO.AddComponent<TileScript>();
								currenTileScript.tileSet = tileSet;
								currenTileScript.tileType = currentTileType;
							}
						}
                        
                    }
					else
					{
						Debug.LogError("mapData/Raw enthält Lücken : x=" + x + " y= " + y + " l="+ l );
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
								if(useAssetSubSprites)
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

		return mapRootGO;
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

	string EP_lastBatchMapsImportFolder = "EP_lastBatchMapsImportFolder";
	string batch_MapsImportPath;
	string batch_LastWorkingMapsImportPath;
	int batchQuantity = 2;
	void OnGUI_AutoImport()
	{
		GUILayout.Label ("Auto Import", EditorStyles.boldLabel);
		GUILayout.Label ("Path = " + batch_MapsImportPath, GUILayout.ExpandWidth(false));
		GUILayout.BeginVertical ();
		if (GUILayout.Button("Select Import Folder", GUILayout.ExpandWidth(false)))
		{
			// open folder dialog
			batch_MapsImportPath = EditorUtility.OpenFolderPanel ("Select Import Folder Maps", batch_LastWorkingMapsImportPath, "");
			if(!string.IsNullOrEmpty(batch_MapsImportPath))
			{
				batch_LastWorkingMapsImportPath = batch_MapsImportPath;
				//absolutenPath in EditorPrefs speichern 
				EditorPrefs.SetString(EP_lastBatchMapsImportFolder, batch_LastWorkingMapsImportPath);
				window_Batch_FileInfo = GetFileList(batch_MapsImportPath);
			}
			else
			{
				//WITCHTIG!!!!!!!!!!
				batch_MapsImportPath = "";
				window_Batch_FileInfo = null;
				
			}
			
		}
		batchQuantity = EditorGUILayout.IntField("Import Anzahl:", batchQuantity);
		if(!Directory.Exists(batch_MapsImportPath))
		{
			EditorGUILayout.LabelField("select existing Maps Folder!", EditorStyles.boldLabel);
			GUI.enabled = false;
		}
		if(batchQuantity < 0)
		{
			GUI.enabled = false;
		}
		if (GUILayout.Button("Start Batch Import", GUILayout.ExpandWidth(false)))
		{
			StartBatchImport(g_TilesetManager, w_BackgroundAssetFolderPath, batch_MapsImportPath, batchQuantity);
		}
//		if (GUILayout.Button("Open Folder in Unity", GUILayout.ExpandWidth(false)))
//		{
//			// open folder dialog
//			if(!string.IsNullOrEmpty(batch_LastWorkingMapsImportPath))
//			{
//				string relPath = AbsolutPathToUnityProjectRelativePath(batch_LastWorkingMapsImportPath);
//				if(relPath != null)
//				{
//					EditorUtility.FocusProjectWindow();
//					UnityEngine.Object folder = AssetDatabase.LoadAssetAtPath (relPath,typeof(UnityEngine.Object));
//					Selection.activeObject = folder;
//				}
//			}
//		}
		GUILayout.EndVertical ();
	}

	string AbsolutPathToUnityProjectRelativePath(string absPath)
	{
		if (absPath.StartsWith(Application.dataPath))
		{
			string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
			Debug.Log(absPath);
			Debug.Log(relPath);
			
			return relPath;
		}
		return null;
	}

	FileInfo[] window_Batch_FileInfo = null;
	
	FileInfo[] GetFileList (string absPath)
	{
		if (!string.IsNullOrEmpty(absPath))
		{
			DirectoryInfo dir = new DirectoryInfo(absPath);
			FileInfo[] info = dir.GetFiles("*.map");
			
			
			// Einmalige ausgabe auf Console
			foreach (FileInfo f in info)
			{
				//				Debug.Log("Found " + f.Name);
				//				Debug.Log("f.DirectoryName=" + f.DirectoryName);
				//				Debug.Log("f.FullName=" + f.FullName);
				//				Debug.Log("modified=" + f.FullName.Substring(Application.dataPath.Length - "Assets".Length));
				// relative pfad angabe
//relPath		string currentMapPath = f.FullName.Substring(Application.dataPath.Length - "Assets".Length);
				string currentMapPath = f.FullName;		//absPath
				Debug.Log("currentMapPath=" + currentMapPath);
				
//				string mapName = GetMapNameFromFileName(f.Name);
//				if(mapName != null)
//				{
//					Debug.Log(mapName);
//				}
//				else
//				{
//					Debug.LogError(f.Name + " konnte mapName Name nicht extrahieren");
//				}
			}
			return info;
		}
		else
		{
			Debug.LogError("absPath == \"\" or NULL ");
			return null;
		}
	}

	public string GetMapNameFromFileName(string fileName)
	{
		return fileName;
	}

	void StartBatchImport(TilesetManager tilesetManager, string backgroundAssetFolderPath, string importPath, int batchNumLimit)
	{
		Debug.Log("<color=green><b>StartBatchImport</b></color>");
		if(string.IsNullOrEmpty(importPath))
		{
			Debug.LogError ("importPath == \"\" oder null !!!");
			return;
		}
		
		//TODO DONE ordner auf existenz prüfen
		FileInfo[] info = GetFileList(importPath);

		if(info == null)
		{
			Debug.LogError ("FileInfo[] == null !!!");
			return;
		}

		if (tilesetManager == null) {
			Debug.LogError ("tilesetManager == null !!!");
			return;
		}

		if(!AssetDatabase.IsValidFolder(backgroundAssetFolderPath))
		{
			Debug.LogError ("backgroundAssetFolderPath = " + backgroundAssetFolderPath + " is not valid (needs to be in Assets/");
			return;
		}
		int count = 1;
		if(info != null)
		{
			foreach (FileInfo f in info)
			{
				// Abbruchbedingung, batchNum sagt wie viele Maps automatisch erstellt werden sollen, batchNum = 0 heißt alle im Ordner!
				if(batchNumLimit != 0)
				{
					if(count > batchNumLimit)
						break;
					else
						Debug.Log("count = " + count + " < " + batchNumLimit + " batchNumLimit"  );
				}
				count++;

				// relative pfad angabe
				string currentAbsMapPath = f.FullName;
				Debug.Log("<color=white><b>Found " + currentAbsMapPath + "</b></color>");

				Map currentBatchMap = OpenMapFile(currentAbsMapPath, true);
				GameObject currentBatchMapGO = CreateUnityMap(currentBatchMap, backgroundAssetFolderPath, w_UseAssetSubSpritesToggle, w_DontTranslationUnknown, w_SetNotValidToUnknown, w_SetTileTypeForNonValidTiles);

				currentBatchMapGO.SetActive(false);

			}
			AssetDatabase.SaveAssets();
		}
	}
}
