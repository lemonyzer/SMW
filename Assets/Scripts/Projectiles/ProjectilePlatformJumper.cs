using UnityEngine;
using System.Collections;

public class ProjectilePlatformJumper : MonoBehaviour {

	BoxCollider2D bodyCollider;
	CircleCollider2D groundStopper;
	Rigidbody2D projectileRigidbody2D;
	
	LayerMask jumpOnPlatform;

	// Use this for initialization
	void Start () {
		projectileRigidbody2D = this.transform.parent.GetComponent<Rigidbody2D>();
		bodyCollider = this.GetComponent<BoxCollider2D>();
		groundStopper =  this.GetComponent<CircleCollider2D>();

		jumpOnPlatform = 1 << Layer.jumpAblePlatform;
	}

	void Update()
	{
		JumpAblePlatformV4();
	}

//	Vector2 playerBodyColliderTopLeftPos;
//	Vector2 playerBodyColliderBottomRightPos;
//
	Vector2 platformColliderFinderTopLeftPos;
	Vector2 platformColliderFinderBottomRightPos;
//
//	void CalculateColliderEdges()
//	{
//		playerBodyColliderTopLeftPos = new Vector2(transform.position.x - bodyCollider.size.x*0.5f + bodyCollider.offset.x,
//		                                           transform.position.y + bodyCollider.size.y*0.5f + bodyCollider.offset.y);	// Collider Top Left
//		
//		playerBodyColliderBottomRightPos = new Vector2(transform.position.x + bodyCollider.size.x*0.5f + bodyCollider.offset.x,
//		                                               transform.position.y - bodyCollider.size.y*0.5f + bodyCollider.offset.y);	// Collider Bottom Right
//		
//		//		playerBodyColliderBottomLeftPos = new Vector2(transform.position.x - bodyCollider.size.x*0.5f + bodyCollider.center.x,
//		//		                                              transform.position.y - bodyCollider.size.y*0.5f + bodyCollider.center.y);	// Collider Bottom Left
//		
//		//		playerGroundStopperColliderBottomLeftPos = new Vector2(transform.position.x - groundStopper.size.x*0.5f + groundStopper.center.x,
//		//		                                              transform.position.y - groundStopper.size.y*0.5f + groundStopper.center.y);	// Collider Bottom Left
//		
//		platformColliderFinderTopLeftPos = playerBodyColliderTopLeftPos + new Vector2(-1,1);		// höhe (nach oben) und breite (nach links) verschieben
//		platformColliderFinderBottomRightPos = playerBodyColliderBottomRightPos + new Vector2(1,-1);				// breite (nach rechts) verschieben
//		
//		
//	}


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
			Debug.Log(this.transform.parent.name + " and " + platformColliderIgnoringArray[0].name + " disabled collision");
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
		
		if(projectileRigidbody2D.velocity.y >0)			// fix (directly activate collider will result in little beam by UnityPhysikEngine
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
}
