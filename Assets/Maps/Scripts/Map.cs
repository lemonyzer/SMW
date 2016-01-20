
//#define NUM_AUTO_FILTERS	12
//#define MAPWIDTH			20			//width of the map
//#define MAPHEIGHT			15			//height of the map
//#define TILESETUNKNOWN	-3

#undef Debug_ReadString
#define SDL_LITTLE_ENDIAN
#define SDL_BYTEORDER
//#define SDL_BIG_ENDIAN
#undef SDL_BIG_ENDIAN
//	#define SDL_BYTEORDER = SDL_LITTLE_ENDIAN

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

[Serializable]
public enum MovingPathType {
	StraightPath = 0,
	StraightPathContinuous = 1,
	EllipsePath = 2,
	HazardString = 3
};

[Serializable]
public enum HazardType {
	fireball_string = 0,
	rotodisc = 1,
	bullet_bill = 2,
	flame_cannon = 3,
	pirhana_plants_0_random = 4,	//	(green shooter)		Face the green fireball plant in a random direction
	pirhana_plants_1_target = 5,	// 	(red shooter)		face the plant towards the nearest player
	pirhana_plants_2_animated = 6,	// 	(red eater)			Animate if these are animated plants
	pirhana_plants_3_animated = 7,	// 	(green eater) 		Animate if these are animated plants 
	count = 8
};

[Serializable]
public enum WarpEnterDirection {
	WARP_DOWN = 0,
	WARP_LEFT = 1,
	WARP_UP = 2,
	WARP_RIGHT = 3,
	WARP_UNDEFINED = -1
};

[Serializable]
public enum WarpExitDirection {
	WARP_EXIT_UP = 0,
	WARP_EXIT_RIGHT = 1,
	WARP_EXIT_DOWN = 2,
	WARP_EXIT_LEFT = 3,
	WARP_EXIT_UNDEFINED = -1
};

[Serializable]
public enum ImportErrorType {
	version,
	ReadingAutoFilterValues,
	BuildingAutoFilter,
	ReadingMapNumOfTilesets,
	ReadingMapTilesetsInformations,
	BuildTranlations,
	ReadingMapDataAndObjectData,
	ReadingBackgroundFileName,
	ReadingSwitches,
	ReadingPlatforms,
	ReadingMapItems,
	ReadingMapHazards,
	ReadingEyeCandys,
	ReadingMusicCategory,
	ReadingWarpAndNoSpawnData,
	ReadingSwitchBlockStateData,
	ReadingWarpExitData,
	ReadingSpawnAreaData,
	ReadingDrawAreaData,
	ReadingExtendedDataBlocks,
	ReadingRaceGoalsData,
	ReadingFlagBasesData,
	NoError
};

[Serializable]
public class Map : ScriptableObject {

	public static float TranslateEllipsePathVelocity (float velocity)
	{
		return velocity / 0.003f;
	}

	public static float TranslateStraightPathVelocity (float velocity)
	{
		return velocity * 4.0f;
	}

	[SerializeField]
	public bool isImportSuccessful = false;
	[SerializeField]
	public ImportErrorType importError;

	[SerializeField]
	public string mapName = ""; 

	public void OnEnable()
	{
		Debug.LogWarning(this.ToString() + " OnEnable()");
//		if(m_Tileset == null)
//		{
//			Debug.LogWarning("m_Tileset == NULL");
//		}
//		else
//		{
//			Debug.Log("m_Tileset is set");
//		}
	}

	public MovingPlatform[] GetPlatforms()
	{
		return platforms;
	}

	public MapBlockLayer GetObjectData()
	{
		return objectdata;
	}

//	public MapTile[,] GetMapDataTop()
//	{
//		return mapdatatop;
//	}

	public MapTopLayer GetMapDataTop()
	{
		return mapdatatop;
	}

//	public TilesetTile[,,] GetMapData()
//	{
//		return mapdata;
//	}

	public MapLayer[] GetMapData()
	{
		return mapdata;
	}

	public MapLayer[] GetMapDataRaw()
	{
		return mapdataRaw;
	}

	public short GetMaxTilesetID()
	{
		return m_iMaxTilesetID;
	}

	public MapDataFlags[] GetCustomMapData()
	{
		return mapdataCustom;
	}

	public string GetBackgroundFilename()
	{
		return szBackgroundFile;
	}

	public WarpMap GetWarpMap ()
	{
		return warpdata;
	}

	public List<WarpExit> GetWarpExits ()
	{
		return warpexits;
	}

	public MapHazard[] GetHazards ()
	{
		return mapHazards;
	}

	public void CreateSwitchConnections () {
		switchConnections = new List<SwitchConnection> ();
		
		for (int i=0; i< 4; i++)
		{
			switchConnections.Add (new SwitchConnection ());
		}
	}

	public void ConnectSwitchBlocks ()
	{
		CreateSwitchConnections ();
		for (int y=0; y< objectdata.Height; y++)
		{
			for (int x=0; x< objectdata.Width; x++)
			{
				MapBlock mapBlock = objectdata.GetBlock (x,y);
				bool state = false;
				if (mapBlock.iType == (short) 1)
				{
					// PowerUp Block [?]

				}
				else if (mapBlock.iType >= (short) 7 &&
				         mapBlock.iType <= (short) 10)
				{
					// ON-Switch [ON]
					if (iSwitchStates[mapBlock.iType-7] == 0)
					{
						state = false;
//						switchBlocks[mapBlock.iType-7].State = false;
					}
					else if (iSwitchStates[mapBlock.iType-7] == 1)
					{
						state = true;
//						switchBlocks[mapBlock.iType-7].State = true;
					}

					switchConnections[mapBlock.iType-7].AddSwitch (x, y, state);
//					switchConnections[mapBlock.iType-7].State = state;
					
					
				}
				else if (mapBlock.iType >= (short) 22 &&
				         mapBlock.iType <= (short) 25)
				{
					// OFF-Switch [OFF]
					if (iSwitchStates[mapBlock.iType-22] == 0)
					{
						state = false;
					}
					else if (iSwitchStates[mapBlock.iType-22] == 1)
					{
						state = true;
					}

					switchConnections[mapBlock.iType-22].AddSwitch (x, y, state);
//					switchConnections[mapBlock.iType-22].State = state;
					
				}
				else if (mapBlock.iType >= (short) 11 &&
				         mapBlock.iType <= (short) 14)
				{
					switchConnections[mapBlock.iType-11].AddBlock (new SwitchTargetBlock (x, y, mapBlock));
					// ON/OFF Block [!]
//					if (mapBlock.GetSetting (0) == (short) 0)
//					{
//						// [ ] 
//					}
//					else if (mapBlock.GetSetting (0) == (short) 1)
//					{
//						// [!]
//					}
				}
			}
		}	
	}


	//Converts the tile type into the flags that this tile carries (solid + ice + death, etc)
//	short[] g_iTileTypeConversion = new short[Globals.NUMTILETYPES] = {0, 1, 2, 5, 121, 9, 17, 33, 65, 6, 21, 37, 69, 3961, 265, 529, 1057, 2113, 4096};
//	[SerializeField]
//	short[] g_iTileTypeConversion = new short[] {0, 1, 2, 5, 121, 9, 17, 33, 65, 6, 21, 37, 69, 3961, 265, 529, 1057, 2113, 4096};

//	public Map(TilesetManager tilesetManager)
//	{
//		this.m_TilesetManager = tilesetManager;
//	}

	[SerializeField]
	int[] m_Version = new int[] {0, 0, 0, 0};

	[SerializeField]
	bool[] fAutoFilter = new bool[Globals.NUM_AUTO_FILTERS];

	[SerializeField]
	short iNumOfTilesets;

	[SerializeField]
	short m_iMaxTilesetID;


	[SerializeField]
	public TilesetTranslation[] translations;
	
	[SerializeField]
	public int[] translationid;
	[SerializeField]
	public int[] tilesetwidths;
	[SerializeField]
	public int[] tilesetheights;
	
	[SerializeField]
	short[] iSwitchStates;

	public short[] SwitchStates {
		get {return iSwitchStates;}
	}

//	int[] g_iVersion = new int[] {0, 0, 0, 0};
//	[SerializeField]
//	TilesetManager m_TilesetManager;
//	public List<Tileset> m_Tileset;
	
//	[SerializeField]
//	int iPlatformCount = 0;
//	[SerializeField]
//	int iHazardCount = 0;
//	[SerializeField]
//	int iIceCount = 0;

	//	TilesetTile	mapdata[MAPWIDTH][MAPHEIGHT][MAPLAYERS];
	//	MapTile		mapdatatop[MAPWIDTH][MAPHEIGHT];
	//	MapBlock	objectdata[MAPWIDTH][MAPHEIGHT];
	//	IO_Block*   blockdata[MAPWIDTH][MAPHEIGHT];
	//	bool		nospawn[NUMSPAWNAREATYPES][MAPWIDTH][MAPHEIGHT];
	//	bool[] 		fAutoFilter = new bool[NUM_AUTO_FILTERS];
//	[SerializeField]
//	MapLayer[] mapLayers;
	[SerializeField]
//	bool[,,] mapdataCustom;
	MapDataFlags[] mapdataCustom;
	[SerializeField]
	MapLayer[] mapdataRaw;	// komplett eingelesene Tiles der Map
	[SerializeField]
	MapLayer[] mapdata;	// komplett eingelesene Tiles der Map
//	[SerializeField]
//	MapTile[,] mapdatatop;		// Oberste Layer der eingelesenen Map
	[SerializeField]
	MapTopLayer mapdatatop;		// Oberste Layer der eingelesenen Map
	
	[SerializeField]
//	MapBlock[,] objectdata;		// ka.
	MapBlockLayer objectdata;		// ka.

	[SerializeField]
	string szBackgroundFile;

	[SerializeField]
	int iNumPlatforms = 0;
	[SerializeField]
	MovingPlatform[] platforms;

	[SerializeField]
//	TilesetTile[,] platformTiles;
	MapLayer platformTiles;
	[SerializeField]
//	TilesetTile[,] platformTilesRaw;
	MapLayer platformTilesRaw;
	[SerializeField]
//	MapTile[,] platformTileTypes;
	MapTopLayer platformTileTypes;
	[SerializeField]
	int iNumMapItems = 0;
	[SerializeField]
	MapItem[] mapItems;

	[SerializeField]
	int iNumMapHazards = 0;
	[SerializeField]
	MapHazard[] mapHazards;

	[SerializeField]
	short[] eyecandy; //= new short[Globals.NUMEYECANDY];
	[SerializeField]
	short musicCategoryID = 0;

	[SerializeField]
	WarpMap warpdata;//[MAPWIDTH][MAPHEIGHT];
	[SerializeField]
//	bool[,,] nospawn;
	MapDataFlags[] nospawn;

	[SerializeField]
	int iNumSwitchBlockData;
	[SerializeField]
	List<SwitchConnection> switchConnections;

	public List<SwitchConnection> SwitchConnections {
		get {return switchConnections;}
	}

	[SerializeField]
	int maxConnection;
	[SerializeField]
	int numwarpexits;
	[SerializeField]
	List<WarpExit> warpexits;//[MAXWARPS];

	[SerializeField]
	short[] totalspawnsize;//[NUMSPAWNAREATYPES];
	[SerializeField]
	short[]	numspawnareas;//[NUMSPAWNAREATYPES];
	[SerializeField]
	SpawnAreaMap spawnareas;//[NUMSPAWNAREATYPES][MAXSPAWNAREAS];

	[SerializeField]
	int numdrawareas;

	[SerializeField]
	List<SDL_Rect> drawareas;

