using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class Player
{
	public bool loadingLevelComplete = false;
	private int id;
	public int teamPos;
	public Team team;
	private string name;
//	private PhotonPlayer photonPlayer;
	private NetworkPlayer networkPlayer;
	private Character character;
	private Character characterClone;			// Liste von Characteren
	//	private bool isAI;


	private int nemesis = 0;
	// anzahl der punkte (setzt sich zusammen aus kills: headjumps, weapons kills, nemesis, multikill, combo...)
	private int points = 0;
	// anzahl der kills
	private int kills = 0;
	// anzahl der tote (wie oft gestorben)
	private int deads = 0;
	// anzahl der leben
	private int lifes = 20;
	// aktuelle lebensgesundheit; health = 0 -> lifes--
	private int health = 0;
	//

	public int getKills()
	{
		return kills;
	}

	public void addKill()
	{
		kills++;
	}

	public int GetLifes()
	{
		return lifes;
	}

	public void LostLife()
	{
		if (lifes > 0)
			lifes--;
		AddDeath();
	}

	public void AddDeath()
	{
		deads++;
	}

	public void AddNemesis()
	{
		nemesis++;
	}


	public void addHealth(int value)
	{
		health += value;
	}

//	// Constructor SinglePlayer
//	public Player(int id, string name, Character character)
//	{
//		this.id = id;
//		this.name = name;
//		this.character = character;
//	}

//	// Construcor Photon Network, Character Selector
//	public Player(PhotonPlayer player, GameObject characterSelector)
//	{
//		this.photonPlayer = player;
//		this.characterSelectorPhotonView = characterSelector.GetComponent<PhotonView>();
//		this.id = photonPlayer.ID;
//		this.name = photonPlayer.name;
//		this.characterSelector = characterSelector;
//		this.character = null;
//	}

//	 CharacterSelector information gets lost if creating a new Player() instance ...

	// Construcor Photon Network
//	public Player(PhotonPlayer player, Character character)
//	{
//		this.photonPlayer = player;
//		this.id = photonPlayer.ID;
//		this.name = photonPlayer.name;
//		this.character = character;
//	}

//	// Construcor Unity Network
//	public Player(NetworkPlayer player, Character character)
//	{
//		this.networkPlayer = player;
//		this.name = networkPlayer.ToString (); 
//		this.id = int.Parse(name);
//		this.character = character;
//	}
	
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
	
	public string getUserName()
	{
		return name;
	}
	
	public void setUserName(string name)
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

//	public PhotonPlayer getPhotonPlayer()
//	{
//		return photonPlayer;
//	}

	public NetworkPlayer getNetworkPlayer()
	{
		return networkPlayer;
	}

//	public GameObject getCharacterSelector()
//	{
//		return characterSelector;
//	}
//	
	public Character getCharacter()
	{
		return character;
	}
	
	public void setCharacter(Character character)
	{
		this.character = character;
	}

	/*************************************************/

	public SmwCharacter characterScriptableObject;
	public PlatformCharacter platformCharacterScript;
	
	public SelectorSlotScript UISelectorSlotScript;
	public PlayerStatsSlotScript UIStatsSlotScript;

	// Construcor Unity Network
	public Player(NetworkPlayer player)
	{
		this.networkPlayer = player;
//		this.name = networkPlayer.ToString (); 
//		this.id = int.Parse(name);

	}

	public void SetCharacterScriptableObject(SmwCharacter character)
	{
		this.characterScriptableObject = character;
	}

	private GameObject StatsSlot;
	private GameObject SelectorSlot;
	
	public void setStatsSlot(GameObject slot)
	{
		this.StatsSlot = slot;
		this.UIStatsSlotScript = slot.GetComponent<PlayerStatsSlotScript>();
	}
	
	public GameObject getStatsSlot()
	{
		return StatsSlot;
	}
	
	public void setSelectorSlot(GameObject slot)
	{
		this.SelectorSlot = slot;
		this.UISelectorSlotScript = slot.GetComponent<SelectorSlotScript>();
	}
	
	public GameObject getSelectorSlot()
	{
		return SelectorSlot;
	}

}
