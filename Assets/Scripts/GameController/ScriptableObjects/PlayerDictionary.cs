using UnityEngine;
using System.Collections;
using System.Collections.Generic;	// für Dictionary

/**
 * ScriptableObject,
 * kann nicht DIREKT auf ein GameObject gezogen werden!
 * Im xManager wird eine static public instanz erzeugt, der den scenenübergreifenden Zugriff auf die generische Sammlung gewährleistet.
 * 
 **/


public class PlayerDictionary : ScriptableObject {

	// Key = instanz of PlayerCharacter GameObject
	// Value = Player

	public int nextLevelId = 0;
	public bool serverHasPlayer = false;

	Dictionary<NetworkPlayer, Player> playerDictionary = new Dictionary<NetworkPlayer, Player>();

	/// <summary>
	/// Removes all player characer game objects.
	/// </summary>
	public void RemoveAllPlayerCharacerGameObjects()
	{
		List<NetworkPlayer> buffer = new List<NetworkPlayer>(playerDictionary.Keys);
		foreach(NetworkPlayer networkPlayer in buffer)								// iterator will result in generic method ???
		{
			Player currPlayer = null;
			if(playerDictionary.TryGetValue(networkPlayer, out currPlayer))
		 	{
				// Key NetworkPlayer has Value in Dictionary (Player exists in Dictionary)
				currPlayer.getCharacter().RemoveCharacterGameObject();
				Debug.Log("removed GameObject referenz (" + currPlayer.getCharacter().getPrefabFilename() + ") from Player: " + networkPlayer.ToString() );
			}
			else
			{
				Debug.Log("Player: " + networkPlayer.ToString() + " is not in Dictionary! WTF, how is that possible?!!");
			}
		}
	}

	/// <summary>
	/// Tries the get character prefab filename.
	/// </summary>
	/// <returns>The get character prefab filename.</returns>
	/// <param name="networkPlayer">Network player.</param>
	public string TryGetCharacterPrefabFilename ( NetworkPlayer networkPlayer )
	{
		Player player = null;
		
		if( playerDictionary.TryGetValue(networkPlayer, out player) )
		{
			// Key NetworkPlayer has Value in Dictionary
			
			if(player.getCharacter() != null)
			{
				// Player has Character, should have GameObject, too!
				
				if( !string.IsNullOrEmpty( player.getCharacter().getPrefabFilename() ) )
				{
					// GameObject found
					return player.getCharacter().getPrefabFilename();
				}
				else
				{
					// Character but no GameObject
					Debug.LogError(networkPlayer.ToString() + " has Player and Character but no PrefabFilename!!!");
				}
			}
			else
			{
				// no Character
				Debug.LogError(networkPlayer.ToString() + " has Player but no Character set in Dictionary!!!");
			}
		}
		else
		{
//			Debug.LogError(networkPlayer.ToString() + " has no Player set in Dictionary!!!");
		}
		return null;
	}

	/// <summary>
	/// Tries the get character game object.
	/// </summary>
	/// <returns>The get character game object.</returns>
	/// <param name="networkPlayer">Network player.</param>
	public GameObject TryGetCharacterGameObject( NetworkPlayer networkPlayer )
	{
		Player player = null;
		
		if( playerDictionary.TryGetValue(networkPlayer, out player) )
		{
			// Key NetworkPlayer has Value in Dictionary
			
			if(player.getCharacter() != null)
			{
				// Player has Character, should have GameObject, too!
				
				if(player.getCharacter().getGameObject() != null )
				{
					// GameObject found
					return player.getCharacter().getGameObject();
				}
				else
				{
					// Character but no GameObject
					Debug.LogError(networkPlayer.ToString() + " has Player and Character but no GameObject!!!");
				}
			}
			else
			{
				// no Character
				Debug.LogError(networkPlayer.ToString() + " has Player but no Character set in Dictionary!!!");
			}
		}
		{
			Debug.LogError(networkPlayer.ToString() + " has no Player set in Dictionary!!!");
		}
		return null;
	}

	/// <summary>
	/// Prefabs the in use.
	/// </summary>
	/// <returns><c>true</c>, if in use was prefabed, <c>false</c> otherwise.</returns>
	/// <param name="prefabFileName">Prefab file name.</param>
	public bool PrefabInUse( string prefabFileName )
	{
		List<Player> buffer = new List<Player>(playerDictionary.Values);
		foreach( Player currPlayer in buffer )
		{
			if( currPlayer.getCharacter() != null )
			{
				if(currPlayer.getCharacter().getPrefabFilename() == prefabFileName)
				{
					// already in use
					Debug.LogWarning(currPlayer.getNetworkPlayer().ToString() + " verwendet " + prefabFileName + " schon.");
					return true;
				}
			}
		}
		return false;

	}

