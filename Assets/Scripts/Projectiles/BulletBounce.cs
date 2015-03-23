using UnityEngine;
using System.Collections;

public class BulletBounce : MonoBehaviour {

	GameObject gameController;
	Layer layer;
//	StatsManager statsManager;

	Vector3 groundCheckPositionOffset;
	Vector3 groundCheckPosition;
	SpriteRenderer bulletSpriteRenderer;
	
	public static Vector3 moveSpeed = new Vector3(5,5,0);
	public Vector3 moveDirection = new Vector3(1,0,0);
	// Use this for initialization
	void Start () {
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		layer = gameController.GetComponent<Layer>();
//		statsManager = gameController.GetComponent<StatsManager>();
		bulletSpriteRenderer = this.transform.parent.GetComponent<SpriteRenderer>();
		
		groundCheckPositionOffset = new Vector3(0,bulletSpriteRenderer.bounds.extents.y,0);

		randomDirection = RandomSign();

		moveDirection.x *= randomDirection;
	}

	int RandomSign()
	{
		return Random.value < .5? 1 : -1;
	}

	int randomDirection;

	
	// Update is called once per frame
	void Update () {
		
		if(Network.isServer || Network.peerType == NetworkPeerType.Disconnected)
		{
			Debug.DrawLine(groundCheckPosition, groundCheckPosition + new Vector3(1,0,0));
			
			groundCheckPosition = this.transform.position - groundCheckPositionOffset;		// sprite pivot need to be in center position
			if(Physics2D.OverlapPoint(groundCheckPosition, layer.whatIsAllGround))
			{
				Debug.Log(this.ToString() +": bounce");
				Bounce();
			}
		}
	}


	void OnCollisionEnter2D(Collision2D collision)
	{
		if(Network.isServer || Network.peerType == NetworkPeerType.Disconnected)
		{
			if(collision.gameObject.layer == layer.ground ||
			   collision.gameObject.layer == layer.jumpAblePlatform ||
			   collision.gameObject.layer == layer.block)
			{
	//			Debug.Log(this.ToString() +": UnityPhysics -> BOUNCE");
				Bounce();
			}
		}
	}

	void Bounce()
	{
		this.transform.parent.GetComponent<Rigidbody2D>().velocity = new Vector3(moveDirection.x * moveSpeed.x, moveSpeed.y,0);
	}
}
