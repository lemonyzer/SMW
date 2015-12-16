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
	Hazards g_HazardManager;
	Map m_CurrentMap;
	bool w_UseAssetSubSpritesToggle = true;
	bool w_DontTranslationUnknown = true;
	bool w_SetNotValidToUnknown = true;
	bool w_SetTileTypeForNonValidTiles = true;
	string EP_TilesetManagerKey = "EP_tilesetManagerKey";
	string EP_HazardManagerKey = "EP_HazardManagerKey";
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
		if(EditorPrefs.HasKey(EP_HazardManagerKey))
		{
			string objectPath = EditorPrefs.GetString(EP_HazardManagerKey);
			g_HazardManager = AssetDatabase.LoadAssetAtPath(objectPath, typeof(Hazards)) as Hazards;
		}
		
		if(EditorPrefs.HasKey(EP_BackgroundAssetFolderPathKey))
		{
			w_BackgroundAssetFolderPath = EditorPrefs.GetString(EP_BackgroundAssetFolderPathKey);
		}
	}

//	public GUISkin guiSkin;
//	public GUIStyle textFieldStlye;
	Vector2 windowScrollPos;

	bool fShowMapTop = false;
	

	void OnGUI()
	{
		windowScrollPos = EditorGUILayout.BeginScrollView(windowScrollPos);
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
		EditorGUI.BeginChangeCheck();
		g_HazardManager = (Hazards) EditorGUILayout.ObjectField("HazardManager", g_HazardManager, typeof(Hazards), false, GUILayout.ExpandWidth(true));
		if(EditorGUI.EndChangeCheck())
		{
			if(g_HazardManager != null)
			{
				EditorPrefs.SetString(EP_HazardManagerKey, AssetDatabase.GetAssetPath(g_HazardManager));
				Debug.Log("Path " + AssetDatabase.GetAssetPath(g_HazardManager)+ " saved in EditorPrefs("+EP_HazardManagerKey+")");
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
			fShowMapTop = UnityEditor.EditorGUILayout.Foldout(fShowMapTop,"Preview Map Top Data: (Type, Flag)");
			if(fShowMapTop)
			{
				m_CurrentMap.OnGUI_Preview_MapTop (g_TilesetManager);
			}
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
		EditorGUILayout.EndScrollView ();
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

	void CreateBackground (GameObject mapRootGO, Map mapSO, string backgroundAssetFolderPath)
	{
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
	}

	GameObject CreateUnityMap(Map mapSO, string backgroundAssetFolderPath, bool useAssetSubSprites, bool dontTranslateUnknown, bool setNotValidToUnknown, bool setTileTypeForNonValidTiles)
	{
		if(mapSO == null)
		{
			Debug.LogError("mapSO == NULL");
			return null;
		}

		MapLayer[] mapDataRaw = mapSO.GetMapDataRaw();
		if(mapDataRaw == null)
		{
			Debug.LogError("mapSO.GetMapDataRaw() == NULL -> keine Informationen über Tile<->SubSprite vorhanden");
			return null;
		}

//		TilesetTile[,,] mapData = mapSO.GetMapData();
		MapLayer[] mapData = mapSO.GetMapData();
		if(mapData == null)
		{
			Debug.LogError("mapSO.GetMapData() == NULL -> keine Informationen über Tile<->SubSprite vorhanden");
			return null;
		}

//		bool[,,] customMapData = mapSO.GetCustomMapData();
		MapDataFlags[] customMapData = mapSO.GetCustomMapData();
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
		CreateBackground (mapRootGO, mapSO, backgroundAssetFolderPath);

		// Tiles Layer 0-4 & Animated Layer
		GameObject animatedLayerGO = new GameObject("Animated Layer");
		animatedLayerGO.transform.SetParent(mapRootGO.transform);
		animatedLayerGO.transform.localPosition = new Vector3(0f,0f,-10f);
		for(int l=0; l<Globals.MAPLAYERS; l++)
		{
			GameObject currentMapLayerGO = new GameObject("Layer " + l);
			currentMapLayerGO.transform.SetParent(mapRootGO.transform);
			currentMapLayerGO.transform.localPosition = new Vector3(0f,0f,(l+1)*-2f);		//(l+1) (trenne layer von background)
			for(int y=0; y<Globals.MAPHEIGHT; y++)
			{
				for(int x=0; x<Globals.MAPWIDTH; x++)
				{

					TilesetTile currentRawTilesetTile = mapDataRaw[l].GetTile(x,y);
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

							if(useAssetSubSprites)
							{
								if(translatedTile.iTilesetID == Globals.TILESETANIMATED)
								{
									currentTileGO.transform.SetParent(animatedLayerGO.transform, true);
									currentSpriteRenderer.sortingLayerName = "MapTileAnimated";
//									Vector3 animTilePos = currentTileGO.transform.localPosition;
//									animTilePos.z = -10;
//									currentTileGO.transform.localPosition = animTilePos;

									//Animations Script an Gameobject fügen
									// mit Sprites füllen
									//TODO
									Sprite[] animationSprites;
									animationSprites = tileSet.GetAnimationTileSprites(tilePosX, tilePosY);
									if(animationSprites != null)
									{
										AnimatedTile animScript = currentTileGO.AddComponent<AnimatedTile>();
										animScript.reverseAnim = false;
										animScript.SetAnimation(animationSprites);
									}
									else
									{
										Debug.LogError("Animated Tile animationSprites == null: " + tileSet.tilesetName + " iCol" + tilePosX + " iRow" + tilePosY);
									}
								}
								else
								{
									tileSprite = tileSet.GetTileSprite(tilePosX, tilePosY);
									currentSpriteRenderer.sprite = tileSprite;
								}
							}
							else
							{
								tileSprite = tileSet.GetNewCreatetTileSprite(tilePosX, tilePosY);
								currentSpriteRenderer.sprite = tileSprite;
							}

							// TODO OBSOLETE!!!
//							if (useTileType)
//							{
//								TileType currentTileType = tileSet.GetTileType((short)tilePosX, (short)tilePosY);
//								
//								TileTypeToUnityTranslation(currentTileType, currentTileGO);
//								
//								TileScript currenTileScript = currentTileGO.AddComponent<TileScript>();
//								currenTileScript.tileSet = tileSet;
//								currenTileScript.tileType = currentTileType;
//							}
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
//		// create 4 SwitchConnections
		mapSO.CreateSwitchConnections ();
		EditorUtility.SetDirty (mapSO);

		int layer = Globals.MAPLAYERS;			
		layer++;									// +1 because Background and Layer 0 are two different layers 
		MapBlockLayer mapObjectData = mapSO.GetObjectData();
		if (mapObjectData != null)	
		{
			GameObject mapObjectDataLayerGO = new GameObject("ObjectData");
			mapObjectDataLayerGO.transform.SetParent(mapRootGO.transform);
			mapObjectDataLayerGO.transform.localPosition = new Vector3(0f,0f,(layer)*-2f);		//(l+1) (trenne layer von background)

			string spriteRendererLayer = "MapObjectDataLayer";

			for(int y=0; y<Globals.MAPHEIGHT; y++)
			{
				for(int x=0; x<Globals.MAPWIDTH; x++)
				{
					MapBlock currenObjectDataMapBlock = mapObjectData.GetBlock(x,y);
					if(currenObjectDataMapBlock != null)
					{
						if (currenObjectDataMapBlock.iType != 255)
						{
							string tileTypeString = ""+ ((TileType) currenObjectDataMapBlock.iType);
							GameObject currentTileGO = new GameObject(x.ToString("D2") + " " + y.ToString("D2") + " " + tileTypeString );
							currentTileGO.transform.SetParent(mapObjectDataLayerGO.transform);
							Vector3 tileLocalPos = new Vector3(-Globals.MAPWIDTH*0.5f +x,		// x+1: pivot Right , x+0.5f: pivor Center, x: pivot Left
							                                   Globals.MAPHEIGHT*0.5f -(y+1),	// y-1: pivot Bottom, y-0.5f: pivot Center, y: pivot Top //TODO Tileset SlicedSprite Pivot setzen!
							                                   0f);
							currentTileGO.transform.localPosition = tileLocalPos;

							TileScript currenTileScript = currentTileGO.AddComponent<TileScript>();
							currenTileScript.SetMapBlock (currenObjectDataMapBlock);
							MapBlock mapBlock = currenObjectDataMapBlock;
							if (mapBlock.iType < (short) 7 ||
							    mapBlock.iType > (short) 14 && mapBlock.iType < (short) 22)
							{
								SpriteRenderer currentTileSprite = currentTileGO.GetComponent<SpriteRenderer>();
								if (currentTileSprite == null)
								{
									currentTileSprite = currentTileGO.AddComponent<SpriteRenderer>();
									currentTileSprite.sprite = g_TilesetManager.GetBlockSprite (currenObjectDataMapBlock.iType);
								}
								currentTileSprite.sortingLayerName = spriteRendererLayer;

								if (mapBlock.iType == (short) 0)
								{
									// BreakableBlock [#]
									BreakableBlock blockScript = currentTileGO.AddComponent <BreakableBlock> ();
									blockScript.CreateBlock ();
								}
								else if (mapBlock.iType == (short) 1)
								{
									// PowerUp Block [?]
									// global settings: enabled powerups

									List<string> relFilePathList = new List<string> ();
									List<GameObject> powerUpGOs = new List<GameObject> ();
									
									string powerUpsFolderPath = Application.dataPath + "/Resources/Items/";
									FileInfo[] info = GetFileList (powerUpsFolderPath, "*.prefab");
									if (info != null)
									{
//										Debug.Log ("Test " + info.ToString());
										for (int i=0; i<info.Length; i++)
										{
//											Debug.Log ("info [" + i + "]");
											
											if (info[i] != null)
											{
//												Debug.Log ("info [" + i + "] = " + info[i]);
												
												string absPath = info[i].FullName;//.Substring( Application.dataPath.Length-1 );
												string relPath = "";

//												Debug.LogWarning (System.IO.Path.GetFullPath (Application.dataPath));
//												Debug.LogWarning (System.IO.Path.GetFullPath (absPath));

												if (absPath.StartsWith(System.IO.Path.GetFullPath(Application.dataPath))) {
													relPath=  "Assets" + absPath.Substring(Application.dataPath.Length);
													relFilePathList.Add (relPath);
													GameObject assetGo = AssetDatabase.LoadAssetAtPath<GameObject> (relPath);
													powerUpGOs.Add (assetGo);
//													Debug.Log ("==\n" + Application.dataPath + "\n" + absPath + "\n" + relPath);
												}
												else
												{
													Debug.Log ("!=\n" +Application.dataPath + "\n" + absPath);
												}
//												Debug.Log ("datPath = " + Application.dataPath);
//												Debug.Log ("absPath = " + absPath);
//												Debug.Log ("relPath = " + relPath);
											}
										}
									}
									else 
										Debug.LogError ("keine Dateien (PowerUps) gefunden");
//									GameObject[] powerUpGOs = (GameObject[]) AssetDatabase.LoadAllAssetsAtPath (powerUpsFolderPath);

//									string[] assetsPaths = AssetDatabase.GetAllAssetPaths ();
//									foreach (string assetPath in assetsPaths) {
//										if (assetPath.Contains (powerUpsFolderPath)) {
//											powerUpsFolderPath.Add(assetPath);
//										}   
//									}

//									for (int i=0; i<powerUpGOs.Count; i++)
//									{
//										Debug.Log ("GO gefunden: " + powerUpGOs[i].name);
//									}

									PowerUpBlock blockScript = currentTileGO.AddComponent <PowerUpBlock> ();
									blockScript.CreateBlock (powerUpGOs.ToArray ());
									
								}
								else if (mapBlock.iType == (short) 2)
								{
									// PowerUp Block [?]
									// global settings: enabled powerups
									
								}
							}
							else if (mapBlock.iType >= (short) 7 &&
							         mapBlock.iType <= (short) 10)
							{
								// ON-Switch [ON]
								CreateSwitchGO (mapSO, mapBlock,currentTileGO, spriteRendererLayer);
//								SwitchConnection currSwitchConnection = mapSO.SwitchConnections[mapBlock.iType-7];
//								OnOffSwitchBlockScript ioSwitchScript = currentTileGO.AddComponent<OnOffSwitchBlockScript> ();
//								bool state = false;
//								Sprite defaultStateSprite;
//								Sprite otherStateSprite;
//								if (mapSO.SwitchStates[mapBlock.iType-7] == 0)
//								{
//									state = false;
//									defaultStateSprite = g_TilesetManager.GetBlockSprite (mapBlock.iType+15);
//									otherStateSprite = g_TilesetManager.GetBlockSprite (mapBlock.iType);
//								}
//								else // if (mapSO.SwitchStates[mapBlock.iType-7] == 1)
//								{
//									state = true;
//									defaultStateSprite = g_TilesetManager.GetBlockSprite (mapBlock.iType);
//									otherStateSprite = g_TilesetManager.GetBlockSprite (mapBlock.iType+15);
//								}
//
//								ioSwitchScript.CreateBlock (state,defaultStateSprite,otherStateSprite, currSwitchConnection, spriteRendererLayer);
//								currSwitchConnection.AddSwitchScript (ioSwitchScript);
							}
							else if (mapBlock.iType >= (short) 22 &&
							         mapBlock.iType <= (short) 25)
							{
								// OFF-Switch [OFF]
								CreateSwitchGO (mapSO, mapBlock,currentTileGO, spriteRendererLayer);
							}
							else if (mapBlock.iType >= (short) 11 &&
							         mapBlock.iType <= (short) 14)
							{
								CreateSwitchTargetGO (mapSO, mapBlock, currentTileGO, spriteRendererLayer);
								// ON [!] /OFF Block [ ]
//								if (mapBlock.GetSetting (0) == (short) 0)
//								{
//									// "[ ]";
//								}
//								else if (mapBlock.GetSetting (0) == (short) 1)
//								{
//									// "[!]";
//								}
							}
							else if (mapBlock.iType >= (short) 26 &&
							         mapBlock.iType <= (short) 29)
							{
								CreateSwitchTargetGO (mapSO, mapBlock, currentTileGO, spriteRendererLayer);
								// ON [!] /OFF Block [ ]
//								if (mapBlock.GetSetting (0) == (short) 0)
//								{
//									// "[ ]";
//								}
//								else if (mapBlock.GetSetting (0) == (short) 1)
//								{
//									// "[!]";
//								}
							}
							else
							{
								SpriteRenderer currentTileSprite = currentTileGO.GetComponent<SpriteRenderer>();
								if (currentTileSprite == null)
								{
									currentTileSprite = currentTileGO.AddComponent<SpriteRenderer>();
									currentTileSprite.sprite = g_TilesetManager.GetBlockSprite (currenObjectDataMapBlock.iType);
								}
								currentTileSprite.sortingLayerName = spriteRendererLayer;
							}


							if (currenObjectDataMapBlock.fHidden)
							{
								SpriteRenderer currentTileSprite = currentTileGO.GetComponent<SpriteRenderer>();
								if (currentTileSprite == null)
								{
									currentTileSprite = currentTileGO.AddComponent<SpriteRenderer>();
									currentTileSprite.sprite = g_TilesetManager.GetBlockSprite (currenObjectDataMapBlock.iType);
								}
								currentTileSprite.sortingLayerName = spriteRendererLayer;
								currentTileSprite.color = new Color (1,1,1,0.5f);
							}
						//currenTileScript.Add(currentMapBlock)
						//currenTileScript.Add(currentTilesetTile)
						//currenTileScript.Add(currentMapTile)
						//currenTileScript.Add(currentMovingPlatform)
						}
					}
				}
			}
		}
		else
			Debug.LogError("Map: " + mapSO.mapName + " GetObjectData == NULL");

		layer++;
//		MapTile[,] mapDataTop = mapSO.GetMapDataTop();
		MapTopLayer mapDataTop = mapSO.GetMapDataTop();
		if (mapDataTop != null)
		{
			// mapDataTop
			GameObject mapDataTopLayerGO = new GameObject("MapDataTop");
			mapDataTopLayerGO.transform.SetParent(mapRootGO.transform);
			mapDataTopLayerGO.transform.localPosition = new Vector3(0f,0f,(layer)*-2f);		//(l+1) (trenne layer von background)

			GameObject mapDataTopLayerTileTypesGO = new GameObject("TileTypes");
			mapDataTopLayerTileTypesGO.transform.SetParent(mapDataTopLayerGO.transform);
			

			for(int y=0; y<Globals.MAPHEIGHT; y++)
			{
				for(int x=0; x<Globals.MAPWIDTH; x++)
				{
					MapTile currentMapDataTopTile = mapDataTop.GetTile(x,y);
					if (currentMapDataTopTile != null)
					{
						if (currentMapDataTopTile.iType != TileType.tile_nonsolid)
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

							CreateTileTypeSprite (mapDataTopLayerTileTypesGO, tileLocalPos, currentMapDataTopTile, x.ToString("D2") + " " + y.ToString("D2") + " " + tileTypeString);
						}
					}
				}
			}
		}
		else
			Debug.LogError("Map: " + mapSO.mapName + " GetMapDataTop == NULL");

		//
		CreateMapCollider (mapSO, mapRootGO);

		// Platforms Translated + Raw
		CreatePlatformGOs (mapSO, mapRootGO, layer, useAssetSubSprites);

		CreateHazards (mapSO, mapRootGO);

		WarpsPreview (mapSO, mapRootGO);

		WarpExitsPreview (mapSO, mapRootGO);

//		mapSO.ConnectSwitchBlocks ();
//		EditorUtility.SetDirty (mapSO);

		return mapRootGO;
	}

	public void CreateSwitchGO (Map mapSO, MapBlock mapBlock, GameObject currentTileGO, string spriteRendererLayer)
	{
		int sub = 0;
		int addBlockNum = 15;
		if (mapBlock.iType >= (short) 7 &&
		    mapBlock.iType <= (short) 10)
		{
			sub = 7;
		}
		else if (mapBlock.iType >= (short) 22 &&
		         mapBlock.iType <= (short) 25)
		{
			sub = 22;
			addBlockNum *= -1;
		}

		SwitchConnection currSwitchConnection = mapSO.SwitchConnections[mapBlock.iType-sub];
		OnOffSwitchBlockScript ioSwitchScript = currentTileGO.AddComponent<OnOffSwitchBlockScript> ();
		bool state = false;
		Sprite onStateSprite;
		Sprite offStateSprite;
		if (mapSO.SwitchStates[mapBlock.iType-sub] == 0)
		{
			state = false;
			onStateSprite = g_TilesetManager.GetBlockSprite (mapBlock.iType);
			offStateSprite = g_TilesetManager.GetBlockSprite (mapBlock.iType + addBlockNum);
		}
		else 
		{
			state = true;
			onStateSprite = g_TilesetManager.GetBlockSprite (mapBlock.iType);
			offStateSprite = g_TilesetManager.GetBlockSprite (mapBlock.iType + addBlockNum);
		}

		ioSwitchScript.CreateBlock (mapBlock, state, onStateSprite, offStateSprite, currSwitchConnection, spriteRendererLayer);
		currSwitchConnection.AddSwitchScript (ioSwitchScript);
	}

	public void CreateSwitchTargetGO (Map mapSO, MapBlock mapBlock, GameObject currentTileGO, string spriteRendererLayer)
	{
		int sub = 0;
		int addBlockNum = 15;
		if (mapBlock.iType >= (short) 11 &&
		    mapBlock.iType <= (short) 14)
		{
			sub = 11;
		}
		else if (mapBlock.iType >= (short) 26 &&
		         mapBlock.iType <= (short) 29)
		{
			sub = 26;
			addBlockNum *= -1;
		}
		
		SwitchConnection currSwitchConnection = mapSO.SwitchConnections[mapBlock.iType-sub];
		SwitchTargetBlockScript switchTargetScript = currentTileGO.AddComponent<SwitchTargetBlockScript> ();
		bool state = false;
		Sprite onStateSprite = null;
		Sprite offStateSprite = null;

		if (mapBlock.SettingsCount () < 1)
		{
			Debug.LogError(this.ToString () + " mapBlock (Switch) has no Settings", currentTileGO);
			return; 
		}

		if (mapBlock.GetSetting (0) == (short) 0)
		{
			state = false;
			onStateSprite = g_TilesetManager.GetBlockSprite (mapBlock.iType);
			offStateSprite = g_TilesetManager.GetBlockSprite (mapBlock.iType + addBlockNum);
		}
		else if (mapBlock.GetSetting (0) == (short) 1)
		{
			state = true;
			onStateSprite = g_TilesetManager.GetBlockSprite (mapBlock.iType);
			offStateSprite = g_TilesetManager.GetBlockSprite (mapBlock.iType + addBlockNum);
		}
		else 
		{
			Debug.LogError (this.ToString () + " ?!!! STATE FALSE OR TRUE ?!!!!");
		}
		
		switchTargetScript.CreateBlock (mapBlock ,state,onStateSprite,offStateSprite, spriteRendererLayer);
		currSwitchConnection.AddBlockScript (switchTargetScript);
	}

	public void CreatePlatformGOs (Map mapSO, GameObject mapRootGO, int layer, bool useAssetSubSprites)
	{
		// Platforms Translated + Raw
		layer++;
		MovingPlatform[] platforms = mapSO.GetPlatforms();
		if (platforms != null)
		{
			// Platforms Raw
			GameObject mapPlatformsLayerGO = new GameObject("Platforms Raw");
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
					MovingPlatformScript currentPlatformScript = currenPlatformGO.AddComponent<MovingPlatformScript>();
					currentPlatformScript.movingPlatform = currentPlatform;

					// TODO grip on platforms
//					Rigidbody2D rb2d = currenPlatformGO.AddComponent<Rigidbody2D> ();
//					rb2d.gravityScale = 0f;
//					rb2d.fixedAngle = true;
					
					float offsetX = 0f;
					float offsetY = 0f;
					
					if (currentPlatform.path.iPathType == (short) MovingPathType.StraightPath)
					{
						currenPlatformGO.name += " StraightPath"; 
						// Start -> End -> Start ...
						offsetX = currentPlatform.path.startX / 32.0f - 10f;
						offsetY = 15f - currentPlatform.path.startY / 32.0f - 7.5f;
						
					}
					else if (currentPlatform.path.iPathType == (short) MovingPathType.StraightPathContinuous)
					{
						currenPlatformGO.name += " StraightPathCont"; 
						// Platform need to Beam!
						// Start @ angle -> velocity
						offsetX = currentPlatform.path.startX / 32.0f - 10f;
						offsetY = 15f - currentPlatform.path.startY / 32.0f - 7.5f;
					}
					else if (currentPlatform.path.iPathType == (short) MovingPathType.EllipsePath)
					{
						currenPlatformGO.name += " EllipsePath"; 
						// Center + r @ angle
						offsetX = currentPlatform.path.dCenterX / 32.0f - 10f;
						offsetY = 15f - currentPlatform.path.dCenterY / 32.0f - 7.5f;
						offsetX += Mathf.Sin (currentPlatform.path.dAngle) * currentPlatform.path.dRadiusX / 32.0f; 
						offsetY += Mathf.Cos (currentPlatform.path.dAngle) * currentPlatform.path.dRadiusY / 32.0f; 
					}
					
					currenPlatformGO.transform.position =  new Vector3 (offsetX, offsetY, 0f);
//					Vector3 additionalOffsetToCenter = Vector3.zero;
//					additionalOffsetToCenter.x = -currentPlatform.iPlatformWidth *0.5f;
//					additionalOffsetToCenter.y = currentPlatform.iPlatformHeight *0.5f;
//					currenPlatformGO.transform.position += additionalOffsetToCenter;

					// Create Colliders (merged)
					CreatePlatformColliders (currenPlatformGO, currentPlatform);

					// Create Sprites
					GameObject currentPlatformGOSprites = new GameObject ("Sprites");
					currentPlatformGOSprites.transform.SetParent (currenPlatformGO.transform, false);

					// Visible MapTopData (TileType)
					GameObject currentPlatformGOTileTypes = new GameObject ("TileTypes");
					currentPlatformGOTileTypes.transform.SetParent (currenPlatformGO.transform, false);
					
					for(int y=0; y<currentPlatform.iPlatformHeight; y++)
					{
						for(int x=0; x<currentPlatform.iPlatformWidth; x++)
						{
							TilesetTile currentTilesetTileRaw = currentPlatform.platformTilesRaw.GetTile(x,y);
							TilesetTile currentTilesetTileTranslated = currentPlatform.platformTiles.GetTile(x,y);
							MapTile currentMapTile = currentPlatform.platformTileTypes.GetTile(x,y);
							if(currentTilesetTileRaw != null)
							{
								bool animated = false;
								if (currentTilesetTileRaw.iTilesetID == Globals.TILESETNONE)
									continue;
								else if (currentTilesetTileRaw.iTilesetID == Globals.TILESETANIMATED)
								{
									animated = true;
									currentTilesetTileTranslated.iTilesetID = Globals.TILESETANIMATED;
									currentTilesetTileTranslated.iCol = currentTilesetTileRaw.iCol;
									currentTilesetTileTranslated.iRow = currentTilesetTileRaw.iRow;
//									Debug.Log (this.ToString () + " ANIMATED", go);
								}
								
								string tileTypeString = "";
								if (currentMapTile != null)
									tileTypeString += currentMapTile.iType;
								else
									tileTypeString += "NULL";
								
								GameObject currentPlatformTileGO = new GameObject(x.ToString("D2") + " " + y.ToString("D2") + " " + tileTypeString );
								currentPlatformTileGO.transform.SetParent(currentPlatformGOSprites.transform);
								//								Vector3 tileLocalPos = new Vector3(-Globals.MAPWIDTH*0.5f +x,		// x+1: pivot Right , x+0.5f: pivor Center, x: pivot Left
								//								                                   Globals.MAPHEIGHT*0.5f -(y+1),	// y-1: pivot Bottom, y-0.5f: pivot Center, y: pivot Top //TODO Tileset SlicedSprite Pivot setzen!
								//								                                   0f);
								Vector3 tileLocalPos = new Vector3(-currentPlatform.iPlatformWidth*0.5f +x,		// x+1: pivot Right , x+0.5f: pivor Center, x: pivot Left
								                                   currentPlatform.iPlatformHeight*0.5f -(y+1),	// y-1: pivot Bottom, y-0.5f: pivot Center, y: pivot Top //TODO Tileset SlicedSprite Pivot setzen!
								                                   0f);
								currentPlatformTileGO.transform.localPosition = tileLocalPos;
								
								SpriteRenderer tileRenderer = currentPlatformTileGO.AddComponent<SpriteRenderer>();
								tileRenderer.sortingLayerName = "MapPlatformLayer";
								
								int iTileSetId = currentTilesetTileTranslated.iTilesetID;
								int tilePosX = currentTilesetTileTranslated.iCol;
								int tilePosY = currentTilesetTileTranslated.iRow;
								
								Tileset tileSet = g_TilesetManager.GetTileset(iTileSetId);
								if(animated)
								{
									Sprite[] animationSprites;
									animationSprites = tileSet.GetAnimationTileSprites(tilePosX, tilePosY);
									if(animationSprites != null)
									{
										AnimatedTile animTileScript = currentPlatformTileGO.AddComponent<AnimatedTile>();
										animTileScript.reverseAnim = false;
										animTileScript.SetAnimation(animationSprites);
									}
									else
									{
										Debug.LogError("Animated Tile animationSprites == null: " + tileSet.tilesetName + " iCol" + tilePosX + " iRow" + tilePosY);
									}
								}
								else
								{
									Sprite tileSprite;
									if(useAssetSubSprites)
										tileSprite = tileSet.GetTileSprite(tilePosX, tilePosY);
									else
										tileSprite = tileSet.GetNewCreatetTileSprite(tilePosX, tilePosY);	
									tileRenderer.sprite = tileSprite;
								}

								// WRONG
//								TileType currentTileType = tileSet.GetTileType((short)tilePosX, (short)tilePosY);
								// FIX:
								TileType currentTileType = currentMapTile.iType;
								TileType defaultTileType;
								bool useDefaultTileType = true;
								if (animated)
								{
									useDefaultTileType = false;
									defaultTileType = TileType.tile_NA;
								}
								else
								{
									defaultTileType = tileSet.GetTileType((short)tilePosX, (short)tilePosY);
								}
								//TODO
//								TileTypeToUnityTranslation(currentTileType, currentPlatformTileGO);
								//TODO replaced by merging collider algorithm
//								TileTypeToUnityTranslation(currentMapTile.iType, currentPlatformTileGO);

								// Debug
//								GameObject child = new GameObject (currentMapTile.iType + "");
//								GameObject child = new GameObject (x.ToString("D2") + " " + y.ToString("D2") + " " + tileTypeString);
//								child.transform.SetParent (currentPlatformGOTileTypes.transform, false);
//								child.transform.position = currentPlatformTileGO.transform.position;
//								child.layer = LayerMask.NameToLayer ("Debug");
//								if (currentMapTile.iType != TileType.tile_nonsolid)
//								{
//									SpriteRenderer tileTypeRenderer = child.AddComponent <SpriteRenderer> ();
//									tileTypeRenderer.sprite = g_TilesetManager.GetLevelEditorTileTypeTileset ().GetTileSprite ((int)currentMapTile.iType-1,0);
//									tileTypeRenderer.sortingLayerName = "MapDebug";
//									tileTypeRenderer.color = new Color (1,1,1,0.6f);
//								}
								CreateTileTypeSprite (currentPlatformGOTileTypes, currentPlatformTileGO.transform.position, currentMapTile, x.ToString("D2") + " " + y.ToString("D2") + " " + tileTypeString); 
								// Debug


								TileScript tileScript = currentPlatformTileGO.AddComponent <TileScript> ();
								tileScript.mapTile = currentMapTile;
								tileScript.tileSet = tileSet;
								tileScript.tilesetPosX = tilePosX;
								tileScript.tilesetPosY = tilePosY;
								tileScript.tileType = currentTileType;
								if (useDefaultTileType)
									tileScript.defaultTileType = defaultTileType;
								else 
									tileScript.defaultTileType = defaultTileType;
									
							}
							
						}
					}
					// create Clones
					List<GameObject> clones = CreateClones (currenPlatformGO);
					for (int j=0; j<clones.Count; j++)
					{
						MovingPlatformScript[] scripts = clones[j].GetComponents <MovingPlatformScript> ();
						for (int k=0; k<scripts.Length; k++)
						{
							DestroyImmediate (scripts[k]);
						}
					}
					ParentingClones (clones, currenPlatformGO);
				}
			}
		}
		else
			Debug.Log("Map: " + mapSO.mapName + " Platforms == NULL -> Map hat keine MovingPlatform");
	}

	public void CreateTileTypeSprite (GameObject parentGO, Vector3 position, MapTile currentMapTile, string goName)
	{
		GameObject child = new GameObject (goName);
		child.transform.SetParent (parentGO.transform, false);
		child.transform.position = position;
		child.layer = LayerMask.NameToLayer ("Debug");
		if (currentMapTile.iType != TileType.tile_nonsolid)
		{
			SpriteRenderer tileTypeRenderer = child.AddComponent <SpriteRenderer> ();
			tileTypeRenderer.sprite = g_TilesetManager.GetLevelEditorTileTypeTileset ().GetTileSprite ((int)currentMapTile.iType-1,0);
			tileTypeRenderer.sortingLayerName = "MapDebug";
			tileTypeRenderer.color = new Color (1,1,1,0.6f);
		}
	}

	public void CreatePlatformColliders (GameObject platformGO, MovingPlatform platformData)
	{
		GameObject platformColliders = new GameObject ("Colliders");
		platformColliders.transform.SetParent (platformGO.transform, false);

		for(int y=0; y< platformData.iPlatformHeight; y++)
		{
//			Debug.Log ("<color=magenta>y=" + y + "</color>");

			for(int x=0; x< platformData.iPlatformWidth; x++)
			{
//				Debug.Log ("<color=red><b>"+x+"</b></color=red>");
				x = x + 0;

				MapTile currentStartRefTile = platformData.platformTileTypes.GetTile (x, y);
				
//				if (currentStartRefTile.iType == TileType.tile_nonsolid)
//					continue;

				int currentWidth = 1;	// FIX was  = 0
				int restWidth = platformData.iPlatformWidth - x;

//				Debug.Log ("<color=blue>x=" + x + ", y= " + y + "= " + currentStartRefTile.iType + "=?=</color>");

				bool valid = true;

				for (int xa=1; xa< restWidth && valid; xa++)
				{
					MapTile currentTile = platformData.platformTileTypes.GetTile (x+xa, y);
//					Debug.Log ("<b><color=lime>" + xa + "/" + restWidth + "</color></b>" );
					
					currentWidth = xa;
					if (currentTile.iType != currentStartRefTile.iType)
					{
//						Debug.Log ("<color=red>!=" + "x+a=" + (x+xa) + ", y= " + y + "= " + currentTile.iType + "</color>" );
//						break;
						valid = false;
					}
					else
					{
//						Debug.Log ("<color=green>==" + "x+a=" + (x+xa) + ", y= " + y + "= " + currentTile.iType + "</color>" );
						currentWidth++; // TODO FIX +1 da letzter (letzer == rand) nicht mehr gecheckt wird
					}
				}

				GameObject currentColliderGO = new GameObject ( restWidth + " " + currentStartRefTile.iType + " " + platformData.iPlatformWidth + "/" + platformData.iPlatformHeight + " (" +x+ "," +y+ ") " + currentWidth);
				currentColliderGO.transform.SetParent (platformColliders.transform, false);

				if (currentStartRefTile.iType == TileType.tile_solid)
				{
					BoxCollider2D box = currentColliderGO.AddComponent <BoxCollider2D> ();
//					box.bounds = new Vector2 ();
					box.size = new Vector2 (currentWidth,1f);
//					box.offset = refPoint + startPoint + currentPoint;
					
					//Calc offset
					Vector2 startPoint = new Vector2 (-platformData.iPlatformWidth*0.5f, platformData.iPlatformHeight*0.5f -1);
//					Vector2 refPoint =  (Vector2) currentColliderGO.transform.position;
//					Vector2 refPoint2 =  TransformPixelPositionToUnityWorldPosition( new Vector2 (platformData.path.startX,platformData.path.startY));
					Vector2 currentPoint = new Vector2 (x,-y);
//					Debug.Log ("NEW", currentColliderGO);
//					Debug.Log ("refPoint = " + refPoint, currentColliderGO);
//					Debug.Log ("refPoint2 = " + refPoint2, currentColliderGO);
//					Debug.Log ("startPoint = " + startPoint, currentColliderGO);
//					Debug.Log ("currentPoint = " + currentPoint, currentColliderGO);
//					Debug.Log ("offset = " + refPoint + startPoint + currentPoint, currentColliderGO);

					Vector2 platformMergeShift = Vector2.zero;
					platformMergeShift.x = currentWidth *0.5f;
					platformMergeShift.y = 0.5f;
					
					box.offset = startPoint + currentPoint + platformMergeShift;
					currentColliderGO.layer = LayerMask.NameToLayer (Layer.groundLayerName);
				}
				else if (currentStartRefTile.iType == TileType.tile_solid_on_top)
				{
					BoxCollider2D box = currentColliderGO.AddComponent <BoxCollider2D> ();
					box.size = new Vector2 (currentWidth,0.1f);

					//Calc offset
					Vector2 startPoint = new Vector2 (-platformData.iPlatformWidth*0.5f, platformData.iPlatformHeight*0.5f -1);
//					Vector2 refPoint =  (Vector2) currentColliderGO.transform.position;
					Vector2 currentPoint = new Vector2 (x,-y);

					Vector2 platformMergeShift = Vector2.zero;
					platformMergeShift.x = currentWidth *0.5f;	// halbe breite
					platformMergeShift.y = 0.5f + (0.5f - box.size.y*0.5f);	// halbe höhe (==immer 1/2) + ...

					box.offset = startPoint + currentPoint + platformMergeShift;
					
//					box.offset = new Vector2 (-platformData.iPlatformWidth*0.5f, platformData.iPlatformHeight*0.5f -0.05f) + new Vector2 (x,-y) + (Vector2) currentColliderGO.transform.position;
					currentColliderGO.layer = LayerMask.NameToLayer (Layer.jumpAblePlatformLayerName);
				}

//				Debug.Log ("<color=grey><b>+" + currentWidth + "</b></color>" );
				x += currentWidth-1;
//				Debug.Log ("<color=magenta><b>"+x+"</b></color=red>");
			}
		}
	}

	public void CreateHazards (Map mapSO, GameObject mapRootGO)
	{
		MapHazard[] hazards = mapSO.GetHazards ();

		if (hazards == null)
		{
			Debug.Log ("keine Hazards!");
			return;
		}

		GameObject hazardsRoot = new GameObject ("Hazards");
		hazardsRoot.transform.SetParent (mapRootGO.transform);

		for (int i=0; i< hazards.Length; i++)
		{
//			GameObject hazardGO = new GameObject ("Hazard " + i + " (" +  + ")");
			MapHazard currHazard = hazards[i];
			if (currHazard != null)
			{
				GameObject hazardGO = new GameObject (i + " " + currHazard.iType);
				hazardGO.transform.SetParent (hazardsRoot.transform);

				Vector3 originalPos = new Vector3 (currHazard.ix,currHazard.iy,0f);
				Vector3 translatedPos = Vector3.zero;
//				originalPos.x = ((int)(originalPos.x)) << 4; 		// bitwise Left shift operator
//				originalPos.y = ((int)(originalPos.y)) << 4;			// bitwise Left shift operator


				Hazard listHazard = g_HazardManager.GetHazard (currHazard.iType);
				
				if (currHazard.iType == HazardType.bullet_bill)
				{
//					SpriteRenderer sr = hazardGO.AddComponent<SpriteRenderer> ();
//					sr.sortingLayerName = "MapDebug";
//					sr.sprite = listHazard.previewSprite;

					translatedPos = TransformHazardPositionToUnityWorld (originalPos);
					BulletBillScript script = hazardGO.AddComponent <BulletBillScript> ();
					script.hazard = currHazard;
				}
				else if (currHazard.iType == HazardType.fireball_string)
				{
					translatedPos = TransformOrbitHazardPositionToUnityWorld (originalPos);

					MovingPlatformScript movingScript = hazardGO.AddComponent <MovingPlatformScript> ();

					float vel = currHazard.dparam[0];
					float angle = currHazard.dparam[1];

					// w = 32
					// w = 18
					// d = 0.2 == 32/5 = 6.4
					// l = 2 -> r =  (l * w + (l-1) * d)/ 2

					// 1 = 32f
					// r = 18f + iparm[0] * 32f * 0.2f * 0.5f

					bool depthTest = false;
					float duration = 2f;
					Color color = Color.green;

					Vector3 refPos = hazardGO.transform.position;
					Debug.DrawLine (refPos + Vector3.up * 20f, refPos + Vector3.down * 20f, color, duration, depthTest);
					Debug.DrawLine (refPos + Vector3.left * 20f, refPos + Vector3.right * 20f, color, duration, depthTest);
					Debug.Log ("size =" + listHazard.previewSprite.bounds.size);
					
//					float radiusInPixel = currHazard.iparam[0] * 0.5f * 32f;		// nicht 100% genau
					float width = listHazard.previewSprite.bounds.size.x * 32f;
					float quantity = currHazard.iparam[0];
					float distanceBetween = 0.2f * 32f;
					float durchmesserInPixel = quantity * width;
					if (quantity > 1)
						durchmesserInPixel += (quantity-1)*distanceBetween;

					float radiusInPixel = durchmesserInPixel * 0.5f;

					Vector2 center = GetPixelPositionFromUnityWorld (hazardGO.transform.position);

					MovingPlatformPath path = new MovingPlatformPath( vel, angle, radiusInPixel, radiusInPixel, center.x, center.y, true );
					path.iPathType = (int) MovingPathType.HazardString;
					movingScript.movingPlatform = new MovingPlatform(null,null,null, currHazard.iparam[0], 1, 1,path,true);

					// iparm[0] == fireball string länge
					for (int k=0; k< currHazard.iparam[0]; k++)
					{

						float distance = listHazard.previewSprite.bounds.size.x + 0.2f;		// distance between string elements
//						Debug.Log ("NEW");
//						Debug.Log ("center =" + listHazard.previewSprite.bounds.center);
//						Debug.Log ("extents =" + listHazard.previewSprite.bounds.extents);
//						Debug.Log ("size =" + listHazard.previewSprite.bounds.size);

						GameObject stringPart = new GameObject (currHazard.iType + " " + k);
						stringPart.transform.SetParent (hazardGO.transform);
						stringPart.transform.localPosition = new Vector3 (k*distance, 0f, 0f);

						Vector3 offset = - listHazard.previewSprite.bounds.center;
//						offset.x = -listHazard.previewSprite.bounds.center.x;
//						offset.y = listHazard.previewSprite.bounds.center.y;



						stringPart.transform.localPosition += offset;

						SpriteRenderer renderer = stringPart.AddComponent <SpriteRenderer> ();
						renderer.sortingLayerName = "Hazards";
						AnimatedTile animScript = stringPart.AddComponent <AnimatedTile> ();

						animScript.reverseAnim = false;
						animScript.SetAnimation (listHazard.projectile.ToArray());
					}


					refPos.x += durchmesserInPixel / 32f;
					Debug.DrawLine (refPos + Vector3.up, refPos + Vector3.down, color, duration, depthTest);
					Debug.DrawLine (refPos + Vector3.left, refPos + Vector3.right, color, duration, depthTest);

					refPos.x += radiusInPixel / 32f;
					Debug.DrawLine (refPos + Vector3.up, refPos + Vector3.down, color, duration, depthTest);
					Debug.DrawLine (refPos + Vector3.left, refPos + Vector3.right, color, duration, depthTest);
					
				}
				else if (currHazard.iType == HazardType.flame_cannon)
				{
					translatedPos = TransformHazardPositionToUnityWorld (originalPos);
					hazardGO.AddComponent <FlameCannonScript> ().Create (currHazard, listHazard);
				}
				else if (currHazard.iType == HazardType.pirhana_plants_0_random || 
				         currHazard.iType == HazardType.pirhana_plants_1_target ||
				         currHazard.iType == HazardType.pirhana_plants_2_animated ||
				         currHazard.iType == HazardType.pirhana_plants_3_animated)
				{
					translatedPos = TransformHazardPositionToUnityWorld (originalPos);
					PirhanaPlantScript pirhanaPlantScript = hazardGO.AddComponent <PirhanaPlantScript> ();
					// listHazard hat previewSprite
					pirhanaPlantScript.CreateHazard (listHazard, currHazard);

					// 32 ==> 1
					// pirhana sprite height = 48 pixel
					// 48 ==> 48/32

					translatedPos.y += hazardGO.transform.localScale.y * -1* 48f/32f;
					
					if (currHazard.iparam[1] == (short) PirhanaDirection.downwards)
					{
						translatedPos += Vector3.up;
					}
				}
				else if (currHazard.iType == HazardType.rotodisc)
				{
					translatedPos = TransformOrbitHazardPositionToUnityWorld (originalPos);
				}

				hazardGO.transform.position = translatedPos;

//				GameObject clone = GameObject.Instantiate (hazardGO);
//				clone.transform.SetParent (hazardsRoot.transform);
//				clone.transform.position = TransformPixelPositionToUnityWorldPosition (originalPos);

			}
		}
		//if(HazardType.bullet_bill
	}

	public void CreateMapCollider (Map mapSO, GameObject mapRootGO)
	{
		MapTopLayer mapDataTop = mapSO.GetMapDataTop();

		if (mapDataTop != null)
		{
			GameObject colliders = new GameObject ("Colliders");
			colliders.transform.SetParent (mapRootGO.transform);
			
			for(int y=0; y< Globals.MAPHEIGHT; y++)
			{
				for(int x=0; x< Globals.MAPWIDTH; x++)
				{
					MapTile currentElement = mapDataTop.GetTile (x,y);

					if (currentElement.iType == TileType.tile_nonsolid)
						continue;

					int restWidth = Globals.MAPWIDTH - x;
					int restHeight = Globals.MAPHEIGHT - y;

					int currentWidth = 1;
					int currentHeight = 1;

					bool otherTypeFound = false;

					for (int a=1; a< restWidth; a++)
					{
						currentWidth = a;
						if (currentElement.iType != mapDataTop.GetTile (x+a, y).iType)
						{
							otherTypeFound = true;
							break;
						}
						else
						{
							currentWidth++; // TODO FIX
						}
					}

					GameObject currentCollider = new GameObject (currentElement.iType + " " + currentWidth);
					currentCollider.transform.SetParent (colliders.transform);
					TileTypeToUnityTranslation (currentElement.iType, currentCollider, x, y, currentWidth );

					// TODO attention TODO!!
					x += currentWidth-1;

//					for (int a=0; a< restWidth; a++)
//					{
//						for (int b=0; b< restHeight; b++)
//						{
//							if (currentElement.iType != mapDataTop.GetTile (x+a, y+b).iType)
//							{
//								currentWidth = b+1;
//								otherTypeFound = true;
//								break;
//							}
//						}
//
//						if (a==0)
//						{
//
//						}
//						else 
//						{
//							if (otherTypeFound)
//								break;
//						}
//					}

				}
			}
		}
		else
		{
			Debug.LogError ("map top data emtpy, cant create colliders!!!");
		}
	}

	public static Vector2 GetPixelPositionFromUnityWorld (Vector2 originalWorldPosition)
	{
		Vector2 pixelPosition;
		pixelPosition.x = (originalWorldPosition.x + 10f) * 32;
		pixelPosition.y = (-originalWorldPosition.y + 7.5f) * 32;

		// -10, 7.5 ->  0,0
		// -10, 6.5 ->  0,32
		// -10, 0 ->  0,32
		
		return pixelPosition;
	}

	public static Vector3 TransformOrbitHazardPositionToUnityWorld (Vector3 originalHazardPosition)
	{
		return TransformPixelPositionToUnityWorldPosition(TransformOrbitHazardPositionToPixelPosition(originalHazardPosition));
	}

	public static Vector3 TransformHazardPositionToUnityWorld (Vector3 originalHazardPosition)
	{
		return TransformPixelPositionToUnityWorldPosition(TransformHazardPositionToPixelPosition(originalHazardPosition));
	}
	
	public static Vector3 TransformOrbitHazardPositionToPixelPosition (Vector3 originalHazardPosition)
	{
		Vector3 pixelPosition = Vector3.zero;
		pixelPosition.x = (((int) originalHazardPosition.x) << 4) + 16;
		pixelPosition.y = (((int) originalHazardPosition.y) << 4) + 16;
//		Debug.Log ("pixelPosition = " + pixelPosition);
		return pixelPosition;
	}
	
	public static Vector3 TransformHazardPositionToPixelPosition (Vector3 originalHazardPosition)
	{
		Vector3 pixelPosition = Vector3.zero;
		pixelPosition.x = ((int) originalHazardPosition.x) << 4;
		pixelPosition.y = +32 + (((int) originalHazardPosition.y) << 4);
//		Debug.Log ("pixelPosition = " + pixelPosition);
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
	
	[SerializeField]
	Vector3[] clonePositions = { new Vector3(0f,15f,0f), new Vector3(20f,0f,0f), new Vector3(0f,-15f,0f), new Vector3(-20f,0f,0f)};

	public List<GameObject> CreateClones (GameObject original)
	{
		List<GameObject> clones = new List<GameObject> ();
		for (int i=0; i<4; i++)
		{
			GameObject clone = GameObject.Instantiate (original);
			clone.transform.position = original.transform.position + clonePositions[i];
			clones.Add (clone);
		}
		return clones;
	}

	public void ParentingClones (List<GameObject> clones, GameObject parent)
	{
		for(int i=0; i<clones.Count; i++)
		{
			if(clones[i] != null)
				clones[i].transform.SetParent (parent.transform);
		}
	}

	public void WarpsPreview(Map mapSO, GameObject mapRootGO)
	{
		WarpMap warpdata = mapSO.GetWarpMap ();
		if(warpdata != null)
		{
			GameObject goWarps = new GameObject ("Warps");
			goWarps.transform.SetParent (mapRootGO.transform);
			goWarps.transform.localPosition = new Vector3 (0f,0f,-18f);
			
			for (int y=0; y< warpdata.GetHeight (); y++)
			{
				for (int x=0; x< warpdata.GetWidth (); x++)
				{
					Warp currentWarp = warpdata.GetField (x,y);
					if (currentWarp != null)
					{
						if (currentWarp.connection != (short) -1)
						{
							GameObject currentWarpGO = new GameObject ("Warp " + currentWarp.id);
						
							currentWarpGO.transform.SetParent (goWarps.transform);
							
							Vector3 offset = Vector3.zero;
							if (currentWarp.direction == (short)WarpEnterDirection.WARP_DOWN ||
							    currentWarp.direction == (short)WarpEnterDirection.WARP_UP)
							{
//								offset.x = 0.5f;
								offset.y = 0.5f;
							}
							else if (currentWarp.direction == (short)WarpEnterDirection.WARP_LEFT ||
							         currentWarp.direction == (short)WarpEnterDirection.WARP_RIGHT)
							{
								offset.x = .5f;
//								offset.y = -.5f;
							}
								
							float xPos = x - 10.0f;
							float yPos = -1f * y + 6.5f;
							Vector3 cPos = new Vector3 (xPos, yPos, 0f);
							currentWarpGO.transform.localPosition = cPos + offset;
//							currentWarpGO.transform.Rotate (Vector3.forward, 180f);
							
							SpriteRenderer currentSpriteRenderer = currentWarpGO.AddComponent<SpriteRenderer> ();
							currentSpriteRenderer.sprite = g_TilesetManager.GetWarpArrow ().GetEnterArrow(currentWarp.direction, currentWarp.connection);
							currentSpriteRenderer.sortingLayerName = "MapDebug";
							currentSpriteRenderer.color = new Color (0,0,1,.75f);
						}

						
					}
					else
						Debug.LogError (this.ToString () + " warpdata [" + x + ", " + y + "] == NULL");
				}
			}
		}
	}

	public void WarpExitsPreview(Map mapSO, GameObject mapRootGO)
	{
		List<WarpExit> warpexits = mapSO.GetWarpExits ();
		if(warpexits != null)
		{
			GameObject goWarpExits = new GameObject ("WarpExits");
			goWarpExits.transform.SetParent (mapRootGO.transform);
			goWarpExits.transform.localPosition = new Vector3 (0f,0f,-18f);
			
			for (int i=0; i< warpexits.Count; i++)
			{
				if (warpexits[i] != null)
				{
					int xRef = warpexits[i].x;
					int yRef = warpexits[i].y;
					


					for (int j=0; j< warpexits[i].numblocks; j++)
					{
						GameObject currentWarpExit = new GameObject ("WarpExit " + warpexits[i].id + ", " + j);
						currentWarpExit.transform.SetParent (goWarpExits.transform);

						Vector3 offset = Vector3.zero;
						if (warpexits[i].direction == (short)WarpExitDirection.WARP_EXIT_DOWN ||
						    warpexits[i].direction == (short)WarpExitDirection.WARP_EXIT_UP)
						{
							offset.x = j*1f;
						}
						else if (warpexits[i].direction == (short)WarpExitDirection.WARP_EXIT_LEFT ||
						         warpexits[i].direction == (short)WarpExitDirection.WARP_EXIT_RIGHT)
						{
							offset.y = j*-1f;
							
						}
//						switch (warpexits[i].direction)
//						{
//						case (WarpExitDirection.WARP_EXIT_DOWN):
//							{
//							}
//						}

						//					float xPos = (((warpexits[i].x)) / 32f) - 10f;
						//					float yPos = ((480-(warpexits[i].y)) / 32f) - 7.5f;
						//					Vector3 cPos = new Vector3 (xPos, yPos, 0f);
						float xPos = warpexits[i].warpx - 10.0f;
						float yPos = -1f * warpexits[i].warpy + 6.5f;
						Vector3 cPos = new Vector3 (xPos, yPos, 0f);
						currentWarpExit.transform.localPosition = cPos + offset;
						
						SpriteRenderer currentSpriteRenderer = currentWarpExit.AddComponent<SpriteRenderer> ();
						currentSpriteRenderer.sprite = g_TilesetManager.GetWarpArrow ().GetExitArrow(warpexits[i].direction, warpexits[i].connection);
						currentSpriteRenderer.sortingLayerName = "MapDebug";
					}

				}
				else
					Debug.LogError (this.ToString () + " warpexits [" + i + "] == NULL");
			}
		}
	}

	void TileTypeToUnityTranslation(TileType tileType, GameObject tileGO)		// Polymorphy!
	{
		// Polymorphie
		// tileType.AddComponent(tileGO);
		if(tileType == TileType.tile_solid)
		{
			// Block
			BoxCollider2D box = tileGO.AddComponent<BoxCollider2D>();
			tileGO.layer = LayerMask.NameToLayer (Layer.groundLayerName);
			box.isTrigger = false;
		}
		else if(tileType == TileType.tile_solid_on_top)
		{
			// JumpOnPlatform
			tileGO.layer = LayerMask.NameToLayer(Layer.jumpAblePlatformLayerName);
			BoxCollider2D box = tileGO.AddComponent<BoxCollider2D>();
			box.size = new Vector2 (1f,0.1f);
			box.offset = new Vector2 (0.5f,0.95f);
			box.isTrigger = false;
		}
		else if(tileType == TileType.tile_nonsolid)
		{
			// nothing but Sprite
		}
	}

	void TileTypeToUnityTranslation(TileType tileType, GameObject tileGO, int x, int y, int width)		// Polymorphy!
	{
		Vector3 position = new Vector3 (x,y,0f);
		position = TransformPositionToUnityPosition (position);
		position.x += width * 0.5f;

		Vector3 colliderPositionOffset = new Vector3 (0f,-0.5f,0f);
		tileGO.transform.position = position + colliderPositionOffset;

		Vector3 scale = Vector3.one;
		scale.x = width;
		tileGO.transform.localScale = scale;

		// Polymorphie
		// tileType.AddComponent(tileGO);
		if(tileType == TileType.tile_solid)
		{
			// Block
			BoxCollider2D box = tileGO.AddComponent<BoxCollider2D>();
			tileGO.layer = LayerMask.NameToLayer (Layer.groundLayerName);
			box.isTrigger = false;
		}
		else if(tileType == TileType.tile_ice)
		{
			// Block
			BoxCollider2D box = tileGO.AddComponent<BoxCollider2D>();
			tileGO.layer = LayerMask.NameToLayer (Layer.groundLayerName);
			box.isTrigger = false;

			PhysicsMaterial2D icy = new PhysicsMaterial2D ("Icey");
			icy.friction = 0.5f;
			box.sharedMaterial = icy;
		}
		else if (tileType == TileType.tile_death)
		{
			BoxCollider2D box = tileGO.AddComponent<BoxCollider2D>();
			tileGO.layer = LayerMask.NameToLayer (Layer.deathLayerName);
			box.isTrigger = true;
		}
		else if (tileType == TileType.tile_death_on_bottom ||
		         tileType == TileType.tile_death_on_left ||
		         tileType == TileType.tile_death_on_right ||
		         tileType == TileType.tile_death_on_top)
		{
			BoxCollider2D box = tileGO.AddComponent<BoxCollider2D>();
			tileGO.layer = LayerMask.NameToLayer (Layer.deathLayerName);
			box.isTrigger = true;
		}
		else if (tileType == TileType.tile_player_death)
		{
			
		}
		else if (tileType == TileType.tile_super_death ||
		         tileType == TileType.tile_super_death_bottom ||
		         tileType == TileType.tile_super_death_left ||
		         tileType == TileType.tile_super_death_right ||
		         tileType == TileType.tile_super_death_top)
		{
			BoxCollider2D box = tileGO.AddComponent<BoxCollider2D>();
			tileGO.layer = LayerMask.NameToLayer (Layer.superDeathLayerName);
			box.isTrigger = true;
		}
		else if(tileType == TileType.tile_solid_on_top)
		{
			// JumpOnPlatform
			tileGO.layer = LayerMask.NameToLayer(Layer.jumpAblePlatformLayerName);
			BoxCollider2D box = tileGO.AddComponent<BoxCollider2D>();
			box.size = new Vector2 (1f,0.1f);
			box.offset = new Vector2 (0f,0.45f);
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
				window_Batch_FileInfo = GetFileList(batch_MapsImportPath, "*.map");
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
	
	FileInfo[] GetFileList (string absPath, string fileEnd)
	{
		if (!string.IsNullOrEmpty(absPath))
		{
			DirectoryInfo dir = new DirectoryInfo(absPath);
			FileInfo[] info = dir.GetFiles(fileEnd);
			
			
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
//				Debug.Log("currentMapPath=" + currentMapPath);
				
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
		FileInfo[] info = GetFileList(importPath, "*.map");

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
				if (currentBatchMapGO != null)
					currentBatchMapGO.SetActive(false);
				else
					Debug.LogError ("CreateUnityMap failed: " + f.FullName);

			}
			AssetDatabase.SaveAssets();
		}
	}
}
