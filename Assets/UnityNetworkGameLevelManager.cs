using UnityEngine;
using System.Collections;

public class UnityNetworkGameLevelManager : MonoBehaviour {

	NetworkView myNetworkView;
	UnityNetworkManager baseManager;

	int playerReadyCount = 0;

	void Awake()
	{
		myNetworkView = this.GetComponent<NetworkView>();
		baseManager = this.GetComponent<UnityNetworkManager>();
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

		// TODO instantiate Clients Character

		// TODO instantiate Clients UI Slot

		// TODO update references in Player Class

		Player player;
		if(baseManager.playerDictionary.TryGetValue(netPlayer, out player))
		{
			player.loadingLevelComplete = true;
			playerReadyCount++;												//TODO umgeht Update() iteration über Network.connections array
		}

		if(playerReadyCount >= Network.connections.Length)					//TODO >=
		{
			myNetworkView.RPC("SyncGameStart_Rpc", RPCMode.All);
		}
	}

	void OnPlayerDisconnected(NetworkPlayer netPlayer)
	{
		Player player;
		if(baseManager.playerDictionary.TryGetValue(netPlayer, out player))
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
