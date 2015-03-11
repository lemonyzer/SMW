using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnityNetworkRoomManager : MonoBehaviour {


	public static string resourcesSubPath = "PlayerCharacter/UnityNetwork/";
	public static string gameSlotsCountPlayerPrefsString = "gameSlotsCount";
	public static string noCharacter = "noCharacter";
	
	//	Dictionary<GameObject, Player> localPlayerDictionary;
	
	Vector3 spawnPosition = new Vector3(0,0,0);
	
	// Prefab des CharacterSelectors (benötigt PhotonView)
	public GameObject photonCharacterSelectorPrefab;
	
	/**
	 * GameSlots, Scenename, GameMode, AISlots...
	 **/
	static public GamePrefs gamePrefs;
	
	/**
	 *  CharacterPrefab GameObjects in  GameScene
	 **/
	PlayerDictionary syncedLocalPersistentPlayerDictionary;// = PlayerDictionaryManager.syncedLocalPersistentPlayerDictionary;
	
	
	// nicht mehr static ?!
	//public GameState.States currentGameState = GameState.currentState;
	
	
	/**
	 * Initialisieren von ScriptableObjects, wie zB. letzte SinglePlayer Characterauswahl aus PlayerPrefs laden
	 **/
	private bool initValues = false;
	
	
	public AudioSource myAudioSource;
	public AudioClip roomBackgroundMusic;
	private Animator anim;
	private HashID hash;

	// Start Animation
	public AudioClip beepAudioClip;
	public AudioClip goAudioClip;
	// Room Background Musik
	public AudioClip gameBackgroundMusic;
	// Character Selection
	public AudioClip characterInUseAudioClip;
	public AudioClip characterSelectedSuccessfulyAudioClip;

	NetworkView myNetworkView;
	
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
	
	void Update()
	{
		BackButton();
		if(Network.isServer)
		{
			// abfrage ob bereits gestartet=?!
			if(GameState.currentState == GameState.States.Initializing)
			{
				if(EveryPlayerHasCharacter())
				{
					Debug.Log("Every Player has Character!");
					myNetworkView.RPC ("EverybodyReady", RPCMode.AllBuffered);
				}
			}
		}
	}
	
	[RPC]
	void EverybodyReady()
	{
		Debug.LogWarning("RPC EverybodyReady");
		PrepareAndStartCountDownAnimation();
	}
	
	void PrepareAndStartCountDownAnimation()
	{
		// hide all character gameobjects
		ShowAllCharacters(false);
		
		// hide all character gameobjects
		GameState.currentState = GameState.States.Starting;
		
		// music off
		if(myAudioSource != null)
		{
			myAudioSource.Stop();
		}
		StartCoroutine(startCountDown());
	}
	
	IEnumerator startCountDown()
	{
		//start CountDown
		anim.SetTrigger(hash.startCountDownTrigger);
		// 3, 2, 1, GO...
		// beep sound every second
		// 3
		yield return new WaitForSeconds(1.0f); 
		AudioSource.PlayClipAtPoint(beepAudioClip,transform.position,1);
		// 2
		yield return new WaitForSeconds(1.0f); 
		AudioSource.PlayClipAtPoint(beepAudioClip,transform.position,1);
		// 1
		yield return new WaitForSeconds(1.0f); 
		AudioSource.PlayClipAtPoint(beepAudioClip,transform.position,1);
		// GO
		yield return new WaitForSeconds(1.0f); 
		AudioSource.PlayClipAtPoint(goAudioClip,transform.position,1);
		
		StartCountDownAnimationFinished();			// needs to be public!!!
	}
	
	public void StartCountDownAnimationFinished()	// needs to be public!!!
	{
		// music on
		myAudioSource.clip = gameBackgroundMusic;
		myAudioSource.loop = true;
		myAudioSource.Play();
		ShowAllCharacters(true);
		// renderer enable true
		// spawn animation
		GameState.currentState = GameState.States.Running;
	}
	
	void ShowAllCharacters(bool enabled)
	{
		// Client: Network.connections contains only the server
		// Server: Network.connections contains only the clients
		foreach(NetworkPlayer networkPlayer in Network.connections)
		{
			GameObject character = syncedLocalPersistentPlayerDictionary.TryGetCharacterGameObject(networkPlayer);
			if(character != null)
			{
				character.GetComponent<Renderer>().enabled = enabled;
			}
		}
	}
	
	void Awake()
	{
		myNetworkView = GetComponent<NetworkView>();
		InitGUIStyle ();
		//		PhotonNetwork.automaticallySyncScene = false;
		//		localPlayerDictionary = new Dictionary<GameObject, Player> ();
		Screen.sleepTimeout = (int)SleepTimeout.NeverSleep;
		//		AwakeScriptableObjects ();
		//		playerList = new List<Player> ();
		
		
		if(Network.isMessageQueueRunning)
		{
			Debug.LogError(this.ToString() + ": (Awake) MessageQueue is running, NOT ALLOWED AT THIS MOMENT!!!");
		}
		else
		{
			Debug.Log(this.ToString() + ": (Awake) MessageQueue is queuing!!!");
		}
		
		// Start Countdown Animation 3,2,1 
		anim = GetComponent<Animator>();
		hash = GetComponent<HashID>();
		
		GameState.currentState = GameState.States.Initializing;
		
		// Kommunikationsbeginn/fortzsetzung (Buffered RPC's werden abgearbeitet)
		Network.isMessageQueueRunning = true;
	}
	
	
	void Start()
	{
		// Looping Background Audio CLip
		if(roomBackgroundMusic != null)
		{
			myAudioSource = GetComponent<AudioSource>();
			myAudioSource.clip = roomBackgroundMusic;
			myAudioSource.loop = true;
			myAudioSource.Play();
		}
		else
			Debug.LogWarning("room Background Music in roomManager not set in the Inspector");

		syncedLocalPersistentPlayerDictionary = PlayerDictionaryManager.syncedLocalPersistentPlayerDictionary;
//		Player server = new Player(Network.player, null);																		// WTF ???
//		syncedLocalPersistentPlayerDictionary.AddPlayer(Network.player,server);
	}
	
	/**
	 * Class GamePrefs
	 **/
	
	public void setNumberOfGameSlots(int slots)
	{
		//		gamePrefs.SetGameSlots(slots);
	}
	
	public int getNumberOfGameSlots()
	{
		return 4;
		//		Debug.Log("GameSlots " + gamePrefs.GetGameSlots());
		//		return gamePrefs.GetGameSlots();
	}
	
	public int getNumberOfTeams()
	{
		return 4;
		//		Debug.Log("gameTeamsCount " + gamePrefs.GetNumberOfTeams());
		//		return gamePrefs.GetNumberOfTeams();
	}

	// Server only
	void OnPlayerConnected(NetworkPlayer newPlayer)
	{
		Debug.LogWarning(this.ToString() + ": OnPlayerConnected()");
		
		//wird für MasterClient nicht ausgeführt!
		
		// OnPlayerConnected event is triggered on Server only!
		if (Network.isServer)
		{
			//A player connected to me(the MasterClient), spawn a player for it:
			// Authorative
			// Called on the MasterClient only, MasterClient instantiated GameObject and tells ControlsOwner to all Clients
			//			SpawnAuthorativeCharacterSelector(newPlayer);
			
			// Check if NetworkPlayer who connected is other than me (MasterClient)
			// newPlayer != localPlayer (MasterClient)
			if(newPlayer != Network.player)
			{
				// sync Dictionary
				//				SendCompletePlayerDictionary(newPlayer);			// sync with BUFFERED, need to check if NetworkPlayer still in room?
			}
		}
	}
	
	/**
	 * sync Dictionary
	 * new Client in room gets Complete Dictionary from MasterClient
	 **/
	void SendCompletePlayerDictionary(NetworkPlayer target)
	{
		string characterPrefabFilename = null;
		GameObject characterGameObject = null;
		NetworkView characterGameObjectNetworkView = null;
//		NetworkViewID viewID;	
		
		foreach(NetworkPlayer currentNetworkPlayer in Network.connections)
		{
			NetworkViewID viewID;			// PROBLEM ??????????????????????????????????????????????????????????????????????????????????????????????????????????
			characterPrefabFilename = syncedLocalPersistentPlayerDictionary.TryGetCharacterPrefabFilename(currentNetworkPlayer);
			if(string.IsNullOrEmpty(characterPrefabFilename))
				return;
			
			// only sync, if NetworkPlayer has Character
			
			characterGameObject = syncedLocalPersistentPlayerDictionary.TryGetCharacterGameObject(currentNetworkPlayer);
			characterGameObjectNetworkView = characterGameObject.GetComponent<NetworkView>();
			if(characterGameObjectNetworkView != null)
			{	
				viewID = characterGameObjectNetworkView.viewID;
				if(viewID != null)
				{
					myNetworkView.RPC("SyncCurrentPlayer", target, currentNetworkPlayer, characterPrefabFilename, viewID);
				}
				else
				{
					Debug.LogError("PhotonView.ViewID von " + characterGameObject.name + " == 0!!!");
				}
			}
			else
			{
				Debug.LogError("CharacterGameObject " + characterGameObject.name + " besitzt kein PhotonView!!!");
			}
		}
	}
	
	/**
	 * sync Dictionary
	 * new Client in room gets Complete Dictionary from MasterClient
	 * 
	 * viewID ist entweder von ChraracterSelector oder von CharacerGameObject!
	 * nachtrag, viewID kann nur von ChracterGameObject sein, MasterClient übertragt nur wenn Client character besitzt!!!
	 **/
	[RPC]
	void SyncCurrentPlayer(NetworkPlayer syncedNetworkPlayer, string prefabFilename, NetworkViewID viewID)
	{
		Debug.LogWarning("RPC SyncCurrentPlayer" + syncedNetworkPlayer.ipAddress + "'s GameObject: " + prefabFilename + " viewID: " + viewID);
		if (Network.isServer)		// MasterClient nicht syncen!
			return;
		NetworkView view = NetworkView.Find (viewID); 
		GameObject characterGameObject = null;
		if(view != null)
			characterGameObject = view.gameObject;
		else
		{
			characterGameObject = null;
			Debug.Log ("Sync Problem: kein GameObject mit ViewID " + viewID + " gefunden!");
		}
		Character syncedCharacter;
		Player syncedPlayer;
		
		if(characterGameObject == null)
		{
			Debug.LogError("syncCurrentPlayer failed, no characterGameObject with correct ViewID " + viewID + " found!");
		}
		
		if(string.IsNullOrEmpty(prefabFilename))
		{
			Debug.LogError("syncCurrentPlayer failed, no character prefab filename received!");
		}
		syncedCharacter = new Character (prefabFilename, characterGameObject, false);
		syncedPlayer = new Player (syncedNetworkPlayer, syncedCharacter);
		syncedLocalPersistentPlayerDictionary.AddPlayer (syncedNetworkPlayer, syncedPlayer);
	}
	
	public void SpawnAuthorativeCharacterSelector(NetworkPlayer realOwner)
	{
		// Called on the MasterClient only! (Authorative Mode)
		
		if(photonCharacterSelectorPrefab == null)
		{
			Debug.LogError("no CharacterSelector prefab setted!!! fix it in editor!!!");
			return;
		}
		// Instantiate a new object for this player, remember; the server is therefore the owner.
		GameObject playerCharacterSelectorGameObject = Network.Instantiate(photonCharacterSelectorPrefab, transform.position, transform.rotation, 0) as GameObject;
		
		// Get the networkview of this new transform
		NetworkView newObjectsview = playerCharacterSelectorGameObject.GetComponent<NetworkView>();
		
		// Tell all Clients new Client joined Room
		myNetworkView.RPC ("AddCharacterSelectorToPlayerDictionary", RPCMode.AllBuffered, realOwner, newObjectsview.viewID);
		
		/*interessant->*/		
		// Call an RPC on this new NetworkView, set the NetworkPlayer who controls this new player
		newObjectsview.RPC("SetCharacterSelectorOwner", RPCMode.AllBuffered, realOwner); // Tell every Client who is the real Owner of this CharacterSelector
		/*<-interessant*/
	}
	
	[RPC]
	void AddCharacterSelectorToPlayerDictionary(NetworkPlayer realOwner, NetworkViewID characterSelectorGameObjectViewID)
	{
		if(realOwner == null)
		{
			Debug.LogWarning("tried syncing diconnected player from buffer (masterclient/server), STOP!");
			return;
		}
		Debug.LogWarning("RPC AddCharacterSelectorToPlayerDictionary" + realOwner.ipAddress + "'s CharacterSelector: " +characterSelectorGameObjectViewID);
		
		bool stillConnected = false;
		foreach(NetworkPlayer player in Network.connections)
		{
			if(realOwner == player)
			{
				Debug.Log(realOwner.ipAddress + " is still connected!");
				stillConnected = true;
			}
		}
		if(stillConnected)
		{
			GameObject characterSelectorGameObject = NetworkView.Find (characterSelectorGameObjectViewID).gameObject;
			if(characterSelectorGameObject != null)
			{
//				Player syncedPlayer = new Player (realOwner, characterSelectorGameObject);			// add characterSelectorGameObject to newPlayer
//				syncedLocalPersistentPlayerDictionary.AddPlayer(realOwner,syncedPlayer);			// add newPlayer to Dictionary
			}
		}
		else
		{
			Debug.LogWarning(realOwner.ipAddress + " is not connected anymore!\n Dont apply buffered Action");
		}
	}
	
	
	
	/**
	 * NetworkPlayer clicked unUsed Character, MasterClient instantiats authorativ characterGameObject with Input Controls by realOwner (NetworkPlayer)
	 * after instantiation, tell every Client who is the realOwner (Controller)
	 **/
	public void SpawnAuthorativePlayerCharacter(NetworkPlayer realOwner, string characterPrefabName, Vector3 spawnPosition)
	{
		// Called on the MasterClient only! (Authorative Mode)
		GameObject characterPrefab = null;

		if(!string.IsNullOrEmpty(characterPrefabName))
		{
			// Resources.LoadAssetAtPath(resourcesPath+characterPrefabName+".prefab" <--- Editor ONLY!
			characterPrefab = (GameObject) Resources.Load(resourcesSubPath+characterPrefabName, typeof(GameObject));
		}

		if(characterPrefab != null)
		{
			// Instantiate a new object for this player, remember; the server is therefore the owner.
			GameObject playerCharacterGameObject = (GameObject) Network.Instantiate( characterPrefab, spawnPosition, Quaternion.identity,0 );
			
			//			// Bot AI
			//			bool isAI = false;
			//			Character newCharacter = new Character(characterPrefabName, playerCharacterGameObject, isAI);
			
			//			// Photon Network
			//			// update Dictionary with new Characterinformation
			//			Player player = syncedLocalPersistentPlayerDictionary.GetPlayer(realOwner);
			//			if(player == null)
			//			{
			//				// Player was not in Dictionary
			//				player = new Player(realOwner, newCharacter);
			//				syncedLocalPersistentPlayerDictionary.AddPlayer(realOwner,player);
			//				Debug.LogWarning(realOwner.name + " was not in Dictionary, therefore has no CharacterSelector.\n Is added to Dictionary with a Character now! WTF?!");
			//			}
			//			else
			//			{
			//				// Player is in Dictionary
			//				player.setCharacter(newCharacter);
			//				Debug.Log(realOwner.name + " added new Character");
			//			}
			
			// sync Dictionary with MasterClient
			myNetworkView.RPC("UpdateCurrentPlayerCharacter", RPCMode.AllBuffered, realOwner, characterPrefabName, playerCharacterGameObject.GetComponent<NetworkView>().viewID);	// ??? OthersBuffered
			
			// Get the networkview of this new transform
			NetworkView newObjectsNetworkView = playerCharacterGameObject.GetComponent<NetworkView>();
			
			// Keep track of this new player so we can properly destroy it when required.
			RealOwner playerControlScript = playerCharacterGameObject.GetComponent<RealOwner>();
			playerControlScript.owner = realOwner;
			
			// Call an RPC on this new PhotonView, set the NetworkPlayer who controls this new player
			newObjectsNetworkView.RPC("SetCharacterControlsOwner", RPCMode.AllBuffered, realOwner);
			newObjectsNetworkView.RPC("DeactivateKinematic", RPCMode.AllBuffered);
		}
		else
			Debug.LogError("prefabFile not found! " + characterPrefabName + ".prefab");
	}
	
	/**
	 * sync Dictionary if NetworkPlayer selects first/new Character
	 **/
	[RPC]
	void UpdateCurrentPlayerCharacter(NetworkPlayer currentNetworkPlayer, string prefabFilename, NetworkViewID characterGameObjectViewID)
	{
		//		if(NetworkPlayer)
		//		{
		//			Debug.LogWarning("UpdateCurrentPlayerCharacter already done!"); // ??? maybe bad, should be also doin it in this RPC
		//			return;
		//		}

		if(characterGameObjectViewID == null)
		{
			Debug.LogError("Character existiert nicht mehr: characterGameObjectViewID == null");
			return;
		}

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
		
		Player player = syncedLocalPersistentPlayerDictionary.GetPlayer(currentNetworkPlayer);
		if(player == null)
		{
			// player was not in Dictionary
			player = new Player(currentNetworkPlayer, character);
			syncedLocalPersistentPlayerDictionary.AddPlayer(currentNetworkPlayer, player);
		}
		else
		{
			// player found in dictionary
			player.setCharacter(character);
		}
	}
	
	
	//	void Spawnplayer(NetworkPlayer newPlayer)
	//	{
	//		// Authorative
	//		// Called on the MasterClient only
	//
	//		// Instantiate a new object for this player, remember; the server is therefore the owner.
	//		// Transform myNewTransPhotonNetwork.Instantiate...
	//
	//
	//		// Naive
	//		// Called on the Client itself in OnJoinedRoom event
	//
	//		//Instantiate a new object for this player, remember; the server is therefore the owner.
	//		//Transform myNewTrans = PhotonNetwork.Instantiate(playerPrefab.name, transform.position, transform.rotation, 0).transform;
	//	}

	
	/**
	 * sync Dictionary and MasterClient Destroys current GameObjects "owned" by disconnecting NetworkPlayer
	 **/
	// wird nur auf Server aufgerufen!
	void OnPlayerDisconnected(NetworkPlayer disconnectedNetworkPlayer)
	{
		Debug.Log(disconnectedNetworkPlayer.ipAddress + " (disconnected)");

		// TODO DONE
		// OnPlayerDisconnected() wird auch an GameObjects ausgeführt die kein NetworkView Komponente besitzen!!!
		// funktion wird in ConnectionStats direkt eingebunden, kein unnötiger GameObject.Find("") aufruf!
		//
		//GameObject.Find("GUI").GetComponent<ConnectionStats>().RemovePlayer(disconnectedNetworkPlayer);
		
		// wenn MasterClient disconnected kann er keinem mehr sagen das er gegangen ist!
		// neuer MasterClient muss alten verabschieden, sync Dictionary!!!
		
		if(Network.isServer)
		{
			// Current Player is MasterClient
			
			// remove Player Objects
			RemovePlayer(disconnectedNetworkPlayer);

			// remove Player from Dictionary
			myNetworkView.RPC("RemoveFromPlayerDictionary", RPCMode.AllBuffered, disconnectedNetworkPlayer);
		}
	}
	
	/**
	 * Client disconnects, sync Dictionary
	 * MasterClient Destroys GameObject controlled by disconnecting Client
	 **/

	[RPC]
	void RemoveFromPlayerDictionary(NetworkPlayer disconnectedPlayer)
	{
		Debug.Log("LOCAL RemoveFromPlayerDictionary " + disconnectedPlayer.ipAddress); 
		if(disconnectedPlayer == null)
		{
			Debug.LogError("NetworkPlayer == null!");
			Debug.LogError("cant remove from Dictionary!!");
		}
		else
		{
			syncedLocalPersistentPlayerDictionary.RemovePlayer(disconnectedPlayer);
		}
	}
	
	
	void RemovePlayer(NetworkPlayer networkPlayer)
	{
		Debug.LogWarning("RemovePlayer " + networkPlayer.ipAddress);
		if (Network.isServer)
		{
			GameObject networkPlayerCharacter = syncedLocalPersistentPlayerDictionary.TryGetCharacterGameObject(networkPlayer);
			GameObject characterSelector = syncedLocalPersistentPlayerDictionary.TryGetCharacterSelectorGameObject(networkPlayer);
			
			if(networkPlayerCharacter != null)
			{
				Network.RemoveRPCs(networkPlayerCharacter.GetComponent<NetworkView>().viewID);
				Network.RemoveRPCs(networkPlayer);
				Network.DestroyPlayerObjects(networkPlayer);
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
	
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		// Client and Server Code
		Debug.Log("OnDisconnectedFromServer -> MainMenu");
		Application.LoadLevel(Scenes.mainmenu);
	}
	
	/**
	 * RPC des Servers, (Client fordert Server auf diese Funktion zu starten)
	 * Server checks if Character at clicked Position is already in Use
	 * if not in use rigister character in PlayerPrefs and send 
	 * RPC to Animate selection on all Clients
	 * if in use RPC to requested Client and play characterInUseSound
	 **/
	[RPC]
	void CharacterClicked(string characterPrefabName, NetworkPlayer player)
	{
		// if Funktion started on normal Client (why are we here?!) break!
		if(!Network.isServer)
			return;
		
		string playerClickedID = player.ToString ();
		
		GameObject prefab = GameObject.Find(characterPrefabName);
		if( prefab != null)
		{
			// Prefab (GameObject) in Scene gefunden
			Debug.Log("Server: Prefab " + characterPrefabName + " in aktueller Scene gefunden.");
			
			if(!CheckPrefabInUse(characterPrefabName))
			{
				// kein Spieler hat diesen Character gewählt, Client Character zuteilen und freigabe mitteilen.
				
				// information in Dictionary / Liste / Array speichern
				//				SetPlayerCharacter(playerClickedID, characterPrefabName);	// Register CharacterPrefab with Player in PlayerPref
				
				// Check if player aready have a Character
				Debug.Log(this.ToString() + ": " + player.ipAddress);
				GameObject currentCharacter = syncedLocalPersistentPlayerDictionary.TryGetCharacterGameObject(player);
				if(currentCharacter != null)
				{
					RemoveCurrentCharacterGameObject(currentCharacter);	// only on server
					myNetworkView.RPC( "RemoveCurrentCharacterFromDictionary", RPCMode.AllBuffered, player );
				}
				
				// Authorative: PlayerCharacter is instantiated by MasterClient,
				// and CharacterControlsScript is enabled on Owner Client
				SpawnAuthorativePlayerCharacter(player, characterPrefabName, new Vector3(Random.Range(0,10),1,0));
				
				// Zuteilung allen Clients mitteilen
				myNetworkView.RPC( "AllowSelectedCharacter", RPCMode.AllBuffered, playerClickedID, characterPrefabName );	// RPC geht von Server an alle
			}
			else
			{
				// Character schon in Verwendung
				// anfragendem Client mitteilen (spielt sound ab)
				// spiele auf allen ab!
				if(Network.isServer)
				{
					SelectedCharacterInUse();																// RPC: Server an Server geht nicht, daher direkter aufruf
				}
				else
				{
					myNetworkView.RPC( "SelectedCharacterInUse", player );									// RPC geht von Server an requested Client
				}
			}
		}
		else
		{
			// keine Prefab (GameObject) mit passendem Name in Scene gefunden
			Debug.LogError("Server: Prefab " + characterPrefabName + " NICHT in aktueller Scene gefunden.");
			return;																			// RPC abbrechen!
		}
	}
	
	void RemoveCurrentCharacterGameObject(GameObject go)
	{
		// Server Only!
		if(Network.isServer)
		{
			Network.RemoveRPCs(go.GetComponent<NetworkView>().viewID);
			Network.Destroy(go);
		}
	}
	
	[RPC]
	void RemoveCurrentCharacterFromDictionary(NetworkPlayer disconnectingPlayer)
	{
		Debug.Log("Removing Current Character From Dictionary: " + disconnectingPlayer.ipAddress);
		Player player = syncedLocalPersistentPlayerDictionary.GetPlayer(disconnectingPlayer);
		if(player != null)
		{
			player.setCharacter(null);
		}
		else {
			Debug.Log("Player didnt exist in Dictionary: " + disconnectingPlayer.ipAddress);
		}
	}
	
	/**
	 * RPC des Clients, (Server fordert Clients auf diese Funktion zu starten)
	 * Server teilt Clients mit welcher Player einen neuen Character gewählt hat
	 **/
	[RPC]
	void AllowSelectedCharacter(string networkPlayerID, string characterPrefabName, NetworkMessageInfo info)
	{
		Debug.LogWarning("RPC AllowSelectedCharacter");
		// animation
		AudioSource.PlayClipAtPoint(characterSelectedSuccessfulyAudioClip,transform.position);
	}
	
	[RPC]
	void SelectedCharacterInUse()
	{
		Debug.LogWarning("RPC SelectedCharacterInUse");
		// play Sound
		AudioSource.PlayClipAtPoint(characterInUseAudioClip,transform.position);
	}
	
	/**
	 * MulitplayerMode == UnityNetwork
	 **/
	public bool CheckPrefabInUse(string characterPrefabName)
	{
		/*
		 * ACHTUNG!!!! Server hat ID 0, NetworkPlayer geht bei 1 los!
		 * //foreach(NetworkPlayer player in Network.connections)
		 * 
		 * wenn player 2 disconnected, wird slot nicht direkt freigegeben!
		 * Debug.LogWarning("Verbindungsanzahl: " + Network.connections.Length);
		 */
		
		// Charactere der Clients checken
		foreach(NetworkPlayer networkPlayer in Network.connections)
		{
			if( syncedLocalPersistentPlayerDictionary.TryGetCharacterPrefabFilename(networkPlayer) == characterPrefabName)
			{
				return true;
			}
		}
		return false;
	}
	
	public bool PlayerHasValidCharacter( NetworkPlayer networkPlayer )
	{
		string playerPrefabName = syncedLocalPersistentPlayerDictionary.TryGetCharacterPrefabFilename(networkPlayer);
		if(!string.IsNullOrEmpty(playerPrefabName))
		{
			// Dictionary hat einen Eintrag, Spieler hat Character gewählt.
			// Eintrag validieren 	(Filesystem nach CharacterPrefab durchsuchen
			//						oder Szene nach GameObject mit Dictionaryeigenschaften suchen
			if(GameObject.Find(playerPrefabName) != null)
			{
				// GameObject mit passendem Namen in aktueller Szene gefunden!!!
				return true;
			}
			else
			{
				// in aktueller Szene kein GameObject mit passendem Namen gefunden!!!
				Debug.LogError("Achtung! falscher PrefabName im Dictionary!!!!");
				Debug.LogError(networkPlayer.ipAddress + " " + playerPrefabName);
				//				DeleteInvalidCharacter(playerId);
				return false;
			}
		}
		else
		{
			// kein Eintrag im Dicionary, Spieler hat noch kein Character gewählt!!
			return false;
		}
	}
	
	//	public void SetPlayerCharacter(NetworkPlayer photonPlayer, string characterPrefabName)
	//	{
	//		syncedLocalPersistentPlayerDictionary.Add(photonPlayer, characterPrefabName);
	//		Debug.Log("GameManager: Key: " + playerId + " mit Value: " + playerSelectedCharacterPrefabDictionary.Get(playerId) + " in Dictionary eingetragen!");
	//	}
	
	float minButtonHeight;
	GUIStyle guiStyle;
	
	GUIStyle ownPlayerStyle;
	GUIStyle ownPlayerStyleSmall;
	
	GUIStyle otherPlayerStyle;
	GUIStyle otherPlayerStyleSmall;
	
	GUIStyle currentPlayerStyle;
	
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
		
		currentPlayerStyle = new GUIStyle ();
		currentPlayerStyle.fontStyle = FontStyle.Bold;
		
		guiStyle = new GUIStyle ();
		guiStyle.normal.textColor = Color.black;
		guiStyle.fontSize = 16;
		
		ownPlayerStyle = new GUIStyle();
		ownPlayerStyle.normal.textColor = Color.green;
		ownPlayerStyle.fontSize = 16;
		
		ownPlayerStyleSmall = new GUIStyle();
		ownPlayerStyleSmall.normal.textColor = ownPlayerStyle.normal.textColor;
		
		otherPlayerStyle = new GUIStyle();
		otherPlayerStyle.normal.textColor = Color.red;
		otherPlayerStyle.fontSize = 16;
		
		otherPlayerStyleSmall = new GUIStyle();
		otherPlayerStyleSmall.normal.textColor = otherPlayerStyle.normal.textColor;
		
		avatarStyle = new GUIStyle ();
		avatarStyle.fixedWidth = 64f;
		avatarStyle.fixedHeight = 64f;
	}
	
	bool EveryPlayerHasCharacter()
	{
		// server character abfrage
		//Debug.LogError("Server Player Character abfrage fehlt!");
		if(Network.connections.Length > 0)
		{
			foreach(NetworkPlayer player in Network.connections)
			{
				if(string.IsNullOrEmpty(syncedLocalPersistentPlayerDictionary.TryGetCharacterPrefabFilename(player)) )
				{
					return false;
				}
			}
			return true;
		}
		else
			return false;
	}
	
	public bool debugEnabled = false;
	
	private Rect windowPlayerDictionaryRect = new Rect(20, 25, Screen.width-40, 200);
	private Rect windowPlayerInfoGUIRect = new Rect(80f,10f,Screen.width-160,90);

	/// <summary>
	/// OFFs the on GU.
	/// </summary>
	void OFFOnGUI()
	{
		if(syncedLocalPersistentPlayerDictionary == null)
		{
			return;
		}
		List<Player> buffer = new List<Player> ( syncedLocalPersistentPlayerDictionary.Values() );
		if (Network.peerType == NetworkPeerType.Disconnected)
			return;
		if (buffer == null)
			return;
		// FullScreen 0,0 = Left,Top
		
		if(Network.isServer)
		{
			// GameScene kann nur von Master Client gestartet werden
			
			if(EveryPlayerHasCharacter())
			{
				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace();
				if(GUILayout.Button("Start!", GUILayout.MinHeight(minButtonHeight)))
				{
					InitGameScene();
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal ();
			}
		}
		if(debugEnabled)
			windowPlayerDictionaryRect = GUI.Window(0, windowPlayerDictionaryRect, WindowPlayerDictionary, "Player Dictionary");
		windowPlayerInfoGUIRect = GUI.Window(1, windowPlayerInfoGUIRect, WindowPlayerInfoGUI, "Players");
		
	}
	
	void WindowPlayerInfoGUI(int windowID)
	{
		List<Player> buffer = new List<Player> ( syncedLocalPersistentPlayerDictionary.Values() );
		/**
		 * Player Info
		 **/
		//		GUILayout.BeginArea (new Rect(0,Screen.height-100f,PhotonNetwork.room.playerCount*200,100));
		GUILayout.BeginHorizontal ();
		// Schleife über Spielerliste
		foreach(Player player in buffer)
		{

			GUILayout.BeginHorizontal (GUILayout.Width(200));
			if(player.getNetworkPlayer() == Network.player)
			{
				// Eigener Spieler
				currentPlayerStyle = ownPlayerStyle;
			}
			else
			{
				// anderer Spieler
				currentPlayerStyle = otherPlayerStyle;
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
//				GUILayout.Label(player.getPoints().ToString());
//			}
//			else
//			{
			GUILayout.Label("P.ID: " + player.getName(), currentPlayerStyle);	// um Text anzuzeigen
			GUILayout.Label("Kills: " + player.getPoints().ToString(), currentPlayerStyle);
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
					GUILayout.Label(player.getCharacter().getName(), currentPlayerStyle);	// um Text anzuzeigen
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
		List<Player> buffer = new List<Player> ( syncedLocalPersistentPlayerDictionary.Values() );
		/**
		 * Connection Info
		 **/
		GUILayout.BeginVertical ();
		GUILayout.Label ("Connected clients: " + Network.connections.Length + " / " + Network.maxConnections, guiStyle);
		GUILayout.Label ("Selected characters: " + buffer.Count + " / " + Network.connections.Length, guiStyle);
		//		if(Network.isServer)
		//			GUILayout.Label ("isMasterClient! "+ PhotonNetwork.player.name, guiStyle);
		//		else
		//			GUILayout.Label ("isClient! "+ PhotonNetwork.player.name, guiStyle);
		
		foreach(Player player in buffer)
		{
			GUILayout.BeginHorizontal();
			//			GUIStyle textStyle = clientStyle;
			//			GUILayout.Label (player.getNetworkPlayer().name +
			//			                 (player.getNetworkPlayer().isMasterClient ? " MasterClient":" Client"), guiStyle);
			if(player.getNetworkPlayer() == Network.player)
			{
				// Eigener Spieler
				if(Network.isServer)
				{

				}
			}
			else
			{
				// anderer Spieler
			}
//			if(player.getNetworkPlayer().isMasterClient)
//			{
//				//				textStyle = masterStyle;
//				GUILayout.Label (player.getNetworkPlayer().name + " MasterClient", masterStyle);
//			}
//			else
//			{
				GUILayout.Label (player.getNetworkPlayer().ipAddress + " Client", otherPlayerStyle);
//			}
			GUILayout.Space(20);
			// gibt kein spawnenden character selector mehr
//			if(player.getCharacterSelector() != null)
//				GUILayout.Label( "charSelector: Yes", otherPlayerStyle);
//			else
//				GUILayout.Label( "charSelector: NO", clientStyle);
			
			GUILayout.Space(20);
			if(player.getCharacter() != null)
			{
				GUILayout.Label( "Character: " + player.getCharacter().getPrefabFilename(), ownPlayerStyle);
				GUILayout.Space(20);
				if(player.getCharacter().getGameObject() != null)
					GUILayout.Label( "GO: Yes", ownPlayerStyle);
				else
					GUILayout.Label( "GO: NO", otherPlayerStyle);
			}
			else
				GUILayout.Label( "Character: NO", otherPlayerStyle);
			
			
			
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical ();
		
		
		GUI.DragWindow(new Rect(0, 0, 10000, 10000));
	}
	
	
	// ROOM Scene (CharacterSelection, and Testing Character)
	// MasterClient clicks Start:
	//	- checks wich Scene should be loaded (Voted/Selected)
	// 	- Send RPC to all Clients StartScene(int scenenumber)
	// All Clients:
	//  - in StartScene() all Clients set isMessageQueueRunning = false // to stop executing already received or next incomming RPC's
	// 	- in StartScene() Application.LoadLevel(scenenumber)
	
	// LoadedScene (GameScene, PLAYING!!!)
	// [Order]	[Who]			[What]
	// 1		All Clients		
	// 							PlayerDictionaryManager() - Awake() tries to find instantiated ScriptableObject PlayerDictionary				// Order wain?
	// 							GameManger: Awake() - set Referenz to public static PlayerDictionary from PlayerDictionaryManager()				// Order wain?
	// 2						GameManager: Start() - referenz.RemoveAllPlayerCharacterGameObjects();											// ORDER!!!! should be START
	// 3						GameManager: LateStart() isMessageQueueRunning: TRUE
	// 4		MasterClient	GameManager: SpawnAllPlayerCharacter() need BUFFERED, need update and add new CharacterGameObjects to PlayerDictionary (RPC UpdateCurrentPlayerCharacter)
	
	void InitGameScene( )
	{
		//	PhotonNetwork.LoadLevel(Scenes.photonTagging); <-- does not work 100% with RemoveAllPlayerCharacterGameObjects before incomming RPC are executed!!! 
		myNetworkView.RPC("LoadGameScene", RPCMode.AllBuffered, Scenes.photonTagging, 1);
	}
	
	[RPC]
	void LoadGameScene( string scenename, int gameMode)
	{
		// gameMode auswerten int -> GameMode gm = new GameMode(gameMode);
		Network.isMessageQueueRunning = false;	// deaktivieren
		Application.LoadLevel(scenename);
		// message queue muss wieder angeschaltet werden
	}


}
