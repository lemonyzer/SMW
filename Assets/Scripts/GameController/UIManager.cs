using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(UnityNetworkManager))]
[RequireComponent (typeof(UnityNetworkGameLevelManager))]
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

	}


	void onNetworkLevelCharacterSelectionLoaded()
	{
		if (Network.isServer)
		{
			Server();
		}
		else
		{
			// Client Code...
		}
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

		if(!PlayerDictionaryManager._instance.serverHasPlayer)
		{
			// Server hat noch keinen Spieler
			// Auswhl zum Joinen anzeigen
			serverSelectorSlot = (GameObject) Instantiate(ServerSelectorSlotPrefab, Vector3.zero, Quaternion.identity);
			serverSelectorSlot.GetComponent<SelectorSlotScript>().btnNextCharacter.gameObject.SetActive(false);
			serverSelectorSlot.GetComponent<SelectorSlotScript>().btnSwitchTeams.gameObject.SetActive(false);
			serverSelectorSlot.transform.SetParent(SelectorSlotPanel.transform,false);
		}
	}

	void OnEnable()
	{
		// Server Join Button
		UnityNetworkManager.onNetworkLevelCharacterSelectionLoaded += onNetworkLevelCharacterSelectionLoaded;

		// Selector Slot
		UnityNetworkManager.onNewPlayerConnected += AddNewPlayerSelectorSlot;
		UnityNetworkManager.onPlayerChangedSelection += UpdateSelectorSlot;
		UnityNetworkManager.onPlayerDisconnected += PlayerDisconnected;

		// Stats Slot
		UnityNetworkGameLevelManager.onPlayerLevelLoadComplete += AddNewPlayerStatsSlot;		// player has no properties set!!!
		PlatformCharacter.onLateJoinerInstantiateNetworkCharacter += AddNewPlayerStatsSlot;										// player gets properties from Character GO
		PlatformCharacter.onRegistered += UpdateStatesSlot;										// player gets properties from Character GO

		// Server UI
		ButtonServerJoinGameScript.OnClicked += ServerJoins_Button;

		// Other
		StatsManager.onPvPKill += OnPvPKill;
	}
	
	void OnDisable()
	{
		// Server Join Button
		UnityNetworkManager.onNetworkLevelCharacterSelectionLoaded -= onNetworkLevelCharacterSelectionLoaded;
		
		// Selector Slot
		UnityNetworkManager.onNewPlayerConnected -= AddNewPlayerSelectorSlot;
		UnityNetworkManager.onPlayerChangedSelection -= UpdateSelectorSlot;
		UnityNetworkManager.onPlayerDisconnected -= PlayerDisconnected;

		// Selector Slot
		UnityNetworkGameLevelManager.onPlayerLevelLoadComplete -= AddNewPlayerStatsSlot;		// player has no properties set!!!
		PlatformCharacter.onLateJoinerInstantiateNetworkCharacter -= AddNewPlayerStatsSlot;										// player gets properties from Character GO
		PlatformCharacter.onRegistered -= UpdateStatesSlot;										// player gets properties from Character GO

		// Server UI
		ButtonServerJoinGameScript.OnClicked -= ServerJoins_Button;

		// Other
		StatsManager.onPvPKill -= OnPvPKill;
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

	static public bool IsCurrentlyInGameScene()
	{
		if (Application.loadedLevelName == Scenes.unityNetworkGame)
			return true;
		return false;
	}

	/// <summary>
	/// Adds the new player slot.
	/// </summary>
	/// <param name="netPlayer">Net player.</param>
	/// <param name="newPlayer">New player.</param>
	void AddNewPlayerSelectorSlot(NetworkPlayer netPlayer, Player newPlayer)
	{

		if (IsCurrentlyInGameScene())
			return;

		// create UI Element for player
		GameObject newNetPlayerUiSlot = (GameObject) Instantiate(SelectorUiSlotPrefab,Vector3.zero, Quaternion.identity);
		
		// disable button if Slot is not local player
		if(netPlayer != Network.player)
		{
			//newNetPlayerUiSlot.GetComponent<UiSlotScript>().next.enabled = false;
			newNetPlayerUiSlot.GetComponent<SelectorSlotScript>().btnNextCharacter.gameObject.SetActive(false);
			newNetPlayerUiSlot.GetComponent<SelectorSlotScript>().btnSwitchTeams.gameObject.SetActive(false);
		}
		
		// add it to GridLayout
		newNetPlayerUiSlot.transform.SetParent(SelectorSlotPanel.transform,false);
		
		// register UI Slot to Player
		newPlayer.UISelectorSlotScript = newNetPlayerUiSlot.GetComponent<SelectorSlotScript>();

		// Update Slot with correct Player and Character Information
		UpdateSelectorSlot(netPlayer, newPlayer);
	}

	void UpdateSelectorSlot(NetworkPlayer netPlayer, Player player)
	{
		// Update Slot with correct Player and Character Information
		player.UISelectorSlotScript.UpdateSlot(player);
    }

	void AddNewPlayerStatsSlot(NetworkPlayer netPlayer, Player player, int teamId)
	{

		/**
		 * Warum zwei mal AdNewPlayerStatsSlot ?
		 * 
		 * man könnte die StatsSlots bereits anzeigen lassen bevor die SpielCharacter Instantiert wurden!
		 * 
		 * waiting for other player -> isReady [x]
		 * 
		 **/

		if (player.UIStatsSlotScript != null)
		{
			Debug.LogWarning(this.ToString() + " Player " + player.getUserName()+ " Id:" + netPlayer.ToString() + " hat bereits ein UI Stats Slots..."); 
			UpdateStatesSlot (netPlayer, player);
			return;
		}
		Debug.LogWarning(this.ToString() + " Player " + player.getUserName() + " hat noch kein UIStatsSlot, Instantierte und update Slot informationen!");
		//random Rand
//		int randomColor = Random.Range(0,playerStatsSlotPrefabs.Length);
		if(teamId >= 0 && teamId < playerStatsSlotPrefabs.Length)
		{

		}
		else
		{
			Debug.LogError ("TeamId " + teamId + " stimmt nicht mit playerStatsSlotPrefabs.Length = " + playerStatsSlotPrefabs.Length + " überein!");
			if (playerStatsSlotPrefabs.Length > 0)
				teamId = playerStatsSlotPrefabs.Length -1;
			else
			{
				return;
			}
		}

		// erzeuge UI Slot Element
		GameObject statsSlot = Instantiate(playerStatsSlotPrefabs[teamId].gameObject, Vector3.zero, Quaternion.identity) as GameObject;

		// füge es GridLayout hinzu
		statsSlot.transform.SetParent(PlayerStatsSlotPanel.transform, false);

		// speichere Referenz in player
		player.setStatsSlot(statsSlot);

		// greife auf Referenz zu und Update Slot
		//TODO alternative suchen Start() muss vorher einmal ausgeführt werden (initialisierung)
		//player.UIStatsSlotScript.Awake();															// wird implizit mit Instantiate ausgeführt!
//		Debug.LogError(this.ToString() + " " + player.GetHashCode());
		player.UIStatsSlotScript.UpdateSlot(netPlayer, player);
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

	void OnPvPKill(Player killer, Player victim)
	{
		if (killer.UIStatsSlotScript != null)
		{
//			killer.UIStatsSlotScript.slotKills.text = "Kills: " + killer.getKills();
//			killer.UIStatsSlotScript.slotPoints.text = "Points: " + killer.getPoints();
			killer.UIStatsSlotScript.AddKill();
			killer.UIStatsSlotScript.AddPoint();
		}
		else
			Debug.LogError("killer.UIStatsSlotScript == NULL!");

		if (victim.UIStatsSlotScript != null)
		{
//			victim.UIStatsSlotScript.slotLifes.text = "Lifes: " + victim.GetLifes();
			victim.UIStatsSlotScript.LostLife();
		}
		else
			Debug.LogError("killer.UIStatsSlotScript == NULL!");
	}

	void AddPoints(Player killer, Player victim)
	{

	}

	void UpdateStatesSlot(NetworkPlayer netPlayer, Player player)
	{
		if(player == null)
		{
			Debug.LogError("player == null");
			return;
		}
		else
		{
			if(player.UIStatsSlotScript == null)
			{
				Debug.LogError("player.UIStatsSlotScript == null " + player.getUserName());
//				AddNewPlayerStatsSlot(netPlayer, player);
			}
			player.UIStatsSlotScript.UpdateSlot(netPlayer, player);
		}
	}

}
