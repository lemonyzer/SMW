using UnityEngine;
using System.Collections;

public class HeadlessServer : MonoBehaviour {
	// Use this for initialization
	void Start () {
		if(Application.loadedLevelName == Scenes.mainmenu)
		{
			Application.LoadLevel(Scenes.unityNetworkConnectLobby);
		}
		else if(Application.loadedLevelName == Scenes.unityNetworkConnectLobby)
		{
			this.GetComponent<UnityNetworkConnectMenu>().StartHeadlessServer();
		}
	}
}
