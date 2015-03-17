using UnityEngine;
using System.Collections;

public class SpecialInput : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
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
				foreach(NetworkPlayer netPlayer in Network.connections)
				{
					Network.CloseConnection(netPlayer, true);
				}
			}
			Application.LoadLevel(Scenes.mainmenu);
		}

	}
}
