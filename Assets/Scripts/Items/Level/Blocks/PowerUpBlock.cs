using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpBlock : MonoBehaviour {

//	public string targetTag = "Head";
	private float powerUpRespawnTime = 5f;
	public bool hasPowerUp = true;

	public AudioClip powerUpReleaseSound;
	public AudioClip powerUpReloadedSound;

//	public string powerUpsPath;
	public GameObject[] powerups;
//	public Object[] powerups2;
//	public List<GameObject> powerups3;
//	public List<Pref> powerups4;
//	public float powerUpStayTime = 8.0f;

	private Animator anim;
//	public Object animController;
	public RuntimeAnimatorController animController;

	private GameObject powerupClone;

	SpriteRenderer blockSpriteRenderer;
	float blockCenterPositionX;
	float blockCenterPositionY;
	float blockWidth;
	float blockHeight;

	// Use this for initialization
	void Awake() {
		anim = GetComponent<Animator>();

		LoadRandomPowerUp();
	}

	void LoadRandomPowerUp()
	{
		// resources durchsuchen, ein powerup wählen und laden
	}

	void Start() 
	{
//		anim.SetBool(hash.hasPowerUpBool,hasPowerUp);
		blockSpriteRenderer = this.GetComponent<SpriteRenderer>();
		blockCenterPositionX = blockSpriteRenderer.bounds.center.x;
		blockCenterPositionY = blockSpriteRenderer.bounds.center.y;
		blockWidth = blockSpriteRenderer.bounds.size.x;
		blockHeight = blockSpriteRenderer.bounds.size.y;
		anim.SetTrigger(HashID.powerUpBlockLoadedTrigger);
	}
	
//	// Update is called once per frame
//	void Update () {
//	
//	}

	public void CreateBlock (GameObject[] powerUps) {

		//init Animator & Animator Controller
		anim = this.gameObject.AddComponent<Animator> ();
		anim.runtimeAnimatorController = animController;

		// Renderer
//		blockSpriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
		blockSpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();

		// BoxCollider
		BoxCollider2D blockCollider = this.gameObject.AddComponent<BoxCollider2D> ();

		// Layer
		this.gameObject.layer = LayerMask.NameToLayer (Layer.blockLayerName);

		// PowerUps...
		//"Assets/Resources/Items/..."
		this.powerups = powerUps;
//		for(int i=0; i< powerUps.Length; i++)
//		{
//			if (powerUps[i] != null)
//			{
//
//			}
//		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(Network.isServer || Network.peerType == NetworkPeerType.Disconnected)
		{
			if(other.gameObject.layer == Layer.head)
			{
				if(hasPowerUp)
				{
					if(HeadTriggerUnderBlock(other))
					{
						//if(other.gameObject.transform.parent.rigidbody2D.velocity.y >= 0f)			// nur zerstören wenn Spieler nach oben springt
						//{
							if(hasPowerUp)
							{
								if(Network.peerType == NetworkPeerType.Disconnected)
								{
									ReleasingRandomPowerUp();
								}
								if(Network.isServer)
									GetComponent<NetworkView>().RPC("ReleasingRandomPowerUp", RPCMode.All);			// PowerUpBlock animation setzen, Item selbst wird über Network.instantiated
							}
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
				}
			}
		}
	}

	bool HeadTriggerUnderBlock(Collider2D other)
	{
		//BoxCollider2D headCollider = other.gameObject.GetComponent<BoxCollider2D>();
//		float blockBottomPos = this.transform.position.y - this.transform.localScale.y*0.5f;
		float blockBottomPos = this.transform.position.y;
		float headTriggerUpEdgePos = other.transform.position.y + ((BoxCollider2D)other).size.y*0.5f;// + ((BoxCollider2D)other).center.y;

#if UNITY_EDITOR
//		Debug.DrawLine(Vector3.zero, new Vector3(5,5,0), Color.red, 5f);
//		Debug.DrawLine(Vector3.zero, new Vector3(5,4.5f,0), Color.magenta, 5f);
//		Debug.DrawLine(Vector3.zero, new Vector3(5,5.5f,0), Color.blue, 5f);
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


	IEnumerator ReloadPowerUpBlock()
	{
		yield return new WaitForSeconds(powerUpRespawnTime);
		if(Network.peerType == NetworkPeerType.Disconnected)
		{
			BlockReloaded();
		}
		if(Network.isServer)
		{
			GetComponent<NetworkView>().RPC("BlockReloaded", RPCMode.All);
		}
	}

	[RPC]
	void ReleasingRandomPowerUp()
	{
		AudioSource.PlayClipAtPoint(powerUpReleaseSound,transform.position,1);
		hasPowerUp = false;
		anim.SetTrigger(HashID.powerUpBlockReleaseTrigger);
		//anim.SetBool(hash.hasPowerUpBool,hasPowerUp);
		StartCoroutine(ReloadPowerUpBlock());

		Vector3 offset = new Vector3(0,1,0);

		if(Network.peerType == NetworkPeerType.Disconnected)
		{
			// offline movement
			//Returns a random integer number between min [inclusive] and max [exclusive]
			int i = Random.Range(0, powerups.Length);
			powerupClone = (GameObject)Instantiate( powerups[i], transform.position + offset, Quaternion.identity );
			if(powerupClone.GetComponent<Rigidbody2D>() != null)
			{
				int direction = RandomSign();
				powerupClone.GetComponent<Rigidbody2D>().velocity = new Vector2(direction*10f,8f);
				//powerupClone.rigidbody2D.AddForce(new Vector2(-250.0f,350.0f));
			}
			else
			{
				
			}
			ItemScript itemScript = powerupClone.GetComponent<ItemScript>();
			if (itemScript != null)
			{
				if(itemScript.autoDestroy)
				{
					itemScript.StartDestroyTimer();
				}
			}
		}

		if(Network.isServer)
		{
			//Vector3 offset = new Vector3(.5f,.5f,0.0f);

			//static float Range(float min, float max); 
			//Returns a random integer number between min [inclusive] and max [exclusive]
			int i = Random.Range(0, powerups.Length);
			powerupClone = (GameObject)Network.Instantiate( powerups[i], transform.position + offset, Quaternion.identity,0 );
			//powerupClone = (GameObject)PhotonNetwork.Instantiate( powerups[i].name, transform.position + offset, Quaternion.identity,0 );
			//GameObject powerupClone = (GameObject)Instantiate(powerups[i],transform.position + offset ,Quaternion.identity);
			if(powerupClone.GetComponent<Rigidbody2D>() != null)
			{
				int direction = RandomSign();
				powerupClone.GetComponent<Rigidbody2D>().velocity = new Vector2(direction*10f,8f);
				//powerupClone.rigidbody2D.AddForce(new Vector2(-250.0f,350.0f));
			}
			else
			{
				
			}
			powerupClone.GetComponent<ItemScript>().StartDestroyTimer();
//			StartCoroutine(DestroyPowerUp());	// BAD Programming!! powerupClone loses referenz if new PowerUp Spawns...
//			Destroy(powerupClone,powerUpStayTime);
		}
	}

	int RandomSign()
	{
		return Random.value < .5? 1 : -1;
	}


//	IEnumerator DestroyPowerUp()
//	{
//		yield return new WaitForSeconds(powerUpStayTime);
//		if(powerupClone != null)											
//			PhotonNetwork.Destroy (powerupClone);					// BAD Programming!! powerupClone loses referenz if new PowerUp Spawns...
//	}

	[RPC]
	void BlockReloaded()
	{
		hasPowerUp=true;
		//anim.SetBool(hash.hasPowerUpBool,hasPowerUp);
		anim.SetTrigger(HashID.powerUpBlockLoadedTrigger);
		AudioSource.PlayClipAtPoint(powerUpReloadedSound,transform.position,1);
	}

}
