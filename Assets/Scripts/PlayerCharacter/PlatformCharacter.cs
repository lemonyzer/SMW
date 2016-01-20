using UnityEngine;
using System.Collections;

public class PlatformCharacter : MonoBehaviour {

	//TODO überschreibt vererbung, zum cachen
	new public Transform transform;
	public Rigidbody2D rb2d;

	[SerializeField]
	private SmwCharacter myCharScriptableObject;

	public SmwCharacter GetSmwCharacterSO()
	{
		return myCharScriptableObject;
	}

	public void SetSmwCharacterSO(SmwCharacter charSO)
	{
		this.myCharScriptableObject = charSO;
	}
	
	public delegate void OnCharacterRegistered(NetworkPlayer netPlayer, Player player);
	public static event OnCharacterRegistered onRegistered;

	public delegate void OnLateJoinerInstantiateNetworkCharacter(NetworkPlayer netPlayer, Player player, int teamId);
	public static event OnLateJoinerInstantiateNetworkCharacter onLateJoinerInstantiateNetworkCharacter;

	public delegate void OnRageKill(GameObject killer, GameObject victim);
	public static event OnRageKill onRageKill;

	public delegate void OnHeadJump(GameObject killer, GameObject victim);
	public static event OnHeadJump onHeadJump;
	
	public delegate void OnProjectileHit(GameObject killer, GameObject victim);
	public static event OnProjectileHit onProjectileHit;

	public delegate void OnBlastHit(GameObject killer, GameObject victim);
	public static event OnBlastHit onBlastHit;

	public delegate void OnBulletHit(GameObject killer, GameObject victim);
	public static event OnBulletHit onBulletHit;

	public delegate void OnBombHit(GameObject killer, GameObject victim);
	public static event OnBombHit onBombHit;


	HeadJump myHeadJump;

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
//	private float inputVelocity = 0f;
//	private bool inputJump = false;	
//	private float inputPCVelocity = 0f;
//	private bool inputPCJump = false;							// stores Input Key 
//	private float inputTouchVelocity = 0f;
//	private bool inputTouchJump = false;							// stores Input Key 

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
//	StatsManager statsManager;
//	private HashID hash;
//	private Layer layer;
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


	public Transform lastReceivedPos;
	public SpriteRenderer lastReceivedPosRenderer;

	public Transform currentEstimatedPosOnServer;
	public SpriteRenderer currentEstimatedPosOnServerRenderer;

	public Transform predictedPosSimulatedWithLastInput;
	public SpriteRenderer predictedPosSimulatedWithLastInputRenderer;

	public Transform predictedPosCalculatedWithLastInput;
	public SpriteRenderer predictedPosCalculatedWithLastInputRenderer;

//	public Transform predictedPosV3;
//	public SpriteRenderer predictedPosV3Renderer;

	public Transform iceWalled;
	public SpriteRenderer iceWalledRenderer;


	/**
	 * Pre-Instantiated GameObjects
	 **/
//	private GameObject bullet;			// should be public in a gameController script

//	public void setMaxSpeed(float newMaxSpeed)
//	{
//		maxSpeed = newMaxSpeed;
//	}

	/// <summary>
	/// Gets the max speed.
	/// </summary>
	/// <returns>The max speed.</returns>
	public float getMaxSpeed()
	{
		return maxSpeed;
	}

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		this.transform = GetComponent<Transform>();
		myGroundStopperCollider = transform.Find(Tags.name_groundStopper).GetComponent<BoxCollider2D>();
		myNetworkView = GetComponent<NetworkView>();

		inputScript = GetComponent<PlatformUserControl>();
		myHeadJump = GetComponent<HeadJump>();

		spriteRenderer = GetComponent<SpriteRenderer>();

		// Bodyparts
		bodyCollider2D = transform.FindChild(Tags.name_body).GetComponent<BoxCollider2D>();
		headCollider2D = transform.FindChild(Tags.name_head).GetComponent<BoxCollider2D>();
		feetCollider2D = transform.FindChild(Tags.name_feet).GetComponent<BoxCollider2D>();
		itemCollectorCollider2D = transform.FindChild(Tags.name_itemCollector).GetComponent<BoxCollider2D>();
		powerUpCollider2D = transform.FindChild(Tags.name_powerUpHitArea).GetComponent<BoxCollider2D>();

		currentEstimatedPosOnServer = transform.FindChild(Tags.name_CurrentEstimatedPosOnServer);
		currentEstimatedPosOnServerRenderer = currentEstimatedPosOnServer.GetComponent<SpriteRenderer>();

