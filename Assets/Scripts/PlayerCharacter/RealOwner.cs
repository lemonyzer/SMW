using UnityEngine;
using System.Collections;

public class RealOwner : MonoBehaviour {

	public NetworkPlayer owner;
	public Behaviour characterControls;

	void Awake()
	{
		this.enabled = false;
		characterControls = GetComponent<PlatformUserControl>();
	}

	[RPC]
	void SetCharacterControlsOwner(NetworkPlayer player)
	{
		owner = player;
		if (player == Network.player)
		{
			//Hey thats us! We can control this player: enable this script (this enables Update());
			characterControls.enabled = true;

			if(Application.loadedLevelName == "UnityNetworkRace")
			{
				Camera.main.GetComponent<RaceCam>().target = this.gameObject;
				Camera.main.GetComponent<RaceCam>().holdStartPos = false;
				Debug.Log("following " + this.gameObject.name);
				//Camera.main.GetComponent<RaceCam>().target = this.gameObject;
			}
		}
	}
	
}
