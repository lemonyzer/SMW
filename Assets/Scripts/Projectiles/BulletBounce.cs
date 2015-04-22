using UnityEngine;
using System.Collections;

public class BulletBounce : MonoBehaviour {

	Vector3 groundCheckPositionOffset;
	Vector3 groundCheckPosition;
	SpriteRenderer bulletSpriteRenderer;


	AuthoritativeFireBall fireBallScript;

	
	public Vector3 moveSpeed = new Vector3(5,5,0);
	public Vector3 moveDirection = new Vector3(1,0,0);


	// Use this for initialization
	void Start () {

		bulletSpriteRenderer = this.transform.parent.GetComponent<SpriteRenderer>();
		fireBallScript = this.transform.parent.GetComponent<AuthoritativeFireBall>();
		
		groundCheckPositionOffset = new Vector3(0,bulletSpriteRenderer.bounds.extents.y,0);

//		randomDirection = RandomSign();

//		moveDirection.x *= randomDirection;
	}

	int RandomSign()
	{
		return Random.value < .5? 1 : -1;
	}

	int randomDirection;

	
//	// Update is called once per frame
//	void Update () {
//		
//		if(Network.isServer || Network.peerType == NetworkPeerType.Disconnected)
//		{
//			#if UNITY_EDITOR
//			Debug.DrawLine(groundCheckPosition, groundCheckPosition + new Vector3(1,0,0));
//			#endif
//			
//			groundCheckPosition = this.transform.position - groundCheckPositionOffset;		// sprite pivot need to be in center position
//			if(Physics2D.OverlapPoint(groundCheckPosition, layer.whatIsAllGround))
//			{
//				Debug.Log(this.ToString() +": bounce");
//				Bounce();
//			}
//		}
//	}

	public float fireBallRadius = 0.25f;

	void OnCollisionEnter2D(Collision2D collision)
	{
		if(Network.isServer || Network.peerType == NetworkPeerType.Disconnected)
		{
			if(collision.gameObject.layer == Layer.ground ||
			   collision.gameObject.layer == Layer.jumpAblePlatform ||
			   collision.gameObject.layer == Layer.block)
			{
	//			Debug.Log(this.ToString() +": UnityPhysics -> BOUNCE");

				#if UNITY_EDITOR
				Debug.DrawLine(Vector3.zero, collision.contacts[0].point, Color.white, 2f);
				Debug.DrawLine(Vector3.zero, this.transform.position, Color.red, 2f);
				#endif

				if( collision.contacts[0].point.x >= this.transform.position.x +fireBallRadius )
				{
//					Debug.DrawLine(this.transform.position + new Vector3(fireBallRadius,fireBallRadius,0), this.transform.position + new Vector3(fireBallRadius,-fireBallRadius,0), Color.blue, 2f);
//					Debug.DrawLine(this.transform.position + new Vector3(-fireBallRadius,fireBallRadius,0), this.transform.position + new Vector3(-fireBallRadius,-fireBallRadius,0), Color.blue, 2f);
					Debug.Log("Bullet hitted frontal " + collision.collider.name);
					fireBallScript.DestroyPowerUp();
				}
				else if( collision.contacts[0].point.x <= this.transform.position.x -fireBallRadius )
				{
//					Debug.DrawLine(this.transform.position + new Vector3(fireBallRadius,fireBallRadius,0), this.transform.position + new Vector3(fireBallRadius,-fireBallRadius,0), Color.yellow, 2f);
//					Debug.DrawLine(this.transform.position + new Vector3(-fireBallRadius,fireBallRadius,0), this.transform.position + new Vector3(-fireBallRadius,-fireBallRadius,0), Color.yellow, 2f);
					Debug.Log("Bullet hitted frontal " + collision.collider.name);
					fireBallScript.DestroyPowerUp();
				}

				if( collision.contacts[0].point.y < this.transform.position.y)
				{
					Bounce();
				}
			}
		}
	}

	void Bounce()
	{
		this.transform.parent.GetComponent<Rigidbody2D>().velocity = new Vector3(moveDirection.x * moveSpeed.x, moveSpeed.y,0);
	}
}
