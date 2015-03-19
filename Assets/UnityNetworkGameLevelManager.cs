using UnityEngine;
using System.Collections;

public class UnityNetworkGameLevelManager : MonoBehaviour {

	CharacterLibrary characters;

	NetworkView myNetworkView;
//	UnityNetworkManager baseManager;

	int playerReadyCount = 0;

	void Awake()
	{
		characters = GameObject.Find("CharacterLibrary").GetComponent<CharacterLibrary>();
		myNetworkView = this.GetComponent<NetworkView>();
//		baseManager = this.GetComponent<UnityNetworkManager>();
	}

	void Start()
	{
		Network.isMessageQueueRunning = true;
		if(!Network.isServer)
			myNetworkView.RPC("ClientLoadingLevelComplete_Rpc", RPCMode.All, Network.player);
	}

	/// <summary>
	/// Clients the loading level complete_ rpc.
	/// </summary>
	/// <param name="netPlayer">Net player.</param> ----------------- to work also on Server to Server message
	/// <param name="info">Info.</param>
	[RPC]
	void ClientLoadingLevelComplete_Rpc(NetworkPlayer netPlayer, NetworkMessageInfo info)
	{
		if(!Network.isServer)
			return;
		// only Server

		// TODO instantiate Clients Character

		// TODO instantiate Clients UI Slot

		// TODO update references in Player Class

		Player player;
		if(PlayerDictionaryManager._instance.TryGetPlayer(netPlayer, out player))
		{
			player.loadingLevelComplete = true;
			playerReadyCount++;												//TODO umgeht Update() iteration über Network.connections array

			// Instantiate Character GameObject
			InstantiateAndSetupPlayerCharacter(netPlayer, player);
		}

		if(playerReadyCount >= Network.connections.Length)					//TODO >=
		{
			myNetworkView.RPC("SyncGameStart_Rpc", RPCMode.All);
		}
	}

	void InstantiateAndSetupPlayerCharacter(NetworkPlayer netPlayer, Player realOwner)
	{
		GameObject playerCharacterGameObject;
		playerCharacterGameObject = (GameObject) Network.Instantiate(realOwner.characterAvatarScript.prefabUnityCharacter, RandomSpawnPoint(), Quaternion.identity, 0);

		// Keep track of this new player so we can properly destroy it when required.
		RealOwner playerControlScript = playerCharacterGameObject.GetComponent<RealOwner>();
		playerControlScript.owner = netPlayer;

		// Get the networkview of this new GameObject
		NetworkView newObjectsNetworkView = playerCharacterGameObject.GetComponent<NetworkView>();

		// Call an RPC on this new PhotonView, set the NetworkPlayer who controls this new player
		newObjectsNetworkView.RPC("SetCharacterControlsOwner", RPCMode.AllBuffered, netPlayer);
		newObjectsNetworkView.RPC("DeactivateKinematic", RPCMode.AllBuffered);
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

	void OnPlayerDisconnected(NetworkPlayer netPlayer)
	{
		Player player;
		if(PlayerDictionaryManager._instance.TryGetPlayer(netPlayer, out player))
		{
			if(player.loadingLevelComplete)
				playerReadyCount--;											//TODO consistent? disconnect kommt meistens später
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
