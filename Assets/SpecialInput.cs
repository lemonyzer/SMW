using UnityEngine;
using System.Collections;

public class SpecialInput : MonoBehaviour {

	//TODO events (dont need to spam update)
	//TODO is WRONG?? Update needed to check Key!
	
	// Update is called once per frame
	void Update () {
	
		if( Input.GetKey(KeyCode.Menu) )
		{
			
		}
		else if( Input.GetKey(KeyCode.Escape) )
		{
			if(Network.peerType == NetworkPeerType.Client)
			{
				Network.Disconnect();
			}
			else if(Network.peerType == NetworkPeerType.Server)
			{
//				foreach(NetworkPlayer netPlayer in Network.connections)
//				{
//					Network.CloseConnection(netPlayer, true);
//				}
				Network.Disconnect();
				MasterServer.UnregisterHost();
			}
			Application.LoadLevel(Scenes.mainmenu);
		}

	}
}
