using UnityEngine;
using System.Collections;

public class SingleplayerMenu : MonoBehaviour {

	public GUIStyle myStyle;
	public GUISkin mySkin;

	Vector2 mapScrollPosition;
	Texture[] mapArray;
	Vector2 avatarScrollPosition;
	Texture[] avatarArray;

	int currentPlayerIndex = 0;
	Player currentPlayer;
	Character currentCharacter;

	bool enablePowerUps = true;
	bool enableHeadJumps = true;
	bool enableStartAnimation = true;

	void Awake()
	{
		mapArray = Resources.LoadAll<Texture>("Textures/Maps");			// alle Sliced Sprites (Spritename_0) ...
		avatarArray = Resources.LoadAll<Texture>("Textures/Avatars");			// alle Sliced Sprites (Spritename_0) ...

	}

	// Use this for initialization
	void Start () {
		Debug.LogError("Konstruktor has Changed!!!!");
		//currentPlayer = new Player(1,"player1",null);
		this.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Application.LoadLevel(Scenes.mainmenu);
		}
	}

	void OnGUI()
	{
		GUI.skin = mySkin;

		GUILayout.Box("Map Selection");
		mapScrollPosition = GUILayout.BeginScrollView(mapScrollPosition, GUILayout.Width(154));
		foreach(Texture mapTexture in mapArray)
		{
			if(GUILayout.Button(mapTexture, GUILayout.Width(128), GUILayout.Height(96) ))
			{
				Debug.Log(mapTexture.name);
			}
		}
		GUILayout.EndScrollView();

		GUILayout.BeginArea(new Rect(154,0,Screen.width-154,Screen.height));
		GUILayout.BeginVertical();
		GUILayout.Box("Singleplayer");
		GUILayout.BeginHorizontal();
		GUILayout.Button("Player 1", GUILayout.MinHeight(40));
		GUILayout.Button("Player 2", GUILayout.MinHeight(40));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Button("Player 3", GUILayout.MinHeight(40));
		GUILayout.Button("Player 4", GUILayout.MinHeight(40));
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();

		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		GUILayout.Box("Game Settings");
		enablePowerUps = GUILayout.Toggle(enablePowerUps,"Enable PowerUps", "Button", GUILayout.MinHeight(40));
		enableHeadJumps = GUILayout.Toggle(enableHeadJumps,"Enable HeadJumps", "Button", GUILayout.MinHeight(40));
		GUILayout.EndVertical();


		GUILayout.BeginVertical();
		GUILayout.Box("Avatar Selection");
		/**
		 * Avatar Auswahl
		 **/
		avatarScrollPosition = GUILayout.BeginScrollView(avatarScrollPosition);
		for(int i=0; i< avatarArray.Length; i++)
		{
// geht nur bei Sprites
//			if(i%6 == 0)
//			{
//				GUILayout.Button(avatarArray[i]);
//			}
			GUILayout.Button(avatarArray[i]);
		}
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();

		GUILayout.EndArea();
	}
}
