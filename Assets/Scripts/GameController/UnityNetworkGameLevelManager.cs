using UnityEngine;
using System.Collections;

[RequireComponent (typeof(UnityNetworkManager))]
public class UnityNetworkGameLevelManager : MonoBehaviour {

	public delegate void OnPlayerLevelLoadComplete(NetworkPlayer netPlayer, Player newPlayer);
	public static event OnPlayerLevelLoadComplete onPlayerLevelLoadComplete;

	public GameObject _PrefabCharacterLibrary;
	CharacterLibrary characters;

	NetworkView myNetworkView;
//	UnityNetworkManager baseManager;

	int playerReadyCount = 0;


	void OnEnable()
	{
		UnityNetworkManager.onPlayerDisconnected += OnPlayerDisconnectedEvent;
	}
	
	void OnDisable()
	{
		UnityNetworkManager.onPlayerDisconnected -= OnPlayerDisconnectedEvent;
	}


	void Awake()
	{
		GameObject LibraryGO  = GameObject.Find("CharacterLibrary");
		if(LibraryGO == null)
		{
			Debug.LogError("GameObject CharacterLibrary fehlt!!! (kommt normalerweise direkt aus vorherigen Scene");
			LibraryGO = (GameObject) Instantiate(_PrefabCharacterLibrary);
		}
		else
		{
			characters = LibraryGO.GetComponent<CharacterLibrary>();
		}
		myNetworkView = this.GetComponent<NetworkView>();
//		baseManager = this.GetComponent<UnityNetworkManager>();
	}

	void Start()
	{
		Network.isMessageQueueRunning = true;
		Debug.LogWarning(this.ToString() + " Start()");
		
		if(Network.isServer)
		{
			Debug.LogWarning(this.ToString() + " Server!");
			
			if(PlayerDictionaryManager._instance.serverHasPlayer)
			{
				myNetworkView.RPC("ClientLoadingLevelComplete_Rpc", RPCMode.AllBuffered, Network.player);
				Debug.LogWarning("Server: ClientLoadingLevelComplete_Rpc");
				
			}
			else
			{
				Debug.LogWarning("PlayerDictionaryManager.serverHasPlayer == false");
			}
		}
		else if(Network.isClient)
		{
			myNetworkView.RPC("ClientLoadingLevelComplete_Rpc", RPCMode.AllBuffered, Network.player);		// ACHTUNG wird auch lokal ausgeführt!!!! für spieler der später kommt stehen seine eigenen informationen NOCH NICHT zur verfügung
		}
	}

	/// <summary>
	/// Clients the loading level complete_ rpc.
	/// </summary>
	/// <param name="netPlayer">Net player.</param> ----------------- to work also on Server to Server message
	/// <param name="info">Info.</param>
	[RPC]
	void ClientLoadingLevelComplete_Rpc(NetworkPlayer otherNetPlayer, NetworkMessageInfo info)
	{

		/**
		 * Clients: Da diese RPC auch lokal ausgeführt wird muss 
		 * dafür gesorgt werden das diese RPC nur für andere Mitspieler ausgeführt wird
		 * return Network.player == otherNetPlayer
		 * 
		 * Server: Da diese RPC auch lokal ausgeführt wird und Server sich nicht selbst bestätigen muss
		 * kann Server direkt ausführen
		 * 
		 * Server: Aufgabe 2 bestätige die Clients, das diese auch Ihr UI Elemt für Ihren Eigenen Spieler erstellen!!!!
		 * 
		 * Server: Aufgabe 3 nach bestätigung der Clients, erstelle Charactere!
		 **/



		// TODO warum alle an alle? -> das Server diese RPC auch an sich selbst senden kannn!!
		// server muss doch autoritative handeln, alle clients melden loading complete -> server gibt es weiter
		// TODO
		// Server erhält ClientLoadingLevelComplete und sendet an diese eine Person wer noch alles LoadingComplete

		if (Network.player != otherNetPlayer)
		{
			// sende information von neuem Spieler in der Szene den anderen Spielern mit
			// erstelle UI Stats Elemt für andere Clients
			PlayerLoadWasComplete(otherNetPlayer);
		}

		// TODO instantiate Clients Character

		// TODO instantiate Clients UI Slot

		// TODO update references in Player Class


		if(Network.isServer)
		{
			Player player = null;
			if(!PlayerDictionaryManager._instance.TryGetPlayer(otherNetPlayer, out player))
			{
				Debug.LogError("ClientLoadingLevelComplete_Rpc ich bin netPlayer:" + Network.player + " und hab keinen Spieler zu NetPlayer:" + otherNetPlayer.ToString() + " gefunden!");
				return;
			}
			// only Server
			// TODO Reihenfolge beachten!!!
			// bestätige Spieler seine teilnahme an aktueller Scene! (jetzt hat er auch die Buffered informationen hinter sich und bekommt für seinen Character relevante Infos)
			// 1. erstelle UI Slot
			myNetworkView.RPC("PingPongServerToAllClientLoadingLevelComplete_Rpc", RPCMode.AllBuffered, otherNetPlayer);
			// only Server
			// Instantiate Character GameObject
			// 2. Spieler findet UI Slot
			Debug.Log("InstantiateAndSetupPlayerCharacter");
			InstantiateAndSetupPlayerCharacter(otherNetPlayer, player);

			if(playerReadyCount >= Network.connections.Length)					//TODO >=
			{
				myNetworkView.RPC("SyncGameStart_Rpc", RPCMode.AllBuffered);			// TODO changed 11.04.2015		SyncGameStart & LateGameStart!
			}
		}
	}


