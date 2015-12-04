using UnityEngine;
using System.Collections;

/**
 * V3: optimized
 * V2: 
 * no extra Layer for each Player needed!!!!
 * no JumpSaveZone needed
 **/

public class PlatformJumperV2 : MonoBehaviour {

	//TODO überschreibt vererbung, zum cachen
	new public Transform transform;


	// relevant world stoppers
	BoxCollider2D bodyCollider;
	BoxCollider2D groundStopper;

	GameObject gameController;
	PlatformCharacter myPlatformCharacter;
	Layer layer;
	public LayerMask jumpOnPlatform;

	SpriteRenderer spriteRenderer;

	void Awake()
	{
		this.transform = GetComponent<Transform>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		gameController = GameObject.FindGameObjectWithTag(Tags.tag_gameController);
		layer = gameController.GetComponent<Layer>();
		myPlatformCharacter = GetComponent<PlatformCharacter>();

		bodyCollider = transform.Find(Tags.name_body).GetComponent<BoxCollider2D>();

//		BoxCollider2D[] myBody = transform.Find(Tags.body).GetComponents<BoxCollider2D>();
//		if(myBody == null)
//			return;
//		foreach(BoxCollider2D coll in myBody)
//		{
//			if(coll.isTrigger)
//			{
//				bodyCollider = coll;
//			}
//			else
//			{
//				bodyCollider = coll;
//			}
//		}

		// same
//		groundStopper = transform.FindChild(Tags.groundStopper).GetComponent<BoxCollider2D>();
//		Debug.Log(groundStopper.name);
		groundStopper = transform.Find(Tags.name_groundStopper).GetComponent<BoxCollider2D>();
//		Debug.Log(groundStopper.name);
	}

	// Use this for initialization
	void Start () {
		jumpOnPlatform = 1 << Layer.jumpAblePlatform;
		CalculateColliderEdges();
	}

	Vector2 playerBodyColliderTopLeftPos;
	Vector2 playerBodyColliderBottomRightPos;
	Vector2 playerBodyColliderBottomLeftPos;
//	Vector2 playerGroundStopperColliderBottomLeftPos;

	Vector2 platformColliderFinderTopLeftPos;
	Vector2 platformColliderFinderBottomRightPos;


	void CalculateColliderEdges()
	{
		playerBodyColliderTopLeftPos = new Vector2(transform.position.x - bodyCollider.size.x*0.5f + bodyCollider.offset.x,
		                                           transform.position.y + bodyCollider.size.y*0.5f + bodyCollider.offset.y);	// Collider Top Left
		
		playerBodyColliderBottomRightPos = new Vector2(transform.position.x + bodyCollider.size.x*0.5f + bodyCollider.offset.x,
		                                               transform.position.y - bodyCollider.size.y*0.5f + bodyCollider.offset.y);	// Collider Bottom Right

//		playerBodyColliderBottomLeftPos = new Vector2(transform.position.x - bodyCollider.size.x*0.5f + bodyCollider.center.x,
//		                                              transform.position.y - bodyCollider.size.y*0.5f + bodyCollider.center.y);	// Collider Bottom Left

//		playerGroundStopperColliderBottomLeftPos = new Vector2(transform.position.x - groundStopper.size.x*0.5f + groundStopper.center.x,
//		                                              transform.position.y - groundStopper.size.y*0.5f + groundStopper.center.y);	// Collider Bottom Left

		platformColliderFinderTopLeftPos = playerBodyColliderTopLeftPos + new Vector2(-1,1);		// höhe (nach oben) und breite (nach links) verschieben
		platformColliderFinderBottomRightPos = playerBodyColliderBottomRightPos + new Vector2(1,-1);				// breite (nach rechts) verschieben


	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(myPlatformCharacter.isDead)
		{
			//if headjumped
			//	break;

			//if powerUpKill
			//	ignor collisions	<-- calculation work
			//	override: grounded = false	<-- easy
			return;
		}
		JumpAblePlatformV4();
	}

	Collider2D[] platformColliderIgnoringArray = new Collider2D[1];
	Collider2D[] platformColliderConsideringArray = new Collider2D[1];

