using UnityEngine;
using System.Collections;

// Photon GUIDE Tutorial 3: PlayerScript

public class PhotonCharacterSelectorAuthorative : Photon.MonoBehaviour
{
	private string debugmsg; 
	private Vector3 clickedPosition = Vector3.zero;
	public AudioClip characterInUseSound;
	public AudioClip characterSelected;				// Prefab Sound später abspielen

	private GameObject gameController;
	private PhotonRoomManagerAuthorative gameManager;
	private PhotonView photonViewGameManager;
	private Layer layer;

	void Awake()
	{
		gameController = GameObject.FindGameObjectWithTag (Tags.gameController);
		gameManager = gameController.GetComponent<PhotonRoomManagerAuthorative> ();
		photonViewGameManager = gameController.GetComponent<PhotonView> ();
	}

	void Update()
	{
		GetClickedCharacterPrefabName();
	}


	/**
	 * Client Funktion, mit RPC an Server
	 **/
	void GetClickedCharacterPrefabName()
	{
		if(Input.GetMouseButtonUp(0))
		{
			debugmsg = "clicked" + "\n";
			string selectedCharacterPrefabName = GetSelectedCharacterName(Input.mousePosition);
			if(selectedCharacterPrefabName != null)
			{
				clickedPosition = Input.mousePosition;
				if(photonView == null)
				{
					// SinglePlayer
					// Debug.Log("SinglePlayer");
					Debug.LogError(this.ToString()+": has no photonView, cant talkt with Server!");
					// CharacterClickedSinglePlayer(selectedCharacterPrefabName);
				}
				else
				{
					// Multiplayer
					// PhotonNetwork muss im Gegensatz zu UnityNetwork nicht zwischen MasterClient und "normalen" Clients unterscheiden: 
					// RPC an PhotonTargets.MasterClient funktioniert auch auf MasterClient selbst
					// UnityNetwork: RPCMode.Server kann nicht auf Server selbst ausgeführt werden!
					// UnityNetwork: if(Network.isServer) { " ServerSpieler (lokale funktion) code" } else { "Clients: networkView.RPC RPCMode.Server" }
					photonViewGameManager.RPC("CharacterClicked", PhotonTargets.MasterClient, selectedCharacterPrefabName, PhotonNetwork.player);
				}
			}
		}
	}

	/**
	 * Client / Server Funktion
	 **/
	public string GetSelectedCharacterName(Vector3 clickedPosition)
	{
		Ray ray = Camera.main.ScreenPointToRay(clickedPosition);		
		Vector2 origin = ray.origin;										// startPoint
		Vector2 direction = ray.direction;									// direction
		float distance = 100f;
		// 2D
		RaycastHit2D hit = Physics2D.Raycast(origin,direction,distance);
		bool hitted = false;
		if(hit.collider != null)
			hitted = true;
// 3D
//		RaycastHit hit;
//		bool hitted = Physics.Raycast(ray, out hit);
		if(hitted)
		{
			if(hit.collider.tag == Tags.character)
			{
				Debug.Log(this.ToString()+": selected Character = " + hit.collider.name);
				
				// Name des getroffenen GameObject's zurückgeben
				return hit.collider.name;
			}
			else 
			{
				// nothing spawnable hitted
				Debug.Log(this.ToString() + ": wrong Tag!");
			}
		}
		else
			Debug.Log(this.ToString() + ": nothing hitted with RayCast!");
		return null;
	}
//
//	/**
//	 * Client / Server Funktion
//	 **/
//	public string GetSelectedCharacterName(Vector3 clickedPosition)
//	{
//		Ray ray = Camera.main.ScreenPointToRay(clickedPosition);		
//		Vector2 origin = ray.origin;										// startPoint
//		Vector2 direction = ray.direction;									// direction
//		float distance = 100f;
//		RaycastHit2D hit = Physics2D.Raycast(origin,direction,distance);
//		if(hit.collider != null)
//		{
//			if(hit.collider.tag == Tags.character)
//			{
//				Debug.Log("GameManager, Selected Character: " + hit.collider.transform.parent.name);
//				
//				// Name des getroffenen GameObject's zurückgeben
//				return hit.collider.transform.parent.name;
//			}
//			// Head und Feet gehen nicht, da auch die neu instantierten GameObject diese Eigenschaften haben!
//			//			else if(hit.collider.tag == Tags.feet ||
//			//			        hit.collider.tag == Tags.head)
//			//			{
//			//				// Kopf oder Füße getroffen -> Parent GameObject Name
//			//				Debug.Log("LobbyCharacterManager, Head/Feet Selected Character:" + hit.collider.transform.parent.name );
//			//				return hit.collider.transform.parent.name;
//			//			}
//			else 
//			{
//				// nothing spawnable hitted
//			}
//		}
//		return null;
//	}
}
