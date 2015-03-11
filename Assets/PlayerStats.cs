using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStats : MonoBehaviour {

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
	void Start () {

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

}