	[SerializeField]
	int iNumRaceGoals;
	[SerializeField]
	Vector2[] racegoallocations;

	[SerializeField]
	int iNumFlagBases;
	[SerializeField]
	Vector2[] flagbaselocations;


	
//	IO_Block[,] blockdata;


//	char szBackgroundFile[128];	// BACKGROUND_CSTRING_SIZE


	[SerializeField]
	List<MovingPlatform> platformsList = new List<MovingPlatform>();

	[SerializeField]
	MovingPlatform[] tempPlatforms = new MovingPlatform[Globals.PLATFORMDRAWLAYERS];
	[SerializeField]
	List<MovingPlatform> tempPlatformsList = new List<MovingPlatform>();

	[SerializeField]
	MovingPlatform[] platformdrawlayer = new MovingPlatform[Globals.PLATFORMDRAWLAYERS];
	[SerializeField]
	List<MovingPlatform> platformdrawlayerList = new List<MovingPlatform>();

//	public void SetTiletsetManager(TilesetManager tilesetManager)
//	{
//		this.m_TilesetManager = tilesetManager;
//		if(m_TilesetManager != null)
//			Debug.Log("<color=green>m_TilesetManager is set</color>");
//		else
//			Debug.LogWarning("m_TilesetManager == NULL");
//	}
//
//	public TilesetManager GetTilesetManager()
//	{
//		return this.m_TilesetManager;
//	}

	void initSwitches()
	{
		iSwitchStates = new short[Globals.NUM_SWITCHES];
		for(short iSwitch = 0; iSwitch < iSwitchStates.Length; iSwitch++)
			iSwitchStates[iSwitch] = 0;
	}

	public bool loadMap(string filePath, ReadType iReadType, TilesetManager f_TilesetManager)
	{
		Debug.Log("<b><color=black> Trying to read File <color=black>" + Path.GetFileName(filePath) + "</color>@" + filePath + "</color></b>" );
		
		isImportSuccessful = false;
		FileStream fs = new FileStream(@filePath, FileMode.Open);
		BinaryReader binReader = new BinaryReader(fs);

		Debug.Log("FileStream.Length = " + fs.Length);

		// check if datei in pfad != null	//TODO
		if(fs.Length <= 0)
		{
			Debug.LogError("FileStream.Length <= 0");
			binReader.Close();
			fs.Close();
			isImportSuccessful = false;
			return isImportSuccessful;
		}

		m_Version = ReadingMapVersion(binReader, iReadType);

		if(VersionIsEqualOrAfter(m_Version, 1, 8, 0, 0))
		{
			//Read summary information here
			Debug.Log("Version is Equal or After: 1, 8, 0, 0");

			try
			{
				loadMapVersionEqualOrAfter1800(binReader, iReadType, m_Version, f_TilesetManager);
				isImportSuccessful = true;
			}
			// Catch the EndOfStreamException and write an error message.
			catch (EndOfStreamException e)
			{
				Debug.LogError("Error reading the data.\n" + e.GetType().Name);
			}
			// Catch the EndOfStreamException and write an error message.
			catch (ObjectDisposedException e)
			{
				Debug.LogError("Error reading the data.\n" + e.GetType().Name);
			}
			// Catch the EndOfStreamException and write an error message.
			catch (IOException e)
			{
				Debug.LogError("Error reading the data.\n" + e.GetType().Name);
			}
			catch (Exception ex)
			{
				Debug.LogError("loadMapVersionEqualOrAfter1800 failed\n" + ex);
			}
			finally
			{
				if(importError != ImportErrorType.NoError)
					Debug.LogError("<color=red>ImportError = " + importError+"</color>");
				else
					Debug.Log("<color=green>ImportError = " + importError+"</color>");
			}
		}
		else
		{
			Debug.LogError("Map Version < 1.8.0.0 ");
		}

		// close stream and file
		binReader.Close();
		fs.Close();
		return isImportSuccessful;
	}

	bool VersionIsEqualOrAfter(int[] iVersion, short iMajor, short iMinor, short iMicro, short iBuild)
	{
		if(iVersion[0] > iMajor)
			return true;
		
		if(iVersion[0] == iMajor)
		{
			if(iVersion[1] > iMinor)
				return true;
			
			if(iVersion[1] == iMinor)
			{
				if(iVersion[2] > iMicro)
					return true;
				
				if(iVersion[2] == iMicro)
				{
					return iVersion[3] >= iBuild;
				}
			}
		}
		
		return false;
	}

	void loadMapVersionEqualOrAfter1800(BinaryReader binReader, ReadType iReadType, int[] version, TilesetManager f_TilesetManager)
	{
		Debug.Log("loading map ");	//TODO mapname
		
		if(iReadType == ReadType.read_type_preview)
			Debug.LogWarning("(preview)");

		//Read summary information here
		importError = ImportErrorType.ReadingAutoFilterValues;
		int[] iAutoFilterValues = ReadingAutoFilterValues(binReader, iReadType);
		BuildingAutoFilter(binReader, iReadType, iAutoFilterValues);
		
		if(iReadType == ReadType.read_type_summary)
		{
			Debug.LogWarning("summary only");
//			binReader.Close();
			return;
		}
		//clearPlatforms();

		importError = ImportErrorType.ReadingMapNumOfTilesets;
		iNumOfTilesets = ReadingMapNumOfTilesets(binReader, iReadType);
		importError = ImportErrorType.ReadingMapTilesetsInformations;
		m_iMaxTilesetID = ReadingMapTilesetsInformations(binReader, iReadType, iNumOfTilesets);
		
		importError = ImportErrorType.BuildTranlations;
		BuildTranlations(iNumOfTilesets, m_iMaxTilesetID, f_TilesetManager);

		mapdatatop = new MapTopLayer(Globals.MAPWIDTH, Globals.MAPHEIGHT);

		importError = ImportErrorType.ReadingMapDataAndObjectData;
		ReadingMapDataAndObjectData(binReader, iReadType, m_iMaxTilesetID);
		importError = ImportErrorType.ReadingBackgroundFileName;
		ReadingBackgroundFileName(binReader, iReadType);

		initSwitches();
		importError = ImportErrorType.ReadingSwitches;
		ReadingSwitches(binReader, iReadType);

		bool fPreview;
		if(iReadType == ReadType.read_type_preview)
			fPreview = true;
		else
			fPreview = false;

		importError = ImportErrorType.ReadingPlatforms;
		loadPlatforms(binReader, fPreview, version, f_TilesetManager, translationid, tilesetwidths, tilesetheights, m_iMaxTilesetID);

		//All tiles have been loaded so the translation is no longer needed
		//		delete [] translationid;
		//		delete [] tilesetwidths;
		//		delete [] tilesetheights;
		Debug.LogWarning("reading more MapData");
		
//		loadingRest(binReader, iReadType);
		importError = ImportErrorType.ReadingMapItems;
		ReadingMapItems(binReader, iReadType);
		importError = ImportErrorType.ReadingMapHazards;
		ReadingMapHazards(binReader, iReadType);
		importError = ImportErrorType.ReadingEyeCandys;
		ReadingEyeCandys(binReader, iReadType);
		importError = ImportErrorType.ReadingMusicCategory;
		ReadingMusicCategory(binReader, iReadType);
		importError = ImportErrorType.ReadingWarpAndNoSpawnData;
		ReadingWarpAndNoSpawnData(binReader, iReadType);
		importError = ImportErrorType.ReadingSwitchBlockStateData;
		ReadingSwitchBlockStateData(binReader, iReadType);
		
		if(iReadType == ReadType.read_type_preview)
		{
			Debug.LogWarning("ReadType.read_type_preview");
			return;
		}

		importError = ImportErrorType.ReadingWarpExitData;
		ReadingWarpExitData(binReader, iReadType);
		importError = ImportErrorType.ReadingSpawnAreaData;
		ReadingSpawnAreaData(binReader, iReadType);
		importError = ImportErrorType.ReadingDrawAreaData;
		ReadingDrawAreaData(binReader, iReadType);
		importError = ImportErrorType.ReadingExtendedDataBlocks;
		ReadingExtendedDataBlocks(binReader, iReadType);
		importError = ImportErrorType.ReadingRaceGoalsData;
		ReadingRaceGoalsData(binReader, iReadType);
		importError = ImportErrorType.ReadingFlagBasesData;
		ReadingFlagBasesData(binReader, iReadType);
		importError = ImportErrorType.NoError;
	}

	int[] ReadingMapVersion(BinaryReader binReader, ReadType iReadType)
	{
		//Load version number
		int[] f_Version = new int[Globals.VERSIONLENGTH];
		Debug.Log("version.length = " + f_Version.Length);
		ReadIntChunk(f_Version, (uint)f_Version.Length, binReader);
		string sversion = "";
		for(int i=0; i<f_Version.Length; i++)
		{
			if(i != f_Version.Length -1)
				sversion += f_Version[i] + ", ";  
			else
				sversion += f_Version[i];  
		}
		Debug.Log("Map Version = " + sversion);
		return f_Version;
	}

	int[] ReadingAutoFilterValues(BinaryReader binReader, ReadType iReadType)
	{
		int[] iAutoFilterValues = new int[Globals.NUM_AUTO_FILTERS + 1];
		ReadIntChunk(iAutoFilterValues, Globals.NUM_AUTO_FILTERS + 1, binReader);
		return iAutoFilterValues;
	}

	void BuildingAutoFilter(BinaryReader binReader, ReadType iReadType, int[] iAutoFilterValues)
	{
		string autoFilterString = "";
		//		string flagAutoFilterString = "";
		for(short iFilter = 0; iFilter < Globals.NUM_AUTO_FILTERS; iFilter++)
		{
			//			Debug.Log("fAutoFilter["+iFilter+"] = " + iAutoFilterValues[iFilter] + " von " + fAutoFilter.Length);
			fAutoFilter[iFilter] = iAutoFilterValues[iFilter] > 0;
			autoFilterString += "["+iFilter+"]="+iAutoFilterValues[iFilter]+" ("+(fAutoFilter[iFilter] ? "1" : "0") + ")\t";
		}
		Debug.Log(autoFilterString);
	}

	short ReadingMapNumOfTilesets(BinaryReader binReader, ReadType iReadType)
	{
		short iNumTilesets = (short) ReadInt(binReader);
		Debug.Log("<color=blue>iNumTilesets = <b>" + iNumTilesets + "</b></color> Anzahl an Tileset Translations");
		return iNumTilesets;
	}