	void PlayerLoadWasComplete(NetworkPlayer netPlayer)
	{
		Debug.LogWarning("PlayerLoadWasComplete() für " + netPlayer.ToString() + " erzeuge UI Element und speichere in PlayerDictionary<Player>!!!!!");
		Player player;
		if(PlayerDictionaryManager._instance.TryGetPlayer(netPlayer, out player))
		{
			Debug.LogWarning("ERROORORORORORRORORORERROORORORORORROROROR -> NO ERROR für netPlayer " + netPlayer.ToString() + " =)");
			player.loadingLevelComplete = true;
			playerReadyCount++;												//TODO umgeht Update() iteration über Network.connections array

			onPlayerLevelLoadComplete(netPlayer, player); // erzeuge UI Slot
		}
		else
		{
			Debug.LogError("ERROORORORORORRORORORERROORORORORORROROROR -> UI Elemnt für netPlayer " + netPlayer.ToString() + " wurde nicht erstellt, da Player nicht in playerDictionary gefunden wurde!!");
			// wird aufgerufen, wenn Spieler in laufende Game Session eingestiegen ist
			
		}
	}

	[RPC]
	void PingPongServerToAllClientLoadingLevelComplete_Rpc(NetworkPlayer netPlayer, NetworkMessageInfo info)
	{
		Debug.Log("PingPongServerToAllClientLoadingLevelComplete_Rpc von netPlayer: " + netPlayer);
		// client hat spätestens jetzt seine informationen (Buffered RPC's aus vorherigenden Scene wurde jetzt schon beantwortet)
		if (Network.player != netPlayer)
		{
			return;
		}

		PlayerLoadWasComplete(netPlayer);

	}

	void InstantiateAndSetupPlayerCharacter(NetworkPlayer netPlayerOwner, Player realOwner)
	{
		GameObject playerCharacterGameObject;
		playerCharacterGameObject = (GameObject) Network.Instantiate(realOwner.characterScriptableObject.unityNetworkPrefab, RandomSpawnPoint(), Quaternion.identity, 0);

		// Keep track of this new player so we can properly destroy it when required.
		RealOwner playerControlScript = playerCharacterGameObject.GetComponent<RealOwner>();
		playerControlScript.owner = netPlayerOwner;


		// Get the networkview of this new GameObject
		NetworkView newObjectsNetworkView = playerCharacterGameObject.GetComponent<NetworkView>();

		//TODO without viewID search 
		//TODO myNetworkView.RPC("RegisterCharacterGameObjectInPlayerDictionary_Rpc", RPCMode.AllBuffered, netPlayer, newObjectsNetworkView.viewID );

		// Call an RPC on this new PhotonView, set the NetworkPlayer who controls this new player
		// TODO BUFFERED because Player joins running session needs to know who is owner!
		newObjectsNetworkView.RPC("RegisterCharacterGameObjectInPlayerDictionary_Rpc", RPCMode.AllBuffered, netPlayerOwner );
		newObjectsNetworkView.RPC("SetCharacterControlsOwner", RPCMode.AllBuffered, netPlayerOwner);			// RealOwner Script
		newObjectsNetworkView.RPC("DeactivateKinematic", RPCMode.AllBuffered);							// PlatformCharacter Script
	}

	Vector3 RandomSpawnPoint()
	{
		float xmin = -8f;
		float xmax = 8f;
		float ymin = -5f;
		float ymax = 5f;
		float z = 0f;
		return new Vector3(Random.Range(xmin,xmax),Random.Range(ymin, ymax), z);
	}


	void OnPlayerDisconnectedEvent(NetworkPlayer netPlayer, Player player)
	{
		if(player.loadingLevelComplete)
			playerReadyCount--;		
    }

	// TODO event wird ausgeführt, somit is gewährleistet das die player reference noch existiert und nicht aus playerDictionary gelöscht wurde!! 
//	void OnPlayerDisconnected(NetworkPlayer netPlayer)
//	{
//
//
//		// TODO auf Reihenfolge ACHTEN in UnityNetworkManager wird player aus playerDictionary gelöscht !!!
//
//		Player player;
//		if(PlayerDictionaryManager._instance.TryGetPlayer(netPlayer, out player))
//		{
//			if(player.loadingLevelComplete)
//				playerReadyCount--;											//TODO consistent? disconnect kommt meistens später
//
//			// remove Character GameObject
//			RemoveCurrentPlayerCharacterGameObject(player);
//
//			// dont remove Stats
//
//		}
//		else
//		{
//			Debug.LogError("NetworkPlayer existiert nicht (mehr) in PlayerDictionary!!!");
//		}
//
//
//	}

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

	/// <summary>
	/// Syncs the game start_ rpc.
	/// </summary>
	/// <param name="info">Info.</param>
	[RPC]
	void SyncGameStart_Rpc(NetworkMessageInfo info)
	{
		// calculate TripTime of Message
		double messageTripTime = Network.time - info.timestamp;
		// TODO StartAnimationDurationTime - TripTime
		// TODO show rest of Animation
		// TODO activate controlls after Start Countdown Animation
	}

}
