using UnityEngine;
using System.Collections;

public class ConnectButtonScript : MonoBehaviour {

	public string hostip;
	public int hostport;
	public string guid;
	public bool useNat;
//	public bool useUid;

	ConnectToGame ctg;

	void Awake()
	{
		ctg = GameObject.Find("GUI").GetComponent<ConnectToGame>();
	}

	public void Connect_Button()
	{
		if(useNat)
		{
			Debug.Log("connected with NAT");
			ctg.ConnectWithGUID_Button(guid);
		}
		else
		{
			Debug.Log("connected without NAT, directly!");
			ctg.ConnectWithIp_Button(hostip, hostport);
		}
	}
}
