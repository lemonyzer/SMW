using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OnOffSwitchBlockScript : MonoBehaviour {

	[SerializeField]
	List<SwitchableBlockScript> switchBlocks;
	
	public MapBlock mapBlock;
	public Sprite currentStateSprite;
	public Sprite defaultStateSprite;
	public Sprite switchStateSprite;
	public SpriteRenderer blockSpriteRenderer;
	public BoxCollider2D myCollider;
	public bool on;

	public void PreInit ()
	{
		if (switchBlocks == null)
			switchBlocks = new List<SwitchableBlockScript> ();

		blockSpriteRenderer = this.GetComponent<SpriteRenderer> ();
	}

	public void AddBlock (SwitchableBlockScript block)
	{
		switchBlocks.Add (block);
	}

	public void CreateBlock (bool state, Sprite defaultSprite, Sprite otherSprite)
	{
		PreInit ();
		on = state;
		
//		myCollider.enabled = on;
		
		//		if (on)
		//		{
		//			myCollider.enabled = true;
		//		}
		//		else if (on)
		//		{
		//			myCollider.enabled = true;
		//		}
		defaultStateSprite = defaultSprite;
		currentStateSprite = defaultSprite;
		switchStateSprite = otherSprite;
	}
	
	public void Switch ()
	{
		if (on)
		{
		}
		else
		{
		}
		on = !on;
	}

	// Use this for initialization
	void Start () {
	
	}

	void TriggerSwitch ()
	{
		for (int i=0; i<switchBlocks.Count; i++)
		{
			if (switchBlocks[i] != null)
			{
				switchBlocks[i].Switch ();
			}
		}
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if(Network.isServer || Network.peerType == NetworkPeerType.Disconnected)
		{
			if(other.gameObject.layer == Layer.head)
			{
				//				if(hasPowerUp)
				//				{
				if(HeadTriggerUnderBlock(other))
				{
					//if(other.gameObject.transform.parent.rigidbody2D.velocity.y >= 0f)			// nur zerstören wenn Spieler nach oben springt
					//{
					TriggerSwitch ();
					//						if(hasPowerUp)
					//						{
					//							if(Network.peerType == NetworkPeerType.Disconnected)
					//							{
					//								ReleasingRandomPowerUp();
					//							}
					//							if(Network.isServer)
					//								GetComponent<NetworkView>().RPC("ReleasingRandomPowerUp", RPCMode.All);			// PowerUpBlock animation setzen, Item selbst wird über Network.instantiated
					//						}
					//}
					//else
					//{
					//	Debug.LogError(this.ToString() + ": nicht gesprungen!");
					//}
				}
				else
				{
					Debug.Log("HeadTriggerUnderBlock() == false");
				}
				//				}
			}
		}
	}
	
	bool HeadTriggerUnderBlock(Collider2D other)
	{
		//BoxCollider2D headCollider = other.gameObject.GetComponent<BoxCollider2D>();
		float blockBottomPos = this.transform.position.y - this.transform.localScale.y*0.5f;
		float headTriggerUpEdgePos = other.transform.position.y + ((BoxCollider2D)other).size.y*0.5f;// + ((BoxCollider2D)other).center.y;
		
		#if UNITY_EDITOR
		Debug.DrawLine(Vector3.zero, new Vector3(5,5,0), Color.red, 5f);
		Debug.DrawLine(Vector3.zero, new Vector3(5,4.5f,0), Color.magenta, 5f);
		Debug.DrawLine(Vector3.zero, new Vector3(5,5.5f,0), Color.blue, 5f);
		#endif
		
		//				Debug.Log("Block bottom Position: " + blockBottomPos);
		//				Debug.Log("Head Trigger UpEdge Position: " + headTriggerUpEdgePos);
		
		// other shit (not needed)
		//				Debug.Log("((BoxCollider2D)other).size.y: " + ((BoxCollider2D)other).size);
		//				Debug.Log("other.bounds: " + other.bounds);
		
		
		//		Debug.DrawLine(Vector3.zero, this.transform.position, Color.magenta, 5f);
		//		Debug.DrawLine(Vector3.zero, new Vector3(this.transform.position.x, blockBottomPos,0), Color.red, 5f);
		
		//		Debug.DrawLine(Vector3.zero, blockSpriteRenderer.bounds.center, Color.blue, 5f);
		
		
		//		Debug.DrawLine(transform.position, (transform.position-blockSpriteRenderer.bounds.center)+blockSpriteRenderer.bounds.size, Color.yellow, 5f);
		
		//blockBottomPos = this.transform.position.y - blockSpriteRenderer.bounds.extents.y * this.transform.localScale.y;
		#if UNITY_EDITOR
		Debug.Log("Renderer Bounds " + blockSpriteRenderer.bounds); 
		Debug.Log("BlockPosition " + this.transform.position); 
		Debug.Log("LocalScale " + this.transform.localScale); 
		#endif
		//blockBottomPos = this.transform.position.y 
		float diff = blockBottomPos - headTriggerUpEdgePos;
		
		
		#if UNITY_EDITOR
		Debug.LogWarning("blockBottomPos: " + blockBottomPos);
		Debug.LogWarning("headTriggerUpEdgePos: " + headTriggerUpEdgePos);
		Debug.LogWarning("Difference: " + diff);
		#endif
		//				if(Mathf.Abs(diff) < 0.02)
		//					Debug.LogWarning("Head is under the PowerUpBlock; " + diff);
		//				else
		//					Debug.LogWarning("Head is NOT under the PowerUpBlock; " + diff);
		if(Mathf.Abs(diff) < PhysicTolerances.TriggerColliderDifference)				// Unity Physics ungenauigkeit/tolleranz
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
