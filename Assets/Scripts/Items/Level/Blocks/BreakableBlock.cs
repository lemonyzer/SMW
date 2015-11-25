using UnityEngine;
using System.Collections;

public class BreakableBlock : MonoBehaviour {
	
	public string targetTag = "Head";

	public bool breakTriggered = false;

	public AudioClip destroyBlockSound;
	public GameObject destroyedBlockPrefab;
	public float destroyedBlockPrefabStayTime = 5.0f;

//	private BoxCollider2D myTriggerZone;
	private GameObject myBlock;
	private BoxCollider2D myBlockCollider;

	NetworkView myNView;

	void Awake()
	{
		myNView = this.GetComponent<NetworkView>();

//		myTriggerZone = GetComponent<BoxCollider2D>();
		myBlock = this.gameObject;
		myBlockCollider = myBlock.GetComponent<BoxCollider2D>();
	}

	// Use this for initialization
	void Start () {
		
	}

	void PreInit () {
		myBlockCollider = this.gameObject.AddComponent<BoxCollider2D>();
		this.gameObject.layer = LayerMask.NameToLayer (Layer.blockLayerName);
	}

	public void CreateBlock () {
		PreInit ();
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(Network.isServer || Network.peerType == NetworkPeerType.Disconnected)
		{
			if(breakTriggered)
				return;

			Debug.Log("OnTriggerEnter2D: " + other.name);
			if(other.gameObject.layer == Layer.head)
			{
				// Abfrage ob Trigger/Collision am unteren Rand des Blocks
				Debug.Log("Parent: " + other.gameObject.transform.parent.name);
				if(HeadTriggerUnderBlock(other))
				{

					// velocity funktioniert nicht, da Player Collider mit BlockCollider collidiert und velocity = 0 setzt!
//					if(other.gameObject.transform.parent.GetComponent<Rigidbody2D>().velocity.y >= 0f)			// nur zerstören wenn Spieler nach oben springt
//					{
//						other.gameObject.transform.parent.rigidbody2D.velocity = Vector2.zero;	// collision simulieren (player stoppt bei trigger erkennung kurz)
						breakTriggered = true;	
						if (Network.peerType == NetworkPeerType.Disconnected)
							BreakEffekt ();
						else
							myNView.RPC("Breaking", RPCMode.AllBuffered, (float)Network.time);				// all buffered, level is changeing!!
//					}
//					else
//					{
//						Debug.LogError(this.ToString() + ": nicht gesprungen!");
//					}
				}
			}
		}
	}

