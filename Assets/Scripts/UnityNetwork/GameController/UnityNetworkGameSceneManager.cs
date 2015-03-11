using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnityNetworkGameSceneManager : MonoBehaviour {

	// was ist wenn in diesem Moment PlayerDictionaryManager.syncedLocalPersistentPlayerDictionary == null ist ?!
	// wird die Referenz automatisch erneuert?
	PlayerDictionary playerDictionary = PlayerDictionaryManager.syncedLocalPersistentPlayerDictionary;
	
	// was ist wenn in diesem Moment GameState.currentState == null ist ?!
	// wird die Referenz automatisch erneuert?
	GameState.States currentGameState = GameState.currentState;
	
	// Spawnpoints tagged Spawn
	GameObject[] spawns;
	
	// all Players Ready ? compare with PhotonNetwork.playerList.Length
	int playerReadyCount = 0;
	
	/**
	 * GUI
	 **/
	
	float minButtonHeight;
	GUIStyle guiStyle;
	
	GUIStyle masterStyle;
	GUIStyle masterSmallStyle;
	
	GUIStyle clientStyle;
	GUIStyle clientSmallStyle;
	
	GUIStyle myStyle;
	
	GUIStyle avatarStyle;
	
	public bool debugEnabled = false;
	
	private Rect windowPlayerDictionaryRect = new Rect(20, 25, Screen.width-40, 200);
	private Rect windowPlayerInfoGUIRect = new Rect(10,Screen.height-100f,Screen.width-20,90);
	
	void InitSpawnPoints()
	{
		spawns = GameObject.FindGameObjectsWithTag("Spawn");
		Debug.Log(spawns.Length + " gefundene SpawnPoints");
	}
	
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

	NetworkView myNetworkView;

	void Awake()
	{
		myNetworkView = GetComponent<NetworkView>();
		InitSpawnPoints();
		InitGUIStyle();
	}
	
	// Use this for initialization
	void Start () {																			// ???? 
		Debug.LogWarning(this.ToString() + ": Start");
		if(playerDictionary != null)
		{
			// auf allen Clients ausführen
			
			// PhotonNetwork.isMessageQueueRunning DARF NICHT TRUE SEIN!!!
			if(Network.isMessageQueueRunning)
			{
				Debug.LogError("MessageQueue is running, NOT ALLOWED AT THIS MOMENT!!!");
			}
			else
			{
				Debug.Log("MessageQueue is queuing!!!");
			}
			
			//PROBLEM	// was passiert wenn Client lange Sceneläd und RPC UpdateCurrentPlayerCharacter vor diesem aufruf ausführt?!!!!!
			// ???? dieser client dürfte darf kein MasterClient werden da er keine Referenz zu dem neuen GameObject des players hat!!!
			playerDictionary.RemoveAllPlayerCharacerGameObjects();

			//SOLUTION	// erst jetzt Communication fortsetzen! Buffered RPC's kommen jetzt erst rein
			Network.isMessageQueueRunning = true;
			// wird nur auf MasterClient ausgeführt
			SpawnAllPlayerCharacter();
			myNetworkView.RPC("ClientReady", RPCMode.AllBuffered);
			//photonView.RPC("StartGame", PhotonTargets.AllBufferedViaServer);			// start Game IS NOT SYNC! need to wait for all players ClientReady RPC
		}
		else
		{
			Debug.LogError("BIG ERROR, PlayerDictionary doesn't exist, cant Spawn Players!!!");
			Application.LoadLevel(Scenes.mainmenu);
		}
		
	}
	
	void SpawnAllPlayerCharacter()
	{
		if(Network.isServer)
		{
			int i=0;
			foreach(NetworkPlayer currentNetworkPlayer in Network.connections)
			{
				Debug.Log("SpawnAllPlayerCharacter working On: " + currentNetworkPlayer.ipAddress);
				string currPrefabFilename = null;
				Player currPlayer = null;
				currPlayer = playerDictionary.GetPlayer(currentNetworkPlayer);
				if(currPlayer != null)
				{
					currPrefabFilename = currPlayer.getCharacter().getPrefabFilename();
					if( !string.IsNullOrEmpty(currPrefabFilename) )
					{
						Debug.Log("Spawning: " + currentNetworkPlayer.ipAddress + " Character: " + currPrefabFilename);
						
						if(spawns.Length >0)
						{
							SpawnAuthorativePlayerCharacter(currentNetworkPlayer, currPrefabFilename, spawns[i].transform.position);
							i++;
							if(i >= spawns.Length -1)
							{
								// alle spawm spots voll, wieder von 0 anfangen
								i=0;
							}
						}
						//SpawnAuthorativePlayerCharacter(currentNetworkPlayer, currPrefabFilename, new Vector3(Random.Range(-10,10),5,Random.Range(-10,10)));
						//photonView.RPC("SpawnAuthorativePlayerCharacter", PhotonTargets.AllBuffered, currentNetworkPlayer, currPrefabFilename);
					}
				}
			}
		}
	}
	
	/**
	 * PhotonPlayer clicked unUsed Character, MasterClient instantiats authorativ characterGameObject with Input Controls by realOwner (PhotonPlayer)
	 * after instantiation, tell every Client who is the realOwner (Controller)
	 **/
	void SpawnAuthorativePlayerCharacter(NetworkPlayer realOwner, string characterPrefabName, Vector3 spawnPosition)
	{
		// Called on the MasterClient only! (Authorative Mode)

		GameObject characterPrefab = null;
		//PrefabFinden

		if(!string.IsNullOrEmpty(characterPrefabName))
		{
			characterPrefab = (GameObject) Resources.Load("Assets/Prefabs/PlayerCharacter/UnityNetwork/"+characterPrefabName+".prefab", typeof(GameObject));
		}

		if(characterPrefab != null)
		{
			// Instantiate a new object for this player, remember; the server is therefore the owner.
			GameObject playerCharacterGameObject = (GameObject) Network.Instantiate( characterPrefab, spawnPosition, Quaternion.identity,0 );
			
			// update Dictionary of all clients with
			myNetworkView.RPC("UpdateCurrentPlayerCharacter", RPCMode.AllBuffered, realOwner, characterPrefabName, playerCharacterGameObject.GetComponent<NetworkView>().viewID);
			
			// Get the networkview of this new transform
			NetworkView newObjectsview = playerCharacterGameObject.GetComponent<NetworkView>();
			
			// Keep track of this new player so we can properly destroy it when required.
			RealOwner realOwnerScript = playerCharacterGameObject.GetComponent<RealOwner>();
			realOwnerScript.owner = realOwner;
			
			// Call an RPC on this new PhotonView, set the PhotonPlayer who controls this new player
			newObjectsview.RPC("SetCharacterControlsOwner", RPCMode.AllBuffered, realOwner);
		}
		else
			Debug.LogError("Empty prefabFilename string: " + characterPrefabName);
	}
	
	/**
	 * sync Dictionary if NetworkPlayer selects first/new Character
	 **/
	[RPC]
	void UpdateCurrentPlayerCharacter(NetworkPlayer currentNetworkPlayer, string prefabFilename, NetworkViewID characterGameObjectViewID)
	{
		//		if(PhotonNetwork.isMasterClient)
		//		{
		//			Debug.LogWarning("UpdateCurrentPlayerCharacter already done!"); // ??? maybe bad, should be also doin it in this RPC
		//			return;
		//		}
		Debug.LogWarning("RPC UpdateCurrentPlayerCharacter " + currentNetworkPlayer + ", " + prefabFilename + ", " + characterGameObjectViewID); 
		GameObject characterGameObject = null;
		try {
			characterGameObject = NetworkView.Find (characterGameObjectViewID).gameObject;
		} catch (System.Exception e)
		{
			characterGameObject = null;
			Debug.LogError("NetworkView.Find( " + characterGameObjectViewID + " ) returns NULL!");
			Debug.LogException(e);
		}
		Character character = null;
		if(characterGameObject == null)
		{
			Debug.LogError("Kein GameObject mit passender ViewID gefunden: " + characterGameObjectViewID);
		}
		else
		{
			// found CharacterGameObject with correct ViewID
			if(!string.IsNullOrEmpty(prefabFilename))
				character = new Character(prefabFilename, characterGameObject, false);
			else
				Debug.LogError("prefabFilename is empty -> character = null !!!");
		}
		
		if(characterGameObject == null || character == null)
		{
			return;				// ?????????????????????????????????????????? konsistenz ?!! only add if all infos found!
		}
		
		Player player = playerDictionary.GetPlayer(currentNetworkPlayer);
		if(player == null)
		{
			// player was not in Dictionary
			player = new Player(currentNetworkPlayer, character);
			playerDictionary.AddPlayer(currentNetworkPlayer, player);
		}
		else
		{
			// player found in dictionary
			player.setCharacter(character);
		}
	}
	
	[RPC]
	void ClientReady(NetworkMessageInfo info)
	{
		playerReadyCount++;
		Debug.Log( playerReadyCount + "/" + Network.connections.Length + " NetworkPlayer sind ready!" );
		if(Network.isServer)
		{
			if(playerReadyCount >= Network.connections.Length)
			{
				myNetworkView.RPC("StartGame", RPCMode.All);			// dont need to be buffered (no player can join PhotonROOM after masterclient closes roomscene
				// RPC's need to be reliable. player Dictionary need to be filled before game starts to support OnMasterClientSwitched feature
			}
		}
	}
	
	// alle Spieler sind ready, starte Spiel...
	[RPC]
	void StartGame()
	{
		currentGameState = GameState.States.Starting;
		//        partyCam.StartCam();
		//        photonTaggingStats.StartTagging();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI()
	{
		if(playerDictionary == null)
		{
			return;
		}
		
		if (!Network.isClient && !Network.isServer)
			return;
		
		if (playerDictionary.Values() == null)
			return;
		
		// FullScreen 0,0 = Left,Top
		
		if(debugEnabled)
			windowPlayerDictionaryRect = GUI.Window(0, windowPlayerDictionaryRect, WindowPlayerDictionary, "Player Dictionary");
		windowPlayerInfoGUIRect = GUI.Window(1, windowPlayerInfoGUIRect, WindowPlayerInfoGUI, "Players");
		
	}
	
	void WindowPlayerInfoGUI(int windowID)
	{
		//		List<Player> buffer = new List<Player> ( syncedLocalPersistentPlayerDictionary.Values() );
		/**
		 * Player Info
		 **/
		//		GUILayout.BeginArea (new Rect(0,Screen.height-100f,PhotonNetwork.room.playerCount*200,100));
		GUILayout.BeginHorizontal ();
		// Schleife über Spielerliste
		foreach(Player player in playerDictionary.Values())
		{
			GUILayout.BeginHorizontal (GUILayout.Width(200));
			if(player.getNetworkPlayer() == Network.player)
			{
				if(Network.isServer)
				{

				}
				else
				{

				}
				// Eigener Spieler
				masterStyle.fontStyle = FontStyle.Bold;
				clientStyle.fontStyle = FontStyle.Bold;
				clientSmallStyle.fontStyle = FontStyle.Bold;
				masterSmallStyle.fontStyle = FontStyle.Bold;
			}
			else
			{
				// anderer Spieler
				masterStyle.fontStyle = FontStyle.Normal;
				clientStyle.fontStyle = FontStyle.Normal;
				clientSmallStyle.fontStyle = FontStyle.Normal;
				masterSmallStyle.fontStyle = FontStyle.Normal;
			}
			
			// Character Bild
			if(player.getCharacter() != null)
			{
				if(player.getCharacter().getAvatarTexture() != null)
				{
					GUILayout.Box(player.getCharacter().getAvatarTexture(), avatarStyle);	// kann Image enthalten
				}
			}
			GUILayout.BeginVertical ();
			// Player Name
//			if(player.getNetworkPlayer().isMasterClient)
//			{
//				GUILayout.Label(player.getName(), masterSmallStyle);	// um Text anzuzeigen
//			}
//			else
//			{
				GUILayout.Label(player.getName());	// um Text anzuzeigen
//			}
			// Character Name
			if(player.getCharacter() != null)
			{
//				if(player.getNetworkPlayer().isMasterClient)
//				{
//					GUILayout.Label(player.getCharacter().getName(), masterSmallStyle);	// um Text anzuzeigen
//				}
//				else
//				{
					GUILayout.Label(player.getCharacter().getName());	// um Text anzuzeigen
//				}
			}
			GUILayout.EndVertical ();
			GUILayout.EndHorizontal ();
		}
		GUILayout.EndHorizontal ();
		//		GUILayout.EndArea ();
	}
	
	
	void WindowPlayerDictionary(int windowID)
	{
		//		List<Player> buffer = new List<Player> ( syncedLocalPersistentPlayerDictionary.Values() );
		/**
		 * Connection Info
		 **/
		GUILayout.BeginVertical ();
		GUILayout.Label ("Connected clients: " + Network.connections.Length + " / " + Network.maxConnections, guiStyle);
		GUILayout.Label ("Selected characters: " + playerDictionary.Values().Count + " / " + Network.connections.Length, guiStyle);
		//		if(PhotonNetwork.isMasterClient)
		//			GUILayout.Label ("isMasterClient! "+ PhotonNetwork.player.name, guiStyle);
		//		else
		//			GUILayout.Label ("isClient! "+ PhotonNetwork.player.name, guiStyle);
		
		foreach(Player player in playerDictionary.Values())
		{
			GUILayout.BeginHorizontal();
			//			GUIStyle textStyle = clientStyle;
			//			GUILayout.Label (player.getPhotonPlayer().name +
			//			                 (player.getPhotonPlayer().isMasterClient ? " MasterClient":" Client"), guiStyle);
			if(player.getNetworkPlayer() == Network.player)
			{
				// Eigener Spieler
				masterStyle.fontStyle = FontStyle.Bold;
				clientStyle.fontStyle = FontStyle.Bold;
				clientSmallStyle.fontStyle = FontStyle.Bold;
				masterSmallStyle.fontStyle = FontStyle.Bold;
			}
			else
			{
				// anderer Spieler
				masterStyle.fontStyle = FontStyle.Normal;
				clientStyle.fontStyle = FontStyle.Normal;
				clientSmallStyle.fontStyle = FontStyle.Normal;
				masterSmallStyle.fontStyle = FontStyle.Normal;
			}
//			if(player.getPhotonPlayer().isMasterClient)
//			{
//				//				textStyle = masterStyle;
//				GUILayout.Label (player.getPhotonPlayer().name + " MasterClient", masterStyle);
//			}
//			else
//			{
				GUILayout.Label (player.getNetworkPlayer().ipAddress + " Client", clientStyle);
//			}
			GUILayout.Space(20);
			if(player.getCharacterSelector() != null)
				GUILayout.Label( "charSelector: Yes", masterStyle);
			else
				GUILayout.Label( "charSelector: NO", clientStyle);
			
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
		GUILayout.EndVertical ();
		
		
		GUI.DragWindow(new Rect(0, 0, 10000, 10000));
	}
	
	
	/**
	 * Menu zuürck, akteull in auf PhotonCharacterSelector und ConnectToPhoton
	 **/
	void BackButton()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if(myNetworkView == null)
			{
				Debug.Log("SinglePlayer");
				Application.LoadLevel( Scenes.mainmenu );
				return;
			}
			
			if(GetComponent<NetworkView>() != null)
			{
				// Unity Network disconnect
				
				// if(connected)
				
				if(Network.isServer)
				{
					// Server disconnect
					// disconnect each client
					foreach(NetworkPlayer player in Network.connections)
					{
						Network.CloseConnection(player,true);
					}
					// than close host
					Network.Disconnect();
				}
				if(Network.isClient)
				{
					// Client disconnect
					Network.Disconnect();
				}
			}
			
			Application.LoadLevel( Scenes.mainmenu );
			return;
		}
	}
	
	void OnPlayerConnected (NetworkPlayer connectedNetworkPlayer)
	{
		// OnPlayerConnected wird nur auf Server ausgeführt!
		Debug.LogWarning(this.ToString() + ": OnPlayerConnected(" + connectedNetworkPlayer.ipAddress + ")");
	}
	
	/**
	 * sync Dictionary and MasterClient Destroys current GameObjects "owned" by disconnecting PhotonPlayer
	 **/
	void OnPlayerDisconnected(NetworkPlayer disconnectedNetworkPlayer)
	{
		Debug.Log(disconnectedNetworkPlayer.ipAddress + " (disconnected)");

		// OnPlayerDisconnected wird nur auf Server ausgeführt!

		// remove Player Objects
		RemovePlayer(disconnectedNetworkPlayer);
		
		// remove Player from Dictionary
		myNetworkView.RPC("RemoveFromPlayerDictionary", RPCMode.AllBuffered, disconnectedNetworkPlayer);
	}
	
	/**
	 * Client disconnects, sync Dictionary
	 * MasterClient Destroys GameObject controlled by disconnecting Client
	 **/
	[RPC]
	void RemoveFromPlayerDictionary(NetworkPlayer networkPlayer)
	{
		Debug.Log("LOCAL RemoveFromPlayerDictionary " + networkPlayer); 
		if(networkPlayer == null)
		{
			Debug.LogError("networkPlayer == null!");
			Debug.LogError("cant remove from Dictionary!!");
			return;
		}
		playerDictionary.RemovePlayer(networkPlayer);
	}
	
	void RemovePlayer(NetworkPlayer networkPlayer)
	{
		Debug.LogWarning("RemovePlayer " + networkPlayer.ipAddress);
		if (Network.isServer)
		{
			GameObject networkPlayerCharacter = playerDictionary.TryGetCharacterGameObject(networkPlayer);
			GameObject characterSelector = playerDictionary.TryGetCharacterSelectorGameObject(networkPlayer);
			
			if(networkPlayerCharacter != null)
			{
				Network.Destroy(networkPlayerCharacter);
				Debug.Log("MasterClient: " + networkPlayerCharacter.name + " destroyed!");
			}
			else
			{
				Debug.LogWarning(networkPlayer.ipAddress + " had no CharacterGameObject!");
			}
			if(characterSelector != null)
			{
				Network.Destroy(characterSelector);
				Debug.Log("MasterClient: " + characterSelector.name + " destroyed!");
			}
			else
			{
				Debug.LogWarning(networkPlayer.ipAddress + " had no CharacterSelectorGameObject!");
			}
		}
	}

}
