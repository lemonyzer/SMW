using UnityEngine;
using System.Collections;

public class RageTrigger : MonoBehaviour {



//	GameObject gameController;
//	Layer layer;
//	StatsManager statsManager;
	public PlatformCharacter myCharacterScript;

	// Use this for initialization
	void Start () {
//		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
//		layer = gameController.GetComponent<Layer>();
//		statsManager = gameController.GetComponent<StatsManager>();
		myCharacterScript = this.transform.parent.GetComponent<PlatformCharacter>();

	}
	

	void OnTriggerEnter2D (Collider2D other)
	{
		if(Network.isServer || Network.peerType == NetworkPeerType.Disconnected)
		{
			if(myCharacterScript == null)
			{
				Debug.LogError( "myCharacterScript == null");
				return;
			}
			if(myCharacterScript.isInRageModus)
			{
				if(other.gameObject.layer == Layer.powerUp)											// problem player interacts not with player (because of collider)
																									// bodyCollider -> in Child GO
				{
					// check if other collider is from a player or a powerup
					if(other.gameObject.name == Tags.name_powerUpHitArea)
					{

						other.transform.parent.GetComponent<PlatformCharacter>().Victim_AttackTriggered(this);

//						// other gameObject is child from a Character
//						if(!other.transform.parent.GetComponent<Rage>().isInRageModus)
//						{
//
//							if(!other.transform.parent.GetComponent<PlatformCharacter>().isDead)			//TODO not needed, if collider is deactivated during respawn-spawnProtection!
//							{
//								// nur wenn anderer Spieler nicht auch in RageModus ist!
//
//								//TODO
//								//TODO
//								//statsManager.InvincibleAttack(this.transform.parent.gameObject, other.transform.parent.gameObject);
//
//
//
//								if(onRageKill != null)
//								{
//									onRageKill(this.transform.parent.gameObject, other.transform.parent.gameObject);
//								}
//								else
//								{
//									Debug.LogWarning("onRageKill no listeners!");
//								}
//							}
//						}
					}
				}
				else if(other.gameObject.layer == Layer.head)
				{
					// spieler in spawnprotection wird auch angegriffen!!!!
					//statsManager.InvincibleAttack(this.gameObject, other.transform.parent.gameObject);
				}
				else if(other.gameObject.layer == Layer.feet)
				{
					// spieler in spawnprotection wird auch angegriffen!!!!
					//statsManager.InvincibleAttack(this.gameObject, other.transform.parent.gameObject);
				}
				else
				{
					
				}
			}
		}
	}

}
