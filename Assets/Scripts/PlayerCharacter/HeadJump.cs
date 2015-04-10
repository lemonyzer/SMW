using UnityEngine;
using System.Collections;

public class HeadJump : MonoBehaviour {

	public delegate void OnHeadJump(Player killer, Player victim);
	public static event OnHeadJump onHeadJump;

	PlatformCharacter myCharacter;
	Animator anim;
	AudioClip headJumpedSound;

	void Awake()
	{
		myCharacter = GetComponent<PlatformCharacter>();
		if(headJumpedSound == null)
		{
			headJumpedSound = myCharacter.deathSound;
		}
		anim = myCharacter.anim;
	}

	public void HeadJumpTriggered(PlatformCharacter killerCharacter)
	{
		NetworkPlayer netKiller = killerCharacter.ownerScript.owner;
		if(!myCharacter.isDead)
		{
			myCharacter.isDead = true;
			myCharacter.myNetworkView.RPC("HeadJumpAnimation_Rpc", RPCMode.All);
			myCharacter.myNetworkView.RPC("HeadJump_Rpc_Buffered", RPCMode.AllBuffered, netKiller);
		}
		else
		{
			#if UNITY_EDITOR
			Debug.LogWarning(this.ToString() + " ist bereits tot, Angriff zählt nicht");
			#endif
		}
	}

	[RPC]
	public void HeadJump_Rpc_Buffered(NetworkPlayer netKiller)
	{
		Player killer;
		Player victim;

		PlayerDictionaryManager._instance.TryGetPlayer(netKiller, out killer);
		PlayerDictionaryManager._instance.TryGetPlayer(myCharacter.ownerScript.owner, out victim);


		if(onHeadJump != null)
			onHeadJump(killer, victim);
	}

	[RPC]
	public void HeadJumpAnimation_Rpc()
	{
		HeadJumpVictim();
		//killer.HeadJumpKiller();
	}

	public void HeadJumpVictim()
	{
		//RemovePower();
		myCharacter.isHit = true;
		myCharacter.isDead = true;

		anim.SetBool(HashID.spawnBool,false);
		anim.SetBool(HashID.gameOverBool,false);
		anim.SetBool(HashID.headJumpedBool,false);
		anim.SetBool(HashID.deadBool,false);
		anim.SetTrigger(HashID.hitTrigger);			// Lösung!
		
		// Death Sound abspielen
		AudioSource.PlayClipAtPoint(headJumpedSound,transform.position,1);
		
		//Animation setzen
		anim.SetBool(HashID.headJumpedBool,true);
		
		/**
		 * Physics
		 * 
		 * // Layer Collisionen mit Gegenspieler und PowerUps ignorieren, GameObject soll aber auf Boden/Platform fallen und liegen bleiben
		 **/
		
//		// GameObject soll aber auf Boden/Platform fallen und liegen bleiben
//		myGroundStopperCollider.enabled = true;
//		
//		// kann keine Items mehr sammeln
//		itemCollectorCollider2D.enabled = false;
//		
//		// Body BoxCollider2D deaktivieren (Gegenspieler können durchlaufen)
//		bodyCollider2D.enabled = false;
//		//myBodyTrigger.enabled = false;
//		
//		// FeetCollider deaktivieren (Gegenspieler nehmen keinen Schaden mehr)
//		feetCollider2D.enabled = false;
//		
//		// HeadCollider deaktivieren (Spieler kann nicht nochmal Schaden durch HeadJump-Angriff nehmen da er tot ist)
//		headCollider2D.enabled = false;
//		
//		// PowerUp Hit Collider deaktivieren (Spieler kann keinen Schaden mehr durch PowerUp-Angriff nehmen da er tot ist)
//		powerUpCollider2D.enabled = false;
		
//		/* Ki und Controlls deaktivieren */
//		isDead = true;
//		isHit = false;				// <-- kann wieder auf false gesetzt werden, da Spieler jetzt tot ist!
		
		
//		if(Network.isServer)
//		{
//			GetComponent<NetworkView>().RPC("SpawnAnimationDelay", RPCMode.All, currentLevel.getRandomSpawnPosition() );			// wenn server jetzt schon rpc mit spawnposition sended
//			// und reSpawnDelay > triptime
//			// kann syncron auf allen clients die spawnanimation starten
//			// ansonsten, nur auf server spawndelay coroutine starten// und nach ablauf rpc an alle clients startanimation
//			//networkView.RPC("SetSpawnPosition", RPCMode.All, currentLevel.getRandomSpawnPosition() );
//			
//			// spieler so zeitnah wie möglich spielen lassen
//		}
		// StartCoroutine(SpawnAnimation)... in kombination mit //networkView.RPC("SetSpawnPosition", RPCMode.All, currentLevel.getRandomSpawnPosition() );
		
		// server erhält erst input wenn spieler !isDead
	}
}
