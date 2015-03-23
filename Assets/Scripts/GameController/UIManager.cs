using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {

	NetworkView myNetworkView;

	public Button btnStart; //TODO Event!
	public GameObject ServerSelectorSlotPrefab;
	GameObject serverSelectorSlot;

	/**
	 * PREFAB UI Slot to select new and show current Character
	 * 
	 * Server:
	 * only for Clients
	 * 
	 * Clients:
	 * for all
	 **/
	public GameObject SelectorUiSlotPrefab;

	public GameObject[] playerStatsSlotPrefabs;	
	
	/**
	 * UI Panel to store the UI Playerslots 
	 **/
	GameObject SelectorSlotPanel;
	public GameObject PlayerStatsSlotPanel;

	void Awake()
	{
		myNetworkView = this.GetComponent<NetworkView>();
		SelectorSlotPanel = GameObject.Find("SelectorSlotPanel");
	}

	void Start()
	{
		if(Network.peerType == NetworkPeerType.Server)
		{
			if(Application.loadedLevelName == Scenes.unityNetworkCharacterSelection)
			{
				// in CharacterSelection Scene 
				Server();
			}
			else
			{
				//TODO GENERIC
//				// in allen anderen Scenen in dem der UIManager sitzt
//				if(Network.isServer)
//				{
//					if(PlayerDictionaryManager.serverHasPlayer)
//					{
//						Player serverPlayer;
//						if( PlayerDictionaryManager._instance.TryGetPlayer(Network.player, out serverPlayer) )
//						{
//							AddNewPlayerStatsSlot(Network.player, serverPlayer);
//						}
//						else
//						{
//							Debug.LogError("Server Has Player set, but no player in dictionary found!");
//						}
//					}
//				}
			}
		}
	}

	void OnServerInitialized()
	{
		Debug.Log("OnServerInitialized");
		Server ();
	}

	/// <summary>
	/// Server this instance.
	/// </summary>
	void Server()
	{
		if(Application.loadedLevelName == Scenes.unityNetworkCharacterSelection)
		{
			// -> wir sind in CharacterSelection Scene
			// aktiviere Startbutton um in Game Scene wechseln zu können
			btnStart.gameObject.SetActive(true);
		}

		if(!PlayerDictionaryManager.serverHasPlayer)
		{
			// Server hat noch keinen Spieler
			// Auswhl zum Joinen anzeigen
			serverSelectorSlot = (GameObject) Instantiate(ServerSelectorSlotPrefab, Vector3.zero, Quaternion.identity);
			serverSelectorSlot.GetComponent<SelectorSlotScript>().next.gameObject.SetActive(false);
			serverSelectorSlot.transform.SetParent(SelectorSlotPanel.transform,false);
		}
	}

	void OnEnable()
	{

		UnityNetworkManager.onNewPlayerConnected += AddNewPlayerSelectorSlot;
		UnityNetworkManager.onPlayerDisconnected += PlayerDisconnected;

		UnityNetworkGameLevelManager.onPlayerLevelLoadComplete += AddNewPlayerStatsSlot;
		ButtonServerJoinGameScript.OnClicked += ServerJoins_Button;
	}
	
	void OnDisable()
	{
		UnityNetworkManager.onNewPlayerConnected -= AddNewPlayerSelectorSlot;
		UnityNetworkManager.onPlayerDisconnected -= PlayerDisconnected;

		UnityNetworkGameLevelManager.onPlayerLevelLoadComplete -= AddNewPlayerStatsSlot;
		ButtonServerJoinGameScript.OnClicked -= ServerJoins_Button;
	}


	//TODO generic (like client connect)
	/// <summary>
	/// Servers the joins_ button.
	/// </summary>
	public void ServerJoins_Button()
	{
		Destroy(serverSelectorSlot);
	}
	
	//TODO generic (like client connect)
	/// <summary>
	/// Servers the leave_ button.
	/// </summary>
	public void ServerLeave_Button()
	{
		Destroy(serverSelectorSlot);
	}

	/// <summary>
	/// Adds the new player slot.
	/// </summary>
	/// <param name="netPlayer">Net player.</param>
	/// <param name="newPlayer">New player.</param>
	void AddNewPlayerSelectorSlot(NetworkPlayer netPlayer, Player newPlayer)
	{
		// create UI Element for player
		GameObject newNetPlayerUiSlot = (GameObject) Instantiate(SelectorUiSlotPrefab,Vector3.zero, Quaternion.identity);
		
		// disable button if Slot is not local player
		if(netPlayer != Network.player)
		{
			//newNetPlayerUiSlot.GetComponent<UiSlotScript>().next.enabled = false;
			newNetPlayerUiSlot.GetComponent<SelectorSlotScript>().next.gameObject.SetActive(false);
		}
		
		// add it to GridLayout
		newNetPlayerUiSlot.transform.SetParent(SelectorSlotPanel.transform,false);
		
		// register UI Slot to Player
		newPlayer.UISelectorSlotScript = newNetPlayerUiSlot.GetComponent<SelectorSlotScript>();

		// Update Slot with correct Player and Character Information
		newPlayer.UISelectorSlotScript.UpdateSlot(newPlayer);
	}

	void AddNewPlayerStatsSlot(NetworkPlayer netPlayer, Player player)
	{
		//random Rand
		int randomColor = Random.Range(0,playerStatsSlotPrefabs.Length);

		// erzeuge UI Slot Element
		GameObject statsSlot = Instantiate(playerStatsSlotPrefabs[randomColor].gameObject, Vector3.zero, Quaternion.identity) as GameObject;

		// füge es GridLayout hinzu
		statsSlot.transform.SetParent(PlayerStatsSlotPanel.transform, false);

		// speichere Referenz in player
		player.setStatsSlot(statsSlot);

		// greife auf Referenz zu und Update Slot
		//TODO alternative suchen Start() muss vorher einmal ausgeführt werden (initialisierung)
		player.UIStatsSlotScript.Start();
		player.UIStatsSlotScript.UpdateSlot(player);
	}

	void PlayerDisconnected(NetworkPlayer netPlayer, Player player)
	{
		RemovePlayerSelectorSlot(player);
		RemovePlayerStatsSlot(player);
	}

	void RemovePlayerSelectorSlot(Player player)
	{
		if(player != null)
		{
			if(player.UISelectorSlotScript != null)
				Destroy(player.UISelectorSlotScript.gameObject);
			else
				Debug.LogWarning("player.UISelectorSlotScript == null");
		}
		else
			Debug.LogWarning("player == null");
	}

	void RemovePlayerStatsSlot(Player player)
	{
		if(player != null)
		{
			if(player.UIStatsSlotScript != null)
				Destroy(player.UIStatsSlotScript.gameObject);
			else
				Debug.LogWarning("player.UIStatsSlotScript == null");
		}
		else
			Debug.LogWarning("player == null");

	}

}
