using UnityEngine;
using System.Collections;

public abstract class ItemScript : MonoBehaviour {

	// wird von Baseclasse geerbt (protected)
	//private Item _item;
	
	// public 	= öffentlich
	// override = überschreibe von Baseclass
	// Item 	= Rückgabewert
	// item		= "Property"-Name
	
	//set-zugriff mit: item = xyz;
	//get-zugriff mit: item.Collecting();


	// vererben ???
//	public delegate void OnCollecting(GameObject killer, GameObject victim);
//	public static event OnCollecting onRageKill;

	protected int _itemId;	

	public int itemId {
		get {
			return _itemId;
		}
		set {
			_itemId = value;
		}
	}

	// This function is always called before any Start functions and also just after a prefab is instantiated
	abstract public void Awake();

//	abstract public float itemStayTime { get; set;}			// debug..geht nicht (coroutine)

//	public abstract void StartDestroyTimer();




	/**
	 * Inspector can't handle Polymorphie Scripts /without MonoBehaviour inheritance
	 **/

	//TODO Item item kann nicht per  Inspector gesetzt und in Prefab gespeichert werden. Um if's zu entgehend muss ItemScript in Items aufgeteilt werden!
	//	public string itemName = "Star";			// alternative mit if's ... ItemScript string
	//	
	//	void OnTriggerEnter2D(Collider2D other)
	//	{
	//		if(itemName == "Star")
	//		{
	//			item = new Star();
	//			item.Collecting(this.gameObject, other.gameObject.GetComponent<PlatformCharacter>());
	//		}
	//		else if(itemName == "Flower")
	//		{
	//			item = new Flower();
	//			item.Collecting(this.gameObject, other.gameObject.GetComponent<PlatformCharacter>());
	//		}
	//	}


	public bool autoDestroy = true;
	public float itemStayTime = 8f;
	

	public virtual void StartDestroyTimer()
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
				Network.RemoveRPCs(this.GetComponent<NetworkView>().viewID);
				Network.Destroy(this.gameObject);
			}
			else
			{
				Debug.LogWarning("nothing to Destroy! already destroyed/collected?!");
			}
		}
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.layer == Layer.item)
		{
			if(other.gameObject.name == Tags.name_itemCollector)
			{
				// Player gefunden
				if(itemId == null)
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
