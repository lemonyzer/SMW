using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConnectOrHost : MonoBehaviour {

	RaceCam raceCam;


	// Use this for initialization
	void Start () {
		raceCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<RaceCam>();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	Rect area = new Rect(0,Screen.height*0.5f,Screen.width,Screen.height*0.5f);

	void OnGUI()
	{
		if(GUILayout.Button("Auto Cam", GUILayout.MinHeight(40f)))
		{
			raceCam.target = null;
		}
		if(GUILayout.Button("Follow Player", GUILayout.MinHeight(40f)))
		{
			raceCam.target = GameObject.FindGameObjectWithTag(Tags.tag_player);
		}
		if(Network.peerType == NetworkPeerType.Disconnected)
		{
			Disconnected();
			GUILayout.BeginArea(area);
			if(GUILayout.Button("Start Server (20 Client Slots)", GUILayout.Width(area.width), GUILayout.Height(area.height)))
			{
				Network.InitializeServer( 20, 25005, true );
			}
			GUILayout.EndArea();
		}
	}

	public int clientSlots = 10;
	
	private string ip = "tofast.ddns.net";
	private int port = 25005;
	
	private string ipPC = "192.168.0.129";
	private string ipMWhite = "192.168.0.113";
	private string ipMBlack = "192.168.0.146";
	
	List<Host> hostList;
	
	class Host
	{
		public string name;
		public string ip;
		
		public Host(string name, string ip)
		{
			this.name = name;
			this.ip = ip;
		}
	}
	
	void Awake()
	{
		hostList = new List<Host>();
		hostList.Add(new Host("PC", ipPC));
		hostList.Add(new Host("MWhite", ipMWhite));
		hostList.Add(new Host("MBlack", ipMBlack));
		
		InitGUIStyle ();
	}
	
	/**
	 * GUIStyle
	 **/
	GUIStyle buttonStyle;
	float minButtonHeight;
	
	void InitGUIStyle()
	{
		buttonStyle = new GUIStyle ();
		buttonStyle.stretchWidth = false;
		
		if(Screen.dpi != 0)
		{
			minButtonHeight = 20f * Screen.height / Screen.dpi;
		}
		else
		{
			minButtonHeight = 20f;
		}
	}
	
	void Disconnected()
	{
		// let the user enter IP address
		GUILayout.Label( "IP Address" );
		ip = GUILayout.TextField( ip, GUILayout.Width( 200f ), GUILayout.MinHeight(minButtonHeight) );
		
		GUILayout.BeginHorizontal();
		// let the user enter port number
		// port is an integer, so only numbers are allowed
		GUILayout.Label( "Port" );
		string port_str = GUILayout.TextField( port.ToString(), GUILayout.Width( 70f ), GUILayout.MinHeight(minButtonHeight) );
		int port_num = port;
		if( int.TryParse( port_str, out port_num ) )
			port = port_num;
		
		// connect to the IP and port
		if( GUILayout.Button( "Connect", GUILayout.Width( 100f ), GUILayout.MinHeight(minButtonHeight) ) )
		{
			Network.Connect( ip, port );
		}
		GUILayout.EndHorizontal();
		
		// host a server on the given port, only allow 3 incoming connection (3 other players)
		if( GUILayout.Button( "Host", GUILayout.Width( 100f ), GUILayout.MinHeight(minButtonHeight) ) )
		{
			Network.InitializeServer( clientSlots, port, true );
		}
		
		GUILayout.BeginArea(new Rect(Screen.width * 0.5f, 0, Screen.width * 0.5f, Screen.height));
		GUILayout.Label("Servers");
		foreach(Host host in hostList)
		{
			if( GUILayout.Button( host.ip + " " + host.name,  GUILayout.MinHeight(minButtonHeight) ) )
			{
				Network.Connect( host.ip, port );
			}
		}
		GUILayout.EndArea();

	}
	
//	void OnConnectedToServer()
//	{
//		Debug.Log( "Connected to server" );
//		// this is the NetworkLevelLoader we wrote earlier in the chapter – pauses the network, loads the level, waits for the level to finish, and then unpauses the network
//		NetworkLevelLoader.Instance.LoadLevel( Scenes.unityNetworkGameRoom );
//	}
//	
//	void OnServerInitialized()
//	{
//		Debug.Log( "Server initialized" );
//		NetworkLevelLoader.Instance.LoadLevel( Scenes.unityNetworkGameRoom );
//	}
}
