using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OnOffSwitchBlockScript : MonoBehaviour {

//	[SerializeField]
//	List<SwitchTargetBlockScript> targetBlocks;

	[SerializeField]
	SwitchConnection switchConnection;
	
	public MapBlock mapBlock;
//	public Sprite currentStateSprite;
	public Sprite onStateSprite;
	public Sprite offStateSprite;
	public SpriteRenderer blockSpriteRenderer;
	public BoxCollider2D myCollider;
	public bool on;

	public void PreInit ()
	{
//		if (targetBlocks == null)
//			targetBlocks = new List<SwitchTargetBlockScript> ();

		blockSpriteRenderer = this.gameObject.AddComponent <SpriteRenderer> ();
		myCollider = this.gameObject.AddComponent <BoxCollider2D> ();
		myCollider.offset += new Vector2 (+0.5f,+0.5f);
	}

//	public void AddBlock (SwitchTargetBlockScript block)
//	{
//		targetBlocks.Add (block);
//	}

	public void CreateBlock (MapBlock mapBlock, bool state, Sprite onSprite, Sprite offSprite, SwitchConnection switchConnection, string spriteLayer)
	{
		PreInit ();
		on = state;

		this.mapBlock = mapBlock;
//		myCollider.enabled = on;
		
		//		if (on)
		//		{
		//			myCollider.enabled = true;
		//		}
		//		else if (on)
		//		{
		//			myCollider.enabled = true;
		//		}
		onStateSprite = onSprite;
		offStateSprite = offSprite;
		if (on)
			blockSpriteRenderer.sprite = onSprite;
		else
			blockSpriteRenderer.sprite = offSprite;
		blockSpriteRenderer.sortingLayerName = spriteLayer;

		this.switchConnection = switchConnection;
		this.gameObject.layer = LayerMask.NameToLayer (Layer.blockLayerName);
	}
	
	public void Switch ()
	{
		Debug.Log (this.ToString () + " switching from " + on + " to " + !on );
		on = !on;
		if (on)
		{
			// was on, now off
			blockSpriteRenderer.sprite = onStateSprite;
		}
		else
		{
			// was off, now on
			blockSpriteRenderer.sprite = offStateSprite;
		}
	}

	// Use this for initialization
	void Start () {
	
	}

//	void TriggerSwitch ()
//	{
//		for (int i=0; i<targetBlocks.Count; i++)
//		{
//			if (targetBlocks[i] != null)
//			{
//				targetBlocks[i].Switch ();
//			}
//		}
//	}

	void TriggerSwitch ()
	{
		if (switchConnection != null)
		{
			switchConnection.Switch ();
		}
		else
			Debug.LogError (this.ToString() + " switchConnection not set!");
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
		float blockBottomPosY = this.transform.position.y - this.transform.localScale.y*0.5f + myCollider.offset.y;
		Vector3 blockBottomPos = this.transform.position;
		blockBottomPos.y = this.transform.position.y - this.transform.localScale.y*0.5f;
		blockBottomPos.y += myCollider.offset.y;
		float headTriggerUpEdgePosY = other.transform.position.y + ((BoxCollider2D)other).size.y*0.5f;// + ((BoxCollider2D)other).center.y;

		Vector3 headTriggerUpEdgePos = other.transform.position;
		headTriggerUpEdgePos.y += ((BoxCollider2D)other).size.y*0.5f;
		

		#if UNITY_EDITOR
		Debug.DrawLine(blockBottomPos + Vector3.left, blockBottomPos + Vector3.right, Color.red, 2f);
		Debug.DrawLine(headTriggerUpEdgePos + Vector3.left, headTriggerUpEdgePos + Vector3.right, Color.yellow, 2f);
//		Debug.DrawLine(Vector3.zero, new Vector3(5,5,0), Color.red, 5f);
//		Debug.DrawLine(Vector3.zero, new Vector3(5,4.5f,0), Color.magenta, 5f);
//		Debug.DrawLine(Vector3.zero, new Vector3(5,5.5f,0), Color.blue, 5f);
		#endif
		
		//				Debug.Log("Block bottom Position: " + blockBottomPosY);
		//				Debug.Log("Head Trigger UpEdge Position: " + headTriggerUpEdgePosYY);
		
		// other shit (not needed)
		//				Debug.Log("((BoxCollider2D)other).size.y: " + ((BoxCollider2D)other).size);
		//				Debug.Log("other.bounds: " + other.bounds);
		
		
		//		Debug.DrawLine(Vector3.zero, this.transform.position, Color.magenta, 5f);
		//		Debug.DrawLine(Vector3.zero, new Vector3(this.transform.position.x, blockBottomPosY,0), Color.red, 5f);
		
		//		Debug.DrawLine(Vector3.zero, blockSpriteRenderer.bounds.center, Color.blue, 5f);
		
		
		//		Debug.DrawLine(transform.position, (transform.position-blockSpriteRenderer.bounds.center)+blockSpriteRenderer.bounds.size, Color.yellow, 5f);
		
		//blockBottomPosY = this.transform.position.y - blockSpriteRenderer.bounds.extents.y * this.transform.localScale.y;
		#if UNITY_EDITOR
		Debug.Log("Renderer Bounds " + blockSpriteRenderer.bounds); 
		Debug.Log("BlockPosition " + this.transform.position); 
		Debug.Log("LocalScale " + this.transform.localScale); 
		#endif
		//blockBottomPosY = this.transform.position.y 
		float diff = blockBottomPosY - headTriggerUpEdgePosY;
		
		
		#if UNITY_EDITOR
		Debug.LogWarning("blockBottomPosY: " + blockBottomPosY);
		Debug.LogWarning("headTriggerUpEdgePosY: " + headTriggerUpEdgePosY);
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
