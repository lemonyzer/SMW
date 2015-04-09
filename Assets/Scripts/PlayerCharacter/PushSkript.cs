using UnityEngine;
using System.Collections;

public class PushSkript : MonoBehaviour {

	Transform myCharacter;
	Rigidbody2D myRigidBody2D;
	Rigidbody2D otherRigidBody2D;
	PlatformCharacter myPlatformCharacter;
	PlatformCharacter otherPlatformCharacter;

	/** 
	 * Connection with GameController 
	 **/
//	GameObject gameController;
//	Layer layer;

	void Awake()
	{
//		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
//		layer = gameController.GetComponent<Layer>();
	}

	// Use this for initialization
	void Start () {
		myCharacter = this.gameObject.transform;

		myRigidBody2D = myCharacter.GetComponent<Rigidbody2D>();
		if(myRigidBody2D == null)
			Debug.LogError(myCharacter.name + " hat kein RigidBody2D");

		myPlatformCharacter = GetComponent<PlatformCharacter>();
		if(myPlatformCharacter == null)
			Debug.LogError(myCharacter.name + " hat kein PlatformCharacter");
	}


//	void OnCollisionStay2D(Collision2D collision)
//	{
//		if(!myPlatformCharacter.isInRageModus)
//		{
//			if((collision.gameObject.layer == layer.player1) || 
//			   (collision.gameObject.layer == layer.player2) ||
//			   (collision.gameObject.layer == layer.player3) || 
//			   (collision.gameObject.layer == layer.player4))
//			{
//				float relativeVelocity = Mathf.Abs(collision.relativeVelocity.x);
//				relativeVelocity = 10f;
//				if(myCharacter.position.x < collision.contacts[0].point.x)
//				{
//					myPlatformCharacter.pushForce = -relativeVelocity;				// Collision rechts, nach links pushen
//					myPlatformCharacter.isBouncing = true;
//				}
//				else if(myCharacter.position.x > collision.contacts[0].point.x)
//				{
//					myPlatformCharacter.pushForce = relativeVelocity;				// Collision links, nach rechts pushen
//					myPlatformCharacter.isBouncing = true;
//				}
//			}
//		}
//	}

