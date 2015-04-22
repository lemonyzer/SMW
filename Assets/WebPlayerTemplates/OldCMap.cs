
//#define NUM_AUTO_FILTERS	12
//#define MAPWIDTH			20			//width of the map
//#define MAPHEIGHT			15			//height of the map
//#define TILESETUNKNOWN	-3

#define SDL_LITTLE_ENDIAN
#define SDL_BYTEORDER
//#define SDL_BIG_ENDIAN
#undef SDL_BIG_ENDIAN
//	#define SDL_BYTEORDER = SDL_LITTLE_ENDIAN

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public struct MapTile
{
	public TileType iType;
	public int iFlags;
};

public struct MapBlock
{
	public short iType;
	//	public short iSettings[NUM_BLOCK_SETTINGS];
	public short[] iSettings;
	public bool fHidden;
	
	public MapBlock(short iType)
	{
		this.iType = iType;
		iSettings = new short[Globals.NUM_BLOCK_SETTINGS];
		fHidden = false;
	}
};

public struct TilesetTile
{
	public short iID;
	public short iCol;
	public short iRow;
};

public class MovingPlatform {

	public int iTileWidth;
	public int iTileHeight;

	public MovingPlatform()
	{

	}

}

public enum TileType {
	tile_nonsolid = 0,
	tile_solid = 1,
	tile_solid_on_top = 2,
	tile_ice = 3,
	tile_death = 4,
	tile_death_on_top = 5,
	tile_death_on_bottom = 6,
	tile_death_on_left = 7,
	tile_death_on_right = 8,
	tile_ice_on_top = 9, 
	tile_ice_death_on_bottom = 10, 
	tile_ice_death_on_left = 11, 
	tile_ice_death_on_right = 12, 
	tile_super_death = 13, 
	tile_super_death_top = 14, 
	tile_super_death_bottom = 15, 
	tile_super_death_left = 16, 
	tile_super_death_right = 17, 
	tile_player_death = 18, 
	tile_gap = 19
};

public enum TileTypeFlag {
	tile_flag_nonsolid = 0, 
	tile_flag_solid = 1, 
	tile_flag_solid_on_top = 2, 
	tile_flag_ice = 4, 
	tile_flag_death_on_top = 8,
	tile_flag_death_on_bottom = 16,
	tile_flag_death_on_left = 32,
	tile_flag_death_on_right = 64,
	tile_flag_gap = 128,
	tile_flag_has_death = 8056, 
	tile_flag_super_death_top = 256, 
	tile_flag_super_death_bottom = 512, 
	tile_flag_super_death_left = 1024,
	tile_flag_super_death_right = 2048,
	tile_flag_player_death = 4096,
	tile_flag_super_or_player_death_top = 4352,
	tile_flag_super_or_player_death_bottom = 4608,
	tile_flag_super_or_player_death_left = 5120,
	tile_flag_super_or_player_death_right = 6144, 
	tile_flag_player_or_death_on_bottom = 4112
};

public enum ReadType {
	read_type_full = 0, 
	read_type_preview = 1, 
	read_type_summary = 2
};

public struct TilesetTranslation
{
	public short iID;
//	public char[] szName;	// TODO 128 -> TILESET_TRANSLATION_CSTRING_SIZE
	// szName ersetzt durch string!!!
	public string Name;
};




public class OldCMap {

	//Converts the tile type into the flags that this tile carries (solid + ice + death, etc)
//	short[] g_iTileTypeConversion = new short[Globals.NUMTILETYPES] = {0, 1, 2, 5, 121, 9, 17, 33, 65, 6, 21, 37, 69, 3961, 265, 529, 1057, 2113, 4096};
	short[] g_iTileTypeConversion = new short[] {0, 1, 2, 5, 121, 9, 17, 33, 65, 6, 21, 37, 69, 3961, 265, 529, 1057, 2113, 4096};

	public CMap(TilesetManager tilesetManager)
	{
		this.m_TilesetManager = tilesetManager;
	}

	int[] g_iVersion = new int[] {0, 0, 0, 0};
	TilesetManager m_TilesetManager;

	int iNumPlatforms = 0;
	int iPlatformCount = 0;
	int iHazardCount = 0;
	int iIceCount = 0;

