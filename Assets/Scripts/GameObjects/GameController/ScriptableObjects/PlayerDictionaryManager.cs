using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerDictionaryManager : MonoBehaviour {


	public static PlayerDictionary syncedLocalPersistentPlayerDictionary;

	void Awake()
	{
		Debug.LogWarning(this.ToString() + ": Awake()");
		if(syncedLocalPersistentPlayerDictionary == null)
		{
			// PlayerDictionary doesn't exist
			// this should haben in first time PhotonLobby!
			syncedLocalPersistentPlayerDictionary = (PlayerDictionary)ScriptableObject.CreateInstance (typeof(PlayerDictionary));
			Debug.Log(this.ToString() +": syncedLocalPersistentPlayerDictionary instantiert!");
		}
		else
		{
			// PlayerDictionary already exists
			// this should haben in PhotonRoom, GameScene and second time PhotonLobby!
			// in PhotonLobby clear PlayerDictionary!!! else they would be exist in next created/joined Room!!!!
			Debug.LogWarning(this.ToString() +": syncedLocalPersistentPlayerDictionary is already instantiated!");

			List<Player> buffer = new List<Player> ( syncedLocalPersistentPlayerDictionary.Values() );
			foreach(Player player in buffer)
			{
				Debug.Log(this.ToString() +": " + player.getName() + " in PlayerDictionary gefunden!");
            }
			if(Application.loadedLevelName == Scenes.photonLobby ||
			   Application.loadedLevelName == Scenes.mainmenu ||
			   Application.loadedLevelName == Scenes.unityNetworkLobby ||
			   Application.loadedLevelName == Scenes.unityNetworkRace)
			{
				// wenn aktuelles Level PhotonLobby ist, lösche alle Einträge aus PlayerDictionary
				syncedLocalPersistentPlayerDictionary.RemoveAll();
				Debug.LogWarning(this.ToString() +": syncedLocalPersistentPlayerDictionary.RemoveAll() executed!!!");
			}
		}
	}

//	void OnLevelWasLoaded()
//	{
//		Debug.LogWarning(this.ToString() + ": OnLevelWasLoaded()");
//	}
//
//	void Start()
//	{
//		Debug.LogWarning(this.ToString() + ": Start()");
//	}

	/**
	 * Delete selection made previously ( all characters can be clicked )
	 **/
//	void Reset()
//	{
//		// playerDictionary leeren, wenn Scene Photon Room gestartet wurde
//		// muss wieder gefüllt werden oder einfach nicht löschen!
//		
//		if(syncedLocalPersistentPlayerDictionary != null)
//			syncedLocalPersistentPlayerDictionary.RemoveAll();
//	}
	
	
//	/**
//	 * PhotonGameManager is also Manager of PlayerDictionary
//	 **/
//	void AwakeScriptableObjects()
//	{
//		if (gamePrefs == null) {
//			// ScriptableObject wurde seit Appstart noch nicht erzeugt.
//			// Spätestens in CharacterSelectionScene erfolgt die erste Instanzierung!!!
//			initValues = true;
//			// instanz kann sceneübergrifend verwendet werden (wenn dieses Script in Scene eingebaut ist (am GameController zB.))
//			gamePrefs = (GamePrefs)ScriptableObject.CreateInstance (typeof(GamePrefs));
//			Debug.Log ("ScriptableObject gamePrefs erzeugt");
//		}
//		if (initValues) {
//			// Werte initialisieren
//			// zB. mit PlayerPrefs (sind auch nach beenden des Programms vorhanden!)
//			int slots = PlayerPrefs.GetInt (gameSlotsCountPlayerPrefsString);
//			Debug.Log ("PlayerPrefs: " + slots + " (" + gameSlotsCountPlayerPrefsString + ")");
//			if (slots <= 0)
//				slots = 4;										// vertraue keinem Userinput!
//			setNumberOfGameSlots (slots);
//		}
//		
//		if (syncedLocalPersistentPlayerDictionary == null) {
//			// ScriptableObject wurde seit Appstart noch nicht erzeugt.
//			// Spätestens in CharacterSelectionScene erfolgt die erste Instanzierung!!!
//			initValues = true;
//			// instanz kann sceneübergrifend verwendet werden (wenn dieses Script in Scene eingebaut ist (am GameController zB.))
//			syncedLocalPersistentPlayerDictionary = (PlayerDictionary)ScriptableObject.CreateInstance (typeof(PlayerDictionary));
//			Debug.Log ("ScriptableObject GameObjectsPlayerDictionary erzeugt");
//        }
//        if (initValues) {
//            // Werte initialisieren
//            // zB. mit PlayerPrefs (sind auch nach beenden des Programms vorhanden!)
//            //			for(int i=0; i<serverslots; i++)
//            //			{
//            //				playerDictonary.SetGameObject(""+i,null);
//            //			}
//        }
//    }


	/**
	 * Delete selection made previously ( all characters can be clicked )
	 **/
	void Reset()
	{
		// playerDictionary leeren, wenn Scene Photon Room gestartet wurde
		// muss wieder gefüllt werden oder einfach nicht löschen!
		if (syncedLocalPersistentPlayerDictionary == null)
		{
			Debug.LogWarning("Dictionary is NULL!");
			//			syncedLocalPersistentPlayerDictionary = (PlayerDictionary)ScriptableObject.CreateInstance (typeof(PlayerDictionary));
		}
		
		if(syncedLocalPersistentPlayerDictionary != null)
		{
			syncedLocalPersistentPlayerDictionary.RemoveAll();
            Debug.LogWarning("AutoReset clearing Dictionary!");
        }
    }

	/**
	 * sobald room joined empfängt client buffered aktionen ???
	 **/
	void AutoReset()
	{
		// playerDictionary leeren, wenn Scene Photon Room gestartet wurde
		// muss wieder gefüllt werden oder einfach nicht löschen!
		if( Application.loadedLevelName != Scenes.photonRoomAuthorative ||
		   Application.loadedLevelName != Scenes.photonLevel1)
		{
			return;
		}
		
		if(syncedLocalPersistentPlayerDictionary != null)
		{
			syncedLocalPersistentPlayerDictionary.RemoveAll();
            Debug.LogWarning("AutoReset clearing Dictionary!");
        }
    }

}
