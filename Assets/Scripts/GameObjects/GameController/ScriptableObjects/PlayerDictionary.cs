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

	Dictionary<NetworkPlayer, Player> playerDictionary = new Dictionary<NetworkPlayer, Player>();

//	public void SetCharacterSelector(GameObject characterSelector, Player player)
//	{
//		List<Player> buffer = new List<Player>(gameObjectsPlayerDictionary.Values);
//		foreach( Player currPlayer in buffer )
//		{
//			if( currPlayer == player )
//			{
//
//			}
//		}
//	}

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
				Debug.Log("removed GameObject referenz (" + currPlayer.getCharacter().getPrefabFilename() + ") from Player: " + networkPlayer.guid );
			}
			else
			{
				Debug.Log("Player: " + networkPlayer.guid + " is not in Dictionary! WTF, how is that possible?!!");
			}
		}
	}

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
					Debug.LogError(networkPlayer.guid + " has Player and Character but no PrefabFilename!!!");
				}
			}
			else
			{
				// no Character
				Debug.LogError(networkPlayer.guid + " has Player but no Character set in Dictionary!!!");
			}
		}
		else
		{
//			Debug.LogError(networkPlayer.guid + " has no Player set in Dictionary!!!");
		}
		return null;
	}

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
					Debug.LogError(networkPlayer.guid + " has Player and Character but no GameObject!!!");
				}
			}
			else
			{
				// no Character
				Debug.LogError(networkPlayer.guid + " has Player but no Character set in Dictionary!!!");
			}
		}
		{
			Debug.LogError(networkPlayer.guid + " has no Player set in Dictionary!!!");
		}
		return null;
	}

	public GameObject TryGetCharacterSelectorGameObject( NetworkPlayer networkPlayer )
	{
		Player player = null;
		
		if( playerDictionary.TryGetValue(networkPlayer, out player) )
		{
			// Key NetworkPlayer has Value in Dictionary
			
			if(player.getCharacterSelector() != null)
			{
				// GameObject (CharacterSelector) found
				// Player has CharacterSelector
				return player.getCharacterSelector();
			}
			else
			{
				// no CharacterSelector
				Debug.LogError(networkPlayer.guid + " has Player but no CharacterSelector set in Dictionary!!!");
			}
		}
		{
			Debug.LogError(networkPlayer.guid + " has no Player set in Dictionary!!!");
		}
		return null;
	}

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
					Debug.LogWarning(currPlayer.getNetworkPlayer().guid + " verwendet " + prefabFileName + " schon.");
					return true;
				}
			}
		}
		return false;

	}

	public void AddPlayer(NetworkPlayer networkPlayer, Player player)
	{
		Player currentPlayer = null;
		
		if(playerDictionary.TryGetValue(networkPlayer, out currentPlayer))
		{
			// alten Wert überschreiben
			playerDictionary[networkPlayer] = player;
			Debug.LogError(networkPlayer.guid + " was already in Dictionary. overwritten!");
		}
		else
		{
			// noch nicht vorhanden, item eintragen
			playerDictionary.Add(networkPlayer, player);
			Debug.Log(networkPlayer.guid + " added to Dictionary.");
		}
	}
	
	public void RemovePlayer( NetworkPlayer networkPlayer )
	{
		Player removedPlayer = null;
		
		if(playerDictionary.TryGetValue(networkPlayer, out removedPlayer))
		{
			playerDictionary.Remove(networkPlayer);
			Debug.Log(networkPlayer.guid + " removed from Dictionary.");
		}
		else
		{
			Debug.LogWarning(networkPlayer.guid + " was not added to Dictionary.");
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
			Debug.LogError(key.guid + " has no Player in Dictionary, to set Character!");
		}
	}

	public void SetPlayer(NetworkPlayer key, Player value)
	{
		Player currentPlayer = null;
		
		if(playerDictionary.TryGetValue(key, out currentPlayer))
		{
			playerDictionary[key] = value;
			Debug.Log(key.guid + " was already in Dictionary, value " + currentPlayer.getName() + " replaced by " + value.getName());
		}
		else
		{
			// was not in Dictionary, added KeyValuePair
			playerDictionary.Add(key, value);
		}
	}
	
	public Player GetPlayer(NetworkPlayer networkPlayer)
	{
		Player currentValue = null;
		Player result = null;

		if(playerDictionary.TryGetValue(networkPlayer, out currentValue))
		{
			result = currentValue;
		}
		
		return result;
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
}
