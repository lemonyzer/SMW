using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FriendsScreen : MonoBehaviour
{
	public GameObject LobbyScreen;

	private string addFriendName = "";

	private List<string> friends = new List<string>();
	private Vector2 friendsScroll;

	private Dictionary<string, bool> onlineStates = new Dictionary<string, bool>();
	private Dictionary<string, string> rooms = new Dictionary<string, string>();

	GUIStyle buttonStyle;
	GUIStyle onlineStyle;
	GUIStyle offlineStyle;
	float minButtonHeight;
	
	void InitGUIStyle()
	{

		onlineStyle = new GUIStyle();
		onlineStyle.normal.textColor = Color.green;

		offlineStyle = new GUIStyle();
		offlineStyle.normal.textColor = Color.red;

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

	void Awake()
	{
		InitGUIStyle ();
		// load friends from PlayerPrefs
		string stored_friends = PlayerPrefs.GetString( "FriendsList", "" );
		if( !string.IsNullOrEmpty( stored_friends ) )
		{
			friends.AddRange( stored_friends.Split( ',' ) );
		}

		// request friend states
		if( friends.Count > 0 )
		{
			PhotonNetwork.FindFriends( friends.ToArray() );
		}
	}

	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width*0.5f,0,Screen.width*0.5f,Screen.height));
		// go back to the lobby screen
//		if( GUILayout.Button( "Back", GUILayout.Width( 200f ), GUILayout.MinHeight(minButtonHeight) ) )
//		{
//			gameObject.SetActive( false );
//			LobbyScreen.SetActive( true );
//		}

		GUILayout.Label( "Add Friend:" );

		GUILayout.BeginHorizontal();

		// let the player type in a friend name
		addFriendName = GUILayout.TextField( addFriendName, GUILayout.Width( 200f ) );

		// add player name to friends list, request friend states
		if( GUILayout.Button( "Add", GUILayout.Width( 100f ), GUILayout.MinHeight(minButtonHeight) ) )
		{
			AddFriend( addFriendName );
		}

		GUILayout.EndHorizontal();

		if( PhotonNetwork.Friends != null )
		{

			friendsScroll = GUILayout.BeginScrollView( friendsScroll, GUILayout.ExpandHeight( true ) );

			foreach( FriendInfo friend in PhotonNetwork.Friends )
			{
				GUILayout.BeginHorizontal();

				//GUILayout.Label( friend.Name + " [" + ( GetOnlineState( friend ) ? "Online]" : "Offline]" ) );
				if(GetOnlineState( friend ))
					GUILayout.Label( friend.Name + " [Online]", onlineStyle );
				else
					GUILayout.Label( friend.Name + " [Offline]", offlineStyle );

				if( GetIsInRoom( friend ) )
				{
					if( GUILayout.Button( "Join", GUILayout.Width( 50f ), GUILayout.MinHeight(minButtonHeight) ) )
					{
						// join the friend in whatever room they are in
						PhotonNetwork.JoinRoom( GetRoom( friend ) );
					}
				}

				// remove the friend from the friends list, and fetch friend states
				if( GUILayout.Button( "Remove", GUILayout.Width( 100f ), GUILayout.MinHeight(minButtonHeight) ) )
				{
					RemoveFriend( friend.Name );
				}

				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
		}
		GUILayout.EndArea();
	}

	void Update()
	{
		if( PhotonNetwork.FriendsListAge >= 1000 )
		{
			PhotonNetwork.FindFriends( friends.ToArray() );
		}
	}

	// while updating a friends list, Photon will temporarily set isOnline and isInRoom to false
	// if you update on a timer, you will notice state rapidly switching between offline and online
	// therefore, we will store online state and room in a dictionary and wait until an update is actually received
	// and store the updated value
	void OnUpdatedFriendList()
	{
		foreach( FriendInfo friend in PhotonNetwork.Friends )
		{
			onlineStates[ friend.Name ] = friend.IsOnline;
			rooms[ friend.Name ] = friend.IsInRoom ? friend.Room : "";
		}
	}

	bool GetOnlineState( FriendInfo friend )
	{
		if( onlineStates.ContainsKey( friend.Name ) )
			return onlineStates[ friend.Name ];
		else
			return false;
	}

	bool GetIsInRoom( FriendInfo friend )
	{
		if( rooms.ContainsKey( friend.Name ) )
			return !string.IsNullOrEmpty( rooms[ friend.Name ] );
		else
			return false;
	}

	string GetRoom( FriendInfo friend )
	{
		if( rooms.ContainsKey( friend.Name ) )
			return rooms[ friend.Name ];
		else
			return "";
	}

	void AddFriend( string friendName )
	{
		friends.Add( friendName );
		PhotonNetwork.FindFriends( friends.ToArray() );

		// save friends to PlayerPrefs
		PlayerPrefs.SetString( "FriendsList", string.Join( ",", friends.ToArray() ) );
	}

	void RemoveFriend( string friendName )
	{
		friends.Remove( friendName );
		PhotonNetwork.FindFriends( friends.ToArray() );

		// save friends to PlayerPrefs
		PlayerPrefs.SetString( "FriendsList", string.Join( ",", friends.ToArray() ) );
	}
}