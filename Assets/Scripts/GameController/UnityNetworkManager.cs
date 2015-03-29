using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UnityNetworkManager : MonoBehaviour {

	public delegate void OnNewPlayerConnected(NetworkPlayer netPlayer, Player newPlayer);
	public static event OnNewPlayerConnected onNewPlayerConnected;

	public delegate void OnPlayerDisconnected_custom(NetworkPlayer netPlayer, Player newPlayer);
	public static event OnPlayerDisconnected_custom onPlayerDisconnected;


	void OnEnable()
	{
		//TODO there is no ButtonNextCharacterScript at the beginning... is this a Problem??
		ButtonNextCharacterScript.OnClicked += NextCharacter_Button;
		ButtonServerJoinGameScript.OnClicked += ServerJoins_Button;
		ButtonStartLoadingGameScene.OnClicked += StartLoadingGameScene_Button;

		PlayerStatsSlotScript.OnClicked += ActivateBotControls;
	}

	void OnDisable()
	{
		ButtonNextCharacterScript.OnClicked -= NextCharacter_Button;
		ButtonServerJoinGameScript.OnClicked -= ServerJoins_Button;
		ButtonStartLoadingGameScene.OnClicked -= StartLoadingGameScene_Button;

		PlayerStatsSlotScript.OnClicked -= ActivateBotControls;
	}

	public void ActivateBotControls(PlayerStatsSlotScript slot)
	{
		Player player;
		PlayerDictionaryManager._instance.TryGetPlayer(slot.netPlayer, out player);

		if(player != null)
		{
			if(player.platformCharacterScript != null)
			{
				if(player.platformCharacterScript.gameObject != null)
				{
					if(player.platformCharacterScript.gameObject.GetComponent<Bot>() != null)
					{
						Debug.Log("player -> sucessFull");
						player.platformCharacterScript.gameObject.GetComponent<Bot>().enabled = !slot.player.platformCharacterScript.gameObject.GetComponent<Bot>().enabled;
					}
					else
						Debug.LogError("player -> GetComponent<Bot>()");
				}
				else
					Debug.LogError("player.platformCharacterScript.gameObject");
			}
			else
				Debug.LogError("player.platformCharacterScript");
		}
		else
			Debug.LogError("player");

		if(slot.player != null)
		{
			if(slot.player.platformCharacterScript != null)
			{
				if(slot.player.platformCharacterScript.gameObject != null)
				{
					if(slot.player.platformCharacterScript.gameObject.GetComponent<Bot>() != null)
					{
						Debug.Log("slot.player -> sucessFull");
						slot.player.platformCharacterScript.gameObject.GetComponent<Bot>().enabled = true;
					}
					else
						Debug.LogError("slot.player -> GetComponent<Bot>()");
				}
				else
					Debug.LogError("slot.player.platformCharacterScript.gameObject");
			}
			else
				Debug.LogError("slot.player.platformCharacterScript");
		}
		else
			Debug.LogError("slot.player");
	}


	/// <summary>
	/// Nexts the character_ button.
	/// </summary>
	public void NextCharacter_Button()
	{
		myNetworkView.RPC("NextCharacter_Rpc", RPCMode.All, Network.player);	// works also with Server -> Server
		// Parameter wird übergeben da in NetworkMessageInfo bei Server -> Server kein sender drin steht (-1)!
	}
	
	//TODO Events / Actions / Delegates
	//TODO persistent object (atleast playerDictionary)
	/// <summary>
	/// Start_s the button.
	/// </summary>
	public void StartLoadingGameScene_Button()
	{
		myNetworkView.RPC("StartLoadingGameScene_Rpc", RPCMode.AllBuffered, nextScene);
	}
	
	//TODO generic (like client connect)
	/// <summary>
	/// Servers the joins_ button.
	/// </summary>
	public void ServerJoins_Button()
	{
		PlayerDictionaryManager.serverHasPlayer = true;
		OnPlayerConnected(Network.player);
	}
	
	//TODO generic (like client connect)
	/// <summary>
	/// Servers the leave_ button.
	/// </summary>
	public void ServerLeave_Button()
	{
		PlayerDictionaryManager.serverHasPlayer = false;
		OnPlayerDisconnected(Network.player);
	}


	/**
	 * Level Transition
	 **/
	public string nextScene = Scenes.unityNetworkGame;

	/**
	 * Message Window
	 **/
	[SerializeField] InputField messageWindow;
	Queue<string> messages;
	const int messageCount = 6;

	/**
	 * NetworkView Reference (Component @ same GameObject)
	 **/
	NetworkView myNetworkView;

	/**
	 * CharacterLibrary Reference (Component @ same GameObject)
	 **/
	CharacterLibrary myCharacterLibrary;
	

	/**
	 * Player on Serverdevice can join the Game
	 **/
//	public static bool serverHasPlayer = false;





	/**
	 * //TODO persistent
	 * PlayerDictionary : PERSISTENT (Informations needed in CharacterSelection and GameScene)
	 * 
	 * Server is authoritative to avoid conflicts and syncs it with all Clients
	 *
	 **/
	//public Dictionary<NetworkPlayer, Player> playerDictionary;
	

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		//playerDictionary = new Dictionary<NetworkPlayer, Player>();
		GameObject LibraryGO  = GameObject.Find("CharacterLibrary");
		if(LibraryGO != null)
			myCharacterLibrary = LibraryGO.GetComponent<CharacterLibrary>();
		else
			Debug.LogError("GameObject CharacterLibrary fehlt!!! (kommt normalerweise direkt aus vorherigen Scene");
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () {

		//TODO persistent
		//method A
		//DontDestroyOnLoad(gameObject);
		//method B singleton

		myNetworkView = GetComponent<NetworkView>();

		messages = new Queue<string>(messageCount);
		Network.logLevel = NetworkLogLevel.Full;


	}

	//TODO persistent object (atleast playerDictionary)
	/// <summary>
	/// Start_s the rpc.
	/// </summary>
	/// <param name="nextScene">Next scene.</param>
	[RPC]
	void StartLoadingGameScene_Rpc(string nextScene)
	{
		this.nextScene = nextScene;

		// MessageQueue pausieren
		Network.isMessageQueueRunning = false;

		// Level laden
		Application.LoadLevel(nextScene);

		// MessageQueue fortsetzen
		//TODO ausgelagert UnityNetworkGameLevelManager

		// auf alle Spieler warten, Timeout 5 Sekunden
		//TODO ausgelagert UnityNetworkGameLevelManager

		// RPC -> Clients SyncStart() .. 3, 2, 1, GO ...
	}
	


//	/**
//	 * Client / Server Funktion
//	 **/
//	//TODO v1. Events (GameObjects has GetSelectedTeam() and fires Event if selected)
//	//TODO v2. UI Elements (CharacterAvatar) with Events (dont need that function => no Update() spammed)
//	//TODO v2. UI Elements (Teams) with Events (dont need that function => no Update() spammed)
//	/// <summary>
//	/// Gets the selected team. (Raycast)
//	/// </summary>
//	/// <returns>The selected team.</returns>
//	/// <param name="clickedPosition">Clicked position.</param>
//	public string GetSelectedTeam(Vector3 clickedPosition)
//	{
//		Ray ray = Camera.main.ScreenPointToRay(clickedPosition);		
//		Vector2 origin = ray.origin;										// startPoint
//		Vector2 direction = ray.direction;									// direction
//		float distance = 100f;
//		// 2D
//		LayerMask mask = 1 << LayerMask.NameToLayer("Team");
////		Debug.Log(LayerMask.NameToLayer("Team"));
////		Debug.Log(mask.value.ToString());
//		RaycastHit2D hit = Physics2D.Raycast(origin,direction,distance,mask);
//		bool hitted = false;
//		if(hit.collider != null)
//			hitted = true;
//		// 3D
//		//		RaycastHit hit;
//		//		bool hitted = Physics.Raycast(ray, out hit);
//		if(hitted)
//		{
//			if(hit.collider.tag == Tags.character)
//			{
//				Debug.Log(this.ToString()+": selected Team = " + hit.collider.name);
//				
//				// Name des getroffenen GameObject's zurückgeben
//				return hit.collider.name;
//			}
//			else 
//			{
//				// nothing spawnable hitted
//				Debug.Log(this.ToString() + ": wrong Tag! (" + hit.collider.name + " " + hit.collider.tag + ")");
//			}
//		}
//		else
//			Debug.Log(this.ToString() + ": nothing hitted with RayCast!");
//		return null;
//	}


	/// <summary>
	/// This is called when the Master Server reports an event to the client – for example, server registered successfully, host list received, etc
	/// </summary>
	/// <param name="msevent">Msevent.</param>
	void OnMasterServerEvent( MasterServerEvent msevent )
	{
		if( msevent == MasterServerEvent.HostListReceived )
		{
			AddMessage("Hostlist received");
		}
	}

	/// <summary>
	/// Server
	/// Called on the server whenever a Network.InitializeServer was invoked and has completed.
	/// </summary>
	void OnServerInitialized()
	{
		AddMessage("Server " + Network.player.externalIP + ":" + Network.player.externalPort + " initialized. Slots: " + Network.maxConnections );
	}

	/// <summary>
	/// Raises the connected to server event.
	/// </summary>
	void OnConnectedToServer()
	{
		/** Client
		 *  Called on the client when you have successfully connected to a server.
		 **/
		myNetworkView.RPC("MyAdditionalInfo", RPCMode.Server, "rul0r");
	}

	/// <summary>
	/// Mies the additional info.
	/// </summary>
	/// <param name="name">Name.</param>
	/// <param name="info">Info.</param>
	[RPC]
	void MyAdditionalInfo(string name, NetworkMessageInfo info)
	{
		//TODO sync Server & Clients name/nick
	}

	/// <summary>
	/// Raises the disconnected from server event.
	/// </summary>
	void OnDisconnectedFromServer()
	{
		/** Client
		 *  Called on client during disconnection from server, but also on the server when the connection has disconnected.
		 **/
		Destroy(PlayerDictionaryManager._instance); // TODO consistent!!! automatisch!
		Application.LoadLevel( Scenes.mainmenu );
	}

	/// <summary>
	/// Raises the failed to connect event.
	/// </summary>
	void OnFailedToConnect()
	{
		/** Client
		 *  Called on the client when a connection attempt fails for some reason.
		 **/
	}

	/// <summary>
	/// Raises the network instantiate event.
	/// </summary>
	/// <param name="info">Info.</param>
	void OnNetworkInstantiate(NetworkMessageInfo info)
	{
		/** Client
		 *  Called on objects which have been network instantiated with Network.Instantiate.
		 **/
		if(UserIsAuthoritative())
			Debug.LogError("OnNetworkInstantiate");
		else
			Debug.Log("OnNetworkInstantiate");
	}

	/// <summary>
	/// Raises the player connected event.
	/// </summary>
	/// <param name="player">Player.</param>
	void OnPlayerConnected(NetworkPlayer netPlayer)
	{
		/** Server
		 *  Called on the server whenever a new player has successfully connected.
		 **/

		AddMessage("Player " + netPlayer.guid + " connected " + netPlayer.externalIP + " ID: " + netPlayer.ToString());

		//TODO update new Client (send all other Player informations)
		//before his own player is send! (order stays correct)
		if(netPlayer != Network.player)
		{
			// GENERIC: nicht für den Serverspieler ausführen!
			SendCurrentPlayerDictionary(netPlayer);
		}

		//new Player gets first unselected Character
		CharacterAvatar avatar = myCharacterLibrary.GetFirstUnselected();
		if(avatar != null)
		{
			//new Player will be registered in PlayerDictionary (Server)
			SetupNewPlayer(netPlayer, avatar.id);
		}
		else
		{
			//TODO myNetworkView.RPC("CharacterSelectionFailed_Rpc", requestedNetPlayer);
			return;
		}

		//Server notifys other Clients about new Player
		myNetworkView.RPC("OnPlayerConnected_Rpc", RPCMode.All, netPlayer, avatar.id);
	}


	/// <summary>
	/// Raises the player connected_ rpc event.
	/// </summary>
	/// <param name="netPlayer">Net player.</param>
	/// <param name="characterAvatarId">Character avatar identifier.</param>
	[RPC]
	void OnPlayerConnected_Rpc(NetworkPlayer netPlayer, int characterAvatarId)
	{
		if(UserIsAuthoritative())
			return;
		// nur Clients

		SetupNewPlayer(netPlayer, characterAvatarId);
	}

	/// <summary>
	/// Setups the new player.
	/// </summary>
	/// <param name="netPlayer">Net player.</param>
	/// <param name="characterAvatarId">Character avatar identifier.</param>
	void SetupNewPlayer(NetworkPlayer netPlayer, int characterAvatarId)
	{
		CharacterAvatar cA = myCharacterLibrary.Get(characterAvatarId);
		if(cA != null)
		{
			// character is selected
			cA.inUse = true;
			
			
			// create new Player
			Player newPlayer = new Player(netPlayer, cA);
			
			// register newPlayer in PlayerDictionary
			PlayerDictionaryManager._instance.AddPlayer(netPlayer, newPlayer);

			if(onNewPlayerConnected != null)
			{
				// we have event listeners
				onNewPlayerConnected(netPlayer, newPlayer);
			}
			else
			{
				Debug.LogWarning("onNewPlayerConnected no listeners!");
			}
			
		}
		else
		{
			Debug.LogError("SetupNewPlayer() characterAvatarId " + characterAvatarId + " is wrong!");
		}
	}

	/// <summary>
	/// Sends the current player dictionary.
	/// </summary>
	/// <param name="netPlayer">Net player.</param>
	void SendCurrentPlayerDictionary(NetworkPlayer netPlayer)
	{
		foreach(NetworkPlayer currentNetPlayer in Network.connections)
		{
			Player currentPlayer;
			if(PlayerDictionaryManager._instance.TryGetPlayer(currentNetPlayer, out currentPlayer))
			{
				// found Player in playerDictionary
				myNetworkView.RPC("OnPlayerConnected_Rpc", netPlayer, currentNetPlayer, currentPlayer.characterAvatarScript.id);
			}
		}

		if(PlayerDictionaryManager.serverHasPlayer)
		{
			Player currentPlayer;
			if(PlayerDictionaryManager._instance.TryGetPlayer(Network.player, out currentPlayer))
			{
				myNetworkView.RPC("OnPlayerConnected_Rpc", netPlayer, Network.player, currentPlayer.characterAvatarScript.id);
			}
			else
			{
				Debug.LogError("serverHasPlayer true, but no player in playerDictionary found!");
			}
		}
	}

	/// <summary>
	/// Raises the player disconnected event.
	/// </summary>
	/// <param name="netPlayer">Net player.</param>
	void OnPlayerDisconnected(NetworkPlayer netPlayer)
	{
		/** Server
		 *  Called on the server whenever a player is disconnected from the server.
		 **/

		myNetworkView.RPC("OnPlayerDisconnected_Rpc", RPCMode.All, netPlayer);
	}

	/// <summary>
	/// Raises the player disconnected_ rpc event.
	/// </summary>
	/// <param name="netPlayer">Net player.</param>
	[RPC]
	void OnPlayerDisconnected_Rpc(NetworkPlayer netPlayer)
	{
		Player disconnectedPlayer = GetPlayer(netPlayer);

		if(disconnectedPlayer != null)
		{
			if(onPlayerDisconnected != null)
			{
				onPlayerDisconnected(netPlayer, disconnectedPlayer);
			}
			else
			{
				Debug.LogWarning("onPlayerDisconnected no listeners!");
			}

			try
			{
				RemoveCurrentPlayerCharacterGameObject(disconnectedPlayer);
				PlayerDictionaryManager._instance.RemovePlayer(netPlayer);
				disconnectedPlayer.characterAvatarScript.inUse = false;
			}
			catch(UnityException e)
			{
				Debug.Log("OnPlayerDisconnected_Rpc: something went wrong");
			}
		}
	}

	void RemoveCurrentPlayerCharacterGameObject(Player player)
	{
		if(Network.isServer)
		{
			if(player.platformCharacterScript != null)
			{
				Network.RemoveRPCs(player.platformCharacterScript.gameObject.GetComponent<NetworkView>().viewID);
				Network.Destroy(player.platformCharacterScript.gameObject);
			}
			else
			{
				Debug.LogWarning("Spieler hatte kein Character GameObject zum Entfernen!");
			}
		}
	}

//	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
//	{
//		/** Server & Client
//		 *  Used to customize synchronization of variables in a script watched by a network view.
//		 **/
//
//		if (stream.isWriting) {
////			health = currentHealth;
////			stream.Serialize(ref health);
//		} else {
////			stream.Serialize(ref health);
////			currentHealth = health;
//		}
//
//	}


	/// <summary>
	/// Adds the message.
	/// </summary>
	/// <param name="message">Message.</param>
	void AddMessage(string message)
	{
		if(Network.peerType == NetworkPeerType.Disconnected)
		{
			AddMessage_RPC("Offline: " + message);
		}
		else
		{
			myNetworkView.RPC("AddMessage_RPC", RPCMode.All, message);
		}
	}

	/// <summary>
	/// Adds the message_ RP.
	/// </summary>
	/// <param name="message">Message.</param>
	[RPC]
	void AddMessage_RPC(string message)
	{
		messages.Enqueue(message);
		if(messages.Count > messageCount)
		{
			messages.Dequeue();
		}

		messageWindow.text = "";
		foreach(string m in messages)
		{
			messageWindow.text += m + "\n";
		}
	}


	/***************************************************************************************************************/





//	[RPC]
//	public void AddNewPlayer(NetworkPlayer newPlayer, string playerName, int characterAvatarId)
//	{
//		playerDictionary.Add(newPlayer, new Player(newPlayer, new Character()));
//	}

	/// <summary>
	/// Gets the player.
	/// </summary>
	/// <returns>The player.</returns>
	/// <param name="netPlayer">Net player.</param>
	Player GetPlayer(NetworkPlayer netPlayer)
	{
		Player player;
		if(PlayerDictionaryManager._instance.TryGetPlayer(netPlayer, out player))
			return player;
		else
			return null;
	}



	/// <summary>
	/// Nexts the character_ rpc.
	/// </summary>
	/// <param name="requestedNetPlayer">Requested net player.</param>
	/// <param name="info">Info.</param>
	[RPC]
	void NextCharacter_Rpc(NetworkPlayer requestedNetPlayer, NetworkMessageInfo info)
	{
		Debug.LogWarning("NextCharacter_Rpc");
		if(!UserIsAuthoritative())
			return;
		// Server only
		// jeder Character kann nur einmal gewählt werden, damit es fair bleibt entscheidet der Server wer
		// welchen Character bekommt: First Come First Get

//TODO	//NetworkPlayer requestedNetPlayer = info.sender;		// Spieler der einen neuen Character wünscht

		// aktuellen Character herausfinden
		Player player = GetPlayer(requestedNetPlayer);
		CharacterAvatar currentAvatar = null;
		int currentSelectedCharacterAvatarId = -1;
		if(player != null)
		{
			currentAvatar = player.characterAvatarScript;
			if(currentAvatar != null)
			{
				currentSelectedCharacterAvatarId = currentAvatar.id;
			}
			else
			{
				// Spieler hat kein CharacterAvatar !
				Debug.LogError("Spieler hat keinen CharacterAvatar, fange bei 0 an und suche freien Character!");
			}
		}
		else
		{
			// Spieler ist nicht in playerDictionary registriert !
			Debug.LogError("Spieler ist nicht in playerDictionary registriert!");
		}

		// hole nächsten verfügbaren Character aus library 
		int nextUnSelectedCharacterAvatarId = -1;
		CharacterAvatar nextAvatar = myCharacterLibrary.GetNextUnselected(currentSelectedCharacterAvatarId);
		if(nextAvatar != null)
		{
			nextUnSelectedCharacterAvatarId = nextAvatar.id;
			Debug.Log("neuer Character ist " + nextAvatar.name);
		}
		else
		{
			// kein freier Character gefunden!
			Debug.LogWarning("kein freier Character gefunden!");
			// wenn keiner mehr existiert abbrechen und requestedPlayer mitteilen (Sound)
			myNetworkView.RPC("NextCharacterFailed_Rpc", requestedNetPlayer);
			return;
		}

		// setzte aktuellen Character unSelected -> RPC
		if(currentAvatar != null)
			currentAvatar.inUse = false;
		// setzte neuen Character Selected -> RPC
		nextAvatar.inUse = true;

		// teile allen Spieler neue selection mit
		Debug.Log("info.sender = " + info.sender + ", requestedNetPlayer =" + requestedNetPlayer.ToString() + ", Lokaler Spieler = " + Network.player.ToString());
//		if(requestedNetPlayer == Network.player)
//			Debug.Log(requestedNetPlayer.ToString() + " ist Server (Hosting Player)");
			

		myNetworkView.RPC("UpdatePlayerSelection_Rpc", RPCMode.All, requestedNetPlayer, nextUnSelectedCharacterAvatarId);
	}

	/// <summary>
	/// Updates the player selection_ rpc.
	/// </summary>
	/// <param name="selector">Selector.</param>
	/// <param name="characterAvatarID">Character avatar I.</param>
	[RPC]
	void UpdatePlayerSelection_Rpc(NetworkPlayer selector, int characterAvatarID)
	{
		Debug.LogWarning("UpdatePlayerSelection_Rpc");
		Player player = GetPlayer(selector);
		if(player != null)
		{
			Debug.LogWarning("UpdatePlayerSelection_Rpc, Spieler gefunden");
			if(UserIsClient())
			{
				// nur auf Clients setzen (Server hat bereits)
				player.characterAvatarScript.inUse = false;
			}

			CharacterAvatar nextAvatar;
			nextAvatar = myCharacterLibrary.Get(characterAvatarID);

			//player neuen character zuweisen
			player.characterAvatarScript = nextAvatar;

			Debug.LogWarning("UpdatePlayerSelection_Rpc, Next Avatar " + nextAvatar.name + " Id:" +nextAvatar.id);

			if(UserIsClient())
			{
				// nur auf Clients setzen (Server hat bereits)
				nextAvatar.inUse = true;
			}
		}
		else
		{
			Debug.LogError("kein Spieler "+ selector.ToString() +" in playerDictionary gefunden!");
//TODO		Eigentlich Connected Spieler zum Server
			// Server meldet das allen mit freier Character Zuweisung (Spieler ist in playerDicitonary enthalten)
			// Server spieler (localhost) client connected aber nicht zum Server und hat deshalb keinen playerDictionary eintrag
			// Spieler existiert nicht in playerDicitionary!
			SetupNewPlayer(selector, characterAvatarID);
			player = GetPlayer(selector);
			if(player == null)
			{
				Debug.LogError("versucht Spieler neu zu erstellen fehlgeschlagen");
			}
			else
			{
				Debug.LogWarning("Spieler erfolgreich nachträglich hinzugefügt!");
			}
		}

		player.UISelectorSlotScript.UpdateSlot(player);
	}


	//TODO
	// static / singleton
	/// <summary>
	/// Users the is authoritative.
	/// </summary>
	/// <returns><c>true</c>, if is authoritative was usered, <c>false</c> otherwise.</returns>
	bool UserIsAuthoritative()
	{
		if(Network.isServer)
			return true;
		else
			return false;

	}

	//TODO
	// static / singleton
	/// <summary>
	/// Users the is client.
	/// </summary>
	/// <returns><c>true</c>, if is client was usered, <c>false</c> otherwise.</returns>
	bool UserIsClient()
	{
		if(Network.isClient)
			return true;
		else
			return false;
		
	}





































}