	short ReadingMapTilesetsInformations(BinaryReader binReader, ReadType iReadType, short iNumTilesets)
	{
		//Load tileset information
//		short iNumTilesets = (short) ReadInt(binReader);
		
		Debug.Log("<color=blue>iNumTilesets = <b>" + iNumTilesets + "</b></color> Anzahl an Tileset Translations");
		translations = new TilesetTranslation[iNumTilesets];
		
		short iMaxTilesetID = 0; //Figure out how big the translation array needs to be
		for(short iTileset = 0; iTileset < iNumTilesets; iTileset++)
		{
			Debug.LogWarning("Tileset Translation: " + (iTileset+1) + " von " + iNumTilesets);
			
			short iTilesetID = (short) ReadInt(binReader);
			Debug.Log("\tiTileset = " + iTileset + ", iTilesetID = " + iTilesetID + ", iMaxTilesetID = " + iMaxTilesetID);
			
			translations[iTileset] = new TilesetTranslation();
			translations[iTileset].iTilesetID = iTilesetID;
			
			if(iTilesetID > iMaxTilesetID)
				iMaxTilesetID = iTilesetID;
			
			// Funktioniert, erste Zeichen fehlt jedoch
			// ReadString erwartet einen 7-Bit langen int-Wert der die länge des zu lesenden Strings angibt
			//			string tilesetName = ReadString(TILESET_TRANSLATION_CSTRING_SIZE,binReader);
			//			Debug.Log(tilesetName);
			
			//TODO NOTE: char array in struct kann nicht direkt adressiert werden, kein Ahnung warum. ersetzt durch string.
			//			translation[iTileset].szName = new char[TILESET_TRANSLATION_CSTRING_SIZE];
			//			ReadString(translation[iTileset].szName, TILESET_TRANSLATION_CSTRING_SIZE, binReader);
			//			Debug.Log(new string(translation[iTileset].szName));
			//			Debug.Log("iTileset = " + iTileset + ", iID = " + iTilesetID + ", szName = " + new string(translation[iTileset].szName) + ", iMaxTilesetID = " + iMaxTilesetID); 
			//TODO NOTE: char array in struct kann nicht direkt adressiert werden, kein Ahnung warum. ersetzt durch string.
			
			translations[iTileset].Name = ReadString(Globals.TILESET_TRANSLATION_CSTRING_SIZE, binReader);
			Debug.Log("\tTilesetName länge =" + translations[iTileset].Name.Length);
			Debug.Log("<color=blue>\tTilesetName in class object: <b>" + translations[iTileset].Name + "</b> (string ungeschnitten)</color>");
#if Debug_ReadString
			Debug.Log("\tTilesetName in class object: " + translations[iTileset].Name.Substring(0,translations[iTileset].Name.Length-1) + " TEST -1");
			Debug.Log("\tTilesetName in class object: " + translations[iTileset].Name.Substring(0,translations[iTileset].Name.Length-2) + " TEST -2 ");
			Debug.Log("\tTilesetName in class object: " + translations[iTileset].Name.Substring(0,translations[iTileset].Name.Length-3) + " TEST -3");
#endif
			Debug.Log("\tiTileset = " + iTileset + ", iTilesetID = " + iTilesetID + ", Name = " + translations[iTileset].Name + ", iMaxTilesetID = " + iMaxTilesetID); 
		}
		Debug.Log("<color=red>iMaxTilesetID = <b>" + iMaxTilesetID + "</b></color> wenn in mapdata eine TilesetID > iMaxTilesetID gefunden wird wird sie angepasst. ACHTUNG: TILENONE, TILEANIMATED, TILEUNKNOWN!!!");
		
		return iMaxTilesetID;
	}
	void BuildTranlations(int iNumTilesets, int iMaxTilesetID, TilesetManager f_TilesetManager)
	{
		translationid = new int[iMaxTilesetID + 1];
		tilesetwidths = new int[iMaxTilesetID + 1];
		tilesetheights = new int[iMaxTilesetID + 1];
		string translationsTilesetIDs = "";
		for(short iTileset = 0; iTileset < iNumTilesets; iTileset++)
		{
			short readediTilesetID = translations[iTileset].iTilesetID;
			short currentiTilesetID = translations[iTileset].iTilesetID;
			//			translationid[iID] = g_tilesetmanager.GetIndexFromName(translation[iTileset].szName);
			translationid[currentiTilesetID] = f_TilesetManager.GetIndexFromName(translations[iTileset].Name);
			
			if(translationid[currentiTilesetID] == (int) Globals.TILESETUNKNOWN)	//TODO achtung int cast
			{
				Debug.LogWarning("TILESETUNKNOWN found");
				tilesetwidths[currentiTilesetID] = 1;
				tilesetheights[currentiTilesetID] = 1;
			}
			else
			{
				tilesetwidths[currentiTilesetID] = f_TilesetManager.GetTileset(translationid[currentiTilesetID]).Width;
				tilesetheights[currentiTilesetID] = f_TilesetManager.GetTileset(translationid[currentiTilesetID]).Height;
			}
			translationsTilesetIDs += "translation["+iTileset+"]: readedTilesetID:" + readediTilesetID + "-> lokaleTilesetID: " + translationid[currentiTilesetID] + "\n"; 
			Debug.Log("Tileset " + translations[iTileset].Name + " width = " + tilesetwidths[currentiTilesetID] + ", height = " + tilesetheights[currentiTilesetID]);
		}
		Debug.Log("<color=red><b>!!!!! Tranlations builded:</b></color>\n"+translationsTilesetIDs);
	}
	void ReadingMapDataAndObjectData(BinaryReader binReader, ReadType iReadType, int iMaxTilesetID)
	{
		//2. load map data
		mapdataCustom = new MapDataFlags[Globals.MAPLAYERS]; //new bool[Globals.MAPWIDTH, Globals.MAPHEIGHT, Globals.MAPLAYERS];
		mapdata = new MapLayer[Globals.MAPLAYERS]; // new TilesetTile[Globals.MAPWIDTH, Globals.MAPHEIGHT, Globals.MAPLAYERS];	// mapdata, hier werden die eingelesenen Daten gespeichert
		mapdataRaw = new MapLayer[Globals.MAPLAYERS]; // new TilesetTile[Globals.MAPWIDTH, Globals.MAPHEIGHT, Globals.MAPLAYERS];	// mapdata, hier werden die eingelesenen Daten gespeichert

//		objectdata = new MapBlock[Globals.MAPWIDTH, Globals.MAPHEIGHT];
		objectdata = new MapBlockLayer (Globals.MAPWIDTH, Globals.MAPHEIGHT);
		

		// Persistent
//		mapLayers = new MapLayer[Globals.MAPLAYERS];
		for (int i=0; i< Globals.MAPLAYERS; i++)
		{
			mapdataCustom [i] = new MapDataFlags (Globals.MAPWIDTH, Globals.MAPHEIGHT);
			mapdata [i] = new MapLayer (Globals.MAPWIDTH, Globals.MAPHEIGHT);
			mapdataRaw [i] = new MapLayer (Globals.MAPWIDTH, Globals.MAPHEIGHT);
//			mapLayers [i] = new MapLayer (Globals.MAPWIDTH, Globals.MAPHEIGHT);
		}

		int iColChanges = 0;
		int iRowChanges = 0;
		int iTilesetIDChanges = 0;
		int iTilesetIDOkCount = 0;
		int iTilesetNegativCount = 0;
		Debug.LogWarning("reading and filling mapdata array BEGINN");
		for(int y = 0; y < Globals.MAPHEIGHT; y++)
		{
			for(int x = 0; x < Globals.MAPWIDTH; x++)
			{
				for(int l = 0; l < Globals.MAPLAYERS; l++)
				{
					//					TilesetTile * tile = &mapdata[i][j][k];	// zeigt auf aktuelles Element in mapdata
//					mapdataRaw[x,y,l] = new TilesetTile();
//					mapdata[x, y, l] = new TilesetTile();
					TilesetTile tile = mapdata[l].GetTile (x, y);
					tile.iTilesetID = ReadByteAsShort(binReader);
					tile.iCol = ReadByteAsShort(binReader);
					tile.iRow = ReadByteAsShort(binReader);
					TilesetTile tileRaw = new TilesetTile();
					tileRaw.iTilesetID = tile.iTilesetID;
					tileRaw.iCol = tile.iCol;
					tileRaw.iRow = tile.iRow;
					mapdataRaw[l].SetTile (x, y, tileRaw);

//					mapLayers[l].SetTile (x,y, tileRaw);

					if(tile.iTilesetID >= 0)
					{
						if(tile.iTilesetID > iMaxTilesetID)
						{
							if(tile.iTilesetID == Globals.TILESETNONE)
							{
//								mapdataCustom[x,y,l] = false;
								mapdataCustom[l].SetField(x,y, false);		// wenn tile.iID == 254 dann enthält tile in aktueller layer kein Sprite!
							}
							else
							{
//								mapdataCustom[x,y,l] = true;
								mapdataCustom[l].SetField(x,y, true);		// wenn tile.iID == 254 dann enthält tile in aktueller layer kein Sprite!
							}
							iTilesetIDChanges++;
							//							Debug.LogWarning("tile.iID = " + tile.iID + " > iMaxTilesetID = " + iMaxTilesetID + " => tile.iID = 0");
							tile.iTilesetID = 0; //TODO
						}
						else
						{
//							mapdataCustom[x,y,l] = true;
							mapdataCustom[l].SetField(x,y, true);		// wenn tile.iID == 254 dann enthält tile in aktueller layer kein Sprite!
							iTilesetIDOkCount++;
						}
						
						//Make sure the column and row we read in is within the bounds of the tileset
						if(tile.iCol < 0 || tile.iCol >= tilesetwidths[tile.iTilesetID])
						{
							iColChanges++;
							tile.iCol = 0;
						}
						
						if(tile.iRow < 0 || tile.iRow >= tilesetheights[tile.iTilesetID])
						{
							iRowChanges++;
							tile.iRow = 0;
						}
						
						//Convert tileset ids into the current game's tileset's ids
						tile.iTilesetID = (short) translationid[tile.iTilesetID];
					}
					else
					{
						iTilesetNegativCount++;
					}
				}
//				objectdata[x,y] = new MapBlock();
				objectdata.GetBlock(x,y).iType = ReadByteAsShort(binReader);
				//				objectdata[x,y].iType = (short) ReadInt(binReader);
				objectdata.GetBlock(x,y).fHidden = ReadBool(binReader);
				//				Debug.LogWarning("objectdata["+x+", "+y+"].fHidden = " + objectdata[x,y].fHidden.ToString()); 
			}
		}
		Debug.Log("<color=green> iTilesetIDOkCount : " + iTilesetIDOkCount + "</color>");
		Debug.Log("<color=red>iTilesetIDChanges : " + iTilesetIDChanges + ", iColChanges:" + iColChanges +" iRowChanges: " + iRowChanges+"</color>");
		Debug.Log("<color=red>iTilesetNegativCount : " + iTilesetNegativCount+"</color>");
		
		Debug.LogWarning("reading and filling mapdata array DONE");
	}
	void ReadingBackgroundFileName(BinaryReader binReader, ReadType iReadType)
	{
		//Read in background to use
		szBackgroundFile = ReadString(Globals.BACKGROUND_CSTRING_SIZE, binReader);
		Debug.Log("<color=blue>BackgroundFile = " + szBackgroundFile+"</color>");
	}
	void ReadingSwitches(BinaryReader binReader, ReadType iReadType)
	{
		//Read on/off switches
		string switchesString ="";
		for(short iSwitch = 0; iSwitch < iSwitchStates.Length; iSwitch++)
		{
			iSwitchStates[iSwitch] = (short)ReadInt(binReader);
			switchesString += "Switch["+iSwitch+"]="+iSwitchStates[iSwitch]+"\t";
			//			Debug.Log("readed iSwitchStates["+iSwitch+"] = " + iSwitchStates[iSwitch]);
		}
		Debug.Log(switchesString);
	}
	void ReadingPlatforms(BinaryReader binReader, ReadType iReadType)
	{
	}