		lastReceivedPos = transform.FindChild(Tags.name_lastReceivedPos);
		lastReceivedPosRenderer = lastReceivedPos.GetComponent<SpriteRenderer>();

		predictedPosSimulatedWithLastInput = transform.FindChild(Tags.name_PredictedPosSimulatedWithLastInput);
		predictedPosSimulatedWithLastInputRenderer = predictedPosSimulatedWithLastInput.GetComponent<SpriteRenderer>();

		predictedPosCalculatedWithLastInput = transform.FindChild(Tags.name_PredictedPosCalculatedWithLastInput);
		predictedPosCalculatedWithLastInputRenderer = predictedPosCalculatedWithLastInput.GetComponent<SpriteRenderer>();

//		predictedPosV3 = transform.FindChild(Tags.name_PredictedPosV3);
//		predictedPosV3Renderer = predictedPosV3.GetComponent<SpriteRenderer>();

		iceWalled = transform.FindChild(Tags.name_iceWalled);
		iceWalledRenderer = iceWalled.GetComponent<SpriteRenderer>();

		gameController = GameObject.FindGameObjectWithTag(Tags.name_gameController);
//		statsManager = gameController.GetComponent<StatsManager>();
		currentLevel = gameController.GetComponent<Level>();
//		hash = gameController.GetComponent<HashID>();
//		layer = gameController.GetComponent<Layer>();

		InitSpawnProtectionAnimation();
		InitPredictedShootAnimation();
//		gameSceneManager = gameController.GetComponent<GameSceneManager>();

		//LayerMasks();	// <-- wichtig in Start... Awake ist zu früh
		rb2d = GetComponent<Rigidbody2D>();
		rb2d.gravityScale = 1;
	}

//	void LayerMasks()
//	{
//		whatIsGround = 1 << layer.floor;
//		whatIsGround |= 1 << layer.jumpAblePlatform;
//		whatIsGround |= 1 << layer.destroyAbleBlock;
//	}

	void AnimatorScriptsInitialisierung()
	{
		SpawnStateScript spawnScript = anim.GetBehaviour<SpawnStateScript>();
		if(spawnScript != null)
		{
			spawnScript.myCharacter = this;
		}
		else
			Debug.LogError("spawnScript not found!!!");

		SpawnDelayStateScript spawnDelayScript = anim.GetBehaviour<SpawnDelayStateScript>();
		if(spawnDelayScript != null)
		{
			spawnDelayScript.myCharacter = this;
		}
		else
			Debug.LogError("spawnDelayScript not found!!!");
	}


	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start() {
		anim = GetComponent<Animator>();

		AnimatorScriptsInitialisierung();

		ownerScript = GetComponent<RealOwner>();

		if(Application.loadedLevelName == "UnityNetworkRace")
		{
			beamEnabled = false;
		}
	}

	// FixedUpdate is called once per frame
	/// <summary>
	/// Enables Movement Offline
	/// </summary>
	void FixedUpdate () {
		if(Network.isClient)
			return;

		if(Network.isServer)
			return;
		

		// net mode: simulate is called by networkedplayer
		// offline: simulate is called here
		if(Network.peerType == NetworkPeerType.Disconnected)
		{
			// offline movement
			Simulate();
		}
	}

	public bool platformJump = false;
	BoxCollider2D myGroundStopperCollider;

	bool beamEnabled = true;

	/// <summary>
	/// Checks Characterposition and beams if nessesary.
	/// No Unity physics engine needed (no trigger no collider)
	/// </summary>
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
		if(transform.position.x < -10.5f)
		{
			playerPos.x += 20f;
		}
		else if(transform.position.x > 10.5f)
		{
			playerPos.x -= 20f;
		}

		if(transform.position.y < -7.5f)
		{
			playerPos.y += 15f;
		}
		else if(transform.position.y > 7.5f)
		{
			playerPos.y -= 15f;
		}
		
		transform.position = playerPos;
	}

	Collider2D[] foundColliderArray = new Collider2D[1];

	/// <summary>
	/// Checks the position of Character.
	/// grounded, walled
	/// Disables Platformcollider while jumping
	/// </summary>
	void CheckPosition()
	{

		Vector2 playerColliderOffset = new Vector2(0.1f,0.0f); // FIX player jump at wall and get grounded

		//playerPos spriterenderer boundaries
		Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);

		Vector2 groundedOffset = new Vector2(0f,0.5f);

		Vector2 playerColliderTopLeftPos = new Vector2(transform.position.x - bodyCollider2D.size.x*0.5f + bodyCollider2D.offset.x,
		                                               transform.position.y);	// Collider Top Left
		
		Vector2 playerColliderBottomRightPos = new Vector2(transform.position.x + bodyCollider2D.size.x*0.5f + bodyCollider2D.offset.x,
		                                                   transform.position.y - spriteRenderer.bounds.extents.y*1.2f);	// Collider Bottom Right

		Vector2 playerColliderTopRightPos = new Vector2(transform.position.x + bodyCollider2D.size.x*0.5f + bodyCollider2D.offset.x,
		                                               transform.position.y);	// Collider Top Right
		
		Vector2 playerColliderBottomLeftPos = new Vector2(transform.position.x - bodyCollider2D.size.x*0.5f + bodyCollider2D.offset.x,
		                                                   transform.position.y - spriteRenderer.bounds.extents.y*1.2f);	// Collider Bottom Left
		#if UNITY_EDITOR
