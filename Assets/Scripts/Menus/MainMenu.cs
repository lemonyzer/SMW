using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	// GUI
	private float halfScreenWidth;
	private float halfScreenHeight;
	private float btnCount;
	private float btnSpacing;
	private float btnHeight;

	// Settings
	private string nextLevel;


	void Awake()
	{
		halfScreenHeight = Screen.height*0.5f;
		halfScreenWidth = Screen.width*0.5f;
		nextLevel = "";
	}

	void OnGUI() {

		// links oben

		if(GUI.Button(new Rect(0,0,halfScreenWidth,halfScreenHeight),"Single Player"))
		{
			nextLevel = Scenes.SingleplayerMenu;
		}


		// links unten
		GUI.enabled = false;
		if(GUI.Button(new Rect(0,halfScreenHeight,halfScreenWidth,halfScreenHeight),"Online\n(PUN)"))
		{
			nextLevel = Scenes.photonLobby;
		}
		GUI.enabled = false;

		// rechts oben
		GUI.enabled = true;
		if(GUI.Button(new Rect(halfScreenWidth,0,halfScreenWidth,halfScreenHeight),"LAN/WLAN (UN, p2p)"))
		{
			nextLevel = Scenes.unityNetworkLobby;

		}
		GUI.enabled = true;

		// rechts unten
		GUI.enabled = false;
		if(GUI.Button(new Rect(halfScreenWidth,halfScreenHeight,halfScreenWidth*0.5f,halfScreenHeight),"Other"))
		{
			nextLevel = "hostclientmenu";
		}
		GUI.enabled = true;
		if(GUI.Button(new Rect(halfScreenWidth+halfScreenWidth*0.5f,halfScreenHeight,halfScreenWidth*0.5f,halfScreenHeight),"Race"))
		{
			nextLevel = "UnityNetworkRace";
		}
		GUI.enabled = true;

		if(GUI.changed)
		{
			if(nextLevel != "")
			{
				OnGUIChanged();
			}
		}
		
	}

	void OnGUIChanged()
	{
		Application.LoadLevel(nextLevel);
	}
}
