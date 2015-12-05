using UnityEngine;
using System.Collections;

public static class Globals {

	public static int[] version = new int[VERSIONLENGTH] {1, 8, 0, 3};
	public static short[] g_iTileTypeConversion = new short[] {0, 1, 2, 5, 121, 9, 17, 33, 65, 6, 21, 37, 69, 3961, 265, 529, 1057, 2113, 4096};
	//	int[] g_iVersion = new int[] {0, 0, 0, 0};

	public const int MAPHEIGHT = 15;
	public const int MAPWIDTH = 20;
	public const int MAPLAYERS = 4;
	
	// Custom
	public const int TILESETSIZE = 960;			//30*32 Tiles by 32 pixel in a 1024*1024 bmp
	public const int TILESETHEIGHT = 30;
	public const int TILESETWIDTH = 32;

	public const int TILESETANIMATED = 255;		// byte = 255 = 0 -1
	public const int TILESETNONE = 254;			// byte = 254 = 0 -2
	public const int TILESETUNKNOWN = 253;		// byte = 253 = 0 -3

	public const int TILESET_TRANSLATION_CSTRING_SIZE = 128;
	public const int NUMTILETYPES = 19;
	public const int PLATFORMDRAWLAYERS = 5;
	public const int VERSIONLENGTH = 4;
	public const int NUM_AUTO_FILTERS = 12;
	public const int BACKGROUND_CSTRING_SIZE = 128;
	public const int NUM_SWITCHES = 4;
	// Custom

	public const int NUM_POWERUPS = 26;
	public const int NUM_BLOCK_SETTINGS = NUM_POWERUPS;


	public const int NUMEYECANDY = 3;
	public const int NUMMAPHAZARDPARAMS = 5;

	public const int MAXWARPS=32;
	public const int MAXMAPITEMS=32;
	public const int MAXMAPHAZARDS=30;
	
	public const int NUMSPAWNAREATYPES=6;
	public const int MAXSPAWNAREAS=128;
	public const int MAXDRAWAREAS=128;

	public const int MAXRACEGOALS = 8;
	public const int MAXFLAGBASES = 4;
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
	tile_gap = 19,
	tile_NA = 20
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
