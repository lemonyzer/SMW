using UnityEngine;
using System.Collections;

public class AnimatorController : MonoBehaviour {

	public bool godmode = false;

	public AudioClip deathSound;
	public AudioClip gameOverSound;
	public AudioClip criticalHealthSound;

	public float startLifes=10f;
	public float currentLifes=5f;

	public float deathTime = 3.0f;				//how long crops stays!
	public float spawnAnimationTime = 1.0f;
	public float spawnProtectionTime = 1.0f;
	public float restartDelay = 5.0f;
	
	public GameObject deathPrefabRight;
	public GameObject HeadJumpedPrefabRight;
	public bool respawn=false;
	public bool enableControlls=false;
	public bool disableSpawnProtection = false;


	GameObject myCharacter;
//	BoxCollider2D myCharacterCollider2D;
//	Transform feet;
//	BoxCollider2D feetCollider2D;
//	Transform head;
//	BoxCollider2D headCollider2D;

	PlatformCharacter myPlatformCharacterScript;
	ReSpawnScript myReSpawnScript;
	Animator anim;

//	GameObject gameController;
//	private HashID hash;
//	private StatsManager statsManager;
//	private Layer layer;

	/**
	 * Collider / Trigger
	 **/
	// root: PlayerLayer
	BoxCollider2D myBodyCollider;
	BoxCollider2D myBodyTrigger;
	// child Feet: FeetLayer
	BoxCollider2D myFeetTrigger;
	// child Head: HeadLayer
	BoxCollider2D myHeadTrigger;
	// child GroundStopper: PlayerLayer
	BoxCollider2D myGroundStopperCollider;
	
	void InitColliderAndTrigger()
	{
		BoxCollider2D[] myBody = GetComponents<BoxCollider2D>();
		if(myBody == null)
			return;
		foreach(BoxCollider2D coll in myBody)
		{
			if(coll.isTrigger)
				myBodyTrigger = coll;
			else
				myBodyCollider = coll;
		}
		myFeetTrigger = transform.Find(Tags.name_feet).GetComponent<BoxCollider2D>();
		myHeadTrigger = transform.Find(Tags.name_head).GetComponent<BoxCollider2D>();
		myGroundStopperCollider = transform.Find(Tags.name_groundStopper).GetComponent<BoxCollider2D>();
	}

	void Awake() {
		myCharacter = this.gameObject;
		InitColliderAndTrigger();
		myReSpawnScript = GetComponent<ReSpawnScript>();
//		Debug.Log(gameObject.name + " HealthController -> Awake()");
//		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
//		hash = gameController.GetComponent<HashID>();
//		statsManager = gameController.GetComponent<StatsManager>();
//		layer = gameController.GetComponent<Layer>();
	}

	// Use this for initialization
	void Start ()
	{
		anim = myCharacter.GetComponent<Animator>();

		myPlatformCharacterScript = myCharacter.GetComponent<PlatformCharacter>();
		if(myPlatformCharacterScript == null)
			Debug.LogError(myCharacter.name + " has no PlatformCharacter Script");
	}

	void SetCharacterColliderHeadJumped()
	{
		// Layer Collisionen mit Gegenspieler und PowerUps ignorieren, GameObject soll aber auf Boden/Platform fallen und liegen bleiben

		// Body BoxCollider2D deaktivieren (Gegenspieler können durchlaufen)
		myBodyCollider.enabled = false;
		myBodyTrigger.enabled = false;
		
		// FeetCollider deaktivieren (Gegenspieler nehmen keinen Schaden mehr)
		myFeetTrigger.enabled = false;

		// HeadCollider deaktivieren (Spieler kann nicht nochmal schaden nehmen)
		myHeadTrigger.enabled = false;
//		feet.gameObject.SetActive(false);
//		head.gameObject.SetActive(false);

		// Jeder Character hat einen GroundStopper (erkennt nur Collisionen mit Ground,Floor,Blocks)
//		verhindern dass das GameObject durch die Gravität in Boden fällt
//		myCharacter.rigidbody2D.isKinematic = true;
		
		/* Ki und Controlls deaktivieren */
		stopControlls();
	}

	void SetCharacterColliderDead()
	{
		SetCharacterColliderHeadJumped();

		// zusätzlich durch Boden fallen
//		Physics2D.IgnoreLayerCollision(myCharacter.layer,layer.floor,true);
//		Physics2D.IgnoreLayerCollision(myCharacter.layer,layer.block,true);
//		Physics2D.IgnoreLayerCollision(myCharacter.layer,layer.jumpAblePlatform,true);
		myGroundStopperCollider.enabled = false;


	}

