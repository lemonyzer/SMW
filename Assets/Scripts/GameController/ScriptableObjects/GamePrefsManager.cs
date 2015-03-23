using UnityEngine;
using System.Collections;

public class GamePrefsManager : MonoBehaviour {

	public static GamePrefs gamePrefs;

	void Awake()
	{
		Debug.LogWarning(this.ToString() + ": Awake()");
		if(gamePrefs == null)
		{
			// PlayerDictionary doesn't exist
			// this should haben in first time PhotonLobby!
			gamePrefs = (GamePrefs)ScriptableObject.CreateInstance (typeof(GamePrefs));
			Debug.Log(this.ToString() +": gamePrefs instantiert!");
		}
		else
		{
			// PlayerDictionary already exists
			// this should haben in PhotonRoom, GameScene and second time PhotonLobby!
			// in PhotonLobby clear PlayerDictionary!!! else they would be exist in next created/joined Room!!!!
			Debug.LogWarning(this.ToString() +": gamePrefs is already instantiated!");
			
//
//			if(Application.loadedLevelName == Scenes.photonLobby)
//			{
//				// wenn aktuelles Level PhotonLobby ist, lösche alle Einträge aus PlayerDictionary
//				syncedLocalPersistentPlayerDictionary.RemoveAll();
//				Debug.LogWarning("syncedLocalPersistentPlayerDictionary.RemoveAll() executed!!!");
//			}
		}
	}

	void OnLevelWasLoaded()
	{
		Debug.LogWarning(this.ToString() + ": OnLevelWasLoaded()");
	}

	// Use this for initialization
	void Start () {
		Debug.LogWarning(this.ToString() + ": Start()");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
