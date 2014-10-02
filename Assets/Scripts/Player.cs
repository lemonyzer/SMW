using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class Player
{
	private int id;
	private string name;
	private PhotonPlayer photonPlayer;
	private NetworkPlayer networkPlayer;
	private GameObject characterSelector;
	private PhotonView characterSelectorPhotonView;
	private Character character;
	private Character characterClone;			// Liste von Characteren
	//	private bool isAI;
	private int health;
	private int points;
	private int nemesis;
	private int kills;
	private int deads;
	//	private int 


	public void addHealth(int value)
	{
		health += value;
	}

	// Constructor SinglePlayer
	public Player(int id, string name, Character character)
	{
		this.id = id;
		this.name = name;
		this.character = character;
	}

//	// Construcor Photon Network, Character Selector
	public Player(PhotonPlayer player, GameObject characterSelector)
	{
		this.photonPlayer = player;
		this.characterSelectorPhotonView = characterSelector.GetComponent<PhotonView>();
		this.id = photonPlayer.ID;
		this.name = photonPlayer.name;
		this.characterSelector = characterSelector;
		this.character = null;
	}

//	 CharacterSelector information gets lost if creating a new Player() instance ...

	// Construcor Photon Network
	public Player(PhotonPlayer player, Character character)
	{
		this.photonPlayer = player;
		this.id = photonPlayer.ID;
		this.name = photonPlayer.name;
		this.character = character;
	}

	// Construcor Unity Network
	public Player(NetworkPlayer player, Character character)
	{
		this.networkPlayer = player;
		this.name = networkPlayer.ToString (); 
		this.id = int.Parse(name);
		this.character = character;
	}
	
	//	public int CompareTo(Character other)
	//	{
	//		if(other == null)
	//		{
	//			return 1;
	//		}
	//		
	//		if(characterName != other.characterName)
	//			return 1;
	//		else
	//			return 0;
	//		
	//	}
	
	
	public int getID()
	{
		return id;
	}

	public int getPoints()
	{
		return points;
	}

	public void addPoints(int summand)
	{
		this.points += summand;
	}

	public void setPoints(int points)
	{
		this.points = points;
	}
	
	public string getName()
	{
		return name;
	}
	
	public void setName(string name)
	{
		this.name = name;
	}
	
	//	public Sprite getSprite()
	//	{
	//		return characterSprite;
	//	}
	//	
	//	public void setSprite(Sprite sprite)
	//	{
	//		characterSprite = sprite;
	//	}
	
	//	public GameObject getPrefab()
	//	{
	//		return characterPrefab;
	//	}
	//	
	//	public void setPrefab(GameObject prefab)
	//	{
	//		characterPrefab = prefab;
	//	}

	public PhotonPlayer getPhotonPlayer()
	{
		return photonPlayer;
	}

	public NetworkPlayer getNetworkPlayer()
	{
		return networkPlayer;
	}

	public GameObject getCharacterSelector()
	{
		return characterSelector;
	}
	
	public Character getCharacter()
	{
		return character;
	}
	
	public void setCharacter(Character character)
	{
		this.character = character;
	}

}