	public void HeadJumpAnimation()
	{
		myPlatformCharacterScript.isHit = true;
		anim.SetBool(HashID.spawnBool,false);
		anim.SetBool(HashID.gameOverBool,false);
		anim.SetBool(HashID.headJumpedBool,false);
		anim.SetBool(HashID.deadBool,false);
		anim.SetTrigger(HashID.hitTrigger);			// Lösung!

		// Death Sound abspielen
		AudioSource.PlayClipAtPoint(deathSound,transform.position,1);

		HeadJumped();
		myReSpawnScript.StartReSpawn();
		myPlatformCharacterScript.isHit = false;
    }

	public void ShootedAnimation()
	{
		myPlatformCharacterScript.isHit = true;
		anim.SetBool(HashID.spawnBool,false);
		anim.SetBool(HashID.gameOverBool,false);
		anim.SetBool(HashID.headJumpedBool,false);
		anim.SetBool(HashID.deadBool,false);
		anim.SetTrigger(HashID.hitTrigger);			// Lösung!
		
		// Death Sound abspielen
		AudioSource.PlayClipAtPoint(deathSound,transform.position,1);
        
        NoHeadJump();
		myReSpawnScript.StartReSpawn();
		myPlatformCharacterScript.isHit = false;
    }
    
    public void InvincibleAttackAnimation()
	{
		myPlatformCharacterScript.isHit = true;
		anim.SetBool(HashID.spawnBool,false);
		anim.SetBool(HashID.gameOverBool,false);
		anim.SetBool(HashID.headJumpedBool,false);
		anim.SetBool(HashID.deadBool,false);
		anim.SetTrigger(HashID.hitTrigger);			// Lösung!

		// Death Sound abspielen
		AudioSource.PlayClipAtPoint(deathSound,transform.position,1);

		NoHeadJump();
		myReSpawnScript.StartReSpawn();
		myPlatformCharacterScript.isHit = false;
    }

	public void GameOverAnimation()
	{
		// Death Sound abspielen
		AudioSource.PlayClipAtPoint(gameOverSound,transform.position,1);
		anim.SetBool (HashID.gameOverBool, true);
		SetCharacterColliderDead();
		DeadAnimationPhysics();
		GameOver();
    }

	void NoHeadJump()
	{
		//Animation setzen
		anim.SetBool(HashID.deadBool,true);
		SetCharacterColliderDead();

		DeadAnimationPhysics();
	}

	void HeadJumped() 
	{
		//Animation setzen
		anim.SetBool(HashID.headJumpedBool,true);
		SetCharacterColliderHeadJumped();

		myCharacter.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
	}

	void DeadAnimationPhysics()
	{
		GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 10f);
	}

	void GameOver() 
	{
		AudioSource.PlayClipAtPoint(gameOverSound,transform.position,1);
		Debug.Log (this.gameObject.name + ": GameOver");

		myCharacter.GetComponent<Rigidbody2D>().fixedAngle = false;
		myCharacter.GetComponent<Rigidbody2D>().AddTorque(20);
		stopControlls();
		StartCoroutine(PlayerGameOver());		// nach gameovertime character nicht mehr fallen lassen, kinematic setzen und ausserhalb camera einblenden (resourcen schonen)
	}

	IEnumerator PlayerGameOver()
	{
		yield return new WaitForSeconds(5f);
		myCharacter.GetComponent<Renderer>().enabled = false;
		myCharacter.GetComponent<Rigidbody2D>().isKinematic = true;
		myCharacter.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
	}

	IEnumerator SpawnAnimationTime()
	{
		yield return new WaitForSeconds(spawnAnimationTime);
		enableControlls=true;
	}

	IEnumerator SpawnProtection()
	{
		yield return new WaitForSeconds(spawnProtectionTime);
		disableSpawnProtection=true;
	}

	void stopControlls()
	{
		if(myPlatformCharacterScript != null)
		{
			myPlatformCharacterScript.isDead = true;
			//myCharacter.GetComponent<PlatformCharacter>().enabled = false;
		}
	}

	void startControlls()
	{
		if(myPlatformCharacterScript != null)
		{
			myPlatformCharacterScript.isDead = false;
			//myCharacter.GetComponent<PlatformCharacter>().enabled = true;
		}
	}
}
