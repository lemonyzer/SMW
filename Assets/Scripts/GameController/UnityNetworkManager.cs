using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UnityNetworkManager : MonoBehaviour {

	public GameObject prefabCharacterPreviewTemplate;


	public delegate void OnNewPlayerConnected(NetworkPlayer netPlayer, Player newPlayer);
	public static event OnNewPlayerConnected onNewPlayerConnected;

	public delegate void OnPlayerDisconnected_custom(NetworkPlayer netPlayer, Player newPlayer);
	public static event OnPlayerDisconnected_custom onPlayerDisconnected;


	void OnEnable()
	{
		//TODO there is no ButtonNextCharacterScript at the beginning... is this a Problem??
		ButtonNextCharacterScript.OnClicked += NextCharacter_Button;
		ButtonSwitchTeamScript.OnClicked += SwitchTeam_Button;
		ButtonServerJoinGameScript.OnClicked += ServerJoins_Button;
		ButtonStartLoadingGameScene.OnClicked += StartLoadingGameScene_Button;

		PlayerStatsSlotScript.OnClicked += ActivateBotControls;
	}

	void OnDisable()
	{
		ButtonNextCharacterScript.OnClicked -= NextCharacter_Button;
		ButtonSwitchTeamScript.OnClicked -= SwitchTeam_Button;
		ButtonServerJoinGameScript.OnClicked -= ServerJoins_Button;
		ButtonStartLoadingGameScene.OnClicked -= StartLoadingGameScene_Button;

		PlayerStatsSlotScript.OnClicked -= ActivateBotControls;
	}

	public void ActivateBotControls(PlayerStatsSlotScript slot)
	{
		Player player;
		PlayerDictionaryManager._instance.TryGetPlayer(slot.netPlayer, out player);

		Debug.LogError(this.ToString() + " " + player.GetHashCode());

		if(player != slot.player)
		{
			Debug.LogError(this.ToString() + " " +  player.GetHashCode() + " != " + slot.player.GetHashCode());
		}
		else
		{
			Debug.LogError(this.ToString() + " " +  player.GetHashCode() + " == " + slot.player.GetHashCode());
		}

		if(player != null)
		{
			if(player.platformCharacterScript != null)
			{
				if(player.platformCharacterScript.gameObject != null)
				{
					if(player.platformCharacterScript.gameObject.GetComponent<Bot>() != null)
					{
						Debug.Log("player -> sucessFull");
						//player.platformCharacterScript.gameObject.GetComponent<Bot>().enabled = !slot.player.platformCharacterScript.gameObject.GetComponent<Bot>().enabled;
						player.platformCharacterScript.gameObject.GetComponent<Bot>().enabled = !player.platformCharacterScript.gameObject.GetComponent<Bot>().enabled;
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

	/// <summary>
	/// Switchs the team_ button.
	/// </summary>
	public void SwitchTeam_Button()
	{
		myNetworkView.RPC("SwitchTeam_Rpc", RPCMode.All, Network.player);	// works also with Server -> Server
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
	 * CharacterLibrary Reference (Component @ same GameObject)
	 **/
	TeamLibrary myTeams;

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

		myTeams = GetComponent<TeamLibrary>();
		if (myTeams == null)
		{
			Debug.LogError("TeamLibrary Componente fehlt!!!");
		}
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
#if UNITY_EDITOR
		Network.logLevel = NetworkLogLevel.Informational;
#endif


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
		SmwCharacter avatar = myCharacterLibrary.characterList.GetFirstUnselected();
		Team playerTeam = myTeams.GetNewPlayerTeam();
		if(playerTeam != null)
			Debug.Log(playerTeam.mId);
		else
			Debug.LogError("playerTeam == null");
			
		int teamPos = Team.ErrorNoFreePosition;
		if(avatar != null && playerTeam != null)
		{
			teamPos = playerTeam.GetFirstFreePosition();

			if(teamPos == Team.ErrorNoFreePosition)
			{
				Debug.LogError ("kein freier Playerslot in members gefunden");
				myNetworkView.RPC("CharacterSelectionFailed_Rpc", RPCMode.All, netPlayer);
				return;
			}

			//new Player will be registered in PlayerDictionary (Server)
			SetupNewPlayer(netPlayer, avatar.charId, playerTeam.mId, teamPos);
		}
		else
		{
			// FEHLER!
			if(avatar == null)
				Debug.LogError("avatar == null");	
			if(playerTeam == null)
				Debug.LogError("playerTeam == null");

			myNetworkView.RPC("CharacterSelectionFailed_Rpc", RPCMode.All, netPlayer);
			return;
		}

		//Server notifys other Clients about new Player
		myNetworkView.RPC("OnPlayerConnected_Rpc", RPCMode.All, netPlayer, avatar.charId, playerTeam.mId, teamPos);
	}


	/// <summary>
	/// Raises the player connected_ rpc event.
	/// </summary>
	/// <param name="netPlayer">Net player.</param>
	/// <param name="characterAvatarId">Character avatar identifier.</param>
	[RPC]
	void OnPlayerConnected_Rpc(NetworkPlayer netPlayer, int characterAvatarId, int teamId, int teamPos)
	{
		if(UserIsAuthoritative())
		{
			// Server hat Spieler bereits registriert und eingestellt!
			// RPC könnte verspätet am Server ausgeführt werden, und würde für kurze Zeit neue Einstellungen vom Spieler überschreiben!
			return;
		}

		// nur Clients
		SetupNewPlayer(netPlayer, characterAvatarId, teamId, teamPos);
	}

	/// <summary>
	/// Setups the new player.
	/// </summary>
	/// <param name="netPlayer">Net player.</param>
	/// <param name="characterAvatarId">Character avatar identifier.</param>
	void SetupNewPlayer(NetworkPlayer netPlayer, int characterAvatarId, int teamId, int teamPos)
	{
		// hole Character aus CharacterLibrary
		SmwCharacter cA = myCharacterLibrary.characterList.Get(characterAvatarId);

		// hole TeamInformation aus TeamLibrary
		Team team = myTeams.GetTeam(teamId);

		if (team == null)
		{
			// abbrechen
			// return null;
			// return ErrorCode
			Debug.LogError(this.ToString() + " team == null");
        }

		if(cA == null)
		{
			Debug.LogError("SetupNewPlayer() characterAvatarId " + characterAvatarId + " is wrong!");
        }
        
        if(cA != null && team != null)
        {

			// character is selected
			cA.charInUse = true;

			// check if Sprite can be accessed
			//TODO
			// change Character Colors (Team Color)
			TeamColor.ChangeColors(team.mColors, cA.charSpritesheet[0].texture);

			// TODO connect to PLAYER !!!!
			GameObject charPreviewGo = (GameObject) Instantiate(prefabCharacterPreviewTemplate, new Vector3(-2f +teamId, 2f - teamPos, 0f), Quaternion.identity);
			CharacterPreview charPreviewScript = charPreviewGo.GetComponent<CharacterPreview>();
			charPreviewScript.run = cA.charRunSprites;
			charPreviewScript.netPlayerOwner = netPlayer;		// connect to NetworPlayer GameObject.FindWithTag()
			charPreviewScript.enabled = true;
			charPreviewGo.transform.position =  GetTeamSlotPosition(teamId, teamPos);

			// create new Player
			Player newPlayer = new Player(netPlayer, cA);
			Debug.LogError(this.ToString() + " newPlayer Hash: " + newPlayer.GetHashCode());
			// register newPlayer in PlayerDictionary
			PlayerDictionaryManager._instance.AddPlayer(netPlayer, newPlayer);

			int tempTeamPos = team.AddMember(newPlayer);
			if(tempTeamPos == teamPos)
			{
				Debug.Log("tempTeamPos == teamPos");
			}
			else
			{
				Debug.LogError("tempTeamPos != teamPos -> clients bekommen falsche Team Position mitgeteilt");
			}

			Debug.LogError(this.ToString() + " newPlayer in Dictionary Hash: " + newPlayer.GetHashCode());
			
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
	}

	/// <summary>
	/// Sends the current player dictionary.
	/// </summary>
	/// <param name="netPlayer">Net player.</param>
	void SendCurrentPlayerDictionary(NetworkPlayer netPlayer)
	{
		// all other Clients
		foreach(NetworkPlayer currentNetPlayer in Network.connections)
		{
			Player currentPlayer;
			if(PlayerDictionaryManager._instance.TryGetPlayer(currentNetPlayer, out currentPlayer))
			{
				// found Player in playerDictionary
				myNetworkView.RPC("OnPlayerConnected_Rpc", netPlayer, currentNetPlayer, currentPlayer.characterScriptableObject.charId, currentPlayer.team.mId, currentPlayer.teamPos);
			}
		}

		// Server!!!!
		if(PlayerDictionaryManager.serverHasPlayer)
		{
			Player currentPlayer;
			if(PlayerDictionaryManager._instance.TryGetPlayer(Network.player, out currentPlayer))
			{
				myNetworkView.RPC("OnPlayerConnected_Rpc", netPlayer, Network.player, currentPlayer.characterScriptableObject.charId, currentPlayer.team.mId, currentPlayer.teamPos);
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
			disconnectedPlayer.team.RemoveMember(disconnectedPlayer);			// Important, remove Player from TeamLibrary

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
				disconnectedPlayer.characterScriptableObject.charInUse = false;
				disconnectedPlayer = null;
			}
			catch(UnityException e)
			{
				Debug.Log("OnPlayerDisconnected_Rpc: something went wrong " + e);
			}
		}
		else
		{
			Debug.LogError("disconnected Player was not in playerDictionary!!!");
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

	[RPC]
	void SwitchTeam_Rpc (NetworkPlayer requestedNetPlayer, NetworkMessageInfo inf)
	{
		Debug.LogWarning("SwitchTeam_Rpc");
		if(!UserIsAuthoritative())
			return;
		// Server only
		// jeder Character kann nur einmal gewählt werden, damit es fair bleibt entscheidet der Server wer
		// welchen Character bekommt: First Come First Get

		Player player = GetPlayer(requestedNetPlayer);
		
		Team playerTeam = null;
		int currentTeamId = -1;
		
		SmwCharacter currentAvatar = null;
		int currentSelectedCharacterAvatarId = -1;

		int newTeamId = TeamLibrary.TeamIdNoTeam;

		if(player != null)
		{
			playerTeam = player.team;
			if(playerTeam != null)
			{
				//
				//	Important Part ->
				//
				currentTeamId = playerTeam.mId;

				// simple, too simple can't reach yellow and blue if alone!
//				newTeamId = myTeams.GetTeamIdWithLowestMemberCount();

				// reach all Teams, with maxMemberCheck
				newTeamId = myTeams.NextTeam(player);

				Debug.Log("Server: currentTeamId = " + currentTeamId);
				Debug.Log("Server: newTeamId = " + newTeamId);

				//
				//	<- Important Part 
				//

			}
			else
			{
				// Spieler hat kein Team !
				Debug.LogError("Spieler hat kein Team!");
				
				playerTeam = myTeams.GetNewPlayerTeam();
				if( playerTeam != null)
				{
					player.team = playerTeam;
					currentTeamId = playerTeam.mId;
				}
				else
				{
					Debug.LogError("Spieler hat IMMER NOCH KEIN kein Team!");
					return;
				}
			}
			
			currentAvatar = player.characterScriptableObject;
			if(currentAvatar != null)
			{
				currentSelectedCharacterAvatarId = currentAvatar.charId;
			}
			else
			{
				// Spieler hat kein CharacterAvatar !
                Debug.LogError("Spieler hat keinen CharacterAvatar!");
				return;
            }
        }
        else
        {
            // Spieler ist nicht in playerDictionary registriert !
            Debug.LogError("Spieler ist nicht in playerDictionary registriert!");
            return;		//TODO added 10.04.2015
        }

		if (currentTeamId == newTeamId)
		{
			myNetworkView.RPC("TeamChangeFailed_Rpc", RPCMode.All, requestedNetPlayer);					//TODO server!!!!!!! cant send to server network.player
        }
        else
		{
			// auf Server wird auftrag schon übernommen
			player.team.RemoveMember(player);
			Team newTeam = myTeams.GetTeam(newTeamId);
			if (newTeam != null)
			{
				int newTeamPos = newTeam.AddMember(player);
				if(newTeamPos != Team.ErrorNoFreePosition)
				{
					myNetworkView.RPC("UpdatePlayerSelection_Rpc", RPCMode.All, requestedNetPlayer, currentSelectedCharacterAvatarId, newTeamId, player.teamPos);
				}
				else
				{
					Debug.LogError ("// kein freier Playerslot in members gefunden");
				}
			}

		}

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

		Team playerTeam = null;
		int currentTeamId = -1;
		
		SmwCharacter currentAvatar = null;
		int currentSelectedCharacterAvatarId = -1;

		if(player != null)
		{
			playerTeam = player.team;
			if(playerTeam != null)
			{
				currentTeamId = playerTeam.mId;
			}
			else
			{
				// Spieler hat kein Team !
				Debug.LogError("Spieler hat kein Team!");

				playerTeam = myTeams.GetNewPlayerTeam();
				if( playerTeam != null)
				{
					player.team = playerTeam;
					currentTeamId = playerTeam.mId;
				}
				else
				{
					Debug.LogError("Spieler hat IMMER NOCH KEIN kein Team!");
					return;
				}
            }
            
            currentAvatar = player.characterScriptableObject;
			if(currentAvatar != null)
			{
				currentSelectedCharacterAvatarId = currentAvatar.charId;
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
			return;		//TODO added 10.04.2015
		}

		// hole nächsten verfügbaren Character aus library 
		int nextUnSelectedCharacterAvatarId = -1;
		Debug.Log("Spieler aktuelle charID= " + currentSelectedCharacterAvatarId);
		SmwCharacter nextAvatar = myCharacterLibrary.characterList.GetNextUnselected(currentSelectedCharacterAvatarId);
		if(nextAvatar != null)
		{
			nextUnSelectedCharacterAvatarId = nextAvatar.charId;
			Debug.Log("neuer Character ist charId=" + nextUnSelectedCharacterAvatarId + " charName= " + nextAvatar.name);
		}
		else
		{
			// kein freier Character gefunden!
			Debug.LogWarning("kein freier Character gefunden!");
			// wenn keiner mehr existiert abbrechen und requestedPlayer mitteilen (Sound)
			myNetworkView.RPC("NextCharacterFailed_Rpc", RPCMode.All, requestedNetPlayer);					//TODO server!!!!!!! cant send to server network.player
			return;
		}

		// setzte aktuellen Character unSelected -> RPC
		if(currentAvatar != null)
			currentAvatar.charInUse = false;
		// setzte neuen Character Selected -> RPC
		nextAvatar.charInUse = true;

		// teile allen Spieler neue selection mit
		Debug.Log("info.sender = " + info.sender + ", requestedNetPlayer =" + requestedNetPlayer.ToString() + ", Lokaler Spieler = " + Network.player.ToString());
//		if(requestedNetPlayer == Network.player)
//			Debug.Log(requestedNetPlayer.ToString() + " ist Server (Hosting Player)");
			

		myNetworkView.RPC("UpdatePlayerSelection_Rpc", RPCMode.All, requestedNetPlayer, nextUnSelectedCharacterAvatarId, currentTeamId, player.teamPos);
	}

	[RPC]
	void TeamChangeFailed_Rpc(NetworkPlayer requestedNetPlayer)
	{
		if (requestedNetPlayer == Network.player)
		{
			// mein Versuch das Team zu wechseln ging schief.
			Debug.LogError("TeamChangeFailed_Rpc");
        }
    }
    
	[RPC]
	void NextCharacterFailed_Rpc(NetworkPlayer requestedNetPlayer)
    {
		if (requestedNetPlayer == Network.player)
		{
			// mein Versuch einen neuen Character zuwählen ging schief.
			Debug.LogError("NextCharacterFailed_Rpc");
		}
	}
        
	Vector3 GetTeamSlotPosition(int teamId, int teamPos)
	{
		// Team 1 oben links
		// Vector3( -4.5, 2.5, 0 )
		// Team 2 oben links
		// Vector3( -1.5, 2.5, 0 )
		// Team 3 oben links
		// Vector3( 1.5, 2.5, 0 )
		// Team 4 oben links
		// Vector3( 4.5, 2.5, 0 )
		float xPos = -4.5f + teamId*3.0f;
		float yPos = 2.5f - teamPos*3.0f;
		float zPos = 0f;
		return new Vector3(xPos, yPos, zPos); 
	}

	/// <summary>
	/// Updates the player selection_ rpc.
	/// </summary>
	/// <param name="selector">Selector.</param>
	/// <param name="characterAvatarID">Character avatar I.</param>
	[RPC]
	void UpdatePlayerSelection_Rpc(NetworkPlayer selector, int characterAvatarID, int teamId, int teamPos)
	{
		Debug.LogWarning("UpdatePlayerSelection_Rpc selector:" + selector + ", charID= " + characterAvatarID);
		Player player = GetPlayer(selector);
		if(player != null)
		{
//			Debug.LogWarning("UpdatePlayerSelection_Rpc, Spieler gefunden");
			if(UserIsClient())
			{
				// nur auf Clients setzen (Server hat bereits)
				player.characterScriptableObject.charInUse = false;
			}

			SmwCharacter nextAvatar;
			nextAvatar = myCharacterLibrary.characterList.Get(characterAvatarID);

			//player neuen character zuweisen
			player.characterScriptableObject = nextAvatar;

			if (player.team != null)
			{
				//TODO checks
				TeamColor.ChangeColors(player.team.mColors, nextAvatar.charSpritesheet[0].texture);
			}

			GameObject[] charPreviews = GameObject.FindGameObjectsWithTag(Tags.tag_CharacterPreview);
			foreach(GameObject previewGo in charPreviews)
			{
				CharacterPreview charPreviewScript = previewGo.GetComponent<CharacterPreview>();
				if(charPreviewScript.netPlayerOwner == selector)
				{
					charPreviewScript.run = nextAvatar.charRunSprites;
					previewGo.transform.position = GetTeamSlotPosition(teamId, teamPos);
					break;
				}
			}

			Debug.LogWarning("UpdatePlayerSelection_Rpc, Next Avatar " + nextAvatar.name + " Id:" +nextAvatar.charId);

			if(UserIsClient())
			{
				// nur auf Clients setzen (Server hat bereits)
				nextAvatar.charInUse = true;
			}
		}
		else
		{
			Debug.LogError("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! kein Spieler "+ selector.ToString() +" in playerDictionary gefunden!");
			return; // added 10.04.2015
//TODO		Eigentlich Connected Spieler zum Server
			// Server meldet das allen mit freier Character Zuweisung (Spieler ist in playerDicitonary enthalten)
			// Server spieler (localhost) client connected aber nicht zum Server und hat deshalb keinen playerDictionary eintrag
			// Spieler existiert nicht in playerDicitionary!
			SetupNewPlayer(selector, characterAvatarID, teamId, teamPos);
			player = GetPlayer(selector);
			if(player == null)
			{
				Debug.LogError("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! versucht Spieler neu zu erstellen fehlgeschlagen");
				return; // added 10.04.2015
			}
			else
			{
				Debug.LogWarning("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!  Spieler erfolgreich nachträglich hinzugefügt!");
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
