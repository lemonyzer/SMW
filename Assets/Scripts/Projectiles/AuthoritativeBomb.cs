using UnityEngine;
using System.Collections;

public class AuthoritativeBomb : AuthoritativeProjectile {




//	public NetworkPlayer owner;
//	public GameObject ownerCharacter;

	public bool exploding = false;
	public AudioClip bombExplosionClip;

	public void PlayExplosionAudioClip()
	{
//		Debug.LogWarning("try bombExplosionClip playing!!!!");
		if(bombExplosionClip != null)
		{
			this.GetComponent<AudioSource>().PlayOneShot(bombExplosionClip);
//			Debug.LogWarning("bombExplosionClip is playing!!!!");
		}
		else
			Debug.LogWarning("bombExplosionClip not set!!!!");
	}

	public float explosionDelay = 4f;

//	public static Vector3 moveSpeed = new Vector3(2,2,0);
//	public Vector3 moveDirection = new Vector3(1,0,0);
	// Use this for initialization
	void Start () {
		StartBombTimer();
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
#if UNITY_EDITOR
		Debug.Log(other.gameObject.name);
#endif
		if(other.gameObject.layer == Layer.powerUp)
		{
			if(exploding)
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

//								if(onBombHit != null)
//								{
//									onBombHit(ownerCharacter, other.transform.parent.gameObject);
//								}
//								else
//								{
//									Debug.LogWarning("onBulletHit no listeners!");
//								}
	// Sound & Animation Fix: Bomb gets automatic destroyed!
	//							ownerCharacter.GetComponent<Shoot>().RemoveBullet(this.gameObject);
	//							Network.RemoveRPCs(this.GetComponent<NetworkView>().viewID);
	//							Network.Destroy(this.gameObject);
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
	}

	public void StartBombTimer()
	{
		StartCoroutine(ExplodePowerUp());
	}

	IEnumerator ExplodePowerUp()
	{
		yield return new WaitForSeconds(explosionDelay);
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
