using UnityEngine;
using System.Collections;

public class SpecialInput : MonoBehaviour {

	//TODO events (dont need to spam update)
	//TODO is WRONG?? Update needed to check Key!

	int count = 0;
	float maxTimeBetween = 1f;
	float minTimeBetween = 0.2f;
	float pastTime = 0f;

	
	// Update is called once per frame
	void Update () {
	
		if( Input.GetKey(KeyCode.Menu) )
		{
			
		}
		else if( Input.GetKey(KeyCode.Escape) )
		{

			if(count == 0)
			{
				pastTime = Time.time;
				count++;
			}
			else if( count > 0)
			{
				if(Time.time - pastTime < minTimeBetween)
				{
					return;
				}
				else if( Time.time - pastTime > minTimeBetween )
				{
					if(Time.time - pastTime < maxTimeBetween )
					{
						count++;
						LoadMainMenu();
					}
					else
					{
						pastTime = Time.time;
						count = 1;
					}
				}
			}
		}

	}

	void LoadMainMenu()
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
