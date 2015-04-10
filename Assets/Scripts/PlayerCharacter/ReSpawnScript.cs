using UnityEngine;
using System.Collections;

public class ReSpawnScript : MonoBehaviour {
	public bool debugSpawn = false;

	float reSpawnDelayTime = 2f;

	bool spawnProtection = false;
	float spawnProtectionTime = 2f;
	Color[] spawnProtectionAnimation;
	

	Animator anim;
	GameObject gameController;
//	HashID hash;
//	Layer layer;

	Level currentLevel;

	GameObject myCharacter;
	SpriteRenderer mySpriteRenderer;
	PlatformCharacter myPlatformCharacterScript;
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
		BoxCollider2D[] myBody = transform.Find(Tags.name_body).GetComponents<BoxCollider2D>();
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

	void SetSpawnAnimationCharacterCollider()
	{
		myGroundStopperCollider.enabled = true;	// wurde bei invincible attack deaktiviert...
		// Body BoxCollider2D deaktivieren (Gegenspieler können durchlaufen)
		myBodyCollider.enabled = false;
		// Body Trigger aktivieren, PowerUps einsammeln 			<<---- problem!!!! wird auch zum angriffserkennung (attack & hitarea) im ragemodus benutzt! 
		myBodyTrigger.enabled = false;						
		// FeetCollider aktivieren (Gegenspieler nehmen Schaden)
		myFeetTrigger.enabled = false;
		// HeadTrigger deaktivieren, (in SpawnProtection nicht angreifbar)
		myHeadTrigger.enabled = false;
		/* Ki und Controlls deaktivieren */
		myPlatformCharacterScript.isDead = true;
	}

	void SetSpawnProtectionCharacterCollider()
	{
		myGroundStopperCollider.enabled = true;	// wurde bei invincible attack deaktiviert...
		// Body BoxCollider2D deaktivieren (Gegenspieler können durchlaufen)
		myBodyCollider.enabled = false;												//aktiviert, da collision ignoriert werden! und spieler auf boden liegen bleiben soll
		// Body Trigger aktivieren, PowerUps einsammeln 			<<---- problem!!!! wird auch zum angriffserkennung (attack & hitarea) im ragemodus benutzt! 
		myBodyTrigger.enabled = false;
		// FeetCollider aktivieren (Gegenspieler nehmen Schaden)
		myFeetTrigger.enabled = true;
		// HeadTrigger deaktivieren, (in SpawnProtection nicht angreifbar)
		myHeadTrigger.enabled = false;
		/* Ki und Controlls aktivieren */
		myPlatformCharacterScript.isDead = false;
	}

	void SetSpawnCompleteCharacterCollider()
	{
		myGroundStopperCollider.enabled = true;	// wurde bei invincible attack deaktiviert...
		// Body BoxCollider2D deaktivieren (Gegenspieler können durchlaufen)
		myBodyCollider.enabled = true;												//aktiviert, da collision ignoriert werden! und spieler auf boden liegen bleiben soll
		// Body Trigger aktivieren, PowerUps einsammeln
		myBodyTrigger.enabled = true;
		// FeetCollider aktivieren (Gegenspieler nehmen Schaden)
		myFeetTrigger.enabled = true;
		// HeadTrigger deaktivieren, (in SpawnProtection nicht angreifbar)
		myHeadTrigger.enabled = true;
		/* Ki und Controlls aktivieren */
		myPlatformCharacterScript.isDead = false;
	}

	void Awake()
	{
		myCharacter = this.gameObject;
		myPlatformCharacterScript = GetComponent<PlatformCharacter>();
		InitColliderAndTrigger();
		InitSpawnProtectionAnimation ();
		mySpriteRenderer = GetComponent<SpriteRenderer> ();
		anim = GetComponent<Animator>();
		gameController = GameObject.FindGameObjectWithTag(Tags.tag_gameController);
//		hash = gameController.GetComponent<HashID>();
		currentLevel = gameController.GetComponent<Level>();
//		layer = gameController.GetComponent<Layer>();
	}

