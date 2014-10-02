using UnityEngine;
using System.Collections;

public class PhotonLobbyMenu : Photon.MonoBehaviour {

	/**
	 * Login Screen
	 **/
	public const string PHOTONUSERNAME = "PhotonUsername";
	private string username = "";
	private bool connecting = false;
	private string error = null;


	/**
	 * Lobby Screen
	 **/
	public GameObject FriendsListScreen;
	public GameObject DictionaryListScreen;
	Vector2 lobbyScroll = Vector2.zero;

	/**
	 * Friends Screen
	 **/
//	bool showFriends = true;

	/**
	 * GUIStyle
	 **/
	GUIStyle buttonStyle;
	float minButtonHeight;

	enum LobbyState
	{
		Login,
		Lobby
	}

	LobbyState currentState = LobbyState.Login;

	void Awake()
	{
		InitGUIStyle ();
	}

	void InitGUIStyle()
	{
		buttonStyle = new GUIStyle ();
		buttonStyle.stretchWidth = false;
		
		if(Screen.dpi != 0)
		{
			minButtonHeight = 20f * Screen.height / Screen.dpi;
		}
		else
		{
			minButtonHeight = 20f;
		}
	}

	// Use this for initialization
	void Start()
	{
		// load the last username the player entered
		username = PlayerPrefs.GetString( PHOTONUSERNAME, "" );
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		if(currentState == LobbyState.Login)
		{
			LoginScreen();
		}
		if(currentState == LobbyState.Lobby)
		{
			LobbyScreen();
		}
	}

	void LoginScreen()
	{
		// in the process of connecting...
		if( connecting )
		{
			GUILayout.Label( "Connecting..." );
			return;
		}
		
		// an error occurred, display it
		if( error != null )
		{
			GUILayout.Label( "Failed to connect: " + error );
			return;
		}
		
		// let the user login with Enter Key (Return), must be before the TextField!!!
		if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
		{
			connect();
		}
		
		// let the user enter their username
		GUILayout.Label( "Username" );
		username = GUILayout.TextField( username, GUILayout.Width( 200f ) );
		
		if( GUILayout.Button( "Connect", GUILayout.MinHeight(minButtonHeight) ) || Input.GetKeyDown(KeyCode.Return) )
		{
			connect();
		}
	}

	void connect()
	{
		// remember username for next time
		PlayerPrefs.SetString( PHOTONUSERNAME, username );
		
		// in the process of connecting
		connecting = true;
		
		// set username, connect to photon
		PhotonNetwork.playerName = username;
		PhotonNetwork.ConnectUsingSettings( "v1.0" );
	}

	void OnJoinedLobby()
	{
		// joined the lobby, show lobby screen
		Debug.Log(this.ToString() +": OnJoinedLobby()");
		connecting = false;
		//gameObject.SetActive( false );
		//LobbyScreen.SetActive( true );
		currentState = LobbyState.Lobby;
	}

	void OnFailedToConnectToPhoton( DisconnectCause cause )
	{
		// failed to connect, store error for display
		
		connecting = false;
		error = cause.ToString();
		Debug.Log( this.ToString() +": OnFailedToConnectToPhoton("+error+")" );
	}

	void LobbyScreen()
	{
		// join a random room
		if( GUILayout.Button( "Join Random", GUILayout.Width( 200f ), GUILayout.MinHeight(minButtonHeight) ) )
		{
			PhotonNetwork.JoinRandomRoom();
		}
		
		// create a new room
		if( GUILayout.Button( "Create Room", GUILayout.Width( 200f ), GUILayout.MinHeight(minButtonHeight) ) )
		{
			PhotonNetwork.CreateRoom( PlayerPrefs.GetString( PHOTONUSERNAME ) + "'s Room", true, true, 32 );
		}
		
		// show the friends list management page
		if( GUILayout.Button( "Friends", GUILayout.Width( 200f ), GUILayout.MinHeight(minButtonHeight) ) )
		{
			//			gameObject.SetActive( false );
			if(FriendsListScreen.activeSelf)
				FriendsListScreen.SetActive( false );
			else
				FriendsListScreen.SetActive( true );
		}
		
		// show the Dictionary ScriptAbleObject
		if( GUILayout.Button( "Debug: Dictionary", GUILayout.Width( 200f ), GUILayout.MinHeight(minButtonHeight) ) )
		{
			//			gameObject.SetActive( false );
			if(DictionaryListScreen.activeSelf)
				DictionaryListScreen.SetActive( false );
			else
				DictionaryListScreen.SetActive( true );
		}
		
		RoomInfo[] rooms = PhotonNetwork.GetRoomList();
		
		// no rooms available, inform the user
		if( rooms.Length == 0 )
		{
			GUILayout.Label( "No Rooms Available" );
		}
		else
		{
			// show a scrollable list of rooms
			
			lobbyScroll = GUILayout.BeginScrollView( lobbyScroll, GUILayout.Width( Screen.width*0.5f ), GUILayout.ExpandHeight( true ) );
			
			foreach( RoomInfo room in PhotonNetwork.GetRoomList() )
			{
				GUILayout.BeginHorizontal( GUILayout.Width( Screen.width*0.4f ) );
				
				// display room name and capacity
				GUILayout.Label( room.name + " - " + room.playerCount + "/" + room.maxPlayers );
				
				// connect to the room
				if( GUILayout.Button( "Enter", GUILayout.MinHeight(minButtonHeight)) )
				{
					PhotonNetwork.JoinRoom( room.name );
				}
				
				GUILayout.EndHorizontal();
			}
			
			GUILayout.EndScrollView();
		}
	}

	// if no room could be randomly joined, create a new room
	void OnPhotonRandomJoinFailed()
	{
		PhotonNetwork.CreateRoom( PlayerPrefs.GetString( PHOTONUSERNAME ) + "'s Room", true, true, 32 );
	}
	
	void OnConnectedToMaster ()
	{
		Debug.LogWarning(this.ToString() +": OnConnectedToMaster()");
	}
	
	// after creating the room, load the room scene
	void OnCreatedRoom()
	{
		// MasterClientCode
		Debug.LogWarning(this.ToString() +": CreatedRoom()");
		PhotonNetwork.LoadLevel( Scenes.photonRoomAuthorative );
	}
	
	void OnPhotonPlayerConnected (PhotonPlayer connectedPhotonPlayer)
	{
		Debug.LogWarning(this.ToString() +": OnPhotonPlayerConnected(" + connectedPhotonPlayer.name + ")");
	}
	
	// after joining the room, load the room scene
	void OnJoinedRoom()
	{
		// NormalClientCode, (no MasterClient)
		Debug.LogWarning(this.ToString() +": OnJoinedRoom()");
		
		// Kommunikation pausieren
		PhotonNetwork.isMessageQueueRunning = false;
		
		// Level laden
		Application.LoadLevel(Scenes.photonRoomAuthorative);
		
		//PhotonNetwork.LoadLevel( Scenes.photonRoomAuthorative );
	}
}
