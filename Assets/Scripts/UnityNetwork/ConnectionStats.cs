using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ConnectionStats : MonoBehaviour {

	Dictionary<NetworkPlayer, GameObject> dictionary;

	public GameObject StatsPanel;

	public GameObject prefabSlotRed;
	public GameObject prefabSlotBlue;
	public GameObject prefabSlotGreen;
	public GameObject prefabSlotYellow;
	public GameObject prefabSlotGrey;

	public GameObject[] statsSlots;	
	public List<GameObject> statsSlotList;	

//	public GameObject statsSlot1;
//	public GameObject statsSlot2;
//	public GameObject statsSlot3;
//	public GameObject statsSlot4;

	void Awake()
	{
		// to early to add UI Elements!!
	}

	void Start()
	{

		dictionary = new Dictionary<NetworkPlayer, GameObject>();
		statsSlotList = new List<GameObject>();

		statsSlots = new GameObject[4];
		statsSlots[0] = Instantiate(prefabSlotRed, Vector3.zero, Quaternion.identity) as GameObject;
		statsSlots[1] = Instantiate(prefabSlotGreen, Vector3.zero, Quaternion.identity) as GameObject;
		statsSlots[2] = Instantiate(prefabSlotBlue, Vector3.zero, Quaternion.identity) as GameObject;
		statsSlots[3] = Instantiate(prefabSlotYellow, Vector3.zero, Quaternion.identity) as GameObject;

//		foreach(GameObject go in statsSlots)
//		{
//			go.transform.SetParent(StatsPanel.transform);
//			go.transform.localScale = Vector3.one;				//scaling fix!
//		}

		//Rebuild();
	}

//	public void Rebuild()
//	{
//
//		foreach(GameObject go in statsSlots)
//		{
//			go.transform.localScale = Vector3.one;
//			LayoutRebuilder.MarkLayoutForRebuild (go.transform as RectTransform);
//		}
//
//		LayoutRebuilder.MarkLayoutForRebuild (StatsPanel.transform as RectTransform);
//		Debug.Log("rebuild UI request");
//	}

//	void Start()
//	{
//
//	}

	void AddStatsSlot()
	{

	}

	public void RemovePlayer(NetworkPlayer player)
	{
		GameObject tempStats;
		if(dictionary.TryGetValue(player, out tempStats))
		{
			Destroy(tempStats);
			dictionary.Remove(player);
		}

	}

	/**
	 * wird auch ohne NetworkView ausgeführt!!!!
	 **/
	void OnPlayerDisconnected(NetworkPlayer disconnectedNetworkPlayer)
	{
		RemovePlayer(disconnectedNetworkPlayer);
	}

	/// <summary>
	/// Updates the server dictionary.
	/// </summary>
	void UpdateServerDictionary()
	{

		// check if new player connected
		// check if player disconnected <- OnPlayerDisconnect (Only on Server)

		foreach(NetworkPlayer player in Network.connections)
		{
			if(!PlayerHasStats(player))
			{
				int randomColor = Random.Range(0,statsSlots.Length);
				GameObject statsSlot = Instantiate(statsSlots[randomColor].gameObject, Vector3.zero, Quaternion.identity) as GameObject;
				statsSlot.transform.SetParent(StatsPanel.transform,false);
				//statsSlot.transform.localScale = Vector3.one;
				

				dictionary.Add(player, statsSlot);
			}
		}
	}

	bool PlayerHasStats(NetworkPlayer player)
	{
		return dictionary.ContainsKey(player);
	}

	/*
	 * Problem Clientseitig:
	 * 			p2p besteht nur mit server!
	 * 			keine verbindung zu anderen clients... pings nicht ohne übertragung einsehbar
	 * 			Network.connections[] ... enthält nur verbindung zum Server
	 * 
	 * Serverseitig (klar)
	 * 			Network.connections[] ... enthält Verbindungen zu allen Clients
	 * 
	 */


	void Update()
	{
		if(Network.peerType == NetworkPeerType.Disconnected)
			return;

		GameObject[] playerCharacters = GameObject.FindGameObjectsWithTag(Tags.tag_player);
//		
//		if(playerCharacters.Length > 0)
//			Debug.Log("Player Characters found:" + playerCharacters[0].name);

		/**
		 * 		Server
		 **/

		if(Network.isServer)
		{
			UpdateServerDictionary();
			/**
			 * Problem, Network.connections ändert sich wenn Client connected/disconnected!
			 * 
			 * StatsArray max. 4 Plätze - fix
			 * 
			 * OnGUI - dynamisch, Elemente werden jedesmal neu erstellt/gezeichnet
			 * 
			 * New GUI: Elemente werden instaziert -> Referenz mit Verbindung zu Network.connections (Spieler) benötigt
			 * Dictionary playerStats<Key:NetworkPlayer, Value:GUIgo>
			 * 
			 **/
			NetworkPlayer[] clients = Network.connections;
			foreach(NetworkPlayer currentClient in clients)
			{
				GameObject canvasSlot;
				PlayerStatsSlotScript currentSlot;
				if(dictionary.TryGetValue(currentClient, out canvasSlot))
				{
					currentSlot = canvasSlot.GetComponent<PlayerStatsSlotScript>();

					currentSlot.slotName.text = currentClient.ipAddress;
					currentSlot.slotWho.text = "Client";
					//currentSlot.slotName.text = "Client";
					currentSlot.lastPing.text = Network.GetLastPing(currentClient).ToString();
					currentSlot.avgPing.text = Network.GetAveragePing(currentClient).ToString();
	//				GUILayout.BeginVertical();
	//				GUILayout.Box(currentClient.ipAddress + " last Ping: " + Network.GetLastPing(currentClient));
	//				GUILayout.Box(currentClient.ipAddress + " avg Ping: " + Network.GetAveragePing(currentClient));
					for(int i=0; i< playerCharacters.Length; i++)
					{
						if(playerCharacters[i].GetComponent<RealOwner>().owner == currentClient)
						{
							NetworkedPlayer netScript = playerCharacters[i].GetComponent<NetworkedPlayer>();

							currentSlot.exinterpCount.text = netScript.correctPositionCount.ToString();
							currentSlot.droppedCount.text = netScript.avgPositionDifference.ToString();

	//						GUILayout.Box(currentClient.ipAddress + " correctPos send: " + netScript.serverCorrectsClientPositionCount);
	//						GUILayout.Box(currentClient.ipAddress + " lastPosDiff: " + netScript.lastPositionDifference);
	//						GUILayout.Box(currentClient.ipAddress + " avgPosDiff: " + netScript.avgPositionDifference);
							break;
						}
					}
	//				GUILayout.EndVertical();
				}
			}
			return;
		}

		/**
		 * 		Clients only (work with playerCharacters, no connection to other Clients available)
		 **/

		//Debug.Log("Player found: " + playerCharacters.Length);

		foreach(GameObject go in playerCharacters)
		{
			if(go != null)
			{
				RealOwner currentOwnerScript = go.GetComponent<RealOwner>();
				string name;

				if(currentOwnerScript == null)
				{
					Debug.Log(go.name + " has no OwnerScriptComponent!"); 
				}

				if(currentOwnerScript.owner == null)
				{
					Debug.Log(go.name + " has no Owner set!"); 
				}

				if(Network.player == null)
				{
					Debug.Log ("Network.player = NULL!!!");
				}

				GameObject canvasSlot;

				if(!PlayerHasStats(currentOwnerScript.owner))
				{
					// Spieler hat kein Canvas, erzeugen und in dictionary eintragen
					int randomColor = Random.Range(0,statsSlots.Length);
					GameObject newStatsSlot = Instantiate(statsSlots[randomColor].gameObject, Vector3.zero, Quaternion.identity) as GameObject;
					newStatsSlot.transform.SetParent(StatsPanel.transform);
					newStatsSlot.transform.localScale = Vector3.one;
					dictionary.Add(currentOwnerScript.owner, newStatsSlot);
				}

				// Spieler hat StatsCanvas
				if(!dictionary.TryGetValue(currentOwnerScript.owner, out canvasSlot))
				{
					Debug.Log(this.name + " problem with TryGetValue in dictionary ... skipping playercharacter");
					continue;	// next foreach element, problems with current player
				}


				PlayerStatsSlotScript currentSlot = canvasSlot.GetComponent<PlayerStatsSlotScript>();

				if(currentOwnerScript.owner != Network.player)
				{
					// not local Players character

					if(PlayerHasStats(currentOwnerScript.owner))
					{
						if(currentOwnerScript.owner == Network.connections[0])
						{
							currentSlot.slotWho.text = "Server";
						}
						else
						{
							currentSlot.slotWho.text = "other Client";
						}
						currentSlot.slotName.text = currentOwnerScript.owner.ipAddress;

						NetworkedPlayer netScript = go.GetComponent<NetworkedPlayer>();
						
						if(netScript.extrapolation)
						{
							currentSlot.exinterpCount.text = netScript.extrapolationCount.ToString();
						}

						if(currentOwnerScript.owner == Network.connections[0])
						{
							// server character
							currentSlot.lastPing.text = Network.GetLastPing(currentOwnerScript.owner).ToString();
							currentSlot.avgPing.text = Network.GetAveragePing(currentOwnerScript.owner).ToString();
						}

						currentSlot.droppedCount.text = netScript.olderPackageReceivedCount.ToString();
					}

					


//					GUILayout.BeginVertical();
					

//					GUILayout.Box(name + " dropped cnt: " + netScript.olderPackageReceivedCount);
//					GUILayout.EndVertical();
				}
				else
				{
					name = "my Client";
					currentSlot.slotName.text = Network.player.externalIP;
					currentSlot.slotWho.text = "my Client";
					// character gehört local player
					
					NetworkedPlayer netScript = go.GetComponent<NetworkedPlayer>();
					if(true)	// netScript.correctPosition ... has to be true	(authorative movement), client would be unsync!
					{
						//TODO unterscheiden zwischen correctPositionCount und Exinterp
						currentSlot.exinterpCount.text = netScript.correctPositionCount.ToString();
						currentSlot.droppedCount.text = netScript.olderPackageReceivedCount.ToString();
					}
				}
			
			}
		}
	}

	/*
	 * Problem Clientseitig:
	 * 			p2p besteht nur mit server!
	 * 			keine verbindung zu anderen clients... pings nicht ohne übertragung einsehbar
	 * 			Network.connections[] ... enthält nur verbindung zum Server
	 * 
	 * Serverseitig (klar)
	 * 			Network.connections[] ... enthält Verbindungen zu allen Clients
	 * 
	 */

	/// <summary>
	/// OLD
	/// </summary>
	void OFFOnGUI12()
	{
		if(Network.peerType == NetworkPeerType.Disconnected)
			return;

		GameObject[] playerCharacters = GameObject.FindGameObjectsWithTag(Tags.tag_player);

		if(playerCharacters.Length > 0)
			Debug.Log("Player Characters found:" + playerCharacters[0].name);

		if(Network.isServer)
		{
			GUILayout.BeginArea(new Rect(10f, 100f, Screen.width-20f, 300f ));
			GUILayout.BeginHorizontal();
			
			NetworkPlayer[] clients = Network.connections;
			foreach(NetworkPlayer currentClient in clients)
			{
				GUILayout.BeginVertical();
				GUILayout.Box(currentClient.ipAddress + " last Ping: " + Network.GetLastPing(currentClient));
				GUILayout.Box(currentClient.ipAddress + " avg Ping: " + Network.GetAveragePing(currentClient));

				for(int i=0; i< playerCharacters.Length; i++)
				{
					if(playerCharacters[i].GetComponent<RealOwner>().owner == currentClient)
					{
						NetworkedPlayer netScript = playerCharacters[i].GetComponent<NetworkedPlayer>();
						GUILayout.Box(currentClient.ipAddress + " correctPos send: " + netScript.serverCorrectsClientPositionCount);
						GUILayout.Box(currentClient.ipAddress + " lastPosDiff: " + netScript.lastPositionDifference);
						GUILayout.Box(currentClient.ipAddress + " avgPosDiff: " + netScript.avgPositionDifference);
						break;
					}
				}

				//				GUILayout.Box(currentClient.ipAddress + " exinterp cnt: " + extrapolationCount);

				GUILayout.EndVertical();
			}
			
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			
			return;
		}
		
//		if(Network.isClient)
//		{
//			GUILayout.BeginArea(new Rect(10f, 100f, 250f, 80f ));
//			GUILayout.BeginHorizontal();
//			
//			NetworkPlayer[] clients = Network.connections;
//			foreach(NetworkPlayer currentClient in clients)
//			{
//				GUILayout.BeginVertical();
////				GUILayout.Box(currentClient.ipAddress + " exinterp cnt: " + extrapolationCount);
//				GUILayout.Box(currentClient.ipAddress + " last Ping: " + Network.GetLastPing(currentClient));
//				GUILayout.Box(currentClient.ipAddress + " avg Ping: " + Network.GetAveragePing(currentClient));
//				GUILayout.EndVertical();
//			}
//			
//			GUILayout.EndHorizontal();
//			GUILayout.EndArea();
//			
//			return;
//		}

		/**
		 * 		Clients only
		 **/

		GUILayout.BeginArea(new Rect(10f, 100f, Screen.width-20f, 80f ));
		GUILayout.BeginHorizontal();
		foreach(GameObject go in playerCharacters)
		{
			if(go != null)
			{
				RealOwner currentOwnerScript = go.GetComponent<RealOwner>();
				string name;
				if(currentOwnerScript.owner != Network.player)
				{
					if(currentOwnerScript.owner == Network.connections[0])
					{
						name = "Server";
					}
					else
					{
						name = "other Client";
					}
					GUILayout.BeginVertical();
					
					NetworkedPlayer netScript = go.GetComponent<NetworkedPlayer>();
					if(netScript.extrapolation)
					{
						GUILayout.Box(name + " exinterp cnt: " + netScript.extrapolationCount);
					}
					if(currentOwnerScript.owner == Network.connections[0])
					{
						// server character
						GUILayout.Box(name + " last Ping: " + Network.GetLastPing(currentOwnerScript.owner));
						GUILayout.Box(name + " avg Ping: " + Network.GetAveragePing(currentOwnerScript.owner));
					}
					GUILayout.Box(name + " dropped cnt: " + netScript.olderPackageReceivedCount);
					GUILayout.EndVertical();
				}
				else
				{
					name = "my Client";
					// character gehört local player
					GUILayout.BeginVertical();
					
					NetworkedPlayer netScript = go.GetComponent<NetworkedPlayer>();
					if(true)	// netScript.correctPosition ... has to be true	(authorative movement), client would be unsync!
					{
						GUILayout.Box(name + " correction cnt: " + netScript.correctPositionCount);
					}
					GUILayout.Box(name + " dropped cnt: " + netScript.olderPackageReceivedCount);

					GUILayout.EndVertical();
				}
			}
			
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		
		//		if(extrapolation)
		//		{
		//			GUI.Box(new Rect(5f,Screen.height-100f,150f,20f), "Extrapolation: " + extrapolationCount);
		//		}
	}
}
