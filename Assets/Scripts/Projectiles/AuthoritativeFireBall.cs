using UnityEngine;
using System.Collections;

public class AuthoritativeFireBall : AuthoritativeProjectile {

	public delegate void OnBulletHit(GameObject killer, GameObject victim);
	public static event OnBulletHit onBulletHit;


//	public NetworkPlayer owner;
//	public GameObject ownerCharacter;

	public float bulletStayTime = 8f;

	GameObject gameController;
	Layer layer;
//	StatsManager statsManager;

//	public static Vector3 moveSpeed = new Vector3(2,2,0);
//	public Vector3 moveDirection = new Vector3(1,0,0);
	// Use this for initialization
	void Start () {
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		layer = gameController.GetComponent<Layer>();
//		statsManager = gameController.GetComponent<StatsManager>();
		StartDestroyTimer();
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
		Debug.Log(other.gameObject.name);
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
							//TODO
							//TODO
							//statsManager.BulletHit(ownerCharacter, other.transform.parent.gameObject );
							
							if(onBulletHit != null)
							{
								onBulletHit(ownerCharacter, other.transform.parent.gameObject);
							}
							else
							{
								Debug.LogWarning("onBulletHit no listeners!");
							}

							ownerCharacter.GetComponent<Shoot>().RemoveBullet(this.gameObject);

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

	public void StartDestroyTimer()
	{
		StartCoroutine(DestroyPowerUp());
	}
	
	IEnumerator DestroyPowerUp()
	{
		yield return new WaitForSeconds(bulletStayTime);
		if(Network.peerType == NetworkPeerType.Disconnected)
		{
			Destroy(this.gameObject);
		}
		if(Network.isServer)
		{
			if(this.gameObject != null)
			{
				ownerCharacter.GetComponent<Shoot>().RemoveBullet(this.gameObject);
				Network.RemoveRPCs(this.GetComponent<NetworkView>().viewID);
				Network.Destroy(this.gameObject);
			}
			else
			{
				Debug.LogWarning("nothing to Destroy! already destroyed/collected?!");
			}
		}
	}
}