	void ReadingMapItems(BinaryReader binReader, ReadType iReadType)
	{
		//Load map items (like carryable spikes and springs)
		Debug.Log("reading MapItems");
		iNumMapItems = (short) ReadInt(binReader);				 // begrenzen
		Debug.Log("<color=blue>iNumMapItems = " + iNumMapItems + "</color>");
		if(iNumMapItems > 0)
		{
			mapItems = new MapItem[iNumMapItems];
			for(int j = 0; j < iNumMapItems; j++)
			{
				mapItems[j] = new MapItem();
				mapItems[j].itype = (short) ReadInt(binReader);
				mapItems[j].ix = (short) ReadInt(binReader);
				mapItems[j].iy = (short) ReadInt(binReader);
			}
		}
	}
	void ReadingMapHazards(BinaryReader binReader, ReadType iReadType)
	{
		//Load map hazards (like fireball strings, rotodiscs, pirhana plants)
		Debug.Log("reading MapHazards");
		iNumMapHazards = (short) ReadInt(binReader);			// begrenzen
		Debug.Log("<color=blue>iNumMapHazards = " + iNumMapHazards + "</color>");
		if(iNumMapHazards >0)
		{
			mapHazards = new MapHazard[iNumMapHazards];
			for(short iMapHazard = 0; iMapHazard < iNumMapHazards; iMapHazard++)
			{
				mapHazards[iMapHazard] = new MapHazard();
				int hazardType = ReadInt(binReader);
				if (hazardType < 0 || hazardType >= (int) HazardType.count)
				{
					Debug.LogError ("unknown hazardtype: " + hazardType);
				}
				mapHazards[iMapHazard].iType = (HazardType) hazardType;
				mapHazards[iMapHazard].ix = (short) ReadInt(binReader);
				mapHazards[iMapHazard].iy = (short) ReadInt(binReader);
				
				for(short iParam = 0; iParam < Globals.NUMMAPHAZARDPARAMS; iParam++)
					mapHazards[iMapHazard].iparam[iParam] = (short) ReadInt(binReader);
				
				for(short iParam = 0; iParam < Globals.NUMMAPHAZARDPARAMS; iParam++)
					mapHazards[iMapHazard].dparam[iParam] = ReadFloat(binReader);
			}
		}
	}
	void ReadingEyeCandys(BinaryReader binReader, ReadType iReadType)
	{
		eyecandy = new short[Globals.NUMEYECANDY];
		
		//For all layers if the map format supports it
		if(VersionIsEqualOrAfter(m_Version, 1, 8, 0, 2))
		{
			eyecandy[0] = (short)ReadInt(binReader);
			eyecandy[1] = (short)ReadInt(binReader);
		}
		
		//Read in eyecandy to use
		eyecandy[2] = (short)ReadInt(binReader);
	}
	void ReadingMusicCategory(BinaryReader binReader, ReadType iReadType)
	{
		musicCategoryID = (short) ReadInt(binReader);
	}
	void ReadingWarpAndNoSpawnData(BinaryReader binReader, ReadType iReadType)
	{
//		if(mapdatatop == null)
//		{
////			mapdatatop = new MapTile[Globals.MAPWIDTH, Globals.MAPHEIGHT];	// wenn keine Platform in der map gefunden wurde ist Array nicht angelegt
//			mapdatatop = new MapTopLayer (Globals.MAPWIDTH, Globals.MAPHEIGHT); // wenn keine Platform in der map gefunden wurde ist Array nicht angelegt
//			Debug.Log ("mapdatatop created");
//		}
//		else
//			Debug.Log ("mapdatatop already exists");
			
		
		warpdata = new WarpMap (Globals.MAPWIDTH, Globals.MAPHEIGHT); //new Warp[Globals.MAPWIDTH, Globals.MAPHEIGHT];
//		nospawn = new bool[Globals.NUMSPAWNAREATYPES, Globals.MAPWIDTH, Globals.MAPHEIGHT];
		nospawn = new MapDataFlags[Globals.NUMSPAWNAREATYPES];

		for(short i = 0; i < Globals.NUMSPAWNAREATYPES; i++)
			nospawn[i] = new MapDataFlags (Globals.MAPWIDTH, Globals.MAPHEIGHT);
		
		for(int j = 0; j < Globals.MAPHEIGHT; j++)
		{
			for(int i = 0; i < Globals.MAPWIDTH; i++)
			{
				TileType iType = (TileType)ReadInt(binReader);
				
//				mapdatatop[i,j] = new MapTile();
//				MapTile tile = mapdatatop[i,j];
//				MapTile tile = new MapTile ();
//				mapdatatop.SetTile (i, j, tile);

				MapTile tile = mapdatatop.GetTile (i, j);
				
				if(iType >= 0 && (int) iType < Globals.NUMTILETYPES)
				{
					//					mapdatatop[i][j].iType = iType;
					//					mapdatatop[i][j].iFlags = g_iTileTypeConversion[iType];
					tile.iType = iType;
					tile.iFlags = Globals.g_iTileTypeConversion[(int)iType];
				}
				else
				{
					//					mapdatatop[i][j].iType = tile_nonsolid;
					//					mapdatatop[i][j].iFlags = tile_flag_nonsolid;
					tile.iType = TileType.tile_nonsolid;
					tile.iFlags = (int)TileTypeFlag.tile_flag_nonsolid;
				}
				
//				warpdata[i,j] = new Warp(); 
				warpdata.GetField(i,j).direction = (short)ReadInt(binReader);
				warpdata.GetField(i,j).connection = (short)ReadInt(binReader);
				warpdata.GetField(i,j).id = (short)ReadInt(binReader);
				
				for(short z = 0; z < Globals.NUMSPAWNAREATYPES; z++)
					nospawn[z].SetField(i,j, ReadBool(binReader));
			}
		}
	}
	void ReadingSwitchBlockStateData(BinaryReader binReader, ReadType iReadType)
	{
//		switchConnections = new List<SwitchConnection> ();
//		switchBlocks.Add ();
		//Read switch block state data
		iNumSwitchBlockData = ReadInt(binReader);
		if (iNumSwitchBlockData > 0)
		{
			for(short iBlock = 0; iBlock < iNumSwitchBlockData; iBlock++)
			{
				short iCol = ReadByteAsShort(binReader);
				short iRow = ReadByteAsShort(binReader);

//				objectdata.GetBlock(iCol,iRow).iSettings[0] = ReadByteAsShort(binReader);
				objectdata.GetBlock(iCol,iRow).AddSetting (ReadByteAsShort(binReader));
//				switchBlocks.Add ();
				Debug.Log("ExtendedDataBlocks ("+ iBlock +") wurde in ObjectData [x=" + iCol + ", y=" + iRow + "] mit Setting: " + objectdata.GetBlock(iCol,iRow).GetSetting(0) + " eingelesen");
			}
		}
	}
	void ReadingWarpExitData(BinaryReader binReader, ReadType iReadType)
	{
		maxConnection = 0;
		
		numwarpexits = (short)ReadInt(binReader);
		Debug.Log("<color=blue>numWarpExits= " + numwarpexits +"</color>");
//		warpexits = new WarpExit[Globals.MAXWARPS];
		warpexits = new List<WarpExit> ();
		for(int i = 0; i < numwarpexits && i < Globals.MAXWARPS; i++)
		{
//			warpexits[i] = new WarpExit();
			warpexits.Add (new WarpExit());
			warpexits[i].direction = (short)ReadInt(binReader);
			warpexits[i].connection = (short)ReadInt(binReader);
			warpexits[i].id = (short)ReadInt(binReader);
			warpexits[i].x = (short)ReadInt(binReader);
			warpexits[i].y = (short)ReadInt(binReader);
			
			warpexits[i].lockx = (short)ReadInt(binReader);
			warpexits[i].locky = (short)ReadInt(binReader);
			
			warpexits[i].warpx = (short)ReadInt(binReader);
			warpexits[i].warpy = (short)ReadInt(binReader);
			warpexits[i].numblocks = (short)ReadInt(binReader);
			
			if(warpexits[i].connection > maxConnection)
				maxConnection = warpexits[i].connection;
		}
		
		//Ignore any more warps than the max
		for(int i = 0; i < numwarpexits - Globals.MAXWARPS; i++)
		{
			for(int j = 0; j < 10; j++)
			{
				ReadInt(binReader);
				Debug.Log("i="+i+", j="+j+"... Ignore any more warps than the max");
			}
		}
		
		if(numwarpexits > Globals.MAXWARPS)
			numwarpexits = Globals.MAXWARPS;
		
		Debug.Log("<color=blue>numwarpexits = " + numwarpexits +"</color> (Ignore any more warps than the max: " + Globals.MAXWARPS +")");
	}
	void ReadingSpawnAreaData(BinaryReader binReader, ReadType iReadType)
	{
		//Read spawn areas
		Debug.Log("Read spawn areas");
		numspawnareas = new short[Globals.NUMSPAWNAREATYPES];
		totalspawnsize = new short[Globals.NUMSPAWNAREATYPES];
//		spawnareas = new SpawnArea[Globals.NUMSPAWNAREATYPES, Globals.MAXSPAWNAREAS];
//		spawnareas = new SpawnAreaMap (Globals.NUMSPAWNAREATYPES, Globals.MAXSPAWNAREAS);
		spawnareas = new SpawnAreaMap (Globals.NUMSPAWNAREATYPES);

		string numSpawnAreasString = "";
		for(int i = 0; i < Globals.NUMSPAWNAREATYPES; i++)
		{
			totalspawnsize[i] = 0;
			numspawnareas[i] = (short)ReadInt(binReader);
			numSpawnAreasString += "[" + i + "] enthält " + numspawnareas[i] + " SpawnAreas\n";
			//			Debug.Log("numspawnareas["+i+"] = " + numspawnareas[i]);
			
			if(numspawnareas[i] > Globals.MAXSPAWNAREAS)
			{
				Debug.LogError(" ERROR: Number of spawn areas (" + numspawnareas[i] + ") was greater than max allowed (" + Globals.MAXSPAWNAREAS + ")");
				//				cout << endl << " ERROR: Number of spawn areas (" << numspawnareas[i]
				//				<< ") was greater than max allowed (" << MAXSPAWNAREAS << ')'
				//					<< endl;
				return;
			}
			
			string spawnAreaString = "";
			for(int m = 0; m < numspawnareas[i]; m++)
			{
//				spawnareas[i,m] = new SpawnArea();
//				spawnareas.SetElement(i,m, new SpawnArea());
				spawnareas.AddElement(i, new SpawnArea());
				spawnareas.GetElement(i,m).left = (short)ReadInt(binReader);
				spawnareas.GetElement(i,m).top = (short)ReadInt(binReader);
				spawnareas.GetElement(i,m).width = (short)ReadInt(binReader);
				spawnareas.GetElement(i,m).height = (short)ReadInt(binReader);
				spawnareas.GetElement(i,m).size = (short)ReadInt(binReader);
				spawnAreaString += 	"["+i+"," + m + "]" + "left=" + spawnareas.GetElement(i,m).left + "\n" +
					"["+i+"," + m + "]" + "top=" + spawnareas.GetElement(i,m).top + "\n" +
						"["+i+"," + m + "]" + "width=" + spawnareas.GetElement(i,m).width + "\n" +
						"["+i+"," + m + "]" + "height=" + spawnareas.GetElement(i,m).height + "\n" +
						"["+i+"," + m + "]" + "size=" + spawnareas.GetElement(i,m).size + "\n";
				totalspawnsize[i] += spawnareas.GetElement(i,m).size;
			}
			Debug.Log(spawnAreaString + "\n");
			
			//If no spawn areas were identified, then create one big spawn area
			if(totalspawnsize[i] == 0)
			{
				Debug.LogError("//If no spawn areas were identified, then create one big spawn area");
				numspawnareas[i] = 1;
//				spawnareas[i,0] = new SpawnArea();
				spawnareas.AddElement(i, new SpawnArea());
				spawnareas.GetElement(i,0).left = 0;
				spawnareas.GetElement(i,0).width = 20;
				spawnareas.GetElement(i,0).top = 1;
				spawnareas.GetElement(i,0).height = 12;
				spawnareas.GetElement(i,0).size = 220;
				totalspawnsize[i] = 220;
			}
		}
		Debug.Log(numSpawnAreasString);
	}
	void ReadingDrawAreaData(BinaryReader binReader, ReadType iReadType)
	{
		Debug.Log("reading DrawAreas");
		
		//Read draw areas (foreground tiles drawing optimization)
		numdrawareas = (short)ReadInt(binReader);
		Debug.Log("<color=blue>numdrawareas = " + numdrawareas +"</color>");
		
		if(numdrawareas > Globals.MAXDRAWAREAS)
		{
			Debug.LogError(" ERROR: Number of spawn areas (" + numdrawareas + ") was greater than max allowed (" + Globals.MAXDRAWAREAS + ")");
			//			cout << endl << " ERROR: Number of draw areas (" << numdrawareas
			//				<< ") was greater than max allowed (" << MAXDRAWAREAS << ')'
			//					<< endl;
			return;
		}
		
		//		Rect[] test = new Rect[23];
		//		test[0].x;
		//		test[0].y;
		//		test[0].width;
		//		test[0].height;
		
		//Load rects to help optimize drawing the foreground
//		drawareas = new SDL_Rect[Globals.MAXDRAWAREAS];
		drawareas = new List<SDL_Rect> ();
		for(int m = 0; m < numdrawareas; m++)
		{
			drawareas.Add (new SDL_Rect());
			drawareas[m].x = (short)ReadInt(binReader);
			drawareas[m].y = (short)ReadInt(binReader);
			drawareas[m].w = (ushort)ReadInt(binReader);
			drawareas[m].h = (ushort)ReadInt(binReader);
			//			drawareas[m].x = (Sint16)ReadInt(binReader);
			//			drawareas[m].y = (Sint16)ReadInt(binReader);
			//			drawareas[m].w = (Uint16)ReadInt(binReader);
			//			drawareas[m].h = (Uint16)ReadInt(binReader);
		}
	}
	void ReadingExtendedDataBlocks(BinaryReader binReader, ReadType iReadType)
	{
		Debug.Log("reading ExtendedDataBlocks");
		
		int iNumExtendedDataBlocks = (short) ReadInt(binReader);
		Debug.Log("<color=blue>iNumExtendedDataBlocks = " + iNumExtendedDataBlocks+"</color>");
		for(short iBlock = 0; iBlock < iNumExtendedDataBlocks; iBlock++)
		{
			short iCol = ReadByteAsShort(binReader);
			short iRow = ReadByteAsShort(binReader);
			
			short iNumSettings = ReadByteAsShort(binReader);
			Debug.Log("ExtendedDataBlocks ("+ iBlock +") wird in ObjectData [x=" + iCol + ", y=" + iRow + "] mit " + iNumSettings + " Settings eingelesen");
			for(short iSetting = 0; iSetting < iNumSettings; iSetting++)
			{
				//				objectdata[iCol,iRow] = new MapBlock();
//				objectdata.GetBlock(iCol,iRow).iSettings[iSetting] = ReadByteAsShort(binReader);
				objectdata.GetBlock(iCol,iRow).AddSetting (ReadByteAsShort(binReader));
			}
		}
	}
	void ReadingRaceGoalsData(BinaryReader binReader, ReadType iReadType)
	{
		Debug.Log("reading RaceGoals");
		
		//read mode item locations like flags and race goals
		iNumRaceGoals = (short)ReadInt(binReader);
		Debug.Log("<color=blue>iNumRaceGoals = " + iNumRaceGoals+"</color>");
		if(iNumRaceGoals >0)
		{
			racegoallocations = new Vector2[Globals.MAXRACEGOALS];
			for(int j = 0; j < iNumRaceGoals; j++)
			{
				racegoallocations[j] = new Vector2();
				racegoallocations[j].x = (short)ReadInt(binReader);
				racegoallocations[j].y = (short)ReadInt(binReader);
			}
		}
	}
	void ReadingFlagBasesData(BinaryReader binReader, ReadType iReadType)
	{
		Debug.Log("reading FlagBases");
		
		iNumFlagBases = (short)ReadInt(binReader);
		Debug.Log("<color=blue>iNumFlagBases = " + iNumFlagBases+"</color>");
		if(iNumFlagBases > 0)
		{
			flagbaselocations = new Vector2[Globals.MAXFLAGBASES];
			for(int j = 0; j < iNumFlagBases; j++)
			{
				flagbaselocations[j] = new Vector2();
				flagbaselocations[j].x = (short)ReadInt(binReader);
				flagbaselocations[j].y = (short)ReadInt(binReader);
			}
		}
	}

