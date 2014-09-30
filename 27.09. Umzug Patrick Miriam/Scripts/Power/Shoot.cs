using UnityEngine;
using System.Collections;

public class Shoot : Power {

	float bulletToBulletTime;
	float shootRate;
	float reloadTime;


	PlatformUserControl inputScript;
	PlatformCharacter characterScript;
	NetworkView myNetworkView;

	// Use this for initialization
	void Start () {
		inputScript = this.GetComponent<PlatformUserControl> ();
		characterScript = this.GetComponent<PlatformCharacter> ();
		inputScript = this.GetComponent<PlatformUserControl> ();
	}
	
	// Update is called once per frame
	void Update () {
//		Debug.Log(this.ToString() + " " + this.gameObject.name);

	}

	void FixedUpdate()
	{
		if(inputScript.inputPower)
		{
			characterScript.PowerPredictedAnimation();	// instant reaktion, feels better!
			myNetworkView.RPC( "ProcessPowerRequest", RPCMode.Server );
		}
	}

	public override void gained (NetworkMessageInfo info)
	{

	}
	
	public override void lost ()
	{
		throw new System.NotImplementedException ();
	}
	
	public override void activate ()
	{
		throw new System.NotImplementedException ();
    }
}