	//	TilesetTile	mapdata[MAPWIDTH][MAPHEIGHT][MAPLAYERS];
	//	MapTile		mapdatatop[MAPWIDTH][MAPHEIGHT];
	//	MapBlock	objectdata[MAPWIDTH][MAPHEIGHT];
	//	IO_Block*   blockdata[MAPWIDTH][MAPHEIGHT];
	//	bool		nospawn[NUMSPAWNAREATYPES][MAPWIDTH][MAPHEIGHT];
	//	bool[] 		fAutoFilter = new bool[NUM_AUTO_FILTERS];

	TilesetTile[,,] mapdata;	// komplett eingelesene Tiles der Map
	MapTile[,]		mapdatatop;		// Oberste Kayer der eingelesenen Map
	MapBlock[,]		objectdata;		// ka.
//	IO_Block[,]   	blockdata;
	bool[,,]		nospawn;
	bool[] fAutoFilter = new bool[Globals.NUM_AUTO_FILTERS];

//	char szBackgroundFile[128];	// BACKGROUND_CSTRING_SIZE
	string szBackgroundFile;

	short[] iSwitches;
	
	MovingPlatform[] platforms;
	List<MovingPlatform> platformsList = new List<MovingPlatform>();

	MovingPlatform[] tempPlatforms = new MovingPlatform[Globals.PLATFORMDRAWLAYERS];
	List<MovingPlatform> tempPlatformsList = new List<MovingPlatform>();

	MovingPlatform[] platformdrawlayer = new MovingPlatform[Globals.PLATFORMDRAWLAYERS];
	List<MovingPlatform> platformdrawlayerList = new List<MovingPlatform>();

	public void SetTiletsetManager(TilesetManager tilesetManager)
	{
		this.m_TilesetManager = tilesetManager;
	}

	public TilesetManager GetTilesetManager()
	{
		return this.m_TilesetManager;
	}

	void initSwitches()
	{
		iSwitches = new short[Globals.NUM_SWITCHES];
		for(short iSwitch = 0; iSwitch < iSwitches.Length; iSwitch++)
			iSwitches[iSwitch] = 0;
	}

