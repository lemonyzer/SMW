using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shoot : Power {

	bool limit = true;
	int currentLimitCount = 0;
	int limitNumber = 2;

	List<GameObject> myBullets = new List<GameObject>();

	Projectile currentProjectile;

	double lastPowerTimeStamp = 0;
	double nextPowerTimeStamp = 0;
	double bulletToBulletTime = 0.5;
	float shootRate;
	float reloadTime;
	
	
	PlatformUserControl inputScript;
	PlatformCharacter characterScript;
	NetworkedPlayer networkedPlayer;
	NetworkView myNetworkView;

	// OnInstantiate
	void Awake()
	{
		myBullets = new List<GameObject>();
		inputScript = this.GetComponent<PlatformUserControl> ();
		characterScript = this.GetComponent<PlatformCharacter> ();
		inputScript = this.GetComponent<PlatformUserControl> ();
		networkedPlayer = this.GetComponent<NetworkedPlayer> ();
		myNetworkView = this.GetComponent<NetworkView> ();
	}
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
//		Debug.Log(this.ToString() + " " + this.gameObject.name);

	}

	public void FixedUpdate()
	{
		// funktioniert nicht, da dieses Methode automatisch läuft wenn Script aktiv ist!
		//power.TriggeredUpdate() wird in NetworkedPlayer FixedUpdate über Polymorphie aufgerufen.
	}

	// local am client
	public override void TriggeredUpdate()
	{
		if(Network.time >= nextPowerTimeStamp)
		{
			if(inputScript.inputPower)
			{
				characterScript.PowerPredictedAnimation();	// instant reaktion, feels better!
				if(Network.isServer)
				{
					networkedPlayer.ProcessPowerRequest();
				}
				else
				{
					myNetworkView.RPC( "ProcessPowerRequest", RPCMode.Server );				// gemeinsame methode die bedingungen überprüft (firerate, ragemode,... ???)
																							// oder doch alles in power spezifischer klasse ???
				}
				lastPowerTimeStamp = Network.time;
				nextPowerTimeStamp = lastPowerTimeStamp + bulletToBulletTime;
			}
		}
		else
		{
//			Debug.Log("lastPowerTimeStamp " + lastPowerTimeStamp);
//			Debug.Log("nextPowerTimeStamp " + nextPowerTimeStamp);
//			Debug.Log("Network.time " + Network.time);
			#if UNITY_EDITOR
			Debug.Log("wait... firerate = " + (1.0f/bulletToBulletTime) + " bullets/second");
			#endif
		}
	}

	public override void gained (double collectedTimeStamp)
	{
		characterScript.SetPower(this);
		//characterScript.power1 = this;
	}
	
	public override void lost ()
	{
		//characterScript.RemovePower();
	}

	//server seitig
	//TODO server checkt ob preccesspowerrequest innerhalb der firerate liegt (sonst cheaten durch ständiges senden von processpowerrequest möglich)
	public override void activate ()
	{
		if(Network.isServer)
		{
			currentLimitCount = myBullets.Count;
			if(currentLimitCount < limitNumber)
			{
				currentLimitCount++;
				// spawn projectile
				// add to list
				// set Owner
				AddBullet(currentProjectile.Instantiate(this.gameObject));
			}
		}
    }

//	Vector3 bulletSpawnPositionOffset = new Vector3(0.75f,0,0);
//
//	public GameObject SpawnSingleBullet()
//	{
//		GameObject bulletPrefab = (GameObject) Resources.Load("PowerUps/"+"Bomb", typeof(GameObject));
//		if(bulletPrefab == null)
//		{
//			Debug.LogError("bulletPrefab coudn't be loaded!!!! check path / and name");
//			return null;
//			//			return;
//		}
//		GameObject bulletGameObject = null;
//		if(Network.isServer)
//		{
//			bulletGameObject = (GameObject) Network.Instantiate( bulletPrefab, new Vector3(this.transform.localScale.x * bulletSpawnPositionOffset.x,1* bulletSpawnPositionOffset.y,1* bulletSpawnPositionOffset.z) + this.transform.position, Quaternion.identity, 0);
//		}
//		else if(Network.peerType == NetworkPeerType.Disconnected)
//		{
//			bulletGameObject = (GameObject) Instantiate(bulletPrefab, new Vector3(this.transform.localScale.x * bulletSpawnPositionOffset.x,1* bulletSpawnPositionOffset.y,1* bulletSpawnPositionOffset.z) + this.transform.position, Quaternion.identity);
//		}
//		
//		bulletGameObject.GetComponent<AuthoritativeBomb>().ownerCharacter = this.gameObject;// important!!!
//		bulletGameObject.GetComponent<AuthoritativeBomb>().moveDirection = new Vector3(this.transform.localScale.x,0,0);
//		bulletGameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(this.transform.localScale.x * AuthoritativeBomb.moveSpeed.x,1 * AuthoritativeBomb.moveSpeed.y,1* AuthoritativeBomb.moveSpeed.z);
//		
//		return bulletGameObject;
//	}

	public void SetProjectile(Projectile projectile)
	{
		this.currentProjectile = projectile;
	}

	public void SetProjectileLimit(int projectileLimit)
	{
		this.limitNumber = projectileLimit;
	}

	public void SetBulletToBulletTime(double bTbT)
	{
		this.bulletToBulletTime = bTbT;
	}

	public void AddBullet(GameObject newBullet)
	{
		myBullets.Add(newBullet);
	}

	public void RemoveBullet(GameObject bullet)
	{
		myBullets.Remove(bullet);
	}
}