	/// <summary>
	/// Adds the player.
	/// </summary>
	/// <param name="networkPlayer">Network player.</param>
	/// <param name="player">Player.</param>
	public void AddPlayer(NetworkPlayer networkPlayer, Player player)
	{
		Player currentPlayer = null;
		
		if(playerDictionary.TryGetValue(networkPlayer, out currentPlayer))
		{
			// alten Wert überschreiben
			playerDictionary[networkPlayer] = player;
			Debug.LogError("networkPlayer " + networkPlayer.ToString() + " " + player.getUserName() + " was already in Dictionary. overwritten!");
		}
		else
		{
			// noch nicht vorhanden, item eintragen
			playerDictionary.Add(networkPlayer, player);
			Debug.Log("networkPlayer " + networkPlayer.ToString() + " " + player.getUserName() + " added to Dictionary.");
		}
	}

	//TODO full consistent integration!
	ConnectionStats conStats;
	GameObject guiGO;

	/// <summary>
	/// Removes the player.
	/// </summary>
	/// <param name="networkPlayer">Network player.</param>
	public void RemovePlayer( NetworkPlayer networkPlayer )
	{
//		//TODO
//		//TODO begin
//		//TODO
//		guiGO = GameObject.Find("GUI");
//		if(guiGO != null)
//		{
//			conStats = guiGO.GetComponent<ConnectionStats>();
//			if(conStats != null)
//			{
//				conStats.RemovePlayer(networkPlayer);
//			}
//		}
//		//TODO
//		//TODO end
//		//TODO

		Player removedPlayer = null;
		
		if(playerDictionary.TryGetValue(networkPlayer, out removedPlayer))
		{
			playerDictionary.Remove(networkPlayer);
			Debug.Log("NetPlayer: " + networkPlayer.ToString() + " " + removedPlayer.getUserName() + " removed from Dictionary.");
		}
		else
		{
			Debug.LogWarning("NetPlayer: " + networkPlayer.ToString() + " was not added to Dictionary.");
		}
	}

	public void SetCharacter(NetworkPlayer key, Character setCharacter)
	{
		Player currentPlayer = null;
		Character currentCharacter = null;
		
		if(playerDictionary.TryGetValue(key, out currentPlayer))
		{
			currentCharacter = currentPlayer.getCharacter();
			currentPlayer.setCharacter(setCharacter);
			if(currentCharacter.getGameObject() != null)
			{
				Debug.LogWarning(currentCharacter.getGameObject().name + " replaced");
				if(setCharacter.getGameObject() != null)
				{
					Debug.LogWarning(currentCharacter.getGameObject().name + " replaced by " + setCharacter.getGameObject().name);
				}
 			}
		}
		else
		{
			Debug.LogError(key.ToString() + " has no Player in Dictionary, to set Character!");
		}
	}

	public void SetPlayer(NetworkPlayer key, Player value)
	{
		Player currentPlayer = null;
		
		if(playerDictionary.TryGetValue(key, out currentPlayer))
		{
			playerDictionary[key] = value;
			Debug.Log(key.ToString() + " was already in Dictionary, value " + currentPlayer.getUserName() + " replaced by " + value.getUserName());
		}
		else
		{
			// was not in Dictionary, added KeyValuePair
			playerDictionary.Add(key, value);
		}
	}

/// <summary>
/// Tries the get player.
/// </summary>
/// <returns><c>true</c>, if get player was tryed, <c>false</c> otherwise.</returns>
/// <param name="networkPlayer">Network player.</param>
/// <param name="player">Player.</param>
	public bool TryGetPlayer(NetworkPlayer networkPlayer, out Player player)
	{
//		Player currentValue = null;
		player = null;
//		Player result = null;

		if(playerDictionary.TryGetValue(networkPlayer, out player))
		{
//			player = currentValue;
			return true;
		}
		
		return false;
	}

	public Dictionary<NetworkPlayer, Player>.KeyCollection Keys()
	{
		return playerDictionary.Keys;
	}

	public Dictionary<NetworkPlayer, Player>.ValueCollection Values()
	{
		return playerDictionary.Values;
	}

	public void RemoveAll()
	{
		playerDictionary.Clear();
		Debug.LogWarning("Dictionary cleared.");
	}







	public bool IsNetPlayerInDictionary(NetworkPlayer netPlayer, out Player player)
	{
		player = null;
		if (PlayerDictionaryManager._instance.TryGetPlayer(netPlayer, out player))
		{
			return true;
		}
		return false;
	}
	
	public Player CreatePlayerInDictionary(NetworkPlayer netPlayer)
	{
		Player newPlayer = new Player(netPlayer);
		PlayerDictionaryManager._instance.AddPlayer(netPlayer, newPlayer);
		return newPlayer;
	}
	
	public Player GetPlayerAndCreateIfNotInDictionary(NetworkPlayer netPlayer)
	{
		Player player = null;
		if (IsNetPlayerInDictionary(netPlayer, out player))
		{
			// schon vorhanden
			
			if(player != null)
			{
				return player;
			}
		}
		else
		{
			player = CreatePlayerInDictionary(netPlayer);
			if (player != null)
			{
				return player;
			}
		}
		Debug.LogError("ERROR GetPlayerAndCreateIfNotInDictionary ERROR");
		return null;
	}
















}