	void loadPlatforms(BinaryReader binReader, bool fPreview, int[] version, TilesetManager f_TilesetManager, int[] translationid, int[] tilesetwidths, int[] tilesetheights, short iMaxTilesetID)
	{
		Debug.LogWarning("reading and loading Platforms"); 

		clearPlatforms();

		// Load moving platforms
		iNumPlatforms = (short) ReadInt(binReader);
		Debug.Log("<color=blue>iNumPlatforms = " + iNumPlatforms + "</color>");

		if(iNumPlatforms > 0)
		{
			platforms = new MovingPlatform[iNumPlatforms];
			
			for(short iPlatform = 0; iPlatform < iNumPlatforms; iPlatform++)
			{
				short iPlatformWidth = (short) ReadInt(binReader);
				short iPlatformHeight = (short) ReadInt(binReader);
				Debug.Log("iPlatform = " + iPlatform + ", iWidth = " + iPlatformWidth);
				Debug.Log("iPlatform = " + iPlatform + ", iHeight = " + iPlatformHeight);
				
//				platformTiles = new TilesetTile[iPlatformWidth, iPlatformHeight];				// geht nicht wenn Platform unterschiedliche längen und breiten auf seinen ebenen hat
				platformTiles = new MapLayer (iPlatformWidth, iPlatformHeight);				// geht nicht wenn Platform unterschiedliche längen und breiten auf seinen ebenen hat
//				platformTilesRaw = new TilesetTile[iPlatformWidth, iPlatformHeight];				// geht nicht wenn Platform unterschiedliche längen und breiten auf seinen ebenen hat
				platformTilesRaw = new MapLayer (iPlatformWidth, iPlatformHeight);				// geht nicht wenn Platform unterschiedliche längen und breiten auf seinen ebenen hat
//				platformTileTypes = new MapTile[iPlatformWidth, iPlatformHeight];
				platformTileTypes = new MapTopLayer (iPlatformWidth, iPlatformHeight);

//				if(mapdatatop == null)
//				{
////					mapdatatop = new MapTile[Globals.MAPWIDTH, Globals.MAPHEIGHT];
//					mapdatatop = new MapTopLayer(Globals.MAPWIDTH, Globals.MAPHEIGHT);
//					Debug.Log ("mapdatatop created");
//				}
//				else
//					Debug.Log ("mapdatatop already exists");
					
				
				for(short iCol = 0; iCol < iPlatformWidth; iCol++)
				{
//					Debug.Log("\tPlatform iCol = " + iCol);
					
					for(short iRow = 0; iRow < iPlatformHeight; iRow++)
					{
//						Debug.Log("\tPlatform iRow = " + iRow);
						
						//TilesetTile * tile = &tiles[iCol][iRow];
//						platformTiles[iCol,iRow] = new TilesetTile();
//						platformTilesRaw[iCol,iRow] = new TilesetTile();
						TilesetTile platformTile = platformTiles.GetTile(iCol,iRow);
						TilesetTile platformTileRaw = platformTilesRaw.GetTile(iCol,iRow);

//						platformTileTypes[iCol,iRow] = new MapTile();
						MapTile platformTileType = platformTileTypes.GetTile(iCol,iRow);

//						mapdatatop[iCol,iRow] = new MapTile ();
//						mapdatatop[iCol,iRow] = new MapTile ();
						
						if(VersionIsEqualOrAfter(version, 1, 8, 0, 0))
						{
//							Debug.LogWarning("\tVersionIsEqualOrAfter = 1, 8, 0, 0");
							platformTile.iTilesetID = ReadByteAsShort(binReader);
							platformTile.iCol = ReadByteAsShort(binReader);
							platformTile.iRow = ReadByteAsShort(binReader);

							platformTileRaw.iTilesetID = platformTile.iTilesetID;
							platformTileRaw.iCol = platformTile.iCol;
							platformTileRaw.iRow = platformTile.iRow;
							
							if(platformTile.iTilesetID >= 0)
							{
								if(iMaxTilesetID != -1 && platformTile.iTilesetID > iMaxTilesetID)
									platformTile.iTilesetID = 0;
								
								//Make sure the column and row we read in is within the bounds of the tileset
								//							if(tile.iCol < 0 || (tilesetwidths && tile.iCol >= tilesetwidths[tile.iID]))
								if(platformTile.iCol < 0 || (tilesetwidths != null && platformTile.iCol >= tilesetwidths[platformTile.iTilesetID]))
									platformTile.iCol = 0;
								
								//							if(tile.iRow < 0 || (tilesetheights && tile.iRow >= tilesetheights[tile.iID]))
								if(platformTile.iRow < 0 || (tilesetheights != null && platformTile.iRow >= tilesetheights[platformTile.iTilesetID]))
									platformTile.iRow = 0;
								
								//Convert tileset ids into the current game's tileset's ids
								if(translationid != null)
									platformTile.iTilesetID = (short) translationid[platformTile.iTilesetID];
							}
							
							TileType iType = (TileType)ReadInt(binReader);
							
							//						if(iType >= 0 && (iType) < Globals.NUMTILETYPES)
							if(iType >= 0 && ((int)iType) < Globals.NUMTILETYPES)
							{
								platformTileType.iType = iType;
								//							types[iCol][iRow].iFlags = g_iTileTypeConversion[iType];
								platformTileType.iFlags = Globals.g_iTileTypeConversion[(int)iType];
							}
							else
							{
								platformTileType.iType = (int) TileType.tile_nonsolid;
								platformTileType.iFlags = (int) TileTypeFlag.tile_flag_nonsolid;
							}
						}
						else
						{
//							Debug.LogWarning("\tVersionIsBefore < 1, 8, 0, 0");
							short iTile = (short) ReadInt(binReader);
							TileType type;
							
							if(iTile == Globals.TILESETSIZE)
							{
								platformTile.iTilesetID = Globals.TILESETNONE;
								platformTile.iCol = 0;
								platformTile.iRow = 0;
								
								type = TileType.tile_nonsolid;
							}
							else
							{
								platformTile.iTilesetID = f_TilesetManager.GetClassicTilesetIndex();
								platformTile.iCol = (short)(iTile % Globals.TILESETWIDTH);
								platformTile.iRow = (short)(iTile / Globals.TILESETWIDTH);
								
								type = f_TilesetManager.GetClassicTileset().GetTileType(platformTile.iCol, platformTile.iRow);
							}
							
							if(type >= 0 && (int)type < Globals.NUMTILETYPES)
							{
								platformTileType.iType = type;
								platformTileType.iFlags = Globals.g_iTileTypeConversion[(int)type];
							}
							else
							{
								mapdatatop.GetTile(iCol,iRow).iType = TileType.tile_nonsolid;
								mapdatatop.GetTile(iCol,iRow).iFlags = (int) TileTypeFlag.tile_flag_nonsolid;
							}
						}
					}
				}

				
				short iDrawLayer = 2;
				if(VersionIsEqualOrAfter(version, 1, 8, 0, 1))
					iDrawLayer = (short) ReadInt(binReader);
				
				//printf("Layer: %d\n", iDrawLayer);
				
				short iPathType = 0;
				
				if(VersionIsEqualOrAfter(version, 1, 8, 0, 0))
					iPathType = (short) ReadInt(binReader);
				
				//printf("PathType: %d\n", iPathType);
				
				MovingPlatformPath path = null;
				if(iPathType == 0) //segment path
				{
					float fStartX = ReadFloat(binReader);
					float fStartY = ReadFloat(binReader);
					float fEndX = ReadFloat(binReader);
					float fEndY = ReadFloat(binReader);
					float fVelocity = ReadFloat(binReader);
					
					path = new StraightPath(fVelocity, fStartX, fStartY, fEndX, fEndY, fPreview);
					
					//printf("Read segment path\n");
					//printf("StartX: %.2f StartY:%.2f EndX:%.2f EndY:%.2f Velocity:%.2f\n", fStartX, fStartY, fEndX, fEndY, fVelocity);
				}
				else if(iPathType == 1) //continuous path
				{
					float fStartX = ReadFloat(binReader);
					float fStartY = ReadFloat(binReader);
					float fAngle = ReadFloat(binReader);
					float fVelocity = ReadFloat(binReader);
					
					path = new StraightPathContinuous(fVelocity, fStartX, fStartY, fAngle, fPreview);
					
					//printf("Read continuous path\n");
					//printf("StartX: %.2f StartY:%.2f Angle:%.2f Velocity:%.2f\n", fStartX, fStartY, fAngle, fVelocity);
				}
				else if(iPathType == 2) //elliptical path
				{
					float fRadiusX = ReadFloat(binReader);
					float fRadiusY = ReadFloat(binReader);
					float fCenterX = ReadFloat(binReader);
					float fCenterY = ReadFloat(binReader);
					float fAngle = ReadFloat(binReader);
					float fVelocity = ReadFloat(binReader);
					
					path = new EllipsePath(fVelocity, fAngle, fRadiusX, fRadiusY, fCenterX, fCenterY, fPreview);
					
					//printf("Read elliptical path\n");
					//printf("CenterX: %.2f CenterY:%.2f Angle:%.2f RadiusX: %.2f RadiusY: %.2f Velocity:%.2f\n", fCenterX, fCenterY, fAngle, fRadiusX, fRadiusY, fVelocity);
				}
				
				MovingPlatform platform = new MovingPlatform(platformTiles, platformTilesRaw, platformTileTypes, iPlatformWidth, iPlatformHeight, iDrawLayer, path, fPreview);
				platforms[iPlatform] = platform;
//				platformdrawlayer[iDrawLayer].push_back(platform);

				path.iPathType = iPathType;
			}
		}

	}

