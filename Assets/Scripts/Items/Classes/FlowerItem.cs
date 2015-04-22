using UnityEngine;
using System.Collections;

public class FlowerItem : Item {

	public FlowerItem (int id) : base (id)
	{
		rpcMode = RPCMode.AllBuffered;
	}
	
//	public RPCMode rpcMode = RPCMode.AllBuffered; // eigentlich brauch die Einsammelinfo nur der betroffende Spieler, ABER andere Spieler sollen sehen das er schießen kann!
	
//	public int itemId = ItemLibrary.flowerID;
	public Shoot powerScript;
	public string powerScriptName = "Shoot";						// <--- Item-Power Zuordnung

	// Weapon Settings
	public int projectileLimit = 2;
	public double bulletToBulletTime = 0.5f;

	// Projectile
	public static string resourcesFolder = "Projectiles";		
	public static string projectilePrefabFilename = "FireBall";		
	public static Vector3 projectileSpawnPositionOffset = new Vector3 (1,0,0);	
	public Projectile projectile = new FireBallProjectile(resourcesFolder, projectilePrefabFilename, projectileSpawnPositionOffset );
	
	
//	public override void Collecting(GameObject itemGO, PlatformCharacter collector)
//	{
//		collector.myNetworkView.RPC("CollectedItem_Rpc", rpcMode, itemId);			// Serverseitig
//		
//		// rpc geht von collctor aus 				-> Client weiß wer!
//		// itemId 									-> Client weiß was!
//		// rpc hat NetworkMessageInfo mit timeStamp -> Client weiß wann!
//		
//		// das itemGO kann Zerstört werden, nach Collecting...
//		// wird für jedes Item seperat gehandelt.
//		// könnte noch interface oder oberklasse mit destroyaftercollecting stayaftercollecting erweitern...
//		if(Network.isServer)
//		{
//			Network.RemoveRPCs(itemGO.GetComponent<NetworkView>().viewID);
//			Network.Destroy(itemGO.gameObject);
//		}
//	}
	
	public override void Collected(PlatformCharacter collector, double collectedTimeStamp)
	{
		//		this.collector = collector;
		//		collector.hasItem = true;
		//		collector.power1 = new Shoot();
		
		Shoot characterPowerScript = collector.gameObject.GetComponent(powerScriptName) as Shoot;
		if(characterPowerScript != null)
		{
			//characterPowerScript.gained(info);
			Debug.LogWarning(this.ToString() + " GetComponent(string) hat funktioniert!");
			characterPowerScript.SetBulletToBulletTime(bulletToBulletTime);
			characterPowerScript.SetProjectileLimit(projectileLimit);
			characterPowerScript.SetProjectile(projectile);
			characterPowerScript.gained(collectedTimeStamp);
			return;
		}
		else
		{
			Debug.LogError(this.ToString() + " GetComponent(string) hat nicht funktioniert!");
		}
	}
}
