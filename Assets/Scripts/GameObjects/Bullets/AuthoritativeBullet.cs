using UnityEngine;
using System.Collections;

public class AuthoritativeBullet : MonoBehaviour {

	public NetworkPlayer owner;
	public GameObject ownerCharacter;


	GameObject gameController;
	Layer layer;
	StatsManager statsManager;

	public static Vector3 moveSpeed = new Vector3(5,5,0);
	public Vector3 moveDirection = new Vector3(1,0,0);
	// Use this for initialization
	void Start () {
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		layer = gameController.GetComponent<Layer>();
		statsManager = gameController.GetComponent<StatsManager>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(ownerCharacter == null)
		{
			//Spieler hat character gewechselt oder ist disconnected.
			Debug.Log("Spieler hat Character gewechselt oder ist disconnected, Bullet wird zerstört.");
			if(Network.isServer || Network.peerType == NetworkPeerType.Disconnected)
			{
				Network.RemoveRPCs(this.GetComponent<NetworkView>().viewID);
				Network.Destroy(this.gameObject);
			}
			return;
		}
		if(other.gameObject.layer == layer.powerUp)
		{
			if(other.transform.parent != null)
			{
				if(other.transform.parent.gameObject != ownerCharacter)
				{
					Debug.Log("BulletTrigger, in enemy Hit Area");

					if(Network.isServer || Network.peerType == NetworkPeerType.Disconnected)
					{
						if(other.transform.parent.GetComponent<PlatformCharacter>().isInRageModus)
						{
							this.ownerCharacter = other.transform.parent.gameObject;								// Rage Mode, sets new Owner
						}
						else
						{
							statsManager.BulletHit(ownerCharacter, other.transform.parent.gameObject );
							Network.RemoveRPCs(this.GetComponent<NetworkView>().viewID);
							Network.Destroy(this.gameObject);
						}
					}
				}
				else
				{
					Debug.LogWarning(this.ToString() +", own Bullet!");
				}
			}
		}
	}
}
