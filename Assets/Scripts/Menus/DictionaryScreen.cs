using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// show the friends list management page

public class DictionaryScreen : MonoBehaviour
{
	GUIStyle buttonStyle;
	float minButtonHeight;
	GUIStyle guiStyle;
	
	GUIStyle masterStyle;
	GUIStyle masterSmallStyle;
	
	GUIStyle clientStyle;
	GUIStyle clientSmallStyle;
	
	GUIStyle myStyle;
	
	GUIStyle avatarStyle;
	
	void InitGUIStyle()
	{
		
		if(Screen.dpi != 0)
		{
			minButtonHeight = 20f * Screen.height / Screen.dpi;
		}
		else
		{
			minButtonHeight = 20f;
		}
		
		myStyle = new GUIStyle ();
		myStyle.fontStyle = FontStyle.Bold;
		
		guiStyle = new GUIStyle ();
		guiStyle.normal.textColor = Color.black;
		guiStyle.fontSize = 16;
		
		masterStyle = new GUIStyle();
		masterStyle.normal.textColor = Color.green;
		masterStyle.fontSize = 16;
		
		masterSmallStyle = new GUIStyle();
		masterSmallStyle.normal.textColor = masterStyle.normal.textColor;
		
		clientStyle = new GUIStyle();
		clientStyle.normal.textColor = Color.red;
		clientStyle.fontSize = 16;
		
		clientSmallStyle = new GUIStyle();
		clientSmallStyle.normal.textColor = masterStyle.normal.textColor;
		
		avatarStyle = new GUIStyle ();
		avatarStyle.fixedWidth = 64f;
		avatarStyle.fixedHeight = 64f;
	}

	PlayerDictionary syncedLocalPersistentPlayerDictionary = PlayerDictionaryManager._instance;



	void Awake()
	{
//		Reset();
		InitGUIStyle ();
	}

	void Start()
	{

	}

	void OnEnable()
	{
//		FriendsListScreen.SetActive( true );		// error, not in JoinedLobbyState!
	}

	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width*0.5f,Screen.height*0.5f,Screen.width*0.5f,Screen.height*0.5f));
		if(syncedLocalPersistentPlayerDictionary == null)
		{
			GUILayout.Label ("no PlayerDictionary created, missing PlayerDictionaryManager Script in Scene!!!",clientStyle);
			GUILayout.EndArea();
			return;
		}

		List<Player> buffer = new List<Player> ( syncedLocalPersistentPlayerDictionary.Values() );
		foreach(Player player in buffer)
		{
			GUILayout.BeginHorizontal();
			//			GUIStyle textStyle = clientStyle;
			//			GUILayout.Label (player.getPhotonPlayer().name +
			//			                 (player.getPhotonPlayer().isMasterClient ? " MasterClient":" Client"), guiStyle);
//			if(player.getPhotonPlayer() == PhotonNetwork.player)
//			{
//				masterStyle.fontStyle = FontStyle.Bold;
//				clientStyle.fontStyle = FontStyle.Bold;
//				clientSmallStyle.fontStyle = FontStyle.Bold;
//				masterSmallStyle.fontStyle = FontStyle.Bold;
//			}
//			else
//			{
//				masterStyle.fontStyle = FontStyle.Normal;
//				clientStyle.fontStyle = FontStyle.Normal;
//				clientSmallStyle.fontStyle = FontStyle.Normal;
//				masterSmallStyle.fontStyle = FontStyle.Normal;
//			}
//			if(player.getPhotonPlayer().isMasterClient)
//			{
//				//				textStyle = masterStyle;
//				GUILayout.Label (player.getPhotonPlayer().name + " MasterClient", masterStyle);
//			}
//			else
//			{
//				GUILayout.Label (player.getPhotonPlayer().name + " Client", clientStyle);
//			}
//			GUILayout.Space(20);
//			if(player.getCharacterSelector() != null)
//				GUILayout.Label( "charSelector: Yes", masterStyle);
//			else
//				GUILayout.Label( "charSelector: NO", clientStyle);
			
			GUILayout.Space(20);
			if(player.getCharacter() != null)
			{
				GUILayout.Label( "Character: " + player.getCharacter().getPrefabFilename(), masterStyle);
				GUILayout.Space(20);
				if(player.getCharacter().getGameObject() != null)
					GUILayout.Label( "GO: Yes", masterStyle);
				else
					GUILayout.Label( "GO: NO", clientStyle);
			}
			else
				GUILayout.Label( "Character: NO", clientStyle);
			
			
			
			GUILayout.EndHorizontal();
		}

		GUILayout.EndArea();
	}
}