//		Debug.DrawLine(playerColliderTopLeftPos, playerColliderBottomRightPos, Color.yellow);
//		Debug.DrawLine(playerColliderBottomLeftPos, playerColliderTopRightPos, Color.yellow);
		Debug.DrawLine(playerColliderTopLeftPos, playerColliderTopRightPos, Color.yellow);
		Debug.DrawLine(playerColliderTopLeftPos, playerColliderBottomRightPos, Color.yellow);
		Debug.DrawLine(playerColliderTopRightPos, playerColliderBottomLeftPos, Color.yellow);
		Debug.DrawLine(playerColliderBottomLeftPos, playerColliderBottomRightPos, Color.yellow);

		Debug.DrawLine(playerColliderTopLeftPos + playerColliderOffset, playerColliderTopRightPos - playerColliderOffset, Color.white);
		Debug.DrawLine(playerColliderTopLeftPos + playerColliderOffset, playerColliderBottomRightPos - playerColliderOffset, Color.white);
		Debug.DrawLine(playerColliderTopRightPos - playerColliderOffset, playerColliderBottomLeftPos + playerColliderOffset, Color.white);
		Debug.DrawLine(playerColliderBottomLeftPos - playerColliderOffset, playerColliderBottomRightPos + playerColliderOffset, Color.white);
		#endif

		/**
		 * check if standing on activ jumpPlatform
		 **/

//		LayerMask jumpOnPlatform = 1 << layer.jumpAblePlatform;
//Layer.class
		//Collider2D foundCollider = Physics2D.OverlapArea(playerColliderTopLeftPos, playerColliderBottomRightPos, jumpOnPlatform);

		bool platformGrounded = false;

		//int overlapCount = Physics2D.OverlapAreaNonAlloc(playerColliderTopLeftPos, playerColliderBottomRightPos, foundColliderArray, jumpOnPlatform );
		foundColliderArray[0] = null;
		Physics2D.OverlapAreaNonAlloc(playerColliderTopLeftPos + playerColliderOffset, playerColliderBottomRightPos - playerColliderOffset, foundColliderArray, Layer.whatIsJumpOnPlatform );

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
			Physics2D.OverlapAreaNonAlloc(playerColliderTopLeftPos + playerColliderOffset, playerColliderBottomRightPos - playerColliderOffset, foundColliderArray, Layer.whatIsStaticGround);
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
		//#if UNITY_EDITOR
		//Debug.DrawLine(playerPos, playerPos+wallCheckPosition + 1*transform.localScale.x * new Vector2(wallRadius,0), Color.green);
		//#endif
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

	public Vector3 moveDirection = Vector3.zero;
	[SerializeField]
	float gravity = 30; // 8
	[SerializeField]
	float jumpPower = 14; // 7

	[SerializeField]
	private bool kinematic = false;
	[SerializeField]
	private bool overrideGrounded = false;
	[SerializeField]
	private bool overrideGroundedValue = false;

	private bool useUnityPhysics = true;
	private float rigibodyMoveForce = 40f; // 7
	private float rigibodyJumpForce = 1000f; // 7
	private float rigibodyMaxSpeed = 7.5f; // 7

	/// <summary>
	/// Simulation, first checks Position (ground, walled states)
	/// Updates Animation
	/// use Input and Move (Translation)
	/// checks Beam after Movement
	/// </summary>
	public void Simulate()
	{
		// Position checken (inAir -> jump nicht möglich)
		CheckPosition();

		if (!useUnityPhysics)
		{
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
				moveDirection.x = inputScript.GetInputHorizontal() * currentSpeed;	// Horizontal Movement

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
		}
		else
		{
			//TODO
			// NEW
			//TODO
			moveDirection.x = inputScript.GetInputHorizontal();
			// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
			if(moveDirection.x * rb2d.velocity.x < rigibodyMaxSpeed)
				// ... add a force to the player.
				rb2d.AddForce(Vector2.right * moveDirection.x * rigibodyMoveForce);

			if(grounded && (rb2d.velocity.y <= 0.1f))
			{
				if (inputScript.inputJump)
				{
					SyncJump();
					moveDirection.y = rigibodyJumpForce;

					// Add a vertical force to the player.
					rb2d.AddForce(new Vector2(0f, rigibodyJumpForce));
					moveDirection.y = 0f;
				}
			}
		}
		CheckBeam ();
		CheckPosition();
		SimulateAnimation();
	}

	

	/// <summary>
	/// Simulates the animation.
	/// </summary>

	void SimulateAnimation()
	{
		anim.SetBool(HashID.groundedBool, grounded);
		anim.SetBool(HashID.walledBool, walled);

		anim.SetFloat(HashID.hSpeedFloat, moveDirection.x);
		if(facingRight && moveDirection.x < 0)
		{
			Flip ();
		}
		else if( !facingRight  && moveDirection.x > 0)
		{
			Flip ();
		}
	}

	/// <summary>
	/// Syncs the jump on all NetworkPlayer
	/// </summary>

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
			anim.SetBool(HashID.groundedBool,false);
		}
	}

	/// <summary>
	/// Syncs the wall jump.
	/// </summary>

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
			anim.SetBool(HashID.groundedBool,false);
			anim.SetBool(HashID.walledBool,false);
		}
	}
	
