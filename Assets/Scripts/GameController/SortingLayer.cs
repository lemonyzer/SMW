using UnityEngine;
using System.Collections;

public class SortingLayer : MonoBehaviour {

	//TODO scriptable object???

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
	
	void Awake()
	{


	}
}