	void clearPlatforms()
	{
//		foreach(MovingPlatform mp in platformdrawlayerList)
//		{
//
//		}
		platformdrawlayerList.Clear();

//		for(short iLayer = 0; iLayer < platformdrawlayerList ; iLayer++)
//			platformdrawlayer[iLayer].clear();


		if(platformsList != null)
		{
			platformsList.Clear();
		}

//		if(platforms != null)
//		{
//			for(short iPlatform = 0; iPlatform < iNumPlatforms; iPlatform++)
//			{
//				platforms[iPlatform] = null;
//				//platforms.delete	// list
//			}
//			platforms = NULL;
//		}
		
		iNumPlatforms = 0;
		
//		std::list<MovingPlatform*>::iterator iter = tempPlatforms.begin(), lim = tempPlatforms.end();
//		while (iter != lim)
//		{
//			delete (*iter);
//			++iter;
//		}
//		for(int i=0; i<tempPlatforms.Length; i++)
//		{
//			tempPlatforms[i] = null;
//		}

		tempPlatformsList.Clear();
		
//		tempPlatforms.clear();
	}
		#if UNITY_EDITOR
	public void OnGUI()
	{
	}

	bool fShowRawMapData = false;
	bool fShowMapData = false;
//	bool fShowPlatformData = false;
	bool fShowWarpData = false;
	bool fShowMapTop = false;
//	bool fShowWarpExits = false;
	bool fShowObjectData = false;

	public void OnGUI_Preview()
	{
		fShowMapData = UnityEditor.EditorGUILayout.Foldout(fShowMapData,"Preview Mapdata");
		if(fShowMapData)
		{
			//		previewSliderPosition = EditorGUILayout.BeginScrollView(previewSliderPosition);
			OnGUI_Preview_Mapdata();
			//		EditorGUILayout.EndScrollView();
		}

		fShowRawMapData = UnityEditor.EditorGUILayout.Foldout(fShowRawMapData,"Preview RawMapdata");
		if(fShowRawMapData)
		{
			//		previewSliderPosition = EditorGUILayout.BeginScrollView(previewSliderPosition);
			OnGUI_Preview_Mapdata(mapdataRaw);
			//		EditorGUILayout.EndScrollView();
		}
		
		fShowObjectData = UnityEditor.EditorGUILayout.Foldout(fShowObjectData,"Preview Objectdata: (Type, Settings)");
		if(fShowObjectData)
		{
			//		previewObjectDataSliderPosition = EditorGUILayout.BeginScrollView(previewObjectDataSliderPosition);	
			OnGUI_Preview_Objectdata();
			//		EditorGUILayout.EndScrollView();
		}

//		fShowPlatformData = UnityEditor.EditorGUILayout.Foldout(fShowPlatformData,"Preview Platform Data");
//		if(fShowPlatformData)
//		{
//			//		OnGUI_Preview_PlatformTiles();
//		}

		fShowWarpData = UnityEditor.EditorGUILayout.Foldout(fShowWarpData,"Preview Warp Data: (ID,Connection,Direction)");
		if(fShowWarpData)
		{
			OnGUI_Preview_WarpData ();
		}

//		fShowMapTop = UnityEditor.EditorGUILayout.Foldout(fShowMapTop,"Preview Map Top Data: (Type, Flag)");
//		if(fShowMapTop)
//		{
//			OnGUI_Preview_MapTop ();
//		}


//		fShowWarpExits = UnityEditor.EditorGUILayout.Foldout(fShowWarpExits,"Preview Warp Exits");
//		if(fShowWarpExits)
//		{
//			//		OnGUI_Preview_PlatformTiles();
//		}

	}



	Vector2 previewPlatformTilesSliderPosition = Vector2.zero;
	