//	public void StartJump() {
//		if(jumpAllowed)
//			inputJump = true;
//	}
//	
//	public void StopJump() {
//		inputJump = false;
//	}
	
	
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
				anim.SetTrigger(HashID.changeRunDirectionTrigger);	// Start Change Run Direction Animation
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
		gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
		gameObject.GetComponent<Rigidbody2D>().WakeUp();
	}
	
	[RPC]
	void ActivateKinematic()
	{
		gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
		gameObject.GetComponent<Rigidbody2D>().WakeUp();
	}
	
	[RPC]
	public void CollectedItem_Rpc(int itemId, NetworkMessageInfo info)
	{
		double collectedTimeStamp = info.timestamp;
#if UNITY_EDITOR
		Debug.LogWarning("Collected TimeStamp: " + collectedTimeStamp);
#endif
		ItemLibrary.getItem(itemId).Collected(this, collectedTimeStamp);
	}

	/// <summary>
	/// Collectings the item.
	/// </summary>
	/// <param name="collectingItem">Collecting item.</param>
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

		// Call items Collect routine (can destroy itemGo after collecting)
		ItemLibrary.getItem(collectingItem.itemId).Collecting(collectingItem.gameObject, this);
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

	private Power power1;
	private Power power2;

	public Power GetPower()
	{
		return power1;
	}

	public void SetPower(Power newPower)
	{
		RemovePower();
		power1 = newPower;
	}

	public void RemovePower()
	{
		if(power1 != null)
		{
			power1.lost();
		}
	}


	public void Power()
	{
		if(isAuthoritativeHost())
		{
			power1.activate();
		}
	}

//	[RPC]
//	public void authoritativePowerStart()
//	{
//		// bullet instantiated by server...
//		if(isAuthoritativeHost())
//		{
//			SpawnSingleBullet();
//		}
//	}




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
	
