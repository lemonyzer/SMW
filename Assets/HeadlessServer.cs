using UnityEngine;
using System.Collections;

public class HeadlessServer : MonoBehaviour {
	// Use this for initialization
	void Start () {
		if(Application.loadedLevelName == Scenes.mainmenu)
		{
			Application.LoadLevel(Scenes.unityNetworkLobby);
		}
		else if(Application.loadedLevelName == Scenes.unityNetworkLobby)
		{
			this.GetComponent<ConnectToGame>().StartHeadlessServer();
		}
	}
}