	bool HeadTriggerUnderBlock(Collider2D other)
	{
		//BoxCollider2D headCollider = other.gameObject.GetComponent<BoxCollider2D>();
		float blockBottomPos = this.transform.position.y;
//		if (this.transform.position.y < 0)
//			blockBottomPos = this.transform.position.y + this.transform.localScale.y*0.5f;
//		else
//			blockBottomPos = this.transform.position.y - this.transform.localScale.y*0.5f;
		float headTriggerUpEdgePos = other.transform.position.y + ((BoxCollider2D)other).size.y*0.5f;
		
						Debug.Log("Block bottom Position: " + blockBottomPos);
						Debug.Log("Head Trigger UpEdge Position: " + headTriggerUpEdgePos);
		
		// other shit (not needed)
		//				Debug.Log("((BoxCollider2D)other).size.y: " + ((BoxCollider2D)other).size);
		//				Debug.Log("other.bounds: " + other.bounds);
		
		float diff = blockBottomPos - headTriggerUpEdgePos;
		Debug.LogWarning("Difference: " + diff);
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

	void BreakEffekt()
	{

		if(destroyBlockSound != null)
			AudioSource.PlayClipAtPoint(destroyBlockSound,transform.position,1);
		else
			Debug.LogError("no Destroy Sound set in Unity Inspector!");

		Vector3 offset = new Vector3(0f,0f,0f);
		GameObject cloneTopLeft = (GameObject)Instantiate(destroyedBlockPrefab,transform.position+offset, Quaternion.identity);
		cloneTopLeft.GetComponent<Rigidbody2D>().AddForce(new Vector2(-250.0f,350.0f));
		cloneTopLeft.GetComponent<Rigidbody2D>().AddTorque(1000f);
		
		GameObject cloneTopRight = (GameObject)Instantiate(destroyedBlockPrefab,transform.position+offset, Quaternion.identity);
		cloneTopRight.GetComponent<Rigidbody2D>().AddForce(new Vector2(+250.0f,350.0f));
		cloneTopRight.GetComponent<Rigidbody2D>().AddTorque(-1000f);
		
		GameObject cloneBottomLeft = (GameObject)Instantiate(destroyedBlockPrefab,transform.position+offset, Quaternion.identity);
		cloneBottomLeft.GetComponent<Rigidbody2D>().AddForce(new Vector2(-150.0f,150.0f));
		cloneBottomLeft.GetComponent<Rigidbody2D>().AddTorque(1000f);
		
		GameObject cloneBottomRight = (GameObject)Instantiate(destroyedBlockPrefab,transform.position+offset, Quaternion.identity);
		cloneBottomRight.GetComponent<Rigidbody2D>().AddForce(new Vector2(+150.0f,150.0f));
		cloneBottomRight.GetComponent<Rigidbody2D>().AddTorque(-1000f);

		Destroy(cloneTopLeft,destroyedBlockPrefabStayTime);
		Destroy(cloneTopRight,destroyedBlockPrefabStayTime);
		Destroy(cloneBottomLeft,destroyedBlockPrefabStayTime);
		Destroy(cloneBottomRight,destroyedBlockPrefabStayTime);

		myBlock.GetComponent<Renderer>().enabled = false;
		myBlockCollider.enabled = false;
		//		myTriggerZone.enabled = false;
		this.enabled = false;
	}

	[RPC]
	void Breaking(float realTimeStamp, NetworkMessageInfo info)
	{
//		Debug.LogWarning("RPC Breaking Time.time = "+ Time.time +"\n" + 
//		                 "Networktime = " + Network.time + "\n" +
//		                 "RPC Timestamp = " + info.timestamp + "\n" +
//		                 "(float) Real Timestamp = " + realTimeStamp + "\n" +
//		                 "(float) Networktime = " + (float)Network.time);

		// Example
		// RPC was Buffered 23 seconds 
		/**
		 * RPC Breaking Time.time = 3.81071
		 * Networktime = 17037.659
		 * RPC Timestamp = 17037.274
		 * (float) Real Timestamp = 17014.73
		 * (float) Networktime = 17037.66
		 */

		//if(info.timestamp < Network.time - 3f)
		if ( realTimeStamp < (float)Network.time - 3f)
		{
			// skip Break animation 
			// TODO geht nicht, RPC bekommt aktuellen Timestamp und nicht Timestamp von früher
		}
		else
		{
			// Break animation
			BreakEffekt();

		}

		//Network.Destroy(this.gameObject);
		//GameObject.Destroy(this.gameObject);

		myBlock.GetComponent<Renderer>().enabled = false;
		myBlockCollider.enabled = false;
//		myTriggerZone.enabled = false;
		this.enabled = false;
	}

//	//Variante 0 
//	void OnSerializeNetworkView( BitStream stream )
//	{
//		//write position, direction, and speed to network
//		if( stream.isWriting )
//		{
//			//destroyPosition
//			Vector3 pos = transform.position;
//			bool destroy = this.destroyed;
//			stream.Serialize( ref pos );
//			stream.Serialize( ref destroy );
//		}
//		// read position, direction, and speed from network
//		else
//		{
//			//destroyPosition
//			Vector3 pos = Vector3.zero;
//			bool destroy = false;
//			stream.Serialize( ref pos );
//			stream.Serialize( ref destroy );
//			this.destroyed = destroy;
////			this.destroyPosition = pos;
//		}
//	}

//	IEnumerator ReloadPowerUpBlock()
//	{
//		yield return new WaitForSeconds(powerUpRespawnTime);
//		hasPowerUp=true;
//		AudioSource.PlayClipAtPoint(powerUpReloadedSound,transform.position,1);
//	}
}