//	IEnumerator SpawnBullet()
//	{
//		if(isAuthoritativeHost())
//		{
//			if(bulletsLeftCount < 3)
//				yield return new WaitForSeconds (2);
//			
//			bulletsLeftCount --;
//			GameObject bulletPrefab = (GameObject) Resources.Load("PowerUps/"+"FireBall", typeof(GameObject));
//			if(bulletPrefab == null)
//			{
//				Debug.Log("bulletPrefab coudn't be loaded!!!! check path / and name");
//			}
//			GameObject bulletGameObject = null;
//			if(server())
//			{
//				bulletGameObject = (GameObject) Network.Instantiate( bulletPrefab, new Vector3(this.transform.localScale.x * bulletSpawnPositionOffset.x,1* bulletSpawnPositionOffset.y,1* bulletSpawnPositionOffset.z) + this.transform.position, Quaternion.identity, 0);
//			}
//			else if(offline())
//			{
//				bulletGameObject = (GameObject) Instantiate(bulletPrefab, new Vector3(this.transform.localScale.x * bulletSpawnPositionOffset.x,1* bulletSpawnPositionOffset.y,1* bulletSpawnPositionOffset.z) + this.transform.position, Quaternion.identity);
//			}
//			
//			bulletGameObject.GetComponent<AuthoritativeBullet>().ownerCharacter = this.gameObject;// important!!!
//			bulletGameObject.GetComponent<AuthoritativeBullet>().moveDirection = new Vector3(this.transform.localScale.x,0,0);
//			bulletGameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(this.transform.localScale.x * AuthoritativeBullet.moveSpeed.x,1 * AuthoritativeBullet.moveSpeed.y,1* AuthoritativeBullet.moveSpeed.z);
//			
//			if(bulletsLeftCount > 0)
//				StartCoroutine(SpawnBullet());
//			else
//				bulletsLeftCount = 3;
//		}
//	}

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
		//RemovePower();
		isHit = true;
		anim.SetBool(HashID.spawnBool,false);
		anim.SetBool(HashID.gameOverBool,false);
		anim.SetBool(HashID.headJumpedBool,false);
		anim.SetBool(HashID.deadBool,false);
		anim.SetTrigger(HashID.hitTrigger);			// Lösung!
		
		// Death Sound abspielen
		AudioSource.PlayClipAtPoint(deathSound,transform.position,1);

		//Animation setzen
		anim.SetBool(HashID.headJumpedBool,true);

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
			GetComponent<NetworkView>().RPC("SpawnAnimationDelay", RPCMode.All, currentLevel.getRandomSpawnPosition() );			// wenn server jetzt schon rpc mit spawnposition sended
																											// und reSpawnDelay > triptime
																											// kann syncron auf allen clients die spawnanimation starten
																											// ansonsten, nur auf server spawndelay coroutine starten// und nach ablauf rpc an alle clients startanimation
			//networkView.RPC("SetSpawnPosition", RPCMode.All, currentLevel.getRandomSpawnPosition() );

			// spieler so zeitnah wie möglich spielen lassen
		}
		// StartCoroutine(SpawnAnimation)... in kombination mit //networkView.RPC("SetSpawnPosition", RPCMode.All, currentLevel.getRandomSpawnPosition() );

		// server erhält erst input wenn spieler !isDead
	}

	//TODO 
	public void StartBetterSpawnDelay()
	{
												// unabhängig von animationslängen von vorherigen states nach einem treffer!!
//		StartCoroutine(BetterSpawnDelay());		// aufruf wird von StateMachine SpawnDelayStateScript gefordert -> genauer!!!
	}

	IEnumerator BetterSpawnDelay()
	{
		#if UNITY_EDITOR
		if(debugSpawn && this.transform.name.StartsWith("Carbuncle"))
			Debug.LogWarning("CoRoutine: SpawnDelay()");
		#endif
		yield return new WaitForSeconds(reSpawnDelayTimeNetwork);
		StartSpawnAnimation();
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
		#if UNITY_EDITOR
		if(debugSpawn && this.transform.name.StartsWith("Carbuncle"))
			Debug.LogWarning("CoRoutine: SpawnDelay()");
		#endif
		yield return new WaitForSeconds(reSpawnDelayTimeNetwork);
		StartSpawnAnimation();
	}

	public void StartSpawnAnimation()
	{
		#if UNITY_EDITOR
		if(debugSpawn && this.transform.name.StartsWith("Carbuncle"))
			Debug.LogWarning("StartSpawnAnimation()");
		#endif
		this.transform.GetComponent<Renderer>().enabled = false;				// sieht besser aus macht eigentlich kein unterschied, da kein neuer frame erstellt wird bis render aktiviert wird
		
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
		anim.SetBool(HashID.spawnBool, true);
		
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
		
		this.transform.GetComponent<Renderer>().enabled = true;				// sieht besser aus macht eigentlich kein unterschied, da kein neuer frame seit dem deaktivieren erzeugt wurde
	}

	public Material predictionMaterial;

