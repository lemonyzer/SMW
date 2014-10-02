using UnityEngine;
using System.Collections;

public class ConnectionStats : MonoBehaviour {

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
	void OnGUI()
	{
		if(Network.peerType == NetworkPeerType.Disconnected)
			return;

		GameObject[] playerCharacters = GameObject.FindGameObjectsWithTag(Tags.player);

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