	void JumpAblePlatformV4()
	{
		// Child ColliderFinder with 4 Childs and 2D BoxCollider's... no point calculation, just use 2d boxcollider position +- center.x/.y

		//Physics2D.OverlapArea
		//Physics2D.OverlapCircle
		//Physics2D.OverlapPoint
		//Physics2D.Raycast
		//Physics2D.BoxCast
		//Physics2D.CircleCast

		//Physics.Raycast
		//Physics.OverlapSphere
		//Physics.CheckCapsule
		//Physics.CheckSphere

		/**
		 * find Platform to deactivate
		 **/

		//Collider2D platformColliderIgnoring;
		platformColliderFinderTopLeftPos = transform.position + new Vector3(-1f,+1f,0f);
		platformColliderFinderBottomRightPos  = transform.position + new Vector3(+1f,-1f,0f);
		//platformColliderIgnoring = Physics2D.OverlapArea(platformColliderFinderTopLeftPos, platformColliderFinderBottomRightPos, jumpOnPlatform);
		platformColliderIgnoringArray[0] = null;
		Physics2D.OverlapAreaNonAlloc(platformColliderFinderTopLeftPos, platformColliderFinderBottomRightPos, platformColliderIgnoringArray, jumpOnPlatform);

		//if(platformColliderIgnoring != null)
		if(platformColliderIgnoringArray[0] != null)
		{
			Physics2D.IgnoreCollision(bodyCollider, platformColliderIgnoringArray[0], true);
			Physics2D.IgnoreCollision(groundStopper, platformColliderIgnoringArray[0], true);
			Debug.DrawLine (transform.position, platformColliderIgnoringArray[0].transform.position, Color.green, 1f);
		}

		Color color = Color.red;
		#if UNITY_EDITOR
		Debug.DrawLine(platformColliderFinderTopLeftPos,platformColliderFinderTopLeftPos + new Vector2(0f,-2f),color);
		Debug.DrawLine(platformColliderFinderTopLeftPos,platformColliderFinderTopLeftPos + new Vector2(2f,0f),color);
		Debug.DrawLine(platformColliderFinderBottomRightPos,platformColliderFinderBottomRightPos + new Vector2(0f,+2f),color);
		Debug.DrawLine(platformColliderFinderBottomRightPos,platformColliderFinderBottomRightPos + new Vector2(-2f,0f),color);
		#endif

		/**
		 * find Platform to activate
		 **/

		if(myPlatformCharacter.moveDirection.y >0)			// fix (directly activate collider will result in little beam by UnityPhysikEngine
			return;											// and save performance, checking and activating only if needed !!!

		//Collider2D platformColliderConsidering;
		platformColliderFinderTopLeftPos = transform.position + new Vector3(-bodyCollider.size.x*0.5f,-0.4f,0f);
		platformColliderFinderBottomRightPos  = transform.position + new Vector3(+bodyCollider.size.x*0.5f,-2f,0f);
		//platformColliderConsidering = Physics2D.OverlapArea(platformColliderFinderTopLeftPos, platformColliderFinderBottomRightPos, jumpOnPlatform);
		platformColliderConsideringArray[0] = null;
		Physics2D.OverlapAreaNonAlloc(platformColliderFinderTopLeftPos, platformColliderFinderBottomRightPos, platformColliderConsideringArray, jumpOnPlatform);
		//if(platformColliderConsidering != null)
		if(platformColliderConsideringArray[0] != null)
		{
			Physics2D.IgnoreCollision(bodyCollider, platformColliderConsideringArray[0], false);
			Physics2D.IgnoreCollision(groundStopper, platformColliderConsideringArray[0], false);
			Debug.DrawLine (transform.position, platformColliderConsideringArray[0].transform.position, Color.magenta, 1f);
		}
		color = Color.green;
		#if UNITY_EDITOR
		Debug.DrawLine(platformColliderFinderTopLeftPos,platformColliderFinderTopLeftPos + new Vector2(0f,-1.75f),color);
		Debug.DrawLine(platformColliderFinderTopLeftPos,platformColliderFinderTopLeftPos + new Vector2(bodyCollider.size.x,0f),color);
		Debug.DrawLine(platformColliderFinderBottomRightPos,platformColliderFinderBottomRightPos + new Vector2(0f,+1.75f),color);
		Debug.DrawLine(platformColliderFinderBottomRightPos,platformColliderFinderBottomRightPos + new Vector2(-bodyCollider.size.x,0f),color);
		#endif

		// DebugCode
//		if(platformColliderIgnoring != null &&
//		   platformColliderIgnoring == platformColliderConsidering)
//		{
//			Debug.Log(platformColliderConsidering.name + " wurde deaktiviert und sofort wieder aktiviert");
//			Debug.Log("Physics2D.GetIgnoreCollision() = " + Physics2D.GetIgnoreCollision(groundStopper,platformColliderIgnoring));
//		}
//		else
//		{
//			if(platformColliderIgnoring != null)
//			{
//				Debug.Log(platformColliderIgnoring.name + " wird ignoriert");
//				Debug.Log("Physics2D.GetIgnoreCollision() = " + Physics2D.GetIgnoreCollision(groundStopper,platformColliderIgnoring));
//			}
//			if(platformColliderConsidering != null)
//			{
//				Debug.Log(platformColliderConsidering.name + " wird als ground ebene verwendet");
//				Debug.Log("Physics2D.GetIgnoreCollision() = " + Physics2D.GetIgnoreCollision(groundStopper,platformColliderIgnoring));
//			}
//		}
	}


	void JumpAblePlatformV3()
	{
		// Netzwerk problem, gibt keine velocity mehr!

		/**
		 * OverlapArea
		 * __________
		 * |		|
		 * |		|
		 * | *....*	|		<-- Character Collider Top
		 * | |	  |	|
		 * | |    | |
		 * | *....*	|		<-- Character Collider Bottom
		 * |		|
		 * |________|
		 * 
		 * Sprungbewegung:
		 * rigidbody.velocity.y > 0 	--> ignore collision				
		 *
		 * Fallbewegung:
		 * rigidbody.velocity.y <= 0	--> collision on!
		 * 
		 **/
		
		// Overlap Area in Collision with JumpOnPlatform, disable in Unity

		// Problem: jumpSaveZone!!! Bewegung nach unten links an die kante des colliders -> stopping
		//			kann an kannte springen und hängen bleiben

		
		CalculateColliderEdges();
		#if UNITY_EDITOR
		Debug.DrawLine(platformColliderFinderTopLeftPos, platformColliderFinderBottomRightPos, Color.green);
		#endif
		
		Collider2D platformCollider = Physics2D.OverlapArea(platformColliderFinderTopLeftPos, platformColliderFinderBottomRightPos, jumpOnPlatform);

		bool ignore = false;

		if(GetComponent<Rigidbody2D>().velocity.y > 0)
			ignore = true;
		else
			ignore = false;

		if(platformCollider != null)
		{
			Physics2D.IgnoreCollision(bodyCollider, platformCollider, ignore);
			Physics2D.IgnoreCollision(groundStopper, platformCollider, ignore);
			//			Debug.LogWarning(platformColliderAbove.name + " found");
		}
	}

}
