using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class ConnectToGame : MonoBehaviour
{

	string nextScene = Scenes.unityNetworkCharacterSelection;

	public Text textIP;
	public Text textExternalIP;
	public Text textNatStatus;

	public Toggle toggleNat;
	public GameObject NatResultPanel;
	public Text textNatTestStatus;
	public Text textNatTestResult;
	
	public Button btnNAT;
	public Button btnUPnPMapping;

	public InputField inputServerAdress;

	private string registeredGameName = "smw";
	private string registeredGameType = "smw_alpha_0.1";
	private string registeredGameComment = "classic";

	private string testStatus = "Testing network connection capabilities.";
	private string testMessage = "Test in progress";
	private string shouldEnableNatMessage;
	private bool doneTesting = false;
	private bool probingPublicIP = false;
	private int serverPort = 25005;
	private ConnectionTesterStatus connectionTestResult = ConnectionTesterStatus.Undetermined;
	private float timer = 0;
	// Indicates if the useNat parameter be enabled when starting a server
	private bool useNat = false;

	public int clientSlots = 10;

	private string ip = "tofast.ddns.net";
	private int port = 25005;


	// masterserver host list

	// are we currently trying to download a host list?
	bool loadingHostList = false;
	// the current position within the scrollview
	private Vector2 scrollPos = Vector2.zero;


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

		//UPnPTest();

		Network.Connect("www.google.com");
		myIP = Network.player.ipAddress;
		myExternalIP = Network.player.externalIP;
		Network.Disconnect();

		Debug.Log(myIP);
		Debug.Log(myExternalIP);


//		if(Network.HavePublicAddress())
//		{
//			myExternalIP = Network.player.externalIP;
//		}
//		else
//		{
//			myExternalIP = "unknown";
//		}
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

	public string upnpStatus = "Test not started";

//	void UPnPTest()
//	{
//		upnpStatus = "Test running";
//        if(NAT.DiscoverWithTryCatch())
//		{
//			upnpStatus = "You have an UPnP-enabled router and your IP is: "+NAT.GetExternalIP();
//		}
//		else
//			upnpStatus = "You do not have an UPnP-enabled router.";
//    }

	bool currentTestRunning = false;

	void Start()
	{
		textIP.text = "IP: " +myIP;
		textExternalIP.text = "External IP: " + myExternalIP;
//		UPnPPortMapping();
		// immediately request a list of hosts
		refreshHostList();
	}

	public void refreshHostList()
	{
		// clear current connect buttons
		Transform contentPanel = ScrollViewServerlistContentPanel.transform;
		for(int i=contentPanel.childCount-1; i >=0; i--)
		{
			Destroy(contentPanel.GetChild(i).gameObject);
		}

		// let the user know we are awaiting results from the master server
		loadingHostList = true;
		MasterServer.ClearHostList();
		MasterServer.RequestHostList( registeredGameType );
	}
	
	// this is called when the Master Server reports an event to the client – for example, server registered successfully, host list received, etc
	void OnMasterServerEvent( MasterServerEvent msevent )
	{
		if( msevent == MasterServerEvent.HostListReceived )
		{
			// received the host list, no longer awaiting results
			loadingHostList = false;

			HostData[] hosts = MasterServer.PollHostList();
			for( int i = 0; i < hosts.Length; i++ )
			{
				string tmpIp = "";
				int x = 0;
				while (x < hosts[i].ip.Length) {
					tmpIp = hosts[i].ip[x] + " ";
					x++;
				}

				GameObject tempBtn = (GameObject) Instantiate(prefabButtonConnectToHost);
				tempBtn.transform.SetParent(ScrollViewServerlistContentPanel.transform,false);
				string serverString = "";
				ConnectButtonScript tmpBtnScript = tempBtn.GetComponent<ConnectButtonScript>();
				tmpBtnScript.hostip = tmpIp;
				tmpBtnScript.hostport = hosts[i].port;
				tmpBtnScript.useNat = hosts[i].useNat;
				serverString += hosts[i].gameName + ", " + tmpIp + ":";
				serverString += hosts[i].port + "\nYou need NAT: ";
				serverString += hosts[i].useNat.ToString() + " ";
				//serverString += hosts[i].guid + " ";

				tempBtn.transform.FindChild("Serverinfo Label").GetComponent<Text>().text = serverString;
			}
		}
	}

	public GameObject prefabButtonConnectToHost;
	public GameObject ScrollViewServerlistContentPanel;


	void Update_Serverlist12312()
	{
		if( loadingHostList )
		{
			// destroy all elemets

			// add one element says loading...
			GUILayout.Label( "Loading..." );
		}
		else
		{
			scrollPos = GUILayout.BeginScrollView( scrollPos, GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) );
			
			HostData[] hosts = MasterServer.PollHostList();
			for( int i = 0; i < hosts.Length; i++ )
			{
				string tmpIp = "";
				int x = 0;
				while (x < hosts[i].ip.Length) {
					tmpIp = hosts[i].ip[x] + " ";
					x++;
				}

				if( GUILayout.Button( hosts[i].gameName + " " + tmpIp + ":" + hosts[i].port + " useNAT:" + hosts[i].useNat.ToString(), GUILayout.ExpandWidth( true ), GUILayout.MinHeight(minButtonHeight) ) )
				{
//					if(tmpIp == myExternalIP)
//					{
//						//myExtrenalIP kann fälschlischerweiße die Interne sein!!! 
//						//vorher prüfen!
//						Debug.LogWarning("same IP");
//						// host kann hinter gleichem router sitzen und ist vielleicht direkt erreichbar
//						if(useNatToConnect)
//						{
//							//connect over nat punchthrough/hairpinning/relay
//							Network.Connect( hosts[i].guid );
//						}
//						else
//						{
//							//ignoriere, connect over hairpinning
//							Debug.Log("same ip "+ tmpIp +"! connect over hairpinning");
//							Network.Connect( tmpIp );
//						}
//					}
//					else
//					{
						if(useNatToConnect)
						{
							Debug.Log("Connecting to " + hosts[i].guid + " with help of NAT punchthrough" );
							Network.Connect( hosts[i].guid );
						}
						else if(false)
						{
							Network.Connect( hosts[i] );
						}
						else
						{
							Debug.Log("Connecting to " + tmpIp + " no NAT punchthrough" );
							Network.Connect( tmpIp, hosts[i].port );
						}
//					}
				}
			}
			
			if( hosts.Length == 0 )
			{
				GUILayout.Label( "No servers running" );
			}
		}
	}

	bool useNatToConnect = true;

