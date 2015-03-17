using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TeamAndCharacterSelection : MonoBehaviour {


	[SerializeField] InputField messageWindow;
	Queue<string> messages;
	const int messageCount = 6;

	NetworkView myNetworkView;

	Vector2 moveDirection;
	

	CharacterLibrary characters;

	bool serverHasPlayer = false;

	// Use this for initialization
	void Start () {
		myNetworkView = GetComponent<NetworkView>();

		messages = new Queue<string>(messageCount);
		Network.logLevel = NetworkLogLevel.Full;


		characters = GetComponent<CharacterLibrary>();


		if(Network.peerType == NetworkPeerType.Server)
		{
			Server();
		}
	}

	public GameObject ServerSlotPrefab;
	public GameObject serverSlot;

	void Server()
	{
		serverSlot = (GameObject) Instantiate(ServerSlotPrefab, Vector3.zero, Quaternion.identity);
		serverSlot.GetComponent<UiSlotScript>().next.gameObject.SetActive(false);
		serverSlot.transform.SetParent(SlotPanel.transform,false);
	}

	public void ServerJoins_Button()
	{
		Destroy(serverSlot);
		serverHasPlayer = true;
		OnPlayerConnected(Network.player);
	}

	public void ServerLeave_Button()
	{
		Destroy(serverSlot);
		serverHasPlayer = false;
		OnPlayerDisconnected(Network.player);
	}
	
	// Update is called once per frame
	void Update () {
		if(true)
		{
			Keyboard();
			if(Input.GetMouseButtonUp(0))
			{
				GetSelectedTeam(Input.mousePosition);
			}
		}
	}

	void Keyboard()
	{
		moveDirection.x = Input.GetAxis("Horizontal");
		moveDirection.y = Input.GetAxis("Vertical");
	}

	/**
	 * Client / Server Funktion
	 **/
	public string GetSelectedTeam(Vector3 clickedPosition)
	{
		Ray ray = Camera.main.ScreenPointToRay(clickedPosition);		
		Vector2 origin = ray.origin;										// startPoint
		Vector2 direction = ray.direction;									// direction
		float distance = 100f;
		// 2D
		LayerMask mask = 1 << LayerMask.NameToLayer("Team");
//		Debug.Log(LayerMask.NameToLayer("Team"));
//		Debug.Log(mask.value.ToString());
		RaycastHit2D hit = Physics2D.Raycast(origin,direction,distance,mask);
		bool hitted = false;
		if(hit.collider != null)
			hitted = true;
		// 3D
		//		RaycastHit hit;
		//		bool hitted = Physics.Raycast(ray, out hit);
		if(hitted)
		{
			if(hit.collider.tag == Tags.character)
			{
				Debug.Log(this.ToString()+": selected Team = " + hit.collider.name);
				
				// Name des getroffenen GameObject's zurückgeben
				return hit.collider.name;
			}
			else 
			{
				// nothing spawnable hitted
				Debug.Log(this.ToString() + ": wrong Tag! (" + hit.collider.name + " " + hit.collider.tag + ")");
			}
		}
		else
			Debug.Log(this.ToString() + ": nothing hitted with RayCast!");
		return null;
	}


	/// <summary>
	/// Raises the master server event event.
	/// </summary>
	/// <param name="msevent">Msevent.</param>

	// this is called when the Master Server reports an event to the client – for example, server registered successfully, host list received, etc
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
		Server ();
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

	[RPC]
	void MyAdditionalInfo(string name, NetworkMessageInfo info)
	{

	}

	/// <summary>
	/// Raises the disconnected from server event.
	/// </summary>

	void OnDisconnectedFromServer()
	{
		/** Client
		 *  Called on client during disconnection from server, but also on the server when the connection has disconnected.
		 **/
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
	}

	/// <summary>
	/// Raises the player connected event.
	/// </summary>
	/// <param name="player">Player.</param>

	void OnPlayerConnected(NetworkPlayer player)
	{
		/** Server
		 *  Called on the server whenever a new player has successfully connected.
		 **/

		AddMessage("Player " + player.guid + " connected " + player.externalIP + " ID: " + player.ToString());

		//TODO update new Client (send all other Player informations)
		//before his own player is send! (order stays correct)
		if(player != Network.player)
			SendCurrentPlayerDictionary(player);

		//new Player gets first unselected Character
		CharacterAvatar avatar = myCharacterLibrary.GetFirstUnselected();
		if(avatar != null)
		{
			//new Player will be registered in PlayerDictionary (Server)
			SetupNewPlayer(player, avatar.id);
		}
		else
		{
			//TODO myNetworkView.RPC("CharacterSelectionFailed_Rpc", requestedNetPlayer);
			return;
		}

		//Server notifys other Clients about new Player
		myNetworkView.RPC("OnPlayerConnected_Rpc", RPCMode.All, player, avatar.id);
	}

	public GameObject SelectorUiSlotPrefab;

	[RPC]
	void OnPlayerConnected_Rpc(NetworkPlayer netPlayer, int characterAvatarId)
	{
		if(UserIsAuthoritative())
			return;
		// nur Clients

		SetupNewPlayer(netPlayer, characterAvatarId);
	}

	void SetupNewPlayer(NetworkPlayer netPlayer, int characterAvatarId)
	{
		CharacterAvatar cA = myCharacterLibrary.Get(characterAvatarId);
		if(cA != null)
		{
			// character is selected
			cA.inUse = true;
			
			// create UI Element for player
			GameObject newNetPlayerUiSlot = (GameObject) Instantiate(SelectorUiSlotPrefab,Vector3.zero, Quaternion.identity);

			// disable button if Slot is not local player
			if(netPlayer != Network.player)
			{
				//newNetPlayerUiSlot.GetComponent<UiSlotScript>().next.enabled = false;
				newNetPlayerUiSlot.GetComponent<UiSlotScript>().next.gameObject.SetActive(false);
			}

			// add it to GridLayout
			newNetPlayerUiSlot.transform.SetParent(SlotPanel.transform,false);


			
			// create new Player
			Player newPlayer = new Player(netPlayer, cA);
			
			// register UI Slot to Player
			newPlayer.uiSlotScript = newNetPlayerUiSlot.GetComponent<UiSlotScript>();
			
			// Update Slot with correct Player and Character Information
			newPlayer.uiSlotScript.UpdateSlot(newPlayer);
			
			// register newPlayer in PlayerDictionary
			playerDictionary.Add(netPlayer, newPlayer);
			
		}
		else
		{
			Debug.LogError("SetupNewPlayer() characterAvatarId " + characterAvatarId + " is wrong!");
		}
	}

	void SendCurrentPlayerDictionary(NetworkPlayer netPlayer)
	{
		foreach(NetworkPlayer currentNetPlayer in Network.connections)
		{
			Player currentPlayer;
			if(playerDictionary.TryGetValue(currentNetPlayer, out currentPlayer))
			{
				// found Player in playerDictionary
				myNetworkView.RPC("OnPlayerConnected_Rpc", netPlayer, currentNetPlayer, currentPlayer.characterAvatarScript.id);
			}
		}

		if(serverHasPlayer)
		{
			Player currentPlayer;
			if(playerDictionary.TryGetValue(Network.player, out currentPlayer))
			{
				myNetworkView.RPC("OnPlayerConnected_Rpc", netPlayer, Network.player, currentPlayer.characterAvatarScript.id);
			}
			else
			{
				Debug.LogError("serverHasPlayer true, but no player in playerDictionary found!");
			}
		}
	}

	void OnPlayerDisconnected(NetworkPlayer netPlayer)
	{
		/** Server
		 *  Called on the server whenever a player is disconnected from the server.
		 **/

		myNetworkView.RPC("OnPlayerDisconnected_Rpc", RPCMode.All, netPlayer);
	}

	[RPC]
	void OnPlayerDisconnected_Rpc(NetworkPlayer netPlayer)
	{
		Player disconnectedPlayer = GetPlayer(netPlayer);

		if(disconnectedPlayer != null)
		{
			try
			{
				playerDictionary.Remove(netPlayer);
				disconnectedPlayer.characterAvatarScript.inUse = false;
				Destroy(disconnectedPlayer.uiSlotScript.gameObject);
			}
			catch(UnityException e)
			{
				Debug.Log("OnPlayerDisconnected_Rpc: something went wrong");
			}
		}
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		/** Server & Client
		 *  Used to customize synchronization of variables in a script watched by a network view.
		 **/

		if (stream.isWriting) {
//			health = currentHealth;
//			stream.Serialize(ref health);
		} else {
//			stream.Serialize(ref health);
//			currentHealth = health;
		}

	}

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

	Dictionary<NetworkPlayer, Player> playerDictionary;
	CharacterLibrary myCharacterLibrary;

	GameObject SlotPanel;

	void Awake()
	{
		playerDictionary = new Dictionary<NetworkPlayer, Player>();
		myCharacterLibrary = GetComponent<CharacterLibrary>();

		SlotPanel = GameObject.Find("SlotPanel");
	}

	[RPC]
	public void AddNewPlayer(NetworkPlayer newPlayer, string playerName, int characterAvatarId)
	{
		playerDictionary.Add(newPlayer, new Player(newPlayer, new Character()));
	}

	Player GetPlayer(NetworkPlayer netPlayer)
	{
		Player player;
		if(playerDictionary.TryGetValue(netPlayer, out player))
			return player;
		else
			return null;
	}

	public void NextCharacter_Button()
	{
		myNetworkView.RPC("NextCharacter_Rpc", RPCMode.All, Network.player);	// works also with Server -> Server
																				// Parameter wird übergeben da in NetworkMessageInfo bei Server -> Server kein sender drin steht!
	}

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

		player.uiSlotScript.UpdateSlot(player);
	}


	bool UserIsAuthoritative()
	{
		if(Network.isServer)
			return true;
		else
			return false;

	}

	bool UserIsClient()
	{
		if(Network.isClient)
			return true;
		else
			return false;
		
	}





































}