	public void loadMap(string filePath, ReadType iReadType)
	{
		FileStream fs = new FileStream(filePath, FileMode.Open);
		BinaryReader binReader = new BinaryReader(fs);

		Debug.Log("FileStream.Length = " + fs.Length);

		// check if datei in pfad != null	//TODO
		if(fs.Length <= 0)
		{
			Debug.LogError("FileStream.Length <= 0");
			binReader.Close();
			fs.Close();
			return;
		}

		//Load version number
		int[] version = new int[Globals.VERSIONLENGTH];
		Debug.Log("version.length = " + version.Length);
		ReadIntChunk(version, (uint)version.Length, binReader);
		string sversion = "";
		for(int i=0; i<version.Length; i++)
		{
			if(i != version.Length -1)
				sversion += version[i] + ", ";  
			else
				sversion += version[i];  
		}
		Debug.Log("Map Version = " + sversion);

		if(VersionIsEqualOrAfter(version, 1, 8, 0, 0))
		{
			//Read summary information here
			Debug.Log("Version is Equal or After: 1, 8, 0, 0");

			try
			{
				loadMapVersionEqualOrAfter1800(binReader, iReadType, version);
			}
			// Catch the EndOfStreamException and write an error message.
			catch (EndOfStreamException e)
			{
				Debug.LogError("Error writing the data.\n" + e.GetType().Name);
			}
			// Catch the EndOfStreamException and write an error message.
			catch (ObjectDisposedException e)
			{
				Debug.LogError("Error writing the data.\n" + e.GetType().Name);
			}
			// Catch the EndOfStreamException and write an error message.
			catch (IOException e)
			{
				Debug.LogError("Error writing the data.\n" + e.GetType().Name);
			}
			catch (Exception ex)
			{
				Debug.LogError("loadMapVersionEqualOrAfter1800 failed\n" + ex);
			}
			finally
			{

			}
		}

		// close stream and file
		binReader.Close();
		fs.Close();
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

	void loadMapVersionEqualOrAfter1800(BinaryReader binReader, ReadType iReadType, int[] version)
	{
		//Read summary information here
		
		int[] iAutoFilterValues = new int[Globals.NUM_AUTO_FILTERS + 1];
		ReadIntChunk(iAutoFilterValues, Globals.NUM_AUTO_FILTERS + 1, binReader);
		
		for(short iFilter = 0; iFilter < Globals.NUM_AUTO_FILTERS; iFilter++)
		{
			Debug.Log("fAutoFilter["+iFilter+"] = " + iAutoFilterValues[iFilter]);
			fAutoFilter[iFilter] = iAutoFilterValues[iFilter] > 0;
		}
		
		if(iReadType == ReadType.read_type_summary)
		{
			Debug.Log("summary only");
//			binReader.Close();
			return;
		}

		//clearPlatforms();

		Debug.Log("loading map ");	//TODO mapname

		if(iReadType == ReadType.read_type_preview)
			Debug.Log("(preview)");

		//Load tileset information
		short iNumTilesets = (short) ReadInt(binReader);

		Debug.Log("iNumTilesets = " + iNumTilesets);
		TilesetTranslation[] translation = new TilesetTranslation[iNumTilesets];

		short iMaxTilesetID = 0; //Figure out how big the translation array needs to be
		for(short iTileset = 0; iTileset < iNumTilesets; iTileset++)
		{
			short iTilesetID = (short) ReadInt(binReader);
			Debug.Log("iTileset = " + iTileset + ", iID = " + iTilesetID + ", iMaxTilesetID = " + iMaxTilesetID); 
			translation[iTileset].iID = iTilesetID;
			
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

			translation[iTileset].Name = ReadString(Globals.TILESET_TRANSLATION_CSTRING_SIZE, binReader);
			Debug.Log("TilesetName in struct object: " + translation[iTileset].Name);
			Debug.Log("iTileset = " + iTileset + ", iID = " + iTilesetID + ", Name = " + translation[iTileset].Name + ", iMaxTilesetID = " + iMaxTilesetID); 
		}

		
		int[] translationid = new int[iMaxTilesetID + 1];
		int[] tilesetwidths = new int[iMaxTilesetID + 1];
		int[] tilesetheights = new int[iMaxTilesetID + 1];

		for(short iTileset = 0; iTileset < iNumTilesets; iTileset++)
		{
			short iID = translation[iTileset].iID;
//			translationid[iID] = g_tilesetmanager.GetIndexFromName(translation[iTileset].szName);
			translationid[iID] = m_TilesetManager.GetIndexFromName(translation[iTileset].Name);
			
			if(translationid[iID] == (int) Globals.TILESETUNKNOWN)	//TODO achtung int cast
			{
				tilesetwidths[iID] = 1;
				tilesetheights[iID] = 1;
			}
			else
			{
				tilesetwidths[iID] = m_TilesetManager.GetTileset(translationid[iID]).width;
				tilesetheights[iID] = m_TilesetManager.GetTileset(translationid[iID]).height;
			}
			Debug.Log("Tileset width = " + tilesetwidths[iID] + ", height = " + tilesetheights[iID]);
		}

		mapdata = new TilesetTile[Globals.MAPWIDTH, Globals.MAPHEIGHT, Globals.MAPLAYERS];	// mapdata, hier werden die eingelesenen Daten gespeichert
		objectdata = new MapBlock[Globals.MAPWIDTH, Globals.MAPHEIGHT];

		//2. load map data
		for(int y = 0; y < Globals.MAPHEIGHT; y++)
		{
			for(int x = 0; x < Globals.MAPWIDTH; x++)
			{
				for(int l = 0; l < Globals.MAPLAYERS; l++)
				{
//					TilesetTile * tile = &mapdata[i][j][k];	// zeigt auf aktuelles Element in mapdata
					TilesetTile tile = mapdata[x, y, l];
					tile.iID = ReadByteAsShort(binReader);
					tile.iCol = ReadByteAsShort(binReader);
					tile.iRow = ReadByteAsShort(binReader);
					
					if(tile.iID >= 0)
					{
						if(tile.iID > iMaxTilesetID)
							tile.iID = 0;
						
						//Make sure the column and row we read in is within the bounds of the tileset
						if(tile.iCol < 0 || tile.iCol >= tilesetwidths[tile.iID])
							tile.iCol = 0;
						
						if(tile.iRow < 0 || tile.iRow >= tilesetheights[tile.iID])
							tile.iRow = 0;
						
						//Convert tileset ids into the current game's tileset's ids
						tile.iID = (short) translationid[tile.iID];
					}
				}
				
				objectdata[x,y].iType = ReadByteAsShort(binReader);
				objectdata[x,y].fHidden = ReadBool(binReader);
//				Debug.LogWarning("objectdata["+x+", "+y+"].fHidden = " + objectdata[x,y].fHidden.ToString()); 
			}
		}

		//Read in background to use
		szBackgroundFile = ReadString(Globals.BACKGROUND_CSTRING_SIZE, binReader);
		Debug.Log("BackgroundFile = " + szBackgroundFile);

		initSwitches();

		//Read on/off switches
		for(short iSwitch = 0; iSwitch < iSwitches.Length; iSwitch++)
		{
			iSwitches[iSwitch] = (short)ReadInt(binReader);
			Debug.Log("readed iSwitches["+iSwitch+"] = " + iSwitches[iSwitch]);
		}

		bool fPreview;
		if(iReadType == ReadType.read_type_preview)
			fPreview = true;
		else
			fPreview = false;

		loadPlatforms(binReader, fPreview, version, translationid, tilesetwidths, tilesetheights, iMaxTilesetID);

	}

	void loadPlatforms(BinaryReader binReader, bool fPreview, int[] version, int[] translationid, int[] tilesetwidths, int[] tilesetheights, short iMaxTilesetID)
	{
		clearPlatforms();

		// Load moving platforms
		iNumPlatforms = (short) ReadInt(binReader);
		Debug.Log("iNumPlatforms = " + iNumPlatforms);
		platforms = new MovingPlatform[iNumPlatforms];

		for(short iPlatform = 0; iPlatform < iNumPlatforms; iPlatform++)
		{
			short iWidth = (short) ReadInt(binReader);
			short iHeight = (short) ReadInt(binReader);
			Debug.Log("iPlatform = " + iPlatform + ", iWidth = " + iWidth);
			Debug.Log("iPlatform = " + iPlatform + ", iHeight = " + iHeight);

//			TilesetTile[,] tiles = new TilesetTile[iWidth, iHeight];
//			MapTile[,] types = new MapTile[iWidth, iHeight];
//
//
//			for(short iCol = 0; iCol < iWidth; iCol++)
//			{
//				tiles[iCol] = new TilesetTile[iHeight];
//				types[iCol] = new MapTile[iHeight];
//				
//				for(short iRow = 0; iRow < iHeight; iRow++)
//				{
//					//TilesetTile * tile = &tiles[iCol][iRow];
//					TilesetTile tile = tiles[iCol][iRow];

			TilesetTile[][] tiles = new TilesetTile[iWidth][];
			MapTile[][] types = new MapTile[iWidth][];

			mapdatatop = new MapTile[iWidth, iHeight];

			for(short iCol = 0; iCol < iWidth; iCol++)
			{
				Debug.Log("Platform iCol = " + iCol);
				tiles[iCol] = new TilesetTile[iHeight];
				types[iCol] = new MapTile[iHeight];

				for(short iRow = 0; iRow < iHeight; iRow++)
				{
					Debug.Log("Platform iRow = " + iRow);
					
					//TilesetTile * tile = &tiles[iCol][iRow];
					TilesetTile tile = tiles[iCol][iRow];

//			TilesetTile[][] tiles = new TilesetTile[iWidth][];
//			MapTile[][] types = new MapTile[iWidth][];

//			for(short iCol = 0; iCol < iWidth; iCol++)
//			{
//				tiles[iCol] = new TilesetTile[iHeight];
//				types[iCol] = new MapTile[iHeight];
//
//				for(short iRow = 0; iRow < iHeight; iRow++)
//				{
//					//TilesetTile * tile = &tiles[iCol][iRow];
//					TilesetTile tile = tiles[iCol][iRow];

					if(VersionIsEqualOrAfter(version, 1, 8, 0, 0))
					{
						Debug.LogWarning("VersionIsEqualOrAfter = 1, 8, 0, 0");
						tile.iID = ReadByteAsShort(binReader);
						tile.iCol = ReadByteAsShort(binReader);
						tile.iRow = ReadByteAsShort(binReader);
						
						if(tile.iID >= 0)
						{
							if(iMaxTilesetID != -1 && tile.iID > iMaxTilesetID)
								tile.iID = 0;
							
							//Make sure the column and row we read in is within the bounds of the tileset
//							if(tile.iCol < 0 || (tilesetwidths && tile.iCol >= tilesetwidths[tile.iID]))
							if(tile.iCol < 0 || (tilesetwidths != null && tile.iCol >= tilesetwidths[tile.iID]))
								tile.iCol = 0;
							
//							if(tile.iRow < 0 || (tilesetheights && tile.iRow >= tilesetheights[tile.iID]))
							if(tile.iRow < 0 || (tilesetheights != null && tile.iRow >= tilesetheights[tile.iID]))
								tile.iRow = 0;
							
							//Convert tileset ids into the current game's tileset's ids
							if(translationid != null)
								tile.iID = (short) translationid[tile.iID];
						}
						
						TileType iType = (TileType)ReadInt(binReader);
						
//						if(iType >= 0 && (iType) < Globals.NUMTILETYPES)
						if(iType >= 0 && ((int)iType) < Globals.NUMTILETYPES)
						{
							types[iCol][iRow].iType = iType;
//							types[iCol][iRow].iFlags = g_iTileTypeConversion[iType];
							types[iCol][iRow].iFlags = g_iTileTypeConversion[(int)iType];
						}
						else
						{
							types[iCol][iRow].iType = (int) TileType.tile_nonsolid;
							types[iCol][iRow].iFlags = (int) TileTypeFlag.tile_flag_nonsolid;
						}
					}
					else
					{
						Debug.LogWarning("VersionIsBefore < 1, 8, 0, 0");
						short iTile = (short) ReadInt(binReader);
						TileType type;
						
						if(iTile == Globals.TILESETSIZE)
						{
							tile.iID = Globals.TILESETNONE;
							tile.iCol = 0;
							tile.iRow = 0;
							
							type = TileType.tile_nonsolid;
						}
						else
						{
							tile.iID = m_TilesetManager.GetClassicTilesetIndex();
							tile.iCol = (short)(iTile % Globals.TILESETWIDTH);
							tile.iRow = (short)(iTile / Globals.TILESETWIDTH);
							
							type = m_TilesetManager.GetClassicTileset().GetTileType(tile.iCol, tile.iRow);
						}
						
						if(type >= 0 && (int)type < Globals.NUMTILETYPES)
						{
							types[iCol][iRow].iType = type;
							types[iCol][iRow].iFlags = g_iTileTypeConversion[(int)type];
						}
						else
						{
							mapdatatop[iCol,iRow].iType = TileType.tile_nonsolid;
							mapdatatop[iCol,iRow].iFlags = (int) TileTypeFlag.tile_flag_nonsolid;
						}
					}
				}
			}

//			short iDrawLayer = 2;
//			if(VersionIsEqualOrAfter(version, 1, 8, 0, 1))
//				iDrawLayer = (short)ReadInt(binReader);
//			
//			//printf("Layer: %d\n", iDrawLayer);
//			
//			short iPathType = 0;
//			
//			if(VersionIsEqualOrAfter(version, 1, 8, 0, 0))
//				iPathType = ReadInt(binReader);
//			
//			//printf("PathType: %d\n", iPathType);
//			
//			MovingPlatformPath * path = NULL;
//			if(iPathType == 0) //segment path
//			{
//				float fStartX = ReadFloat(binReader);
//				float fStartY = ReadFloat(binReader);
//				float fEndX = ReadFloat(binReader);
//				float fEndY = ReadFloat(binReader);
//				float fVelocity = ReadFloat(binReader);
//				
//				path = new StraightPath(fVelocity, fStartX, fStartY, fEndX, fEndY, fPreview);
//				
//				//printf("Read segment path\n");
//				//printf("StartX: %.2f StartY:%.2f EndX:%.2f EndY:%.2f Velocity:%.2f\n", fStartX, fStartY, fEndX, fEndY, fVelocity);
//			}
//			else if(iPathType == 1) //continuous path
//			{
//				float fStartX = ReadFloat(binReader);
//				float fStartY = ReadFloat(binReader);
//				float fAngle = ReadFloat(binReader);
//				float fVelocity = ReadFloat(binReader);
//				
//				path = new StraightPathContinuous(fVelocity, fStartX, fStartY, fAngle, fPreview);
//				
//				//printf("Read continuous path\n");
//				//printf("StartX: %.2f StartY:%.2f Angle:%.2f Velocity:%.2f\n", fStartX, fStartY, fAngle, fVelocity);
//			}
//			else if(iPathType == 2) //elliptical path
//			{
//				float fRadiusX = ReadFloat(binReader);
//				float fRadiusY = ReadFloat(binReader);
//				float fCenterX = ReadFloat(binReader);
//				float fCenterY = ReadFloat(binReader);
//				float fAngle = ReadFloat(binReader);
//				float fVelocity = ReadFloat(binReader);
//				
//				path = new EllipsePath(fVelocity, fAngle, fRadiusX, fRadiusY, fCenterX, fCenterY, fPreview);
//				
//				//printf("Read elliptical path\n");
//				//printf("CenterX: %.2f CenterY:%.2f Angle:%.2f RadiusX: %.2f RadiusY: %.2f Velocity:%.2f\n", fCenterX, fCenterY, fAngle, fRadiusX, fRadiusY, fVelocity);
//			}
//			
//			MovingPlatform * platform = new MovingPlatform(tiles, types, iWidth, iHeight, iDrawLayer, path, fPreview);
//			platforms[iPlatform] = platform;
//			platformdrawlayer[iDrawLayer].push_back(platform);

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

	public void OnGUI()
	{
	}

	void saveMap(string filePath)
	{
		FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
		BinaryWriter binWriter = new BinaryWriter(fs);

		//First write the map compatibility version number 
		//(this will allow the map loader to identify if the map needs conversion)
		WriteInt(g_iVersion[0], binWriter); //Major
		WriteInt(g_iVersion[1], binWriter); //Minor
		WriteInt(g_iVersion[2], binWriter); //Micro
		WriteInt(g_iVersion[3], binWriter); //Build
		
		bool[,] usedtile = new bool[Globals.MAPWIDTH, Globals.MAPHEIGHT];

		for(int iPlatform = 0; iPlatform < iNumPlatforms; iPlatform++)
		{
			for(short iCol = 0; iCol < platforms[iPlatform].iTileWidth; iCol++)
			{
				for(short iRow = 0; iRow < platforms[iPlatform].iTileHeight; iRow++)	
				{
					
				}
			}
		}
				
		iPlatformCount++;
		iHazardCount++;
		iIceCount++;

		binWriter.Close();
		fs.Close();
    }

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

//		fread(&b, sizeof(Uint8), 1, inFile);
		b = binReader.ReadByte();
		
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
		// string länge auslesen
		//		int iLen = ReadInt(inFile);
		int iLen = ReadInt(binReader);
		Debug.Log("iLen = " + iLen + " = cstring länge mit NULL Terminator");

		if(iLen < 0)
		{
			Debug.LogError("string länge < 0!");
			return null;
		}
		else if (iLen > Globals.TILESET_TRANSLATION_CSTRING_SIZE)
		{
			Debug.LogError("string länge > max. länge (" + Globals.TILESET_TRANSLATION_CSTRING_SIZE + ") ");
			return null;
		}
		
		//		char * szReadString = new char[iLen];
		char[] szReadString = new char[iLen];
		
		//		fread(szReadString, sizeof(Uint8), iLen, inFile);
		szReadString = binReader.ReadChars(iLen);
		
		//		szReadString[iLen - 1] = 0;
		szReadString[iLen - 1] = '\0';	// cstrin NULL Terminated 
		
		//Prevent buffer overflow  5253784 5253928
		//		strncpy(szString, szReadString, size - 1);		// -> size = TILESET_TRANSLATION_CSTRING_SIZE
		//		szString[size - 1] = 0;
		//TODO NOTE: szString hat im Struct eine länge von 128, nicht über disen Speicherbereich hinaus schreiben!
		/* copy to sized buffer (overflow safe): */ 
		//strncpy ( str2, str1, sizeof(str2) );
		
		string readString = new string(szReadString);
		
		Debug.Log("readString = " + readString);
		
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
