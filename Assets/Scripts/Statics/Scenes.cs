using UnityEngine;
using System.Collections;

public class Scenes : MonoBehaviour {

	public const string mainmenu = "MainMenu";
	public const string SingleplayerMenu = "SingleplayerMenu";

	public const string sandbox = "sandbox";
	public const string mapTest = "mapTest";

//	public const string singleplayer = "Singleplayer";
//	public const string spSumo = "sp";
//	public const string spTagging = "level2_tagging";


	/**
	 * Unity Network Scenes
	 **/

	public const string unityNetworkConnectLobby = "UnityNetworkConnectLobby";
	public const string unityNetworkCharacterSelection = "UnityNetworkCharacterSelection";
	public const string unityNetworkGame = "UnityNetworkGame";
	public const string unityNetworkGameRoom = "UnityNetworkGameRoom";
	public const string unityNetworkRace = "UnityNetworkRace";

	/**
	 * Photon Scenes
	 **/

	public const string photonLobby = "PhotonLobbyMenu";
	public const string photonRoomNonAuthorative = "PhotonRoomNonAuthorative";
	public const string photonRoomAuthorative = "PhotonRoomAuthorative";
	public const string photonTagging = "photon_tagging";
	public const string photonLevel1 = "PhotonRoom";
	public const string photonLevel2 = "PhotonRoom";
	public const string photonLevel3 = "PhotonRoom";
	
	/**
	 * Position Transfertest
	 **/
	public const string photonRigidbody = "level1_photon_rigidbody";
	public const string photonBasicNetworkedPlayer = "level1_photon_networkedplayer";
	public const string photonNetworkedPlayerNaiveInterpolation = "level1_photon_networkedplayer_naive_interpolation";
	public const string photonNetworkedPlayerImprovedInterpolation = "level1_photon_networkedplayer_improved_interpolation";

}
