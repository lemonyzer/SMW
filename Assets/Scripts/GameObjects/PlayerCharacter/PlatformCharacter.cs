using UnityEngine;
using System.Collections;

public class PlatformCharacter : MonoBehaviour {

//	public Power power;

	// the position read from the network
	// used for interpolation
	private Vector3 readNetworkPos;
	// whether this paddle can accept player input
//	public bool AcceptsInput = true;
	public RealOwner ownerScript;
//	public float gravity=10;

	public NetworkView myNetworkView;

	/**
	 * Debugging GUI Element
	 **/
	public GUIText debugging;
//	private string debugmsg="";

	/** 
	 * Character Position Check 
	 **/
	public bool grounded = false;
	public bool walled = false;
	public Vector2 groundCheckPosition = new Vector2(0, -0.5f);	// Position, where the the Ground will be checked
	public Vector2 wallCheckPosition = new Vector2(0.5f, 0); // Position, where the the Wall will be checked
//	public float groundRadius = 0.2f;	// Size of the Circle @rround the Checkposition 
	public float wallRadius = 0.1f;	// Size of the Circle @rround the Checkposition
//	public LayerMask whatIsGround;	// Floor, JumpAblePlatform, DestroyAblePlatform 
//	public LayerMask whatIsWall;	// Floor, JumpAblePlatform, DestroyAblePlatform
	
	/** 
	 * Character Status 
	 **/
	public bool isHit = false;
	public bool controlsEnabled = true;
	public bool isDead = false;					// is Player currently dead?
	public bool jumpAllowed = true;				// denies/allows player&bots to jump
	public bool moveAllowed = true;				// denies/allows player&bots to move horizontally
//	public bool isInJumpAbleSaveZone = false;	// is Player currently in save Zone (prevent's colliding with Platform) 
	public bool isBouncing = false;				// move speed changed while bouncing with other player
	public bool isInRageModus = false;
	
	/** 
	 * Character Inventory 
	 **/
	public int inventorySlot0 = 0;		// Power Up Slot 1
	public int inventorySlot1 = 0;		// Power Up Slot 2
	
	/** 
	 * Character Sounds 
	 **/
	public AudioClip deathSound;
	public AudioClip gameOverSound;
	public AudioClip criticalHealthSound;
	public AudioClip jumpSound;					// Jump Sound
	public AudioClip changeRunDirectionSound;	// Skid Sound
	public AudioClip wallJumpSound;				// Wall Jump Sound
	
	/** 
	 * Character Movement 
	 **/
	private float maxSpeed = 8.0f;				// max horizontal Speed (in normal state, if ragemode... other speed)
	public float currentSpeed = 8.0f;
	private Vector2 jumpSpeed = new Vector2(8.0F, 10.0F);	// jump Force : wall jump, jump


	/**
	 * Controls Input
	 **/
	PlatformUserControl inputScript;

	/// <summary>
	/// The user input.
	/// </summary>
	/// 
	private float inputVelocity = 0f;
	private bool inputJump = false;	
	private float inputPCVelocity = 0f;
	private bool inputPCJump = false;							// stores Input Key 
	private float inputTouchVelocity = 0f;
	private bool inputTouchJump = false;							// stores Input Key 

	/// <summary>
	/// The game movement physics.
	/// </summary>
	public float pushForce;
	public float pushTime = 0f;

	/** 
	 * Character Animation 
	 **/
//	private SpriteController spriteController;
	public Animator anim;									// Animator State Machine
	public bool facingRight = true;							// keep DrawCalls low, Flip textures scale: texture can be used for both directions 
	public bool changedRunDirection = false;
	
	/** 
	 * Connection to GameController 
	 **/
	private GameObject gameController;
	private HashID hash;
	private Layer layer;
//	private GameSceneManager gameSceneManager;

	/**
	 * Connection to other Body parts
	 **/
	private BoxCollider2D bodyCollider2D;
	private BoxCollider2D headCollider2D;
	private BoxCollider2D feetCollider2D;
	private BoxCollider2D itemCollectorCollider2D;
	private BoxCollider2D powerUpCollider2D;
	private SpriteRenderer spriteRenderer;


	/**
	 * Pre-Instantiated GameObjects
	 **/
//	private GameObject bullet;			// should be public in a gameController script

//	public void setMaxSpeed(float newMaxSpeed)
//	{
//		maxSpeed = newMaxSpeed;
//	}

	public float getMaxSpeed()
	{
		return maxSpeed;
	}

	void Awake()
	{
		myGroundStopperCollider = transform.Find(Tags.groundStopper).GetComponent<BoxCollider2D>();
		myNetworkView = GetComponent<NetworkView>();

		inputScript = GetComponent<PlatformUserControl>();

		spriteRenderer = GetComponent<SpriteRenderer>();

		bodyCollider2D = transform.FindChild(Tags.body).GetComponent<BoxCollider2D>();
		headCollider2D = transform.FindChild(Tags.head).GetComponent<BoxCollider2D>();
		feetCollider2D = transform.FindChild(Tags.feet).GetComponent<BoxCollider2D>();
		itemCollectorCollider2D = transform.FindChild(Tags.itemCollector).GetComponent<BoxCollider2D>();
		powerUpCollider2D = transform.FindChild(Tags.powerUpHitArea).GetComponent<BoxCollider2D>();

		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		currentLevel = gameController.GetComponent<Level>();
		hash = gameController.GetComponent<HashID>();
		layer = gameController.GetComponent<Layer>();

		InitSpawnProtectionAnimation();
		InitPredictedShootAnimation();
//		gameSceneManager = gameController.GetComponent<GameSceneManager>();

		//LayerMasks();	// <-- wichtig in Start... Awake ist zu früh
	}

//	void LayerMasks()
//	{
//		whatIsGround = 1 << layer.floor;
//		whatIsGround |= 1 << layer.jumpAblePlatform;
//		whatIsGround |= 1 << layer.destroyAbleBlock;
//	}

