using UnityEngine;
using System.Collections;

[System.Serializable]
public class InventoryItem : ScriptableObject {

	// Item GO
	public string itemName;
	public int itemId;
	public GameObject itemPrefab;


	// Item Power
	public bool itemHasPower;
	// V1
	public string powerString;
	// V2
	public Power power;



}
