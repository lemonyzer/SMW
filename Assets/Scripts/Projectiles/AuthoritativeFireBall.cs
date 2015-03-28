using UnityEngine;
using System.Collections;

public class AuthoritativeFireBall : AuthoritativeProjectile {




//	public NetworkPlayer owner;
//	public GameObject ownerCharacter;

	public float bulletStayTime = 8f;

//	public static Vector3 moveSpeed = new Vector3(2,2,0);
//	public Vector3 moveDirection = new Vector3(1,0,0);
	// Use this for initialization
	void Start () {
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
		if(other.gameObject.layer == Layer.powerUp)
		{
			if(other.transform.parent != null)
			{
				if(other.transform.parent.gameObject != ownerCharacter)
				{
					Debug.Log("BulletTrigger, in enemy Hit Area");

					if(Network.isServer || Network.peerType == NetworkPeerType.Disconnected)
					{
						// BUGGY, cant shoot -> Projectilelist remove, add
//						if(other.transform.parent.GetComponent<PlatformCharacter>().isInRageModus)
//						{
//							this.ownerCharacter = other.transform.parent.gameObject;								// Rage Mode, sets new Owner
//						}
//						else
//						{
							//TODO
							//TODO
							//statsManager.BulletHit(ownerCharacter, other.transform.parent.gameObject );

						other.transform.parent.gameObject.GetComponent<PlatformCharacter>().Victim_AttackTriggered(this, true);

//							if(onBulletHit != null)
//							{
//								onBulletHit(ownerCharacter, other.transform.parent.gameObject);
//							}
//							else
//							{
//								Debug.LogWarning("onBulletHit no listeners!");
//							}

							DestroyPowerUp();
//						}
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
		StartCoroutine(AutoDestroyPowerUp());
	}
	
	IEnumerator AutoDestroyPowerUp()
	{
		yield return new WaitForSeconds(bulletStayTime);
		DestroyPowerUp();
	}

	public void DestroyPowerUp()
	{
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