	void Start() {
		anim = GetComponent<Animator>();
		ownerScript = GetComponent<RealOwner>();

		if(Application.loadedLevelName == "UnityNetworkRace")
		{
			beamEnabled = false;
		}
	}


	// FixedUpdate is called once per frame
	void FixedUpdate () {
		if(Network.isClient)
			return;

		if(Network.isServer)
			return;
		

		// net mode:
		// simulate is called by networkedplayer
		// offline
		if(Network.peerType == NetworkPeerType.Disconnected)
		{
			// offline movement
			Simulate();
		}
	}

	public bool platformJump = false;
	BoxCollider2D myGroundStopperCollider;

	bool beamEnabled = true;

	void CheckBeam()
	{
		if(!beamEnabled)
			return;

		//playerPos spriterenderer boundaries
		Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);
		
		// Beam
		// 0.5 = half player size (pivot.x)
		// if players pos < leftborder+0.5
		// beam to rightborder-0.5
		if(transform.position.x < -9.5f)
		{
			playerPos.x = 9.5f;
		}
		else if(transform.position.x > 9.5f)
		{
			playerPos.x = -9.5f;
		}
		
		transform.position = playerPos;
	}

	Collider2D[] foundColliderArray = new Collider2D[1];

	void CheckPosition()
	{

		//playerPos spriterenderer boundaries
		Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);

		Vector2 groundedOffset = new Vector2(0f,0.5f);

		Vector2 playerColliderTopLeftPos = new Vector2(transform.position.x - bodyCollider2D.size.x*0.5f + bodyCollider2D.center.x,
		                                               transform.position.y);	// Collider Top Left
		
		Vector2 playerColliderBottomRightPos = new Vector2(transform.position.x + bodyCollider2D.size.x*0.5f + bodyCollider2D.center.x,
		                                                   transform.position.y - spriteRenderer.bounds.extents.y*1.2f);	// Collider Bottom Right

		Vector2 playerColliderTopRightPos = new Vector2(transform.position.x + bodyCollider2D.size.x*0.5f + bodyCollider2D.center.x,
		                                               transform.position.y);	// Collider Top Right
		
		Vector2 playerColliderBottomLeftPos = new Vector2(transform.position.x - bodyCollider2D.size.x*0.5f + bodyCollider2D.center.x,
		                                                   transform.position.y - spriteRenderer.bounds.extents.y*1.2f);	// Collider Bottom Left

//		Debug.DrawLine(playerColliderTopLeftPos, playerColliderBottomRightPos, Color.yellow);
//		Debug.DrawLine(playerColliderBottomLeftPos, playerColliderTopRightPos, Color.yellow);
		Debug.DrawLine(playerColliderTopLeftPos, playerColliderTopRightPos, Color.yellow);
		Debug.DrawLine(playerColliderTopLeftPos, playerColliderBottomRightPos, Color.yellow);
		Debug.DrawLine(playerColliderTopRightPos, playerColliderBottomLeftPos, Color.yellow);
		Debug.DrawLine(playerColliderBottomLeftPos, playerColliderBottomRightPos, Color.yellow);


		/**
		 * check if standing on activ jumpPlatform
		 **/

//		LayerMask jumpOnPlatform = 1 << layer.jumpAblePlatform;
//Layer.class
		//Collider2D foundCollider = Physics2D.OverlapArea(playerColliderTopLeftPos, playerColliderBottomRightPos, jumpOnPlatform);

		bool platformGrounded = false;

		//int overlapCount = Physics2D.OverlapAreaNonAlloc(playerColliderTopLeftPos, playerColliderBottomRightPos, foundColliderArray, jumpOnPlatform );
		foundColliderArray[0] = null;
		Physics2D.OverlapAreaNonAlloc(playerColliderTopLeftPos, playerColliderBottomRightPos, foundColliderArray, layer.whatIsJumpOnPlatform );

		if(foundColliderArray[0] != null)
		{
			// Collider aus JumpOnPlatform Ebene (Laye) inerhalb des definierten Bereiches gefunden
			// yellow zone collids with jumpOnPlatform

			// kontrollieren ob Kollision zwischen Platform und groundStopper ignoriert wird (Ignorierung/Berücksichtigung wird von PlatformJumperScript durchgeführt)
			// wenn Kollision ignoriert wird dann ist der Character zurzeit nicht am Fallen sondern am Springen

			if(Physics2D.GetIgnoreCollision(foundColliderArray[0], myGroundStopperCollider))
			{
				// true => Kollision mit gefundener JumpOnPlatform wird ignoriert (deaktiviert)
				platformGrounded = false;
			}
			else
			{
				// false => Kollision mit gefundener JumpOnPlatform wird berücksichtigt (aktiv)
				if(moveDirection.y <= 0)
				{
					platformGrounded = true;
				}
			}
		}


		/**
		 * 	Checking if standing on solid/static groundCollider
		 **/


