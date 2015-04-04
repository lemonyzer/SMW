using UnityEngine;
using System.Collections;

public class SortingLayer : MonoBehaviour {

	// Layer der Renderer
	public static string name_LevelBackground = "LevelBackground";
	public static string name_LevelMidground = "LevelMidground";
	public static string name_LevelForeground = "LevelForeground";
	public static string name_CharacterBackground = "CharacterBackground";
	public static string name_CharacterMidground = "CharacterMidground";
	public static string name_CharacterForeground = "CharacterForeground";
	public static string name_CharacterKing = "CharacterMidground";
	public static string name_PowerUp = "PowerUp";
	public static string name_Clouds = "Clouds";
	public static string name_GUI = "GUI";

	public static int int_Background;
	public static int int_Midground;
	public static int int_Foreground;
	public static int int_Player;
	public static int int_PlayerForeGround;
	public static int int_PowerUp;
	public static int int_Clouds;
	public static int int_GUI;
//	public int player2;
//	public int player3;
//	public int player4;
//
//	public int enemy;
//	public int feet;
//	public int head;
//
//	public int floor;
//	public int jumpAblePlatform;
//	public int jumpAblePlatformSaveZone;
//	
//	public int powerUp;
//
//	public int fader;

//	public const string player1LayerName = "Player1";
//	public const string player2LayerName = "Player2";
//	public const string player3LayerName = "Player3";
//	public const string player4LayerName = "Player4";
//
//	public const string feetLayerName = "Feet";
//	public const string headLayerName = "Head";
//
//	public const string enemyLayerName = "Enemy";
//
//	public const string floorLayerName = "Floor";
//	public const string jumpAblePlatformLayerName = "JumpAblePlatform";
//	public const string jumpAblePlatformSaveZoneLayerName = "JumpAblePlatformSaveZone";
//
//	public const string powerUpLayerName = "PowerUp";
//	public const string faderLayerName = "Fader";

	void Awake()
	{

		int_GUI = 8;
//		player1 = LayerMask.NameToLayer(player1LayerName);
//		player2 = LayerMask.NameToLayer(player2LayerName);
//		player3 = LayerMask.NameToLayer(player3LayerName);
//		player4 = LayerMask.NameToLayer(player4LayerName);
//
//		feet = LayerMask.NameToLayer(feetLayerName);
//		head = LayerMask.NameToLayer(headLayerName);
//
//		enemy = LayerMask.NameToLayer(enemyLayerName);
//
//		floor = LayerMask.NameToLayer(floorLayerName);
//		jumpAblePlatform = LayerMask.NameToLayer(jumpAblePlatformLayerName);
//		jumpAblePlatformSaveZone = LayerMask.NameToLayer(jumpAblePlatformSaveZoneLayerName);
//
//		powerUp = LayerMask.NameToLayer(powerUpLayerName);
//		fader = LayerMask.NameToLayer(faderLayerName);
	}


}
