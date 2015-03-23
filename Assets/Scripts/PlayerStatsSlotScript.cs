using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStatsSlotScript : MonoBehaviour {

	//All
	public Image border;
	public Image slotAvatar;
	public Text slotName;
	public Text slotWho;
	public Text slotKills;
	public Text slotLifes;
	public Text lastPing;		//Client ping to Server, (Servercharacter)
	public Text avgPing;		//Client avg ping to Server, (Servercharacter)

	// Server
	public Text correctPosSendCount;
	public Text lastPosDiff;
	public Text avgPosDiff;

	//Client
	public Text exinterpCount;	//other Client & Server Character
	public Text droppedCount;	//Client dropped Packages from Server (my & other Client and Servercharacter)
	

	// Use this for initialization
	public void Start () {

		border = GetComponent<Image>();
		slotAvatar = transform.FindChild("SlotImage").GetComponent<Image>();
		slotName = transform.FindChild("SlotName").GetComponent<Text>();
		slotWho = transform.FindChild("SlotWho").GetComponent<Text>();

		slotKills = transform.FindChild("SlotKills").GetComponent<Text>();
		slotLifes = transform.FindChild("SlotLifes").GetComponent<Text>();

		lastPing = transform.FindChild("SlotPing").GetComponent<Text>();
		avgPing = transform.FindChild("SlotAvaragePing").GetComponent<Text>();

		exinterpCount = transform.FindChild("SlotExinterpCount").GetComponent<Text>();
		droppedCount = transform.FindChild("SlotDropCount").GetComponent<Text>();

	}

	public void UpdateSlot(Player player)
	{
		if(player == null)
			return;

		if(slotAvatar == null)
		{
			Debug.LogError("slotAvatar not Set, Start() needs to run first!");
		}
		slotAvatar.sprite = player.characterAvatarScript.gameObject.GetComponent<SpriteRenderer>().sprite;
		slotName.text = player.getNetworkPlayer().ipAddress;

		if(Network.isClient)
		{
			if(player.getNetworkPlayer() == Network.connections[0])
			{
				slotWho.text = "Server";
			}
			else if(player.getNetworkPlayer()  == Network.player)
			{
				slotWho.text = "Me! (Client)";
			}
			else
			{
				slotWho.text = "other Client";
			}
		}
		else if(Network.isServer)
		{
			if(player.getNetworkPlayer()  == Network.player)
			{
				slotWho.text = "Me! (Server)";
			}
			else
			{
				slotWho.text = "Client";
			}
		}

	}

}