//		LayerMask ground = 1 << layer.block;
//		ground |= 1 << layer.ground;
//Layer.class

		grounded = false;

		if(!platformGrounded)
		{
			// KEINEN Collider aus JumpOnPlatform Ebene (Laye) inerhalb des definierten Bereiches gefunden
			// yellow zone doesn't collid with jumpOnPlatform Collider

			platformGrounded = false;
			foundColliderArray[0] = null;
			Physics2D.OverlapAreaNonAlloc(playerColliderTopLeftPos, playerColliderBottomRightPos, foundColliderArray, layer.whatIsStaticGround);
			if(foundColliderArray[0] != null)
			{
				grounded = true;
			}
			else
			{
				grounded = false;
			}
		}

		if(grounded || platformGrounded)		// grounded unnötig
		{
			grounded = true;
		}

		/**
		 * Walled
		 **/

		//		walled = Physics2D.OverlapCircle(playerPos+wallCheckPosition, wallRadius, layer.whatIsWall);
		//Debug.DrawLine(playerPos, playerPos+wallCheckPosition + 1*transform.localScale.x * new Vector2(wallRadius,0), Color.green);
	}

//	void SetAnim() 
//	{
//		if(anim == null)
//		{
//			Debug.LogError("Animator not set");
//		}
//		else
//		{
//			anim.SetBool(hash.groundedBool, grounded);
//			anim.SetBool(hash.walledBool, walled);
//			anim.SetFloat(hash.vSpeedFloat, rigidbody2D.velocity.y);
//			anim.SetFloat(hash.hSpeedFloat, rigidbody2D.velocity.x);
////			if(gameObject.name.StartsWith("Kirby"))
////		   	{	
////				Debug.Log(gameObject.name + ": " + rigidbody2D.velocity);
////			}
//		}
//	}

	float gravity = 30; // 8
	public Vector3 moveDirection = Vector3.zero;
	float jumpPower = 14; // 7

	private bool kinematic = false;
	private bool overrideGrounded = false;
	private bool overrideGroundedValue = false;

	public void Simulate()
	{
		CheckPosition();
		SimulateAnimation();

		if(isDead)
		{
			moveDirection.x = 0f;
			if(kinematic)
			{
				moveDirection.y = 0f;
			}
			else
			{
				if(overrideGrounded)
				{
					if(overrideGroundedValue)
					{
						//grounded (overriden)
						moveDirection.y = 0f;
					}
					else
					{
						moveDirection.y -= gravity * Time.fixedDeltaTime;
					}
				}
				else
				{
					if(grounded)
						moveDirection.y = 0f;			// fix: HeadJumped, fall on ground
					else
						moveDirection.y -= gravity * Time.fixedDeltaTime;
				}
			}
			transform.Translate( moveDirection * Time.fixedDeltaTime );
		}
		else
		{
			moveDirection.x = inputScript.inputHorizontal * currentSpeed;	// Horizontal Movement

			// Vertical Movement
			if(grounded)
			{
				if(moveDirection.y <=0)			// jump fix
				{
					moveDirection.y = 0;
				}

				if(inputScript.inputJump && moveDirection.y == 0)				//  && moveDirection.y <= 0
				{
	//				if(moveDirection.y <= 0f)			// verhindern das sound öfter abgespielt wird!! .... achtung sprung wird trotzdem öfter asugeführt kann  
					SyncJump();
					moveDirection.y = jumpPower;
				}
			}
			else
			{
				if(kinematic)
					moveDirection.y = 0f;
				else
					moveDirection.y -= gravity * Time.fixedDeltaTime;
			}

			transform.Translate( moveDirection * Time.fixedDeltaTime );
		}
		CheckBeam ();
	}
	bool hasHorizontalInputShown = false;
	bool hasNoHorizontalInputShown = false;
	void SimulateAnimation()
	{
		anim.SetBool(hash.groundedBool, grounded);
		anim.SetBool(hash.walledBool, walled);

		//if(inputScript.inputHorizontal != 0f)
		if(moveDirection.x != 0f)							// nochmal kontrollieren warum ich diese abfrage mache... (umständlich zweimal anim.SetFloat(hSpeed...)
		{
//			if(Network.isServer)
//			{
//				if(ownerScript.owner != Network.player)
//				{
//					Debug.Log(moveDirection.x);
//				}
//			}
//			if(!hasHorizontalInputShown)
//			{
//				hasHorizontalInputShown = true;
//				Debug.Log(moveDirection.x);							//switched from inputHorizontal (between -1 and 1) to moveDirection.x
//			}
			hasNoHorizontalInputShown = false;	
			anim.SetFloat(hash.hSpeedFloat, moveDirection.x);
			if(facingRight && moveDirection.x < 0)
			{
				Flip ();
			}
			else if( !facingRight  && moveDirection.x > 0)
			{
				Flip ();
			}
		}
		else
		{
			anim.SetFloat(hash.hSpeedFloat,moveDirection.x);

			hasHorizontalInputShown = false;
			if(!hasNoHorizontalInputShown)
			{
				hasNoHorizontalInputShown = true;
				//Debug.Log(this.ToString() + ": no Input");
			}
		}
	}

	[RPC]
	public void SyncJump()
	{
		// Do Jump
		if(jumpSound != null)
			AudioSource.PlayClipAtPoint(jumpSound,transform.position,1);				//JumpSound
		else
			Debug.LogError("jumpSound nicht gesetzt!");
		if(anim == null)
		{
			Debug.LogError("Animator not set");
		}
		else
		{
			anim.SetBool(hash.groundedBool,false);
		}
	}

	[RPC]
	public void SyncWallJump()
	{
		// Do WallJump
		AudioSource.PlayClipAtPoint(wallJumpSound,transform.position,1);			//WallJump
		Flip();																		//Charakter drehen
		if(anim == null)
		{
			Debug.LogError("Animator not set");
		}
		else
		{
			anim.SetBool(hash.groundedBool,false);
			anim.SetBool(hash.walledBool,false);
		}
	}
	
	public void StartJump() {
		if(jumpAllowed)
			inputJump = true;
	}
	
	public void StopJump() {
		inputJump = false;
	}
	
	
	void Flip() {
		
		// Drift sound abspielen
		if(grounded)
		{
			changedRunDirection = true;
			if(anim == null)
			{
				Debug.LogError("Animator not set");
			}
			else
			{
				anim.SetTrigger(hash.changeRunDirectionTrigger);	// Start Change Run Direction Animation
			}
			if(changeRunDirectionSound != null)
				AudioSource.PlayClipAtPoint(changeRunDirectionSound,transform.position,1);				//ChangeDirection
			else
				Debug.LogError("change run direction sound fehlt!");
		}
		
		// Richtungvariable anpassen
		facingRight = !facingRight;
		
		// WallCheck anpassen
		wallCheckPosition *= -1;
		
		// Transform spiegeln
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
		
	}

	[RPC]
	void DeactivateKinematic()
	{
		gameObject.rigidbody2D.isKinematic = false;
		gameObject.rigidbody2D.WakeUp();
	}
	
	[RPC]
	void ActivateKinematic()
	{
		gameObject.rigidbody2D.isKinematic = true;
		gameObject.rigidbody2D.WakeUp();
	}
	
	[RPC]
	public void CollectedItem(int itemId, NetworkMessageInfo info)
	{
		ItemLibrary.getItem(itemId).Collected(this, info);
	}


	public void CollectingItem(ItemScript collectingItem)
	{
		if(!isAuthoritativeHost())
		{
			return;
		}

		
		if(collectingItem == null)
		{
			Debug.LogError(collectingItem.name + " has no Item Script attached!!!");
			return;
		}

		if(!CharacterCanCollectItems())
		{
			Debug.LogWarning(this.ToString() + " can't collect items right now!");
			return;
		}
		
		collectingItem.item.Collecting(collectingItem.gameObject, this);
	}


	// wird auf server ausgeführt, kontrolleirt ob character und spieler item einsammeln dürfen.
	public void CollectingItem(GameObject goItem)
	{
		if(!isAuthoritativeHost())
		{
			return;
		}
		// runs on server/offline only

		Item currentItem = goItem.GetComponent<ItemScript>().item;		// muss genaues Script erhalten <-- Item is wrong

		// lösung: statt ontriggerenter function an character, ontriggerenterfunction an itemscript
		// collectorCharacterScript.CollectingItem(this (:Item));

		if(currentItem == null)
		{
			Debug.LogError(goItem.name + " has no Item Script attached!!!");
			return;
		}


		/**
		 *  Polymorphie in CharacterCanCollectItems... CharacterStates  myCharacterState.collecting(goItem)?
		 * 																myCharacterState.collecting(currentItem)?
		 * 
		 **/

		if(!CharacterCanCollectItems())
		{
			Debug.LogWarning(this.ToString() + " can't collect items right now!");
            return;
        }
        
        currentItem.Collecting(goItem, this);

		//
		/**
		 * DONE, no more if's!!!!			GO destroy, if(item.destroyAfterCollecting) Destroy go;
		 * 									currentItem.Collecting(this, goItem);	// collecting itself destroys the item (offline/online)
		 **/

		bool destroyItem = true;

		//TODO Polymorphism
		return;


//		if(currentItem.itemName == "Star")
//		{
//			//GetComponent<RageModus>().StartRageModus();
//			if(offline())
//			{
//				NetworkMessageInfo bla = new NetworkMessageInfo();
//				Debug.Log("Time.time " + Time.time);
//				Debug.Log("Network.time " + Network.time);
//				Debug.Log("NetworkMessageInfo: timestamp " + bla.timestamp);
//				//bla.timestamp = Network.time; geht nicht
//				GetComponent<RageModus>().StartRageModus(bla);
//			}
//			if(server())
//				networkView.RPC("StartRageModus", RPCMode.All);
//
//		}
//		else if(currentItem.itemName == "FireFlower")
//		{
//			if(offline())
//				StartCoroutine(SpawnBullet());
//			if(server())
//			{
//				StartCoroutine(SpawnBullet());				// spezzialfall... bullets werden von server gemanaged (authoritativ )
//				//	myNetworkView.RPC ("", RPCMode.All);		// an alle?? eigentlich nur an Spieler	
//			}
//		}
//		else if(currentItem.itemName == "FireFlower2")
//		{
//			// aktiviere Power Button für Character
//			myNetworkView.RPC("ActivatePower", RPCMode.All, "FireFlower2");
//		}
//		else if(currentItem.itemName == "BoBomb")
//		{
//
//		}
//		else if(currentItem.itemName == "1up")
//		{
//
//		}
//		else if(currentItem.itemName == "2up")
//		{
//
//		}
//		else if(currentItem.itemName == "3up")
//		{
//
//		}
//		else if(currentItem.itemName == "5up")
//		{
//
//		}
//		else
//		{
//			Debug.LogWarning("unknown Item found! " + goItem.name + " " + currentItem.itemName);
//		}
//
//
//		if(destroyItem)
//		{
//			if(offline())
//			{
//				Destroy(goItem);
//			}
//			if(server())
//			{
//				Network.RemoveRPCs(goItem.networkView.viewID);
//				Network.Destroy(goItem);
//			}
//		}

	}

	public bool canUsePowerButton = false;
	public bool hasItem = false;

	public bool powerPredictedAnimation = false;
	int powerPredictedAnimationState = 0;
	int inputPowerCount = 0;

	public void PowerPredictedAnimation()
	{
		powerPredictedAnimation = true;
		powerPredictedAnimationState = inputPowerCount++ % predictedShootAnimation.Length;
	}

	public Power power1;
	public Power power2;

	public void Power()
	{
		if(isAuthoritativeHost())
		{
			power1.activate();
		}
//		if(item == "FireFlower2")
//		{
//			if(isAuthoritativeHost())
//			{
//				SpawnSingleBullet();
//			}
//		}
	}

	[RPC]
	public void authoritativePowerStart()
	{
		// bullet instantiated by server...
		if(isAuthoritativeHost())
		{
			SpawnSingleBullet();
		}
	}


	public void SpawnSingleBullet()
	{
		GameObject bulletPrefab = (GameObject) Resources.Load("PowerUps/"+"FireBall", typeof(GameObject));
		if(bulletPrefab == null)
		{
			Debug.LogError("bulletPrefab coudn't be loaded!!!! check path / and name");
			return;
		}
		GameObject bulletGameObject = null;
		if(server())
		{
			bulletGameObject = (GameObject) Network.Instantiate( bulletPrefab, new Vector3(this.transform.localScale.x * bulletSpawnPositionOffset.x,1* bulletSpawnPositionOffset.y,1* bulletSpawnPositionOffset.z) + this.transform.position, Quaternion.identity, 0);
		}
		else if(offline())
		{
			bulletGameObject = (GameObject) Instantiate(bulletPrefab, new Vector3(this.transform.localScale.x * bulletSpawnPositionOffset.x,1* bulletSpawnPositionOffset.y,1* bulletSpawnPositionOffset.z) + this.transform.position, Quaternion.identity);
		}
		
		bulletGameObject.GetComponent<AuthoritativeBullet>().ownerCharacter = this.gameObject;// important!!!
		bulletGameObject.GetComponent<AuthoritativeBullet>().moveDirection = new Vector3(this.transform.localScale.x,0,0);
		bulletGameObject.rigidbody2D.velocity = new Vector3(this.transform.localScale.x * AuthoritativeBullet.moveSpeed.x,1 * AuthoritativeBullet.moveSpeed.y,1* AuthoritativeBullet.moveSpeed.z);

	}

	string item ="";

	[RPC]
	void ActivatePower(string item)
	{
		this.item = item;
		canUsePowerButton = true;
		hasItem = true;
	}

	[RPC]
	void DeactivatePower()
	{
		canUsePowerButton = false;
		hasItem = false;
		item = "";
	}

	bool CharacterCanCollectItems()
	{
		// TODO more Exceptions...
		
		//if(gameMode.collectItems == false)
		//	return false;
		
		//if(myCharacterScript.inventar.isFull())
		// return false;
		
		if(isDead)
			return false;
		
		return true;
	}

	Vector3 bulletSpawnPositionOffset = new Vector3(0.75f,0,0);
	int bulletsLeftCount = 3;
	
	
	bool isAuthoritativeHost()
	{
		if(offline ())
			return true;
		
		if(server ())
			return true;
		
		return false;
		
	}
	
	bool offline()
	{
		if(Network.peerType == NetworkPeerType.Disconnected)
			return true;
		
		return false;
	}
	
	bool server()
	{
		if(Network.isServer)
			return true;
		
		return false;
	}
	
	IEnumerator SpawnBullet()
	{
		if(isAuthoritativeHost())
		{
			if(bulletsLeftCount < 3)
				yield return new WaitForSeconds (2);
			
			bulletsLeftCount --;
			GameObject bulletPrefab = (GameObject) Resources.Load("PowerUps/"+"FireBall", typeof(GameObject));
			if(bulletPrefab == null)
			{
				Debug.Log("bulletPrefab coudn't be loaded!!!! check path / and name");
			}
			GameObject bulletGameObject = null;
			if(server())
			{
				bulletGameObject = (GameObject) Network.Instantiate( bulletPrefab, new Vector3(this.transform.localScale.x * bulletSpawnPositionOffset.x,1* bulletSpawnPositionOffset.y,1* bulletSpawnPositionOffset.z) + this.transform.position, Quaternion.identity, 0);
			}
			else if(offline())
			{
				bulletGameObject = (GameObject) Instantiate(bulletPrefab, new Vector3(this.transform.localScale.x * bulletSpawnPositionOffset.x,1* bulletSpawnPositionOffset.y,1* bulletSpawnPositionOffset.z) + this.transform.position, Quaternion.identity);
			}
			
			bulletGameObject.GetComponent<AuthoritativeBullet>().ownerCharacter = this.gameObject;// important!!!
			bulletGameObject.GetComponent<AuthoritativeBullet>().moveDirection = new Vector3(this.transform.localScale.x,0,0);
			bulletGameObject.rigidbody2D.velocity = new Vector3(this.transform.localScale.x * AuthoritativeBullet.moveSpeed.x,1 * AuthoritativeBullet.moveSpeed.y,1* AuthoritativeBullet.moveSpeed.z);
			
			if(bulletsLeftCount > 0)
				StartCoroutine(SpawnBullet());
			else
				bulletsLeftCount = 3;
		}
	}

	Vector3 authoritativeSpawnPosition;
	
	Level currentLevel;
	
	bool debugSpawn = true;
	float reSpawnDelayTime = 2f;
	float reSpawnDelayTimeNetwork = 2f;
	
	bool spawnProtection = false;
	float spawnProtectionTime = 2f;
	Color[] spawnProtectionAnimation;
	Color[] predictedShootAnimation;

	public void HeadJumpVictim()
	{
		isHit = true;
		anim.SetBool(hash.spawnBool,false);
		anim.SetBool(hash.gameOverBool,false);
		anim.SetBool(hash.headJumpedBool,false);
		anim.SetBool(hash.deadBool,false);
		anim.SetTrigger(hash.hitTrigger);			// Lösung!
		
		// Death Sound abspielen
		AudioSource.PlayClipAtPoint(deathSound,transform.position,1);

		//Animation setzen
		anim.SetBool(hash.headJumpedBool,true);

		/**
		 * Physics
		 * 
		 * // Layer Collisionen mit Gegenspieler und PowerUps ignorieren, GameObject soll aber auf Boden/Platform fallen und liegen bleiben
		 **/

		// GameObject soll aber auf Boden/Platform fallen und liegen bleiben
		myGroundStopperCollider.enabled = true;

		// kann keine Items mehr sammeln
		itemCollectorCollider2D.enabled = false;

		// Body BoxCollider2D deaktivieren (Gegenspieler können durchlaufen)
		bodyCollider2D.enabled = false;
		//myBodyTrigger.enabled = false;
		
		// FeetCollider deaktivieren (Gegenspieler nehmen keinen Schaden mehr)
		feetCollider2D.enabled = false;
		
		// HeadCollider deaktivieren (Spieler kann nicht nochmal Schaden durch HeadJump-Angriff nehmen da er tot ist)
		headCollider2D.enabled = false;

		// PowerUp Hit Collider deaktivieren (Spieler kann keinen Schaden mehr durch PowerUp-Angriff nehmen da er tot ist)
		powerUpCollider2D.enabled = false;
		
		/* Ki und Controlls deaktivieren */
		isDead = true;
		isHit = false;				// <-- kann wieder auf false gesetzt werden, da Spieler jetzt tot ist!


		if(server())
		{
			networkView.RPC("SpawnAnimationDelay", RPCMode.All, currentLevel.getRandomSpawnPosition() );			// wenn server jetzt schon rpc mit spawnposition sended
																											// und reSpawnDelay > triptime
																											// kann syncron auf allen clients die spawnanimation starten
																											// ansonsten, nur auf server spawndelay coroutine starten// und nach ablauf rpc an alle clients startanimation
			//networkView.RPC("SetSpawnPosition", RPCMode.All, currentLevel.getRandomSpawnPosition() );

			// spieler so zeitnah wie möglich spielen lassen
		}
		// StartCoroutine(SpawnAnimation)... in kombination mit //networkView.RPC("SetSpawnPosition", RPCMode.All, currentLevel.getRandomSpawnPosition() );

		// server erhält erst input wenn spieler !isDead
	}

	[RPC]
	void SpawnAnimationDelay(Vector3 spawnPosition, NetworkMessageInfo info)
	{
		authoritativeSpawnPosition = spawnPosition;
		double rpcTripTime = Network.time - info.timestamp;
		
		if(rpcTripTime >= reSpawnDelayTime)
		{
			reSpawnDelayTimeNetwork = 0f;
		}
		else
			reSpawnDelayTimeNetwork = reSpawnDelayTime - (float)rpcTripTime;
		
		Debug.Log("SpawnAnimationDelay RPC trip time: " + rpcTripTime);
		Debug.Log("reSpawnDelayTime: " + reSpawnDelayTime);
		Debug.Log("reSpawnDelayTimeNetwork: " + reSpawnDelayTimeNetwork);
		
		
		StartCoroutine(SpawnDelay());
	}
	


	IEnumerator SpawnDelay()
	{
		if(debugSpawn && this.transform.name.StartsWith("Carbuncle"))
			Debug.LogWarning("CoRoutine: SpawnDelay()");
		yield return new WaitForSeconds(reSpawnDelayTimeNetwork);
		StartSpawnAnimation();
	}

	public void StartSpawnAnimation()
	{
		if(debugSpawn && this.transform.name.StartsWith("Carbuncle"))
			Debug.LogWarning("StartSpawnAnimation()");
		this.transform.renderer.enabled = false;				// sieht besser aus macht eigentlich kein unterschied, da kein neuer frame erstellt wird bis render aktiviert wird
		
		// neue Position halten
		//rigidbody2D.isKinematic = true;				// BUG BUG BUG BUG BUG
		
		//		if(server())
		//		{
		//			// Random Spawn Position
		//			SetSpawnPosition();
		//		}
		
		this.transform.position = authoritativeSpawnPosition;
		
		// schwachsinn, spawnAnimation startet!
		//CheckPosition();
		//SimulateAnimation();
		
		// Spawn Animation
		anim.SetBool(hash.spawnBool, true);
		
		kinematic = true;	// bleibt in luft hängen für die zeit der animation, spieler input ist auch deaktiviert (durch isDead)
		// Kinematic = true, alle Collider & Trigger aus (bis auf groundStopper und body (World stopper)
		
		// kann keine items während spawnanimation einsammeln
		itemCollectorCollider2D.enabled = false;
		
		// kann nicht von powerups getroffen werden
		powerUpCollider2D.enabled = false;
		
		// FeetCollider deaktivieren (Gegenspieler nehmen Schaden)
		feetCollider2D.enabled = false;
		
		// HeadTrigger deaktivieren, (in SpawnProtection nicht angreifbar)
		headCollider2D.enabled = false;
		
		// falls spawn position im boden ist 
		bodyCollider2D.enabled = true;						
		myGroundStopperCollider.enabled = true;
		
		/* Ki und Controlls deaktivieren */
		isDead = true;
		
		this.transform.renderer.enabled = true;				// sieht besser aus macht eigentlich kein unterschied, da kein neuer frame seit dem deaktivieren erzeugt wurde
	}
	
	void LateUpdate()
	{
		if(!spawnProtection)
		{
			if(anim.GetCurrentAnimatorStateInfo(0).nameHash == hash.spawnProtectionState)
			{
				if(debugSpawn && this.transform.name.StartsWith("Carbuncle"))
					Debug.LogWarning("SpawnProtectionState");
				spawnProtection = true;	// coroutine ist zu langsam, wird sonst zweimal gestartet!
				anim.SetTrigger(hash.nextStateTrigger);	// spawnprotection state verlassen
				// Spawn Animation finished!
				// nach SpawnAnimation Collider & Trigger auf SpawnProtection setzen
				SpawnProtection();
				// SpawnProtection Timer starten
				
			}

			if(powerPredictedAnimation)
			{
				spriteRenderer.color = predictedShootAnimation[powerPredictedAnimationState];
			}
			else
			{
				spriteRenderer.color = new Color(1f,1f,1f,1f);
			}
		}
		else //if(spawnProtection)
		{
			// in spawnProtection
			spriteRenderer.color = spawnProtectionAnimation[0];
		}
	}

	void SpawnProtection()
	{
		headCollider2D.enabled = false;
		feetCollider2D.enabled = true;		// kann angreifen
		//bodyCollider2D.enabled = false;										// zwischen body und world stopper unterscheiden?
		itemCollectorCollider2D.enabled = true;	// kann items sammeln
		powerUpCollider2D.enabled = false;
		
		myGroundStopperCollider.enabled = true;	// kann auf boden landen
		bodyCollider2D.enabled = true;			// collidiert mit level
		
		isDead = false;
		isHit = false;
		
		kinematic = false;
		
		StartCoroutine(SpawnProtectionTime());
	}
	
	IEnumerator SpawnProtectionTime()
	{
		if(debugSpawn && this.transform.name.StartsWith("Carbuncle"))
			Debug.LogWarning("CoRoutine: SpawnProtection()");
		spawnProtection = true;
		yield return new WaitForSeconds(spawnProtectionTime);
		spawnProtection = false;
		SpawnComplete();
	}
	
	void SpawnComplete()
	{
		if(debugSpawn && this.transform.name.StartsWith("Carbuncle"))
			Debug.LogWarning("SpawnComplete()");
		spriteRenderer.color = new Color(1f,1f,1f,1f);	// transparenz entfernen
		Fighting();
	}
	
	public void Fighting()
	{
		headCollider2D.enabled = true;
		feetCollider2D.enabled = true;
		bodyCollider2D.enabled = true;
		myGroundStopperCollider.enabled = true;
		itemCollectorCollider2D.enabled = true;
		powerUpCollider2D.enabled = true;


		// Debugging
		// Error not found... collision is not ignored!
//		int foundCount = Physics2D.OverlapAreaNonAlloc(new Vector2(currentLevel.left,currentLevel.bottom),
//		                              new Vector2(currentLevel.width-Mathf.Abs(currentLevel.left),currentLevel.height-Mathf.Abs(currentLevel.bottom)),
//		                              allCurrentColliders,
//		                              layer.whatIsStaticGround);
//
//		for(int i = 0; i< foundCount; i++)
//		{
//			Collider2D collider = allCurrentColliders[i];
//			//Debug.Log(bodyCollider2D.transform.parent.name + ", " + collider.name + " ignored: " + Physics2D.GetIgnoreCollision(myGroundStopperCollider, collider));
//			Debug.Log(bodyCollider2D.transform.parent.name);
//			Debug.Log(collider.name);
//			Debug.Log(" ignored: " + Physics2D.GetIgnoreCollision(myGroundStopperCollider, collider));
//
//			Physics2D.IgnoreCollision(bodyCollider2D, collider, false);
//			Physics2D.IgnoreCollision(myGroundStopperCollider, collider, false);
//		}

	}