//	void LateUpdate()
//	{
////		return;
//		if(!spawnProtection)
//		{
////			Debug.Log("nicht in SparnProtection");
//			if(anim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashID.spawnState)
////			if(anim.GetCurrentAnimatorStateInfo(0).nameHash == HashID.spawnProtectionState)
//			{
//				#if UNITY_EDITOR
//				if(debugSpawn && this.transform.name.StartsWith("Carbuncle"))
//					Debug.LogError("SpawnProtectionState");
//				#endif
//				spawnProtection = true;	// coroutine ist zu langsam, wird sonst zweimal gestartet!
//				anim.SetTrigger(HashID.nextStateTrigger);	// spawnprotection state verlassen
//				// Spawn Animation finished!
//				// nach SpawnAnimation Collider & Trigger auf SpawnProtection setzen
//				SpawnProtection();
//				// SpawnProtection Timer starten
//				
//			}
//
//			if(powerPredictedAnimation)
//			{
////				Debug.LogWarning("in powerPredictedAnimation");
//				
////				spriteRenderer.color = predictedShootAnimation[powerPredictedAnimationState];		// wurde von Rage LateUpdate () übeschrieben!!! geht jetzt nicht mehr
//																									// TODO andere powerPredictionAnimaation!!!
//			}
//			else
//			{
////				Debug.LogWarning("nicht in powerPredictedAnimation");
//				
////				spriteRenderer.color = new Color(1f,1f,1f,1f);
//			}
//		}
//		else //if(spawnProtection)
//		{
//			Debug.LogWarning("in SparnProtection");
//			// in spawnProtection
//			//spriteRenderer.color = spawnProtectionAnimation[0];
//		}
//
//
//		if(isAuthoritativeHost())
//		{
//			lastReceivedPosRenderer.enabled = true;
//			currentEstimatedPosOnServerRenderer.enabled = false;
//			predictedPosCalculatedWithLastInputRenderer.enabled = false;
//			predictedPosSimulatedWithLastInputRenderer.enabled = false;
////			predictedPosV3Renderer.enabled = false;
//			return;
//		}
//
////		lastReceivedPosRenderer.sprite = spriteRenderer.sprite;					// other player character
////		lastReceivedPosRenderer.color = new Color(0f,0f,0f,0.3f);
////
////		currentEstimatedPosOnServerRenderer.sprite = spriteRenderer.sprite;		// local player character
////		currentEstimatedPosOnServerRenderer.color = new Color(0f,0f,0f,0.3f);
////
////		predictedPosCalculatedWithLastInputRenderer.sprite = spriteRenderer.sprite;		// other player character
////		predictedPosCalculatedWithLastInputRenderer.color = new Color(0f,1f,0f,0.2f);
////		predictedPosCalculatedWithLastInputRenderer.material = predictionMaterial;
////
////		predictedPosSimulatedWithLastInputRenderer.sprite = spriteRenderer.sprite;		// other player character
////		predictedPosSimulatedWithLastInputRenderer.color = new Color(1f,0f,0f,0.2f);
////		predictedPosCalculatedWithLastInputRenderer.material = predictionMaterial;
//	}

	public void Protection()
	{
		anim.SetTrigger(HashID.startProtectionTrigger);
//		anim.SetBool(HashID.protectionBool,true);

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
		#if UNITY_EDITOR
		if(debugSpawn && this.transform.name.StartsWith("Carbuncle"))
			Debug.LogWarning("CoRoutine: SpawnProtection()");
		#endif
		spawnProtection = true;
		yield return new WaitForSeconds(spawnProtectionTime);
		spawnProtection = false;
		SpawnComplete();
	}
	
	void SpawnComplete()
	{
#if UNITY_EDITOR
		if(debugSpawn && this.transform.name.StartsWith("Carbuncle"))
			Debug.LogWarning("SpawnComplete()");
#endif
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
		anim.SetBool(HashID.spawnBool,false);
		anim.SetBool(HashID.gameOverBool,false);
		anim.SetBool(HashID.headJumpedBool,false);
		anim.SetBool(HashID.deadBool,false);
		anim.SetTrigger(HashID.hitTrigger);			// Lösung!

		// Death Sound abspielen
		AudioSource.PlayClipAtPoint(deathSound,transform.position,1);

		//NoHeadJump();
		//Animation setzen
		anim.SetBool(HashID.deadBool,true);
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
			GetComponent<NetworkView>().RPC("SpawnAnimationDelay", RPCMode.All, currentLevel.getRandomSpawnPosition() );			// wenn server jetzt schon rpc mit spawnposition sended
			// und reSpawnDelay > triptime
			// kann syncron auf allen clients die spawnanimation starten
			// ansonsten, nur auf server spawndelay coroutine starten// und nach ablauf rpc an alle clients startanimation
			//networkView.RPC("SetSpawnPosition", RPCMode.All, currentLevel.getRandomSpawnPosition() );
			
			// spieler so zeitnah wie möglich spielen lassen
		}
	}

	public void InvincibleMode()
	{
		anim.SetTrigger(HashID.p_rageModusBool);
		anim.SetTrigger(HashID.p_rageTrigger);
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

	/// <summary>
	/// Registers the character game object in player dictionary_ rpc.
	/// </summary>
	/// <param name="netPlayerOwner">Net player owner.</param>
	[RPC]
	void RegisterCharacterGameObjectInPlayerDictionary_Rpc(NetworkPlayer netPlayerOwner, string userName, int teamId)
	{
		Debug.LogWarning("RegisterCharacterGameObjectInPlayerDictionary_Rpc is running for netPlayer " + netPlayerOwner.ToString());

		Player player = PlayerDictionaryManager._instance.GetPlayerAndCreateIfNotInDictionary(netPlayerOwner);
		if(player != null)
		{
			// setzte Username
			player.setUserName(userName);

			// setze Platform Character Script
			player.platformCharacterScript = this;

			Debug.LogWarning("netPlayer " + player.getUserName() + " " + netPlayerOwner.ToString() + " -> player.platformCharacterScript set");

			if(player.UIStatsSlotScript == null)
			{

				Debug.LogError("Spieler " + player.getUserName() + " " + netPlayerOwner.ToString() + " hat kein UIStatsSlotScript -> ich bin late joiner und muss jetzt Slots erstellen");

				// LateJoin Spieler muss in seiner Player Classe noch den SMWCharacter (ScriptableObject) setzten
				// diese Information ist für jeden Character individuell und hängt daher jetzt direkt im PlatformCharacterScript!! Build > 701
				// LateJoin Spieler braucht Character informationen
				player.characterScriptableObject = this.GetSmwCharacterSO(); //TODO auch bei UIStatsSlotScript != null ?!!!

//-> blabla				// Eigene Farbe und von Anderen Spielern bekommt er von UnityNetworkManager OnPlayerConnected -> OnPlayerConnected_Rpc gesendet

				if(onLateJoinerInstantiateNetworkCharacter != null)
				{
					onLateJoinerInstantiateNetworkCharacter(netPlayerOwner, player, teamId);
				}
				else
					Debug.LogError(this.ToString() + " no onCharacterRegistered() listeners");

			}
			else
			{
				if(onRegistered != null)
				{
					onRegistered(netPlayerOwner, player);
				}
				else
					Debug.LogError(this.ToString() + " no onCharacterRegistered() listeners");
			}
		}
		else
		{
			Debug.LogError("RegisterCharacterGameObjectInPlayerDictionary_Rpc failed: NetworkPlayer not in playerDicionary and Could not be created!!");
		}
	}

	bool isIced = false;

	public void IcedTriggered()
	{
		if(!CanGetIced())
			return;

		isIced = true;

		if(Network.isServer)
		{
			myNetworkView.RPC("Iced_Rpc", RPCMode.AllBuffered);
		}
	}

	[RPC]
	public void Iced_Rpc()
	{
		StartCoroutine(Iced());
	}

	public float icedTime = 4f;
	IEnumerator Iced()
	{
		StartIced();
		yield return new WaitForSeconds(icedTime);
		if(!destroyedWhileIced)
		{
			IcedMelted();
		}
		else
		{
			Debug.Log("destroyedWhileIced");
		}
	}

	public void StartIced()
	{
		destroyedWhileIced = false;
		iceWalled.GetComponent<Animator>().SetTrigger(HashID.icedTrigger);
		iceWalledRenderer.enabled = true;
		inputScript.enabled = false;
		inputScript.inputJump = false;
		inputScript.inputPower = false;
		inputScript.SetInputHorizontal(0f);

	}

	bool destroyedWhileIced = false;

	public void IcedMelted()
	{
		iceWalled.GetComponent<Animator>().SetTrigger(HashID.iceBlockMeltTrigger);
		EndIced();
	}

	public void EndIced()
	{
		// endIce bool
		isIced = false;
		
		// invincible (spawnschutz)
		
		// disable renderer
		//		iceWalledRenderer.enabled = false;
		
		//animation disables it!
		
		// disable boxcollider trigger
		
		// enable input (movement)
		inputScript.enabled = true;
	}




//	public class DamageTrigger : MonoBehaviour {
//
//		public void AttackFailed()
//		{
//			
//		}
//
//		public void AttackSuccessfull()
//		{
//
//		}
//	}
//
//	public void AttackTriggered(DamageTrigger attackerScript)
//	{
//		if(!CanTakeDamage())
//		{
//			attackerScript.AttackFailed();
//			return;
//		}
//
//		attackerScript.AttackSuccessfull();
//	}

	public void Attacker_AttackSuccessfull()
	{
//		spawnProtection = false;
//		SpawnComplete();
	}

	public void Victim_AttackTriggered(RageTrigger attackerRageTriggerScript)
	{
		if(!CanTakeDamage())
			return;
		TakeDamage();
		PlatformCharacter attacker = attackerRageTriggerScript.myCharacterScript;

		if(onRageKill != null)
		{
			onRageKill(attacker.gameObject, this.gameObject);
		}
		else
		{
			Debug.LogWarning("onRageKill no listeners!");
		}

		//attacker punkte ++ (RPC)
		
		//victim (this) leben -- (RPC)
		
		//victim (this) die animation (RPC)
	}

	public void Victim_AttackTriggered(SendDamageTrigger attackerFeetScript)
	{
		if(!CanTakeDamage())
			return;
		TakeDamage();
		//HeadJump
		
		//attacker spawnprotection = false setzen!!
		PlatformCharacter attacker = attackerFeetScript.myCharacterScript;
		attacker.Attacker_AttackSuccessfull();

		if(onHeadJump != null)
		{
			onHeadJump(attacker.gameObject, this.gameObject);
		}
		else
		{
			Debug.LogWarning("onHeadJump no listeners!");
		}
		
		//attacker punkte ++ (RPC)
		
		//victim (this) leben -- (RPC)
		
		//victim (this) headjump animation (RPC)
		
	}

	public void Victim_AttackTriggered(AuthoritativeProjectile projectileScript, bool makeDamage)
	{
		if(!makeDamage)	// WandBlast makes no damage
			return;

		if(!CanTakeDamage())
			return;
		TakeDamage();
		//Bullet Hit (FireBall, Bomb, WandBlast)
		
		//attacker spawnprotection = false setzen!!
		PlatformCharacter attacker = projectileScript.ownerCharacter;
		attacker.Attacker_AttackSuccessfull();


		if(onProjectileHit != null)
		{
			onProjectileHit(attacker.gameObject, this.gameObject);
		}
		else
		{
			Debug.LogWarning("onProjectileHit no listeners!");
		}
		//attacker punkte ++ (RPC)
		
		//victim (this) leben -- (RPC)
		
		//victim (this) die animation (RPC)
		
	}

//	public void AttackTriggered(ItemCollectorScript attackerRageTriggerScript)
//	{
//		if(!CanTakeDamage())
//			return;
//		PlatformCharacter attacker = attackerRageTriggerScript.myCharacterScript;
//		
//	}

	[RPC]
	void AttackedWhileIced()
	{
		iceWalled.GetComponent<Animator>().SetTrigger(HashID.iceBlockBreakTrigger);	// wird nicht per RPC übertragen
		destroyedWhileIced = true;

		EndIced();
	}

	public void TakeDamage()
	{
		isHit = true;
		if(isIced)
		{
			myNetworkView.RPC("AttackedWhileIced", RPCMode.All);	//TODO check RPCMode
//			iceWalled.GetComponent<Animator>().SetTrigger(HashID.iceBlockBreakTrigger);	// wird nicht per RPC übertragen
//			destroyedWhileIced = true;
		}
	}

	public bool CanGetIced()
	{
		if(CanTakeDamage())
		{
			if(isIced)
			{
				return false;
			}
			return true;
		}
		//if(isHit || isDead || isInRageModus ||  isIced)

		return false;
	}

	public bool CanTakeDamage()
	{
		if(isDead || isHit || spawnProtection || isInRageModus)
		{
			return false;
		}
		else
			return true;
	}

	public bool CanHeadJump()
	{
		// physikalische eigenschafetn nicht getestet (muss beim headjump nach unten bewegen)
		return CanAttack();
	}

	public bool CanRageAttack()
	{
		if(CanAttack())
			return isInRageModus;
		else
			return false;
	}

	public bool CanAttack()
	{
		if( !isDead && !isHit )
			return true;

		return false;
	}

}
