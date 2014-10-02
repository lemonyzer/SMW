using UnityEngine;
using System.Collections;

public class Flower : WithPower {
	
	public RPCMode rpcMode = RPCMode.All;
	
	
	public int itemId = ItemLibrary.flowerID;
	public Shoot powerScript;
	public string powerScriptName = "Shoot";						// <--- Item-Power Zuordnung
	
	
	public override void Collecting(GameObject itemGO, PlatformCharacter collector)
	{
		collector.myNetworkView.RPC("CollectedItem", rpcMode, itemId);			// Serverseitig
		
		// rpc geht von collctor aus 				-> Client weiß wer!
		// itemId 									-> Client weiß was!
		// rpc hat NetworkMessageInfo mit timeStamp -> Client weiß wann!
		
		// das itemGO kann Zerstört werden, nach Collecting...
		// wird für jedes Item seperat gehandelt.
		// könnte noch interface oder oberklasse mit destroyaftercollecting stayaftercollecting erweitern...
		if(Network.isServer)
		{
			Network.RemoveRPCs(itemGO.networkView.viewID);
			Network.Destroy(itemGO.gameObject);
		}
	}
	
	public override void Collected(PlatformCharacter collector, NetworkMessageInfo info)
	{
		//		this.collector = collector;
		//		collector.hasItem = true;
		//		collector.power1 = new Shoot();
		
		Power characterPowerScript = collector.gameObject.GetComponent(powerScriptName) as Power;
		if(characterPowerScript != null)
		{
			//characterPowerScript.gained(info);
			Debug.LogError("GetComponent(string) hat funktioniert!");
			characterPowerScript.gained(info);
			return;
		}
		else
		{
			Debug.LogError("GetComponent(string) hat nicht funktioniert!");
		}
	}
}
