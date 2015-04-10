using UnityEngine;
using System.Collections;

public class RageModus : MonoBehaviour {

	private float rageTime = 3*3.202f;
	private float rageTimeNetwork = 0;
	public bool isInRageModus = false;
	private float oldMaxSpeed;

	// Sound
	public AudioClip invincibleAudioClip;
	private GameObject invincibleSound;

	Color[] rageAnimationColors;
	int currentAnimColorIndex = 0;

	private float rageMaxSpeed;
	/** 
	 * Character Animation 
	 **/
	//	private SpriteController spriteController;
//	public Animator anim;									// Animator State Machine

	/** 
	 * Connection to GameController 
	 **/
	private GameObject gameController;
	private HashID hash;
	private Layer layer;
	private StatsManager statsManager;

	/**
	 * Connection to other Body parts
	 **/
	private Collider2D bodyCollider2D;
	private Collider2D headCollider2D;
	private Collider2D feetCollider2D;

	private PlatformCharacter myPlatformCharacter;
	private SpriteRenderer mySpriteRenderer;
	void InitRageAnimation()
	{
		rageAnimationColors = new Color[6];

//		for(int i=0; i<rageAnimationColors.Length; i++)
//		{
//			rageAnimationColors[i].r = 1;
//			rageAnimationColors[i].g = 0;
//			rageAnimationColors[i].b = 0;
//			rageAnimationColors[i].a = 1;
//		}
		int i=0;
		rageAnimationColors[i] = new Color(1f, 0f, 0f, 1f);
//		rageAnimationColors[i].r = 1;
//		rageAnimationColors[i].g = 0;
//		rageAnimationColors[i].b = 0;
//		rageAnimationColors[i].a = 1;
		i++;
		rageAnimationColors[i] = new Color(1f, 1f, 0f, 1f);
//		rageAnimationColors[i].r = 1;
//		rageAnimationColors[i].g = 1;
//		rageAnimationColors[i].b = 0;
//		rageAnimationColors[i].a = 1;
		i++;
		rageAnimationColors[i] = new Color(0f, 1f, 0f, 1f);
//		rageAnimationColors[i].r = 0;
//		rageAnimationColors[i].g = 1;
//		rageAnimationColors[i].b = 0;
//		rageAnimationColors[i].a = 1;
		i++;
		rageAnimationColors[i] = new Color(0f, 1f, 1f, 1f);
//		rageAnimationColors[i].r = 0;
//		rageAnimationColors[i].g = 1;
//		rageAnimationColors[i].b = 1;
//		rageAnimationColors[i].a = 1;
		i++;
		rageAnimationColors[i] = new Color(0f, 0f, 1f, 1f);
//		rageAnimationColors[i].r = 0;
//		rageAnimationColors[i].g = 0;
//		rageAnimationColors[i].b = 1;
//		rageAnimationColors[i].a = 1;
		i++;
		rageAnimationColors[i] = new Color(1f, 1f, 1f, 1f);
//		rageAnimationColors[i].r = 1;
//		rageAnimationColors[i].g = 1;
//		rageAnimationColors[i].b = 1;
//		rageAnimationColors[i].a = 1;
//		rageAnimationColors[1] = Color.yellow;
//		rageAnimationColors[2] = Color.green;
//		rageAnimationColors[3] = Color.cyan;
//		rageAnimationColors[4] = Color.blue;
//		rageAnimationColors[5] = Color.white;
	}

	void Awake()
	{
		try {
			invincibleSound = GameObject.FindGameObjectWithTag(Tags.tag_invincibleSound);
		}catch(System.Exception e)
		{
			Debug.LogError( "GameObject invincibleSound nicht in Scene gefunden!!!" );
		}
		InitRageAnimation();
		mySpriteRenderer = GetComponent<SpriteRenderer>();
		myPlatformCharacter = GetComponent<PlatformCharacter>();
//		anim = GetComponent<Animator>();
		bodyCollider2D = GetComponent<BoxCollider2D>();
		headCollider2D = transform.FindChild(Tags.name_head).GetComponent<BoxCollider2D>();
		feetCollider2D = transform.FindChild(Tags.name_feet).GetComponent<BoxCollider2D>();
		
		gameController = GameObject.FindGameObjectWithTag(Tags.tag_gameController);
		hash = gameController.GetComponent<HashID>();
		layer = gameController.GetComponent<Layer>();
		statsManager = gameController.GetComponent<StatsManager>();
	}

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void LateUpdate () {
		if(isInRageModus)
		{
//			Debug.Log("currentAnimColor:" + currentAnimColor);
//			Debug.Log("SpriteRenderer Color:" + mySpriteRenderer.color);
			currentAnimColorIndex = currentAnimColorIndex % rageAnimationColors.Length;
			mySpriteRenderer.color = rageAnimationColors[currentAnimColorIndex];
//			Debug.Log("new SpriteRenderer Color:" + mySpriteRenderer.color);
			currentAnimColorIndex++;

		}
	}