	public void OnGUI_Preview_PlatformTiles()
	{
		if(platformTiles != null)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			previewPlatformTilesSliderPosition = EditorGUILayout.BeginScrollView(previewPlatformTilesSliderPosition);
			for(int y = 0; y < Globals.MAPHEIGHT; y++)
			{
				EditorGUILayout.BeginHorizontal();
				for(int x = 0; x < Globals.MAPWIDTH; x++)
				{
					
					string tileString = "";
					
					TilesetTile platformTile = platformTiles.GetTile(x,y);


					if( platformTile == null)
					{
						GUILayout.Label("null");
					}
					else
					{
						//							if(tile.iCol == 0 && tile.iRow == 0)
						//							{
						//								tileString +="<color=red>";
						//							}
						
						if(platformTile.iCol == 0 && platformTile.iRow == 0)
						{
//							GUI.skin.textArea.fixedWidth = 12;
//							GUI.skin.textArea.stretchWidth = false;
							tileString += platformTile.iTilesetID.ToString("D2");
						}
						else
						{
							tileString += platformTile.iTilesetID.ToString("D2")+","+platformTile.iCol.ToString("D2")+","+platformTile.iRow.ToString("D2")+"\n";
						}

					}
					
					EditorGUILayout.TextArea(tileString);
					
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndScrollView();
			EditorGUILayout.Space();
			GUILayout.Space(20);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
			GUILayout.Space(20);
			EditorGUILayout.EndHorizontal();
		}
		else
		{
			EditorGUILayout.LabelField("platformTiles empty");
		}
	}

	Vector2 previewObjectDataSliderPosition = Vector2.zero;
	
	public void OnGUI_Preview_Objectdata()
	{
		if(objectdata != null)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			previewObjectDataSliderPosition = EditorGUILayout.BeginScrollView(previewObjectDataSliderPosition);
			for(int y = 0; y < Globals.MAPHEIGHT; y++)
			{
				EditorGUILayout.BeginHorizontal();
				for(int x = 0; x < Globals.MAPWIDTH; x++)
				{
					
					string mapBlockString = "";
					
					MapBlock mapBlock = objectdata.GetBlock(x,y);
					
					if( mapBlock == null)
					{
						GUILayout.Label("null");
					}
					else
					{
						//						mapBlockString += mapBlock.fHidden.ToString()+","+mapBlock.iSettings.ToString()+","+mapBlock.iType.ToString("D2");
						//						mapBlockString += mapBlock.fHidden.ToString()+","+mapBlock.iType.ToString("D2");
						if (mapBlock.iType != (short) 255)
						{
							mapBlockString += mapBlock.iType.ToString("D3") + "\n";
						}
						else
							mapBlockString += "\n";

						mapBlockString += mapBlock.fHidden ? "1" : "0";

						string settingsString = "";
						for(int i=0; i<mapBlock.SettingsCount (); i++)
						{
							if(mapBlock.GetSetting (i) != 0)
								settingsString += mapBlock.GetSetting (i).ToString("D2") + ",";
						}
						if (mapBlock.iType == (short) 1)
						{
							// PowerUp Block [?]
							if (settingsString.Length > 3)
							{
								mapBlockString += "\n[?]";
	//							mapBlockString += settingsString.Substring(0,2);
							}
						}
						else if (mapBlock.iType >= (short) 7 &&
						         mapBlock.iType <= (short) 10)
						{
							// ON-Switch [ON]
							if (iSwitchStates[mapBlock.iType-7] == 0)
								mapBlockString += "\nOFF" + settingsString;
							else if (iSwitchStates[mapBlock.iType-7] == 1)
								mapBlockString += "\nON" + settingsString;
						}
						else if (mapBlock.iType >= (short) 22 &&
						         mapBlock.iType <= (short) 25)
						{
							// OFF-Switch [OFF]
							mapBlockString += "\nOFF" + settingsString;
						}
						else if (mapBlock.iType >= (short) 11 &&
						         mapBlock.iType <= (short) 14)
						{
							// ON/OFF Block [!]
							if (mapBlock.GetSetting (0) == (short) 0)
							{
								mapBlockString += "\n" + "[]";
							}
							else if (mapBlock.GetSetting (0) == (short) 1)
							{
								mapBlockString += "\n" + "[!]";
							}
						}
						else
							mapBlockString += "\n";
						
					}
					
					EditorGUILayout.TextArea(mapBlockString);
					
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndScrollView();
//			EditorGUILayout.Space();
//			GUILayout.Space(20);
			EditorGUILayout.EndVertical();
//			EditorGUILayout.Space();
//			GUILayout.Space(20);
			EditorGUILayout.EndHorizontal();
		}
		else
		{
			EditorGUILayout.LabelField("objectdata empty");
		}
	}

	Vector2 previewWarpDataSliderPosition = Vector2.zero;
	
	public void OnGUI_Preview_WarpData()
	{
		if(warpdata != null)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			previewWarpDataSliderPosition = EditorGUILayout.BeginScrollView(previewWarpDataSliderPosition);
			for(int y = 0; y < Globals.MAPHEIGHT; y++)
			{
				EditorGUILayout.BeginHorizontal();
				for(int x = 0; x < Globals.MAPWIDTH; x++)
				{
					
					string elementString = "";

					Warp element = warpdata.GetField(x,y);
//					MapBlock mapBlock = objectdata.GetBlock(x,y);
					
					if( element == null)
					{
						GUILayout.Label("null");
					}
					else
					{
						//						mapBlockString += mapBlock.fHidden.ToString()+","+mapBlock.iSettings.ToString()+","+mapBlock.iType.ToString("D2");
						//						mapBlockString += mapBlock.fHidden.ToString()+","+mapBlock.iType.ToString("D2");
						elementString += element.id + ",";
						if (element.connection == (short)-1)
							elementString += ",";
						else
							elementString += element.connection + ",";

						if (element.direction == (short)-1)
						{
//							elementString += "";
						}
						else
							elementString += element.direction;
//						elementString += element.direction;


//						elementString += element.fHidden ? "1" : "0"+"\n" + 
//							element.iType.ToString("D3") + "\n";
//						
//						for(int i=0; i<element.iSettings.Length; i++)
//						{
//							if(element.iSettings[i] != 0)
//								elementString += element.iSettings[i].ToString("D2") + ",";
//						}
						
					}
					
					EditorGUILayout.TextArea(elementString);
					
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndScrollView();
			EditorGUILayout.Space();
			GUILayout.Space(20);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
			GUILayout.Space(20);
			EditorGUILayout.EndHorizontal();
		}
		else
		{
			EditorGUILayout.LabelField("warpdata empty");
		}
	}

	Vector2 previewMapTopDataSliderPosition = Vector2.zero;


	
	public void OnGUI_Preview_MapTop(TilesetManager tileSetManager)
	{
		if(mapdatatop != null)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			previewMapTopDataSliderPosition = EditorGUILayout.BeginScrollView(previewMapTopDataSliderPosition);
			for(int y = 0; y < Globals.MAPHEIGHT; y++)
			{
				EditorGUILayout.BeginHorizontal();
				for(int x = 0; x < Globals.MAPWIDTH; x++)
				{
					
					string elementString = "";

					MapTile element = GetMapDataTop ().GetTile (x,y);


					if( element == null)
					{
						GUILayout.Label("null");
					}
					else
					{

						elementString += tileSetManager.typeConversions[(int)element.iType] + ",";
						elementString += element.iFlags;
					}
					
					EditorGUILayout.TextArea(elementString);
					
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndScrollView();
			EditorGUILayout.Space();
			GUILayout.Space(20);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
			GUILayout.Space(20);
			EditorGUILayout.EndHorizontal();
		}
		else
		{
			EditorGUILayout.LabelField("mapdatatop empty");
		}
	}

	Vector2 previewHazardsSliderPosition = Vector2.zero;

//	Texture2D warpExitsTexture;
//	int textureWidth = 640;
//	int textureHeight = 480;

	public void OnGUI_Preview_Hazards()
	{
		if(mapHazards != null)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			previewHazardsSliderPosition = EditorGUILayout.BeginScrollView(previewHazardsSliderPosition);

			for (int x=0; x< 20; x++)
			{
				for (int y=0; y< 15; y++)
				{
					for (int i=0; i< mapHazards.Length; i++)
					{
						if (mapHazards[i] != null)
						{

						}
						else
							Debug.LogError (this.ToString () + " warpexits [" + i + "] == NULL");
					}
				}
			}

			EditorGUILayout.EndScrollView();
			EditorGUILayout.Space();
			GUILayout.Space(20);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
			GUILayout.Space(20);
			EditorGUILayout.EndHorizontal();
		}
		else
		{
			EditorGUILayout.LabelField("hazards empty");
		}
	}

	Vector2 previewSliderPosition = Vector2.zero;
	bool fHideNonTilesetTiles = true;

	public GUIStyle textAreaStyle;

	public void OnGUI_Preview_Mapdata(MapLayer[] tilesData)
	{
		textAreaStyle = new GUIStyle(GUI.skin.textArea);		// SMART
		textAreaStyle.richText = true;
		textAreaStyle.stretchWidth = true;
		textAreaStyle.fixedWidth = 24+4+16+4+16;
		//		//textAreaStyle.stretchWidth = true;
		
		if(tilesData != null)
		{
			fHideNonTilesetTiles = EditorGUILayout.Toggle("Hide Non TilesetTiles", fHideNonTilesetTiles);
			previewSliderPosition = EditorGUILayout.BeginScrollView(previewSliderPosition);
			EditorGUILayout.BeginHorizontal();
			//			GUILayout.Space(10);
			//			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical();
			for(int y = 0; y < Globals.MAPHEIGHT; y++)
			{
				EditorGUILayout.BeginHorizontal();
				for(int x = 0; x < Globals.MAPWIDTH; x++)
				{
					
					string tileString = "";
					
					for(int l = 0; l < Globals.MAPLAYERS; l++)
					{

						TilesetTile tile = tilesData[l].GetTile(x,y);
						
						if( tile == null)
						{
							GUILayout.Label("null");
						}
						else
						{
							if(!fHideNonTilesetTiles)
							{
								tileString += tile.iTilesetID.ToString("D2")+","+tile.iCol.ToString("D2")+","+tile.iRow.ToString("D2");
							}
							else
							{
								if(tile.iTilesetID == Globals.TILESETNONE)
									tileString += "---,--,--";
								else if(tile.iTilesetID == Globals.TILESETUNKNOWN)
									tileString += "-U"+",00,00";
								else if(tile.iTilesetID == Globals.TILESETANIMATED)
									tileString += "AD"+","+tile.iCol.ToString("D2")+","+tile.iRow.ToString("D2");
								else
									tileString += tile.iTilesetID.ToString("D2")+","+tile.iCol.ToString("D2")+","+tile.iRow.ToString("D2");
									
							}
							
							if(l == Globals.MAPLAYERS -1)
							{
								// no line end
							}
							else
							{
								tileString += "\n";
							}
						}
					}
					EditorGUILayout.TextArea(tileString, textAreaStyle);
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.Space();
			GUILayout.Space(20);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
			GUILayout.Space(20);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndScrollView();
			
		}
		else
		{
			EditorGUILayout.LabelField("<color=red>tilesData empty</color>");
		}
	}

	public void OnGUI_Preview_Mapdata()
	{
		textAreaStyle = new GUIStyle(GUI.skin.textArea);		// SMART
		textAreaStyle.richText = true;
		textAreaStyle.stretchWidth = true;
		textAreaStyle.fixedWidth = 24+4+16+4+16;
//		//textAreaStyle.stretchWidth = true;

		if(mapdata != null && mapdataCustom != null)
		{
			previewSliderPosition = EditorGUILayout.BeginScrollView(previewSliderPosition);
			EditorGUILayout.BeginHorizontal();
//			GUILayout.Space(10);
//			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical();
			for(int y = 0; y < Globals.MAPHEIGHT; y++)
			{
				EditorGUILayout.BeginHorizontal();
				for(int x = 0; x < Globals.MAPWIDTH; x++)
				{

					string tileString = "";

					for(int l = 0; l < Globals.MAPLAYERS; l++)
					{
//						EditorGUILayout.BeginVertical();
//						GUILayout.BeginVertical();

						TilesetTile tile = mapdata[l].GetTile(x,y);

						if( tile == null)
						{
							GUILayout.Label("null");
						}
						else
						{
//							if(tile.iCol == 0 && tile.iRow == 0)
//							{
//								tileString +="<color=red>";
//							}

							if(mapdataCustom[l].GetField(x,y))
							{
								tileString += tile.iTilesetID.ToString("D2")+","+tile.iCol.ToString("D2")+","+tile.iRow.ToString("D2");
							}
							else
							{
								if(fHideNonTilesetTiles)
								{
									tileString += "---,--,--";
								}
								else
								{
									tileString += tile.iTilesetID.ToString("D2")+","+tile.iCol.ToString("D2")+","+tile.iRow.ToString("D2");
								}
//								if(tile.iTilesetID == Globals.TILESETNONE)
//									tileString += "---,--,--";
//								else if(tile.iTilesetID == Globals.TILESETUNKNOWN)
//									tileString += "-U"+",00,00";
//								else if(tile.iTilesetID == Globals.TILESETANIMATED)
//									tileString += "AD"+","+tile.iCol.ToString("D2")+","+tile.iRow.ToString("D2");
//								else
//									tileString += tile.iTilesetID.ToString("D2")+","+tile.iCol.ToString("D2")+","+tile.iRow.ToString("D2");
							}

							//TODO
//							if(tile.iCol == 0 && tile.iRow == 0)
//							{
////								GUI.skin.textArea.fixedWidth = 12;
////								GUI.skin.textArea.stretchWidth = false;
//								tileString += tile.iID.ToString("D2")+",--,--";
//							}
//							else
//							{
//								tileString += tile.iID.ToString("D2")+","+tile.iCol.ToString("D2")+","+tile.iRow.ToString("D2");
//							}
							//TODO


//							if(tile.iCol == 0 && tile.iRow == 0)
//							{
//								tileString +="</color>";
//							}

							if(l == Globals.MAPLAYERS -1)
							{
								// no line end
							}
							else
							{
								tileString += "\n";
							}

//							GUILayout.Label(tile.iID+","+tile.iCol+","+tile.iRow);
//							GUILayout.Label(tile.iCol + "");
//							GUILayout.Label(tile.iRow + "");
						}
//						GUILayout.Label(mapdata[x,y,l].iID.ToString());
//						EditorGUILayout.LabelField(x+" "+y+" "+l, GUILayout.ExpandWidth(false));
//						string mapDataField = "iID = " + mapdata[x,y,l].iID;
//						EditorGUILayout.LabelField(mapDataField);
						//mapdata[x,y,l] = EditorGUILayout.IntField();



//						GUILayout.EndVertical();
//						EditorGUILayout.EndVertical();
					}

//					EditorGUILayout.TextArea(tileString);
					EditorGUILayout.TextArea(tileString, textAreaStyle);

				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.Space();
			GUILayout.Space(20);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
			GUILayout.Space(20);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndScrollView();

		}
		else
		{
			EditorGUILayout.LabelField("<color=red>mapdata && mapdataCustom empty</color>");
		}
	}
		#endif

//	void saveMap(string filePath)
//	{
//		FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
//		BinaryWriter binWriter = new BinaryWriter(fs);
//
//		//First write the map compatibility version number 
//		//(this will allow the map loader to identify if the map needs conversion)
//		WriteInt(Globals.version[0], binWriter); //Major
//		WriteInt(Globals.version[1], binWriter); //Minor
//		WriteInt(Globals.version[2], binWriter); //Micro
//		WriteInt(Globals.version[3], binWriter); //Build
//		
//		bool[,] usedtile = new bool[Globals.MAPWIDTH, Globals.MAPHEIGHT];
//
//		for(int iPlatform = 0; iPlatform < iNumPlatforms; iPlatform++)
//		{
//			for(short iCol = 0; iCol < platforms[iPlatform].iTileWidth; iCol++)
//			{
//				for(short iRow = 0; iRow < platforms[iPlatform].iTileHeight; iRow++)	
//				{
//					
//				}
//			}
//		}
//				
////		iPlatformCount++;
////		iHazardCount++;
////		iIceCount++;
//
//		binWriter.Close();
//		fs.Close();
//    }

	void WriteInt(int value, BinaryWriter binWriter)
    {
		//	fwrite(&out, sizeof(Uint32), 1, outFile);
		binWriter.Write(value);
	}

	bool ReadBool(BinaryReader binReader)
	{
		bool b;
//		fread(&b, sizeof(Uint8), 1, inFile);
		b = binReader.ReadBoolean();
		
		return b;
	}

	short ReadByteAsShort(BinaryReader binReader)
	{
		byte b;
//		char b;

//		fread(&b, sizeof(Uint8), 1, inFile);
		b = binReader.ReadByte();
//		Debug.LogWarning(b.ToString());
		return (short)b;
	}

	/// <summary>
	/// Reads the int.
	/// </summary>
	/// <returns>The int.</returns>
	/// <param name="inFile">In file.</param>
	int ReadInt(BinaryReader binReader)
	{
		int inValue;
//		fread(&inValue, sizeof(Uint32), 1, inFile);
		inValue = (int) binReader.ReadUInt32();
		
		#if (SDL_BYTEORDER == SDL_BIG_ENDIAN)
		// kopiere value zum bearbeiten der byte reihenfolge
		int t = inValue;

		inValue = (int) ReverseBytes((UInt32)t);

//		((char*)&inValue)[0] = ((char*)&t)[3];
//		((char*)&inValue)[1] = ((char*)&t)[2];
//		((char*)&inValue)[2] = ((char*)&t)[1];
//		((char*)&inValue)[3] = ((char*)&t)[0];
		#endif
		
		return inValue;
	}


	/// <summary>
	/// Reads the int chunk. (Datenblock)
	/// </summary>
	#if (SDL_BYTEORDER == SDL_BIG_ENDIAN)
	void ReadIntChunk(int[] mem, uint iQuantity, BinaryReader binReader)
	{
		for(uint i=0; i<iQuantity; i++)
		{
			mem[i] = (int) binReader.ReadUInt32();

			// kopiere value
			int t = mem[i];

			// Reverse Byte Order - reordner the 4 bytes in Integer (32 bit)
			mem[i] = (int) ReverseBytes((uint)t);
		}
	}

	// reverse byte order (32-bit)
	public static UInt32 ReverseBytes(UInt32 value)
	{
		return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
			(value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
	}

	#else
	void ReadIntChunk(int[] mem, uint iQuantity, BinaryReader binReader)
	{
//		fread(mem, sizeof(Uint32), iQuantity, inFile);
		for(uint i=0; i<iQuantity; i++)
		{
			mem[i] = (int) binReader.ReadUInt32();
		}
	}
	#endif

	float ReadFloat(BinaryReader binReader)
	{
														//TODO ready ReadBytes(4), vielleicht konvertiert ReadSingle bereits falsch
		float inValue = binReader.ReadSingle();			// float ReadSingle()
//		fread(&inValue, sizeof(float), 1, inFile);
		
		#if (SDL_BYTEORDER == SDL_BIG_ENDIAN)
		float t = inValue;
		
		inValue = (float) ReverseBytes((UInt32)t);
		#endif
		
		return inValue;
	}


	string ReadString(uint size, BinaryReader binReader)
	{

		/**
		 * Zusammenfassung:
		 * 
		 * In Datei steht vor der eigentlichen Zeichenkette (string) ein UInt32 (4-Byte = 32 Bit) langer Integer Wert
 		 * dieser Integer Wert gibt die länge der nachfolgenden Zeichenkette + NULL Terminierung an.
 		 * 
		 * Adresse:		[00]	[01]	[02]	[03]
		 * Hex-Wert:	[01]	[00]	[00]	[00]	entspricht Interger = 1	(Little-Endian: hohe Adresse (Endian), kleiner Wert)
		 * Hex-Wert:	[08]	[00]	[00]	[00]	entspricht Interger = 8	(Little-Endian: hohe Adresse (Endian), kleiner Wert))
		 * Hex-Wert:	[00]	[00]	[00]	[01]	entspricht Interger = 1	(Big-Endian: hohe Adresse (Endian), hoher Wert))
		 * Hex-Wert:	[00]	[00]	[00]	[08]	entspricht Interger = 8	(Big-Endian: hohe Adresse (Endian), hoher Wert))
		 * 
		 * Little / Big - Endian - Format
		 * * Bei Lade- und Speichervorgängen in Registern und Speichern gibt es zwei Anordnungssysteme in dem die Bytes eingelesen werden, das Endianness: Das Little-Endian-Format und das Big-Endian-Format.
		 * * Im Gegensatz zu Big Endian ist Little Endian ein Format für die Übertragung oder Speicherung binärer Daten, in denen das Least Significant Byte (LSB) an erster Stelle kommt und auf der niedrigsten Speicheradresse gespeichert wird. So wird das Byte E3 von der hexadezimalen Zahl 6FE3 im Little-Endian-Format auf dem Speicherplatz mit der niedrigsten Adresse gespeichert. 
		 * 
		 * 
		 * native Stringlese-Methode in C#
		 * * binaryReader.ReadString();
		 * * Problem: 	diese liest selbst vor der eigentlichen Zeichenkette eine 7-Bit große Längenangabe ein (damit sie weiß wie viele chars zu dem String gehören)
		 * *			da im SMW Maps Dateiformat diese Längenangabe vom Typ UInt32 und somit 32-Bit groß ist kann diese native Funktion nicht verwendet werden.
		 * 
		 * alternative 
		 * * binaryReader.ReadChars(iLength);
		 * 
		 * * Problem:	char[] cString -> string
		 * 				string readString = new string(cString);
		 * 
		 * * Problem:	in letztem Zeichen steht ein cString Terminierungs Zeichen.
		 * 				Debug.Log("vorher " + cstring + " nacher"); nacher wird nicht angezeigt
		 * 				string readString = new string(cString).Trim('\0');		// TODO-> WICHTIG <-TODO entferne NULL Terminierung<
		 * 				
		 * * kein Problem mehr:	die längenangabe stimmt eigentlich nicht iLength = string.length + 1    //+1 => Zeichen der NULL Terminierung
		 * 				
		 * 
		 * alternative SCHLECHT iLength könnte 0 sein -> iLength-1 => -1!
		 * * cString = new char[iLength-1];
		 * * for(int i=0; i<iLength)
		 * *	if(i
		 * * 	cString[i] = binaryReader.ReadChar();
		 * * Problem:	die längenangabe stimmt eigentlich nicht iLength = string.length + 1    //+1 => Zeichen der NULL Terminierung
		 * 
		 **/


		//http://www.bogotobogo.com/Embedded/Little_endian_big_endian_htons_htonl.php
		//http://www.pcreview.co.uk/threads/problem-on-method-readchars.3896989/
		//https://social.msdn.microsoft.com/Forums/windows/en-US/064287cc-eca9-4e7d-9ac3-3ea1440c96fa/binaryreaderreadstring-and-streams?forum=winforms
		// string länge auslesen
		//		int iLen = ReadInt(inFile);
		int iLen = ReadInt(binReader);
//		Debug.Log("iLen = " + iLen + " --> char[] cstring = new char["+iLen+"]; --> arraylänge mit NULL Terminator");

		if(iLen < 0)
		{
			Debug.LogError("string länge < 0!");
			return null;
		}
		else if(iLen == 0)
		{
			Debug.LogError("string länge == 0!");
			return "";
		}
		else if (iLen > Globals.TILESET_TRANSLATION_CSTRING_SIZE)
		{
			Debug.LogError("string länge > max. länge (" + Globals.TILESET_TRANSLATION_CSTRING_SIZE + ") ");
			return null;
		}
		
		//		char * szReadString = new char[iLen];
		char[] szReadCString = new char[iLen];
		
		//		fread(szReadString, sizeof(Uint8), iLen, inFile);
		szReadCString = binReader.ReadChars(iLen);

		//char lastChar = binReader.ReadChar();			// binReader Pointer um ein Char weiter setzen
		//		szReadString[iLen - 1] = 0;
		//szReadCString[iLen - 1] = '\0';	 				//cstring NULL Terminated nicht nötig da wir in string umwandeln

#if Debug_ReadString
		string[] debugString = new string[2];
		for(int i=0; i<iLen; i++)
		{
			debugString[0] += i +" ";
			debugString[1] += szReadCString[i] +" ";
		}
		debugString[0] += "|";
		debugString[1] += "|";
		Debug.Log("<color=red>cstring länge: "+iLen + "</color>\n" + debugString[0] + "\n" + debugString[1]+"</color>");	// </color> wird nicht mehr ausgegeben da debugString NULL Terminierung enthält
#endif
		
		//		szReadString[iLen - 1] = 0;
//		szReadString[iLen - 1] = '\0';	 cstrin NULL Terminated ACHTUNG  BUG -> string wird dann null terminiert!!
		
		//Prevent buffer overflow  5253784 5253928
		//		strncpy(szString, szReadString, size - 1);		// -> size = TILESET_TRANSLATION_CSTRING_SIZE
		//		szString[size - 1] = 0;
		//TODO NOTE: szString hat im Struct eine länge von 128, nicht über disen Speicherbereich hinaus schreiben!
		/* copy to sized buffer (overflow safe): */ 
		//strncpy ( str2, str1, sizeof(str2) );
		
		string readString = new string(szReadCString).Trim('\0');		// TODO-> WICHTIG <-TODO entferne NULL Terminierung<

		#if Debug_ReadString
//		Debug.Log("readString = " + readString);
		#endif
		
		return readString;
	}


//	void ReadString(char * szString, short size, FILE * inFile)
	void ReadString(char[] szString, uint size, BinaryReader binReader)
	{
		Debug.LogError(this.ToString() + " DON'T USE ME");
		
		// string länge auslesen
//		int iLen = ReadInt(inFile);
		int iLen = ReadInt(binReader);
		Debug.Log("iLen = " + iLen + " (cstring länge)");

		if(iLen < 0)
		{
			Debug.LogError("string länge < 0!");
			return;
		}
		else if (iLen > Globals.TILESET_TRANSLATION_CSTRING_SIZE)
		{
			Debug.LogError("string länge > max. länge (" + Globals.TILESET_TRANSLATION_CSTRING_SIZE + ") ");
			return;
		}

//		char * szReadString = new char[iLen];
		char[] szReadString = new char[iLen];

//		fread(szReadString, sizeof(Uint8), iLen, inFile);
		szReadString = binReader.ReadChars(iLen);

//		szReadString[iLen - 1] = 0;
		szReadString[iLen - 1] = '\0';	//TODO check string/char line end in cpp 
		
		//Prevent buffer overflow  5253784 5253928
		//		strncpy(szString, szReadString, size - 1);		// -> size = TILESET_TRANSLATION_CSTRING_SIZE
		//		szString[size - 1] = 0;
		//TODO NOTE: szString hat im Struct eine länge von 128, nicht über disen Speicherbereich hinaus schreiben!
		/* copy to sized buffer (overflow safe): */ 
		//strncpy ( str2, str1, sizeof(str2) );
		szString = szReadString;					//TODO TODO szString zeigt auf die selbe reference
		szString = new char[iLen];					//TODO TODO szString muss eine eigene reference haben, nur der inhalt soll kopiert werden
		Array.Copy(szReadString, szString, iLen);	// char Array kopieren
		string test = new string(szString);			//TODO löscht diese anweisung den Inhalt aus szString?
		string test2 = new string(szString);		//TODO löscht diese anweisung den Inhalt aus szString?
//		string test3 = string.Join("", szString);	//TODO löscht diese anweisung den Inhalt aus szString?
//		string charToString = new string(CharArray, 0, CharArray.Count());
		Debug.Log("szString = " + new string(szString));	//TODO NEIN: Inhalt noch vorhanden
		Debug.Log("szString = " + test);	
		Debug.Log("szString = " + test2);	
//		Debug.Log("szString = " + test3);	
//		delete [] szReadString;
	}

	string ReadNativString(uint size, BinaryReader binReader)
	{
		// Funktioniert mit dieser Dateistruktur NICHT,
		// in der Datei steht ein 32-bit langer Integer-Wert
		// BinaryReader.ReadString() erwartet einen 7-Bit langen Interger-Wert

		string szString;
		// string länge auslesen
		//		int iLen = ReadInt(inFile);
		// TODO BinaryReader.ReadString() erwartet als erste Information die Stringlänge
//		int iLen = ReadInt(binReader);
//		Debug.Log("iLen = " + iLen + " (string länge)");
		

		//		char * szReadString = new char[iLen];
		string szReadString ;
		
		//		fread(szReadString, sizeof(Uint8), iLen, inFile);
		szReadString = binReader.ReadString();	// TODO achtung was macht es?

		szString = szReadString;
		return szString;
	}

}
