using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class UnityNetworkConnectMenu : MonoBehaviour
{

	public void BtnDefaultPort()
	{
		myServerPort = defaultMyServerPort;
		inputFieldMyServerPort.text = defaultMyServerPort + "";
	}

	void OnEnable()
	{
		ConnectButtonScript.OnClicked += Connect_Button;
	}

	void OnDisable()
	{
		ConnectButtonScript.OnClicked -= Connect_Button;
	}

	public void OnServernameHasChanged(string newServername)
	{
		SaveUsedGameSessionName(newServername);
	}

	public void OnMyServerPortHasChanged(string newMyServerPort)
	{
		bool error = false;
		int port = defaultMyServerPort;
		if (string.IsNullOrEmpty(newMyServerPort))
		{
			// port string is empty
			error = true;
		}
		else
		{
			if (int.TryParse(newMyServerPort, out port))
			{
				if (IsPortValid(port))
				{

				}
				else
				{
					// port is not Valid
					error = true;
				}
			}
			else
			{
				// int parsing failed
				error = true;
			}
		}

		if(error)
		{
			port = defaultMyServerPort;
			// input was wrong, update InputField's text 
			inputFieldMyServerPort.text = port + "";
		}

		SaveUsedGameSessionPort(port);


	}



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

	private string registeredGameType = "";			// unique gamestring (splitt serverlist from other games running on the same masterserver ex. Half-Life / Battlefield
	[SerializeField]
	private string _registeredGameName = "smw";
	[SerializeField]
	private string registeredGameVersion = "alpha";
	[SerializeField]
	private string registeredNetworkProtocolVersion = "0.688";
	private string registeredGameComment = "classic";

	private string registeredGameSessionName ="";

	private string testStatus = "Testing network connection capabilities.";
	private string testMessage = "Test in progress";
	private string shouldEnableNatMessage;
	private bool doneTesting = false;
	private bool probingPublicIP = false;
	private int myServerPort = 25005;
	private ConnectionTesterStatus connectionTestResult = ConnectionTesterStatus.Undetermined;
	private float timer = 0;
	// Indicates if the useNat parameter be enabled when starting a server
	private bool useNat = false;

	public int clientSlots = 10;

	private string ip = "tofast.ddns.net";
	[SerializeField]
	private int connectPort = 25005;


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

	static public string defaultGameSessionName = "User";
	static string playerPrefsGameSessionName = "playerPrefsGameSessionName";

	public int defaultMyServerPort = 25005;
	static string playerPrefsMyServerPort = "playerPrefsMyServerPort";


	bool IsPortValid(int port)
	{
		// Registered Ports
		//  Ports 1024–49151
		// Dynamic Ports
		//  Ports 49152–65535
		if (port > 1024 && port < 49152)
		{
			return true;
		}
		else
		{
			return false;
 	   	}

    }

    
	void GetLastUsedGameSessionPort()
	{
		if (PlayerPrefs.HasKey(playerPrefsMyServerPort))
		{
			int lastGameSessionPort = PlayerPrefs.GetInt(playerPrefsMyServerPort);
			
			if ( IsPortValid (lastGameSessionPort) )
			{
				inputFieldMyServerPort.text = lastGameSessionPort + "";
				return;
			}
		}
		int temp = defaultMyServerPort;
        inputFieldMyServerPort.text = temp + "";
        
        SaveUsedGameSessionPort (temp);
	} 

	public static string GetLastUsedGameSessionNameStatic()
	{
		if (PlayerPrefs.HasKey(playerPrefsGameSessionName))
		{
			string lastGameSessionName = PlayerPrefs.GetString(playerPrefsGameSessionName);
			
			if (!string.IsNullOrEmpty(lastGameSessionName))
			{
				return lastGameSessionName;
			}
		}
		string temp = defaultGameSessionName + " " + Random.Range(1000,2000);
		SaveUsedGameSessionName (temp);
		return temp;
	} 

	void GetLastUsedGameSessionName()
	{
		if (PlayerPrefs.HasKey(playerPrefsGameSessionName))
		{
			string lastGameSessionName = PlayerPrefs.GetString(playerPrefsGameSessionName);

			if (!string.IsNullOrEmpty(lastGameSessionName))
			{
				myGameSessionName.text = lastGameSessionName;
				return;
			}
		}
		string temp = defaultGameSessionName + " " + Random.Range(1000,2000);
		myGameSessionName.text = temp ;

		SaveUsedGameSessionName (temp);
	} 

	void SaveUsedGameSessionPort(int currentSessionPort)
	{
		PlayerPrefs.SetInt (playerPrefsMyServerPort, currentSessionPort);
    }
    
	static void SaveUsedGameSessionName(string currentSessionName)
	{
		PlayerPrefs.SetString (playerPrefsGameSessionName, currentSessionName);
	}

	void Awake()
	{
		// UI in Awake nicht verfügbar
		GetLastUsedGameSessionName();
		GetLastUsedGameSessionPort();

		registeredGameType = _registeredGameName + "_" + registeredGameVersion + "_" + registeredNetworkProtocolVersion;
		registeredGameSessionName = _registeredGameName + "_" + registeredGameVersion;

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

		#if UNITY_EDITOR
			Debug.Log(myIP);
			Debug.Log(myExternalIP);
		#endif


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

//		// UI in Awake nicht verfügbar
//		GetLastUsedGameSessionName();

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
					tmpIp = hosts[i].ip[x] + " ";		// ip[0] enthält komplette IP, ip[1] nächste komplette IP
					x++;
				}


				GameObject tempBtn = (GameObject) Instantiate(prefabButtonConnectToHost);
				tempBtn.transform.SetParent(ScrollViewServerlistContentPanel.transform,false);
				string serverString = "";
				ConnectButtonScript tmpBtnScript = tempBtn.GetComponent<ConnectButtonScript>();
				tmpBtnScript.hostIp = tmpIp;
				tmpBtnScript.hostPort = hosts[i].port;
				tmpBtnScript.hostGuId = hosts[i].guid;
				tmpBtnScript.useNat = hosts[i].useNat;
				serverString += hosts[i].gameName + "\nMasterServer: recvedIP=" + tmpIp + ":";
				serverString += hosts[i].port + "\nMasterServer: You need NAT: ";
				serverString += hosts[i].useNat.ToString() + " ";
				//serverString += hosts[i].guid + " ";

				tempBtn.transform.FindChild("Serverinfo Label").GetComponent<Text>().text = serverString;
			}
		}
	}

	public GameObject prefabButtonConnectToHost;
	public GameObject ScrollViewServerlistContentPanel;



	bool useNatToConnect = true;

	void OnConnectedToServer()
	{
#if UNITY_EDITOR
		Debug.Log( "Connected to server" );
#endif

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
#if UNITY_EDITOR
		Debug.Log( "Server initialized" );
#endif
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
		Network.Connect( inputServerAdress.text, connectPort );
	}

	public void ConnectWithIp_Button(string host, int port)
	{
		Network.Connect( host, port );
	}

	void Connect_Button(ConnectButtonScript clickedButton)
	{
		if(clickedButton.useNat)
		{
			Network.Connect( clickedButton.hostGuId );
			#if UNITY_EDITOR
				Debug.Log("Connect_Button using NAT GUID=" + clickedButton.hostGuId  );
			#endif	
		}
		else
		{
			Network.Connect( clickedButton.hostIp, clickedButton.hostPort );
			#if UNITY_EDITOR
				Debug.Log("Connect_Button using no NAT IP=" + clickedButton.hostIp + ":" + clickedButton.hostPort );
			#endif
		}
	}

	public void ConnectWithGUID_Button(string hostGUID)
	{
		Network.Connect( hostGUID );
	}

	public InputField myGameSessionName;
	public InputField inputFieldMyServerPort;

	public void HostOnline()
	{
		Network.Disconnect();
		hosting = true;

		int inputFieldPort = 0;

		if (int.TryParse (inputFieldMyServerPort.text, out inputFieldPort))
		{
			// Registered Ports
			//  Ports 1024–49151
			// Dynamic Ports
			//  Ports 49152–65535

			if (IsPortValid(inputFieldPort))
			{
				myServerPort = inputFieldPort;
			}
			else
			{
				myServerPort = defaultMyServerPort;
			}
		}
		else
		{
			myServerPort = defaultMyServerPort;
		}

		inputFieldMyServerPort.text = myServerPort + "";

		Network.InitializeServer( clientSlots, myServerPort, useNat );
		MasterServer.dedicatedServer = false;

		SaveUsedGameSessionName(myGameSessionName.text);
		SaveUsedGameSessionPort(myServerPort);

		//MasterServer.RegisterHost(registeredGameType, registeredGameName + " " + port.ToString() + " NAT:" + useNat.ToString() , registeredGameComment);
		registeredGameSessionName = myGameSessionName.text + "; uses Nat=" + useNat + "; IP=" + myIP + ":" + myServerPort;
		MasterServer.RegisterHost(registeredGameType, registeredGameSessionName, registeredGameComment);
		
		//			You can call RegisterHost more than once while a server is running 
		//			to update the information stored on the Master Server. For example, 
		//			if the server changes to a new level, you might call RegisterHost 
		//			again to update the lobby.
	}

	public void HostOffline()
	{
		Network.Disconnect();
		hosting = true;
		Network.InitializeServer( clientSlots, myServerPort, useNat );
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
			testMessage = "Non-connectable public IP address (port " + myServerPort +" blocked), running a server is impossible.";
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
			upnp.name = registeredGameSessionName;
			upnp.OpenUDP(myServerPort);
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
				#if UNITY_EDITOR
				Debug.Log("doneTesting");
				Debug.Log("starting server, useNAT = " +useNat.ToString());
#endif
				hosting = true;
				registeredGameSessionName += "headless, useNat:" + useNat.ToString();
				serverRunning = true;
				Network.InitializeServer( clientSlots, myServerPort, useNat );
				MasterServer.RegisterHost(registeredGameType, registeredGameSessionName, registeredGameComment);
			}
		}
	}

}