	// SpawnArea
	public void StartReSpawn()
	{
		if(debugSpawn && myCharacter.name.StartsWith("Carbuncle"))
			Debug.LogWarning("StartReSpawn()");
		StartCoroutine(SpawnDelay());
//			// disable SpriteRenderer		// nur wenn states von Animator (dead und headjumped) nicht genutzt werden (DeadPrefab mit floorcollider)
		// set new random SpawnPosition
		// spawn Animation!
				

		// animator state = spawnAnimation
		// emable SpriteRenderer
		// wait for SpawnAnimation to finish (yield alternative ?? check Animator is in StateSpawnProtection)
		// after SpawnAnimation
		// enable Controlls

	}

	IEnumerator SpawnDelay()
	{
		if(debugSpawn && myCharacter.name.StartsWith("Carbuncle"))
			Debug.LogWarning("CoRoutine: SpawnDelay()");
		yield return new WaitForSeconds(reSpawnDelayTime);
		StartSpawnAnimation();
	}

	public void StartSpawnAnimation()
	{
		if(debugSpawn && myCharacter.name.StartsWith("Carbuncle"))
			Debug.LogWarning("StartSpawnAnimation()");
		myCharacter.GetComponent<Renderer>().enabled = false;				// sieht besser aus

		// neue Position halten
		GetComponent<Rigidbody2D>().isKinematic = true;

		if(Network.isServer)
		{
			// Random Spawn Position
			SetSpawnPosition();
		}

		// Spawn Animation
		anim.SetBool(HashID.spawnBool, true);

		// Kinematic = true, alle Collider & Trigger aus
		SetSpawnAnimationCharacterCollider();
		myCharacter.GetComponent<Renderer>().enabled = true;				// sieht besser aus
	}

	void SetSpawnPosition()
	{
//		float newPositionX = Random.Range(0.0f, 19.0f);
//		float newPositionY = Random.Range(2f, 15.0f);
//		float oldPositionZ = myCharacter.transform.position.z;
//		myCharacter.gameObject.transform.position = new Vector3(newPositionX,newPositionY,oldPositionZ);
		this.transform.position = currentLevel.getRandomSpawnPosition();
	}

	IEnumerator SpawnProtection()
	{
		if(debugSpawn && myCharacter.name.StartsWith("Carbuncle"))
			Debug.LogWarning("CoRoutine: SpawnProtection()");
		spawnProtection = true;
		yield return new WaitForSeconds(spawnProtectionTime);
		spawnProtection = false;
		SpawnComplete();
	}

	void SpawnComplete()
	{
		if(debugSpawn && myCharacter.name.StartsWith("Carbuncle"))
			Debug.LogWarning("SpawnComplete()");
		mySpriteRenderer.color = new Color(1f,1f,1f,1f);	// transparenz entfernen
		SetSpawnCompleteCharacterCollider();
	}


	void InitSpawnProtectionAnimation()
	{
		spawnProtectionAnimation = new Color[1];
		spawnProtectionAnimation [0] = new Color (1f, 1f, 1f, 0.5f);	// alpha channel = 0.5
	}


	void LateUpdate()
	{
		if(!spawnProtection)
		{
			if(anim.GetCurrentAnimatorStateInfo(0).nameHash == HashID.spawnState)
			{
				if(debugSpawn && myCharacter.name.StartsWith("Carbuncle"))
					Debug.LogWarning("SpawnProtectionState");
				spawnProtection = true;	// coroutine ist zu langsam, wird sonst zweimal gestartet!
				anim.SetTrigger(HashID.nextStateTrigger);	// spawnprotection state verlassen
				// Spawn Animation finished!
				// nach SpawnAnimation Collider & Trigger auf SpawnProtection setzen
				SetSpawnProtectionCharacterCollider();
				// SpawnProtection Timer starten
				StartCoroutine(SpawnProtection());


			}
			else
			{

			}
		}
		else //if(spawnProtection)
		{
			mySpriteRenderer.color = spawnProtectionAnimation[0];
		}
	}
}