//	void OnGUI()
//	{
////		if(Network.peerType == NetworkPeerType.Client ||
////		   Network.peerType == NetworkPeerType.Server)
////			return;
//		GUILayout.Label("IP: " + myIP);
//		GUILayout.Label("External IP: " + myExternalIP);
//		if(upnp != null)
//		{
//			GUILayout.Label("UPnP Portmapping Status: " + upnp.status);
//
//
//			if(upnp.status == TNet.UPnP.Status.Searching)
//			{
//				GUILayout.Label("Gateway IP: searching");
//			}
//			else
//			{
//				GUILayout.Label("Gateway IP: " + upnp.gatewayAddress.ToString());
//			}
//		}
//
//		if(doneTesting)
//		{
//			GUILayout.Label("useNat Status: " + useNat);
//		}
//		else
//		{
//			GUILayout.Label("useNat Status: unknown!");
//		}
//
//		GUILayout.Label("Current Status: " + testStatus);
//		GUILayout.Label("Test result : " + testMessage);
//		GUILayout.Label(shouldEnableNatMessage);
//
//
////		if (!doneTesting)
////		{
//		if(currentTestRunning)
//		{
//			TestConnection();
//		}
//		else
//		{
//		}
//		//			return;
////		}
//		GUILayout.BeginHorizontal();
//		if(upnp != null)
//		{
//			if(upnpActive)
//			{
//				GUI.enabled = false;
//			}
//			else
//			{
//				GUI.enabled = true;
//			}
//		}
//		if( GUILayout.Button( "UPnP Portmapping", GUILayout.Width( 125f ), GUILayout.MinHeight(minButtonHeight) ) )
//		{
//			StartCoroutine(UPnPPortMapping());
//		}
//		GUI.enabled = !currentTestRunning;
//		if( GUILayout.Button( "NAT Test", GUILayout.Width( 75f ), GUILayout.MinHeight(minButtonHeight) ) )
//		{
//			if(!doneTesting)
//			{
//				// test lief noch nicht
////				Network.InitializeServer( clientSlots, port, useNat );
////				MasterServer.RegisterHost(registeredGameType, registeredGameName, registeredGameComment);
//
//				TestConnection();
//				currentTestRunning = true;
//				currentTestFinished = false;
//				myIP = Network.player.ipAddress;
//                myExternalIP = Network.player.externalIP;
//			}
//			else
//			{
//				// test wurde schon einmal gestartet -> server ist bereits initialisiert!
//				doneTesting = false;
//				forceTest = true;
//				TestConnection();
//				forceTest = false;
//				currentTestRunning = true;
//			}
//			
//			//			You can call RegisterHost more than once while a server is running 
//			//			to update the information stored on the Master Server. For example, 
//			//			if the server changes to a new level, you might call RegisterHost 
//			//			again to update the lobby.
//		}
//		GUILayout.EndHorizontal();
//		GUILayout.BeginHorizontal();
//		GUI.enabled = true;
//		// host a server on the given port, only allow 3 incoming connection (3 other players)
//		if( GUILayout.Button( "Host offline", GUILayout.Width( 100f ), GUILayout.MinHeight(minButtonHeight) ) )
//		{
//			Network.Disconnect();
//			hosting = true;
//			Network.InitializeServer( clientSlots, port, useNat );
//			
//			//			You can call RegisterHost more than once while a server is running 
//			//			to update the information stored on the Master Server. For example, 
//			//			if the server changes to a new level, you might call RegisterHost 
//			//			again to update the lobby.
//		}
//		GUI.enabled = doneTesting;
//		if( GUILayout.Button( "Host online", GUILayout.Width( 100f ), GUILayout.MinHeight(minButtonHeight) ) )
//		{
//			Network.Disconnect();
//			hosting = true;
//			Network.InitializeServer( clientSlots, port, useNat );
//			//MasterServer.RegisterHost(registeredGameType, registeredGameName + " " + port.ToString() + " NAT:" + useNat.ToString() , registeredGameComment);
//			MasterServer.RegisterHost(registeredGameType, registeredGameName + " useNat=" + useNat, registeredGameComment);
//
////			You can call RegisterHost more than once while a server is running 
////			to update the information stored on the Master Server. For example, 
////			if the server changes to a new level, you might call RegisterHost 
////			again to update the lobby.
//		}
//		GUILayout.EndHorizontal();
//		GUI.enabled = true;
//
//		GUILayout.BeginArea(new Rect(Screen.width * 0.5f, 0, Screen.width * 0.5f, Screen.height));
//
//		// let the user enter IP address
//		GUILayout.Label( "Manually connecting" );
//		GUILayout.Label( "Server Address (IP/Hostname)" );
//		ip = GUILayout.TextField( ip, GUILayout.Width( 200f ), GUILayout.MinHeight(minButtonHeight) );
//		
//		GUILayout.BeginHorizontal();
//		// let the user enter port number
//		// port is an integer, so only numbers are allowed
//		GUILayout.Label( "Port", GUILayout.Width( 30f ) );
//		string port_str = GUILayout.TextField( port.ToString(), GUILayout.Width( 70f ), GUILayout.MinHeight(minButtonHeight) );
//		int port_num = port;
//		if( int.TryParse( port_str, out port_num ) )
//			port = port_num;
//		
//		// connect to the IP and port
//		if( GUILayout.Button( "Connect", GUILayout.Width( 100f ), GUILayout.MinHeight(minButtonHeight) ) )
//		{
//			Network.Connect( ip, port );
//		}
//		GUILayout.EndHorizontal();
//
//		GUILayout.Label("Servers found");
//
//		useNatToConnect = GUILayout.Toggle (useNatToConnect, "Use NAT punchthrough and connect with MasterServer help");
//
//		if( GUILayout.Button( "Refresh", GUILayout.MinHeight(minButtonHeight) ) )
//		{
//			refreshHostList();
//		}
//		
//		if( loadingHostList )
//		{
//			GUILayout.Label( "Loading..." );
//		}
//		else
//		{
//			scrollPos = GUILayout.BeginScrollView( scrollPos, GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) );
//			
//			HostData[] hosts = MasterServer.PollHostList();
//			for( int i = 0; i < hosts.Length; i++ )
//			{
//				string tmpIp = "";
//				int x = 0;
//				while (x < hosts[i].ip.Length) {
//					tmpIp = hosts[i].ip[x] + " ";
//					x++;
//				}
//
//				if( GUILayout.Button( hosts[i].gameName + " " + tmpIp + ":" + hosts[i].port + " useNAT:" + hosts[i].useNat.ToString(), GUILayout.ExpandWidth( true ), GUILayout.MinHeight(minButtonHeight) ) )
//				{
////					if(tmpIp == myExternalIP)
////					{
////						//myExtrenalIP kann fälschlischerweiße die Interne sein!!! 
////						//vorher prüfen!
////						Debug.LogWarning("same IP");
////						// host kann hinter gleichem router sitzen und ist vielleicht direkt erreichbar
////						if(useNatToConnect)
////						{
////							//connect over nat punchthrough/hairpinning/relay
////							Network.Connect( hosts[i].guid );
////						}
////						else
////						{
////							//ignoriere, connect over hairpinning
////							Debug.Log("same ip "+ tmpIp +"! connect over hairpinning");
////							Network.Connect( tmpIp );
////						}
////					}
////					else
////					{
//						if(useNatToConnect)
//						{
//							Debug.Log("Connecting to " + hosts[i].guid + " with help of NAT punchthrough" );
//							Network.Connect( hosts[i].guid );
//						}
//						else if(false)
//						{
//							Network.Connect( hosts[i] );
//						}
//						else
//						{
//							Debug.Log("Connecting to " + tmpIp + " no NAT punchthrough" );
//							Network.Connect( tmpIp, hosts[i].port );
//						}
////					}
//				}
//			}
//			
//			if( hosts.Length == 0 )
//			{
//				GUILayout.Label( "No servers running" );
//			}
//			
//			GUILayout.EndScrollView();
//		}
//
////		foreach(Host host in hostList)
////		{
////			if( GUILayout.Button( host.ip + " " + host.name,  GUILayout.MinHeight(minButtonHeight) ) )
////			{
////				Network.Connect( host.ip, port );
////			}
////		}
//		GUILayout.EndArea();
//
//	}

	void OnConnectedToServer()
	{
		Debug.Log( "Connected to server" );
		// this is the NetworkLevelLoader we wrote earlier in the chapter – pauses the network, loads the level, waits for the level to finish, and then unpauses the network
		//NetworkLevelLoader.Instance.LoadLevel( Scenes.unityNetworkGameRoom );
		NetworkLevelLoader.Instance.LoadLevel( nextScene );
	}

	private bool hosting = false;

	void OnServerInitialized()
	{
		if(!hosting)
		{
			// server wurde initiiert um TestConnection aufbauen zu lassen. (Test Server... keine neue Scene laden!)
			TestConnection();
			currentTestRunning = true;
			currentTestFinished = false;
			myIP = Network.player.ipAddress;
			myExternalIP = Network.player.externalIP;
//			if(Network.HavePublicAddress())
//			{
//				myExternalIP = Network.player.externalIP;
//			}
//			else
//			{
//				myExternalIP = "unknown";
//			}
			return;
		}

		Debug.Log( "Server initialized" );
		NetworkLevelLoader.Instance.LoadLevel( nextScene );
	}

	string myIP = "unknown";
	string myExternalIP = "unknown";
	bool currentTestFinished = false;

	bool forceTest = false;

	public void Connect()
	{
		//TODO
//		if(useNatToConnect)
//		{
//			Debug.Log("Connecting to " + hosts[i].guid + " with help of NAT punchthrough" );
//			Network.Connect( hosts[i].guid );
//		}
//		else
//		{
//			Debug.Log("Connecting to " + tmpIp + " no NAT punchthrough" );
//			Network.Connect( tmpIp, hosts[i].port );
//		}
		Network.Connect( inputServerAdress.text, port );
	}

	public void ConnectWithIp_Button(string host, int port)
	{
		Network.Connect( host, port );
	}

	public void ConnectWithGUID_Button(string hostGUID)
	{
		Network.Connect( hostGUID );
	}

	public InputField gameName;

	public void HostOnline()
	{
		Network.Disconnect();
		hosting = true;
		Network.InitializeServer( clientSlots, port, useNat );
		//MasterServer.RegisterHost(registeredGameType, registeredGameName + " " + port.ToString() + " NAT:" + useNat.ToString() , registeredGameComment);
		MasterServer.RegisterHost(registeredGameType, gameName.text + " useNat=" + useNat, registeredGameComment);
		
		//			You can call RegisterHost more than once while a server is running 
		//			to update the information stored on the Master Server. For example, 
		//			if the server changes to a new level, you might call RegisterHost 
		//			again to update the lobby.
	}

	public void HostOffline()
	{
		Network.Disconnect();
		hosting = true;
		Network.InitializeServer( clientSlots, port, useNat );
	}

	public void NatTest()
	{
		ShowNatResultPanel(true);
		if(!doneTesting)
		{
			// test lief noch nicht
			//				Network.InitializeServer( clientSlots, port, useNat );
			//				MasterServer.RegisterHost(registeredGameType, registeredGameName, registeredGameComment);
			
			TestConnection();
			currentTestRunning = true;
			currentTestFinished = false;
			myIP = Network.player.ipAddress;
			myExternalIP = Network.player.externalIP;
		}
		else
		{
			// test wurde schon einmal gestartet -> server ist bereits initialisiert!
			doneTesting = false;
			forceTest = true;
			TestConnection();
			forceTest = false;
			currentTestRunning = true;
		}
		
		//			You can call RegisterHost more than once while a server is running 
		//			to update the information stored on the Master Server. For example, 
		//			if the server changes to a new level, you might call RegisterHost 
		//			again to update the lobby.
	}

	void ShowNatResultPanel(bool enable)
	{
		NatResultPanel.SetActive(enable);
	}

	public Button btnNatResultOk;

	void ShowNatResultPanelButtonOk(bool enable)
	{
		btnNatResultOk.gameObject.SetActive(enable);
	}

	public void TestConnection() {
		// Start/Poll the connection test, report the results in a label and 
		// react to the results accordingly
		connectionTestResult = Network.TestConnection(forceTest);
		switch (connectionTestResult) {
		case ConnectionTesterStatus.Error: 
			testMessage = "Problem determining NAT capabilities";
			doneTesting = true;
			break;
			
		case ConnectionTesterStatus.Undetermined: 
			testMessage = "Undetermined NAT capabilities";
			doneTesting = false;
			break;
			
		case ConnectionTesterStatus.PublicIPIsConnectable:
			testMessage = "Directly connectable public IP address.";
			useNat = false;
			doneTesting = true;
			break;
			
			// This case is a bit special as we now need to check if we can 
			// circumvent the blocking by using NAT punchthrough
		case ConnectionTesterStatus.PublicIPPortBlocked:
			testMessage = "Non-connectable public IP address (port " + serverPort +" blocked), running a server is impossible.";
			useNat = false;
			// If no NAT punchthrough test has been performed on this public 
			// IP, force a test
			if (!probingPublicIP) {
				connectionTestResult = Network.TestConnectionNAT();
				probingPublicIP = true;
				testStatus = "Testing if blocked public IP can be circumvented";
				timer = Time.time + 10;
			}
			// NAT punchthrough test was performed but we still get blocked
			else if (Time.time > timer) {
				probingPublicIP = false; 		// reset
				useNat = true;
				doneTesting = true;
			}
			break;
		case ConnectionTesterStatus.PublicIPNoServerStarted:
			testMessage = "Public IP address but server not initialized,\n"+	"it must be started to check server accessibility. Restart\n"+ "connection test when ready.";
			break;
			
		case ConnectionTesterStatus.LimitedNATPunchthroughPortRestricted:
			testMessage = "Limited NAT punchthrough capabilities. Cannot\n"+ "connect to all types of NAT servers. Running a server\n"+ "is ill advised as not everyone can connect.";
			useNat = true;
			doneTesting = true;
			break;
			
		case ConnectionTesterStatus.LimitedNATPunchthroughSymmetric:
			testMessage = "Limited NAT punchthrough capabilities. Cannot\n"+ "connect to all types of NAT servers. Running a server\n"+ "is ill advised as not everyone can connect.";
			useNat = true;
			doneTesting = true;
			break;


		case ConnectionTesterStatus.NATpunchthroughAddressRestrictedCone:
			testMessage = "NAT punchthrough capable. Can connect to all\n"+ "servers and receive connections from all clients. Enabling\n"+ "NAT punchthrough functionality.";
			useNat = true;
			doneTesting = true;
			break;
		case ConnectionTesterStatus.NATpunchthroughFullCone:
			testMessage = "NAT punchthrough capable. Can connect to all\n"+ "servers and receive connections from all clients. Enabling\n"+ "NAT punchthrough functionality.";
			useNat = true;
			doneTesting = true;
			break;
			
		default: 
			testMessage = "Error in test routine, got " + connectionTestResult;
			break;
		}
		if (doneTesting) {
			currentTestRunning = false;
			if (useNat)
				shouldEnableNatMessage = "When starting a server the NAT\n"+ "punchthrough feature should be enabled (useNat parameter)";
			else
				shouldEnableNatMessage = "NAT punchthrough not needed";
			testStatus = "Done testing";
			ShowNatResultPanel(true);
			ShowNatResultPanelButtonOk(true);
			toggleNat.isOn = useNat;
		}

		textNatTestStatus.text = "Test Status: " + testStatus;
		textNatTestResult.text = testMessage;
	}

	public void UPnPMapping()
	{
		StartCoroutine(UPnPPortMapping());
	}

	bool upnpActive = false;
	TNet.UPnP upnp;
	IEnumerator UPnPPortMapping()
	{
//		if(upnpActive)
//		{
//			yield return 0;
//		}
		upnpActive = true;
		upnp = new TNet.UPnP();
		yield return new WaitForSeconds(10);
		if(upnp.status == TNet.UPnP.Status.Success)
		{
			upnp.name = registeredGameName;
			upnp.OpenUDP(port);
		}
		upnp.Close();
		upnpActive = false;
	}



	bool headlessServer = false;
	public void StartHeadlessServer()
	{
		// test connection
		//test if port is open
		
		//if port is closed
		//try opening port with upnp portmapping
		
		//verify portmapping
		//test if port is open
		
		//if port is still closed
		//check nat status
		doneTesting = false;
		headlessServer = true;
		currentTestRunning = true;
		TestConnection();
		
		//start server with useNAT variable
	}

	bool serverRunning = false;
	void Update()
	{
		if(currentTestRunning)
			TestConnection();
		else
		{
			if(Network.HavePublicAddress())
				textExternalIP.text = "External IP: " + Network.player.externalIP;
		}

		if(headlessServer)
		{
			if(serverRunning)
				return;				// dont initialize more than once!

			if(!doneTesting)
			{
				TestConnection();
			}
			else
			{
				if(currentTestRunning)
				{
					return;			// just to be sure Update() runs not parallel!
				}
				Debug.Log("doneTesting");
				Debug.Log("starting server, useNAT = " +useNat.ToString());
				hosting = true;
				registeredGameName += "headless, useNat:" + useNat.ToString();
				serverRunning = true;
				Network.InitializeServer( clientSlots, port, useNat );
				MasterServer.RegisterHost(registeredGameType, registeredGameName, registeredGameComment);
			}
		}
	}

}