using UnityEngine;
using System.Collections;

public class StarScript : ItemScript {

	Item item;

	public override Item GetItem()
	{
		return item;
	}



//	public override Item item {
//		get {
//			return item;
//		}
//		set {
//			item = value;
//		}
//	}

	//Not Polymoph!!!
	//public new Item item = new Star();

	//public new Item item = new Star();		// kann nicht per  Inspector gesetzt und in Prefab gespeichert werden. Um if's zu entgehend muss ItemScript in Items aufgeteilt werden!
											
//	public string itemName = "Star";			// alternative mit if's ... ItemScript string
//
//	void OnTriggerEnter2D(Collider2D other)
//	{
//		if(itemName == "Star")
//		{
//			item = new Star();
//			item.Collecting(this.gameObject, other.gameObject.GetComponent<PlatformCharacter>());
//		}
//	}
	
	//	public string itemName;


	public float itemStayTime = 8f;

//	public override float itemStayTime {
//		get {
//			return itemStayTime;
//		}
//		set {
//			itemStayTime = 8f;
//		}
//	}
	
	public override void StartDestroyTimer()
	{
		StartCoroutine(DestroyPowerUp());
	}
	
	IEnumerator DestroyPowerUp()
	{
		yield return new WaitForSeconds(itemStayTime);
		if(Network.peerType == NetworkPeerType.Disconnected)
		{
			Destroy(this.gameObject);
		}
		if(Network.isServer)
		{
			if(this.gameObject != null)
			{
				Network.RemoveRPCs(this.networkView.viewID);
				Network.Destroy(this.gameObject);
			}
			else
			{
				Debug.LogWarning("nothing to Destroy! already destroyed/collected?!");
			}
		}
	}
	
	
	
	public Layer layer;
	
	public void Awake()
	{
		layer = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<Layer>();
		item = new Star();
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.layer == layer.item)
		{
			if(other.gameObject.name == Tags.itemCollector)
			{
				// Player gefunden
				if(item == null)
				{
					Debug.LogError(this.gameObject.name + " hat kein Item im Inspektor gesetzt!!!");
				}
				else
				{
					//V0: kann im PlatformCharacter noch controllieren ob dieser das Item einsammeln darf! 
					other.transform.parent.GetComponent<PlatformCharacter>().CollectingItem(this);
					
					//V1 
					//item.Collecting(this.gameObject, other.transform.parent.GetComponent<PlatformCharacter>());
				}
			}
		}
	}
}