	void playSound()
	{
		if(invincibleSound != null)
		{
			gameController.GetComponent<AudioSource>().Stop();
			invincibleSound.GetComponent<AudioSource>().Play();
		}

		if(invincibleAudioClip != null)
		{
			gameController.GetComponent<AudioSource>().Stop();
			this.GetComponent<AudioSource>().loop = true;
			this.GetComponent<AudioSource>().clip = invincibleAudioClip;
			this.GetComponent<AudioSource>().Play();
		}
	}

	void stopSound()
	{
		if(invincibleSound != null)
		{
			invincibleSound.GetComponent<AudioSource>().Stop();
			gameController.GetComponent<AudioSource>().Play();
		}
		if(invincibleAudioClip != null)
		{
			this.GetComponent<AudioSource>().Stop();
			gameController.GetComponent<AudioSource>().Play();
		}
	}

	[RPC]
	public void StartRageModus(NetworkMessageInfo info)
	{

		double rpcTripTime = Network.time - info.timestamp;
		if(info.timestamp != 0)
		{
			if(rpcTripTime >= rageTime)
			{
				rageTimeNetwork = 0f;
			}
			else
				rageTimeNetwork = rageTime - (float)rpcTripTime;
		}
		else
		{
			// offline 
			rageTimeNetwork = rageTime;
		}

		Debug.Log(this.ToString() + " rpcTripTime: " + rpcTripTime);
		Debug.Log(this.ToString() + " rageTimeNetwork: " + rageTimeNetwork);

		//TO DONE characterScript.maxSpeed
		//TO DONE characterScript.currentSpeed
		//DONE
		oldMaxSpeed = myPlatformCharacter.getMaxSpeed();
		rageMaxSpeed = oldMaxSpeed * 1.2f;
		myPlatformCharacter.currentSpeed = rageMaxSpeed;


//		headCollider2D.enabled = false;						// wird benötigt um blocks zu zerstören... SendDamageTrigger wird durch isInRageMode abgebrochen
		//TODO doku: 
		//	angreifer und opfer müssen abgefragt werden um headjump zu verifizieren.
		// angreifer muss leben und angreifen dürfen
		// opfer muss leben und angreifbar sein
		// opfer braucht headtrigger noch für andere aktionen, daher muss er aktiv bleiben
		// wird feet für etwas anderes benutzt? falls nein -> collision in headtriggerscript auswerten, kein getcomponent zugrif auf anderen spieler notwendig...
		feetCollider2D.enabled = false;

		playSound();

		myPlatformCharacter.InvincibleMode();		// sets collider

//		bodyCollider2D.isTrigger = false;

//		disableCollision(); erkennt bevorstehende collisionen mit anderen bodyCollider und deaktiviert collisionen unter den beiden collider
		
		isInRageModus = true;
		myPlatformCharacter.isInRageModus = true;
//		if(anim != null)
//		{
//			anim.SetBool(hash.rageModusBool,true);
//			anim.SetTrigger(hash.rageTrigger);
//		}
		Debug.LogError(gameObject.name + "isInRageModus: On");
		//		InventoryManager.inventory.SetItems("Star(Clone)",0f);
		StartCoroutine(RageTime());
	}

	IEnumerator RageTime()
	{
		yield return new WaitForSeconds(rageTimeNetwork);
		stopRageModus();
	}

	public void stopRageModus()
	{
		Debug.LogError(gameObject.name + "isInRageModus: Off");
//		if(anim != null)
//		{
//			anim.SetBool(hash.rageModusBool,false);
//		}
		mySpriteRenderer.color = Color.white;
//		enableCollision();

		stopSound();

		isInRageModus = false;
		myPlatformCharacter.isInRageModus = false;
//		headCollider2D.enabled = true;				// wurde nicht deaktiviert... // wird benötigt um blocks zu zerstören... SendDamageTrigger wird durch isInRageMode abgebrochen
		feetCollider2D.enabled = true;
		myPlatformCharacter.currentSpeed = myPlatformCharacter.getMaxSpeed();

		myPlatformCharacter.Fighting();		// sets colliders
//		bodyCollider2D.isTrigger = false;
		
		//anim.SetBool(hash.hasPowerUpBool,hasPowerUp);
		//AudioSource.PlayClipAtPoint(powerUpReloadedSound,transform.position,1);
	}
}
