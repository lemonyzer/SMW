using UnityEngine;
using System.Collections;

public class Shoot : Power {

	double lastPowerTimeStamp = 0;
	double nextPowerTimeStamp = 0;
	double bulletToBulletTime = 0.5;
	float shootRate;
	float reloadTime;
	
	
	PlatformUserControl inputScript;
	PlatformCharacter characterScript;
	NetworkedPlayer networkedPlayer;
	NetworkView myNetworkView;
	
	// Use this for initialization
	void Start () {
		inputScript = this.GetComponent<PlatformUserControl> ();
		characterScript = this.GetComponent<PlatformCharacter> ();
		inputScript = this.GetComponent<PlatformUserControl> ();
		networkedPlayer = this.GetComponent<NetworkedPlayer> ();
		myNetworkView = this.GetComponent<NetworkView> ();
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
			Debug.LogWarning("wait... firerate = " + (1.0f/bulletToBulletTime) + " bullets/second");
		}
	}

	public override void gained (NetworkMessageInfo info)
	{
		characterScript.power1 = this;
	}
	
	public override void lost ()
	{
		characterScript.power1 = null;
	}

	//server seitig
	//TODO server checkt ob preccesspowerrequest innerhalb der firerate liegt (sonst cheaten durch ständiges senden von processpowerrequest möglich)
	public override void activate ()
	{
		if(Network.isServer)
		{
			characterScript.SpawnSingleBullet();
		}
    }
}