//	Collider2D[] allCurrentColliders = new Collider2D[100];

	//[RPC]
	void SetSpawnPosition()
	{
		//		float newPositionX = Random.Range(0.0f, 19.0f);
		//		float newPositionY = Random.Range(2f, 15.0f);
		//		float oldPositionZ = myCharacter.transform.position.z;
		//		myCharacter.gameObject.transform.position = new Vector3(newPositionX,newPositionY,oldPositionZ);
		this.transform.position = currentLevel.getRandomSpawnPosition();
	}

//	void Dead()
//	{
//		headCollider2D.enabled = false;
//		feetCollider2D.enabled = false;
//		bodyCollider2D.enabled = false;
//		itemCollectorCollider2D.enabled = false;		
//		powerUpCollider2D.enabled = false;
//		myGroundStopperCollider.enabled = false;
//	}

	public void InvincibleAttackVictim()
	{
		isHit = true;
		anim.SetBool(hash.spawnBool,false);
		anim.SetBool(hash.gameOverBool,false);
		anim.SetBool(hash.headJumpedBool,false);
		anim.SetBool(hash.deadBool,false);
		anim.SetTrigger(hash.hitTrigger);			// Lösung!

		// Death Sound abspielen
		AudioSource.PlayClipAtPoint(deathSound,transform.position,1);

		//NoHeadJump();
		//Animation setzen
		anim.SetBool(hash.deadBool,true);
		//SetCharacterColliderDead();
		// Layer Collisionen mit Gegenspieler und PowerUps ignorieren, GameObject soll aber auf Boden/Platform fallen und liegen bleiben
		
		// Body BoxCollider2D deaktivieren (Gegenspieler können durchlaufen)
		bodyCollider2D.enabled = false;
		
		// FeetCollider deaktivieren (Gegenspieler nehmen keinen Schaden mehr)
		feetCollider2D.enabled = false;
		
		// HeadCollider deaktivieren (Spieler kann nicht nochmal schaden nehmen)
		headCollider2D.enabled = false;

		// kann keine Items mehr sammeln
		itemCollectorCollider2D.enabled = false;

		// kann nicht mehr durch powerups angegriffen werden
		powerUpCollider2D.enabled = false;

		// aus bildbereich fallen
		myGroundStopperCollider.enabled = false;


		//DeadAnimationPhysics();
		// TODO
		//rigidbody2D.velocity = new Vector2(0f, 10f);
		bool invincibleVictimAnimation = true;
		bool headJumpVictimAnimation = false;
		overrideGrounded = true;
		overrideGroundedValue = false;
		moveDirection.y = jumpPower;
		moveDirection.x = 0f;
		transform.Translate( moveDirection * Time.fixedDeltaTime );
		// TODO

		//myReSpawnScript.StartReSpawn();
		isDead = true;
		isHit = false;

		if(server())
		{
			networkView.RPC("SpawnAnimationDelay", RPCMode.All, currentLevel.getRandomSpawnPosition() );			// wenn server jetzt schon rpc mit spawnposition sended
			// und reSpawnDelay > triptime
			// kann syncron auf allen clients die spawnanimation starten
			// ansonsten, nur auf server spawndelay coroutine starten// und nach ablauf rpc an alle clients startanimation
			//networkView.RPC("SetSpawnPosition", RPCMode.All, currentLevel.getRandomSpawnPosition() );
			
			// spieler so zeitnah wie möglich spielen lassen
		}
	}

	public void InvincibleMode()
	{
		headCollider2D.enabled = false;
		feetCollider2D.enabled = false;			// keine headjumps!
		bodyCollider2D.enabled = false;
		powerUpCollider2D.enabled = true;		// bemerkt powerUp (bullets umlenken, owner überschreiben) aber nicht getroffen werden! -> if-abfrage!
		myGroundStopperCollider.enabled = true;
		itemCollectorCollider2D.enabled = false; // kann keine items einsammeln
	}

	void InitSpawnProtectionAnimation()
	{
		spawnProtectionAnimation = new Color[1];
		spawnProtectionAnimation [0] = new Color (1f, 1f, 1f, 0.5f);	// alpha channel = 0.5
	}

	void InitPredictedShootAnimation()
	{
		predictedShootAnimation = new Color[8];
		predictedShootAnimation [0] = new Color (0f, 0f, 0f, 1f);	// alpha channel = 0.5
		predictedShootAnimation [1] = new Color (0f, 0f, 1f, 1f);	// alpha channel = 0.5
		predictedShootAnimation [2] = new Color (0f, 1f, 0f, 1f);	// alpha channel = 0.5
		predictedShootAnimation [3] = new Color (0f, 1f, 1f, 1f);	// alpha channel = 0.5
		predictedShootAnimation [4] = new Color (1f, 0f, 0f, 1f);	// alpha channel = 0.5
		predictedShootAnimation [5] = new Color (1f, 0f, 1f, 1f);	// alpha channel = 0.5
		predictedShootAnimation [6] = new Color (1f, 1f, 0f, 1f);	// alpha channel = 0.5
		predictedShootAnimation [7] = new Color (1f, 1f, 1f, 1f);	// alpha channel = 0.5
	}


}