	void OnCollisionEnter2D(Collision2D collision) 
	{

		return; //TODO//TODO//TODO//TODO//TODO//TODO//TODO//TODO//TODO//TODO//TODO//TODO//TODO//TODO//TODO//TODO//TODO//TODO


		if(Network.peerType != NetworkPeerType.Disconnected)
			return;

		if(!myPlatformCharacter.isInRageModus)
		{
			/***
			 * Compare layer > 10 & layer < 14 effektiver?
			 * mit layermask layer 11,12,13,14 und verknüpfen und vergleichen?
			 ***/
//			if((collision.gameObject.layer == layer.player1) || 
//			   (collision.gameObject.layer == layer.player2) ||
//			   (collision.gameObject.layer == layer.player3) || 
//			   (collision.gameObject.layer == layer.player4))
			if(collision.gameObject.layer == Layer.player)
			{
				Debug.Log(myCharacter.name + ": Collision's relative Velocity = " + collision.relativeVelocity);

				float relativeVelocity = Mathf.Abs(collision.relativeVelocity.x);

				#if UNITY_EDITOR
				Debug.DrawLine(myCharacter.position,
				               myCharacter.position + new Vector3(0f,0.5f,0f),
				               Color.blue,
				               2,
				               false);

				Debug.DrawLine(collision.contacts[0].point,		// Start
				               collision.contacts[1].point,		// End
				               Color.red,						// Color
				               2,								// Visible Time
				               false);							// depthTest
				#endif

	//			otherRigidBody2D = collision.rigidbody;		// zgriff auf Physikeigenschaften des Gegenspielers
	//			
	//			float myVelocityX = rigidbody2D.velocity.x;
	//			float myVelocityY = rigidbody2D.velocity.y;
	//			float otherVelocityX = otherRigidBody2D.velocity.x;
	//			float otherVelocityY = otherRigidBody2D.velocity.y;
	//			
	//			Debug.Log(myCharacter.name + " velocity.x= " + myRigidBody2D.velocity.x);
	//			Debug.Log(collision.gameObject.name + " velocity.x= " + collision.rigidbody.velocity.x);

				if(myCharacter.position.x < collision.contacts[0].point.x)
				{
					myPlatformCharacter.pushForce = -relativeVelocity;				// Collision rechts, nach links pushen
					myPlatformCharacter.isBouncing = true;
	/*
	 * 				// Collision rechts
					// KI
					if(isKI)
					{
						if(myPlatformCharacter.facingRight)
						{
							// Gesicht zeigt in Richtung der Collision
							//pushForce = -collision.relativeVelocity.x * 0.5;
						}
						else
						{
							// Rücken zeigt in Richtung der Collision
							//pushForce = -collision.relativeVelocity.x * 0.5;
						}
					}
					// Collision rechts
					// Player
					else
					{
						if(myPlatformCharacter.facingRight)
						{
							// Gesicht zeigt in Richtung der Collision
							//pushForce = -collision.relativeVelocity.x * 0.5;
						}
						else
						{
							// Rücken zeigt in Richtung der Collision
							//pushForce = -collision.relativeVelocity.x * 0.5;
						}
					}
	*/
				}
				else if(myCharacter.position.x > collision.contacts[0].point.x)
				{
					myPlatformCharacter.pushForce = relativeVelocity;				// Collision links, nach rechts pushen
					myPlatformCharacter.isBouncing = true;
	/*
					// Collision links
					// KI
					if(isKI)
					{
						if(myPlatformCharacter.facingRight)
						{
							// Gesicht zeigt in Richtung der Collision
							//pushForce = collision.relativeVelocity.x * 0.5;
						}
						else
						{
							// Rücken zeigt in Richtung der Collision
							//pushForce = collision.relativeVelocity.x * 0.5;
						}
					}
					// Collision links
					// Player
					else
					{
						if(myPlatformCharacter.facingRight)
						{
							// Gesicht zeigt in Richtung der Collision
							//pushForce = collision.relativeVelocity.x * 0.5;
						}
						else
						{
							// Rücken zeigt in Richtung der Collision
							//pushForce = collision.relativeVelocity.x * 0.5;
						}
					} 
	*/
				}



	//
	//			if(myRigidBody2D.velocity.x > 1f)
	//			{
	//				Debug.Log(myCharacter.name + " bewegte sich nach rechts und wird jetzt nach links gedrückt");
	//				// Spieler bewegt sich nach rechts
	//				// muss also nach links gestoßen werden
	//
	////				myRigidBody2D.velocity = new Vector2(-10.0f,0f);
	//				pushForce = -10f;
	//			}
	//			else if(myRigidBody2D.velocity.x < 1f)
	//			{
	//				Debug.Log(myCharacter.name + " bewegte sich nach links und wird jetzt nach rechts gedrückt");
	//				// Spieler bewegt sich nach links
	//				// muss also nach rechts gestoßen werden
	////				myRigidBody2D.velocity = new Vector2(10.0f,0f);
	//				pushForce = 10f;
	//			}
	//			else
	//			{
	//				// Spieler bewegt sich nicht auf x-Achse
	//				// Richtung wird von Gegenspieler vorgegeben
	//				if(otherVelocityX > 0f)
	//				{
	//					Debug.Log("Gegenspieler komm von rechts");
	//					// Gegenspieler bewegt sich nach rechts
	//					// Spieler wird in die gleiche Richtung gedrückt
	////					myRigidBody2D.velocity = new Vector2(10.0f,0f);
	//					pushForce = 10f;
	//				}
	//				else if(otherVelocityX < 0f)
	//				{
	//					Debug.Log("Gegenspieler komm von links");
	//					// Gegenspieler bewegt sich nach links
	//					// Spieler wird in die gleiche Richtung gedrückt
	////					myRigidBody2D.velocity = new Vector2(-10.0f,0f);
	//					pushForce = -10f;
	//				}
	//				else
	//				{
	//					// Gegenspieler bewegt sich nicht!
	//					// facingRight checken!
	//					if(!isKI)
	//					{
	//						if(otherPlatformCharacter.facingRight)
	//						{
	//							pushForce = 10f;
	//						}
	//						else
	//							pushForce = -10f;
	//					}
	//					else
	//					{
	//						if(otherPlatformCharacter.facingRight)
	//						{
	//							pushForce = 10f;
	//						}
	//						else
	//							pushForce = -10f;
	//					}
	//				}
	//			}
	//
	//			if(!isKI)
	//			{
	//				myPlatformCharacter.isBouncing = true;
	//				myPlatformCharacter.pushForce = pushForce;
	//			}
	//			else
	//			{
	//				myPlatformCharacter.isBouncing = true;
	//				myPlatformCharacter.pushForce = pushForce;
	//			}
	//
	////			try {
	////				otherPlatformCharacter = collision.transform.GetComponent<PlatformCharacter>();
	////			} catch(UnityException e) { }
	////
	////			try
	////			{
	////				otherPlatformCharacter = collision.transform.GetComponent<PlatformCharacter>();
	////			} catch(UnityException e) { }
			}
		}
	}
}
